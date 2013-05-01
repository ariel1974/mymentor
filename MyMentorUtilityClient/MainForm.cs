﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private List<Paragraph> m_paragraphs = null;

        private object m_selected = null;
        private bool m_disableScanningText;
        private AnchorType m_selectedAnchorType = AnchorType.None;

        public MainForm()
        {
            InitializeComponent();
        }

        private static Regex m_regexParagraphs = new Regex(@"(?<=\{\{)(.*?)(?=\}\})", RegexOptions.Compiled);
        private static Regex m_regexFreeParagraphs = new Regex(@"({{|}})", RegexOptions.Compiled);

        private static Regex m_regexSentenses = new Regex(@"(?<=\(\()(.*?)(?=\)\))", RegexOptions.Compiled);
        private static Regex m_regexFreeSentenses = new Regex(@"(\(\(|\)\))", RegexOptions.Compiled);

        private static Regex m_regexSections = new Regex(@"(?<=\<\<)(.*?)(?=\>\>)", RegexOptions.Compiled);
        private static Regex m_regexFreeSections = new Regex(@"(\<\<|\>\>)", RegexOptions.Compiled);

        private static Regex m_regexWords = new Regex(@"(?<=\[\[)(.*?)(?=\]\])|(\w+)", RegexOptions.Compiled);
        private static Regex m_regexFreeWords = new Regex(@"(\[\[|\]\])", RegexOptions.Compiled);

        private void ScanText()
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

                /// Paragraphs

                MatchCollection matchesParagraphs = m_regexParagraphs.Matches(richTextBox1.Text);

                foreach (Match matchParagraph in matchesParagraphs)
                {
                    // test for more paragraph sections
                    Match testPar = m_regexFreeParagraphs.Match(matchParagraph.Value);
                    if (testPar.Success)
                    {
                        throw new ApplicationException(string.Format("עוגן פסקה מיותר"));
                    }

                    paragraphIndex++;
                    innerSentenceIndex = -1;

                    TimeSpan start = new TimeSpan(0, 0, 0);
                    TimeSpan duration = new TimeSpan(0, 0, 0);

                    paragraphs_local.Add(new Paragraph
                    {
                        CharIndex = matchParagraph.Index - ((paragraphIndex + 1) * 2) - paragraphIndex * 2,
                        Index = paragraphIndex,
                        Sentences = new List<Sentence>(),
                        Words = new List<Word>(),
                        StartTime = start,
                        Duration = duration,
                    });

                    /// Sentenses 
                    /// 
                    MatchCollection matchesSentenses = m_regexSentenses.Matches(matchParagraph.Value);

                    //in case exists sentences 
                    if (matchesSentenses.Count > 0)
                    {
                        foreach (Match matchSentense in matchesSentenses)
                        {
                            // test for more paragraph sections
                            testPar = m_regexFreeSentenses.Match(matchSentense.Value);
                            if (testPar.Success)
                            {
                                throw new ApplicationException(string.Format("עוגן משפט מיותר בטקסט"));
                            }

                            //fix parent paragraph char index
                            paragraphs_local[paragraphIndex].CharIndex = paragraphIndex == 0 ? 
                                                                        paragraphs_local[paragraphIndex].CharIndex : 
                                                                        paragraphs_local[paragraphIndex].CharIndex - ((sentenceIndex + 1) * 4);

                            sentenceIndex++;
                            innerSentenceIndex++;
                            innerSectionIndex = -1;

                            paragraphs_local[paragraphIndex].Sentences.Add(new Sentence
                            {
                                CharIndex = paragraphs_local[paragraphIndex].CharIndex,
                                Index = sentenceIndex,
                                Sections = new List<Section>(),
                                Words = new List<Word>(),
                                StartTime = start,
                                Duration = duration
                            });

                            /// Sections 
                            /// 
                            MatchCollection matchesSections = m_regexSections.Matches(matchSentense.Value);

                            if (matchesSections.Count > 0)
                            {

                                foreach (Match matchSection in matchesSections)
                                {
                                    // test for more paragraph sections
                                    testPar = m_regexFreeSections.Match(matchSection.Value);
                                    if (testPar.Success)
                                    {
                                        throw new ApplicationException(string.Format("עוגן קטע מיותר"));
                                    }

                                    //fix parent paragraph char index
                                    //paragraphs_local[paragraphIndex].CharIndex = paragraphs_local[paragraphIndex].CharIndex - ((sectionIndex + 1) * 4);

                                    //fix parent paragraph char index
                                    paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].CharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].CharIndex - (paragraphIndex * ((sectionIndex + 1) * 4));

                                    sectionIndex++;
                                    innerSectionIndex++;

                                    paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections.Add(new Section
                                    {
                                        CharIndex = paragraphs_local[paragraphIndex].CharIndex,
                                        Index = sectionIndex,
                                        Words = new List<Word>(),
                                        StartTime = start,
                                        Duration = duration
                                    });

                                    /// Sections 
                                    /// 
                                    MatchCollection matchesWords = m_regexWords.Matches(matchSection.Value);

                                    foreach (Match matchWord in matchesWords)
                                    {
                                        // test for more paragraph sections
                                        testPar = m_regexFreeWords.Match(matchWord.Value);
                                        if (testPar.Success)
                                        {
                                            throw new ApplicationException(string.Format("עוגן מילה מיותר"));
                                        }

                                        wordIndex++;

                                        paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Words.Add(new Word
                                        {
                                            CharIndex = matchWord.Index + matchSection.Index + matchSentense.Index + matchParagraph.Index,
                                            Index = wordIndex,
                                            Text = matchWord.Value,
                                            StartTime = start,
                                            Duration = duration
                                        });
                                    }


                                }
                            }
                            else
                            {
                                //no sections for this sentense
                                /// 
                                MatchCollection matchesWords = m_regexWords.Matches(matchSentense.Value);

                                foreach (Match matchWord in matchesWords)
                                {
                                    // test for more paragraph sections
                                    testPar = m_regexFreeWords.Match(matchWord.Value);
                                    if (testPar.Success)
                                    {
                                        throw new ApplicationException(string.Format("עוגן מילה מיותר"));
                                    }

                                    wordIndex++;

                                    paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Words.Add(new Word
                                    {
                                        CharIndex = matchWord.Index + matchSentense.Index + matchParagraph.Index,
                                        Index = wordIndex,
                                        Text = matchWord.Value,
                                        StartTime = start,
                                        Duration = duration
                                    });
                                }

                            }

                        }
                    }
                    else
                    {
                        //no sentense for this paragraph
                        /// Sections 
                        /// 
                        MatchCollection matchesWords = m_regexWords.Matches(matchParagraph.Value);

                        foreach (Match matchWord in matchesWords)
                        {
                            // test for more paragraph sections
                            testPar = m_regexFreeWords.Match(matchWord.Value);
                            if (testPar.Success)
                            {
                                throw new ApplicationException(string.Format("עוגן מילה מיותר"));
                            }

                            wordIndex++;

                            paragraphs_local[paragraphIndex].Words.Add(new Word
                            {
                                CharIndex = matchWord.Index + matchParagraph.Index,
                                Index = wordIndex,
                                Text = matchWord.Value,
                                StartTime = start,
                                Duration = duration
                            });
                        }

                    }
                }

                m_paragraphs = paragraphs_local;

                paragraphsGrid.DataSource = m_paragraphs.ToList();
                sentencesGrid.DataSource = m_paragraphs.SelectMany(p => p.Sentences).ToList();
                sectionsGrid.DataSource = m_paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).ToList();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
            catch (Exception ex)
            {

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

        private void AddAnchor(AnchorType type, AnchorDirection direction)
        {
            //remember state
            string remember = richTextBox1.Text;
            var selectionIndex = richTextBox1.SelectionStart;

            if (direction == AnchorDirection.Close)
            {
                while (selectionIndex < remember.Length &&
                !remember.Substring(selectionIndex, 1).IsPartOfAnchor() &&
                remember.Substring(selectionIndex, 1) != " ")
                {
                    selectionIndex++;
                }
            }
            else
            {
                while (selectionIndex > 0 &&
                !remember.Substring(selectionIndex - 1, 1).IsPartOfAnchor() &&
                remember.Substring(selectionIndex - 1, 1) != " ")
                {
                    selectionIndex--;
                }
            }

            richTextBox1.SelectionStart = selectionIndex;

            try
            {

                switch (type)
                {
                    case AnchorType.Paragraph:
                        if (direction == AnchorDirection.Open)
                        {
                            richTextBox2.Text = Clip.PAR_SIGN_OPEN;
                        }
                        else
                        {
                            richTextBox2.Text = Clip.PAR_SIGN_CLOSE;
                        }

                        break;
                    case AnchorType.Sentence:
                        if (direction == AnchorDirection.Open)
                        {
                            richTextBox2.Text = Clip.SEN_SIGN_OPEN;
                        }
                        else
                        {
                            richTextBox2.Text = Clip.SEN_SIGN_CLOSE;
                        }

                        break;
                    case AnchorType.Section:
                        if (direction == AnchorDirection.Open)
                        {
                            richTextBox2.Text = Clip.SEC_SIGN_OPEN;
                        }
                        else
                        {
                            richTextBox2.Text = Clip.SEC_SIGN_CLOSE;
                        }

                        break;
                    case AnchorType.Word:
                        if (direction == AnchorDirection.Open)
                        {
                            richTextBox2.Text = Clip.WOR_SIGN_OPEN;
                        }
                        else
                        {
                            richTextBox2.Text = Clip.WOR_SIGN_CLOSE;
                        }

                        break;
                }

                richTextBox2.Select(0, 2);
                //richTextBox2.SelectionColor = Color.Green;

                richTextBox1.SelectedRtf = richTextBox2.SelectedRtf;
                //richTextBox1.Select(selectionIndex + 1 , 0);
                richTextBox1.SelectionStart = selectionIndex + (direction == AnchorDirection.Close ? -2 : 0);
                //richTextBox1.SelectionColor = Color.Black;

                /// FIND AUTO OPEN LOGIC
                if (direction == AnchorDirection.Close)
                {
                    int charIndex = selectionIndex - 2;

                    richTextBox2.Rtf = richTextBox1.Rtf;
                    bool blFoundAnchor = false;

                    while (charIndex >= 0 && !blFoundAnchor)
                    {
                        richTextBox2.Select(charIndex, 2);

                        switch (type)
                        {
                            case AnchorType.Paragraph:

                                if (
                                    richTextBox2.SelectedText == Clip.SEC_SIGN_OPEN ||
                                    richTextBox2.SelectedText == Clip.SEC_SIGN_CLOSE ||
                                    richTextBox2.SelectedText == Clip.SEN_SIGN_CLOSE ||
                                    richTextBox2.SelectedText == Clip.SEN_SIGN_OPEN
                                    )
                                {
                                    throw new ApplicationException();
                                }

                                //if this is opening paragraph tag
                                if (richTextBox2.SelectedText == Clip.PAR_SIGN_OPEN)
                                {
                                    blFoundAnchor = true;
                                    break;
                                }
                                //if this is closing paragraph tag
                                else if (richTextBox2.SelectedText == Clip.PAR_SIGN_CLOSE)
                                {
                                    richTextBox1.Text = richTextBox1.Text.Insert(charIndex + 2, Clip.PAR_SIGN_OPEN);
                                    blFoundAnchor = true;
                                    break;
                                }

                                break;

                            case AnchorType.Sentence:

                                //if this is opening paragraph tag
                                if (richTextBox2.SelectedText == Clip.PAR_SIGN_OPEN ||
                                    richTextBox2.SelectedText == Clip.SEN_SIGN_CLOSE)
                                {
                                    richTextBox1.Text = richTextBox1.Text.Insert(charIndex + 2, Clip.SEN_SIGN_OPEN);
                                    blFoundAnchor = true;
                                    break;
                                }
                                else if (richTextBox2.SelectedText == Clip.SEN_SIGN_OPEN)
                                {
                                    blFoundAnchor = true;
                                    break;
                                }
                                break;

                            case AnchorType.Section:

                                if (
                                    richTextBox2.SelectedText == Clip.PAR_SIGN_CLOSE ||
                                    richTextBox2.SelectedText == Clip.PAR_SIGN_OPEN
                                    )
                                {
                                    throw new ApplicationException();
                                }
                                else if (richTextBox2.SelectedText == Clip.SEC_SIGN_OPEN)
                                {
                                    blFoundAnchor = true;
                                    break;
                                }
                                //if this is opening paragraph tag
                                else if (
                                    richTextBox2.SelectedText == Clip.SEN_SIGN_OPEN)
                                {
                                    richTextBox1.Text = richTextBox1.Text.Insert(charIndex + 2, Clip.SEC_SIGN_OPEN);
                                    blFoundAnchor = true;
                                    break;
                                }

                                break;

                            case AnchorType.Word:

                                if (
                                    //richTextBox2.SelectedText == PAR_SIGN_CLOSE ||
                                    //richTextBox2.SelectedText == PAR_SIGN_OPEN ||
                                    richTextBox2.SelectedText == Clip.SEN_SIGN_OPEN
                                    )
                                {
                                    throw new ApplicationException();
                                }

                                //if this is opening paragraph tag
                                if (
                                    richTextBox2.SelectedText == Clip.SEC_SIGN_OPEN ||
                                    richTextBox2.SelectedText == Clip.WOR_SIGN_CLOSE ||
                                    richTextBox2.SelectedText.Substring(1, 1) == " " ||
                                    richTextBox2.SelectedText.Substring(1, 1) == ".")
                                {
                                    richTextBox1.Text = richTextBox1.Text.Insert(charIndex + 2, Clip.WOR_SIGN_OPEN);
                                    blFoundAnchor = true;
                                    break;
                                }

                                break;
                        }

                        charIndex -= 1;
                    }

                    //inacse not find any opening anchor and reach the start of file
                    if (!blFoundAnchor)
                    {
                        switch (type)
                        {
                            case AnchorType.Paragraph:
                                richTextBox1.Text = string.Concat(Clip.PAR_SIGN_OPEN, richTextBox1.Text);
                                break;
                            case AnchorType.Sentence:
                                richTextBox1.Text = string.Concat(Clip.SEN_SIGN_OPEN, richTextBox1.Text);
                                break;
                            case AnchorType.Section:
                                richTextBox1.Text = string.Concat(Clip.SEC_SIGN_OPEN, richTextBox1.Text);
                                break;
                            case AnchorType.Word:
                                richTextBox1.Text = string.Concat(Clip.WOR_SIGN_OPEN, richTextBox1.Text);
                                break;
                        }

                    }

                    richTextBox1.SelectionStart = selectionIndex + 4;
                }
                else
                {
                    ///OPEN LOGIC
                    ///

                }
            }
            catch
            {
                richTextBox1.Text = remember;
                richTextBox1.SelectionStart = selectionIndex;
                MessageBox.Show("בחירה לא חוקית\n\nשים לב שרמת העוגנים מתאימה ברמה שהנך נמצא", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!m_disableScanningText)
            {
                Clip.Current.IsDirty = true;
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
                ((BaseSection)m_selected).StartTime = ((TimePicker)sender).Value;
            }


        }

        private void button4_Click(object sender, EventArgs e)
        {
            int selectedIndex = richTextBox1.SelectionStart;

            if (m_selected != null)
            {
                switch (m_selectedAnchorType)
                {
                    case AnchorType.Paragraph:
                        break;
                    case AnchorType.Sentence:
                        break;
                    case AnchorType.Section:
                        break;
                    case AnchorType.Word:
                        break;
                    default:
                        break;
                }

                ((BaseSection)m_selected).StartTime = startTimer.Value;
                ((BaseSection)m_selected).Duration = durationTimer.Value;

                ScanText();
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

            //if (ParseUser.CurrentUser == null)
            //{
            //    LoginForm frmLogin = new LoginForm(this);
            //    frmLogin.ShowDialog();
            //}
            //else
            //{
            //    lblLoginUser.Text = "מחובר כ-" + ParseUser.CurrentUser.Username;
            //}

            Clip.Current.Title = "שיעור 1";
            Clip.Current.IsDirty = false;
            Clip.Current.IsNew = true;
            Clip.Current.ID = Guid.NewGuid();
            this.Text = "MyMentor - " + Clip.Current.Title;
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

        private void button7_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsNew)
            {
                Save();

                if (string.IsNullOrEmpty(Clip.Current.FileName))
                {
                    return;
                }
            }

            ScanText();

            Clip.Current.Paragraphs = m_paragraphs;
            Clip.Current.RtfText = richTextBox1.Rtf;
            Clip.Current.Save();

            PublishForm frm = new PublishForm(this);
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
                        startTimer.Value = ((BaseSection)m_selected).StartTime;
                        durationTimer.Value = ((BaseSection)m_selected).Duration;

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
                m_selected = paragraphsGrid.SelectedRows[0].DataBoundItem as Paragraph;

                if (m_selected != null)
                {
                    m_selectedAnchorType = AnchorType.Paragraph;
                    sectionGroup.Text = "תזמון פסקה";
                    lblSectionText.Text = "פסקה";
                    tbSectionText.Text = ((Paragraph)m_selected).Content;
                    startTimer.Value = ((Paragraph)m_selected).StartTime;
                    durationTimer.Value = ((Paragraph)m_selected).Duration;
                }
            }

        }

        private void sentencesGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sentencesGrid.SelectedRows.Count > 0)
            {
                m_selected = sentencesGrid.SelectedRows[0].DataBoundItem as Sentence;

                if (m_selected != null)
                {
                    m_selectedAnchorType = AnchorType.Sentence;
                    sectionGroup.Text = "תזמון משפט";
                    lblSectionText.Text = "משפט";
                    tbSectionText.Text = ((Sentence)m_selected).Content;
                    startTimer.Value = ((Sentence)m_selected).StartTime;
                    durationTimer.Value = ((Sentence)m_selected).Duration;
                }
            }
        }

        private void sectionsGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (sectionsGrid.SelectedRows.Count > 0)
            {
                m_selected = sectionsGrid.SelectedRows[0].DataBoundItem as Section;

                if (m_selected != null)
                {
                    m_selectedAnchorType = AnchorType.Section;
                    sectionGroup.Text = "תזמון קטע";
                    lblSectionText.Text = "קטע";
                    tbSectionText.Text = ((Section)m_selected).Content;
                    startTimer.Value = ((Section)m_selected).StartTime;
                    durationTimer.Value = ((Section)m_selected).Duration;
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
            Save();
        }

        private void saveMenuStrip_Click(object sender, EventArgs e)
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
                    Clip.Current.FileName = saveFileDialog1.FileName;
                    Clip.Current.RtfText = richTextBox1.Rtf;
                    Clip.Current.Save();

                    MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    toolStripMenuItem8.Enabled = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Clip.Current.RtfText = richTextBox1.Rtf;
                Clip.Current.Save();

                MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }

        private void NewClip()
        {
            Clip.Current = null;
            Clip.Current.AutoIncrementVersion = true;
            Clip.Current.Title = "שיעור 1";
            Clip.Current.Version = "1.00";
            Clip.Current.Status = "PENDING";
            Clip.Current.ID = Guid.NewGuid();
            Clip.Current.IsNew = true;

            this.Text = "MyMentor - " + Clip.Current.Title;

            m_disableScanningText = true;
            richTextBox1.Rtf = Clip.Current.RtfText;
            m_disableScanningText = false;

        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (Clip.Current.IsDirty &&
                    MessageBox.Show("השיעור לא נשמר מהשינויים האחרונים.\n\nהאם אתה בטוח להמשיך?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    NewClip();
                }
            }
            else
            {
                NewClip();
            }
        }

        private void OpenClip()
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
                try
                {
                    Clip.Load(openFileDialog1.FileName);
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

                this.Text = "MyMentor - " + Clip.Current.Title;

                m_disableScanningText = true;
                richTextBox1.Rtf = Clip.Current.RtfText;
                m_disableScanningText = false;

                if (Clip.Current.Paragraphs != null && Clip.Current.Paragraphs.Count() > 0)
                {
                    m_paragraphs = Clip.Current.Paragraphs;
                    ScanText();
                }
            }
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (MessageBox.Show("השיעור לא נשמר מהשינויים האחרונים.\n\nהאם אתה בטוח להמשיך?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    OpenClip();
                    toolStripMenuItem8.Enabled = true;
                }
            }
            else
            {
                OpenClip();
                toolStripMenuItem8.Enabled = true;
            }
        }

        private void publishMenuStrip_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsNew)
            {
                Save();

                if (string.IsNullOrEmpty(Clip.Current.FileName))
                {
                    return;
                }
            }

            ScanText();

            Clip.Current.Paragraphs = m_paragraphs;
            Clip.Current.RtfText = richTextBox1.Rtf;
            Clip.Current.Save();

            PublishForm frm = new PublishForm(this);
            frm.ShowDialog();
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            Save(true);
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {

        }

        private void tbrNew_Click(object sender, EventArgs e)
        {
            toolStripMenuItem5_Click(null, new EventArgs());
        }

        private void tbrOpen_Click(object sender, EventArgs e)
        {
            toolStripMenuItem6_Click(null, new EventArgs());
        }

        private void tbrSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void tbrFont_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(richTextBox1.Font == null))
                {
                    fontDialog1.Font = richTextBox1.Font;
                }
                else
                {
                    fontDialog1.Font = null;
                }
                fontDialog1.ShowApply = true;
                if (fontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    richTextBox1.Font = fontDialog1.Font;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        private void tbrLeft_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
        }

        private void tbrCenter_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
        }

        private void tbrRight_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
        }

        private void tbrBold_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(richTextBox1.SelectionFont == null))
                {
                    System.Drawing.Font currentFont = richTextBox1.SelectionFont;
                    System.Drawing.FontStyle newFontStyle;

                    newFontStyle = richTextBox1.SelectionFont.Style ^ FontStyle.Bold;

                    richTextBox1.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        private void tbrItalic_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(richTextBox1.SelectionFont == null))
                {
                    System.Drawing.Font currentFont = richTextBox1.SelectionFont;
                    System.Drawing.FontStyle newFontStyle;

                    newFontStyle = richTextBox1.SelectionFont.Style ^ FontStyle.Italic;

                    richTextBox1.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        private void tbrUnderline_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(richTextBox1.SelectionFont == null))
                {
                    System.Drawing.Font currentFont = richTextBox1.SelectionFont;
                    System.Drawing.FontStyle newFontStyle;

                    newFontStyle = richTextBox1.SelectionFont.Style ^ FontStyle.Underline;

                    richTextBox1.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Paragraph, AnchorDirection.Close);
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Paragraph, AnchorDirection.Open);
        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Sentence, AnchorDirection.Open);
        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Sentence, AnchorDirection.Close);
        }

        private void toolStripButton5_Click_1(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Section, AnchorDirection.Open);
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Section, AnchorDirection.Close);
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Word, AnchorDirection.Open);
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Word, AnchorDirection.Close);
        }

        private void פתחפסקהToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Paragraph, AnchorDirection.Open);
        }

        private void סגורפסקהToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Paragraph, AnchorDirection.Close);
        }

        private void פתחמשפטToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Sentence, AnchorDirection.Open);
        }

        private void סגורמשפטToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Sentence, AnchorDirection.Close);
        }

        private void פתחקטעToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Section, AnchorDirection.Open);
        }

        private void סגורקטעToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Section, AnchorDirection.Close);
        }

        private void פתחמילהToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Word, AnchorDirection.Open);
        }

        private void סגורמילהToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddAnchor(AnchorType.Word, AnchorDirection.Close);
        }

        private void אודותToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox frm = new AboutBox();
            frm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ScanText();
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            ScanText();
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsNew)
            {
                Save();

                if (string.IsNullOrEmpty(Clip.Current.FileName))
                {
                    return;
                }
            }

            ScanText();

            Clip.Current.Paragraphs = m_paragraphs;
            Clip.Current.RtfText = richTextBox1.Rtf;
            Clip.Current.Save();

            PublishForm frm = new PublishForm(this);
            frm.ShowDialog();

        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            ScanText();

        }

        private void validateMenuStrip_Click(object sender, EventArgs e)
        {
            ScanText();
        }

        private void toolStripMenuItem4_Click_1(object sender, EventArgs e)
        {
            ClipPropertiesForm frm = new ClipPropertiesForm(this);
            frm.ShowDialog();

        }

        private void startTimer_OnValueChanged(object sender, EventArgs e)
        {
            ((BaseSection)m_selected).StartTime = startTimer.Value;
            ((BaseSection)m_selected).Duration = durationTimer.Value;

            ScanText();
            richTextBox1.Focus();
        }

        private void durationTimer_OnValueChanged(object sender, EventArgs e)
        {
            ((BaseSection)m_selected).StartTime = startTimer.Value;
            ((BaseSection)m_selected).Duration = durationTimer.Value;

            ScanText();
            richTextBox1.Focus();
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            ClipPropertiesForm frm = new ClipPropertiesForm(this);
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
        [JsonProperty(PropertyName = "index", Order = 1)]
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

        [JsonProperty(PropertyName = "words", Order = 8)]
        [XmlArrayItem("Words")]
        public virtual List<Word> Words { get; set; }

    }

    public class Word : BaseSection
    {
        [JsonProperty(PropertyName = "text", Order = 5)]
        public string Text { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public override List<Word> Words
        {
            get
            {
                return base.Words;
            }
            set
            {
                base.Words = value;
            }
        }

    }

    public class Section : BaseSection
    {

        [JsonIgnore]
        [XmlIgnore]
        public string Content
        {
            get
            {
                try
                {
                    return this.Words.Select(w => w.Text).Aggregate((a, b) => a + " " + b);
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

    }

    public class Sentence : BaseSection
    {
        [JsonProperty(PropertyName = "sections", Order = 5)]
        [XmlArrayItem("Sections")]
        public List<Section> Sections { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public string Content
        {
            get
            {
                try
                {
                    if (this.Words.Count() > 0)
                    {
                        return this.Words.Select(w => w.Text).Aggregate((a, b) => a + " " + b);
                    }
                    else
                    {
                        return this.Sections.Select(s => s.Content).Aggregate((a, b) => a + " " + b);
                    }
                }
                catch
                {
                    return string.Empty;
                }

            }
        }

    }

    public class Paragraph : BaseSection
    {
        [JsonProperty(PropertyName = "sentences", Order = 5)]
        [XmlArrayItem("Sentences")]
        public List<Sentence> Sentences { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public string Content
        {
            get
            {
                try
                {
                    if (this.Words.Count() > 0)
                    {
                        return this.Words.Select(w => w.Text).Aggregate((a, b) => a + " " + b);
                    }
                    else
                    {
                        return this.Sentences.Select(s => s.Content).Aggregate((a, b) => a + " " + b);
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }
        }


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
