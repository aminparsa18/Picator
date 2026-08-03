using Picator.Common.Data.Enums;
using Picator.Entities.Identity;

namespace Picator.Entities.Models;

/// <summary>
/// A player's spot in the random-matchmaking queue.
/// </summary>
public sealed class MatchTicket : BaseEntity
{
    /// <summary>
    /// User key identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Match format being searched for.
    /// </summary>
    public GameFormat Format { get; set; }

    /// <summary>
    /// Ticket status.
    /// </summary>
    public MatchTicketStatus Status { get; set; }

    /// <summary>
    /// Shared code both paired players use to join the live drawing round (GameHub.JoinGameAsync). Set when Status becomes Matched.
    /// </summary>
    public string? GameCode { get; set; }

    /// <summary>
    /// User.
    /// </summary>
    public User? User { get; set; }
}
