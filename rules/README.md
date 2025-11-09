# Rules Directory - Source of Truth

This directory contains the canonical configurations and standards for the Tenstorrent demo monorepo.

## Overview

The `/rules` directory serves as the single source of truth for:
- Build configurations
- Linting standards
- Testing requirements
- Deployment specifications
- CI/CD pipeline definitions

## Structure

- `cpp.json` - C++ project standards (CMake, vcpkg)
- `python.json` - Python project standards (ruff, mypy)
- `dotnet.json` - .NET project standards (analyzers)
- `ansible.json` - Ansible standards (ansible-lint, vault)
- `terraform.json` - Terraform standards (modules, checkov)
- `ci.json` - CI/CD configuration

## Usage

CI pipelines and local development tools read these files to ensure consistency across all projects in the monorepo.
