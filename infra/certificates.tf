resource "aws_acm_certificate" "cloudfront" {
  provider = aws.useast1
  region   = "us-east-1"

  domain_name = "isberlinmoving.com"

  subject_alternative_names = [
    "www.isberlinmoving.com"
  ]

  validation_method = "DNS"
}

resource "aws_acm_certificate_validation" "cloudfront" {
  provider        = aws.useast1
  region          = "us-east-1"
  certificate_arn = aws_acm_certificate.cloudfront.arn

  validation_record_fqdns = module.cloudflare_cert_validation.cloudfront_validation_record_fqdns
}

resource "aws_acm_certificate" "api" {
  domain_name = "api.isberlinmoving.com"

  validation_method = "DNS"
}

resource "aws_acm_certificate_validation" "api" {
  certificate_arn = aws_acm_certificate.api.arn

  validation_record_fqdns = module.cloudflare_cert_validation.api_validation_record_fqdns
}
