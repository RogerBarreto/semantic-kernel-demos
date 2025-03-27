# Azure AI Content Safety and Prompt Shields service example

This sample provides a practical demonstration of how to leverage [Semantic Kernel Prompt Filters](https://devblogs.microsoft.com/semantic-kernel/filters-in-semantic-kernel/#prompt-render-filter) feature together with prompt verification services such as Azure AI Content Safety and Prompt Shields.

[Azure AI Content Safety](https://learn.microsoft.com/en-us/azure/ai-services/content-safety/overview) detects harmful user-generated and AI-generated content in applications and services. Azure AI Content Safety includes text and image APIs that allow to detect material that is harmful.

[Prompt Shields](https://learn.microsoft.com/en-us/azure/ai-services/content-safety/quickstart-jailbreak) service allows to check your large language model (LLM) inputs for both User Prompt and Document attacks.

Together with Semantic Kernel Prompt Filters, it's possible to define detection logic in dedicated place and avoid mixing it with business logic in applications.

## Prerequisites

1. [OpenAI](https://platform.openai.com/docs/introduction) subscription.
2. [Azure](https://azure.microsoft.com/free) subscription.
3. Once you have your Azure subscription, create a [Content Safety resource](https://aka.ms/acs-create) in the Azure portal to get your key and endpoint. Enter a unique name for your resource, select your subscription, and select a resource group, supported region (East US or West Europe), and supported pricing tier. Then select **Create**.
4. Update `appsettings.json/appsettings.Development.json` file with your configuration for `OpenAI` and `AzureContentSafety` sections or use .NET [Secret Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets):

```powershell
# Azure AI Content Safety
dotnet user-secrets set "AzureContentSafety:Endpoint" "... your endpoint ..."
dotnet user-secrets set "AzureContentSafety:ApiKey" "... your api key ... "

# OpenAI
dotnet user-secrets set "OpenAI:ApiKey" "... your api key ... "
```

## More information

- [What is Azure AI Content Safety?](https://learn.microsoft.com/en-us/azure/ai-services/content-safety/overview)
- [Analyze text content with Azure AI Content Safety](https://learn.microsoft.com/en-us/azure/ai-services/content-safety/quickstart-text)
- [Detect attacks with Azure AI Content Safety Prompt Shields](https://learn.microsoft.com/en-us/azure/ai-services/content-safety/quickstart-jailbreak)
