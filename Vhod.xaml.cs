using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PolZhelnov
{
    /// <summary>
    /// Логика взаимодействия для Vhod.xaml
    /// </summary>
    public partial class Vhod : Window
    {
        Entities entities = new Entities();
        public Vhod()
        {
            
            InitializeComponent();
        }
        
private void Avtorizatiya(object sender, RoutedEventArgs e)
        {
        
            string m_aut = "Авторизация";
            string m_error = "Ошибка! Проверьте правильность данных";

            if (textBox_login.Text != "" && textBox_login.Text != null ||
                passwordBox_password.Password != "" && passwordBox_password.Password != null)
            {
                string Login = textBox_login.Text;
                string Password = passwordBox_password.Password;

                bool flag = false;
                foreach (var login in entities.user)
                {
                    if (textBox_login.Text == login.login)
                    {
                        if (passwordBox_password.Password == login.password)
                        {
                            flag = true;
                            if (login.role == "admin")
                            {
                                var window_ = new MainWindow();
                                window_.ShowDialog();
                            }
                            else
                            {
                                var window_ = new User();
                                window_.ShowDialog();
                            }
                            Close();
                            break;
                        }

                    }
                }
                if (!flag)
                {
                    MessageBox.Show(m_error, m_aut, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
                MessageBox.Show(m_error, m_aut, MessageBoxButton.OK, MessageBoxImage.Error);

        }
    }
    

}