using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskDataGrid.Model
{
   public class FilterObj : ViewModelBase
    {

        private bool _isCheked;
        private string _Title;

        public string Title
        {
            get { return _Title; }
            set { _Title = value; RaisePropertyChanged("Title"); }
        }

        public bool IsChecked
        {
            get { return _isCheked; }
            set { _isCheked = value; RaisePropertyChanged("IsChecked"); }
        }


    }
}
