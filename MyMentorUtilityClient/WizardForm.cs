using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AudioSoundEditor;

namespace MyMentorUtilityClient
{
    public partial class WizardForm : Form
    {
        private Chapter m_chapter = null;
        private string m_initClip = string.Empty;
        public static Regex m_regexAll = new Regex(@"(\(\()|(\)\))|(\[\[)|(\]\])|({{)|(}})|(<<)|(>>)", RegexOptions.Compiled | RegexOptions.Multiline);

        private static Regex m_regexRemoveWhiteSpacesParagraphs = new Regex(@"(?<=\{\{)[ ]{1,}|[ ]{1,}(?=\}\})", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexRemoveWhiteSpacesSentences = new Regex(@"(?<=\(\()[ ]{1,}|[ ]{1,}(?=\)\))", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexRemoveWhiteSpacesSections = new Regex(@"(?<=\<\<)[ ]{1,}|[ ]{1,}(?=\>\>)", RegexOptions.Compiled | RegexOptions.Singleline);

        private static Regex m_regexParagraphs = new Regex(@"(.+?)(\[3\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexSentenses = new Regex(@"(.+?)(\[2\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexSections = new Regex(@"(.+?)(\[1\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexWords = new Regex(@"(.+?)(\[0\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);

        private Graphics rtbGraphics;

        private int sizeIndex = 3;
        private int[] sizes = { 9, 10, 12, 16, 32 };

        public WizardForm(string[] args)
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            PaintGraphics();
        }

        private void tbrParagraph_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionStart == 0)
            {
                return;
            }

            richTextBox2.Rtf = richTextBox1.Rtf;

            string remember = richTextBox1.Text;
            var selectionIndex = richTextBox1.SelectionStart;

            while (selectionIndex > 0 &&
                remember.Substring(selectionIndex, 1) != " " &&
                remember.Substring(selectionIndex, 1) != ":" &&
                remember.Substring(selectionIndex, 1) != "\n")
            {
                selectionIndex--;
            }

            //check another or same anchor
            richTextBox2.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
            richTextBox2.SelectionLength = 3;

            if (richTextBox2.SelectedText == "[3]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "";
                return;
            }
            else if (richTextBox2.SelectedText == "[2]" || richTextBox2.SelectedText == "[1]" || richTextBox2.SelectedText == "[0]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "[3]";
            }
            else
            {
                richTextBox1.SelectionStart = selectionIndex;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectedText = "[3]";
            }

            PaintGraphics();

        }

        private void WizardForm_Paint(object sender, PaintEventArgs e)
        {
            PaintGraphics();
        }

        private void PaintGraphics()
        {
            richTextBox1.Refresh();
            richTextBox2.Rtf = richTextBox1.Rtf;

            int index = richTextBox2.Find("[3]", 0, RichTextBoxFinds.None);

            while (index >= 0)
            {
                PaintAnchor(AnchorType.Paragraph, index);
                index = richTextBox2.Find("[3]", index + 3, RichTextBoxFinds.None);

                if (index + 3 >= richTextBox2.TextLength)
                {
                    break;
                }
            }

            index = richTextBox2.Find("[2]", 0, RichTextBoxFinds.None);

            while (index >= 0)
            {
                PaintAnchor(AnchorType.Sentence, index);
                index = richTextBox2.Find("[2]", index + 3, RichTextBoxFinds.None);

                if (index + 3 >= richTextBox2.TextLength)
                {
                    break;
                }
            }

            index = richTextBox2.Find("[1]", 0, RichTextBoxFinds.None);

            while (index >= 0)
            {
                PaintAnchor(AnchorType.Section, index);
                index = richTextBox2.Find("[1]", index + 3, RichTextBoxFinds.None);

                if (index + 3 >= richTextBox2.TextLength)
                {
                    break;
                }
            }

            index = richTextBox2.Find("[0]", 0, RichTextBoxFinds.None);

            while (index >= 0)
            {
                PaintAnchor(AnchorType.Word, index);
                index = richTextBox2.Find("[0]", index + 3, RichTextBoxFinds.None);

                if (index + 3 >= richTextBox2.TextLength)
                {
                    break;
                }
            }
        }

        private void PaintAnchor(AnchorType type, int index)
        {
            Color c;
            Pen cp = new Pen(Color.Beige);
            string number = "";

            switch (type)
            {
                case AnchorType.Paragraph:
                    c = Color.Red;
                    number = "[3]";
                    break;
                case AnchorType.Sentence:
                    c = Color.Violet;
                    number = "[2]";
                    break;
                case AnchorType.Section:
                    c = Color.LimeGreen;
                    number = "[1]";
                    break;
                case AnchorType.Word:
                    c = Color.Yellow;
                    number = "[0]";
                    break;
                default:
                    c = Color.Black;
                    number = "";
                    break;
            }

            SolidBrush cb = new SolidBrush(c);
            int factor1 = 0;
            int factor2 = 0;
            int factor3 = 0;

            var position = richTextBox1.GetPositionFromCharIndex(index);
            richTextBox2.SelectionStart = index;// -3;
            richTextBox2.SelectionLength = 3;

            int width = Convert.ToInt32(rtbGraphics.MeasureString(number, richTextBox2.SelectionFont).Width);

            if (richTextBox2.SelectionFont.Size >= 32)
            {
                factor1 = 25;
                factor2 = -12;
                factor3 = 20;
            }
            else if (richTextBox2.SelectionFont.Size >= 16 && richTextBox2.SelectionFont.Size < 32)
            {
                factor1 = 13;
                factor2 = -7;
                factor3 = 10;
            }
            else if (richTextBox2.SelectionFont.Size < 16)
            {
                factor1 = 11;
                factor2 = -6;
                factor3 = 8;
            }

            // rectangle to specify which region to paint too
            Rectangle r1 = new Rectangle();

            // specify dimensions
            r1.X = position.X + factor1 - width;
            r1.Y = position.Y;
            r1.Width = width + factor2;
            r1.Height = Convert.ToInt32(richTextBox2.SelectionFont.Height * richTextBox2.ZoomFactor);

            rtbGraphics.DrawRectangle(cp, r1);
            rtbGraphics.FillRectangle(cb, r1);

            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(type == AnchorType.Word ? System.Drawing.Color.Black : System.Drawing.Color.White);
            float x = position.X + factor3 - width;
            float y = position.Y;
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
            rtbGraphics.DrawString(number, richTextBox2.SelectionFont, drawBrush, x, y, drawFormat);

        }


        private void WizardForm_Load(object sender, EventArgs e)
        {
            rtbGraphics = richTextBox1.CreateGraphics();
        }

       

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            PaintGraphics();
        }

        private void tbrSentense_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionStart == 0)
            {
                return;
            }

            string remember = richTextBox1.Text;
            var selectionIndex = richTextBox1.SelectionStart;

            while (selectionIndex > 0 &&
                remember.Substring(selectionIndex, 1) != " " &&
                remember.Substring(selectionIndex, 1) != ":" &&
                remember.Substring(selectionIndex, 1) != "\n")
            {
                selectionIndex--;
            }

            //check another or same anchor
            richTextBox2.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
            richTextBox2.SelectionLength = 3;

            if (richTextBox2.SelectedText == "[2]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "";
                return;
            }
            else if (richTextBox2.SelectedText == "[3]" || richTextBox2.SelectedText == "[1]" || richTextBox2.SelectedText == "[0]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "[2]";
            }
            else
            {
                richTextBox1.SelectionStart = selectionIndex;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectedText = "[2]";
            }

            PaintGraphics();

        }

        private void tbrSection_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionStart == 0)
            {
                return;
            }

            string remember = richTextBox1.Text;
            var selectionIndex = richTextBox1.SelectionStart;

            while (selectionIndex > 0 &&
                remember.Substring(selectionIndex, 1) != " " &&
                remember.Substring(selectionIndex, 1) != ":" &&
                remember.Substring(selectionIndex, 1) != "\n")
            {
                selectionIndex--;
            }

            //check another or same anchor
            richTextBox2.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
            richTextBox2.SelectionLength = 3;

            if (richTextBox2.SelectedText == "[1]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "";
                return;
            }
            else if (richTextBox2.SelectedText == "[3]" || richTextBox2.SelectedText == "[2]" || richTextBox2.SelectedText == "[0]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "[1]";
            }
            else
            {
                richTextBox1.SelectionStart = selectionIndex;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectedText = "[1]";
            }

            PaintGraphics();

        }

        private void tbrBiggerFont_Click(object sender, EventArgs e)
        {
            try
            {
                if (sizeIndex >= 0 && sizeIndex < sizes.Length)
                {
                    sizeIndex++;
                    richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, sizes[sizeIndex]);
                }
            }
            catch
            {

            }
        }

        private void tbrSmallerFont_Click(object sender, EventArgs e)
        {
            try
            {
                if (sizeIndex > 0)
                {
                    sizeIndex--;
                    richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, sizes[sizeIndex]);
                }
            }
            catch
            {

            }
        }

        private void tbrWord_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionStart == 0)
            {
                return;
            }

            string remember = richTextBox1.Text;
            var selectionIndex = richTextBox1.SelectionStart;

            while (selectionIndex > 0 &&
                remember.Substring(selectionIndex, 1) != " " &&
                remember.Substring(selectionIndex, 1) != ":" &&
                remember.Substring(selectionIndex, 1) != "\n")
            {
                selectionIndex--;
            }

            //check another or same anchor
            richTextBox2.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
            richTextBox2.SelectionLength = 3;

            if (richTextBox2.SelectedText == "[0]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "";
                return;
            }
            else if (richTextBox2.SelectedText == "[3]" || richTextBox2.SelectedText == "[2]" || richTextBox2.SelectedText == "[1]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "[0]";
            }
            else
            {
                richTextBox1.SelectionStart = selectionIndex;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectedText = "[0]";
            }

            PaintGraphics();
        }

        private void mnuDebugDetails_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsNew)
            {
                MessageBox.Show("Please save clip first");
                return;
            }

            DevideText();

            //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
            //Clip.Current.FontName = richTextBox1.Font.Name;
            Clip.Current.Chapter = m_chapter;
            Clip.Current.Save();

            JsonDebugFrm frm = new JsonDebugFrm();
            frm.ShowDialog();

        }

        private void mnuSaveClip_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            Save(false);
        }

        private void Save(bool isSaveAs)
        {
            if (Clip.Current.IsNew || (!Clip.Current.IsNew && isSaveAs))
            {
                if (isSaveAs)
                {
                    FileInfo fi = new FileInfo(Clip.Current.FileName);
                    saveFileDialog1.InitialDirectory = fi.DirectoryName;
                    saveFileDialog1.FileName = fi.Name;//  .FullName;
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyMentor Clips"));

                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    saveFileDialog1.InitialDirectory = di.FullName;
                    saveFileDialog1.FileName = Clip.Current.Title.ToValidFileName();
                }
                saveFileDialog1.DefaultExt = "mmnx";
                saveFileDialog1.Filter = "MyMentor Source Files|*.mmnx";

                DialogResult result = saveFileDialog1.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    if (isSaveAs)
                    {
                        Clip.Current.ID = Guid.NewGuid();
                    }

                    //Clip.Current.FontName = richTextBox1.Font.Name;
                    //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
                    Clip.Current.FileName = saveFileDialog1.FileName;
                    Clip.Current.RtfText = richTextBox1.Rtf;
                    this.Text = "MyMentor - " + Clip.Current.FileName;
                    Clip.Current.Chapter = m_chapter;
                    Clip.Current.Save();

                    MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    mnuSaveClipAs.Enabled = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Clip.Current.RtfText = richTextBox1.Rtf;
                Clip.Current.Chapter = m_chapter;
                //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
                //Clip.Current.FontName = richTextBox1.Font.Name;
                Clip.Current.Save();

                MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }

        private void tbrSave_Click(object sender, EventArgs e)
        {
            Save();
        }


        /// <summary>
        /// 
        /// </summary>
        private void DevideText()
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

                IEnumerable<Word> allWords = null;

                if (m_chapter != null && m_chapter.Paragraphs != null)
                {
                    allWords = m_chapter.Paragraphs.FlattenWords();
                }


                int bufferIndex = 0;

                List<SectionMatch> matchesParagraphs = m_regexParagraphs.Matches(richTextBox1.Text).Cast<Match>()
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
                        Length = richTextBox1.Text.Length,
                        Value = richTextBox1.Text
                    });
                }

                foreach (SectionMatch matchParagraph in matchesParagraphs)
                {
                    paragraphIndex++;
                    innerSentenceIndex = -1;

                    TimeSpan start = TimeSpan.Zero;
                    TimeSpan duration = TimeSpan.Zero;
                    bool startManually = false;
                    bool durationManually = false;
                    Paragraph ex_paragraph = null;

                    //check for saved schedule
                    if (m_chapter != null && m_chapter.Paragraphs != null)
                    {
                        ex_paragraph = m_chapter.Paragraphs.Where(p => p.Index == paragraphIndex).FirstOrDefault();
                    }

                    if (ex_paragraph != null)
                    {
                        start = ex_paragraph.StartTime;
                        duration = ex_paragraph.Duration;
                        startManually = ex_paragraph.ManuallyStartDate;
                        durationManually = ex_paragraph.ManuallyDuration;
                    }

                    paragraphs_local.Add(new Paragraph
                    {
                        Content = matchParagraph.StrippedValue,
                        RealCharIndex = matchParagraph.CharIndex,
                        CharIndex = matchParagraph.CharIndex - bufferIndex,
                        ManuallyStartDate = startManually,
                        ManuallyDuration = durationManually,
                        Index = paragraphIndex,
                        Sentences = new List<Sentence>(),
                        Words = new List<Word>(),
                        StartTime = start,
                        Duration = duration,
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
                        startManually = false;
                        durationManually = false;
                        Sentence ex_sentence = null;

                        sentenceIndex++;
                        innerSentenceIndex++;
                        innerSectionIndex = -1;

                        //check for saved schedule
                        if (m_chapter != null && m_chapter.Paragraphs != null)
                        {
                            ex_sentence = m_chapter.Paragraphs.SelectMany(s => s.Sentences).Where(p => p.Index == sentenceIndex).FirstOrDefault();
                        }

                        if (ex_sentence != null)
                        {
                            start = ex_sentence.StartTime;
                            duration = ex_sentence.Duration;
                            startManually = ex_sentence.ManuallyStartDate;
                            durationManually = ex_sentence.ManuallyDuration;
                        }

                        int sectionsOffset = 0;
                        int wordsOffset = 0;

                        if (innerSentenceIndex > 0)
                        {
                            sectionsOffset = Math.Max(0, paragraphs_local[paragraphIndex].Sentences.Take(innerSentenceIndex).SelectMany(p => p.Sections).Count() * 3 - 3);
                            wordsOffset = Math.Max(0, paragraphs_local[paragraphIndex].Sentences.Take(innerSentenceIndex).SelectMany(p => p.Sections).SelectMany(w => w.Words).Where(w => w.IsInGroup).Count() * 3 - 3);
                        }

                        paragraphs_local[paragraphIndex].Sentences.Add(new Sentence
                        {   //                     5                               +           15    - 4   - (4 * 1) - 2
                            Content = matchSentense.StrippedValue,
                            RealCharIndex = paragraphs_local[paragraphIndex].RealCharIndex + matchSentense.CharIndex,
                            CharIndex = paragraphs_local[paragraphIndex].CharIndex + matchSentense.CharIndex - sectionsOffset - wordsOffset - Math.Max(0, 3 * innerSentenceIndex),
                            Index = sentenceIndex,
                            ManuallyDuration = durationManually,
                            ManuallyStartDate = startManually,
                            Sections = new List<Section>(),
                            Words = new List<Word>(),
                            StartTime = start,
                            Duration = duration
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
                            startManually = false;
                            durationManually = false;
                            Section ex_section = null;

                            //check for saved schedule
                            if (m_chapter != null && m_chapter.Paragraphs != null)
                            {
                                ex_section = m_chapter.Paragraphs.SelectMany(s => s.Sentences).SelectMany(se => se.Sections).Where(p => p.Index == sectionIndex).FirstOrDefault();
                            }

                            if (ex_section != null)
                            {
                                start = ex_section.StartTime;
                                duration = ex_section.Duration;
                                startManually = ex_section.ManuallyStartDate;
                                durationManually = ex_section.ManuallyDuration;
                            }

                            int groupWordsBuffer = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections.SelectMany(s => s.Words).Where(w => w.IsInGroup).Count();

                            paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections.Add(new Section
                            {
                                Content = matchSection.StrippedValue,
                                ManuallyDuration = durationManually,
                                ManuallyStartDate = startManually,
                                RealCharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].RealCharIndex + matchSection.CharIndex,
                                CharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].CharIndex + matchSection.CharIndex - (3 * innerSectionIndex),
                                Index = sectionIndex,
                                Words = new List<Word>(),
                                StartTime = start,
                                Duration = duration
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

                                paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Words.Add(new Word
                                {
                                    RealCharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].RealCharIndex + matchWord.CharIndex,
                                    CharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].CharIndex + matchWord.CharIndex - (3 * Math.Max(0, paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Words.Where(w => w.IsInGroup).Count() - 1)),
                                    IsInGroup = true, //matchWord.Groups["group"].Success,
                                    Index = wordIndex,
                                    Content = matchWord.Value,
                                    StartTime = start,
                                    Duration = duration
                                });

                                bufferIndex += innerWordIndex == 0 ? 0 : 3;
                            }
                        }
                    }
                }

                m_chapter = new Chapter();
                m_chapter.Words = new List<Word>();
                m_chapter.Content = m_regexAll.Replace(richTextBox1.Text, string.Empty);
                m_chapter.Paragraphs = paragraphs_local;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }

        private void tbrParse_Click(object sender, EventArgs e)
        {
            DevideText();
        }

        private void WizardForm_Shown(object sender, EventArgs e)
        {
            richTextBox1.Focus();

            //if (ParseUser.CurrentUser == null)
            //{
            //    LoginForm frmLogin = new LoginForm(this);
            //    frmLogin.ShowDialog();
            //}
            //else
            //{
            //    lblLoginUser.Text = "מחובר כ-" + ParseUser.CurrentUser.Username;
            //}

            if (!string.IsNullOrEmpty(m_initClip))
            {
                OpenClip(m_initClip);
            }
            else
            {
                Clip.Current.Title = "שיעור 1";
                Clip.Current.IsDirty = false;
                Clip.Current.IsNew = true;
                Clip.Current.ID = Guid.NewGuid();
                this.Text = "MyMentor - " + Clip.Current.Title;
            }
        }

        private void OpenClip()
        {
            OpenClip(null);
        }

        private string GetFileDialog()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyMentor Clips"));

            if (!di.Exists)
            {
                di.Create();
            }

            openFileDialog1.InitialDirectory = di.FullName;
            openFileDialog1.DefaultExt = "mmnx";
            openFileDialog1.Filter = "MyMentor Source Files|*.mmnx";
            openFileDialog1.FileName = "";

            DialogResult result = openFileDialog1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
            {
                return null;
            }
        }

        private void OpenClip(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                file = GetFileDialog();
            }

            if (string.IsNullOrEmpty(file))
            {
                return;
            }

            m_chapter = null;

            try
            {
                Clip.Load(file);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("קובץ שיעור אינו תקין", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            this.Text = "MyMentor - " + file;

            richTextBox1.Rtf = Clip.Current.RtfText;

            if (Clip.Current.Chapter != null)
            {
                m_chapter = Clip.Current.Chapter;
            }
            //for old version
            else if (Clip.Current.Paragraphs != null)
            {
                m_chapter = new Chapter();
                m_chapter.Paragraphs = Clip.Current.Paragraphs;
            }

            DevideText();
        }

        private void הפניהלתומספרToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new GotoForm())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    richTextBox1.SelectionStart = form.CharIndex;
                    richTextBox1.SelectionLength = 0;
                }
            }
        }

        private void tbrOpen_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (MessageBox.Show("השיעור לא נשמר מהשינויים האחרונים.\n\nהאם אתה בטוח להמשיך?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    OpenClip();
                    mnuSaveClipAs.Enabled = true;
                }
            }
            else
            {
                OpenClip();
                mnuSaveClipAs.Enabled = true;
            }

        }

        private void mnuQuit_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (MessageBox.Show("השיעור לא נשמר מהשינויים האחרונים.\n\nהאם אתה בטוח לצאת?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }

        }

        private void mnuAbout_Click(object sender, EventArgs e)
        {
            AboutBox frm = new AboutBox();
            frm.ShowDialog();

        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 1)
            {
                e.Cancel = true;
                SoundStudio.FormMain frm = new SoundStudio.FormMain();
                frm.ShowDialog();
            }

        }

        private void tbrNew_Click(object sender, EventArgs e)
        {

        }


    }



    public struct SectionMatch
    {
        private string m_value;

        public int CharIndex { get; set; }
        public int Length { get; set; }
        public AnchorType Type { get; set; }

        public string StrippedValue
        {
            get
            {
                return m_value.Replace("[0]", "").Replace("[1]", "").Replace("[2]", "").Replace("[3]", "");
            }
        }

        public string Value
        {
            get
            {
                return m_value;
            }

            set
            {
                m_value = value;
            }
        }
    }

}
