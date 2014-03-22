using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyMentor.Forms;
using Parse;

namespace MyMentor
{
    public partial class FormReadyTexts : Form
    {
        public ClipText SelectedSource
        {
            get
            {
                return m_selected;
            }
        }

        private List<ClipText> m_result = new List<ClipText>();

        private FormStudio m_formMain = null;
        private ClipText m_selected = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formMain"></param>
        public FormReadyTexts(FormStudio formMain)
        {
            InitializeComponent();

            m_formMain = formMain;

            string[] labels = m_formMain.GetCategoriesLabels();

            lblCategory1.Text = labels[0];
            lblCategory2.Text = labels[1];
            lblCategory3.Text = labels[2];

            this.comboCategory1.DisplayMember = "Value";
            this.comboCategory1.ValueMember = "ObjectId";
            comboCategory1.DataSource = m_formMain.GetCategory1();

            button3_Click(null, new EventArgs());
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            btnSearch.Enabled = false;
            button2.Enabled = false;
            listView1.Items.Clear();

            // The following queries are equivalent:
            //var query = from clip in ParseObject.GetQuery("ClipsV2")
            //            where clip["category1"] == ParseObject.CreateWithoutData("Category1", (string)comboCategory1.SelectedValue)
            //            && clip["clipType"] == ParseObject.CreateWithoutData("ClipType", "enaWrne5xe")
            //            select clip;

            var query1 = ParseObject.GetQuery("ClipsV2").OrderByDescending("createdAt")
                    .Limit(10) // Only retrieve the last 10 comments
                    .Include("status"); // Include the post data with each comment

            query1 = query1.WhereEqualTo("clipType", ParseObject.CreateWithoutData("ClipType", "enaWrne5xe"));
            query1 = query1.WhereEqualTo("status", ParseObject.CreateWithoutData("ClipStatus", "3DYQsyGZIk"));
            query1 = query1.WhereEqualTo("contentType", ParseObject.CreateWithoutData("WorldContentType", m_formMain.ContentType.ObjectId));

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
            else
            {
            //    query1 = query1.WhereContainedIn("category3", ((IEnumerable<Category>)comboCategory3.DataSource).Select( s => ParseObject.CreateWithoutData("Category3", s.ObjectId)));
            }

            var result = await query1.FindAsync();

            if (result != null && result.Count() > 0)
            {
                int counter = 0;

                m_result = new List<ClipText>(result.Select(a => new ClipText
                {
                    Index = counter++,
                    ObjectId = a.ObjectId,
                    Name = a.Get<string>("name"),
                    Rtf = a.Get<string>("clipSourceRtf"),
                    Category1 = a.Get<ParseObject>("category1"),
                    Category2 = a.Get<ParseObject>("category2"),
                    Category3 = a.Get<ParseObject>("category3"),
                    Description = a.Get<string>("description"),
                    DescriptionEnglish = a.ContainsKey("descriptionEnglish") ? a.Get<string>("descriptionEnglish") : string.Empty,
                    Status = a.Get<ParseObject>("status").FetchIfNeededAsync().Result
                }));

                foreach (var o in m_result)
                {
                    ListViewItem lvi = listView1.Items.Add(o.ObjectId, o.Name, -1);
                    lvi.SubItems.Add(o.Status.Get<string>("status_he_il"));
                }

            }
            else
            {
                MessageBox.Show(m_formMain.Strings.Single(a => a.Key == "STD_NO_SOURCES_FOUND").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0));
            }

            btnSearch.Enabled = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] labels = m_formMain.GetCategoriesLabels();

            lblCategory1.Text = labels[0];
            lblCategory2.Text = labels[1];
            lblCategory3.Text = labels[2];
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboCategory1.SelectedItem = null;
            comboCategory2.SelectedItem = null;
            comboCategory3.SelectedItem = null;
        }

        private async void FormReadyTexts_Load(object sender, EventArgs e)
        {
            string value_key = "value_" + MyMentor.Properties.Settings.Default.CultureInfo.Replace("-", "_");

            this.comboCategory3.DisplayMember = "Value";
            this.comboCategory3.ValueMember = "ObjectId";
            this.comboCategory3.DataSource = (await ParseTables.GetCategory3(m_formMain.ContentType.ObjectId, "0y8A4XTNeR")).Select(c => new Category
            {
                ObjectId = c.ObjectId,
                Value = c.ContainsKey(value_key) ? c.Get<string>(value_key) : string.Empty
            }).ToList();

            comboCategory3.SelectedItem = null;

        }

        private async void comboCategory1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string value_key = "value_" + MyMentor.Properties.Settings.Default.CultureInfo.Replace("-", "_");

            if (comboCategory1.SelectedIndex >= 0)
            {
                var list = (await ParseTables.GetCategory2((string)comboCategory1.SelectedValue)).Where(o => o.Keys.Count() == 4);

                this.comboCategory2.DisplayMember = "Value";
                this.comboCategory2.ValueMember = "ObjectId";
                this.comboCategory2.DataSource = list.Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    Value = c.ContainsKey(value_key) ? c.Get<string>(value_key) : string.Empty
                }).ToList(); ;

                this.comboCategory2.SelectedValue = string.Empty;
            }
            else
            {
                this.comboCategory2.DataSource = null;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                int indexer = listView1.SelectedItems[0].Index;
                m_selected = m_result.Single(a => a.Index == indexer);

                if (m_selected != null)
                {
                    button2.Enabled = true;
                }
            }
            else
            {
                button2.Enabled = false;
            }
        }
    }

    public class ClipText
    {
        public int Index { get; set; }
        public string ObjectId { get; set; }
        public string Name { get; set; }
        public string Rtf { get; set; }
        public ParseObject Category1 { get; set; }
        public ParseObject Category2 { get; set; }
        public ParseObject Category3 { get; set; }
        public string Description { get; set; }
        public string DescriptionEnglish { get; set; }
        public ParseObject Status { get; set; }
    }
}
