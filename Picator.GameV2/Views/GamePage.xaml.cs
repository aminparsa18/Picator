using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;
using Picator.Game.Hubs;
using Picator.Game.ViewModels;
using System.Collections.ObjectModel;

namespace Picator.Game.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GamePage : ContentPage
{
    private List<PointF> _points = [];
    private Color _selectedColor = Colors.Black;
    private float _selectedThickness = 5f;

    public ObservableCollection<IDrawingLine> _lines { get; set; } = [new DrawingLine() { Points = [], LineColor = Colors.Black }];

    public GamePage(bool isDrawingPlayer, string? gameCode)
    {
        InitializeComponent();
        PlayerDrawingView.Lines = _lines;
       
        this.BindingContext = new GameViewModel(isDrawingPlayer, gameCode);
        if (!isDrawingPlayer)
        {
            GameHub.Instance.PointReceived += PointReceived;
            GameHub.Instance.LineCompleted += LineCompleted;
            GameHub.Instance.ColorReceived += ColorReceived;
            GameHub.Instance.ThicknessReceived += ThicknessReceived;
        }
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        if (BindingContext is GameViewModel viewModel)
        {
            await viewModel.OnNavigatedTo();
        }
    }

    protected override async void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

        if (BindingContext is GameViewModel vm)
        {
            await vm.DisposeAsync();
        }
    }

    private void DrawingView_DrawingLineStarted(object sender, CommunityToolkit.Maui.Core.DrawingLineStartedEventArgs e)
    {
        DrawHereLbl.IsVisible = false;
    }

    private void LineCompleted(object? sender, EventArgs e)
    {

        MainThread.BeginInvokeOnMainThread(() =>
        {
            _lines.Add(new DrawingLine
            {
                Points = [],
                LineColor = _selectedColor,
                LineWidth = _selectedThickness
            });
            _points.Clear();
        });
    }

    private void PointReceived(object? sender, PointF e)
    {
        _points.Add(new PointF(e.X, e.Y));
        MainThread.BeginInvokeOnMainThread(() =>
        {
            _lines.RemoveAt(_lines.Count - 1);
            _lines.Add(new DrawingLine
            {
                Points = new ObservableCollection<PointF>(_points),
                LineColor = _selectedColor,
                LineWidth = _selectedThickness
            });
        });
    }

    private void ThicknessReceived(object? sender, float e)
    {
        _selectedThickness = e;
    }

    private void ColorReceived(object? sender, uint e)
    {
        _selectedColor = Color.FromUint(e);
    }

    private async void ImageButton_Clicked(object sender, EventArgs e)
    {
        _selectedColor = Colors.LightPink;
        DrawerDrawingView.LineColor = _selectedColor;
        await GameHub.Instance.SendDrawingColor(_selectedColor.ToUint());
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var ss = e.CurrentSelection.First();
        _selectedColor = (Color)ss;
        DrawerDrawingView.LineColor = _selectedColor;
        await GameHub.Instance.SendDrawingColor(_selectedColor.ToUint());
    }

    private async void CollectionView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
    {
        var ss = e.CurrentSelection.First();
        _selectedThickness = (float)ss;
        DrawerDrawingView.LineWidth = _selectedThickness;
        await GameHub.Instance.SendDrawingThickness(_selectedThickness);
    }
}