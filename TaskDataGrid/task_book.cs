using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskDataGrid
{
    public class task_book
    {
        public int Number { get; set; }
        public DateTime Date_of_compilation { get; set; }
        public string from_whom { get; set; }
        public string task_type { get; set; }
        public string name_of_the_task { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public string executor { get; set; }
        public string priority { get; set; }
        public string status { get; set; }
        public string User { get; set; }
    }
}
