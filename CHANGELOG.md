# Changelog

Notable changes to `Jumoo.TranslationManager.UmbracoAi`. The connector ships one release line per
Umbraco major, so versions track the Umbraco major they target.

## Unreleased

### Added

- Repository standards: `LICENSE`, `README.md`, `CHANGELOG.md`, `SECURITY.md`,
  `CODE_OF_CONDUCT.md`, `.editorconfig`, `global.json`, `Directory.Build.props`,
  `GitVersion.yml`, dependabot, and issue and PR templates, matching the other
  Translation Manager connectors.
- CI workflows — PR build, package build, release, and CodeQL.
- Releases publish to NuGet from a `v{version}` tag pushed on a release branch, gated
  behind the `nuget` GitHub environment and authenticated with trusted publishing (OIDC)
  rather than a stored API key.

### Changed

- License switched from MIT to MPL-2.0, matching the rest of the Translation Manager
  connectors.
- Bumped `Jumoo.TranslationManager.AI` to 17.4.1 (from 17.3.3), and the referenced
  `Umbraco.Cms.*` packages to 17.5.0 to match its transitive requirement.
  `UmbracoAiTranslator`'s constructor still matches `AITranslatorBase`'s current (3-argument)
  constructor at this version — a `TranslationMemoryJobService` parameter exists only on an
  unreleased branch of the AI library, not yet in a published version, so no code change was
  needed here. Worth re-checking before the next bump.
- Bumped `Umbraco.Cms.*` further to 17.6.2 (latest 17.x) and `Umbraco.AI.Core` to 17.3.1
  (from 1.10.1 — the old `1.x` version scheme was superseded by one that tracks the Umbraco
  major it targets, and 17.3.1 is the latest under that scheme). Verified by booting the demo
  site: Umbraco, Umbraco.AI's migrations, and Translation Manager's migrations all completed,
  and the backoffice responded. `Jumoo.TranslationManager.AI` stays at 17.4.1 — its own latest
  17.x release, matching Umbraco 17.6.x on the Umbraco.Cms.* side.

## 1.0.0

- Umbraco 17 release, integrating Translation Manager with Umbraco.AI's profile-based
  chat service.
