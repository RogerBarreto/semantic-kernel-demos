namespace Sample;

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
