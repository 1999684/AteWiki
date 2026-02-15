using System;
using System.Windows;
using AtelierWiki.Data; // 确保引用了 Data

namespace AtelierWiki
{
    public partial class App : Application
    {
        // 这个方法名必须和 XAML 里的 Startup="Application_Startup" 完全一致
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 1. 先弹窗，证明程序活过来了
            //MessageBox.Show("程序已启动！正在初始化数据库...");

            try
            {
                // 2. 初始化数据库
                DbManager.Initialize();
                //MessageBox.Show("数据库初始化成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库初始化失败: " + ex.Message);
                // 就算数据库失败，也尝试打开窗口，方便调试
            }

            // 3. 手动创建并显示主窗口
            try
            {
                MainWindow window = new MainWindow();
                window.Show();
                //MessageBox.Show("窗口已显示！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("创建窗口失败: " + ex.Message);
            }
        }
    }
}
