using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// Update display name dto.
/// </summary>
[MemoryPackable]
public sealed partial class UpdateDisplayNameRequest
{
    /// <summary>
    /// New display name.
    /// </summary>
    public string? Name { get; set; }
}
