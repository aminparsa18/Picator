using MemoryPack;

namespace Picator.Common.Data.Dtos.Users;

/// <summary>
/// User status dto.
/// </summary>
[MemoryPackable]
public sealed partial class UserStatusResult
{
    /// <summary>
    /// Wins count as mafia.
    /// </summary>
    public double MafiaWin { get; set; }

    /// <summary>
    /// Wins count as citizen.
    /// </summary>
    public double CitizenWin { get; set; }

    /// <summary>
    /// Total wins count.
    /// </summary>
    public double TotalWin { get; set; }
}