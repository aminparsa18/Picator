using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// Register user dto.
/// </summary>
[MemoryPackable]
public sealed partial class RegisterUserRequest
{
    /// <summary>
    /// Username.
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Display name.
    /// </summary>
    public string? DisplayName { get; set; }
}