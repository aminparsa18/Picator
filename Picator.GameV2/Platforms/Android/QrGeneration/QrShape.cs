namespace Picator.GameV2.Platforms.Android.QrGeneration;

/// <summary>
/// Shape of the overall QR-code pattern (the "code shape" / macro mask), as opposed to the
/// per-pixel vector shapes in <c>VectorStyle</c>. Native port of customqrgenerator's
/// style/QrShape.kt.
/// </summary>
public interface IQrShape
{
    /// <summary>Growth factor applied to the matrix side length by <see cref="Apply"/>.</summary>
    float ShapeSizeIncrease { get; }

    /// <summary>Transform (or replace with a BIGGER) matrix. Shrinking the matrix is not supported.</summary>
    QrCodeMatrix Apply(QrCodeMatrix matrix);

    /// <summary>Whether pixel (i, j) of the (already <see cref="Apply"/>-transformed) matrix is inside the shape.</summary>
    bool PixelInShape(int i, int j, QrCodeMatrix modifiedMatrix);
}

/// <summary>Plain square code, unchanged.</summary>
public sealed class QrShapeDefault : IQrShape
{
    public static readonly QrShapeDefault Instance = new();

    public float ShapeSizeIncrease => 1f;

    public QrCodeMatrix Apply(QrCodeMatrix matrix) => matrix;

    public bool PixelInShape(int i, int j, QrCodeMatrix modifiedMatrix) => true;
}

/// <summary>
/// Circular macro-mask. Grows the matrix and fills the added ring with random dark/light
/// pixels (like upstream) so the circle reaches beyond the original square before masking
/// pixels outside the circle radius away in <see cref="PixelInShape"/>.
/// </summary>
public sealed class QrShapeCircle : IQrShape
{
    private readonly float _padding;
    private readonly long _seed;

    /// <param name="padding">1.0..2.0 — how far the circle extends past the original square.</param>
    /// <param name="seed">Seed for the deterministic random padding pixels.</param>
    public QrShapeCircle(float padding = 1.1f, long seed = 233)
    {
        _padding = padding;
        _seed = seed;
    }

    public float ShapeSizeIncrease => 1 + (float)(_padding * Math.Sqrt(2.0) - 1);

    public QrCodeMatrix Apply(QrCodeMatrix matrix)
    {
        var padding = Math.Clamp(_padding, 1f, 2f);
        var size = matrix.Size;
        var added = (int)Math.Round((size * padding * Math.Sqrt(2.0) - size) / 2);

        var newSize = size + 2 * added;
        var newMatrix = new QrCodeMatrix(newSize);

        var center = newSize / 2f;
        var random = new Random((int)_seed);

        for (var i = 0; i < newSize; i++)
        {
            for (var j = 0; j < newSize; j++)
            {
                var onAddedRing = i <= added - 1 || j <= added - 1 || i >= added + size || j >= added + size;
                if (onAddedRing && Math.Sqrt((center - i) * (center - i) + (center - j) * (center - j)) <= center)
                {
                    newMatrix[i, j] = random.Next(2) == 1
                        ? QrCodeMatrix.PixelType.DarkPixel
                        : QrCodeMatrix.PixelType.LightPixel;
                }
            }
        }

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                newMatrix[added + i, added + j] = matrix[i, j];
            }
        }

        return newMatrix;
    }

    public bool PixelInShape(int i, int j, QrCodeMatrix modifiedMatrix)
    {
        var center = modifiedMatrix.Size / 2f;
        return Math.Sqrt((center - i) * (center - i) + (center - j) * (center - j)) <= center;
    }
}
