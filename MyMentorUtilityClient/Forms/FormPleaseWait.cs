using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMentor.Forms
{
    public partial class FormPleaseWait : Form
    {
        public int Progress
        {
            get
            {
                return this.progressBar1.Value;
            }
            set
            {
                this.progressBar1.Value = value;
            }
        }


        public FormPleaseWait()
        {
            InitializeComponent();
        }
    }
}
