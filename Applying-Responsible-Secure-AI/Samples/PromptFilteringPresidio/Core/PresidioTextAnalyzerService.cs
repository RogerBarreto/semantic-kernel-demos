using System.Text;
using System.Text.Json;

namespace Sample;

/// <summary>
/// Service which performs HTTP request to Text Analyzer.
/// </summary>
internal sealed class PresidioTextAnalyzerService(HttpClient httpClient)
{
    private const string RequestUri = "analyze";

    public async Task<List<PresidioTextAnalyzerResponse>> AnalyzeAsync(PresidioTextAnalyzerRequest request)
    {
        var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(new Uri(RequestUri, UriKind.Relative), requestContent);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<List<PresidioTextAnalyzerResponse>>(responseContent) ??
            throw new Exception("Analyzer response is not available.");
    }
}
