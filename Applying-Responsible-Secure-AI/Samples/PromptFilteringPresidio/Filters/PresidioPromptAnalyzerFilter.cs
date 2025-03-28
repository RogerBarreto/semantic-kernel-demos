using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// Filter which use Text Analyzer to detect PII in prompt and prevent sending it to LLM.
/// </summary>
internal sealed class PromptAnalyzerFilter(
    PresidioTextAnalyzerService analyzerService,
    PresidioAnalyzerConfig config) : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        await next(context);

        if (!config.Enabled) { return; }

        // Get rendered prompt
        var prompt = context.RenderedPrompt!;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Prompt: {prompt}");

        // Call analyzer to detect PII
        var analyzerResults = await analyzerService.AnalyzeAsync(new PresidioTextAnalyzerRequest { Text = prompt });

        var piiDetected = false;

        // Check analyzer results
        foreach (var result in analyzerResults)
        {
            Console.WriteLine($"Entity type: {result.EntityType}. Score: {result.Score}");

            if (result.Score > config.ScoreThreshold!)
            {
                piiDetected = true;
            }
        }

        Console.ResetColor();

        // If PII detected, throw an exception to prevent this prompt from being sent to LLM.
        // It's also possible to override 'context.Result' to return some default function result instead.
        if (piiDetected)
        {
            throw new InvalidOperationException("Prompt contains PII information. Operation is canceled.");
        }
    }
}