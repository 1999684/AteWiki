using AtelierWiki.Components.Dock;
using AtelierWiki.Data;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AtelierWiki.Pages.A11
{
    public partial class A11MainFrame : Page
    {
        public ObservableCollection<DockItemData> DockItems { get; set; }
        public ICommand NavigateCommand { get; set; }

        public A11MainFrame()
        {
            InitializeComponent();

            NavigateCommand = new RelayCommand<DockItemData>(OnNavigate);

            DockItems = new ObservableCollection<DockItemData>
            {
                new DockItemData { Label = "素材", TargetPage = "A11Material", IconPath = "/Assets/Icon/material.png" },
                new DockItemData { Label = "特性", TargetPage = "A11Feature", IconPath = "/Assets/Icon/feature.png" },
                //new DockItemData { Label = "怪物", TargetPage = "A11Monster", IconPath = "/Assets/Icon/monster.png" },
                new DockItemData { Label = "調合", TargetPage = "A11Harmony", IconPath = "/Assets/Icon/harmony.png" },
                new DockItemData { Label = "主页", TargetPage = "Back", IconPath = "/Assets/Icon/home.png" }
            };

            MainDock.Items = DockItems;
            MainDock.ItemClickCommand = NavigateCommand;

            ContentFrame.Navigate(new A11MaterialPage());
        }

        private void OnNavigate(DockItemData item)
        {
            if (item == null) return;

            switch (item.TargetPage)
            {
                case "A11Material":
                    ContentFrame.Navigate(new A11MaterialPage());
                    break;
                case "A11Feature":
                    ContentFrame.Navigate(new A11FeaturePage());
                    break;
                case "A11Monster":
                    // ContentFrame.Navigate(new MapPage());
                    MessageBox.Show("怪物");
                    break;
                case "A11Harmony":
                    ContentFrame.Navigate(new A11HarmonyPage());
                    break;
                case "Back":
                    NavigationService.Navigate(new Uri("/Pages/Home/HomePage.xaml", UriKind.Relative));
                    break;
                default:
                    break;
            }
        }
    }
}
