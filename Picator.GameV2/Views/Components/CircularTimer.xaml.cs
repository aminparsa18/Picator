using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Diagnostics;
using Color = Microsoft.Maui.Graphics.Color;

namespace Picator.GameV2.Views.Components;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CircularTimer : ContentView
{
    public static readonly BindableProperty DurationProperty = BindableProperty.Create(
        nameof(Duration),
        typeof(int),
        typeof(CircularTimer),
        60,
        propertyChanged: OnDurationPropertyChanged);

    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(CircularTimer),
        Colors.Blue,
        propertyChanged: OnColorPropertyChanged);

    public static readonly BindableProperty StrokeProperty = BindableProperty.Create(
        nameof(Stroke),
        typeof(double),
        typeof(CircularTimer),
        9d,
        propertyChanged: OnStrokePropertyChanged);

    public static readonly BindableProperty IsRunningProperty = BindableProperty.Create(
        nameof(IsRunning),
        typeof(bool),
        typeof(CircularTimer),
        false,
        propertyChanged: OnIsRunningPropertyChanged);

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor),
        typeof(Color),
        typeof(CircularTimer),
        Colors.Black,
        propertyChanged: OnTextColorPropertyChanged);

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
        nameof(FontSize),
        typeof(float),
        typeof(CircularTimer),
        48f,
        propertyChanged: OnFontSizePropertyChanged);

    // Event fired when timer completes
    public event EventHandler? TimerCompleted;

    public int Duration
    {
        get => (int)GetValue(DurationProperty);
        set => SetValue(DurationProperty, Math.Max(0, value));
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public double Stroke
    {
        get => (double)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public bool IsRunning
    {
        get => (bool)GetValue(IsRunningProperty);
        set => SetValue(IsRunningProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public float FontSize
    {
        get => (float)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    private static void OnDurationPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularTimer)bindable;
        view.Reset();
    }

    private static void OnColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularTimer)bindable;
        view.UpdatePaint();
        view.canvas.InvalidateSurface();
    }

    private static void OnStrokePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularTimer)bindable;
        view.UpdatePaint();
        view.canvas.InvalidateSurface();
    }

    private static void OnIsRunningPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularTimer)bindable;
        if ((bool)newValue)
        {
            view.Start();
        }
        else
        {
            view.Stop();
        }
    }

    private static void OnTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularTimer)bindable;
        view.UpdateTextPaint();
        view.canvas.InvalidateSurface();
    }

    private static void OnFontSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularTimer)bindable;
        view.UpdateTextPaint();
        view.canvas.InvalidateSurface();
    }

    public CircularTimer()
    {
        UpdatePaint();
        UpdateTextPaint();
        InitializeComponent();
        canvas.PaintSurface += Canvas_PaintSurface;
    }

    protected override void InvalidateLayout()
    {
        base.InvalidateLayout();
        canvas.InvalidateSurface();
    }

    private void UpdatePaint()
    {
        paint = new SKPaint()
        {
            Style = SKPaintStyle.Stroke,
            Color = Color.ToSKColor(),
            StrokeCap = SKStrokeCap.Round,
            StrokeJoin = SKStrokeJoin.Round,
            StrokeWidth = (float)XamDIUConvertToPixels(Stroke),
            IsAntialias = true,
        };

        paintBackground = paint.Clone();
        paintBackground.StrokeWidth *= 0.9f;
        paintBackground.Color = new SKColor(paintBackground.Color.Red, paintBackground.Color.Green, paintBackground.Color.Blue, (byte)50);
    }

    private void UpdateTextPaint()
    {
        textPaint = new SKPaint()
        {
            Style = SKPaintStyle.Fill,
            Color = TextColor.ToSKColor(),
            IsAntialias = true,
        };

        textFont = new SKFont(
            SKTypeface.FromFamilyName("Arial", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
            (float)FontSize);
    }

    private SKRect rect;
    private SKPaint paint;
    private SKPaint paintBackground;
    private SKPaint textPaint;
    private SKFont textFont;
    private const int padding = 2;
    private readonly Stopwatch stopwatch = new();
    private TimeSpan remainingTime;
    private bool hasCompleted;

    public void Start()
    {
        if (!stopwatch.IsRunning)
        {
            remainingTime = TimeSpan.FromSeconds(Duration);
            hasCompleted = false;
            stopwatch.Restart();
            canvas.InvalidateSurface();
        }
    }

    public void Stop()
    {
        stopwatch.Stop();
    }

    public void Reset()
    {
        Stop();
        remainingTime = TimeSpan.FromSeconds(Duration);
        hasCompleted = false;
        canvas.InvalidateSurface();
    }

    private void Canvas_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        canvas.Clear();

        // Calculate remaining time
        if (IsRunning && stopwatch.IsRunning)
        {
            var elapsed = stopwatch.Elapsed;
            remainingTime = TimeSpan.FromSeconds(Duration) - elapsed;

            if (remainingTime.TotalSeconds <= 0)
            {
                remainingTime = TimeSpan.Zero;
                if (!hasCompleted)
                {
                    hasCompleted = true;
                    Stop();
                    TimerCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        // Setup drawing area
        rect.Size = new SKSize(e.Info.Width - paint.StrokeWidth - padding, e.Info.Height - paint.StrokeWidth - padding);
        rect.Location = new SKPoint(paint.StrokeWidth / 2, paint.StrokeWidth / 2);

        // Calculate progress (remaining percentage)
        var progressPercentage = Math.Max(0, Math.Min(100, (remainingTime.TotalSeconds / Duration) * 100));
        var angle = progressPercentage / 100d * 360d;

        // Draw background circle
        var pathBackground = new SKPath();
        pathBackground.AddArc(rect, 0, 360);
        canvas.DrawPath(pathBackground, paintBackground);

        // Draw progress arc
        var path = new SKPath();
        path.AddArc(rect, 270, (float)angle); // Start from top
        canvas.DrawPath(path, paint);

        // Draw time text in center
        var seconds = (int)Math.Ceiling(remainingTime.TotalSeconds);
        var timeText = seconds.ToString();

        // Calculate center position
        var centerX = e.Info.Width / 2f;
        var centerY = e.Info.Height / 2f;

        // Adjust for text baseline
        var textBounds = new SKRect();

        textFont.MeasureText(timeText, out textBounds, textPaint);
        var textY = centerY - textBounds.MidY;

        canvas.DrawText(timeText, centerX, textY, SKTextAlign.Center, textFont, textPaint);

        //// Continue animation if running
        //if (IsRunning && remainingTime.TotalSeconds > 0)
        //{
        //    Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(16), () => // ~60fps
        //    {
        //        canvas.InvalidateSurface();
        //    });
        //}
    }

    private static double XamDIUConvertToPixels(double XamDIU)
    {
        var k = DeviceDisplay.MainDisplayInfo.Density / 2d;
        var pixels = k * XamDIU;
        return pixels;
    }
}