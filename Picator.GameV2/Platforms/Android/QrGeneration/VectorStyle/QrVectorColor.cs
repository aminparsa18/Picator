using Android.Graphics;
using AndroidColor = Android.Graphics.Color;
using AndroidPaint = Android.Graphics.Paint;

namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>
/// Paint (solid / gradient / special) used for a QR code element. Native port of
/// vector/style/QrVectorColor.kt.
/// </summary>
public abstract record QrVectorColor
{
    public abstract AndroidPaint CreatePaint(float width, float height);

    public sealed record Transparent : QrVectorColor
    {
        public override AndroidPaint CreatePaint(float width, float height)
        {
            var paint = new AndroidPaint { Color = new AndroidColor(0) };
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Dst));
            return paint;
        }
    }

    /// <summary>Cuts out the QR code part of the resulting drawable, making it transparent regardless of background.</summary>
    public sealed record Eraser : QrVectorColor
    {
        public override AndroidPaint CreatePaint(float width, float height)
        {
            var paint = new AndroidPaint { Alpha = 0 };
            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Src));
            return paint;
        }
    }

    public sealed record Unspecified : QrVectorColor
    {
        public override AndroidPaint CreatePaint(float width, float height) => new Transparent().CreatePaint(width, height);
    }

    public sealed record Solid(int Color) : QrVectorColor
    {
        public override AndroidPaint CreatePaint(float width, float height) => new() { Color = new AndroidColor(Color) };
    }

    public enum GradientOrientation { Vertical, Horizontal, LeftDiagonal, RightDiagonal }

    public sealed record LinearGradient(IReadOnlyList<(float Position, int Color)> Colors, GradientOrientation Orientation)
        : QrVectorColor
    {
        public override AndroidPaint CreatePaint(float width, float height)
        {
            var (x0, y0) = Start(width, height);
            var (x1, y1) = End(width, height);

            var shader = new global::Android.Graphics.LinearGradient(
                x0, y0, x1, y1,
                Colors.Select(c => (int)new AndroidColor(c.Color)).ToArray(),
                Colors.Select(c => c.Position).ToArray(),
                Shader.TileMode.Clamp!);

            var paint = new AndroidPaint();
            paint.SetShader(shader);
            return paint;
        }

        private (float, float) Start(float w, float h) => Orientation switch
        {
            GradientOrientation.Vertical => (w / 2, 0f),
            GradientOrientation.Horizontal => (0f, h / 2),
            GradientOrientation.LeftDiagonal => (0f, 0f),
            GradientOrientation.RightDiagonal => (0f, h),
            _ => (0f, 0f),
        };

        private (float, float) End(float w, float h) => Orientation switch
        {
            GradientOrientation.Vertical => (w / 2, h),
            GradientOrientation.Horizontal => (w, h / 2),
            GradientOrientation.LeftDiagonal => (w, h),
            GradientOrientation.RightDiagonal => (w, 0f),
            _ => (w, h),
        };
    }

    public sealed record RadialGradient(IReadOnlyList<(float Position, int Color)> Colors, float Radius = 1.4142135f)
        : QrVectorColor
    {
        public override AndroidPaint CreatePaint(float width, float height)
        {
            var shader = new global::Android.Graphics.RadialGradient(
                width / 2, height / 2,
                Math.Max(width, height) / 2 * Math.Max(Radius, 0f),
                Colors.Select(c => (int)new AndroidColor(c.Color)).ToArray(),
                Colors.Select(c => c.Position).ToArray(),
                Shader.TileMode.Clamp!);

            var paint = new AndroidPaint();
            paint.SetShader(shader);
            return paint;
        }
    }

    public sealed record SweepGradient(IReadOnlyList<(float Position, int Color)> Colors) : QrVectorColor
    {
        public override AndroidPaint CreatePaint(float width, float height)
        {
            var shader = new global::Android.Graphics.SweepGradient(
                width / 2, height / 2,
                Colors.Select(c => (int)new AndroidColor(c.Color)).ToArray(),
                Colors.Select(c => c.Position).ToArray());

            var paint = new AndroidPaint();
            paint.SetShader(shader);
            return paint;
        }
    }
}

public static class QrVectorColorExtensions
{
    public static bool IsTransparent(this QrVectorColor color) => color switch
    {
        QrVectorColor.Transparent => true,
        QrVectorColor.Unspecified => true,
        QrVectorColor.Solid solid => new AndroidColor(solid.Color).A == 0,
        _ => false,
    };
}
