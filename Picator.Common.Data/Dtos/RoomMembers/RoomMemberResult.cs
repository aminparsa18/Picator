using MemoryPack;

namespace Picator.Common.Data.Dtos.RoomMembers;

/// <summary>
/// Room member dto.
/// </summary>
[MemoryPackable]
public sealed partial class RoomMemberResult
{
    /// <summary>
    /// Room member user key identifier.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Avatar.
    /// </summary>
    public string? Avatar { get; set; }

    /// <summary>
    /// Total game played count.
    /// </summary>
    public int TotalGame { get; set; }

    /// <summary>
    /// Member level.
    /// </summary>
    public int Level { get; set; }
}