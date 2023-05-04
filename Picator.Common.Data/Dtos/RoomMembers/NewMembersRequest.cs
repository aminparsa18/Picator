using MemoryPack;

namespace Picator.Common.Data.Dtos.RoomMembers;

/// <summary>
/// Add game member dto.
/// </summary>
[MemoryPackable]
public sealed partial class NewMembersRequest
{
    /// <summary>
    /// Room key identifier.
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// List of users.
    /// </summary>
    public List<Guid>? Users { get; set; }
}