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
    public partial class LoginForm : Form
    {
        private MainForm m_mainForm;

        public LoginForm(MainForm mainForm)
        {
            InitializeComponent();

            m_mainForm = mainForm;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                await ParseUser.LogInAsync(textBox1.Text, textBox2.Text);

                //m_mainForm.lblLoginUser.Text = "מחובר כ-" + ParseUser.CurrentUser.Username;

                this.Close();
            }
            catch
            {
                MessageBox.Show("שם המשתמש או הסיסמה אינם תואמים, נסה שוב");
            }
        }
    }
}
