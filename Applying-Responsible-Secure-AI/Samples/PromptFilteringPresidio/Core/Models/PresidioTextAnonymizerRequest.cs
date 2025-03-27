using System.Text.Json.Serialization;

namespace Sample;

/// <summary>
/// Request model for Text Anonymizer.
/// Full schema can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Anonymizer/paths/~1anonymize/post
/// </summary>
internal sealed class PresidioTextAnonymizerRequest
{
    /// <summary>The text to anonymize.</summary>
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    /// <summary>Object where the key is DEFAULT or the ENTITY_TYPE and the value is the anonymizer definition.</summary>
    [JsonPropertyName("anonymizers")]
    public Dictionary<string, PresidioTextAnonymizer>? Anonymizers { get; set; }

    /// <summary>Array of analyzer detections.</summary>
    [JsonPropertyName("analyzer_results")]
    public List<PresidioTextAnalyzerResponse>? AnalyzerResults { get; set; }
}
