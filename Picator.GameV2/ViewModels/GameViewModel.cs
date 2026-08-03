using CommunityToolkit.Maui.Core.Views;
using Picator.Game.Cache;
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

    public string WordLengthHint => $"{CorrectWord?.Count(c => !char.IsWhiteSpace(c)) ?? CorrectWordLetters?.Count ?? 0} letters";

    partial void OnCorrectWordChanged(string? value) => OnPropertyChanged(nameof(WordLengthHint));

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

    public bool IsSubmitEnabled => CorrectWordLetters is not null && !CorrectWordLetters.Contains(' ');

    [ObservableProperty]
    private bool _showWrongGuess;

    [ObservableProperty]
    private bool _showCorrectGuess;

    [ObservableProperty]
    private bool _showRoundResult;

    [ObservableProperty]
    private string? _roundResultMessage;

    [ObservableProperty]
    private ObservableCollection<DrawingLine> _lines = [new DrawingLine()];
    [ObservableProperty]
    private double _remainingSeconds;

    public string RemainingTimeDisplay => TimeSpan.FromSeconds(Math.Max(0, RemainingSeconds)).ToString(@"m\:ss");

    public bool IsTimeCritical => RemainingSeconds <= 10;

    partial void OnRemainingSecondsChanged(double value)
    {
        OnPropertyChanged(nameof(RemainingTimeDisplay));
        OnPropertyChanged(nameof(IsTimeCritical));
    }

    [ObservableProperty]
    private bool _isTimerRunning;

    [ObservableProperty]
    private bool _isIntroTimerRunning;

    private const int IntroSeconds = 10;

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
        _hub.RoundEnded += RoundEndedReceived;
        if (IsDrawingPlayer)
        {
            ColorItems =
            [
                Color.FromArgb("#1A1A1A"), Color.FromArgb("#C7431F"), Color.FromArgb("#E8532E"), Color.FromArgb("#2F9E44"),
                Color.FromArgb("#1D5FAD"), Color.FromArgb("#F2B705"), Color.FromArgb("#8E44AD"), Color.FromArgb("#FDFCF8"),
            ];
            LineWidths = [3, 7, 13];
            SelectedLineWidth = LineWidths[1];
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

        var channel = Picator.Game.Helpers.GrpcChannelFactory.Create(UrlConstants.GameHubUrl);
        var jwt = Barrel.Current.Get<string>("Token");

        await _hub.ConnectAsync(channel, jwt!);

        var (isDrawer, word, wordLength) = await _hub.JoinGameAsync(_gameCode!);
        IsDrawingPlayer = isDrawer;

        if (isDrawer && ColorItems is null)
        {
            ColorItems =
            [
                Color.FromArgb("#1A1A1A"), Color.FromArgb("#C7431F"), Color.FromArgb("#E8532E"), Color.FromArgb("#2F9E44"),
                Color.FromArgb("#1D5FAD"), Color.FromArgb("#F2B705"), Color.FromArgb("#8E44AD"), Color.FromArgb("#FDFCF8"),
            ];
            LineWidths = [3, 7, 13];
            SelectedLineWidth = LineWidths[1];
        }

        if (word != null)
        {
            // Drawer, round already active (pre-matched quick-match, or reconnect to an already-paired legacy game).
            GameWordReceived(this, word);
        }
        else if (wordLength > 0)
        {
            // Guesser, round already active: only the word length is known - the real word stays server-side.
            PrepareLettersFromLength(wordLength);
            StartGameTimer(60);
        }
        // else: legacy join-by-code first joiner, still WaitingFriend - the real word arrives later via OnGameWordReceived.

        IsBusy = false;
    }

    private void GameWordReceived(object? sender, string? e)
    {
        CorrectWord = e;
        IsBusy = false;

        PrepareLetters();

        StartGameTimer(60);
    }

    private void RoundEndedReceived(object? sender, (bool WasCorrect, string Word, int Points) e)
    {
        _hasWon = e.WasCorrect;
        IsTimerRunning = false;
        ShowRoundResult = true;
        RoundResultMessage = e.WasCorrect
            ? $"Guessed! The word was \"{e.Word}\" (+{e.Points} pts)"
            : $"Round over. The word was \"{e.Word}\"";
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

    private void PrepareLettersFromLength(int length)
    {
        // Guesser never receives the real word (server-authoritative guessing) - build a plain random
        // tile pool and word-length blanks; the guess itself is validated server-side in SubmitGuess().
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var random = new Random();

        ObservableCollection<char> pool = [];
        while (pool.Count < Math.Max(16, length))
        {
            pool.Add(alphabet[random.Next(alphabet.Length)]);
        }
        Letters = pool;

        CorrectWordLetters = [.. Enumerable.Repeat((char?)' ', length)];
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

            IsIntroTimerRunning = true;
            await Task.Delay(IntroSeconds * 1000, ct);
            IsIntroTimerRunning = false;

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
            IsIntroTimerRunning = false;
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
        ShowWrongGuess = false;
        OnPropertyChanged(nameof(IsSubmitEnabled));
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
        OnPropertyChanged(nameof(IsSubmitEnabled));
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task ClearGuess()
    {
        for (int i = CorrectWordLetters.Count - 1; i >= 0; i--)
        {
            var filled = CorrectWordLetters[i];
            if (filled != null && filled != ' ')
            {
                Letters.Insert(0, filled.Value);
                CorrectWordLetters[i] = ' ';
            }
        }
        ShowWrongGuess = false;
        OnPropertyChanged(nameof(IsSubmitEnabled));
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SubmitGuess()
    {
        if (!IsSubmitEnabled)
            return;

        var guess = MapNullableCharsToString([.. CorrectWordLetters]);
        var wasCorrect = await _hub.SubmitGuessAsync(guess);

        if (wasCorrect)
        {
            _hasWon = true;
            IsTimerRunning = false;
            ShowCorrectGuess = true;
        }
        else
        {
            ShowWrongGuess = true;
            await Task.Delay(1200);
            await ClearGuess();
        }
    }

    [RelayCommand]
    private void Undo()
    {
        // Local-only: removes the drawer's own last completed stroke. Not synced to the guesser —
        // GameHub has no undo/clear message today, only point/line/color/thickness broadcasts.
        if (Lines.Count > 1)
            Lines.RemoveAt(Lines.Count - 2);
    }

    [RelayCommand]
    private void ClearCanvas()
    {
        // Local-only, same caveat as Undo.
        Lines = [new DrawingLine()];
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
        _hub.RoundEnded -= RoundEndedReceived;

        if (!IsDrawingPlayer)
        {
            _hub.PointReceived -= PointReceived;
            _hub.LineCompleted -= LineCompleted;
        }

        // End of game session – safe to dispose the hub client
        await _hub.DisposeAsync();
    }
}