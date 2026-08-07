using Android.Graphics;
using Android.Graphics.Drawables;
using Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;
using ZXing.QrCode.Internal;
using AndroidColor = Android.Graphics.Color;
using Paint = Android.Graphics.Paint;
using Path = Android.Graphics.Path;
using Rect = Android.Graphics.Rect;

namespace Picator.GameV2.Platforms.Android.QrGeneration.Vector;

/// <summary>
/// Renders an <see cref="IQrData"/> into a styled QR code bitmap. Native C# port of
/// customqrgenerator's vector/QrCodeDrawable.kt, using ZXing.Net's <see cref="Encoder"/> in
/// place of the Java `com.google.zxing:core` encoder the Kotlin library delegated to.
///
/// NOTE (scope): <see cref="HighlightingType.Styled"/> is treated the same as
/// <see cref="HighlightingType.Default"/> here — the app doesn't use custom highlighting today
/// (default options carry <c>HighlightingType.None</c> throughout), so this trims real Kotlin
/// behavior for an unused corner rather than leaving it unported.
/// </summary>
public sealed class QrCodeDrawable : Drawable
{
    private const int FrameSize = 7;
    private const int BallSize = 3;

    private readonly IQrData _data;
    private readonly QrVectorOptions _options;

    private readonly QrCodeMatrix _initialMatrix;
    private readonly int _shapeIncrease;
    private readonly List<(int X, int Y)> _anchorCenters;
    private readonly QrCodeMatrix _codeMatrix;
    private readonly List<(int X, int Y)> _balls;
    private readonly List<(int X, int Y)> _frames;
    private readonly IQrVectorBallShape _ballShape;
    private readonly IQrVectorFrameShape _frameShape;

    private ColorFilter? _colorFilter;
    private int _alpha = 255;
    private Bitmap? _bitmap;

    public QrCodeDrawable(IQrData data, QrVectorOptions? options = null)
    {
        _data = data;
        _options = options ?? new QrVectorOptions();

        var content = data.Encode();
        if (string.IsNullOrEmpty(content))
            throw new ArgumentException("Found empty contents");

        var ecc = _options.ErrorCorrectionLevel == QrErrorCorrectionLevel.Auto
            ? _options.ErrorCorrectionLevel.Fit(HasLogo(), LogoFootprint()).ToZXing()
            : _options.ErrorCorrectionLevel.ToZXing();

        var code = Encoder.encode(content, ecc);

        _initialMatrix = QrCodeMatrix.FromByteMatrix(code.Matrix!);
        if (_options.FourthEyeEnabled)
        {
            for (var i = _initialMatrix.Size - 8; i < _initialMatrix.Size; i++)
                for (var j = _initialMatrix.Size - 8; j < _initialMatrix.Size; j++)
                    _initialMatrix[i, j] = QrCodeMatrix.PixelType.Background;
        }

        var codeShape = _options.CodeShapeOrDefault;
        _shapeIncrease = ((int)Math.Round(_initialMatrix.Size * codeShape.ShapeSizeIncrease) - _initialMatrix.Size) / 2;

        var centers = code.Version!.AlignmentPatternCenters;
        var max = centers.Length == 0 ? 0 : centers.Max();
        var min = centers.Length == 0 ? 0 : centers.Min();

        var pairs = new HashSet<(int, int)>();
        foreach (var a in centers)
        {
            foreach (var b in centers)
            {
                var isCorner =
                    (a == min && b == min) ||
                    (a == max && b == min) ||
                    (a == min && b == max) ||
                    (_options.FourthEyeEnabled && a == max && b == max);
                if (!isCorner)
                    pairs.Add((a, b));
            }
        }
        _anchorCenters = pairs.ToList();

        if (_options.HighlightingOrDefault.VersionEyesOrNone is HighlightingType.Styled)
        {
            foreach (var (ax, ay) in _anchorCenters)
            {
                for (var x = ax - 2; x < ax + 3; x++)
                    for (var y = ay - 2; y < ay + 3; y++)
                        if (x >= 0 && x < _initialMatrix.Size && y >= 0 && y < _initialMatrix.Size)
                            _initialMatrix[x, y] = QrCodeMatrix.PixelType.VersionEye;
            }
        }

        _codeMatrix = codeShape.Apply(_initialMatrix);

        var size = _codeMatrix.Size;
        _balls = new List<(int, int)>
        {
            (2 + _shapeIncrease, 2 + _shapeIncrease),
            (2 + _shapeIncrease, size - 5 - _shapeIncrease),
            (size - 5 - _shapeIncrease, 2 + _shapeIncrease),
        };
        if (_options.FourthEyeEnabled)
            _balls.Add((size - 5 - _shapeIncrease, size - 5 - _shapeIncrease));

        _frames = new List<(int, int)>
        {
            (_shapeIncrease, _shapeIncrease),
            (_shapeIncrease, size - 7 - _shapeIncrease),
            (size - 7 - _shapeIncrease, _shapeIncrease),
        };
        if (_options.FourthEyeEnabled)
            _frames.Add((size - 7 - _shapeIncrease, size - 7 - _shapeIncrease));

        var shapes = _options.ShapesOrDefault;
        _ballShape = shapes.BallOrDefault is QrBallShapeAsDarkPixels
            ? new QrBallShapeAsPixelShape(shapes.DarkPixelOrDefault)
            : shapes.BallOrDefault;
        _frameShape = shapes.FrameOrDefault is QrFrameShapeAsDarkPixels
            ? new QrFrameShapeAsPixelShape(shapes.DarkPixelOrDefault)
            : shapes.FrameOrDefault;
    }

    private bool HasLogo() => _options.LogoOrDefault.Bitmap != null || _options.LogoOrDefault.PaddingOrEmpty is not QrVectorLogoPadding.EmptyPadding;

    private float LogoFootprint()
    {
        var logo = _options.LogoOrDefault;
        return logo.Size * (1 + logo.PaddingOrEmpty.Value) * _options.CodeShapeOrDefault.ShapeSizeIncrease;
    }

    public override void SetAlpha(int alpha) => _alpha = alpha;

    public override void SetColorFilter(ColorFilter? colorFilter) => _colorFilter = colorFilter;

    public override int Opacity => (int)Format.Translucent;

    protected override void OnBoundsChange(Rect bounds)
    {
        base.OnBoundsChange(bounds);
        Resize(bounds.Width(), bounds.Height());
    }

    public override void Draw(Canvas canvas)
    {
        if (_bitmap != null)
            canvas.DrawBitmap(_bitmap, 0f, 0f, null);
    }

    // ----- geometry helpers -----

    private static Matrix RotationMatrix(float degrees, float px, float py)
    {
        var m = new Matrix();
        m.SetRotate(degrees, px, py);
        return m;
    }

    private static Matrix TranslationMatrix(float dx, float dy)
    {
        var m = new Matrix();
        m.SetTranslate(dx, dy);
        return m;
    }

    private static Path Rotated(Path source, float angle, float px, float py)
    {
        var copy = new Path(source);
        copy.Transform(RotationMatrix(angle, px, py));
        return copy;
    }

    private static float CentralSymmetryAngle(int index) => index switch
    {
        0 => 0f,
        1 => -90f,
        2 => 90f,
        _ => 180f,
    };

    private static void WithTranslation(Canvas canvas, float dx, float dy, Action draw)
    {
        var count = canvas.Save();
        canvas.Translate(dx, dy);
        draw();
        canvas.RestoreToCount(count);
    }

    // ----- draw helpers -----

    private void DrawBg(Canvas canvas, Rect bounds, Bitmap? background)
    {
        var bgColor = _options.BackgroundOrDefault.ColorOrTransparent;
        if (bgColor is not QrVectorColor.Unspecified and not QrVectorColor.Transparent)
        {
            using var paint = bgColor.CreatePaint(bounds.Width(), bounds.Height());
            canvas.DrawPaint(paint);
        }
        if (background != null)
            canvas.DrawBitmap(background, 0f, 0f, null);
    }

    private void DrawBalls(Canvas canvas, float pixelSize, Path ballPath, Paint ballPaint)
    {
        var ballNumber = -1;
        foreach (var (x, y) in _balls)
        {
            var path = ballPath;
            if (_options.ShapesOrDefault.CentralSymmetry)
            {
                ballNumber++;
                path = Rotated(ballPath, CentralSymmetryAngle(ballNumber), pixelSize * 3 / 2, pixelSize * 3 / 2);
            }
            WithTranslation(canvas, x * pixelSize, y * pixelSize, () => canvas.DrawPath(path, ballPaint));
        }
    }

    private void DrawFrames(Canvas canvas, float pixelSize, Path framePath, Paint framePaint)
    {
        var frameNumber = -1;
        foreach (var (x, y) in _frames)
        {
            var path = framePath;
            if (_options.ShapesOrDefault.CentralSymmetry)
            {
                frameNumber++;
                path = Rotated(framePath, CentralSymmetryAngle(frameNumber), pixelSize * 7 / 2, pixelSize * 7 / 2);
            }
            WithTranslation(canvas, x * pixelSize, y * pixelSize, () => canvas.DrawPath(path, framePaint));
        }
    }

    private bool IsOnTimingLine(int x, int y) =>
        (x - _shapeIncrease == 6 || y - _shapeIncrease == 6) && !IsInsideFrameOrBall(x, y);

    private bool IsVersionEyeCenter(int x, int y) =>
        _anchorCenters.Any(c => c.X == x - _shapeIncrease && c.Y == y - _shapeIncrease);

    private bool IsFrameStart(int x, int y)
    {
        var size = _codeMatrix.Size;
        return (x - _shapeIncrease == 0 && y - _shapeIncrease == 0) ||
               (x - _shapeIncrease == 0 && y + _shapeIncrease == size - 7) ||
               (x + _shapeIncrease == size - 7 && y - _shapeIncrease == 0) ||
               (_options.FourthEyeEnabled && x + _shapeIncrease == size - 7 && y + _shapeIncrease == size - 7);
    }

    private bool IsBallStart(int x, int y)
    {
        var size = _codeMatrix.Size;
        return (x - _shapeIncrease == 2 && y + _shapeIncrease == size - 5) ||
               (x + _shapeIncrease == size - 5 && y - _shapeIncrease == 2) ||
               (x - _shapeIncrease == 2 && y - _shapeIncrease == 2) ||
               (_options.FourthEyeEnabled && x + _shapeIncrease == size - 5 && y + _shapeIncrease == size - 5);
    }

    private bool IsInsideFrameOrBall(int x, int y, bool checkAnchor = true)
    {
        var size = _codeMatrix.Size;
        var highlighting = _options.HighlightingOrDefault;

        var skipVersionEye = checkAnchor &&
            highlighting.VersionEyesOrNone is not HighlightingType.NoneType &&
            _anchorCenters.Any(c =>
                x - _shapeIncrease >= c.X - 2 && x - _shapeIncrease < c.X + 3 &&
                y - _shapeIncrease >= c.Y - 2 && y - _shapeIncrease < c.Y + 3);

        if (skipVersionEye) return true;

        bool In(int v, int lo, int hi) => v >= lo && v <= hi;

        return (In(x - _shapeIncrease, -1, 7) && In(y - _shapeIncrease, -1, 7)) ||
               (In(x - _shapeIncrease, -1, 7) && y + _shapeIncrease >= size - 8 && y + _shapeIncrease <= size) ||
               (x + _shapeIncrease >= size - 8 && x + _shapeIncrease <= size && In(y - _shapeIncrease, -1, 7)) ||
               (_options.FourthEyeEnabled &&
                   x + _shapeIncrease >= size - 8 && x + _shapeIncrease <= size &&
                   y + _shapeIncrease >= size - 8 && y + _shapeIncrease <= size);
    }

    // ----- resize / render pipeline -----

    private void Resize(int width, int height)
    {
        var darkPixelPath = new Path();
        var lightPixelPath = new Path();
        var darkTimingPath = new Path();
        var lightTimingPath = new Path();

        var size = Math.Min(width, height) * (1 - Math.Clamp(_options.Padding, 0f, .5f));
        if (size <= float.Epsilon)
            return;

        var pixelSize = size / _codeMatrix.Size;

        base.Alpha = _alpha;
        if (_colorFilter != null)
            SetColorFilter(_colorFilter);

        var ballPath = _ballShape.CreatePath(pixelSize * 3f, Neighbors.Empty);
        var framePath = _frameShape.CreatePath(pixelSize * 7f, Neighbors.Empty);

        var logo = _options.LogoOrDefault;
        var logoSize = size * logo.Size;
        var logoBgSize = (int)Math.Round(logoSize * (1 + logo.PaddingOrEmpty.Value));

        if (logo.PaddingOrEmpty is QrVectorLogoPadding.Natural)
            ApplyNaturalLogo(logoBgSize, (float)size, pixelSize);

        var logoBackgroundPath = logo.ShapeOrDefault.CreatePath(logoBgSize, Neighbors.Empty);

        Paint? logoPaint = logo.PaddingOrEmpty is QrVectorLogoPadding.EmptyPadding
            ? null
            : (logo.BackgroundColorOrUnspecified is QrVectorColor.Unspecified
                ? _options.BackgroundOrDefault.ColorOrTransparent
                : logo.BackgroundColorOrUnspecified).CreatePaint(logoBgSize, logoBgSize);

        CreateMainElements(pixelSize, framePath, ballPath, darkPixelPath, lightPixelPath, darkTimingPath, lightTimingPath);

        var logoBitmap = CreateLogo(logoSize);
        var background = CreateBackground(width, height);

        _bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888!);
        _bitmap.HasAlpha = true;
        using var canvas = new Canvas(_bitmap);
        DrawToCanvas(
            canvas, new Rect(0, 0, width, height), (float)size, pixelSize,
            darkPixelPath, lightPixelPath, darkTimingPath, lightTimingPath,
            framePath, ballPath, background,
            logoBgSize, logoBackgroundPath, logoPaint, logoBitmap);
    }

    private void CreateMainElements(
        float pixelSize, Path framePath, Path ballPath,
        Path darkPixelPath, Path lightPixelPath, Path darkTimingPath, Path lightTimingPath)
    {
        var shapes = _options.ShapesOrDefault;
        var colors = _options.ColorsOrDefault;
        var highlighting = _options.HighlightingOrDefault;

        var frameNumber = -1;
        var ballNumber = -1;

        for (var x = 0; x < _codeMatrix.Size; x++)
        {
            for (var y = 0; y < _codeMatrix.Size; y++)
            {
                var neighbors = _codeMatrix.Neighbors(x, y);
                var darkPath = shapes.DarkPixelOrDefault.CreatePath(pixelSize, neighbors);
                var lightPath = shapes.LightPixelOrDefault.CreatePath(pixelSize, neighbors);

                if (colors.FrameOrUnspecified is QrVectorColor.Unspecified && IsFrameStart(x, y))
                {
                    var path = framePath;
                    if (shapes.CentralSymmetry)
                    {
                        frameNumber++;
                        path = Rotated(framePath, CentralSymmetryAngle(frameNumber), pixelSize * 7 / 2, pixelSize * 7 / 2);
                    }
                    darkPixelPath.AddPath(path, x * pixelSize, y * pixelSize);
                }
                else if (colors.BallOrUnspecified is QrVectorColor.Unspecified && IsBallStart(x, y))
                {
                    var path = ballPath;
                    if (shapes.CentralSymmetry)
                    {
                        ballNumber++;
                        path = Rotated(ballPath, CentralSymmetryAngle(ballNumber), pixelSize * 3 / 2, pixelSize * 3 / 2);
                    }
                    darkPixelPath.AddPath(path, x * pixelSize, y * pixelSize);
                }
                else if (highlighting.VersionEyesOrNone is not HighlightingType.NoneType &&
                         (colors.FrameOrUnspecified is QrVectorColor.Unspecified || colors.BallOrUnspecified is QrVectorColor.Unspecified) &&
                         IsVersionEyeCenter(x, y))
                {
                    if (colors.FrameOrUnspecified is QrVectorColor.Unspecified)
                    {
                        var shape = highlighting.VersionEyesOrNone is HighlightingType.Styled
                            ? shapes.FrameOrDefault
                            : (IQrVectorFrameShape)DefaultVersionFrame.Instance;
                        darkPixelPath.AddPath(shape.CreatePath(pixelSize * 5, Neighbors.Empty), (x - 2) * pixelSize, (y - 2) * pixelSize);
                    }
                    if (colors.BallOrUnspecified is QrVectorColor.Unspecified)
                    {
                        var shape = highlighting.VersionEyesOrNone is HighlightingType.Styled
                            ? shapes.BallOrDefault
                            : QrBallShapeDefault.Instance;
                        darkPixelPath.AddPath(shape.CreatePath(pixelSize, Neighbors.Empty), x * pixelSize, y * pixelSize);
                    }
                }
                else if (IsInsideFrameOrBall(x, y))
                {
                    // covered by a frame/ball/version-eye draw above; nothing more to add.
                }
                else if (highlighting.TimingLinesOrNone is not HighlightingType.NoneType && IsOnTimingLine(x, y))
                {
                    var timingPath = TimingLinePath(highlighting.TimingLinesOrNone, pixelSize, neighbors, x, y, colors);
                    if (_codeMatrix[x, y] == QrCodeMatrix.PixelType.DarkPixel)
                        darkTimingPath.AddPath(timingPath, x * pixelSize, y * pixelSize);
                    else if (_codeMatrix[x, y] == QrCodeMatrix.PixelType.LightPixel)
                        lightTimingPath.AddPath(timingPath, x * pixelSize, y * pixelSize);
                }
                else
                {
                    if (_codeMatrix[x, y] == QrCodeMatrix.PixelType.DarkPixel)
                        darkPixelPath.AddPath(darkPath, x * pixelSize, y * pixelSize);
                    else if (_codeMatrix[x, y] == QrCodeMatrix.PixelType.LightPixel)
                        lightPixelPath.AddPath(lightPath, x * pixelSize, y * pixelSize);
                }
            }
        }
    }

    private Path TimingLinePath(HighlightingType type, float pixelSize, Neighbors neighbors, int x, int y, QrVectorColors colors)
    {
        if (type is HighlightingType.NoneType) return new Path();
        if (type is HighlightingType.DefaultType) return QrPixelShapeDefault.Instance.CreatePath(pixelSize, neighbors);

        // Styled (treated like Default's dark/light-shape fallback — see class-level scope note).
        return _codeMatrix[x, y] == QrCodeMatrix.PixelType.DarkPixel
            ? _options.ShapesOrDefault.DarkPixelOrDefault.CreatePath(pixelSize, colors.Dark.IsTransparent() ? Neighbors.Empty : neighbors)
            : _options.ShapesOrDefault.LightPixelOrDefault.CreatePath(pixelSize, colors.LightOrUnspecified.IsTransparent() ? Neighbors.Empty : neighbors);
    }

    private void DrawToCanvas(
        Canvas canvas, Rect bounds, float size, float pixelSize,
        Path darkPixelPath, Path lightPixelPath, Path darkTimingPath, Path lightTimingPath,
        Path framePath, Path ballPath, Bitmap? background,
        int logoBgSize, Path logoBgPath, Paint? logoBgPaint, Bitmap? logo)
    {
        var colors = _options.ColorsOrDefault;
        var highlighting = _options.HighlightingOrDefault;

        using var darkPixelPaint = colors.Dark.CreatePaint(_codeMatrix.Size * pixelSize, _codeMatrix.Size * pixelSize);
        darkPixelPaint.AntiAlias = true;
        using var lightPixelPaint = colors.LightOrUnspecified.CreatePaint(_codeMatrix.Size * pixelSize, _codeMatrix.Size * pixelSize);
        lightPixelPaint.AntiAlias = true;
        using var ballPaint = colors.BallOrUnspecified.CreatePaint(pixelSize * 3f, pixelSize * 3f);
        ballPaint.AntiAlias = true;
        using var framePaint = colors.FrameOrUnspecified.CreatePaint(pixelSize * 7f, pixelSize * 7f);
        framePaint.AntiAlias = true;

        var offsetX = Math.Clamp(_options.Offset.X, -1f, 1f) + 1;
        var offsetY = Math.Clamp(_options.Offset.Y, -1f, 1f) + 1;

        var density = canvas.Density;
        canvas.Density = (int)Bitmap.DensityNone;

        DrawBg(canvas, bounds, background);

        WithTranslation(canvas, (bounds.Width() - size) / 2f * offsetX, (bounds.Height() - size) / 2f * offsetY, () =>
        {
            HighlightCornerEyes(canvas, pixelSize);
            HighlightVersionEyes(canvas, pixelSize);

            canvas.DrawPath(darkPixelPath, darkPixelPaint);
            canvas.DrawPath(lightPixelPath, lightPixelPaint);

            using (var timingDarkPaint = TimingPaint(highlighting.TimingLinesOrNone, isDark: true, pixelSize, darkPixelPaint))
                canvas.DrawPath(darkTimingPath, timingDarkPaint);

            using (var timingLightPaint = TimingPaint(highlighting.TimingLinesOrNone, isDark: false, pixelSize, lightPixelPaint))
                canvas.DrawPath(lightTimingPath, timingLightPaint);

            if (colors.FrameOrUnspecified is not QrVectorColor.Unspecified)
                DrawFrames(canvas, pixelSize, framePath, framePaint);

            if (colors.BallOrUnspecified is not QrVectorColor.Unspecified)
                DrawBalls(canvas, pixelSize, ballPath, ballPaint);

            var lx = (size - logoBgSize) / 2f;
            var ly = (size - logoBgSize) / 2f;

            if (logoBgPaint != null)
                WithTranslation(canvas, lx, ly, () => canvas.DrawPath(logoBgPath, logoBgPaint));

            if (logo != null)
            {
                var x = (size - logo.Width) / 2f;
                var y = (size - logo.Height) / 2f;
                canvas.DrawBitmap(logo, x, y, null);
            }
        });

        canvas.Density = density;
    }

    private Paint TimingPaint(HighlightingType type, bool isDark, float pixelSize, Paint fallback)
    {
        if (type is HighlightingType.NoneType)
            return new Paint(); // empty path is drawn with this; color is irrelevant.

        if (type is HighlightingType.DefaultType)
        {
            if (isDark)
                return new QrVectorColor.Solid(unchecked((int)0xFF000000)).CreatePaint(
                    _codeMatrix.Size * pixelSize, _codeMatrix.Size * pixelSize);

            var paint = new QrVectorColor.Solid(unchecked((int)0xFFFFFFFF)).CreatePaint(
                _codeMatrix.Size * pixelSize, _codeMatrix.Size * pixelSize);
            paint.Alpha = (int)Math.Round(Math.Clamp(_options.HighlightingOrDefault.Alpha, 0f, 1f) * 255);
            return paint;
        }

        // Styled — fall back to the matching main-path paint (see class-level scope note).
        return fallback;
    }

    private Paint CreateHighlightingPaint(HighlightingType type, float size)
    {
        var styled = type is HighlightingType.Styled;
        var styledColor = (type as HighlightingType.Styled)?.Color;

        var colors = _options.ColorsOrDefault;
        QrVectorColor color = styledColor
            ?? (styled && !colors.LightOrUnspecified.IsTransparent() ? colors.LightOrUnspecified : null)
            ?? (styled && _options.BackgroundOrDefault.ColorOrTransparent.IsTransparent() ? _options.BackgroundOrDefault.ColorOrTransparent : null)
            ?? new QrVectorColor.Solid(unchecked((int)0xFFFFFFFF));

        var paint = color.CreatePaint(size, size);
        paint.Alpha = (int)Math.Round(Math.Clamp(_options.HighlightingOrDefault.Alpha, 0f, 1f) * 255);
        return paint;
    }

    private void HighlightVersionEyes(Canvas canvas, float pixelSize)
    {
        var type = _options.HighlightingOrDefault.VersionEyesOrNone;
        if (type is HighlightingType.NoneType) return;

        var shapes = _options.ShapesOrDefault;
        var colors = _options.ColorsOrDefault;

        IQrVectorFrameShape frame = type is HighlightingType.Styled ? shapes.FrameOrDefault : DefaultVersionFrame.Instance;
        IQrVectorBallShape ball = type is HighlightingType.Styled ? shapes.BallOrDefault : QrBallShapeDefault.Instance;

        var framePath = frame.CreatePath(pixelSize * 5, Neighbors.Empty);
        var ballPath = ball.CreatePath(pixelSize, Neighbors.Empty);
        using var highlightPaint = CreateHighlightingPaint(type, pixelSize * 5);

        Path highlightShape;
        if (type is HighlightingType.Styled styled && styled.Shape != null)
        {
            highlightShape = styled.Shape.CreatePath(pixelSize * 5, Neighbors.Empty);
        }
        else
        {
            var ballDefault = QrBallShapeDefault.Instance.CreatePath(pixelSize * 3, Neighbors.Empty);
            ballDefault.Transform(TranslationMatrix(pixelSize, pixelSize));
            highlightShape = new Path(framePath);
            highlightShape.AddPath(ballDefault);
        }

        foreach (var (ax, ay) in _anchorCenters)
        {
            WithTranslation(canvas, (_shapeIncrease + ax - 2) * pixelSize, (_shapeIncrease + ay - 2) * pixelSize, () =>
            {
                canvas.DrawPath(highlightShape, highlightPaint);

                if (colors.FrameOrUnspecified is not QrVectorColor.Unspecified)
                {
                    using var p = colors.FrameOrUnspecified.CreatePaint(pixelSize * 5, pixelSize * 5);
                    canvas.DrawPath(framePath, p);
                }

                WithTranslation(canvas, pixelSize * 2, pixelSize * 2, () =>
                {
                    if (colors.FrameOrUnspecified is not QrVectorColor.Unspecified)
                    {
                        using var p = colors.BallOrUnspecified.CreatePaint(pixelSize, pixelSize);
                        canvas.DrawPath(ballPath, p);
                    }
                });
            });
        }
    }

    private void HighlightCornerEyes(Canvas canvas, float pixelSize)
    {
        var type = _options.HighlightingOrDefault.CornerEyesOrNone;
        if (type is HighlightingType.NoneType) return;

        var shapes = _options.ShapesOrDefault;

        Path shape;
        if (type is HighlightingType.Styled styled)
        {
            if (styled.Shape != null)
            {
                shape = styled.Shape.CreatePath(pixelSize * 9, Neighbors.Empty);
            }
            else
            {
                shape = shapes.FrameOrDefault.CreatePath(pixelSize * 9, Neighbors.Empty);
                var ball = QrBallShapeDefault.Instance.CreatePath(pixelSize * 7, Neighbors.Empty);
                ball.Transform(TranslationMatrix(pixelSize, pixelSize));
                shape.AddPath(ball);
            }
        }
        else
        {
            shape = QrBallShapeDefault.Instance.CreatePath(pixelSize * 9, Neighbors.Empty);
        }

        using var paint = CreateHighlightingPaint(type, pixelSize * 9);
        foreach (var (x, y) in _frames)
        {
            WithTranslation(canvas, (x - 1) * pixelSize, (y - 1) * pixelSize, () => canvas.DrawPath(shape, paint));
        }
    }

    private void ApplyNaturalLogo(int logoBgSize, float size, float pixelSize)
    {
        var bgPath = _options.LogoOrDefault.ShapeOrDefault.CreatePath(logoBgSize, Neighbors.Empty);
        bgPath.Transform(TranslationMatrix((size - logoBgSize) / 2f, (size - logoBgSize) / 2f));

        var shapes = _options.ShapesOrDefault;

        for (var x = 0; x < _codeMatrix.Size; x++)
        {
            for (var y = 0; y < _codeMatrix.Size; y++)
            {
                var neighbors = _codeMatrix.Neighbors(x, y);
                var type = _codeMatrix[x, y];
                if (type != QrCodeMatrix.PixelType.DarkPixel && type != QrCodeMatrix.PixelType.LightPixel)
                    continue;

                var pixelPath = type == QrCodeMatrix.PixelType.DarkPixel
                    ? shapes.DarkPixelOrDefault.CreatePath(pixelSize, neighbors)
                    : shapes.LightPixelOrDefault.CreatePath(pixelSize, neighbors);
                pixelPath.Transform(TranslationMatrix(x * pixelSize, y * pixelSize));

                var intersection = new Path(bgPath);
                intersection.InvokeOp(pixelPath, Path.Op.Intersect);

                if (!intersection.IsEmpty)
                    _codeMatrix[x, y] = QrCodeMatrix.PixelType.Logo;
            }
        }
    }

    private Bitmap? CreateLogo(float logoSize)
    {
        var logo = _options.LogoOrDefault;
        if (logo.Bitmap == null || logoSize <= 0f)
            return null;

        var scaled = logo.Scale.Scale(logo.Bitmap, (int)logoSize, (int)logoSize);
        var mutable = scaled.IsMutable ? scaled : scaled.Copy(scaled.GetConfig()!, true)!;

        var fullRect = new Path();
        fullRect.AddRect(0f, 0f, logoSize, logoSize, Path.Direction.Cw);
        var shape = logo.ShapeOrDefault.CreatePath(logoSize, Neighbors.Empty);
        fullRect.InvokeOp(shape, Path.Op.Difference);

        using var canvas = new Canvas(mutable);
        var count = canvas.Save();
        canvas.ClipPath(fullRect);
        using var clearPaint = new Paint { Color = new AndroidColor(AndroidColor.Transparent.ToArgb()), AntiAlias = true };
        clearPaint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.Src));
        canvas.DrawRect(0f, 0f, mutable.Width, mutable.Height, clearPaint);
        canvas.RestoreToCount(count);

        return mutable;
    }

    private Bitmap? CreateBackground(int width, int height)
    {
        var background = _options.BackgroundOrDefault;
        return background.Bitmap == null ? null : background.Scale.Scale(background.Bitmap, width, height);
    }

    private sealed class DefaultVersionFrame : IQrVectorFrameShape
    {
        public static readonly DefaultVersionFrame Instance = new();

        public Path CreatePath(float size, Neighbors neighbors)
        {
            var path = new Path();
            var width = size / 5f;
            path.AddRect(0f, 0f, size, width, Path.Direction.Cw);
            path.AddRect(0f, 0f, width, size, Path.Direction.Cw);
            path.AddRect(size - width, 0f, size, size, Path.Direction.Cw);
            path.AddRect(0f, size - width, size, size, Path.Direction.Cw);
            return path;
        }
    }
}
