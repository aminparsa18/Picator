#!/usr/bin/env bash
# Build the MAUI app for iOS, launch it in the Simulator, then start the
# Aspire AppHost (Debug) so the app has a local backend to talk to.
#
# `dotnet build -t:Run -f net10.0-ios` cannot be used on this Intel Mac: an
# upstream dotnet/macios bug in GetAvailableDevices.cs discards every
# simulator device once it sees "arm64" in the runtime's supported
# architectures list, even when a valid x86_64 identifier was already found.
# So we build plain, then drive simctl ourselves.
set -euo pipefail
cd "$(dirname "$0")/.."

UDID="${1:-AE3F49B1-3BBE-40F3-B158-164EF2F4430A}"  # iPhone 16 Pro
BUNDLE_ID="club.picator.gamev2"
APP_PATH="Picator.GameV2/bin/Debug/net10.0-ios/iossimulator-x64/Picator.Game.app"
APPHOST="Picator.AppHost/Picator.AppHost.csproj"

dotnet build Picator.GameV2/Picator.Game.csproj -f net10.0-ios -c Debug

if [ "$(xcrun simctl list devices | grep "$UDID" | grep -c Booted)" -eq 0 ]; then
    xcrun simctl boot "$UDID"
fi
open -a Simulator

xcrun simctl install "$UDID" "$APP_PATH"
xcrun simctl launch "$UDID" "$BUNDLE_ID"

echo "Starting Aspire AppHost (Debug)..."
aspire start --apphost "$APPHOST" --non-interactive --nologo
