using Picator.Entities.Identity;
using System.ComponentModel.DataAnnotations;

namespace Picator.Entities.Models;

/// <summary>
/// Refresh token.
/// </summary>
public sealed class RefreshToken : BaseEntity
{
    /// <summary>
    /// Jwt token.
    /// </summary>
    [Required]
    public string Token { get; set; } = default!;

    /// <summary>
    /// Jwt key identifier.
    /// </summary>
    [Required]
    public string JwtId { get; set; } = default!;

    /// <summary>
    /// Token expiration date.
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Indicates token is already used.
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// Indicates token is already invalidated.
    /// </summary>
    public bool IsInvalidated { get; set; }

    /// <summary>
    /// User key identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// User.
    /// </summary>
    public User? User { get; set; }
}