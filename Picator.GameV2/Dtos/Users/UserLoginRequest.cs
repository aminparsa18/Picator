using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// User login dto.
/// </summary>
[MemoryPackable]
public sealed partial class UserLoginRequest
{
    /// <summary>
    /// Username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    public string? Password { get; set; }
}