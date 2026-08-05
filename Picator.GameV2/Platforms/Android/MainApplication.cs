using Android.App;
using Android.Runtime;

namespace Picator.GameV2;

[Application(UsesCleartextTraffic = true)]
public class MainApplication : MauiApplication
{
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership)
    {
        // Without these, an unhandled exception just kills the process instantly --
        // no log, no dialog, the app "closes immediately" as if it never ran.
        // AndroidEnvironment.UnhandledExceptionRaiser is the one that matters most:
        // it fires early enough (and Handled = true keeps the process alive) to
        // actually stop the crash and show the alert below. AppDomain/TaskScheduler
        // are added too so background-thread and fire-and-forget task exceptions
        // get logged as well.
        AndroidEnvironment.UnhandledExceptionRaiser += OnAndroidUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandledException;
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    private void OnAndroidUnhandledException(object? sender, RaiseThrowableEventArgs e)
    {
        LogAndAlert("Unhandled exception", e.Exception);
        e.Handled = true;
    }

    private void OnAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
            LogAndAlert("Fatal unhandled exception", ex);
    }

    private void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        LogAndAlert("Unobserved task exception", e.Exception);
        e.SetObserved();
    }

    private static void LogAndAlert(string title, Exception ex)
    {
        // Goes to logcat under the app's process tag either way; if you're not
        // seeing it in the run-r.sh terminal, `adb logcat` will still have it.
        Console.WriteLine($"[{title}] {ex}");
        System.Diagnostics.Trace.TraceError($"[{title}] {ex}");

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            var page = Microsoft.Maui.Controls.Application.Current?.MainPage;
            if (page is not null)
                await page.DisplayAlert(title, ex.ToString(), "OK");
        });
    }
}
