using ScanCode.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ScanCode.Converters
{
    internal class StatusToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            if (((ViewStatus)value).ToString() == parameter.ToString())
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Visible;
        }
    }
}
