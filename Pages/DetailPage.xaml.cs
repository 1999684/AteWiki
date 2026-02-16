using AtelierWiki.ViewModels;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AtelierWiki.Pages
{
    public partial class GameDetailPage : Page
    {
        private ExcelViewModel _viewModel;

        public GameDetailPage(int gameId, string title)
        {
            InitializeComponent();

            _viewModel = new ExcelViewModel();
            _viewModel.PageTitle = title;
            this.DataContext = _viewModel;

            LoadExcelData(gameId);
        }

        private void LoadExcelData(int gameId)
        {
            string fileName = "";
            switch (gameId)
            {
                case 1: fileName = "A11_Data.xlsx"; break;
                case 2: fileName = "A12_Data.xlsx"; break;
                case 3: fileName = "A13_Data.xlsx"; break;
                default:
                    break;
            }

            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show($"暂无 ID={gameId} 的数据文件配置");
                return;
            }

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles", fileName);

            if (File.Exists(path))
            {
                _viewModel.LoadExcel(path);
            }
            else
            {
                MessageBox.Show($"未找到数据文件: {fileName}\n请确认文件已放入 DataFiles 目录并设为'复制到输出目录'");
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
