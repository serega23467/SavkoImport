using Import.DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
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
    /// Логика взаимодействия для WindowEditRequest.xaml
    /// </summary>
    public partial class WindowEditRequest : Window
    {
        Requests rToEdit;
        Users currentUser;
        Roles userRole;
        public WindowEditRequest(Requests request, Users user, Roles role)
        {
            InitializeComponent();

            userRole = role;
            currentUser = user;
            rToEdit = request;

            List<string> listMasters = new List<string>();
            List<string> listStatuses = new List<string>();
            using(ImportAbrEntities db = new ImportAbrEntities())
            {
                if (role.Name != "Мастер" && role.Name != "Оператор")
                {
                    textBoxMessage.IsEnabled = false;
                    textBlockMessage.IsEnabled = false;
                    textBoxParts.IsEnabled = false;
                    textBlockParts.IsEnabled = false;

                    listMasters.Add("Нет");
                    listMasters.AddRange(db.Users.Where(u=>u.RoleId==db.Roles.FirstOrDefault(r=>r.Name=="Мастер").Id || u.RoleId == db.Roles.FirstOrDefault(r => r.Name == "Оператор").Id).Select(u => u.Login).ToList());
                }
                else
                {
                    comboBoxMaster.IsEnabled = false;
                    var comment = db.Comments.FirstOrDefault(c => c.MasterId == currentUser.Id && c.RequestId == rToEdit.Id);
                    if (comment != null)
                        textBoxMessage.Text = comment.Message;
                }

                listStatuses.AddRange(db.Statuses.Select(s => s.Name).ToList());

                comboBoxMaster.ItemsSource = listMasters;
                comboBoxStatus.ItemsSource = listStatuses;

                comboBoxMaster.SelectedIndex = 0;
                comboBoxStatus.SelectedIndex = 0;

                textBlockId.Text = request.Id.ToString();
                textBoxDesc.Text = request.Description.ToString();
                if(rToEdit.MasterId!=null)
                    comboBoxMaster.SelectedItem = db.Users.FirstOrDefault(u=>u.Id == rToEdit.MasterId).Login;
                comboBoxStatus.SelectedItem = db.Statuses.FirstOrDefault(s => s.Id == rToEdit.StatusId).Name;
            }
            
        }

        private void buttonEdit_Click(object sender, RoutedEventArgs e)
        {
            if(textBoxDesc.Text == string.Empty)
            {
                MessageBox.Show("Добавьте описание", "Ошибка");
                return;
            }
            using (ImportAbrEntities db = new ImportAbrEntities())
            {
                Requests rE = db.Requests.FirstOrDefault(r=>r.Id==rToEdit.Id);
                if(rE.StatusId != db.Statuses.FirstOrDefault(s => s.Name == "Готова к выдаче").Id && comboBoxStatus.Text == "Готова к выдаче")
                {
                    rE.CompletionDate = DateTime.Now;
                }
                if(userRole.Name == "Мастер" || userRole.Name == "Оператор")
                {
                    rE.MasterId = currentUser.Id;
                    if (textBoxMessage.Text != string.Empty)
                    {
                        var com = db.Comments.FirstOrDefault(c => c.RequestId == rE.Id && c.MasterId == currentUser.Id);
                        if (com == null)
                        {
                            Comments comment = new Comments();
                            comment.Message = textBoxMessage.Text;
                            comment.RequestId = rE.Id;
                            comment.MasterId = currentUser.Id;
                            db.Comments.Add(comment);
                        }
                        else
                        {
                            com.Message = textBoxMessage.Text;
                        }
                        db.SaveChanges();

                    }
                    if (textBoxParts.Text != string.Empty)
                    {
                        rE.RepairParts = textBoxParts.Text;
                    }
                }
                else if (comboBoxMaster.SelectedItem == "Нет")
                {
                    rE.MasterId = null;
                }
                else
                {
                    rE.MasterId = db.Users.FirstOrDefault(u => u.Login == comboBoxMaster.SelectedItem).Id;
                }
                rE.StatusId = db.Statuses.FirstOrDefault(s => s.Name == comboBoxStatus.SelectedItem).Id;
                rE.Description = textBoxDesc.Text;

                db.SaveChanges();
                MessageBox.Show("Изменения приняты", "Успех");
                Close();
            }
        }
    }
}
