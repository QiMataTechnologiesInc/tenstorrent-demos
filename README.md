# Tenstorrent Demos Monorepo

A standardized monorepo for Tenstorrent demos with comprehensive language guidelines and standards.

## Overview

This monorepo provides centralized guidelines and standards for:
- **C++** - CMake + vcpkg
- **Python** - pyproject.toml with ruff/mypy
- **.NET** - .NET 8 with analyzers
- **Ansible** - ansible-lint + vault support
- **Terraform** - modules + checkov security scanning

The `/rules` directory serves as the single source of truth for all language standards and guidelines.

## Quick Start

### Prerequisites

Review the guidelines for the language you're working with:

- **C++**: See [rules/cpp/GUIDELINES.md](rules/cpp/GUIDELINES.md)
- **Python**: See [rules/python/GUIDELINES.md](rules/python/GUIDELINES.md)
- **.NET**: See [rules/dotnet/GUIDELINES.md](rules/dotnet/GUIDELINES.md)
- **Ansible**: See [rules/ansible/GUIDELINES.md](rules/ansible/GUIDELINES.md)
- **Terraform**: See [rules/terraform/GUIDELINES.md](rules/terraform/GUIDELINES.md)

## Project Structure

```
tenstorrent-demos/
├── rules/                    # Source of truth for all configurations
│   ├── cpp/                  # C++ standards and guidelines
│   │   ├── GUIDELINES.md     # C++ development guidelines for agents
│   │   └── cpp.json          # C++ configuration
│   ├── python/               # Python standards and guidelines
│   │   ├── GUIDELINES.md     # Python development guidelines for agents
│   │   └── python.json       # Python configuration
│   ├── dotnet/               # .NET standards and guidelines
│   │   ├── GUIDELINES.md     # .NET development guidelines for agents
│   │   └── dotnet.json       # .NET configuration
│   ├── ansible/              # Ansible standards and guidelines
│   │   ├── GUIDELINES.md     # Ansible development guidelines for agents
│   │   └── ansible.json      # Ansible configuration
│   ├── terraform/            # Terraform standards and guidelines
│   │   ├── GUIDELINES.md     # Terraform development guidelines for agents
│   │   └── terraform.json    # Terraform configuration
│   └── ci.json               # CI/CD configuration
├── .github/
│   └── workflows/            # CI/CD pipelines
└── Makefile                  # Project automation
```

## Using the Guidelines

### For Developers

Each language directory in `/rules` contains comprehensive GUIDELINES.md files that provide:
- Standards and version requirements
- Code style and naming conventions
- Project structure templates
- Testing requirements and frameworks
- Documentation standards
- Best practices and examples

### For Agents

Agents should read the GUIDELINES.md files to understand language-specific standards when working with code in this repository.

## CI/CD Pipeline

The CI pipeline is defined in `.github/workflows/ci.yml` and dynamically configures jobs based on `/rules/ci.json`.

## Rules Directory

The `/rules` directory contains language-specific subdirectories that serve as the source of truth:

Each language directory contains:
- **GUIDELINES.md** - Comprehensive development guidelines for agents and developers
- **<language>.json** - Machine-readable configuration standards

Language directories:
- **`rules/cpp/`** - CMake version, C++ standard, compiler flags, vcpkg dependencies, coding conventions
- **`rules/python/`** - Python version, ruff/mypy configuration, test coverage requirements, type hinting standards
- **`rules/dotnet/`** - .NET version, analyzer packages, build configuration, XML documentation standards
- **`rules/ansible/`** - Ansible version, ansible-lint rules, vault configuration, playbook structure
- **`rules/terraform/`** - Terraform version, tflint rules, checkov security policies, module organization

The GUIDELINES.md files provide detailed instructions for agents on how to work with each language in this repository.
- **`rules/ci.json`** - CI/CD pipeline configuration

These files are the single source of truth and are read by:
- CI/CD pipelines
- Agents and developers
- Code generation tools

## Security Standards

The guidelines in `/rules` specify security standards for each language:

- **C++**: Compiler warnings treated as errors
- **Python**: Type checking with mypy, linting with ruff
- **.NET**: Code analyzers (StyleCop, SonarAnalyzer)
- **Ansible**: ansible-lint validation, vault support for secrets
- **Terraform**: checkov security scanning, encryption enabled by default

## Contributing

1. Fork the repository
2. Create a feature branch
3. Follow the guidelines in `/rules/<language>/GUIDELINES.md`
4. Submit a pull request

All contributions must adhere to the standards defined in the `/rules` directory.
3. Make your changes
4. Run `make lint` and `make test`
5. Submit a pull request

All pull requests must pass CI checks before merging.

## License

See [LICENSE](LICENSE) file for details.

## Support

For issues or questions, please open a GitHub issue in the repository.