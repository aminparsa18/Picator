using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// Update profile dto.
/// </summary>
[MemoryPackable]
public sealed partial class UpdateProfileRequest
{
    /// <summary>
    /// Name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Image.
    /// </summary>
    public string? Image { get; set; }
}