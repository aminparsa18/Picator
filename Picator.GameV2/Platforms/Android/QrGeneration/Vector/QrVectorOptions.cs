using Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

namespace Picator.GameV2.Platforms.Android.QrGeneration.Vector;

/// <summary>
/// Full set of options controlling how <see cref="QrCodeDrawable"/> renders a
/// <see cref="IQrData"/>. Native port of vector/QrVectorOptions.kt.
/// </summary>
public sealed record QrVectorOptions(
    /// <summary>0..0.5 fraction of the smaller bound reserved as blank padding around the code.</summary>
    float Padding = 0f,
    QrOffset Offset = default,
    QrVectorShapes? Shapes = null,
    /// <summary>Overall macro shape of the code (square/circle mask). Defaults to <see cref="QrShapeDefault"/>.</summary>
    IQrShape? CodeShape = null,
    QrVectorColors? Colors = null,
    QrVectorLogo? Logo = null,
    QrVectorBackground? Background = null,
    QrErrorCorrectionLevel ErrorCorrectionLevel = QrErrorCorrectionLevel.Auto,
    /// <summary>Enable the bottom-right eye. Can overwrite an alignment eye and make the code harder to scan.</summary>
    bool FourthEyeEnabled = false,
    QrHighlighting? Highlighting = null)
{
    public QrVectorShapes ShapesOrDefault => Shapes ?? new QrVectorShapes();
    public IQrShape CodeShapeOrDefault => CodeShape ?? QrShapeDefault.Instance;
    public QrVectorColors ColorsOrDefault => Colors ?? QrVectorColors.Defaults();
    public QrVectorLogo LogoOrDefault => Logo ?? new QrVectorLogo();
    public QrVectorBackground BackgroundOrDefault => Background ?? new QrVectorBackground();
    public QrHighlighting HighlightingOrDefault => Highlighting ?? new QrHighlighting();
}
