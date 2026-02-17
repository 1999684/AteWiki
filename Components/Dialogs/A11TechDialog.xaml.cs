using System.Windows;

namespace AtelierWiki.Components.Dialogs
{
    public partial class A11TechDialog : Window
    {
        public A11TechDialog(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static void Show(string message)
        {
            var dialog = new A11TechDialog(message);
            dialog.ShowDialog();
        }
    }
}
