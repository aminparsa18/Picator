namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>Shapes of the QR code elements. Native port of vector/style/QrVectorShapes.kt.</summary>
public sealed record QrVectorShapes(
    IQrVectorPixelShape? DarkPixel = null,
    IQrVectorPixelShape? LightPixel = null,
    IQrVectorBallShape? Ball = null,
    IQrVectorFrameShape? Frame = null,
    bool CentralSymmetry = true)
{
    public IQrVectorPixelShape DarkPixelOrDefault => DarkPixel ?? QrPixelShapeDefault.Instance;
    public IQrVectorPixelShape LightPixelOrDefault => LightPixel ?? QrPixelShapeDefault.Instance;
    public IQrVectorBallShape BallOrDefault => Ball ?? QrBallShapeDefault.Instance;
    public IQrVectorFrameShape FrameOrDefault => Frame ?? QrFrameShapeDefault.Instance;
}
