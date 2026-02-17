using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace AtelierWiki.Converters
{
    public class RelativePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;

            if (string.IsNullOrEmpty(path)) return null;

            try
            {
                // 1. 获取绝对路径
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                // 去掉开头可能存在的 / 或 \
                string cleanPath = path.TrimStart('/', '\\');
                string fullPath = Path.Combine(baseDir, cleanPath);

                // 2. 再次确认文件是否存在
                if (!File.Exists(fullPath))
                {
                    // 调试模式下可以在这里打断点，看 fullPath 到底是指向哪里
                    return null;
                }

                // 3. 【核心修改】使用文件流读取，而不是 URI
                // 这种方式最稳健，不会因为路径格式问题导致不显示
                var bitmap = new BitmapImage();

                using (var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                {
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // 加载即关闭文件流
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                }

                bitmap.Freeze(); // 冻结对象，提高性能
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
