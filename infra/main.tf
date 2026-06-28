terraform {
  required_version = ">= 1.8.0"

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }

    cloudflare = {
      source  = "cloudflare/cloudflare"
      version = "~> 5"
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

provider "cloudflare" {}

data "aws_caller_identity" "current" {}

module "cloudflare" {
  source         = "./cloudfare"
  zone_id        = var.cloudflare_zone_id
  api_url        = "${aws_apigatewayv2_api.status.id}.execute-api.${aws_apigatewayv2_api.status.region}.amazonaws.com"
  cloudfront_url = aws_cloudfront_distribution.frontend.domain_name
}
