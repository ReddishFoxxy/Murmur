using System;
using System.Linq;
using System.Windows;

namespace PolZhelnov
{
    public partial class Vrachi : Window
    {
        private Entities entities = new Entities();
        private Doc selectedDoc;

        public Vrachi()
        {
            InitializeComponent();
            LoadDoctors();
        }

        private void LoadDoctors()
        {
            var doctors = entities.Doc.ToList();
            SpisokDoc.ItemsSource = doctors;
        }

        private void SpisokDoc_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedDoc = SpisokDoc.SelectedItem as Doc;

            if (selectedDoc != null)
            {
                BoxFIO.Text = selectedDoc.FIODoc;
                BoxSpec.Text = selectedDoc.kodSpec;
                BoxBirth.Text = selectedDoc.birthDoc.ToString("dd.MM.yyyy");
                BoxPhone.Text = selectedDoc.PhoneDoc.ToString();
                BoxKab.Text = selectedDoc.Kabinet.ToString();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BoxFIO.Text))
            {
                MessageBox.Show("Введите ФИО врача!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BoxSpec.Text))
            {
                MessageBox.Show("Укажите специальность!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DateTime.TryParseExact(BoxBirth.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime birth))
            {
                MessageBox.Show("Дата рождения должна быть в формате ДД.ММ.ГГГГ (например: 12.05.1985)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(BoxPhone.Text, out int phone))
            {
                MessageBox.Show("Телефон должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(BoxKab.Text, out int kabinet))
            {
                MessageBox.Show("Кабинет должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedDoc == null)
            {
                selectedDoc = new Doc
                {
                    FIODoc = BoxFIO.Text,
                    kodSpec = BoxSpec.Text,
                    birthDoc = birth,
                    PhoneDoc = phone,
                    Kabinet = kabinet
                };
                entities.Doc.Add(selectedDoc);
            }
            else
            {
                selectedDoc.FIODoc = BoxFIO.Text;
                selectedDoc.kodSpec = BoxSpec.Text;
                selectedDoc.birthDoc = birth;
                selectedDoc.PhoneDoc = phone;
                selectedDoc.Kabinet = kabinet;
            }

            try
            {
                entities.SaveChanges();
                LoadDoctors();
                MessageBox.Show("Данные врача сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedDoc == null)
            {
                MessageBox.Show("Выберите врача для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить врача '{selectedDoc.FIODoc}'?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    entities.Doc.Remove(selectedDoc);
                    entities.SaveChanges();
                    LoadDoctors();
                    ClearForm();
                    selectedDoc = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
            selectedDoc = null;
            SpisokDoc.SelectedIndex = -1;
        }

        private void ClearForm()
        {
            BoxFIO.Clear();
            BoxSpec.Clear();
            BoxBirth.Text = "дд.мм.гггг";
            BoxPhone.Clear();
            BoxKab.Clear();
        }
    }
}