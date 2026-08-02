#!/usr/bin/env bash
# Terminate the running app on the Android device (also ends any attached
# VS Code debug session). The `adb reverse` port forward is removed; the
# device itself is left connected.
set -euo pipefail
cd "$(dirname "$0")/.."

SERIAL="${1:-R5CW50YDGAB}"  # Galaxy S23 Ultra
PACKAGE="com.parsoft.picator"
API_PORT="7106"
RUSTFS_PORT="9100"

adb -s "$SERIAL" shell am force-stop "$PACKAGE" || true
adb -s "$SERIAL" reverse --remove "tcp:$API_PORT" || true
adb -s "$SERIAL" reverse --remove "tcp:$RUSTFS_PORT" || true
