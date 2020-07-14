using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBStore_MVP.Model;

namespace MBStore_MVP.Presenter
{
    class Pre_Notice : INotice
    {
        #region Fileds
        private readonly INotice_view view;
        mbDB mbdb = new mbDB();
        #endregion

        #region Constructor
        public Pre_Notice(INotice_view view)
        {
            this.view = view;
        }
        #endregion

        #region Create IPresenter method

        public bool Insert_Notice(int emp_id, DateTime date, string title, string text, string part)
        {
            return mbdb.Insert_Notice(emp_id, date, title, text, part);
        }
        #endregion
    }
}
