using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// Register user dto.
/// </summary>
[MemoryPackable]
public sealed partial class RegisterUserRequest
{
    /// <summary>
    /// Phone number.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Password.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Country code.
    /// </summary>
    public string? CountryCode { get; set; }
}