# Terraform Guidelines

This document provides guidelines for Terraform projects in the Tenstorrent demo monorepo.

## Standards

- **Terraform Version**: 1.6+ (minimum)
- **Provider Versions**: Pin major versions with ~>
- **State Backend**: S3 with DynamoDB locking (recommended)
- **Linter**: tflint
- **Security Scanner**: checkov

## Project Structure

```
terraform/
├── modules/
│   └── module_name/
│       ├── main.tf
│       ├── variables.tf
│       ├── outputs.tf
│       ├── versions.tf
│       └── README.md
├── environments/
│   ├── dev/
│   │   ├── main.tf
│   │   ├── variables.tf
│   │   ├── outputs.tf
│   │   ├── backend.tf
│   │   └── terraform.tfvars
│   ├── staging/
│   └── prod/
├── .tflint.hcl
└── .checkov.yml
```

## Naming Conventions

- **Resources**: lowercase with underscores (snake_case)
- **Variables**: descriptive, lowercase with underscores
- **Modules**: lowercase with hyphens (kebab-case)
- **Files**: main.tf, variables.tf, outputs.tf, versions.tf

## Code Organization

### main.tf
Core resource definitions

### variables.tf
Input variable declarations

### outputs.tf
Output value definitions

### versions.tf
Provider and Terraform version constraints

## Best Practices

1. Use modules for reusable infrastructure
2. Pin provider versions
3. Use variables for all configurable values
4. Always include outputs for important values
5. Use data sources for existing resources
6. Tag all resources consistently

## Example Module

```hcl
# versions.tf
terraform {
  required_version = ">= 1.6"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

# variables.tf
variable "environment" {
  description = "Environment name"
  type        = string
}

variable "tags" {
  description = "Common tags"
  type        = map(string)
  default     = {}
}

# main.tf
resource "aws_s3_bucket" "example" {
  bucket = "example-${var.environment}"
  tags   = var.tags
}

# outputs.tf
output "bucket_name" {
  description = "Name of the bucket"
  value       = aws_s3_bucket.example.id
}
```

## Security Best Practices

1. Enable encryption at rest
2. Enable encryption in transit
3. Block public access (S3, RDS, etc.)
4. Use least privilege IAM policies
5. Enable versioning for stateful resources
6. Use security groups instead of 0.0.0.0/0

## checkov Configuration

```yaml
# .checkov.yml
framework:
  - terraform

skip-check: []

compact: true
download-external-modules: true
```

## tflint Configuration

```hcl
# .tflint.hcl
plugin "aws" {
  enabled = true
  version = "0.29.0"
  source  = "github.com/terraform-linters/tflint-ruleset-aws"
}

rule "terraform_naming_convention" {
  enabled = true
}

rule "terraform_deprecated_interpolation" {
  enabled = true
}
```

## State Management

Use remote backend for team collaboration:
```hcl
terraform {
  backend "s3" {
    bucket         = "terraform-state-bucket"
    key            = "env/terraform.tfstate"
    region         = "us-east-1"
    dynamodb_table = "terraform-state-lock"
    encrypt        = true
  }
}
```

## Tagging Strategy

Always include minimum tags:
```hcl
locals {
  common_tags = {
    Environment = var.environment
    Project     = var.project_name
    ManagedBy   = "Terraform"
    Repository  = var.repository_url
  }
}
```

## Validation

Before commit, always run:
```bash
terraform fmt -recursive
terraform validate
tflint
checkov -d .
```

## Testing

- **Framework**: Terratest (Go)
- **Unit Tests**: Test modules in isolation
- **Integration Tests**: Test complete environments
- **Plan Validation**: Always review plans before apply
