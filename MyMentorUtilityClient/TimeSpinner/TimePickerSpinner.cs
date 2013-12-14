using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace MyMentorUtilityClient.TimeSpinner
{
    public partial class TimePickerSpinner : UserControl
    {
        private bool m_skipEvents = false;

        public event EventHandler ValueChanged;

        public TimePickerSpinner()
        {
            InitializeComponent();
        }

        private TimeSpan m_Value = TimeSpan.Zero;

        public TimeSpan Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;

                m_skipEvents = true;
                minutes.Value = m_Value.Minutes;
                seconds.Value = m_Value.Seconds;
                milliseconds.Value = m_Value.Milliseconds;
                m_skipEvents = false;
            }

        }

        private void milliseconds_ValueChanged(object sender, EventArgs e)
        {
            if (m_skipEvents)
                return;

            if (milliseconds.Value == 1000)
            {
                m_skipEvents = true;
                milliseconds.Value = 0;

                if (seconds.Value == 59)
                {
                    seconds.Value = 0;
                    minutes.Value = minutes.Value + 1;
                }
                else
                {
                    seconds.Value = seconds.Value + 1;
                }
                m_skipEvents = false;
            }

            m_Value = new TimeSpan(0, 0, (int)minutes.Value, (int)seconds.Value, (int)milliseconds.Value);

            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        private void seconds_ValueChanged(object sender, EventArgs e)
        {
            if (m_skipEvents)
                return;

            if (seconds.Value == 60)
            {
                m_skipEvents = true;
                seconds.Value = 0;

                if (minutes.Value == 59)
                {
                    minutes.Value = 0;
                }
                else
                {
                    minutes.Value = minutes.Value + 1;
                }
                m_skipEvents = false;
            }
            else if (seconds.Value == -1)
            {
                m_skipEvents = true;
                seconds.Value = 0;

                if (minutes.Value > 0)
                {
                    minutes.Value = minutes.Value - 1;
                }

                m_skipEvents = false;
            }


            m_Value = new TimeSpan(0, 0, (int)minutes.Value, (int)seconds.Value, (int)milliseconds.Value);

            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        private void minutes_ValueChanged(object sender, EventArgs e)
        {
            if (m_skipEvents)
                return;

            m_Value = new TimeSpan(0, 0, (int)minutes.Value, (int)seconds.Value, (int)milliseconds.Value);

            if (ValueChanged != null)
            {
                ValueChanged(this,e);
            }
        }

    }
}
