using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;

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

var kernel = kernelBuilder.Build();

// Get the service by name
var service = kernel.GetRequiredService<ITextEmbeddingGenerationService>("ollama");