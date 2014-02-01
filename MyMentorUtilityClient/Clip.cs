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
using MyMentor.Json;
using MyMentor.ParseObjects;
using NAudio.Wave;
using Newtonsoft.Json;
using Parse;

namespace MyMentor
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
                    instance.ReadingDates = new List<DateTime>();
                    instance.User = ParseUser.CurrentUser.ObjectId;
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
        public decimal Price { get; set; }
        public decimal PriceSupport { get; set; }
        public string User { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public bool RightAlignment { get; set; }
        public string Description { get; set; }
        public string EnglishDescription { get; set; }
        public string Remarks { get; set; }
        public string Version { get; set; }
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public string Category4 { get; set; }
        public string Keywords { get; set; }
        public List<DateTime> ReadingDates { get; set; }
        public string Status { get; set; }
        public string ContentType { get; set; }
        public string AudioFileName { get; set; }
        public long ClipSize { get; set; }
        public long DemoClipSize { get; set; }
        public string ClipType { get; set; }
        public Nullable<DateTime> Expired { get; set; }
        public sections DefaultSections { get; set; }
        public sections LockedSections { get; set; }
        public learningOptions DefaultLearningOptions { get; set; }
        public learningOptions LockedLearningOptions { get; set; }
        public string JsonSchemaFileName { get; set; }
        public string FingerPrint { get; set; }
        public string HtmlFileName { get; set; }
        public string HtmlOnlyNikudFileName { get; set; }
        public string HtmlOnlyTeamimFileName { get; set; }
        public string HtmlClearTextFileName { get; set; }
        public string MmnFileName { get; set; }
        public string MmnDemoFileName { get; set; }
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

        [JsonProperty("onlyNikudChapter")]
        public Chapter OnlyNikudChapter { get; set; }

        [JsonProperty("clearTextChapter")]
        public Chapter ClearTextChapter { get; set; }

        [JsonProperty("onlyTeamimChapter")]
        public Chapter OnlyTeamimChapter { get; set; }

        [JsonProperty("isNikudIncluded")]
        public bool IsNikudIncluded { get; set; }

        [JsonProperty("isTeamimIncluded")]
        public bool IsTeamimIncluded { get; set; }

        /// <summary>
        /// Holds the content non stripped
        /// </summary>
        public string Text { get; set; }

        public string RtfText { get; set; }

        public void Devide()
        {
            Devide(false, false);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Devide(bool onlyNikud, bool onlyTeamim)
        {
            try
            {
                var text = this.Text;

                if (onlyNikud)
                {
                    text = text.RemoveTeamim();
                }

                if (onlyTeamim)
                {
                    text = text.RemoveNikud();
                }

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

                List<SectionMatch> matchesParagraphs = m_regexParagraphs.Matches(text).Cast<Match>()
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

                if (!onlyNikud && !onlyTeamim)
                {
                    //set is last to the last word (for duration manually);
                    this.Chapter = new Chapter();
                    this.Chapter.FirstWord = firstWord;
                    this.Chapter.LastWord = lastWord;
                    this.Chapter.Content = text;
                    this.Chapter.Paragraphs = paragraphs_local;
                }
                else if (onlyNikud && !onlyTeamim)
                {
                    //set is last to the last word (for duration manually);
                    this.OnlyNikudChapter = new Chapter();
                    this.OnlyNikudChapter.FirstWord = firstWord;
                    this.OnlyNikudChapter.LastWord = lastWord;
                    this.OnlyNikudChapter.Content = text;
                    this.OnlyNikudChapter.Paragraphs = paragraphs_local;
                }
                else if (onlyNikud && onlyTeamim)
                {
                    //set is last to the last word (for duration manually);
                    this.ClearTextChapter = new Chapter();
                    this.ClearTextChapter.FirstWord = firstWord;
                    this.ClearTextChapter.LastWord = lastWord;
                    this.ClearTextChapter.Content = text;
                    this.ClearTextChapter.Paragraphs = paragraphs_local;
                }
                else
                {
                    //set is last to the last word (for duration manually);
                    this.OnlyTeamimChapter = new Chapter();
                    this.OnlyTeamimChapter.FirstWord = firstWord;
                    this.OnlyTeamimChapter.LastWord = lastWord;
                    this.OnlyTeamimChapter.Content = text;
                    this.OnlyTeamimChapter.Paragraphs = paragraphs_local;
                }
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
            FileInfo info = new FileInfo(fileName);

            //extract zip
            using (var zip = Ionic.Zip.ZipFile.Read(fileName))
            {
                zip.Password = "97359194";
                zip[0].FileName = Path.ChangeExtension(info.Name , ".mmnt2");
                zip[0].Extract(System.IO.Path.GetTempPath(), ExtractExistingFileAction.OverwriteSilently);
            }

            //read stream
            using (StreamReader reader = new StreamReader(Path.Combine(System.IO.Path.GetTempPath(), Path.ChangeExtension(info.Name, ".mmnt2"))))
            {
                object deserialized = serializer.Deserialize(reader.BaseStream);
                instance = (Clip)deserialized;
            }

            //remove stream
            File.Delete(Path.Combine(System.IO.Path.GetTempPath(), Path.ChangeExtension(info.Name, ".mmnt2")));

            if (string.IsNullOrEmpty(instance.FingerPrint))
            {
                throw new ApplicationException("שיעור זה נוצר בגרסאות קודמות של הסטודיו ואינו נתמך עוד");
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

            if (string.IsNullOrEmpty(instance.ClipType))
            {
                //set as clip
                instance.ClipType = "piL85bMGtR";
            }

            if (instance.ReadingDates == null)
            {
                instance.ReadingDates = new List<DateTime>();
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

            using (ZipFile zip = new ZipFile())
            {
                zip.Password = "97359194";
                zip.AddFile(this.FileName, string.Empty);
                zip.Save(Path.ChangeExtension(this.FileName, ".mmnt2"));
            }

            File.Delete(this.FileName);
            File.Move(Path.ChangeExtension(this.FileName, ".mmnt2"), this.FileName);

            this.IsDirty = false;
            this.IsNew = false;
             
            if (editor != null && editor.GetSoundDuration() > 0 && !string.IsNullOrEmpty(this.FileName))
            {
                editor.ExportToFile(44100, 1, 0, 0, -1, Path.ChangeExtension(this.FileName, ".mp3"));
            }
        }

        private Task<bool> CutPreviewFile()
        {
            return Task.Factory.StartNew(() =>
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
                        });
        }

        public bool Publish(AudioSoundEditor.AudioSoundEditor editor, Action<string> log)
        {
            log("מכין קובץ להעלאה...");

            var t1 = Task.Factory.StartNew(() =>    
            {
                string tempPath = System.IO.Path.GetTempPath();

                this.MmnFileName = Path.Combine(tempPath, string.Format("{0}.mmn", this.ID.ToString()));

                using (ZipFile zip = new ZipFile())
                {
                    //zip.AddFile(Path.Combine(tempPath, this.FontFileName), string.Empty);
                    zip.AddFile(this.JsonSchemaFileName, string.Empty);

                    if (editor.GetSoundDuration() > 0 && File.Exists(Path.ChangeExtension(this.FileName, ".mp3")))
                    {
                        File.Copy(Path.ChangeExtension(this.FileName, ".mp3"), Path.Combine(tempPath, string.Format("{0}.mp3", this.ID.ToString())), true);
                        zip.AddFile(Path.Combine(tempPath, string.Format("{0}.mp3", this.ID.ToString())), string.Empty);
                    }

                    zip.AddFile(this.HtmlFileName, string.Empty);
                    zip.AddFile(this.HtmlOnlyNikudFileName, string.Empty);
                    zip.AddFile(this.HtmlOnlyTeamimFileName, string.Empty);
                    zip.AddFile(this.HtmlClearTextFileName, string.Empty);
                    zip.Save(this.MmnFileName);
                }

                FileInfo info = new FileInfo(this.MmnFileName);
                this.ClipSize = info.Length;


                //cut for audio preview file
                if (editor.GetSoundDuration() > 0)
                {
                    var processData = CutPreviewFile().ContinueWith((cut) =>
                    {
                        if (cut.Result)
                        {
                            //create demo file
                            this.MmnDemoFileName = Path.Combine(tempPath, string.Format("{0}_demo.mmn", this.ID.ToString()));

                            using (ZipFile zip = new ZipFile())
                            {
                                //zip.AddFile(Path.Combine(tempPath, this.FontFileName), string.Empty);
                                zip.AddFile(this.JsonSchemaFileName, string.Empty);

                                File.Copy(Path.ChangeExtension(this.FileName, "_preview.mp3"), Path.Combine(tempPath, string.Format("{0}_demo.mp3", this.ID.ToString())), true);
                                zip.AddFile(Path.Combine(tempPath, string.Format("{0}_demo.mp3", this.ID.ToString())), string.Empty);

                                zip.AddFile(this.HtmlFileName, string.Empty);
                                zip.AddFile(this.HtmlOnlyNikudFileName, string.Empty);
                                zip.AddFile(this.HtmlOnlyTeamimFileName, string.Empty);
                                zip.AddFile(this.HtmlClearTextFileName, string.Empty);
                                zip.Save(this.MmnDemoFileName);
                            }

                            FileInfo info2 = new FileInfo(this.MmnDemoFileName);
                            this.DemoClipSize = info2.Length;

                        }
                    }); 



                }

            });

            t1.Wait();

            return true;
        }


        public async Task<bool> UploadAsync(IProgress<ParseUploadProgressEventArgs> progress)
        {
            //read mmn file content
            byte[] mmnBytes = File.ReadAllBytes(this.MmnFileName);
            ParseFile mmnFile = new ParseFile(string.Format("{0}.mmn", this.ID.ToString()), mmnBytes);
            await mmnFile.SaveAsync(progress);

            ParseFile mmnDemoFile = null;

            //read mmn file content
            if (!string.IsNullOrEmpty(this.MmnDemoFileName))
            {
                byte[] mmnDemoBytes = File.ReadAllBytes(this.MmnDemoFileName);
                mmnDemoFile = new ParseFile(string.Format("{0}_demo.mmn", this.ID.ToString()), mmnDemoBytes);
                await mmnDemoFile.SaveAsync(progress);
            }

            //check if exists clip
            var query = ParseObject.GetQuery("ClipsV2").WhereEqualTo("clipId", this.ID.ToString());

            ParseObject clip = await query.FirstOrDefaultAsync();

            if (clip == null)
            {
                clip = new ParseObject("ClipsV2");
                clip["clipId"] = this.ID.ToString();
            }

            clip["name"] = this.Title;
            clip["clipSourceText"] = this.Text;
            clip["description"] = this.Description;
            clip["descriptionEnglish"] = this.EnglishDescription;
            clip["remarks"] = this.Remarks;
            clip["version"] = this.Version;
            clip["fingerPrint"] = this.FingerPrint;
            clip["clipType"] = ParseObject.CreateWithoutData("ClipType", this.ClipType);
            clip["price"] = (float)this.Price;
            clip["priceWithSupport"] = (float)this.PriceSupport;
            clip["status"] = ParseObject.CreateWithoutData("ClipStatus", this.Status);
            clip["existsNikud"] = this.IsNikudIncluded;
            clip["existsTeamim"] = this.IsTeamimIncluded;
            clip["contentType"] = ParseObject.CreateWithoutData("WorldContentType",this.ContentType);

            if (!string.IsNullOrEmpty(this.Category1))
            {
                clip["category1"] = ParseObject.CreateWithoutData("Category1", this.Category1);
            }
            //else
            //{
            //    clip["category1"] = ParseObject.CreateWithoutData("Category1", this.Category1);
            //}

            if (!string.IsNullOrEmpty(this.Category2))
            {
                clip["category2"] = ParseObject.CreateWithoutData("Category2", this.Category2);
            }
            //else
            //{
            //    clip["category2"] = string.Empty;
            //}

            if (!string.IsNullOrEmpty(this.Category3))
            {
                clip["category3"] = ParseObject.CreateWithoutData("Category3", this.Category3);
            }
            //else
            //{
            //    clip["category3"] = null;
            //}

            if (!string.IsNullOrEmpty(this.Category4))
            {
                clip["category4"] = ParseObject.CreateWithoutData("Category4", this.Category4);
            }
            //else
            //{
            //    clip["category4"] = ParseObject.CreateWithoutData("Category4", null); ;
            //}

            clip["updatedByMyMentor"] = DateTime.Now;
            clip["keywords"] = this.Keywords.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            clip["clipFile"] = mmnFile;
            clip["clipSize"] = this.ClipSize;

            if (!string.IsNullOrEmpty(this.MmnDemoFileName))
            {
                clip["demoClipFile"] = mmnDemoFile;
                clip["demoClipSize"] = this.DemoClipSize;
            }
            clip["readingDates"] = this.ReadingDates;

            if (this.Expired.HasValue)
            {
                clip["expiration"] = this.Expired.Value;
            }
            else
            {
                clip["expiration"] = null;
            }

            clip["teacher"] = ParseUser.CreateWithoutData("_User", ParseUser.CurrentUser.ObjectId);

            if (!string.IsNullOrEmpty(this.FileName))
            {
                var preview = Path.ChangeExtension(this.FileName, "_preview.mp3");

                if (File.Exists(preview))
                {
                    //read file content
                    byte[] pbytes = File.ReadAllBytes(preview);
                    ParseFile pfile = new ParseFile("preview.mp3", pbytes);
                    await pfile.SaveAsync();

                    clip["audioPreview"] = pfile;
                }
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ExtractHtml(Action<string> log)
        {
            string tempPath = System.IO.Path.GetTempPath();
            string rtfCode = Guid.NewGuid().ToString();

            string tempRtf = Path.Combine(tempPath, string.Format("{0}.rtf", rtfCode));
            string tempOnlyNikudRtf = Path.Combine(tempPath, string.Format("{0}_onlyNikud.rtf", rtfCode));
            string tempOnlyTeamimRtf = Path.Combine(tempPath, string.Format("{0}_onlyTeamim.rtf", rtfCode));
            string tempClearTextRtf = Path.Combine(tempPath, string.Format("{0}_clearText.rtf", rtfCode));
            string tempHtmlFolder = Path.Combine(tempPath, string.Format("{0}", Guid.NewGuid().ToString()));

            this.HtmlFileName = Path.Combine(tempHtmlFolder, string.Format("{0}.html", rtfCode));
            this.HtmlOnlyNikudFileName = Path.Combine(tempHtmlFolder, string.Format("{0}_onlyNikud.html", rtfCode));
            this.HtmlOnlyTeamimFileName = Path.Combine(tempHtmlFolder, string.Format("{0}_onlyTeamim.html", rtfCode));
            this.HtmlClearTextFileName = Path.Combine(tempHtmlFolder, string.Format("{0}_clearText.html", rtfCode));

            log("מכין טקסט ...");

            var t0 = Task.Factory.StartNew(() =>
                {
                    System.Windows.Forms.RichTextBox rtb = new System.Windows.Forms.RichTextBox();
                    rtb.Rtf = this.RtfText;
                    rtb.Text = this.Text.Replace("[3]", string.Empty).Replace("[2]", string.Empty).Replace("[1]", string.Empty).Replace("[0]", string.Empty);// MainForm.m_regexAll.Replace(rtb.Text, string.Empty);
                    //rtb.Text = Paragraphs.Select(p => p.Content).Aggregate((a, b) => a + b);
                    rtb.SaveFile(tempRtf);

                    //only nikud
                    rtb.Text = this.Text.Replace("[3]", string.Empty).Replace("[2]", string.Empty).Replace("[1]", string.Empty).Replace("[0]", string.Empty)
                        .RemoveTeamim();
                    rtb.Refresh();
                    rtb.SaveFile(tempOnlyNikudRtf);

                    //only teamim
                    rtb.Text = this.Text.Replace("[3]", string.Empty).Replace("[2]", string.Empty).Replace("[1]", string.Empty).Replace("[0]", string.Empty)
                        .RemoveNikud();
                    rtb.Refresh();
                    rtb.SaveFile(tempOnlyTeamimRtf);

                    //only teamim
                    rtb.Text = this.Text.Replace("[3]", string.Empty).Replace("[2]", string.Empty).Replace("[1]", string.Empty).Replace("[0]", string.Empty)
                        .RemoveNikud().RemoveTeamim();
                    rtb.Refresh();
                    rtb.SaveFile(tempClearTextRtf);
                });

            t0.Wait();

            //get the full location of the assembly with DaoTests in it
            string rtf2html_exe = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Rtf2Html", "rtf2html.exe");

            log("מייצא טקסט מקורי...");

            var t1 = Task.Factory.StartNew(() =>
            {
                System.Diagnostics.ProcessStartInfo startInfo = new ProcessStartInfo(rtf2html_exe);
                startInfo.Arguments = string.Format("\"{0}\" \"{1}\" /IDF", tempRtf, tempHtmlFolder);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = System.Diagnostics.Process.Start(startInfo);
                while (!p.HasExited) ;
            });

            t1.Wait();

            log("מייצא טקסט ללא טעמים...");

            var t2 = Task.Factory.StartNew(() =>
            {
                System.Diagnostics.ProcessStartInfo startInfo = new ProcessStartInfo(rtf2html_exe);
                startInfo.Arguments = string.Format("\"{0}\" \"{1}\" /IDF", tempOnlyNikudRtf, tempHtmlFolder);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = System.Diagnostics.Process.Start(startInfo);
                while (!p.HasExited) ;
            });

            t2.Wait();

            log("מייצא טקסט ללא ניקוד...");

            var t3 = Task.Factory.StartNew(() =>
            {
                System.Diagnostics.ProcessStartInfo startInfo = new ProcessStartInfo(rtf2html_exe);
                startInfo.Arguments = string.Format("\"{0}\" \"{1}\" /IDF", tempOnlyTeamimRtf, tempHtmlFolder);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = System.Diagnostics.Process.Start(startInfo);
                while (!p.HasExited) ;
            });

            t3.Wait();

            log("מייצא טקסט נקי...");

            var t4 = Task.Factory.StartNew(() =>
            {
                System.Diagnostics.ProcessStartInfo startInfo = new ProcessStartInfo(rtf2html_exe);
                startInfo.Arguments = string.Format("\"{0}\" \"{1}\" /IDF", tempClearTextRtf, tempHtmlFolder);
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = System.Diagnostics.Process.Start(startInfo);
                while (!p.HasExited) ;
            });

            t4.Wait();

            var htmlFileLocation = Path.Combine(tempPath, string.Format("{0}.html", this.ID.ToString()));
            var htmlOnlyNikudFileLocation = Path.Combine(tempPath, string.Format("{0}_onlyNikud.html", this.ID.ToString()));
            var htmlOnlyTeamimFileLocation = Path.Combine(tempPath, string.Format("{0}_onlyTeamim.html", this.ID.ToString()));
            var htmlClearTextFileLocation = Path.Combine(tempPath, string.Format("{0}_clearText.html", this.ID.ToString()));

            log("אורז קבצים...");
            
            var t5 = Task.Factory.StartNew(() =>
                        {

                            FixHtmlAttributes(this.HtmlFileName);
                            File.Copy(this.HtmlFileName, htmlFileLocation, true);
                            this.HtmlFileName = htmlFileLocation;

                            FixHtmlAttributes(this.HtmlOnlyNikudFileName);
                            File.Copy(this.HtmlOnlyNikudFileName, htmlOnlyNikudFileLocation, true);
                            this.HtmlOnlyNikudFileName = htmlOnlyNikudFileLocation;

                            FixHtmlAttributes(this.HtmlOnlyTeamimFileName);
                            File.Copy(this.HtmlOnlyTeamimFileName, htmlOnlyTeamimFileLocation, true);
                            this.HtmlOnlyTeamimFileName = htmlOnlyTeamimFileLocation;

                            FixHtmlAttributes(this.HtmlClearTextFileName);
                            File.Copy(this.HtmlClearTextFileName, htmlClearTextFileLocation, true);
                            this.HtmlClearTextFileName = htmlClearTextFileLocation;
                        });
            t5.Wait();

            return true;
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
            this.IsNikudIncluded = this.Text.IsNikudExists();
            this.IsTeamimIncluded = this.Text.IsTeamimExists();

            Devide(true, true);
            Devide(true, false);
            Devide(false, true);
            Devide(false, false);

            jsonClip clip = new jsonClip();
            clip.id = this.ID.ToString();
            clip.name = this.Title;
            clip.description = this.Description;
            clip.remarks = this.Remarks;
            clip.clipVersion = this.Version;
            clip.chapter = this.Chapter;
            clip.isNikudIncluded = this.IsNikudIncluded;
            clip.isTeamimIncluded = this.IsTeamimIncluded;
            clip.onlyNikudChapter = this.OnlyNikudChapter;
            clip.onlyTeamimChapter = this.OnlyTeamimChapter;
            clip.clearTextChapter = this.ClearTextChapter;
            clip.schemaVersion = "2.03";
            //clip.duration = this.Duration;
            clip.defaultSections = this.DefaultSections;
            clip.lockedSections = this.LockedSections;
            clip.defaultLearningOptions = this.DefaultLearningOptions;
            clip.lockedLearningOptions = this.LockedLearningOptions;
            clip.category1 = this.Category1;
            clip.category2 = this.Category2;
            clip.category3 = this.Category3;
            clip.category4 = this.Category4;
            clip.keywords = this.Keywords;

            //clip.paragraphs = this.Paragraphs;
            return JsonConvert.SerializeObject(clip, Formatting.Indented);
        }

    }
}
