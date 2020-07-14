using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBStore_MVP.Model;

namespace MBStore_MVP.Presenter
{
    class Pre_StockReturn : IStockReturn
    {
        #region Fileds
        private readonly IStockReturn_view view;
        mbDB mbdb = new mbDB();
        #endregion

        #region Constructor
        public Pre_StockReturn(IStockReturn_view view)
        {
            this.view = view;
        }

        #endregion

        #region Create IPresenter method
        
        #endregion


    }
}
