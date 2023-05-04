using Picator.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Picator.Entities.Models;

/// <summary>
/// Game messages.
/// </summary>
public sealed class GameMessage : BaseEntity
{
    /// <summary>
    /// Game key identifier.
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// Game owner user key identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Game message content.
    /// </summary>
    [Required]
    public string Content { get; set; } = default!;

    /// <summary>
    /// Game.
    /// </summary>
    public Game? Game { get; set; }

    /// <summary>
    /// User.
    /// </summary>
    public User? User { get; set; }
}