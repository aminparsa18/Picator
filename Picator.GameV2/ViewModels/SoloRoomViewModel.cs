using Picator.Game.Cache;
using Picator.Game.Constants;
using Picator.Game.Helpers;
using Picator.Game.Hubs;

namespace Picator.Game.ViewModels;

/// <summary>
/// Solo Room lobby: generates a shareable code/QR for a "Play with Friends" (1v1) room, joins
/// the game hub under that code as the first player, and waits for a friend to join. Reuses the
/// same GameHub.JoinGameAsync mechanic as the existing "Join by Code" flow (see
/// MainViewModel.SubmitCode / GameViewModel.ConnectRealtimeServer) - the host just gets there first.
/// </summary>
public sealed partial class SoloRoomViewModel : ViewModelBase, IAsyncDisposable
{
    private static readonly string[] CodeWords =
    [
        "FOX", "OWL", "ELK", "RAY", "JAY", "KOI", "CUB", "WREN", "LYNX", "IBIS", "PUMA", "SEAL"
    ];

    private readonly GameHub _hub = GameHub.Instance;
    private bool _handedOffToGame;
    private CancellationTokenSource? _copyResetCts;

    [ObservableProperty]
    private string _roomCode = string.Empty;

    [ObservableProperty]
    private bool _isCopied;

    [ObservableProperty]
    private bool _friendJoined;

    public string CopiedLabel => IsCopied ? "Copied!" : FriendJoined ? "Friend joined!" : string.Empty;

    public SoloRoomViewModel()
    {
        RoomCode = GenerateRoomCode();
        _hub.WordReceived += OnWordReceived;
    }

    partial void OnIsCopiedChanged(bool value) => OnPropertyChanged(nameof(CopiedLabel));

    partial void OnFriendJoinedChanged(bool value) => OnPropertyChanged(nameof(CopiedLabel));

    private static string GenerateRoomCode()
    {
        var word = CodeWords[Random.Shared.Next(CodeWords.Length)];
        var number = Random.Shared.Next(100, 1000);
        return $"{word}-{number}";
    }

    // Called from SoloRoomPage.OnNavigatedTo - opens the room by joining the game hub under our
    // freshly generated code. Mirrors GameViewModel.ConnectRealtimeServer's connect+join, but
    // stays in the "waiting" state here since BeginRound only happens once a friend joins.
    public async Task OpenRoomAsync()
    {
        try
        {
            var jwt = Barrel.Current.Get<string>("Token");
            if (string.IsNullOrEmpty(jwt))
                return;

            var channel = GrpcChannelFactory.Create(UrlConstants.GameHubUrl);
            await _hub.ConnectAsync(channel, jwt);
            await _hub.JoinGameAsync(RoomCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SoloRoomViewModel: failed to open room - {ex}");
            var page = Application.Current?.MainPage;
            if (page is not null)
                await page.DisplayAlert("Couldn't open room", "Something went wrong setting up the room. Please try again.", "OK");
        }
    }

    // The round only begins once a second player joins under our code, at which point the
    // server sends the word and we become the drawer - see GameViewModel.GameWordReceived for
    // the same handoff on the "Join by Code" path.
    private async void OnWordReceived(object? sender, string? word)
    {
        if (_handedOffToGame)
            return;
        _handedOffToGame = true;

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            FriendJoined = true;
            await Task.Delay(500);

            _hub.WordReceived -= OnWordReceived;

            var nav = Application.Current?.MainPage?.Navigation;
            if (nav is not null)
                await nav.PushAsync(new GamePage(true, RoomCode));
        });
    }

    [RelayCommand]
    private async Task CopyCode()
    {
        await Clipboard.Default.SetTextAsync(RoomCode);

        _copyResetCts?.Cancel();
        _copyResetCts = new CancellationTokenSource();
        IsCopied = true;

        try
        {
            await Task.Delay(1500, _copyResetCts.Token);
            IsCopied = false;
        }
        catch (TaskCanceledException)
        {
        }
    }

    [RelayCommand]
    private async Task ShareQr()
    {
        var joinUrl = $"{UrlConstants.InvitementUrl}/{Uri.EscapeDataString(RoomCode)}?player={Guid.NewGuid()}";

        try
        {
            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Title = "Join my Picator room",
                Text = $"Join my Picator room! Code: {RoomCode}",
                Uri = joinUrl,
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"SoloRoomViewModel: share failed - {ex}");
        }
    }

    [RelayCommand]
    private async Task LeaveRoom()
    {
        if (_handedOffToGame)
            return;

        await DisposeAsync();

        var nav = Application.Current?.MainPage?.Navigation;
        if (nav is not null)
            await nav.PopAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_handedOffToGame)
            return;

        _copyResetCts?.Cancel();
        _hub.WordReceived -= OnWordReceived;

        if (_hub.IsConnected)
        {
            try { await _hub.LeaveAsync(); }
            catch (Exception ex) { Console.WriteLine($"SoloRoomViewModel: leave failed - {ex}"); }

            await _hub.DisposeAsync();
        }
    }
}
