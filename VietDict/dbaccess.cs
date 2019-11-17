using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace VietDict
{
    class dbaccess
    {
        SqlConnection sqlcnt = null;
        //string strcnt => ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string strcnt = "Data Source=.; Initial Catalog=DICTIONARY; Trusted_Connection=Yes;";

        public dbaccess()
        {
            //Init. new database connection
            try
            {
                sqlcnt = new SqlConnection(strcnt);
                sqlcnt.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private bool checkConnection()
        {
            if (sqlcnt == null)
                sqlcnt = new SqlConnection(strcnt);
            else if (sqlcnt.State == ConnectionState.Closed)
            {
                MessageBox.Show("Connection Stopped, please reconnect.");
                return false;
            }
            return true;
        }
        public List<string> loadAllWord()
        {
            if (!checkConnection()) return null;
            try
            {
                List<string> res = new List<string>();
                SqlCommand listAllCmd = new SqlCommand();
                listAllCmd.CommandType = CommandType.Text;

                listAllCmd.CommandText = "SELECT TOP 1000 TenTu FROM TU";
                listAllCmd.Connection = sqlcnt;

                SqlDataReader sqlrdr = listAllCmd.ExecuteReader();
                while (sqlrdr.Read())
                {
                    res.Add(sqlrdr.GetString(0));
                }
                sqlrdr.Close();
                return res;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public List<string> listCollection()
        {
            if (!checkConnection()) return null;
            try
            {
                List<string> res = new List<string>();
                SqlCommand listAllCmd = new SqlCommand();
                listAllCmd.CommandType = CommandType.Text;

                listAllCmd.CommandText = "SELECT * FROM BOSUUTAP";
                listAllCmd.Connection = sqlcnt;

                SqlDataReader sqlrdr = listAllCmd.ExecuteReader();
                while (sqlrdr.Read())
                {
                    res.Add(sqlrdr.GetString(0));
                }
                sqlrdr.Close();
                return res;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }
        public List<string> wordQuery(string query)
        {
            if (!checkConnection()) return null;
            try
            {
                List<string> res = new List<string>();
                SqlCommand queryCmd = new SqlCommand();
                queryCmd.CommandType = CommandType.Text;

                queryCmd.CommandText = "SELECT TOP 1000 TenTu FROM TU WHERE TenTu LIKE @word";
                SqlParameter param_tentu = new SqlParameter("@word", SqlDbType.VarChar);
                param_tentu.Value = query + "%";
                queryCmd.Parameters.Add(param_tentu);
                queryCmd.Connection = sqlcnt;

                SqlDataReader sqlrdr = queryCmd.ExecuteReader();
                while (sqlrdr.Read())
                {
                    res.Add(sqlrdr.GetString(0));
                }
                sqlrdr.Close();
                return res;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }
        public string recallWordInfo(string query, out string pronounce)
        {
            pronounce = "";
            if (!checkConnection()) return null;
            
            string res = "";
            SqlCommand queryCmd = new SqlCommand();
            queryCmd.CommandType = CommandType.Text;

            queryCmd.CommandText = "SELECT TOP 1 * FROM TU WHERE TenTu=@word";
            SqlParameter param_tentu = new SqlParameter("@word", SqlDbType.VarChar);
            param_tentu.Value = query;
            queryCmd.Parameters.Add(param_tentu);
            queryCmd.Connection = sqlcnt;

            SqlDataReader sqlrdr = queryCmd.ExecuteReader();
            try
            {
                if (sqlrdr.Read())
                {
                    pronounce = sqlrdr.GetString(1);
                    res = sqlrdr.GetString(3);
                }
                sqlrdr.Close();
                return res;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString());
                sqlrdr.Close();
                return null ;
            }
        }

        public string specialtyWordMean(string query)
        {
            if (!checkConnection()) return null;

            string res = "";
            SqlCommand queryCmd = new SqlCommand();
            queryCmd.CommandType = CommandType.Text;

            queryCmd.CommandText = "SELECT * FROM TUCHUYENNGANH INNER JOIN (SELECT TOP 1 * FROM TU WHERE TenTu=@word) AS A ON TUCHUYENNGANH.TenTu=A.TenTu";
            SqlParameter param_tentu = new SqlParameter("@word", SqlDbType.VarChar);
            param_tentu.Value = query;
            queryCmd.Parameters.Add(param_tentu);
            queryCmd.Connection = sqlcnt;

            SqlDataReader sqlrdr = queryCmd.ExecuteReader();
            try
            {
                while (sqlrdr.Read())
                {
                    res += sqlrdr.GetString(1) + "\n";
                    if (sqlrdr.GetString(2) != "Base") res += "\t" + sqlrdr.GetString(2) + "\n";
                    res += "\n\t" + Regex.Replace(sqlrdr.GetString(3), @"\n", "\n\t") + "\n";
                }
                sqlrdr.Close();
                return res;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                sqlrdr.Close();
                return null;
            }
        }
    }
}
