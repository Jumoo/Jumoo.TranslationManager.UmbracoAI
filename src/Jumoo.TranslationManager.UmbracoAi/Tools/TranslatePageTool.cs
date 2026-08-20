using System.ComponentModel;

using Jumoo.TranslationManager.Core.Models;
using Jumoo.TranslationManager.Core.Providers;
using Jumoo.TranslationManager.Core.Services;

using Microsoft.AspNetCore.Http;

using Umbraco.AI.Core.Tools;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;

namespace Jumoo.TranslationManager.UmbracoAi.Tools;

public record TranslatePageArgs(
    [property: Description("The Umbraco content key (GUID) of the page to translate.")]
    Guid ContentKey,
    [property: Description("The target language to translate the page into, as the Umbraco culture code configured for it in Translation Manager, e.g. 'fr-FR'. " +
        "Leave this unset if the user didn't name a language (e.g. just 'translate this page') - the tool will use the page's only configured target language " +
        "automatically, or tell you what the options are so you can ask the user, if there's more than one.")]
    string? TargetCulture = null,
    [property: Description("When true, also translates every descendant page under this one (e.g. 'translate this page and its children'). When false (default), only this page is translated.")]
    bool IncludeChildren = false,
    [property: Description("Only set this to true if the user explicitly asks for the translation to be published/approved/made live (e.g. 'translate and publish it'). " +
        "Leave it false for a plain translation request (e.g. 'translate this page into French') - the job is created and translated but left for a translator to review " +
        "before it goes live.")]
    bool Publish = false
);

[AITool("translate_page", "Translate Page", ScopeId = Constants.ToolScopeId, IsDestructive = true)]
public class TranslatePageTool : AIToolBase<TranslatePageArgs>
{
    private readonly IContentService _contentService;
    private readonly IEntityService _entityService;
    private readonly TranslationSetService _setService;
    private readonly TranslationNodeService _nodeService;
    private readonly TranslationJobService _jobService;
    private readonly TranslationProviderCollection _providers;
    private readonly IBackOfficeSecurityAccessor _securityAccessor;
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TranslatePageTool(
        IContentService contentService,
        IEntityService entityService,
        TranslationSetService setService,
        TranslationNodeService nodeService,
        TranslationJobService jobService,
        TranslationProviderCollection providers,
        IBackOfficeSecurityAccessor securityAccessor,
        IHostingEnvironment hostingEnvironment,
        IHttpContextAccessor httpContextAccessor)
    {
        _contentService = contentService;
        _entityService = entityService;
        _setService = setService;
        _nodeService = nodeService;
        _jobService = jobService;
        _providers = providers;
        _securityAccessor = securityAccessor;
        _hostingEnvironment = hostingEnvironment;
        _httpContextAccessor = httpContextAccessor;
    }

    public override string Description =>
        "Translates an Umbraco page (optionally with all its descendant pages) into another language, via a " +
        "Translation Manager job that uses the Umbraco.AI connector. Requires the page to already be part of a " +
        "configured Translation Manager translation set. If no target language is given and the page has more " +
        "than one configured, the tool returns the available options in needsClarification/availableCultures " +
        "instead of creating anything - ask the user which one they want, then call the tool again with their " +
        "choice as targetCulture. By default the job is created and translated but left for a translator to " +
        "review; pass publish=true only when the user explicitly asks for the translation to be published. The " +
        "result's jobUrl is a link to the job in the backoffice - always share it with the user in your reply, " +
        "formatted as a clickable link, whether or not the job was published.";

    protected override async Task<object> ExecuteAsync(TranslatePageArgs args, CancellationToken cancellationToken = default)
    {
        var master = _contentService.GetById(args.ContentKey);
        if (master is null)
            return new { Error = $"Content with key '{args.ContentKey}' was not found." };

        var sets = (await _setService.GetSetsByPathAsync(master.Path)).ToList();
        var setSitePairs = sets
            .SelectMany(s => s.Sites.Select(site => (Set: s, Site: site)))
            .ToList();

        if (setSitePairs.Count == 0)
        {
            return new
            {
                Error = $"'{master.Name}' is not part of any configured Translation Manager translation set. " +
                    "A translator needs to configure that before it can be translated."
            };
        }

        (TranslationSet Set, TranslationSetSite Site) match;

        if (string.IsNullOrWhiteSpace(args.TargetCulture))
        {
            var distinctCultures = setSitePairs
                .GroupBy(p => p.Site.CultureName, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.First())
                .ToList();

            if (distinctCultures.Count > 1)
            {
                return new
                {
                    NeedsClarification = true,
                    AvailableCultures = distinctCultures.Select(p => new
                    {
                        Culture = p.Site.CultureName,
                        Name = p.Site.Culture?.DisplayName ?? p.Site.CultureName
                    }),
                    Message = $"'{master.Name}' has more than one target language configured: " +
                        string.Join(", ", distinctCultures.Select(p => p.Site.Culture?.DisplayName ?? p.Site.CultureName)) +
                        ". Which one would you like to translate it into?"
                };
            }

            match = distinctCultures[0];
        }
        else
        {
            var found = setSitePairs.FirstOrDefault(p =>
                p.Site.CultureName.Equals(args.TargetCulture, StringComparison.OrdinalIgnoreCase));

            if (found.Set is null)
            {
                var options = setSitePairs
                    .GroupBy(p => p.Site.CultureName, StringComparer.OrdinalIgnoreCase)
                    .Select(g => g.First().Site.Culture?.DisplayName ?? g.Key);

                return new
                {
                    Error = $"'{master.Name}' has no configured site for culture '{args.TargetCulture}'. " +
                        $"Available languages: {string.Join(", ", options)}"
                };
            }

            match = found;
        }

        var set = match.Set;
        var site = match.Site;
        var targetCulture = site.CultureName;

        var provider = _providers.GetProvider(Constants.ProviderKey);
        if (provider is null)
            return new { Error = "The Umbraco AI translation provider is not available." };

        var siteDetails = new[]
        {
            new SiteDetail { SetId = set.Id, SiteId = site.Id, CultureName = site.CultureName }
        };

        var nodeOptions = new NodeCreationOptions
        {
            ChangeType = TranslationChangeType.Force,
            IncludeNameChange = true,
            DefaultNodeStatus = NodeStatus.Open,
        };

        var contentItems = new List<IContent> { master };
        if (args.IncludeChildren)
        {
            var descendantKeys = _entityService.GetDescendants(master.Id).Select(x => x.Key);
            contentItems.AddRange(_contentService.GetByIds(descendantKeys));
        }

        var nodes = new List<TranslationNode>();
        foreach (var contentItem in contentItems)
        {
            nodes.AddRange(await _nodeService.CreateNodesAsync(contentItem, nodeOptions, siteDetails));
        }

        if (nodes.Count == 0)
        {
            return new
            {
                Error = $"No translation nodes could be created for '{master.Name}'" +
                    (args.IncludeChildren ? " or its children" : "") + $" in culture '{targetCulture}' " +
                    "- the content type(s) may be excluded from the translation set."
            };
        }

        var user = _securityAccessor.BackOfficeSecurity?.CurrentUser;

        var jobOptions = new JobOptions
        {
            AutoApprove = args.Publish,
        };

        var jobName = args.IncludeChildren
            ? $"{master.Name} & children - {site.Culture?.DisplayName ?? targetCulture}"
            : $"{master.Name} - {site.Culture?.DisplayName ?? targetCulture}";

        var job = await _jobService.CreateJobAsync(
            jobName,
            nodes,
            provider,
            providerOptions: null,
            user,
            jobOptions,
            groupId: Guid.NewGuid().ToString());

        if (job is null)
            return new { Error = "Failed to create the translation job." };

        job = await _jobService.LoadJobNodesAsync(job) ?? job;

        var submitAttempt = await _jobService.SubmitJob(job);
        if (submitAttempt.Success is false || submitAttempt.Result is null)
        {
            await _jobService.Cancel(job, deleteNodes: false);
            return new { Error = $"Failed to submit the translation job: {submitAttempt.Exception?.Message ?? "unknown error"}" };
        }

        // if Publish was requested, a notification handler will have already approved and
        // published the job synchronously as part of SubmitJob above - but it does so against
        // its own freshly-loaded copy of the job, so re-fetch to see the final status.
        var finalJob = await _jobService.GetAsync(job.Id) ?? submitAttempt.Result;

        var published = args.Publish && finalJob.Status >= JobStatus.Accepted;
        var jobPath = $"{_hostingEnvironment.GetBackOfficePath()}/section/translation/workspace/translation-job/edit/{finalJob.Key}";

        var request = _httpContextAccessor.HttpContext?.Request;
        var jobUrl = request is not null
            ? $"{request.Scheme}://{request.Host}{jobPath}"
            : jobPath;

        return new
        {
            JobKey = finalJob.Key,
            JobUrl = jobUrl,
            Status = finalJob.Status.ToString(),
            TargetCulture = targetCulture,
            PageCount = contentItems.Count,
            NodeCount = nodes.Count,
            Published = published,
            Message = published
                ? $"Translated and published {nodes.Count} page(s) into {targetCulture}. [View job]({jobUrl})"
                : $"Translated {nodes.Count} page(s) into {targetCulture}. The job is awaiting review before it can be published. [View job]({jobUrl})"
        };
    }
}
