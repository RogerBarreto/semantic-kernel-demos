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
