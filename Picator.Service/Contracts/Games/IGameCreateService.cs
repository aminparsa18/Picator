using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Games;

namespace Picator.Service.Contracts.Games;

public interface IGameCreateService
{
    Task<ApiResult<GameCreateResult>> Create(GameCreateRequest request);

    /// <summary>
    /// Eagerly creates a matched quick-match game: the game, both game members, and the first active round,
    /// with drawer/guesser roles already assigned. Called by matchmaking right after pairing, before either
    /// client is notified.
    /// </summary>
    Task CreateMatchedGame(string gameCode, Guid drawerUserId, Guid guesserUserId);

    /// <summary>
    /// Handles a player joining the game hub for the given game code. Serves both the pre-matched quick-match
    /// flow (game/round already exist) and the legacy join-by-code flow (game is created/completed on join).
    /// </summary>
    Task<GameJoinOutcome> JoinGame(string gameCode, Guid userId);

    /// <summary>
    /// Validates a guess against the active round's word, persists it, and awards score/completes the round
    /// on a correct guess.
    /// </summary>
    Task<GuessOutcome> SubmitGuess(string gameCode, Guid userId, string guess);

    /// <summary>
    /// Called when a client's local round timer runs out. Ends the active round scorelessly (no-op if it
    /// was already resolved by a guess or the other player's timeout call) and advances/completes the game.
    /// </summary>
    Task<GuessOutcome> TimeoutRound(string gameCode, Guid userId);
}