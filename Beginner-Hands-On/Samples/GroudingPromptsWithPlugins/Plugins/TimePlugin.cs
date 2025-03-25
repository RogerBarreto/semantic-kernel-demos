using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// Stateless plugin (It does not keep state)
/// </summary>
public class TimePlugin
{
    const string foregroundYellow = "\u001b[93m";
    const string foregroundReset = "\u001b[0m";

    [KernelFunction]
    public static string GetDateTime()
    {
        Console.WriteLine($"{foregroundYellow}Plugin Triggered: {nameof(TimePlugin)} - GetDateTime{foregroundReset}");

        var currentTime = DateTime.UtcNow.ToString("R");
        return currentTime;
    }
}