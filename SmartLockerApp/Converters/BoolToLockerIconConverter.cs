using System.Globalization;

namespace SmartLockerApp.Converters;

public class BoolToLockerIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isAvailable)
        {
            return isAvailable ? "🔓" : "🔒";
        }
        return "🔒";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
