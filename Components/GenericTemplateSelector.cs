using System.Windows;
using System.Windows.Controls;
using System.Reflection;

namespace AtelierWiki.Components
{
    /// <summary>
    /// 一个通用的模板选择器，用于根据对象属性切换模板。
    /// 默认支持带有 IsAddButton 属性的对象。
    /// </summary>
    public class GenericTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate AddTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return null;

            // 使用反射检查对象是否包含 IsAddButton 属性
            // 这样可以兼容 Material, Harmony 或任何其他具有该标识的类
            PropertyInfo prop = item.GetType().GetProperty("IsAddButton");
            if (prop != null)
            {
                bool isAdd = (bool)prop.GetValue(item);
                if (isAdd) return AddTemplate;
            }

            return NormalTemplate;
        }
    }
}
