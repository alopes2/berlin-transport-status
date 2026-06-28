data "cloudflare_zone" "main" {
  zone_id = var.zone_id
}

resource "cloudflare_dns_record" "www" {
  zone_id = data.cloudflare_zone.main.id
  type    = "CNAME"
  name    = var.app_record_names["www"]
  content = var.cloudfront_url
  proxied = true
  ttl     = 1
}

resource "cloudflare_dns_record" "root" {
  zone_id = data.cloudflare_zone.main.id
  type    = "CNAME"
  name    = var.app_record_names["@"]
  content = var.cloudfront_url
  proxied = true
  ttl     = 1
}

resource "cloudflare_dns_record" "api" {
  zone_id = data.cloudflare_zone.main.id
  type    = "CNAME"
  name    = var.api_record_name
  content = var.api_url
  proxied = true
  ttl     = 1
}
