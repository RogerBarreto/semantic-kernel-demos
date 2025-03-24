# Beginner Hands-On Semantic Kernel Demos

## Setup Ollama with Docker

Follow instructions at:

https://ollama.com/blog/ollama-is-now-available-as-an-official-docker-image

```powershell
docker run -d -e OLLAMA_KEEP_ALIVE=-1 -v D:\temp\ollama:/root/.ollama -p 11434:11434 --name ollama ollama/ollama
```

### Installing a Ollama Model 

Our demo uses some ollama models like `llama3.2` and `phi4` models, if you don't have them installed, you can install them with:

```powershell
docker exec -it ollama ollama pull modelname
```

## Setup Ollama in Windows

Download and follow instructions at:

https://ollama.com/download

### Installing a Ollama Model 

Our demo uses some ollama models like `llama3.2` and `phi4` models, if you don't have them installed, you can install them from the command line:

```powershell
ollama pull modelname
```

