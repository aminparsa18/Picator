using ZXingEcc = ZXing.QrCode.Internal.ErrorCorrectionLevel;

namespace Picator.GameV2.Platforms.Android.QrGeneration;

/// <summary>
/// Native port of customqrgenerator's QrErrorCorrectionLevel.kt.
/// QR technology allows reading encoded information even if part of the QR code image is
/// damaged. It also allows having a logo inside the code as a part of "damage".
/// </summary>
public enum QrErrorCorrectionLevel
{
    /// <summary>Minimum possible level will be used.</summary>
    Auto,

    /// <summary>~7% of QR code can be damaged (or used as logo).</summary>
    Low,

    /// <summary>~15% of QR code can be damaged (or used as logo).</summary>
    Medium,

    /// <summary>~25% of QR code can be damaged (or used as logo).</summary>
    MediumHigh,

    /// <summary>~30% of QR code can be damaged (or used as logo).</summary>
    High,
}

public static class QrErrorCorrectionLevelExtensions
{
    public static ZXingEcc ToZXing(this QrErrorCorrectionLevel level) => level switch
    {
        QrErrorCorrectionLevel.Auto => ZXingEcc.L,
        QrErrorCorrectionLevel.Low => ZXingEcc.L,
        QrErrorCorrectionLevel.Medium => ZXingEcc.M,
        QrErrorCorrectionLevel.MediumHigh => ZXingEcc.Q,
        QrErrorCorrectionLevel.High => ZXingEcc.H,
        _ => ZXingEcc.L,
    };

    /// <summary>Port of QrErrorCorrectionLevel.kt's `fit(hasLogo, logoSize)` extension.</summary>
    public static QrErrorCorrectionLevel Fit(this QrErrorCorrectionLevel level, bool hasLogo, float logoSize)
    {
        if (level != QrErrorCorrectionLevel.Auto)
            return level;

        if (!hasLogo)
            return QrErrorCorrectionLevel.Low;

        var lvl = level.ToZXing();

        if (logoSize > .3f)
            return QrErrorCorrectionLevel.High;

        if (logoSize is >= .2f and <= .3f && CompareEcc(lvl, ZXingEcc.Q) < 0)
            return QrErrorCorrectionLevel.MediumHigh;

        if (logoSize > .05f && CompareEcc(lvl, ZXingEcc.M) < 0)
            return QrErrorCorrectionLevel.Medium;

        return level;
    }

    // ZXing.Net's ErrorCorrectionLevel doesn't implement IComparable; upstream relies on the
    // Kotlin enum's declared ordinal order L < M < Q < H, which we replicate here.
    private static int CompareEcc(ZXingEcc a, ZXingEcc b) => Rank(a).CompareTo(Rank(b));

    private static int Rank(ZXingEcc e)
    {
        if (e == ZXingEcc.L) return 0;
        if (e == ZXingEcc.M) return 1;
        if (e == ZXingEcc.Q) return 2;
        return 3; // H
    }
}
