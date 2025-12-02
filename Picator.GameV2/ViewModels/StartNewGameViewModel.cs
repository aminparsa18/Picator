using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Converters;
using Picator.Game.Constants;
using Picator.Game.Helpers;
using Picator.Game.Hubs;

namespace Picator.Game.ViewModels;

public partial class StartNewGameViewModel : ViewModelBase, IDisposable
{
    private readonly GameHub _hub;
    private bool _initialized;
    private bool _gameStarted;

    [ObservableProperty]
    private LayoutState _currentState = LayoutState.Loading;

    [ObservableProperty]
    private string? _gameCode;

    public StartNewGameViewModel()
    {
        _hub = GameHub.Instance;
        _hub.PlayerJoined += OnPlayerJoined;
    }

    public async Task OnNavigatedToAsync()
    {
        if (_initialized)
            return;

        _initialized = true;
        await ConnectRealtimeServer();
    }

    private async Task ConnectRealtimeServer()
    {
        IsBusy = true;

        GameCode = RandomHelper.CreateRandomText(12);

        // Create a fresh channel *per page* and pass it to the hub
        var channel = Grpc.Net.Client.GrpcChannel.ForAddress(UrlConstants.GameHubUrl);

        await _hub.ConnectAsync(channel);

        CurrentState = LayoutState.Success;

        var playerId = Guid.NewGuid().ToString();
        await _hub.JoinGameAsync(GameCode!, playerId);

        IsBusy = false;
    }

    private async void OnPlayerJoined(object? sender, EventArgs e)
    {
        // Avoid double firing
        _hub.PlayerJoined -= OnPlayerJoined;
        _gameStarted = true;
        try
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Application.Current.MainPage.Navigation
                    .PushAsync(new GamePage(false, GameCode));

                await Snackbar.Make("player joined").Show();
            });
        }
        catch
        {
            // log if you want
        }
    }

    [RelayCommand]
    private async Task ShareAsync()
    {
        if (string.IsNullOrWhiteSpace(GameCode))
            return;

        await Share.RequestAsync(new ShareTextRequest
        {
            Uri = $"{UrlConstants.InvitementUrl}/{Uri.EscapeDataString(GameCode)}?player={Guid.NewGuid()}"
        });
    }

    [RelayCommand]
    private async Task CopyAsync()
    {
        if (string.IsNullOrWhiteSpace(GameCode))
            return;

        await Clipboard.SetTextAsync(GameCode);
        await Snackbar
            .Make("Game code copied to clipboard")
            .Show();
    }

    [RelayCommand]
    private Task ScanAsync()
    {
        // TODO: implement QR scanning
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task EnterGame()
    {
        _gameStarted = true;
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Application.Current.MainPage.Navigation.PushAsync(new GamePage(false, GameCode));
        });
    }

    public void Dispose()
    {
        _hub.PlayerJoined -= OnPlayerJoined;
    }
}