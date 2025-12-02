using System.Globalization;

namespace Picator.GameV2.Converters;

public class HasShadowConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool hasShadow && hasShadow)
            return "#4D000000";
        return Colors.Transparent;
    }
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}