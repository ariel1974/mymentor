using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SoundStudio
{
	/// <summary>
	/// Summary description for FormPitch.
	/// </summary>
	public class FormPitch : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TrackBar trackBar1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.TextBox textBoxSemitones;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public bool		m_bCancel;
		public float	m_fChangeValue;

		public FormPitch()
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
			this.trackBar1 = new System.Windows.Forms.TrackBar();
			this.textBoxSemitones = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labelMessage = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBar1
			// 
			this.trackBar1.Location = new System.Drawing.Point(72, 112);
			this.trackBar1.Maximum = 5000;
			this.trackBar1.Minimum = -5000;
			this.trackBar1.Name = "trackBar1";
			this.trackBar1.Size = new System.Drawing.Size(184, 45);
			this.trackBar1.TabIndex = 18;
			this.trackBar1.TickFrequency = 500;
			this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
			// 
			// textBoxSemitones
			// 
			this.textBoxSemitones.Location = new System.Drawing.Point(120, 72);
			this.textBoxSemitones.Name = "textBoxSemitones";
			this.textBoxSemitones.Size = new System.Drawing.Size(88, 20);
			this.textBoxSemitones.TabIndex = 17;
			this.textBoxSemitones.Text = "";
			this.textBoxSemitones.TextChanged += new System.EventHandler(this.textBoxSemitones_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(88, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(144, 24);
			this.label2.TabIndex = 16;
			this.label2.Text = "Semitones";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelMessage
			// 
			this.labelMessage.Location = new System.Drawing.Point(8, 8);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size(304, 24);
			this.labelMessage.TabIndex = 15;
			this.labelMessage.Text = "Change Pitch without changing Tempo or Playback Rate";
			this.labelMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(176, 176);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(104, 24);
			this.buttonCancel.TabIndex = 14;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(48, 176);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(96, 24);
			this.buttonOK.TabIndex = 13;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// FormPitch
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(320, 222);
			this.ControlBox = false;
			this.Controls.Add(this.trackBar1);
			this.Controls.Add(this.textBoxSemitones);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.labelMessage);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.KeyPreview = true;
			this.Name = "FormPitch";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Pitch change";
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormPitch_KeyPress);
			this.Load += new System.EventHandler(this.FormPitch_Load);
			((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void FormPitch_Load(object sender, System.EventArgs e)
		{
			textBoxSemitones.Text = "0";		
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
			m_fChangeValue = ((float) trackBar1.Value) / 100.0f;
			textBoxSemitones.Text = m_fChangeValue.ToString ();
		}

		private void textBoxSemitones_TextChanged(object sender, System.EventArgs e)
		{
			if (textBoxSemitones.Text == "" || textBoxSemitones.Text == "-")
				return;

			m_fChangeValue = Convert.ToSingle (textBoxSemitones.Text);
			trackBar1.Value = (int) (m_fChangeValue * 100.0f);
		}

		private void FormPitch_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			e.Handled = FormMain.CheckKeyPress (textBoxSemitones, Convert.ToInt32(e.KeyChar));
		}
	}
}
