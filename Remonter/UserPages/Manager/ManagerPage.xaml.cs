using Remonter.Appdata;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Image = iTextSharp.text.Image;
using Paragraph = iTextSharp.text.Paragraph;
using System.IO;

using System.CodeDom.Compiler;
using System.Xml;

namespace Remonter.UserPages.Manager
{
    /// <summary>
    /// Interaction logic for ManagerPage.xaml
    /// </summary>
    public partial class ManagerPage : Page
    {


        List<Staff> staff_list = AppConnect.Current_Db_model.Staffs.Where(x => x.time_fired == null).ToList();
        int globalContextUserId;
        Staff globalContextStaffMember;


        public ManagerPage(int Autorized_user)
        {
            InitializeComponent();
            globalContextUserId = Autorized_user;

            Staff _currentUserContext = AppConnect.Current_Db_model.Staffs.FirstOrDefault(x => x.id_user == globalContextUserId);
            globalContextStaffMember = _currentUserContext;
        }
        private void messageBtn_click(object sender, EventArgs e)
        {
            Navigation_bar.IsEnabled = false;
            List<Message> messages_list = AppConnect.Current_Db_model.Messages.Where(x => x.to_id_staff_member == globalContextStaffMember.id_staff_member).ToList();

            list_of_messages.ItemsSource = messages_list;
            list_of_messages.Visibility = Visibility.Visible;
            Btn_write_message.Visibility = Visibility.Visible;
        }

        private void writeMessageBtn_click(object sender, EventArgs e)
        {
            Navigation_bar.IsEnabled = false;
            list_of_messages.Visibility = Visibility.Hidden;
            Message_Send.Visibility = Visibility.Visible;
            Message new_message = new Message()
            {
                from_id_staff_member = globalContextStaffMember.id_staff_member,
                to_id_staff_member = 2,
                message_content = "Тестовое сообщение",
            };
            Message_Send.DataContext = new_message;



        }
        private void Send_Message(object sender, EventArgs e)
        {

            try
            {
                AppConnect.Current_Db_model.Messages.Add((Message)Message_Send.DataContext);
                AppConnect.Current_Db_model.SaveChanges();
                MessageBox.Show("Сообщение отправлено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }

        public void Show_Change_button(object sender, EventArgs e)

        {
            ChangePriceStatus_bar.DataContext = (PurchaseList)list_of_price_lists.SelectedItem;
            Edit_Price_list_panel.Visibility = Visibility.Visible;
            ChangePriceStatus_bar.Visibility = Visibility.Visible;
        }
        public void Btn_changeStatusPriceList(object sender, EventArgs e)
        {
            try
            {
                AppConnect.Current_Db_model.SaveChanges();
                MessageBox.Show("Статус изменен");
             
                ChangePriceStatus_bar.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            
        }

        public void showPriceLists(object sender, EventArgs e)
        {
            list_of_price_lists.ItemsSource = AppConnect.Current_Db_model.PurchaseLists.ToList();
            list_of_price_lists.Visibility = Visibility.Visible;
            Edit_Price_list_panel.Visibility = Visibility.Visible;
        }
        public void createPdf(object sender, EventArgs e)
        {


           
            Document doc = new Document();

            try
            {
                PdfWriter.GetInstance(doc, new FileStream("..\\..\\PriceList.pdf", FileMode.Create));
                doc.Open();
                BaseFont baseFont = BaseFont.CreateFont("C:\\Windows\\Fonts\\Arial.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                Font font = new Font(baseFont, 12);
                Font font1 = new Font(baseFont, 25, 3, BaseColor.RED);
                Font font2 = new Font(baseFont, 16, 0, BaseColor.BLUE);
                Paragraph paragraph1 = new Paragraph("ДАННЫЕ ЗАПРОСА", font1);
                paragraph1.Alignment = Element.ALIGN_CENTER;
                doc.Add(paragraph1);
                foreach (var item in AppConnect.Current_Db_model.PurchaseLists.ToList())
                {
                    if (item is PurchaseList)
                    {
                        PurchaseList data = (PurchaseList)item;
                        doc.Add(new Paragraph("ID прайс листа: " + data.id_purchase_list.ToString(), font2));
                        doc.Add(new Paragraph("Содержимое прайс листа: \n " + data.items_to_buy, font));
                        doc.Add(new Paragraph("Цена: " + data.price.ToString() + "руб.", font2));

                    }
                }

            }
            catch (DocumentException de)
            {
                Console.Error.WriteLine(de.Message);
            }
            catch (IOException ioe)
            {
                Console.Error.WriteLine(ioe.Message);
            }
            finally
            {
                doc.Close();
            }
            MessageBox.Show("Прай лист сформирован");
        }

        public void showStaffList(object sender, EventArgs e)
        {
            Navigation_bar.IsEnabled = false;
            name_edit_staffmember_panel.Visibility = Visibility.Visible;
            List<Staff> actual_staff_list = AppConnect.Current_Db_model.Staffs.Where(x => x.time_fired == null && x.id_user != globalContextUserId).ToList();
            list_of_staffMembers.ItemsSource = actual_staff_list;
            list_of_staffMembers.Visibility = Visibility.Visible;
            Staff_filters.Visibility = Visibility.Visible;
        }
        /* private void Btn_delete_Click(object sender, RoutedEventArgs e)
         {


             if (list_of_staffMembers.Visibility == Visibility.Visible)
             {
                 var StaffsForRemoving = list_of_staffMembers.SelectedItems.Cast<Staff>().ToList();
                 if (MessageBox.Show($"Вы точно хотите удалить  {StaffsForRemoving.Count()} сотрудников?", "Внимание",
                 MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                 {
                     try
                     {

                         for (int i = 0; i < StaffsForRemoving.Count(); i++)
                         {
                             List<staff_moving_log> moving_logs_delte = AppConnect.Current_Db_model.staff_moving_log.Where(x => x.id_staff_member == StaffsForRemoving[i].id_staff_member).Cast<staff_moving_log>().ToList();
                             AppConnect.Current_Db_model.staff_moving_log.RemoveRange(moving_logs_delte);


                         }
                         AppConnect.Current_Db_model.SaveChanges();

                         // AppConnect.Current_Db_model.Staffs.RemoveRange(StaffsForRemoving);





                         MessageBox.Show("Сотрудник удален из базы данных!");
                         list_of_staffMembers.ItemsSource = AppConnect.Current_Db_model.Staffs.ToList();
                     }
                     catch (Exception ex)
                     {
                         MessageBox.Show(ex.Message.ToString());
                     }

                 }

             }


         } */
        private void Btn_Close_windows(object sender, EventArgs e)
        {
            Navigation_bar.IsEnabled = true;

            list_of_messages.Visibility = Visibility.Hidden;
            Btn_write_message.Visibility = Visibility.Hidden;
            Message_Send.Visibility = Visibility.Hidden;
            list_of_staff_to_message.Visibility = Visibility.Hidden;
            list_of_staffMembers.Visibility = Visibility.Hidden;
            
           
            name_edit_staffmember_panel.Visibility = Visibility.Hidden;
            name_edit_staffmember_panel.Visibility = Visibility.Hidden;
            Edit_StaffMember_Panel.Visibility = Visibility.Hidden;
            Staff_filters.Visibility = Visibility.Hidden;

            list_of_moving_log.Visibility = Visibility.Hidden;

            list_of_price_lists.Visibility = Visibility.Hidden;
            Edit_Price_list_panel.Visibility = Visibility.Hidden;
            ChangePriceStatus_bar.Visibility = Visibility.Hidden;
        }

        private void Btn_show_edit_panel(object sender, EventArgs e)
        {
            list_of_staffMembers.Visibility = Visibility.Hidden;
            Edit_StaffMember_Panel.Visibility = Visibility.Visible;
            save_data_button.IsEnabled = true;
            name_edit_staffmember_panel.Visibility = Visibility.Visible;
            Staff_data.DataContext = list_of_staffMembers.SelectedItems;
            BtnConfirm_add_staff_member.IsEnabled = false;


        }
        private void Btn_save_staff(object sender, EventArgs e)
        {

            try
            {
                AppConnect.Current_Db_model.SaveChanges();
                MessageBox.Show("Данные успешно обновлены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }

        }
        private void Btn_fire_Click(object sender, RoutedEventArgs e)
        {


            if (list_of_staffMembers.Visibility == Visibility.Visible)
            {
                var StaffsForFiring = list_of_staffMembers.SelectedItems.Cast<Staff>().ToList();
                if (MessageBox.Show($"Вы точно хотите уволить  {StaffsForFiring.Count()} сотрудников?", "Внимание",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        for (int i = 0; i < StaffsForFiring.Count(); i++)
                        {
                            StaffsForFiring[i].time_fired = DateTime.Now;
                        }
                        AppConnect.Current_Db_model.SaveChanges();
                        MessageBox.Show("Дата увольнения изменена!");
                        list_of_staffMembers.ItemsSource = AppConnect.Current_Db_model.Staffs.Where(x => x.time_fired == null).ToList();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }

                }

            }


        }
        private void Btn_add_new_staff_member(object sender, EventArgs e)
        {
            BtnConfirm_add_staff_member.IsEnabled = true;
            list_of_staffMembers.Visibility = Visibility.Hidden;
            save_data_button.IsEnabled = false;
            Random rnd = new Random();
            Char[] pwdChars = new Char[26] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            string generated_login = "";
            string generated_password = "";
            for (int i = 0; i < 10; i++)
            {

                generated_login += pwdChars[rnd.Next(0, 25)];
                generated_password += Convert.ToString(rnd.Next(0, 7000));
            }


            User new_user = new User()
            {
                user_name = generated_login,
                login = generated_login,
                password = generated_password
            };

            try
            {
                AppConnect.Current_Db_model.Users.Add(new_user);
                AppConnect.Current_Db_model.SaveChanges();

                var id_user_to_staff_id_user = AppConnect.Current_Db_model.Users.FirstOrDefault(x => x.login == generated_login && x.password == generated_password);

                Staff new_staff_member = new Staff()
                {
                    name_staff_member = "Имя Сотрудника",
                    last_name_staff_member = "Фамилия сотрудника",
                    time_hired = DateTime.Now,
                    time_fired = null,
                    id_branch = 2,
                    id_staff_role = 2,
                    id_user = id_user_to_staff_id_user.id_user,
                    email = null,
                    telephone = null,

                };


                Staff_data.DataContext = new_staff_member;
                Edit_StaffMember_Panel.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }


        }
        private void Btn_add_new_staff_member_confirm(object sender, EventArgs e)
        {
            try
            {
                AppConnect.Current_Db_model.Staffs.Add((Staff)Staff_data.DataContext);

                AppConnect.Current_Db_model.SaveChanges();
                MessageBox.Show("Сотрудник успешно добавлен, редактируйте его данные");
                Edit_StaffMember_Panel.Visibility = Visibility.Hidden;
                Staff_data.Visibility = Visibility.Hidden;
                name_edit_staffmember_panel.Visibility = Visibility.Hidden;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }



        }
        private void Btn_show_move_log(object sender, EventArgs e)
        {
       
            List<staff_moving_log> staff_move_log_list = AppConnect.Current_Db_model.staff_moving_log.ToList();
            list_of_moving_log.ItemsSource = staff_move_log_list;
            list_of_moving_log.Visibility = Visibility.Visible;
            Navigation_bar.IsEnabled = false;
        }
      

        private void FindStaffMember(object sender, TextChangedEventArgs e)
        {
            var staff_members_filtered = staff_list;

            if (TextSearch != null)
            {
                try
                {
                    int id_user_searched = int.Parse(TextSearch.Text);
                    staff_members_filtered = staff_members_filtered.Where(x => x.id_staff_member == id_user_searched).ToList();
                    list_of_staffMembers.ItemsSource = staff_members_filtered;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Сброс поиска: \n" + ex.Message.ToString());
                    list_of_staffMembers.ItemsSource = staff_list.Where(x => x.id_user != globalContextUserId && x.time_fired == null).ToList();

                }

            }
           
        }

        private void ComboBoxFilterChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboFilter.SelectedIndex)
            {
                case 0:
                    staff_list = staff_list.OrderBy(x => x.time_hired).ToList();

                    list_of_staffMembers.ItemsSource = staff_list;
                    break;
                case 1:
                    staff_list = staff_list.OrderByDescending(x => x.time_hired).ToList();

                    list_of_staffMembers.ItemsSource = staff_list;
                    break;
                default:
                    break;
            }
        }

     
    }

        
}
