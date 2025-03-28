using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// This filter is used to intercept before and after a kernel function is invoked and write to the console its name and result.
/// </summary>
public class OverrideFunctionInvocationFilter : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        await next.Invoke(context);

        context.Result = new FunctionResult(context.Result, "The light is off.");
    }
}