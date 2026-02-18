using AtelierWiki.Components.Dialogs;
using AtelierWiki.Data.A11;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;

namespace AtelierWiki.ViewModels
{
    public class A11HarmonyViewModel : ObservableObject
    {
        private ObservableCollection<Harmony> _displayList = new ObservableCollection<Harmony>();
        public ObservableCollection<Harmony> DisplayList
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

        public A11HarmonyViewModel()
        {
            // 确保数据库已初始化
            A11DbManager.Initialize();
            LoadData();
        }

        private void LoadData()
        {
            var list = string.IsNullOrWhiteSpace(SearchText)
                ? A11DbManager.GetAllHarmonies()
                : A11DbManager.SearchHarmonies(SearchText);

            DisplayList.Clear();
            foreach (var item in list)
            {
                DisplayList.Add(item);
            }

            if (IsEditMode)
            {
                DisplayList.Add(new Harmony { IsAddButton = true, Name = "ADD_NEW" });
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
                        var dialog = new A11HarmonyEditDialog();
                        if (dialog.ShowDialog() == true)
                        {
                            A11DbManager.AddHarmony(dialog.Result);
                            LoadData();
                        }
                    });
                }
                return _addNewCommand;
            }
        }

        private RelayCommand<Harmony> _deleteItemCommand;
        public RelayCommand<Harmony> DeleteItemCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                {
                    _deleteItemCommand = new RelayCommand<Harmony>((item) =>
                    {
                        if (item == null) return;
                        var result = MessageBox.Show($"确认删除【{item.Name}】吗？", "删除确认", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Yes)
                        {
                            A11DbManager.DeleteHarmony(item.Id);
                            LoadData();
                        }
                    });
                }
                return _deleteItemCommand;
            }
        }

        private RelayCommand<Harmony> _showDetailCommand;
        public RelayCommand<Harmony> ShowDetailCommand
        {
            get
            {
                if (_showDetailCommand == null)
                {
                    _showDetailCommand = new RelayCommand<Harmony>((item) =>
                    {
                        if (item == null) return;
                        if (IsEditMode || item.IsAddButton) return;

                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine($"【{item.Name}】 (Lv.{item.Level})");
                        sb.AppendLine($"类别: {item.Category}");
                        sb.AppendLine();

                        sb.AppendLine("[所需材料]");
                        if (!string.IsNullOrEmpty(item.Mat1)) sb.AppendLine($"- {item.Mat1}");
                        if (!string.IsNullOrEmpty(item.Mat2)) sb.AppendLine($"- {item.Mat2}");
                        if (!string.IsNullOrEmpty(item.Mat3)) sb.AppendLine($"- {item.Mat3}");
                        if (!string.IsNullOrEmpty(item.Mat4)) sb.AppendLine($"- {item.Mat4}");

                        if (!string.IsNullOrEmpty(item.Acquisition))
                        {
                            sb.AppendLine();
                            sb.AppendLine("[习得方式]");
                            sb.AppendLine(item.Acquisition);
                        }

                        A11TechDialog.Show(sb.ToString());
                    });
                }
                return _showDetailCommand;
            }
        }
    }
}
