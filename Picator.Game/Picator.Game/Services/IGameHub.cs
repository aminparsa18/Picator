using MagicOnion;
using Picator.Game.Services;
using System.Threading.Tasks;

namespace Picator.Realtime.Common.Services;

public interface IGameHub : IStreamingHub<IGameHub, IGameDrawingReceiver>
{
    string CreateGameAsync();
    ValueTask JoinGameAsync(string gameCode);
    ValueTask SendDrawingPoint(string gameCode, float x, float y);
    ValueTask SendDrawingCompleted(string gameCode);
}