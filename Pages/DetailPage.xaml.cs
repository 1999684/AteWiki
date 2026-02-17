using AtelierWiki.ViewModels;
using AtelierWiki.Components.Dialogs; // 引入弹窗命名空间
using System;
using System.Data;
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

            _viewModel.RequestScrollToRow += OnRequestScrollToRow;

            this.DataContext = _viewModel;
            this.Unloaded += GameDetailPage_Unloaded;

            LoadExcelData(gameId);
        }

        private void OnRequestScrollToRow(DataRowView row)
        {
            if (row != null && MyDataGrid.Items.Count > 0)
            {
                MyDataGrid.ScrollIntoView(row);
            }
        }

        private void GameDetailPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.RequestScrollToRow -= OnRequestScrollToRow;
            }
        }

        private void LoadExcelData(int gameId)
        {
            string fileName = "";
            switch (gameId)
            {
                case 1: fileName = "A11_Data.xlsx"; break;
                case 2: fileName = "A12_Data.xlsx"; break;
                case 3: fileName = "A13_Data.xlsx"; break;
                case 4: fileName = "A14_Data.xlsx"; break;
                case 5: fileName = "A15_Data.xlsx"; break;
                case 6: fileName = "A16_Data.xlsx"; break;
                case 7: fileName = "A17_Data.xlsx"; break;
                case 8: fileName = "A18_Data.xlsx"; break;
                case 9: fileName = "A19_Data.xlsx"; break;
                case 10: fileName = "A20_Data.xlsx"; break;
                case 11: fileName = "A21_Data.xlsx"; break;
                case 12: fileName = "A22_Data.xlsx"; break;
                case 13: fileName = "A23_Data.xlsx"; break;
                case 14: fileName = "A24_Data.xlsx"; break;
                case 15: fileName = "A25_Data.xlsx"; break;
                case 16: fileName = "A26_Data.xlsx"; break;
                case 17: fileName = "A1r_Data.xlsx"; break;
                case 18: fileName = "A25rw_Data.xlsx"; break;
                default:
                    break;
            }

            // 1. 检查是否配置了文件名
            if (string.IsNullOrEmpty(fileName))
            {
                // 使用自定义弹窗
                TechDialog.Show($"ACCESS DENIED\n\nNo data configuration found for ID: {gameId}");
                return;
            }

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataFiles", fileName);

            // 2. 检查文件是否存在
            if (File.Exists(path))
            {
                _viewModel.LoadExcel(path);
            }
            else
            {
                // 使用自定义弹窗
                TechDialog.Show($"暂无相关信息，抱歉！！\n\ndata file is missing:\n{fileName}\n\n");
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
