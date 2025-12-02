using Picator.Game.ViewModels;

namespace Picator.Game.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class ProfileSettingsPage : ContentPage
{
    public ProfileSettingsPage()
    {
        InitializeComponent();
        NavigationPage.SetHasNavigationBar(this, false);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ((ProfileSettingsViewModel) BindingContext).RefreshImage();
    }

    private void CarouselView_OnCurrentItemChanged(object sender, CurrentItemChangedEventArgs e)
    {
        //if (e.CurrentItem is Avatar avatar)
        //    avatar.Scale = 1.2;
        //if (e.PreviousItem is Avatar pAvatar)
        //   pAvatar.Scale = 1;
    }

    private async void Switch_OnToggled(object sender, ToggledEventArgs e)
    {
        if (e.Value)
        {
            AvatarFrame.IsVisible = true;
            AvatarFrame.TranslateTo(0, 100, 800, Easing.SpringOut);
        }
        else
        {
            await AvatarFrame.TranslateTo(0, 0, 600, Easing.SpringIn);
            AvatarFrame.IsVisible = false;
        }
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