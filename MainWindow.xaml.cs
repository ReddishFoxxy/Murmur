using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PolZhelnov
{
    public partial class MainWindow : Window
    {
        private Entities entities = new Entities();
        private List<Zapis> _allRecords;
        private ICollectionView _view;
        private string _currentSearch = "";
        private bool _isAscending = true;

        public MainWindow()
        {
            InitializeComponent();
            _allRecords = new List<Zapis>();
            LoadData();
            ApplyFilterAndSort();
        }

        private void LoadData()
        {
            try
            {
                _allRecords = entities.Zapis
                    .Include("Pacient")
                    .Include("Doc")
                    .ToList();

                var doctors = entities.Doc.ToList();
                comboDoci.ItemsSource = doctors;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    z.Doc != null &&
                    !string.IsNullOrEmpty(z.Doc.FIODoc) &&
                    z.Doc.FIODoc.ToLower().Contains(term));
            }


            var inMemoryList = filtered.ToList();


            var sorted = _isAscending
                ? inMemoryList.OrderBy(z => z.Doc?.FIODoc ?? "")
                : inMemoryList.OrderByDescending(z => z.Doc?.FIODoc ?? "");

            _view = CollectionViewSource.GetDefaultView(sorted.ToList());
            ZapisiList.ItemsSource = _view;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentSearch = SearchBox.Text;
            ApplyFilterAndSort();
        }

        private void SortDocAsc_Click(object sender, RoutedEventArgs e)
        {
            _isAscending = true;
            ApplyFilterAndSort();
        }

        private void SortDocDesc_Click(object sender, RoutedEventArgs e)
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
                comboDoci.SelectedItem = select.Doc;
                TextPac.Text = select.Pacient?.FIOPacient ?? "";
            }
            else
            {
                DateTimeZap.Clear();
                TextPac.Clear();
                comboDoci.SelectedItem = null;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextPac.Text) ||
                string.IsNullOrWhiteSpace(DateTimeZap.Text) ||
                comboDoci.SelectedItem == null)
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var save = ZapisiList.SelectedItem as Zapis;
            var pac = entities.Pacient.FirstOrDefault(p => p.FIOPacient == TextPac.Text);

            if (pac == null)
            {
                MessageBox.Show("Пациент не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (save == null)
            {
                save = new Zapis();
                entities.Zapis.Add(save);
                _allRecords.Add(save);
            }

            try
            {
                save.datetimeZapis = DateTime.ParseExact(DateTimeZap.Text, "dd.MM.yyyy HH:mm", null);
                save.Pacient = pac;
                save.Doc = comboDoci.SelectedItem as Doc;

                entities.SaveChanges();
                LoadData();
                ApplyFilterAndSort();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            TextPac.Clear();
            DateTimeZap.Clear();
            comboDoci.SelectedItem = null;
            ZapisiList.SelectedItem = null;
            DateTimeZap.Focus();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var del = ZapisiList.SelectedItem as Zapis;
            if (del == null)
            {
                MessageBox.Show("Выберите запись для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show("Вы действительно хотите удалить запись?", "Удаление",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                entities.Zapis.Remove(del);
                _allRecords.Remove(del);
                try
                {
                    entities.SaveChanges();
                    ApplyFilterAndSort();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Vrachi(object sender, RoutedEventArgs e)
        {
            new Vrachi().ShowDialog();
        }

        private void Pacienti_Click(object sender, RoutedEventArgs e)
        {
            new PacientWindow().ShowDialog();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            new Vhod().Show();
        }
    }
}