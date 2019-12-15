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
        int countTuChuaLoad = 0;
        //string strcnt => ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string strcnt = "Data Source=.; Initial Catalog=DICTIONARY; Trusted_Connection=Yes;MultipleActiveResultSets=True";

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

        public List<string> loadLearnWord(int countfour, int countthree, int counttwo, int countone)
        {

            if (!checkConnection())
                return null;
            int totalWord = countfour + countthree + counttwo + countone;
            List<string> res = new List<string>();
            
            res = LoadDanhSachHocTu(res, countone, ref counttwo, 1);
            
            res = LoadDanhSachHocTu(res, counttwo, ref countthree, 2);
            
            res = LoadDanhSachHocTu(res, countthree, ref countfour, 3);
            
            res = LoadDanhSachHocTu(res, countfour, ref countone ,4);

            if (res.Count < totalWord)
                res = LoadTuChuaLoad(res, totalWord - res.Count);
        
            return res;
            
        }

        public List<string> LoadDanhSachHocTu(List<string> res, int SoLuong, ref int SL_Nextlevel, int UuTien) {
            int count = 0;
            
            SqlCommand countWord = new SqlCommand("SELECT COUNT(*) FROM HOCTU WHERE DoUuTien =" + UuTien, sqlcnt);
            using (SqlDataReader rdr = countWord.ExecuteReader())
            {
                if (rdr.Read())
                {
                    count = rdr.GetInt32(0);
                }
                
            }
            if (count < SoLuong)
            {
                
                SL_Nextlevel = SL_Nextlevel + SoLuong - count;
                SoLuong = count;
            }
            else
            {
                countTuChuaLoad = countTuChuaLoad + count - SoLuong;
            }
            SqlCommand listAll = new SqlCommand("SELECT TOP " + SoLuong + " TenTu FROM HOCTU WHERE DoUuTien = " + UuTien + " ORDER BY NEWID()", sqlcnt);
            using(SqlDataReader rdr = listAll.ExecuteReader())
            {
                while (rdr.Read())
                {
                    res.Add(rdr.GetString(0));
                }
            }
            return res;
        }

        public List<string> LoadTuChuaLoad(List<string> res, int countTuCanLoad)
        {
            SqlCommand Load = new SqlCommand("SELECT TOP " + countTuCanLoad + "TenTu FROM HOCTU ORDER BY NEWID()", sqlcnt);
            using (SqlDataReader rdr = Load.ExecuteReader())
            {
                while (rdr.Read())
                {
                    res.Add(rdr.GetString(0));
                }
            }
            return res;
        }

        public string MeaningLearnWord(string s)
        {
            string meaning = "";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "SELECT NghiaCoBan FROM TU WHERE TenTu = @tentu";
            cmd.Parameters.Add("@tentu", SqlDbType.NVarChar).Value = s;
            cmd.Connection = sqlcnt;
            SqlDataReader sqlrdr = cmd.ExecuteReader();
            while (sqlrdr.Read())
                meaning = sqlrdr.GetString(0);
            sqlrdr.Close();
            return meaning;
        }

        public void UpdateDoKho(string s, int x, string learnword)
        {
            string DoUuTien = "";
            
            
            if (x == 1)
            {
                try
                {
                    SqlCommand getDoUuTien = new SqlCommand("SELECT DoUuTien FROM HOCTU WHERE TenTu = @tentu", sqlcnt);
                    getDoUuTien.Parameters.Add("@tentu", SqlDbType.NVarChar).Value = learnword;
                    SqlDataReader sqlrdr = getDoUuTien.ExecuteReader();
                    if (sqlrdr.Read())
                    {
                        DoUuTien = sqlrdr.GetInt32(0).ToString();
                    }
                    
                    sqlrdr.Close();
                    if (DoUuTien == x.ToString())
                    {
                        DeleteHocTu(learnword);
                        return;
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
            
            SqlCommand sqlcmdupdate = new SqlCommand(s, sqlcnt);
            sqlcmdupdate.Parameters.Add("@tentu", SqlDbType.NVarChar).Value = learnword;
            
            sqlcmdupdate.ExecuteNonQuery();
        }
        public void DeleteHocTu(string s)
        {
            SqlCommand cmd = new SqlCommand("DELETE FROM HOCTU WHERE TenTu = " + s, sqlcnt);
            cmd.ExecuteNonQuery();
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
    }
}
