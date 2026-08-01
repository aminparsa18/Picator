using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Picator.GameV2.Views.Components;
using Plugin.Maui.Audio;
using Plugin.MauiMtAdmob;
using SkiaSharp.Views.Maui.Controls.Hosting;
using ZXing.Net.Maui.Controls;

#if ANDROID
using Picator.GameV2.Platforms.Android.Handlers;
#endif

#if IOS
using AVFoundation;
#endif

namespace Picator.GameV2;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .AddAudio()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement(isAndroidForegroundServiceEnabled: false)
            .UseMauiMTAdmob()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Kalam-Bold.ttf", "KalamBold");
                fonts.AddFont("Kalam-Regular.ttf", "KalamRegular");
                fonts.AddFont("Kalam-Light.ttf", "KalamLight");
                fonts.AddFont("PatrickHand-Regular.ttf", "PatrickHandRegular");
                fonts.AddFont("SpaceMono-Regular.ttf", "SpaceMonoRegular");
                fonts.AddFont("SpaceMono-Bold.ttf", "SpaceMonoBold");
            }).ConfigureMauiHandlers(handlers =>
            {
#if ANDROID
                handlers.AddHandler<QrCodeView, QrCodeViewHandler>();
#endif
            });
        builder.Services.AddSingleton(AudioManager.Current);
#if DEBUG
        builder.Logging.AddDebug();
#endif

#if IOS
        AVAudioSession.SharedInstance().SetActive(true);
        AVAudioSession.SharedInstance().SetCategory(AVAudioSessionCategory.Playback);
#endif

        return builder.Build();
    }
}
