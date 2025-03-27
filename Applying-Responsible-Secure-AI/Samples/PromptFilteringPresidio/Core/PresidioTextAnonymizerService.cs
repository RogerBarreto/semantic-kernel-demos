
using System.Text;
using System.Text.Json;

namespace Sample;

/// <summary>
/// Service which performs HTTP request to Text Anonymizer.
/// </summary>
internal sealed class PresidioTextAnonymizerService(HttpClient httpClient)
{
    private const string RequestUri = "anonymize";

    public async Task<PresidioTextAnonymizerResponse> AnonymizeAsync(PresidioTextAnonymizerRequest request)
    {
        var requestContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(new Uri(RequestUri, UriKind.Relative), requestContent);

        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<PresidioTextAnonymizerResponse>(responseContent) ??
            throw new Exception("Anonymizer response is not available.");
    }
}