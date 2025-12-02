namespace Picator.Game.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var a = new Animation
            {
                {0, 1, new Animation(v => Logo.TranslationY = v, 96, 0, Easing.SpringOut)},
            };
        a.Commit(
            Logo,
            "flip1",
            length: 3000);
    }

    private void RegisterBtn_OnClicked(object sender, EventArgs e)
    {
        RegisterBtn.BackgroundColor = Colors.Black;
        LoginBtn.BackgroundColor = Color.FromArgb("#232228");
        RegisterPanel.ScaleY = 0;
        RegisterPanel.IsVisible = true;
        RegisterPanel.ScaleYToAsync(1, 400, Easing.SpringOut);
        RegisterPanel.FadeToAsync(1, 400, Easing.Linear);
        LoginPanel.FadeToAsync(0, 400, Easing.Linear);
        LoginPanel.IsVisible = false;
        var a = new Animation
            {
                {0, 1, new Animation(v => Logo.TranslationY = v, 96, 0, Easing.SpringOut)},
            };
        a.Commit(
            Logo,
            "flip1",
            length: 1500);
    }

    private void LoginBtn_OnClicked(object sender, EventArgs e)
    {
        LoginBtn.BackgroundColor = Colors.Black;
        RegisterBtn.BackgroundColor = Color.FromArgb("#707070");

        LoginPanel.ScaleY = 0;
        LoginPanel.IsVisible = true;
        LoginPanel.ScaleYToAsync(1, 400, Easing.SpringOut);
        LoginPanel.FadeToAsync(1, 400, Easing.Linear);
        RegisterPanel.FadeToAsync(0, 400, Easing.Linear);
        RegisterPanel.IsVisible = false;
        var a = new Animation
            {
                {0, 1, new Animation(v => Logo.TranslationY = v, 96, 0, Easing.SpringOut)},
            };
        a.Commit(
            Logo,
            "flip1",
            length: 1500);
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
        RegisterUsernameEntry.IsEnabled = false;
        RegisterUsernameEntry.IsEnabled = true;
        RegisterPassEntry.IsEnabled = false;
        RegisterPassEntry.IsEnabled = true;
        ConfPassEntry.IsEnabled = false;
        ConfPassEntry.IsEnabled = true;
    }
}