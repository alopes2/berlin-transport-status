data "archive_file" "lambda_bootstrap" {
  type                    = "zip"
  output_path             = "${path.module}/.terraform/lambda-bootstrap.zip"
  source_content          = "Application code is deployed by GitHub Actions."
  source_content_filename = "README.txt"
}

resource "aws_lambda_function" "collector" {
  function_name = "${var.project_name}-collector"
  role          = aws_iam_role.lambda.arn
  runtime       = "dotnet10"
  handler       = "TransportStatus::TransportStatus.Functions.CollectorFunction::HandleAsync"
  filename      = data.archive_file.lambda_bootstrap.output_path
  memory_size   = 2048
  timeout       = 30

  environment {
    variables = {
      STATUS_TABLE   = aws_dynamodb_table.status.name
      TRACKING_SINCE = var.tracking_since
    }
  }

  depends_on = [aws_cloudwatch_log_group.collector]

  lifecycle {
    ignore_changes = [filename, source_code_hash]
  }
}

resource "aws_lambda_function" "api" {
  function_name    = "${var.project_name}-api"
  role             = aws_iam_role.lambda.arn
  runtime          = "dotnet10"
  handler          = "TransportStatus::TransportStatus.Functions.StatusApiFunction::HandleAsync"
  filename         = data.archive_file.lambda_bootstrap.output_path
  source_code_hash = data.archive_file.lambda_bootstrap.output_base64sha256
  memory_size      = 256
  timeout          = 10

  environment {
    variables = {
      STATUS_TABLE = aws_dynamodb_table.status.name
    }
  }

  depends_on = [aws_cloudwatch_log_group.api]

  lifecycle {
    ignore_changes = [filename, source_code_hash]
  }
}
