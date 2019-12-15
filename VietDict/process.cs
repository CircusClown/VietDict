using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Speech.Synthesis;

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
            List<string> res = mainaccess.wordQuery(query);
            for (int i = 0; i < res.Count; i++)
            {
                res[i] = Regex.Replace(res[i], "<sq>", "'");
            }
            return res;
        }
        public string outputWordBaseInfo(string query, out string pronounce)
        {
            string res = mainaccess.recallWordInfo(query, out pronounce);
            if (res == null || res == "") return "Không có nghĩa cơ bản cho từ này";
            res = Regex.Replace(res, "<sq>", "'");
            res = Regex.Replace(res, "<dq>", "\"");
            res = Regex.Replace(res, "Loại từ", "-> ");
            pronounce = Regex.Replace(pronounce, "<sq>", "'");
            return res;
        }
        public string outputWordSpecialInfo(string query)
        {
            string res = mainaccess.specialtyWordMean(query);
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
            List<string> res = mainaccess.queryCollectionWord(query, selectCollection);
            for (int i = 0; i < res.Count; i++)
            {
                res[i] = Regex.Replace(res[i], "<sq>", "'");
            }
            return res;
        }

    }
}
