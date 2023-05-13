using Mafiator.Common.Client.Services.Avatars;
using Mafiator.Common.Client.Services.Users;
using Microsoft.AppCenter.Crashes;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Game.Cache;
using Picator.Game.Extensions;
using Picator.Game.Services.Avatars;
using Picator.Game.Views.Popups;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Xamarin.Essentials;

namespace Picator.Game.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    //OAuth2Authenticator? oAuth2Authenticator;

    public static EventHandler OnPresenter;

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
            // legacy
            //oAuth2Authenticator = OAuthAuthenticatorHelper.CreateOAuth2();
            //oAuth2Authenticator.Completed += OAuth2Authenticator_Completed;
            //oAuth2Authenticator.Error += OAuth2Authenticator_Error;
            //var presenter = new OAuthLoginPresenter();
            //presenter.Login(oAuth2Authenticator);
            var authResult = await WebAuthenticator.AuthenticateAsync(
                new Uri("https://picatorexternalauth-app-20230511.victoriousrock-9f2ad982.centralus.azurecontainerapps.io/Auth/Google"),
                new Uri("https://picatorexternalauth-app-20230511.victoriousrock-9f2ad982.centralus.azurecontainerapps.io/redirect"));

            var accessToken = authResult?.AccessToken;


        }
        catch (Exception ex)
        {
            int a = 2;
        }
    }

    //private void OAuth2Authenticator_Completed(object sender, AuthenticatorCompletedEventArgs e)
    //{
    //    if (e.IsAuthenticated)
    //    {
    //        var account = e.Account;

    //        string email = string.Empty;


    //        // email = await ProviderService.GetGoogleEmailAsync();


    //        //await SecureStorage.SetAsync("Email", email);

    //        //await Application.Current.MainPage.Navigation.PushAsync(new MainPage());

    //    }
    //    else
    //    {
    //        oAuth2Authenticator.OnCancelled();
    //        oAuth2Authenticator = default;
    //    }
    //}

    //private async void OAuth2Authenticator_Error(object sender, AuthenticatorErrorEventArgs e)
    //{
    //    OAuth2Authenticator authenticator = (OAuth2Authenticator)sender;
    //    if (authenticator != null)
    //    {
    //        authenticator.Completed -= OAuth2Authenticator_Completed;
    //        authenticator.Error -= OAuth2Authenticator_Error;
    //    }

    //    string title = "Authentication Error";
    //    string msg = e.Message;

    //    await Application.Current.MainPage.DisplayAlert(title, msg, "OK");
    //}

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
        if (isValid)
        {
            await PopupNavigation.Instance.PushAsync(new WaitingView("LoggingIn"));
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
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    else if (result.StatusCode == ApiResultStatusCode.Forbidden)
                    {
                        await Application.Current.MainPage.Navigation.PushAsync(new ConfirmEmailPage());
                        Alert.Show(string.Join(",", result.Errors), MessageType.Error);
                    }
                    else
                    {
                        Alert.Show(string.Join(',', result.Errors), MessageType.Error);
                    }
                }
                else
                {
                    var reason = await response.Content.ReadAsStringAsync();
                    Alert.Show(reason, MessageType.Error);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>()
                {
                    {"Task", nameof(Login)},
                    {"Sender", nameof(LoginViewModel)}
                });
                // Alert.Show(LocalizationResourceManager.Current["ServerError"], MessageType.Error);
            }

            await PopupNavigation.Instance.PopAsync();
        }
    }

    [RelayCommand]
    private async Task Register()
    {
        var isValid = ValidateRegister();
        if (isValid)
        {
            await PopupNavigation.Instance.PushAsync(new WaitingView("RegisteringAccount"));
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
                        await Application.Current.MainPage.Navigation.PushAsync(new ConfirmEmailPage());
                    else
                    {
                        Alert.Show(string.Join(',', result.Errors), MessageType.Error);
                    }
                }
                else
                {
                    Alert.Show(string.Join(',', result.Errors), MessageType.Error);
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string>()
                {
                    {"Task", nameof(Register)},
                    {"Sender", nameof(LoginViewModel)}
                });
                Alert.Show("ServerError", MessageType.Error);
            }

            await PopupNavigation.Instance.PopAsync();
        }
    }

    private bool ValidateLogin()
    {
        return LoginUsername.Validate() && LoginPassword.Validate();
    }

    private bool ValidateRegister()
    {
        if (Password.Value != ConfirmPassword.Value)
            Alert.Show("PasswordNotMatch", MessageType.Error);
        return Username.Validate() && Password.Validate() && ConfirmPassword.Validate();
    }
}