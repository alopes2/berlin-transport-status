output "cloudfront_validation_record_fqdns" {
  value = [
    for record in cloudflare_dns_record.cloudfront_validation :
    record.name
  ]
}

output "api_validation_record_fqdns" {
  value = [
    for record in cloudflare_dns_record.api_validation :
    record.name
  ]
}
