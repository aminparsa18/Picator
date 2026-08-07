using Grpc.Net.Client;
using Picator.Common.Data.Enums;
using Picator.Game.Cache;
using Picator.Game.Constants;
using Picator.Game.Helpers;
using Picator.Game.Hubs;
using Picator.GameV2;
using System.Web;

namespace Picator.Game.ViewModels;

public enum HomeOverlayMode
{
    None,
    Searching,
    MatchFound,
    Friends,
    JoinByCode,
    Stats,
    Settings,
    HowToPlay
}

public sealed partial class MainViewModel : ViewModelBase
{
    private CancellationTokenSource? _searchCts;
    private MatchmakingHubClient? _matchmakingHub;
    private GrpcChannel? _matchmakingChannel;

    [ObservableProperty]
    private HomeOverlayMode _overlayMode = HomeOverlayMode.None;

    [ObservableProperty]
    private int _searchSeconds;

    [ObservableProperty]
    private string? _chosenFormat;

    [ObservableProperty]
    private string _codeInput = string.Empty;

    [ObservableProperty]
    private string _howToTab = "modes";

    [ObservableProperty]
    private bool _isMusicOn = true;

    public bool IsOverlayVisible => OverlayMode != HomeOverlayMode.None;
    public bool IsSearching => OverlayMode == HomeOverlayMode.Searching;
    public bool IsMatchFound => OverlayMode == HomeOverlayMode.MatchFound;
    public bool IsFriendsPicker => OverlayMode == HomeOverlayMode.Friends;
    public bool IsJoinByCode => OverlayMode == HomeOverlayMode.JoinByCode;
    public bool IsStats => OverlayMode == HomeOverlayMode.Stats;
    public bool IsSettings => OverlayMode == HomeOverlayMode.Settings;
    public bool IsHowToPlay => OverlayMode == HomeOverlayMode.HowToPlay;

    public bool HowToShowModes => HowToTab == "modes";
    public bool HowToShowRounds => HowToTab == "rounds";
    public bool HowToShowScoring => HowToTab == "scoring";
    public bool IsModesTabActive => HowToTab == "modes";
    public bool IsRoundsTabActive => HowToTab == "rounds";
    public bool IsScoringTabActive => HowToTab == "scoring";

    public string SearchTimeDisplay => TimeSpan.FromSeconds(SearchSeconds).ToString(@"m\:ss");

    public string MusicStatusDisplay => IsMusicOn ? "On" : "Off";

    public string ChosenFormatNote => ChosenFormat switch
    {
        "solo" => "Opening Solo Room — share a code, wait for one friend.",
        "teams" => "Teams Room (2v2 relay guessing) is coming soon.",
        _ => string.Empty
    };

    public MainViewModel()
    {
        if (Barrel.Current.Exists("User"))
        {
            var user = MemoryPack.MemoryPackSerializer.Deserialize<Picator.Common.Data.Dtos.Users.UserDetailsResult>(Barrel.Current.Get<byte[]>("User"));
            Console.WriteLine($"MainViewModel: cached user - Email={user?.Email}, DisplayName={user?.DisplayName}, Avatar={user?.Avatar}, Score={user?.Score}");
        }
        else
        {
            Console.WriteLine("MainViewModel: no cached user found in Barrel.");
        }

        Application.Current.Dispatcher.Dispatch(async () =>
        {
            Uri? appUri = (Application.Current as App)?.AppUri;
            string[]? segments = appUri?.Segments;
            if (segments != null && segments.Length > 1)
            {
                var path = segments[1];
                if (path.StartsWith("join-game"))
                {
                    var queryDictionary = HttpUtility.ParseQueryString(appUri?.Query);
                    string? gameCode = HttpUtility.HtmlDecode(queryDictionary["game_code"]);
                    if (Barrel.Current.Exists("Token"))
                        await Application.Current.MainPage.Navigation.PushAsync(new GamePage(false, gameCode));
                    else
                        await Shell.Current.GoToAsync("//login");
                }
            }
        });
    }

    partial void OnOverlayModeChanged(HomeOverlayMode value)
    {
        OnPropertyChanged(nameof(IsOverlayVisible));
        OnPropertyChanged(nameof(IsSearching));
        OnPropertyChanged(nameof(IsMatchFound));
        OnPropertyChanged(nameof(IsFriendsPicker));
        OnPropertyChanged(nameof(IsJoinByCode));
        OnPropertyChanged(nameof(IsStats));
        OnPropertyChanged(nameof(IsSettings));
        OnPropertyChanged(nameof(IsHowToPlay));
    }

    partial void OnSearchSecondsChanged(int value) => OnPropertyChanged(nameof(SearchTimeDisplay));

    partial void OnChosenFormatChanged(string? value) => OnPropertyChanged(nameof(ChosenFormatNote));

    partial void OnIsMusicOnChanged(bool value) => OnPropertyChanged(nameof(MusicStatusDisplay));

    partial void OnHowToTabChanged(string value)
    {
        OnPropertyChanged(nameof(HowToShowModes));
        OnPropertyChanged(nameof(HowToShowRounds));
        OnPropertyChanged(nameof(HowToShowScoring));
        OnPropertyChanged(nameof(IsModesTabActive));
        OnPropertyChanged(nameof(IsRoundsTabActive));
        OnPropertyChanged(nameof(IsScoringTabActive));
    }

    [RelayCommand]
    private async Task NavigateToLogin()
    {
        if (Barrel.Current.Exists("Token"))
            await Application.Current.MainPage.Navigation.PushAsync(new ProfileSettingsPage());
        else
            await Shell.Current.GoToAsync("//login");
    }

    [RelayCommand]
    private async Task StartQuickMatch()
    {
        if (!Barrel.Current.Exists("Token"))
        {
            await Shell.Current.GoToAsync("//login");
            return;
        }

        _searchCts?.Cancel();
        _searchCts = new CancellationTokenSource();
        SearchSeconds = 0;
        OverlayMode = HomeOverlayMode.Searching;

        _ = TickSearchAsync(_searchCts.Token);
        _ = EnterQuickMatchAsync(_searchCts.Token);
    }

    private async Task TickSearchAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                await Task.Delay(1000, token);
                SearchSeconds++;
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    private async Task EnterQuickMatchAsync(CancellationToken token)
    {
        try
        {
            await DisconnectMatchmakingHubAsync();

            var jwt = Barrel.Current.Get<string>("Token");
            _matchmakingChannel = GrpcChannelFactory.Create(UrlConstants.GameHubUrl);
            _matchmakingHub = new MatchmakingHubClient();
            _matchmakingHub.MatchFound += OnMatchFound;
            await _matchmakingHub.ConnectAsync(_matchmakingChannel, jwt!);

            if (token.IsCancellationRequested)
                return;

            await _matchmakingHub.EnterQueueAsync(GameFormat.Solo);
        }
        catch (Exception ex)
        {
            if (token.IsCancellationRequested)
                return;

            Console.WriteLine($"MainViewModel: quick match failed - {ex}");
            OverlayMode = HomeOverlayMode.None;
            await DisconnectMatchmakingHubAsync();

            var page = Application.Current?.MainPage;
            if (page is not null)
                await page.DisplayAlert("Quick match failed", ex.ToString(), "OK");
        }
    }

    private async void OnMatchFound(object? sender, (string GameCode, bool IsDrawer) e)
    {
        _searchCts?.Cancel();

        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            OverlayMode = HomeOverlayMode.MatchFound;
            await Task.Delay(1200);
            OverlayMode = HomeOverlayMode.None;
            await Application.Current.MainPage.Navigation.PushAsync(new GamePage(e.IsDrawer, e.GameCode));
        });

        await DisconnectMatchmakingHubAsync();
    }

    [RelayCommand]
    private async Task CancelSearch()
    {
        _searchCts?.Cancel();
        OverlayMode = HomeOverlayMode.None;

        if (_matchmakingHub is not null)
            await _matchmakingHub.CancelQueueAsync();

        await DisconnectMatchmakingHubAsync();
    }

    private async Task DisconnectMatchmakingHubAsync()
    {
        if (_matchmakingHub is not null)
        {
            _matchmakingHub.MatchFound -= OnMatchFound;
            await _matchmakingHub.DisposeAsync();
            _matchmakingHub = null;
        }

        if (_matchmakingChannel is not null)
        {
            await _matchmakingChannel.ShutdownAsync();
            _matchmakingChannel = null;
        }
    }

    [RelayCommand]
    private void CloseOverlay()
    {
        _searchCts?.Cancel();
        OverlayMode = HomeOverlayMode.None;
        ChosenFormat = null;
        CodeInput = string.Empty;
    }

    [RelayCommand]
    private void OpenFriendsPicker() => OverlayMode = HomeOverlayMode.Friends;

    [RelayCommand]
    private async Task ChooseSolo()
    {
        if (!Barrel.Current.Exists("Token"))
        {
            OverlayMode = HomeOverlayMode.None;
            await Shell.Current.GoToAsync("//login");
            return;
        }

        ChosenFormat = "solo";
        await Task.Delay(700);
        OverlayMode = HomeOverlayMode.None;
        await Application.Current.MainPage.Navigation.PushAsync(new SoloRoomPage());
        ChosenFormat = null;
    }

    [RelayCommand]
    private void ChooseTeams() => ChosenFormat = "teams";

    [RelayCommand]
    private void OpenJoinByCode() => OverlayMode = HomeOverlayMode.JoinByCode;

    [RelayCommand]
    private async Task SubmitCode()
    {
        var code = CodeInput?.Trim();
        if (string.IsNullOrWhiteSpace(code))
            return;

        OverlayMode = HomeOverlayMode.None;
        if (Barrel.Current.Exists("Token"))
            await Application.Current.MainPage.Navigation.PushAsync(new GamePage(false, code));
        else
            await Shell.Current.GoToAsync("//login");
        CodeInput = string.Empty;
    }

    [RelayCommand]
    private void OpenStats() => OverlayMode = HomeOverlayMode.Stats;

    [RelayCommand]
    private void OpenSettings() => OverlayMode = HomeOverlayMode.Settings;

    [RelayCommand]
    private void ToggleMusic()
    {
        IsMusicOn = !IsMusicOn;
        if (IsMusicOn)
            (Application.Current as App)?.PlayMusic();
        else
            (Application.Current as App)?.StopMusic();
    }

    [RelayCommand]
    private void HowToPlay()
    {
        HowToTab = "modes";
        OverlayMode = HomeOverlayMode.HowToPlay;
    }

    [RelayCommand]
    private void HowToTabModes() => HowToTab = "modes";

    [RelayCommand]
    private void HowToTabRounds() => HowToTab = "rounds";

    [RelayCommand]
    private void HowToTabScoring() => HowToTab = "scoring";
}
