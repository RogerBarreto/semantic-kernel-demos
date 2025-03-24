using System.ComponentModel;
using Microsoft.SemanticKernel;
using Sample;

#pragma warning disable SKEXP0070 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

Console.WriteLine("=== Grounding Prompts with Stateful and Stateless Plugins ===\n");

var myLightBulbPlugin = new LightBulbPlugin(isOn: true);

var modelId = "llama3.2";
var endpoint = new Uri("http://localhost:11434");
var kernel = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(modelId, endpoint)
    .Build();

kernel.Plugins.AddFromObject(myLightBulbPlugin);
kernel.Plugins.AddFromType<TimePlugin>();

var arguments = new KernelArguments
{
    ["name"] = "Roger"
};

Console.WriteLine("Take 1: ");
await foreach (var token in kernel.InvokePromptStreamingAsync("\"At {{GetDateTime}}: {{$name}} looked at the light bulb and see that the light is {{GetStatus}} and then {{Switch}} it.\", Repeat the provided sentence.", arguments))
{
    Console.Write(token);
}

Console.WriteLine("\n----");

Console.WriteLine("Take 2: ");
await foreach (var token in kernel.InvokePromptStreamingAsync("\"At {{GetDateTime}}: {{$name}} looked at the light bulb and see that the light is {{GetStatus}}.\", Repeat the provided sentence.", arguments))
{
    Console.Write(token);
}
