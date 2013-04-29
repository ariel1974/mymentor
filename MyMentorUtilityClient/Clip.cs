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
using Microsoft.Win32;
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
                    instance.DefaultSections = new sections();
                    instance.LockedSections = new sections();
                    instance.LearningOptions = new learningOptions();
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public string FileName { get; set; }
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string FontName { get; set; }
        public string FontFileName { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Tags { get; set; }
        public string Status { get; set; }
        public string AudioFileName { get; set; }
        public sections DefaultSections { get; set; }
        public sections LockedSections { get; set; }
        public learningOptions LearningOptions { get; set; }
        public string JsonSchemaFileName { get; set; }
        public string HtmlFileName { get; set; }
        public string MmnFileName { get; set; }

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
        public async static void Load(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(new Clip().GetType());

            using (StreamReader reader = new StreamReader(fileName))
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
            using (StreamWriter writer = new StreamWriter(this.FileName))
            {
                serializer.Serialize(writer.BaseStream, this);
            }

            this.IsDirty = false;
            this.IsNew = false;
        }

        public bool Publish()
        {
            string tempPath = System.IO.Path.GetTempPath();

            string fontsPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

            File.Copy(Path.Combine(fontsPath, this.FontFileName), Path.Combine(tempPath, this.FontFileName), true);

            //
            FileInfo info = new FileInfo(this.FileName);


            this.MmnFileName = Path.Combine(tempPath, string.Format("{0}.mmn", this.ID.ToString()));

            using (ZipFile zip = new ZipFile())
            {
                zip.AddFile(Path.Combine(tempPath, this.FontFileName), string.Empty);
                zip.AddFile(this.JsonSchemaFileName, string.Empty);

                if ( !string.IsNullOrEmpty(this.AudioFileName) && File.Exists(this.AudioFileName))
                {
                    File.Copy(this.AudioFileName, Path.Combine(tempPath, string.Format("{0}.mp3", this.ID.ToString())),true);
                    zip.AddFile(Path.Combine(tempPath, string.Format("{0}.mp3", this.ID.ToString())), string.Empty);
                }

                zip.AddFile(this.HtmlFileName, string.Empty);
                zip.Save(this.MmnFileName);
            }

            return true;
        }

        public async Task<bool> UploadAsync(IProgress<ParseUploadProgressEventArgs> progress)
        {
            //read file content
            byte[] bytes = File.ReadAllBytes(this.MmnFileName );
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
            clip["clipTitle"] = this.Title;
            clip["clipDescription"] = this.Description;
            clip["clipVersion"] = this.Version;
            clip["fontName"] = this.FontName;
            clip["fontFileName"] = this.FontFileName;
            clip["status"] = this.Status;
            clip["category"] = this.Category;
            clip["subCategory"] = this.SubCategory;
            clip["keywords"] = this.Tags;
            clip["clipFile"] = file;

            //TODO change real user
            clip["createdByUser"] = "natan";// user.Username;
            //clip.ACL = new ParseACL(user);
            await clip.SaveAsync();

            return true;
        }
        
        static RegistryKey fontsKey =
    Registry.LocalMachine.OpenSubKey(
        @"Software\Microsoft\Windows NT\CurrentVersion\Fonts");

        private string GetFontFile(string fontName)
        {
            foreach (string key in fontsKey.GetValueNames())
            {
                if (key.IndexOf(fontName) >= 0)
                {
                    return fontsKey.GetValue(key, string.Empty) as string;
                }
            }

            return string.Empty;
        }

        public bool ExtractHtml()
        {
            string tempPath = System.IO.Path.GetTempPath();

            this.HtmlFileName = Path.Combine(tempPath, string.Format("{0}.txt", this.ID.ToString()));

            RichTextBox rtb = new RichTextBox();
            rtb.Rtf = this.RtfText;

            System.IO.File.WriteAllText(this.HtmlFileName, rtb.Text.Replace("[", string.Empty).Replace("]", string.Empty));

            return true;
        }

        public bool ExtractJson()
        {
            this.FontFileName = GetFontFile(this.FontName);

            string tempPath = System.IO.Path.GetTempPath();

            this.JsonSchemaFileName = Path.Combine(tempPath, string.Format("{0}.json", this.ID.ToString()));

            jsonClip clip = new jsonClip();
            clip.id = this.ID.ToString();
            clip.title = this.Title;
            clip.fontName = this.FontName;
            clip.fontFileName = this.FontFileName;
            clip.description = this.Description;
            clip.clipVersion = this.Version;
            clip.schemaVersion = "1.02";
            clip.duration = this.Duration;
            clip.defaultSections = this.DefaultSections;
            clip.lockedSections = this.LockedSections;
            clip.learningOptions = this.LearningOptions;
            clip.category = this.Category;
            clip.subCategory = this.SubCategory;
            clip.tags = this.Tags;

            clip.paragraphs = this.Paragraphs;
            string json = JsonConvert.SerializeObject(clip,Formatting.Indented);

            System.IO.File.WriteAllText(this.JsonSchemaFileName, json);

            return true;
        }

    }
}
