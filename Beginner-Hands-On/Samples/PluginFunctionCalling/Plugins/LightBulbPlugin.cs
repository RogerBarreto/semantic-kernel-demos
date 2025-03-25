using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace Sample;

/// <summary>
/// Stateful plugin (It keeps the current bulb state)
/// </summary>
/// <param name="isOn">At the initialization, defines if the bulb is on or off</param>
public class LightBulbPlugin(bool isOn = false)
{
    [KernelFunction, Description("Checks if the light bulb status is on or off")]
    public string GetStatus()
    {
        Console.WriteLine($"{foregroundYellow}Plugin Triggered: {nameof(LightBulbPlugin)} - GetStatus{foregroundReset}");

        return this._isOn ? "on" : "off";
    }

    [KernelFunction, Description("Switches the light bulb status")]
    public string Switch()
    {
        Console.WriteLine($"{foregroundYellow}Plugin Triggered: {nameof(LightBulbPlugin)} - Switch{foregroundReset}");

        this._isOn = !this._isOn;
        return "switches";
    }

    const string foregroundYellow = "\u001b[93m";
    const string foregroundReset = "\u001b[0m";

    private bool _isOn = isOn;
}