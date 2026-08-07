using Android.Graphics;

namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>
/// Background of the QR code. Native port of vector/style/QrVectorBackground.kt — simplified to
/// accept a <see cref="Bitmap"/> directly rather than upstream's generic <c>Drawable</c>, since
/// nothing in the app currently supplies a background image.
/// </summary>
public sealed record QrVectorBackground(
    Bitmap? Bitmap = null,
    BitmapScale Scale = BitmapScale.FitXY,
    QrVectorColor? Color = null)
{
    public QrVectorColor ColorOrTransparent => Color ?? new QrVectorColor.Transparent();
}
