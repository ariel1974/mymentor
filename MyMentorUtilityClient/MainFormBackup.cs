//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Diagnostics;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Xml;
//using System.Xml.Serialization;
//using ECN.SchoolSoundSystem;
//using Ionic.Zip;
//using Parse;

//namespace MyMentorUtilityClient
//{
//    public partial class MainForm : Form
//    {
//        private MultiKeyGesture m_ShortcutPiska = new MultiKeyGesture(new List<Keys> { Keys.M, Keys.P }, Keys.Control);
//        private MultiKeyGesture m_ShortcutKeta = new MultiKeyGesture(new List<Keys> { Keys.M, Keys.K }, Keys.Control);
//        private MultiKeyGesture m_ShortcutMishpat = new MultiKeyGesture(new List<Keys> { Keys.M, Keys.M }, Keys.Control);

//        private List<Word> m_words = null;
//        private List<Piska> m_piskaot = null;

//        private Word m_selected = null;
//        private bool m_disableScanningText;

//        public MainForm()
//        {
//            InitializeComponent();
//        }

//        private void Recalculate()
//        {
//            int charIndex = 0;
//            int piskaIndex = -1;
//            int mishpatIndex = -1;
//            int ketaIndex = -1;
//            int wordIndex = -1;

//            richTextBox2.Rtf = richTextBox1.Rtf;
//            string word = string.Empty;
//            int realCharIndex = 0;
//            int lastWordCharIndex = 0;

//            var m_temp = new List<Word>();

//            if (m_words == null)
//            {
//                m_words = new List<Word>();
//            }

//            TimeSpan next = new TimeSpan(0, 0, 0);
//            bool blOpenMila = false;

//            while (charIndex < richTextBox2.Text.Length)
//            {
//                richTextBox2.Select(charIndex, 1);

//                //start piska
//                if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Red)
//                {
//                    piskaIndex++;
//                }

//                //start mishpat
//                else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Blue)
//                {
//                    mishpatIndex++;
//                }

//                //start keta
//                else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Green)
//                {
//                    ketaIndex++;
//                }

//                //start mila
//                else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Black)
//                {
//                    blOpenMila = true;
//                }

//                //end mila
//                else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Black)
//                {
//                    blOpenMila = false;
//                }

//                    // if empty OR start mila
//                else if ((!blOpenMila && richTextBox2.SelectedText == " ") || richTextBox2.SelectedText == "]")
//                {
//                    if (richTextBox2.SelectedText != "[" && richTextBox2.SelectedText != "]")
//                    {
//                        realCharIndex++;
//                    }

//                    if (!string.IsNullOrEmpty(word.Trim()))
//                    {
//                        wordIndex++;

//                        var tmp = m_words.FirstOrDefault(p => p.WordIndex == wordIndex);
//                        TimeSpan start = new TimeSpan(0, 0, 0);
//                        TimeSpan duration = new TimeSpan(0, 0, 0);

//                        if (tmp != null)
//                        {
//                            start = tmp.StartTime;
//                            duration = tmp.Duration;
//                        }
//                        else
//                        {
//                            start = next;
//                        }

//                        if (start.Hours == 0 && start.Minutes == 0 && start.Seconds == 0)
//                        {
//                            start = next;
//                        }
//                        else if (wordIndex >= 1 && start.Add(duration) < next)
//                        {
//                            start = next;
//                        }

//                        m_temp.Add(new Word
//                        {
//                            CharIndex = lastWordCharIndex,
//                            KetaIndex = ketaIndex,
//                            WordIndex = wordIndex,
//                            MishpatIndex = mishpatIndex,
//                            PiskaIndex = piskaIndex,
//                            StartTime = start,
//                            Duration = duration,
//                            Text = word

//                        });

//                        next = start.Add(duration);

//                        word = string.Empty;
//                    }
//                }
//                else
//                {
//                    if (string.IsNullOrEmpty(word))
//                    {
//                        lastWordCharIndex = realCharIndex;
//                    }

//                    if (richTextBox2.SelectedText != "\n")
//                    {
//                        word = word + richTextBox2.SelectedText;
//                        realCharIndex++;
//                    }
//                }

//                charIndex++;
//            }

//            m_words = m_temp;

//            wordsGridView.DataSource = null;
//            wordsGridView.DataSource = m_words;

//            FixGridLayout();

//            button4.Enabled = false;
//        }

//        private void FixGridLayout()
//        {
//            wordsGridView.Columns[0].Width = 70;
//            wordsGridView.Columns[1].Width = 70;
//            wordsGridView.Columns[2].Width = 70;
//            wordsGridView.Columns[3].Width = 70;
//            wordsGridView.Columns[4].Width = 70;
//            wordsGridView.Columns[5].Width = 70;
//            wordsGridView.Columns[8].Visible = false;
//            wordsGridView.Columns[9].Visible = false;

//            DataGridViewColumn timeColumn = wordsGridView.Columns["StartTime"];
//            timeColumn.DefaultCellStyle.FormatProvider = new TimeSpanFormatter();
//            timeColumn.DefaultCellStyle.Format = "hh:mm:ss.fff";

//            DataGridViewColumn durationColumn = wordsGridView.Columns["Duration"];
//            durationColumn.DefaultCellStyle.FormatProvider = new TimeSpanFormatter();
//            durationColumn.DefaultCellStyle.Format = "hh:mm:ss.fff";


//        }

//        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
//        {
//        }


//        private async void button1_Click(object sender, EventArgs e)
//        {
//            var testObject = new ParseObject("TestObject");
//            testObject["foo"] = "bar";
//            await testObject.SaveAsync();
//        }

//        private async void button2_Click(object sender, EventArgs e)
//        {
//            byte[] bytes = File.ReadAllBytes(@"c:\test.zip");
//            ParseFile file = new ParseFile("test.zip", bytes);
//            await file.SaveAsync();

//            var jobApplication = new ParseObject("JobApplication");
//            jobApplication["applicantName"] = "Joe Smith";
//            jobApplication["applicantResumeFile"] = file;
//            await jobApplication.SaveAsync();
//        }

//        public class MultiKeyGesture
//        {
//            private List<Keys> _keys;
//            public MultiKeyGesture(IEnumerable<Keys> keys, Keys modifiers)
//            {
//                _keys = new List<Keys>(keys);

//                if (_keys.Count == 0)
//                {
//                    throw new ArgumentException("At least one key must be specified.", "keys");
//                }
//            }

//            private int currentindex;
//            public bool Matches(KeyEventArgs e)
//            {
//                if (_keys[currentindex] == e.KeyCode)
//                    //at least a partial match
//                    currentindex++;
//                else
//                    //No Match
//                    currentindex = 0;
//                if (currentindex + 1 > _keys.Count)
//                {
//                    //Matched last key
//                    currentindex = 0;
//                    return true;
//                }
//                return false;
//            }
//        }

//        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
//        {
//            base.OnKeyDown(e);
//            try
//            {
//                if (m_ShortcutKeta.Matches(e))
//                {
//                    if (GetNextDirection(AnchorType.Keta) == AnchorDirection.Open)
//                    {
//                        richTextBox1.AppendText("[");
//                    }
//                    else
//                    {
//                        richTextBox1.AppendText("]");

//                    }
//                    richTextBox1.Select(richTextBox1.TextLength - 1, 1);
//                    richTextBox1.SelectionColor = Color.Green;
//                    richTextBox1.Select(richTextBox1.TextLength, 0);
//                    richTextBox1.SelectionColor = Color.Black;
//                }
//                if (m_ShortcutMishpat.Matches(e))
//                {
//                    if (GetNextDirection(AnchorType.Mishpat) == AnchorDirection.Open)
//                    {
//                        richTextBox1.AppendText("[");
//                    }
//                    else
//                    {
//                        richTextBox1.AppendText("]");

//                    }
//                    richTextBox1.Select(richTextBox1.TextLength - 1, 1);
//                    richTextBox1.SelectionColor = Color.Blue;
//                    richTextBox1.Select(richTextBox1.TextLength, 0);
//                    richTextBox1.SelectionColor = Color.Black;
//                }
//                if (m_ShortcutPiska.Matches(e))
//                {
//                    if (GetNextDirection(AnchorType.Piska) == AnchorDirection.Open)
//                    {
//                        richTextBox1.AppendText("[");
//                    }
//                    else
//                    {
//                        richTextBox1.AppendText("]");

//                    }
//                    richTextBox1.Select(richTextBox1.TextLength - 1, 1);
//                    richTextBox1.SelectionColor = Color.Red;
//                    richTextBox1.Select(richTextBox1.TextLength, 0);
//                    richTextBox1.SelectionColor = Color.Black;
//                }
//            }
//            catch (ApplicationException ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
//        }

//        private void Form1_Load(object sender, EventArgs e)
//        {

//        }

//        private void richTextBox1_TextChanged(object sender, EventArgs e)
//        {
//            if (!m_disableScanningText && cbScanText.Checked)
//            {
//                Recalculate();
//            }
//        }

//        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
//        {
//            //e.
//        }

//        private void wordsGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
//        {
//            var formatter = e.CellStyle.FormatProvider as ICustomFormatter;
//            if (formatter != null)
//            {
//                e.Value = formatter.Format(e.CellStyle.Format, e.Value, e.CellStyle.FormatProvider);
//                e.FormattingApplied = true;
//            }
//        }

//        private void label10_Click(object sender, EventArgs e)
//        {

//        }

//        private void timePicker1_OnValueChanged(object sender, EventArgs e)
//        {
//            if (m_selected != null)
//            {
//                m_selected.StartTime = ((TimePicker)sender).Value;
//            }


//        }

//        private void wordsGridView_CellClick(object sender, DataGridViewCellEventArgs e)
//        {
//            if (wordsGridView.SelectedRows.Count > 0)
//            {

//                int wordIndex = (int)wordsGridView.SelectedRows[0].Cells[1].Value;

//                m_selected = m_words.First(p => p.WordIndex == wordIndex);

//                startTimer.Value = m_selected.StartTime;
//                durationTimer.Value = m_selected.Duration;

//                button4.Enabled = true;
//            }
//        }

//        private void wordsGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {

//        }

//        private void button4_Click(object sender, EventArgs e)
//        {
//            int selectedIndex = richTextBox1.SelectionStart;

//            if (m_selected != null)
//            {
//                m_selected.StartTime = startTimer.Value;
//                m_selected.Duration = durationTimer.Value;

//                Recalculate();
//                //richTextBox1.SelectionStart = selectedIndex;
//                //richTextBox1.SelectionLength = 0;
//                richTextBox1.Focus();

//                richTextBox1_SelectionChanged(null, new EventArgs());
//            }

//        }

//        private void wordsGridView_SelectionChanged(object sender, EventArgs e)
//        {
//            if (wordsGridView.SelectedRows.Count > 0)
//            {

//                int wordIndex = (int)wordsGridView.SelectedRows[0].Cells[1].Value;

//                m_selected = m_words.First(p => p.WordIndex == wordIndex);

//                startTimer.Value = m_selected.StartTime;
//                durationTimer.Value = m_selected.Duration;

//                button4.Enabled = true;
//            }
//        }

//        private AnchorDirection GetNextDirection(AnchorType anchorType)
//        {
//            int piskaot = 0;
//            int mishpatim = 0;
//            int ktaim = 0;
//            int milim = 0;

//            int charIndex = 0;
//            int remember = richTextBox1.SelectionStart;

//            richTextBox1.Select(0, remember);

//            richTextBox2.Rtf = richTextBox1.SelectedRtf;

//            richTextBox1.SelectionStart = remember;
//            richTextBox1.SelectionLength = 0;

//            while (charIndex < richTextBox2.Text.Length)
//            {
//                richTextBox2.Select(charIndex, 1);

//                //start piska
//                if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Red)
//                {
//                    piskaot++;
//                }

//                //close piska
//                else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Red)
//                {
//                    piskaot--;
//                }

//                //start mishpat
//                else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Blue)
//                {
//                    mishpatim++;
//                }

//                //close mishpat
//                else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Blue)
//                {
//                    mishpatim--;
//                }

//                //start keta
//                else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Green)
//                {
//                    ktaim++;
//                }

//                //close keta
//                else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Green)
//                {
//                    ktaim--;
//                }

//                //start mila
//                else if (richTextBox2.SelectedText == "[" && richTextBox2.SelectionColor == Color.Black)
//                {
//                    milim++;
//                }

//                //close mila
//                else if (richTextBox2.SelectedText == "]" && richTextBox2.SelectionColor == Color.Black)
//                {
//                    milim--;
//                }

//                charIndex++;
//            }

//            switch (anchorType)
//            {
//                case AnchorType.Mila:

//                    if (piskaot == 0 || mishpatim == 0 || ktaim == 0)
//                    {
//                        throw new ApplicationException("בחירה לא חוקית");
//                    }
//                    else
//                    {
//                        if (milim == 0)
//                        {
//                            return AnchorDirection.Open;
//                        }
//                        else
//                        {
//                            return AnchorDirection.Close;
//                        }
//                    }

//                case AnchorType.Keta:

//                    if (piskaot == 0 || mishpatim == 0)
//                    {
//                        throw new ApplicationException("בחירה לא חוקית");
//                    }
//                    else
//                    {
//                        if (ktaim == 0)
//                        {
//                            return AnchorDirection.Open;
//                        }
//                        else
//                        {
//                            return AnchorDirection.Close;
//                        }
//                    }

//                case AnchorType.Mishpat:

//                    if (piskaot == 0 || ktaim > 0)
//                    {
//                        throw new ApplicationException("בחירה לא חוקית");
//                    }
//                    else
//                    {
//                        if (mishpatim == 0)
//                        {
//                            return AnchorDirection.Open;
//                        }
//                        else
//                        {
//                            return AnchorDirection.Close;
//                        }
//                    }

//                case AnchorType.Piska:

//                    if (ktaim > 0 || mishpatim > 0)
//                    {
//                        throw new ApplicationException("בחירה לא חוקית");
//                    }
//                    else
//                    {
//                        if (piskaot == 0)
//                        {
//                            return AnchorDirection.Open;
//                        }
//                        else
//                        {
//                            return AnchorDirection.Close;
//                        }
//                    }

//                default:

//                    throw new ApplicationException("WTF");


//            }

//        }

//        private void Form1_Shown(object sender, EventArgs e)
//        {
//            richTextBox1.Focus();

//            ClipDetails frm = new ClipDetails();
//            frm.ShowDialog();

//            m_disableScanningText = true;
//            richTextBox1.Rtf = Clip.Current.RtfText;
//            m_disableScanningText = false;

//            if (Clip.Current.Words != null && Clip.Current.Words.Length > 0)
//            {
//                m_words = Clip.Current.Words.ToList();

//                wordsGridView.DataSource = m_words;
//                FixGridLayout();
//            }

//            tbShemShiur.Text = Clip.Current.Title;
//            mtbVersion.Text = Clip.Current.Version;
//        }

//        private void toolStripButton1_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (GetNextDirection(AnchorType.Keta) == AnchorDirection.Open)
//                {
//                    richTextBox2.Text = "[";
//                }
//                else
//                {
//                    richTextBox2.Text = "]";
//                }

//                richTextBox2.Select(0, 1);
//                richTextBox2.SelectionColor = Color.Green;

//                var selectionIndex = richTextBox1.SelectionStart;
//                richTextBox1.SelectedRtf = richTextBox2.SelectedRtf;
//                richTextBox1.Select(selectionIndex + 1, 0);
//                richTextBox1.SelectionColor = Color.Black;
//            }
//            catch (ApplicationException ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
//        }

//        private void toolStripButton2_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (GetNextDirection(AnchorType.Mishpat) == AnchorDirection.Open)
//                {
//                    richTextBox2.Text = "[";
//                }
//                else
//                {
//                    richTextBox2.Text = "]";
//                }

//                richTextBox2.Select(0, 1);
//                richTextBox2.SelectionColor = Color.Blue;

//                var selectionIndex = richTextBox1.SelectionStart;
//                richTextBox1.SelectedRtf = richTextBox2.SelectedRtf;
//                richTextBox1.Select(selectionIndex + 1, 0);
//                richTextBox1.SelectionColor = Color.Black;
//            }
//            catch (ApplicationException ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
//        }

//        private void toolStripButton3_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (GetNextDirection(AnchorType.Piska) == AnchorDirection.Open)
//                {
//                    richTextBox2.Text = "[";
//                }
//                else
//                {
//                    richTextBox2.Text = "]";
//                }

//                richTextBox2.Select(0, 1);
//                richTextBox2.SelectionColor = Color.Red;

//                var selectionIndex = richTextBox1.SelectionStart;
//                richTextBox1.SelectedRtf = richTextBox2.SelectedRtf;
//                richTextBox1.Select(selectionIndex + 1, 0);
//                richTextBox1.SelectionColor = Color.Black;
//            }
//            catch (ApplicationException ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
//        }

//        private void button5_Click(object sender, EventArgs e)
//        {
//            Clip.Current.Title = tbShemShiur.Text;
//            Clip.Current.Version = mtbVersion.Text;
//            Clip.Current.Words = m_words.ToArray();
//            Clip.Current.RtfText = richTextBox1.Rtf;
//            Clip.Current.Save();

//            MessageBox.Show("השיעור נשמר בהצלחה !");
//        }

//        private void toolStripButton4_Click(object sender, EventArgs e)
//        {
//            try
//            {
//                if (GetNextDirection(AnchorType.Mila) == AnchorDirection.Open)
//                {
//                    richTextBox2.Text = "[";
//                }
//                else
//                {
//                    richTextBox2.Text = "]";
//                }

//                richTextBox2.Select(0, 1);
//                richTextBox2.SelectionColor = Color.Black;

//                var selectionIndex = richTextBox1.SelectionStart;
//                richTextBox1.SelectedRtf = richTextBox2.SelectedRtf;
//                richTextBox1.Select(selectionIndex + 1, 0);
//                richTextBox1.SelectionColor = Color.Black;
//            }
//            catch (ApplicationException ex)
//            {
//                MessageBox.Show(ex.Message);
//            }
//        }

//        private void button6_Click(object sender, EventArgs e)
//        {
//            Recalculate();
//        }

//        private void btnUpdate_Click(object sender, EventArgs e)
//        {
//            Clip.Current.Title = tbShemShiur.Text;
//            Clip.Current.Version = mtbVersion.Text;
//        }

//        private void button7_Click(object sender, EventArgs e)
//        {
//            Clip.Current.Title = tbShemShiur.Text;
//            Clip.Current.Version = mtbVersion.Text;
//            Clip.Current.Words = m_words.ToArray();
//            Clip.Current.RtfText = richTextBox1.Rtf;
//            Clip.Current.Save();

//            if (Clip.Current.ExtractJson())
//            {
//                if (Clip.Current.Publish())
//                {

//                }
//            }
//        }

//        private void richTextBox1_CursorChanged(object sender, EventArgs e)
//        {

//        }

//        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
//        {
//            int selection = richTextBox1.SelectionStart;

//            if (selection > 0)
//            {
//                char[] chars = richTextBox1.Text.ToCharArray(0, selection);

//                //minus special characters
//                int minus = chars.Count(c => c == ']' || c == '[' || (int)c == 10);

//                int i = selection - 1;

//                while (chars[i] != ' ' && chars[i] != ']' && chars[i] != '[' && i >= 0)
//                {
//                    i--;
//                }

//                Word word = m_words.FirstOrDefault(w => w.CharIndex == (i + 1 - minus));

//                if (word != null)
//                {
//                    m_selected = word;

//                    tbMila.Text = word.Text;
//                    startTimer.Value = m_selected.StartTime;
//                    durationTimer.Value = m_selected.Duration;
//                    button4.Enabled = true;

//                }
//            }

//        }

//        private void button8_Click(object sender, EventArgs e)
//        {
//            Debug.WriteLine((int)richTextBox1.SelectedText.ToCharArray()[0]);
//        }

//        private void יציאהToolStripMenuItem_Click(object sender, EventArgs e)
//        {
//            this.Close();
//        }
//    }

//    public abstract class BaseSection
//    {
//        public int CharIndex { get; set; }

//        [XmlIgnore]
//        public TimeSpan StartTime { get; set; }

//        [XmlIgnore]
//        public TimeSpan Duration { get; set; }

//        [XmlIgnore]
//        public string StartTimeText
//        {
//            get
//            {
//                return this.StartTime.ToString(@"hh\:mm\:ss\.fff");
//            }
//        }

//        [XmlIgnore]
//        public string DurationText
//        {
//            get
//            {
//                return this.Duration.ToString(@"hh\:mm\:ss\.fff");
//            }
//        }


//        [XmlAttribute("StartTime")]
//        public long XmlStartTime
//        {
//            get { return StartTime.Ticks; }
//            set { StartTime = new TimeSpan(value); }
//        }

//        [XmlAttribute("Duration")]
//        public long XmlDuration
//        {
//            get { return Duration.Ticks; }
//            set { Duration = new TimeSpan(value); }
//        }
//    }

//    public class Word : BaseSection
//    {
//        public int WordIndex { get; set; }
//        public int KetaIndex { get; set; }
//        public int MishpatIndex { get; set; }
//        public int PiskaIndex { get; set; }
//        public string Text { get; set; }
//    }

//    public enum AnchorType
//    {
//        Piska,
//        Mishpat,
//        Keta,
//        Mila
//    }

//    public enum AnchorDirection
//    {
//        Open,
//        Close
//    }

//}
