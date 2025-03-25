using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

Console.WriteLine("=== Using prompt execution settings ===\n");

var modelId = "llama3.2";
var endpoint = new Uri("http://localhost:11434");
var prompt = "Write a 5-word sentence describing a sunny day.";

// At a low temperature, you'd expect a straightforward, predictable response like "Warm sunshine fills the air." 
var lowTempSettings = new OllamaPromptExecutionSettings { Temperature = 0 };

// At a high temperature, you might get something more unusual or poetic, like "Warmth filled the bright sky."
var highTempSettings = new OllamaPromptExecutionSettings { Temperature = 1.0f };

var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId, endpoint)
    .Build();

// Get the service from the kernel
var service = kernel.GetRequiredService<IChatCompletionService>();

Console.WriteLine("Kernel (low temperature):");
await foreach (var token in kernel.InvokePromptStreamingAsync(prompt, new(lowTempSettings)))
{
    Console.Write(token);
}

Console.WriteLine("\n\nChat Service (high temperature):");
await foreach (var token in service.GetStreamingChatMessageContentsAsync(prompt, highTempSettings))
{
    Console.Write(token);
}
