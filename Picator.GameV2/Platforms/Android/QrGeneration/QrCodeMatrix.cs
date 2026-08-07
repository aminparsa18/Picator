namespace Picator.GameV2.Platforms.Android.QrGeneration;

/// <summary>
/// Native C# port of customqrgenerator's encoder/QrCodeMatrix.kt: a square matrix of
/// pixel classifications built from the raw ZXing QR byte matrix, then reshaped/annotated
/// by the code shape and vector drawable before rendering.
/// </summary>
public sealed class QrCodeMatrix
{
    public enum PixelType
    {
        DarkPixel,
        LightPixel,
        Background,
        Logo,
        VersionEye,
    }

    public int Size { get; }

    private PixelType[] _types;

    public QrCodeMatrix(int size)
    {
        Size = size;
        _types = new PixelType[size * size];
        Array.Fill(_types, PixelType.Background);
    }

    public PixelType this[int i, int j]
    {
        get
        {
            CheckBounds(i, j);
            return _types[i + j * Size];
        }
        set
        {
            CheckBounds(i, j);
            _types[i + j * Size] = value;
        }
    }

    public bool TryGet(int i, int j, out PixelType type)
    {
        if (i < 0 || i >= Size || j < 0 || j >= Size)
        {
            type = default;
            return false;
        }
        type = _types[i + j * Size];
        return true;
    }

    public QrCodeMatrix Copy()
    {
        var copy = new QrCodeMatrix(Size)
        {
            _types = (PixelType[])_types.Clone()
        };
        return copy;
    }

    private void CheckBounds(int i, int j)
    {
        if (i < 0 || i >= Size)
            throw new IndexOutOfRangeException($"Index {i} is out of 0..{Size - 1} matrix bound");
        if (j < 0 || j >= Size)
            throw new IndexOutOfRangeException($"Index {j} is out of 0..{Size - 1} matrix bound");
    }

    /// <summary>Port of QrCodeMatrix.kt's `neighbors` extension: same-type comparison with the pixel itself.</summary>
    public Neighbors Neighbors(int i, int j)
    {
        bool Cmp(int i2, int j2) => TryGet(i2, j2, out var t) && t == this[i, j];

        return new Neighbors(
            TopLeft: Cmp(i - 1, j - 1),
            TopRight: Cmp(i + 1, j - 1),
            Left: Cmp(i - 1, j),
            Top: Cmp(i, j - 1),
            Right: Cmp(i + 1, j),
            BottomLeft: Cmp(i - 1, j + 1),
            Bottom: Cmp(i, j + 1),
            BottomRight: Cmp(i + 1, j + 1));
    }

    // Note: upstream's `neighborsReversed` extension is only used by the raster QrEncoder.kt
    // rendering path, which this port intentionally skips (the app only ever used the vector
    // QrCodeDrawable path) — see the plan's "Scope kept vs. dropped from upstream" section.

    /// <summary>Port of ByteMatrix.toQrMatrix(): builds a QrCodeMatrix from a raw 0/1 ZXing byte matrix.</summary>
    public static QrCodeMatrix FromByteMatrix(ZXing.QrCode.Internal.ByteMatrix byteMatrix)
    {
        if (byteMatrix.Width != byteMatrix.Height)
            throw new InvalidOperationException("Non-square qr byte matrix");

        var width = byteMatrix.Width;
        var matrix = new QrCodeMatrix(width);
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < width; j++)
            {
                matrix[i, j] = byteMatrix[i, j] == 1
                    ? PixelType.DarkPixel
                    : PixelType.LightPixel;
            }
        }
        return matrix;
    }
}
