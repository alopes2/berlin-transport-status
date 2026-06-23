resource "aws_scheduler_schedule" "collector" {
  name                = "${var.project_name}-collector"
  schedule_expression = "rate(5 minutes)"

  flexible_time_window {
    mode = "OFF"
  }

  target {
    arn      = aws_lambda_function.collector.arn
    role_arn = aws_iam_role.scheduler.arn
  }
}
