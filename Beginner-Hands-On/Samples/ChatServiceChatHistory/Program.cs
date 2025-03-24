using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Set the ApiKey
// dotnet user-secrets set "OpenAI:ApiKey" "your-api-key"

Console.WriteLine("=== Service Streaming with Chat History ===");

var apiKey = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["OpenAI:ApiKey"]!;
var modelId = "o1-mini";
var service = new OpenAIChatCompletionService(modelId, apiKey);

ChatHistory chatHistory = [
    new ChatMessageContent(AuthorRole.System, "You are presenting in a conference"),
    new ChatMessageContent(AuthorRole.User, "Why is Neptune blue?")
];

await foreach (var token in service.GetStreamingChatMessageContentsAsync(chatHistory))
{
    Console.Write(token);
}