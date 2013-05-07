using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Parse;

namespace MyMentorUtilityClient
{
    public partial class PublishForm : Form
    {
        private MainForm m_mainForm = null;

        public PublishForm(MainForm mainForm)
        {
            m_mainForm = mainForm;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Clip.Current.Title)
                || string.IsNullOrEmpty(Clip.Current.Description)
                || string.IsNullOrEmpty(Clip.Current.Category)
                || string.IsNullOrEmpty(Clip.Current.SubCategory)
                || string.IsNullOrEmpty(Clip.Current.Tags)
                || string.IsNullOrEmpty(Clip.Current.Status))
            {
                MessageBox.Show("אנא השלם את מאפייני השיעור", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);

                ClipPropertiesForm frm = new ClipPropertiesForm(m_mainForm);
                frm.ShowDialog();

                return;
            }

            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            panelWait.Visible = true;

            label12.Text = "מכין קובץ...";

            if (Clip.Current.SaveJson(Clip.Current.ExtractJson()) && Clip.Current.ExtractHtml())
            {
                if (Clip.Current.Publish())
                {
                    label12.Text = "מעלה...אנא המתן";
                    
                    bool result = await Clip.Current.UploadAsync(new Progress<ParseUploadProgressEventArgs>(ev =>
                    {
                        progressBar1.Value = Convert.ToInt32(ev.Progress * 100);
                    }));

                    if (result)
                    {
                        if (Clip.Current.AutoIncrementVersion)
                        {
                            Clip.Current.Version = Convert.ToString( Convert.ToDouble(Clip.Current.Version) + 0.01);
                            Clip.Current.Save();
                        }

                        label12.Text = "הסתיים בהצלחה!";
                        panelWait.Visible = false;
                        MessageBox.Show("הסרטון פורסם בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                        this.Close();
                    }
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClipPropertiesForm frm = new ClipPropertiesForm(m_mainForm);
            frm.ShowDialog();
        }
    }
}
