using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// This filter is used to intercept before and after a kernel prompt is rendered and write to the console its name and result.
/// </summary>
public class ConsoleWritePromptRenderingFilter : IPromptRenderFilter
{
    public async Task OnPromptRenderAsync(PromptRenderContext context, Func<PromptRenderContext, Task> next)
    {
        // Before prompt was rendered
        await next.Invoke(context);

        // After prompt was rendered
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Prompt Rendered: '{context.RenderedPrompt}'");
        Console.ResetColor();

        // Write the prompt to the console as is with no color formatting (Similar to assistant output)
        Console.WriteLine($"{context.RenderedPrompt}");
    }
}

