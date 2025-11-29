# Shared JavaScript components for Tenstorrent demos

JavaScript helpers for Tenstorrent workflows using the built-in `fetch` API against the Koyeb-hosted OpenAI-compatible endpoints.

## Contents
- `koyebTenstorrentClient.js`: Fetch-based client for Tenstorrent Cloud services hosted on Koyeb.
- `koyebTenstorrentServer.js`: Lightweight HTTP server that mirrors the OpenAI-compatible Koyeb endpoints and selects Wormhole + Blackhole cards when supported.
- `package.json`: Module metadata for consuming the client as a library (ESM).

## Installation
- From npm (if published): `npm install @tenstorrent/shared-js`
- From a git checkout or submodule:
  ```bash
  git submodule add https://github.com/tenstorrent/tenstorrent-demos external/tenstorrent-demos
  npm install ./external/tenstorrent-demos/src/shared/javascript
  ```

## Koyeb Tenstorrent client
Targets the OpenAI-compatible `/v1` endpoints exposed by `tt-inference-server` in a Koyeb deployment.

Environment variables:
- `KOYEB_TT_BASE_URL`: Base URL for the Koyeb app (e.g., `https://<prefix>.koyeb.app`).
- `KOYEB_TT_API_KEY`: Bearer token or JWT; defaults to `fake` if unset.

Example (ESM):
```javascript
import { KoyebTenstorrentClient } from "@tenstorrent/shared-js";
// or: import { KoyebTenstorrentClient } from "./koyebTenstorrentClient.js";

const client = KoyebTenstorrentClient.fromEnvironment(
  "meta-llama/Llama-3.1-8B-Instruct"
);

const response = await client.createChatCompletion([
  { role: "user", content: "Tell me a joke." },
]);
console.log(await response.json());
```

## Stub Tenstorrent server

`koyebTenstorrentServer.js` exposes a simple HTTP server mirroring the `/v1` OpenAI endpoints. By default it drives both a Wormhole and Blackhole card when the requested model appears in `TT_WORMHOLE_SUPPORTED_MODELS` (comma-separated, case-insensitive) or matches the built-in defaults. Otherwise it falls back to the Blackhole.

```javascript
import { startTenstorrentServer } from "@tenstorrent/shared-js/server";

// Starts on TT_SERVER_PORT or 8000 by default
startTenstorrentServer();
```
