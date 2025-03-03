using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
using Import.DB;

namespace Import
{
    /// <summary>
    /// Логика взаимодействия для WindowRequests.xaml
    /// </summary>
    public partial class WindowRequests : Window
    {
        Users currentUser;
        Roles currentRole;
        public WindowRequests(Users user)
        {
            InitializeComponent();
            currentUser = user;
            using (ImportAbrEntities db = new ImportAbrEntities())
            {
                var role = db.Roles.FirstOrDefault(r => r.Id == user.RoleId);
                currentRole = role;
                switch (role.Name)
                {
                    case "Мастер":
                    case "Оператор":
                        buttonAdd.IsEnabled = false;
                        break;
                    case "Менеджер":
                        break;
                    case "Заказчик":
                        buttonEdit.IsEnabled = false;
                        break;
                }
            }
            UpdateDataGrid();
        }
        void UpdateDataGrid()
        {
            dataGrid.ItemsSource = null;
            List<RequestsDG> requests = new List<RequestsDG>();
            using (ImportAbrEntities db = new ImportAbrEntities())
            {
                var dbReqs = db.Requests.ToList();
                if(currentRole.Name == "Заказчик")
                {
                    dbReqs = db.Requests.Where(u=>u.ClientId == currentUser.Id).ToList();
                }
                else if (currentRole.Name == "Мастер" || currentRole.Name == "Оператор")
                {
                    dbReqs = db.Requests.Where(u => u.MasterId == currentUser.Id).ToList();
                }
                foreach(Requests dbReq in dbReqs)
                {
                    var status = db.Statuses.FirstOrDefault(s=>s.Id==dbReq.StatusId).Name;
                    var client = db.Users.FirstOrDefault(u => u.Id == dbReq.ClientId).Login;
                    var master = db.Users.FirstOrDefault(u => u.Id == dbReq.MasterId);

                    RequestsDG req = new RequestsDG();
                    req.Id = dbReq.Id;
                    req.StartDate = dbReq.StartDate;
                    req.Comments = dbReq.Comments;
                    req.CompletionDate = dbReq.CompletionDate;
                    req.Description = dbReq.Description;
                    req.RepairParts = dbReq.RepairParts;
                    req.TechType = dbReq.TechType;
                    req.TechModel = dbReq.TechModel;
                    req.Client = client;
                    req.Status = status;
                    if (master != null)
                    {
                        req.Master = master.Login;
                        req.Comment = db.Comments.FirstOrDefault(c => c.MasterId == master.Id && c.RequestId == dbReq.Id).Message;
                    }
                    requests.Add(req);
                }
                int completedCount = db.Requests.Where(r=>r.StatusId==db.Statuses.FirstOrDefault(s=>s.Name=="Готова к выдаче").Id).Count();
                var requestsM = db.Requests.Where(r=>r.CompletionDate!=null);
                int days = 0;
                foreach(var r in requestsM)
                {
                    DateTime comp = (DateTime)r.CompletionDate;
                    TimeSpan difference = comp.Subtract(r.StartDate);
                    int daysDifference = (int)difference.TotalDays;

                    days += daysDifference;
                }
                float average = (float) days / (float)completedCount;

                textBlockStats.Text = $"колво выполненых заявок: {completedCount}\nсреднее время в днях: {average}";

            }
            dataGrid.ItemsSource = requests;

        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowAddRequest windowAddRequest = new WindowAddRequest(currentUser, currentRole);
            windowAddRequest.ShowDialog();
            UpdateDataGrid();
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dataGrid.SelectedItem as RequestsDG;
            if (selected == null)
            {
                MessageBox.Show("Выберите строку", "Ошибка");
                return;
            }
            using (ImportAbrEntities db = new ImportAbrEntities())
            {
                Requests request = db.Requests.FirstOrDefault(r => r.Id == selected.Id);
                if (request != null)
                {
                    WindowEditRequest windowEdit = new WindowEditRequest(request, currentUser, currentRole);
                    windowEdit.ShowDialog();
                    UpdateDataGrid();
                }
            }

        }

        private void textBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(textBoxSearch.Text==string.Empty)
            {
                UpdateDataGrid();
            }

            dataGrid.ItemsSource = null;
            List<RequestsDG> requests = new List<RequestsDG>();
            using (ImportAbrEntities db = new ImportAbrEntities())
            {
                var dbReqs = db.Requests.Where(t=> t.Id.ToString().Contains(textBoxSearch.Text) 
                || t.Description.ToLower().Contains(textBoxSearch.Text.ToLower()) 
                || t.TechModel.ToLower().Contains(textBoxSearch.Text.ToLower())
                || t.TechType.ToLower().Contains(textBoxSearch.Text.ToLower())).ToList();
                if (currentRole.Name == "Заказчик")
                {
                    dbReqs = dbReqs.Where(u => u.ClientId == currentUser.Id).ToList();
                }
                else if (currentRole.Name == "Мастер" || currentRole.Name == "Оператор")
                {
                    dbReqs = dbReqs.Where(u => u.MasterId == currentUser.Id).ToList();
                }
                foreach (Requests dbReq in dbReqs)
                {
                    var status = db.Statuses.FirstOrDefault(s => s.Id == dbReq.StatusId).Name;
                    var client = db.Users.FirstOrDefault(u => u.Id == dbReq.ClientId).Login;
                    var master = db.Users.FirstOrDefault(u => u.Id == dbReq.MasterId);

                    RequestsDG req = new RequestsDG();
                    req.Id = dbReq.Id;
                    req.StartDate = dbReq.StartDate;
                    req.Comments = dbReq.Comments;
                    req.CompletionDate = dbReq.CompletionDate;
                    req.Description = dbReq.Description;
                    req.RepairParts = dbReq.RepairParts;
                    req.TechType = dbReq.TechType;
                    req.TechModel = dbReq.TechModel;
                    req.Client = client;
                    req.Status = status;
                    if (master != null)
                        req.Master = master.Login;
                    requests.Add(req);
                }
            }
            dataGrid.ItemsSource = requests;
        }
    }
}
