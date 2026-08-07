namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>Colors of the QR code elements. Native port of vector/style/QrVectorColors.kt.</summary>
public sealed record QrVectorColors(
    QrVectorColor Dark,
    QrVectorColor? Light = null,
    QrVectorColor? Ball = null,
    QrVectorColor? Frame = null)
{
    public static QrVectorColors Defaults() => new(
        Dark: new QrVectorColor.Solid(unchecked((int)0xFF000000)),
        Light: new QrVectorColor.Unspecified(),
        Ball: new QrVectorColor.Unspecified(),
        Frame: new QrVectorColor.Unspecified());

    public QrVectorColor LightOrUnspecified => Light ?? new QrVectorColor.Unspecified();
    public QrVectorColor BallOrUnspecified => Ball ?? new QrVectorColor.Unspecified();
    public QrVectorColor FrameOrUnspecified => Frame ?? new QrVectorColor.Unspecified();
}
