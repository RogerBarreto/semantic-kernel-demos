using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using Sample;

Console.WriteLine("=== Plugin Function Calling with Stateful and Stateless Plugins ===");

var modelId = "llama3.2";
var endpoint = new Uri("http://localhost:11434");
var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId, endpoint)
    .Build();

var lightBulbPlugin = new LightBulbPlugin(isOn: true);
kernel.Plugins.AddFromObject(lightBulbPlugin);
kernel.Plugins.AddFromType<TimePlugin>();

var service = kernel.GetRequiredService<IChatCompletionService>();

var settings = new OllamaPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

var prompt = "What is the light status and what time of the day is it?";
Console.WriteLine($"\nUser: {prompt}");

var result = await kernel.InvokePromptAsync(prompt, new(settings));
Console.WriteLine($"Assistant: {result}");

prompt = "Please switch the light.";
Console.WriteLine($"\nUser: {prompt}");

result = await kernel.InvokePromptAsync(prompt, new(settings));
Console.WriteLine($"Assistant: {result}");

prompt = "What is the light bulb status and what time of the day is it?";
Console.WriteLine($"\nUser: {prompt}");

var contentResult = await service.GetChatMessageContentAsync(prompt, settings, kernel);
Console.WriteLine($"Assistant: {contentResult}");

Console.ReadLine();