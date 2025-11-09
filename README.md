# Tenstorrent Demos Monorepo

A standardized monorepo for Tenstorrent demos featuring C++, Python, .NET, Ansible, and Terraform projects with consistent tooling and CI/CD.

## Overview

This monorepo provides drop-in scaffolds and standardized configurations for:
- **C++** - CMake + vcpkg
- **Python** - pyproject.toml with ruff/mypy
- **.NET** - .NET 8 with analyzers
- **Ansible** - ansible-lint + vault support
- **Terraform** - modules + checkov security scanning

The `/rules` directory serves as the single source of truth for all project configurations.

## Quick Start

### Prerequisites

Install the required tools for the projects you want to work with:

- **C++**: CMake 3.20+, C++17 compiler, vcpkg (optional)
- **Python**: Python 3.10+
- **.NET**: .NET SDK 8.0+
- **Ansible**: Ansible 2.15+, ansible-lint
- **Terraform**: Terraform 1.6+, checkov

### Initialize Environment

```bash
# Clone the repository
git clone https://github.com/QiMataTechnologiesInc/tenstorrent-demos.git
cd tenstorrent-demos

# Initialize environment and run checks
make init
```

The `make init` command will:
1. Check system requirements
2. Set up Python, Ansible, and Terraform environments
3. Run initial validation checks
4. Report any missing dependencies

## Project Structure

```
tenstorrent-demos/
├── rules/              # Source of truth for all configurations
│   ├── cpp.json        # C++ standards
│   ├── python.json     # Python standards
│   ├── dotnet.json     # .NET standards
│   ├── ansible.json    # Ansible standards
│   ├── terraform.json  # Terraform standards
│   └── ci.json         # CI/CD configuration
├── cpp/                # C++ demos (CMake + vcpkg)
├── python/             # Python demos (pyproject.toml + ruff/mypy)
├── dotnet/             # .NET demos (analyzers)
├── ansible/            # Ansible playbooks (ansible-lint + vault)
├── terraform/          # Terraform modules (checkov)
├── .github/
│   └── workflows/      # CI/CD pipelines
└── Makefile            # Project automation
```

## Development Workflow

### Available Make Targets

```bash
make help              # Show all available targets
make init              # Initialize environment and run checks
make clean             # Clean all build artifacts
make lint              # Run all linters
make test              # Run all tests
make build             # Build all projects

# Project-specific checks
make check-cpp         # Check C++ project
make check-python      # Check Python project
make check-dotnet      # Check .NET project
make check-ansible     # Check Ansible project
make check-terraform   # Check Terraform project
```

### Working with Individual Projects

Each project has its own README with detailed instructions:

- [C++ Demo](cpp/README.md)
- [Python Demo](python/README.md)
- [.NET Demo](dotnet/README.md)
- [Ansible Demo](ansible/README.md)
- [Terraform Demo](terraform/README.md)

## CI/CD Pipeline

The CI pipeline automatically:
1. Reads configuration from `/rules`
2. Lints all projects
3. Runs tests
4. Builds artifacts
5. Performs security scans

The pipeline is defined in `.github/workflows/ci.yml` and dynamically configures jobs based on `/rules/ci.json`.

## Rules Directory

The `/rules` directory contains JSON configuration files that define standards for each project type:

- **cpp.json** - CMake version, C++ standard, compiler flags, vcpkg dependencies
- **python.json** - Python version, ruff/mypy configuration, test coverage requirements
- **dotnet.json** - .NET version, analyzer packages, build configuration
- **ansible.json** - Ansible version, ansible-lint rules, vault configuration
- **terraform.json** - Terraform version, tflint rules, checkov security policies
- **ci.json** - CI/CD pipeline configuration

These files are the single source of truth and are read by:
- CI/CD pipelines
- Local development tools
- Project scaffolds

## Adding New Projects

To add a new demo project:

1. Choose the appropriate directory (`cpp/`, `python/`, etc.)
2. Follow the existing structure and patterns
3. Update the relevant `/rules` configuration if needed
4. Ensure your project passes `make check-<language>`

## Security

- **C++**: Compiler warnings treated as errors
- **Python**: Type checking with mypy, linting with ruff
- **.NET**: Code analyzers (StyleCop, SonarAnalyzer)
- **Ansible**: ansible-lint validation, vault support for secrets
- **Terraform**: checkov security scanning, encryption enabled by default

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run `make lint` and `make test`
5. Submit a pull request

All pull requests must pass CI checks before merging.

## License

See [LICENSE](LICENSE) file for details.

## Support

For issues or questions, please open a GitHub issue in the repository.