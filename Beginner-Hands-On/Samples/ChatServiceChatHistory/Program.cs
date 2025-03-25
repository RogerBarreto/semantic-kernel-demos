using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

Console.WriteLine("=== Chat Service Streaming with Chat History ===\n");

var apiKey = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["OpenAI:ApiKey"]!;
var modelId = "o1-mini";
var chatService = new OpenAIChatCompletionService(modelId, apiKey);

var userPrompt = "Why is Neptune blue?";

ChatHistory chatHistory = [];
chatHistory.AddSystemMessage("You are presenting in a conference");
chatHistory.AddUserMessage(userPrompt);

Console.WriteLine($"User > {userPrompt}");
Console.Write("Assistant > ");
await foreach (var token in chatService.GetStreamingChatMessageContentsAsync(chatHistory))
{
    Console.Write(token);
}
