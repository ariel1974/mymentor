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
    public partial class JsonDebugFrm : Form
    {
        public JsonDebugFrm()
        {
            InitializeComponent();
        }

        private void JsonDebugFrm_Load(object sender, EventArgs e)
        {
            this.textBox1.Text = Clip.Current.ExtractJson();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
