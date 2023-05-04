using MemoryPack;
using Picator.Common.Data.Enums;

namespace Picator.Common.Data.Dtos.GameMembers;

/// <summary>
/// Game member dto.
/// </summary>
[MemoryPackable]
public sealed partial class GameMemberResult
{
    /// <summary>
    /// Game member key identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Avatar.
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// Player score.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    ///Player status.
    /// </summary>
    public PlayerStatus Status { get; set; }
}