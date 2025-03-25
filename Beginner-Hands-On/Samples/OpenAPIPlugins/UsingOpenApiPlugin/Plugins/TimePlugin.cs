using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Sample;

/// <summary>
/// This plugin is important so the AI Model can know which day is today so it know what is the weather forecast for tomorrow.
/// </summary>
public class TimePlugin
{
    const string foregroundYellow = "\u001b[93m";
    const string foregroundReset = "\u001b[0m";

    [KernelFunction, Description("Get the current date and time in UTC")]
    public static string GetDateTime()
    {
        Console.WriteLine($"{foregroundYellow}Plugin Triggered: {nameof(TimePlugin)} - GetDateTime{foregroundReset}");

        var currentTime = DateTime.UtcNow.ToString("R");
        return currentTime;
    }
}