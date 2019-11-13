using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return res;
        }
        public List<string> wordQuery(string query)
        {
            List<string> res = mainaccess.wordQuery(query);
            return res;
        }
        public string outputWordInfo(string query)
        {
            string res = mainaccess.recallWordInfo(query);
            return res;
        }
     }
}
