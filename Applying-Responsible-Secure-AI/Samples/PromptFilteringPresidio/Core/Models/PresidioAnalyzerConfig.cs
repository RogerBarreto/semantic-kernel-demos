namespace Sample;

internal class PresidioAnalyzerConfig
{
    /// <summary>
    /// Enable or disable analyzer.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Threshold for score to consider PII detected.
    /// </summary>
    public double? ScoreThreshold { get; set; }
}
