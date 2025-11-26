"""Lightweight logging helpers shared across Tenstorrent demos."""

from __future__ import annotations

import logging
import os
from typing import Dict, Union

_LOG_FORMAT = "%(asctime)s [%(levelname)s] %(name)s: %(message)s"
_LEVEL_ALIASES: Dict[str, int] = {
    "DEBUG": logging.DEBUG,
    "INFO": logging.INFO,
    "WARNING": logging.WARNING,
    "WARN": logging.WARNING,
    "ERROR": logging.ERROR,
    "CRITICAL": logging.CRITICAL,
}


def _parse_level(level: Union[str, int]) -> int:
    if isinstance(level, int):
        return level

    normalized = level.upper()
    if normalized in _LEVEL_ALIASES:
        return _LEVEL_ALIASES[normalized]

    raise ValueError(f"Unrecognized log level: {level}")


def resolve_log_level(level: str | int | None = None) -> int:
    """Resolve a log level value from input or ``TT_LOG_LEVEL``.

    Args:
        level: Explicit level value (name or integer). If ``None``,
            fall back to the ``TT_LOG_LEVEL`` environment variable or ``INFO``.

    Returns:
        The numeric logging level constant.
    """

    if level is None:
        env_level = os.getenv("TT_LOG_LEVEL")
        if env_level:
            return _parse_level(env_level)
        return logging.INFO

    return _parse_level(level)


def configure_logging(level: str | int | None = None) -> int:
    """Configure root logging with a consistent format.

    Returns the numeric level that was applied to the root logger.
    """

    resolved_level = resolve_log_level(level)
    root_logger = logging.getLogger()
    if not root_logger.handlers:
        logging.basicConfig(level=resolved_level, format=_LOG_FORMAT)
    else:
        root_logger.setLevel(resolved_level)

    logger = logging.getLogger(__name__)
    logger.debug("Configured logging", extra={"level": resolved_level})
    return resolved_level


__all__ = ["configure_logging", "resolve_log_level"]
