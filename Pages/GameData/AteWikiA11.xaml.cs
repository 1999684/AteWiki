using AtelierWiki.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AtelierWiki.Pages.GameData
{
    public partial class AteWikiA11 : Page
    {
        private ExcelViewModel _viewModel;

        public AteWikiA11()
        {
            InitializeComponent();

            _viewModel = new ExcelViewModel();
            this.DataContext = _viewModel;

            Loaded += AteWikiA11_Loaded;
        }

        private void AteWikiA11_Loaded(object sender, RoutedEventArgs e)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles", "A11_Data.xlsx");

            if (File.Exists(path))
            {
                _viewModel.LoadExcel(path);
            }
            else
            {
                MessageBox.Show($"未找到数据文件: {path}\n请确认已创建文件并设为'复制到输出目录'");
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
