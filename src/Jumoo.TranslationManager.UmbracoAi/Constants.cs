using System;
using System.Collections.Generic;
using System.Text;

namespace Jumoo.TranslationManager.UmbracoAi;

internal static class Constants
{
    public const string ConnectorName = "Umbraco AI Connector";
    public const string ConnectorAlias = "UmbracoAiConnector";

    public const string ConnectorPath = "/App_Plugins/Jumoo.TranslationManagerUmbracoAi";

    public static readonly Guid ProviderKey = new("a62573ad-c99b-47e4-8b5a-0087a4fa510f");

    public const string ToolScopeId = "translation-manager";
}
