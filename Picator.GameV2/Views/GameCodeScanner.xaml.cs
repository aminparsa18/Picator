using ZXing.Net.Maui;

namespace Picator.GameV2.Views;

public partial class GameCodeScanner : ContentPage
{
	public GameCodeScanner()
	{
		InitializeComponent();
	}

    protected async void BarcodesDetected(object sender, BarcodeDetectionEventArgs e)
    {
        await Launcher.Default.OpenAsync(e.Results.FirstOrDefault()?.Value ?? "");
    }
}