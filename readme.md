# Umbraco AI Connector for Translation Manager

A connector for [Jumoo Translation Manager](https://jumoo.co.uk/translation-manager/) that integrates with Umbraco.AI library, allowing you to use Umbraco's centralized AI configuration to power your translations.

## Overview

This connector enables Translation Manager to leverage Umbraco.AI's profile-based AI configuration system. Instead of configuring AI providers separately for translations, you can use the AI profiles already configured in your Umbraco backoffice.

## Features

- ✨ Seamless integration with Umbraco.AI Core
- 🔧 Use Umbraco's AI profile management for translations
- 🌍 Support for all AI providers configured in Umbraco
- 📊 Token usage tracking and reporting
- 🎯 Profile-specific or default AI configuration

## Requirements

- Umbraco CMS v17+
- Jumoo Translation Manager v17.2.1+
- Umbraco.AI.Core v1.0.0+
- .NET 10.0

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package Jumoo.TranslationManager.UmbracoAi
```

Or via Package Manager Console:

```bash
Install-Package Jumoo.TranslationManager.UmbracoAi
```

## Configuration

### 1. Configure Umbraco.AI

First, ensure you have Umbraco.AI configured with at least one AI profile in your Umbraco backoffice. This is typically done through the Settings section.

### 2. Use in Translation Manager

When creating translation jobs in Translation Manager:

1. Select "Umbraco AI Connector" as your AI provider
2. (Optional) Specify a profile ID using the `umbai-profileId` option
3. If no profile is specified, the default Umbraco.AI profile will be used

### Profile Configuration

To use a specific AI profile, add the profile ID as an additional option:

```json
{
  "umbai-profileId": "your-profile-guid-here"
}
```

## How It Works

The connector acts as a bridge between Translation Manager and Umbraco.AI:

1. Translation Manager sends text to be translated
2. The connector formats the request using Translation Manager's AI prompts
3. The request is sent to Umbraco.AI's chat service
4. Umbraco.AI uses the configured profile (or default) to process the request
5. The translated text and token usage statistics are returned to Translation Manager

## Development

### Building the Client Assets

The connector includes TypeScript client components that need to be built:

#### Requirements

- Node LTS Version 20.17.0+
- Use a tool like NVM (Node Version Manager) to manage Node versions

#### Build Steps

```bash
cd src/Jumoo.TranslationManager.UmbracoAi/Client
npm install
npm run build
```

The build output is copied to `wwwroot/App_Plugins/JumooTranslationManagerUmbracoAi/`

#### File Watching (Development)

For active development:

```bash
cd src/Jumoo.TranslationManager.UmbracoAi/Client
npm run watch
```

This will monitor changes to TypeScript files and rebuild automatically.

### Project Structure

```
src/Jumoo.TranslationManager.UmbracoAi/
├── Constants.cs                          # Connector constants
├── UmbracoAiTranslator.cs               # Main translator implementation
├── Startup/
│   └── TranslationUmbracoAiComposer.cs  # Umbraco composer
├── Client/                               # TypeScript client assets
│   ├── src/
│   ├── package.json
│   ├── tsconfig.json
│   └── vite.config.ts
└── wwwroot/
    └── App_Plugins/                      # Built client assets
```

## Demo

A demo Umbraco site is included in the `demo/AIConnector.Site/` folder to showcase the connector in action.

## Resources

- [Umbraco Documentation](https://docs.umbraco.com/umbraco-cms/customizing/overview)
- [Translation Manager Documentation](https://jumoo.co.uk/translation-manager/)
- [Node Version Manager (Windows)](https://github.com/coreybutler/nvm-windows)
- [Node Version Manager (Unix)](https://github.com/nvm-sh/nvm)

## License

Please refer to the license file included in this repository.

## Support

For issues, questions, or contributions, please visit the project repository.
