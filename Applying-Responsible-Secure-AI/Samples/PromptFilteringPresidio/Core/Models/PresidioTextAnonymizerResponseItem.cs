using System.Text.Json.Serialization;

namespace Sample;

/// <summary>
/// Response item model for Text Anonymizer.
/// Full schema can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Anonymizer/paths/~1anonymize/post
/// </summary>
internal sealed class PresidioTextAnonymizerResponseItem
{
    /// <summary>Name of the used operator.</summary>
    [JsonPropertyName("operator")]
    public string? Operator { get; set; }

    /// <summary>Type of the PII entity.</summary>
    [JsonPropertyName("entity_type")]
    public string? EntityType { get; set; }

    /// <summary>Start index of the changed text.</summary>
    [JsonPropertyName("start")]
    public int? Start { get; set; }

    /// <summary>End index in the changed text.</summary>
    [JsonPropertyName("end")]
    public int? End { get; set; }
}
