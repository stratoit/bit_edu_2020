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

        public List<Customer> SelectCustomer(string number)
        {
            return mbdb.SelectCustomer(number);
        }

        public int SelectStockProductStock(int stock_product)
        {
            return mbdb.SelectStockProductStock(stock_product);
        }
        public bool sell_transaction(List<Sell_Info> sell_list, int id, int employee_id, DateTime sales_date, long savings)
        {
            return mbdb.sell_transaction(sell_list, id, employee_id, sales_date, savings);
        }

        #endregion
    }
}
