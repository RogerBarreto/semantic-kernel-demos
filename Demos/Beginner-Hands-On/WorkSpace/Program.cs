using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.HuggingFace;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Data;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using Microsoft.SemanticKernel.Services;
using OllamaSharp;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Text;

List<Func<Task>> examples = [];
var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

#region Examples

examples.Add(async () => // Kernel prompting with custom settings
{
    Console.WriteLine("=== Kernel prompting with custom settings ===\n\n");

    var modelId = "llama3.2";
    var endpoint = new Uri("http://localhost:11434");

    var kernel = Kernel.CreateBuilder()
        .AddOllamaChatCompletion(modelId, endpoint)
        .Build();

    var settings = new OllamaPromptExecutionSettings
    {
        Temperature = 0.9f
    };

    await foreach (var token in kernel.InvokePromptStreamingAsync("Why does Jupiter has storms?", new(settings)))
    {
        Console.Write(token);
    };
});

examples.Add(async () => // Kernel prompting with Templated Prompt with Variables
{
    Console.WriteLine("=== Kernel prompting with Templated Prompt with Variables ===\n\n");

    var modelId = "llama3.2";
    var endpoint = new Uri("http://localhost:11434");
    var kernel = Kernel.CreateBuilder()
        .AddOllamaChatCompletion(modelId, endpoint)
        .Build();

    var settings = new OllamaPromptExecutionSettings
    {
        TopP = 0.9f,
        Temperature = 0.9f,
    };

    var myPrompt = "Hello, I'm {{$name}}, and I have {{$age}}. What is my name?";

    var arguments = new KernelArguments(settings)
    {
        ["name"] = "Roger",
        ["age"] = 30
    };

    await foreach (var token in kernel.InvokePromptStreamingAsync(myPrompt, arguments))
    {
        Console.Write(token);
    }
});

examples.Add(async () => // Kernel prompting with Templated Prompt with Plugin Functions
{
    Console.WriteLine("=== Kernel prompting with Templated Prompt with Plugin Functions ===\n\n");

    var modelId = "llama3.2";
    var endpoint = new Uri("http://localhost:11434");
    var kernel = Kernel.CreateBuilder()
        .AddOllamaChatCompletion(modelId, endpoint)
        .Build();

    string GetDateTime()
    {
        return DateTime.UtcNow.ToString("R");
    }

    var myFunction = KernelFunctionFactory.CreateFromMethod(GetDateTime, "GetDateTime");
    kernel.Plugins.AddFromFunctions("MyPlugin", [myFunction]);

    var settings = new OllamaPromptExecutionSettings
    {
        TopP = 0.9f,
        Temperature = 0.9f,
    };
    var myPrompt = "Hello, I'm {{$name}}, and current date and time now is {{GetDateTime}}. What day is today?";
    var arguments = new KernelArguments(settings)
    {
        ["name"] = "Roger"
    };

    await foreach (var token in kernel.InvokePromptStreamingAsync(myPrompt, arguments))
    {
        Console.Write(token);
    }
});

examples.Add(async () => // Kernel prompting with Templated Prompt with Stateless Plugin
{
    Console.WriteLine("=== Kernel prompting with Templated Prompt with Stateless Plugin ===\n\n");

    var modelId = "llama3.2";
    var endpoint = new Uri("http://localhost:11434");
    var kernelBuilder = Kernel.CreateBuilder()
        .AddOllamaChatCompletion(modelId, endpoint);

    // Plugins can be added also at the kernel configuration phase.
    kernelBuilder.Plugins.AddFromType<MyStatelessPlugin>();

    var kernel = kernelBuilder.Build();

    var settings = new OllamaPromptExecutionSettings
    {
        TopP = 0.9f,
        Temperature = 0.9f,
    };

    var myPrompt = "Hello, I'm {{$name}}, and current date and time now is {{GetDateTime}}. What day is today?";
    var arguments = new KernelArguments(settings)
    {
        ["name"] = "Roger"
    };

    await foreach (var token in kernel.InvokePromptStreamingAsync(myPrompt, arguments))
    {
        Console.Write(token);
    }
});

examples.Add(async () => // Kernel prompting with Templated Prompt with Stateful Plugins
{
    Console.WriteLine("=== Kernel prompting with Templated Prompt with Stateful Plugins ===\n\n");

    var myStatefulPlugin = new MyStatefulPlugin(counter: 15);
    var modelId = "llama3.2";
    var endpoint = new Uri("http://localhost:11434");
    var kernel = Kernel.CreateBuilder()
        .AddOllamaChatCompletion(modelId, endpoint)
        .Build();

    kernel.Plugins.AddFromObject(myStatefulPlugin);
    var myPrompt = "Hello, I'm {{$name}}, and current count now is {{GetCounter}}. Is the current count an even or an odd number?";
    var arguments = new KernelArguments
    {
        ["name"] = "Roger"
    };

    await foreach (var token in kernel.InvokePromptStreamingAsync(myPrompt, arguments))
    {
        Console.Write(token);
    }
    Console.WriteLine("\n----\n");
    await foreach (var token in kernel.InvokePromptStreamingAsync(myPrompt, arguments))
    {
        Console.Write(token);
    }
});

examples.Add(async () => // Kernel prompting with function calling and stateful Plugins
{
    Console.WriteLine("=== Kernel prompting with function calling and stateful Plugins ===\n\n");

    var myDescribedStatefulPlugin = new MyDescribedStatefulPlugin(isOn: true);
    var modelId = "llama3.2";
    var endpoint = new Uri("http://localhost:11434");
    var kernel = Kernel.CreateBuilder()
        .AddOllamaChatCompletion(modelId, endpoint)
        .Build();

    kernel.Plugins.AddFromObject(myDescribedStatefulPlugin);
    kernel.PromptRenderFilters.Add(new EchoPromptRenderFilter());

    var myPrompt = "Hello, I'm {{$name}}. Is the light or or off?";
    var settings = new OllamaPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
    var arguments = new KernelArguments(settings)
    {
        ["name"] = "Roger"
    };

    // Ollama only support function calling without streaming mode.
    Console.WriteLine("Processing...");
    var result = await kernel.InvokePromptAsync(myPrompt, arguments);
    Console.WriteLine(result);

    myPrompt = "Please turn the light off please?";
    Console.WriteLine("Processing..."); 
    result = await kernel.InvokePromptAsync(myPrompt, arguments);
    Console.WriteLine(result);
});

examples.Add(async () => // Kernel prompting with function calling and stateful Plugins (OpenAI)
{
    Console.WriteLine("=== Kernel prompting with function calling and stateful Plugins (OpenAI) ===\n\n");

    var myDescribedStatefulPlugin = new MyDescribedStatefulPlugin(isOn: true);
    var modelId = "gpt-4o-mini";
    var kernel = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(modelId, apiKey: config["OpenAI:ApiKey"]!)
        .Build();

    kernel.Plugins.AddFromObject(myDescribedStatefulPlugin);
    kernel.PromptRenderFilters.Add(new EchoPromptRenderFilter());

    var myPrompt = "Hello, I'm {{$name}}. Is the light or or off?";
    var settings = new OllamaPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
    var arguments = new KernelArguments(settings)
    {
        ["name"] = "Roger"
    };

    // Ollama only support function calling without streaming mode.
    Console.WriteLine("Processing...");
    var result = await kernel.InvokePromptAsync(myPrompt, arguments);
    Console.WriteLine(result);

    myPrompt = "Please turn the light off please?";
    Console.WriteLine("Processing...");
    result = await kernel.InvokePromptAsync(myPrompt, arguments);
    Console.WriteLine(result);
});

examples.Add(async () => // Get Service from Kernel prompting with function calling and stateful Plugins
{
    Console.WriteLine("=== Get Service from Kernel prompting with function calling and stateful Plugins ===\n\n");

    var myDescribedStatefulPlugin = new MyDescribedStatefulPlugin();
    var modelId = "gpt-4o-mini";
    var endpoint = new Uri("http://localhost:11434");
    var kernel = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion(modelId, apiKey: config["OpenAI:ApiKey"]!)
        .Build();

    kernel.Plugins.AddFromObject(myDescribedStatefulPlugin);
    kernel.PromptRenderFilters.Add(new EchoPromptRenderFilter());

    var service = kernel.GetRequiredService<IChatCompletionService>();

    var myPrompt = "Is the light or or off?";
    var settings = new OllamaPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

    ChatHistory chatHistory = [
        new ChatMessageContent(AuthorRole.User, myPrompt)
    ];

    // Ollama only support function calling without streaming mode.
    Console.WriteLine("Processing...");
    var result = await service.GetChatMessageContentAsync(chatHistory, settings, kernel);
    Console.WriteLine(result);

    Console.WriteLine("Processing...");
    chatHistory.AddUserMessage("Please turn the light on");
    result = await service.GetChatMessageContentAsync(chatHistory, settings, kernel);
    Console.WriteLine(result);
});

examples.Add(async () => // Kernel prompting with Open API Plugins
{
    var kernel = Kernel.CreateBuilder()
        .AddOpenAIChatCompletion("gpt-4o-mini", config["OpenAI:ApiKey"]!)
        .Build();
    var executionParameters = new OpenApiFunctionExecutionParameters();

    var openApiUri = new Uri("https://localhost:7299/swagger/v1/swagger.json");
    var plugin = await OpenApiKernelPluginFactory.CreateFromOpenApiAsync("WeatherService", openApiUri, executionParameters);
    kernel.Plugins.Add(plugin);
    kernel.Plugins.AddFromType<MyStatelessPlugin>(); // Enable AI to know which day is today

    // var listResult = await plugin["GetWeatherForecast"].InvokeAsync(kernel);

    var settings = new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
    var result = await kernel.InvokePromptAsync("What is the weather forecast for tomorrow? Ensure you check which day is today.", new(settings));

    Console.WriteLine(result);
});

examples.Add(async () => // Using Embeddings & Search Services 
{
    Console.WriteLine("=== Embeddings Service ===\n\n");

    var embeddingModelId = "mxbai-embed-large";
    var endpoint = new Uri("http://localhost:11434");
    var embeddingService = new EmbeddingGeneratorBuilder<string, Embedding<float>>()
                    .Use(new OllamaApiClient(endpoint, embeddingModelId))
                    .AsTextEmbeddingGenerationService();

    // Create a vector store and a collection to store information
    var vectorStore = new InMemoryVectorStore();
    var collection = vectorStore.GetCollection<string, SelectedModel>("ExampleCollection");
    await collection.CreateCollectionIfNotExistsAsync();

    // Save some information to the memory
    List<(string Id, string Detail)> factList = [
        ("phi3", "Scientific content"),
        ("llama3.2", "General chat content"),
        ("gpt-4o-mini", "Other subjects content")
    ];

    foreach (var (Id, Detail) in factList)
    {
        await collection.UpsertAsync(new()
        {
            ModelId = Guid.NewGuid().ToString(),
            PromptingDomain = Id,
            Embedding = await embeddingService.GenerateEmbeddingAsync(Detail)
        });
    }

    var vectorStoreTextSearch = new VectorStoreTextSearch<SelectedModel>(collection, embeddingService);

    while (true)
    {
        Console.Write($"\n\nUser > ");
        var userPrompt = Console.ReadLine();
        if (string.IsNullOrEmpty(userPrompt)) { break; }
        Console.WriteLine();
        var search = await vectorStoreTextSearch.SearchAsync(userPrompt, new TextSearchOptions { Top = 1 });
        await foreach (var item in search.Results)
        {
            Console.WriteLine($"Id: {item}");
        }
    }
});

examples.Add(async () => // Kernel simple chat model routing using model id
{
    Console.WriteLine("=== Kernel simple chat model routing using model id ===\n\n");

    var ollamaModelId = "llama3.2";
    var ollamaEndpoint = new Uri("http://localhost:11434");
    var fileModelId = "phi3";
    // https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx
    var fileModelPath = "E:\\repo\\huggingface\\Phi-3-mini-4k-instruct-onnx\\cpu_and_mobile\\cpu-int4-rtn-block-32";
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder
            .AddOllamaChatCompletion(ollamaModelId, ollamaEndpoint)
            .AddOnnxRuntimeGenAIChatCompletion(fileModelId, fileModelPath);
            // Last service will be the default service

    var kernel = kernelBuilder.Build();

    kernel.PromptRenderFilters.Add(new SelectedModelRenderFilter());

    var modelNames = string.Join(", ", kernel.GetAllServices<IChatCompletionService>().Select(s => s.GetModelId()));

    while (true)
    {
        Console.Write($"\n\nType one of the available models [{modelNames}] > ");
        var selectedModel = Console.ReadLine();
        Console.WriteLine();

        if (string.IsNullOrEmpty(selectedModel)) { break; }

        var settings = new PromptExecutionSettings { ModelId = selectedModel };
        await foreach (var token in kernel.InvokePromptStreamingAsync("Answer in a small sentence, why does Jupiter have storms?", new(settings)))
        {
            Console.Write(token);
        };
    }
});

examples.Add(async () => // Kernel simple chat model routing using service id
{
    Console.WriteLine("=== Kernel simple chat model routing using service id ===\n\n");

    var ollamaModelId = "llama3.2";
    var ollamaEndpoint = new Uri("http://localhost:11434");
    var fileModelId = "phi3";
    var fileModelPath = "E:\\repo\\huggingface\\Phi-3-mini-4k-instruct-onnx\\cpu_and_mobile\\cpu-int4-rtn-block-32";
    var kernelBuilder = Kernel.CreateBuilder();

    kernelBuilder
        .Services
            .AddOllamaChatCompletion(
                serviceId: "ollama",
                modelId: ollamaModelId,
                endpoint: ollamaEndpoint)
            .AddOnnxRuntimeGenAIChatCompletion(
                serviceId: "onnx", 
                modelId: fileModelId, 
                modelPath: fileModelPath);

    var kernel = kernelBuilder.Build();

    kernel.PromptRenderFilters.Add(new SelectedModelRenderFilter());

    while (true)
    {
        Console.Write($"\n\nType one of the available models > ");
        var selectedService = Console.ReadLine();
        Console.WriteLine();

        if (string.IsNullOrEmpty(selectedService)) { break; }

        var settings = new PromptExecutionSettings { ServiceId = selectedService };
        await foreach (var token in kernel.InvokePromptStreamingAsync("Answer in a small sentence, why does Jupiter have storms?", new(settings)))
        {
            Console.Write(token);
        };
    }
});

examples.Add(async () => // Kernel AI routing
{
    Console.WriteLine("=== Kernel AI routing ===\n\n");

    var ollamaModelId = "llama3.2";
    var ollamaEndpoint = new Uri("http://localhost:11434");
    var fileModelId = "phi3";
    var fileModelPath = "E:\\repo\\huggingface\\Phi-3-mini-4k-instruct-onnx\\cpu_and_mobile\\cpu-int4-rtn-block-32";
    var kernelBuilder = Kernel.CreateBuilder();
    var openAIModelId = "gpt-4o-mini";

    kernelBuilder
        .AddOllamaChatCompletion(ollamaModelId, ollamaEndpoint)
        .AddOnnxRuntimeGenAIChatCompletion(fileModelId, fileModelPath)
        .AddOpenAIChatCompletion(openAIModelId, config["OpenAI:ApiKey"]!);

    var kernel = kernelBuilder.Build();
    kernel.PromptRenderFilters.Add(new SelectedModelRenderFilter());

    // Last service will be the default service

    StringBuilder systemPrompt = new("""
            You are a model router and you will decide on only one of the 
            the models below that matches the capability more in line with the user's question. 
            Answer only with the Id of the model as the first word and a reason for it following.

            | Model Id | Capability |
            |----------|------------|
            """);

    // Router instructions
    List<(string, string)> modelCapabilities = [
        (ollamaModelId, "General chat"),
        (fileModelId, "Scientific chat"),
        (openAIModelId, "Other subjects")
    ];
    foreach (var (modelId, capability) in modelCapabilities)
    {
        systemPrompt.Append($"\n| {modelId} | {capability} |");
    }

    async Task<string> GetModelForPromptAsync(string prompt, string systemPrompt)
    {
        var routerService = kernel.GetAllServices<IChatCompletionService>()
            .OfType<OpenAIChatCompletionService>()
            .First();

        ChatHistory chatHistory = [
            new ChatMessageContent(AuthorRole.System, systemPrompt),
            new ChatMessageContent(AuthorRole.User, prompt)
        ];

        var selectionAndReason = (await routerService.GetChatMessageContentAsync(chatHistory)).ToString();
        var selectedModel = selectionAndReason[..selectionAndReason.IndexOf(' ')];

        Console.WriteLine($"\nSelected model by AI router: {selectionAndReason}\n");

        return selectedModel.ToString();
    }

    var modelNames = string.Join(", ", kernel.GetAllServices<IChatCompletionService>().Select(s => s.GetModelId()));

    while (true)
    {
        Console.Write($"\n\nUser > ");
        var userPrompt = Console.ReadLine();
        if (string.IsNullOrEmpty(userPrompt)) { break; }
        Console.WriteLine();

        var selectedModel = await GetModelForPromptAsync(userPrompt, systemPrompt.ToString());

        var settings = new PromptExecutionSettings { ModelId = selectedModel };
        await foreach (var token in kernel.InvokePromptStreamingAsync(userPrompt, new(settings)))
        {
            Console.Write(token);
        };
    }
});

examples.Add(async () => // Kernel Embeddings Routing
{
    Console.WriteLine("=== Kernel Embeddings Routing ===\n\n");

    var ollamaModelId = "llama3.2";
    var ollamaEndpoint = new Uri("http://localhost:11434");
    var fileModelId = "phi3";
    var fileModelPath = "E:\\repo\\huggingface\\Phi-3-mini-4k-instruct-onnx\\cpu_and_mobile\\cpu-int4-rtn-block-32";
    var kernelBuilder = Kernel.CreateBuilder();
    var openAIModelId = "gpt-4o-mini";

    kernelBuilder
        .AddOllamaChatCompletion(ollamaModelId, ollamaEndpoint)
        .AddOnnxRuntimeGenAIChatCompletion(fileModelId, fileModelPath)
        .AddOpenAIChatCompletion(openAIModelId, config["OpenAI:ApiKey"]!);
    // Last service will be the default service

    var embeddingModelId = "mxbai-embed-large";
    var endpoint = new Uri("http://localhost:11434");
    var embeddingService = new EmbeddingGeneratorBuilder<string, Embedding<float>>()
                    .Use(new OllamaApiClient(endpoint, embeddingModelId))
                    .AsTextEmbeddingGenerationService();
    
    // Create a vector store and a collection to store information
    var vectorStore = new InMemoryVectorStore();
    var collection = vectorStore.GetCollection<string, SelectedModel>("ExampleCollection");
    await collection.CreateCollectionIfNotExistsAsync();

    // Save some information to the memory
    List<(string Id, string Detail)> factList = [
        (fileModelId, "Scientific content"),
        (ollamaModelId, "General chat content"),
        (openAIModelId, "Other subjects content")
    ];

    foreach (var (Id, Detail) in factList)
    {
        await collection.UpsertAsync(new()
        {
            ModelId = Id,
            PromptingDomain = Detail,
            Embedding = await embeddingService.GenerateEmbeddingAsync(Detail)
        });
    }

    async Task<string> GetModelForPromptAsync(string prompt)
    {
        var searchVector = await embeddingService.GenerateEmbeddingAsync(prompt);
        var search = (await collection.VectorizedSearchAsync(searchVector, new() { Top = 1 }));
        string? selectedModel = null;
        string? selectedModelPromptingDomain = null;
        await foreach (var item in search.Results)
        {
            selectedModel = item.Record.ModelId;
            selectedModelPromptingDomain = item.Record.PromptingDomain;
        }
   
        Console.WriteLine($"\nSelected model by AI router: {selectedModel} {selectedModelPromptingDomain}\n");

        return selectedModel ?? "default";
    }

    var kernel = kernelBuilder.Build();

    kernel.PromptRenderFilters.Add(new SelectedModelRenderFilter());

    var modelNames = string.Join(", ", kernel.GetAllServices<IChatCompletionService>().Select(s => s.GetModelId()));

    while (true)
    {
        Console.Write($"\n\nUser > ");
        var userPrompt = Console.ReadLine();
        if (string.IsNullOrEmpty(userPrompt)) { break; }
        Console.WriteLine();

        var selectedModel = await GetModelForPromptAsync(userPrompt);

        var settings = new PromptExecutionSettings { ModelId = selectedModel };
        await foreach (var token in kernel.InvokePromptStreamingAsync(userPrompt, new(settings)))
        {
            Console.Write(token);
        };
    }
});

examples.Add(async () => // Open Telemetry Aspire Dashboard 
{
    Console.WriteLine("=== Open Telemetry Aspire Dashboard ===\n\n");

    // https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone?tabs=bash#start-the-dashboard
    // docker run --rm -it -d -p 18888:18888 -p 4317:18889 --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:9.0

    var builder = Kernel.CreateBuilder();

    var oTelExporterEndpoint = "http://localhost:4317";

    var resourceBuilder = ResourceBuilder
        .CreateDefault()
        .AddService("CZUpdate-SemanticKernel-Telemetry");

    // Enable model diagnostics with sensitive data.
    AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

    using var traceProvider = Sdk.CreateTracerProviderBuilder()
        .SetResourceBuilder(resourceBuilder)
        .AddSource("Microsoft.SemanticKernel*")
        .AddOtlpExporter(options => options.Endpoint = new Uri(oTelExporterEndpoint))
        .Build();

    using var meterProvider = Sdk.CreateMeterProviderBuilder()
        .SetResourceBuilder(resourceBuilder)
        .AddMeter("Microsoft.SemanticKernel*")
        .AddOtlpExporter(options => options.Endpoint = new Uri(oTelExporterEndpoint))
        .Build();

    using var loggerFactory = LoggerFactory.Create(builder =>
    {
        // Add OpenTelemetry as a logging provider
        builder.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resourceBuilder);
            options.AddOtlpExporter(options => options.Endpoint = new Uri(oTelExporterEndpoint));
            // Format log messages. This is default to false.
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
        });
        builder.SetMinimumLevel(LogLevel.Information);
    });

    builder.Services.AddSingleton(loggerFactory);
    builder.Plugins.AddFromObject(new MyDescribedStatefulPlugin());

    builder.AddOpenAIChatCompletion(
        modelId: "gpt-4o-mini",
        apiKey: config["OpenAI:ApiKey"]!);

    Kernel kernel = builder.Build();

    while (true)
    {
        Console.Write("User > ");
        var userPrompt = Console.ReadLine();
        if (string.IsNullOrEmpty(userPrompt)) { break; }

        Console.Write("\nAssistant > ");

        var settings = new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
        var result = await kernel.InvokePromptAsync(userPrompt, new(settings));
        Console.WriteLine(result);
    }
});

examples.Add(async () => // Multi modalities text -> audio services
{
    Console.WriteLine("=== Multi modalities text -> audio services ===\n\n");

    var chatService = new Azure.AI.Inference.ChatCompletionsClient(
        endpoint: new Uri(config["AzureAIInference:Endpoint"]!),
        credential: new Azure.AzureKeyCredential(config["AzureAIInference:ApiKey"]!))
        .AsChatClient("phi3")
        .AsChatCompletionService();

    var textToAudioService = new OpenAITextToAudioService(
        modelId: "tts-1", 
        apiKey: config["OpenAI:ApiKey"]!);

    StringBuilder answer = new();
    var prompt = "Explain in a simple phrase why Jupiter has storms.";
    Console.WriteLine($"{prompt}\nGenerating response...\n");
    await foreach (var token in chatService.GetStreamingChatMessageContentsAsync(prompt))
    {
        Console.Write(token);
        answer.Append(token);
    }

    Console.WriteLine("\nGenerating audio...\n");
    var generatedAudios = await textToAudioService.GetAudioContentsAsync(answer.ToString());
    var generatedAudio = generatedAudios[0];

    var file = Path.Combine(Directory.GetCurrentDirectory(), "output.mp3");
    await File.WriteAllBytesAsync(file, generatedAudio.Data!.Value);

    Console.WriteLine($"\nAudio Generated. Ctrl + Click to listen: {new Uri(file).AbsoluteUri}");
});

examples.Add(async () => // Multi modalities audio -> text -> image services
{
    Console.WriteLine("=== Multi modalities audio -> text -> image services ===\n\n");

    string folderPath = @"C:\Users\rbarreto\OneDrive - Microsoft\Documents\Sound Recordings";

    DirectoryInfo directoryInfo = new(folderPath);

    // Get the most recent file created in the directory
    FileInfo? mostRecentFile = directoryInfo.GetFiles()
                                           .OrderByDescending(f => f.CreationTime)
                                           .FirstOrDefault();

    Console.WriteLine($"Most recent recording: {mostRecentFile!.FullName}");

    var audioContent = new Microsoft.SemanticKernel.AudioContent(File.ReadAllBytes(mostRecentFile!.FullName), "audio/m4a");

    var audioToTextService = new OpenAIAudioToTextService(
        modelId: "whisper-1",
        apiKey: config["OpenAI:ApiKey"]!);

    var textToImageService = new OpenAITextToImageService(
    modelId: "dall-e-3",
    apiKey: config["OpenAI:ApiKey"]!);

    Console.WriteLine("\nGenerating text from audio ...\n");
    var generatedTexts = await audioToTextService.GetTextContentsAsync(audioContent);
    var generatedText = generatedTexts[0];

    Console.WriteLine($"Text Generated: {generatedText}");

    Console.WriteLine("\nGenerating image from text ...\n");
    var generatedImages = await textToImageService.GetImageContentsAsync(generatedText);
    var generatedImage = generatedImages[0];

    Console.WriteLine("Image Generated. Ctrl + Click to view: " + generatedImage.Uri);
});

examples.Add(async () => // Multi modalities image -> text -> audio)
{
    Console.WriteLine("=== Multi modalities image -> text -> audio ===\n\n");

    string folderPath = @"C:\Users\rbarreto\OneDrive - Microsoft\Pictures\Camera Roll";
    string huggingFaceModel = "Salesforce/blip-image-captioning-large";
    string openAIModel = "gpt-4o-mini";

    DirectoryInfo directoryInfo = new(folderPath);

    // Get the most recent image created in the directory
    FileInfo? mostRecentFile = directoryInfo.GetFiles("*.jpg")
                                           .OrderByDescending(f => f.CreationTime)
                                           .FirstOrDefault();

    Console.WriteLine($"Most recent photo: {mostRecentFile!.FullName}");

    var imageContent = new Microsoft.SemanticKernel.ImageContent(File.ReadAllBytes(mostRecentFile!.FullName), "image/jpeg");

    var huggingFaceImageToTextService = new HuggingFaceImageToTextService(huggingFaceModel, apiKey: config["HuggingFace:ApiKey"]!);

    var openAiChatService = new OpenAIChatCompletionService(modelId: openAIModel, apiKey: config["OpenAI:ApiKey"]!);

    Console.WriteLine("\nGenerating text from image (Hugging face) ...\n");
    var huggingFaceImageText = (await huggingFaceImageToTextService.GetTextContentsAsync(imageContent))[0];
    Console.WriteLine($"Hugging Face Text Generated:\n{huggingFaceImageText}\n");

    Console.WriteLine("\nGenerating text from image (OpenAI)...\n");
    ChatMessageContent chatMessageContent = new(AuthorRole.User, [
        new Microsoft.SemanticKernel.TextContent("Describe the image"),
        imageContent
    ]);
    var openAIImageText = await openAiChatService.GetChatMessageContentAsync([chatMessageContent]);
    Console.WriteLine($"OpenAI Text Generated:\n{openAIImageText}\n");

    Console.WriteLine("\nGenerating audio from text ...\n");
    var textToAudioService = new OpenAITextToAudioService(
        modelId: "tts-1",
        apiKey: config["OpenAI:ApiKey"]!);

    var huggingFaceGeneratedAudio = (await textToAudioService.GetAudioContentsAsync(huggingFaceImageText.ToString()))[0];
    var file = Path.Combine(Directory.GetCurrentDirectory(), "image-description-output.mp3");
    await File.WriteAllBytesAsync(file, huggingFaceGeneratedAudio.Data!.Value);

    var openAIGeneratedAudio = (await textToAudioService.GetAudioContentsAsync(openAIImageText.ToString()))[0];
    var file2 = Path.Combine(Directory.GetCurrentDirectory(), "image-description-output-2.mp3");
    await File.WriteAllBytesAsync(file2, openAIGeneratedAudio.Data!.Value);

    Console.WriteLine($"\nHuggingFace Audio Description. Ctrl + Click to listen: \n{new Uri(file).AbsoluteUri}");

    Console.WriteLine($"\nOpenAI Audio Description. Ctrl + Click to listen: \n{new Uri(file2).AbsoluteUri}");
});

examples.Add(async () => // Multi modalities audio -> text services
{
    Console.WriteLine("=== Multi modalities audio -> text services ===\n\n");

    string folderPath = @"C:\Users\rbarreto\OneDrive - Microsoft\Documents\Sound Recordings";

    DirectoryInfo directoryInfo = new(folderPath);

    // Get the most recent file created in the directory
    FileInfo? mostRecentFile = directoryInfo.GetFiles()
                                           .OrderByDescending(f => f.CreationTime)
                                           .FirstOrDefault();

    Console.WriteLine($"Most recent file: {mostRecentFile!.FullName}");

    var audioContent = new Microsoft.SemanticKernel.AudioContent(File.ReadAllBytes(mostRecentFile!.FullName), "audio/m4a");

    var audioToTextService = new OpenAIAudioToTextService(
    modelId: "whisper-1",
    apiKey: config["OpenAI:ApiKey"]!);

    Console.WriteLine("\nGenerating text from audio ...\n");
    var generatedTexts = await audioToTextService.GetTextContentsAsync(audioContent);
    var generatedText = generatedTexts[0];

    Console.WriteLine($"Text Generated: {generatedText}");
});

examples.Add(async () => // Multi modalities audio -> text services -> audio
{
    Console.WriteLine("=== Multi modalities audio -> text services ===\n\n");

    string folderPath = @"C:\Users\rbarreto\OneDrive - Microsoft\Documents\Sound Recordings";

    DirectoryInfo directoryInfo = new(folderPath);

    // Get the most recent file created in the directory
    FileInfo? mostRecentFile = directoryInfo.GetFiles()
                                           .OrderByDescending(f => f.CreationTime)
                                           .FirstOrDefault();

    Console.WriteLine($"Most recent file: {mostRecentFile!.FullName}");

    var audioContent = new Microsoft.SemanticKernel.AudioContent(File.ReadAllBytes(mostRecentFile!.FullName), "audio/m4a");

    var audioToTextService = new OpenAIAudioToTextService(
    modelId: "whisper-1",
    apiKey: config["OpenAI:ApiKey"]!);

    Console.WriteLine("\nGenerating text from audio ...\n");
    var generatedTexts = await audioToTextService.GetTextContentsAsync(audioContent);
    var generatedText = generatedTexts[0].ToString();

    Console.WriteLine($"Text from Audio Generated: {generatedText}");

    var chatService = new Azure.AI.Inference.ChatCompletionsClient(
    endpoint: new Uri(config["AzureAIInference:Endpoint"]!),
    credential: new Azure.AzureKeyCredential(config["AzureAIInference:ApiKey"]!))
    .AsChatClient("phi3")
    .AsChatCompletionService();

    var response = await chatService.GetChatMessageContentAsync(generatedText);

    Console.WriteLine($"\nAI Model Generated Text:\n {response}");

    var textToAudioService = new OpenAITextToAudioService(
        modelId: "tts-1",
        apiKey: config["OpenAI:ApiKey"]!);

    var generatedAudio = (await textToAudioService.GetAudioContentsAsync(response.ToString()!))[0];

    var file = Path.Combine(Directory.GetCurrentDirectory(), "output.mp3");
    await File.WriteAllBytesAsync(file, generatedAudio.Data!.Value);

    Console.WriteLine($"\nAudio Generated. Ctrl + Click to listen: {new Uri(file).AbsoluteUri}");

});

#endregion Examples

await examples[0](); // Run first example

Console.WriteLine("\n\n\n");

#region Plugins

public class MyStatelessPlugin
{
    [KernelFunction]
    public static string GetDateTime()
    {
        var currentTime = DateTime.UtcNow.ToString("R");
        return currentTime;
    }
}

public class MyStatefulPlugin
{
    private int _counter;

    public MyStatefulPlugin(int counter)
    {
        this._counter = counter;
    }

    [KernelFunction]
    public int GetCounter()
    {
        return this._counter++;
    }
}

[Description("This is a light bulb")]
public class MyDescribedStatefulPlugin(bool isOn = false)
{
    private bool _isOn = isOn;
    [KernelFunction, Description("Turns the light bulb on")]
    public void TurnOn()
    {
        this._isOn = true;
    }
    [KernelFunction, Description("Turns the light bulb off")]
    public void TurnOff()
    {
        this._isOn = false;
    }
    [KernelFunction, Description("Checks if the light bulb is on")]
    public bool IsOn()
    {
        return this._isOn;
    }
}

#endregion

#region Filters

public class SelectedModelRenderFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        Console.WriteLine($"Settings service id: {context.Arguments.ExecutionSettings?.FirstOrDefault().Value.ServiceId ?? "--"}");
        Console.WriteLine($"Settings model id: {context.Arguments.ExecutionSettings?.FirstOrDefault().Value.ModelId ?? "--"}");

        await next(context);
    }
}

public class EchoPromptRenderFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        await next(context);
        Console.WriteLine($"User Prompt: {context.RenderedPrompt}\n");
    }
}
#endregion

#region Embedding Models

internal sealed class SelectedModel
{
    [VectorStoreRecordKey]
    [TextSearchResultName]
    public string ModelId { get; set; } = string.Empty;

    [VectorStoreRecordData]
    [TextSearchResultValue]
    public string PromptingDomain { get; set; } = string.Empty;

    [VectorStoreRecordVector(Dimensions: 512)]
    public ReadOnlyMemory<float> Embedding { get; set; }
}

#endregion

#region Debugging
public class CustomHttpClientHandler : HttpClientHandler
{
    protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Console.WriteLine($"Request: {request.RequestUri}");
        // var requestBody = await request.Content!.ReadAsStringAsync();
        // Console.WriteLine($"Request Body: {requestBody}");
        return await base.SendAsync(request, cancellationToken);
    }
}
#endregion