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

        internal List<string> queryCollectionWord(string query, string col)
        {
            if (!checkConnection()) return null;

            List<string> res = new List<string>();
            SqlCommand queryCmd = new SqlCommand();
            queryCmd.CommandType = CommandType.Text;

            queryCmd.CommandText = "SELECT TenTu FROM LUUTU WHERE TenBST=@col AND TenTu LIKE @word";
            SqlParameter param_tentu = new SqlParameter("@word", SqlDbType.VarChar);
            SqlParameter param_tenbst = new SqlParameter("@col", SqlDbType.NVarChar);
            param_tenbst.Value = col;
            param_tentu.Value = query + "%";
            queryCmd.Parameters.Add(param_tentu);
            queryCmd.Parameters.Add(param_tenbst);
            queryCmd.Connection = sqlcnt;

            SqlDataReader sqlrdr = queryCmd.ExecuteReader();
            try
            {
                while (sqlrdr.Read())
                {
                    res.Add(sqlrdr.GetString(0));
                }
                sqlrdr.Close();
                return res;
            }
            catch (Exception ex)
            {
                sqlrdr.Close();
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        internal List<string> loadCollectionWord(string col)
        {
            if (!checkConnection()) return null;
            
            List<string> res = new List<string>();
            SqlCommand listAllCmd = new SqlCommand();
            listAllCmd.CommandType = CommandType.Text;

            listAllCmd.CommandText = "SELECT TenTu FROM LUUTU WHERE TenBST=@col";
            SqlParameter param_tenbst = new SqlParameter("@col", SqlDbType.NVarChar);
            param_tenbst.Value = col;
            listAllCmd.Parameters.Add(param_tenbst);
            listAllCmd.Connection = sqlcnt;

            SqlDataReader sqlrdr = listAllCmd.ExecuteReader();
            try
            {
                while (sqlrdr.Read())
                {
                    res.Add(sqlrdr.GetString(0));
                }
                sqlrdr.Close();
                return res;
            }
            catch (Exception ex)
            {
                sqlrdr.Close();
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public bool addToCollection(string query, string col)
        {
            if (!checkConnection()) return false;

            SqlCommand queryCmd = new SqlCommand();
            queryCmd.CommandType = CommandType.Text;

            queryCmd.CommandText = "INSERT INTO LUUTU VALUES (@word, @col)";

            SqlParameter param_tentu = new SqlParameter("@word", SqlDbType.VarChar);
            SqlParameter param_tenbst = new SqlParameter("@col", SqlDbType.NVarChar);
            param_tenbst.Value = col;
            param_tentu.Value = query;
            queryCmd.Parameters.Add(param_tentu);
            queryCmd.Parameters.Add(param_tenbst);
            queryCmd.Connection = sqlcnt;

            try
            {
                int res = queryCmd.ExecuteNonQuery();           
                if (res > 0) return true;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public bool insertCollection(string col)
        {
            if (!checkConnection()) return false;

            SqlCommand queryCmd = new SqlCommand();
            queryCmd.CommandType = CommandType.Text;

            queryCmd.CommandText = "INSERT INTO BOSUUTAP VALUES (@col)";

            SqlParameter param_tenbst = new SqlParameter("@col", SqlDbType.NVarChar);
            param_tenbst.Value = col;
            queryCmd.Parameters.Add(param_tenbst);
            queryCmd.Connection = sqlcnt;

            try
            {
                int res = queryCmd.ExecuteNonQuery();
                if (res > 0) return true;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public bool removeFromCollection(string query, string col)
        {
            if (!checkConnection()) return false;

            SqlCommand queryCmd = new SqlCommand();
            queryCmd.CommandType = CommandType.Text;

            queryCmd.CommandText = "DELETE FROM LUUTU WHERE TenTu=@word AND TenBST=@col";

            SqlParameter param_tentu = new SqlParameter("@word", SqlDbType.VarChar);
            SqlParameter param_tenbst = new SqlParameter("@col", SqlDbType.NVarChar);
            param_tenbst.Value = col;
            param_tentu.Value = query;
            queryCmd.Parameters.Add(param_tentu);
            queryCmd.Parameters.Add(param_tenbst);
            queryCmd.Connection = sqlcnt;

            try
            {
                int res = queryCmd.ExecuteNonQuery();
                if (res > 0) return true;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
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
                    if (sqlrdr.GetString(3) == "") continue;
                    res += sqlrdr.GetString(1) + "\n";
                    if (sqlrdr.GetString(2) != "Base") res += " - " + sqlrdr.GetString(2) + "\n\t";
                    res += "\n\t +" + Regex.Replace(sqlrdr.GetString(3), @"\n", "\n\t +") + "\n";
                    res = res.Remove(res.LastIndexOf('+'), 1);
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

        public bool isInCollection(string query, string col)
        {
            if (!checkConnection()) return false;
            SqlCommand queryCmd = new SqlCommand();
            queryCmd.CommandType = CommandType.Text;

            queryCmd.CommandText = "SELECT * FROM LUUTU WHERE TenTu=@word AND TenBST=@col";
            SqlParameter param_tentu = new SqlParameter("@word", SqlDbType.VarChar);
            SqlParameter param_tenbst = new SqlParameter("@col", SqlDbType.NVarChar);
            param_tenbst.Value = col;
            param_tentu.Value = query;
            queryCmd.Parameters.Add(param_tentu);
            queryCmd.Parameters.Add(param_tenbst);
            queryCmd.Connection = sqlcnt;

            SqlDataReader sqlrdr = queryCmd.ExecuteReader();
            try
            {
                if (sqlrdr.Read())
                {
                    sqlrdr.Close();
                    return true;
                }
                sqlrdr.Close();
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                sqlrdr.Close();
                return false;
            }
        }

        public bool removeWord(string query)
        {
            //TODO: remove all special meaning and collection-word binding
            if (!checkConnection()) return false;
            SqlCommand specialMeanRemoveCmd = new SqlCommand();
            specialMeanRemoveCmd.CommandType = CommandType.Text;
            SqlCommand collectionBindRemoveCmd = new SqlCommand();
            collectionBindRemoveCmd.CommandType = CommandType.Text;
            specialMeanRemoveCmd.CommandText = "DELETE FROM TUCHUYENNGANH WHERE TenTu=@word";
            collectionBindRemoveCmd.CommandText = "DELETE FROM LUUTU WHERE TenTu=@word";
            SqlParameter param_tentu = new SqlParameter("@word", SqlDbType.VarChar);
            param_tentu.Value = query;
            specialMeanRemoveCmd.Parameters.Add(param_tentu);
            collectionBindRemoveCmd.Parameters.Add(param_tentu);

            //remove said word
            SqlCommand removeCmd = new SqlCommand();
            removeCmd.CommandType = CommandType.Text;
            removeCmd.CommandText = "DELETE FROM TU WHERE TenTu=@word";
            removeCmd.Parameters.Add(param_tentu);

            try
            {
                int res = specialMeanRemoveCmd.ExecuteNonQuery();
                res += collectionBindRemoveCmd.ExecuteNonQuery();
                res += removeCmd.ExecuteNonQuery();
                if (res == 0) return false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public bool removeCollection(string col)
        {
            //TODO: remove all collection-word binding
            if (!checkConnection()) return false;
            SqlCommand collectionBindRemoveCmd = new SqlCommand();
            collectionBindRemoveCmd.CommandType = CommandType.Text;
            collectionBindRemoveCmd.CommandText = "DELETE FROM LUUTU WHERE TenBST=@col";
            SqlParameter param_tenbst = new SqlParameter("@col", SqlDbType.NVarChar);
            param_tenbst.Value = col;
            collectionBindRemoveCmd.Parameters.Add(param_tenbst);

            //remove said collection
            SqlCommand removeCmd = new SqlCommand();
            removeCmd.CommandType = CommandType.Text;
            removeCmd.CommandText = "DELETE FROM BOSUUTAP WHERE TenBST=@col";
            removeCmd.Parameters.Add(param_tenbst);

            try
            {
                int res = collectionBindRemoveCmd.ExecuteNonQuery();
                res += removeCmd.ExecuteNonQuery();
                if (res == 0) return false;
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public void saveWord(string word, string pronounce, string bmean, string smean, string edit_target = "")
        {
            //TODO: a lot of stuffs here
        }
    }
}
