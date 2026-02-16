using CommunityToolkit.Mvvm.ComponentModel;
using MiniExcelLibs;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace AtelierWiki.ViewModels
{
    public class ExcelViewModel : ObservableObject
    {
        private Dictionary<string, DataTable> _allSheetsData = new Dictionary<string, DataTable>();

        private ObservableCollection<string> _sheetNames = new ObservableCollection<string>();
        public ObservableCollection<string> SheetNames
        {
            get => _sheetNames;
            set => SetProperty(ref _sheetNames, value);
        }

        private string _selectedSheetName;
        public string SelectedSheetName
        {
            get => _selectedSheetName;
            set
            {
                if (SetProperty(ref _selectedSheetName, value))
                {
                    if (!string.IsNullOrEmpty(value) && _allSheetsData.ContainsKey(value))
                    {
                        SearchText = string.Empty; // 清空搜索框
                        CurrentDataTable = _allSheetsData[value].DefaultView; // 切换显示的数据
                    }
                }
            }
        }

        private DataView _currentDataTable;
        public DataView CurrentDataTable
        {
            get => _currentDataTable;
            set => SetProperty(ref _currentDataTable, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilter();
                }
            }
        }

        public void LoadExcel(string filePath)
        {
            if (!File.Exists(filePath)) return;

            var sheetNames = MiniExcel.GetSheetNames(filePath);

            _allSheetsData.Clear();
            SheetNames.Clear();

            foreach (var sheetName in sheetNames)
            {
                var table = MiniExcel.QueryAsDataTable(filePath, useHeaderRow: true, sheetName: sheetName);
                _allSheetsData.Add(sheetName, table);
                SheetNames.Add(sheetName);
            }

            if (SheetNames.Count > 0)
            {
                SelectedSheetName = SheetNames[0];
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrEmpty(SelectedSheetName) || !_allSheetsData.ContainsKey(SelectedSheetName))
                return;

            var table = _allSheetsData[SelectedSheetName];
            var view = table.DefaultView;

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                view.RowFilter = string.Empty; // 清除过滤
            }
            else
            {
                string safeSearch = SearchText.Replace("'", "''")
                                              .Replace("[", "[[]")
                                              .Replace("%", "[%]")
                                              .Replace("*", "[*]");

                List<string> filters = new List<string>();
                foreach (DataColumn col in table.Columns)
                {
                    filters.Add($"Convert([{col.ColumnName}], 'System.String') LIKE '%{safeSearch}%'");
                }

                if (filters.Count > 0)
                {
                    view.RowFilter = string.Join(" OR ", filters);
                }
            }

            CurrentDataTable = view;
        }


        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
        }
    }
}
