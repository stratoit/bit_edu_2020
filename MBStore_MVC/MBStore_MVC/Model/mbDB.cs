using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MBStore_MVC.Model
{
    class mbDB
    {
        SqlConnection conn = null;
        public void Connect()
        {
            conn = new SqlConnection();
            conn.ConnectionString =
                ConfigurationManager.ConnectionStrings["UserDB"].ToString();
            conn.Open();    //  데이터베이스 연결           
        }

        public void Dispose()
        {
            MessageBox.Show("해제");   
        }

        public Employee SelectEmpId(string id)
        {
            Employee emp = new Employee();
            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "select login_id,login_pw,rank,name, employee_id from employee where login_id = @ID";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_id = new SqlParameter("@ID", id);
                cmd.Parameters.Add(param_id);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    try
                    {
                        myDataReader.Read();
                        emp.Login_id = myDataReader.GetString(0);
                        emp.Login_pw = myDataReader.GetString(1);
                        emp.Rank = myDataReader.GetString(2);
                        emp.Name = myDataReader.GetString(3);
                        emp.Employee_id = myDataReader.GetInt32(4);
                    }
                    catch (Exception e) { }
                    return emp;
                }
            }
        }
        public bool Check_empID(string id)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "select login_id from employee where login_id = @ID";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_id = new SqlParameter("@ID", id);
                cmd.Parameters.Add(param_id);

                SqlDataReader myDataReader = cmd.ExecuteReader();

                if (myDataReader.Read())
                    return false;
                else
                    return true;

            }
        }
        public bool Insert_SignUp(string name, string id, string pw, string gender, string social_number, string phone, string address, string email ,DateTime sign_date)
        {

            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결  

                string sql = "insert into signup(name,login_id,login_pw,gender,social_number,phone,address,email,sign_date) " +
                    "values(@id,@pw,@gender,@social_number,@phone,@address,@email,@sign_date)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_name = new SqlParameter("@name", name);
                cmd.Parameters.Add(param_name);

                SqlParameter param_id = new SqlParameter("@id", id);
                cmd.Parameters.Add(param_id);

                SqlParameter param_pw = new SqlParameter("@pw", pw);
                cmd.Parameters.Add(param_pw);

                SqlParameter param_gender = new SqlParameter("@gender", gender);
                cmd.Parameters.Add(param_gender);

                SqlParameter param_social = new SqlParameter("@social_number", social_number);
                cmd.Parameters.Add(param_social);

                SqlParameter param_phone = new SqlParameter("@phone", phone);
                cmd.Parameters.Add(param_phone);

                SqlParameter param_address = new SqlParameter("@address", address);
                cmd.Parameters.Add(param_address);

                SqlParameter param_email = new SqlParameter("@email", email);
                cmd.Parameters.Add(param_email);

                SqlParameter param_date = new SqlParameter("@sign_date", sign_date);
                param_date.SqlDbType = System.Data.SqlDbType.DateTime;
                cmd.Parameters.Add(param_date);

                //4. 쿼리문 실행
                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }


        #region 판매팀
        #region 판매팀
        //재고가 있는 물품 SELECT
        public List<Product> SelectProduct(int product_id, int memory, int price, int ram, string name, string brand, DateTime manufacture, string query)
        {
            List<Product> proList = new List<Product>();

            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "SELECT sp.stock_product, p.product_id,p.name,p.manufacture,sp.color,sp.color_value,p.brand,p.inch,p.display,p.mAh,p.ram,p.memory,p.camera,p.cpu,p.weight,p.price,sp.stock " +
                    "FROM product p JOIN stock_product sp ON p.product_id = sp.product_id WHERE sp.stock != 0 " + query + " ORDER BY p.product_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_id = new SqlParameter("@Product_id", product_id);
                param_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_id);

                SqlParameter param_memory = new SqlParameter("@Memory", memory);
                param_memory.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_memory);

                SqlParameter param_price = new SqlParameter("@Price", price);
                param_price.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_price);

                SqlParameter param_ram = new SqlParameter("@Ram", ram);
                param_ram.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_ram);

                SqlParameter param_name = new SqlParameter("@Name", "%" + name + "%");
                cmd.Parameters.Add(param_name);

                SqlParameter param_brand = new SqlParameter("@Brand", "%" + brand + "%");
                cmd.Parameters.Add(param_brand);

                SqlParameter param_manufacture = new SqlParameter("@Manufacture", manufacture);
                param_manufacture.SqlDbType = System.Data.SqlDbType.Date;
                cmd.Parameters.Add(param_manufacture);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        Product product = new Product();
                        product.Stock_product_id = myDataReader.GetInt32(0);
                        product.Product_id = myDataReader.GetInt32(1);
                        product.Name = myDataReader.GetString(2);
                        product.Manufacture = myDataReader.GetDateTime(3);
                        product.Color = myDataReader.GetString(4);
                        product.ColorValue = myDataReader.GetString(5);
                        product.Brand = myDataReader.GetString(6);
                        product.Inch = myDataReader.GetString(7);
                        product.Display = myDataReader.GetString(8);
                        product.MAh = myDataReader.GetInt32(9);
                        product.Ram = myDataReader.GetInt32(10);
                        product.Memory = myDataReader.GetInt32(11);
                        product.Camera = myDataReader.GetInt32(12);
                        product.Cpu = myDataReader.GetString(13);
                        product.Weight = myDataReader.GetInt32(14);
                        product.Price = myDataReader.GetInt64(15);
                        product.Stock = myDataReader.GetInt32(16);

                        proList.Add(product);
                    }
                }
            }
            return proList;
        }


        //핸드폰번호 뒷자리4개로 고객 SELECT
        public List<Customer> SearchCustomer(string number)
        {
            List<Customer> cusList = new List<Customer>();

            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "SELECT customer_id ,name, birth, gender, savings FROM customer WHERE phone Like @Number";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_number = new SqlParameter("@Number", "%" + number);
                cmd.Parameters.Add(param_number);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        Customer customer = new Customer();
                        customer.Id = myDataReader.GetInt32(0);
                        customer.Name = myDataReader.GetString(1);
                        customer.Date = myDataReader.GetDateTime(2);
                        customer.Gender = myDataReader.GetString(3);
                        customer.Savings = myDataReader.GetInt64(4);

                        cusList.Add(customer);
                    }
                }
            }
            return cusList;
        }

        //상품의 재고 SELECT
        public int SelectStockProductStock(int product_id, string color)
        {

            int stock = 0;

            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();
                string sql = "SELECT stock FROM stock_product WHERE product_id=@Product_id AND color=@Color";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_product_id = new SqlParameter("@Product_id", product_id);
                param_product_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_product_id);

                SqlParameter param_color = new SqlParameter("@Color", color);
                cmd.Parameters.Add(param_color);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        stock = myDataReader.GetInt32(0);
                    }
                    return stock;
                }
            }
        }


        //상품의 재고 UPDATE
        public bool UpdateStockProduct(int product_id, string color, int quantity)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();
                string sql = "UPDATE stock_product SET stock+=@Quantity WHERE stock_product=(SELECT stock_product FROM stock_product WHERE product_id=@Product_id AND color=@Color)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_product_id = new SqlParameter("@Product_id", product_id);
                param_product_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_product_id);

                SqlParameter param_color = new SqlParameter("@Color", color);
                cmd.Parameters.Add(param_color);

                SqlParameter param_quantity = new SqlParameter("@Quantity", quantity);
                param_quantity.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_quantity);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }



        //영수증(판매,환불) INSERT
        public bool InsertSalesHistroy(int customer_id, int employee_id, DateTime sales_date)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();
                string sql = "INSERT INTO sales_history(customer_id,employee_id,sales_date) VALUES (@Customer_id,@Employee_id,@Sales_date)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_customer_id = new SqlParameter("@Customer_id", customer_id);
                param_customer_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_customer_id);

                SqlParameter param_employee_id = new SqlParameter("@Employee_id", employee_id);
                param_employee_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_employee_id);

                SqlParameter param_sales_date = new SqlParameter("@Sales_date", sales_date);
                param_sales_date.SqlDbType = System.Data.SqlDbType.Date;
                cmd.Parameters.Add(param_sales_date);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }


        //영수증(판매,환불) id의 최댓값 SELECT
        public int SelectMaxHistoryId()
        {
            using (conn = new SqlConnection())
            {
                int max_history_id = -1;
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "SELECT MAX(sales_history_id) FROM sales_history";
                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {

                        max_history_id = myDataReader.GetInt32(0);
                    }
                    return max_history_id;
                }
            }
        }


        //거래(판매,환불)내역 Insert
        public bool InsertSalesProduct(int history_id, int product_id, int quantity, string color, string color_value, string type)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();
                string sql = "INSERT INTO sales_product(sales_history_id,product_id,quantity,color,color_value,type) VALUES (@History_id, @Product_id, @Quantity, @Color, @Color_value, @Type)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_history_id = new SqlParameter("@History_id", history_id);
                param_history_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_history_id);

                SqlParameter param_product_id = new SqlParameter("@Product_id", product_id);
                param_product_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_product_id);

                SqlParameter param_quantity = new SqlParameter("@Quantity", quantity);
                param_quantity.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_quantity);

                SqlParameter param_color = new SqlParameter("@Color", color);
                cmd.Parameters.Add(param_color);

                SqlParameter param_color_value = new SqlParameter("@Color_value", color_value);
                cmd.Parameters.Add(param_color_value);

                SqlParameter param_type = new SqlParameter("@Type", type);
                cmd.Parameters.Add(param_type);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }


        //거래(판매,환불) SELECT
        public List<Sell_Info> SelectSalesHistory(string customer_name, string employee_name, string type, DateTime sales_date, int sales_history_id, string query)
        {
            List<Sell_Info> saleshistoryList = new List<Sell_Info>();

            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "SELECT p.name, sp.color, sp.color_value, sh.sales_date, c.name, e.name, sp.type, sh.sales_history_id, sp.quantity, p.price * sp.quantity, p.product_id, sh.customer_id, sh.refunded" +
                            " FROM sales_history sh" +
                            " JOIN sales_product sp ON sh.sales_history_id = sp.sales_history_id" +
                            " JOIN product p ON sp.product_id = p.product_id" +
                            " JOIN customer c ON sh.customer_id = c.customer_id" +
                            " JOIN employee e ON sh.employee_id = e.employee_id " + query + " ORDER BY sh.sales_history_id DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_customer_name = new SqlParameter("@Customer_name", customer_name);
                cmd.Parameters.Add(param_customer_name);

                SqlParameter param_employee_name = new SqlParameter("@Employee_name", employee_name);
                cmd.Parameters.Add(param_employee_name);

                SqlParameter param_type = new SqlParameter("@Type", type);
                cmd.Parameters.Add(param_type);

                SqlParameter param_sales_date = new SqlParameter("@Sales_date", sales_date);
                param_sales_date.SqlDbType = System.Data.SqlDbType.Date;
                cmd.Parameters.Add(param_sales_date);

                SqlParameter param_sales_history_id = new SqlParameter("@Sales_hisory_id", sales_history_id);
                param_sales_history_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_sales_history_id);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        Sell_Info sell_info = new Sell_Info();

                        sell_info.Product_name = myDataReader.GetString(0);
                        sell_info.Color = myDataReader.GetString(1);
                        sell_info.ColorValue = myDataReader.GetString(2);
                        sell_info.Sales_date = myDataReader.GetDateTime(3);
                        sell_info.Customer_name = myDataReader.GetString(4);
                        sell_info.Employee_name = myDataReader.GetString(5);
                        sell_info.Sales_type = myDataReader.GetString(6);
                        sell_info.Sales_history_id = myDataReader.GetInt32(7);
                        sell_info.Quantity = myDataReader.GetInt32(8);
                        sell_info.Total_price = myDataReader.GetInt64(9);
                        sell_info.Product_id = myDataReader.GetInt32(10);
                        sell_info.Customer_id = myDataReader.GetInt32(11);
                        sell_info.Refunded = myDataReader.GetBoolean(12);

                        saleshistoryList.Add(sell_info);
                    }
                }
            }
            return saleshistoryList;
        }


        //환불로 인한 영수증 refund UPDATE
        public bool UpdateSalesHistory(int sales_history_id)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();
                string sql = "UPDATE sales_history SET refunded=1 WHERE sales_history_id = @Sales_history_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_sales_history_id = new SqlParameter("@Sales_history_id", sales_history_id);
                param_sales_history_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_sales_history_id);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }


        //영수증(판매,환불) refund SELECT
        public bool SelectSalesHistoryRefunded(int sales_history_id)
        {
            using (conn = new SqlConnection())
            {
                bool refunded = false;
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "SELECT refunded FROM sales_history WHERE sales_history_id = @Sales_history_id";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_sales_history_id = new SqlParameter("@Sales_history_id", sales_history_id);
                param_sales_history_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_sales_history_id);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        refunded = myDataReader.GetBoolean(0);
                    }
                    return refunded;
                }
            }
        }


        //고객 적립금 UPDATE
        public bool UpdateCustomerSavings(int customer_id, long savings)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();
                string sql = "UPDATE customer SET savings+=@Savings WHERE customer_id = @Customer_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_savings = new SqlParameter("@Savings", savings);
                param_savings.SqlDbType = System.Data.SqlDbType.BigInt;
                cmd.Parameters.Add(param_savings);

                SqlParameter param_customer_id = new SqlParameter("@Customer_id", customer_id);
                param_customer_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_customer_id);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }



        //판매 영수증 가격 SELECT
        public Sell_Info SelectHistoryTotalPrice(int sales_history_id)
        {
            using (conn = new SqlConnection())
            {
                Sell_Info sell_Info = new Sell_Info();
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "SELECT c.customer_id, sp.quantity*p.price FROM sales_history sh " +
                    "JOIN customer c ON sh.customer_id = c.customer_id " +
                    "JOIN sales_product sp ON sh.sales_history_id = sp.sales_history_id " +
                    "JOIN product p ON sp.product_id = p.product_id " +
                    "WHERE sh.sales_history_id = @Sales_history_id";
                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_sales_history_id = new SqlParameter("@Sales_history_id", sales_history_id);
                param_sales_history_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_sales_history_id);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        sell_Info.Customer_id = myDataReader.GetInt32(0);
                        sell_Info.Total_price = myDataReader.GetInt64(1);
                    }
                    return sell_Info;
                }
            }
        }
        #endregion

        #endregion

        #region 물류팀
        //함수 이름예시
        //_Lo_, _lo_ : 물류팀 (Logistics)
        //_Reg_, _reg_ : 제품등록 영역
        //_Io_, _io_ : 입고/반품 영역(Input/Output)
        //_Pse_, _pse_ : 제품 조회 영역(Product Search)
        //_Rse_, _rse_ : 내역 조회 영역(Receipt Search) ------- 미구현

        //여기부터 시작
        //제품 등록 영역에서 제품을 등록하는 함수
        public void Add_Lo_Reg_Product(string name, DateTime manufacture, string cpu, string inch, int mAh, int ram, string brand, int camera, int weight, Int64 price, string display, int memory)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();
                string sqlchk = "select count(*) from product where name = @NAME";

                SqlCommand cmd = new SqlCommand(sqlchk, conn);

                SqlParameter param_name = new SqlParameter("@NAME", name);
                cmd.Parameters.Add(param_name);

                SqlDataReader myDataReader = cmd.ExecuteReader();

                try
                {
                    if (myDataReader.Read())
                    {
                        if (myDataReader.GetInt32(0) > 0)
                        {
                            myDataReader.Close();
                            MessageBox.Show("이미 존재하는 기종입니다.");
                        }
                        else
                        {
                            myDataReader.Close();
                            string str = "insert into product values(@ID, @NAME, @MANUFACTURE, @CPU, @INCH, @MAH, @RAM, @BRAND, @CAMERA, @WEIGHT, @PRICE, @DISPLAY, @MEMORY)";
                            string mx = "select MAX(product_id) from product";
                            int i;
                            SqlCommand cmdmx = new SqlCommand(mx, conn);
                            myDataReader = cmdmx.ExecuteReader();
                            try
                            {
                                if (myDataReader.Read())
                                {
                                    i = myDataReader.GetInt32(0) + 1;
                                    myDataReader.Close();
                                }
                                else
                                {
                                    i = 100000;
                                    myDataReader.Close();
                                }

                                SqlParameter param_id = new SqlParameter("@ID", i);
                                param_id.SqlDbType = System.Data.SqlDbType.Int;
                                param_name = new SqlParameter("@NAME", name);
                                SqlParameter param_manufacture = new SqlParameter("@MANUFACTURE", manufacture);
                                param_manufacture.SqlDbType = System.Data.SqlDbType.Date;
                                SqlParameter param_cpu = new SqlParameter("@CPU", cpu);
                                SqlParameter param_inch = new SqlParameter("@INCH", inch);
                                SqlParameter param_mah = new SqlParameter("@MAH", mAh);
                                param_mah.SqlDbType = System.Data.SqlDbType.Int;
                                SqlParameter param_ram = new SqlParameter("@RAM", ram);
                                param_ram.SqlDbType = System.Data.SqlDbType.Int;
                                SqlParameter param_brand = new SqlParameter("@BRAND", brand);
                                SqlParameter param_camera = new SqlParameter("@CAMERA", camera);
                                param_camera.SqlDbType = System.Data.SqlDbType.Int;
                                SqlParameter param_weigt = new SqlParameter("@WEIGHT", weight);
                                param_weigt.SqlDbType = System.Data.SqlDbType.Int;
                                SqlParameter param_price = new SqlParameter("@PRICE", price);
                                param_price.SqlDbType = System.Data.SqlDbType.BigInt;
                                SqlParameter param_display = new SqlParameter("@DISPLAY", display);
                                SqlParameter param_memory = new SqlParameter("@MEMORY", memory);
                                param_memory.SqlDbType = System.Data.SqlDbType.Int;
                                SqlCommand cmdmain = new SqlCommand(str, conn);
                                cmdmain.Parameters.Add(param_id);
                                cmdmain.Parameters.Add(param_name);
                                cmdmain.Parameters.Add(param_manufacture);
                                cmdmain.Parameters.Add(param_cpu);
                                cmdmain.Parameters.Add(param_inch);
                                cmdmain.Parameters.Add(param_mah);
                                cmdmain.Parameters.Add(param_ram);
                                cmdmain.Parameters.Add(param_brand);
                                cmdmain.Parameters.Add(param_camera);
                                cmdmain.Parameters.Add(param_weigt);
                                cmdmain.Parameters.Add(param_price);
                                cmdmain.Parameters.Add(param_display);
                                cmdmain.Parameters.Add(param_memory);
                                cmdmain.ExecuteScalar();
                                MessageBox.Show("등록완료");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        //제품조회 영역에서 제품을 목록을 출력하는 함수
        public List<Product> Select_Lo_Pse_Product(int product_id, int memory, int price, int ram, string name, string brand, DateTime manufacture, string query)
        {
            List<Product> proList = new List<Product>();

            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "SELECT p.product_id,p.name,p.manufacture,sp.color,sp.color_value,p.brand,p.inch,p.display,p.mAh,p.ram,p.memory,p.camera,p.cpu,p.weight,p.price,sp.stock FROM product p JOIN stock_product sp ON p.product_id = sp.product_id " + query + " ORDER BY p.product_id";
                //string sql = "SELECT p.product_id,p.name,p.manufacture,sp.color,p.brand,p.inch,p.display,p.mAh,p.ram,p.memory,p.camera,p.cpu,p.weight,p.price,sp.stock FROM product p JOIN stock_product sp ON p.product_id = sp.product_id WHERE name Like @Name ORDER BY p.product_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_id = new SqlParameter("@Product_id", product_id);
                param_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_id);

                SqlParameter param_memory = new SqlParameter("@Memory", memory);
                param_memory.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_memory);

                SqlParameter param_price = new SqlParameter("@Price", price);
                param_price.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_price);

                SqlParameter param_ram = new SqlParameter("@Ram", ram);
                param_ram.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_ram);

                SqlParameter param_name = new SqlParameter("@Name", "%" + name + "%");
                cmd.Parameters.Add(param_name);

                SqlParameter param_brand = new SqlParameter("@Brand", "%" + brand + "%");
                cmd.Parameters.Add(param_brand);

                SqlParameter param_manufacture = new SqlParameter("@Manufacture", manufacture);
                param_manufacture.SqlDbType = System.Data.SqlDbType.Date;
                cmd.Parameters.Add(param_manufacture);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        Product product = new Product();
                        product.Product_id = myDataReader.GetInt32(0);
                        product.Name = myDataReader.GetString(1);
                        product.Manufacture = myDataReader.GetDateTime(2);
                        product.Color = myDataReader.GetString(3);
                        product.ColorValue = myDataReader.GetString(4);
                        product.Brand = myDataReader.GetString(5);
                        product.Inch = myDataReader.GetString(6);
                        product.Display = myDataReader.GetString(7);
                        product.MAh = myDataReader.GetInt32(8);
                        product.Ram = myDataReader.GetInt32(9);
                        product.Memory = myDataReader.GetInt32(10);
                        product.Camera = myDataReader.GetInt32(11);
                        product.Cpu = myDataReader.GetString(12);
                        product.Weight = myDataReader.GetInt32(13);
                        product.Price = myDataReader.GetInt64(14);
                        product.Stock = myDataReader.GetInt32(15);
                        proList.Add(product);
                    }
                }
            }
            return proList;
        }
        //입출고 영역에서 물품 목록을 불러오는 함수
        public List<Product> Get_Lo_Io_ProductList()
        {
            List<Product> productList = new List<Product>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                    conn.Open();    //  데이터베이스 연결   

                    string sql = "select distinct p.product_id, p.name, sp.color, sp.stock, sp.color_value from product as p, stock_product as sp, trade_history as th, trade_product as tp where p.product_id = sp.product_id order by p.product_id asc";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.Product_id = reader.GetInt32(0);
                            product.Name = reader.GetString(1);
                            product.Color = reader.GetString(2);
                            product.Stock = reader.GetInt32(3);
                            product.ColorValue = reader.GetString(4);
                            productList.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return productList;
        }
        //등록 영역에서 등록된 물품의 리스트를 출력하는 함수
        public List<Product> Get_Lo_Reg_RegistProductList()
        {
            List<Product> productList = new List<Product>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                    conn.Open();    //  데이터베이스 연결   

                    string sql = "select product_id, name, manufacture, cpu, inch, mAh, ram, brand, camera, weight, price, display, memory from product order by product_id asc";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product();
                            product.Product_id = reader.GetInt32(0);
                            product.Name = reader.GetString(1);
                            product.Manufacture = reader.GetDateTime(2);
                            product.Cpu = reader.GetString(3);
                            product.Inch = reader.GetString(4);
                            product.MAh = reader.GetInt32(5);
                            product.Ram = reader.GetInt32(6);
                            product.Brand = reader.GetString(7);
                            product.Camera = reader.GetInt32(8);
                            product.Weight = reader.GetInt32(9);
                            product.Price = reader.GetInt64(10);
                            product.Display = reader.GetString(11);
                            product.Memory = reader.GetInt32(12);
                            productList.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return productList;
        }
        //입출고 영역에서 사용하는 (현재 product 테이블에 등록된 물품번호의 목록을 가져오는) 함수
        public List<Int32> Get_Lo_Io_ProductNumList()
        {
            List<Int32> productList = new List<Int32>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                    conn.Open();    //  데이터베이스 연결   

                    string sql = "select distinct product_id from product order by product_id asc";

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int product;
                            product = reader.GetInt32(0);
                            productList.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return productList;
        }
        //입출고 영역에서 로그인한 사용자의 사번을 불러오는 함수
        public List<Int32> Get_Lo_Io_EmployeeID(string eid)
        {
            List<Int32> productList = new List<Int32>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                    conn.Open();    //  데이터베이스 연결   

                    string sql = "select employee_id from employee where name = @EID";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlParameter param_id = new SqlParameter("@EID", eid);
                    cmd.Parameters.Add(param_id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int product;
                            product = reader.GetInt32(0);
                            productList.Add(product);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return productList;
        }
        //물류 전 영역에서 입력을 할 때 사용되는 함수 (insert)
        public Product Set_Lo_Product(string sql)
        {
            Product productList = new Product();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                    conn.Open();    //데이터베이스 연결
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    try
                    {
                        int x = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return productList;
        }
        //물류 전 영역에서 영수증 및 기록을 추가하는 함수
        public Product Set_Lo_History(string sql)
        {
            Product productList = new Product();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                    conn.Open();    //데이터베이스 연결
                    SqlCommand cmd = new SqlCommand(sql, conn);

                    try
                    {
                        int x = cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return productList;
        }
        #endregion
    }
}
