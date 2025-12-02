using Picator.Game.Constants;
using System.Globalization;

namespace Picator.Game.Converters;

public class GameCodeUrlConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string code)
        {
            return $"{UrlConstants.InvitementUrl}/{Uri.EscapeDataString(code)}?player={Guid.NewGuid()}";
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}