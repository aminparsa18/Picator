using CommunityToolkit.Mvvm.Input;
using Picator.Game.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Picator.Game.ViewModels;

public sealed partial class MainViewModel : ViewModelBase
{

    [RelayCommand]
    private async Task NavigateToStartGame() =>
        await Application.Current.MainPage.Navigation.PushAsync(new StartNewGamePage());
}