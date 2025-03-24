using Microsoft.SemanticKernel;

// Setup ollama
// https://ollama.com/blog/ollama-is-now-available-as-an-official-docker-image
// docker run -d -e OLLAMA_KEEP_ALIVE=-1 -v D:\temp\ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama

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
