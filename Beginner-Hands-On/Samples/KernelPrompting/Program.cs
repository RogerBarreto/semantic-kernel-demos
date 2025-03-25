using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Console.WriteLine("=== Kernel prompting non-streaming ===");

var modelId = "llama3.2";
var endpoint = new Uri("http://localhost:11434");

var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId, endpoint)
    .Build();

var response = await kernel.InvokePromptAsync("Hello, how are you?");
Console.WriteLine(response);

Console.WriteLine("\n=== Kernel prompting streaming ===");

await foreach (var token in kernel.InvokePromptStreamingAsync("Hello, how are you?"))
{
    Console.Write(token);
}
