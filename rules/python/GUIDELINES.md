# Python Guidelines

This document provides guidelines for Python projects in the Tenstorrent demo monorepo.

## Standards

- **Python Version**: 3.10+ (minimum)
- **Build System**: pyproject.toml (PEP 621)
- **Package Manager**: pip
- **Virtual Environment**: Required for development

## Code Style

- **Formatter**: ruff format
- **Linter**: ruff
- **Type Checker**: mypy (strict mode)
- **Line Length**: 100 characters

## Linting Rules

Enable the following ruff rule sets:
- E (pycodestyle errors)
- F (pyflakes)
- I (isort)
- N (pep8-naming)
- W (pycodestyle warnings)
- UP (pyupgrade)
- B (flake8-bugbear)
- C4 (flake8-comprehensions)
- SIM (flake8-simplify)

## Project Structure

```
project/
├── pyproject.toml
├── README.md
├── src/
│   └── package_name/
│       ├── __init__.py
│       ├── py.typed
│       └── module.py
└── tests/
    ├── __init__.py
    └── test_module.py
```

## Type Hints

- Use type hints for all function signatures
- Enable mypy strict mode
- Mark packages as typed with `py.typed`
- Use `typing` module for complex types

## Testing

- **Framework**: pytest
- **Coverage**: Minimum 70% line coverage
- **Test Organization**: Mirror source structure in tests/
- **Test Naming**: test_<function>_<scenario>

## Dependencies

Specify dependencies in pyproject.toml:
```toml
[project]
dependencies = [
    "numpy>=1.24.0",
]

[project.optional-dependencies]
dev = [
    "pytest>=7.4.0",
    "pytest-cov>=4.1.0",
    "ruff>=0.1.0",
    "mypy>=1.7.0",
]
```

## Documentation

- Use docstrings for all public modules, functions, classes, and methods
- Follow Google or NumPy docstring style
- Include type information in docstrings
- Provide examples in docstrings

## Example

```python
def add(a: int, b: int) -> int:
    """Add two integers.

    Args:
        a: First integer.
        b: Second integer.

    Returns:
        The sum of a and b.

    Examples:
        >>> add(2, 3)
        5
    """
    return a + b
```

## pyproject.toml Configuration

```toml
[tool.ruff]
line-length = 100
target-version = "py310"

[tool.ruff.lint]
select = ["E", "F", "I", "N", "W", "UP", "B", "C4", "SIM"]

[tool.mypy]
python_version = "3.10"
strict = true
warn_return_any = true
warn_unused_configs = true
disallow_untyped_defs = true
```
