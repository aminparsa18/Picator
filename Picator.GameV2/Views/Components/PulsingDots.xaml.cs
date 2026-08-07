using Microsoft.Maui.Controls.Shapes;

namespace Picator.GameV2.Views.Components;

/// <summary>
/// Three-dot "waiting" indicator: each dot pulses opacity/scale on a 1.2s loop,
/// staggered 150ms apart, mirroring the Solo Room design's dotPulse keyframe animation.
/// </summary>
public partial class PulsingDots : ContentView
{
    private const string PulseAnimationName = "DotPulse";
    private const uint CycleMs = 1200;

    public static readonly BindableProperty DotColorProperty =
        BindableProperty.Create(nameof(DotColor), typeof(Color), typeof(PulsingDots), Colors.Gray);

    public Color DotColor
    {
        get => (Color)GetValue(DotColorProperty);
        set => SetValue(DotColorProperty, value);
    }

    public static readonly BindableProperty IsRunningProperty =
        BindableProperty.Create(nameof(IsRunning), typeof(bool), typeof(PulsingDots), false, propertyChanged: OnIsRunningChanged);

    public bool IsRunning
    {
        get => (bool)GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }

    private CancellationTokenSource? _cts;

    public PulsingDots()
    {
        InitializeComponent();
    }

    private static void OnIsRunningChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (PulsingDots)bindable;
        if ((bool)newValue)
            view.Start();
        else
            view.Stop();
    }

    private void Start()
    {
        Stop();
        _cts = new CancellationTokenSource();
        PulseLoop(Dot1, 0, _cts.Token);
        PulseLoop(Dot2, 150, _cts.Token);
        PulseLoop(Dot3, 300, _cts.Token);
    }

    private void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;

        foreach (var dot in new[] { Dot1, Dot2, Dot3 })
        {
            dot.AbortAnimation(PulseAnimationName);
            dot.Opacity = 0.25;
            dot.Scale = 0.8;
        }
    }

    // Loops a single dot's pulse indefinitely until cancelled. Uses the Animation/Commit
    // pattern (rather than chained ViewExtensions) since it needs to repeat on its own clock,
    // staggered from the other two dots by startDelayMs.
    private async void PulseLoop(Ellipse dot, int startDelayMs, CancellationToken token)
    {
        try
        {
            if (startDelayMs > 0)
                await Task.Delay(startDelayMs, token);
        }
        catch (OperationCanceledException)
        {
            return;
        }

        while (!token.IsCancellationRequested)
        {
            var tcs = new TaskCompletionSource();
            var anim = new Animation();
            anim.Add(0.00, 0.40, new Animation(v => dot.Opacity = v, 0.25, 1, Easing.SinOut));
            anim.Add(0.00, 0.40, new Animation(v => dot.Scale = v, 0.8, 1, Easing.SinOut));
            anim.Add(0.40, 1.00, new Animation(v => dot.Opacity = v, 1, 0.25, Easing.SinIn));
            anim.Add(0.40, 1.00, new Animation(v => dot.Scale = v, 1, 0.8, Easing.SinIn));
            anim.Commit(dot, PulseAnimationName, length: CycleMs, finished: (_, _) => tcs.TrySetResult());

            try { await tcs.Task.WaitAsync(token); }
            catch (OperationCanceledException) { return; }
        }
    }
}
