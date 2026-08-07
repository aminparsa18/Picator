namespace Picator.GameV2.Platforms.Android.QrGeneration;

/// <summary>Native port of customqrgenerator's style/QrOffset.kt: -1..1 offset of the code within its bounds.</summary>
public readonly record struct QrOffset(float X = 0f, float Y = 0f);
