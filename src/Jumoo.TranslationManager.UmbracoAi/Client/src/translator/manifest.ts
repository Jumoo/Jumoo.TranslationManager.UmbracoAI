import { ManifestAITranslatorConfig } from "@jumoo/translate-ai";

const connectorManifest: ManifestAITranslatorConfig = {
  type: "jumoo-tm-ai-translator",
  alias: "UmbracoAiConnector",
  name: "Umbraco AI Connector",
  js: () => import("./config-element"),
  elementName: "jumoo-tm-umbraco-ai-config",
};

export const manifests = [connectorManifest];
