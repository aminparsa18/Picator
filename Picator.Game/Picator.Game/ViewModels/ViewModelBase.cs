using CommunityToolkit.Mvvm.ComponentModel;
using Picator.Game.Services;
using Xamarin.Forms;

namespace Picator.Game.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    protected IAlert Alert;

    [ObservableProperty]
    private string _waitingMessage;

    [ObservableProperty]
    private string _errorMessage;

    public ViewModelBase()
    {
        Alert = DependencyService.Get<IAlert>();
    }
}