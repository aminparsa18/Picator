using MemoryPack;

namespace Picator.Common.Data.Dtos.Games;

/// <summary>
/// Game create dto.
/// </summary>
[MemoryPackable]
public sealed partial class GameCreateRequest
{
    /// <summary>
    /// Room key identifier.
    /// </summary>
    public Guid? RoomId { get; set; }
}