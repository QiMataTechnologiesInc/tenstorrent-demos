"""Self-hosted Flask server that mimics the OpenAI-style Koyeb endpoints."""

from __future__ import annotations

import os
import time
from typing import Any, Dict, Iterable, List

from flask import Flask, jsonify, request

from .logging_utils import configure_logging

Message = Dict[str, str]


# Stubs for hooking into local Tenstorrent hardware. Replace the placeholder
# implementations with TT-NN / TT-Metalium calls when available.
def run_tenstorrent_chat(
    messages: Iterable[Message],
    model: str,
    *,
    max_tokens: int | None,
    temperature: float | None,
) -> str:
    """Stub placeholder for chat inference on local Tenstorrent hardware."""

    del messages, model, max_tokens, temperature
    return "[stub] Integrate Tenstorrent chat inference here."


def run_tenstorrent_completion(
    prompt: str,
    model: str,
    *,
    max_tokens: int | None,
    temperature: float | None,
) -> str:
    """Stub placeholder for completion inference on local Tenstorrent hardware."""

    del prompt, model, max_tokens, temperature
    return "[stub] Integrate Tenstorrent text completion here."


def run_tenstorrent_embedding(
    input_texts: Iterable[str],
    model: str,
    *,
    dimensions: int | None,
) -> List[float]:
    """Stub placeholder for embedding generation on local Tenstorrent hardware."""

    del input_texts, model
    dim = dimensions or 16
    return [0.0 for _ in range(dim)]


def create_app(default_model: str | None = None) -> Flask:
    """Build a Flask app that exposes OpenAI-compatible endpoints locally."""

    app = Flask(__name__)
    configure_logging()
    logger = app.logger
    configured_model = default_model or os.getenv("TT_MODEL", "local-tenstorrent")
    logger.info(
        "Starting Tenstorrent Flask app",
        extra={"model": configured_model, "log_level": logger.level},
    )

    @app.post("/v1/chat/completions")
    def chat_completions() -> Any:
        payload = request.get_json(silent=True) or {}
        model = payload.get("model", configured_model)
        messages = payload.get("messages", [])
        max_tokens = payload.get("max_tokens")
        temperature = payload.get("temperature")

        logger.debug(
            "Handling chat completion",
            extra={
                "message_count": len(messages),
                "model": model,
                "max_tokens": max_tokens,
                "temperature": temperature,
            },
        )

        content = run_tenstorrent_chat(
            messages,
            model,
            max_tokens=max_tokens,
            temperature=temperature,
        )

        response = {
            "id": "chatcmpl-local-stub",
            "object": "chat.completion",
            "created": int(time.time()),
            "model": model,
            "choices": [
                {
                    "index": 0,
                    "message": {"role": "assistant", "content": content},
                    "finish_reason": "stop",
                }
            ],
            "usage": {"prompt_tokens": 0, "completion_tokens": 0, "total_tokens": 0},
        }
        return jsonify(response)

    @app.post("/v1/completions")
    def completions() -> Any:
        payload = request.get_json(silent=True) or {}
        model = payload.get("model", configured_model)
        prompt = payload.get("prompt", "")
        max_tokens = payload.get("max_tokens")
        temperature = payload.get("temperature")

        logger.debug(
            "Handling text completion",
            extra={
                "prompt_length": len(prompt),
                "model": model,
                "max_tokens": max_tokens,
                "temperature": temperature,
            },
        )

        text = run_tenstorrent_completion(
            prompt,
            model,
            max_tokens=max_tokens,
            temperature=temperature,
        )

        response = {
            "id": "cmpl-local-stub",
            "object": "text_completion",
            "created": int(time.time()),
            "model": model,
            "choices": [
                {
                    "index": 0,
                    "text": text,
                    "logprobs": None,
                    "finish_reason": "stop",
                }
            ],
            "usage": {"prompt_tokens": 0, "completion_tokens": 0, "total_tokens": 0},
        }
        return jsonify(response)

    @app.post("/v1/embeddings")
    def embeddings() -> Any:
        payload = request.get_json(silent=True) or {}
        model = payload.get("model", configured_model)
        input_texts = payload.get("input", [])
        dimensions = payload.get("dimensions")

        logger.debug(
            "Handling embeddings",
            extra={
                "input_count": len(input_texts),
                "model": model,
                "dimensions": dimensions,
            },
        )

        vector = run_tenstorrent_embedding(input_texts, model, dimensions=dimensions)

        response = {
            "object": "list",
            "data": [
                {
                    "object": "embedding",
                    "index": 0,
                    "embedding": vector,
                }
            ],
            "model": model,
            "usage": {"prompt_tokens": 0, "total_tokens": 0},
        }
        return jsonify(response)

    return app


__all__ = [
    "create_app",
    "run_tenstorrent_chat",
    "run_tenstorrent_completion",
    "run_tenstorrent_embedding",
]
