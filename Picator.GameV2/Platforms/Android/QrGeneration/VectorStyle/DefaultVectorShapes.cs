using Android.Graphics;
using Path = Android.Graphics.Path;
using RectF = Android.Graphics.RectF;

namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>
/// Shared path-building geometry reused by the pixel/ball/frame/logo shapes. Native port of
/// customqrgenerator's vector/style/DefaultVectorShapes.kt (trimmed to the shapes actually used
/// by the app: plain square, circle, and neighbor-aware rounded corners).
/// </summary>
internal static class DefaultVectorShapes
{
    public static Path Square(float size)
    {
        var path = new Path();
        path.AddRect(0f, 0f, size, size, Path.Direction.Cw);
        return path;
    }

    public static Path Circle(float size, float sizeFraction)
    {
        var path = new Path();
        var clamped = Math.Clamp(sizeFraction, 0f, 1f);
        path.AddCircle(size / 2f, size / 2f, size / 2f * clamped, Path.Direction.Cw);
        return path;
    }

    /// <summary>
    /// Rounded rectangle whose corners are only rounded where allowed by
    /// <paramref name="topLeft"/>/etc. and (if <paramref name="withNeighbors"/>) where there's no
    /// same-type neighbor pulling that corner flush against an adjacent pixel.
    /// </summary>
    public static Path RoundCorners(
        float size,
        Neighbors neighbors,
        float cornerRadius,
        bool withNeighbors,
        bool topLeft = true,
        bool bottomLeft = true,
        bool topRight = true,
        bool bottomRight = true)
    {
        var corner = Math.Clamp(cornerRadius, 0f, .5f) * size;

        bool Rounded(bool allowed, bool n1, bool n2) =>
            allowed && (!withNeighbors || (!n1 && !n2));

        var tl = Rounded(topLeft, neighbors.Top, neighbors.Left) ? corner : 0f;
        var tr = Rounded(topRight, neighbors.Top, neighbors.Right) ? corner : 0f;
        var br = Rounded(bottomRight, neighbors.Bottom, neighbors.Right) ? corner : 0f;
        var bl = Rounded(bottomLeft, neighbors.Bottom, neighbors.Left) ? corner : 0f;

        var path = new Path();
        path.AddRoundRect(
            new RectF(0f, 0f, size, size),
            new[] { tl, tl, tr, tr, br, br, bl, bl },
            Path.Direction.Cw);
        return path;
    }
}
