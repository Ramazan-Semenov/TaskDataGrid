using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TaskDataGrid.Model;

namespace TaskDataGrid.ViewModel
{
  public  class ViewModelCustomDG: ViewModelBase
    {
        private DataTable DataTable = new DataTable();
      public  DataView DataView { get; set; } = new DataView();
        private static string ColumnHeader_Property="";
        public bool IsOpen { get; set; } = false;
        Dictionary<string, ObservableCollection<FilterObj>> filters=new Dictionary<string, ObservableCollection<FilterObj>>();
        Dictionary<string, ObservableCollection<IHierarchy<DateTime>>> filtersDate = new Dictionary<string, ObservableCollection<IHierarchy<DateTime>>>();

        private List<DateTime> DateTimes = new List<DateTime>();

        private ObservableCollection<IHierarchy<DateTime>> _dates;
        public ObservableCollection<IHierarchy<DateTime>> Dates { 
            
            get {
                return _dates;
            } 
            set {

                _dates = value;
            } }

        public ObservableCollection<FilterObj> ListFilterContent { get; set; } = new ObservableCollection<FilterObj>();
        public ViewModelCustomDG()
        {
            SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lenovo\source\repos\ConsoleApp1\ConsoleApp1\Database1.mdf;Integrated Security=True;Connect Timeout=30");

            
            connection.Open();
            Console.WriteLine("Start1");
            SqlCommand command = new SqlCommand("SELECT TOP(100) [Id] ,[FirstName] ,[LastName] ,[Manager] ,[Salary] ,[StartDate]  FROM [dbo].[Employe]", connection);

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(DataTable);
            connection.Close();
            connection.Dispose();
            DataView=DataTable.DefaultView;
       
          //  BuildDates(new List<DateTime> { DateTime.Now, DateTime.Parse("17.12.2022"), DateTime.Parse("17.12.2021"), DateTime.Parse("10.01.2021") }) ;
        }
      //  public List<string> ListFilterContent { get; set; } = new List<string>();
       public RelayCommand<string> OpenPopup { get
            {
             
                return new RelayCommand<string>(( string ColumnHeader_Property_Parameter) => {
                    ListFilterContent = new ObservableCollection<FilterObj>();
                    ColumnHeader_Property = ColumnHeader_Property_Parameter;
                    if (DataTable.Columns[ColumnHeader_Property].DataType == typeof(DateTime))
                    {
                        foreach (DataRow item in DataTable.Rows)
                        {
                            DateTimes.Add(Convert.ToDateTime( item[ColumnHeader_Property]));
                        }
                        _dates = BuildDates(DateTimes);
                        if (!filtersDate.ContainsKey(ColumnHeader_Property))
                        {
                            filtersDate.Add(ColumnHeader_Property, _dates);
                        }
                        _dates = filtersDate[ColumnHeader_Property];
                        RaisePropertyChanged("Dates");
                    }
                    else
                    {
                        foreach (DataRow item in DataTable.Rows)
                        {
                            ListFilterContent.Add(new FilterObj { IsChecked = true, Title = item[ColumnHeader_Property].ToString() });
                        }

                        ListFilterContent.Distinct();
                        if (!filters.ContainsKey(ColumnHeader_Property))
                        {
                            filters.Add(ColumnHeader_Property, ListFilterContent);
                        }

                        ListFilterContent = filters[ColumnHeader_Property];
                        RaisePropertyChanged("ListFilterContent");
                    }
                   

                 

                });
            }
        }


        public RelayCommand StartSorted
        { 
            get
            {
                return new RelayCommand(()=> {
                    string filterstring = "";

                    foreach (var item in ListFilterContent.Where(x => x.IsChecked == false))
                    {

                        filterstring += string.Format("{0} <>'{1}' And ", ColumnHeader_Property,item.Title);
                        //MessageBox.Show();

                    }
                    int count = filterstring.Length;
                    int i = filterstring.LastIndexOf("And");
                    if (i >= 0)
                    {
                        filterstring = filterstring.Substring(0, i) + filterstring.Substring(i + "And".Length);
                    }
                    DataView.RowFilter = filterstring;
                 

                    RaisePropertyChanged("DataTable");

                    //foreach (var item in _dates)
                    //{
                    //    foreach (var item2 in item.Children)
                    //    {
                    //        MessageBox.Show(item2.Level.ToString());
                    //    }

                    //}
                    int count1 = 0;
                    int count2 = 0;
                    int count3 = 0;
                    foreach (var item in _dates.Where(x=>x.Level==1))
                    {
                        count1++;
                        //MessageBox.Show(item.Value.ToString()); ;
                        foreach (var item2 in item.Children)
                        {
                        if (item2.IsChecked == true)
                            {

                            }
                            count2++;
                        }
                    }

                    foreach (var item in Vs)
                    {
                        v += item/*+" Or "*/;
                    }

                    int ii = v.LastIndexOf("Or");
                    if (ii >= 0)
                    {
                        v = v.Substring(0, ii) + v.Substring(ii + "Or".Length);
                    }
                    MessageBox.Show(v);

                    DataView.RowFilter = v;

                    RaisePropertyChanged("DataView");

                    //  MessageBox.Show( string.Format("1:{0} 2:{1}",count1,count2));


                    v = "";
                });  
 

               
            }
        }  

        void vd()
        {
            //if (content == "2")
            //{
            //    string v = String.Format("StartDate >= # {0} # and StartDate<=# {1} #", DateTime.Parse(value).ToString("MM/01/yyyy"), DateTime.Parse(value).ToString("MM/31/yyyy"));
            //    Table.RowFilter = v;
            //}
            //else if (content == "1")
            //{
            //    string v = String.Format("StartDate >= # {0} # and StartDate<=# {1} #", DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/31/yyyy"));
            //    Table.RowFilter = v;
            //}
            //else if (content == "3")
            //{
            //    string v = String.Format("StartDate = # {0}# ", DateTime.Parse(value).ToString("MM/dd/yyyy"));
            //    Table.RowFilter = v;
            //}
        }
        public bool IsChecked
        {
            get { MessageBox.Show(IsChecked.ToString()); return true; }
        }
        public string tag;
        public string Tag { get { MessageBox.Show(tag); return tag; } set { tag = value; } }
        //public RelayCommand IsChecked
        //{ 
        //    get
        //    {
        //        return new RelayCommand(()=> {


        //            MessageBox.Show("ok");


        //        });  



        //    }
        //}

        string v = "";
        List<string> Vs = new List<string>();

        public RelayCommand<DateHierarchy> Date_Checked
        {
            get
            {
                return new RelayCommand<DateHierarchy>((DateHierarchy sender)=> {
                    string content = sender.Level.ToString();
                    string value = sender.Value.ToString();
                    int colday = DateTime.DaysInMonth(DateTime.Parse(value).Year, DateTime.Parse(value).Month);

                    if (content == "2")
                    {
                        // MessageBox.Show(DateTime.Parse(value).ToString("MM/31/yyyy").ToString()); ;
                        ////v += String.Format("StartDate >= # {0} # and StartDate<=#{1}# Or ", DateTime.Parse(value).ToString("MM/01/yyyy"), DateTime.Parse(value).ToString("MM/"+colday.ToString()+ "/yyyy"));
                        //DataView.RowFilter = v;
                        Vs.Remove(String.Format("StartDate >= # {0} # and StartDate<=# {1}# Or ", DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/31/yyyy")));
                        Vs.Add(String.Format("StartDate >= # {0} # and StartDate<=#{1}#  Or ", DateTime.Parse(value).ToString("MM/01/yyyy"), DateTime.Parse(value).ToString("MM/" + colday.ToString() + "/yyyy")));
                    }
                    else if (content == "1")
                    {
                        //v = String.Format("StartDate >= # {0} # and StartDate<=# {1}# Or ", DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/31/yyyy"));
                       
                        Vs.Add(String.Format("StartDate >= # {0} # and StartDate<=# {1}# Or ", DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/31/yyyy")));
                        //DataView.RowFilter = v;
                    }
                    else if (content == "3")
                    {
                         //v += String.Format("StartDate = # {0}# Or ", DateTime.Parse(value).ToString("MM/dd/yyyy"));
                        Vs.Add(String.Format("StartDate = # {0}#  ", DateTime.Parse(value).ToString("MM/dd/yyyy")));
                    }
                   
                
                });
            }
        } public RelayCommand<DateHierarchy> Date_UnChecked
        {
            get
            {
                return new RelayCommand<DateHierarchy>((DateHierarchy sender)=> {
                    //string content = sender.Level.ToString();
                    //string value = sender.Value.ToString();
                    //if (content == "2")
                    //{
                    //    string v = String.Format("StartDate >= # {0} # and StartDate<=# {1} #", DateTime.Parse(value).ToString("MM/01/yyyy"), DateTime.Parse(value).ToString("MM/31/yyyy"));
                    //    DataView.RowFilter = v;
                    //}
                    //else if (content == "1")
                    //{
                    //    string v = String.Format("StartDate = # {0} # and StartDate>=# {1} #", DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/31/yyyy"));
                    //    DataView.RowFilter = v;
                    //}
                    //else if (content == "3")
                    //{
                    //    string v = String.Format("StartDate = # {0}# ", DateTime.Parse(value).ToString("MM/dd/yyyy"));
                    //    DataView.RowFilter = v;
                    //}
                    Vs.Clear();
                    v = "";
                    DataView.RowFilter = "";
                    RaisePropertyChanged("DataView");
                
                });
            }
        }







        private ObservableCollection<Model.IHierarchy<DateTime>> BuildDates(List<DateTime> dates)
        {
            var d = from date in dates
                    group date by date.Year into year
                    select new DateHierarchy
                    {
                        Level = 1,
                        IsChecked = false,

                        Value = new DateTime(year.Key, 1, 1),
                        Children = from date in year
                                   group date by date.Month into month
                                   select new DateHierarchy
                                   {
                                       Level = 2,
                                       IsChecked = false,
                                       Value = new DateTime(year.Key, month.Key, 1),
                                       Children = from day in month
                                                  select new DateHierarchy
                                                  {
                                                      Level = 3,
                                                      Value = day,
                                                      IsChecked = false,

                                                      Children = null
                                                  }
                                   }
                    };
            return new ObservableCollection<IHierarchy<DateTime>>(d);
        }
    }
    
  
}
