using iTextSharp.text.pdf;
using iTextSharp.text;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto.Digests;
using Remonter.Appdata;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Paragraph = iTextSharp.text.Paragraph;
using System.CodeDom.Compiler;
using System.Xml;

namespace Remonter.UserPages.Remonter
{
    /// <summary>
    /// Interaction logic for RemonterPage.xaml
    /// </summary>
    public partial class RemonterPage : Page
    {
        int globalContextUserId;
        Staff globalContextStaffMember;
        public RemonterPage(int Autorized_user)
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
        private void Btn_Close_windows(object sender, EventArgs e)
        {
            Navigation_bar.IsEnabled = true;

            list_of_messages.Visibility = Visibility.Hidden;
            Btn_write_message.Visibility = Visibility.Hidden;
            Message_Send.Visibility = Visibility.Hidden;
            list_of_devices.Visibility = Visibility.Hidden;
            list_of_price_lists.Visibility = Visibility.Hidden;
            Edit_device_list.Visibility = Visibility.Hidden;
            panel_create_PriceList.Visibility = Visibility.Hidden;
            Edit_Price_list_panel.Visibility = Visibility.Hidden;
            list_of_requests.Visibility = Visibility.Hidden;
            btn_change_request_status.Visibility = Visibility.Hidden;
            Add_transferLog.Visibility = Visibility.Hidden;
            list_of_transfetDevice.Visibility = Visibility.Hidden;
        }
        private void Btn_show_devices(object sender, EventArgs e)
        {

            List<Device> device_list = AppConnect.Current_Db_model.Devices.ToList();
            list_of_devices.ItemsSource = device_list;
            list_of_devices.Visibility = Visibility.Visible;
            Edit_device_list.Visibility = Visibility.Visible;

        }
        private void delete_device_btn(object sender, EventArgs e)
        {
            List<Device> _context_device_list = list_of_devices.SelectedItems.Cast<Device>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить Заказ? {_context_device_list.Count()} элементов?", "Внимание",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    AppConnect.Current_Db_model.Devices.RemoveRange(_context_device_list);
                    AppConnect.Current_Db_model.SaveChanges();
                    MessageBox.Show("Данные удалены!");
                    list_of_devices.ItemsSource = AppConnect.Current_Db_model.Devices.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());

                }

            }
        }
        private void edit_device_btn(object sender, EventArgs e)
        {
            Device _context_device = (Device)list_of_devices.SelectedItem;
            panel_selected_device.DataContext = _context_device;
            panel_selected_device.Visibility = Visibility.Visible;
            list_of_devices.Visibility = Visibility.Hidden;
        }
        private void btn_add_device(object sender, EventArgs e)
        {
            Device new_device_add = new Device()
            {
                name_device = "Название девайса",
                model_device = "Модель девайса",
                dateOfDevelopment = DateTime.Now,
                id_branch = 2,
                time_branch_approach = DateTime.Now,

            };
            panel_selected_device.DataContext = new_device_add;
            panel_selected_device.Visibility = Visibility.Visible;

            try
            {
                AppConnect.Current_Db_model.Devices.Add((Device)panel_selected_device.DataContext);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());

            }



        }
        private void btn_add_device_confirm(object sender, EventArgs e)
        {
            try
            {
                AppConnect.Current_Db_model.SaveChanges();
                MessageBox.Show("Данные сохранены");
                panel_selected_device.Visibility = Visibility.Hidden;
                list_of_devices.Visibility = Visibility.Visible;
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
        private void Add_Price_List(object sender, RoutedEventArgs e)
        {
            list_of_price_lists.Visibility = Visibility.Hidden;

            PurchaseList new_Pricelist = new PurchaseList()
            {
                items_to_buy = "Содержимое прайс листа",
                price = 0.0,
                purchase_list_status = "Создан"

            };
            panel_create_PriceList.DataContext = new_Pricelist;
            panel_create_PriceList.Visibility = Visibility.Visible;


        }

        private void Save_Price_List(object sender, RoutedEventArgs e)
        {
            try
            {
                AppConnect.Current_Db_model.PurchaseLists.Add((PurchaseList)panel_create_PriceList.DataContext);
                AppConnect.Current_Db_model.SaveChanges();
                MessageBox.Show("Прайс лист сформирован!");
                panel_create_PriceList.Visibility = Visibility.Hidden;
                list_of_price_lists.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());

            }

        }

        private void Btn_show_requests(object sender, RoutedEventArgs e)
        {
            List<Request> requests_list = AppConnect.Current_Db_model.Requests.ToList();
            list_of_requests.ItemsSource = requests_list;
            list_of_requests.Visibility = Visibility.Visible;
            btn_change_request_status.Visibility = Visibility.Visible;


        }
        private void Btn_edit_request(object sender, RoutedEventArgs e)
        {
            Request _selected_request = (Request)list_of_requests.SelectedItem;

            list_of_requests.Visibility = Visibility.Hidden;
            panel_edit_request.Visibility = Visibility.Visible;
            panel_edit_request.DataContext = _selected_request;



        }
        private void Save_request_status(object sender, EventArgs e)
        {
            try
            {

                AppConnect.Current_Db_model.SaveChanges();
                MessageBox.Show("Статус запроса изменен!");
                panel_edit_request.Visibility = Visibility.Hidden;
                list_of_requests.Visibility = Visibility.Visible;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());

            }
        }


        private void Btn_show_device_rotation_log(object Sender, EventArgs e)
        {
            List<TransferDevice> transferDevices_list = AppConnect.Current_Db_model.TransferDevices.ToList();
            list_of_transfetDevice.ItemsSource = transferDevices_list;
            Add_transferLog.Visibility = Visibility.Visible;
            list_of_transfetDevice.Visibility = Visibility.Visible;

        }
        private void btn_add_transferlog(object Sender, EventArgs e)
        {
            TransferDevice new_transfer_log = new TransferDevice()
            {
                id_transfer = 0,
                id_device = 2,
                branch_name_removed = "Главный филлиал",
                branch_name_moved = "Второй корпус",
                time_removed = DateTime.Now

            };

            panel_add_transfer.DataContext = new_transfer_log;
            panel_add_transfer.Visibility = Visibility.Visible;


        }
        private void Save_new_transferDevice_log(object sender, EventArgs e)
        {
            try
            {
                AppConnect.Current_Db_model.TransferDevices.Add((TransferDevice)panel_add_transfer.DataContext);
                
                AppConnect.Current_Db_model.SaveChanges();
                MessageBox.Show("Добавлен новый лог");


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());

            }
        }
        private void Btn_show_RepairngDevice(object sender, EventArgs e)
        {
            List<RepairingDevice> RepairingDevices_list = AppConnect.Current_Db_model.RepairingDevices.ToList();
            
        }
    }
}
