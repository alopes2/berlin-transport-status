terraform {
  required_providers {
    cloudflare = {
      source  = "cloudflare/cloudflare"
      version = "~> 5"
    }
  }
}

resource "cloudflare_dns_record" "cloudfront_validation" {
  for_each = {
    for dvo in var.cloudfront_validation_options :
    dvo.domain_name => dvo
  }

  zone_id = var.zone_id
  name    = trimsuffix(each.value.resource_record_name, ".")
  type    = each.value.resource_record_type
  content = trimsuffix(each.value.resource_record_value, ".")
  proxied = false
  ttl     = 60
}

resource "cloudflare_dns_record" "api_validation" {
  for_each = {
    for dvo in var.api_validation_options :
    dvo.domain_name => dvo
  }

  zone_id = var.zone_id
  name    = trimsuffix(each.value.resource_record_name, ".")
  type    = each.value.resource_record_type
  content = trimsuffix(each.value.resource_record_value, ".")
  proxied = false
  ttl     = 60
}
