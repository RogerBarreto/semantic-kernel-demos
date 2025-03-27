using System.Text.Json.Serialization;

namespace Sample;

/// <summary>
/// Anonymizer model that describes how to update the prompt.
/// </summary>
internal sealed class PresidioTextAnonymizer
{
    /// <summary>Anonymizer action type that can be performed to update the prompt.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>New value for "replace" anonymizer type.</summary>
    [JsonPropertyName("new_value")]
    public string? NewValue { get; set; }
}
