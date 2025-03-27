using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Sample;

/// <summary>
/// Filter which use Text Anonymizer to detect PII in prompt and update the prompt by following specified rules before sending it to LLM.
/// </summary>
internal sealed class PromptAnonymizerFilter(
        ILogger logger,
        PresidioTextAnalyzerService analyzerService,
        PresidioTextAnonymizerService anonymizerService,
        Dictionary<string, PresidioTextAnonymizer> anonymizers) : IPromptRenderFilter
    {
        public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
        {
            await next(context);

            // Get rendered prompt
            var prompt = context.RenderedPrompt!;

            logger.LogTrace("Prompt before anonymization : \n{Prompt}", prompt);

            // Call analyzer to detect PII
            var analyzerResults = await analyzerService.AnalyzeAsync(new PresidioTextAnalyzerRequest { Text = prompt });

            // Call anonymizer to update the prompt by following specified rules. Pass analyzer results received on previous step.
            var anonymizerResult = await anonymizerService.AnonymizeAsync(new PresidioTextAnonymizerRequest
            {
                Text = prompt,
                AnalyzerResults = analyzerResults,
                Anonymizers = anonymizers
            });

            logger.LogTrace("Prompt after anonymization : \n{Prompt}", anonymizerResult.Text);

            // Update prompt in context to sent new prompt without PII to LLM
            context.RenderedPrompt = anonymizerResult.Text;
        }
    }