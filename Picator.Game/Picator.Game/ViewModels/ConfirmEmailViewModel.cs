using Mafiator.Common.Client.Services.Users;
using Picator.Common.Data.Dtos.Api.Auth;
using Picator.Common.Data.Dtos.Users;
using Picator.Game.Cache;
using Picator.Game.Extensions;
using Picator.Game.Views.Popups;
using Rg.Plugins.Popup.Services;
using System.Net.Http.Headers;

namespace Picator.Game.ViewModels;

public partial class ConfirmEmailViewModel : ViewModelBase
{
    [ObservableProperty]
    private ValidatableObject<string> _code;

    private readonly IUsersApiService _usersApiService;
    private readonly string? email;

    public ConfirmEmailViewModel()
    {
        _usersApiService = new UsersApiService(); ;
        Code = new ValidatableObject<string>();
        AddValidations();
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        var isValid = CodeValidate();
        if (!isValid)
            return;
        await PopupNavigation.Instance.PushAsync(new WaitingView("Checking Code..."));
        var response = await _usersApiService.ConfirmEmail(new ConfirmEmailRequest()
        { Email = email, Token = Code.Value });
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsMemoryPackAsync<AuthResult>();
            if (result.IsSuccess)
            {
                Barrel.Current.Add("Token", result.Token, TimeSpan.FromMinutes(6));
                Barrel.Current.Add("RefreshToken", result.RefreshToken, TimeSpan.FromDays(150));
                BaseHttpClient.Instance.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", result.Token);
                await Application.Current.MainPage.Navigation.PushAsync(new ProfileSettingsPage());
            }
            else
            {
                Alert.Show(string.Join(',', result.Errors), MessageType.Error);
            }
        }
        else
        {
            var result = await response.Content.ReadAsMemoryPackAsync<AuthResult>();
            Alert.Show(string.Join(',', result.Errors), MessageType.Error);
        }
        await PopupNavigation.Instance.PopAsync();
    }

    [RelayCommand]
    private async Task Pop()
    {
        await Application.Current.MainPage.Navigation.PopAsync();
    }

    private void AddValidations()
    {
        Code.Validations.Add(new IsNotNullOrEmptyRule<string>
        {
            ValidationMessage = "Enter activation code"
        });
    }

    private bool CodeValidate()
    {
        return Code.Validate();
    }
}