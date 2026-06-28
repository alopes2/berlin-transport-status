output "zone" {
  value = data.cloudflare_zone.main
}

output "cloudfront_records" {
  value = [
    cloudflare_dns_record.www,
    cloudflare_dns_record.root
  ]
}

output "api_record" {
  value = cloudflare_dns_record.api
}
