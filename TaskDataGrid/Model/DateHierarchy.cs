using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskDataGrid.Model
{
    public class DateHierarchy : IHierarchy<DateTime>
    {
        public object Value { get; set; }
        // public int Value { get; set; }

        public IEnumerable<IHierarchy<DateTime>> Children { get; set; }
        public bool IsChecked{ get; set; }
        public int Level { get; set; }
    }
    public interface IHierarchy<T>
    {
        object Value { get; set; }
        //  int Value { get; set; }
        IEnumerable<IHierarchy<T>> Children { get; set; }
        bool IsChecked { get; set; }
        int Level { get; set; }
    }
}
