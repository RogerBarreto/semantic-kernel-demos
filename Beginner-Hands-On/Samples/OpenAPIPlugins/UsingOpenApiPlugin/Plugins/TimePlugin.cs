using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// This plugin is important so the AI Model can know which day is today so it know what is the weather forecast for tomorrow.
/// </summary>
public class TimePlugin
{
    [KernelFunction]
    public static string GetDateTime()
    {
        Console.WriteLine($"Plugin Triggered: {nameof(TimePlugin)} - GetDateTime");

        var currentTime = DateTime.UtcNow.ToString("R");
        return currentTime;
    }
}