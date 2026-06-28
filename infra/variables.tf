variable "aws_region" {
  description = "AWS region for the application."
  type        = string
  default     = "eu-central-1"
}

variable "project_name" {
  description = "Prefix used for AWS resource names."
  type        = string
  default     = "berlin-transport-status"
}

variable "tracking_since" {
  description = "First Berlin calendar date included in streak tracking."
  type        = string

  validation {
    condition     = can(regex("^\\d{4}-\\d{2}-\\d{2}$", var.tracking_since))
    error_message = "tracking_since must use YYYY-MM-DD."
  }
}

variable "api_throttling_rate_limit" {
  description = "Maximum sustained requests per second accepted by API Gateway."
  type        = number
  default     = 5

  validation {
    condition     = var.api_throttling_rate_limit > 0
    error_message = "api_throttling_rate_limit must be greater than zero."
  }
}

variable "api_throttling_burst_limit" {
  description = "Maximum burst of concurrent requests accepted by API Gateway."
  type        = number
  default     = 10

  validation {
    condition     = var.api_throttling_burst_limit > 0 && floor(var.api_throttling_burst_limit) == var.api_throttling_burst_limit
    error_message = "api_throttling_burst_limit must be a positive whole number."
  }
}

variable "frontend_dist_path" {
  description = "Path to the built Vite frontend."
  type        = string
  default     = "../frontend/dist"
}

variable "cloudflare_zone_id" {
  description = "The ID of the Cloudflare zone."
  type        = string
  sensitive   = true
}
