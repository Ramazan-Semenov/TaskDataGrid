using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskDataGrid.Model
{
    public class DateHierarchy : ViewModelBase, IHierarchy<DateTime>
    {
        private object _Value;
        public object Value { get => _Value; set { _Value = value; RaisePropertyChanged(nameof(Value)); } }
        // public int Value { get; set; }

        public List<IHierarchy<DateTime>> Children { get; set; } = new List<IHierarchy<DateTime>>();
        private bool? _isChecked = true;
        public bool? IsChecked { get => _isChecked; set { _isChecked = value; RaisePropertyChanged(nameof(IsChecked)); } }

        private bool? _isThreeState = false;
        public bool? IsThreeState { get => _isThreeState; set { _isThreeState = value; RaisePropertyChanged(nameof(IsThreeState)); } }

        private int _level;
        public int Level { get => _level; set { _level = value; RaisePropertyChanged(nameof(Level)); } }
        //public bool? IsThreeState { get; set; } = false;
        //public object Value { get ; set ; }
        //public List<IHierarchy<DateTime>> Children { get; set; } = new List<IHierarchy<DateTime>>();
        //public bool? IsChecked { get; set; } = true;
        //public int Level { get ; set ; }
    }
    public interface IHierarchy<T> 
    {
        bool? IsThreeState { get; set; }
        object Value { get; set; }
        //  int Value { get; set; }
        List<IHierarchy<T>> Children { get; set; }
        bool? IsChecked { get; set; }
        int Level { get; set; }
    }
}
