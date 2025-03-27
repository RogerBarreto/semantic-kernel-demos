using System.Text.Json.Serialization;

namespace Sample;

/// <summary>
/// Response model for Text Anonymizer.
/// Full schema can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Anonymizer/paths/~1anonymize/post
/// </summary>
internal sealed class PresidioTextAnonymizerResponse
{
    /// <summary>The new text returned.</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>Array of anonymized entities.</summary>
    [JsonPropertyName("items")]
    public List<PresidioTextAnonymizerResponseItem>? Items { get; set; }
}
