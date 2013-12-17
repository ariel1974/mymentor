using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;

namespace MyMentor.ParseObjects
{
    [ParseClassName("ClipsV2")]
    public class ClipsV2 : ParseObject
    {
        [ParseFieldName("clipId")]
        public string ID
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("name")]
        public string Name
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("description")]
        public string Description
        {
            get { return GetProperty<string>(); }
            set { SetProperty<string>(value); }
        }

        [ParseFieldName("status")]
        public ParseRelation<ClipStatus> Status
        {
            get { return GetRelationProperty<ClipStatus>(); }
        }
    }
}
