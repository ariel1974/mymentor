using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MyMentor.Json
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
        public string name { get; set; }
        public string description { get; set; }
        public string performer { get; set; }
        public string performerEnglish { get; set; }
        public string remarks { get; set; }
        public string schemaVersion { get; set; }
        public string clipVersion { get; set; }
        public sections defaultSections { get; set; }
        public sections lockedSections { get; set; }
        public learningOptions defaultLearningOptions { get; set; }
        public learningOptions lockedLearningOptions { get; set; }
        public string voicePrompts { get; set; }
        public string category1 { get; set; }
        public string category2 { get; set; }
        public string category3 { get; set; }
        public string category4 { get; set; }
        public string keywords { get; set; }
        public bool isNikudIncluded { get; set; }
        public bool isTeamimIncluded { get; set; }
        public Dictionary<string, List<float>> fonts { get; set; }
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
        public Chapter onlyNikudChapter { get; set; }
        public Chapter onlyTeamimChapter { get; set; }
        public Chapter clearTextChapter { get; set; }
    }
}
