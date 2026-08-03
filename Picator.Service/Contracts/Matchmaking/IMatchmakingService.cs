using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Matchmaking;
using Picator.Common.Data.Enums;

namespace Picator.Service.Contracts.Matchmaking;

public interface IMatchmakingService
{
    /// <summary>
    /// Adds the user to the matchmaking queue for the given format. Fails with Conflict if the user already has an active ticket.
    /// </summary>
    Task<ApiResult<MatchTicketResult>> EnqueueAsync(Guid userId, GameFormat format);

    /// <summary>
    /// Cancels an active (Queued) ticket owned by the user.
    /// </summary>
    Task<ApiResult> CancelAsync(Guid userId, Guid ticketId);

    /// <summary>
    /// Attempts to pair the two oldest queued tickets for a format. Returns the matched players' user ids and the
    /// shared game code on success, or null if there weren't at least two queued tickets or a concurrent claim won the race.
    /// </summary>
    Task<(Guid UserIdA, Guid UserIdB, string GameCode)?> TryPairAsync(GameFormat format);

    /// <summary>
    /// Expires all queued tickets older than the given TTL.
    /// </summary>
    /// <returns>Number of tickets expired.</returns>
    Task<int> ExpireStaleAsync(TimeSpan ttl);
}
