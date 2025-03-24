using Microsoft.SemanticKernel;

Console.WriteLine("=== Multi modalities image -> text -> audio ===\n\n");

string folderPath = @"C:\Users\rbarreto\OneDrive - Microsoft\Pictures\Camera Roll";
string huggingFaceModel = "Salesforce/blip-image-captioning-large";
string openAIModel = "gpt-4o-mini";

DirectoryInfo directoryInfo = new(folderPath);
FileInfo? mostRecentFile = directoryInfo.GetFiles("*.jpg")
                                       .OrderByDescending(f => f.CreationTime)
                                       .FirstOrDefault();

Console.WriteLine($"Most recent photo: {mostRecentFile!.FullName}");

var imageContent = new ImageContent(File.ReadAllBytes(mostRecentFile!.FullName), "image/jpeg");

var huggingFaceImageToTextService = new HuggingFaceImageToTextService(huggingFaceModel, apiKey: config["HuggingFace:ApiKey"]!);
var openAiChatService = new OpenAIChatCompletionService(modelId: openAIModel, apiKey: config["OpenAI:ApiKey"]!);

Console.WriteLine("\nGenerating text from image (Hugging face) ...\n");
var huggingFaceImageText = (await huggingFaceImageToTextService.GetTextContentsAsync(imageContent))[0];
Console.WriteLine($"Hugging Face Text Generated:\n{huggingFaceImageText}\n");

Console.WriteLine("\nGenerating text from image (OpenAI)...\n");
ChatMessageContent chatMessageContent = new(AuthorRole.User, [
    new TextContent("Describe the image"),
    imageContent
]);
var openAIImageText = await openAiChatService.GetChatMessageContentAsync([chatMessageContent]);
Console.WriteLine($"OpenAI Text Generated:\n{openAIImageText}\n");

Console.WriteLine("\nGenerating audio from text ...\n");
var textToAudioService = new OpenAITextToAudioService(
    modelId: "tts-1",
    apiKey: config["OpenAI:ApiKey"]!);

var huggingFaceGeneratedAudio = (await textToAudioService.GetAudioContentsAsync(huggingFaceImageText.ToString()))[0];
var file = Path.Combine(Directory.GetCurrentDirectory(), "image-description-output.mp3");
await File.WriteAllBytesAsync(file, huggingFaceGeneratedAudio.Data!.Value);

var openAIGeneratedAudio = (await textToAudioService.GetAudioContentsAsync(openAIImageText.ToString()))[0];
var file2 = Path.Combine(Directory.GetCurrentDirectory(), "image-description-output-2.mp3");
await File.WriteAllBytesAsync(file2, openAIGeneratedAudio.Data!.Value);

Console.WriteLine($"\nHuggingFace Audio Description. Ctrl + Click to listen: \n{new Uri(file).AbsoluteUri}");
Console.WriteLine($"\nOpenAI Audio Description. Ctrl + Click to listen: \n{new Uri(file2).AbsoluteUri}");