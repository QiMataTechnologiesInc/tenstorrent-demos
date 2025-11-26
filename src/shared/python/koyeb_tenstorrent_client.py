"""
Client wrapper for calling a Tenstorrent Cloud service running on Koyeb.

The service exposes an OpenAI-compatible HTTP API. Provide the base URL of
your Koyeb deployment (e.g., ``https://<prefix>.koyeb.app``) and an API key
for the Authorization header (JWT or the value configured for ``JWT_SECRET``).
The client automatically targets the ``/v1`` OpenAI routes.
"""

from __future__ import annotations

import os
from dataclasses import dataclass
from typing import Any, Dict, Iterable, Optional

from openai import OpenAI


Message = Dict[str, str]


@dataclass
class KoyebTenstorrentClient:
    """Lightweight helper around the OpenAI SDK for Koyeb/TT Cloud services."""

    base_url: str
    api_key: str
    model: str
    request_timeout: Optional[float] = None

    def __post_init__(self) -> None:
        normalized_base = self.base_url.rstrip("/")
        self._client = OpenAI(
            api_key=self.api_key,
            base_url=f"{normalized_base}/v1",
        )

    @classmethod
    def from_environment(
        cls,
        model: str,
        *,
        base_url_env: str = "KOYEB_TT_BASE_URL",
        api_key_env: str = "KOYEB_TT_API_KEY",
        default_api_key: str = "fake",
        request_timeout: Optional[float] = None,
    ) -> "KoyebTenstorrentClient":
        """
        Build a client using environment variables.

        ``KOYEB_TT_BASE_URL`` should hold the base service URL
        (``https://<prefix>.koyeb.app``). ``KOYEB_TT_API_KEY`` may contain the
        JWT or bearer token; if missing, ``default_api_key`` is used to match
        Tenstorrent's quickstart examples.
        """

        base_url = os.getenv(base_url_env)
        if not base_url:
            raise ValueError(
                f"Environment variable {base_url_env} is required for the base URL"
            )

        api_key = os.getenv(api_key_env, default_api_key)
        return cls(
            base_url=base_url,
            api_key=api_key,
            model=model,
            request_timeout=request_timeout,
        )

    def create_chat_completion(
        self,
        messages: Iterable[Message],
        *,
        max_tokens: Optional[int] = None,
        temperature: Optional[float] = None,
        stream: bool = False,
        **kwargs: Any,
    ) -> Any:
        """
        Call ``/v1/chat/completions`` on the Koyeb service.

        Args:
            messages: Sequence of OpenAI-style chat message dictionaries.
            max_tokens: Optional maximum tokens for the response.
            temperature: Optional sampling temperature.
            stream: Whether to request a streaming response.
            **kwargs: Additional parameters forwarded to ``chat.completions.create``.
        """

        return self._client.chat.completions.create(
            messages=list(messages),
            model=self.model,
            max_tokens=max_tokens,
            temperature=temperature,
            stream=stream,
            timeout=self.request_timeout,
            **kwargs,
        )

    def create_completion(
        self,
        prompt: str,
        *,
        max_tokens: Optional[int] = None,
        temperature: Optional[float] = None,
        stream: bool = False,
        **kwargs: Any,
    ) -> Any:
        """
        Call ``/v1/completions`` on the Koyeb service.
        """

        return self._client.completions.create(
            prompt=prompt,
            model=self.model,
            max_tokens=max_tokens,
            temperature=temperature,
            stream=stream,
            timeout=self.request_timeout,
            **kwargs,
        )

    def create_embeddings(
        self,
        input_texts: Iterable[str],
        *,
        dimensions: Optional[int] = None,
        **kwargs: Any,
    ) -> Any:
        """Call ``/v1/embeddings`` on the Koyeb service."""

        return self._client.embeddings.create(
            input=list(input_texts),
            model=self.model,
            dimensions=dimensions,
            timeout=self.request_timeout,
            **kwargs,
        )


__all__ = ["KoyebTenstorrentClient", "Message"]
