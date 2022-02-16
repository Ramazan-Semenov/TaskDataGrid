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
    public class ViewModelCustomDG : ViewModelBase
    {
        public DataTable DataTable { get; set; } = new DataTable();
        public DataView DataView { get; set; } = new DataView();
        private static string ColumnHeader_Property = "";
        private List<int> listColumnHeader_PropertyDateTime = new List<int>();
        public Visibility lbFilterVisibility { get; set; } = Visibility.Visible;
        public Visibility TreeVisibility { get; set; } = Visibility.Visible;
        public Visibility ButtonVisibility { get; set; } = Visibility.Visible;
        public bool IsOpen { get; set; } = false;
        Dictionary<string, ObservableCollection<FilterObj>> filters = new Dictionary<string, ObservableCollection<FilterObj>>();
        Dictionary<string, List<IHierarchy<DateTime>>> filtersDate = new Dictionary<string, List<IHierarchy<DateTime>>>();

        private List<DateTime> DateTimes = new List<DateTime>();


        private bool _selected = true;
        public bool Selected { get 
            {
               // MessageBox.Show(_selected.ToString());
                if (_selected == true)
                {
                    if (_dates!=null)
                    {
                        for (int i = 0; i < _dates.Count; i++)
                        {
                            _dates[i].IsChecked = true;
                            for (int j = 0; j < _dates[i].Children.Count; j++)
                            {
                                _dates[i].Children[j].IsChecked = true;

                                for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                                {
                                    _dates[i].Children[j].Children[k].IsChecked = true;

                                }
                            }
                        }
                    }
                   
                    
                    return _selected;
                }
                else
                {
                    for (int i = 0; i < _dates.Count; i++)
                    {
                        _dates[i].IsChecked = false;
                        for (int j = 0; j < _dates[i].Children.Count; j++)
                        {
                            _dates[i].Children[j].IsChecked = false;

                            for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                            {
                                _dates[i].Children[j].Children[k].IsChecked = false;

                            }
                        }
                    }
                    //_dates.AsParallel().ForAll((y) => { y.IsChecked = false; });
                    // return _selected;
                }
                RaisePropertyChanged(nameof(Dates));
                return _selected; 
            } set { _selected = value; } }


        private List<IHierarchy<DateTime>> _dates;
        public List<IHierarchy<DateTime>> Dates
        {

            get
            {
                return _dates;
            }
            set
            {

                _dates = value;
            }
        }

        public ObservableCollection<FilterObj> ListFilterContent { get; set; } = new ObservableCollection<FilterObj>();
        public ViewModelCustomDG(DataTable DataTable, IEnumerable<int> listColumnHeader_PropertyDateTime = null)
        {

            this.listColumnHeader_PropertyDateTime = new List<int>(listColumnHeader_PropertyDateTime);
            //connection.Close();
            //connection.Dispose();
            this.DataTable = DataTable;
            DataView = this.DataTable.DefaultView;

        }
        public RelayCommand<string> OpenPopup
        {
            get
            {

                return new RelayCommand<string>((string ColumnHeader_Property_Parameter) => {
                    ListFilterContent = new ObservableCollection<FilterObj>();
                    ColumnHeader_Property = ColumnHeader_Property_Parameter;
                    lbFilterVisibility = Visibility.Visible;
                    TreeVisibility = Visibility.Visible;
                    _dates = new List<IHierarchy<DateTime>>();
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
                        _dates = new List<IHierarchy<DateTime>>();
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

                       // MessageBox.Show("LL");

                        _dates = filtersDate[ColumnHeader_Property];
                        lbFilterVisibility = Visibility.Collapsed;
                        ButtonVisibility = Visibility.Collapsed;
                      
                        RaisePropertyChanged<IHierarchy<DateTime>>(nameof(Dates));
                 //       MessageBox.Show("yy");

                    }
                    else
                    {
                        List<string> vs1 = new List<string>();
                        foreach (DataRow item in DataTable.Rows)
                        {
                            vs1.Add(item[ColumnHeader_Property].ToString());
                            //   ListFilterContent.Add(new FilterObj { IsChecked = true, Title = item[ColumnHeader_Property].ToString() });
                        }

                        vs1 = vs1.Distinct().ToList();
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
                return new RelayCommand(() =>
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
               
                    string newfiltdate = "";
                   ///Заходим в словарь
                    for (int elemdic = 0; elemdic < filtersDate.Count; elemdic++)
                    {
                        ///заходим в года
                        for (int year = 0; year < filtersDate.ElementAt(elemdic).Value.Count; year++)
                        {
                            
                            if ((filtersDate.ElementAt(elemdic).Value[year].IsChecked==false)||(filtersDate.ElementAt(elemdic).Value[year].IsChecked == null))
                            {
                               
                                int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).Month);
                                Vs.Add(String.Format("({0} < # {1} # or {0} > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));
                             //   Console.WriteLine("--------" + filtersDate.ElementAt(elemdic).Key+" || год");

                                //   Console.WriteLine(filtersDate.ElementAt(elemdic).Value[year].IsChecked+" || "+ filtersDate.ElementAt(elemdic).Value[year].Value);
                            }
                            else if(filtersDate.ElementAt(elemdic).Value[year].IsChecked==true)
                            {
                                int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).Month);
                                Vs.Remove(String.Format("({0} < # {1} # or {0} > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));

                            }


                            ///заходим в месяц
                            for (int month = 0; month < filtersDate.ElementAt(elemdic).Value[year].Children.Count; month++)
                            {

                                if ((filtersDate.ElementAt(elemdic).Value[year].Children[month].IsChecked == false) || (filtersDate.ElementAt(elemdic).Value[year].Children[month].IsChecked == null))
                                {
                                    int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).Month);
                                    Vs.Remove(String.Format("({0} < # {1} # or {0} > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));

                                    Vs.Add(String.Format("{0}  > # {1} # or {0} > #{2}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy")));
                                  //  Console.WriteLine("--------" + filtersDate.ElementAt(elemdic).Key + " || месяц");

                                }
                                else
                                {
                                    int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).Month);
                                    Vs.Remove(String.Format("({0} < # {1} # or {0} > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));
                                    Vs.Remove(String.Format("{0}  > # {1} # or {0} > #{2}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy")));

                                }
                                /// Просматриваем все дни
                                for (int day = 0; day < filtersDate.ElementAt(elemdic).Value[year].Children[month].Children.Count; day++)
                                {
                                    if ((filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].IsChecked==false) ||(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].IsChecked == null))
                                    {
                                        int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).Month);

                                        Vs.Remove(String.Format("({0} < # {1} # or {0} > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));
                                        Vs.Remove(String.Format("{0}  > # {1} # or {0} > #{2}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy")));
                                        Vs.Add(String.Format("{0}  <> # {1}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).ToString("MM/dd/yyyy")));

                                    }
                                    else
                                    {
                                        int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).Month);

                                        Vs.Remove(String.Format("({0} < # {1} # or {0} > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));
                                        Vs.Remove(String.Format("{0}  > # {1} # or {0} > #{2}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy")));
                                        Vs.Remove(String.Format("{0}  <> # {1}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).ToString("MM/dd/yyyy")));

                                    }
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
                    if ((filterstring.Length > 0) & (v.Length > 0))
                    {
                        resultfiltertext = filterstring + " And ( " + v + " )";
                    }
                    else if (filterstring.Length > 0 && v.Length == 0)
                    {
                        resultfiltertext = filterstring;
                    }
                    else
                    {
                        resultfiltertext = v;
                    }
                   // MessageBox.Show(resultfiltertext);
                    DataView.RowFilter = resultfiltertext;
                    RaisePropertyChanged("DataView");

                    newfiltdate = "";
                    resultfiltertext = "";
                    filterstring = "";
                    v = "";
                   // Vs.Clear();
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
                return new RelayCommand(() =>
                {
                    if (_textsearch != null)
                    {
                        ListFilterContent = new ObservableCollection<FilterObj>(ListFilterContent.Where(w => w.Title.IndexOf(_textsearch, StringComparison.OrdinalIgnoreCase) >= 0));

                    }
                    if (_textsearch == string.Empty)
                    {

                        ListFilterContent = filters[ColumnHeader_Property];
                    }

                    RaisePropertyChanged("ListFilterContent");

                });
            }
        }
        public string textsearch
        {
            get
            {

                return _textsearch;
            }
            set
            {
                _textsearch = value;
            }
        }
        private string _textsearch = string.Empty;

        string v = "";
        List<string> Vs = new List<string>();

        public RelayCommand<DateHierarchy> Date_UnChecked
        {
            get
            {
                return new RelayCommand<DateHierarchy>((DateHierarchy sender) => {
                    string content = sender.Level.ToString();
                    // MessageBox.Show(content);
                    string value = sender.Value.ToString();

                    int colday = DateTime.DaysInMonth(DateTime.Parse(value).Year, DateTime.Parse(value).Month);
                 
                    if (content == "2")

                    {
                        for (int i = 0; i < _dates.Count; i++)
                        {
                            if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                            {
                              //  _dates[i].IsChecked = null;
                            }
                            for (int j = 0; j < _dates[i].Children.Count; j++)
                            {
                                if ((DateTime.Parse(_dates[i].Children[j].Value.ToString()).Year == DateTime.Parse(value).Year) & (DateTime.Parse(_dates[i].Children[j].Value.ToString()).Month == DateTime.Parse(value).Month))
                                {
                                    _dates[i].IsChecked = null;
                                    _dates[i].Children[j].IsChecked = false;

                                }
                                for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                                {
                                    if ((DateTime.Parse(_dates[i].Children[j].Children[k].Value.ToString()).Year == DateTime.Parse(value).Year)&(DateTime.Parse(_dates[i].Children[j].Children[k].Value.ToString()).Month == DateTime.Parse(value).Month))
                                    {
                                        _dates[i].Children[j].Children[k].IsChecked = false;

                                    }
                                }
                            }
                        }
                    //    Vs.Add(String.Format("{0} > # {1} # or {0} > # {2}# and ", ColumnHeader_Property, DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/" + colday.ToString() + "/yyyy")));
                        //Vs.Add(String.Format("StartDate >= # {0} # and StartDate<=#{1}#  Or ", DateTime.Parse(value).ToString("MM/01/yyyy"), DateTime.Parse(value).ToString("MM/" + colday.ToString() + "/yyyy")));
                    }
                    else if (content == "1")
                    {
                        for (int i = 0; i < _dates.Count; i++)
                        {
                            if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                            {
                                _dates[i].IsChecked = false;
                            }

                            for (int j = 0; j < _dates[i].Children.Count; j++)
                            {
                                if ((DateTime.Parse(_dates[i].Children[j].Value.ToString()).Year == DateTime.Parse(value).Year))
                                {
                                    _dates[i].Children[j].IsChecked = false;
                                    
                                }
                                for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                                {
                                    if ((DateTime.Parse(_dates[i].Children[j].Children[k].Value.ToString()).Year == DateTime.Parse(value).Year))
                                    {
                                        _dates[i].Children[j].Children[k].IsChecked = false;

                                    }
                                }
                            }
                        }

                      //  Vs.Add(String.Format("{0} < # {1} # or {0} > # {2}# and ", ColumnHeader_Property, DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/" + colday.ToString() + "/yyyy")));
                    }
                    else if (content == "3")
                    {


                        for (int i = 0; i < _dates.Count; i++)
                        {
                            if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                            {
                                _dates[i].IsChecked = null;
                            }

                            for (int j = 0; j < _dates[i].Children.Count; j++)
                            {
                                if ((DateTime.Parse(_dates[i].Children[j].Value.ToString()).Month == DateTime.Parse(value).Month) & (DateTime.Parse(_dates[i].Children[j].Value.ToString()).Year == DateTime.Parse(value).Year))
                                {
                                    _dates[i].Children[j].IsChecked = null;
                                }
                            }
                        }
                    }

                });
            }
        }
        public RelayCommand<DateHierarchy> Date_Checked
        {
            get
            {
                return new RelayCommand<DateHierarchy>((DateHierarchy sender) => {

                    string level = sender.Level.ToString();
                    string value = sender.Value.ToString();

                    int colday = DateTime.DaysInMonth(DateTime.Parse(value).Year, DateTime.Parse(value).Month);

                    //Месяц checkbox=true
                    if (level == "2")
                    {
                        //Цикл на год
                        for (int i = 0; i < _dates.Count; i++)
                        {
                            if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                            {
                                _dates[i].IsChecked = true;
                            }
                            //Цикл на месяц
                            for (int j = 0; j < _dates[i].Children.Count; j++)
                            {
                                if ((DateTime.Parse(_dates[i].Children[j].Value.ToString()).Year == DateTime.Parse(value).Year) & (DateTime.Parse(_dates[i].Children[j].Value.ToString()).Month == DateTime.Parse(value).Month))
                                {
                                    _dates[i].Children[j].IsChecked = true;

                                }
                                //Цикл на день
                                for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                                {
                                    if ((DateTime.Parse(_dates[i].Children[j].Children[k].Value.ToString()).Year == DateTime.Parse(value).Year))
                                    {
                                        _dates[i].Children[j].Children[k].IsChecked = true;

                                    }
                                }
                            }
                        }

                         //   Vs.Remove(String.Format("{0} > # {1} # or {0} > # {2}# and ", ColumnHeader_Property, DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/" + colday.ToString() + "/yyyy")));
                    }
                    //Год checkbox=true
                    else if (level == "1")
                    {
                        //Цикл на год
                        for (int i = 0; i < _dates.Count; i++)
                        {
                            if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                            {
                                _dates[i].IsChecked = true;
                            }
                            //Цикл на месяц
                            for (int j = 0; j < _dates[i].Children.Count; j++)
                            {
                                if ((DateTime.Parse(_dates[i].Children[j].Value.ToString()).Year == DateTime.Parse(value).Year))
                                {
                                    _dates[i].Children[j].IsChecked = true;
                                }
                                //Цикл на день
                                for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                                {
                                    if ((DateTime.Parse(_dates[i].Children[j].Children[k].Value.ToString()).Year == DateTime.Parse(value).Year))
                                    {
                                        _dates[i].Children[j].Children[k].IsChecked = true;

                                    }
                                }
                            }
                        }

                           // var g = Vs.Remove(String.Format("{0} < # {1} # or {0} > # {2}# and ", ColumnHeader_Property, DateTime.Parse(value).ToString("01/01/yyyy"), DateTime.Parse(value).ToString("12/" + colday.ToString() + "/yyyy")));
                    }
                    else if (level == "3")
                    {
                        for (int i = 0; i < _dates.Count; i++)
                        {
                            if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                            {
                               // _dates[i].IsChecked = true;
                            }

                            for (int j = 0; j < _dates[i].Children.Count; j++)
                            {
                                if ((DateTime.Parse(_dates[i].Children[j].Value.ToString()).Month == DateTime.Parse(value).Month) & (DateTime.Parse(_dates[i].Children[j].Value.ToString()).Year == DateTime.Parse(value).Year))
                                {
                                   // _dates[i].Children[j].IsChecked = true;
                                }
                                //Цикл на день
                                for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                                {
                                    if ((DateTime.Parse(_dates[i].Children[j].Children[k].Value.ToString()).Month == DateTime.Parse(value).Month))
                                    {
                                        if (_dates[i].Children[j].Children[k].IsChecked!=false)
                                        {
                                           _dates[i].Children[j].Children[k].IsChecked = _dates[i].Children[j].Children[k].IsChecked;

                                        }

                                    }
                                }
                            }
                        }
                    }


                });
            }
        }
        List<string> stringfilter = new List<string>();
        public RelayCommand<FilterObj> String_Checked
        {
            get
            {
                return new RelayCommand<FilterObj>((FilterObj sender) => {
                    stringfilter.Remove(string.Format("{0} <>'{1}' And ", ColumnHeader_Property, sender.Title));


                });
            }
        }
        public RelayCommand<FilterObj> String_UnChecked
        {
            get
            {
                return new RelayCommand<FilterObj>((FilterObj sender) => {
                    stringfilter.Add(string.Format("{0} <>'{1}' And ", ColumnHeader_Property, sender.Title));



                });
            }
        }
        public RelayCommand SortDesk
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DataView.Sort = ColumnHeader_Property + " DESC";
                    RaisePropertyChanged("DataView");
                });
            }
        }
        public RelayCommand SortAsc
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DataView.Sort = ColumnHeader_Property + " ASC";
                    RaisePropertyChanged("DataView");
                });
            }
        }
        public RelayCommand SortDel
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (filters.ContainsKey(ColumnHeader_Property))
                    {
                        for (int i = 0; i < filters[ColumnHeader_Property].Count; i++)
                        {
                            filters[ColumnHeader_Property][i].IsChecked = true;
                        }
                    }
                    Vs.Clear();
                    v = "";
                    _selected = true;

                    DataView.RowFilter = "";
                    RaisePropertyChanged("DataView");
                });
            }
        }







        private List<Model.IHierarchy<DateTime>> BuildDates(List<DateTime> dates)
        {
            List<DateHierarchy> list = new List<DateHierarchy>();

            DateHierarchy year = new DateHierarchy();
            DateHierarchy Month = new DateHierarchy();
            DateHierarchy Day = new DateHierarchy();
            foreach (var item in dates.GroupBy(x => x.Year))
            {
                year = new DateHierarchy();

                year.Value = new DateTime(item.Key, 1, 1);
                year.Level = 1;
                foreach (var item2 in item.GroupBy(x => x.Month))
                {

                    Month = new DateHierarchy();
                    Month.Value = new DateTime(item.Key, item2.Key, 1);


                    Month.Level = 2;
                    foreach (var item3 in item2.GroupBy(x => x.Day))
                    {
                        Day = new DateHierarchy();

                        Day.Level = 3;
                        Day.Value = new DateTime(item.Key, item2.Key, item3.Key);
                        Month.Children.Add(Day);
                    }



                    year.Children.Add(Month);

                }
                list.Add(year);
            }
            return new List<IHierarchy<DateTime>>(list);
        }
    }


}
