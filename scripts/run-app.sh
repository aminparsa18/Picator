#!/usr/bin/env bash
# Build the MAUI app for Android and deploy it to a connected device.
#
# Also sets up `adb reverse` so the device can reach a locally running API at
# https://localhost:<port> over the USB connection. Start the Aspire AppHost
# separately (e.g. `aspire start`) before running this.
set -euo pipefail
cd "$(dirname "$0")/.."

SERIAL="${1:-R5CW50YDGAB}"  # Galaxy S23 Ultra
PACKAGE="com.parsoft.picator"
API_PORT="7106"

if ! adb devices | grep -q "^${SERIAL}[[:space:]]*device$"; then
    echo "Error: device $SERIAL not connected (check 'adb devices')." >&2
    exit 1
fi

dotnet build Picator.GameV2/Picator.Game.csproj -f net10.0-android -t:Run -c Debug \
    -p:AdbTarget="-s $SERIAL"

adb -s "$SERIAL" reverse "tcp:$API_PORT" "tcp:$API_PORT"
