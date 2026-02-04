using Jumoo.TranslationManager.AI;
using Jumoo.TranslationManager.AI.Models;
using Jumoo.TranslationManager.AI.Translators;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

using Umbraco.AI.Core.Chat;

namespace Jumoo.TranslationManager.UmbracoAi;

public class UmbracoAiTranslator : AITranslatorBase, IAITranslator
{
    public string Name => Constants.ConnectorName;
    public override string Alias => Constants.ConnectorAlias;

    private readonly IAIChatService _chatService;

    public UmbracoAiTranslator(ILogger<AITranslatorBase> logger, IAIChatService chatService) : base(logger)
    {
        _chatService = chatService;
    }

    public Task Initialize(AITranslatorRequestOptions options)
        => Task.CompletedTask;

    public async Task<AITranslationValueResult<List<string>>> TranslateText(IEnumerable<string> text, AITranslatorRequestOptions options)
    {
        var messages = GetBasePrompts(text, options);
        var chatOptions = GetBaseChatOptions(options);
        var profile = options.Options.GetAdditionalOption<string?>("umbai-profileId", "");

        ChatResponse? response;
        if (string.IsNullOrEmpty(profile) is false && Guid.TryParse(profile, out var profileId) is true)
        {
            // use the specified profile
            response = await _chatService.GetChatResponseAsync(profileId, messages);
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
}
