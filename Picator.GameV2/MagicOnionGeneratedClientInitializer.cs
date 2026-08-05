using System.Runtime.CompilerServices;
using MagicOnion.Client;
using MessagePack;
using MessagePack.Resolvers;
using Picator.Realtime.Common.Services;

namespace Picator.GameV2;

// Forces MagicOnion's client Source Generator to emit static StreamingHub client
// proxies for these hubs at COMPILE TIME, instead of StreamingHubClient.ConnectAsync
// falling back to DynamicStreamingHubClientBuilder's runtime Reflection.Emit codegen.
//
// That dynamic path is what's actually crashing in Release
// (TypeInitializationException / InvalidOperationException: NoElements in
// DefineConstructor, enumerating StreamingHubClientBase<,>'s constructors via
// TypeBuilder.BaseType.GetConstructors()) -- confirmed NOT caused by IL trimming or
// AOT (identical failure with AndroidLinkMode=SdkOnly, None, and RunAOTCompilation=
// true), so it's a Mono Reflection.Emit limitation with TypeBuilder over a closed
// generic base type. The static generated client sidesteps Reflection.Emit entirely,
// per MagicOnion's own AOT/IL2CPP guidance: https://cysharp.github.io/MagicOnion/fundamentals/aot
//
// One attribute application listing both hubs, not one partial class per hub: the
// attribute's ctor takes a `params Type[]`, and giving each hub its own separate
// [MagicOnionClientGeneration] application (even on two different classes) makes
// MagicOnionClientSourceGenerator 7.10.2 try to emit the shared GameHubClient output
// file twice in the same compilation, which throws
// ArgumentException("The hintName '...GameHubClient.g.cs' ... must be unique") and
// silently drops ALL generated members (including .Resolver) -- confirmed via
// -p:EmitCompilerGeneratedFiles=true;CompilerGeneratedFilesOutputPath=generated and
// CS8785 in the build log. A single attribute application with both types avoids it.
[MagicOnionClientGeneration(typeof(IMatchmakingHub), typeof(IGameHub))]
partial class MagicOnionGeneratedClientInitializer { }

// Registering the class above only swaps in the static, Reflection.Emit-free hub
// client (auto-registered via a generator-emitted ModuleInitializer). It does NOT
// register the generated static MessagePack resolver -- that's a manual step per
// MagicOnion's own docs (https://cysharp.github.io/MagicOnion/source-generator/client).
// Without it, MessagePack falls back to its own dynamic resolver chain (e.g.
// DynamicEnumResolver for plain enums like GameFormat, used by
// IMatchmakingHub.EnterQueueAsync), which is *also* Reflection.Emit-based and would
// hit this exact crash pattern one call later.
static class MagicOnionGeneratedResolverRegistration
{
    [ModuleInitializer]
    internal static void RegisterResolvers()
    {
        StaticCompositeResolver.Instance.Register(
            MagicOnionGeneratedClientInitializer.Resolver,
            StandardResolver.Instance);

        MessagePackSerializer.DefaultOptions =
            MessagePackSerializer.DefaultOptions.WithResolver(StaticCompositeResolver.Instance);
    }
}
