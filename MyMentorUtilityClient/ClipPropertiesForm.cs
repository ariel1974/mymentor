using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMentorUtilityClient
{
    public partial class ClipPropertiesForm : Form
    {
        private MainForm m_mainForm;

        public ClipPropertiesForm(MainForm mainForm)
        {
            InitializeComponent();

            m_mainForm = mainForm;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (
                string.IsNullOrEmpty(textBox1.Text.Trim()) ||
                string.IsNullOrEmpty(textBox3.Text.Trim()) ||
                string.IsNullOrEmpty(textBox4.Text.Trim()) ||
                string.IsNullOrEmpty(textBox5.Text.Trim()) ||
                string.IsNullOrEmpty(textBox6.Text.Trim()) ||
                string.IsNullOrEmpty(comboBox1.Text.Trim()) ||
                string.IsNullOrEmpty(maskedTextBox1.Text.Trim())
                )
            {
                MessageBox.Show("יש להזין את כל שדות מאפייני השיעור");
                return;
            }

            Clip.Current.Title = textBox1.Text;
            Clip.Current.Description = textBox3.Text;
            Clip.Current.Version = maskedTextBox1.Text;
            Clip.Current.Category = textBox4.Text;
            Clip.Current.SubCategory = textBox5.Text;
            Clip.Current.Tags = textBox6.Text;
            Clip.Current.AudioFileName = textBox2.Text;
            Clip.Current.Status = comboBox1.Text;
            Clip.Current.Duration = clipDurationTimer.Value;
            Clip.Current.AutoIncrementVersion = checkBox1.Checked;

            Clip.Current.DefaultSections.paragraph = def_par.Checked ? 1 : 0;
            Clip.Current.DefaultSections.sentence = def_sen.Checked ? 1 : 0;
            Clip.Current.DefaultSections.section = def_sec.Checked ? 1 : 0;
            Clip.Current.DefaultSections.chapter = def_wor.Checked ? 1 : 0;

            Clip.Current.LockedSections.paragraph = def_par.Checked ? 1 : 0;
            Clip.Current.LockedSections.sentence = def_sen.Checked ? 1 : 0;
            Clip.Current.LockedSections.section = def_sec.Checked ? 1 : 0;
            Clip.Current.LockedSections.chapter = def_wor.Checked ? 1 : 0;

            Clip.Current.DefaultLearningOptions.teacher1 = sop_teacher1.Checked ? 1 : 0;
            Clip.Current.DefaultLearningOptions.teacherAndStudent = sop_teacherAndStudent.Checked ? 1 : 0;
            Clip.Current.DefaultLearningOptions.teacher2 = sop_teacher2.Checked ? 1 : 0;
            Clip.Current.DefaultLearningOptions.student = sop_student.Checked ? 1 : 0;

            Clip.Current.LockedLearningOptions.teacher1 = sop_teacher1l.Checked ? 1 : 0;
            Clip.Current.LockedLearningOptions.teacherAndStudent = sop_teacherAndStudentl.Checked ? 1 : 0;
            Clip.Current.LockedLearningOptions.teacher2 = sop_teacher2l.Checked ? 1 : 0;
            Clip.Current.LockedLearningOptions.student = sop_studentl.Checked ? 1 : 0;

            if (string.IsNullOrEmpty(Clip.Current.FileName) && Clip.Current.IsNew)
            {
                DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyMentor Clips"));

                if (!di.Exists)
                {
                    di.Create();
                }

                saveFileDialog1.InitialDirectory = di.FullName;
                saveFileDialog1.FileName = Clip.Current.Title.ToValidFileName();

                saveFileDialog1.DefaultExt = "mmnx";
                saveFileDialog1.Filter = "MyMentor Source Files|*.mmnx";

                DialogResult result = saveFileDialog1.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Clip.Current.FileName = saveFileDialog1.FileName;
                    Clip.Current.RtfText = m_mainForm.richTextBox1.Rtf;
                    Clip.Current.Save();
                    m_mainForm.toolStripMenuItem8.Enabled = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Clip.Current.Save();
                m_mainForm.toolStripMenuItem8.Enabled = true;
            }

            m_mainForm.Text = "MyMentor - " + textBox1.Text;

            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClipPropertiesForm_Load(object sender, EventArgs e)
        {
            textBox1.Text = Clip.Current.Title;
            textBox3.Text = Clip.Current.Description;
            maskedTextBox1.Text = Clip.Current.Version;
            textBox4.Text = Clip.Current.Category;
            textBox2.Text = Clip.Current.AudioFileName;
            textBox5.Text = Clip.Current.SubCategory;
            textBox6.Text = Clip.Current.Tags;
            comboBox1.Text = Clip.Current.Status;
            clipDurationTimer.Value = Clip.Current.Duration;
            checkBox1.Checked = Clip.Current.AutoIncrementVersion;

            def_par.Checked = Clip.Current.DefaultSections.paragraph == 1;
            def_sen.Checked = Clip.Current.DefaultSections.sentence == 1;
            def_sec.Checked = Clip.Current.DefaultSections.section == 1;
            def_wor.Checked = Clip.Current.DefaultSections.chapter == 1;

            loc_par.Checked = Clip.Current.LockedSections.paragraph == 1;
            loc_sen.Checked = Clip.Current.LockedSections.sentence == 1;
            loc_sec.Checked = Clip.Current.LockedSections.section == 1;
            loc_wor.Checked = Clip.Current.LockedSections.chapter == 1;

            sop_teacher1.Checked = Clip.Current.DefaultLearningOptions.teacher1 == 1;
            sop_teacherAndStudent.Checked = Clip.Current.DefaultLearningOptions.teacherAndStudent == 1;
            sop_teacher2.Checked = Clip.Current.DefaultLearningOptions.teacher2 == 1;
            sop_student.Checked = Clip.Current.DefaultLearningOptions.student == 1;

            sop_teacher1l.Checked = Clip.Current.LockedLearningOptions.teacher1 == 1;
            sop_teacherAndStudentl.Checked = Clip.Current.LockedLearningOptions.teacherAndStudent == 1;
            sop_teacher2l.Checked = Clip.Current.LockedLearningOptions.teacher2 == 1;
            sop_studentl.Checked = Clip.Current.LockedLearningOptions.student == 1;

            if (Clip.Current.LastPublishedOn.HasValue)
            {
                textBox7.Text = Clip.Current.LastPublishedOn.Value.ToString("F");
            }
            else
            {
                textBox7.Text = "לא פורסם מעולם";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyMentor Clips"));

            if (!di.Exists)
            {
                di.Create();
            }

            openFileDialog1.InitialDirectory = di.FullName;
            openFileDialog1.Filter = "Mp3 Files|*.mp3";
            openFileDialog1.FileName = "";

            DialogResult result = openFileDialog1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }
    }
}
