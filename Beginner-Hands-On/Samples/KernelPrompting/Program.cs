using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

Console.WriteLine("=== Kernel prompting  - Non-Streaming ===\n");

var apiKey = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build()["OpenAI:ApiKey"]!;

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4o", apiKey)
    .Build();
var prompt = "Why is Neptune blue?";

Console.WriteLine(await kernel.InvokePromptAsync(prompt));

Console.WriteLine("\n=== Kernel prompting - Streaming ===\n");
await foreach (var token in kernel.InvokePromptStreamingAsync(prompt))
{
    Console.Write(token);
}
