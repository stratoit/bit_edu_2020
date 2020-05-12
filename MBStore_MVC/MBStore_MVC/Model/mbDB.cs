using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
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
    }
}
