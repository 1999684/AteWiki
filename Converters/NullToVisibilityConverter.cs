using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AtelierWiki.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isNullOrEmpty = value == null || string.IsNullOrEmpty(value.ToString());

            if (parameter?.ToString() == "Reverse")
            {
                return isNullOrEmpty ? Visibility.Collapsed : Visibility.Visible;
            }

            return isNullOrEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
