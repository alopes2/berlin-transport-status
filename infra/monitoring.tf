resource "aws_cloudwatch_log_group" "collector" {
  name              = "/aws/lambda/${var.project_name}-collector"
  retention_in_days = 30
}

resource "aws_cloudwatch_log_group" "api" {
  name              = "/aws/lambda/${var.project_name}-api"
  retention_in_days = 30
}

resource "aws_cloudwatch_metric_alarm" "collector_errors" {
  alarm_name          = "${var.project_name}-collector-errors"
  namespace           = "AWS/Lambda"
  metric_name         = "Errors"
  statistic           = "Sum"
  period              = 300
  evaluation_periods  = 1
  threshold           = 1
  comparison_operator = "GreaterThanOrEqualToThreshold"
  treat_missing_data  = "notBreaching"

  dimensions = {
    FunctionName = aws_lambda_function.collector.function_name
  }
}
