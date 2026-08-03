using Grpc.Core;
using MagicOnion.Server.Hubs;
using Microsoft.AspNetCore.Authorization;
using Picator.Common.Data.Dtos.Games;
using Picator.Realtime.Common.Services;
using Picator.Service.Contracts.Games;
using System.Security.Claims;

namespace Picator.Realtime.Services;

[Authorize]
public class GameHub : StreamingHubBase<IGameHub, IGameDrawingReceiver>, IGameHub
{
    private readonly IGameCreateService _gameCreateService;
    private readonly ILogger<GameHub> _logger;
    private IGroup<IGameDrawingReceiver>? _group;
    private string? _gameCode;

    public GameHub(IGameCreateService gameCreateService, ILogger<GameHub> logger)
    {
        _gameCreateService = gameCreateService;
        _logger = logger;
    }

    public async ValueTask<(bool IsDrawer, string? Word, int WordLength, int RoundDurationSeconds)> JoinGameAsync(string gameCode)
    {
        var userId = Guid.Parse(Context.CallContext.GetHttpContext()!.User.FindFirstValue(ClaimTypes.Name)!);

        // Join/create the group for this game
        _group = await Group.AddAsync(gameCode);
        _gameCode = gameCode;
        _logger.LogInformation("***User {UserId} joined game {GameCode}***", userId, gameCode);
        _group.Except([ConnectionId]).OnPlayerJoined();

        var outcome = await _gameCreateService.JoinGame(gameCode, userId);
        if (outcome.JustCompletedLegacyPairing)
        {
            // We (the guesser) just completed pairing - push the word to the already-connected drawer,
            // whose own JoinGameAsync call returned long before this round existed.
            _group.Except([ConnectionId]).OnGameWordReceived(outcome.DrawerWordToPush);
            _logger.LogInformation("***Sent game word to drawer in game {GameCode}***", gameCode);
        }

        return (outcome.IsDrawer, outcome.Word, outcome.WordLength, outcome.RoundDurationSeconds);
    }

    public async ValueTask<bool> SubmitGuessAsync(string guess)
    {
        var userId = Guid.Parse(Context.CallContext.GetHttpContext()!.User.FindFirstValue(ClaimTypes.Name)!);
        var outcome = await _gameCreateService.SubmitGuess(_gameCode!, userId, guess);
        BroadcastIfRoundEnded(outcome);
        return outcome.WasCorrect;
    }

    public async ValueTask SubmitTimeoutAsync()
    {
        var userId = Guid.Parse(Context.CallContext.GetHttpContext()!.User.FindFirstValue(ClaimTypes.Name)!);
        var outcome = await _gameCreateService.TimeoutRound(_gameCode!, userId);
        BroadcastIfRoundEnded(outcome);
    }

    private void BroadcastIfRoundEnded(GuessOutcome outcome)
    {
        if (!outcome.RoundJustCompleted)
            return;

        _group!.All.OnRoundEnded(outcome.WasCorrect, outcome.Word, outcome.PointsAwarded, outcome.GameCompleted, outcome.DrawerScore, outcome.GuesserScore);
        _logger.LogInformation("***Round ended in game {GameCode}: correct={WasCorrect}, gameCompleted={GameCompleted}***", _gameCode, outcome.WasCorrect, outcome.GameCompleted);
    }

    public async ValueTask LeaveAsync()
    {
        if (_group == null)
            return;

        _group.All.OnPlayerLeft();
        await _group.RemoveAsync(Context);
    }

    // notifies other player when drawing a line is completed
    public ValueTask SendDrawingCompleted(string roomName)
    {
        _group?.Except([ConnectionId]).OnLineCompleted();
        _logger.LogInformation("***Notified other players in game {GameCode} of line completion***", roomName);
        return ValueTask.CompletedTask;
    }

    // notifies other player when player is drawing a line
    public ValueTask SendDrawingPoint(string roomName, float x, float y)
    {
        _group?.Except([ConnectionId]).OnPointAdded(x, y);
        return ValueTask.CompletedTask;
    }

    // notifies other player when player change color
    public ValueTask SendDrawingColor(string roomName, uint color)
    {
        _group?.Except([ConnectionId]).OnColorChanged(color);
        return ValueTask.CompletedTask;
    }

    // notifies other player when player change line thickness
    public ValueTask SendDrawingThickness(string roomName, float thickness)
    {
        _group?.Except([ConnectionId]).OnThicknessChanged(thickness);
        return ValueTask.CompletedTask;
    }
}