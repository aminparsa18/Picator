namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>
/// Type of padding applied to the logo (padding is applied even with no logo drawable set).
/// Native port of QrVectorLogoPadding.kt.
/// </summary>
public abstract record QrVectorLogoPadding
{
    public abstract float Value { get; }

    /// <summary>Logo is drawn on top of the QR code without any padding.</summary>
    public sealed record EmptyPadding : QrVectorLogoPadding
    {
        public override float Value => 0f;
    }

    /// <summary>Padding is applied according to the logo's shape; some QR pixels may be cut.</summary>
    public sealed record Accurate(float Value) : QrVectorLogoPadding
    {
        public override float Value { get; } = Value;
    }

    /// <summary>Like <see cref="Accurate"/>, but clipped pixels are fully removed rather than left dangling.</summary>
    public sealed record Natural(float Value) : QrVectorLogoPadding
    {
        public override float Value { get; } = Value;
    }

    public static readonly QrVectorLogoPadding Empty = new EmptyPadding();
}
