using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AtelierWiki.Converters // 必须完全匹配
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = value is bool b && b;

            // 处理反转逻辑 ConverterParameter='Reverse'
            if (parameter?.ToString() == "Reverse")
                isVisible = !isVisible;

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility v && v == Visibility.Visible;
        }
    }
}
