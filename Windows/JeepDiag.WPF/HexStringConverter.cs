using System;
using System.Globalization;
using System.Windows.Data;

namespace JeepDiag.WPF;

public class HexStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
            return string.Empty;

        string hex;
        switch (value)
        {
            case ushort u:
                hex = u.ToString("x");
                break;
            case uint u:
                hex = u.ToString("x");
                break;
            default:
                return string.Empty;
        }
        
        return "0x" + hex;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}