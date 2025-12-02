using Android.Widget;
using Com.Github.Alexzhirkevich.Customqrgenerator;
using Com.Github.Alexzhirkevich.Customqrgenerator.Encoder;
using Com.Github.Alexzhirkevich.Customqrgenerator.Style;
using Com.Github.Alexzhirkevich.Customqrgenerator.Vector;
using Com.Github.Alexzhirkevich.Customqrgenerator.Vector.Style;
using Microsoft.Maui.Handlers;
using Picator.GameV2.Views.Components;

namespace Picator.GameV2.Platforms.Android.Handlers;

public class QrCodeViewHandler : ViewHandler<QrCodeView, ImageView>
{
    public static IPropertyMapper<QrCodeView, QrCodeViewHandler> Mapper =
       new PropertyMapper<QrCodeView, QrCodeViewHandler>(ViewMapper)
       {
           [nameof(QrCodeView.Data)] = MapData,
           [nameof(QrCodeView.QrType)] = MapQrType,
           [nameof(QrCodeView.PaddingRatio)] = MapPaddingRatio,
           [nameof(QrCodeView.Color)] = MapColor,
       };

    // Constructor
    public QrCodeViewHandler() : base(Mapper)
    {
    }

    public QrCodeViewHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null)
        : base(mapper ?? Mapper, commandMapper)
    {
    }

    protected override ImageView CreatePlatformView()
    {
        var imageView = new ImageView(Context);
        imageView.SetScaleType(ImageView.ScaleType.FitCenter);
        return imageView;
    }

    protected override void ConnectHandler(ImageView platformView)
    {
        base.ConnectHandler(platformView);
        UpdateQrCode();
    }

    public static void MapData(QrCodeViewHandler handler, QrCodeView view)
    {
        handler.UpdateQrCode();
    }

    public static void MapQrType(QrCodeViewHandler handler, QrCodeView view)
    {
        handler.UpdateQrCode();
    }

    public static void MapPaddingRatio(QrCodeViewHandler handler, QrCodeView view)
    {
        handler.UpdateQrCode();
    }

    public static void MapColor(QrCodeViewHandler handler, QrCodeView view)
    {
        handler.UpdateQrCode();
    }

    private void UpdateQrCode()
    {
        if (PlatformView == null || VirtualView == null)
            return;

        if (string.IsNullOrEmpty(VirtualView.Data))
            return;

        try
        {
            // Step 1: Create QR data based on type
            var qrData = new QrData(VirtualView.Data);

            // Step 2: Create options
            var options = CreateQrOptions();

            // Step 3: Create drawable
            var drawable = QrCodeDrawableKt.QrCodeDrawable(qrData, CreateQrOptions(), Java.Nio.Charset.Charset.DefaultCharset());
            // Set to ImageView
            PlatformView.SetImageDrawable(drawable);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"QR Code generation error: {ex}");
        }
    }

    private QrVectorOptions CreateQrOptions()
    {
        var options = new QrVectorOptions(VirtualView.PaddingRatio, new QrOffset(0, 0), new QrVectorShapes(new QrVectorPixelShape(4), new QrVectorPixelShape(), new QrVectorBallShape(100), new QrVectorFrameShape(100), true), 
            new QrBallShape(), new QrVectorColors(), new QrVectorLogo(),
            new QrVectorBackground(), QrErrorCorrectionLevel.Auto, false, new QrHighlighting());

        return options;
    }

    private QrVectorOptions WithBuilder()
    {
       return new QrVectorOptions.Builder()
            .SetPadding(VirtualView.PaddingRatio)
            .SetShapes(new QrVectorShapes()).Build();
    }

    public class QrData : Java.Lang.Object, IQrData
    {
        private string _data;

        public QrData(string data)
        {
            _data = data;
        }
        public string Encode()
        {
            return "wtf";
        }
    }

    public class QrBallShape : Java.Lang.Object, IQrShape
    {
        public float ShapeSizeIncrease => 0f;

        public QrCodeMatrix Apply(QrCodeMatrix matrix)
        {
            // Return the matrix unchanged - ball shape is handled by QrVectorBallShape
            return matrix;
        }

        public bool PixelInShape(int i, int j, QrCodeMatrix modifiedByteMatrix)
        {
            // Get the size (it's square)
            int size = modifiedByteMatrix.Size;

            // Calculate center
            float centerX = (size - 1) / 2f;
            float centerY = (size - 1) / 2f;

            // Calculate normalized distance from center
            float dx = (i - centerX) / centerX;
            float dy = (j - centerY) / centerY;

            // Use circle equation: x² + y² <= r²
            // Adjust the threshold: 1.0 = perfect circle, < 1.0 = smaller/tighter circle
            float distanceSquared = dx * dx + dy * dy;

            return distanceSquared <= 0.95f; // Try values between 0.7 - 1.0 for different roundness
        }
    }

    public class QrVectorPixelShape : Java.Lang.Object, IQrVectorPixelShape
    {
        private float _cornerRadius;

        public QrVectorPixelShape(float cornerRadius = 0f)
        {
            _cornerRadius = cornerRadius;
        }

        public global::Android.Graphics.Path CreatePath(float size, Neighbors neighbors)
        {
            var path = new global::Android.Graphics.Path();

            if (_cornerRadius <= 0)
            {
                // Simple square pixel
                path.AddRect(0, 0, size, size, global::Android.Graphics.Path.Direction.Cw);
            }
            else
            {
                // Rounded corners based on neighbors
                var rect = new global::Android.Graphics.RectF(0, 0, size, size);
                var radius = size * _cornerRadius;

                // Create rounded rectangle
                var radii = new float[8];

                // Top-left corner
                if (!neighbors.Top && !neighbors.Left)
                    radii[0] = radii[1] = radius;

                // Top-right corner
                if (!neighbors.Top && !neighbors.Right)
                    radii[2] = radii[3] = radius;

                // Bottom-right corner
                if (!neighbors.Bottom && !neighbors.Right)
                    radii[4] = radii[5] = radius;

                // Bottom-left corner
                if (!neighbors.Bottom && !neighbors.Left)
                    radii[6] = radii[7] = radius;

                path.AddRoundRect(rect, radii, global::Android.Graphics.Path.Direction.Cw);
            }

            return path;
        }
    }

    public class QrVectorBallShape : Java.Lang.Object, IQrVectorBallShape
    {
        private float _cornerRadius;

        public QrVectorBallShape(float cornerRadius = 0.5f)
        {
            _cornerRadius = cornerRadius;
        }

        public global::Android.Graphics.Path CreatePath(float size, Neighbors neighbors)
        {
            var path = new global::Android.Graphics.Path();
            var rect = new global::Android.Graphics.RectF(8, 8, size, size);

            if (_cornerRadius >= 0.5f)
            {
                // Circular ball (eye)
                path.AddCircle(size / 2, size / 2, size, global::Android.Graphics.Path.Direction.Cw);
            }
            else if (_cornerRadius > 0)
            {
                // Rounded square ball
                var radius = size * _cornerRadius;
                path.AddRoundRect(rect, radius, radius, global::Android.Graphics.Path.Direction.Cw);
            }
            else
            {
                // Square ball
                path.AddRect(rect, global::Android.Graphics.Path.Direction.Cw);
            }

            return path;
        }
    }

    public class QrVectorFrameShape : Java.Lang.Object, IQrVectorFrameShape
    {
        private float _cornerRadius;
        private float _strokeWidth;

        public QrVectorFrameShape(float cornerRadius = 0.5f, float strokeWidth = 1f)
        {
            _cornerRadius = cornerRadius;
            _strokeWidth = strokeWidth;
        }

        public global::Android.Graphics.Path CreatePath(float size, Neighbors neighbors)
        {
            var path = new global::Android.Graphics.Path();
            var strokeSize = size * _strokeWidth;

            // Outer rectangle
            var outerRect = new global::Android.Graphics.RectF(0, 0, size, size);
            // Inner rectangle (creates the frame effect)
            var innerRect = new global::Android.Graphics.RectF(
                strokeSize,
                strokeSize,
                size - strokeSize,
                size - strokeSize
            );

            if (_cornerRadius >= 0.5f)
            {
                // Circular frame
                path.AddCircle(size / 2, size / 2, size / 2, global::Android.Graphics.Path.Direction.Cw);
                path.AddCircle(size / 2, size / 2, (size / 2) - strokeSize, global::Android.Graphics.Path.Direction.Ccw);
            }
            else if (_cornerRadius > 0)
            {
                // Rounded frame
                var outerRadius = size * _cornerRadius;
                var innerRadius = Math.Max(0, outerRadius - strokeSize);

                path.AddRoundRect(outerRect, outerRadius, outerRadius, global::Android.Graphics.Path.Direction.Cw);
                path.AddRoundRect(innerRect, innerRadius, innerRadius, global::Android.Graphics.Path.Direction.Ccw);
            }
            else
            {
                // Square frame
                path.AddRect(outerRect, global::Android.Graphics.Path.Direction.Cw);
                path.AddRect(innerRect, global::Android.Graphics.Path.Direction.Ccw);
            }

            return path;
        }
    }

}