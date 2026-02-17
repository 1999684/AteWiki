using AtelierWiki.Data;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data; // 必须引用

namespace AtelierWiki.Pages.Home
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            LoadGameData();
        }

        private void LoadGameData()
        {
            try
            {
                // 1. 从数据库读取原始列表
                var games = DbManager.GetGames();

                // 2. 获取页面资源里的 CollectionViewSource
                CollectionViewSource cvs = (CollectionViewSource)this.Resources["GroupedGames"];

                // 3. 将数据赋值给 Source
                cvs.Source = games;
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取游戏列表失败: " + ex.Message);
            }
        }

        // 卡片点击事件
        private void GameCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int gameId)
            {
                // 推荐根据 ID 跳转
                if (gameId == 1)
                {
                    NavigationService.Navigate(new Pages.A11.A11MainFrame());
                }
                else
                {
                    MessageBox.Show($"你点击了游戏 ID: {gameId} (暂无详情页)");
                }
            }
        }
    }
}
