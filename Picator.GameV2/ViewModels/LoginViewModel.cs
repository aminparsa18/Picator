using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Platform;
using Mafiator.Common.Client.Services.Avatars;
using Mafiator.Common.Client.Services.Users;
using Mopups.Services;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Game.Cache;
using Picator.Game.Constants;
using Picator.Game.Extensions;
using Picator.Game.Services.Avatars;
using Picator.Game.Views.Popups;
using System.Net.Http.Headers;

namespace Picator.Game.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IUsersApiService _usersApiService;
    private readonly IAvatarsApiService _avatarsApiService;

    [ObservableProperty]
    private ValidatableObject<string> _loginUsername;

    [ObservableProperty]
    private ValidatableObject<string> _loginPassword;

    [ObservableProperty]
    private ValidatableObject<string> _username;

    [ObservableProperty]
    private ValidatableObject<string> _password;

    [ObservableProperty]
    private ValidatableObject<string> _confirmPassword;

    public LoginViewModel()
    {
        _avatarsApiService = new AvatarsApiService();
        _usersApiService = new UsersApiService();
        LoginUsername = new ValidatableObject<string>();
        LoginPassword = new ValidatableObject<string>();
        Username = new ValidatableObject<string>();
        Password = new ValidatableObject<string>();
        ConfirmPassword = new ValidatableObject<string>();
        AddValidations();
    }

    [RelayCommand]
    private async Task Google()
    {
        try
        {
#if WINDOWS
            ExternalAuthService.AuthResultTcs = new TaskCompletionSource<ExternalAuthResult>();
            var authUrl = "https://localhost:7045/mobileauth/Google?redirect_uri=app://auth.pctor"; // Add redirect_uri
          //  Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });
            var result = await ExternalAuthService.AuthResultTcs.Task;
            if (result != null)
            {
                // Handle successful authentication
                Barrel.Current.Add("Token", result.Token, TimeSpan.FromMinutes(20));
                Barrel.Current.Add("RefreshToken", result.RefreshToken, TimeSpan.FromDays(150));
                BaseHttpClient.Instance.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", result.Token);
            }
#else
            var authResult = await WebAuthenticator.AuthenticateAsync(
                new Uri($"{UrlConstants.ExternalAuthUrl}/mobileauth/Google"),
                new Uri("app://auth.pctor"));
            var accessToken = authResult.Properties["token"];
#endif
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex);
        }
        finally
        {
            await Application.Current.MainPage.Navigation.PopAsync();

        }
    }

    private void AddValidations()
    {
        LoginUsername.Validations.Add(new IsNotNullOrEmptyRule<string>()
        { ValidationMessage = "EmptyUsername" });
        LoginPassword.Validations.Add(new IsNotNullOrEmptyRule<string>()
        { ValidationMessage = "EmptyPassword" });
        Username.Validations.Add(new IsNotNullOrEmptyRule<string>()
        { ValidationMessage = "EmptyUsername" });
        Password.Validations.Add(new IsNotNullOrEmptyRule<string>()
        { ValidationMessage = "EmptyPassword" });
        Password.Validations.Add(new PasswordRule<string>());
        ConfirmPassword.Validations.Add(new IsNotNullOrEmptyRule<string>()
        { ValidationMessage = "EmptyConfirmPassword" });
        ConfirmPassword.Validations.Add(new PasswordRule<string>());
    }

    [RelayCommand]
    private async Task Login()
    {
        var isValid = ValidateLogin();
        if (!isValid)
            return;
        IsBusy = true;
        try
        {
            var response = await _usersApiService.Login(new UserLoginRequest()
            {
                Password = LoginPassword.Value,
                Username = LoginUsername.Value
            });

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsMemoryPackAsync<AuthResult>();
                if (result.IsSuccess)
                {
                    Barrel.Current.Add("Token", result.Token, TimeSpan.FromMinutes(20));
                    Barrel.Current.Add("RefreshToken", result.RefreshToken, TimeSpan.FromDays(150));
                    BaseHttpClient.Instance.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", result.Token);
                    await GetUserDetails();
                    await Application.Current.MainPage.Navigation.PushAsync(new MainPage());
                }
                else if (result.StatusCode == ApiResultStatusCode.Forbidden)
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new ConfirmEmailPage());
                    await Snackbar.Make(string.Join(",", result.Errors)).Show();
                }
                else
                {
                    await Snackbar.Make(string.Join(",", result.Errors)).Show();
                }
            }
            else
            {
                var reason = await response.Content.ReadAsStringAsync();
                await Snackbar.Make(reason).Show();
            }
        }
        catch (Exception ex)
        {
            Crashes.TrackError(ex, new Dictionary<string, string>()
                {
                    {"Task", nameof(Login)},
                    {"Sender", nameof(LoginViewModel)}
                });
            await Snackbar.Make(ex.Message).Show();
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task GetUserDetails()
    {
        var result = await _usersApiService.GetUser();
        if (result.IsSuccess && result.Data is not null)
        {
            Barrel.Current.Add("User", MemoryPack.MemoryPackSerializer.Serialize(result.Data.Avatar), TimeSpan.FromDays(30));
        }
        else
        {
            await Snackbar.Make(string.Join("-", result.Errors)).Show();
        }
    }

    [RelayCommand]
    private async Task Register()
    {
        var isValid = ValidateRegister();
        if (isValid)
        {
            IsBusy = true;
            try
            {
                var response = await _usersApiService.RegisterUser(new RegisterUserRequest()
                {
                    UserName = Username.Value,
                    Password = Password.Value,
                });
                var result = await response.Content.ReadAsMemoryPackAsync<ApiResult>();
                if (response.IsSuccessStatusCode)
                {
                    if (result.IsSuccess)
                        await Snackbar.Make("You are registered successfully! Check your email inbox for activation").Show();
                    else
                    {
                        await Snackbar.Make(string.Join(',', result.Errors)).Show();
                    }
                }
                else
                {
                    await Snackbar.Make(string.Join(',', result.Errors)).Show();
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>()
                {
                    {"Task", nameof(Register)},
                    {"Sender", nameof(LoginViewModel)}
                });
                await Snackbar.Make(ex.Message).Show();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }

    private bool ValidateLogin()
    {
        return LoginUsername.Validate() && LoginPassword.Validate();
    }

    private bool ValidateRegister()
    {
        if (Password.Value != ConfirmPassword.Value)
            Snackbar.Make("PasswordNotMatch").Show();
        return Username.Validate() && Password.Validate() && ConfirmPassword.Validate();
    }

    public class ExternalAuthResult
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public static class ExternalAuthService
    {
        public static TaskCompletionSource<ExternalAuthResult> AuthResultTcs { get; set; }

        public static void HandleExternalAuthResult(ExternalAuthResult result)
        {
            AuthResultTcs?.TrySetResult(result);
        }
    }
}