namespace Sample;

/// <summary>
/// Anonymizer action type that can be performed to update the prompt.
/// More information here: https://microsoft.github.io/presidio/api-docs/api-docs.html#tag/Anonymizer/paths/~1anonymizers/get
/// </summary>
internal readonly struct PresidioAnonymizerType(string name)
{
    public string Name { get; } = name;

    public static PresidioAnonymizerType Hash = new("hash");
    public static PresidioAnonymizerType Mask = new("mask");
    public static PresidioAnonymizerType Redact = new("redact");
    public static PresidioAnonymizerType Replace = new("replace");
    public static PresidioAnonymizerType Encrypt = new("encrypt");

    public static implicit operator string(PresidioAnonymizerType type) => type.Name;
}
