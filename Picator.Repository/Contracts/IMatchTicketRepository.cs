using Picator.Common.Data.Enums;

namespace Picator.Repository.Contracts;

/// <summary>
/// Repository provides methods to retrieve/handle matchmaking ticket data.
/// </summary>
public interface IMatchTicketRepository : IBaseRepository<MatchTicket>
{
    /// <summary>
    /// Retrieves the oldest queued tickets for a format, oldest first.
    /// </summary>
    /// <param name="format">Match format.</param>
    /// <param name="take">Max tickets to return.</param>
    Task<List<MatchTicket>> GetOldestQueued(GameFormat format, int take);

    /// <summary>
    /// Atomically marks both tickets Matched with the given game code, provided both are still Queued.
    /// </summary>
    /// <returns>True if both tickets were claimed; false if one was already taken by a concurrent pairing attempt.</returns>
    Task<bool> TryClaimPair(Guid ticketId1, Guid ticketId2, string gameCode);

    /// <summary>
    /// Marks all Queued tickets older than the TTL as Expired.
    /// </summary>
    /// <returns>Number of tickets expired.</returns>
    Task<int> ExpireStale(TimeSpan ttl);
}
