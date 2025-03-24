using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Console.WriteLine("=== Kernel prompting streaming with Dependency Injection ===");

var modelId = "llama3.2";
var endpoint = new Uri("http://localhost:11434");

var services = new ServiceCollection();
services
    .AddKernel()
        .AddOllamaChatCompletion(modelId, endpoint);

var serviceProvider = services.BuildServiceProvider();

var kernel = serviceProvider.GetRequiredService<Kernel>();

await foreach (var token in kernel.InvokePromptStreamingAsync("Why is Neptune blue?"))
{
    Console.Write(token);
};
