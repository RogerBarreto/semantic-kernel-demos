using System.Text.Json.Serialization;

namespace Sample;

/// <summary>
/// Anonymizer action type that can be performed to update the prompt.
/// More information here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Anonymizer/paths/~1anonymizers/get
/// </summary>
internal readonly struct AnonymizerType(string name)
{
    public string Name { get; } = name;

    public static AnonymizerType Hash = new("hash");
    public static AnonymizerType Mask = new("mask");
    public static AnonymizerType Redact = new("redact");
    public static AnonymizerType Replace = new("replace");
    public static AnonymizerType Encrypt = new("encrypt");

    public static implicit operator string(AnonymizerType type) => type.Name;
}

/// <summary>
/// Anonymizer model that describes how to update the prompt.
/// </summary>
internal sealed class PresidioTextAnonymizer
{
    /// <summary>Anonymizer action type that can be performed to update the prompt.</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>New value for "replace" anonymizer type.</summary>
    [JsonPropertyName("new_value")]
    public string NewValue { get; set; }
}

/// <summary>
/// Request model for Text Anonymizer.
/// Full schema can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Anonymizer/paths/~1anonymize/post
/// </summary>
internal sealed class PresidioTextAnonymizerRequest
{
    /// <summary>The text to anonymize.</summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>Object where the key is DEFAULT or the ENTITY_TYPE and the value is the anonymizer definition.</summary>
    [JsonPropertyName("anonymizers")]
    public Dictionary<string, PresidioTextAnonymizer> Anonymizers { get; set; }

    /// <summary>Array of analyzer detections.</summary>
    [JsonPropertyName("analyzer_results")]
    public List<PresidioTextAnalyzerResponse> AnalyzerResults { get; set; }
}

/// <summary>
/// Response item model for Text Anonymizer.
/// Full schema can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Anonymizer/paths/~1anonymize/post
/// </summary>
internal sealed class PresidioTextAnonymizerResponseItem
{
    /// <summary>Name of the used operator.</summary>
    [JsonPropertyName("operator")]
    public string Operator { get; set; }

    /// <summary>Type of the PII entity.</summary>
    [JsonPropertyName("entity_type")]
    public string EntityType { get; set; }

    /// <summary>Start index of the changed text.</summary>
    [JsonPropertyName("start")]
    public int Start { get; set; }

    /// <summary>End index in the changed text.</summary>
    [JsonPropertyName("end")]
    public int End { get; set; }
}

/// <summary>
/// Response model for Text Anonymizer.
/// Full schema can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Anonymizer/paths/~1anonymize/post
/// </summary>
internal sealed class PresidioTextAnonymizerResponse
{
    /// <summary>The new text returned.</summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>Array of anonymized entities.</summary>
    [JsonPropertyName("items")]
    public List<PresidioTextAnonymizerResponseItem> Items { get; set; }
}

    /// <summary>
    /// PII entities Presidio Text Analyzer is capable of detecting. Only some of them are defined here for demonstration purposes.
    /// Full list can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Analyzer/paths/~1supportedentities/get.
    /// </summary>
    internal readonly struct AnalyzerEntityType(string name)
    {
        public string Name { get; } = name;

        public static AnalyzerEntityType Person = new("PERSON");
        public static AnalyzerEntityType PhoneNumber = new("PHONE_NUMBER");
        public static AnalyzerEntityType EmailAddress = new("EMAIL_ADDRESS");
        public static AnalyzerEntityType CreditCard = new("CREDIT_CARD");

        public static implicit operator string(AnalyzerEntityType type) => type.Name;
    }

    /// <summary>
    /// Request model for Text Analyzer. Only required properties are defined here for demonstration purposes.
    /// Full schema can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Analyzer/paths/~1analyze/post.
    /// </summary>
    internal sealed class PresidioTextAnalyzerRequest
    {
        /// <summary>The text to analyze.</summary>
        [JsonPropertyName("text")]
        public string Text { get; set; }

        /// <summary>Two characters for the desired language in ISO_639-1 format.</summary>
        [JsonPropertyName("language")]
        public string Language { get; set; } = "en";
    }

    /// <summary>
    /// Response model from Text Analyzer. Only required properties are defined here for demonstration purposes.
    /// Full schema can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Analyzer/paths/~1analyze/post.
    /// </summary>
    internal sealed class PresidioTextAnalyzerResponse
    {
        /// <summary>Where the PII starts.</summary>
        [JsonPropertyName("start")]
        public int Start { get; set; }

        /// <summary>Where the PII ends.</summary>
        [JsonPropertyName("end")]
        public int End { get; set; }

        /// <summary>The PII detection confidence score from 0 to 1.</summary>
        [JsonPropertyName("score")]
        public double Score { get; set; }

        /// <summary>The supported PII entity types.</summary>
        [JsonPropertyName("entity_type")]
        public string EntityType { get; set; }
    }