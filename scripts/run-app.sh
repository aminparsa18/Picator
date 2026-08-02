#!/usr/bin/env bash
# Build the MAUI app for Android, deploy it to a connected device, then start
# the Aspire AppHost (Debug) so the app has a local backend to talk to.
#
# Also sets up `adb reverse` so the device can reach the Aspire-hosted API at
# https://localhost:<port> over the USB connection.
set -euo pipefail
cd "$(dirname "$0")/.."

SERIAL="${1:-R5CW50YDGAB}"  # Galaxy S23 Ultra
PACKAGE="com.parsoft.picator"
APPHOST="Picator.AppHost/Picator.AppHost.csproj"
API_PORT="7106"

if ! adb devices | grep -q "^${SERIAL}[[:space:]]*device$"; then
    echo "Error: device $SERIAL not connected (check 'adb devices')." >&2
    exit 1
fi

dotnet build Picator.GameV2/Picator.Game.csproj -f net10.0-android -t:Run -c Debug \
    -p:AdbTarget="-s $SERIAL"

adb -s "$SERIAL" reverse "tcp:$API_PORT" "tcp:$API_PORT"

echo "Starting Aspire AppHost (Debug)..."
aspire start --apphost "$APPHOST" --non-interactive --nologo
