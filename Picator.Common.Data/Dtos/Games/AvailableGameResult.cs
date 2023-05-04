using MemoryPack;

namespace Picator.Common.Data.Dtos.Games;

/// <summary>
/// Game dto.
/// </summary>
[MemoryPackable]
public sealed partial class AvailableGameResult
{
    /// <summary>
    /// Game key identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Room key identifier.
    /// </summary>
    public Guid RoomId { get; set; }

    /// <summary>
    /// Game member count.
    /// </summary>
    public short MemberCount { get; set; }
}