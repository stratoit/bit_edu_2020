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

        public string SelectEmpId(string id)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string sql = "select login_id,login_pw,rank,name from employee where login_id = @ID";

                SqlCommand cmd = new SqlCommand(sql, conn);

                SqlParameter param_id = new SqlParameter("@ID", id);
                cmd.Parameters.Add(param_id);

                SqlDataReader myDataReader = cmd.ExecuteReader();

                try
                {
                    if (myDataReader.Read())
                    {
                        string str = string.Empty;
                        str += myDataReader["login_id"].ToString() + "#";
                        str += myDataReader["login_pw"].ToString() + "#";
                        str += myDataReader["rank"].ToString() + "#";
                        str += myDataReader["name"].ToString();

                        myDataReader.Close();
                        return str;
                    }
                    else
                    {
                        return "";
                    }
                }
                catch
                {
                    return "";
                }
            }
        }

        public List<string> SelectCustomer(string name, string gender, string phone)
        {
            using (conn = new SqlConnection())
            {
                conn.ConnectionString =
                    ConfigurationManager.ConnectionStrings["UserDB"].ToString();
                conn.Open();    //  데이터베이스 연결           
                string query = "";

                if (!string.IsNullOrEmpty(name))
                    query += " name = @name" + " and";

                if (gender != "전체")
                    query += " gender = @gender" + " and";

                if (!string.IsNullOrEmpty(phone))
                    query += " phone = @phone" + " and";

                string sql = "select name,gender,birth,phone,savings from customer where" + query;

                SqlCommand cmd = new SqlCommand(sql.Substring(0,sql.Length-3), conn);

                SqlParameter param_name = new SqlParameter("@name", name);
                cmd.Parameters.Add(param_name);
                SqlParameter param_gender = new SqlParameter("@gender", gender);
                cmd.Parameters.Add(param_gender);
                SqlParameter param_phone = new SqlParameter("@phone", phone);
                cmd.Parameters.Add(param_phone);

                SqlDataReader myDataReader = cmd.ExecuteReader();

                List<string> retstr = new List<string>();

                while (myDataReader.Read())
                {
                    string str = string.Empty;
                    str += myDataReader["name"].ToString() + "#";
                    str += myDataReader["gender"].ToString() + "#";
                    str += myDataReader["birth"].ToString() + "#";
                    str += myDataReader["phone"].ToString() + "#";
                    str += myDataReader["savings"].ToString();                  
                    retstr.Add(str);
                }
                myDataReader.Close();
                return retstr;
            }
        }
    }
}
