using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace MyMentorUtilityClient.Json
{
    public class jsonClip
    {
        public string id { get; set; }
        public string title { get; set; }
        public string schemaVersion { get; set; }
        public string clipVersion { get; set; }

        [JsonIgnore]
        public TimeSpan duration { get; set; }

        [XmlIgnore]
        public string clipDuration
        {
            get
            {
                return this.duration.ToString(@"hh\:mm\:ss\.fff");
            }
        }
        public List<Paragraph> paragraphs { get; set; }
    }
}
