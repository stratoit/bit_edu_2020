using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBStore_MVVM.Model
{
    public class Sell_Info
    {
        public int Stock_product { get; set; }
        public int Product_id { get; set; }
        public string Product_name { get; set; }
        public string Color { get; set; }
        public string ColorValue { get; set; }
        public int Quantity { get; set; }
        public long Total_price { get; set; }
        public int Customer_id { get; set; }
        public string Customer_name { get; set; }
        public string Employee_name { get; set; }
        public DateTime Sales_date { get; set; }
        public int Sales_history_id { get; set; }
        public string Sales_type { get; set; }
        public bool Refunded { get; set; }
    }
}
