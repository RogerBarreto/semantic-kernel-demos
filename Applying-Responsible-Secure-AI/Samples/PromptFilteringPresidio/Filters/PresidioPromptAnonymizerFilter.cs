using Microsoft.SemanticKernel;
using Sample;

/// <summary>
/// Filter which use Text Anonymizer to detect PII in prompt and update the prompt by following specified rules before sending it to LLM.
/// </summary>
internal sealed class PromptAnonymizerFilter(
        PresidioTextAnalyzerService analyzerService,
        PresidioTextAnonymizerService anonymizerService,
        PresidioAnonymizerConfig config) : IPromptRenderFilter
    {
        public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
        {
            await next(context);
            
            if (!config.Enabled) { return; }

            // Get rendered prompt
            var prompt = context.RenderedPrompt!;

            Console.ForegroundColor = ConsoleColor.Cyan;

            // Call analyzer to detect PII
            var analyzerResults = await analyzerService.AnalyzeAsync(new PresidioTextAnalyzerRequest { Text = prompt });

            // Call anonymizer to update the prompt by following specified anonymizer rules.
            // Pass analyzer results received on previous step.
            var anonymizerResult = await anonymizerService.AnonymizeAsync(new PresidioTextAnonymizerRequest
            {
                Text = prompt,
                AnalyzerResults = analyzerResults,
                Anonymizers = config.Anonymizers
            });

            Console.WriteLine($"Anonymized prompt: \n{anonymizerResult.Text}\n");
            Console.ResetColor();

            // Update prompt in context to override the prompt without PII before it goes to LLM
            context.RenderedPrompt = anonymizerResult.Text;
        }
    }