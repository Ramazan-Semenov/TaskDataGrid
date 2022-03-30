using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DataTable dt { get; set; }
        public List<int> rr { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            //DataContext = new s();
            SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lenovo\source\repos\ConsoleApp1\ConsoleApp1\Database1.mdf;Integrated Security=True;Connect Timeout=30");
           

            connection.Open();
            Console.WriteLine("Start1");
            DataTable dataTable = new DataTable();
            SqlCommand command = new SqlCommand("SELECT TOP(100) [Id] ,[FirstName] as 'Имя' ,[LastName] As 'Фамилия' ,[Manager] ,[Salary] ,[StartDate]  FROM [dbo].[Employe]", connection);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);

            dt = dataTable;
            //rr= new List<int>() { 4, 5 };
            //DataContext = this;

            M.DataTableProp = dataTable;
            M.ListTypeDate = new List<int>() { 4, 5 };
        }

       


    }
  public  class s
    {
        public string txt { get; set; } = "dddddddddd";
        public string Task_Book { get; set; } = "bbasfasdfbbbbb";

        public List<task_book> Tasks { get; set; } = new List<task_book>();
        public s()
        {
            Tasks.Add(new task_book { executor="sdsd" });
        }

    }
}
