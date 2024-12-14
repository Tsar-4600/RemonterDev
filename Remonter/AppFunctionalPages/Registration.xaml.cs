using Remonter.Appdata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Remonter.AppFunctionalPages
{
    /// <summary>
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Page
    {
        public Registration()
        {
            InitializeComponent();
            //comboBOx filling from database
            List<Branch> branch_list = AppConnect.Current_Db_model.Branches.ToList();
            for (int i = 0; i < branch_list.Count(); i++)
            {
                branch_Name.Items.Add(branch_list[i].address);
            }
        }
        private void back_button_Click(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.GoBack();
        }


        private void pass_pass_changed(object sender, RoutedEventArgs e)
        {
            if (password_input.Password != password_submit.Password)
            {
                regin_button.IsEnabled = false;
                password_submit.Background = Brushes.LightCoral;
                password_submit.BorderBrush = Brushes.Red;
            }
            else
            {
                regin_button.IsEnabled = true;
                password_submit.Background = Brushes.LightGreen;
                password_submit.BorderBrush = Brushes.Green;
            }

        }
        private void create_acc(object sender, RoutedEventArgs e)
        {
            if (AppConnect.Current_Db_model.Users.Count(x => x.login == login_inputbar.Text) > 0)
            {
                MessageBox.Show("Пользовтель с таким логином есть!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try
            {   // registration new user
                User userObj = new User()
                {
                    login = login_inputbar.Text,
                    user_name = user_name_inputbar.Text,
                    password = password_submit.Password,
                };
                AppConnect.Current_Db_model.Users.Add(userObj);
                AppConnect.Current_Db_model.SaveChanges();
                //find new registred user and take id_user
                User UserToClient = AppConnect.Current_Db_model.Users.FirstOrDefault(x => x.login == userObj.login && x.password == userObj.password);
                // take id by value from combobox
                Branch branchName = AppConnect.Current_Db_model.Branches.FirstOrDefault(x => x.address == (string)branch_Name.SelectedItem.ToString());
                //make new client after registration
                Staff staffObj = new Staff()
                {
                    id_user = UserToClient.id_user,
                    name_staff_member = user_name_inputbar.Text,
                    last_name_staff_member = user_last_name_inputbar.Text,
                    time_hired = DateTime.Now,
                    id_branch = branchName.id_branch,
                    id_staff_role = 2 
                };
                AppConnect.Current_Db_model.Staffs.Add(staffObj);
                AppConnect.Current_Db_model.SaveChanges();

                MessageBox.Show("Данные успешно добавлены!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                AppFrame.frameMain.Navigate(new Autorization(userObj.login, userObj.password));
            }
            catch
            {
                MessageBox.Show("Ошибка при добавлении данных! Проверьте правильность написания", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

   
    }
}

