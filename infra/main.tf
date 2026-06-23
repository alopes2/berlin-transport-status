terraform {
  required_version = ">= 1.8.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }
  }

  backend "s3" {
    bucket       = "andre-lopes-iac"
    key          = "berlin-transport-status.tfstate"
    encrypt      = true
    use_lockfile = true
    region       = "eu-central-1"
  }
}

provider "aws" {
  region = var.aws_region

  default_tags {
    tags = {
      application = var.project_name
    }
  }
}

data "aws_caller_identity" "current" {}
