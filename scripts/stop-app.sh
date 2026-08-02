#!/usr/bin/env bash
# Stop the Aspire AppHost and terminate the running app on the Android device
# (also ends any attached VS Code debug session). The `adb reverse` port
# forward is removed; the device itself is left connected.
set -euo pipefail
cd "$(dirname "$0")/.."

SERIAL="${1:-R5CW50YDGAB}"  # Galaxy S23 Ultra
PACKAGE="com.parsoft.picator"
APPHOST="Picator.AppHost/Picator.AppHost.csproj"
API_PORT="7106"

echo "Stopping Aspire AppHost..."
aspire stop --apphost "$APPHOST" --non-interactive --nologo || true

adb -s "$SERIAL" shell am force-stop "$PACKAGE" || true
adb -s "$SERIAL" reverse --remove "tcp:$API_PORT" || true
