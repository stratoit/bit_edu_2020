using System;
using System.Collections.Generic;
using MBStore_MVP.Model;
using MBStore_MVP.View;

namespace MBStore_MVP.Presenter
{
    class Pre_Emp_Sign_up : IEmp_Sing_up
    {
        #region Fileds
        private readonly IEmp_Sign_up_view view;
        mbDB mbdb = new mbDB();
        #endregion

        #region Constructor
        public Pre_Emp_Sign_up(IEmp_Sign_up_view view)
        {
            this.view = view;
        }

        #endregion

        #region Create IPresenter method
        public void Delete_Sign_up(string login_id)
        {
            mbdb.Delete_Sign_up(login_id);
        }

        public List<Sign_up> GetList_Sign_Up_Emp()
        {
            return mbdb.GetList_Sign_Up_Emp();
        }

        public Employee locate_Emp_Id(string rank1, string rank2)
        {
            return mbdb.locate_Emp_Id(rank1, rank2);
        }

        public Sign_up Sign_Up_Data_Catch(Sign_up sign_up_data)
        {
            return mbdb.Sign_Up_Data_Catch(sign_up_data);
        }

        public void Upload_Emp_Sign_up(int emp_id, string rank, DateTime start_date, DateTime? end_date, Sign_up sign)
        {
            mbdb.Upload_Emp_Sign_up(emp_id, rank, start_date, end_date, sign);
        }
        #endregion
    }
}
