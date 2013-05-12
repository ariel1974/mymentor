using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrendanGrant.Helpers.FileAssociation;
using Microsoft.Win32;
using Parse;

namespace MyMentorUtilityClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ParseClient.Initialize("qvC0Pgq7QGSqntpqnA75vGnNUBewQ08DplQcJtMI", "65j2W36stF0GXUhJwAEuTwJp6geDEWeaUSSFyHKg");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileAssociationInfo fai = new FileAssociationInfo(".mnnx");
            if (fai.Exists)
            {
                fai.Delete();
            }

            fai.Create("MyMentor");

            //Specify MIME type (optional)
            //fai.ContentType = "application/myfile";

            //Programs automatically displayed in open with list
            fai.OpenWithList = new string[] { "MyMentorUtilityClient.exe" };

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
                    System.Reflection.Assembly.GetExecutingAssembly().Location + " %1"
                    )
                );

            //optional
            //pai.DefaultIcon = new ProgramIcon(@"C:\SomePath\SomeIcon.ico");

            Application.Run(new MainForm(args));
        }
    }
}
