using AtelierWiki.Data.A11;
using AtelierWiki.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AtelierWiki.ViewModels
{
    public class A11FeatureViewModel : ObservableObject
    {
        private ObservableCollection<Feature> _displayList;
        public ObservableCollection<Feature> DisplayList
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
                    // 搜索改为异步调用
                    LoadDataAsync();
                }
            }
        }

        private bool _isEditMode;
        public bool IsEditMode
        {
            get => _isEditMode;
            set => SetProperty(ref _isEditMode, value);
        }

        public A11FeatureViewModel()
        {
            DisplayList = new ObservableCollection<Feature>();

            // 确保数据库存在 (如果是第一次运行)
            A11DbManager.Initialize();

            // 构造函数中调用异步方法 (Fire and Forget)
            LoadDataAsync();
        }

        // === 核心优化：异步加载数据 ===
        private async void LoadDataAsync()
        {
            // 1. 在后台线程读取数据库，此时 UI 不会卡死
            List<Feature> list;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                list = await A11DbManager.GetAllFeaturesAsync();
            }
            else
            {
                list = await A11DbManager.SearchFeaturesAsync(SearchText);
            }

            // 2. 清理旧数据并解除事件绑定 (防止内存泄漏)
            if (DisplayList != null)
            {
                foreach (var item in DisplayList)
                {
                    item.PropertyChanged -= OnItemPropertyChanged;
                }
            }

            // 3. 准备新数据 (在内存中操作，不影响 UI)
            var newList = new ObservableCollection<Feature>();
            if (list != null)
            {
                foreach (var item in list)
                {
                    item.PropertyChanged += OnItemPropertyChanged; // 重新绑定事件
                    newList.Add(item);
                }
            }

            // 4. 一次性更新 UI 集合 (触发一次 CollectionChanged，而不是 200 次)
            DisplayList = newList;
        }

        // === 属性变更监听器 (自动保存) ===
        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Feature item)
            {
                // 过滤辅助属性 (不存库的属性)
                if (e.PropertyName == nameof(Feature.SynthesizeDisplay) ||
                    e.PropertyName == nameof(Feature.AttackDisplay) ||
                    e.PropertyName == nameof(Feature.HealDisplay) ||
                    e.PropertyName == nameof(Feature.WeaponDisplay) ||
                    e.PropertyName == nameof(Feature.ArmorDisplay) ||
                    e.PropertyName == nameof(Feature.AccessoryDisplay))
                {
                    return;
                }

                // 执行数据库更新 (这里保持同步即可，单条更新很快)
                A11DbManager.UpdateFeature(item);
            }
        }

        // --- Commands (兼容 C# 7.3) ---

        private RelayCommand _toggleEditCommand;
        public ICommand ToggleEditCommand
        {
            get
            {
                if (_toggleEditCommand == null)
                {
                    _toggleEditCommand = new RelayCommand(() => IsEditMode = !IsEditMode);
                }
                return _toggleEditCommand;
            }
        }

        private RelayCommand _addNewCommand;
        public ICommand AddNewCommand
        {
            get
            {
                if (_addNewCommand == null)
                {
                    _addNewCommand = new RelayCommand(() =>
                    {
                        var newItem = new Feature { Name = "新特性", Cost = 0 };

                        // 存入库以获取 ID
                        A11DbManager.AddFeature(newItem);

                        // 绑定事件
                        newItem.PropertyChanged += OnItemPropertyChanged;

                        // 插入到列表头部
                        DisplayList.Insert(0, newItem);
                    });
                }
                return _addNewCommand;
            }
        }

        private RelayCommand<Feature> _deleteItemCommand;
        public ICommand DeleteItemCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                {
                    _deleteItemCommand = new RelayCommand<Feature>((item) =>
                    {
                        if (item == null) return;
                        if (MessageBox.Show($"確定刪除【{item.Name}】嗎？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            A11DbManager.DeleteFeature(item.Id);

                            item.PropertyChanged -= OnItemPropertyChanged;
                            DisplayList.Remove(item);
                        }
                    });
                }
                return _deleteItemCommand;
            }
        }
    }
}
