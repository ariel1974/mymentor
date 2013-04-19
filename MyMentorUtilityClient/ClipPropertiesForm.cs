using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            Clip.Current.Version = maskedTextBox1.Text;
            Clip.Current.Category = textBox4.Text;
            Clip.Current.SubCategory = textBox5.Text;
            Clip.Current.Tags = textBox6.Text;
            Clip.Current.Status = comboBox1.Text;
            Clip.Current.Duration = clipDurationTimer.Value;
            Clip.Current.AutoIncrementVersion = checkBox1.Checked;
            Clip.Current.Save();

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
            textBox2.Text = Clip.Current.Directory;
            maskedTextBox1.Text = Clip.Current.Version;
            textBox4.Text = Clip.Current.Category;
            textBox5.Text = Clip.Current.SubCategory;
            textBox6.Text = Clip.Current.Tags;
            comboBox1.Text = Clip.Current.Status;
            clipDurationTimer.Value = Clip.Current.Duration;
            checkBox1.Checked = Clip.Current.AutoIncrementVersion;
        }
    }
}
