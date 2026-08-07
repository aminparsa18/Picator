using Android.Graphics;

namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>
/// Logo drawn in the middle of the QR code. Native port of vector/style/QrVectorLogo.kt —
/// simplified to accept a <see cref="Bitmap"/> directly rather than upstream's generic
/// <c>Drawable</c>, since nothing in the app currently supplies a logo image.
/// </summary>
public sealed record QrVectorLogo(
    Bitmap? Bitmap = null,
    float Size = 0.2f,
    QrVectorLogoPadding? Padding = null,
    IQrVectorLogoShape? Shape = null,
    BitmapScale Scale = BitmapScale.FitXY,
    QrVectorColor? BackgroundColor = null)
{
    public QrVectorLogoPadding PaddingOrEmpty => Padding ?? QrVectorLogoPadding.Empty;
    public IQrVectorLogoShape ShapeOrDefault => Shape ?? QrLogoShapeDefault.Instance;
    public QrVectorColor BackgroundColorOrUnspecified => BackgroundColor ?? new QrVectorColor.Unspecified();
}
