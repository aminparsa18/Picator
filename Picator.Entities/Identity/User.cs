using Microsoft.AspNetCore.Identity;
using Picator.Entities.Models;
using System.ComponentModel.DataAnnotations;

namespace Picator.Entities.Identity;

/// <summary>
/// User.
/// </summary>
public sealed class User : IdentityUser<Guid>
{
    /// <summary>
    /// User key identifier.
    /// </summary>
    public override Guid Id { get; set; }

    /// <summary>
    /// User code.
    /// </summary>
    [Required]
    public string Code { get; set; } = default!;

    /// <summary>
    /// Display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Indicates if user is suspended.
    /// </summary>
    public bool IsSuspended { get; set; }

    /// <summary>
    /// Image.
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// Score.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Collection of game members.
    /// </summary>
    public ICollection<GameMember>? GameMember { get; set; }

    /// <summary>
    /// Collection of game messages.
    /// </summary>
    public ICollection<GameMessage>? GameMessage { get; set; }

    /// <summary>
    /// Collection of refresh tokens.
    /// </summary>
    public ICollection<RefreshToken>? RefreshToken { get; set; }

    /// <summary>
    /// Collectio of rooms.
    /// </summary>
    public ICollection<Room>? Room { get; set; }

    /// <summary>
    /// Collection of room members.
    /// </summary>
    public ICollection<RoomMember>? RoomMember { get; set; }

    /// <summary>
    /// Collection of matchmaking tickets.
    /// </summary>
    public ICollection<MatchTicket>? MatchTicket { get; set; }

    /// <summary>
    /// Collection of user roles.
    /// </summary>
    public ICollection<UserRole>? Roles { get; set; }

    /// <summary>
    /// Collection of user claims.
    /// </summary>
    public ICollection<UserClaim>? Claims { get; set; }
}