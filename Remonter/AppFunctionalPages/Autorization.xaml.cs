using Remonter.Appdata;
using Remonter.UserPages.Admin;
using Remonter.UserPages.Manager;
using Remonter.UserPages.Remonter;
using Remonter.UserPages.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for Autorization.xaml
    /// </summary>
    public partial class Autorization : Page
    {
        List<Branch> branch_list = AppConnect.Current_Db_model.Branches.ToList();
        public Autorization()
        {
            InitializeComponent();
            
            for (int i = 0; i < branch_list.Count(); i++)
            {
                branch_Name.Items.Add(branch_list[i].address);
            }
        }
        public Autorization(string login, string password)
        {
            InitializeComponent();
            textbarPassword.Text = login;
            passbaric.Password = password;

            for (int i = 0; i < branch_list.Count(); i++)
            {
                branch_Name.Items.Add(branch_list[i].address);
            }
        }
        private void AutorizationButton(object sender, RoutedEventArgs e)
        {
  

            try
            {   
                //create an object of user that trying to enter the system
                var userObj = AppConnect.Current_Db_model.Users.FirstOrDefault(x => x.login == textbarPassword.Text && x.password == passbaric.Password);
                if (userObj == null)
                {
                    MessageBox.Show("Такого пользователя нет!", "Ошибка при авторизации", MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else
                {


                    //find identified Staff memebet by user_id in system

                    Staff indetifiedUserObj = AppConnect.Current_Db_model.Staffs.FirstOrDefault(x => x.id_user == userObj.id_user);
                    if ( indetifiedUserObj == null || indetifiedUserObj.time_fired != null)
                    {
                        MessageBox.Show("Вы уволены, у вас нет прав входа в систему, свяжитесь с вашим менеджером", "Ошибка при авторизации", MessageBoxButton.OK, MessageBoxImage.Error);

                    }
                    else
                    {

                        //write log of Staff moving

                        Branch branchName = AppConnect.Current_Db_model.Branches.FirstOrDefault(x => x.address == (string)branch_Name.SelectedItem.ToString());

                        if (indetifiedUserObj.id_branch != branchName.id_branch)
                        {

                            staff_moving_log last_indentified_user_log_in = AppConnect.Current_Db_model.staff_moving_log
                                .Where(u => u.id_staff_member == indetifiedUserObj.id_staff_member)
                                .OrderByDescending(u => u.time_branch_approach)
                                .FirstOrDefault();
                            if (last_indentified_user_log_in == null)
                            {
                                staff_moving_log new_log_to_db = new staff_moving_log()
                                {
                                    id_staff_member = indetifiedUserObj.id_staff_member,
                                    id_branch = branchName.id_branch,
                                    time_branch_approach = DateTime.Now,
                                    time_branch_come_out = new DateTime(2000, 1, 20, 12, 40, 40)

                                };
                                AppConnect.Current_Db_model.staff_moving_log.Add(new_log_to_db);
                                AppConnect.Current_Db_model.SaveChanges();
                                MessageBox.Show("Добавлен новый лог");
                            }
                            else
                            {
                                last_indentified_user_log_in.time_branch_come_out = DateTime.Now;
                                AppConnect.Current_Db_model.SaveChanges();
                                staff_moving_log new_log_to_db = new staff_moving_log()
                                {
                                    id_staff_member = indetifiedUserObj.id_staff_member,
                                    id_branch = branchName.id_branch,
                                    time_branch_approach = (DateTime)last_indentified_user_log_in.time_branch_come_out,
                                    time_branch_come_out = new DateTime(2000, 1, 20, 12, 40, 40)
                                };
                                AppConnect.Current_Db_model.staff_moving_log.Add(new_log_to_db);
                                AppConnect.Current_Db_model.SaveChanges();
                                MessageBox.Show("Сотрудник сменил дислокацию");
                            }
                        }

                        indetifiedUserObj.id_branch = branchName.id_branch;
                        AppConnect.Current_Db_model.SaveChanges();
                        //Navigation 
                        switch (indetifiedUserObj.id_staff_role)
                        {
                            case 1:
                                MessageBox.Show("Здраствуйте, " + indetifiedUserObj.name_staff_member + " " + indetifiedUserObj.last_name_staff_member + "!", "Уведомление", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                                AppFrame.frameMain.Navigate(new AdminPage((int)indetifiedUserObj.id_user));
                                break;
                            case 2:
                                MessageBox.Show("Здраствуйте, " + indetifiedUserObj.name_staff_member + " " + indetifiedUserObj.last_name_staff_member + "!", "Уведомление", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                                AppFrame.frameMain.Navigate(new WorkerPage((int)indetifiedUserObj.id_user));
                                break;
                            case 3:
                                MessageBox.Show("Здраствуйте, " + indetifiedUserObj.name_staff_member + " " + indetifiedUserObj.last_name_staff_member + "!", "Уведомление", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                                AppFrame.frameMain.Navigate(new ManagerPage((int)indetifiedUserObj.id_user));
                                break;
                            case 4:
                                MessageBox.Show("Здраствуйте, " + indetifiedUserObj.name_staff_member + " " + indetifiedUserObj.last_name_staff_member + "!", "Уведомление", MessageBoxButton.OK,
                                MessageBoxImage.Information);

                                AppFrame.frameMain.Navigate(new RemonterPage((int)indetifiedUserObj.id_user));
                                break;

                            default:
                                break;

                        }

                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Ошибка" + Ex.Message.ToString() + "Критическая работа приложения!", "Уведомление", MessageBoxButton.OK,
                            MessageBoxImage.Warning);

            }

        }

        private void reg_in_button(object sender, RoutedEventArgs e)
        {
            AppFrame.frameMain.Navigate(new Registration());
        }
    }
}

