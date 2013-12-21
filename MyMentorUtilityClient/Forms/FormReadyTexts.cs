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

namespace MyMentor
{
    public partial class FormReadyTexts : Form
    {
        public string SelectedText
        {
            get
            {
                return m_selectedText;
            }
        }

        private FormMain m_formMain = null;
        private string m_selectedText = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formMain"></param>
        public FormReadyTexts(FormMain formMain)
        {
            InitializeComponent();

            m_formMain = formMain;

            string[] labels = m_formMain.GetCategoriesLabels();

            lblCategory1.Text = labels[0];
            lblCategory2.Text = labels[1];
            lblCategory3.Text = labels[2];
            lblCategory4.Text = labels[3];

            this.comboCategory1.DisplayMember = "Value";
            this.comboCategory1.ValueMember = "ObjectId";
            comboCategory1.DataSource = m_formMain.GetCategory1();

            this.comboCategory3.DisplayMember = "Value";
            this.comboCategory3.ValueMember = "ObjectId";
            comboCategory3.DataSource = m_formMain.GetCategory3();

            this.comboCategory4.DisplayMember = "Value";
            this.comboCategory4.ValueMember = "ObjectId";
            comboCategory4.DataSource = m_formMain.GetCategory4();

            button3_Click(null, new EventArgs());
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            button2.Enabled = false;

            // The following queries are equivalent:
            //var query = from clip in ParseObject.GetQuery("ClipsV2")
            //            where clip["category1"] == ParseObject.CreateWithoutData("Category1", (string)comboCategory1.SelectedValue)
            //            && clip["clipType"] == ParseObject.CreateWithoutData("ClipType", "enaWrne5xe")
            //            select clip;

            var query1 = ParseObject.GetQuery("ClipsV2");
            query1 = query1.WhereEqualTo("clipType", ParseObject.CreateWithoutData("ClipType", "enaWrne5xe"));

            if (!string.IsNullOrEmpty(tbClipDescription.Text.Trim()))
            {
                query1 = query1.WhereContains("description", tbClipDescription.Text.Trim());
            }

            if (!string.IsNullOrEmpty(tbKeywords.Text.Trim()))
            {
                query1 = query1.WhereContainedIn("keywords", tbKeywords.Text.Trim().Split(','));
            }

            if (comboCategory1.SelectedValue != null)
            {
                query1 = query1.WhereEqualTo("category1", ParseObject.CreateWithoutData("Category1", (string)comboCategory1.SelectedValue));
            }

            if (comboCategory2.SelectedValue != null)
            {
                query1 = query1.WhereEqualTo("category2", ParseObject.CreateWithoutData("Category2", (string)comboCategory2.SelectedValue));
            }

            if (comboCategory3.SelectedValue != null)
            {
                query1 = query1.WhereEqualTo("category3", ParseObject.CreateWithoutData("Category3", (string)comboCategory3.SelectedValue));
            }

            if (comboCategory4.SelectedValue != null)
            {
                query1 = query1.WhereEqualTo("category4", ParseObject.CreateWithoutData("Category4", (string)comboCategory4.SelectedValue));
            }

            var result = await query1.FindAsync();

            if (result != null)
            {
                listBox1.Items.Clear();
                listBox1.ValueMember = "ObjectId";
                listBox1.DisplayMember = "Name";
                listBox1.Items.AddRange(result.Select(a => new ClipText
                {
                    ObjectId = a.ObjectId,
                    Name = a.Get<string>("name"),
                    Text = a.Get<string>("clipSourceText")
                }).ToArray());
            }

            btnSearch.Enabled = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                m_selectedText = ((ClipText)listBox1.SelectedItem).Text;
                button2.Enabled = true;

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] labels = m_formMain.GetCategoriesLabels();

            lblCategory1.Text = labels[0];
            lblCategory2.Text = labels[1];
            lblCategory3.Text = labels[2];
            lblCategory4.Text = labels[3];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboCategory1.SelectedItem = null;
            comboCategory2.SelectedItem = null;
            comboCategory3.SelectedItem = null;
            comboCategory4.SelectedItem = null;
            tbClipDescription.Text = "";
            tbKeywords.Text = "";
        }
    }

    public class ClipText
    {
        public string ObjectId { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
    }
}
