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
    /// Indicates room is private (joinable only with invitation).
    /// </summary>
    public bool IsPrivate { get; set; }

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