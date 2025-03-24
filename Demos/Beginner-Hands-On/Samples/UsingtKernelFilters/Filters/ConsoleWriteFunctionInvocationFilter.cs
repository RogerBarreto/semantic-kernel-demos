using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// This filter is used to intercept before and after a kernel function is invoked and write to the console its name and result.
/// </summary>
public class ConsoleWriteFunctionInvocationFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        string writeMessage = $"Function Invoked: '{context.Function.Name}'";
        if (!string.IsNullOrEmpty(context.Function.PluginName))
        {
            writeMessage += $" - Plugin: '{context.Function.PluginName}'";
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(writeMessage);
        Console.ResetColor();

        await next.Invoke(context);

        // On this sample all functions without a plugin name are AI model responses
        if (string.IsNullOrEmpty(context.Function.PluginName))
        {
            Console.WriteLine($"Assistant: {context.Result}");
        }
    }
}