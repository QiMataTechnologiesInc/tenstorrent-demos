# Rules Directory - Source of Truth

This directory contains the canonical configurations and standards for the Tenstorrent demo monorepo.

## Overview

The `/rules` directory serves as the single source of truth for:
- Build configurations
- Linting standards
- Testing requirements
- Deployment specifications
- CI/CD pipeline definitions
- Language-specific guidelines for agents

## Structure

Each language has its own subdirectory containing:
- **`<language>/GUIDELINES.md`** - Comprehensive guidelines for that language
- **`<language>/<language>.json`** - Machine-readable configuration standards

### Language Directories

- **`cpp/`** - C++ standards (CMake, vcpkg, clang-format)
- **`python/`** - Python standards (ruff, mypy, pytest)
- **`dotnet/`** - .NET standards (analyzers, xUnit)
- **`ansible/`** - Ansible standards (ansible-lint, vault)
- **`terraform/`** - Terraform standards (modules, checkov, tflint)

### CI/CD Configuration

- **`ci.json`** - CI/CD pipeline configuration

## Usage

### For Agents
Agents should read the GUIDELINES.md files to understand language-specific standards, conventions, and best practices when working with code in this repository.

### For CI/CD
CI pipelines and local development tools read the JSON configuration files to ensure consistency across all projects in the monorepo.

### For Developers
Developers should reference both the guidelines and JSON configs to understand project standards and tooling requirements.
