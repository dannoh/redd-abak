using System;
using System.Globalization;
using System.Windows;

namespace AbakHelper.Converters
{
    public class NullToVisibilityConverter : ConverterMarkupExtensionBase
    {
        public bool Invert { get; set; }
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNull = value == null ^ Invert;
            return isNull ? Visibility.Collapsed : Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
