using System.Windows;

namespace AtelierWiki.Components.Dialogs
{
    public partial class TechDialog : Window
    {
        public TechDialog(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // 静态辅助方法，方便调用
        public static void Show(string message)
        {
            var dialog = new TechDialog(message);
            dialog.ShowDialog(); // 模态显示，阻塞后续代码直到关闭
        }
    }
}
