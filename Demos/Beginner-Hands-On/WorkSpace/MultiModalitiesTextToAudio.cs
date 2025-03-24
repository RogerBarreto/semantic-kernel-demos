using Microsoft.SemanticKernel;
using System.Text;

Console.WriteLine("=== Multi modalities text -> audio services ===\n\n");

var chatService = new Azure.AI.Inference.ChatCompletionsClient(
    endpoint: new Uri(config["AzureAIInference:Endpoint"]!),
    credential: new Azure.AzureKeyCredential(config["AzureAIInference:ApiKey"]!))
    .AsChatClient("phi3")
    .AsChatCompletionService();

var textToAudioService = new OpenAITextToAudioService(
    modelId: "tts-1", 
    apiKey: config["OpenAI:ApiKey"]!);

StringBuilder answer = new();
var prompt = "Explain in a simple phrase why Jupiter has storms.";
Console.WriteLine($"{prompt}\nGenerating response...\n");
await foreach (var token in chatService.GetStreamingChatMessageContentsAsync(prompt))
{
    Console.Write(token);
    answer.Append(token);
}

Console.WriteLine("\nGenerating audio...\n");
var generatedAudios = await textToAudioService.GetAudioContentsAsync(answer.ToString());
var generatedAudio = generatedAudios[0];

var file = Path.Combine(Directory.GetCurrentDirectory(), "output.mp3");
await File.WriteAllBytesAsync(file, generatedAudio.Data!.Value);

Console.WriteLine($"\nAudio Generated. Ctrl + Click to listen: {new Uri(file).AbsoluteUri}");