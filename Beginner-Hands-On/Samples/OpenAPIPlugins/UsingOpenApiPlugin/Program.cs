using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.OpenApi;
using Microsoft.Extensions.Configuration;
using Sample;

var apiKey = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["OpenAI:ApiKey"]!;

var kernel = Kernel.CreateBuilder()
    .AddOpenAIChatCompletion("gpt-4o-mini", apiKey)
    .Build();

var weatherOpenApiUri = new Uri("https://localhost:7299/swagger/v1/swagger.json");
var weatherPlugin = await OpenApiKernelPluginFactory.CreateFromOpenApiAsync("WeatherService", weatherOpenApiUri);

kernel.Plugins.Add(weatherPlugin);
kernel.Plugins.AddFromType<TimePlugin>(); // Enable AI to know which day is today

var settings = new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };

var prompt = "What is the weather forecast for tomorrow? Ensure you check which day is today.";
Console.WriteLine($"\nUser > {prompt}");
var result = await kernel.InvokePromptAsync(prompt, new(settings));

Console.WriteLine($"Assistant > {result}");

Console.WriteLine("\n\nPress any key to exit...");
Console.ReadLine();