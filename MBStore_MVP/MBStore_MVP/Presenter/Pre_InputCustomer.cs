using MBStore_MVP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBStore_MVP.Presenter
{
    class Pre_InputCustomer : IInputCustomer
    {
        #region Fileds
        private readonly IInputCustomer_view view;
        mbDB mbdb = new mbDB();
        #endregion

        #region Constructor
        public Pre_InputCustomer(IInputCustomer_view view)
        {
            this.view = view;
        }
        #endregion

        #region Create IPresenter method
        public bool InsertSalesHistroy(int customer_id, int employee_id, DateTime sales_date, bool refunded)
        {
            return mbdb.InsertSalesHistroy(customer_id, employee_id, sales_date, refunded);
        }

        public bool InsertSalesProduct(int history_id, int product_id, int quantity, string color, string color_value, string type)
        {
            return mbdb.InsertSalesProduct(history_id, product_id, quantity, color, color_value, type);
        }

        public List<Customer> SelectCustomer(string number)
        {
            return mbdb.SelectCustomer(number);
        }

        public int SelectMaxHistoryId(int employee_id)
        {
            return mbdb.SelectMaxHistoryId(employee_id);
        }

        public int SelectStockProductStock(int stock_product)
        {
            return mbdb.SelectStockProductStock(stock_product);
        }

        public bool UpdateCustomerSavings(int customer_id, long savings)
        {
            return mbdb.UpdateCustomerSavings(customer_id, savings);
        }

        public bool UpdateStockProduct(int product_id, string color, int quantity)
        {
            return mbdb.UpdateStockProduct(product_id, color, quantity);
        }
        #endregion
    }
}
