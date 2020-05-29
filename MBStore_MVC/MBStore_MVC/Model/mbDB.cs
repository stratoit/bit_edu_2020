using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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

        public bool Insert_Notice(int emp_id, DateTime date, string title, string text, string part)
        {

            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결  

                string sql = "insert into notice(employee_id,first_date,last_date,title,text,category,views) " +
                    "values(@empid,@date,@date,@title,@text,@part, 0)";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_empid = new SqlParameter("@empid", emp_id);
                param_empid.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_empid);

                SqlParameter param_date = new SqlParameter("@date", date);
                param_date.SqlDbType = System.Data.SqlDbType.DateTime;
                cmd.Parameters.Add(param_date);

                SqlParameter param_title = new SqlParameter("@title", title);
                cmd.Parameters.Add(param_title);

                SqlParameter param_text = new SqlParameter("@text", text);
                cmd.Parameters.Add(param_text);

                SqlParameter param_part = new SqlParameter("@part", part);
                cmd.Parameters.Add(param_part);
             

                //4. 쿼리문 실행
                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }

        public List<Notices> PrintNotice(string part)
        {
            List<Notices> noticeList = new List<Notices>();
            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "select n.notice_id, e.name, n.last_date, n.title, n.text, n.views " +
                    "from employee e join notice n on n.employee_id = e.employee_id " +
                    "where n.category = @part order by n.notice_id desc";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_part = new SqlParameter("@part", part);
                cmd.Parameters.Add(param_part);

                

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        Notices notice = new Notices();
                        notice.Notice_id = myDataReader.GetInt32(0);
                        notice.Name = myDataReader.GetString(1);
                        notice.Date = myDataReader.GetDateTime(2);
                        notice.Title = myDataReader.GetString(3);
                        notice.Text = myDataReader.GetString(4);
                        notice.Views = myDataReader.GetInt32(5);


                        noticeList.Add(notice);
                    }
                }
            }
            return noticeList;
        }

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
                    "FROM product p JOIN stock_product sp ON p.product_id = sp.product_id WHERE sp.stock != 0 " + query + " ORDER BY sp.stock_product";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_product_id = new SqlParameter("@Product_id", product_id);
                param_product_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_product_id);

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
                        product.Stock_product = myDataReader.GetInt32(0);
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

        // 재고가 있는 물품의 Brand SELECT
        public List<string> SelectStockBrand()
        {
            List<string> brandList = new List<string>();
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                conn.Open();    //  데이터베이스 연결   

                string sql = "select DISTINCT p.brand from product p join stock_product sp on p.product_id = sp.product_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string brand;
                        brand = reader.GetString(0);
                        brandList.Add(brand);
                    }
                }
            }
            return brandList;
        }

        // 재고가 있는 물품의 Name SELECT
        public List<string> SelectStockName()
        {
            List<string> productList = new List<string>();
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                conn.Open();    //  데이터베이스 연결   

                string sql = "select DISTINCT p.name from product p join stock_product sp on p.product_id = sp.product_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name;
                        name = reader.GetString(0);
                        productList.Add(name);
                    }
                }
            }
            return productList;
        }

        // 재고가 있는 물품의 RAM SELECT
        public List<int> SelectStockRam()
        {
            List<int> ramList = new List<int>();
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                conn.Open();    //  데이터베이스 연결   

                string sql = "select DISTINCT p.ram from product p join stock_product sp on p.product_id = sp.product_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int ram;
                        ram = reader.GetInt32(0);
                        ramList.Add(ram);
                    }
                }
            }
            return ramList;
        }

        // 재고가 있는 물품의 Memory SELECT
        public List<int> SelectStockMemory()
        {
            List<int> memoryList = new List<int>();
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                conn.Open();    //  데이터베이스 연결   

                string sql = "select DISTINCT p.memory from product p join stock_product sp on p.product_id = sp.product_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int memory;
                        memory = reader.GetInt32(0);
                        memoryList.Add(memory);
                    }
                }
            }
            return memoryList;
        }

        // 재고가 있는 물품의 Product_id SELECT
        public List<int> SelectStockProductId()
        {
            List<int> idList = new List<int>();
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                conn.Open();    //  데이터베이스 연결   

                string sql = "select DISTINCT p.product_id from product p join stock_product sp on p.product_id = sp.product_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id;
                        id = reader.GetInt32(0);
                        idList.Add(id);
                    }
                }
            }
            return idList;
        }

        // 판매 내역의 제품의 Name SELECT
        public List<string> SelectSalesname()
        {
            List<string> nameList = new List<string>();
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                conn.Open();    //  데이터베이스 연결   

                string sql = "SELECT DISTINCT p.name FROM sales_product sp JOIN product p ON sp.product_id = p.product_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string name;
                        name = reader.GetString(0);
                        nameList.Add(name);
                    }
                }
            }
            return nameList;
        }

        //핸드폰번호 뒷자리4개로 고객 SELECT
        public List<Customer> SelectCustomer(string number)
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
        public int SelectStockProductStock(int stock_product)
        {

            int stock = 0;

            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();
                string sql = "SELECT stock FROM stock_product WHERE stock_product = @Stock_product";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_stock_productd = new SqlParameter("@Stock_product", stock_product);
                param_stock_productd.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_stock_productd);

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
        public bool InsertSalesHistroy(int customer_id, int employee_id, DateTime sales_date, Boolean refunded)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();
                string sql = "INSERT INTO sales_history(customer_id,employee_id,sales_date, refunded) VALUES (@Customer_id,@Employee_id,@Sales_date, @Refunded)";
                
                //string sql;
                //if(refunded)
                //    sql = "INSERT INTO sales_history(customer_id,employee_id,sales_date, refunded) VALUES (@Customer_id,@Employee_id,@Sales_date, 0)";
                //else
                //    sql = "INSERT INTO sales_history(customer_id,employee_id,sales_date, refunded) VALUES (@Customer_id,@Employee_id,@Sales_date, 1)";

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

                SqlParameter param_refunded = new SqlParameter("@Refunded", refunded);
                param_refunded.SqlDbType = System.Data.SqlDbType.Bit;
                cmd.Parameters.Add(param_refunded);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }


        //영수증(판매,환불) id의 최댓값 SELECT
        public int SelectMaxHistoryId(int employee_id)
        {
            using (conn = new SqlConnection())
            {
                int max_history_id = -1;
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "SELECT MAX(sales_history_id) FROM sales_history WHERE employee_id = @Employee_id";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_employee_id = new SqlParameter("@Employee_id", employee_id);
                param_employee_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_employee_id);

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
        public List<Sell_Info> SelectSalesHistory(string customer_name, string employee_name, string type, DateTime sales_s_date, DateTime sales_e_date, int sales_history_id, string proudct_name, string query)
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

                SqlParameter param_sales_s_date = new SqlParameter("@Sales_s_date", sales_s_date);
                param_sales_s_date.SqlDbType = System.Data.SqlDbType.Date;
                cmd.Parameters.Add(param_sales_s_date);

                SqlParameter param_sales_e_date = new SqlParameter("@Sales_e_date", sales_e_date);
                param_sales_e_date.SqlDbType = System.Data.SqlDbType.Date;
                cmd.Parameters.Add(param_sales_e_date);

                SqlParameter param_sales_history_id = new SqlParameter("@Sales_hisory_id", sales_history_id);
                param_sales_history_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_sales_history_id);

                SqlParameter param_product_name = new SqlParameter("@Product_name", proudct_name);
                cmd.Parameters.Add(param_product_name);

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



        #region 물류팀
        //함수 이름예시
        //_Lo_, _lo_ : 물류팀 (Logistics)
        //_Reg_, _reg_ : 제품등록 영역
        //_Input_, _input_ : 입고 영역(Input)
        //_Pse_, _pse_ : 제품 조회 영역(Product Search)
        //_Rse_, _rse_ : 내역 조회 영역(Receipt Search)
        //_refund_ : 반품 영역

        #region 제품등록
        //제품등록 : 제품등록 함수
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
        //제품등록 : 등록된 물품의 리스트 출력
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

        #endregion

        #region 제품조회
        //제품조회 : 제품목록 출력
        public List<Product> Select_Lo_Pse_Product(int stock_product, int product_id, string product_name, string color, string query)
        {
            List<Product> proList = new List<Product>();

            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결     
                string sql = "select sp.stock_product, sp.product_id, p.name, sp.color, sp.color_value, sp.stock from stock_product as sp join product as p on sp.product_id = p.product_id" + query;
                //string sql = "select sp.stock_product, p.product_id, p.name, tp.color, tp.color_value, sp.stock from trade_product as tp join product as p on tp.product_id = p.product_id join stock_product as sp on sp.product_id = p.product_id where sp.stock > 0 " + query;

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_stock_product = new SqlParameter("@Stock_product", stock_product);
                param_stock_product.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_stock_product);

                SqlParameter param_product_id = new SqlParameter("@Product_id", product_id);
                param_product_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_product_id);

                SqlParameter param_name = new SqlParameter("@Name", "%" + product_name + "%");
                param_name.SqlDbType = System.Data.SqlDbType.VarChar;
                cmd.Parameters.Add(param_name);

                SqlParameter param_color = new SqlParameter("@Color", "%" + color + "%");
                param_color.SqlDbType = System.Data.SqlDbType.VarChar;
                cmd.Parameters.Add(param_color);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        Product product = new Product();
                        product.Stock_product = myDataReader.GetInt32(0);
                        product.Product_id = myDataReader.GetInt32(1);
                        product.Name = myDataReader.GetString(2);
                        product.Color = myDataReader.GetString(3);
                        product.ColorValue = myDataReader.GetString(4);
                        product.Stock = myDataReader.GetInt32(5);
                        proList.Add(product);
                    }
                }
            }
            return proList;
        }

        #endregion

        #region 내역조회
        //내역조회 : 물품 목록 출력
        public List<Product> Get_Lo_Rse_ProductList(int productid, string tradetype, int tradehistoryID, string color, string name, DateTime startDay, DateTime endDay, string query)
        {
            List<Product> productList = new List<Product>();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                    conn.Open();    //  데이터베이스 연결   

                    string sql = "select tp.trade_type, tp.trade_history_id, tp.product_id, p.name, tp.color, tp.color_value, p.price, tp.quantity, th.trade_date, emp.employee_id, emp.name from employee as emp, trade_product as tp, product as p, trade_history as th where tp.product_id = p.product_id and th.employee_id = emp.employee_id and th.trade_history_id = tp.trade_history_id" + query;

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlParameter param_id = new SqlParameter("@Product_id", productid);
                    param_id.SqlDbType = System.Data.SqlDbType.Int;
                    cmd.Parameters.Add(param_id);

                    SqlParameter param_type = new SqlParameter("@Trade_type", tradetype);
                    param_type.SqlDbType = System.Data.SqlDbType.VarChar;
                    cmd.Parameters.Add(param_type);

                    SqlParameter param_historyid = new SqlParameter("@Trade_history_id", tradehistoryID);
                    param_historyid.SqlDbType = System.Data.SqlDbType.Int;
                    cmd.Parameters.Add(param_historyid);

                    SqlParameter param_color = new SqlParameter("@Color", "%" + color + "%");
                    param_color.SqlDbType = System.Data.SqlDbType.VarChar;
                    cmd.Parameters.Add(param_color);

                    SqlParameter param_name = new SqlParameter("@Name", "%" + name + "%");
                    param_name.SqlDbType = System.Data.SqlDbType.VarChar;
                    cmd.Parameters.Add(param_name);

                    SqlParameter param_startDay = new SqlParameter("@Start_date", startDay);
                    param_startDay.SqlDbType = System.Data.SqlDbType.Date;
                    cmd.Parameters.Add(param_startDay);

                    SqlParameter param_endDay = new SqlParameter("@End_date", endDay);
                    param_endDay.SqlDbType = System.Data.SqlDbType.Date;
                    cmd.Parameters.Add(param_endDay);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product();

                            product.Trade_type = reader.GetString(0);
                            product.Trade_history_id = reader.GetInt32(1);
                            product.Product_id = reader.GetInt32(2);
                            product.Name = reader.GetString(3);
                            product.Color = reader.GetString(4);
                            product.ColorValue = reader.GetString(5);
                            product.Price = reader.GetInt64(6);
                            product.Stock = reader.GetInt32(7);
                            product.Trade_date = reader.GetDateTime(8);
                            product.Employee_id = reader.GetInt32(9);
                            product.Employee_name = reader.GetString(10);
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

        #endregion

        #region 입고
        //입고 : 등록물품 목록 출력
        public List<Product> Get_Lo_Input_ProductList()
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
        //입고 : 현재 product 테이블에 등록된 물품번호의 목록 출력
        public List<Int32> Get_Lo_Input_ProductNumList()
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

        //입고 : insert 작업 시 사용
        public Product Set_Lo_Input_Product(string sql)
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
        //입고 : 영수증 및 기록 추가
        public Product Set_Lo_Input_History(string sql)
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

        #region 반품
        //반품 : 남은 재고 확인
        public Int32 Lo_Check_Stock(int id, string color)
        {
            Int32 i = new Int32();
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                    conn.Open();
                    string sql = "select stock from stock_product where product_id = @ID and color = @Color";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlParameter param_id = new SqlParameter("@ID", id);
                    SqlParameter param_color = new SqlParameter("@Color", color);
                    param_id.SqlDbType = System.Data.SqlDbType.Int;
                    cmd.Parameters.Add(param_id);
                    cmd.Parameters.Add(param_color);
                    SqlDataReader sqlDataReader = cmd.ExecuteReader();
                    while (sqlDataReader.Read())
                        i = sqlDataReader.GetInt32(0);
                    conn.Close();
                    return i;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }
        //반품 : 반품작업 수행
        public bool Set_Lo_Return_Stock(Return_Info return_Info, int eid)
        {
            try
            {
                using (conn = new SqlConnection())
                {
                    conn.ConnectionString = ConfigurationManager.ConnectionStrings["userDB"].ToString();
                    conn.Open();
                    string sql = "update stock_product SET stock-=@QUANTITY WHERE stock_product = (SELECT stock_product FROM stock_product WHERE product_id=@Product_id AND color=@Color)";
                    string sql2 = "insert into trade_history values(@EMPLOYEE_ID, @TRADE_DATE)";
                    string sql3 = "insert into trade_product values(@Product_id, (SELECT MAX(trade_history_id) FROM trade_history), @COLOR, @Quantity, '반품', @COLOR_VALUE)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlCommand cmd2 = new SqlCommand(sql2, conn);
                    SqlCommand cmd3 = new SqlCommand(sql3, conn);
                    try
                    {
                        SqlParameter param_employee_id = new SqlParameter("@EMPLOYEE_ID", eid);
                        param_employee_id.SqlDbType = System.Data.SqlDbType.Int;
                        cmd2.Parameters.Add(param_employee_id);

                        SqlParameter param_TRADE_DATE = new SqlParameter("@TRADE_DATE", DateTime.Now);
                        param_TRADE_DATE.SqlDbType = System.Data.SqlDbType.DateTime;
                        cmd2.Parameters.Add(param_TRADE_DATE);

                        SqlParameter param_name = new SqlParameter("@Product_id", return_Info.Product_id);
                        param_name.SqlDbType = System.Data.SqlDbType.Int;
                        SqlParameter param_name2 = new SqlParameter("@Product_id", return_Info.Product_id);
                        param_name2.SqlDbType = System.Data.SqlDbType.Int;
                        cmd.Parameters.Add(param_name);
                        cmd3.Parameters.Add(param_name2);

                        SqlParameter param_color = new SqlParameter("@Color", return_Info.Color);
                        SqlParameter param_color2 = new SqlParameter("@Color", return_Info.Color);
                        cmd.Parameters.Add(param_color);
                        cmd3.Parameters.Add(param_color2);

                        SqlParameter param_color_value = new SqlParameter("@COLOR_VALUE", return_Info.ColorValue);
                        cmd3.Parameters.Add(param_color_value);

                        SqlParameter param_quantity2 = new SqlParameter("@Quantity", return_Info.Quantity);
                        param_quantity2.SqlDbType = System.Data.SqlDbType.Int;
                        SqlParameter param_quantity = new SqlParameter("@Quantity", return_Info.Quantity);
                        param_quantity.SqlDbType = System.Data.SqlDbType.Int;
                        cmd.Parameters.Add(param_quantity2);
                        cmd3.Parameters.Add(param_quantity);
                        if (return_Info.Quantity != 0)
                            if (cmd.ExecuteNonQuery() >= 1 && cmd2.ExecuteNonQuery() >= 1 && cmd3.ExecuteNonQuery() >= 1)
                            {
                                string sqldel = "delete from stock_product where product_id = @ID and color = @COLOR and stock = 0";
                                SqlCommand cmd4 = new SqlCommand(sqldel, conn);
                                SqlParameter param_ID = new SqlParameter("@ID", return_Info.Product_id);
                                param_ID.SqlDbType = System.Data.SqlDbType.Int;
                                cmd4.Parameters.Add(param_ID);
                                SqlParameter param_COLOR = new SqlParameter("@COLOR", return_Info.Color);
                                cmd4.Parameters.Add(param_COLOR);
                                if (cmd4.ExecuteNonQuery() >= 1)
                                    return true;
                                return true;
                            }
                            else return false;
                        else return false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        #endregion

        #endregion


        #region 지원팀
        // 지원 -> 고객관리 -> 전체출력
        public List<Customer> GetList_Customer_Search(string name, string gender, string phone, string grammar)
        {
            List<Customer> cusList = new List<Customer>();

            using (conn = new SqlConnection())
            {
                string sql = "select customer_id ,name, gender, birth, phone, savings from customer" + grammar;
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_name = new SqlParameter("@name", "%" + name + "%");
                cmd.Parameters.Add(param_name);

                SqlParameter param_gender = new SqlParameter("@gender", gender);
                cmd.Parameters.Add(param_gender);

                SqlParameter param_phone = new SqlParameter("@phone", "%" + phone + "%");
                cmd.Parameters.Add(param_phone);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        Customer customer = new Customer();
                        customer.Id = myDataReader.GetInt32(0);
                        customer.Name = myDataReader.GetString(1);
                        customer.Gender = myDataReader.GetString(2);
                        customer.Date = myDataReader.GetDateTime(3);
                        customer.Phone = myDataReader.GetString(4);
                        customer.Savings = myDataReader.GetInt64(5);
                        cusList.Add(customer);
                    }
                }
            }
            return cusList;
        }

        public bool Update_Emp_Info(string login_id, string rank, string name, string gender,
            string social_num, string phone, string email, string address, DateTime? end_date)
        {
            using (conn = new SqlConnection())
            {
                string sql = "update employee set rank=@rank, name=@name, gender=@gender, social_number=@social_number, " +
                    "phone=@phone, email=@email, address=@address, end_date=@end_date where login_id=@login_id";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_login_id = new SqlParameter("@login_id", login_id);
                cmd.Parameters.Add(param_login_id);

                SqlParameter param_name = new SqlParameter("@name", name);
                cmd.Parameters.Add(param_name);

                SqlParameter param_rank = new SqlParameter("@rank", rank);
                cmd.Parameters.Add(param_rank);

                SqlParameter param_gender = new SqlParameter("@gender", gender);
                cmd.Parameters.Add(param_gender);

                SqlParameter param_social_num = new SqlParameter("@social_number", social_num);
                cmd.Parameters.Add(param_social_num);

                SqlParameter param_phone = new SqlParameter("@phone", phone);
                cmd.Parameters.Add(param_phone);

                SqlParameter param_email = new SqlParameter("@email", email);
                cmd.Parameters.Add(param_email);

                SqlParameter param_address = new SqlParameter("@address", address);
                cmd.Parameters.Add(param_address);

                SqlParameter param_end_date = new SqlParameter("@end_date", end_date == null ? (object)DBNull.Value : end_date);
                param_end_date.IsNullable = true;
                param_end_date.Direction = ParameterDirection.Input;
                param_end_date.SqlDbType = System.Data.SqlDbType.DateTime;
                cmd.Parameters.Add(param_end_date);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }

        //직원 -> 직원관리 -> 정보번경
        public Employee Get_Emloyee_info(int id)//11
        {
            string str = string.Empty;
            Employee employee1 = new Employee();
            using (conn = new SqlConnection())
            {
                string sql = "select employee_id,login_id, login_pw, name, gender, social_number, phone, address, start_date, end_date, rank, email, post_number from employee where employee_id=@id";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_emp_id = new SqlParameter("@id", id);
                param_emp_id.SqlDbType = SqlDbType.Int;
                cmd.Parameters.Add(param_emp_id);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        employee1.Employee_id = myDataReader.GetInt32(0);
                        employee1.Login_id = myDataReader.GetString(1);
                        employee1.Login_pw = myDataReader.GetString(2);
                        employee1.Name = myDataReader.GetString(3);
                        employee1.Gender = myDataReader.GetString(4);
                        employee1.Social_number = myDataReader.GetString(5);
                        employee1.Phone = myDataReader.GetString(6);
                        employee1.Address = myDataReader.GetString(7);
                        employee1.Start_date = myDataReader.GetDateTime(8).ToShortDateString();
                        employee1.End_date = myDataReader.IsDBNull(9) ? "해당사항없음" : myDataReader.GetDateTime(6).ToShortDateString();
                        employee1.Rank = myDataReader.GetString(10);
                        employee1.Email = myDataReader.GetString(11);
                        employee1.Post_number = myDataReader.GetString(12);
                    }
                }
                return employee1;
            }
        }

        //지원 -> 직원관리 -> 내용변경
        public void Upload_EMP_Info(int emp_id, string login_id, string login_pw, string name,
            string gender, string social_number, string phone, string address,
            DateTime start_date, DateTime end_date, string rank, string email)
        {
            using (conn = new SqlConnection())
            {
                try
                {
                    string sql = "update employee set login_id=@login_id, login_pw=@login_pw, " +
                  "name=@name, gender=@gender, social_number=@social_number, " +
                  "phone=@phone, address=@address, start_date=@start_date, " +
                  "end_date=@end_date, rank=@rank, email=email " +
                  "whrere employee_id=@emp_id";
                    conn.ConnectionString =
                        ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                    conn.Open();    //  데이터베이스 연결           

                    SqlCommand cmd = new SqlCommand(sql, conn);

                    SqlParameter param_Login_ID = new SqlParameter("@login_id", login_id);
                    cmd.Parameters.Add(param_Login_ID);

                    SqlParameter param_Login_Pw = new SqlParameter("@login_pw", login_pw);
                    cmd.Parameters.Add(param_Login_Pw);

                    SqlParameter param_Login_Name = new SqlParameter("@name", name);
                    cmd.Parameters.Add(param_Login_Name);

                    SqlParameter param_Login_Social_n = new SqlParameter("@social_number", social_number);
                    cmd.Parameters.Add(param_Login_Social_n);

                    SqlParameter param_Login_Phone = new SqlParameter("@phone", phone);
                    cmd.Parameters.Add(param_Login_Phone);

                    SqlParameter param_Login_Address = new SqlParameter("@address", address);
                    cmd.Parameters.Add(param_Login_Address);

                    SqlParameter param_Login_Start_date = new SqlParameter("@start_date", start_date);
                    cmd.Parameters.Add(param_Login_Start_date);

                    SqlParameter param_Login_End_date = new SqlParameter("@end_date", end_date);
                    cmd.Parameters.Add(param_Login_End_date);

                    SqlParameter param_Login_Rank = new SqlParameter("@rank", rank);
                    cmd.Parameters.Add(param_Login_Rank);

                    SqlParameter param_Login_Email = new SqlParameter("@email", email);
                    cmd.Parameters.Add(param_Login_Email);

                    SqlParameter param_Login_emp_Id = new SqlParameter("@employee_id", emp_id);
                    cmd.Parameters.Add(param_Login_emp_Id);

                    MessageBox.Show("Succeeded Update");
                }
                catch
                {
                    MessageBox.Show("Failed Update");
                }
            }
        }

        //지원-> 직원관리 -> 회원등록 전체출력
        public List<Sign_up> GetList_Sign_Up_Emp()
        {
            List<Sign_up> Signup_List = new List<Sign_up>();

            using (conn = new SqlConnection())
            {
                string sql = "select Login_id, Gender, Name, Social_number, Phone, " +
                    "Email, Post_number, Address, Sign_date from signup";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        Sign_up sign_up = new Sign_up();
                        sign_up.Login_id = myDataReader.GetString(0);
                        sign_up.Gender = myDataReader.GetString(1);
                        sign_up.Name = myDataReader.GetString(2);
                        sign_up.Social_number = myDataReader.GetString(3);
                        sign_up.Phone = myDataReader.GetString(4);
                        sign_up.Email = myDataReader.GetString(5);
                        sign_up.Post_number = myDataReader.GetString(6);
                        sign_up.Address = myDataReader.GetString(7);
                        sign_up.Sign_date = myDataReader.IsDBNull(8) ? "없음" : myDataReader.GetDateTime(8).ToShortDateString();

                        Signup_List.Add(sign_up);
                    }
                }
            }
            return Signup_List;
        }

        // sign_up에서 다음 employee_id 가져올 때 사용
        public Employee locate_Emp_Id(string rank1, string rank2)
        {
            Employee employee = new Employee();
            using (conn = new SqlConnection())
            {
                string sql = "select top 1(employee_id) from employee where rank=@rank1 or rank=@rank2 order by employee_id desc";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_rank1 = new SqlParameter("@rank1", rank1);
                cmd.Parameters.Add(param_rank1);

                SqlParameter param_rank2 = new SqlParameter("@rank2", rank2);
                cmd.Parameters.Add(param_rank2);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        employee.Employee_id = myDataReader.GetInt32(0);
                    }
                }
            }
            return employee;
        }

        //로그인 아이디로 sign_up에서 가져온다
        public Sign_up Sign_Up_Data_Catch(Sign_up sign)
        {
            Sign_up su = new Sign_up();
            using (conn = new SqlConnection())
            {
                string sql = "select * from signup where login_id=@login_id ";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_login_id = new SqlParameter("@login_id", sign.Login_id);
                cmd.Parameters.Add(param_login_id);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        su.Login_id = myDataReader.GetString(0);
                        su.Login_pw = myDataReader.GetString(1);
                        su.Gender = myDataReader.GetString(2);
                        su.Social_number = myDataReader.GetString(3);
                        su.Phone = myDataReader.GetString(4);
                        su.Address = myDataReader.GetString(5);
                        su.Sign_date = myDataReader.GetDateTime(6).ToShortDateString();
                        su.Email = myDataReader.GetString(7);
                        su.Name = myDataReader.GetString(8);
                        su.Post_number = myDataReader.GetString(9);
                    }
                }
            }
            return su;
        }

        // sign_up DB를 Employee값으로 이동
        public bool Upload_Emp_Sign_up(int emp_id, string rank, DateTime start_date, DateTime? end_date, Sign_up sign)
        {
            using (conn = new SqlConnection())
            {
                string sql = "insert into employee (employee_id, login_id, login_pw, " +
                    "name, gender, social_number, phone, address, start_date, end_date, " +
                    "rank, email, post_number) values (@emp_id, @login_id, @login_pw, @name, " +
                    " @gender, @social_number, @phone, @address, @start_date, @end_date, " +
                    "@rank, @email, @post_number)";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_emp_id = new SqlParameter("@emp_id", emp_id);
                param_emp_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_emp_id);

                SqlParameter param_login_id = new SqlParameter("@login_id", sign.Login_id);
                cmd.Parameters.Add(param_login_id);

                SqlParameter param_login_pw = new SqlParameter("@login_pw", sign.Login_pw);
                cmd.Parameters.Add(param_login_pw);

                SqlParameter param_name = new SqlParameter("@name", sign.Name);
                cmd.Parameters.Add(param_name);

                SqlParameter param_gender = new SqlParameter("@gender", sign.Gender);
                cmd.Parameters.Add(param_gender);

                SqlParameter param_social = new SqlParameter("@social_number", sign.Social_number);
                cmd.Parameters.Add(param_social);

                SqlParameter param_phone = new SqlParameter("@phone", sign.Phone);
                cmd.Parameters.Add(param_phone);

                SqlParameter param_adress = new SqlParameter("@address", sign.Address);
                cmd.Parameters.Add(param_adress);

                SqlParameter param_start_date = new SqlParameter("@start_date", start_date);
                param_start_date.SqlDbType = System.Data.SqlDbType.Date;
                cmd.Parameters.Add(param_start_date);

                SqlParameter param_end_date = new SqlParameter("@end_date", end_date == null ? (object)DBNull.Value : end_date);
                param_end_date.IsNullable = true;
                param_end_date.Direction = ParameterDirection.Input;
                param_end_date.SqlDbType = System.Data.SqlDbType.DateTime;
                cmd.Parameters.Add(param_end_date);

                SqlParameter param_rank = new SqlParameter("@rank", rank);
                cmd.Parameters.Add(param_rank);

                SqlParameter param_email = new SqlParameter("@email", sign.Email);
                cmd.Parameters.Add(param_email);

                SqlParameter param_post = new SqlParameter("@post_number", sign.Post_number);
                cmd.Parameters.Add(param_post);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }

        // 회원등록 DB에서 삭제
        public bool Delete_Sign_up(string login_id)
        {
            using (conn = new SqlConnection())
            {
                string sql = "delete from signup where login_id=@login_id";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_login_id = new SqlParameter("@login_id", login_id);
                cmd.Parameters.Add(param_login_id);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }

        //지원에서 직원관리 -> 비밀번호 리셋 버튼 구현
        public bool Reset_PW_EMP(string login_id) //11
        {
            using (conn = new SqlConnection())
            {
                string set_pw = "1234";
                string sql = "update employee set login_pw=@login_pw where login_id=@login_id";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_login_id = new SqlParameter("@login_id", login_id);
                cmd.Parameters.Add(param_login_id);

                SqlParameter param_login_pw = new SqlParameter("@login_pw", set_pw);
                cmd.Parameters.Add(param_login_pw);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }

        //지원 -> 직원조회 -> 직원 리스트 뽑아오기 11
        public List<Employee> GetList_Emp_info(string query, string login_id, string name, string phone, string gender, DateTime? start_date, DateTime? end_date) //지원-> 직원조회 -> 직원 전체출력
        {
            List<Employee> emp_List = new List<Employee>();
            using (conn = new SqlConnection())
            {
                string sql = "select * from employee " + query;
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_login_id = new SqlParameter("@login_id", "%" + login_id + "%");
                cmd.Parameters.Add(param_login_id);

                SqlParameter param_name = new SqlParameter("@name", "%" + name + "%");
                cmd.Parameters.Add(param_name);

                SqlParameter param_gender = new SqlParameter("@gender", "%" + gender + "%");
                cmd.Parameters.Add(param_gender);

                SqlParameter param_phone = new SqlParameter("@phone", "%" + phone + "%");
                cmd.Parameters.Add(param_phone);

                SqlParameter param_start_date = new SqlParameter("@start_date", start_date == null ? (object)DBNull.Value : start_date);
                cmd.Parameters.Add(param_start_date);

                SqlParameter param_end_date = new SqlParameter("@end_date", end_date == null ? (object)DBNull.Value : end_date);
                param_end_date.IsNullable = true;
                param_end_date.Direction = ParameterDirection.Input;
                param_end_date.SqlDbType = System.Data.SqlDbType.DateTime;
                cmd.Parameters.Add(param_end_date);

                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        Employee employee = new Employee();
                        employee.Employee_id = myDataReader.GetInt32(0);
                        employee.Login_id = myDataReader.GetString(1);
                        employee.Login_pw = myDataReader.GetString(2);
                        employee.Name = myDataReader.GetString(3);
                        employee.Gender = myDataReader.GetString(4);
                        employee.Social_number = myDataReader.GetString(5);
                        employee.Phone = myDataReader.GetString(6);
                        employee.Address = myDataReader.GetString(7);
                        employee.Start_date = myDataReader.IsDBNull(8) ? "없음" : myDataReader.GetDateTime(8).ToShortDateString();
                        employee.End_date = myDataReader.IsDBNull(9) ? "없음" : myDataReader.GetDateTime(9).ToShortDateString();
                        employee.Rank = myDataReader.GetString(10);
                        employee.Email = myDataReader.GetString(11);
                        employee.Post_number = myDataReader.GetString(12);
                        emp_List.Add(employee);
                    }
                }
            }
            return emp_List;
        }

        //지원 -> 고객관리 -> 신규등록 시  cus_id 가져오기
        public Customer Get_Cus_Id()//11
        {
            Customer customer = new Customer();
            using (conn = new SqlConnection())
            {
                string sql = "select top 1(customer_id) from customer order by customer_id desc";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);
                using (SqlDataReader myDataReader = cmd.ExecuteReader())
                {
                    while (myDataReader.Read())
                    {
                        customer.Id = myDataReader.GetInt32(0);
                    }
                }
            }
            return customer;
        }

        //지원 -> 고객관리 -> 고객정보 등록
        public bool Insert_Cus_Info(string name, string gender, DateTime? birth, string phone, long savings)//11
        {
            using (conn = new SqlConnection())
            {
                string sql = "insert into customer (name, gender, " +
                    "birth, phone, savings) values (@name, @gender, @birth," +
                    " @phone, @savings)";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_name = new SqlParameter("@name", name);
                cmd.Parameters.Add(param_name);

                SqlParameter param_gender = new SqlParameter("@gender", gender);
                cmd.Parameters.Add(param_gender);

                SqlParameter param_birth = new SqlParameter("@birth", birth);
                param_birth.SqlDbType = System.Data.SqlDbType.DateTime;
                cmd.Parameters.Add(param_birth);

                SqlParameter param_phone = new SqlParameter("@phone", phone);
                cmd.Parameters.Add(param_phone);

                SqlParameter param_savings = new SqlParameter("@savings", savings);
                param_savings.SqlDbType = System.Data.SqlDbType.BigInt;
                cmd.Parameters.Add(param_savings);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }


        // 지원 -> 고객관리 -> 고객info 변경
        public bool Update_Cus_Info(int cus_id, string name, string gender, DateTime? birth, string phone, long savings)//11
        {
            using (conn = new SqlConnection())
            {
                string sql = "update customer set name=@name, gender=@gender, birth=@birth, phone =@phone, savings = @savings where customer_id = @customer_id";
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["userDB"].ToString();
                conn.Open();    //  데이터베이스 연결           

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_cus_id = new SqlParameter("@customer_id", cus_id);
                param_cus_id.SqlDbType = System.Data.SqlDbType.Int;
                cmd.Parameters.Add(param_cus_id);

                SqlParameter param_name = new SqlParameter("@name", name);
                cmd.Parameters.Add(param_name);

                SqlParameter param_gender = new SqlParameter("@gender", gender);
                cmd.Parameters.Add(param_gender);

                SqlParameter param_birth = new SqlParameter("@birth", birth);
                param_birth.SqlDbType = System.Data.SqlDbType.DateTime;
                cmd.Parameters.Add(param_birth);

                SqlParameter param_phone = new SqlParameter("@phone", phone);
                cmd.Parameters.Add(param_phone);

                SqlParameter param_savings = new SqlParameter("@savings", savings);
                param_savings.SqlDbType = System.Data.SqlDbType.BigInt;
                cmd.Parameters.Add(param_savings);

                if (cmd.ExecuteNonQuery() >= 1)
                    return true;
                else
                    return false;
            }
        }
        #endregion
    }
}
