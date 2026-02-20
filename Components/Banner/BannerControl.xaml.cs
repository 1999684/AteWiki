using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using AtelierWiki.Data;

namespace AtelierWiki.Components.Banner
{
    public partial class BannerControl : UserControl
    {
        private List<BannerItem> _bannerItems;
        private int _currentIndex = 0;
        private DispatcherTimer _timer;

        public BannerControl()
        {
            InitializeComponent();
            InitializeTimer();
            LoadData();
            UpdateBanner();
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(2);
            _timer.Tick += (s, e) => ShowNext();
        }

        private void LoadData()
        {
            try
            {
                // 从数据库读取
                _bannerItems = DbManager.GetBanners();
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取轮播图失败: " + ex.Message);
                _bannerItems = new List<BannerItem>();
            }
        }

        private void UpdateBanner()
        {
            if (_bannerItems == null || _bannerItems.Count == 0)
            {
                // 如果没有数据，清空显示
                BannerBg.Source = null;
                BannerIcon.Source = null;
                BannerPerson.Source = null;
                IndexIndicator.Text = "No Data";
                return;
            }

            try
            {
                var item = _bannerItems[_currentIndex];

                // 1. 设置背景 (BG)
                SetImageSource(BannerBg, item.ImagePath);

                // 2. 设置图标 (Icon)
                SetImageSource(BannerIcon, item.IconPath);

                // 3. 设置人物 (Person)
                SetImageSource(BannerPerson, item.PersonPath);

                IndexIndicator.Text = $"{_currentIndex + 1} / {_bannerItems.Count}";
            }
            catch (Exception)
            {
                IndexIndicator.Text = "Err";
            }
        }

        // 通用的图片设置辅助方法，兼容 Resource 和 文件路径
        private void SetImageSource(Image imgControl, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                imgControl.Source = null;
                return;
            }

            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();

                string packUri = path;

                if (!path.StartsWith("pack:"))
                {
                    packUri = $"pack://application:,,,/AtelierWiki;component{path}";
                }

                bitmap.UriSource = new Uri(packUri, UriKind.Absolute);

                bitmap.CacheOption = BitmapCacheOption.OnLoad; // 加载后释放文件锁
                bitmap.EndInit();
                imgControl.Source = bitmap;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载失败 [{path}]: {ex.Message}");
                imgControl.Source = null;
            }
        }

        // --- 交互事件 ---

        private void BannerImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_bannerItems == null || _bannerItems.Count == 0) return;

            var url = _bannerItems[_currentIndex].SteamUrl;
            if (!string.IsNullOrEmpty(url))
            {
                string steamUrl2 = "steam://openurl/" + url;
                try
                {
                    Process.Start(new ProcessStartInfo { FileName = steamUrl2, UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("无法打开链接: " + ex.Message);
                }
            }
        }

        private void ShowNext()
        {
            if (_bannerItems == null || _bannerItems.Count == 0) return;
            _currentIndex = (_currentIndex + 1) % _bannerItems.Count;
            UpdateBanner();
        }

        private void ShowPrev()
        {
            if (_bannerItems == null || _bannerItems.Count == 0) return;
            _currentIndex--;
            if (_currentIndex < 0) _currentIndex = _bannerItems.Count - 1;
            UpdateBanner();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e) { ShowPrev(); RestartTimer(); }
        private void NextButton_Click(object sender, RoutedEventArgs e) { ShowNext(); RestartTimer(); }

        private void RestartTimer() { _timer.Stop(); _timer.Start(); }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e) => _timer.Stop();
        private void UserControl_MouseLeave(object sender, MouseEventArgs e) => _timer.Start();
        private void UserControl_Loaded(object sender, RoutedEventArgs e) => _timer.Start();
        private void UserControl_Unloaded(object sender, RoutedEventArgs e) => _timer.Stop();
    }
}
