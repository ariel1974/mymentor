using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrendanGrant.Helpers.FileAssociation;
using log4net;
using Microsoft.Win32;
using MyMentor;
using MyMentor.Forms;
using MyMentor.ParseObjects;
using Parse;

namespace MyMentor
{
    static class Program
    {
        public static readonly ILog Logger = LogManager.GetLogger("MyMentor");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //ParseObject.RegisterSubclass<Cat_Kria>();
            //ParseObject.RegisterSubclass<WorldContentType>();
            //ParseObject.RegisterSubclass<ClipStatus>();
            //ParseObject.RegisterSubclass<ClipsV2>();
            ParseClient.Initialize("qvC0Pgq7QGSqntpqnA75vGnNUBewQ08DplQcJtMI", "65j2W36stF0GXUhJwAEuTwJp6geDEWeaUSSFyHKg");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileAssociationInfo fai = new FileAssociationInfo(".mmnt");
            if (fai.Exists)
            {
                fai.Delete();
            }

            fai.Create("MyMentor");

            //Specify MIME type (optional)
            //fai.ContentType = "application/myfile";

            //Programs automatically displayed in open with list
            fai.OpenWithList = new string[] { "MyMentor.exe" };

            ProgramAssociationInfo pai = new ProgramAssociationInfo(fai.ProgID);
            if (pai.Exists)
            {
                pai.Delete();
            }

            pai.Create
            (
                //Description of program/file type
            "MyMentor file type",

            new ProgramVerb
                    (
                //Verb name
                    "Open",
                //Path and arguments to use
                    System.Reflection.Assembly.GetExecutingAssembly().Location + " \"%1\""
                    )
                );

            //optional
            //pai.DefaultIcon = new ProgramIcon(@"C:\SomePath\SomeIcon.ico");

            var file = string.Empty;

            if (args.Length > 0)
            {
                file = args[0];
            }
            
            //Load from App.Config file
            log4net.Config.XmlConfigurator.Configure(); 

            if (!CheckForInternetConnection())
            {
                MessageBox.Show("No internet connection, please try again later.", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else
            {
                Application.Run(new FormStudio(file));
            }
        }


        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead("http://www.google.com"))
                    {
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {
                Program.Logger.Error(ex);
                return true;
            }
        }
    }
}
