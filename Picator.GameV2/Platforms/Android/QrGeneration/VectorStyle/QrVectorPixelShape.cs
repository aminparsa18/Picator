using Android.Graphics;
using Path = Android.Graphics.Path;

namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>Style of the individual QR-code data pixels. Native port of QrVectorPixelShape.kt (trimmed set).</summary>
public interface IQrVectorPixelShape : IQrVectorShapeModifier
{
}

public sealed class QrPixelShapeDefault : IQrVectorPixelShape
{
    public static readonly QrPixelShapeDefault Instance = new();

    public Path CreatePath(float size, Neighbors neighbors) => DefaultVectorShapes.Square(size);
}

public sealed class QrPixelShapeCircle : IQrVectorPixelShape
{
    private readonly float _size;

    /// <param name="size">0..1 fraction of the cell the circle fills.</param>
    public QrPixelShapeCircle(float size = 1f) => _size = size;

    public Path CreatePath(float size, Neighbors neighbors) => DefaultVectorShapes.Circle(size, _size);
}

public sealed class QrPixelShapeRoundCorners : IQrVectorPixelShape
{
    private readonly float _radius;

    /// <param name="radius">0..0.5 corner radius as a fraction of the cell size.</param>
    public QrPixelShapeRoundCorners(float radius) => _radius = radius;

    public Path CreatePath(float size, Neighbors neighbors) =>
        DefaultVectorShapes.RoundCorners(size, neighbors, _radius, withNeighbors: true);
}
