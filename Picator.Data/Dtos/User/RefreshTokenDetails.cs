namespace Picator.Data.Dtos.User;

/// <summary>
/// Refresh token dto.
/// </summary>
public sealed class RefreshTokenDetails
{
    /// <summary>
    /// Refresh token key identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Jwt key identifier.
    /// </summary>
    public string? JwtId { get; set; }

    /// <summary>
    /// Token expiration date.
    /// </summary>
    public DateTime ExpirationDate { get; set; }

    /// <summary>
    /// Indicating token is already used.
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// Indicating token is already invalidated.
    /// </summary>
    public bool IsInvalidated { get; set; }
}