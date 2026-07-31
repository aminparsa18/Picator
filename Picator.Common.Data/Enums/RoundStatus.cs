namespace Picator.Common.Data.Enums;

/// <summary>
/// Round status.
/// </summary>
public enum RoundStatus
{
    /// <summary>
    /// Pending, not started yet.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Active, currently being drawn and guessed.
    /// </summary>
    Active = 1,

    /// <summary>
    /// Completed.
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Skipped.
    /// </summary>
    Skipped = 3,
}
