using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MyMentorUtilityClient.Json
{
    public class sections
    {
        public int chapter { get; set; }
        public int paragraph { get; set; }
        public int sentence { get; set; }
        public int section { get; set; }
    }

    public class learningOptions
    {
        public int teacher1 { get; set; }
        public int teacherAndStudent { get; set; }
        public int teacher2 { get; set; }
        public int student { get; set; }
    }

    public class jsonClip
    {
        public string id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string schemaVersion { get; set; }
        public string clipVersion { get; set; }
        public string fontName { get; set; }
        public float fontSize { get; set; }
        public sections defaultSections { get; set; }
        public sections lockedSections { get; set; }
        public learningOptions defaultLearningOptions { get; set; }
        public learningOptions lockedLearningOptions { get; set; }
        public string category { get; set; }
        public string subCategory { get; set; }
        public string tags { get; set; }

        //[JsonIgnore]
        //public TimeSpan duration { get; set; }

        //[XmlIgnore]
        //public string clipDuration
        //{
        //    get
        //    {
        //        return this.duration.ToString(@"hh\:mm\:ss\.fff");
        //    }
        //}
        //public int length { get; set; }
        public Chapter chapter { get; set; }
    }
}
