using System;
using System.Drawing;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ModernKeePass.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var color = value is Color color1 ? color1 : Color.Empty;
            if (color == Color.Empty && parameter is SolidColorBrush) return (SolidColorBrush) parameter;
            return new SolidColorBrush(Windows.UI.Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(value is SolidColorBrush brush) ? new Color() : Color.FromArgb(brush.Color.A, brush.Color.R, brush.Color.G, brush.Color.B);
        }
    }
}