using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBStore_MVP.Model;

namespace MBStore_MVP.Presenter
{
    class Pre_SignUp : ISignUp
    {
        #region Fileds
        private readonly ISignUp_view view;
        mbDB mbdb = new mbDB();
        #endregion

        #region Constructor
        public Pre_SignUp(ISignUp_view view)
        {
            this.view = view;
        }

        #endregion

        #region Create IPresenter method
        bool ISignUp.Check_empID(string id)
        {
            return mbdb.Check_empID(id);
        }
        bool ISignUp.Insert_SignUp(string name, string id, string pw, string gender, string social_number, string phone, string address, string email, DateTime sign_date)
        {
            return mbdb.Insert_SignUp(name, id, pw, gender, social_number, phone, address, email, sign_date);
        }
        #endregion
    }
}
