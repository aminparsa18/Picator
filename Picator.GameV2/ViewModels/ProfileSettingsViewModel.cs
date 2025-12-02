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
using System.Web;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

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
    private bool photoSet;

    public ObservableCollection<Avatar> Avatars { get; set; }

    private IEnumerable<Avatar> avatars;
    private FileResult photo;
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
    private async Task SetPhotoAsync()
    {
        if (!Validate())
            return;
        await MopupService.Instance.PushAsync(new WaitingView("Uploading Image..."));
        //try
        //{
        //    var blobServiceClient = new BlobServiceClient(
        //        "DefaultEndpointsProtocol=https;AccountName=mftor;AccountKey=pLoQjG6uKWpWe1vG+iVU+zKjYRpuM/tPKACmd/kM/AuBXHHfsvLOGKXsq96BusnCfrx/4St1INHVk4tibVLElA==;EndpointSuffix=core.windows.net");
        //    var blobContainerClient = blobServiceClient.GetBlobContainerClient("avatars");
        //    var blobClient = blobContainerClient.GetBlobClient(HttpUtility.UrlEncode(photo.FileName));
        //    await blobClient.UploadAsync(System.IO.File.OpenRead(photo.FullPath), new BlobUploadOptions());
        //    await SetProfilePicture(HttpUtility.UrlEncode(photo.FileName));
        //}
        //catch (RequestFailedException)
        //{
        //    await MopupService.Instance.PopAsync();
        //    Alert.Show("Upload failed,try again later", MessageType.Error);
        //}
    }

    [RelayCommand]
    private async Task ChoosePhotoAsync()
    {
        try
        {
            //   var result = await DialogService.ShowPickImageAsync();
            // if (result == LocalizationResourceManager.Current.GetValue("Camera"))
            // {
            //   photo = await MediaPicker.CapturePhotoAsync();
            //await NavigationService.NavigateToPopupAsync<CropImageViewModel>(photo.FullPath);
            //}
            // else
            //{
            photo = await MediaPicker.PickPhotoAsync();
            //    //  await NavigationService.NavigateToPopupAsync<CropImageViewModel>(photo.FullPath);
            //}

            if (photo == null)
                return;
            PhotoSet = true;
            Image = ImageSource.FromFile(photo.FullPath);
        }
        catch (Exception e)
        {
            await MopupService.Instance.PopAsync();
            Alert.Show(e.Message, MessageType.Error);
        }
    }

    [RelayCommand]
    private async Task AvatarSelectedAsync()
    {
        if (!Validate())
            return;
        await MopupService.Instance.PushAsync(new WaitingView("Changing profile picture"));
        var response = await _usersApiService.UpdateProfile(new UpdateProfileRequest()
        {
            Image = System.IO.Path.GetFileName(new Uri(Avatar.Name).LocalPath),
            Name = DisplayName.Value
        });
        await MopupService.Instance.PopAsync();
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
                await Application.Current.MainPage.Navigation.PopAsync();
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
            Alert.Show(response.Errors.ToString(), MessageType.Error);
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

    public async Task SetProfilePicture(string image)
    {

        var response = await _usersApiService.UpdateProfile(new UpdateProfileRequest()
        {
            Image = image,
            Name = DisplayName.Value
        });
        var result = await response.Content.ReadAsMemoryPackAsync<ApiResult>();
        if (response.IsSuccessStatusCode)
        {
            if (result.IsSuccess)
            {
                Barrel.Current.Add("UserImage", DisplayName, TimeSpan.FromDays(180));
                Barrel.Current.Empty("User");
                await MopupService.Instance.PopAsync();
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await MopupService.Instance.PopAsync();
                Alert.Show(result.Errors.ToString(), MessageType.Error);
            }
        }
        else
        {
            await MopupService.Instance.PopAsync();
            Alert.Show(result.Errors.ToString(), MessageType.Error);
        }
    }

    private bool Validate()
    {
        return DisplayName.Validate();
    }
}