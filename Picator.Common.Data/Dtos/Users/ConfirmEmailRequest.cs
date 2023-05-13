using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// Confirm email dto.
/// </summary>
[MemoryPackable]
public sealed partial class ConfirmEmailRequest
{
    /// <summary>
    /// Email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Jwt token.
    /// </summary>
    public string? Token { get; set; }
}