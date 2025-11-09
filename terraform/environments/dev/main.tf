terraform {
  required_version = ">= 1.6"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }

  # Backend configuration for state management
  # Uncomment and configure for production use
  # backend "s3" {
  #   bucket = "tenstorrent-terraform-state"
  #   key    = "dev/terraform.tfstate"
  #   region = "us-east-1"
  # }
}

provider "aws" {
  region = var.aws_region

  default_tags {
    tags = {
      Environment = "dev"
      Project     = "tenstorrent-demo"
      ManagedBy   = "Terraform"
    }
  }
}

module "demo" {
  source = "../../modules/demo"

  environment  = "dev"
  project_name = "tenstorrent-demo"

  tags = {
    Team = "Engineering"
  }
}
