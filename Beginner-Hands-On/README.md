# Beginner Hands-On Semantic Kernel Demos

## Setup OpenAI

Some of the samples use OpenAI models, ensure you have an OpenAI API key.

### User Secrets Manager

For safely storing secrets, we use the user secret manager. This is a .NET Core feature that allows you to store sensitive information like API keys in a secure way.

> [!NOTE]
> All samples share the same secret store, so you only need to set it for the first sample and it will be available for all the others.

Execute the following commands in your terminal to set the OpenAI API key secret:

```powershell
cd <repo-root>\Beginner-Hands-On\Samples\KernelPrompting
dotnet user-secrets set "OpenAI:ApiKey" "YOUR_OPENAI_API_KEY"
```

## Setup Ollama

### Using Docker

Follow instructions at:

https://ollama.com/blog/ollama-is-now-available-as-an-official-docker-image

Ensure you select a valid path for you ollama to download the AI models (i.e. `C:\temp\ollama`)

```powershell
docker run -d -e OLLAMA_KEEP_ALIVE=-1 -v C:\temp\ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
```

### Using Windows

Download and follow instructions at:

https://ollama.com/download

### Installing Ollama Models 

Our demo (not limited to) uses some ollama models like `llama3.2` and `phi4` 

All available ollama models are available here:

https://ollama.com/models

> [!NOTE]
> Samples that use `llama3.2` expect the model to support `tools` feature, when using another model, ensure it has [tools feature](https://ollama.com/search?c=tools) enabled.

### Using Docker

Execute the following commands in your terminal:

```powershell
docker exec -it ollama ollama pull llama3.2
docker exec -it ollama ollama pull phi4
```

### Using Windows

Execute the following command in your terminal:

```powershell
ollama pull llama3.2
ollama pull phi4
```

## Setup ONNX

### Installing ONNX Models

1. Ensure you added the LFS (large file storage) extension to your git client to download the large model files:
    ```powershell
    git lfs install
    ```

2. Clone the repo/downloading the model

    ```
    git clone https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx
    ```

3. Save the path where your <u>repo was cloned</u> to update the sample.
