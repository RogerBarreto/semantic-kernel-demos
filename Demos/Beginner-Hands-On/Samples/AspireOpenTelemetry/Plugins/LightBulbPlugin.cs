using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Sample;

[Description("This is a light bulb")]
public class LightBulbPlugin(bool isOn = false)
{
    private bool _isOn = isOn;
    [KernelFunction, Description("Turns the light bulb on")]
    public void TurnOn()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Plugin Triggered > {nameof(LightBulbPlugin)} - TurnOn");
        this._isOn = true;
    }
    [KernelFunction, Description("Turns the light bulb off")]
    public void TurnOff()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Plugin Triggered > {nameof(LightBulbPlugin)} - TurnOff");
        this._isOn = false;
    }
    [KernelFunction, Description("Checks if the light bulb is on")]
    public bool IsOn()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Plugin Triggered > {nameof(LightBulbPlugin)} - IsOn");
        return this._isOn;
    }
}