using Picator.Common.Data.Enums;
using Picator.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Picator.Entities.Models;

/// <summary>
/// Room.
/// </summary>
public sealed class Room : BaseEntity
{
    /// <summary>
    /// Room name.
    /// </summary>
    [Required] 
    public string Name { get; set; } = default!;

    /// <summary>
    /// Room owner.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Room code.
    /// </summary>
    [Required] 
    public string Code { get; set; } = default!;

    /// <summary>
    /// Match format (Solo or Teams), fixed for the room's lifetime and snapshotted onto each Game it creates.
    /// </summary>
    public GameFormat Format { get; set; }

    /// <summary>
    /// User.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Collection of games.
    /// </summary>
    public ICollection<Game>? Game { get; set; }

    /// <summary>
    /// Collection of room members.
    /// </summary>
    public ICollection<RoomMember>? RoomMember { get; set; }
}