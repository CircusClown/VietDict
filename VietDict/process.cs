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
    }
}
