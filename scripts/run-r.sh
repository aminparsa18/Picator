#!/usr/bin/env bash
# Build the MAUI app for Android in Release mode and deploy it to a connected
# device -- for reproducing bugs that only show up in the release APK
# (Debug works fine, Release doesn't). Debug symbols and android:debuggable
# are forced on top of the Release config so you still get resolvable stack
# traces / debugger attachment, without disabling the Release-only behavior
# (linking, optimizations) that's presumably what's triggering the bug.
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

# Keep Release's normal linking/optimizations (that's the point -- reproduce
# what Release actually does) but force PDBs and a debuggable manifest so the
# result is still debuggable.
#
# RunAOTCompilation=true here overrides the csproj's Release-only
# <RunAOTCompilation>False</RunAOTCompilation> -- a command-line -p: property
# always wins over an unconditioned assignment in the project file, so this
# takes effect despite that. Matches what a real Play Store release build
# would actually run with, which plain -t:Run Release otherwise wouldn't.
#
# The quick-match TypeInitializationException/NoElements crash was MagicOnion's
# dynamic (Reflection.Emit) StreamingHubClient hitting a Mono TypeBuilder
# limitation building a proxy over a closed generic base type -- not trimming
# or AOT. Picator.GameV2/MagicOnionGeneratedClientInitializer.cs now forces
# MagicOnion's Source Generator to emit static, Reflection.Emit-free hub
# clients (and registers the matching static MessagePack resolver) instead,
# so quick match connecting + EnterQueueAsync should now succeed here too --
# if it doesn't, the dynamic-client path is being hit again somehow (check
# that the generator actually ran: rerun with
# -p:EmitCompilerGeneratedFiles=true;CompilerGeneratedFilesOutputPath=generated
# and confirm MagicOnionMatchmakingClientInitializer/MagicOnionGameClientInitializer
# have real generated bodies, not stubs).
#
# dotnet watch's option parser (unlike dotnet build's) chokes on repeated
# -p: flags ("Option '-p' expects a single argument but N were provided"),
# so fold everything into one semicolon-separated -p: property list, which
# both dotnet build and dotnet watch accept.
PROPS="AdbTarget=-s $SERIAL;DebugType=Portable;DebugSymbols=true;Debuggable=true;RunAOTCompilation=true"

if $WATCH; then
    dotnet watch --project Picator.GameV2/Picator.Game.csproj -f net10.0-android -c Release \
        -p:"$PROPS"
else
    dotnet build Picator.GameV2/Picator.Game.csproj -f net10.0-android -t:Run -c Release \
        -p:"$PROPS"
fi
