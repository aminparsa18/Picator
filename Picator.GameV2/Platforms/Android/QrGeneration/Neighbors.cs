namespace Picator.GameV2.Platforms.Android.QrGeneration;

/// <summary>
/// Status of the 8 neighboring QR-code pixels around a given cell.
/// Native C# port of customqrgenerator's style/Neighbors.kt.
/// </summary>
public readonly record struct Neighbors(
    bool TopLeft = false,
    bool TopRight = false,
    bool Left = false,
    bool Top = false,
    bool Right = false,
    bool BottomLeft = false,
    bool Bottom = false,
    bool BottomRight = false)
{
    public static readonly Neighbors Empty = new();

    public bool HasAny =>
        TopLeft || TopRight || Left || Top || Right || BottomLeft || Bottom || BottomRight;

    public bool HasAllNearest => Top && Bottom && Left && Right;

    public bool HasAll =>
        TopLeft && TopRight && Left && Top && Right && BottomLeft && Bottom && BottomRight;
}
