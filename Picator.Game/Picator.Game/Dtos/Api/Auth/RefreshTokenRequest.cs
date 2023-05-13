using MemoryPack;

namespace Picator.Common.Data.Dtos.Api.Auth;

/// <summary>
/// Request to generate new token with refresh token.
/// </summary>
[MemoryPackable]
public sealed partial class RefreshTokenRequest
{
    /// <summary>
    /// Jwt expired token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Refresh token needed for refresh expired token.
    /// </summary>
    public string? RefreshToken { get; set; }
}