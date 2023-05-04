using MemoryPack;

namespace Picator.Common.Data.Dtos.Rooms;

/// <summary>
/// Leave game dto.
/// </summary>
[MemoryPackable]
public sealed partial class LeaveRoomRequest
{
    // Room key identifier.
    public Guid RoomId { get; set; }
}