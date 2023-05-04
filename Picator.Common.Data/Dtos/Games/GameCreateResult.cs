using MemoryPack;

namespace Picator.Common.Data.Dtos.Games;

/// <summary>
/// Game creation result dto.
/// </summary>
[MemoryPackable]
public sealed partial class GameCreateResult
{
    /// <summary>
    /// Game key identifier.
    /// </summary>
    public Guid Id { get; set; }
}