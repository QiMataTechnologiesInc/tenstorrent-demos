# Shared .NET components for Tenstorrent demos

C# client and server utilities for Tenstorrent workflows, covering the Koyeb-hosted OpenAI-compatible API and a local minimal API stub.

## Contents
- `KoyebTenstorrentClient.cs`: Minimal HttpClient-based wrapper for Tenstorrent Cloud services.
- `KoyebTenstorrentServer.cs`: Minimal API server exposing the OpenAI-style endpoints locally.
- `Tenstorrent.Shared.csproj`: Project file referencing ASP.NET Core dependencies.

## Koyeb Tenstorrent client
The client targets the OpenAI-compatible `/v1` routes exposed by a Koyeb deployment running `tt-inference-server`.

Environment variables:
- `KOYEB_TT_BASE_URL`: Base URL for the Koyeb app (e.g., `https://<prefix>.koyeb.app`).
- `KOYEB_TT_API_KEY`: Bearer token or JWT for the service. Defaults to `fake` if unset to match the quickstart samples.

Example:
```csharp
using System.Net.Http;
using Tenstorrent.Shared;

var client = KoyebTenstorrentClient.FromEnvironment(
    model: "meta-llama/Llama-3.1-8B-Instruct"
);

var response = await client.CreateChatCompletionAsync(
    new [] { new Dictionary<string, string> { ["role"] = "user", ["content"] = "Tell me a joke." } },
    maxTokens: 60
);

var content = await response.Content.ReadAsStringAsync();
Console.WriteLine(content);
```

## Self-hosted Tenstorrent server (Minimal APIs)
`KoyebTenstorrentServer` wires `/v1/chat/completions`, `/v1/completions`, and `/v1/embeddings` using ASP.NET Core minimal APIs. The handlers return stub payloads until you connect them to TT-NN / TT-Metalium bindings.

To run:

```csharp
using Tenstorrent.Shared;

var app = KoyebTenstorrentServer.Build(args);
app.Run("http://0.0.0.0:8000");
```

Point existing OpenAI-compatible clients at `http://localhost:8000/v1` to exercise the wire format.
