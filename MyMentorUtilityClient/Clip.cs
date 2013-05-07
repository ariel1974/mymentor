using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public const string PAR_SIGN_OPEN = "{{";
        public const string PAR_SIGN_CLOSE = "}}";

        public const string SEN_SIGN_OPEN = "((";
        public const string SEN_SIGN_CLOSE = "))";

        public const string SEC_SIGN_OPEN = "<<";
        public const string SEC_SIGN_CLOSE = ">>";

        public const string WOR_SIGN_OPEN = "[[";
        public const string WOR_SIGN_CLOSE = "]]";


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
                    instance.DefaultLearningOptions = new learningOptions();
                    instance.LockedLearningOptions = new learningOptions();
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
        public string Description { get; set; }
        public string Version { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Tags { get; set; }
        public string Status { get; set; }
        public string AudioFileName { get; set; }
        public long ClipSize { get; set; }
        public sections DefaultSections { get; set; }
        public sections LockedSections { get; set; }
        public learningOptions DefaultLearningOptions { get; set; }
        public learningOptions LockedLearningOptions { get; set; }
        public string JsonSchemaFileName { get; set; }
        public string HtmlFileName { get; set; }
        public string MmnFileName { get; set; }
        public Nullable<DateTime> LastPublishedOn { get; set; }

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
        public TimeSpan Duration
        {
            get
            {
                return new TimeSpan(this.Paragraphs.SelectMany(p => p.Words).Sum(p => p.Duration.Ticks) + this.Paragraphs.SelectMany( p => p.Sentences).Sum(s => s.Duration.Ticks));
            }
        }

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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void Load(string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(new Clip().GetType());

            using (StreamReader reader = new StreamReader(fileName))
            {
                object deserialized = serializer.Deserialize(reader.BaseStream);

                instance = (Clip)deserialized;
            }

            instance.FileName = fileName;
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

            this.MmnFileName = Path.Combine(tempPath, string.Format("{0}.mmn", this.ID.ToString()));

            using (ZipFile zip = new ZipFile())
            {
                //zip.AddFile(Path.Combine(tempPath, this.FontFileName), string.Empty);
                zip.AddFile(this.JsonSchemaFileName, string.Empty);

                if ( !string.IsNullOrEmpty(this.AudioFileName) && File.Exists(this.AudioFileName))
                {
                    File.Copy(this.AudioFileName, Path.Combine(tempPath, string.Format("{0}.mp3", this.ID.ToString())),true);
                    zip.AddFile(Path.Combine(tempPath, string.Format("{0}.mp3", this.ID.ToString())), string.Empty);
                }

                zip.AddFile(this.HtmlFileName, string.Empty);
                zip.Save(this.MmnFileName);
            }

            FileInfo info = new FileInfo(this.MmnFileName);
            this.ClipSize = info.Length;

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
            clip["clipSize"] = this.ClipSize;
            //clip["fontFileName"] = this.FontFileName;
            clip["status"] = this.Status;
            clip["category"] = this.Category;
            clip["subCategory"] = this.SubCategory;
            clip["keywords"] = this.Tags;
            clip["clipFile"] = file;

            //TODO change real user
            clip["createdByUser"] = "natan";// user.Username;
            //clip.ACL = new ParseACL(user);
            await clip.SaveAsync();

            this.LastPublishedOn = DateTime.Now;
            this.Save();

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

        private static string ReadTemplate()
        {
            string result = string.Empty;

            using (Stream stream = Assembly.GetExecutingAssembly()
                               .GetManifestResourceStream("MyMentorUtilityClient.Resources.template.html"))
                using (StreamReader reader = new StreamReader(stream))
                {
                     result = reader.ReadToEnd();
                }

            return result;
        }

        public bool ExtractHtml()
        {
            string tempPath = System.IO.Path.GetTempPath();
            string rtfCode =  Guid.NewGuid().ToString();

            string tempRtf = Path.Combine(tempPath, string.Format("{0}.rtf", rtfCode));
            string tempHtmlFolder = Path.Combine(tempPath, string.Format("{0}", Guid.NewGuid().ToString()));

            this.HtmlFileName = Path.Combine(tempHtmlFolder, string.Format("{0}.html", rtfCode));

            RichTextBox rtb = new RichTextBox();
            rtb.Rtf = this.RtfText;
            rtb.Text = rtb.Text.Replace(Clip.PAR_SIGN_CLOSE, string.Empty).Replace(Clip.PAR_SIGN_OPEN, string.Empty)
                .Replace(Clip.PAR_SIGN_CLOSE, string.Empty)
                .Replace(Clip.SEN_SIGN_OPEN, string.Empty)
                .Replace(Clip.SEN_SIGN_CLOSE, string.Empty)
                .Replace(Clip.SEC_SIGN_OPEN, string.Empty)
                .Replace(Clip.SEC_SIGN_CLOSE, string.Empty)
                .Replace(Clip.WOR_SIGN_OPEN, string.Empty)
                .Replace(Clip.WOR_SIGN_CLOSE, string.Empty);

            rtb.SaveFile(tempRtf);

            //get the full location of the assembly with DaoTests in it
            string rtf2html_exe = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Rtf2Html", "rtf2html.exe");

            var t = Task.Factory.StartNew(() =>
            {

                System.Diagnostics.ProcessStartInfo startInfo = new ProcessStartInfo(rtf2html_exe);
                startInfo.Arguments = string.Format("\"{0}\" \"{1}\"", tempRtf, tempHtmlFolder);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = System.Diagnostics.Process.Start(startInfo);
                while (!p.HasExited) ;
            });

            t.Wait();

            var newHtmlFileLocation = Path.Combine(tempPath, string.Format("{0}.html", this.ID.ToString()));

            if (File.Exists(this.HtmlFileName))
            {
                File.Copy(this.HtmlFileName, newHtmlFileLocation, true);
                this.HtmlFileName = newHtmlFileLocation;

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SaveJson(string json)
        {
            string tempPath = System.IO.Path.GetTempPath();

            this.JsonSchemaFileName = Path.Combine(tempPath, string.Format("{0}.json", this.ID.ToString()));
            System.IO.File.WriteAllText(this.JsonSchemaFileName, json);

            return true;
        }

        public string ExtractJson()
        {
            jsonClip clip = new jsonClip();
            clip.id = this.ID.ToString();
            clip.title = this.Title;
            clip.description = this.Description;
            clip.clipVersion = this.Version;
            clip.schemaVersion = "1.02";
            clip.duration = this.Duration;
            clip.defaultSections = this.DefaultSections;
            clip.lockedSections = this.LockedSections;
            clip.defaultLearningOptions = this.DefaultLearningOptions;
            clip.lockedLearningOptions = this.LockedLearningOptions;
            clip.category = this.Category;
            clip.subCategory = this.SubCategory;
            clip.tags = this.Tags;

            clip.paragraphs = this.Paragraphs;
            return JsonConvert.SerializeObject(clip,Formatting.Indented);
        }

    }
}
