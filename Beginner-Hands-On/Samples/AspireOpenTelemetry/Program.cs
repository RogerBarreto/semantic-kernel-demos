using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sample;

Console.WriteLine("=== Open Telemetry Aspire Dashboard ===\n\n");

var builder = Kernel.CreateBuilder();
var apiKey = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["OpenAI:ApiKey"]!;
var modelId = "gpt-4o-mini";
var oTelExporterEndpoint = "http://localhost:4317";

var resourceBuilder = ResourceBuilder
    .CreateDefault()
    .AddService("SemanticKernel-Telemetry");

// Enable model diagnostics with sensitive data.
AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);

// Configuring the tracer provider
using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddSource("Microsoft.SemanticKernel*")
    .AddOtlpExporter(options => options.Endpoint = new Uri(oTelExporterEndpoint))
    .Build();

// Configuring the meter provider
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(resourceBuilder)
    .AddMeter("Microsoft.SemanticKernel*")
    .AddOtlpExporter(options => options.Endpoint = new Uri(oTelExporterEndpoint))
    .Build();

// Configuring the logger factory
using var loggerFactory = LoggerFactory.Create(builder =>
{
    // Add OpenTelemetry as a logging provider
    builder.AddOpenTelemetry(options =>
    {
        options.SetResourceBuilder(resourceBuilder);
        options.AddOtlpExporter(options => options.Endpoint = new Uri(oTelExporterEndpoint));
        // Format log messages. This is default to false.
        options.IncludeFormattedMessage = true;
        options.IncludeScopes = true;
    });
    builder.SetMinimumLevel(LogLevel.Information);
});

builder.AddOpenAIChatCompletion(modelId, apiKey);
builder.Services.AddSingleton(loggerFactory);
builder.Plugins.AddFromObject(new LightBulbPlugin());

Kernel kernel = builder.Build();

Console.WriteLine("""
Use prompts to control the light bulb and give its status:
- Turn on the light
- Turn off the light
- What is the light status?
""");

while (true)
{
    Console.Write("User > ");
    var userPrompt = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(userPrompt)) { break; }

    var settings = new PromptExecutionSettings { FunctionChoiceBehavior = FunctionChoiceBehavior.Auto() };
    var result = await kernel.InvokePromptAsync(userPrompt, new(settings));

    Console.ResetColor();
    Console.WriteLine($"Assistant > {result}");
}

const string foregroundGreen = "\u001b[32m";
const string foregroundReset = "\u001b[0m";
Console.WriteLine($"\nCheck aspire telemetry dashboard at {foregroundGreen}http://localhost:18888/login?t=2f917b9650cf62ef50dfab3bc5fccc29{foregroundReset}");

