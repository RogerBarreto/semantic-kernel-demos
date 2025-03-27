using System.ComponentModel.DataAnnotations;

namespace ContentSafety.Options;

/// <summary>
/// Configuration for Azure AI Content Safety service.
/// </summary>
public class AzureContentSafetyOptions
{
    public const string SectionName = "AzureContentSafety";

    [Required]
    public Uri? Endpoint { get; set; }

    [Required]
    public string? ApiKey { get; set; }
}
