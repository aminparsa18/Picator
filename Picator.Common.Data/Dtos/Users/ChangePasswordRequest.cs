using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// Change password dto.
/// </summary>
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
