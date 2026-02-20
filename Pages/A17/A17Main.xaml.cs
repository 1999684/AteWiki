using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Web.WebView2.Core;

namespace AtelierWiki.Pages.A17
{
    public partial class A17Main : Page
    {
        public A17Main()
        {
            InitializeComponent();
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await WebBrowser.EnsureCoreWebView2Async();

            WebBrowser.WebMessageReceived += WebBrowser_WebMessageReceived;

            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string htmlPath = Path.Combine(appPath, "WebAssets", "A17", "index.html");

            if (File.Exists(htmlPath))
            {
                WebBrowser.CoreWebView2.Navigate(htmlPath);
            }
            else
            {
                MessageBox.Show($"文件不存在：{htmlPath}");
            }

            WebBrowser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebBrowser.CoreWebView2.Settings.AreDevToolsEnabled = true;
            WebBrowser.CoreWebView2.Settings.IsStatusBarEnabled = false;
        }

        private void WebBrowser_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString();

            if (message == "NavigateToHome")
            {
                if (this.NavigationService != null)
                {
                    this.NavigationService.Navigate(new Uri("/Pages/Home/HomePage.xaml", UriKind.Relative));
                }
            }
        }
    }
}
