using MagicOnion;

namespace Picator.Realtime.Common.Services;

public interface IGameHub : IStreamingHub<IGameHub, IGameDrawingReceiver>
{
    ValueTask JoinGameAsync(string gameCode, string playerId);
    ValueTask SendDrawingPoint(string roomName, float x, float y);
    ValueTask SendDrawingCompleted(string roomName);
    ValueTask SendDrawingColor(string roomName, uint color);
    ValueTask SendDrawingThickness(string roomName, float thickness);
    ValueTask LeaveAsync();
}