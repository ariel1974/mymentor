using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            RegexOptions options = RegexOptions.None;

            Regex regex = new Regex(@"(?<=\{\{)[ ]{1,}|[ ]{1,}(?=\}\})", options);
            Console.Write(regex.Replace(@"{{ asda sdasd << asd asda >> asdad sasd }}", @""));

            Console.ReadKey();

        }
    }
}
