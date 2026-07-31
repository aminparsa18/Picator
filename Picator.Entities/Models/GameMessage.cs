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
    /// Round key identifier, set when this message is a guess submitted during a round.
    /// </summary>
    public Guid? RoundId { get; set; }

    /// <summary>
    /// Indicates whether this message is a correct guess.
    /// </summary>
    public bool IsCorrectGuess { get; set; }

    /// <summary>
    /// Points awarded for this guess.
    /// </summary>
    public int PointsAwarded { get; set; }

    /// <summary>
    /// Game.
    /// </summary>
    public Game? Game { get; set; }

    /// <summary>
    /// Round this message belongs to, if it is a guess.
    /// </summary>
    public Round? Round { get; set; }

    /// <summary>
    /// User.
    /// </summary>
    public User? User { get; set; }
}