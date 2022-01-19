using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskDataGrid
{
    /// <summary>
    /// Логика взаимодействия для CustomDG.xaml
    /// </summary>
    public partial class CustomDG : UserControl
    {
        public static readonly DependencyProperty ListTypeDateProperty = DependencyProperty.Register(
       nameof(ListTypeDate), typeof(IList<int>), typeof(CustomDG), new FrameworkPropertyMetadata(new List<int>(),
          FrameworkPropertyMetadataOptions.None));

        public IList<int> ListTypeDate
        {
            get { return (IList<int>)GetValue(ListTypeDateProperty); }
            set { SetValue(ListTypeDateProperty, value); }
        } 
       
        public static readonly DependencyProperty DataTablePropProperty = DependencyProperty.Register(
       nameof(DataTableProp), typeof(DataTable), typeof(CustomDG), new FrameworkPropertyMetadata(new DataTable(),
          FrameworkPropertyMetadataOptions.None));

        public DataTable DataTableProp
        {
            get { return (DataTable)GetValue(DataTablePropProperty); }
            set { SetValue(DataTablePropProperty, value); }
        }
        public CustomDG()
        {
            InitializeComponent();
            this.Loaded += CustomDG_Loaded;

        }

        private void CustomDG_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = new ViewModel.ViewModelCustomDG(DataTableProp, ListTypeDate);
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
