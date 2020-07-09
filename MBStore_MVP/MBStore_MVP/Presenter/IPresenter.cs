using MBStore_MVP.Model;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace MBStore_MVP.Presenter
{
    interface IEmp_Sing_up
    {
        List<Sign_up> GetList_Sign_Up_Emp();
        Sign_up Sign_Up_Data_Catch(Sign_up sign_up_data);
        void Upload_Emp_Sign_up(int emp_id, string rank, DateTime start_date, DateTime? end_date, Sign_up sign);
        void Delete_Sign_up(string login_id);
        Employee locate_Emp_Id(string rank1, string rank2);
    }

    interface IFindAddress
    {
        //empty
    }

    interface IInputCustomer
    {
        List<Customer> SelectCustomer(string number);
        int SelectStockProductStock(int stock_product);
        bool sell_transaction(List<Sell_Info> sell_list, int id, int employee_id, DateTime sales_date, long savings);
    }

    interface ILogin
    {
        string ComputeSha256Hash(string str);
        Employee SelectEmpId(string id);
    }

    interface INotice
    {
        bool Insert_Notice(int emp_id, DateTime date, string title, string text, string part);
    }

    interface IShoppingBasket
    {
        BitmapImage GetPhoneImage(string uri, string name, string color, string option, UriKind urikind);
        BitmapImage GetBrandImage(string uri, string brand, string option, UriKind urikind);
    }

    interface ISignUp
    {
        string ComputeSha256Hash(string str);
        bool Check_empID(string id);
        bool Insert_SignUp(string name, string id, string pw, string gender, string social_number, string phone, string post, string address, string email, DateTime sign_date);
    }

    interface IStockReturn
    {
        //empty
    }
    interface IMainWindow
    {
        #region 공지사항
        List<Notices> PrintNotice(string part);
        #endregion

        #region 파일 업로드
        void FtpUploadFile(string filename, string to_uri);

        #endregion

        #region 판매팀
        List<Product> SelectProduct(int product_id, int memory, int price, int ram, string name, string brand, DateTime manufacture, string query);
        List<string> SelectStockBrand();
        List<string> SelectStockName();
        List<int> SelectStockRam();
        List<int> SelectStockMemory();
        List<int> SelectStockProductId();
        List<string> SelectSalesname();
        List<Sell_Info> SelectSalesHistory(string customer_name, string employee_name, string type, DateTime sales_s_date, DateTime sales_e_date, int sales_history_id, string proudct_name, string query);
        bool SelectSalesHistoryRefunded(int sales_history_id);
        List<Sell_Info> SelectHistoryTotalPrice(int sales_history_id);
        bool Insert_Cus_Info(string name, string gender, DateTime? birth, string phone, long savings);
        bool Update_Cus_Info(int cus_id, string name, string gender, DateTime? birth, string phone, long savings);
        Customer Get_Cus_Id();
        List<Customer> GetList_Customer_Search(string name, string gender, string phone, string grammar);
        bool transaction_refund(int sales_history_id, int r_customer_id, int employee_id, DateTime sales_date, List<Sell_Info> refund_list, int s_customer_id, long savings);

        #endregion

        #region 물류팀
        List<string> Check_Lo_Reg_Overlap();
        List<Product> Get_Lo_Reg_RegistProductList();
        void Add_Lo_Reg_Product(string name, DateTime manufacture, string cpu, string inch, int mAh, int ram, string brand, int camera, int weight, Int64 price, string display, int memory);
        List<Product> Select_Lo_Pse_Product(int stock_product, int product_id, string product_name, string color, string query);
        List<Product> Get_Lo_Rse_ProductList(int productid, string tradetype, int tradehistoryID, string color, string name, DateTime startDay, DateTime endDay, string query);
        List<Int32> Get_Lo_Input_ProductNumList();
        List<Product> Get_Lo_Input_ProductColor(int product_id);
        bool DupleCheck_stock_product(int product_id, string color);
        List<Product> Select_Lo_Pse_stockProduct(string query);
        bool input_transaction(List<Product> inputdata, bool[] check, int[] newstock);
        string Select_productname_id(int product_id);
        Int32 Lo_Check_Stock(int id, string color);
        bool return_transacion(Return_Info return_Info, int eid);

        #endregion

        #region 지원팀
        List<Employee> GetList_Emp_info(string query, string login_id, string name, string phone, string gender, DateTime? start_date, DateTime? end_date);
        bool Update_Emp_Info(string login_id, string rank, string name, string gender, string social_num, string phone, string email, string address, DateTime? end_date);
        bool Reset_PW_EMP(string login_id);
        List<Sign_up> GetList_Sign_Up_Emp();
        List<Chart_Brsell> Get_Sell_Unit(string qr);
        List<Chart_SellKing> PieChart_Sell_King(string qr);
        List<Chart_Sellproduct> PieChart_Sell_Product(string qr);
        List<Basic_Chart_Sales> BChart_Sales_Product();
        Employee Get_Employee_info(int id);

        #endregion

    }
}
