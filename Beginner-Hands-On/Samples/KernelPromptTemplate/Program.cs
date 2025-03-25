using Microsoft.SemanticKernel;

Console.WriteLine("=== Kernel Basic Template & Argument variables ===\n");

var modelId = "phi4";
var endpoint = new Uri("http://localhost:11434");

var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId, endpoint)
    .Build();

var prompt = """
    Hello, today is {{$date}}, I'm {{$name}}, and I have {{$age}}. 
    What you thin about my name and what technological advancement happened the year I was born?
    """;

var arguments = new KernelArguments()
{
    ["date"] = DateTime.Now.ToString("yyyy-MM-dd"),
    ["name"] = "Roger",
    ["age"] = 42
};

Console.WriteLine($"User Template\n---\n{prompt}\n---\n");
Console.Write("Assistant > ");
await foreach (var token in kernel.InvokePromptStreamingAsync(prompt, arguments))
{
    Console.Write(token);
}

Console.WriteLine("\n\nEnter to continue...");
Console.ReadLine();
Console.Clear();

Console.WriteLine("=== Kernel Prompty Template & Argument variables ===\n");

var promptyTemplate = """
    ---
    name: PromptySample
    ---
    Hello, today is {{date}}, I'm {{name}}, and I have {{age}}. 
    What you thin about my name and what technological advancement happened the year I was born?
    """;

#pragma warning disable SKEXP0040 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var promptyFunction = kernel.CreateFunctionFromPrompty(promptyTemplate);

Console.WriteLine($"User Template\n{promptyTemplate}\n---\n");
Console.Write("Assistant > ");
await foreach (var token in kernel.InvokeStreamingAsync(promptyFunction, arguments))
{
    Console.Write(token);
}
