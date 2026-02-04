const localizationManifests: UmbExtensionManifest[] = [
  {
    type: "localization",
    alias: "jumoo.ai.umbai.localization.en",
    weight: -100,
    name: "English",
    meta: {
      culture: "en",
    },
    js: () => import("./en.js"),
  },
];

export const manifests: UmbExtensionManifest[] = [...localizationManifests];
