using Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

namespace Picator.GameV2.Platforms.Android.QrGeneration;

/// <summary>
/// Highlighting of the anchor QR code elements (corner eyes, version/alignment eyes, timing
/// lines). Has the most impact when using a background image or color. Native port of
/// customqrgenerator's QrHighlighting.kt.
/// </summary>
public sealed record QrHighlighting(
    HighlightingType? CornerEyes = null,
    HighlightingType? VersionEyes = null,
    HighlightingType? TimingLines = null,
    float Alpha = .75f)
{
    public HighlightingType CornerEyesOrNone => CornerEyes ?? HighlightingType.None;
    public HighlightingType VersionEyesOrNone => VersionEyes ?? HighlightingType.None;
    public HighlightingType TimingLinesOrNone => TimingLines ?? HighlightingType.None;
}

public abstract record HighlightingType
{
    public static readonly HighlightingType None = new NoneType();
    public static readonly HighlightingType Default = new DefaultType();

    public sealed record NoneType : HighlightingType;
    public sealed record DefaultType : HighlightingType;

    public sealed record Styled(IQrVectorShapeModifier? Shape = null, QrVectorColor? Color = null) : HighlightingType;
}
