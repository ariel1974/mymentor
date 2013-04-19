using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ionic.Zip;
using MyMentorUtilityClient.Json;
using Newtonsoft.Json;
using Parse;

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
            set
            {
                instance = value;
            }
        }

        public string Directory { get; set; }
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Tags { get; set; }
        public string Status { get; set; }
        public bool AutoIncrementVersion { get; set; }
 
        [XmlIgnore]
        public bool IsNew { get; set; }
        [XmlIgnore]
        public bool IsDirty { get; set; }

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
        public async static void Load(string path)
        {
            XmlSerializer serializer = new XmlSerializer(new Clip().GetType());

            using (StreamReader reader = new StreamReader(Path.Combine(path, "clip.mmt")))
            {
                object deserialized = serializer.Deserialize(reader.BaseStream);

                instance = (Clip)deserialized;
            }

            //check in the cloud if exists
            //var query = ParseObject.GetQuery("Clips").WhereEqualTo("clipId", instance.ID.ToString());

            //ParseObject obj = await query.FirstOrDefaultAsync();

            //if (obj == null)
            //{
            //    instance = null;
            //    throw new ApplicationException("סרט זה אינו קיים בענן");
            //}
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

            this.IsDirty = false;
        }

        public bool Publish()
        {
            using (ZipFile zip = new ZipFile())
            {
                zip.AddFile(Path.Combine(this.Directory, "schema.json"), string.Empty);
                //zip.AddFile(Path.Combine(this.Directory, "clip.mp3"), string.Empty);

                if (File.Exists(Path.Combine(this.Directory, "clip.mp3")))
                {
                    zip.AddFile(Path.Combine(this.Directory, "clip.mp3"), string.Empty);
                }

                zip.AddFile(Path.Combine(this.Directory, "clip.txt"), string.Empty);
                zip.Save(Path.Combine(this.Directory, string.Format("{0}.mmn", this.ID.ToString() ) ) );
            }

            return true;
        }

        public async Task<bool> UploadAsync(IProgress<ParseUploadProgressEventArgs> progress)
        {
            //read file content
            byte[] bytes = File.ReadAllBytes(Path.Combine(this.Directory, string.Format("{0}.mmn", this.ID.ToString() ) ) );
            ParseFile file = new ParseFile(string.Format("{0}.mmn", this.ID.ToString()), bytes);
            await file.SaveAsync(progress);

            var user = ParseUser.CurrentUser;

            //check if exists clip
            var query = ParseObject.GetQuery("Clips").WhereEqualTo("clipId", this.ID.ToString());

            ParseObject clip = await query.FirstOrDefaultAsync();

            if (clip == null)
            {
                clip = new ParseObject("Clips");
            }

            clip["clipId"] = this.ID.ToString();
            clip["clipVersion"] = this.Version;
            clip["status"] = this.Status;
            clip["category"] = this.Category;
            clip["subCategory"] = this.SubCategory;
            clip["keywords"] = this.Tags;
            clip["clipFile"] = file;
            clip["createdByUser"] = user.Username;
            //clip.ACL = new ParseACL(user);
            await clip.SaveAsync();

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
