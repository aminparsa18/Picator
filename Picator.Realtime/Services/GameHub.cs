using MagicOnion.Server.Hubs;
using Picator.Common.Helpers;
using Picator.Realtime.Common.Services;

namespace Picator.Realtime.Services;

public class GameHub : StreamingHubBase<IGameHub, IGameDrawingReceiver>, IGameHub
{
    private IGroup? _room;
    
    //  private IGameCreateService _gameCreateService;

    //public GameHub(IGameCreateService gameCreateService)
    //{
    //    _gameCreateService = gameCreateService;
    //}

    // Create new game code and Join as player
    public async ValueTask CreateGameAsync(string gameCode)
    {
        _room = await Group.AddAsync(gameCode);
    }

    // When a player joins game it notifies other player(s)
    // Then creates a game and notifies other player for game word
    public async ValueTask JoinGameAsync(string gameCode)
    {
        _room = await Group.AddAsync(gameCode);
        BroadcastExceptSelf(_room).OnPlayerJoined();
       // var word = await _gameCreateService.CreateTimeGame(gameCode);
       // BroadcastExceptSelf(_room).OnGameWordReceived(word);
    }

    // notifies other player when drawing a line is completed
    public ValueTask SendDrawingCompleted(string roomName)
    {
        BroadcastExceptSelf(_room).OnLineCompleted();
        return ValueTask.CompletedTask;
    }

    // notifies other player when player is drawing a line
    public ValueTask SendDrawingPoint(string roomName, float x, float y)
    {
        BroadcastExceptSelf(_room).OnPointAdded(x, y);
        return ValueTask.CompletedTask;
    }
}