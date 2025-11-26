# Shared Python components for Tenstorrent demos

Python client and server utilities for Tenstorrent workflows, covering the Koyeb-hosted OpenAI-compatible API and a local Flask stub.

## Contents
- `__init__.py`: Exports the shared helpers for convenient imports.
- `koyeb_tenstorrent_client.py`: Lightweight wrapper around the OpenAI SDK configured for Tenstorrent Cloud services hosted on Koyeb.
- `koyeb_tenstorrent_server.py`: Flask app that exposes local OpenAI-compatible endpoints backed by Tenstorrent hardware stubs.

## Dependencies
- `requirements.txt`: Install with `pip install -r requirements.txt` to pull Flask and OpenAI SDK dependencies used by the client and server.

## Koyeb Tenstorrent client
The `KoyebTenstorrentClient` expects a Koyeb deployment that runs `tt-inference-server` and exposes OpenAI-compatible endpoints under `/v1`.

Configuration:
- `KOYEB_TT_BASE_URL`: Base URL for the Koyeb service (e.g., `https://<prefix>.koyeb.app`).
- `KOYEB_TT_API_KEY`: Bearer token or JWT for the service. If not set, the client defaults to the placeholder `fake` used in Koyeb quickstarts.

Example usage:

```python
from shared.python import KoyebTenstorrentClient

client = KoyebTenstorrentClient.from_environment(
    model="meta-llama/Llama-3.1-8B-Instruct",
)

response = client.create_chat_completion(
    messages=[{"role": "user", "content": "Tell me a joke."}],
    max_tokens=60,
)
print(response)
```

## Self-hosted Tenstorrent server (Flask)
`koyeb_tenstorrent_server.py` exposes `/v1/chat/completions`, `/v1/completions`, and `/v1/embeddings` using Flask. The stub implementations return placeholder values; swap in TT-NN / TT-Metalium calls to drive local hardware. Install Flask first (e.g., `pip install Flask`).

Example entrypoint:

```python
from shared.python import create_app

app = create_app(default_model="meta-llama/Llama-3.1-8B-Instruct")
app.run(host="0.0.0.0", port=8000, debug=True)
```

Endpoints mirror the OpenAI wire format so existing Koyeb clients can point at `http://localhost:8000/v1` during development.
