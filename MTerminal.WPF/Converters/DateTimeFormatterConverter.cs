using System.Globalization;
using System.Windows.Data;

namespace MTerminal.WPF.Converters;

[ValueConversion(typeof(DateTime), typeof(string))]
public class DateTimeFormatterConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        DateTime dateTime = (DateTime)value;

        return dateTime.ToString("[HH:mm:ss]", DateTimeFormatInfo.InvariantInfo);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}