using System.Globalization;
using System.Windows.Data;

namespace MTerminal.WPF.Converters;

internal class FontSizeBiggerConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double fontSize = (double)value;
        return fontSize + System.Convert.ToDouble(parameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
