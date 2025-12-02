using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// User dto.
/// </summary>
[MemoryPackable]
public sealed partial class UserDetailsResult
{
    /// <summary>
    /// Display name.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Avatar.
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// Score.
    /// </summary>
    public int Score { get; set; }
}