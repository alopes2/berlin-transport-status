variable "zone_id" {
  description = "The ID of the Cloudflare zone."
  type        = string
  sensitive   = true
}

variable "api_url" {
  description = "The URL of the API."
  type        = string
}
variable "cloudfront_url" {
  description = "The URL of the CloudFront distribution."
  type        = string
}
