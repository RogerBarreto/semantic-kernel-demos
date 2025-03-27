using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Sample;

var apiKey = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["OpenAI:ApiKey"]!;
var builder = Kernel.CreateBuilder();
var presidioAnalyzerEndpoint = new Uri("http://localhost:5002");
var presidioAnonymizerEndpoint = new Uri("http://localhost:5001");

builder.Services.AddHttpClient<PresidioTextAnalyzerService>(client => { client.BaseAddress = presidioAnalyzerEndpoint; });

// Change confidence score threshold ratio value from 0 to 1 during testing to see how the logic will behave.
builder.Services.AddSingleton(new PresidioAnalyzerConfig() { ScoreThreshold = 0.9 });

// Add prompt filter to analyze rendered prompt for PII before sending it to LLM.
builder.Services.AddSingleton<IPromptRenderFilter, PromptAnalyzerFilter>();

// Add OpenAI chat completion service
builder.AddOpenAIChatCompletion("gpt-4o-mini", apiKey);

var kernel = builder.Build();

Console.Write("=== Using prompt with PII ===\n\n");

try
{
    var promptWithPII = "John Smith has a card 1111 2222 3333 4444";
    Console.WriteLine($"User > {promptWithPII}");
    await kernel.InvokePromptAsync(promptWithPII);
}
catch (InvalidOperationException exception)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Prompt aborted - {exception.Message}");
    Console.ResetColor();
}

Console.Write("\n=== Using prompt without PII ===\n\n");

var prompt = "Hi, can you help me?";
Console.WriteLine($"User > {prompt}");
var result = await kernel.InvokePromptAsync(prompt);
Console.WriteLine($"Assistant > {result}");