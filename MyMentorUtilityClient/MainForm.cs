using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using ECN.SchoolSoundSystem;
using Ionic.Zip;
using MyMentorUtilityClient.Properties;
using Newtonsoft.Json;
using Parse;

namespace MyMentorUtilityClient
{
    public partial class MainForm : Form
    {
        private MultiKeyGesture m_ShortcutPiska = new MultiKeyGesture(new List<Keys> { Keys.M, Keys.P }, Keys.Control);
        private MultiKeyGesture m_ShortcutKeta = new MultiKeyGesture(new List<Keys> { Keys.M, Keys.K }, Keys.Control);
        private MultiKeyGesture m_ShortcutMishpat = new MultiKeyGesture(new List<Keys> { Keys.M, Keys.M }, Keys.Control);

        private List<Paragraph> m_paragraphs = null;

        private Word m_selected = null;
        private bool m_disableScanningText;
        private AnchorType m_selectedAnchorType = AnchorType.None;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Recalculate()
        {
            bool blError = false;

            try
            {
                int charIndex = 0;
                int paragraphIndex = -1;
                int sentenceIndex = -1;
                int sectionIndex = -1;
                int wordIndex = -1;

                int innerSentenceIndex = -1;
                int innerSectionIndex = -1;

                richTextBox2.Rtf = richTextBox1.Rtf;
                string word = string.Empty;
                int realCharIndex = 0;
                int lastWordCharIndex = 0;

                var paragraphs_local = new List<Paragraph>();

                TimeSpan nextStartTime = new TimeSpan(0, 0, 0);
                TimeSpan nextParagraphDuration = new TimeSpan(0, 0, 0);
                TimeSpan nextSentenceDuration = new TimeSpan(0, 0, 0);
                TimeSpan nextSectionDuration = new TimeSpan(0, 0, 0);

                bool blOpenMila = false;

                while (charIndex < richTextBox2.Text.Length)
                {
                    richTextBox2.Select(charIndex, 1);

                    //start piska
                    if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Red)
                    {
                        paragraphIndex++;

                        //reset inner counters
                        innerSentenceIndex = -1;
                        innerSectionIndex = -1;

                        TimeSpan start = new TimeSpan(0, 0, 0);
                        TimeSpan duration = new TimeSpan(0, 0, 0);
                        Paragraph ex_paragraph = null;

                        //check for this word
                        if (m_paragraphs != null)
                        {
                            ex_paragraph = m_paragraphs.Where(p => p.Index == paragraphIndex).FirstOrDefault();
                        }

                        if (ex_paragraph != null)
                        {
                            start = ex_paragraph.StartTime;
                            duration = ex_paragraph.Duration;
                        }
                        else
                        {
                            start = nextStartTime;
                        }

                        if (start.Hours == 0 && start.Minutes == 0 && start.Seconds == 0)
                        {
                            start = nextStartTime;
                        }
                        else if (wordIndex >= 1 && start.Add(duration) < nextStartTime)
                        {
                            start = nextStartTime;
                        }


                        paragraphs_local.Add(new Paragraph
                        {
                            CharIndex = realCharIndex,
                            Index = paragraphIndex,
                            Sentences = new List<Sentence>(),
                            StartTime = start,
                            Duration = duration
                        });
                    }

                    //start mishpat
                    else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Blue)
                    {
                        sentenceIndex++;
                        innerSentenceIndex++;

                        //reset inner counters
                        innerSectionIndex = -1;

                        TimeSpan start = new TimeSpan(0, 0, 0);
                        TimeSpan duration = new TimeSpan(0, 0, 0);
                        Sentence ex_sentence = null;

                        //check for this word
                        if (ex_sentence != null)
                        {
                            ex_sentence = m_paragraphs.SelectMany(s => s.Sentences).Where(p => p.Index == sentenceIndex).FirstOrDefault();
                        }

                        if (ex_sentence != null)
                        {
                            start = ex_sentence.StartTime;
                            duration = ex_sentence.Duration;
                        }
                        else
                        {
                            start = nextStartTime;
                        }

                        if (start.Hours == 0 && start.Minutes == 0 && start.Seconds == 0)
                        {
                            start = nextStartTime;
                        }
                        else if (wordIndex >= 1 && start.Add(duration) < nextStartTime)
                        {
                            start = nextStartTime;
                        }

                        paragraphs_local[paragraphIndex].Sentences.Add(new Sentence
                        {
                            CharIndex = realCharIndex,
                            Index = sentenceIndex,
                            Sections = new List<Section>(),
                            StartTime = start,
                            Duration = duration
                        });
                    }

                    //start keta
                    else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Green)
                    {
                        sectionIndex++;
                        innerSectionIndex++;

                        TimeSpan start = new TimeSpan(0, 0, 0);
                        TimeSpan duration = new TimeSpan(0, 0, 0);
                        Section ex_section = null;

                        //check for this word
                        if (ex_section != null)
                        {
                            ex_section = m_paragraphs.SelectMany(s => s.Sentences).SelectMany(se => se.Sections).
                                Where(p => p.Index == sectionIndex).FirstOrDefault();
                        }

                        if (ex_section != null)
                        {
                            start = ex_section.StartTime;
                            duration = ex_section.Duration;
                        }
                        else
                        {
                            start = nextStartTime;
                        }

                        if (start.Hours == 0 && start.Minutes == 0 && start.Seconds == 0)
                        {
                            start = nextStartTime;
                        }
                        else if (wordIndex >= 1 && start.Add(duration) < nextStartTime)
                        {
                            start = nextStartTime;
                        }


                        paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections.Add(new Section
                        {
                            CharIndex = realCharIndex,
                            Index = sectionIndex,
                            Words = new List<Word>(),
                            StartTime = start,
                            Duration = duration
                        });
                    }

                    //start mila
                    else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Black)
                    {
                        blOpenMila = true;
                    }

                    //end mila
                    else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Black)
                    {
                        blOpenMila = false;
                    }

                    // if empty OR start mila
                    else if ((!blOpenMila && richTextBox2.SelectedText == " ") || richTextBox2.SelectedText == "]")
                    {
                        if (richTextBox2.SelectedText != "[" && richTextBox2.SelectedText != "]")
                        {
                            realCharIndex++;
                        }

                        //end piska
                        else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Red)
                        {
                            paragraphs_local[paragraphIndex].Duration = nextParagraphDuration;
                            nextParagraphDuration = new TimeSpan(0, 0, 0);
                        }

                        //end mishpat
                        else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Blue)
                        {
                            paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Duration = nextSentenceDuration;
                            nextSentenceDuration = new TimeSpan(0, 0, 0);
                        }

                        //end keta
                        else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Green)
                        {
                            paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Duration = nextSectionDuration;
                            nextSectionDuration = new TimeSpan(0, 0, 0);
                        }
                    

                        if (!string.IsNullOrEmpty(word.Trim()))
                        {
                            wordIndex++;

                            Word ex_word = null;

                            TimeSpan start = new TimeSpan(0, 0, 0);
                            TimeSpan duration = new TimeSpan(0, 0, 0);

                            //check for this word
                            if (m_paragraphs != null)
                            {
                                ex_word = m_paragraphs.SelectMany(s => s.Sentences).SelectMany(sc => sc.Sections)
                                    .SelectMany(w => w.Words).Where(w => w.Index == wordIndex).FirstOrDefault();
                            }

                            if (ex_word != null)
                            {
                                start = ex_word.StartTime;
                                duration = ex_word.Duration;
                            }
                            else
                            {
                                start = nextStartTime;
                            }

                            if (start.Hours == 0 && start.Minutes == 0 && start.Seconds == 0)
                            {
                                start = nextStartTime;
                            }
                            else if (wordIndex >= 1 && start.Add(duration) < nextStartTime)
                            {
                                start = nextStartTime;
                            }

                            //add words
                            paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Words.Add(new Word
                            {
                                CharIndex = lastWordCharIndex,
                                Index = wordIndex,
                                Text = word,
                                StartTime = start,
                                Duration = duration
                            });

                            nextStartTime = start.Add(duration);

                            nextParagraphDuration = nextParagraphDuration.Add(duration);
                            nextSentenceDuration = nextSentenceDuration.Add(duration);
                            nextSectionDuration = nextSectionDuration.Add(duration);

                            word = string.Empty;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(word))
                        {
                            lastWordCharIndex = realCharIndex;
                        }

                        if (richTextBox2.SelectedText != "\n")
                        {
                            word = word + richTextBox2.SelectedText;
                            realCharIndex++;
                        }
                    }

                    charIndex++;
                }

                m_paragraphs = paragraphs_local;

                paragraphsGrid.DataSource = m_paragraphs.Select(p => new SectionCellData
                {
                    Index = p.Index,
                    StartTime = p.StartTime,
                    Duration = p.Duration,
                    Text = p.Sentences.SelectMany(s => s.Sections).SelectMany(w => w.Words).Select( w => w.Text).Aggregate((a, b) => a + " " + b)
                }).ToList();

                sentencesGrid.DataSource = m_paragraphs.SelectMany(p => p.Sentences).Select(s => new SectionCellData
                {
                    Index = s.Index,
                    StartTime = s.StartTime,
                    Duration = s.Duration,
                    Text = s.Sections.SelectMany(w => w.Words).Select( w => w.Text).Aggregate((a, b) => a + " " + b)
                }).ToList();


                sectionsGrid.DataSource = m_paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).Select(s => new SectionCellData
                {
                    Index = s.Index,
                    StartTime = s.StartTime,
                    Duration = s.Duration,
                    Text = s.Words.Select( w => w.Text).Aggregate((a, b) => a + " " + b)
                }).ToList();

            }
            catch(ApplicationException ex)
            {
                LogTextBox.AppendText(Environment.NewLine + DateTime.Now.ToLongTimeString() + " : " + ex.Message);
                blError = true;
            }
            catch(Exception ex)
            {
                LogTextBox.AppendText(Environment.NewLine + DateTime.Now.ToLongTimeString() + " : העוגנים בטקסט אינם חוקים");
                blError = true;
            }

            if (!blError)
            {
                LogTextBox.AppendText(Environment.NewLine + DateTime.Now.ToLongTimeString() + " : לא נמצאו שגיאות תקינות בטקסט השיעור");
            }
        }

        private void FixGridLayout()
        {
            //wordsGridView.Columns[0].Width = 70;
            //wordsGridView.Columns[1].Width = 70;
            //wordsGridView.Columns[2].Width = 70;
            //wordsGridView.Columns[3].Width = 70;
            //wordsGridView.Columns[4].Width = 70;
            //wordsGridView.Columns[5].Width = 70;
            //wordsGridView.Columns[8].Visible = false;
            //wordsGridView.Columns[9].Visible = false;

            //DataGridViewColumn timeColumn = wordsGridView.Columns["StartTime"];
            //timeColumn.DefaultCellStyle.FormatProvider = new TimeSpanFormatter();
            //timeColumn.DefaultCellStyle.Format = "hh:mm:ss.fff";

            //DataGridViewColumn durationColumn = wordsGridView.Columns["Duration"];
            //durationColumn.DefaultCellStyle.FormatProvider = new TimeSpanFormatter();
            //durationColumn.DefaultCellStyle.Format = "hh:mm:ss.fff";


        }

        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            var testObject = new ParseObject("TestObject");
            testObject["foo"] = "bar";
            await testObject.SaveAsync();
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            byte[] bytes = File.ReadAllBytes(@"c:\test.zip");
            ParseFile file = new ParseFile("test.zip", bytes);
            await file.SaveAsync();

            var jobApplication = new ParseObject("Clips");
            jobApplication["applicantName"] = "Joe Smith";
            jobApplication["applicantResumeFile"] = file;
            await jobApplication.SaveAsync();
        }

        public class MultiKeyGesture
        {
            private List<Keys> _keys;
            public MultiKeyGesture(IEnumerable<Keys> keys, Keys modifiers)
            {
                _keys = new List<Keys>(keys);

                if (_keys.Count == 0)
                {
                    throw new ArgumentException("At least one key must be specified.", "keys");
                }
            }

            private int currentindex;
            public bool Matches(KeyEventArgs e)
            {
                if (_keys[currentindex] == e.KeyCode)
                    //at least a partial match
                    currentindex++;
                else
                    //No Match
                    currentindex = 0;
                if (currentindex + 1 > _keys.Count)
                {
                    //Matched last key
                    currentindex = 0;
                    return true;
                }
                return false;
            }
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
            try
            {
                if (m_ShortcutKeta.Matches(e))
                {
                    if (GetNextDirection(AnchorType.Section) == AnchorDirection.Open)
                    {
                        richTextBox1.AppendText("[");
                    }
                    else
                    {
                        richTextBox1.AppendText("]");

                    }
                    richTextBox1.Select(richTextBox1.TextLength - 1, 1);
                    richTextBox1.SelectionColor = Color.Green;
                    richTextBox1.Select(richTextBox1.TextLength, 0);
                    richTextBox1.SelectionColor = Color.Black;
                }
                if (m_ShortcutMishpat.Matches(e))
                {
                    if (GetNextDirection(AnchorType.Sentence) == AnchorDirection.Open)
                    {
                        richTextBox1.AppendText("[");
                    }
                    else
                    {
                        richTextBox1.AppendText("]");

                    }
                    richTextBox1.Select(richTextBox1.TextLength - 1, 1);
                    richTextBox1.SelectionColor = Color.Blue;
                    richTextBox1.Select(richTextBox1.TextLength, 0);
                    richTextBox1.SelectionColor = Color.Black;
                }
                if (m_ShortcutPiska.Matches(e))
                {
                    if (GetNextDirection(AnchorType.Paragraph) == AnchorDirection.Open)
                    {
                        richTextBox1.AppendText("[");
                    }
                    else
                    {
                        richTextBox1.AppendText("]");

                    }
                    richTextBox1.Select(richTextBox1.TextLength - 1, 1);
                    richTextBox1.SelectionColor = Color.Red;
                    richTextBox1.Select(richTextBox1.TextLength, 0);
                    richTextBox1.SelectionColor = Color.Black;
                }
            }
            catch(ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!m_disableScanningText)
            {
                Clip.Current.IsDirty = true;
            }

            if (!m_disableScanningText && cbScanText.Checked)
            {
                Recalculate();
            }
        }

        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            //e.
        }

        private void wordsGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var formatter = e.CellStyle.FormatProvider as ICustomFormatter;
            if (formatter != null)
            {
                e.Value = formatter.Format(e.CellStyle.Format, e.Value, e.CellStyle.FormatProvider);
                e.FormattingApplied = true;
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void timePicker1_OnValueChanged(object sender, EventArgs e)
        {
            if (m_selected != null)
            {
                m_selected.StartTime = ((TimePicker)sender).Value;
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            int selectedIndex = richTextBox1.SelectionStart;

            if (m_selected != null)
            {
                m_selected.StartTime = startTimer.Value;
                m_selected.Duration = durationTimer.Value;

                Recalculate();
                //richTextBox1.SelectionStart = selectedIndex;
                //richTextBox1.SelectionLength = 0;
                richTextBox1.Focus();

                richTextBox1_SelectionChanged(null, new EventArgs());
            }

        }

        private AnchorDirection GetNextDirection(AnchorType anchorType)
        {
            int piskaot = 0;
            int mishpatim = 0;
            int ktaim = 0;
            int milim = 0;

            int charIndex = 0;
            int remember = richTextBox1.SelectionStart;

            richTextBox1.Select(0, remember);

            richTextBox2.Rtf = richTextBox1.SelectedRtf;

            richTextBox1.SelectionStart = remember;
            richTextBox1.SelectionLength = 0;

            while (charIndex < richTextBox2.Text.Length)
            {
                richTextBox2.Select(charIndex, 1);

                //start piska
                if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Red)
                {
                    piskaot++;
                }

                //close piska
                else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Red)
                {
                    piskaot--;
                }

                //start mishpat
                else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Blue)
                {
                    mishpatim++;
                }

                //close mishpat
                else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Blue)
                {
                    mishpatim--;
                }

                //start keta
                else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Green)
                {
                    ktaim++;
                }

                //close keta
                else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Green)
                {
                    ktaim--;
                }

                //start mila
                else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Black)
                {
                    milim++;
                }

                //close mila
                else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Black)
                {
                    milim--;
                }

                charIndex++;
            }

            switch (anchorType)
            {
                case AnchorType.Word:

                    if (piskaot == 0 || mishpatim == 0 || ktaim == 0)
                    {
                        throw new ApplicationException("בחירה לא חוקית");
                    }
                    else
                    {
                        if (milim == 0)
                        {
                            return AnchorDirection.Open;
                        }
                        else
                        {
                            return AnchorDirection.Close;
                        }
                    }

                case AnchorType.Section:

                    if (piskaot == 0 || mishpatim == 0)
                    {
                        throw new ApplicationException("בחירה לא חוקית");
                    }
                    else
                    {
                        if (ktaim == 0)
                        {
                            return AnchorDirection.Open;
                        }
                        else
                        {
                            return AnchorDirection.Close;
                        }
                    }

                case AnchorType.Sentence:

                    if (piskaot == 0 || ktaim > 0)
                    {
                        throw new ApplicationException("בחירה לא חוקית");
                    }
                    else
                    {
                        if (mishpatim == 0)
                        {
                            return AnchorDirection.Open;
                        }
                        else
                        {
                            return AnchorDirection.Close;
                        }
                    }

                case AnchorType.Paragraph:

                    if (ktaim > 0 || mishpatim > 0)
                    {
                        throw new ApplicationException("בחירה לא חוקית");
                    }
                    else
                    {
                        if (piskaot == 0)
                        {
                            return AnchorDirection.Open;
                        }
                        else
                        {
                            return AnchorDirection.Close;
                        }
                    }

                default:

                    throw new ApplicationException("WTF");


            }

        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            richTextBox1.Focus();

            if (ParseUser.CurrentUser == null)
            {
                LoginForm frmLogin = new LoginForm(this);
                frmLogin.ShowDialog();
            }
            else
            {
                lblLoginUser.Text = "מחובר כ-" + ParseUser.CurrentUser.Username;
            }

            if (ParseUser.CurrentUser == null)
            {
                Application.Exit();
            }
            else
            {
                ClipDetails frm = new ClipDetails();
                frm.ShowDialog();

                if (string.IsNullOrEmpty(Clip.Current.Directory))
                {
                    Application.Exit();
                }
                else
                {
                    this.Text = "MyMentor - " + Clip.Current.Title;

                    m_disableScanningText = true;
                    richTextBox1.Rtf = Clip.Current.RtfText;
                    m_disableScanningText = false;

                    if (Clip.Current.Paragraphs != null && Clip.Current.Paragraphs.Count() > 0)
                    {
                        m_paragraphs = Clip.Current.Paragraphs;
                        Recalculate();
                    }
                }
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetNextDirection(AnchorType.Section) == AnchorDirection.Open)
                {
                    richTextBox2.Text = "[";
                }
                else
                {
                    richTextBox2.Text = "]";
                }

                richTextBox2.Select(0, 1);
                richTextBox2.SelectionColor = Color.Green;

                var selectionIndex = richTextBox1.SelectionStart;
                richTextBox1.SelectedRtf = richTextBox2.SelectedRtf;
                richTextBox1.Select(selectionIndex + 1, 0);
                richTextBox1.SelectionColor = Color.Black;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetNextDirection(AnchorType.Sentence) == AnchorDirection.Open)
                {
                    richTextBox2.Text = "[";
                }
                else
                {
                    richTextBox2.Text = "]";
                }

                richTextBox2.Select(0, 1);
                richTextBox2.SelectionColor = Color.Blue;

                var selectionIndex = richTextBox1.SelectionStart;
                richTextBox1.SelectedRtf = richTextBox2.SelectedRtf;
                richTextBox1.Select(selectionIndex + 1, 0);
                richTextBox1.SelectionColor = Color.Black;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetNextDirection(AnchorType.Paragraph) == AnchorDirection.Open)
                {
                    richTextBox2.Text = "[";
                }
                else
                {
                    richTextBox2.Text = "]";
                }

                richTextBox2.Select(0, 1);
                richTextBox2.SelectionColor = Color.Red;

                var selectionIndex = richTextBox1.SelectionStart;
                richTextBox1.SelectedRtf = richTextBox2.SelectedRtf;
                richTextBox1.Select(selectionIndex + 1, 0);
                richTextBox1.SelectionColor = Color.Black;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            try
            {
                if (GetNextDirection(AnchorType.Word) == AnchorDirection.Open)
                {
                    richTextBox2.Text = "[";
                }
                else
                {
                    richTextBox2.Text = "]";
                }

                richTextBox2.Select(0, 1);
                richTextBox2.SelectionColor = Color.Black;

                var selectionIndex = richTextBox1.SelectionStart;
                richTextBox1.SelectedRtf = richTextBox2.SelectedRtf;
                richTextBox1.Select(selectionIndex + 1, 0);
                richTextBox1.SelectionColor = Color.Black;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Clip.Current.Paragraphs = m_paragraphs;
            Clip.Current.RtfText = richTextBox1.Rtf;
            Clip.Current.Save();

            PublishForm frm = new PublishForm();
            frm.ShowDialog();
        }

        private void richTextBox1_CursorChanged(object sender, EventArgs e)
        {
            
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                int selection = richTextBox1.SelectionStart;

                if (selection > 0)
                {
                    char[] chars = richTextBox1.Text.ToCharArray(0, selection);

                    //minus special characters
                    int minus = chars.Count(c => c == ']' || c == '[' || (int)c == 10);

                    int i = selection - 1;

                    while (chars[i] != ' ' && chars[i] != ']' && chars[i] != '[' && i >= 0)
                    {
                        i--;
                    }

                    Word word = m_paragraphs.SelectMany(s => s.Sentences).SelectMany(sc => sc.Sections)
                            .SelectMany(w => w.Words).Where(w => w.CharIndex == (i + 1 - minus)).FirstOrDefault();

                    if (word != null)
                    {
                        m_selected = word;

                        tbSectionText.Text = word.Text;
                        startTimer.Value = m_selected.StartTime;
                        durationTimer.Value = m_selected.Duration;
                        button4.Enabled = true;

                        m_selectedAnchorType = AnchorType.Word;
                        sectionGroup.Text = "תזמון מילה";
                        lblSectionText.Text = "מילה";

                    }
                }
            }
            catch
            {

            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            Debug.WriteLine((int)richTextBox1.SelectedText.ToCharArray()[0]);
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void paragraphsGrid_SelectionChanged(object sender, EventArgs e)
        {
        }

        private void paragraphsGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (paragraphsGrid.SelectedRows.Count > 0)
            {
                var row = paragraphsGrid.SelectedRows[0].DataBoundItem as SectionCellData;

                if (row != null)
                {
                    m_selectedAnchorType = AnchorType.Paragraph;
                    sectionGroup.Text = "תזמון פסקה";
                    lblSectionText.Text = "פסקה";
                    tbSectionText.Text = row.Text;
                    startTimer.Value = row.StartTime;
                    durationTimer.Value = row.Duration;
                    button4.Enabled = true;
                }
            }

        }

        private void sentencesGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sentencesGrid.SelectedRows.Count > 0)
            {
                var row = sentencesGrid.SelectedRows[0].DataBoundItem as SectionCellData;

                if (row != null)
                {
                    m_selectedAnchorType = AnchorType.Sentence;
                    sectionGroup.Text = "תזמון משפט";
                    lblSectionText.Text = "משפט";
                    tbSectionText.Text = row.Text;
                    startTimer.Value = row.StartTime;
                    durationTimer.Value = row.Duration;
                    button4.Enabled = true;
                }
            }
        }

        private void sectionsGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sectionsGrid.SelectedRows.Count > 0)
            {
                var row = sectionsGrid.SelectedRows[0].DataBoundItem as SectionCellData;

                if (row != null)
                {
                    m_selectedAnchorType = AnchorType.Section;
                    sectionGroup.Text = "תזמון קטע";
                    lblSectionText.Text = "קטע";
                    tbSectionText.Text = row.Text;
                    startTimer.Value = row.StartTime;
                    durationTimer.Value = row.Duration;
                    button4.Enabled = true;
                }
            }
        }

        private void menuConnectAsDifferentUser_Click(object sender, EventArgs e)
        {
            //ParseUser.LogOut();

            LoginForm form = new LoginForm(this);
            form.ShowDialog();
        }

        private void menuClipProperties_Click(object sender, EventArgs e)
        {
            ClipPropertiesForm frm = new ClipPropertiesForm(this);
            frm.ShowDialog();
        }

        private void exitMenuStrip_Click(object sender, EventArgs e)
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

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Recalculate();
        }

        private void saveMenuStrip_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            Clip.Current.Paragraphs = m_paragraphs;
            Clip.Current.RtfText = richTextBox1.Rtf;
            Clip.Current.Save();

            MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (Clip.Current.IsDirty && 
                    MessageBox.Show("השיעור לא נשמר מהשינויים האחרונים.\n\nהאם אתה בטוח להמשיך?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    ClipDetails frm = new ClipDetails(FormMode.New);
                    frm.ShowDialog();

                    if ( frm.Result != System.Windows.Forms.DialogResult.Cancel )
                    {
                        this.Text = "MyMentor - " + Clip.Current.Title;

                        m_disableScanningText = true;
                        richTextBox1.Rtf = Clip.Current.RtfText;
                        m_disableScanningText = false;

                        if (Clip.Current.Paragraphs != null && Clip.Current.Paragraphs.Count() > 0)
                        {
                            m_paragraphs = Clip.Current.Paragraphs;
                            Recalculate();
                        }
                    }
                }
            }
            else
            {
                ClipDetails frm = new ClipDetails(FormMode.New);
                frm.ShowDialog();

                if (frm.Result != System.Windows.Forms.DialogResult.Cancel)
                {
                    this.Text = "MyMentor - " + Clip.Current.Title;

                    m_disableScanningText = true;
                    richTextBox1.Rtf = Clip.Current.RtfText;
                    m_disableScanningText = false;

                    if (Clip.Current.Paragraphs != null && Clip.Current.Paragraphs.Count() > 0)
                    {
                        m_paragraphs = Clip.Current.Paragraphs;
                        Recalculate();
                    }
                }
            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (MessageBox.Show("השיעור לא נשמר מהשינויים האחרונים.\n\nהאם אתה בטוח להמשיך?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    ClipDetails frm = new ClipDetails(FormMode.Exists);
                    frm.ShowDialog();

                    if (frm.Result != System.Windows.Forms.DialogResult.Cancel)
                    {
                        this.Text = "MyMentor - " + Clip.Current.Title;

                        m_disableScanningText = true;
                        richTextBox1.Rtf = Clip.Current.RtfText;
                        m_disableScanningText = false;

                        if (Clip.Current.Paragraphs != null && Clip.Current.Paragraphs.Count() > 0)
                        {
                            m_paragraphs = Clip.Current.Paragraphs;
                            Recalculate();
                        }
                    }
                }
            }
            else
            {
                ClipDetails frm = new ClipDetails(FormMode.Exists);
                frm.ShowDialog();

                if (frm.Result != System.Windows.Forms.DialogResult.Cancel)
                {
                    this.Text = "MyMentor - " + Clip.Current.Title;

                    m_disableScanningText = true;
                    richTextBox1.Rtf = Clip.Current.RtfText;
                    m_disableScanningText = false;

                    if (Clip.Current.Paragraphs != null && Clip.Current.Paragraphs.Count() > 0)
                    {
                        m_paragraphs = Clip.Current.Paragraphs;
                        Recalculate();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectedPath"></param>
        private void OpenClip(string selectedPath)
        {
            try
            {
                Clip.Load(selectedPath);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("קובץ שיעור אינו תקין");
                return;
            }

            Settings.Default.LastDirectory = selectedPath;
            Settings.Default.Save();

            this.Text = "MyMentor - " + Clip.Current.Title;

            m_disableScanningText = true;
            richTextBox1.Rtf = Clip.Current.RtfText;
            m_disableScanningText = false;

            if (Clip.Current.Paragraphs != null && Clip.Current.Paragraphs.Count() > 0)
            {
                m_paragraphs = Clip.Current.Paragraphs;
                Recalculate();
            }

        }

        private void publishMenuStrip_Click(object sender, EventArgs e)
        {
            Clip.Current.Paragraphs = m_paragraphs;
            Clip.Current.RtfText = richTextBox1.Rtf;
            Clip.Current.Save();

            PublishForm frm = new PublishForm();
            frm.ShowDialog();
        }
    }

    public class SectionCellData
    {
        public int Index { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string Text { get; set; }
    }

    public abstract class BaseSection
    {
        [JsonProperty(PropertyName= "index", Order = 1)]
        public int Index { get; set; }

        [JsonProperty(PropertyName = "charIndex", Order = 2)]
        public int CharIndex { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public TimeSpan StartTime { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public TimeSpan Duration { get; set; }

        [JsonProperty(PropertyName = "audioStart", Order = 3)]
        [XmlIgnore]
        public string StartTimeText
        {
            get
            {
                return this.StartTime.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        [JsonProperty(PropertyName = "audioDuration", Order = 4)]
        [XmlIgnore]
        public string DurationText
        {
            get
            {
                return this.Duration.ToString(@"hh\:mm\:ss\.fff");
            }
        }

        [JsonIgnore]
        [XmlAttribute("StartTime")]
        public long XmlStartTime
        {
            get { return StartTime.Ticks; }
            set { StartTime = new TimeSpan(value); }
        }

        [JsonIgnore]
        [XmlAttribute("Duration")]
        public long XmlDuration
        {
            get { return Duration.Ticks; }
            set { Duration = new TimeSpan(value); }
        }
    }

    public class Word : BaseSection
    {
        [JsonProperty(PropertyName = "text", Order = 5)]
        public string Text { get; set; }
    }

    public class Section : BaseSection
    {
        [JsonProperty(PropertyName = "words", Order = 5)]
        [XmlArrayItem("Words")]
        public List<Word> Words { get; set; }
    }

    public class Sentence : BaseSection
    {
        [JsonProperty(PropertyName = "sections", Order = 5)]
        [XmlArrayItem("Sections")]
        public List<Section> Sections { get; set; }
    }

    public class Paragraph : BaseSection
    {
        [JsonProperty(PropertyName = "sentences", Order = 5)]
        [XmlArrayItem("Sentences")]
        public List<Sentence> Sentences { get; set; }
    }

    public enum AnchorType
    {
        Paragraph,
        Sentence,
        Section,
        Word,
        None
    }

    public enum AnchorDirection
    {
        Open,
        Close
    }

}
