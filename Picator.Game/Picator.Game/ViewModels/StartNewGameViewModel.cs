using Picator.Game.Constants;
using Picator.Game.Helpers;
using Picator.Game.Hubs;
using System.IO;
using System.Web;
using Xamarin.Essentials;

namespace Picator.Game.ViewModels;

public partial class StartNewGameViewModel : ViewModelBase
{
    private readonly GameHub _hub;

    [ObservableProperty]
    private string? _gameCode;

    public StartNewGameViewModel()
    {
        _hub = GameHub.Instance;
        _hub.PlayerJoined += OnPlayerJoined;
        _ = ConnectRealtimeServer();
    }

    private async void OnPlayerJoined(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new GamePage(true, GameCode));
        Alert.Show("player joined", MessageType.Info);
        _hub.PlayerJoined -= OnPlayerJoined;
    }

    private async Task ConnectRealtimeServer()
    {
        // var channel = Grpc.Net.Client.GrpcChannel.ForAddress("https://picatorrealtime-app-202305091235.victoriousrock-9f2ad982.centralus.azurecontainerapps.io");
        var channel = new Grpc.Core.Channel(UrlConstants.RealTimeHubUrl, 443, Grpc.Core.ChannelCredentials.SecureSsl);
        await _hub.ConnectAsync(channel);
        GameCode = RandomHelper.CreateRandomText(12);
        await _hub.CreateGameAsync(GameCode);
    }

    [RelayCommand]
    private async Task ShareAsync()
    {
        await Share.RequestAsync(new ShareTextRequest
        {
            Uri = "https://lively-tree-061c28b10.3.azurestaticapps.net?game_code=" + HttpUtility.UrlEncode(GameCode)
        });
    }

    [RelayCommand]
    private async Task CopyAsync()
    {
        await Clipboard.SetTextAsync(GameCode);
        Alert.Show("Game code copied to clipboard", MessageType.Success);
    }

    [RelayCommand]
    private async Task ScanAsync()
    {

    }
}