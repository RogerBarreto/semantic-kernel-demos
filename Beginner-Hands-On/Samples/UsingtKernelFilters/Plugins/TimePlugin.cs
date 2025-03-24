using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// Stateless plugin (It does not keep state)
/// </summary>
public class TimePlugin
{
    [KernelFunction]
    public static string GetDateTime()
    {
        var currentTime = DateTime.UtcNow.ToString("R");
        return currentTime;
    }
}