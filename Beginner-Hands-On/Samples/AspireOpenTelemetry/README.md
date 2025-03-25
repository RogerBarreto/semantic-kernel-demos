## Prerequisites

- .NET 8.0+
- OpenAI api key
- Docker

## Aspire Dashboard

For info: [Starting Aspire Dashboard](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/dashboard/standalone?tabs=bash#start-the-dashboard)

Before running this sample, start the Aspire dashboard

This command starts the dashboard with a specific browser token and destroys it when it exits.

```powershell
docker run --rm -it -d -p 18888:18888 -e DASHBOARD__FRONTEND__BROWSERTOKEN=2f917b9650cf62ef50dfab3bc5fccc29 -p 4317:18889 --name aspire-dashboard mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

## Setup

Follow the instructions in the [README.md](../../README.md) file to set up the environment.