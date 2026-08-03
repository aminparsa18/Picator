using Grpc.Core;
using MagicOnion.Client;
using Picator.Common.Data.Enums;
using Picator.Realtime.Common.Services;

namespace Picator.Game.Hubs;

public sealed class MatchmakingHubClient : IMatchFoundReceiver, IAsyncDisposable
{
    private IMatchmakingHub? _client;

    public event EventHandler<string>? MatchFound;
    public event EventHandler? QueueExpired;

    public bool IsConnected => _client is not null;

    public async Task ConnectAsync(ChannelBase grpcChannel, string jwt)
    {
        var callOptions = new CallOptions(headers: new Metadata { { "Authorization", $"Bearer {jwt}" } });

        _client = await StreamingHubClient.ConnectAsync<IMatchmakingHub, IMatchFoundReceiver>(
            grpcChannel, this, option: callOptions);
    }

    public async ValueTask<Guid> EnterQueueAsync(GameFormat format)
    {
        if (_client is null)
            throw new InvalidOperationException("MatchmakingHubClient is not connected. Call ConnectAsync first.");

        return await _client.EnterQueueAsync(format);
    }

    public async ValueTask CancelQueueAsync()
    {
        if (_client is null)
            return;

        await _client.CancelQueueAsync();
    }

    public void OnMatchFound(string gameCode) =>
        MatchFound?.Invoke(this, gameCode);

    public void OnQueueExpired() =>
        QueueExpired?.Invoke(this, EventArgs.Empty);

    public async ValueTask DisposeAsync()
    {
        if (_client is null)
            return;

        await _client.DisposeAsync();
        _client = null;
    }
}
