using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBStore_MVC
{
    public class Product
    {
        public int Stock_product { get; set; }
        public int Product_id { get; set; }
        public string Name { get; set; }
        public DateTime Manufacture { get; set; }
        public string Brand { get; set; }
        public string Inch { get; set; }
        public string Display { get; set; }
        public int MAh { get; set; }
        public int Ram { get; set; }
        public int Memory { get; set; }
        public int Camera { get; set; }
        public string Cpu { get; set; }
        public int Weight { get; set; }
        public long Price { get; set; }

        public string Color { get; set; }
        public string ColorValue { get; set; }
        public int Stock { get; set; }

        public int Employee_id { get; set; }
        public string Employee_name { get; set; }
        public int Trade_history_id { get; set; }
        public DateTime Trade_date { get; set; }
        public string Trade_type { get; set; }
        public string Image_dir { get; set; }
        


    }
}
