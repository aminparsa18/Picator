namespace Picator.Entities.Models;

/// <summary>
/// Database object base entity.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Primary key identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Object creation date.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Object modification date.
    /// </summary>
    public DateTime ModifiedDate { get; set; }
}