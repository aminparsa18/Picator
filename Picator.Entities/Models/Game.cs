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
    /// Game capacity.
    /// </summary>
    public short? Capacity { get; set; }

    /// <summary>
    /// Word.
    /// </summary>
    public string GameCode { get; set; } = default!;

    /// <summary>
    /// Word.
    /// </summary>
    public string? Word { get; set; }

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
}