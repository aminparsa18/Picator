using Picator.Game.Constants;
using Picator.Game.Extensions;
using Picator.Game.Hubs;
using Picator.Game.Services.GameWords;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.CommunityToolkit.ObjectModel;
using XamDrawingView.Views.DrawingView;

namespace Picator.Game.ViewModels;

public partial class GameViewModel : ViewModelBase
{
    private List<string> _options;
    private readonly List<Point> _points = new();
    private bool _isDrawingPlayer;
    private string _gameCode;
    private string _correctWord;
    private int _selectedCount;

    [ObservableProperty]
    private string _selectedWord;
    [ObservableProperty]
    private ObservableRangeCollection<string> _words = new();
    [ObservableProperty]
    private ObservableCollection<Line> _lines;

    private readonly IGameWordsApiService _gameWordsApiService;
    private readonly GameHub _hub;

    public GameViewModel(bool isDrawingPlayer, string gameCode)
    {
        _isDrawingPlayer = isDrawingPlayer;
        _gameCode = gameCode;
        _words = new ObservableRangeCollection<string>();
        _lines = new ObservableCollection<Line>();
        _gameWordsApiService = new GameWordsApiService();
        _hub = GameHub.Instance;
        _hub.WordReceived += GameWordReceived;
        if (!_isDrawingPlayer)
        {
            _ = ConnectRealtimeServer();
            _hub.PointReceived += PointReceived;
            _hub.LineCompleted += LineCompleted;
            _ = GetRandomWordsCommand.ExecuteAsync(null);
        }
    }

    private async Task ConnectRealtimeServer()
    {
        var channel = new Grpc.Core.Channel(UrlConstants.RealTimeHubUrl, 80, Grpc.Core.ChannelCredentials.Insecure);
        await _hub.ConnectAsync(channel);
        await _hub.JoinGameAsync(_gameCode);
    }

    private void GameWordReceived(object sender, string? e)
    {
        _correctWord = e;
        if (!string.IsNullOrEmpty(e))
        {
            Words.Add(e);
            Words.Shuffle();
        }
    }

    private void LineCompleted(object sender, EventArgs e)
    {
        _points.Clear();
        Lines.Add(new Line { });
    }

    private void PointReceived(object sender, Point e)
    {
        _points.Add(e);
        if (!Lines.Any())
            return;
        Lines.RemoveAt(Lines.Count - 1);
        Lines.Add(new Line() { Points = new ObservableCollection<Point>(_points), LineColor = Color.Blue });
    }

    [RelayCommand]
    private async Task GetRandomWordsAsync()
    {
        var response = await _gameWordsApiService.GetRandomWords();
        if (response.IsSuccess)
        {
            _options = response.Data;
            for (int i = 0; i < 11; i++)
            {
                Words.Add(_options[i]);
            }
            Words.Shuffle();
        }
        else
            Alert.Show(response.Errors.FirstOrDefault(), MessageType.Error);
    }

    [RelayCommand]
    private async Task SelectedWordChangedAsync()
    {
        if (SelectedWord == _correctWord)
        {
            // you win the game
        }
        else
        {
            Words.Remove(SelectedWord);
            _selectedCount++;
            if (_selectedCount == 5)
            {
                Alert.Show("You lost the game! No more choice.", MessageType.Error);
                return;
            }
            Words.Add(_options[10 + _selectedCount]);
            Words.Shuffle();
            Alert.Show("Wrong answer!!!", MessageType.Error);
        }
    }

    [RelayCommand]
    private async Task DrawingPointAddedAsync(Point pt)
    {
        await _hub.SendDrawingPoint(_gameCode, (float)pt.X, (float)pt.Y);
    }

    [RelayCommand]
    private async Task DrawingLineCompletedAsync()
    {
        await _hub.SendDrawingCompleted(_gameCode);
    }
}