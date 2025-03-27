namespace Sample;

internal class PresidioAnonymizerConfig
{
    /// <summary>
    /// Enable or disable anonymizer.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Anonymizer rules to be applied to the prompt.
    /// </summary>
    public Dictionary<string, PresidioTextAnonymizer> Anonymizers { get; set; } = [];
}
