using Remonter.Appdata;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Remonter.UserPages.Worker
{
    /// <summary>
    /// Interaction logic for WorkerPage.xaml
    /// </summary>
    public partial class WorkerPage : Page
    {
        public WorkerPage(int Autorized_user)
        {
            InitializeComponent();
        }
        private void make_new_request(object sender, EventArgs e)
        {
            Request new_request = new Request()
            {

                content_request = "Введите описание проблемы",
                request_status = "Ожидает",
                time_request = DateTime.Now

            };
            panel_edit_request.DataContext = new_request;
            panel_edit_request.Visibility = Visibility.Visible;
            Make_request_Btn.Visibility = Visibility.Hidden;



        }
        private void Save_request_status (object sender, EventArgs e)
        {
            
            try
            {
                

                AppConnect.Current_Db_model.Requests.Add((Request)panel_edit_request.DataContext);
                AppConnect.Current_Db_model.SaveChanges();
                MessageBox.Show("Ваш запрос отправлен на обработку, спасибо за помощь нашему отделу ремонта оборудования!");
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());

            }
        }
    }
}
