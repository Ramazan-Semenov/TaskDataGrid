using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
        public DataTable DataTable { get; set; } = new DataTable();
      public  DataView DataView { get; set; } = new DataView();
        private static string ColumnHeader_Property="";
        private List<int> listColumnHeader_PropertyDateTime = new List<int>();
        public Visibility lbFilterVisibility { get; set; } = Visibility.Visible;
        public Visibility TreeVisibility { get; set; } = Visibility.Visible;
        public Visibility ButtonVisibility { get; set; } = Visibility.Visible;
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
        public ViewModelCustomDG(DataTable DataTable,IEnumerable<int> listColumnHeader_PropertyDateTime=null)
        {
            //SqlConnection connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\lenovo\source\repos\ConsoleApp1\ConsoleApp1\Database1.mdf;Integrated Security=True;Connect Timeout=30");

            
            //connection.Open();
            //Console.WriteLine("Start1");
            //SqlCommand command = new SqlCommand("SELECT TOP(100) [Id] ,[FirstName] ,[LastName] ,[Manager] ,[Salary] ,[StartDate]  FROM [dbo].[Employe]", connection);

            //SqlDataAdapter adapter = new SqlDataAdapter(command);
            //adapter.Fill(DataTable);

            this.listColumnHeader_PropertyDateTime =new List<int>(listColumnHeader_PropertyDateTime);
            //connection.Close();
            //connection.Dispose();
            this.DataTable = DataTable;
            DataView =this.DataTable.DefaultView;
       
          //  BuildDates(new List<DateTime> { DateTime.Now, DateTime.Parse("17.12.2022"), DateTime.Parse("17.12.2021"), DateTime.Parse("10.01.2021") }) ;
        }
      //  public List<string> ListFilterContent { get; set; } = new List<string>();
       public RelayCommand<string> OpenPopup { get
            {
             
                return new RelayCommand<string>(( string ColumnHeader_Property_Parameter) => {
                    ListFilterContent = new ObservableCollection<FilterObj>();
                    ColumnHeader_Property = ColumnHeader_Property_Parameter;
                    lbFilterVisibility = Visibility.Visible;
                    TreeVisibility = Visibility.Visible;
                    Dates = new ObservableCollection<IHierarchy<DateTime>>();
                    DateTimes = new List<DateTime>();
                    //if (DataTable.Columns[ColumnHeader_Property].DataType == typeof(DateTime))
                    //{
                    //    _dates = new ObservableCollection<IHierarchy<DateTime>>();

                    //    foreach (DataRow item in DataTable.Rows)
                    //    {
                    //        DateTimes.Add(Convert.ToDateTime(item[ColumnHeader_Property]));
                    //    }
                    //    _dates = BuildDates(DateTimes);
                    //    if (!filtersDate.ContainsKey(ColumnHeader_Property))
                    //    {
                    //        filtersDate.Add(ColumnHeader_Property, _dates);
                    //    }
                    //    _dates = filtersDate[ColumnHeader_Property];
                    //    lbFilterVisibility = Visibility.Collapsed;
                    //    ButtonVisibility = Visibility.Collapsed;
                    //    RaisePropertyChanged("Dates");
                    //}
                    if (listColumnHeader_PropertyDateTime.Contains(DataTable.Columns[ColumnHeader_Property].Ordinal))
                    {
                        _dates = new ObservableCollection<IHierarchy<DateTime>>();
                        foreach (DataRow item in DataTable.Rows)
                        {
                            DateTimes.Add(Convert.ToDateTime(item[ColumnHeader_Property]).Date);
                           // MessageBox.Show(ColumnHeader_Property.ToString());
                        }
                        _dates = BuildDates(DateTimes);
                        if (!filtersDate.ContainsKey(ColumnHeader_Property))
                        {
                            filtersDate.Add(ColumnHeader_Property, _dates);
                        }
                        _dates = filtersDate[ColumnHeader_Property];
                        lbFilterVisibility = Visibility.Collapsed;
                        ButtonVisibility = Visibility.Collapsed;

                        RaisePropertyChanged("Dates");
                    }
                    else
                    {
                        List<string> vs1 = new List<string>();
                        foreach (DataRow item in DataTable.Rows)
                        {
                            vs1.Add(item[ColumnHeader_Property].ToString());
                            //   ListFilterContent.Add(new FilterObj { IsChecked = true, Title = item[ColumnHeader_Property].ToString() });
                        }

                       vs1= vs1.Distinct().ToList();
                        //  MessageBox.Show(.Count().ToString()) ;
                        foreach (var item in vs1)
                        {
                            ListFilterContent.Add(new FilterObj { IsChecked = true, Title = item });
                           // MessageBox.Show(item);
                        }

                        if (!filters.ContainsKey(ColumnHeader_Property))
                        {
                            filters.Add(ColumnHeader_Property, ListFilterContent);
                        }

                        ListFilterContent = filters[ColumnHeader_Property];
                        TreeVisibility = Visibility.Collapsed;
                        ButtonVisibility = Visibility.Visible;

                        RaisePropertyChanged("ListFilterContent");
                    }

                    RaisePropertyChanged("lbFilterVisibility");
                    RaisePropertyChanged("TreeVisibility");
                    RaisePropertyChanged("ButtonVisibility");



                });
            }
        }

        string filterstring = "";
        public RelayCommand StartSorted
        { 
            get
            {
                return new RelayCommand(()=>
                {
                    string filterstring = "";
                    foreach (var item in stringfilter)
                    {
                        filterstring += item;
                    }
                    int count = filterstring.Length;
                    int i = filterstring.LastIndexOf("And");
                    if (i >= 0)
                    {
                        filterstring = filterstring.Substring(0, i) + filterstring.Substring(i + "And".Length);
                    }
               //    MessageBox.Show(filterstring);

                    DataView.RowFilter = filterstring;
                    RaisePropertyChanged("DataView");
                    string newfiltdate="";
                    foreach (var item in _dates)
                    {
                        //MessageBox.Show(DateTime.Parse(item.Value.ToString()).ToString());
                        if (item.IsChecked==false)
                        {
                            int colday = DateTime.DaysInMonth(DateTime.Parse(item.Value.ToString()).Year, DateTime.Parse(item.Value.ToString()).Month);
                                Vs.Add(String.Format("{0} < # {1} # or {0} > # {2}# and ", ColumnHeader_Property, DateTime.Parse(item.Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(item.Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));
                            //      newfiltdate += String.Format("{0} >= # {1} # and {0} <=# {2}# Or ", ColumnHeader_Property, DateTime.Parse(item.Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(item.Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy"));

                        }
                        //Debug.WriteLine(item.Level);
                        foreach (var item2 in item.Children)
                        {
                            //Debug.WriteLine(item2.Level);
                            if (item2.IsChecked==false)
                            {
                                int colday = DateTime.DaysInMonth(DateTime.Parse(item2.Value.ToString()).Year, DateTime.Parse(item2.Value.ToString()).Month);
                                Vs.Remove(String.Format("{0} > # {1} # or {0} > # {2}# and ", ColumnHeader_Property, DateTime.Parse(item.Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(item2.Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));
                                Vs.Add(String.Format("{0}  > # {1} # or {0} > #{2}#  and ", ColumnHeader_Property, DateTime.Parse(item2.Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(item2.Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy")));

                                //    newfiltdate += String.Format("{0}  >= # {1} # and {0} <=#{2}#  Or ", ColumnHeader_Property, DateTime.Parse(item2.Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(item2.Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy"));
                            }

                            foreach (var item3 in item2.Children)
                            {
                                if (item3.IsChecked == false)
                                {
                                    int colday = DateTime.DaysInMonth(DateTime.Parse(item2.Value.ToString()).Year, DateTime.Parse(item2.Value.ToString()).Month);

                                    Vs.Remove(String.Format("{0} > # {1} # or {0} > # {2}# and ", ColumnHeader_Property, DateTime.Parse(item.Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(item.Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));
                                  var g=  Vs.Remove(String.Format("{0}  > # {1} # or {0} > #{2}#  and ", ColumnHeader_Property, DateTime.Parse(item2.Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(item2.Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy")));
                                    Vs.Add(String.Format("{0}  = # {1}#  and ", ColumnHeader_Property, DateTime.Parse(item3.Value.ToString()).ToString("MM/dd/yyyy")));
                                    Console.WriteLine(String.Format("{0}  = # {1}#  and ", ColumnHeader_Property, DateTime.Parse(item3.Value.ToString()).ToString("MM/dd/yyyy")));
                                   

                                }

                            }
                        }
                    }
                    string resultfiltertext = "";
                    foreach (var item in Vs)
                    {
                        v += item;
                    }

                    int ii = v.LastIndexOf("and");
                    if (ii >= 0)
                    {
                        v = v.Substring(0, ii) + v.Substring(ii + "and".Length);
                    }
                   
                    //MessageBox.Show(filterstring);
                    if ((filterstring.Length>0)&(v.Length>0))
                    {
                        resultfiltertext = filterstring + " And ( " + v+" )";
                    }
                    else if (filterstring.Length>0&&v.Length==0)
                    {
                        resultfiltertext = filterstring;
                    }
                    else
                    {
                        resultfiltertext = v;
                    }
                   MessageBox.Show(resultfiltertext);
                    DataView.RowFilter = resultfiltertext;
                    RaisePropertyChanged("DataView");

                    newfiltdate = "";
                    resultfiltertext = "";
                    filterstring = "";
                    v = "";
                 //   Vs.Clear();
                   // MessageBox.Show(filterstring + " Or " + newfiltdate);
                   //  DataView.RowFilter = filterstring + " Or " + newfiltdate;
                    //  Ok();
                });  
 

               
            }
        }

        private void Ok()
        {
            foreach (var item in Vs)
            {
                v += item;
            }

            int ii = v.LastIndexOf("Or");
            if (ii >= 0)
            {
                v = v.Substring(0, ii) + v.Substring(ii + "Or".Length);
            }

            DataView.RowFilter = v;
          //  MessageBox.Show(v);
            RaisePropertyChanged("DataView");
           v = "";
        }
        public RelayCommand textsearchcommand
        {
            get
            {
                return new RelayCommand(()=>
                {
                    if (_textsearch!=null)
                    {
                    ListFilterContent=new ObservableCollection<FilterObj>( ListFilterContent.Where(w => w.Title.IndexOf(_textsearch, StringComparison.OrdinalIgnoreCase) >= 0));

                    }
                     if (_textsearch==string.Empty)
                    {

                        ListFilterContent = filters[ColumnHeader_Property];
                    }

                    RaisePropertyChanged("ListFilterContent");

                });
            }
        }
      public string textsearch { 
            get {
              
                return _textsearch;
            } set {
                _textsearch = value;
              } }
        private string _textsearch=string.Empty;

        string v = "";
        List<string> Vs = new List<string>();

        public RelayCommand<DateHierarchy> Date_UnChecked
        {
            get
            {
                return new RelayCommand<DateHierarchy>((DateHierarchy sender)=> {
                    //string content = sender.Level.ToString();
                    //string value = sender.Value.ToString();
                    //int colday = DateTime.DaysInMonth(DateTime.Parse(value).Year, DateTime.Parse(value).Month);
                    //foreach (var item in Dates)
                    //{
                    //    foreach (var item2 in item.Children)
                    //    {
                    //        if (item2.IsChecked == true)
                    //        {
                    //            Debug.WriteLine(item2.Value + " | " + item2.IsChecked);

                    //        }
                    //    }
                       
                    //}
                    //if (content == "2")
                    //{

                    //    Vs.Remove(String.Format("{0} >= # {1} # and {0} <=# {2}# Or ", ColumnHeader_Property, DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/" + colday.ToString() + "/yyyy")));
                    //    Vs.Add(String.Format("{0}  >= # {1} # and {0} <=#{2}#  Or ", ColumnHeader_Property, DateTime.Parse(value).ToString("MM/01/yyyy"), DateTime.Parse(value).ToString("MM/" + colday.ToString() + "/yyyy")));
                    //}
                    //else if (content == "1")
                    //{
                       
                    //    Vs.Add(String.Format("{0} >= # {1} # and {0} <=# {2}# Or ", ColumnHeader_Property, DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/" + colday.ToString() + "/yyyy")));
                    //}
                    //else if (content == "3")
                    //{
                    //    //v += String.Format("StartDate = # {0}# Or ", DateTime.Parse(value).ToString("MM/dd/yyyy"));
                    //    Vs.Remove(String.Format("{0}  >= # {1} # and {0} <=# {2}# Or ", ColumnHeader_Property, DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/" + colday.ToString() + "/yyyy")));
                    //    Vs.Remove(String.Format("{0}  >= # {1} # and {0} <=#{2}#  Or ", ColumnHeader_Property, DateTime.Parse(value).ToString("MM/01/yyyy"), DateTime.Parse(value).ToString("MM/" + colday.ToString() + "/yyyy")));

                    //    Vs.Add(String.Format("{0}  = # {1}#  Or ", ColumnHeader_Property, DateTime.Parse(value).ToString("MM/dd/yyyy")));
                    //}

                    //Ok();
                });
            }
        } 
        public RelayCommand<DateHierarchy> Date_Checked
        {
            get
            {
                return new RelayCommand<DateHierarchy>((DateHierarchy sender)=> {

                    string content = sender.Level.ToString();
                   // MessageBox.Show(content);
                    string value = sender.Value.ToString();

                    int colday = DateTime.DaysInMonth(DateTime.Parse(value).Year, DateTime.Parse(value).Month);
                    foreach (var item in _dates)
                    {
                        Debug.WriteLine(item.Value + " | " + item.IsChecked);
                    }
                    if (content == "2")
                    {

                        Vs.Remove(String.Format("{0} > # {1} # or {0} > # {2}# and ", ColumnHeader_Property, DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/" + colday.ToString() + "/yyyy")));
                        //Vs.Add(String.Format("StartDate >= # {0} # and StartDate<=#{1}#  Or ", DateTime.Parse(value).ToString("MM/01/yyyy"), DateTime.Parse(value).ToString("MM/" + colday.ToString() + "/yyyy")));
                    }
                    else if (content == "1")
                    {

                       var g= Vs.Remove(String.Format("{0} < # {1} # or {0} > # {2}# and ", ColumnHeader_Property, DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/" + colday.ToString() + "/yyyy")));
                        Debug.WriteLine(g);
                    }
                    else if (content == "3")
                    {

                        Vs.Remove(String.Format("{0}  = # {1}#  and ", ColumnHeader_Property, DateTime.Parse(value).ToString("MM/dd/yyyy")));
                    }

                    //Ok();

                });
            }
        }
        List<string> stringfilter = new List<string>();
        public RelayCommand<FilterObj> String_Checked
        {
            get
            {
                return new RelayCommand<FilterObj>((FilterObj sender)=> {
                    stringfilter.Remove(string.Format("{0} <>'{1}' And ", ColumnHeader_Property, sender.Title));

                    // MessageBox.Show(sender..ToString());
                    //  RaisePropertyChanged("DataView");

                });
            }
        }
        public RelayCommand<FilterObj> String_UnChecked
        {
            get
            {
                return new RelayCommand<FilterObj>((FilterObj sender)=> {
                    stringfilter.Add(string.Format("{0} <>'{1}' And ", ColumnHeader_Property, sender.Title));


                    //  RaisePropertyChanged("DataView");

                });
            }
        }
        public RelayCommand SortDesk
        {
            get
            {
                return new RelayCommand(()=>
                {
                    DataView.Sort = ColumnHeader_Property+ " DESC";
                    RaisePropertyChanged("DataView");
                });
            }
        } 
        public RelayCommand SortAsc
        {
            get
            {
                return new RelayCommand(()=>
                {
                    DataView.Sort = ColumnHeader_Property+ " ASC";
                    RaisePropertyChanged("DataView");
                });
            }
        } 
        public RelayCommand SortDel
        {
            get
            {
                return new RelayCommand(()=>
                {
                    if (filters.ContainsKey(ColumnHeader_Property))
                    {
                        for (int i = 0; i < filters[ColumnHeader_Property].Count; i++)
                        {
                            filters[ColumnHeader_Property][i].IsChecked = true;
                        }
                    }
                   
                 
                    DataView.RowFilter = "";
                    RaisePropertyChanged("DataView");
                });
            }
        }







        private ObservableCollection<Model.IHierarchy<DateTime>> BuildDates(List<DateTime> dates)
        {
            List<DateHierarchy> list = new List<DateHierarchy>();

            DateHierarchy year = new DateHierarchy();
            DateHierarchy Month = new DateHierarchy();
            DateHierarchy Day = new DateHierarchy();
            foreach (var item in dates.GroupBy(x=>x.Year))
            {
                year = new DateHierarchy();

                year.Value = new DateTime(item.Key, 1, 1);
                year.Level = 1;
                foreach (var item2 in  item.GroupBy(x=>x.Month))
                {

                    Month = new DateHierarchy();
                    Month.Value = new DateTime(item.Key, item2.Key, 1);


                    Month.Level = 2;
                    foreach (var item3 in item2.GroupBy(x=>x.Day))
                    {
                        Day = new DateHierarchy();

                        Day.Level = 3;
                        Day.Value =  new DateTime(item.Key, item2.Key, item3.Key);
                        Month.Children.Add(Day);
                        Console.WriteLine(new DateTime(item.Key, item2.Key, item3.Key).ToString());
                    }
                  
               

                    year.Children.Add(Month);

                }
                //hierarchy.Children.Add(hierarchy1);
                list.Add(year);
            }


            //var d = from date in dates
            //        group date by date.Year into year
            //        select new DateHierarchy
            //        {
            //            Level = 1,
            //            IsChecked = false,

            //            Value = new DateTime(year.Key, 1, 1),
            //            Children = from date in year
            //                       group date by date.Month into month
            //                       select new DateHierarchy
            //                       {
            //                           Level = 2,
            //                           IsChecked = false,
            //                           Value = new DateTime(year.Key, month.Key, 1),
            //                           Children = from day in month
            //                                      select new DateHierarchy
            //                                      {
            //                                          Level = 3,
            //                                          Value = day,
            //                                          IsChecked = false,

            //                                          Children = null
            //                                      }
            //                       }
            //        };
            return new ObservableCollection<IHierarchy<DateTime>>(list);
        }
    }
    
  
}
