using Picator.Game.Cache;
using Picator.Game.ViewModels;
using System.ComponentModel;

namespace Picator.Game.Views;

public partial class MainPage : ContentPage
{
    private const uint SheetAnimationDuration = 280;
    private const double SheetOffscreenOffset = 700;

    private MainViewModel? _viewModel;
    private bool _sheetVisible;

    public MainPage()
    {
        InitializeComponent();
        BindingContextChanged += OnBindingContextChanged;
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

        _viewModel = BindingContext as MainViewModel;

        if (_viewModel != null)
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private async void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(MainViewModel.IsOverlayVisible))
            return;

        var isVisible = _viewModel?.IsOverlayVisible == true;
        if (isVisible == _sheetVisible)
            return;

        _sheetVisible = isVisible;
        if (isVisible)
            await AnimateSheetInAsync();
    }

    private async Task AnimateSheetInAsync()
    {
        OverlaySheet.CancelAnimations();
        OverlaySheet.TranslationY = SheetOffscreenOffset;
        await OverlaySheet.TranslateTo(0, 0, SheetAnimationDuration, Easing.CubicOut);
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        if(Barrel.Current.Exists("InvitationCode"))
        {
            await Navigation.PushAsync(new GamePage(true, Barrel.Current.Get<string>("InvitationCode")));
            Barrel.Current.Empty("InvitationCode");
        }
    }
}