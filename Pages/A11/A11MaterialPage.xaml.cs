using AtelierWiki.ViewModels;
using System.Windows.Controls;

namespace AtelierWiki.Pages.A11
{
    public partial class A11MaterialPage : Page
    {
        public A11MaterialPage()
        {
            InitializeComponent();
            var vm = new A11MaterialViewModel();
            this.DataContext = vm;
        }
    }
}
