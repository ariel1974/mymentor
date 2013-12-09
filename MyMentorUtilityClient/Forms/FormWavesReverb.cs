using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SoundStudio
{
	/// <summary>
	/// Summary description for FormWavesReverb.
	/// </summary>
	public class FormWavesReverb : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxInputGain;
		private System.Windows.Forms.TextBox textBoxReverbMix;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textBoxReverbTime;
		private System.Windows.Forms.TextBox textBoxHighFreqRTRatio;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public float	m_fInGain;
		public float	m_fReverbMix;
		public float	m_fReverbTime;
		public float	m_fHighFreqRTRatio;
		public bool		m_bCancel;

		const int	DSFX_WAVESREVERB_INGAIN_MAX = 0;
		const int	DSFX_WAVESREVERB_INGAIN_MIN = -96;
		const int	DSFX_WAVESREVERB_REVERBMIX_MAX = 0;
		const int	DSFX_WAVESREVERB_REVERBMIX_MIN = -96;
		const float	DSFX_WAVESREVERB_HIGHFREQRTRATIO_MAX = 0.999f;
		const float	DSFX_WAVESREVERB_HIGHFREQRTRATIO_MIN = 0.001f;
		const int	DSFX_WAVESREVERB_REVERBTIME_MAX = 3000;
		const float	DSFX_WAVESREVERB_REVERBTIME_MIN = 0.001f;
		
		public FormWavesReverb()
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
			this.textBoxReverbTime = new System.Windows.Forms.TextBox();
			this.textBoxInputGain = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxHighFreqRTRatio = new System.Windows.Forms.TextBox();
			this.textBoxReverbMix = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(200, 144);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(104, 24);
			this.buttonCancel.TabIndex = 12;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(72, 144);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(96, 24);
			this.buttonOK.TabIndex = 11;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// textBoxReverbTime
			// 
			this.textBoxReverbTime.Location = new System.Drawing.Point(24, 96);
			this.textBoxReverbTime.Name = "textBoxReverbTime";
			this.textBoxReverbTime.Size = new System.Drawing.Size(88, 20);
			this.textBoxReverbTime.TabIndex = 10;
			this.textBoxReverbTime.Text = "";
			this.textBoxReverbTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxReverbTime_KeyPress);
			// 
			// textBoxInputGain
			// 
			this.textBoxInputGain.Location = new System.Drawing.Point(24, 40);
			this.textBoxInputGain.Name = "textBoxInputGain";
			this.textBoxInputGain.Size = new System.Drawing.Size(88, 20);
			this.textBoxInputGain.TabIndex = 9;
			this.textBoxInputGain.Text = "";
			this.textBoxInputGain.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxInputGain_KeyPress);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(160, 16);
			this.label2.TabIndex = 8;
			this.label2.Text = "Reverb time (expressed in ms)";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(168, 16);
			this.label1.TabIndex = 7;
			this.label1.Text = "Input gain (expressed in dB)";
			// 
			// textBoxHighFreqRTRatio
			// 
			this.textBoxHighFreqRTRatio.Location = new System.Drawing.Point(208, 96);
			this.textBoxHighFreqRTRatio.Name = "textBoxHighFreqRTRatio";
			this.textBoxHighFreqRTRatio.Size = new System.Drawing.Size(88, 20);
			this.textBoxHighFreqRTRatio.TabIndex = 16;
			this.textBoxHighFreqRTRatio.Text = "";
			this.textBoxHighFreqRTRatio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxHighFreqRTRatio_KeyPress);
			// 
			// textBoxReverbMix
			// 
			this.textBoxReverbMix.Location = new System.Drawing.Point(208, 40);
			this.textBoxReverbMix.Name = "textBoxReverbMix";
			this.textBoxReverbMix.Size = new System.Drawing.Size(88, 20);
			this.textBoxReverbMix.TabIndex = 15;
			this.textBoxReverbMix.Text = "";
			this.textBoxReverbMix.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxReverbMix_KeyPress);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(208, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(160, 16);
			this.label3.TabIndex = 14;
			this.label3.Text = "High-frequency reverb time ratio";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(208, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(168, 16);
			this.label4.TabIndex = 13;
			this.label4.Text = "Reverb mix (expressed in dB)";
			// 
			// FormWavesReverb
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(376, 182);
			this.ControlBox = false;
			this.Controls.Add(this.textBoxHighFreqRTRatio);
			this.Controls.Add(this.textBoxReverbMix);
			this.Controls.Add(this.textBoxReverbTime);
			this.Controls.Add(this.textBoxInputGain);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.KeyPreview = true;
			this.Name = "FormWavesReverb";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Waves Reverb DMO settings";
			this.Load += new System.EventHandler(this.FormWavesReverb_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormWavesReverb_Load(object sender, System.EventArgs e)
		{
			// set defualt values
			textBoxInputGain.Text = Convert.ToString (0);
			textBoxReverbMix.Text = Convert.ToString (0);
			textBoxReverbTime.Text = Convert.ToString (1000);
			textBoxHighFreqRTRatio.Text = Convert.ToString (0.001);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			m_fInGain = Convert.ToSingle (textBoxInputGain.Text);
			m_fReverbMix = Convert.ToSingle (textBoxReverbMix.Text);
			m_fReverbTime = Convert.ToSingle (textBoxReverbTime.Text);
			m_fHighFreqRTRatio = Convert.ToSingle (textBoxHighFreqRTRatio.Text);
    
			// check limits
			if (m_fInGain < DSFX_WAVESREVERB_INGAIN_MIN || m_fInGain > DSFX_WAVESREVERB_INGAIN_MAX)
			{
				textBoxInputGain.Focus ();
				textBoxInputGain.SelectionStart = 0;
				textBoxInputGain.SelectionLength = 100;
				MessageBox.Show ("Selected value is out of range");
				return;
			}
			if (m_fReverbMix < DSFX_WAVESREVERB_REVERBMIX_MIN || m_fReverbMix > DSFX_WAVESREVERB_REVERBMIX_MAX)
			{
				textBoxReverbMix.Focus ();
				textBoxReverbMix.SelectionStart = 0;
				textBoxReverbMix.SelectionLength = 100;
				MessageBox.Show ("Selected value is out of range");
				return;
			}
			if (m_fReverbTime < DSFX_WAVESREVERB_REVERBTIME_MIN || m_fReverbTime > DSFX_WAVESREVERB_REVERBTIME_MAX)
			{
				textBoxReverbTime.Focus ();
				textBoxReverbTime.SelectionStart = 0;
				textBoxReverbTime.SelectionLength = 100;
				MessageBox.Show ("Selected value is out of range");
				return;
			}
			if (m_fHighFreqRTRatio < DSFX_WAVESREVERB_HIGHFREQRTRATIO_MIN || m_fHighFreqRTRatio > DSFX_WAVESREVERB_HIGHFREQRTRATIO_MAX)
			{
				textBoxHighFreqRTRatio.Focus ();
				textBoxHighFreqRTRatio.SelectionStart = 0;
				textBoxHighFreqRTRatio.SelectionLength = 100;
				MessageBox.Show ("Selected value is out of range");
				return;
			}

			m_bCancel = false;
			Close ();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			m_bCancel = true;
			Close ();
		}

		private void textBoxInputGain_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			e.Handled = FormMain.CheckKeyPress (textBoxInputGain, Convert.ToInt32(e.KeyChar));
		}

		private void textBoxReverbTime_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			e.Handled = FormMain.CheckKeyPress (textBoxReverbTime, Convert.ToInt32(e.KeyChar));
		}

		private void textBoxReverbMix_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			e.Handled = FormMain.CheckKeyPress (textBoxReverbMix, Convert.ToInt32(e.KeyChar));
		}

		private void textBoxHighFreqRTRatio_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			e.Handled = FormMain.CheckKeyPress (textBoxHighFreqRTRatio, Convert.ToInt32(e.KeyChar));
		}
	}
}
