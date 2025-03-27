using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
    /// Filter which use Text Analyzer to detect PII in prompt and prevent sending it to LLM.
    /// </summary>
    internal sealed class PromptAnalyzerFilter(
        ILogger logger,
        PresidioTextAnalyzerService analyzerService,
        double scoreThreshold) : IPromptRenderFilter
    {
        public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
        {
            await next(context);

            // Get rendered prompt
            var prompt = context.RenderedPrompt!;

            logger.LogTrace("Prompt: {Prompt}", prompt);

            // Call analyzer to detect PII
            var analyzerResults = await analyzerService.AnalyzeAsync(new PresidioTextAnalyzerRequest { Text = prompt });

            var piiDetected = false;

            // Check analyzer results
            foreach (var result in analyzerResults)
            {
                logger.LogInformation("Entity type: {EntityType}. Score: {Score}", result.EntityType, result.Score);

                if (result.Score > scoreThreshold)
                {
                    piiDetected = true;
                }
            }

            // If PII detected, throw an exception to prevent this prompt from being sent to LLM.
            // It's also possible to override 'context.Result' to return some default function result instead.
            if (piiDetected)
            {
                throw new KernelException("Prompt contains PII information. Operation is canceled.");
            }
        }
    }