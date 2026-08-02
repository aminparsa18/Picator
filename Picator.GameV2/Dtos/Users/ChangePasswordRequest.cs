using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// Change password dto.
/// </summary>
/// <remarks>
/// TODO: no backend endpoint consumes this yet ("users/change-password" is not implemented
/// server-side). Added for the client-side Change Password UI ahead of server support.
/// </remarks>
[MemoryPackable]
public sealed partial class ChangePasswordRequest
{
    /// <summary>
    /// Current password.
    /// </summary>
    public string? CurrentPassword { get; set; }

    /// <summary>
    /// New password.
    /// </summary>
    public string? NewPassword { get; set; }
}
