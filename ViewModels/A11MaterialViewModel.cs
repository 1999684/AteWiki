using AtelierWiki.Components.Dialogs;
using AtelierWiki.Data.A11;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace AtelierWiki.ViewModels
{
    public class A11MaterialViewModel : ObservableObject
    {
        private ObservableCollection<Material> _displayList = new ObservableCollection<Material>();
        public ObservableCollection<Material> DisplayList
        {
            get => _displayList;
            set => SetProperty(ref _displayList, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    LoadData();
                }
            }
        }

        private bool _isEditMode = false;
        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                if (SetProperty(ref _isEditMode, value))
                {
                    LoadData();
                }
            }
        }

        public A11MaterialViewModel()
        {
            // 初始化数据库
            A11DbManager.Initialize();
            LoadData();
        }

        private void LoadData()
        {
            var list = string.IsNullOrWhiteSpace(SearchText)
                ? A11DbManager.GetAllMaterials()
                : A11DbManager.SearchMaterials(SearchText);

            DisplayList.Clear();
            foreach (var item in list)
            {
                DisplayList.Add(item);
            }

            if (IsEditMode)
            {
                DisplayList.Add(new Material { IsAddButton = true, Name = "ADD_NEW" });
            }
        }

        private RelayCommand _toggleEditCommand;
        public RelayCommand ToggleEditCommand
        {
            get
            {
                if (_toggleEditCommand == null)
                {
                    _toggleEditCommand = new RelayCommand(() =>
                    {
                        IsEditMode = !IsEditMode;
                    });
                }
                return _toggleEditCommand;
            }
        }

        private RelayCommand _addNewCommand;
        public RelayCommand AddNewCommand
        {
            get
            {
                if (_addNewCommand == null)
                {
                    _addNewCommand = new RelayCommand(() =>
                    {
                        var dialog = new A11MaterialEditDialog();
                        if (dialog.ShowDialog() == true)
                        {
                            A11DbManager.AddMaterial(dialog.Result);
                            LoadData();
                        }
                    });
                }
                return _addNewCommand;
            }
        }

        private RelayCommand<Material> _deleteItemCommand;
        public RelayCommand<Material> DeleteItemCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                {
                    _deleteItemCommand = new RelayCommand<Material>((item) =>
                    {
                        if (item == null) return;
                        var result = MessageBox.Show($"确认删除【{item.Name}】吗？", "删除确认", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            A11DbManager.DeleteMaterial(item.Id);
                            LoadData();
                        }
                    });
                }
                return _deleteItemCommand;
            }
        }

        private RelayCommand<Material> _showDetailCommand;
        public RelayCommand<Material> ShowDetailCommand
        {
            get
            {
                if (_showDetailCommand == null)
                {
                    _showDetailCommand = new RelayCommand<Material>((item) =>
                    {
                        if (item == null) return;
                        if (IsEditMode || item.IsAddButton) return;

                        string info = $"【{item.Name}】 (Lv.{item.Level})\n\n[采集地]\n{item.Locations}\n\n[怪物掉落]\n{item.Monsters}";
                        A11TechDialog.Show(info);
                    });
                }
                return _showDetailCommand;
            }
        }
    }
}
