using System;
using System.Globalization;
using System.Windows.Data;

namespace StatsBB.Converters;

public class BoolToXConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && b)
            return "x";
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s)
            return s.Equals("x", StringComparison.InvariantCultureIgnoreCase);
        return false;
    }
}
