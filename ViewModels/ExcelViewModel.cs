using CommunityToolkit.Mvvm.ComponentModel;
using MiniExcelLibs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;

namespace AtelierWiki.ViewModels
{
    public class ExcelViewModel : ObservableObject
    {
        public event Action<DataRowView> RequestScrollToRow;

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
                        SearchText = string.Empty;
                        var view = _allSheetsData[value].DefaultView;
                        view.RowFilter = string.Empty;
                        CurrentDataTable = view;
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

        private DataRowView _selectedRow;
        public DataRowView SelectedRow
        {
            get => _selectedRow;
            set => SetProperty(ref _selectedRow, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FindAndLocateRow();
                }
            }
        }

        private string _pageTitle;
        public string PageTitle
        {
            get => _pageTitle;
            set => SetProperty(ref _pageTitle, value);
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

        private void FindAndLocateRow()
        {
            if (string.IsNullOrEmpty(SelectedSheetName) ||
                !_allSheetsData.ContainsKey(SelectedSheetName))
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                SelectedRow = null;
                return;
            }

            var table = _allSheetsData[SelectedSheetName];
            var view = table.DefaultView;

            view.RowFilter = string.Empty;

            string keyword = SearchText.ToLower(); 

            foreach (DataRowView rowView in view)
            {
                bool match = false;

                foreach (var item in rowView.Row.ItemArray)
                {
                    if (item != null && item.ToString().ToLower().Contains(keyword))
                    {
                        match = true;
                        break;
                    }
                }

                if (match)
                {
                    SelectedRow = rowView; 

                    RequestScrollToRow?.Invoke(rowView);
                    return;
                }
            }

            SelectedRow = null;
        }
    }
}
