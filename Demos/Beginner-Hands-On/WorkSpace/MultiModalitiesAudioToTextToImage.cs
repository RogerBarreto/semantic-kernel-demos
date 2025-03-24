using Microsoft.SemanticKernel;

Console.WriteLine("=== Multi modalities audio -> text -> image services ===\n\n");

string folderPath = @"C:\Users\rbarreto\OneDrive - Microsoft\Documents\Sound Recordings";
DirectoryInfo directoryInfo = new(folderPath);

FileInfo? mostRecentFile = directoryInfo.GetFiles()
                                       .OrderByDescending(f => f.CreationTime)
                                       .FirstOrDefault();

Console.WriteLine($"Most recent recording: {mostRecentFile!.FullName}");

var audioContent = new AudioContent(File.ReadAllBytes(mostRecentFile!.FullName), "audio/m4a");

var audioToTextService = new OpenAIAudioToTextService(
    modelId: "whisper-1",
    apiKey: config["OpenAI:ApiKey"]!);

var textToImageService = new OpenAITextToImageService(
    modelId: "dall-e-3",
    apiKey: config["OpenAI:ApiKey"]!);

Console.WriteLine("\nGenerating text from audio ...\n");
var generatedTexts = await audioToTextService.GetTextContentsAsync(audioContent);
var generatedText = generatedTexts[0];

Console.WriteLine($"Text Generated: {generatedText}");

Console.WriteLine("\nGenerating image from text ...\n");
var generatedImages = await textToImageService.GetImageContentsAsync(generatedText);
var generatedImage = generatedImages[0];

Console.WriteLine("Image Generated. Ctrl + Click to view: " + generatedImage.Uri);