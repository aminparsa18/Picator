using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;

namespace Picator.Game.Controls;

public class WaitingProgressBar : SKCanvasView
{
    public static BindableProperty PercentageProperty = BindableProperty.Create(nameof(Percentage), typeof(float),
        typeof(WaitingProgressBar), 0f, BindingMode.OneWay,
        (_, value) => value != null,
        OnPropertyChangedInvalidate);

    public float Percentage
    {
        get => (float)GetValue(PercentageProperty);
        set => SetValue(PercentageProperty, value);
    }

    public static BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(Percentage), typeof(float),
        typeof(WaitingProgressBar), 5f, BindingMode.OneWay,
        (_, value) => value != null && (float)value >= 0,
        OnPropertyChangedInvalidate);

    public float CornerRadius
    {
        get => (float)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public static BindableProperty BarBackgroundColorProperty = BindableProperty.Create(nameof(BarBackgroundColor), typeof(Color),
        typeof(WaitingProgressBar), Color.White, BindingMode.OneWay,
        (_, value) => value != null, OnPropertyChangedInvalidate);

    public Color BarBackgroundColor
    {
        get => (Color)GetValue(BarBackgroundColorProperty);
        set => SetValue(BarBackgroundColorProperty, value);
    }


    public static BindableProperty GradientStartColorProperty = BindableProperty.Create(nameof(GradientStartColor), typeof(Color),
        typeof(WaitingProgressBar), Color.Purple, BindingMode.OneWay,
        (_, value) => value != null, OnPropertyChangedInvalidate);

    public Color GradientStartColor
    {
        get => (Color)GetValue(GradientStartColorProperty);
        set => SetValue(GradientStartColorProperty, value);
    }

    public static BindableProperty GradientEndColorProperty = BindableProperty.Create(nameof(GradientEndColor), typeof(Color),
        typeof(WaitingProgressBar), Color.Blue, BindingMode.OneWay,
        (_, value) => value != null, OnPropertyChangedInvalidate);

    public Color GradientEndColor
    {
        get => (Color)GetValue(GradientEndColorProperty);
        set => SetValue(GradientEndColorProperty, value);
    }

    private static void OnPropertyChangedInvalidate(BindableObject bindable, object oldvalue, object newvalue)
    {
        var control = (WaitingProgressBar)bindable;

        if (oldvalue != newvalue)
            control.InvalidateSurface();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        var info = e.Info;
        var canvas = e.Surface.Canvas;

        var width = (float)Width;
        var scale = CanvasSize.Width / width;

        var percentage = Percentage;

        var cornerRadius = CornerRadius * scale;


        var height = e.Info.Height;

        var percentageWidth = (int)Math.Floor(info.Width * percentage);

        canvas.Clear();

        var backgroundBar = new SKRoundRect(new SKRect(0, 0, info.Width, height), cornerRadius, cornerRadius);
        var progressBar = new SKRoundRect(new SKRect(0, 0, percentageWidth, height), cornerRadius, cornerRadius);

        var background = new SKPaint { Color = BarBackgroundColor.ToSKColor(), IsAntialias = true };

        canvas.DrawRoundRect(backgroundBar, background);

        using var paint = new SKPaint() { IsAntialias = true };
        float x = percentageWidth;
        float y = info.Height;
        var rect = new SKRect(0, 0, x, y);

        paint.Shader = SKShader.CreateLinearGradient(
            new SKPoint(rect.Left, rect.Top),
            new SKPoint(rect.Right, rect.Top),
            new[]
            {
                GradientStartColor.ToSKColor(),
                GradientEndColor.ToSKColor()
            },
            new float[] { 0, 1 },
            SKShaderTileMode.Clamp);

        canvas.DrawRoundRect(progressBar, paint);
    }
}