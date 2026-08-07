using Android.Widget;
using Microsoft.Maui.Handlers;
using Picator.GameV2.Platforms.Android.QrGeneration;
using Picator.GameV2.Platforms.Android.QrGeneration.Vector;
using Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;
using Picator.GameV2.Views.Components;

namespace Picator.GameV2.Platforms.Android.Handlers;

/// <summary>
/// Handler for <see cref="QrCodeView"/>, backed by the native C# QR renderer in
/// Picator.GameV2/Platforms/Android/QrGeneration/ (no Java/Kotlin binding involved).
///
/// This replaced an earlier binding-based handler that wrapped a Kotlin/.aar library
/// (QrGenerator.Binding, since removed) via a generated Java-interop binding project. That
/// binding also had two shape bugs the native port intentionally does not reproduce: an
/// oversized ball radius (drawn at the full cell size instead of half) and a frame ring whose
/// inner "hole" had a negative radius due to an unset stroke width, both of which made eyes
/// render as fused blobs rather than clean shapes.
/// </summary>
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

    public static void MapData(QrCodeViewHandler handler, QrCodeView view) => handler.UpdateQrCode();
    public static void MapQrType(QrCodeViewHandler handler, QrCodeView view) => handler.UpdateQrCode();
    public static void MapPaddingRatio(QrCodeViewHandler handler, QrCodeView view) => handler.UpdateQrCode();
    public static void MapColor(QrCodeViewHandler handler, QrCodeView view) => handler.UpdateQrCode();

    private void UpdateQrCode()
    {
        if (PlatformView == null || VirtualView == null)
            return;

        if (string.IsNullOrEmpty(VirtualView.Data))
            return;

        try
        {
            var data = BuildQrData(VirtualView.Data, VirtualView.QrType);
            var options = CreateQrOptions();
            var drawable = new QrCodeDrawable(data, options);
            PlatformView.SetImageDrawable(drawable);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"QR code generation error: {ex}");
        }
    }

    // NOTE: WiFi/Email/GeoPos have no established structured-string format on QrCodeView.Data
    // today, so they fall back to plain text pending a real input contract for those types.
    private static IQrData BuildQrData(string data, QrCodeType qrType) => qrType switch
    {
        QrCodeType.Url => new QrDataUrl(data),
        QrCodeType.Phone => new QrDataPhone(data),
        _ => new QrDataText(data),
    };

    private QrVectorOptions CreateQrOptions() => new(
        Padding: VirtualView.PaddingRatio,
        Offset: new QrOffset(0, 0),
        Shapes: new QrVectorShapes(
            DarkPixel: new QrPixelShapeRoundCorners(4f),
            LightPixel: QrPixelShapeDefault.Instance,
            Ball: new QrBallShapeCircle(1f),
            Frame: new QrFrameShapeCircle(width: 1f, radius: 1f),
            CentralSymmetry: true),
        CodeShape: new QrShapeCircle(),
        Colors: QrVectorColors.Defaults() with { Dark = new QrVectorColor.Solid(ToArgb(VirtualView.Color)) },
        ErrorCorrectionLevel: QrErrorCorrectionLevel.Auto,
        FourthEyeEnabled: false);

    private static int ToArgb(Microsoft.Maui.Graphics.Color color) =>
        unchecked((int)((byte)(color.Alpha * 255) << 24 | (byte)(color.Red * 255) << 16 | (byte)(color.Green * 255) << 8 | (byte)(color.Blue * 255)));
}
