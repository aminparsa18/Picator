using System.ComponentModel.DataAnnotations;

namespace Picator.Entities.Models;

/// <summary>
/// Avatar picture of user.
/// </summary>
public sealed class Avatar : BaseEntity
{
    /// <summary>
    /// Avatar file name.
    /// </summary>
    [Required]
    public string Name { get; set; } = default!;
}