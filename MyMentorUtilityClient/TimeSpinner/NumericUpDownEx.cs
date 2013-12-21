using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyMentor.TimeSpinner
{
    public partial class NumericUpDownEx : NumericUpDown
    {
        private string m_Format;

        public NumericUpDownEx()
        {
            InitializeComponent();
        }

        [DefaultValue("{0:00}")]
        [Description("Format"), Category("Data")]
        public string Format
        {
            get { return m_Format; }
            set { m_Format = value; }
        }

        protected override void UpdateEditText()
        {
            // Append the units to the end of the numeric value
            this.Text = String.Format(this.Format ?? "{0:00}", this.Value);
        }
    }
}
