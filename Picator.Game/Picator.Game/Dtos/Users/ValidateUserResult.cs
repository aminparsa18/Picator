using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// Validated user dto.
/// </summary>
[MemoryPackable]
public sealed partial class ValidateUserResult
{
    /// <summary>
    /// User key identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Display name.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Image.
    /// </summary>
    public string? Image { get; set; }
}