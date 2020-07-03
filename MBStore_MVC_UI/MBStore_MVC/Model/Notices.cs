using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBStore_MVC.Model
{
    public class Notices
    {
        public int Notice_id { get; set; }
        public int Employee_id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }

        public string Url { get; set; }
    }
}
