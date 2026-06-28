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

variable "app_record_names" {
  description = "The names of the DNS records for the application."
  type        = map(string)
}

variable "api_record_name" {
  description = "The name of the DNS record for the API."
  type        = string
}

