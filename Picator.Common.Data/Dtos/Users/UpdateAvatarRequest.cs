using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// Update avatar dto.
/// </summary>
[MemoryPackable]
public sealed partial class UpdateAvatarRequest
{
    /// <summary>
    /// Avatar file name.
    /// </summary>
    public string? Image { get; set; }
}
