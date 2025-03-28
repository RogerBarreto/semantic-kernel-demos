using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// This filter is used to intercept before and after a kernel prompt is rendered and write to the console its name and result.
/// </summary>
public class OverridePromptRenderingFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        if (context.Arguments.TryGetValue("name", out var name))
        {
            context.Arguments["name"] = $"Mr. Override";
        };

        // Before prompt was rendered
        await next.Invoke(context);

        // Write the prompt to the console as is with no color formatting (Similar to assistant output)
        Console.WriteLine($"Final prompt: {context.RenderedPrompt}");
    }
}

