using System.Globalization;

namespace Picator.GameV2.Converters;

public class GameWordSpaceCharColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is char ch)
        {
            if (ch != ' ')
            {
                return Colors.Red;
            }
            Application.Current.Resources.TryGetValue("Primary", out var colorResource);
            return colorResource;
        }
        return Colors.White;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}
