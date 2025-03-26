using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Sample;

var apiKey = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["OpenAI:ApiKey"]!;

Console.WriteLine("=== Using Kernel Filters ===");

var lightBulbPlugin = new LightBulbPlugin(isOn: true);

var modelId = "gpt-4o-mini";
var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion(modelId, apiKey)
    .Build();

// Add the custom function invocation filter
kernel.FunctionInvocationFilters.Add(new ConsoleWriteFunctionInvocationFilter());

// Add the custom prompt rendering filter
kernel.PromptRenderFilters.Add(new ConsoleWritePromptRenderingFilter());

// Add the custom auto function invocation filter
kernel.AutoFunctionInvocationFilters.Add(new ConsoleWriteAutoFunctionInvocationFilter());

kernel.Plugins.AddFromObject(lightBulbPlugin);
kernel.Plugins.AddFromType<TimePlugin>();

var service = kernel.GetRequiredService<IChatCompletionService>();

var settings = new OpenAIPromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
var arguments = new KernelArguments(settings) { ["name"] = "Roger" };

var prompt = "{{$name}}: What is the light status and what time of the day is it?";
await kernel.InvokePromptAsync(prompt, arguments);

prompt = "{{$name}}: Please switch the light.";
var promptFunction = kernel.CreateFunctionFromPrompt(prompt, functionName: "FunctionFromPrompt");
await kernel.InvokeAsync(promptFunction, arguments);

// Direct service calls don't trigger prompt rendering
prompt = $"{arguments["name"]}: What is the light bulb status and what time of the day is it?";
Console.WriteLine($"\n{prompt}");
var kernelContent = await service.GetChatMessageContentAsync(prompt, settings, kernel);
Console.WriteLine($"Assistant: {kernelContent}");

Console.ReadLine();