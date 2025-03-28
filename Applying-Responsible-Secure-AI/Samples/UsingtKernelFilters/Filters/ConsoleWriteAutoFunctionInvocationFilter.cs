using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// This filter is used specifically for function calling models where your want to intercept 
// AI Model function calls and trace (Console.Write) or change the behavior.
/// </summary>
public class ConsoleWriteAutoFunctionInvocationFilter : IAutoFunctionInvocationFilter
{
    public async Task OnAutoFunctionInvocationAsync(AutoFunctionInvocationContext context, Func<AutoFunctionInvocationContext, Task> next)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;

        await next.Invoke(context);

        Console.WriteLine(string.Concat($"Auto Function Result: '{context.Result}'"));
        Console.ResetColor();
    }
}