data "cloudflare_zone" "main" {
  zone_id = var.zone_id
}

resource "cloudflare_dns_record" "www" {
  zone_id = data.cloudflare_zone.main.id
  type    = "CNAME"
  name    = "api"
  content = "awyy0lywgc.execute-api.eu-central-1.amazonaws.com"
  proxied = true
  ttl     = 1
}
