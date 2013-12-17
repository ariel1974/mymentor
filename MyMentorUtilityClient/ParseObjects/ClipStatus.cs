using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;

namespace MyMentor.ParseObjects
{
    [ParseClassName("ClipStatus")]
    public class ClipStatus : ParseObject
    {
        [ParseFieldName("status")]
        public string Status
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("isVisibleToMentor")]
        public bool IsVisibleToMentor
        {
            get { return GetProperty<bool>(); }
            set { SetProperty<bool>(value); }
        }
    }
}
