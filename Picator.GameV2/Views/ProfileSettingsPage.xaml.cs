using Picator.Game.ViewModels;
using System.ComponentModel;

namespace Picator.Game.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ProfileSettingsPage : ContentPage
{
    private const uint SheetAnimationDuration = 280;
    private const double SheetOffscreenOffset = 700;

    private ProfileSettingsViewModel _viewModel;
    private bool _sheetVisible;

    public ProfileSettingsPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
        BindingContextChanged += OnBindingContextChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((ProfileSettingsViewModel) BindingContext).RefreshImage();
    }

    private void OnBindingContextChanged(object sender, EventArgs e)
    {
        if (_viewModel != null)
            _viewModel.PropertyChanged -= OnViewModelPropertyChanged;

        _viewModel = BindingContext as ProfileSettingsViewModel;

        if (_viewModel != null)
            _viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    private async void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(ProfileSettingsViewModel.IsAvatarPickerVisible))
            return;

        var isVisible = _viewModel?.IsAvatarPickerVisible == true;
        if (isVisible == _sheetVisible)
            return;

        _sheetVisible = isVisible;
        if (isVisible)
            await AnimateSheetInAsync();
    }

    private async Task AnimateSheetInAsync()
    {
        AvatarSheet.CancelAnimations();
        AvatarSheet.TranslationY = SheetOffscreenOffset;
        await AvatarSheet.TranslateTo(0, 0, SheetAnimationDuration, Easing.CubicOut);
    }

    private void CarouselView_OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
    }

    private void PreviousTap_OnTapped(object sender, EventArgs e)
    {
        if (AvatarView.Position != 0)
            AvatarView.Position--;
    }

    private void NextTap_OnTapped(object sender, EventArgs e)
    {
        if (AvatarView.Position != 7)
            AvatarView.Position++;
    }
}
