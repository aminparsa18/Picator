using Grpc.Core;
using MagicOnion.Client;
using Picator.Realtime.Common.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Picator.Game.Services;

public class GameHub : IGameDrawingReceiver
{
    IGameHub client;

    public event EventHandler<Point> PointReceived;
    public event EventHandler LineCompleted;
    public event EventHandler PlayerJoined;
    public event EventHandler<string> WordReceived;

    public async Task<string> ConnectAsync(ChannelBase grpcChannel)
    {
        client = await StreamingHubClient.ConnectAsync<IGameHub, IGameDrawingReceiver>(grpcChannel, this);
        return client.CreateGameAsync();
    }

    public async ValueTask joinJoinGameAsync(string gameCode)
    {
        await client.JoinGameAsync(gameCode);
    }

    public async ValueTask SendDrawingPoint(float x, float y)
    {
        await client.SendDrawingPoint("test", x, y);
    }

    public async ValueTask SendDrawingCompleted()
    {
        await client.SendDrawingCompleted("test");
    }

    // dispose client-connection before channel.ShutDownAsync is important!
    public Task DisposeAsync()
    {
        return client.DisposeAsync();
    }

    // You can watch connection state, use this for retry etc.
    public Task WaitForDisconnect()
    {
        return client.WaitForDisconnect();
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
}