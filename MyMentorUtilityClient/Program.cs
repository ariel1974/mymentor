using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Parse;

namespace MyMentorUtilityClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ParseClient.Initialize("02MGXhihNwW8TJGbCH2wyD2nQZ4ZFSiVNec2AkHQ", "WWnFNF7Gxizq7lzAI38MEBobco6yqhywR9baFtls");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
