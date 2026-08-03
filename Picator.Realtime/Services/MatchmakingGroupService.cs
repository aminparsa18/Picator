using Cysharp.Runtime.Multicast;
using Picator.Common.Data.Enums;
using Picator.Realtime.Common.Services;
using System.Collections.Concurrent;

namespace Picator.Realtime.Services;

/// <summary>
/// Owns one long-lived, application-managed MagicOnion group per match format, keyed by UserId.
/// Unlike StreamingHub-scoped groups, this is resolvable outside a live Hub call (constructor-injectable
/// IMulticastGroupProvider), which is what lets the TickerQ sweep notify players it pairs on its own.
/// See MagicOnion's "Application-managed Groups" docs (added v7.0.0).
/// </summary>
public sealed class MatchmakingGroupService : IDisposable
{
    private readonly ConcurrentDictionary<GameFormat, IMulticastSyncGroup<Guid, IMatchFoundReceiver>> _groups = new();
    private readonly IMulticastGroupProvider _groupProvider;

    public MatchmakingGroupService(IMulticastGroupProvider groupProvider)
    {
        _groupProvider = groupProvider;
    }

    public IMulticastSyncGroup<Guid, IMatchFoundReceiver> GetGroup(GameFormat format) =>
        _groups.GetOrAdd(format, f => _groupProvider.GetOrAddSynchronousGroup<Guid, IMatchFoundReceiver>($"matchmaking:{f}"));

    public void Dispose()
    {
        foreach (var group in _groups.Values)
            group.Dispose();
    }
}
