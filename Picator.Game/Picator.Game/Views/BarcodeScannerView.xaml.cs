using BarcodeScanner.Mobile;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace Picator.Game.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class BarcodeScannerView : ContentPage
{
    public BarcodeScannerView()
    {
        InitializeComponent();
    }

    private void CameraView_OnDetected(object sender, OnDetectedEventArg e)
    {
        List<BarcodeResult> obj = e.BarcodeResults;

        string result = string.Empty;
        for (int i = 0; i < obj.Count; i++)
        {
            result += $"Type : {obj[i].BarcodeType}, Value : {obj[i].DisplayValue}{Environment.NewLine}";
        }
        Device.BeginInvokeOnMainThread(async () =>
        {
            await DisplayAlert("Result", result, "OK");
        });
    }
}