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

provider "aws" {
  region = "us-east-1"
  alias  = "useast1"
  default_tags {
    tags = {
      application = var.project_name
    }
  }
}

provider "cloudflare" {}

data "aws_caller_identity" "current" {}

module "cloudflare_cert_validation" {
  source  = "./cloudflare/cert-validation"
  zone_id = var.cloudflare_zone_id

  cloudfront_validation_options = aws_acm_certificate.cloudfront.domain_validation_options
  api_validation_options        = aws_acm_certificate.api.domain_validation_options
}

module "cloudflare" {
  source           = "./cloudflare"
  zone_id          = var.cloudflare_zone_id
  api_url          = aws_apigatewayv2_domain_name.api.domain_name_configuration[0].target_domain_name
  cloudfront_url   = aws_cloudfront_distribution.frontend.domain_name
  app_record_names = local.app_record_names
  api_record_name  = local.api_record_name
}
