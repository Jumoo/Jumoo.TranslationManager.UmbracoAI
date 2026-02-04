# Umbraco AI Connector for Translation Manager

Connect your Translation Manager instance to Umbraco AI for seamless content translation powered by AI.

## Overview

The Umbraco AI Connector integrates Translation Manager with Umbraco's built-in AI functionality, allowing you to translate your content using the AI profiles configured in your Umbraco instance. This connector leverages the power of AI language models to provide fast, accurate translations directly within the Translation Manager workflow.

## Features

- **Native Umbraco AI Integration**: Uses Umbraco's AI Chat Service for translations
- **Profile Support**: Select from your configured AI profiles or use the default profile
- **Seamless Workflow**: Translate content without leaving the Umbraco interface
- **Multi-language Support**: Translate content into any language supported by your AI model
- **Local Translation Memory**: Use translation managers local memory to reduce repeat requests for content that has already been translated.

## Requirements

- Umbraco 17+ with Translation Manager installed
- Umbraco AI configured with at least one AI profile
- An active AI service provider (OpenAI, Azure OpenAI, etc.) configured in Umbraco AI

## Installation

Install the package via NuGet

```
dotnet add package Jumoo.TranslationManager.UmbracoAi
```

The connector will automatically register itself with Translation Manager upon installation.

## Configuration

1. Ensure Umbraco AI is properly configured in your Umbraco instance
2. Navigate to Translation Manager settings
3. Select the "AI Connector"
4. Select "Umbraco AI Connector" as your translator
5. (Optional) Specify an AI profile ID to use a specific profile, or leave blank to use the default profile

## Usage

Once configured, the Umbraco AI Connector will be available as a translation option in Translation Manager. Simply select your content, choose the target language, and let AI handle the translation.

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/Jumoo/Jumoo.TranslationManager.UmbracoAI).

## License

This package is licensed under the MIT License.
