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

        public List<IHierarchy<DateTime>> Children { get; set; } = new List<IHierarchy<DateTime>>();
        public bool IsChecked { get; set; } = true;
        public int Level { get; set; }
    }
    public interface IHierarchy<T>
    {
        object Value { get; set; }
        //  int Value { get; set; }
        List<IHierarchy<T>> Children { get; set; }
        bool IsChecked { get; set; }
        int Level { get; set; }
    }
}
