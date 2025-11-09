# Terraform Demo

Tenstorrent Terraform demo with modules and checkov security scanning.

## Prerequisites

- Terraform 1.6+
- checkov (for security scanning)

## Installation

```bash
# Install checkov
pip install checkov
```

## Usage

### Initialize

```bash
cd environments/dev
terraform init
```

### Plan

```bash
terraform plan
```

### Apply

```bash
terraform apply
```

### Security Scan

```bash
checkov -d .
```

## Project Structure

- `modules/` - Reusable Terraform modules
- `environments/` - Environment-specific configurations
  - `dev/` - Development environment
  - `staging/` - Staging environment
  - `prod/` - Production environment
