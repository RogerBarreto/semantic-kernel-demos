using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Services;
using Sample;

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Console.WriteLine("=== Kernel AI Model Selection ===");

// 1st GPU Inference AI Model (Ollama - llama3.2)
var ollamaModelId = "llama3.2";
var ollamaEndpoint = new Uri("http://localhost:11434");

// 2nd CPU Inference AI Model (ONNX - phi3)
var fileModelId = "phi3";

// Ensure the path match the path where you cloned the repo
var fileModelPath = "D:\\repo\\huggingface-models\\Phi-3-mini-4k-instruct-onnx\\cpu_and_mobile\\cpu-int4-rtn-block-32";

// 3rd Cloud Inference AI Model (OpenAI - gpt-4o-mini)
var openAIModelId = "gpt-4o-mini";
var apiKey = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["OpenAI:ApiKey"]!;

var kernelBuilder = Kernel.CreateBuilder();


kernelBuilder
        .AddOpenAIChatCompletion(openAIModelId, apiKey)
        .AddOnnxRuntimeGenAIChatCompletion(fileModelId, fileModelPath)
        .AddOllamaChatCompletion(ollamaModelId, ollamaEndpoint);
        // Last service will be the default service

var kernel = kernelBuilder.Build();

kernel.PromptRenderFilters.Add(new SelectedModelRenderFilter());

var modelNames = kernel.GetAllServices<IChatCompletionService>().Select(s => s.GetModelId());

while (true)
{
    Console.Write($"\nAvailable models: {string.Join(", ", modelNames)}\nModel name > ");
    var selectedModel = Console.ReadLine();
    if (string.IsNullOrEmpty(selectedModel)) { break; }

    if (!modelNames.Contains(selectedModel))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Invalid model name");
        Console.ResetColor();
        continue;
    }

    Console.Write("User > ");

    var userPrompt = Console.ReadLine();

    if (string.IsNullOrEmpty(userPrompt)) { break; }

    var settings = new PromptExecutionSettings { ModelId = selectedModel };

    
    var isFirstToken = true;
    await foreach (var token in kernel.InvokePromptStreamingAsync(userPrompt, new(settings)))
    {
        if (isFirstToken)
        {
            Console.Write("\nAssistant > ");
            isFirstToken = false;
        }

        Console.Write(token);
    };
}

// Dispose the ONNX service to avoid memory leaking
var onnxService = kernel.GetAllServices<IChatCompletionService>()
    .Where(s => s.GetModelId() == fileModelId)
    .FirstOrDefault() as IDisposable;

onnxService?.Dispose();
