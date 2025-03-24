using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AudioToText;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

#pragma warning disable SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

var apiKey = new ConfigurationBuilder().AddUserSecrets<Program>().Build()["OpenAI:ApiKey"]!;

// Used for audio speech transcription
var audioToTextService = new OpenAIAudioToTextService("whisper-1", apiKey);

// Used for image generation
var textToImageService = new OpenAITextToImageService(modelId: "dall-e-3", apiKey: apiKey);

// Used for image description
var chatService = new OpenAIChatCompletionService("gpt-4o-mini", apiKey);

// Used for audio speech generation
var textToAudioService = new OpenAITextToAudioService("tts-1", apiKey);
var currentDirectory = Directory.GetCurrentDirectory();

Console.WriteLine("""
=== Multi Modality Sample === 
Pipeline:
1. Load the input audio request
2. Transcribe the audio speech into text
3. Use the transcription as the prompt to generate an image
4. Capture a detailed text description from the generated image
5. Generate and audio speech from the detailed image description

Starting ...
""");

Console.Write($"\n1. Loading audio file ... ");
var audioContent = new AudioContent(File.ReadAllBytes("Audios/input.m4a"), "audio/m4a");
#pragma warning restore SKEXP0001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
Console.WriteLine("Done");

Console.Write("\n2. Transcribing audio speech into text ... ");
var audioTranscriptionText = await audioToTextService.GetTextContentAsync(audioContent);
Console.WriteLine("Done\n");

Console.WriteLine($"Transcription: {audioTranscriptionText}");

Console.WriteLine("\n3. Generating image from transcription ... ");
var generatedImage = (await textToImageService.GetImageContentsAsync(audioTranscriptionText, new OpenAITextToImageExecutionSettings { ResponseFormat = "bytes" })).First();
Console.Write("Done\n");

ChatHistory chatHistory = [];
var imageContent = new ImageContent(generatedImage.Data!.Value, "image/jpeg");

// Save the image to disk
var outputImagePath = "Images/output.jpg";
if (!Directory.Exists("Images")) { Directory.CreateDirectory("Images"); }
if (File.Exists(outputImagePath)) { File.Delete(outputImagePath); }
await File.WriteAllBytesAsync(outputImagePath, generatedImage.Data!.Value.ToArray());

Console.WriteLine($"Image Generated. Ctrl + Click to view: {new Uri(Path.Combine(currentDirectory, outputImagePath)).AbsoluteUri}");

chatHistory.AddUserMessage(
    [
        new TextContent("Describe the image in detail"), 
        imageContent
    ]);

Console.Write("\n4. Generating image description ... ");
var imageDescriptionText = await chatService.GetChatMessageContentAsync(chatHistory);
Console.Write("Done\n");

Console.WriteLine($"Image Description: {imageDescriptionText}");

Console.Write("\n5. Generating audio speech from image description ... ");
var generatedAudio = (await textToAudioService.GetAudioContentsAsync(imageDescriptionText.ToString())).First();
Console.Write("Done\n");

// Save the audio to disk
var outputAudioPath = "Audios/output.mp3";
if (File.Exists(outputAudioPath)) { File.Delete(outputAudioPath); }
await File.WriteAllBytesAsync(outputAudioPath, generatedAudio.Data!.Value.ToArray());

Console.WriteLine($"Audio speech from image description: (Ctrl + Click to view) {new Uri(Path.Combine(currentDirectory, outputAudioPath)).AbsoluteUri}");