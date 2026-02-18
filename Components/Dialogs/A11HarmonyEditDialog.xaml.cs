using AtelierWiki.Data.A11;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AtelierWiki.Components.Dialogs
{
    public partial class A11HarmonyEditDialog : Window
    {
        public Harmony Result { get; private set; }
        private string _selectedImagePath = "";

        public A11HarmonyEditDialog()
        {
            InitializeComponent();
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                TxtImgPath.Text = Path.GetFileName(_selectedImagePath);

                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(_selectedImagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImgPreview.Source = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("图片加载失败: " + ex.Message);
                }
            }
        }

        private void BtnClearImage_Click(object sender, RoutedEventArgs e)
        {
            _selectedImagePath = "";
            TxtImgPath.Text = "No file selected";
            ImgPreview.Source = null;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text))
            {
                MessageBox.Show("名称不能为空！");
                return;
            }

            int level = 0;
            int.TryParse(TxtLevel.Text, out level);

            string finalImagePath = "";
            if (!string.IsNullOrEmpty(_selectedImagePath))
            {
                try
                {
                    string targetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Pages", "A11", "Img");
                    if (!Directory.Exists(targetDir))
                    {
                        Directory.CreateDirectory(targetDir);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(_selectedImagePath);
                    string destPath = Path.Combine(targetDir, fileName);
                    File.Copy(_selectedImagePath, destPath, true);

                    finalImagePath = "Pages/A11/Img/" + fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存图片失败: " + ex.Message);
                }
            }

            Result = new Harmony
            {
                Name = TxtName.Text.Trim(),
                Level = level,
                Mat1 = TxtMat1.Text.Trim(),
                Mat2 = TxtMat2.Text.Trim(),
                Mat3 = TxtMat3.Text.Trim(),
                Mat4 = TxtMat4.Text.Trim(),
                Category = TxtCategory.Text.Trim(),
                Acquisition = TxtAcquisition.Text.Trim(),
                BgImagePath = finalImagePath
            };

            this.DialogResult = true;
        }
    }
}
