
using Jumoo.TranslationManager.Core.Models;

using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Infrastructure.Manifest;

namespace Jumoo.TranslationManager.UmbracoAi.Startup;

public class TranslationUmbracoAiComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.Services.AddSingleton<IPackageManifestReader, UmbracoAiConnectorManifestReader>();
    }
}

internal class UmbracoAiConnectorManifestReader : IPackageManifestReader
{
    public Task<IEnumerable<PackageManifest>> ReadPackageManifestsAsync()
    {
        var bundle = new ConnectorPackageManifest
        {
            Name = Constants.ConnectorName,
            Alias = Constants.ConnectorAlias,
            Version = "1.0.0",
            EntryPointScript = WebPath.Combine(Constants.ConnectorPath, "umbraco-ai-connector.js")
        }.ToBundleManifest();

        return Task.FromResult<IEnumerable<PackageManifest>>([bundle]);
    }
}
