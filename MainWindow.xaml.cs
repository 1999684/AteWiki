using System.Windows;
using System.Windows.Input;
using AtelierWiki.Pages.Home;

namespace AtelierWiki
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // 启动时导航到首页
            MainFrame.Navigate(new HomePage());
        }

        // 1. 实现窗口拖动
        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // 双击标题栏最大化/还原
                if (e.ClickCount == 2)
                {
                    Maximize_Click(sender, e);
                }
                else
                {
                    this.DragMove();
                }
            }
        }

        // 2. 最小化
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // 3. 最大化 / 还原
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                BtnMaximize.Content = "❐"; // 切换图标为"还原"

                // 最大化时修正 Margin，防止遮挡任务栏或溢出屏幕
                // 注意：使用 WindowStyle=None 时，最大化默认会盖住任务栏，需要特殊处理
                // 这里简单处理，如果追求完美需要引入 Win32 API 修复最大化边距
                this.BorderThickness = new Thickness(0);
            }
            else
            {
                this.WindowState = WindowState.Normal;
                BtnMaximize.Content = "▢"; // 切换图标为"最大化"
                this.BorderThickness = new Thickness(1); // 恢复边框
            }
        }

        // 4. 关闭
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
