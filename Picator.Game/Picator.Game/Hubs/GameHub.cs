using Grpc.Core;
using MagicOnion.Client;
using Picator.Realtime.Common.Services;

namespace Picator.Game.Hubs;

public class GameHub : IGameDrawingReceiver
{
    private IGameHub? _client;
    private static GameHub? _instance;
    private static readonly object _padlock = new();
    public event EventHandler<Point>? PointReceived;
    public event EventHandler? LineCompleted;
    public event EventHandler? PlayerJoined;
    public event EventHandler<string?>? WordReceived;

    public async Task<IGameHub> ConnectAsync(ChannelBase grpcChannel)
    {
        return await StreamingHubClient.ConnectAsync<IGameHub, IGameDrawingReceiver>(grpcChannel, this);
    }

    public async ValueTask CreateGameAsync(string gameCode)
    {
        await _client.CreateGameAsync(gameCode);
    }

    public async ValueTask JoinGameAsync(string gameCode)
    {
        await _client.JoinGameAsync(gameCode);
    }

    public async ValueTask SendDrawingPoint(string gameCode, float x, float y)
    {
        await _client.SendDrawingPoint(gameCode, x, y);
    }

    public async ValueTask SendDrawingCompleted(string gameCode)
    {
        await _client.SendDrawingCompleted(gameCode);
    }

    // dispose client-connection before channel.ShutDownAsync is important!
    public Task DisposeAsync()
    {
        return _client.DisposeAsync();
    }

    // You can watch connection state, use this for retry etc.
    public Task WaitForDisconnect()
    {
        return _client.WaitForDisconnect();
    }

    public void OnPointAdded(float x, float y)
    {
        PointReceived?.Invoke(this, new Point(x, y));
    }

    public void OnLineCompleted()
    {
        LineCompleted?.Invoke(this, new EventArgs { });
    }

    public void OnPlayerJoined()
    {
        PlayerJoined?.Invoke(this, new EventArgs { });
    }

    public void OnGameWordReceived(string? word)
    {
        WordReceived?.Invoke(this, word);
    }

    public static GameHub Instance
    {
        get
        {
            lock (_padlock)
            {
                return _instance ??= new GameHub();
            }
        }
    }

    public IGameHub Client => _client;
}