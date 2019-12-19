using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Speech.Synthesis;
using RestSharp;
using System.Configuration;
using System.Windows.Forms;

namespace VietDict
{
    class process
    {
        dbaccess mainaccess;
        private List<string> searchHistory = new List<string>();
        private int historyIndex = 0;

        public process()
        {
            mainaccess = new dbaccess();
        }
        public List<string> wordListing()
        {
            List<string> res = mainaccess.loadAllWord();
            for (int i = 0; i < res.Count; i++)
            {
                res[i] = Regex.Replace(res[i], "<sq>", "'");
            }
            return res;
        }
        public List<string> collectionListing()
        {
            List<string> res = mainaccess.listCollection();
            return res;
        }
        public List<string> wordQuery(string query)
        {
            List<string> res = mainaccess.wordQuery(Regex.Replace(query, "'", "<sq>"));
            for (int i = 0; i < res.Count; i++)
            {
                res[i] = Regex.Replace(res[i], "<sq>", "'");
            }
            return res;
        }
        public string outputWordBaseInfo(string query, out string pronounce, out string img_path)
        {
            string res = mainaccess.recallWordInfo(Regex.Replace(query, "'", "<sq>"), out pronounce, out img_path);
            if (res == null || res == "") return "Không có nghĩa cơ bản cho từ này";
            res = Regex.Replace(res, "<sq>", "'");
            res = Regex.Replace(res, "<dq>", "\"");
            res = Regex.Replace(res, "Loại từ", "-> ");
            pronounce = Regex.Replace(pronounce, "<sq>", "'");
            return res;
        }
        public string outputWordSpecialInfo(string query)
        {
            string res = mainaccess.specialtyWordMean(Regex.Replace(query, "'", "<sq>"));
            if (res == null || res == "") return "Không có nghĩa chuyên ngành cho từ này";
            res = Regex.Replace(res, "<sq>", "'");
            return res;
        }
        public void speakWord(string input)
        {
            SpeechSynthesizer a = new SpeechSynthesizer();
            a.Speak(input);
        }
        public void addToHistory(string input)
        {
            searchHistory.Add(input);
            if (searchHistory.Count > 50)
            {
                searchHistory.RemoveAt(0);

            }
            historyIndex = searchHistory.Count - 1;

        }
        public string retrievePrevHistory()
        {
            if (searchHistory.Count == 0) return "";
            if (historyIndex > 0) historyIndex--;
            return searchHistory[historyIndex];
        }
        public string retrieveNextHistory()
        {
            if (searchHistory.Count == 0) return "";
            if (historyIndex < searchHistory.Count - 1) historyIndex++;
            return searchHistory[historyIndex];
        }
        public bool isBookmarked(string query)
        {
            return mainaccess.isInCollection(Regex.Replace(query, "'", "<sq>"), "Bookmark");
        }

        public bool removeFromBookmark(string input)
        {
            return mainaccess.removeFromCollection(Regex.Replace(input, "'", "<sq>"), "Bookmark");
        }

        public bool addToBookmark(string input)
        {
            return mainaccess.addToCollection(Regex.Replace(input, "'", "<sq>"), "Bookmark");
        }

        internal List<string> collectionWordListing(string selectCollection)
        {
            List<string> res = mainaccess.loadCollectionWord(selectCollection);
            for (int i = 0; i < res.Count; i++)
            {
                res[i] = Regex.Replace(res[i], "<sq>", "'");
            }
            return res;
        }

        internal List<string> wordCollectionQuery(string query, string selectCollection)
        {
            List<string> res = mainaccess.queryCollectionWord(Regex.Replace(query, "'", "<sq>"), selectCollection);
            for (int i = 0; i < res.Count; i++)
            {
                res[i] = Regex.Replace(res[i], "<sq>", "'");
            }
            return res;
        }

        public bool insertCollection(string colName)
        {
            return mainaccess.insertCollection(colName);
        }

        public string translatePhrase(string query)
        {
            //MOVE API KEY TO SOMEWHERE SECURE ASAP
            string URL = "https://translation.googleapis.com/language/translate/v2?key=" + ConfigurationManager.AppSettings["GoogleAPIKey"] + "&source=en&target=vi&q=" + query;
            //string urlParameters = "?api_key=123";
            var client = new RestClient(URL);
            var response = client.Execute(new RestRequest());
            dynamic stuff = Newtonsoft.Json.JsonConvert.DeserializeObject(response.Content);
            if (stuff.data.translations[0].translatedText == null) return "<Dịch thất bại, xin hãy kiểm tra kết nối Internet>";
            return stuff.data.translations[0].translatedText;
        }

        internal void addToCollection(string word, string col)
        {
            mainaccess.addToCollection(Regex.Replace(word, "'", "<sq>"), col);
        }

        internal void removeFromCollection(string word, string col)
        {
            mainaccess.removeFromCollection(Regex.Replace(word, "'", "<sq>"), col);
        }
        internal void removeCollection(string col)
        {
            mainaccess.removeCollection(col);
        }

        internal bool saveWord(string word, string illu_path, string pronounce, string bmean, string smean, string edit_target = "")
        {
            bmean = Regex.Replace(bmean, "'", "<sq>");
            bmean = Regex.Replace(bmean, "\"", "<dq>");
            bmean = Regex.Replace(bmean, "-> ", "Loại từ ");
            pronounce = Regex.Replace(pronounce, "'", "<sq>");
            edit_target = Regex.Replace(edit_target, "'", "<sq>");
            word = Regex.Replace(word, "'", "<sq>");
            bool res = mainaccess.saveWord(word, illu_path, pronounce, bmean, smean, edit_target);
            return res;
        }

        internal bool removeWord(string word)
        {
            bool res = mainaccess.removeWord(Regex.Replace(word, "'", "<sq>"));
            return res;
        }

        public List<string> wordlearn(int v)
        {
            List<string> WL = new List<string>();
            int four = 0, three = 0, two = 0, one = 0;
            Random rnd = new Random();
            for (int i = 0; i < v; i++)
            {
                int n = rnd.Next(0, 100);
                if (0 <= n && n < 50)
                {
                    four++;
                }
                else if (50 <= n && n < 70)
                {
                    three++;
                }
                else if (70 <= n && n < 90)
                {
                    two++;
                }
                else if (90 <= n && n < 100)
                {
                    one++;
                }
            }
            WL = mainaccess.loadLearnWord(four, three, two, one);
            return WL;
        }
        public void danhgiaLW(int x, string learnword)
        {
            string s = "";
            switch (x)
            {
                case 1:
                    s = "UPDATE HOCTU SET DoUuTien = 1 WHERE TenTu = @word";
                    break;
                case 2:
                    s = "UPDATE HOCTU SET DoUuTien = 2 WHERE TenTu = @word";
                    break;
                case 3:
                    s = "UPDATE HOCTU SET DoUuTien = 3 WHERE TenTu = @word";
                    break;
                case 4:
                    s = "UPDATE HOCTU SET DoUuTien = 4 WHERE TenTu = @word";
                    break;
                default:
                    break;
            }
            mainaccess.UpdateDoKho(s, x, learnword);

        }
        public string getMeaning(string s)
        {
            string meaning = "";
            meaning = mainaccess.MeaningLearnWord(s);
            return meaning;
        }
    }
}
