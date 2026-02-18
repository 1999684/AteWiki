using AtelierWiki.ViewModels;
using System.Windows.Controls;

namespace AtelierWiki.Pages.A11
{
    public partial class A11FeaturePage : Page
    {
        public A11FeaturePage()
        {
            InitializeComponent();
            var vm = new A11FeatureViewModel();
            this.DataContext = vm;
        }
    }
}
