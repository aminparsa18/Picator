using Picator.Common.Data.Dtos.GameMembers;

namespace Picator.Repository.Contracts;

/// <summary>
/// Repository provides methods to retrieve/handle chat game member data.
/// </summary>
public interface IGameMemberRepository : IBaseRepository<GameMember>
{
    /// <summary>
    /// Retrieves game members by game.
    /// </summary>
    /// <param name="gameId">Game key identifier.</param>
    /// <returns>List of game members.</returns>
    Task<List<GameMemberResult>> GetByGame(Guid gameId);

    /// <summary>
    /// Retrieves game members by game.
    /// </summary>
    /// <param name="gameId">Game key identifier.</param>
    /// <returns>List of game members.</returns>
    Task<List<GameMemberResult>> GetByGameFast(Guid gameId);

    /// <summary>
    /// Join a game.
    /// </summary>
    /// <param name="userId">User key identifier.</param>
    /// <param name="memberId">Game member key identifier.</param>
    /// <returns>Number of affected rows.</returns>
    Task<int> Join(Guid userId, Guid memberId);

    /// <summary>
    /// Leave a game.
    /// </summary>
    /// <param name="userId">User key identifier.</param>
    /// <returns>Number of affected rows.</returns>
    Task<int> Leave(Guid userId);
}