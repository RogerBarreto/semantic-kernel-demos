namespace Sample;

/// <summary>
/// PII entities Presidio Text Analyzer is capable of detecting. Only some of them are defined here for demonstration purposes.
/// Full list can be found here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Analyzer/paths/~1supportedentities/get.
/// </summary>
internal readonly struct PresidioAnalyzerEntityType(string name)
    {
        public string Name { get; } = name;

        public static PresidioAnalyzerEntityType Person = new("PERSON");
        public static PresidioAnalyzerEntityType PhoneNumber = new("PHONE_NUMBER");
        public static PresidioAnalyzerEntityType EmailAddress = new("EMAIL_ADDRESS");
        public static PresidioAnalyzerEntityType CreditCard = new("CREDIT_CARD");

        public static implicit operator string(PresidioAnalyzerEntityType type) => type.Name;
    }
