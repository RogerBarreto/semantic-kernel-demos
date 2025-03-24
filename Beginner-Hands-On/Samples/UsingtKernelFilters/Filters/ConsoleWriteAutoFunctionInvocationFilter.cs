using System.Text;
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
        Console.WriteLine(string.Concat($"Auto Function Invoked: '{context.Function.Name}'",

                                        // Request sequence index identifies what and how many requests were made to fulfill the function calling
                                        $" - Request sequence index: {context.RequestSequenceIndex}",

                                        // Function sequence index identifies what and which is the current function index in the function calling for the current request, 
                                        // Note: Multiple functions can be called in a single request (parallel function calling)
                                        $" - Function sequence index: {context.FunctionSequenceIndex}",

                                        // Total number of functions requested to be called by the AI Model for the current request
                                        $" - Total number of functions: {context.FunctionCount}"));
        Console.ResetColor();

        await next.Invoke(context);
    }
}