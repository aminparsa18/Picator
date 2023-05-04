using CommunityToolkit.Mvvm.Input;
using Grpc.Net.Client;
using Picator.Game.Services;
using Picator.Game.Views;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Picator.Game.ViewModels;

public partial class StartNewGameViewModel : ViewModelBase
{
    private readonly GameHub _hub;
    private string _gameCode;

    public StartNewGameViewModel()
    {
        _hub = new GameHub();
        _hub.PlayerJoined += OnPlayerJoined;
        _ = ConnectRealtimeServer();
    }

    private async void OnPlayerJoined(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushAsync(new TimerGameView());
        Alert.Show("player joined", MessageType.Info);
        _hub.PlayerJoined -= OnPlayerJoined;
    }

    private async Task ConnectRealtimeServer()
    {
        _gameCode = await _hub.ConnectAsync(GrpcChannel.ForAddress("http://localhost:5000"));
    }

    [RelayCommand]
    private async Task ShareAsync()
    {
        var readStatus = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
        var writeStatus = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
        if (readStatus != PermissionStatus.Granted)
            readStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
        if (writeStatus != PermissionStatus.Granted)
            writeStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();
        if (readStatus != PermissionStatus.Granted || writeStatus != PermissionStatus.Granted)
        {
            Alert.Show("You didnt give us permission to access QR image", MessageType.Info);
            await Application.Current.MainPage.Navigation.PopAsync();
        }
        await Share.RequestAsync(new ShareFileRequest
        {
            File = new ShareFile(Path.Combine(FileSystem.CacheDirectory, "qr.png"))
        });
    }

    [RelayCommand]
    private async Task CopyAsync()
    {
        await Clipboard.SetTextAsync(_gameCode);
        Alert.Show("Game code copied to clipboard", MessageType.Success);
    }
}