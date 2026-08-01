#!/usr/bin/env bash
# Stop the Aspire AppHost and terminate the running app on the iOS Simulator
# (also ends any attached VS Code debug session). The Simulator itself and
# the booted device are left running — only the app and Aspire are stopped.
set -euo pipefail
cd "$(dirname "$0")/.."

UDID="${1:-AE3F49B1-3BBE-40F3-B158-164EF2F4430A}"  # iPhone 16 Pro
BUNDLE_ID="club.picator.gamev2"
APPHOST="Picator.AppHost/Picator.AppHost.csproj"

echo "Stopping Aspire AppHost..."
aspire stop --apphost "$APPHOST" --non-interactive --nologo || true

xcrun simctl terminate "$UDID" "$BUNDLE_ID"
