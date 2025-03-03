using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Import.DB;

namespace Import
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            using(ImportAbrEntities db = new ImportAbrEntities())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == textBoxLogin.Text && u.Password == passwordBox.Password);
                if (user != null)
                {
                    MessageBox.Show("Успешный вход!", "Успех");
                    WindowRequests windowRequests = new WindowRequests(user);
                    windowRequests.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль!", "Ошибка");
                }
            }
        }
    }
}
