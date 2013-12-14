using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;

namespace MyMentor.ParseObjects
{
    [ParseClassName("Cat_Kria")]
    public class Cat_Kria : ParseObject
    {
        [ParseFieldName("value")]
        public string Value
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }
    }
}
