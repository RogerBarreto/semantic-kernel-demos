using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// This filter is used to intercept before a kernel prompt is rendered and write to the console what is the selected model in 
/// the prompt execution settings.
/// </summary>
public class SelectedModelRenderFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        // For majority of scenarios the prompt execution settings will be a single setting item, for simplicity capturing the first item
        Console.WriteLine($"Selected ModelId > {context.Arguments.ExecutionSettings?.FirstOrDefault().Value.ModelId ?? "--"}");

        await next(context);
    }
}