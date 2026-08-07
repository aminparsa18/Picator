using Android.Graphics;
using Path = Android.Graphics.Path;

namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>Style of the QR-code eye's internal "ball". Native port of QrVectorBallShape.kt (trimmed set).</summary>
public interface IQrVectorBallShape : IQrVectorShapeModifier
{
}

public sealed class QrBallShapeDefault : IQrVectorBallShape
{
    public static readonly QrBallShapeDefault Instance = new();

    public Path CreatePath(float size, Neighbors neighbors) => DefaultVectorShapes.Square(size);
}

/// <summary>Marker: draw the ball using <c>shapes.darkPixel</c> instead of a dedicated ball shape.</summary>
public sealed class QrBallShapeAsDarkPixels : IQrVectorBallShape
{
    public static readonly QrBallShapeAsDarkPixels Instance = new();

    public Path CreatePath(float size, Neighbors neighbors) => new();
}

public sealed class QrBallShapeAsPixelShape : IQrVectorBallShape
{
    private readonly IQrVectorPixelShape _pixelShape;

    public QrBallShapeAsPixelShape(IQrVectorPixelShape pixelShape) => _pixelShape = pixelShape;

    public Path CreatePath(float size, Neighbors neighbors)
    {
        var path = new Path();
        var matrix = new QrCodeMatrix(3);
        for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
                matrix[i, j] = QrCodeMatrix.PixelType.DarkPixel;

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                path.AddPath(
                    _pixelShape.CreatePath(size / 3f, matrix.Neighbors(i, j)),
                    size / 3f * i, size / 3f * j);
            }
        }
        return path;
    }
}

public sealed class QrBallShapeCircle : IQrVectorBallShape
{
    private readonly float _size;

    public QrBallShapeCircle(float size = 1f) => _size = size;

    public Path CreatePath(float size, Neighbors neighbors) => DefaultVectorShapes.Circle(size, _size);
}

public sealed class QrBallShapeRoundCorners : IQrVectorBallShape
{
    private readonly float _radius;
    private readonly bool _topLeft, _bottomLeft, _topRight, _bottomRight;

    public QrBallShapeRoundCorners(
        float radius, bool topLeft = true, bool bottomLeft = true, bool topRight = true, bool bottomRight = true)
    {
        _radius = radius;
        _topLeft = topLeft;
        _bottomLeft = bottomLeft;
        _topRight = topRight;
        _bottomRight = bottomRight;
    }

    public Path CreatePath(float size, Neighbors neighbors) =>
        DefaultVectorShapes.RoundCorners(size, Neighbors.Empty, _radius, withNeighbors: false,
            _topLeft, _bottomLeft, _topRight, _bottomRight);
}
