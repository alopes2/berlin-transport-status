locals {
  bucket_name = "${var.project_name}-${data.aws_caller_identity.current.account_id}"
  app_record_names = {
    "www" = "www"
    "@"   = "@"
  }
  api_record_name = "api"
}
