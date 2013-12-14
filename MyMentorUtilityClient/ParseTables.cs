using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyMentor.ParseObjects;
using Parse;

namespace MyMentor
{
    public static class ParseTables
    {
        public static async Task<IEnumerable<WorldContentType>> GetWorldContentTypes()
        {
            var query = from cat in new ParseQuery<WorldContentType>()
                        where cat.Value != ""
                        select cat;

            return await query.FindAsync();
        }

        public static async Task<IEnumerable<Cat_Kria>> GetKriot()
        {
            var query = from cat in new ParseQuery<Cat_Kria>()
                        where cat.Value != ""
                        select cat;

            return await query.FindAsync();
        }

    }
}
