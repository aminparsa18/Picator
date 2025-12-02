using MagicOnion.Server.Hubs;
using Picator.Realtime.Common.Services;
using Picator.Service.Contracts.Games;

namespace Picator.Realtime.Services;

public class GameHub : StreamingHubBase<IGameHub, IGameDrawingReceiver>, IGameHub
{
    private readonly IGameCreateService _gameCreateService;
    private readonly ILogger<GameHub> _logger;
    private IGroup<IGameDrawingReceiver>? _group;

    public GameHub(IGameCreateService gameCreateService, ILogger<GameHub> logger)
    {
        _gameCreateService = gameCreateService; 
        _logger = logger;
    }

    // Seconf player creates a new game code and Join as player
    public async ValueTask JoinGameAsync(string gameCode, string playerId)
    {
        // Join/create the group for this game
        _group = await Group.AddAsync(gameCode);
        _logger.LogInformation("***Player {PlayerId} joined game {GameCode}***", playerId, gameCode);
        // notify others that game is created.
        _group.Except([ConnectionId]).OnPlayerJoined();
        _logger.LogInformation("***Notified other players in game {GameCode} of new player {PlayerId}***", gameCode, playerId);
        var count = await _group.CountAsync();
        if (count == 2)
        {
            var gameword = await _gameCreateService.CreateTimeGame(gameCode);
            _group.All.OnGameWordReceived(gameword);
            _logger.LogInformation("***Sent game word to players in game {GameCode}***", gameCode);
            _logger.LogInformation("***Number of people in the game: {Count}***", count);
        }
    }

    public async ValueTask LeaveAsync()
    {
        _group.All.OnPlayerLeft();
        await _group.RemoveAsync(Context);
    }

    // notifies other player when drawing a line is completed
    public ValueTask SendDrawingCompleted(string roomName)
    {
        _group.Except([ConnectionId]).OnLineCompleted();
        _logger.LogInformation("***Notified other players in game {GameCode} of line completion***", roomName);
        return ValueTask.CompletedTask;
    }

    // notifies other player when player is drawing a line
    public ValueTask SendDrawingPoint(string roomName, float x, float y)
    {
        _group.Except([ConnectionId]).OnPointAdded(x, y);
        return ValueTask.CompletedTask;
    }

    // notifies other player when player change color
    public ValueTask SendDrawingColor(string roomName, uint color)
    {
        _group.Except([ConnectionId]).OnColorChanged(color);
        return ValueTask.CompletedTask;
    }

    // notifies other player when player change line thickness
    public ValueTask SendDrawingThickness(string roomName, float thickness)
    {
        _group.Except([ConnectionId]).OnThicknessChanged(thickness);
        return ValueTask.CompletedTask;
    }
}