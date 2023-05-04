using MagicOnion.Server.Hubs;
using Picator.Common.Helpers;
using Picator.Realtime.Common.Services;
using Picator.Service.Contracts.Games;

namespace Picator.Realtime.Services;

public class GameHub : StreamingHubBase<IGameHub, IGameDrawingReceiver>, IGameHub
{
    private IGroup? _room;
    private IGameCreateService _gameCreateService;

    public GameHub(IGameCreateService gameCreateService)
    {
        _gameCreateService = gameCreateService;
    }

    public string CreateGameAsync()
    {
        return RandomHelper.CreateRandomText(16);
    }

    public async ValueTask JoinGameAsync(string gameCode)
    {
        _room = await Group.AddAsync(gameCode);
        Broadcast(_room).OnPlayerJoined();
        var word = await _gameCreateService.CreateTimeGame(gameCode);
        Broadcast(_room).OnGameWordReceived(word);
    }

    public ValueTask SendDrawingCompleted(string roomName)
    {
        Broadcast(_room).OnLineCompleted();
        return ValueTask.CompletedTask;
    }

    public ValueTask SendDrawingPoint(string roomName, float x, float y)
    {
        Broadcast(_room).OnPointAdded(x, y);
        return ValueTask.CompletedTask;
    }
}