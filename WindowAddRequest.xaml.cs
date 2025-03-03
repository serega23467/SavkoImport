using Import.DB;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace Import
{
    /// <summary>
    /// Логика взаимодействия для WindowAddRequest.xaml
    /// </summary>
    public partial class WindowAddRequest : Window
    {
        Roles uRole;
        public WindowAddRequest(Users user, Roles role)
        {
            InitializeComponent();

            uRole = role;
            List<string> listClients = new List<string>();
            List<string> listStatuses = new List<string>();

            if(role.Name == "Заказчик")
            {
                listClients.Add(user.Name);
                comboBoxStatus.IsEnabled = false;              
            }
            else
            {
                using (ImportAbrEntities db= new ImportAbrEntities())
                {
                    listClients.AddRange(db.Users.Select(u => u.Login).ToList());
                    listStatuses.AddRange(db.Statuses.Select(s=>s.Name).ToList());
                }
            }

            comboBoxClient.ItemsSource = listClients;
            comboBoxStatus.ItemsSource = listStatuses;

            comboBoxClient.SelectedIndex = 0;
            comboBoxStatus.SelectedIndex = 0;

        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            using (ImportAbrEntities db = new ImportAbrEntities())
            {
                if(!int.TryParse(textBoxId.Text, out int id) || db.Requests.FirstOrDefault(r=>r.Id == id) != null)
                {
                    MessageBox.Show("Id используется или не подходит", "Ошибка");
                    return;
                }
                if(datePicker.Text == string.Empty)
                {
                    MessageBox.Show("Неправильная дата", "Ошибка");
                    return;
                }
                if(textBoxType.Text == string.Empty)
                {
                    MessageBox.Show("Введите тип", "Ошибка");
                    return;
                }
                if(textBoxModel.Text == string.Empty)
                {
                    MessageBox.Show("Введите модель", "Ошибка");
                    return;
                }
                if(textBoxDesc.Text == string.Empty)
                {
                    MessageBox.Show("Введите описание", "Ошибка");
                    return;
                }
                if(comboBoxClient.SelectedItem == null)
                {
                    MessageBox.Show("Не выбран клиент", "Ошибка");
                    return;
                }
                else if (comboBoxStatus.SelectedItem == null && uRole.Name != "Заказчик")
                {
                    MessageBox.Show("Не выбран статус заявки", "Ошибка");
                    return;
                }

                Requests request = new Requests();
                request.Id = id;
                request.StartDate = DateTime.Parse(datePicker.Text);
                request.TechType = textBoxType.Text;
                request.TechModel = textBoxModel.Text;
                request.Description = textBoxDesc.Text;
                request.ClientId = db.Users.FirstOrDefault(u => u.Login == comboBoxClient.SelectedItem).Id;
                if(uRole.Name == "Заказчик")
                    request.StatusId = db.Statuses.FirstOrDefault(s => s.Name == "Новая заявка").Id;
                else
                    request.StatusId = db.Statuses.FirstOrDefault(s=>s.Name==comboBoxStatus.SelectedItem).Id;

                db.Requests.Add(request);
                db.SaveChanges();
                MessageBox.Show("Заявка успешно добавлена", "Успех");
                Close();
            }
        }
    }
}
