#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
publish_dir="$repo_root/artifacts/lambda-publish"
zip_path="$repo_root/artifacts/transport-status.zip"

rm -rf "$publish_dir" "$zip_path"
mkdir -p "$publish_dir"

dotnet publish \
  "$repo_root/backend/src/TransportStatus/TransportStatus.csproj" \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained false \
  --output "$publish_dir"

(
  cd "$publish_dir"
  zip -qr "$zip_path" .
)

echo "$zip_path"

