using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrendanGrant.Helpers.FileAssociation;
using Microsoft.Win32;
using MyMentor;
using MyMentor.Forms;
using MyMentor.ParseObjects;
using Parse;

namespace MyMentor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //ParseObject.RegisterSubclass<Cat_Kria>();
            ParseObject.RegisterSubclass<WorldContentType>();
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

            Application.Run(new FormStudio(file));
            //Application.Run(new FormMain(file));
        }
    }
}
