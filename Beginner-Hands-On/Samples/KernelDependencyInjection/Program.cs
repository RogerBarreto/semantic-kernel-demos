using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Console.WriteLine("=== Kernel Dependency Injection ===\n");

var modelId = "llama3.2";
var endpoint = new Uri("http://localhost:11434");

var services = new ServiceCollection();
services.AddOllamaChatCompletion(modelId, endpoint);
services.AddKernel();

var serviceProvider = services.BuildServiceProvider();
var kernel = serviceProvider.GetRequiredService<Kernel>();

var prompt = "Why is Neptune blue?";
Console.WriteLine($"User > {prompt}");
Console.Write("Assistant > ");
await foreach (var token in kernel.InvokePromptStreamingAsync(prompt))
{
    Console.Write(token);
};

Console.WriteLine("\n\nPress any key to exit...");
Console.ReadLine();
