using System.Text.Json.Serialization;

namespace Sample;

/// <summary>
/// Response model from Text Analyzer. Only required properties are defined here for demonstration purposes.
/// Full schema can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Analyzer/paths/~1analyze/post.
/// </summary>
internal sealed class PresidioTextAnalyzerResponse
    {
        /// <summary>Where the PII starts.</summary>
        [JsonPropertyName("start")]
        public int? Start { get; set; }

        /// <summary>Where the PII ends.</summary>
        [JsonPropertyName("end")]
        public int? End { get; set; }

        /// <summary>The PII detection confidence score from 0 to 1.</summary>
        [JsonPropertyName("score")]
        public double? Score { get; set; }

        /// <summary>The supported PII entity types.</summary>
        [JsonPropertyName("entity_type")]
        public string? EntityType { get; set; }
    }