# Prompt Filtering with Presidio Sample

This sample demonstrates how to use Semantic Kernel with Microsoft Presidio to detect and anonymize PII (Personally Identifiable Information) in prompts before sending them to AI models. It showcases:

- Prompt filtering using middleware
- PII detection using Presidio Analyzer
- PII anonymization using Presidio Anonymizer
- Integration with OpenAI

## Prerequisites

- .NET 8.0+
- OpenAI API key
- Docker
- Presidio services (via Docker)

## Setup

1. Follow the instructions in the [README.md](../../README.md) file to set up the OpenAI API key.

2. Pull the required Presidio Docker images:
   ```powershell
   docker pull mcr.microsoft.com/presidio-analyzer
   docker pull mcr.microsoft.com/presidio-anonymizer
   ```

3. Start the Presidio services:
   ```powershell
   # Run Analyzer service
   docker run -d -p 5002:3000 mcr.microsoft.com/presidio-analyzer:latest

   # Run Anonymizer service
   docker run -d -p 5001:3000 mcr.microsoft.com/presidio-anonymizer:latest
   ```

## Testing the Services

The sample includes HTTP files to test the Presidio services directly:

- `Http/analyzer-analyze.http`: Test PII analysis
- `Http/analyzer-recognizers.http`: List available recognizers
- `Http/analyzer-supported-entities.http`: List supported entity types
- `Http/anonymizer-deanonymize.http`: Test deanonymization

You can use these files with REST Client in Visual Studio Code or other HTTP clients.

## Key Components

- `PromptAnalyzerFilter`: Detects PII in prompts and prevents sending sensitive data to LLMs
- `PromptAnonymizerFilter`: Anonymizes detected PII before sending to LLMs
- `PresidioTextAnalyzerService`: Wrapper for Presidio Analyzer API
- `PresidioTextAnonymizerService`: Wrapper for Presidio Anonymizer API

## Additional Resources

- [Presidio API Documentation](https://microsoft.github.io/presidio/api-docs/api-docs.html)
https://microsoft.github.io/presidio/api-docs/api-docs.html
