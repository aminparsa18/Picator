using Android.Graphics;
using Path = Android.Graphics.Path;
using RectF = Android.Graphics.RectF;

namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>Style of the QR-code eye's outer "frame". Native port of QrVectorFrameShape.kt (trimmed set).</summary>
public interface IQrVectorFrameShape : IQrVectorShapeModifier
{
}

/// <summary>Plain square ring frame, one-seventh of the cell wide (matches a standard finder pattern).</summary>
public sealed class QrFrameShapeDefault : IQrVectorFrameShape
{
    public static readonly QrFrameShapeDefault Instance = new();

    public Path CreatePath(float size, Neighbors neighbors)
    {
        var path = new Path();
        var width = size / 7f;
        path.AddRect(0f, 0f, size, width, Path.Direction.Cw);
        path.AddRect(0f, 0f, width, size, Path.Direction.Cw);
        path.AddRect(size - width, 0f, size, size, Path.Direction.Cw);
        path.AddRect(0f, size - width, size, size, Path.Direction.Cw);
        return path;
    }
}

/// <summary>Marker: draw the frame using <c>shapes.darkPixel</c> instead of a dedicated frame shape.</summary>
public sealed class QrFrameShapeAsDarkPixels : IQrVectorFrameShape
{
    public static readonly QrFrameShapeAsDarkPixels Instance = new();

    public Path CreatePath(float size, Neighbors neighbors) => new();
}

public sealed class QrFrameShapeAsPixelShape : IQrVectorFrameShape
{
    private readonly IQrVectorPixelShape _pixelShape;

    public QrFrameShapeAsPixelShape(IQrVectorPixelShape pixelShape) => _pixelShape = pixelShape;

    public Path CreatePath(float size, Neighbors neighbors)
    {
        var matrix = new QrCodeMatrix(7);
        for (var i = 0; i < 7; i++)
            for (var j = 0; j < 7; j++)
                matrix[i, j] = i == 0 || j == 0 || i == 6 || j == 6
                    ? QrCodeMatrix.PixelType.DarkPixel
                    : QrCodeMatrix.PixelType.Background;

        var path = new Path();
        for (var i = 0; i < 7; i++)
        {
            for (var j = 0; j < 7; j++)
            {
                if (matrix[i, j] == QrCodeMatrix.PixelType.DarkPixel)
                {
                    path.AddPath(
                        _pixelShape.CreatePath(size / 7f, matrix.Neighbors(i, j)),
                        size / 7f * i, size / 7f * j);
                }
            }
        }
        return path;
    }
}

/// <summary>Concentric-ring frame.</summary>
public sealed class QrFrameShapeCircle : IQrVectorFrameShape
{
    private readonly float _width;
    private readonly float _radius;

    public QrFrameShapeCircle(float width = 1f, float radius = 1f)
    {
        _width = width;
        _radius = radius;
    }

    public Path CreatePath(float size, Neighbors neighbors)
    {
        var path = new Path();
        var width = size / 7f * _width;
        var radius = Math.Max(_radius, 0f);
        path.AddCircle(size / 2f, size / 2f, size / 2f * radius, Path.Direction.Cw);
        path.AddCircle(size / 2f, size / 2f, (size / 2f - width) * radius, Path.Direction.Ccw);
        return path;
    }
}

/// <summary>Rounded-rectangle ring frame (outer rounded rect minus an inset, independently-rounded inner rect).</summary>
public sealed class QrFrameShapeRoundCorners : IQrVectorFrameShape
{
    private readonly float _corner;
    private readonly float _width;
    private readonly bool _topLeft, _bottomLeft, _topRight, _bottomRight;

    public QrFrameShapeRoundCorners(
        float corner, float width = 1f,
        bool topLeft = true, bool bottomLeft = true, bool topRight = true, bool bottomRight = true)
    {
        _corner = corner;
        _width = width;
        _topLeft = topLeft;
        _bottomLeft = bottomLeft;
        _topRight = topRight;
        _bottomRight = bottomRight;
    }

    public Path CreatePath(float size, Neighbors neighbors)
    {
        var width = size / 7f * Math.Max(_width, 0f);
        var outerCorner = _corner * size;
        var innerCorner = _corner * (size - 4 * width);

        var outer = new Path();
        outer.AddRoundRect(
            new RectF(0f, 0f, size, size),
            CornerRadii(outerCorner),
            Path.Direction.Cw);

        var inner = new Path();
        inner.AddRoundRect(
            new RectF(width, width, size - width, size - width),
            CornerRadii(innerCorner),
            Path.Direction.Ccw);

        outer.InvokeOp(inner, Path.Op.Difference);
        return outer;
    }

    private float[] CornerRadii(float corner) => new[]
    {
        _topLeft ? corner : 0f, _topLeft ? corner : 0f,
        _topRight ? corner : 0f, _topRight ? corner : 0f,
        _bottomRight ? corner : 0f, _bottomRight ? corner : 0f,
        _bottomLeft ? corner : 0f, _bottomLeft ? corner : 0f,
    };
}
