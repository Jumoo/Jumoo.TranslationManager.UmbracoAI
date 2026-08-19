# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

`Jumoo.TranslationManager.UmbracoAi` bridges Translation Manager to
[Umbraco.AI](https://umbraco.ai/) — instead of the connector holding its own provider
credentials, it hands prompts to Umbraco.AI's `IAIChatService` and lets Umbraco's own
profile configuration (provider, model, API key) decide how the request is actually sent.
Like the other connectors it is a plugin — it does nothing without the Translation Manager
package (`Jumoo.TranslationManager.AI`), whose types come in via NuGet and are not in this
repo.

Two halves: the C# connector under `src/Jumoo.TranslationManager.UmbracoAi`, and a backoffice
client (TypeScript, Vite) under `src/Jumoo.TranslationManager.UmbracoAi/Client`.

## Commands

The client has to be built before the connector — it produces `wwwroot/App_Plugins`, which is
gitignored, so a fresh clone has no backoffice assets until you build it.

```bash
npm ci --prefix src/Jumoo.TranslationManager.UmbracoAi/Client
```

```bash
npm run build --prefix src/Jumoo.TranslationManager.UmbracoAi/Client
```

```bash
dotnet build src/Jumoo.TranslationManager.UmbracoAi/Jumoo.TranslationManager.UmbracoAi.csproj -c Release
```

There are no automated tests. Verification is: dotnet build clean, `npm run build`, and for
anything user facing, a click-through against a running backoffice (the `demo/` site).

## Repository shape

Issues are **not** tracked in this repo — they go to
[Jumoo.TranslationManager.Issues](https://github.com/Jumoo/Jumoo.TranslationManager.Issues),
which covers Translation Manager and all its connectors. A bare `#123` in a commit message
resolves against the wrong repo; use the full URL.

**`demo/` is not in the repository.** `.gitignore` excludes the whole folder. It exists locally
as a test Umbraco site. Build and pack the **project**, never the solution — CI does the same.

Adding `Directory.Build.props` here stops `D:\Source\directory.build.props` applying, which is
why `NuGetAuditMode` is repeated in it.

## The translator/AI-library coupling — the thing to get right

`UmbracoAiTranslator` derives from `AITranslatorBase` in the `Jumoo.TranslationManager.AI`
NuGet package. That base class's constructor signature is not stable across minor versions of
the package — it gained a `TranslationMemoryJobService` parameter partway through the 17.x
line (batch/translation-memory work). **Before bumping the `Jumoo.TranslationManager.AI`
package reference, check `AITranslatorBase`'s current constructor** in the installed package
and make sure `UmbracoAiTranslator`'s constructor and `base(...)` call still match — a version
bump alone can silently fail to compile, or (worse, if a matching overload still resolves)
silently drop a required dependency.

This connector implements only `IAITranslator` — it does not implement `IAIBatchTranslator` or
`IAIConfigTranslator`. That's a deliberate scope choice, not an oversight: batch submission and
provider-driven model lists both assume the connector talks to a specific AI provider directly,
whereas this connector always delegates to Umbraco.AI's chat service, which already owns model
and profile selection.

`UmbracoAiTranslator.GetChatModel` returns a fixed placeholder string rather than resolving a
model — because Umbraco.AI's profile (selected via `umbai-profileId`, or the default profile)
is what actually determines the model, not anything this connector can see. Don't try to make
this resolve a "real" model name without checking whether Umbraco.AI exposes one first.

## Things that will catch you out

**Version numbers** come from `Directory.Build.props` (`VersionPrefix`) and `GitVersion.yml`,
and CI stamps the built version via `dotnet pack /p:version=`. Don't hardcode versions in the
csproj.

**License.** This repo uses MPL-2.0, matching the other Translation Manager connectors — not
MIT, which is what the csproj said before this was brought in line with the others.
