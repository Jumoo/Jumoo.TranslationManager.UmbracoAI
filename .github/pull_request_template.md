## What does this change?

<!-- A sentence or two on the change and why it is needed. -->

## Notes for the reviewer

<!-- Anything non-obvious: behaviour changes, things you decided against, areas you want a
     second opinion on. Delete if there is nothing to say. -->

## Checklist

- [ ] `dotnet build src/Jumoo.TranslationManager.UmbracoAi/Jumoo.TranslationManager.UmbracoAi.csproj -c Release` is clean
- [ ] `npm run build` passes in `src/Jumoo.TranslationManager.UmbracoAi/Client`
- [ ] If the `Jumoo.TranslationManager.AI` package version changed, `UmbracoAiTranslator`
      still matches the current `AITranslatorBase` constructor and interfaces
- [ ] `CHANGELOG.md` updated under **Unreleased**
- [ ] If a dependency changed, `dotnet restore --force-evaluate` was run and the updated
      `packages.lock.json` is committed
- [ ] Backoffice changes were clicked through in a running site, not just built
