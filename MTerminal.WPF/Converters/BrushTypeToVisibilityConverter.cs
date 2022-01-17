using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MTerminal.WPF.Converters;

internal class BrushTypeToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ImageBrush)
            return Visibility.Visible;
        else
            return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
