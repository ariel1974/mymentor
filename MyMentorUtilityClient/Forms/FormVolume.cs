using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using AudioSoundEditor;

namespace MyMentor
{
	/// <summary>
	/// Summary description for FormVolume.
	/// </summary>
	public class FormVolume : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBoxInitialVolume;
		private System.Windows.Forms.TextBox textBoxFinalVolume;
		private System.Windows.Forms.RadioButton radioButtonBoth;
		private System.Windows.Forms.RadioButton radioButtonLeft;
		private System.Windows.Forms.RadioButton radioButtonRight;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Label labelInitialVolume;
		private System.Windows.Forms.Label labelFinalVolume;
		private System.Windows.Forms.GroupBox groupBoxChannels;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		[DllImport("user32.dll")]
		public static extern int SetWindowLong( IntPtr window, int index, int value);
		[DllImport("user32.dll")]
		public static extern int GetWindowLong( IntPtr window, int index);

		const int GWL_STYLE = -16;
		const int ES_NUMBER = 0x2000;

		internal AudioSoundEditor.AudioSoundEditor	audioSoundEditor1;

		const int VOLUME_FLAT = 0;
		const int VOLUME_SLIDING = 1;

		public Int16		m_nVolumeMode;
		public Int16		m_nInitialVolume;
		public Int16		m_nFinalVolume;
		public enumChannels	m_nAffectedChannels;
		public bool			m_bCancel;

		public FormVolume()
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
			this.labelInitialVolume = new System.Windows.Forms.Label();
			this.labelFinalVolume = new System.Windows.Forms.Label();
			this.textBoxInitialVolume = new System.Windows.Forms.TextBox();
			this.textBoxFinalVolume = new System.Windows.Forms.TextBox();
			this.groupBoxChannels = new System.Windows.Forms.GroupBox();
			this.radioButtonRight = new System.Windows.Forms.RadioButton();
			this.radioButtonLeft = new System.Windows.Forms.RadioButton();
			this.radioButtonBoth = new System.Windows.Forms.RadioButton();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBoxChannels.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelInitialVolume
			// 
			this.labelInitialVolume.Location = new System.Drawing.Point(16, 16);
			this.labelInitialVolume.Name = "labelInitialVolume";
			this.labelInitialVolume.Size = new System.Drawing.Size(168, 16);
			this.labelInitialVolume.TabIndex = 0;
			this.labelInitialVolume.Text = "Flat volume (expressed in %)";
			// 
			// labelFinalVolume
			// 
			this.labelFinalVolume.Location = new System.Drawing.Point(16, 72);
			this.labelFinalVolume.Name = "labelFinalVolume";
			this.labelFinalVolume.Size = new System.Drawing.Size(160, 16);
			this.labelFinalVolume.TabIndex = 1;
			this.labelFinalVolume.Text = "Final volume (expressed in %)";
			// 
			// textBoxInitialVolume
			// 
			this.textBoxInitialVolume.Location = new System.Drawing.Point(16, 40);
			this.textBoxInitialVolume.Name = "textBoxInitialVolume";
			this.textBoxInitialVolume.Size = new System.Drawing.Size(168, 20);
			this.textBoxInitialVolume.TabIndex = 2;
			this.textBoxInitialVolume.Text = "200";
			// 
			// textBoxFinalVolume
			// 
			this.textBoxFinalVolume.Location = new System.Drawing.Point(16, 96);
			this.textBoxFinalVolume.Name = "textBoxFinalVolume";
			this.textBoxFinalVolume.Size = new System.Drawing.Size(168, 20);
			this.textBoxFinalVolume.TabIndex = 3;
			this.textBoxFinalVolume.Text = "100";
			// 
			// groupBoxChannels
			// 
			this.groupBoxChannels.Controls.Add(this.radioButtonRight);
			this.groupBoxChannels.Controls.Add(this.radioButtonLeft);
			this.groupBoxChannels.Controls.Add(this.radioButtonBoth);
			this.groupBoxChannels.Location = new System.Drawing.Point(200, 8);
			this.groupBoxChannels.Name = "groupBoxChannels";
			this.groupBoxChannels.Size = new System.Drawing.Size(128, 112);
			this.groupBoxChannels.TabIndex = 4;
			this.groupBoxChannels.TabStop = false;
			this.groupBoxChannels.Text = "Affected channels";
			// 
			// radioButtonRight
			// 
			this.radioButtonRight.Location = new System.Drawing.Point(16, 80);
			this.radioButtonRight.Name = "radioButtonRight";
			this.radioButtonRight.Size = new System.Drawing.Size(96, 16);
			this.radioButtonRight.TabIndex = 2;
			this.radioButtonRight.Text = "Right channel";
			// 
			// radioButtonLeft
			// 
			this.radioButtonLeft.Location = new System.Drawing.Point(16, 56);
			this.radioButtonLeft.Name = "radioButtonLeft";
			this.radioButtonLeft.Size = new System.Drawing.Size(96, 16);
			this.radioButtonLeft.TabIndex = 1;
			this.radioButtonLeft.Text = "Left channel";
			// 
			// radioButtonBoth
			// 
			this.radioButtonBoth.Location = new System.Drawing.Point(16, 32);
			this.radioButtonBoth.Name = "radioButtonBoth";
			this.radioButtonBoth.Size = new System.Drawing.Size(96, 16);
			this.radioButtonBoth.TabIndex = 0;
			this.radioButtonBoth.Text = "Both channels";
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(64, 144);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(96, 24);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(192, 144);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(104, 24);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// FormVolume
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(360, 182);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBoxChannels);
			this.Controls.Add(this.textBoxFinalVolume);
			this.Controls.Add(this.textBoxInitialVolume);
			this.Controls.Add(this.labelFinalVolume);
			this.Controls.Add(this.labelInitialVolume);
			this.Name = "FormVolume";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Volume settings";
			this.Load += new System.EventHandler(this.FormVolume_Load);
			this.groupBoxChannels.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormVolume_Load(object sender, System.EventArgs e)
		{
			// set the numeric style for the textBoxInitialVolume and textBoxFinalVolume textboxes
			// getting the current style then adding ES_NUMBER to the style
			Int32   nStyle = GetWindowLong(textBoxInitialVolume.Handle, GWL_STYLE);
			SetWindowLong (textBoxInitialVolume.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxFinalVolume.Handle, GWL_STYLE);
			SetWindowLong (textBoxFinalVolume.Handle, GWL_STYLE, nStyle | ES_NUMBER);

			if (m_nVolumeMode == VOLUME_FLAT)
			{
				this.Text = "Flat volume settings";
				labelInitialVolume.Text = "Flat volume (expressed in %)";
				labelFinalVolume.Visible = false;
				textBoxFinalVolume.Visible = false;
				textBoxInitialVolume.Text = "200";
			}
			else
			{
				this.Text = "Sliding volume settings";
				labelInitialVolume.Text = "Initial volume (expressed in %)";
				labelFinalVolume.Visible = true;
				textBoxFinalVolume.Visible = true;
				textBoxInitialVolume.Text = "0";
				textBoxFinalVolume.Text = "100";
			}

			m_nInitialVolume = -1;
			m_nFinalVolume = -1;
    
			if (audioSoundEditor1.GetChannels() == 2)
			{
				groupBoxChannels.Enabled = true;
				radioButtonBoth.Checked = true;
			}
			else
			{
				groupBoxChannels.Enabled = false;
				radioButtonBoth.Enabled = false;
				radioButtonLeft.Enabled = false;
				radioButtonRight.Enabled = false;
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			m_nInitialVolume = Convert.ToInt16 (textBoxInitialVolume.Text);
			m_nFinalVolume = Convert.ToInt16 (textBoxFinalVolume.Text);
			if (radioButtonBoth.Checked)
				m_nAffectedChannels = enumChannels.CHANNELS_BOTH;
			else if (radioButtonLeft.Checked)
				m_nAffectedChannels = enumChannels.CHANNELS_LEFT;
			else if (radioButtonRight.Checked)
				m_nAffectedChannels = enumChannels.CHANNELS_RIGHT;

			m_bCancel = false;
			Close ();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			m_bCancel = true;
			Close ();
		}
	}
}
