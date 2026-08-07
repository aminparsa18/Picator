using Android.Graphics;

namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>How a logo/background bitmap is fit into its target size. Native port of style/BitmapScale.kt.</summary>
public enum BitmapScale
{
    FitXY,
    CenterCrop,
}

public static class BitmapScaleExtensions
{
    public static Bitmap Scale(this BitmapScale scale, Bitmap source, int width, int height)
    {
        if (width <= 0 || height <= 0)
            return source;

        if (scale == BitmapScale.FitXY || source.Width == 0 || source.Height == 0)
            return Bitmap.CreateScaledBitmap(source, width, height, filter: true)!;

        // CenterCrop: scale to cover, then crop the centered excess.
        var scaleFactor = Math.Max((float)width / source.Width, (float)height / source.Height);
        var scaledW = (int)Math.Round(source.Width * scaleFactor);
        var scaledH = (int)Math.Round(source.Height * scaleFactor);
        var scaled = Bitmap.CreateScaledBitmap(source, scaledW, scaledH, filter: true)!;

        var x = Math.Max(0, (scaledW - width) / 2);
        var y = Math.Max(0, (scaledH - height) / 2);
        return Bitmap.CreateBitmap(scaled, x, y, Math.Min(width, scaledW), Math.Min(height, scaledH))!;
    }
}
