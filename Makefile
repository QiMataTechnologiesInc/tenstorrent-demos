.PHONY: help init clean lint test build check-cpp check-python check-dotnet check-ansible check-terraform

# Default target
help:
	@echo "Tenstorrent Demo Monorepo - Make Targets"
	@echo ""
	@echo "Setup:"
	@echo "  init              - Initialize all environments and run checks"
	@echo "  clean             - Clean all build artifacts"
	@echo ""
	@echo "Development:"
	@echo "  lint              - Run all linters"
	@echo "  test              - Run all tests"
	@echo "  build             - Build all projects"
	@echo ""
	@echo "Project-specific checks:"
	@echo "  check-cpp         - Check C++ project"
	@echo "  check-python      - Check Python project"
	@echo "  check-dotnet      - Check .NET project"
	@echo "  check-ansible     - Check Ansible project"
	@echo "  check-terraform   - Check Terraform project"

# Initialize environment and run checks
init:
	@echo "==> Initializing Tenstorrent Demo Monorepo..."
	@echo ""
	@echo "==> Checking system requirements..."
	@command -v cmake >/dev/null 2>&1 || echo "Warning: cmake not found (required for C++)"
	@command -v python3 >/dev/null 2>&1 || echo "Warning: python3 not found (required for Python)"
	@command -v dotnet >/dev/null 2>&1 || echo "Warning: dotnet not found (required for .NET)"
	@command -v ansible >/dev/null 2>&1 || echo "Warning: ansible not found (required for Ansible)"
	@command -v terraform >/dev/null 2>&1 || echo "Warning: terraform not found (required for Terraform)"
	@echo ""
	@echo "==> Setting up Python environment..."
	@if command -v python3 >/dev/null 2>&1; then \
		cd python && python3 -m pip install -e ".[dev]" || true; \
	fi
	@echo ""
	@echo "==> Setting up Ansible environment..."
	@if command -v pip >/dev/null 2>&1; then \
		pip install ansible ansible-lint || true; \
	fi
	@echo ""
	@echo "==> Setting up Terraform tools..."
	@if command -v pip >/dev/null 2>&1; then \
		pip install checkov || true; \
	fi
	@echo ""
	@echo "==> Running initial checks..."
	@$(MAKE) check-python || true
	@$(MAKE) check-dotnet || true
	@$(MAKE) check-ansible || true
	@$(MAKE) check-terraform || true
	@echo ""
	@echo "==> Initialization complete!"
	@echo "==> See individual project READMEs for detailed usage instructions."

# Clean build artifacts
clean:
	@echo "==> Cleaning build artifacts..."
	@rm -rf cpp/build
	@cd dotnet && dotnet clean || true
	@find python -type d -name "__pycache__" -exec rm -rf {} + 2>/dev/null || true
	@find python -type d -name "*.egg-info" -exec rm -rf {} + 2>/dev/null || true
	@find python -type d -name ".pytest_cache" -exec rm -rf {} + 2>/dev/null || true
	@find python -type d -name ".mypy_cache" -exec rm -rf {} + 2>/dev/null || true
	@find python -type d -name ".ruff_cache" -exec rm -rf {} + 2>/dev/null || true
	@find python -type d -name "htmlcov" -exec rm -rf {} + 2>/dev/null || true
	@find terraform -type d -name ".terraform" -exec rm -rf {} + 2>/dev/null || true
	@find terraform -name "*.tfstate*" -delete 2>/dev/null || true
	@echo "==> Clean complete!"

# Run all linters
lint: check-python check-dotnet check-ansible check-terraform
	@echo "==> All linting complete!"

# Run all tests
test: check-python check-dotnet
	@echo "==> All tests complete!"

# Build all projects
build:
	@echo "==> Building all projects..."
	@if command -v cmake >/dev/null 2>&1; then \
		cd cpp && cmake -B build -S . && cmake --build build; \
	fi
	@if command -v dotnet >/dev/null 2>&1; then \
		cd dotnet && dotnet build --configuration Release; \
	fi
	@echo "==> Build complete!"

# C++ checks
check-cpp:
	@echo "==> Checking C++ project..."
	@if command -v cmake >/dev/null 2>&1; then \
		cd cpp && \
		cmake -B build -S . && \
		cmake --build build && \
		cd build && ctest --output-on-failure; \
	else \
		echo "cmake not found, skipping C++ checks"; \
	fi

# Python checks
check-python:
	@echo "==> Checking Python project..."
	@if command -v python3 >/dev/null 2>&1; then \
		cd python && \
		ruff check . && \
		ruff format --check . && \
		mypy src && \
		pytest; \
	else \
		echo "python3 not found, skipping Python checks"; \
	fi

# .NET checks
check-dotnet:
	@echo "==> Checking .NET project..."
	@if command -v dotnet >/dev/null 2>&1; then \
		cd dotnet && \
		dotnet restore && \
		dotnet format --verify-no-changes && \
		dotnet build --configuration Release && \
		dotnet test --configuration Release; \
	else \
		echo "dotnet not found, skipping .NET checks"; \
	fi

# Ansible checks
check-ansible:
	@echo "==> Checking Ansible project..."
	@if command -v ansible-lint >/dev/null 2>&1; then \
		cd ansible && \
		ansible-lint && \
		for playbook in playbooks/*.yml; do \
			ansible-playbook --syntax-check "$$playbook"; \
		done; \
	else \
		echo "ansible-lint not found, skipping Ansible checks"; \
	fi

# Terraform checks
check-terraform:
	@echo "==> Checking Terraform project..."
	@if command -v terraform >/dev/null 2>&1; then \
		cd terraform && \
		terraform fmt -check -recursive && \
		cd environments/dev && \
		terraform init -backend=false && \
		terraform validate; \
	else \
		echo "terraform not found, skipping Terraform checks"; \
	fi
	@if command -v checkov >/dev/null 2>&1; then \
		cd terraform && checkov -d . --config-file .checkov.yml; \
	else \
		echo "checkov not found, skipping security scan"; \
	fi
