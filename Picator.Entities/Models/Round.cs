using Picator.Common.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace Picator.Entities.Models;

/// <summary>
/// A single player's drawing turn within a game.
/// </summary>
public sealed class Round : BaseEntity
{
    /// <summary>
    /// Game key identifier.
    /// </summary>
    public Guid GameId { get; set; }

    /// <summary>
    /// Round number within the game.
    /// </summary>
    public int RoundNumber { get; set; }

    /// <summary>
    /// Game member key identifier of the drawer for this round.
    /// </summary>
    public Guid DrawerGameMemberId { get; set; }

    /// <summary>
    /// Word to draw, picked randomly from the word bank when the round starts.
    /// </summary>
    [Required]
    public string Word { get; set; } = default!;

    /// <summary>
    /// Round status.
    /// </summary>
    public RoundStatus Status { get; set; }

    /// <summary>
    /// Round start date.
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Round end date.
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Game.
    /// </summary>
    public Game? Game { get; set; }

    /// <summary>
    /// Game member drawing this round.
    /// </summary>
    public GameMember? DrawerGameMember { get; set; }
}
