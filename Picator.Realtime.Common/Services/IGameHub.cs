using MagicOnion;

namespace Picator.Realtime.Common.Services;

public interface IGameHub : IStreamingHub<IGameHub, IGameDrawingReceiver>
{
    ValueTask<(bool IsDrawer, string? Word, int WordLength, int RoundDurationSeconds)> JoinGameAsync(string gameCode);
    ValueTask<bool> SubmitGuessAsync(string guess);
    ValueTask SubmitTimeoutAsync();
    ValueTask SendDrawingPoint(string roomName, float x, float y);
    ValueTask SendDrawingCompleted(string roomName);
    ValueTask SendDrawingColor(string roomName, uint color);
    ValueTask SendDrawingThickness(string roomName, float thickness);
    ValueTask SendDrawingUndo(string roomName);
    ValueTask SendDrawingClear(string roomName);
    ValueTask LeaveAsync();
}