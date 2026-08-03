namespace Picator.Game.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void RegisterBtn_OnClicked(object sender, EventArgs e)
    {
        RegisterBtn.BackgroundColor = (Color)Application.Current.Resources["InkStrong"];
        RegisterBtn.TextColor = (Color)Application.Current.Resources["OnInk"];
        LoginBtn.BackgroundColor = Colors.Transparent;
        LoginBtn.TextColor = (Color)Application.Current.Resources["InkMuted"];
        RegisterPanel.ScaleY = 0;
        RegisterPanel.IsVisible = true;
        RegisterPanel.ScaleYToAsync(1, 400, Easing.SpringOut);
        RegisterPanel.FadeToAsync(1, 400, Easing.Linear);
        LoginPanel.FadeToAsync(0, 400, Easing.Linear);
        LoginPanel.IsVisible = false;
    }

    private void LoginBtn_OnClicked(object sender, EventArgs e)
    {
        LoginBtn.BackgroundColor = (Color)Application.Current.Resources["InkStrong"];
        LoginBtn.TextColor = (Color)Application.Current.Resources["OnInk"];
        RegisterBtn.BackgroundColor = Colors.Transparent;
        RegisterBtn.TextColor = (Color)Application.Current.Resources["InkMuted"];

        LoginPanel.ScaleY = 0;
        LoginPanel.IsVisible = true;
        LoginPanel.ScaleYToAsync(1, 400, Easing.SpringOut);
        LoginPanel.FadeToAsync(1, 400, Easing.Linear);
        RegisterPanel.FadeToAsync(0, 400, Easing.Linear);
        RegisterPanel.IsVisible = false;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        LoginUsernameEntry.IsEnabled = false;
        LoginUsernameEntry.IsEnabled = true;
        LoginPassEntry.IsEnabled = false;
        LoginPassEntry.IsEnabled = true;
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        RegisterDisplayNameEntry.IsEnabled = false;
        RegisterDisplayNameEntry.IsEnabled = true;
        RegisterUsernameEntry.IsEnabled = false;
        RegisterUsernameEntry.IsEnabled = true;
        RegisterPassEntry.IsEnabled = false;
        RegisterPassEntry.IsEnabled = true;
    }
}