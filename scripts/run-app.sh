#!/usr/bin/env bash
# Build the MAUI app for Android and deploy it to a connected device.
#
# Also sets up `adb reverse` so the device can reach the locally running API,
# RustFS, and realtime (matchmaking/game hub) services at https://localhost:<port>
# over the USB connection. Start the Aspire AppHost separately (e.g. `aspire start`)
# before running this.
#
# Pass --watch to use `dotnet watch` instead of a one-shot build: it rebuilds
# and redeploys on file changes and streams the app's console output to this
# terminal, which is handy for watching logs while debugging. It blocks in
# the foreground until Ctrl+C.
set -euo pipefail
cd "$(dirname "$0")/.."

WATCH=false
if [[ "${1:-}" == "--watch" ]]; then
    WATCH=true
    shift
fi

SERIAL="${1:-R5CW50YDGAB}"  # Galaxy S23 Ultra
PACKAGE="com.parsoft.picator"
API_PORT="7106"
RUSTFS_PORT="9100"
REALTIME_PORT="5205"

if ! adb devices | grep -q "^${SERIAL}[[:space:]]*device$"; then
    echo "Error: device $SERIAL not connected (check 'adb devices')." >&2
    exit 1
fi

# Set up before deploying: watch mode blocks on the run below and never
# reaches code placed after it.
adb -s "$SERIAL" reverse "tcp:$API_PORT" "tcp:$API_PORT"
adb -s "$SERIAL" reverse "tcp:$RUSTFS_PORT" "tcp:$RUSTFS_PORT"
adb -s "$SERIAL" reverse "tcp:$REALTIME_PORT" "tcp:$REALTIME_PORT"

if $WATCH; then
    dotnet watch --project Picator.GameV2/Picator.Game.csproj -f net10.0-android \
        -p:AdbTarget="-s $SERIAL"
else
    dotnet build Picator.GameV2/Picator.Game.csproj -f net10.0-android -t:Run -c Debug \
        -p:AdbTarget="-s $SERIAL"
fi
