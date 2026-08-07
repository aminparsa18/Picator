using Android.Graphics;
using Path = Android.Graphics.Path;

namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>Shape of the logo cut-out/background in the middle of the code. Native port of QrVectorLogoShape.kt.</summary>
public interface IQrVectorLogoShape : IQrVectorShapeModifier
{
}

public sealed class QrLogoShapeDefault : IQrVectorLogoShape
{
    public static readonly QrLogoShapeDefault Instance = new();

    public Path CreatePath(float size, Neighbors neighbors) => DefaultVectorShapes.Square(size);
}

public sealed class QrLogoShapeCircle : IQrVectorLogoShape
{
    public static readonly QrLogoShapeCircle Instance = new();

    public Path CreatePath(float size, Neighbors neighbors) => DefaultVectorShapes.Circle(size, 1f);
}

public sealed class QrLogoShapeRoundCorners : IQrVectorLogoShape
{
    private readonly float _radius;

    public QrLogoShapeRoundCorners(float radius) => _radius = radius;

    public Path CreatePath(float size, Neighbors neighbors) =>
        DefaultVectorShapes.RoundCorners(size, Neighbors.Empty, _radius, withNeighbors: false);
}
