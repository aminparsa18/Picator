using CommunityToolkit.Maui.Alerts;
using Grpc.Core;
using MagicOnion.Client;
using Picator.Realtime.Common.Services;

namespace Picator.Game.Hubs;

public class GameHub : IGameDrawingReceiver, IAsyncDisposable
{
    private IGameHub? _client;
    private static GameHub? _instance;
    private static readonly object _padlock = new();
    private readonly SemaphoreSlim _gate = new(1, 1);

    public event EventHandler<PointF>? PointReceived;
    public event EventHandler? LineCompleted;
    public event EventHandler? PlayerJoined;
    public event EventHandler<string?>? WordReceived;
    public event EventHandler<uint>? ColorReceived;
    public event EventHandler<float>? ThicknessReceived;
    public event EventHandler<(bool WasCorrect, string Word, int Points)>? RoundEnded;

    private GameHub()
    {
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

    public bool IsConnected => _client is not null;

    public async Task<IGameHub> ConnectAsync(ChannelBase grpcChannel, string jwt)
    {
        await _gate.WaitAsync();
        try
        {
            if (_client is not null)
                return _client;

            var callOptions = new CallOptions(headers: new Metadata { { "Authorization", $"Bearer {jwt}" } });

            _client = await StreamingHubClient.ConnectAsync<IGameHub, IGameDrawingReceiver>(
                grpcChannel, this, option: callOptions);

            return _client;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async ValueTask<(bool IsDrawer, string? Word, int WordLength)> JoinGameAsync(string gameCode)
    {
        if (_client is null)
            throw new InvalidOperationException("GameHub is not connected. Call ConnectAsync first.");

        return await _client.JoinGameAsync(gameCode);
    }

    public async ValueTask<bool> SubmitGuessAsync(string guess)
    {
        if (_client is null)
            throw new InvalidOperationException("GameHub is not connected.");

        return await _client.SubmitGuessAsync(guess);
    }

    public async ValueTask SendDrawingPoint(string gameCode, float x, float y)
    {
        if (_client is null)
            throw new InvalidOperationException("GameHub is not connected.");

        await _client.SendDrawingPoint(gameCode, x, y);
    }

    public async ValueTask SendDrawingCompleted(string gameCode)
    {
        if (_client is null)
            throw new InvalidOperationException("GameHub is not connected.");

        await _client.SendDrawingCompleted(gameCode);
    }

    public async ValueTask SendDrawingThickness(float thickness)
    {
        if (_client is null)
            throw new InvalidOperationException("GameHub is not connected.");

        await _client.SendDrawingThickness("", thickness);
    }

    public async ValueTask SendDrawingColor(uint color)
    {
        if (_client is null)
            throw new InvalidOperationException("GameHub is not connected.");

        await _client.SendDrawingColor("", color);
    }

    // dispose client-connection before channel.ShutDownAsync is important!
    public async ValueTask DisposeAsync()
    {
        await _gate.WaitAsync();
        try
        {
            if (_client is null)
                return;

            await _client.DisposeAsync();
            _client = null;
        }
        finally
        {
            _gate.Release();
        }
    }

    // You can watch connection state, use this for retry etc.
    public Task WaitForDisconnect()
    {
        if (_client is null)
            return Task.CompletedTask;

        return _client.WaitForDisconnect();
    }

    public void OnPointAdded(float x, float y) =>
        PointReceived?.Invoke(this, new PointF(x, y));

    public void OnLineCompleted() =>
        LineCompleted?.Invoke(this, EventArgs.Empty);

    public void OnPlayerJoined() =>
        PlayerJoined?.Invoke(this, EventArgs.Empty);

    public void OnGameWordReceived(string? word) =>
        WordReceived?.Invoke(this, word);

    public async void OnPlayerLeft()
    {
        await Snackbar.Make("Madar ghabe gorikht").Show();
    }

    public void OnColorChanged(uint color) =>
        ColorReceived?.Invoke(this, color);

    public void OnThicknessChanged(float thickness) =>
        ThicknessReceived?.Invoke(this, thickness);

    public void OnRoundEnded(bool wasCorrect, string word, int pointsAwarded) =>
        RoundEnded?.Invoke(this, (wasCorrect, word, pointsAwarded));
}