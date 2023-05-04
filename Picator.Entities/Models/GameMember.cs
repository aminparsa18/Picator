using Picator.Common.Data.Enums;
using Picator.Entities.Identity;

namespace Picator.Entities.Models;

/// <summary>
/// Game member.
/// </summary>
public sealed class GameMember : BaseEntity
{
    /// <summary>
    /// Game key identifier.
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// Game member user key identifier.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Player status.
    /// </summary>
    public PlayerStatus Status { get; set; }

    /// <summary>
    /// Game.
    /// </summary>
    public Game? Game { get; set; }

    /// <summary>
    /// User.
    /// </summary>
    public User? User { get; set; }
}