using System;
using System.Globalization;
using System.Windows.Data;

namespace StatsBB.Converters;

public class ColorAvailabilityConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0] != null && values[1] != null)
        {
            return !Equals(values[0], values[1]);
        }
        return true;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
