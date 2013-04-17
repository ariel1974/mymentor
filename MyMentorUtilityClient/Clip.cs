using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ionic.Zip;
using MyMentorUtilityClient.Json;
using Newtonsoft.Json;

namespace MyMentorUtilityClient
{
    public class Clip
    {
        private static Clip instance;

        private Clip()
        {

        }

        public static Clip Current
        {
            get
            {
                if (instance == null)
                {
                    instance = new Clip();
                }

                return instance;
            }
        }

        public string Directory { get; set; }
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }

        [JsonProperty("paragraphs")]
        [XmlArrayItem("Paragraphs")]
        public List<Paragraph> Paragraphs { get; set; }
        public string RtfText { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public TimeSpan Duration { get; set; }

        [JsonProperty(PropertyName = "clipDuration")]
        [XmlIgnore]
        public string DurationText
        {
            get
            {
                return this.Duration.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        [JsonIgnore]
        [XmlAttribute("Duration")]
        public long XmlDuration
        {
            get { return Duration.Ticks; }
            set { Duration = new TimeSpan(value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void Load(string path)
        {
            XmlSerializer serializer = new XmlSerializer(new Clip().GetType());

            using (StreamReader reader = new StreamReader(Path.Combine(path, "clip.mmt")))
            {
                object deserialized = serializer.Deserialize(reader.BaseStream);

                instance = (Clip)deserialized;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save()
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            using (StreamWriter writer = new StreamWriter(Path.Combine(this.Directory, "clip.mmt")))
            {
                serializer.Serialize(writer.BaseStream, this);
            }
        }

        public bool Publish()
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AddFile(Path.Combine(this.Directory, "schema.json"), string.Empty);
                //zip.AddFile(Path.Combine(this.Directory, "clip.mp3"), string.Empty);
                zip.AddFile(Path.Combine(this.Directory, "clip.txt"), string.Empty);
                zip.Save(Path.Combine(this.Directory, string.Format("{0}.mmn", this.ID.ToString() ) ) );
            }

            return true;
        }

        public bool ExtractJson()
        {
            jsonClip clip = new jsonClip();
            clip.id = this.ID.ToString();
            clip.title = this.Title;
            clip.clipVersion = this.Version;
            clip.schemaVersion = "1.01";
            clip.duration = this.Duration;

            clip.paragraphs = this.Paragraphs;
            string json = JsonConvert.SerializeObject(clip,Formatting.Indented);

            System.IO.File.WriteAllText(Path.Combine(this.Directory, "schema.json"), json);

            RichTextBox rtb = new RichTextBox();
            rtb.Rtf = this.RtfText;

            System.IO.File.WriteAllText(Path.Combine(this.Directory, "clip.txt"), rtb.Text.Replace("[",string.Empty).Replace("]",string.Empty));

            return true;
        }
    }
}
