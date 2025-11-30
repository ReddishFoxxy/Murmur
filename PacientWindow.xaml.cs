using System;
using System.Windows;
using System.Linq;

namespace PolZhelnov
{
    public partial class PacientWindow : Window
    {
        private Entities entities = new Entities();
        private Pacient selectedPacient;

        public PacientWindow()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void LoadPatients()
        {
            var patients = entities.Pacient.ToList();
            PacientList.ItemsSource = patients;
        }

        private void PacientList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            selectedPacient = PacientList.SelectedItem as Pacient;

            if (selectedPacient != null)
            {
                TxtFIO.Text = selectedPacient.FIOPacient;
                TxtSnils.Text = selectedPacient.Snils.ToString();
                TxtBirth.Text = selectedPacient.Birth.ToString("dd.MM.yyyy");
                TxtPhone.Text = selectedPacient.Phone.ToString();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFIO.Text))
            {
                MessageBox.Show("Введите ФИО пациента!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtSnils.Text, out int snils))
            {
                MessageBox.Show("СНИЛС должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DateTime.TryParseExact(TxtBirth.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime birthDate))
            {
                MessageBox.Show("Дата рождения должна быть в формате ДД.ММ.ГГГГ (например: 15.08.1990)!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(TxtPhone.Text, out int phone))
            {
                MessageBox.Show("Телефон должен быть числом!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedPacient == null)
            {
                selectedPacient = new Pacient
                {
                    FIOPacient = TxtFIO.Text,
                    Snils = snils,
                    Birth = birthDate,
                    Phone = phone
                };
                entities.Pacient.Add(selectedPacient);
            }
            else
            {
                selectedPacient.FIOPacient = TxtFIO.Text;
                selectedPacient.Snils = snils;
                selectedPacient.Birth = birthDate;
                selectedPacient.Phone = phone;
            }

            try
            {
                entities.SaveChanges();
                LoadPatients();
                MessageBox.Show("Данные сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPacient == null)
            {
                MessageBox.Show("Выберите пациента для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Удалить пациента '{selectedPacient.FIOPacient}'?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    entities.Pacient.Remove(selectedPacient);
                    entities.SaveChanges();
                    LoadPatients();
                    ClearForm();
                    selectedPacient = null;
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
            selectedPacient = null;
            PacientList.SelectedIndex = -1;
        }

        private void ClearForm()
        {
            TxtFIO.Clear();
            TxtSnils.Clear();
            TxtBirth.Clear();
            TxtPhone.Clear();
        }
    }
}