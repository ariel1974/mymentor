using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using MyMentor.Forms;

namespace MyMentor.Resources
{
    static class ResourceHelper
    {
        static ResourceManager rm ;

        static ResourceHelper()
        {
             rm = new ResourceManager("MyMentor.Resources.Strings",
                                      typeof(FormStudio).Assembly);
        }

        public static string GetLabel(string code)
        {
                       
            return rm.GetString(code);

        }
    }
}
