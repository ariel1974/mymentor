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
        private FormMode m_mode;
        private System.Windows.Forms.DialogResult result;

        public System.Windows.Forms.DialogResult Result
        {
            get { return result; }
            set { result = value; }
        }


        public ClipDetails()
        {
            InitializeComponent();
        }

        public ClipDetails(FormMode mode)
        {
            InitializeComponent();

            m_mode = mode;
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
                if ( 
                    string.IsNullOrEmpty( textBox1.Text.Trim() ) ||
                    string.IsNullOrEmpty( textBox2.Text.Trim() ) ||
                    string.IsNullOrEmpty( textBox4.Text.Trim() ) ||
                    string.IsNullOrEmpty( textBox5.Text.Trim() ) ||
                    string.IsNullOrEmpty( textBox6.Text.Trim() ) ||
                    string.IsNullOrEmpty( comboBox1.Text.Trim() ) 
                    )
                {
                    MessageBox.Show("יש להזין את כל שדות מאפייני השיעור", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    return;
                }

                Clip.Current = null;
                Clip.Current.AutoIncrementVersion = true;
                Clip.Current.Directory = textBox2.Text;
                Clip.Current.Title = textBox1.Text;
                Clip.Current.Version = "1.00";
                Clip.Current.Category = textBox4.Text;
                Clip.Current.SubCategory = textBox5.Text;
                Clip.Current.Duration = new TimeSpan(0, 0, 0);
                Clip.Current.Tags = textBox6.Text;
                Clip.Current.Status = comboBox1.Text;
                Clip.Current.ID = Guid.NewGuid();
                Clip.Current.IsNew = true;
                Clip.Current.Save();

                Settings.Default.LastDirectory = textBox2.Text;
                Settings.Default.Save();

                this.Close();
            }
            else
            {
                if (
                    string.IsNullOrEmpty(textBox3.Text.Trim())
                    )
                {
                    MessageBox.Show("יש לבחור תקייה", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    return;
                }

                Clip.Current = null;

                try
                {
                    Clip.Load(textBox3.Text);
                }
                catch (ApplicationException ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
                catch(Exception ex)
                {
                    MessageBox.Show("קובץ שיעור אינו תקין", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    return;
                }

                Settings.Default.LastDirectory = textBox3.Text;
                Settings.Default.Save();

                this.Close();
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButton1.Checked = !radioButton2.Checked;

            FixLayout();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            radioButton2.Checked = !radioButton1.Checked;

            FixLayout();
        }

        private void FixLayout()
        {
            groupBox1.Enabled = radioButton1.Checked;
            groupBox2.Enabled = radioButton2.Checked;

            radioButton1.Enabled = true;
            radioButton2.Enabled = true;
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
            if (m_mode == FormMode.New)
            {
                radioButton1.Checked = true;
            }
            else if (m_mode == FormMode.Exists)
            {
                radioButton2.Checked = true;
                textBox3.Text = Settings.Default.LastDirectory;
            }
            else
            {
                radioButton2.Checked = !Settings.Default.IsNewClipOption;

                textBox3.Text = Settings.Default.LastDirectory;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            result = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }

    public enum FormMode
    {
        NA,
        New,
        Exists
    }
}
