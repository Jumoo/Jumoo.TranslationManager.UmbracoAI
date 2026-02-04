export const manifests: Array<UmbExtensionManifest> = [
  {
    name: "Jumoo Translation Manager Umbraco Ai Entrypoint",
    alias: "Jumoo.TranslationManager.UmbracoAi.Entrypoint",
    type: "backofficeEntryPoint",
    js: () => import("./entrypoint.js"),
  },
];
