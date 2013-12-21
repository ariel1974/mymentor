using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;

namespace MyMentor.ParseObjects
{
    [ParseClassName("WorldContentType")]
    public class WorldContentType : ParseObject
    {
        [ParseFieldName("value")]
        public string Value
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("clipTitlePattern")]
        public string ClipTitlePattern
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }
    }
}
