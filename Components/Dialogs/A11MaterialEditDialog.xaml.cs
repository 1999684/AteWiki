using AtelierWiki.Data.A11;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AtelierWiki.Components.Dialogs
{
    public partial class A11MaterialEditDialog : Window
    {
        public Material Result { get; private set; }

        private string _selectedImageSourcePath;

        private bool _isImageCleared = false;

        public A11MaterialEditDialog()
        {
            InitializeComponent();
        }

        private void BtnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImageSourcePath = openFileDialog.FileName;
                _isImageCleared = false;

                ShowPreview(_selectedImageSourcePath);
                TxtImgPath.Text = Path.GetFileName(_selectedImageSourcePath);
            }
        }

        private void BtnClearImage_Click(object sender, RoutedEventArgs e)
        {
            _selectedImageSourcePath = null;
            _isImageCleared = true;
            ImgPreview.Source = null;
            TxtImgPath.Text = "已清除";
        }

        private void ShowPreview(string path)
        {
            try
            {
                if (!Path.IsPathRooted(path))
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path.TrimStart('/', '\\'));
                }

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(path);
                bitmap.EndInit();
                ImgPreview.Source = bitmap;
            }
            catch
            {
                TxtImgPath.Text = "预览失败";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtName.Text)) return;

            int.TryParse(TxtLevel.Text, out int lv);

            string finalDbPath = "";

            if (_isImageCleared)
            {
                finalDbPath = "";
            }
            else if (!string.IsNullOrEmpty(_selectedImageSourcePath))
            {
                finalDbPath = CopyImageToAssets(_selectedImageSourcePath);
            }
            else
            {
                finalDbPath = "";
            }

            Result = new Material
            {
                Name = TxtName.Text,
                Level = lv,
                Locations = TxtLoc.Text,
                Monsters = TxtMonster.Text,
                BgImagePath = finalDbPath
            };

            this.DialogResult = true;
            this.Close();
        }

        private string CopyImageToAssets(string sourcePath)
        {
            try
            {
                string appPath = AppDomain.CurrentDomain.BaseDirectory;
                string targetFolder = Path.Combine(appPath, "Assets", "UserImages");

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                string extension = Path.GetExtension(sourcePath);
                string newFileName = $"{Guid.NewGuid()}{extension}";
                string targetPath = Path.Combine(targetFolder, newFileName);

                File.Copy(sourcePath, targetPath, true);

                return $"/Assets/UserImages/{newFileName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("图片保存失败: " + ex.Message);
                return "";
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
