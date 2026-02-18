using AtelierWiki.ViewModels;
using System.Windows.Controls;

namespace AtelierWiki.Pages.A11
{
    /// <summary>
    /// A11HarmonyPage.xaml 的交互逻辑
    /// </summary>
    public partial class A11HarmonyPage : Page
    {
        public A11HarmonyPage()
        {
            InitializeComponent();
            this.DataContext = new A11HarmonyViewModel();
        }
    }
}
