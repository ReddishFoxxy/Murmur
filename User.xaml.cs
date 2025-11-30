using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PolZhelnov
{
    public partial class User : Window
    {
        private Entities entities = new Entities();
        private List<Zapis> _allRecords;
        private ICollectionView _view;
        private string _currentSearch = "";
        private bool _isAscending = true;

        public User()
        {
            InitializeComponent();
            _allRecords = new List<Zapis>();
            LoadRecords();
            ApplyFilterAndSort();
        }

        private void LoadRecords()
        {
            try
            {
                _allRecords = entities.Zapis
                    .Include("Pacient")
                    .Include("Doc")
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки записей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                _allRecords = new List<Zapis>();
            }
        }

        private void ApplyFilterAndSort()
        {
            if (_allRecords == null) _allRecords = new List<Zapis>();

            var filtered = _allRecords.AsQueryable();


            if (!string.IsNullOrWhiteSpace(_currentSearch))
            {
                string term = _currentSearch.Trim().ToLower();
                filtered = filtered.Where(z =>
                    z.Pacient != null &&
                    !string.IsNullOrEmpty(z.Pacient.FIOPacient) &&
                    z.Pacient.FIOPacient.ToLower().Contains(term));
            }

            var inMemoryList = filtered.ToList();

            var sorted = _isAscending
                ? inMemoryList.OrderBy(z => z.Pacient?.FIOPacient ?? "")
                : inMemoryList.OrderByDescending(z => z.Pacient?.FIOPacient ?? "");

            _view = CollectionViewSource.GetDefaultView(sorted);
            ZapisiList.ItemsSource = _view;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentSearch = SearchBox.Text;
            ApplyFilterAndSort();
        }

        private void SortAsc_Click(object sender, RoutedEventArgs e)
        {
            _isAscending = true;
            ApplyFilterAndSort();
        }

        private void SortDesc_Click(object sender, RoutedEventArgs e)
        {
            _isAscending = false;
            ApplyFilterAndSort();
        }

        private void ZapisiList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var select = ZapisiList.SelectedItem as Zapis;
            if (select != null)
            {
                DateTimeZap.Text = select.datetimeZapis.ToString("dd.MM.yyyy HH:mm");
                var doc = select.Doc;
                comboDoci.Text = doc != null
                    ? $"{doc.FIODoc} — каб. {doc.Kabinet}"
                    : "Врач не найден";
            }
            else
            {
                DateTimeZap.Clear();
                comboDoci.Clear();
            }
        }
    }
}