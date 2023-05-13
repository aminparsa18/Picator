using System.Globalization;
using System.Web;

namespace Picator.Game.Converters;

public class GameCodeUrlConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string code)
        {
            return "https://lively-tree-061c28b10.3.azurestaticapps.net/qr.html?game_code=" + HttpUtility.UrlEncode(code);
        }
        return "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}