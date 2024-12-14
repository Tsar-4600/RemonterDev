
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
using Remonter.Appdata;
using Remonter.AppFunctionalPages;

namespace Remonter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppConnect.Current_Db_model = new RepairServiceDevEntities();
            AppFrame.frameMain = FrmMain;
            AppFrame.frameMain.Navigate(new Autorization());
        }

       
    }
}
