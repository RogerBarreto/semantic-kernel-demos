using Microsoft.SemanticKernel;
using Sample;

Console.WriteLine("=== Grounding Prompts with Stateful and Stateless Plugins ===\n");

var modelId = "llama3.2";
var endpoint = new Uri("http://localhost:11434");
var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId, endpoint)
    .Build();

var myLightBulbPlugin = new LightBulbPlugin(isOn: true);
kernel.Plugins.AddFromObject(myLightBulbPlugin);
kernel.Plugins.AddFromType<TimePlugin>();

var arguments = new KernelArguments
{
    ["name"] = "Roger"
};

var prompt = """
    "At {{GetDateTime}}: {{$name}} looked at the light bulb and see that the light is {{GetStatus}} and then {{Switch}} it.", Repeat the provided sentence.
    """;
Console.WriteLine("Take 1: ");
await foreach (var token in kernel.InvokePromptStreamingAsync(prompt, arguments))
{
    Console.Write(token);
}

Console.WriteLine("\n----");

prompt = """
    "At {{GetDateTime}}: {{$name}} looked at the light bulb and see that the light is {{GetStatus}}.", Repeat the provided sentence.
    """;
Console.WriteLine("Take 2: ");
await foreach (var token in kernel.InvokePromptStreamingAsync(prompt, arguments))
{
    Console.Write(token);
}

Console.ReadLine();