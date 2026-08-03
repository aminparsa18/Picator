using MagicOnion;
using Picator.Common.Data.Enums;

namespace Picator.Realtime.Common.Services;

public interface IMatchmakingHub : IStreamingHub<IMatchmakingHub, IMatchFoundReceiver>
{
    /// <summary>
    /// Joins the matchmaking queue for the given format. Returns the caller's authenticated user id
    /// (resolved server-side from the JWT, not client-supplied) so the client can reuse it as the real
    /// playerId when it subsequently joins GameHub with the game code from OnMatchFound.
    /// </summary>
    ValueTask<Guid> EnterQueueAsync(GameFormat format);

    /// <summary>
    /// Cancels the caller's active ticket, if any.
    /// </summary>
    ValueTask CancelQueueAsync();
}
