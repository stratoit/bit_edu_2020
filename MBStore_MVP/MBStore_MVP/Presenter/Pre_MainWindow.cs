using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using MBStore_MVP.Model;

namespace MBStore_MVP.Presenter
{
    class Pre_MainWindow : IMainWindow
    {
        #region Fileds
        private readonly IMainWindow_view view;
        mbDB mbdb = new mbDB();
        #endregion

        #region Constructor
        public Pre_MainWindow(IMainWindow_view view)
        {
            this.view = view;
        }


        #endregion

        #region Create IPresenter method

        #region 공지사항
        public List<Notices> PrintNotice(string part)
        {
            return mbdb.PrintNotice(part);
        }
        #endregion

        #region 파일업로드
        public void FtpUploadFile(string filename, string to_uri)
        {
            // Get the object used to communicate with the server.
            FtpWebRequest request =
                (FtpWebRequest)WebRequest.Create(to_uri.Replace("+","plus"));
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UsePassive = false;
            // Get network credentials.
            request.Credentials =
                new NetworkCredential("ftp_bit", "dlqtkgkwk12#$");

            // Read the file's contents into a byte array.
            byte[] bytes = System.IO.File.ReadAllBytes(filename);

            // Write the bytes into the request stream.
            request.ContentLength = bytes.Length;
            using (Stream request_stream = request.GetRequestStream())
            {
                request_stream.Write(bytes, 0, bytes.Length);
            }
        }
        #endregion

        #region 판매팀


        public List<Customer> GetList_Customer_Search(string name, string gender, string phone, string grammar)
        {
            return mbdb.GetList_Customer_Search(name, gender, phone, grammar);
        }

        public Customer Get_Cus_Id()
        {
            return mbdb.Get_Cus_Id();
        }

        public bool Insert_Cus_Info(string name, string gender, DateTime? birth, string phone, long savings)
        {
            return mbdb.Insert_Cus_Info(name, gender, birth, phone, savings);
        }

        public List<Sell_Info> SelectHistoryTotalPrice(int sales_history_id)
        {
            return mbdb.SelectHistoryTotalPrice(sales_history_id);
        }

        public List<Product> SelectProduct(int product_id, int memory, int price, int ram, string name, string brand, DateTime manufacture, string query)
        {
            return mbdb.SelectProduct(product_id, memory, price, ram, name, brand, manufacture, query);
        }

        public List<Sell_Info> SelectSalesHistory(string customer_name, string employee_name, string type, DateTime sales_s_date, DateTime sales_e_date, int sales_history_id, string product_name, string query)
        {
            return mbdb.SelectSalesHistory(customer_name, employee_name, type, sales_s_date, sales_e_date, sales_history_id, product_name, query);
        }

        public bool SelectSalesHistoryRefunded(int sales_history_id)
        {
            return mbdb.SelectSalesHistoryRefunded(sales_history_id);
        }

        public List<string> SelectSalesname()
        {
            return mbdb.SelectSalesname();
        }

        public List<string> SelectStockBrand()
        {
            return mbdb.SelectStockBrand();
        }

        public List<int> SelectStockMemory()
        {
            return mbdb.SelectStockMemory();
        }

        public List<string> SelectStockName()
        {
            return mbdb.SelectStockName();
        }

        public List<int> SelectStockProductId()
        {
            return mbdb.SelectStockProductId();
        }

        public List<int> SelectStockRam()
        {
            return mbdb.SelectStockRam();
        }

        public bool Update_Cus_Info(int cus_id, string name, string gender, DateTime? birth, string phone, long savings)
        {
            return mbdb.Update_Cus_Info(cus_id, name, gender, birth, phone, savings);
        }

        public bool transaction_refund(int sales_history_id, int r_customer_id, int employee_id, DateTime sales_date, List<Sell_Info> refund_list, int s_customer_id, long savings)
        {
            return mbdb.transaction_refund(sales_history_id, r_customer_id, employee_id, sales_date, refund_list, s_customer_id, savings);
        }

        #endregion

        #region 물류팀

        public List<string> Check_Lo_Reg_Overlap()
        {
            return mbdb.Check_Lo_Reg_Overlap();
        }
        public List<Product> Get_Lo_Reg_RegistProductList()
        {
            return mbdb.Get_Lo_Reg_RegistProductList();
        }

        public void Add_Lo_Reg_Product(string name, DateTime manufacture, string cpu, string inch, int mAh, int ram, string brand, int camera, int weight, long price, string display, int memory)
        {
            mbdb.Add_Lo_Reg_Product(name, manufacture, cpu, inch, mAh, ram, brand, camera, weight, price, display, memory);
        }

        public List<Product> Select_Lo_Pse_Product(int stock_product, int product_id, string product_name, string color, string query)
        {
            return mbdb.Select_Lo_Pse_Product(stock_product, product_id, product_name, color, query);
        }

        public List<Product> Get_Lo_Rse_ProductList(int productid, string tradetype, int tradehistoryID, string color, string name, DateTime startDay, DateTime endDay, string query)
        {
            return mbdb.Get_Lo_Rse_ProductList(productid, tradetype, tradehistoryID, color, name, startDay, endDay, query);
        }

        public List<int> Get_Lo_Input_ProductNumList()
        {
            return mbdb.Get_Lo_Input_ProductNumList();
        }

        public List<Product> Get_Lo_Input_ProductColor(int product_id)
        {
            return mbdb.Get_Lo_Input_ProductColor(product_id);
        }

        public bool DupleCheck_stock_product(int product_id, string color)
        {
            return mbdb.DupleCheck_stock_product(product_id, color);
        }

        public List<Product> Select_Lo_Pse_stockProduct(string query)
        {
            return mbdb.Select_Lo_Pse_stockProduct(query);
        }

        public string Select_productname_id(int product_id)
        {
            return mbdb.Select_productname_id(product_id);
        }

        public bool input_transaction(List<Product> inputdata, bool[] check, int[] newstock)
        {
            return mbdb.input_transaction(inputdata, check, newstock);
        }

        public int Lo_Check_Stock(int id, string color)
        {
            return mbdb.Lo_Check_Stock(id, color);
        }

        public bool return_transacion(Return_Info return_Info, int eid)
        {
            return mbdb.return_transacion(return_Info, eid);
        }


        #endregion

        #region 지원팀
        public List<Employee> GetList_Emp_info(string query, string login_id, string name, string phone, string gender, DateTime? start_date, DateTime? end_date)
        {
            return mbdb.GetList_Emp_info(query, login_id, name, phone, gender, start_date, end_date);
        }

        public bool Update_Emp_Info(string login_id, string rank, string name, string gender, string social_num, string phone, string email, string address, DateTime? end_date)
        {
            return mbdb.Update_Emp_Info(login_id, rank, name, gender, social_num, phone, email, address, end_date);
        }

        public bool Reset_PW_EMP(string login_id)
        {
            return mbdb.Reset_PW_EMP(login_id);
        }

        public List<Sign_up> GetList_Sign_Up_Emp()
        {
            return mbdb.GetList_Sign_Up_Emp();
        }

        public List<Chart_Brsell> Get_Sell_Unit(string qr)
        {
            return mbdb.Get_Sell_Unit(qr);
        }

        public List<Chart_SellKing> PieChart_Sell_King(string qr)
        {
            return mbdb.PieChart_Sell_King(qr);
        }

        public List<Chart_Sellproduct> PieChart_Sell_Product(string qr)
        {
            return mbdb.PieChart_Sell_Product(qr);
        }

        public List<Basic_Chart_Sales> BChart_Sales_Product()
        {
            return mbdb.BChart_Sales_Product();
        }

        public Employee Get_Employee_info(int id)
        {
            return mbdb.Get_Employee_info(id);
        }

        #endregion


        #endregion
    }
}
