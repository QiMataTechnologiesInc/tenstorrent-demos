# Shared Java components for Tenstorrent demos

Java client and server utilities for Tenstorrent workflows, covering the Koyeb-hosted OpenAI-compatible API and a local JDK HTTP server stub.

## Contents
- `KoyebTenstorrentClient.java`: HTTP client for Tenstorrent Cloud services on Koyeb.
- `KoyebTenstorrentServer.java`: JDK `HttpServer` exposing local OpenAI-compatible endpoints with Tenstorrent stubs.
- `pom.xml`: Maven build configuration targeting Java 17.

## Koyeb Tenstorrent client
The client posts to the OpenAI-compatible `/v1` routes exposed by `tt-inference-server` in a Koyeb deployment.

Environment variables:
- `KOYEB_TT_BASE_URL`: Base URL for the Koyeb app (e.g., `https://<prefix>.koyeb.app`).
- `KOYEB_TT_API_KEY`: Bearer token or JWT; defaults to `fake` if unset.

Example:
```java
import com.tenstorrent.shared.KoyebTenstorrentClient;
import com.tenstorrent.shared.KoyebTenstorrentClient.ChatMessage;

var client = KoyebTenstorrentClient.fromEnvironment("meta-llama/Llama-3.1-8B-Instruct");
var response = client.createChatCompletion(
    List.of(new ChatMessage("user", "Tell me a joke.")),
    60,
    null,
    false
);
System.out.println(response.body());
```

## Self-hosted Tenstorrent server (JDK HTTP server)
`KoyebTenstorrentServer` uses the built-in `com.sun.net.httpserver.HttpServer` to expose `/v1/chat/completions`, `/v1/completions`, and `/v1/embeddings`. The handlers currently return stub data; replace the `TenstorrentLocalInference` methods with calls into Tenstorrent libraries on your hardware.

To start the stub server on port `8000`:

```bash
export TT_SERVER_PORT=8000
java com.tenstorrent.shared.KoyebTenstorrentServer
```

Point your OpenAI/Koyeb client at `http://localhost:8000/v1` to validate request/response wiring.
