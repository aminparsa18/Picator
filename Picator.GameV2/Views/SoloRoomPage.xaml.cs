using Picator.Game.ViewModels;

namespace Picator.Game.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SoloRoomPage : ContentPage
{
    public SoloRoomPage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (BindingContext is SoloRoomViewModel viewModel)
            await viewModel.OpenRoomAsync();
    }

    protected override async void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);

        if (BindingContext is SoloRoomViewModel viewModel)
            await viewModel.DisposeAsync();
    }
}
