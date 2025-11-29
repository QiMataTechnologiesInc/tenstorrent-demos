"""Self-hosted Flask server that mimics the OpenAI-style Koyeb endpoints."""

from __future__ import annotations

import os
import time
from typing import Any, Dict, Iterable, List, Sequence, Set

from flask import Flask, jsonify, request

from .logging_utils import configure_logging

Message = Dict[str, str]


# The demo servers pair a Blackhole and a Wormhole. If the requested
# LLM can run on the Wormhole, we drive both cards; otherwise, we stay on
# the Blackhole only. The supported models can be provided via the
# TT_WORMHOLE_SUPPORTED_MODELS environment variable as a comma-separated
# list. Default values cover the common shared demo LLMs.
DEFAULT_WORMHOLE_MODELS: Set[str] = {
    "meta-llama/llama-3.1-8b-instruct",
    "meta-llama/llama-3.1-70b-instruct",
    "mistralai/mixtral-8x7b-instruct-v0.1",
}


def _normalize_model_name(model: str) -> str:
    return model.strip().lower()


def _load_wormhole_supported_models(env_var: str = "TT_WORMHOLE_SUPPORTED_MODELS") -> Set[str]:
    """Return a normalized set of Wormhole-supported models."""

    configured = os.getenv(env_var)
    if not configured:
        return set(DEFAULT_WORMHOLE_MODELS)

    raw_models = [entry.strip() for entry in configured.split(",")]
    return {_normalize_model_name(entry) for entry in raw_models if entry}


def resolve_card_plan(model: str, wormhole_supported_models: Set[str]) -> List[str]:
    """
    Choose which Tenstorrent cards to drive for the requested model.

    If the Wormhole supports the model, use both the Wormhole and Blackhole.
    Otherwise, default to the Blackhole.
    """

    normalized_model = _normalize_model_name(model)
    for supported in wormhole_supported_models:
        if normalized_model == supported or normalized_model.startswith(supported):
            return ["wormhole", "blackhole"]

    return ["blackhole"]


# Stubs for hooking into local Tenstorrent hardware. Replace the placeholder
# implementations with TT-NN / TT-Metalium calls when available.
def run_tenstorrent_chat(
    messages: Iterable[Message],
    model: str,
    cards: Sequence[str],
    *,
    max_tokens: int | None,
    temperature: float | None,
) -> str:
    """Stub placeholder for chat inference on local Tenstorrent hardware."""

    del messages, model, max_tokens, temperature
    return f"[stub] Integrate Tenstorrent chat inference here using {', '.join(cards)}."


def run_tenstorrent_completion(
    prompt: str,
    model: str,
    cards: Sequence[str],
    *,
    max_tokens: int | None,
    temperature: float | None,
) -> str:
    """Stub placeholder for completion inference on local Tenstorrent hardware."""

    del prompt, model, max_tokens, temperature
    return (
        "[stub] Integrate Tenstorrent text completion here using "
        + ", ".join(cards)
        + "."
    )


def run_tenstorrent_embedding(
    input_texts: Iterable[str],
    model: str,
    cards: Sequence[str],
    *,
    dimensions: int | None,
) -> List[float]:
    """Stub placeholder for embedding generation on local Tenstorrent hardware."""

    del input_texts, model, cards
    dim = dimensions or 16
    return [0.0 for _ in range(dim)]


def create_app(default_model: str | None = None) -> Flask:
    """Build a Flask app that exposes OpenAI-compatible endpoints locally."""

    app = Flask(__name__)
    configure_logging()
    logger = app.logger
    configured_model = default_model or os.getenv("TT_MODEL", "local-tenstorrent")
    wormhole_models = _load_wormhole_supported_models()
    logger.info(
        "Starting Tenstorrent Flask app",
        extra={
            "model": configured_model,
            "log_level": logger.level,
            "wormhole_supported_models": sorted(wormhole_models),
        },
    )

    @app.post("/v1/chat/completions")
    def chat_completions() -> Any:
        payload = request.get_json(silent=True) or {}
        model = payload.get("model", configured_model)
        messages = payload.get("messages", [])
        max_tokens = payload.get("max_tokens")
        temperature = payload.get("temperature")
        cards = resolve_card_plan(model, wormhole_models)

        logger.debug(
            "Handling chat completion",
            extra={
                "message_count": len(messages),
                "model": model,
                "cards": cards,
                "max_tokens": max_tokens,
                "temperature": temperature,
            },
        )

        content = run_tenstorrent_chat(
            messages,
            model,
            cards,
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
        cards = resolve_card_plan(model, wormhole_models)

        logger.debug(
            "Handling text completion",
            extra={
                "prompt_length": len(prompt),
                "model": model,
                "cards": cards,
                "max_tokens": max_tokens,
                "temperature": temperature,
            },
        )

        text = run_tenstorrent_completion(
            prompt,
            model,
            cards,
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
        cards = resolve_card_plan(model, wormhole_models)

        logger.debug(
            "Handling embeddings",
            extra={
                "input_count": len(input_texts),
                "model": model,
                "cards": cards,
                "dimensions": dimensions,
            },
        )

        vector = run_tenstorrent_embedding(
            input_texts, model, cards, dimensions=dimensions
        )

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
    "resolve_card_plan",
    "run_tenstorrent_chat",
    "run_tenstorrent_completion",
    "run_tenstorrent_embedding",
]
