using System.Globalization;

namespace Picator.GameV2.Converters;

public class GameWordSpaceCharColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var key = value is char ch && ch != ' ' ? "AccentTint" : "Surface";
        Application.Current!.Resources.TryGetValue(key, out var colorResource);
        return colorResource;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
