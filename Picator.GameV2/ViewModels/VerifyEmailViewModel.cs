namespace Picator.Game.ViewModels;

public partial class VerifyEmailViewModel : ViewModelBase, IQueryAttributable
{
    [ObservableProperty]
    private string _email;

    [ObservableProperty]
    private bool _resent;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("email", out var email))
            Email = Uri.UnescapeDataString((string)email);
    }

    [RelayCommand]
    private async Task OpenEmailApp()
    {
        try
        {
            await Launcher.Default.OpenAsync("mailto:");
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex, new Dictionary<string, string>()
            {
                {"Task", nameof(OpenEmailApp)},
                {"Sender", nameof(VerifyEmailViewModel)}
            });
        }
    }

    [RelayCommand]
    private void Resend()
    {
        // TODO: wire up once a resend-verification-email API endpoint exists.
    }

    [RelayCommand]
    private async Task Pop()
    {
        await Shell.Current.GoToAsync("..");
    }
}
