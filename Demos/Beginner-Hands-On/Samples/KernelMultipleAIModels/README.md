## Prerequisites

- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0).

## 1. Configuring the sample

## Setup Ollama

Follow instructions at:

https://ollama.com/blog/ollama-is-now-available-as-an-official-docker-image

Ensure you are using a valid path for you ollama to pull the models (i.e. `D:\temp\ollama`)
```powershell
docker run -d -e OLLAMA_KEEP_ALIVE=-1 -v D:\temp\ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
```

### Installing a Ollama Model 

Our demo uses some ollama models like `llama3.2` and `phi4` models, if you don't have them installed, you can install them using or use other models as you wish.

```powershell
docker exec -it ollama ollama pull llama3.2
docker exec -it ollama ollama pull phi4
```

## Setup ONNX

### Downloading the Model

1. Ensure you added the LFS (large file storage) extension to your git client to download the model files:
    ```powershell
    git lfs install
    ```

2. Clone the repo/downloading the model

    ```
    git clone https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx
    ```

3. Take note of the path where your repo is cloned to configure the sample.