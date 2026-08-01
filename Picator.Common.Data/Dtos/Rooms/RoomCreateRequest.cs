using MemoryPack;
using Picator.Common.Data.Enums;

namespace Picator.Common.Data.Dtos.Rooms;

/// <summary>
/// Room create dto.
/// </summary>
[MemoryPackable]
public sealed partial class RoomCreateRequest
{
    /// <summary>
    /// Room name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Match format (Solo or Teams).
    /// </summary>
    public GameFormat Format { get; set; }

    /// <summary>
    /// List of users.
    /// </summary>
    public List<Guid>? Users { get; set; }
}