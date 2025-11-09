# Python Demo

Tenstorrent Python demo using pyproject.toml with ruff and mypy.

## Prerequisites

- Python 3.10+
- pip

## Setup

```bash
# Install in development mode
pip install -e ".[dev]"
```

## Development

### Linting

```bash
# Check formatting and linting
ruff check .
ruff format --check .

# Auto-fix issues
ruff check --fix .
ruff format .
```

### Type Checking

```bash
mypy src
```

### Testing

```bash
pytest
```

## Project Structure

- `src/tenstorrent_demo/` - Source code
- `tests/` - Unit tests
- `pyproject.toml` - Project configuration
