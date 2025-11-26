# Shared C++ components for Tenstorrent demos

C++ client and server utilities for Tenstorrent workflows, using `cpr` for the Koyeb client and Crow for the local stub server.

## Contents
- `client/koyeb_tenstorrent_client.hpp`: CPR-based client for Koyeb-hosted Tenstorrent Cloud services.
- `server/koyeb_tenstorrent_server.cpp`: Crow-based server exposing OpenAI-style routes locally with Tenstorrent stubs.
- `client/CMakeLists.txt`: Standalone CMake target for the client interface library (pulls in CPR).
- `server/CMakeLists.txt`: Standalone CMake target for the Crow stub server.

## Using as a submodule
1. Add this repository as a submodule, e.g. `git submodule add https://github.com/tenstorrent/tenstorrent-demos external/tenstorrent-demos`.
2. In your root `CMakeLists.txt`, add the desired subdirectories:
   ```cmake
   add_subdirectory(external/tenstorrent-demos/src/shared/cpp/client)
   add_subdirectory(external/tenstorrent-demos/src/shared/cpp/server) # optional if you only need the client
   ```
3. Link against the interface library or server executable:
   ```cmake
   target_link_libraries(your_target PRIVATE koyeb_tenstorrent_client)
   # or build the server
   add_dependencies(your_target koyeb_tenstorrent_server)
   ```

If you prefer to build the modules in isolation:
```bash
cmake -S src/shared/cpp/client -B build/cpp-client
cmake --build build/cpp-client

cmake -S src/shared/cpp/server -B build/cpp-server
cmake --build build/cpp-server
```

## Koyeb Tenstorrent client
Targets the OpenAI-compatible `/v1` endpoints served by `tt-inference-server` in a Koyeb deployment.

Environment variables:
- `KOYEB_TT_BASE_URL`: Base URL for the Koyeb app (e.g., `https://<prefix>.koyeb.app`).
- `KOYEB_TT_API_KEY`: Bearer token or JWT; defaults to `fake` when unset to match sample deployments.

Example:
```cpp
#include "koyeb_tenstorrent_client.hpp"
#include <iostream>

int main() {
    auto client = tenstorrent::shared::KoyebTenstorrentClient::from_environment(
        "meta-llama/Llama-3.1-8B-Instruct"
    );

    std::vector<tenstorrent::shared::ChatMessage> messages{{"user", "Tell me a joke."}};
    auto response = client.create_chat_completion(messages, 60);
    std::cout << response.text << std::endl;
    return 0;
}
```

## Self-hosted Tenstorrent server (Crow)
`server/koyeb_tenstorrent_server.cpp` uses Crow to implement `/v1/chat/completions`, `/v1/completions`, and `/v1/embeddings`. Each handler currently returns stub data; replace the `run_tenstorrent_*` helpers with calls into Tenstorrent libraries or bindings when available.

To run the stub server through CMake (with Crow available):
```bash
cmake -S src/shared/cpp/server -B build/cpp-server
cmake --build build/cpp-server
./build/cpp-server/koyeb_tenstorrent_server
```

Clients can target `http://localhost:8000/v1` to exercise the wire format.
