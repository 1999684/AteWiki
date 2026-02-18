using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation; // 必须引用，用于 NavigationService
using Microsoft.Web.WebView2.Core;

namespace AtelierWiki.Pages.A12
{
    public partial class A12Main : Page
    {
        public A12Main()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            // 1. 初始化环境
            await WebBrowser.EnsureCoreWebView2Async();

            // 2. [关键] 订阅 JS 发来的消息事件
            WebBrowser.WebMessageReceived += WebBrowser_WebMessageReceived;

            // 3. 配置路径
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            // 注意：请确保你的 HTML 文件确实在这个路径下
            string htmlPath = Path.Combine(appPath, "WebAssets", "A12", "index.html");

            // 4. 加载页面
            if (File.Exists(htmlPath))
            {
                WebBrowser.CoreWebView2.Navigate(htmlPath);
            }
            else
            {
                MessageBox.Show($"文件不存在：{htmlPath}");
            }

            // 5. 设置浏览器选项
            WebBrowser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebBrowser.CoreWebView2.Settings.AreDevToolsEnabled = true;
            WebBrowser.CoreWebView2.Settings.IsStatusBarEnabled = false;
        }

        // [关键] 处理来自 HTML 的消息
        private void WebBrowser_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // 获取 HTML 发过来的字符串
            string message = e.TryGetWebMessageAsString();

            // 判断暗号是否匹配
            if (message == "NavigateToHome")
            {
                // 执行 WPF 跳转逻辑
                if (this.NavigationService != null)
                {
                    this.NavigationService.Navigate(new Uri("/Pages/Home/HomePage.xaml", UriKind.Relative));
                }
            }
        }
    }
}
