using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using HtmlAgilityPack;
using Ionic.Zip;
using Microsoft.Win32;
using MyMentorUtilityClient.Json;
using Newtonsoft.Json;
using Parse;

namespace MyMentorUtilityClient
{
    public class Clip
    {
        public const string PAR_SIGN = "[3]";
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
        public bool RightAlignment { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public float FontSize { get; set; }
        public string FontName { get; set; }
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

        [JsonIgnore]
        [XmlArrayItem("Paragraphs")]
        public List<Paragraph> Paragraphs { get; set; }

        [XmlIgnore]
        public bool IsNew { get; set; }
        [XmlIgnore]
        public bool IsDirty { get; set; }

        [JsonProperty("chapter")]
        public Chapter Chapter { get; set; }

        public string RtfText { get; set; }


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

        private void Save()
        {
            Save(null);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Save(AudioSoundEditor.AudioSoundEditor editor)
        {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            using (StreamWriter writer = new StreamWriter(this.FileName))
            {
                serializer.Serialize(writer.BaseStream, this);
            }

            this.IsDirty = false;
            this.IsNew = false;

            if (editor != null)
            {
                editor.ExportToFile(44100, 1, 0, 0, -1, Path.ChangeExtension(this.FileName, ".mp3"));
            }
        }

        public bool Publish()
        {
            string tempPath = System.IO.Path.GetTempPath();

            this.MmnFileName = Path.Combine(tempPath, string.Format("{0}.mmn", this.ID.ToString()));

            using (ZipFile zip = new ZipFile())
            {
                //zip.AddFile(Path.Combine(tempPath, this.FontFileName), string.Empty);
                zip.AddFile(this.JsonSchemaFileName, string.Empty);

                if (!string.IsNullOrEmpty(this.AudioFileName) && File.Exists(this.AudioFileName))
                {
                    File.Copy(this.AudioFileName, Path.Combine(tempPath, string.Format("{0}.mp3", this.ID.ToString())), true);
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
            byte[] bytes = File.ReadAllBytes(this.MmnFileName);
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
            //clip["fontName"] = this.FontFileName;
            clip["status"] = this.Status;
            clip["category"] = this.Category;
            clip["subCategory"] = this.SubCategory;
            clip["keywords"] = this.Tags;
            clip["clipFile"] = file;
            clip["createdByUser"] = user.Username;

            var ACL = new ParseACL(ParseUser.CurrentUser)
            {
                PublicReadAccess = true,
                PublicWriteAccess = false
            };
            clip.ACL = ACL;

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

        public bool ExtractHtml()
        {
            string tempPath = System.IO.Path.GetTempPath();
            string rtfCode = Guid.NewGuid().ToString();

            string tempRtf = Path.Combine(tempPath, string.Format("{0}.rtf", rtfCode));
            string tempHtmlFolder = Path.Combine(tempPath, string.Format("{0}", Guid.NewGuid().ToString()));

            this.HtmlFileName = Path.Combine(tempHtmlFolder, string.Format("{0}.html", rtfCode));

            System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
            rtb.Rtf = this.RtfText;
            rtb.Text = MainForm.m_regexAll.Replace(rtb.Text, string.Empty);
            //rtb.Text = Paragraphs.Select(p => p.Content).Aggregate((a, b) => a + b);
            rtb.SaveFile(tempRtf);

            //get the full location of the assembly with DaoTests in it
            string rtf2html_exe = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Rtf2Html", "rtf2html.exe");

            var t = Task.Factory.StartNew(() =>
            {
                System.Diagnostics.ProcessStartInfo startInfo = new ProcessStartInfo(rtf2html_exe);
                startInfo.Arguments = string.Format("\"{0}\" \"{1}\" /IDF", tempRtf, tempHtmlFolder);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = System.Diagnostics.Process.Start(startInfo);
                while (!p.HasExited) ;
            });

            t.Wait();

            var newHtmlFileLocation = Path.Combine(tempPath, string.Format("{0}.html", this.ID.ToString()));

            if (File.Exists(this.HtmlFileName))
            {
                FixHtmlAttributes(this.HtmlFileName);
                File.Copy(this.HtmlFileName, newHtmlFileLocation, true);
                this.HtmlFileName = newHtmlFileLocation;

                return true;
            }
            else
            {
                return false;
            }
        }

        private void FixHtmlAttributes(string path)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);

            doc.DocumentNode.SelectNodes("html").FirstOrDefault().Attributes.Add("style", "direction:" + (this.RightAlignment ? "rtl" : "ltr") );

            doc.Save(path);

            var reg = new Regex(" (?= )|(?<= ) ");

            var fileContents = System.IO.File.ReadAllText(path);
            fileContents = reg.Replace(fileContents, "&nbsp;");//.Replace("</p>","</p>&nbsp;");
            System.IO.File.WriteAllText(path, fileContents);
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
            clip.fontSize = this.FontSize;
            clip.fontName = this.FontName;
            clip.chapter = this.Chapter;
            clip.schemaVersion = "1.02";
            //clip.duration = this.Duration;
            clip.defaultSections = this.DefaultSections;
            clip.lockedSections = this.LockedSections;
            clip.defaultLearningOptions = this.DefaultLearningOptions;
            clip.lockedLearningOptions = this.LockedLearningOptions;
            clip.category = this.Category;
            clip.subCategory = this.SubCategory;
            clip.tags = this.Tags;

            //clip.paragraphs = this.Paragraphs;
            return JsonConvert.SerializeObject(clip, Formatting.Indented);
        }

    }
}
