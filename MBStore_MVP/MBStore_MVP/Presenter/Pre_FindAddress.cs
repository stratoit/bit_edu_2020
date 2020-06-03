using System;
using System.Collections.Generic;
using MBStore_MVP.Model;
using MBStore_MVP.View;

namespace MBStore_MVP.Presenter
{
    class Pre_FindAddress  : IFindAddress
    {
        #region Fileds
        private readonly IFindAddress_view view;
        mbDB mbdb = new mbDB();
        #endregion

        #region Constructor
        public Pre_FindAddress(IFindAddress_view view)
        {
            this.view = view;
        }

        #endregion

        #region Create IPresenter method

     
        #endregion
    }
}
