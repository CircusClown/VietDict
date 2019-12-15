using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace VietDict
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            System.IO.DirectoryInfo x = System.IO.Directory.GetParent(executable);
            x = System.IO.Directory.GetParent(x.FullName);
            string path = (System.IO.Path.GetDirectoryName(x.FullName));
            //DELETE THIS BEFORE FINAL BUILD
            AppDomain.CurrentDomain.SetData("SolutionDirectory", path);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new XtraForm1());
        }
    }
}
