using System.Text.Json.Serialization;

namespace Sample;

/// <summary>
/// Request model for Text Analyzer. Only required properties are defined here for demonstration purposes.
/// Full schema can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Analyzer/paths/~1analyze/post.
/// </summary>
internal sealed class PresidioTextAnalyzerRequest
    {
        /// <summary>The text to analyze.</summary>
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        /// <summary>Two characters for the desired language in ISO_639-1 format.</summary>
        [JsonPropertyName("language")]
        public string? Language { get; set; } = "en";
    }
