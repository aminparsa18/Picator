using Mafiator.Common.Client.Services.Avatars;
using Mafiator.Common.Client.Services.Users;
using Mopups.Services;
using Picator.Common.Data.Dtos.Api;
using Picator.Common.Data.Dtos.Users;
using Picator.Game.Cache;
using Picator.Game.Extensions;
using Picator.Game.Mappers;
using Picator.Game.Models;
using Picator.Game.Services.Avatars;
using Picator.Game.Views.Popups;
using System.Collections.ObjectModel;

namespace Picator.Game.ViewModels;

public partial class ProfileSettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private ValidatableObject<string> _displayName;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private Avatar _avatar;

    [ObservableProperty]
    private ImageSource _image = "choose_photo.png";

    [ObservableProperty]
    private bool nameSaved;

    [ObservableProperty]
    private bool isAvatarPickerVisible;

    [ObservableProperty]
    private string currentPassword = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string passwordError = string.Empty;

    [ObservableProperty]
    private bool hasPasswordError;

    [ObservableProperty]
    private bool passwordSaved;

    partial void OnPasswordErrorChanged(string value)
    {
        HasPasswordError = !string.IsNullOrEmpty(value);
    }

    public ObservableCollection<Avatar> Avatars { get; set; }

    private IEnumerable<Avatar> avatars;
    private bool isEdit;

    private readonly IAvatarsApiService _avatarsApiService;
    private readonly IUsersApiService _usersApiService;

    public ProfileSettingsViewModel()
    {
        _avatarsApiService = new AvatarsApiService();
        _usersApiService = new UsersApiService();
        DisplayName = new ValidatableObject<string>();
        Avatars = new ObservableCollection<Avatar>();
        _ = LoadAvatarsCommand.ExecuteAsync(null);
        AddValidations();
    }

    private void AddValidations()
    {
        DisplayName.Validations.Add(new IsNotNullOrEmptyRule<string>());
    }

    [RelayCommand]
    private async Task AvatarSelectedAsync()
    {
        await MopupService.Instance.PushAsync(new WaitingView("Changing profile picture"));
        try
        {
            var response = await _usersApiService.UpdateAvatar(new UpdateAvatarRequest()
            {
                Image = Path.GetFileName(new Uri(Avatar.Name).LocalPath),
            });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsMemoryPackAsync<ApiResult>();
                if (result.IsSuccess)
                {
                    Barrel.Current.Add("UserImage", Avatar.Name, TimeSpan.FromDays(180));
                    Barrel.Current.Empty("User");
                    if (isEdit)
                    {
                        //_publisher.Publish(new UpdateProfileEvent());
                    }
                    Image = Avatar.Name;
                    IsAvatarPickerVisible = false;
                }
                else
                    Alert.Show(result.Errors.ToString(), MessageType.Error);
            }
            else
            {
                var result = await response.Content.ReadAsMemoryPackAsync<ApiResult>();
                Alert.Show(result.Errors.ToString(), MessageType.Error);
            }
        }
        catch (Exception ex)
        {
            //Crashes.TrackError(ex);
            Alert.Show(ex.Message, MessageType.Error);
        }
        finally
        {
            await MopupService.Instance.PopAsync();
        }
    }

    [RelayCommand]
    private async Task SaveNameAsync()
    {
        if (!Validate())
            return;
        var response = await _usersApiService.UpdateDisplayName(new UpdateDisplayNameRequest
        {
            Name = DisplayName.Value,
        });
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsMemoryPackAsync<ApiResult>();
            if (result.IsSuccess)
            {
                Barrel.Current.Empty("User");
                NameSaved = true;
                ResetNameSavedAfterDelay();
            }
            else
                Alert.Show(result.Errors.ToString(), MessageType.Error);
        }
        else
        {
            var result = await response.Content.ReadAsMemoryPackAsync<ApiResult>();
            Alert.Show(result.Errors.ToString(), MessageType.Error);
        }
    }

    private async void ResetNameSavedAfterDelay()
    {
        await Task.Delay(2000);
        NameSaved = false;
    }

    [RelayCommand]
    private void OpenAvatarPicker()
    {
        IsAvatarPickerVisible = true;
    }

    [RelayCommand]
    private void CloseAvatarPicker()
    {
        IsAvatarPickerVisible = false;
    }

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        if (string.IsNullOrEmpty(CurrentPassword) || string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
        {
            PasswordSaved = false;
            PasswordError = "Fill in all password fields.";
            return;
        }
        if (NewPassword != ConfirmPassword)
        {
            PasswordSaved = false;
            PasswordError = "New passwords do not match.";
            return;
        }
        if (NewPassword.Length < 6)
        {
            PasswordSaved = false;
            PasswordError = "New password must be at least 6 characters.";
            return;
        }

        PasswordError = string.Empty;
        try
        {
            var response = await _usersApiService.ChangePassword(new ChangePasswordRequest
            {
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
            });
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsMemoryPackAsync<ApiResult>();
                if (result.IsSuccess)
                {
                    PasswordSaved = true;
                    CurrentPassword = string.Empty;
                    NewPassword = string.Empty;
                    ConfirmPassword = string.Empty;
                    ResetPasswordSavedAfterDelay();
                }
                else
                    PasswordError = result.Errors?.ToString() ?? "Could not update password.";
            }
            else
                PasswordError = "Could not update password.";
        }
        catch (Exception)
        {
            PasswordError = "Could not update password. Please try again later.";
        }
    }

    private async void ResetPasswordSavedAfterDelay()
    {
        await Task.Delay(2500);
        PasswordSaved = false;
    }

    [RelayCommand]
    private async Task LoadAvatarsAsync()
    {
        var response = await _avatarsApiService.GetAllAvatars();
        if (response.IsSuccess)
        {
            avatars = new AvatarMapper().AvatarResultToAvatar(response.Data);
            Avatars.Clear();
            foreach(var avatar in avatars)
            {
                Avatars.Add(avatar);
            }
        }
        else
        {
            Alert.Show(response.Errors.ToString(), MessageType.Error);
        }
    }

    [RelayCommand]
    private async Task SkipAsync()
    {
        await Application.Current.MainPage.Navigation.PopAsync();
    }

    public void RefreshImage()
    {
        Name = Barrel.Current.Get<string>("UserImage");
    }

    private bool Validate()
    {
        return DisplayName.Validate();
    }
}