using Picator.Common.Data.Dtos.Games;

namespace Picator.Repository.Contracts;

/// <summary>
/// Repository provides methods to retrieve/handle game data.
/// </summary>
public interface IGameRepository : IBaseRepository<Game>
{
    /// <summary>
    /// Retrieves available games to play.
    /// </summary>
    /// <returns>List of available games.</returns>
    Task<List<AvailableGameResult>> GetAvailables();

    /// <summary>
    /// Rertieves game member key identifier if is joined.
    /// </summary>
    /// <param name="userId">User key identifier.</param>
    /// <param name="gameId">Game key identifier.</param>
    /// <returns>Game member key identifier.</returns>
    Task<string?> IsJoinedFast(string userId, string gameId);

    /// <summary>
    /// Retrieves game member key identifier if is already playing.
    /// </summary>
    /// <param name="roomId">Room key identifier.</param>
    /// <returns></returns>
    Task<string?> IsAlreadyPlaying(string roomId);

    /// <summary>
    /// Update flag of game status to started.
    /// </summary>
    /// <param name="gameId">Game key identifier.</param>
    /// <returns>Number of affected rows.</returns>
    Task<int> StartGame(string gameId);
}