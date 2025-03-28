# Semantic Kernel Demonstrations

This repository contains a collection of Semantic Kernel demonstration projects. Semantic Kernel is a powerful framework for building AI applications using Large Language Models (LLMs) and other AI models. These demonstrations showcase various use cases and features of Semantic Kernel, providing insights into its capabilities and potential applications.

## Beginner Hands-On Semantic Kernel Demos

A collection of .NET samples demonstrating various Semantic Kernel features:

1. [**Kernel Prompting**](Beginner-Hands-On/Samples/KernelPrompting): Basic prompt handling and streaming
2. [**Kernel Dependency Injection**](Beginner-Hands-On/Samples/KernelDependencyInjection): Using DI with Semantic Kernel
3. [**Kernel Prompt Template**](Beginner-Hands-On/Samples/KernelPromptTemplate): Working with prompt templates and variables
4. [**Chat Service with Chat History**](Beginner-Hands-On/Samples/ChatServiceWithHistory): Managing conversation history
5. [**Prompt Execution Settings**](Beginner-Hands-On/Samples/UsingPromptSettings): Configuring prompt execution
6. [**Grounding Prompts with Plugins**](Beginner-Hands-On/Samples/GroudingPromptsWithPlugins): Using plugins for context
7. [**Plugin Function Calling**](Beginner-Hands-On/Samples/PluginFunctionCalling): Implementing and using plugins
8. [**Using OpenAPI Plugins**](Beginner-Hands-On/Samples/OpenAPIPlugins): Integrating external APIs via OpenAPI
9. [**Using Kernel Filters**](Beginner-Hands-On/Samples/UsingtKernelFilters): Implementing kernel filters
10. [**Aspire Dashboard + OpenTelemetry**](Beginner-Hands-On/Samples/AspireOpenTelemetry): Monitoring and telemetry
11. [**Multiple AI Models**](Beginner-Hands-On/Samples/KernelMultipleAIModels): Working with different AI providers
12. [**Multi-Modality**](Beginner-Hands-On/Samples/MultiModality): Handling text, images, and audio

## Applying Responsible/Secure AI Demos

A collection of .NET samples demonstrating how to implement responsible and secure AI practices using Semantic Kernel:

1. [**Prompt Filtering with Presidio**](Applying-Responsible-Secure-AI/Samples/PromptFilteringPresidio): 
   - PII detection and anonymization in prompts
   - Integration with Microsoft Presidio
   - Middleware filters for secure prompt handling

2. [**Azure AI Content Safety**](Applying-Responsible-Secure-AI/Samples/AzureAIContentSafety):
   - Content moderation using Azure AI services
   - Text moderation filters
   - Attack detection for prompts
   - Integration with Prompt Shields service

### Prerequisites

- .NET 8.0+
- OpenAI API key (for OpenAI-based samples)
- Ollama (for local AI model samples)
- Docker (for specific samples)
- ONNX runtime (for specific samples)

### Getting Started

1. Clone the repository
2. Set up your OpenAI API key:
   ```powershell
   cd Beginner-Hands-On/Samples/KernelPrompting
   dotnet user-secrets set "OpenAI:ApiKey" "YOUR_OPENAI_API_KEY"
   ```
3. Set up Ollama (choose one):
   - Docker:
     ```powershell
     docker run -d -e OLLAMA_KEEP_ALIVE=-1 -v C:\temp\ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
     ```
   - Windows: Download from [ollama.com/download](https://ollama.com/download)

4. Install required Ollama models:
   ```powershell
   ollama pull llama3.2
   ollama pull phi4
   ```

Each sample includes its own README with specific setup instructions and requirements.
