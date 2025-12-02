namespace Picator.GameV2.Views.Components;

public class QrCodeView : View
{
    public static readonly BindableProperty DataProperty =
        BindableProperty.Create(
            nameof(Data),
            typeof(string),
            typeof(QrCodeView),
            default(string));

    public static readonly BindableProperty QrTypeProperty =
        BindableProperty.Create(
            nameof(QrType),
            typeof(QrCodeType),
            typeof(QrCodeView),
            QrCodeType.Url);

    public static readonly BindableProperty PaddingRatioProperty =
        BindableProperty.Create(
            nameof(PaddingRatio),
            typeof(float),
            typeof(QrCodeView),
            0.125f);

    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(
            nameof(Color),
            typeof(Color),
            typeof(QrCodeView),
            Colors.Black);

    public string Data
    {
        get => (string)GetValue(DataProperty);
        set => SetValue(DataProperty, value);
    }

    public QrCodeType QrType
    {
        get => (QrCodeType)GetValue(QrTypeProperty);
        set => SetValue(QrTypeProperty, value);
    }

    public float PaddingRatio
    {
        get => (float)GetValue(PaddingRatioProperty);
        set => SetValue(PaddingRatioProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
}

public enum QrCodeType
{
    PlainText,
    Url,
    WiFi,
    Email,
    GeoPos,
    Phone
}