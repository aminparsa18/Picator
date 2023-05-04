namespace Picator.Data;

/// <summary>
/// Pagination arguments.
/// </summary>
public sealed class PaginationArgs
{
    /// <summary>
    /// Page row count.
    /// </summary>
    public int PageRowCount { get; set; }

    /// <summary>
    /// Starting row.
    /// </summary>
    public int StartingRow { get; set; }
}