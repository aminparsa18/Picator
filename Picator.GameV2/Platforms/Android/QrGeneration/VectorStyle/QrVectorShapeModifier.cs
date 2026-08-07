using Android.Graphics;

namespace Picator.GameV2.Platforms.Android.QrGeneration.VectorStyle;

/// <summary>
/// Creates a drawable path for a single QR element of the given <paramref name="size"/>, aware
/// of its neighboring pixels. Native port of customqrgenerator's
/// vector/style/QrVectorShapeModifier.kt.
/// </summary>
public interface IQrVectorShapeModifier
{
    global::Android.Graphics.Path CreatePath(float size, Neighbors neighbors);
}
