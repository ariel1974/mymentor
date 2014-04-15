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
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                button1.Enabled = false;
                await ParseUser.LogInAsync(textBox1.Text, textBox2.Text);
                this.Close();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            catch
            {
                MessageBox.Show("שם המשתמש או הסיסמה אינם תואמים, נסה שוב");
                button1.Enabled = true;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Do something
                button1_Click(null, new EventArgs());
            }

        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Do something
                button1_Click(null, new EventArgs());
            }

        }
    }
}
