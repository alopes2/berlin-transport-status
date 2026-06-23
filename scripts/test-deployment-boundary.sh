#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
infra="$repo_root/infra"
workflow="$repo_root/.github/workflows/deploy.yml"

if rg -q 'var\.lambda_zip_path|source_code_hash\s*=\s*filebase64sha256' "$infra"; then
  echo "Terraform must not deploy the application Lambda ZIP." >&2
  exit 1
fi

if ! rg -q 'ignore_changes\s*=\s*\[[^]]*source_code_hash' "$infra/lambda.tf"; then
  echo "Terraform must ignore Lambda application code changes." >&2
  exit 1
fi

if [[ "$(rg -c 'aws lambda update-function-code' "$workflow")" -ne 2 ]]; then
  echo "GitHub Actions must deploy both Lambda functions." >&2
  exit 1
fi

if ! rg -q 'collector_function_name' "$infra/outputs.tf"; then
  echo "Terraform must output the collector Lambda function name." >&2
  exit 1
fi

if ! rg -q 'api_function_name' "$infra/outputs.tf"; then
  echo "Terraform must output the API Lambda function name." >&2
  exit 1
fi

echo "Lambda deployment ownership is correctly separated."
