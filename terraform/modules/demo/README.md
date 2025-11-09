# Tenstorrent Demo Module

This module demonstrates Terraform best practices for the Tenstorrent demo monorepo.

## Features

- S3 bucket with versioning enabled
- Server-side encryption configured
- Public access blocked
- Comprehensive tagging

## Usage

```hcl
module "demo" {
  source = "../../modules/demo"

  environment  = "dev"
  project_name = "tenstorrent-demo"

  tags = {
    Team = "Engineering"
  }
}
```

## Security

This module follows security best practices:
- Encryption at rest enabled
- Public access blocked
- Versioning enabled for data protection
