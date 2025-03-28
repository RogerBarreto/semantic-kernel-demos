using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Sample;

var apiKey = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["OpenAI:ApiKey"]!;
var builder = Kernel.CreateBuilder();
var presidioAnalyzerEndpoint = new Uri("http://localhost:5002");
var presidioAnonymizerEndpoint = new Uri("http://localhost:5001");
var analyzerConfig = new PresidioAnalyzerConfig() { Enabled = true, ScoreThreshold = 0.9 };
var anonymizerConfig = new PresidioAnonymizerConfig()
{
    Enabled = false,
    Anonymizers =
    {
        [PresidioAnalyzerEntityType.PhoneNumber] = new PresidioTextAnonymizer { Type = PresidioAnonymizerType.Redact },
        [PresidioAnalyzerEntityType.Person] = new PresidioTextAnonymizer { Type = PresidioAnonymizerType.Replace, NewValue = "ANONYMIZED" }
    }
};

// Add Presidio Text Analyzer service and configure HTTP client for it
builder.Services.AddHttpClient<PresidioTextAnalyzerService>(client => { client.BaseAddress = presidioAnalyzerEndpoint; });

// Add Presidio Text Anonymizer service and configure HTTP client for it
builder.Services.AddHttpClient<PresidioTextAnonymizerService>(client => { client.BaseAddress = presidioAnonymizerEndpoint; });

// Change confidence score threshold ratio value from 0 to 1 during testing to see how the logic will behave.
builder.Services.AddSingleton(anonymizerConfig);
builder.Services.AddSingleton(analyzerConfig);

// Add prompt filter to analyze rendered prompt for PII before sending it to LLM.
builder.Services.AddSingleton<IPromptRenderFilter, PromptAnalyzerFilter>();

// Add prompt filter to anonymize rendered prompt before sending it to LLM.
builder.Services.AddSingleton<IPromptRenderFilter, PromptAnonymizerFilter>();

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

analyzerConfig.Enabled = false;

Console.WriteLine("\n=== Using prompt anonymizer ===\n\n");

anonymizerConfig.Enabled = true;

// Define instructions for LLM how to react when certain conditions are met for demonstration purposes
var executionSettings = new OpenAIPromptExecutionSettings
{
    ChatSystemPrompt = "If prompt does not contain first and last names - return 'true'."
};

prompt = """

    | Name | Phone number | Position |
    |---|---|---|
    | John Smith | +1 (123) 456-7890 | Developer |
    | Alice Doe | +1 (987) 654-3120 | Manager |
    | Emily Davis | +1 (555) 555-5555 | Designer |
    """
;

Console.WriteLine($"User > {prompt}\n");
result = await kernel.InvokePromptAsync(prompt, new(executionSettings));
Console.WriteLine($"Assistant > {result}");

Console.ReadLine();