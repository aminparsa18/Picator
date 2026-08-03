namespace Picator.Common.Data.Enums;

/// <summary>
/// Matchmaking ticket status.
/// </summary>
public enum MatchTicketStatus
{
    /// <summary>
    /// Waiting to be paired.
    /// </summary>
    Queued = 0,

    /// <summary>
    /// Paired with another ticket; GameCode is set.
    /// </summary>
    Matched = 1,

    /// <summary>
    /// No pair found before the TTL elapsed.
    /// </summary>
    Expired = 2,

    /// <summary>
    /// Cancelled by the player before being matched.
    /// </summary>
    Cancelled = 3,
}
