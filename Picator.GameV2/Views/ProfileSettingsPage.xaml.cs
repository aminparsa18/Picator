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
        OnBindingContextChanged(this, EventArgs.Empty);
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
        else
            await AnimateSheetOutAsync();
    }

    private async Task AnimateSheetInAsync()
    {
        AvatarSheet.CancelAnimations();
        AvatarPickerRoot.IsVisible = true;
        AvatarSheet.TranslationY = SheetOffscreenOffset;
        await AvatarSheet.TranslateTo(0, 0, SheetAnimationDuration, Easing.CubicOut);
    }

    private async Task AnimateSheetOutAsync()
    {
        AvatarSheet.CancelAnimations();
        await AvatarSheet.TranslateTo(0, SheetOffscreenOffset, SheetAnimationDuration, Easing.CubicIn);

        // Only hide if nothing re-opened the sheet while it was animating out.
        if (!_sheetVisible)
            AvatarPickerRoot.IsVisible = false;
    }

    private void CarouselView_OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
    }

    private void PreviousTap_OnTapped(object sender, EventArgs e)
    {
        MoveAvatarSelection(-1);
    }

    private void NextTap_OnTapped(object sender, EventArgs e)
    {
        MoveAvatarSelection(1);
    }

    private void MoveAvatarSelection(int delta)
    {
        var avatars = _viewModel?.Avatars;
        if (avatars == null || avatars.Count == 0)
            return;

        var currentIndex = avatars.IndexOf(_viewModel.Avatar);
        if (currentIndex < 0)
            currentIndex = 0;

        var nextIndex = (currentIndex + delta + avatars.Count) % avatars.Count;
        _viewModel.Avatar = avatars[nextIndex];
    }
}
