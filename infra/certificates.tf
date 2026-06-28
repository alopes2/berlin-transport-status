resource "aws_acm_certificate" "cloudfront" {
  domain_name = "isberlinmoving.com"
  subject_alternative_names = [
    "www.isberlinmoving.com"
  ]

  validation_method = "DNS"
}

resource "aws_acm_certificate" "api" {
  domain_name = "api.isberlinmoving.com"

  validation_method = "DNS"
}
