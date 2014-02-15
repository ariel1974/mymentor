using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MyMentor
{
	/// <summary>
	/// Summary description for FormTempoRate.
	/// </summary>
	public class FormTempoRate : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TrackBar trackBar1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.TextBox textBoxPercentage;
		
		public bool		m_bIsChangingTempo;
		public bool		m_bCancel;
		public float	m_fChangePercentage;

		public FormTempoRate()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.labelMessage = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBoxPercentage = new System.Windows.Forms.TextBox();
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(176, 176);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(104, 24);
			this.buttonCancel.TabIndex = 8;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(48, 176);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(96, 24);
			this.buttonOK.TabIndex = 7;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(12, 8);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(304, 24);
			this.labelMessage.TabIndex = 9;
			this.labelMessage.Text = "-";
			this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(92, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(144, 24);
			this.label2.TabIndex = 10;
			this.label2.Text = "Percentage";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// textBoxPercentage
			// 
			this.textBoxPercentage.Location = new System.Drawing.Point(120, 72);
			this.textBoxPercentage.Name = "textBoxPercentage";
			this.textBoxPercentage.Size = new System.Drawing.Size(88, 20);
			this.textBoxPercentage.TabIndex = 11;
			this.textBoxPercentage.Text = "";
			this.textBoxPercentage.TextChanged += new System.EventHandler(this.textBoxPercentage_TextChanged);
			// 
			// trackBar1
			// 
			this.trackBar1.Location = new System.Drawing.Point(72, 112);
			this.trackBar1.Maximum = 9000;
			this.trackBar1.Minimum = -9000;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(184, 45);
			this.trackBar1.TabIndex = 12;
			this.trackBar1.TickFrequency = 1000;
			this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
			// 
			// FormTempoRate
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(328, 214);
			this.ControlBox = false;
			this.Controls.Add(this.trackBar1);
			this.Controls.Add(this.textBoxPercentage);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.KeyPreview = true;
			this.Name = "FormTempoRate";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Tempo change";
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormTempoRate_KeyPress);
			this.Load += new System.EventHandler(this.FormTempoRate_Load);
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void FormTempoRate_Load(object sender, System.EventArgs e)
		{
			if (m_bIsChangingTempo)
			{
				this.Text = "Tempo change";
				labelMessage.Text = "Change Tempo without affecting Pitch";
			}
			else
			{
				this.Text = "Playback Rate change";
				labelMessage.Text = "Change Playback Rate affecting both Tempo and Pitch";
			}
		    
			textBoxPercentage.Text = "0";
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			m_bCancel = false;
			Close ();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			m_bCancel = true;
			Close ();
		}

		private void trackBar1_Scroll(object sender, System.EventArgs e)
		{
			m_fChangePercentage = ((float) trackBar1.Value) / 100.0f;
			textBoxPercentage.Text = m_fChangePercentage.ToString ();
		}

		private void textBoxPercentage_TextChanged(object sender, System.EventArgs e)
		{
			if (textBoxPercentage.Text == "" || textBoxPercentage.Text == "-")
				return;

			m_fChangePercentage = Convert.ToSingle (textBoxPercentage.Text);
			trackBar1.Value = (int) (m_fChangePercentage * 100.0f);
		}

		private void FormTempoRate_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			//e.Handled = FormStudio.CheckKeyPress (textBoxPercentage, Convert.ToInt32(e.KeyChar));
		}
	}
}
