resource "aws_acm_certificate" "cloudfront" {
  domain_name = "isberlinmoving.com"
  subject_alternative_names = [
    "www.isberlinmoving.com"
  ]

  validation_method = "DNS"
}

resource "aws_acm_certificate_validation" "cloudfront" {
  certificate_arn = aws_acm_certificate.cloudfront.arn

  validation_record_fqdns = [
    local.app_record_names["www"],
    local.app_record_names["@"]
  ]
}

resource "aws_acm_certificate" "api" {
  domain_name = "api.isberlinmoving.com"

  validation_method = "DNS"
}

resource "aws_acm_certificate_validation" "api" {
  certificate_arn = aws_acm_certificate.api.arn

  validation_record_fqdns = [
    local.api_record_name
  ]
}
