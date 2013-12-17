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
using MyMentor.ParseObjects;
using MyMentorUtilityClient.Json;
using NAudio.Wave;
using Newtonsoft.Json;
using Parse;
using SoundStudio;

namespace MyMentorUtilityClient
{
    public class Clip
    {

        private static Regex m_regexParagraphs = new Regex(@"(.+?)(\[3\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexSentenses = new Regex(@"(.+?)(\[2\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexSections = new Regex(@"(.+?)(\[1\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexWords = new Regex(@"(.+?)(\[0\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);

        public const string PAR_SIGN = "[3]";
        public const string SEN_SIGN = "[2]";
        public const string SEC_SIGN = "[1]";
        public const string WRD_SIGN = "[0]";

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
        public string Name { get; set; }
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

        [XmlIgnore]
        public bool Saved { get; set; }

        [JsonProperty("chapter")]
        public Chapter Chapter { get; set; }

        /// <summary>
        /// Holds the content non stripped
        /// </summary>
        public string Text { get; set; }

        public string RtfText { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void Devide()
        {
            try
            {
                var paragraphs_local = new List<Paragraph>();
                int paragraphIndex = -1;
                int sectionIndex = -1;
                int sentenceIndex = -1;
                int wordIndex = -1;

                int innerSentenceIndex = -1;
                int innerSectionIndex = -1;

                TimeSpan nextStartTime = TimeSpan.Zero;
                TimeSpan nextParagraphDuration = TimeSpan.Zero;
                TimeSpan nextSentenceDuration = TimeSpan.Zero;
                TimeSpan nextSectionDuration = TimeSpan.Zero;

                Word previousWord = null;
                Word firstWord = null;
                Word lastWord = null;

                int bufferIndex = 0;

                List<SectionMatch> matchesParagraphs = m_regexParagraphs.Matches(this.Text).Cast<Match>()
                            .Select(m => m.Groups[1])
                            .Select(m => new SectionMatch()
                            {
                                CharIndex = m.Index,
                                Length = m.Length,
                                Value = m.Value
                            })
                            .ToList();

                if (matchesParagraphs.Count == 0)
                {
                    matchesParagraphs.Add(new SectionMatch()
                    {
                        CharIndex = 0,
                        Length = this.Text.Length,
                        Value = this.Text
                    });
                }

                foreach (SectionMatch matchParagraph in matchesParagraphs)
                {
                    paragraphIndex++;
                    innerSentenceIndex = -1;

                    TimeSpan start = TimeSpan.Zero;
                    TimeSpan duration = TimeSpan.Zero;

                    paragraphs_local.Add(new Paragraph
                    {
                        Content = matchParagraph.StrippedValue,
                        RealCharIndex = matchParagraph.CharIndex,
                        CharIndex = matchParagraph.CharIndex - bufferIndex,
                        Index = paragraphIndex,
                        Sentences = new List<Sentence>()
                    });

                    bufferIndex += 3;

                    List<SectionMatch> matchesSentenses = m_regexSentenses.Matches(matchParagraph.Value).Cast<Match>()
                        .Select(m => m.Groups[1])
                        .Select(m => new SectionMatch()
                        {
                            CharIndex = m.Index,
                            Length = m.Length,
                            Value = m.Value
                        })
                        .ToList();

                    if (matchesSentenses.Count == 0)
                    {
                        matchesSentenses.Add(matchParagraph);
                    }

                    foreach (SectionMatch matchSentense in matchesSentenses)
                    {
                        start = TimeSpan.Zero;
                        duration = TimeSpan.Zero;

                        sentenceIndex++;
                        innerSentenceIndex++;
                        innerSectionIndex = -1;

                        int sectionsOffset = 0;
                        int wordsOffset = 0;

                        if (innerSentenceIndex > 0)
                        {
                            sectionsOffset = Math.Max(0, paragraphs_local[paragraphIndex].Sentences.Take(innerSentenceIndex).SelectMany(p => p.Sections).Count() * 3 - 3);
                            wordsOffset = Math.Max(0, paragraphs_local[paragraphIndex].Sentences.Take(innerSentenceIndex).SelectMany(p => p.Sections).SelectMany(w => w.Words).Count() * 3);
                        }

                        paragraphs_local[paragraphIndex].Sentences.Add(new Sentence
                        {   //                     5                               +           15    - 4   - (4 * 1) - 2
                            Content = matchSentense.StrippedValue,
                            RealCharIndex = paragraphs_local[paragraphIndex].RealCharIndex + matchSentense.CharIndex,
                            CharIndex = paragraphs_local[paragraphIndex].CharIndex + matchSentense.CharIndex - wordsOffset,// - Math.Max(0, 3 * innerSentenceIndex ),
                            Index = sentenceIndex,
                            Sections = new List<Section>()
                        });

                        bufferIndex += innerSentenceIndex == 0 ? 0 : 3;

                        List<SectionMatch> matchesSections = m_regexSections.Matches(matchSentense.Value).Cast<Match>()
                            .Select(m => m.Groups[1])
                            .Select(m => new SectionMatch()
                            {
                                CharIndex = m.Index,
                                Length = m.Length,
                                Value = m.Value
                            })
                            .ToList();

                        if (matchesSentenses.Count == 0)
                        {
                            matchesSentenses.Add(matchSentense);
                        }

                        foreach (SectionMatch matchSection in matchesSections)
                        {
                            int innerWordIndex = -1;
                            sectionIndex++;
                            innerSectionIndex++;

                            start = TimeSpan.Zero;
                            duration = TimeSpan.Zero;

                            int groupWordsBuffer = Math.Max(0, paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections.SelectMany(s => s.Words).Count() * 3);

                            paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections.Add(new Section
                            {
                                Content = matchSection.StrippedValue,
                                RealCharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].RealCharIndex + matchSection.CharIndex,
                                CharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].CharIndex + matchSection.CharIndex
                                - groupWordsBuffer,
                                Index = sectionIndex,
                                Words = new List<Word>(),
                            });

                            bufferIndex += innerSectionIndex == 0 ? 0 : 3;

                            List<SectionMatch> matchesWords = m_regexWords.Matches(matchSection.Value).Cast<Match>()
                                .Select(m => m.Groups[1])
                                .Select(m => new SectionMatch()
                                {
                                    CharIndex = m.Index,
                                    Length = m.Length,
                                    Value = m.Value
                                })
                                .ToList();

                            if (matchesWords.Count == 0)
                            {
                                matchesWords.Add(matchSection);
                            }

                            foreach (SectionMatch matchWord in matchesWords)
                            {
                                wordIndex++;
                                innerWordIndex++;

                                Word ex_word = null;

                                if (this.Chapter != null &&
                                    this.Chapter.Paragraphs != null)
                                {
                                    ex_word = this.Chapter.Paragraphs.SelectMany(s => s.Sentences).SelectMany(se => se.Sections).SelectMany(sc => sc.Words).FirstOrDefault(w => w.Index == wordIndex);

                                    if (ex_word != null)
                                    {
                                        start = ex_word.StartTime;

                                        if (ex_word.NextWord == null)
                                        {
                                            duration = ex_word.Duration;
                                        }
                                    }
                                }

                                var newWord = new Word
                                {
                                    RealCharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].RealCharIndex + matchWord.CharIndex,
                                    CharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].CharIndex + matchWord.CharIndex
                                    - (3 * Math.Max(0, paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Words.Count())),
                                    Index = wordIndex,
                                    Content = matchWord.Value,
                                    StartTime = start
                                };

                                //in case last word grab the duration
                                if (ex_word != null && ex_word.NextWord == null)
                                {
                                    newWord.Duration = duration;
                                }

                                //set previous word
                                if (previousWord != null)
                                {
                                    newWord.PreviousWord = previousWord;
                                }

                                paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Words.Add(newWord);

                                //set next word to previous one
                                if (wordIndex > 0)
                                {
                                    previousWord.NextWord = newWord;
                                }
                                else
                                {
                                    firstWord = newWord;
                                }

                                previousWord = newWord;
                                lastWord = newWord;
                                //save first word

                                bufferIndex += innerWordIndex == 0 ? 0 : 3;
                            }
                        }
                    }
                }

                //set is last to the last word (for duration manually);
                this.Chapter = new Chapter();
                this.Chapter.FirstWord = firstWord;
                this.Chapter.LastWord = lastWord;
                this.Chapter.Content = this.Text;
                this.Chapter.Paragraphs = paragraphs_local;
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException();
            }
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

            //for older versions
            if (string.IsNullOrEmpty(instance.Text))
            {
                System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
                rtBox.Rtf = instance.RtfText;
                instance.Text = rtBox.Text;
                rtBox.Dispose();
            }

            instance.Devide();
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

        private bool CutPreviewFile()
        {
             using (Mp3FileReader rdr = new Mp3FileReader(Path.ChangeExtension(this.FileName, ".mp3")))
            {
                int count = 1;
                Mp3Frame objmp3Frame = rdr.ReadNextFrame();
                System.IO.FileStream _fs = new System.IO.FileStream(Path.ChangeExtension(this.FileName, "_preview.mp3"), System.IO.FileMode.Create, System.IO.FileAccess.Write);

                while (objmp3Frame != null)
                {
                    if (count > 500) //retrieve a sample of 500 frames
                        break;

                    _fs.Write(objmp3Frame.RawData, 0, objmp3Frame.RawData.Length);
                    count = count + 1;
                    objmp3Frame = rdr.ReadNextFrame();
                }

                _fs.Close();
            }

            return true;
        }

        public bool Publish(AudioSoundEditor.AudioSoundEditor editor)
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

            //cut file
            CutPreviewFile();

            return true;
        }


        public async Task<bool> UploadAsync(IProgress<ParseUploadProgressEventArgs> progress)
        {
            //read file content
            byte[] bytes = File.ReadAllBytes(this.MmnFileName);
            ParseFile file = new ParseFile(string.Format("{0}.mmn", this.ID.ToString()), bytes);
            await file.SaveAsync(progress);

            //check if exists clip
            var query = ParseObject.GetQuery("ClipsV2").WhereEqualTo("clipId", this.ID.ToString());

            ParseObject clip = await query.FirstOrDefaultAsync();

            if (clip == null)
            {
                clip = new ParseObject("ClipsV2");
                clip["clipId"] = this.ID.ToString();
            }

            //clip.ID = this.ID.ToString();
            //clip.Name = this.Name;
            //clip.Description = this.Description;
            //clip.Status = ParseObject.CreateWithoutData("ClipStatus", this.Status);

            clip["name"] = this.Name;
            clip["description"] = this.Description;
            clip["version"] = this.Version;
            clip["clipType"] = "שיעור";
            clip["clipSize"] = this.ClipSize;
            //clip["fontName"] = this.FontFileName;

            clip["status"] = ParseObject.CreateWithoutData("ClipStatus", this.Status);

            //clip["category"] = this.Category;
            //clip["subCategory"] = this.SubCategory;
            //clip["keywords"] = this.Tags;
            clip["clipFile"] = file;
            //clip["createdByUser"] = user.Username;

            var preview = Path.ChangeExtension(this.FileName, "_preview.mp3");

            if (File.Exists(preview))
            {
                //read file content
                byte[] pbytes = File.ReadAllBytes(preview);
                ParseFile pfile = new ParseFile("preview.mp3", pbytes);
                await pfile.SaveAsync();

                clip["audioPreview"] = pfile;
            }

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

            doc.DocumentNode.SelectNodes("html").FirstOrDefault().Attributes.Add("style", "direction:" + (this.RightAlignment ? "rtl" : "ltr"));

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
            clip.name = this.Name;
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
