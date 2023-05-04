using MemoryPack;

namespace Picator.Common.Data.Dtos.Rooms;

/// <summary>
/// Update room image dto.
/// </summary>
[MemoryPackable]
public sealed partial class UpdateRoomNameRequest
{
    /// <summary>
    /// Room key identifier.
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Image name.
    /// </summary>
    public string? Name { get; set; }
}