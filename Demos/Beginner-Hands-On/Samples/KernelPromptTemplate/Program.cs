using Microsoft.SemanticKernel;

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Console.WriteLine("=== Kernel Prompt with Template & Variables ===");

var modelId = "phi4";
var endpoint = new Uri("http://localhost:11434");

var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId, endpoint)
    .Build();

var myPrompt = "Hello, I'm {{$name}}, and I have {{$age}}. What is origin of my name?";

var arguments = new KernelArguments()
{
    ["name"] = "Roger",
    ["age"] = 42
};

await foreach (var token in kernel.InvokePromptStreamingAsync(myPrompt, arguments))
{
    Console.Write(token);
}