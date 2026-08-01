using Picator.Common.Data.Enums;

namespace Picator.Entities.Models;

/// <summary>
/// Game.
/// </summary>
public sealed class Game : BaseEntity
{
    /// <summary>
    /// Game room key identifier.
    /// </summary>
    public Guid? RoomId { get; set; }

    /// <summary>
    /// Word.
    /// </summary>
    public string GameCode { get; set; } = default!;

    /// <summary>
    /// Game status.
    /// </summary>
    public GameStatus Status { get; set; }

    /// <summary>
    /// Match format (Solo or Teams). Snapshotted from Room.Format at creation; always Solo for quick-match games (no Room).
    /// </summary>
    public GameFormat Format { get; set; }

    /// <summary>
    /// Number of times each player draws.
    /// </summary>
    public int RoundsPerPlayer { get; set; }

    /// <summary>
    /// Duration of each round, in seconds.
    /// </summary>
    public int RoundDurationSeconds { get; set; }

    /// <summary>
    /// Room.
    /// </summary>
    public Room? Room { get; set; }

    /// <summary>
    /// Collection of game members.
    /// </summary>
    public ICollection<GameMember>? GameMember { get; set; }

    /// <summary>
    /// Collection of game messages.
    /// </summary>
    public ICollection<GameMessage>? GameMessage { get; set; }

    /// <summary>
    /// Collection of rounds.
    /// </summary>
    public ICollection<Round>? Round { get; set; }
}