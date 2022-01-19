using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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

namespace TaskDataGrid
{
    /// <summary>
    /// Логика взаимодействия для CustomDG.xaml
    /// </summary>
    public partial class CustomDG : UserControl
    {

        public CustomDG()
        {
            InitializeComponent();
            DataContext = new ViewModel.ViewModelCustomDG();
        

        }

        private void p_Click(object sender, RoutedEventArgs e)
        {
            var columnHeader = sender as Button;
            popExcel.PlacementTarget = columnHeader;
            popExcel.IsOpen = true;
        }
    }

    public  class v : ViewModelBase
    {
        public static string cont { get; set; } = "cont";
        public static RelayCommand ProjMenuItem_Edit
        {
            get; set;
        }
        public List<task_book> Tasks { get; set; } = new List<task_book>();
        public string Task_Book { get; set; }
         static v()
        {
            ProjMenuItem_Edit = new RelayCommand(hh);
        }
       static void hh() { MessageBox.Show("Ok"); }
    }
}
