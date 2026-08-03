using MemoryPack;

namespace Picator.Common.Data.Dtos.Matchmaking;

/// <summary>
/// Result of enqueueing a matchmaking ticket.
/// </summary>
[MemoryPackable]
public sealed partial class MatchTicketResult
{
    /// <summary>
    /// Ticket key identifier.
    /// </summary>
    public Guid TicketId { get; set; }
}
