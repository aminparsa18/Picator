using MemoryPack;

namespace Picator.Common.Data.Dtos.Rooms;

/// <summary>
/// Room create result dto.
/// </summary>
[MemoryPackable]
public sealed partial class RoomCreateResult
{
    /// <summary>
    /// Room key identifier.
    /// </summary>
    public Guid Id { get; set; }
}