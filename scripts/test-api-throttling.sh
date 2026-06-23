#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
api_gateway="$repo_root/infra/api-gateway.tf"
variables="$repo_root/infra/variables.tf"

if ! rg -q 'default_route_settings\s*\{' "$api_gateway"; then
  echo "API Gateway must define default route throttling." >&2
  exit 1
fi

if ! rg -q 'throttling_rate_limit\s*=\s*var\.api_throttling_rate_limit' "$api_gateway"; then
  echo "API Gateway must configure a sustained request rate limit." >&2
  exit 1
fi

if ! rg -q 'throttling_burst_limit\s*=\s*var\.api_throttling_burst_limit' "$api_gateway"; then
  echo "API Gateway must configure a burst request limit." >&2
  exit 1
fi

if ! rg -q 'variable "api_throttling_rate_limit"' "$variables"; then
  echo "Terraform must expose the API throttling rate limit." >&2
  exit 1
fi

if ! rg -q 'variable "api_throttling_burst_limit"' "$variables"; then
  echo "Terraform must expose the API throttling burst limit." >&2
  exit 1
fi

echo "API Gateway throttling is configured."
