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
            ParseClient.Initialize("qvC0Pgq7QGSqntpqnA75vGnNUBewQ08DplQcJtMI", "65j2W36stF0GXUhJwAEuTwJp6geDEWeaUSSFyHKg");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new MainForm());
        }
    }
}
