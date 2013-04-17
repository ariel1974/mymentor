using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyMentorUtilityClient.Properties;

namespace MyMentorUtilityClient
{
    public partial class ClipDetails : Form
    {
        public ClipDetails()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Clip.Current.Directory = textBox2.Text;
                Clip.Current.Title = textBox1.Text;
                Clip.Current.Version = maskedTextBox1.Text;
                Clip.Current.ID = Guid.NewGuid();
                Clip.Current.Save();

                Settings.Default.LastDirectory = textBox2.Text;
                Settings.Default.Save();

                this.Close();
            }
            else
            {
                Clip.Load(textBox3.Text);

                Settings.Default.LastDirectory = textBox3.Text;
                Settings.Default.Save();

                this.Close();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton2.Checked;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = !radioButton1.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBox3.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void ClipDetails_Load(object sender, EventArgs e)
        {
            radioButton2.Checked = !Settings.Default.IsNewClipOption;

            textBox3.Text = Settings.Default.LastDirectory;
        }
    }
}
