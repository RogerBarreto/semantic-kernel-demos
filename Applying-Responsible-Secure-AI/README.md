# Applying Responsible/Secure AI with Semantic Kernel

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