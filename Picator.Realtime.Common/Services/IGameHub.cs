using MagicOnion;

namespace Picator.Realtime.Common.Services;

public interface IGameHub : IStreamingHub<IGameHub, IGameDrawingReceiver>
{
    string CreateGameAsync();
    ValueTask JoinGameAsync(string gameCode);
    ValueTask SendDrawingPoint(string roomName, float x, float y);
    ValueTask SendDrawingCompleted(string roomName);
}