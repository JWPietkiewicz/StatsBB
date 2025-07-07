using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace StatsBB.Converters
{
    public class IsTeamAToCourtStyleConverter : IValueConverter
    {
        public Style TeamAStyle { get; set; }
        public Style TeamBStyle { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isTeamA)
                return isTeamA ? TeamAStyle : TeamBStyle;
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
