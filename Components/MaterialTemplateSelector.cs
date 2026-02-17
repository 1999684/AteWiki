using System.Windows;
using System.Windows.Controls;
using AtelierWiki.Data.A11; // 确保引用了 Material 类所在的命名空间

namespace AtelierWiki.Components // 必须完全匹配
{
    public class MaterialTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate AddTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is Material m && m.IsAddButton)
            {
                return AddTemplate;
            }
            return NormalTemplate;
        }
    }
}
