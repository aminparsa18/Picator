namespace Picator.Common.Server.Auth;

/// <summary>
/// Token result generated in token service.
/// </summary>
public class GenerateTokenResult
{
    /// <summary>
    /// Jwt identifier.
    /// </summary>
    public string? JwtId { get; set; }

    /// <summary>
    /// Jwt token.
    /// </summary>
    public string? Token { get; set; }
}