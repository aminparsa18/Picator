using MemoryPack;

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
    /// Indicating room is private.
    /// </summary>
    public bool IsPrivate { get; set; }

    /// <summary>
    /// List of users.
    /// </summary>
    public List<Guid>? Users { get; set; }
}