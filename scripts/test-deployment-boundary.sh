#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
infra="$repo_root/infra"
workflow="$repo_root/.github/workflows/deploy.yml"

if grep -Eq 'var\.lambda_zip_path|source_code_hash[[:space:]]*=[[:space:]]*filebase64sha256' "$infra"/*.tf; then
  echo "Terraform must not deploy the application Lambda ZIP." >&2
  exit 1
fi

if ! grep -Eq 'ignore_changes[[:space:]]*=[[:space:]]*\[[^]]*source_code_hash' "$infra/lambda.tf"; then
  echo "Terraform must ignore Lambda application code changes." >&2
  exit 1
fi

update_count="$(grep -c 'aws lambda update-function-code' "$workflow" || true)"
if [[ "$update_count" -ne 2 ]]; then
  echo "GitHub Actions must deploy both Lambda functions." >&2
  exit 1
fi

if ! grep -q 'collector_function_name' "$infra/outputs.tf"; then
  echo "Terraform must output the collector Lambda function name." >&2
  exit 1
fi

if ! grep -q 'api_function_name' "$infra/outputs.tf"; then
  echo "Terraform must output the API Lambda function name." >&2
  exit 1
fi

echo "Lambda deployment ownership is correctly separated."
