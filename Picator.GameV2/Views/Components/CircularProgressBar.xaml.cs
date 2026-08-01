using Microsoft.Maui.Controls;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Diagnostics;
using System.Runtime.Intrinsics.X86;

namespace Picator.Game.Controls;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class CircularProgressBar : ContentView
{
    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        nameof(Progress),
        typeof(double),
        typeof(CircularProgressBar),
        0d,
        propertyChanged: OnProgressPropertyChanged);

    public static readonly BindableProperty ColorProperty = BindableProperty.Create(
        nameof(Color),
        typeof(Color),
        typeof(CircularProgressBar),
        Colors.Black,
        propertyChanged: OnColorPropertyChanged);

    public static readonly BindableProperty StrokeProperty = BindableProperty.Create(
        nameof(Stroke),
        typeof(double),
        typeof(CircularProgressBar),
        9d,
        propertyChanged: OnStrokePropertyChanged);

    public static readonly BindableProperty SpinProperty = BindableProperty.Create(
        nameof(Spin),
        typeof(bool),
        typeof(CircularProgressBar),
        false,
        propertyChanged: OnAnimatedPropertyChanged);

    public static readonly BindableProperty EasingProperty = BindableProperty.Create(
        nameof(Easing),
        typeof(bool),
        typeof(CircularProgressBar),
        false,
        propertyChanged: OnEasingPropertyChanged);

    public static readonly BindableProperty MaxProgressProperty = BindableProperty.Create(
    nameof(MaxProgress),
    typeof(double),
    typeof(CircularProgressBar),
    100d,
    propertyChanged: OnMaxProgressPropertyChanged);

    public double MaxProgress
    {
        get => (double)GetValue(MaxProgressProperty);
        set => SetValue(MaxProgressProperty, value);
    }

    private static void OnMaxProgressPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularProgressBar)bindable;
        view.canvas.InvalidateSurface();
    }

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set
        {
            currentProgress = (float)this.value;
            SetValue(ProgressProperty, Math.Max(0, Math.Min(MaxProgress, value)));
        }
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

    public bool Spin
    {
        get => (bool)GetValue(SpinProperty);
        set => SetValue(SpinProperty, value);
    }

    public bool Easing
    {
        get => (bool)GetValue(EasingProperty);
        set => SetValue(EasingProperty, value);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="bindable"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private static void OnProgressPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularProgressBar)bindable;
        view._easing = 0;
        view.canvas.InvalidateSurface();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="bindable"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private static void OnColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularProgressBar)bindable;
        view.UpdatePaint();
        view.canvas.InvalidateSurface();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="bindable"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private static void OnStrokePropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularProgressBar)bindable;
        view.UpdatePaint();
        view.canvas.InvalidateSurface();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="bindable"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private static void OnAnimatedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularProgressBar)bindable;
        view.canvas.InvalidateSurface();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bindable"></param>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private static void OnEasingPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var view = (CircularProgressBar)bindable;
        view.canvas.InvalidateSurface();
    }



    public CircularProgressBar()
    {
        UpdatePaint();
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


    private SKRect rect;
    private SKPaint paint;
    private SKPaint paintBackground;
    private float _easing;
    private float _rotate;
    const int padding = 2;
    private readonly Stopwatch time = new();
    private readonly TimeSpan drawInterval = TimeSpan.FromMilliseconds(30);
    private double currentProgress;
    private double value;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Canvas_PaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        time.Stop();

        var a = (float)Math.Max(0.1, Math.Min(10, time.ElapsedMilliseconds / drawInterval.TotalMilliseconds));

        time.Reset();
        time.Start();

        if (Easing || Spin)
        {
            _easing += 0.05f * a;
            _easing = Math.Min(1, _easing);
        }
        else
        {
            _easing = 1;
        }

        if (Spin)
        {
            _rotate += 8f * a;

            if (_rotate > 360)
                _rotate -= 360;
        }
        else
        {
            _rotate = 0;
        }


        if (Progress != value || Spin && Progress is > 0 and < 100 || Easing && _easing is > 0 and < 1)
        {
            Dispatcher.DispatchDelayed(drawInterval, () =>
            {
                this.canvas.InvalidateSurface();
            });
            //Device.StartTimer(drawInterval, () =>
            //{
            //    this.canvas.InvalidateSurface();
            //    return false;
            //});
        }



        var canvas = e.Surface.Canvas;
        canvas.Clear();

        rect.Size = new SKSize(e.Info.Width - paint.StrokeWidth - padding, e.Info.Height - paint.StrokeWidth - padding);
        rect.Location = new SKPoint(paint.StrokeWidth / 2, paint.StrokeWidth / 2);


        var delta = (Progress - currentProgress) * (float)Microsoft.Maui.Easing.CubicInOut.Ease(_easing);
        value = currentProgress + delta;

        var _angle = value / MaxProgress * 360d;

        double _startAngle = _rotate + 270;

        var path = new SKPath();
        path.AddArc(rect, (float)_startAngle, (float)_angle);

        var pathBackground = new SKPath();
        pathBackground.AddArc(rect, 0, 360);

        canvas.DrawPath(path, paint);
        canvas.DrawPath(pathBackground, paintBackground);
    }


    /// <summary>
    /// https://stackoverflow.com/a/63615455/6499748
    /// </summary>
    /// <param name="XamDIU"></param>
    /// <returns></returns>
    private static double XamDIUConvertToPixels(double XamDIU)
    {
        var k = DeviceDisplay.MainDisplayInfo.Density / 2d;
        var pixcels = k * XamDIU;
        return pixcels;
    }
}