using MemoryPack;

namespace Picator.Common.Data.Dtos.Rooms;

/// <summary>
/// Room details result.
/// </summary>
[MemoryPackable]
public sealed partial class RoomDetailsResult
{
    /// <summary>
    /// Room key identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Code.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Member count.
    /// </summary>
    public short MemberCount { get; set; }

    /// <summary>
    /// Game played count.
    /// </summary>
    public int GamePlayedCount { get; set; }

    /// <summary>
    /// Indicating if requester user is admin of room.
    /// </summary>
    public bool IsAdmin { get; set; }
}