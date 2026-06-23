output "api_url" {
  value = "${aws_apigatewayv2_api.status.api_endpoint}/status"
}

output "collector_function_name" {
  value = aws_lambda_function.collector.function_name
}

output "api_function_name" {
  value = aws_lambda_function.api.function_name
}

output "frontend_bucket" {
  value = aws_s3_bucket.frontend.id
}

output "cloudfront_distribution_id" {
  value = aws_cloudfront_distribution.frontend.id
}

output "site_url" {
  value = "https://${aws_cloudfront_distribution.frontend.domain_name}"
}
