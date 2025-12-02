using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Views;
using Picator.Game.Constants;
using Picator.Game.Hubs;
using Picator.Game.Services.GameWords;
using System.Collections.ObjectModel;

namespace Picator.Game.ViewModels;

public partial class GameViewModel : ViewModelBase, IAsyncDisposable
{
    private List<string>? _options;
    private readonly List<PointF> _points = [];

    private string _gameCode;
    [ObservableProperty]
    private string? _correctWord;

    [ObservableProperty]
    private ObservableCollection<Color> _colorItems;

    [ObservableProperty]
    private ObservableCollection<int> _lineWidths;

    [ObservableProperty]
    private int _selectedLineWidth;

    private bool _hasWon;

    [ObservableProperty]
    private string _currentState = "WaitingFriend";

    [ObservableProperty]
    private char? _selectedWord;
    [ObservableProperty]
    private ObservableCollection<char> _letters = [];
    [ObservableProperty]
    private ObservableCollection<char?> _correctWordLetters = [];
    [ObservableProperty]
    private ObservableCollection<DrawingLine> _lines = [new DrawingLine()];
    [ObservableProperty]
    private double _remainingSeconds;

    [ObservableProperty]
    private bool _isTimerRunning;

    private readonly IGameWordsApiService _gameWordsApiService;
    private CancellationTokenSource? _timerCts;
    private readonly GameHub _hub;

    public bool IsDrawingPlayer { get; set; }

    public GameViewModel(bool isDrawingPlayer, string gameCode)
    {
        IsDrawingPlayer = isDrawingPlayer;
        _gameCode = gameCode;
        _hub = GameHub.Instance;
        _hub.WordReceived += GameWordReceived;
        if (IsDrawingPlayer)
        {
            ColorItems = [Colors.Black, Colors.Red, Colors.Orange, Colors.Yellow, Colors.Green, Colors.Blue, Colors.Purple, Colors.White, Colors.LightGray];
            LineWidths = [5, 10, 15, 20];
            SelectedLineWidth = LineWidths[0];
        }
        _gameWordsApiService = new GameWordsApiService();
    }

    public async Task OnNavigatedTo()
    {
        await ConnectRealtimeServer();
    }

    private async Task ConnectRealtimeServer()
    {
        IsBusy = true;

        var channel = Grpc.Net.Client.GrpcChannel.ForAddress(UrlConstants.GameHubUrl);

        await _hub.ConnectAsync(channel);

        if (IsDrawingPlayer)
        {
            var playerId = Guid.NewGuid().ToString();
            await _hub.JoinGameAsync(_gameCode!, playerId);
        }

        IsBusy = false;
    }

    private void GameWordReceived(object? sender, string? e)
    {
        CorrectWord = e;
        IsBusy = false;

        PrepareLetters();

        StartGameTimer(60);
    }

    private void PrepareLetters()
    {
        if (string.IsNullOrEmpty(CorrectWord))
            return;

        // Start with the letters from the correct word
        Letters = [.. CorrectWord.ToUpper().Where(c => !char.IsWhiteSpace(c))];

        // Add random letters to reach 16 total
        Random random = new Random();
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        while (Letters.Count < 16)
        {
            Letters.Add(alphabet[random.Next(alphabet.Length)]);
        }

        // Shuffle all letters
        for (int i = Letters.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (Letters[i], Letters[j]) = (Letters[j], Letters[i]);
        }

        CorrectWordLetters = [.. MapStringToNullableChars(CorrectWord)];
    }

    private static List<char?> MapStringToNullableChars(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        int length = input.Length;
        var buffer = new char?[length];

        for (int i = 0; i < length; i++)
        {
            buffer[i] = input[i] == ' ' ? null : ' ';
        }

        return [.. buffer];
    }

    private static string MapNullableCharsToString(List<char?> chars)
    {
        ArgumentNullException.ThrowIfNull(chars);
        return string.Concat(chars.Select(c => c ?? ' '));
    }

    private void StartGameTimer(int seconds)
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        _timerCts = new CancellationTokenSource();

        RemainingSeconds = seconds;
        IsTimerRunning = true;
        _hasWon = false;

        var token = _timerCts.Token;
        _ = RunTimerAsync(token);
    }

    private async Task RunTimerAsync(CancellationToken ct)
    {
        try
        {
            if (IsDrawingPlayer)
                CurrentState = "IntroForDrawer";
            else
                CurrentState = "IntroForPlayer";

            await Task.Delay(5000);

            if (IsDrawingPlayer)
                CurrentState = "DrawerGame";
            else
                CurrentState = "PlayerGame";

            while (RemainingSeconds > 0 && !ct.IsCancellationRequested && !_hasWon)
            {
                await Task.Delay(250, ct);
                if (ct.IsCancellationRequested || _hasWon)
                    break;

                RemainingSeconds -= 0.25f;
            }

            if (!_hasWon && !ct.IsCancellationRequested)
            {
                // Time’s up -> lose
                IsTimerRunning = false;
                // TODO: expose Lose state / raise event / navigate to result page
                // e.g. ShowLoseState();
            }
        }
        catch (TaskCanceledException)
        {
            // ignore
        }
        finally
        {
            IsTimerRunning = false;
        }
    }
    
    [RelayCommand]
    private async Task SelectedThicknessChanged(float thickness)
    { 
        await _hub.SendDrawingThickness(thickness);
    }

    [RelayCommand]
    private async Task SelectedColorChanged(Color color)
    {
    }

    private void LineCompleted(object? sender, EventArgs e)
    {
        _points.Clear();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Lines.Add(new DrawingLine());
        });
    }

    private void PointReceived(object? sender, PointF e)
    {
        _points.Add(e);
        if (!Lines.Any())
            return;
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Lines.RemoveAt(Lines.Count - 1);
            Lines.Add(new DrawingLine
            {
                Points = new ObservableCollection<PointF>(_points),
                LineColor = Colors.Black
            });
        });
    }

    [RelayCommand]
    private async Task SelectCustomColor(Color colorItem)
    {
        // Show color picker
        var result = await Application.Current.MainPage.DisplayPromptAsync(
            "Custom Color",
            "Enter hex color (e.g., #FF5733):",
            "OK",
            "Cancel",
            "e.g., #FF5733");

        if (!string.IsNullOrEmpty(result) && result.StartsWith("#"))
        {
            try
            {
                var customColor = Color.FromArgb(result);
                // Optional: Add to colors list
                ColorItems.Insert(ColorItems.Count - 1, customColor);
            }
            catch
            {
                await Application.Current.MainPage.DisplayAlertAsync("Error", "Invalid color format", "OK");
            }
        }
    }

    [RelayCommand]
    private Task SelectedLetterChangedAsync(char letter)
    {
        if (CorrectWordLetters.Contains(' '))
        {
            int index = CorrectWordLetters.IndexOf(' ');
            CorrectWordLetters[index] = letter;
        }
        Letters.Remove(letter);

        if (!CorrectWordLetters.Contains(' '))
        {
            if (MapNullableCharsToString([.. CorrectWordLetters]).Equals(CorrectWord, StringComparison.InvariantCultureIgnoreCase))
            {
                // Player has guessed the word
                Snackbar.Make("Afarin chaghal!!!", duration: TimeSpan.FromSeconds(3)).Show();
                _hasWon = true;
                IsTimerRunning = false;
            }
            else
            {
                Snackbar.Make("Nice Try chaghal!!!", duration: TimeSpan.FromSeconds(3)).Show();
            }
        }
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task SelectedCorrectLetterChangedAsync(char? letter)
    {
        if (letter == null || letter == ' ')
            return Task.CompletedTask;
        Letters.Insert(0, letter.Value);
        var index = CorrectWordLetters.IndexOf(letter);
        CorrectWordLetters[index] = ' ';
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DrawingPointAddedAsync(PointF pt)
    {
        await _hub.SendDrawingPoint(_gameCode, pt.X, pt.Y);
    }

    [RelayCommand]
    private async Task DrawingLineCompletedAsync()
    {
        await _hub.SendDrawingCompleted(_gameCode);
    }

    public async ValueTask DisposeAsync()
    {
        _timerCts?.Cancel();
        _timerCts?.Dispose();
        _timerCts = null;

        _hub.WordReceived -= GameWordReceived;

        if (!IsDrawingPlayer)
        {
            _hub.PointReceived -= PointReceived;
            _hub.LineCompleted -= LineCompleted;
        }

        // End of game session – safe to dispose the hub client
        await _hub.DisposeAsync();
    }
}