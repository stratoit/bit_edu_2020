using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MBStore_MVP.Model;

namespace MBStore_MVP.Presenter
{
    interface IEmp_Sign_up_view
    {
        void Show(Window owner);
        void Update_List();
        void Set_Update_Cost(Sign_up sign, MainWindow mainwindow);
    }

    interface IFindAddress_view
    {
        void Base_Message(Exception ex);
        void Base_Message(string messgage);
        void lfn_WEB_Cshap_Script2(object obj01, object obj02, object obj03, object obj04, object obj05, object obj06, object obj07, object obj08, object obj09
            , object obj10, object obj11, object obj12, object obj13, object obj14, object obj15, object obj16, object obj17, object obj18, object obj19
            , object obj20, object obj21, object obj22, object obj23, object obj24, object obj25, object obj26, object obj27, object obj28, object obj29
            , object obj30, object obj31, object obj32, object obj33);
        void lfn_dr(ref DataRow dr);
        void lfn_Cshap_WEB_initLayerPosition(int w, int h);
        void lfn_Cshap_WEB_Script2();
    }

    interface IInputCustomer_view
    {
        void SetProduct(MainWindow mainWindow, int userid);
    }
    
    interface ILogin_view
    {
        void func_login(string id, string pw);
    }

    interface INotice_view
    {
        void Show(Employee employee);
    }

    interface IShoppingBasket_view
    {
        void SetProduct(Product product, MainWindow mainWindow, string uri);
    }

    interface ISignUp_view
    {
        bool IsNumeric(string source);
        void Show(Window owner);
    }

    interface IStockReturn_view
    {
        void SetProduct(Product product, MainWindow mainWindow);
    }

    interface IMainWindow_view
    {
        void Print_Notice(string part);
        void Sort(string sortBy, ListSortDirection direction, ListView listview);

        #region 판매팀
        void Set_Sell_listview(Product product, int quantity);
        void Check_and2(int cnt);
        void su_cus_All_Clear2();
        #endregion

        #region 물류팀
        void Set_Lo_Return_listview(Product product, int quantity);
        #endregion

        #region 지원팀
        void Check_and(int cnt);
        void su_em_Reset_text();
        bool IsValidEmail(string email);
        bool V_check();
        void X_or_V();
        void su_cus_All_Clear();
        void Input_Pie_Chart_1(string a, int b);
        void Input_Pie_Chart_2(long a, string b);
        void Input_Pie_Chart_3(string a, int b);
        void Basic_Chart_AddItem(List<long> p, string title);
        void Setting_Chart();

        #endregion
    }
}
