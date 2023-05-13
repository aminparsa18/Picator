using MemoryPack;

namespace Picator.Common.Data.Dtos.Api.Auth;

/// <summary>
/// Api result for authentication api calls.
/// </summary>
[MemoryPackable]
public sealed partial class AuthResult : ApiResult
{
    /// <summary>
    /// Jwt token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Refresh token needed for refresh expired token.
    /// </summary>
    public string? RefreshToken { get; set; }
}