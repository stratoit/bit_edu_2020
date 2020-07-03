using MBStore_MVP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBStore_MVP.Presenter
{
    class Pre_Login : ILogin
    {
        #region Fileds
        private readonly ILogin_view view;
        mbDB mbdb = new mbDB();
        Sha256 sha256 = new Sha256();
        #endregion

        #region Constructor
        public Pre_Login(ILogin_view view)
        {
            this.view = view;
        }
        #endregion

        #region Create IPresenter method

        public string ComputeSha256Hash(string str)
        {
            return sha256.ComputeSha256Hash(str);
        }

        public Employee SelectEmpId(string id)
        {
            return mbdb.SelectEmpId(id);
        }
        #endregion
    }
}
