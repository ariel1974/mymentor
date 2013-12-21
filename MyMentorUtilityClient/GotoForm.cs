using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMentor
{
    public partial class GotoForm : Form
    {
        public int CharIndex { get; set; }

        public GotoForm()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.CharIndex = int.Parse(maskedTextBox1.Text);
        }

        private void GotoForm_Shown(object sender, EventArgs e)
        {
            maskedTextBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
