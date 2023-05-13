using Picator.Game.ViewModels;
using Xamarin.Forms.Xaml;

namespace Picator.Game.Views;
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class GamePage : ContentPage
{
    public GamePage(bool isDrawingPlayer, string gameCode)
    {
        InitializeComponent();
        this.BindingContext = new GameViewModel(isDrawingPlayer, gameCode);
    }
}