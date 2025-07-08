using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using StatsBB.Model;

namespace StatsBB.Converters
{
    public class TeamColorToForegroundConverter : IValueConverter
    {
        private static readonly Dictionary<TeamColor, Brush> BrushMap = new()
        {
            { TeamColor.Yellow, Brushes.Black },
            { TeamColor.Orange, Brushes.Black },
            { TeamColor.Blue, Brushes.White },
            { TeamColor.LightBlue, Brushes.Black },
            { TeamColor.DarkBlue, Brushes.White },
            { TeamColor.Grey, Brushes.Black },
            { TeamColor.Black, Brushes.White },
            { TeamColor.Purple, Brushes.White },
            { TeamColor.Pink, Brushes.Black },
            { TeamColor.Red, Brushes.Black },
            { TeamColor.Green, Brushes.White },
            { TeamColor.White, Brushes.Black },
            { TeamColor.Teal, Brushes.White },
            { TeamColor.Lime, Brushes.Black },
            { TeamColor.Brown, Brushes.White },
            { TeamColor.Maroon, Brushes.White },
            { TeamColor.BlueGrey, Brushes.Black },
            { TeamColor.Mustard, Brushes.Black }
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TeamColor color && BrushMap.TryGetValue(color, out var brush))
                return brush;
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
