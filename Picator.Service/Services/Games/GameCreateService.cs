using FluentValidation;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Games;
using Picator.Common.Data.Enums;
using Picator.Entities.Models;
using Picator.Repository;
using Picator.Service.Contracts.Games;

namespace Picator.Service.Services.Games;

public class GameCreateService : IGameCreateService
{
    private const int GuesserPoints = 10;
    private const int DrawerPoints = 5;

    private readonly IValidator<GameCreateRequest> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public GameCreateService(IValidator<GameCreateRequest> validator, IUnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResult<GameCreateResult>> Create(GameCreateRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
            return new ApiResult<GameCreateResult>
            {
                StatusCode = ApiResultStatusCode.BadRequest,
                Errors = validationResult.Errors.Select(e => e.ErrorMessage)
            };
        var already = await _unitOfWork.Game.IsAlreadyPlaying(request.RoomId.ToString());
        if (!string.IsNullOrEmpty(already))
            return new ApiResult<GameCreateResult>()
            {
                IsSuccess = false,
                StatusCode = ApiResultStatusCode.Conflict,
                Errors = new[] { "Another game is already playing" }
            };

        //var game = _mapper.Map<GameCreateRequest, Game>(request);
        //await _unitOfWork.Game.AddFast(game);
        //var members = new List<GameMember>();
        //await _unitOfWork.GameMember.AddRangeFast(members);
        return new ApiResult<GameCreateResult>
        {
           // Data = new GameCreateResult() { Id = game.Id },
            IsSuccess = true
        };
    }

    public async Task CreateMatchedGame(string gameCode, Guid drawerUserId, Guid guesserUserId)
    {
        var word = await _unitOfWork.GameWord.GetRandomWord(new Random().Next(0, 150));

        var drawerMember = new GameMember { UserId = drawerUserId, Status = PlayerStatus.Playing };
        var guesserMember = new GameMember { UserId = guesserUserId, Status = PlayerStatus.Playing };

        var game = new Game
        {
            GameCode = gameCode,
            Status = GameStatus.InProgress,
            Format = GameFormat.Solo,
            RoundsPerPlayer = 1,
            GameMember = new List<GameMember> { drawerMember, guesserMember },
            Round = new List<Round>
            {
                new()
                {
                    RoundNumber = 1,
                    Word = word!,
                    Status = RoundStatus.Active,
                    StartedAt = DateTime.UtcNow,
                    DrawerGameMember = drawerMember,
                    ActiveGuesserGameMember = guesserMember
                }
            }
        };

        await _unitOfWork.Game.Add(game);
        await _unitOfWork.Commit();
    }

    public async Task<GameJoinOutcome> JoinGame(string gameCode, Guid userId)
    {
        var game = await _unitOfWork.Game.GetByGameCode(gameCode);

        if (game == null)
        {
            // Legacy join-by-code: first joiner. Game stays Waiting until a second player joins.
            var member = new GameMember { UserId = userId, Status = PlayerStatus.Playing };
            var newGame = new Game
            {
                GameCode = gameCode,
                Status = GameStatus.Waiting,
                Format = GameFormat.Solo,
                RoundsPerPlayer = 1,
                GameMember = new List<GameMember> { member }
            };
            await _unitOfWork.Game.Add(newGame);
            await _unitOfWork.Commit();
            return new GameJoinOutcome(IsDrawer: true, Word: null, WordLength: 0, JustCompletedLegacyPairing: false, DrawerWordToPush: null);
        }

        var activeRound = game.Round?.FirstOrDefault(r => r.Status == RoundStatus.Active);
        if (activeRound != null)
        {
            // Pre-matched quick-match game, or a legacy pairing that already completed: just resolve the caller's role.
            var member = game.GameMember!.First(m => m.UserId == userId);
            var isDrawer = member.Id == activeRound.DrawerGameMemberId;
            return new GameJoinOutcome(isDrawer, isDrawer ? activeRound.Word : null, activeRound.Word.Length, JustCompletedLegacyPairing: false, DrawerWordToPush: null);
        }

        // No active round yet: legacy join-by-code flow, waiting for a second player.
        var existingMember = game.GameMember!.Single();
        if (existingMember.UserId == userId)
        {
            // Same player reconnecting before anyone else joined - still waiting.
            return new GameJoinOutcome(IsDrawer: true, Word: null, WordLength: 0, JustCompletedLegacyPairing: false, DrawerWordToPush: null);
        }

        // Second joiner completes pairing now.
        var guesserMember = new GameMember { UserId = userId, Status = PlayerStatus.Playing };
        game.GameMember!.Add(guesserMember);

        var word = await _unitOfWork.GameWord.GetRandomWord(new Random().Next(0, 150));
        game.Round ??= new List<Round>();
        game.Round.Add(new Round
        {
            RoundNumber = 1,
            Word = word!,
            Status = RoundStatus.Active,
            StartedAt = DateTime.UtcNow,
            DrawerGameMember = existingMember,
            ActiveGuesserGameMember = guesserMember
        });
        game.Status = GameStatus.InProgress;

        await _unitOfWork.Commit();
        return new GameJoinOutcome(IsDrawer: false, Word: null, WordLength: word!.Length, JustCompletedLegacyPairing: true, DrawerWordToPush: word);
    }

    public async Task<GuessOutcome> SubmitGuess(string gameCode, Guid userId, string guess)
    {
        var game = await _unitOfWork.Game.GetByGameCode(gameCode)
            ?? throw new InvalidOperationException($"Game {gameCode} not found.");

        var round = game.Round?.SingleOrDefault(r => r.Status == RoundStatus.Active)
            ?? throw new InvalidOperationException($"Game {gameCode} has no active round.");

        var member = game.GameMember?.FirstOrDefault(m => m.UserId == userId)
            ?? throw new InvalidOperationException($"User {userId} is not a member of game {gameCode}.");

        if (member.Id != round.ActiveGuesserGameMemberId)
            throw new InvalidOperationException("Only the active guesser can submit a guess.");

        var isCorrect = string.Equals(guess?.Trim(), round.Word, StringComparison.OrdinalIgnoreCase);

        await _unitOfWork.GameMessage.Add(new GameMessage
        {
            GameId = game.Id,
            UserId = userId,
            RoundId = round.Id,
            Content = guess ?? string.Empty,
            IsCorrectGuess = isCorrect,
            PointsAwarded = isCorrect ? GuesserPoints : 0
        });

        if (!isCorrect)
        {
            await _unitOfWork.Commit();
            return new GuessOutcome(false, round.Word, 0, false);
        }

        round.Status = RoundStatus.Completed;
        round.EndedAt = DateTime.UtcNow;
        game.Status = GameStatus.Completed;
        member.Score += GuesserPoints;

        var drawer = game.GameMember!.FirstOrDefault(m => m.Id == round.DrawerGameMemberId);
        if (drawer != null)
            drawer.Score += DrawerPoints;

        await _unitOfWork.Commit();
        return new GuessOutcome(true, round.Word, GuesserPoints, true);
    }
}
