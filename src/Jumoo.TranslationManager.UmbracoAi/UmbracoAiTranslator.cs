using Jumoo.TranslationManager.AI;
using Jumoo.TranslationManager.AI.Models;
using Jumoo.TranslationManager.AI.Services;
using Jumoo.TranslationManager.AI.Translators;
using Jumoo.TranslationManager.AI.Translators.Models;
using Jumoo.TranslationManager.Core.Providers;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using System.Runtime.CompilerServices;

using Umbraco.AI.Core.Chat;

namespace Jumoo.TranslationManager.UmbracoAi;

public class UmbracoAiTranslator : AITranslatorBase, IAITranslator, ITranslationProvider
{
    public override string Name => Constants.ConnectorName;
    public override string Alias => Constants.ConnectorAlias;
    public override Guid Key => new("a62573ad-c99b-47e4-8b5a-0087a4fa510f");

    private readonly IAIChatService _chatService;

    public UmbracoAiTranslator(
        AITranslationService translationService,
        AIConfigService aIConfigService,
        IAIChatService chatService,
        ILogger<AITranslatorBase> logger) : base(translationService, aIConfigService, logger)
    {
        _chatService = chatService;
    }

    public Task Initialize(AITranslatorRequestOptions options)
        => Task.CompletedTask;

    public override async Task<AITranslationValueResult<List<string>>> TranslateText(IEnumerable<string> text, AITranslatorRequestOptions options)
    {
        var messages = GetPrompts(text, options);
        var chatOptions = GetChatOptions(options.Options);
        var profile = options.Options.GetAdditionalOption<string?>("umbai-profileId", "");

        ChatResponse? response;
        if (string.IsNullOrEmpty(profile) is false && Guid.TryParse(profile, out var profileId) is true)
        {
            // use the specified profile
            // response = await _chatService.GetChatResponseAsync(profileId, messages, chatOptions);
            response = await _chatService.GetChatResponseAsync(chat =>
            {
                chat.WithAlias("jumoo-umbracoai-translator");
                chat.WithProfile(profileId);
            }, messages);
        }
        else
        {
            // assume the default profile (if its set)
            response = await _chatService.GetChatResponseAsync(messages);
        }
        

        return new AITranslationValueResult<List<string>>
        {
            Value = [.. response.Messages.Select(m => m.Text)],
            AIResult = new()
            {
                TokensUsed = response.Usage?.TotalTokenCount ?? 0,
                InputTokens = response.Usage?.InputTokenCount ?? 0,
                OutputTokens = response.Usage?.OutputTokenCount ?? 0
            }
        };
    }

    // model comes from the 
    public override string? GetChatModel(AIOptions options) => "UmbracoAIModel";
}
