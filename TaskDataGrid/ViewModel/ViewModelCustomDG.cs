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
        public ObservableCollection<FilterObj> ListFilterContent { get; set; } = new ObservableCollection<FilterObj>();
        private Dictionary<string, ObservableCollection<FilterObj>> filters = new Dictionary<string, ObservableCollection<FilterObj>>();

        private Dictionary<string, List<IHierarchy<DateTime>>> filtersDate = new Dictionary<string, List<IHierarchy<DateTime>>>();
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
        private List<string> stringfilter = new List<string>();

        private string Joined_sql_string_date = "";
        private List<string> ListSettingSqlFilter = new List<string>();
        private List<DateTime> DateTimes = new List<DateTime>();


        private bool _selected = true;
        public bool Selected { get 
            {
                if (_selected == true)
                {
                    SelectedDateCheckBox(true);

                    return _selected;
                }
                else
                {
                    SelectedDateCheckBox(false);
                }
                RaisePropertyChanged(nameof(Dates));
                return _selected; 
            } set { _selected = value; } }
        /// <summary>
        /// Метод для checkbox
        /// </summary>
        /// <param name="ischecked"></param>
        private void SelectedDateCheckBox(bool ischecked)
        {
            try 
            {            
                if (_dates != null)           
                {
                    for (int i = 0; i < _dates.Count; i++)
                    {
                        _dates[i].IsChecked = ischecked;
                        for (int j = 0; j < _dates[i].Children.Count; j++)
                        {
                            _dates[i].Children[j].IsChecked = ischecked;

                            for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                            {
                                _dates[i].Children[j].Children[k].IsChecked = ischecked;

                            }
                        }
                    }
                }
            }
            catch(Exception exp)
            {
                MessageBox.Show(exp.Message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
            }
        }

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

        public ViewModelCustomDG(DataTable DataTable, IEnumerable<int> listColumnHeader_PropertyDateTime = null)
        {

            this.listColumnHeader_PropertyDateTime = new List<int>(listColumnHeader_PropertyDateTime);

            this.DataTable = DataTable;
            DataView = this.DataTable.DefaultView;

        }

        #region все команды 
        /// <summary>
        /// Команда на открытие PopUp
        /// </summary>
        public RelayCommand<string> OpenPopup
        {
            get
            {

                return new RelayCommand<string>((string ColumnHeader_Property_Parameter) => {
                    try { 
                    #region инициализация всех свойств
                    ListFilterContent = new ObservableCollection<FilterObj>();
                    ColumnHeader_Property = ColumnHeader_Property_Parameter;
                    lbFilterVisibility = Visibility.Visible;
                    TreeVisibility = Visibility.Visible;
                    _dates = new List<IHierarchy<DateTime>>();
                    DateTimes = new List<DateTime>();
                    #endregion
                    //if (DataTable.Columns[ColumnHeader_Property].DataType == typeof(DateTime))
                    //{
                    //    _dates = new List<IHierarchy<DateTime>>();

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
                    #region Проверка на наличие даты в данных
                    if (listColumnHeader_PropertyDateTime.Contains(DataTable.Columns[ColumnHeader_Property].Ordinal))
                    {
                        _dates = new List<IHierarchy<DateTime>>();
                        foreach (DataRow item in DataTable.Rows)
                        {
                            DateTimes.Add(Convert.ToDateTime(item[ColumnHeader_Property]).Date);
                        }

                        _dates = BuildDates(DateTimes);
                        if (!filtersDate.ContainsKey(ColumnHeader_Property))
                        {
                            filtersDate.Add(ColumnHeader_Property, _dates);
                        }


                        _dates = filtersDate[ColumnHeader_Property];
                        lbFilterVisibility = Visibility.Collapsed;
                        ButtonVisibility = Visibility.Collapsed;

                        RaisePropertyChanged<IHierarchy<DateTime>>(nameof(Dates));

                    }
                    else
                    {
                        List<string> vs1 = new List<string>();
                        foreach (DataRow item in DataTable.Rows)
                        {
                            vs1.Add(item[ColumnHeader_Property].ToString());
                        }

                        vs1 = vs1.Distinct().ToList();
                        foreach (var item in vs1)
                        {
                            ListFilterContent.Add(new FilterObj { IsChecked = true, Title = item });
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
                    #endregion
                    RaisePropertyChanged("lbFilterVisibility");
                    RaisePropertyChanged("TreeVisibility");
                    RaisePropertyChanged("ButtonVisibility");
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }


                });
            }
        }
        /// <summary>
        /// Алгоритм фильтрации, срабатывает на нажатие кнопки Ок
        /// </summary>
        public RelayCommand StartSorted
        {
            get
            {
                return new RelayCommand(() =>
                {
                    try 
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
               
                       ///Заходим в словарь
                        for (int elemdic = 0; elemdic < filtersDate.Count; elemdic++)
                        {
                            ///заходим в года
                            for (int year = 0; year < filtersDate.ElementAt(elemdic).Value.Count; year++)
                            {
                            
                                if ((filtersDate.ElementAt(elemdic).Value[year].IsChecked==false)||(filtersDate.ElementAt(elemdic).Value[year].IsChecked == null))
                                {
                               
                                    int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).Month);
                                    ListSettingSqlFilter.Add(String.Format("([{0}] < # {1} # or [{0}] > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));

                                    //   Console.WriteLine(filtersDate.ElementAt(elemdic).Value[year].IsChecked+" || "+ filtersDate.ElementAt(elemdic).Value[year].Value);
                                }
                                else if(filtersDate.ElementAt(elemdic).Value[year].IsChecked==true)
                                {
                                    int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).Month);
                                    ListSettingSqlFilter.Remove(String.Format("([{0}] < # {1} # or [{0}] > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));

                                }


                                ///заходим в месяц
                                for (int month = 0; month < filtersDate.ElementAt(elemdic).Value[year].Children.Count; month++)
                                {

                                    if ((filtersDate.ElementAt(elemdic).Value[year].Children[month].IsChecked == false) || (filtersDate.ElementAt(elemdic).Value[year].Children[month].IsChecked == null))
                                    {
                                        int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).Month);
                                        ListSettingSqlFilter.Remove(String.Format("([{0}] < # {1} # or [{0}] > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));

                                        ListSettingSqlFilter.Add(String.Format("[{0}]  > # {1} # or [{0}] > #{2}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy")));

                                    }
                                    else
                                    {
                                        int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).Month);
                                        ListSettingSqlFilter.Remove(String.Format("([{0}] < # {1} # or [{0}] > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));
                                        ListSettingSqlFilter.Remove(String.Format("[{0}]  > # {1} # or [{0}] > #{2}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy")));

                                    }
                                    /// Просматриваем все дни
                                    for (int day = 0; day < filtersDate.ElementAt(elemdic).Value[year].Children[month].Children.Count; day++)
                                    {
                                        if ((filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].IsChecked==false) ||(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].IsChecked == null))
                                        {
                                            int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).Month);

                                            ListSettingSqlFilter.Remove(String.Format("([{0}] < # {1} # or [{0}] > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));
                                            ListSettingSqlFilter.Remove(String.Format("[{0}]  > # {1} # or [{0}] > #{2}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy")));
                                            ListSettingSqlFilter.Add(String.Format("[{0}]  <> # {1}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).ToString("MM/dd/yyyy")));

                                        }
                                        else
                                        {
                                            int colday = DateTime.DaysInMonth(DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).Year, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).Month);

                                            ListSettingSqlFilter.Remove(String.Format("([{0}] < # {1} # or [{0}] > # {2}#) and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("01/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Value.ToString()).ToString("12/" + colday.ToString() + "/yyyy")));
                                            ListSettingSqlFilter.Remove(String.Format("[{0}]  > # {1} # or [{0}] > #{2}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/01/yyyy"), DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Value.ToString()).ToString("MM/" + colday.ToString() + "/yyyy")));
                                            ListSettingSqlFilter.Remove(String.Format("[{0}]  <> # {1}#  and ", filtersDate.ElementAt(elemdic).Key, DateTime.Parse(filtersDate.ElementAt(elemdic).Value[year].Children[month].Children[day].Value.ToString()).ToString("MM/dd/yyyy")));

                                        }
                                    }
                                }
                            }  
                        }
      
                        string resultfiltertext = "";
                        foreach (var item in ListSettingSqlFilter)
                        {
                            Joined_sql_string_date += item;
                        }

                        int ii = Joined_sql_string_date.LastIndexOf("and");
                        if (ii >= 0)
                        {
                            Joined_sql_string_date = Joined_sql_string_date.Substring(0, ii) + Joined_sql_string_date.Substring(ii + "and".Length);
                        }

                        if ((filterstring.Length > 0) & (Joined_sql_string_date.Length > 0))
                        {
                            resultfiltertext = filterstring + " And ( " + Joined_sql_string_date + " )";
                        }
                        else if (filterstring.Length > 0 && Joined_sql_string_date.Length == 0)
                        {
                            resultfiltertext = filterstring;
                        }
                        else
                        {
                            resultfiltertext = Joined_sql_string_date;
                        }
                        DataView.RowFilter = resultfiltertext;
                        RaisePropertyChanged("DataView");

                        resultfiltertext = "";
                        filterstring = "";
                        Joined_sql_string_date = "";
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }
                });
                
            }
        }
        

        /// <summary>
        /// Поиск нужных данных по textbox
        /// </summary>
        public RelayCommand textsearchcommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    //if (_textsearch != null)
                    //{
                    //    ListFilterContent = new ObservableCollection<FilterObj>(ListFilterContent.Where(w => w.Title.IndexOf(_textsearch, StringComparison.OrdinalIgnoreCase) >= 0));

                    //}
                    //if (_textsearch == string.Empty)
                    //{

                    //    ListFilterContent = filters[ColumnHeader_Property];
                    //}

                    //RaisePropertyChanged("ListFilterContent");

                });
            }
        }
        /// <summary>
        /// Сортировка по убывание
        /// </summary>
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
        /// <summary>
        /// Сортировка по возрастанию
        /// </summary>
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
        /// <summary>
        /// Удаление Фильтра
        /// </summary>
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
                    ListSettingSqlFilter.Clear();
                    Joined_sql_string_date = "";
                    _selected = true;

                    DataView.RowFilter = "";
                    RaisePropertyChanged("DataView");
                });
            }
        }
       

        public RelayCommand CancelCommand
        {
            get
            {
                return new RelayCommand(()=> {
                    IsOpen = false;
                    RaisePropertyChanged(nameof(IsOpen));
                
                });
            }
        }

        #endregion


        #region События на нажатия CheckBox
        /// <summary>
        /// Событие на нажатие на дату checkbox=false
        /// </summary>
        public RelayCommand<DateHierarchy> Date_UnChecked
        {
            get
            {
                return new RelayCommand<DateHierarchy>((DateHierarchy sender) => {
                    try
                    { 
                        string level = sender.Level.ToString();
                        string value = sender.Value.ToString();
                     
                        int colday = DateTime.DaysInMonth(DateTime.Parse(value).Year, DateTime.Parse(value).Month);                 
                        if (level == "2")
                        {
                            for (int i = 0; i < _dates.Count; i++)
                            {
                                if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                                {
                                    _dates[i].IsChecked = null;
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
                             }
                        else if (level == "1")
                        {
                            for (int i = 0; i < _dates.Count; i++)
                            {
                                if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                                {
                                    _dates[i].IsThreeState = false;
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

                        }
                        else if (level == "3")
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
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }

                });
            }
        }                       
        bool dopM = true;
        bool dopY = true;

        /// <summary>
        /// Событие на нажатие на дату checkbox=true
        /// </summary>
        public RelayCommand<DateHierarchy> Date_Checked
        {
            get
            {
                return new RelayCommand<DateHierarchy>((DateHierarchy sender) => {
                    try 
                    { 
                        string level = sender.Level.ToString();
                        string value = sender.Value.ToString();
                        int countd = 0;
                        int countp = 0;

                        int countm = 0;
                        int countmp = 0;
                        int colday = DateTime.DaysInMonth(DateTime.Parse(value).Year, DateTime.Parse(value).Month);

                        //Месяц checkbox=true
                        if (level == "2"&& dopM==true&& dopY==true)
                        {
                            //Цикл на год
                            for (int i = 0; i < _dates.Count; i++)
                            {
                                if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                                {
                                    Console.WriteLine("4");
                                    _dates[i].IsChecked = true;
                                }
                                //Цикл на месяц
                                for (int j = 0; j < _dates[i].Children.Count; j++)
                                {
                                    if ((DateTime.Parse(_dates[i].Children[j].Value.ToString()).Year == DateTime.Parse(value).Year) & (DateTime.Parse(_dates[i].Children[j].Value.ToString()).Month == DateTime.Parse(value).Month))
                                    {
                                        Console.WriteLine("5");

                                        _dates[i].Children[j].IsChecked = true;

                                    }
                                    //Цикл на день
                                    for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                                    {
                                        if ((DateTime.Parse(_dates[i].Children[j].Children[k].Value.ToString()).Year == DateTime.Parse(value).Year))
                                        {
                                            Console.WriteLine("6");

                                            _dates[i].Children[j].Children[k].IsChecked = true;
                                           
                                        }
                                    }
                                }
                            }

                        }
                        //Год checkbox=true
                         if (level == "1" && dopM == true && dopY == true)
                        {
                            //Цикл на год
                            for (int i = 0; i < _dates.Count; i++)
                            {
                                if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                                {
                                    Console.WriteLine("1");
                                    _dates[i].IsChecked = true;
                                }
                                //Цикл на месяц
                                for (int j = 0; j < _dates[i].Children.Count; j++)
                                {
                                    if ((DateTime.Parse(_dates[i].Children[j].Value.ToString()).Year == DateTime.Parse(value).Year))
                                    {
                                        Console.WriteLine("2");

                                        _dates[i].Children[j].IsChecked = true;
                                    }
                                    //Цикл на день
                                    for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                                    {
                                        if ((DateTime.Parse(_dates[i].Children[j].Children[k].Value.ToString()).Year == DateTime.Parse(value).Year))
                                        {
                                            Console.WriteLine("3");

                                            _dates[i].Children[j].Children[k].IsChecked = true;

                                        }
                                    }
                                }
                            }

                        }
                         if (level == "3" && dopM == true && dopY == true)
                         {
                            for (int i = 0; i < _dates.Count; i++)
                            {
                                if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                                {
                                    
                                }

                                for (int j = 0; j < _dates[i].Children.Count; j++)
                                {
                                
                                    //Цикл на день
                                    for (int k = 0; k < _dates[i].Children[j].Children.Count; k++)
                                    {
                                        if (DateTime.Parse(_dates[i].Children[j].Children[k].Value.ToString()).Month == DateTime.Parse(value).Month)
                                        {

                                            if (_dates[i].Children[j].Children[k].IsChecked!=false)
                                            {
                                               _dates[i].Children[j].Children[k].IsChecked = _dates[i].Children[j].Children[k].IsChecked;

                                            }
                                            countd++;
                                            //Console.WriteLine(_dates[i].Children[j].Children[k].IsChecked+" | "+ _dates[i].Children[j].Children[k].Value);
                                            if (_dates[i].Children[j].Children[k].IsChecked == true)
                                            {
                                                //checkm = false;
                                                countp++;
                                            }

                                        }

                                    }
                                  
                                    if ((DateTime.Parse(_dates[i].Children[j].Value.ToString()).Month == DateTime.Parse(value).Month) & (DateTime.Parse(_dates[i].Children[j].Value.ToString()).Year == DateTime.Parse(value).Year))
                                    {
                                        if (countd == countp)
                                        {
                                            dopM = false;
                                         
                                            _dates[i].Children[j].IsChecked = true;
                                        }
                                    }
                                    if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                                    {
                                        countm++;
                                        if (_dates[i].Children[j].IsChecked == true)
                                        {
                                            countmp++;
                                        }
                                    }

                                    dopM = true;
                                }

                                  if (DateTime.Parse(_dates[i].Value.ToString()).Year == DateTime.Parse(value).Year)
                                 {
                                    if (countm== countmp)
                                    {
                                        dopY = false;

                                        _dates[i].IsChecked = true;
                                    }
                                }
                          
                            }
                            dopY = true;

                        }


                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.Message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                    }

                });
            }
        }

        /// <summary>
        /// Событие на нажатие на текст checkbox=true
        /// </summary>
        public RelayCommand<FilterObj> String_Checked
        {
            get
            {
                return new RelayCommand<FilterObj>((FilterObj sender) => {
                    stringfilter.Remove(string.Format("[{0}] <>'{1}' And ", ColumnHeader_Property, sender.Title));


                });
            }
        }
        /// <summary>
        /// Событие на нажатие на текст checkbox=false
        /// </summary>
        public RelayCommand<FilterObj> String_UnChecked
        {
            get
            {
                return new RelayCommand<FilterObj>((FilterObj sender) => {
                    stringfilter.Add(string.Format("[{0}] <>'{1}' And ", ColumnHeader_Property, sender.Title));



                });
            }
        }


        #endregion





        

        /// <summary>
        /// Преобразование списка дат в иерархию
        /// </summary>
        /// <param name="dates"></param>
        /// <returns></returns>
        private List<Model.IHierarchy<DateTime>> BuildDates(List<DateTime> dates)
        {
            try 
            { 
                List<DateHierarchy> list = new List<DateHierarchy>();

                DateHierarchy year = new DateHierarchy();
                DateHierarchy Month = new DateHierarchy();
                DateHierarchy Day = new DateHierarchy();
                foreach (var elementyear in dates.GroupBy(x => x.Year))
                {
                    year = new DateHierarchy();

                    year.Value = new DateTime(elementyear.Key, 1, 1);
                    year.Level = 1;
                    foreach (var elementmonth in elementyear.GroupBy(x => x.Month))
                    {

                        Month = new DateHierarchy();
                        Month.Value = new DateTime(elementyear.Key, elementmonth.Key, 1);


                        Month.Level = 2;
                        foreach (var elementday in elementmonth.GroupBy(x => x.Day))
                        {
                            Day = new DateHierarchy();

                            Day.Level = 3;
                            Day.Value = new DateTime(elementyear.Key, elementmonth.Key, elementday.Key);
                            Month.Children.Add(Day);
                        }



                        year.Children.Add(Month);

                    }
                    list.Add(year);
                }
                return new List<IHierarchy<DateTime>>(list);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message, "Error", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return new List<IHierarchy<DateTime>>();
            }
        }
    }


}
