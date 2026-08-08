using MemoryPack;

namespace Picator.Common.Data.Dtos.Api.Auth;

/// <summary>
/// Request to log out and invalidate the current refresh token.
/// </summary>
[MemoryPackable]
public sealed partial class LogoutRequest
{
    /// <summary>
    /// Refresh token to invalidate.
    /// </summary>
    public string? RefreshToken { get; set; }
}
