using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using AudioSoundEditor;

namespace SoundStudio
{
	/// <summary>
	/// Summary description for FormEqualizer.
	/// </summary>
	public class FormEqualizer : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox comboBoxPresets;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TrackBar trackBar16000Hz;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TrackBar trackBar14000Hz;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TrackBar trackBar12000Hz;
		private System.Windows.Forms.TrackBar trackBar6000Hz;
		private System.Windows.Forms.TrackBar trackBar3000Hz;
		private System.Windows.Forms.TrackBar trackBar1000Hz;
		private System.Windows.Forms.TrackBar trackBar600Hz;
		private System.Windows.Forms.TrackBar trackBar310Hz;
		private System.Windows.Forms.TrackBar trackBar170Hz;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TrackBar trackBar80Hz;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;

		internal AudioSoundEditor.AudioSoundEditor	audioSoundEditor1;
		public bool		m_bCancel;
		private bool	m_bOpeningEqualizer = true;

		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormEqualizer()
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
			this.comboBoxPresets = new System.Windows.Forms.ComboBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.trackBar16000Hz = new System.Windows.Forms.TrackBar();
			this.label12 = new System.Windows.Forms.Label();
			this.trackBar14000Hz = new System.Windows.Forms.TrackBar();
			this.label10 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.trackBar12000Hz = new System.Windows.Forms.TrackBar();
			this.trackBar6000Hz = new System.Windows.Forms.TrackBar();
			this.trackBar3000Hz = new System.Windows.Forms.TrackBar();
			this.trackBar1000Hz = new System.Windows.Forms.TrackBar();
			this.trackBar600Hz = new System.Windows.Forms.TrackBar();
			this.trackBar310Hz = new System.Windows.Forms.TrackBar();
			this.trackBar170Hz = new System.Windows.Forms.TrackBar();
			this.label11 = new System.Windows.Forms.Label();
			this.trackBar80Hz = new System.Windows.Forms.TrackBar();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.trackBar16000Hz)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar14000Hz)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar12000Hz)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar6000Hz)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar3000Hz)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1000Hz)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar600Hz)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar310Hz)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar170Hz)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar80Hz)).BeginInit();
			this.SuspendLayout();
			// 
			// comboBoxPresets
			// 
			this.comboBoxPresets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxPresets.Location = new System.Drawing.Point(186, 208);
			this.comboBoxPresets.Name = "comboBoxPresets";
			this.comboBoxPresets.Size = new System.Drawing.Size(172, 21);
			this.comboBoxPresets.TabIndex = 134;
			this.comboBoxPresets.SelectedIndexChanged += new System.EventHandler(this.comboBoxPresets_SelectedIndexChanged);
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(194, 184);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(156, 20);
			this.label14.TabIndex = 133;
			this.label14.Text = "Apply WinAmp (TM) presets";
			// 
			// label13
			// 
			this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label13.Location = new System.Drawing.Point(468, 24);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(48, 16);
			this.label13.TabIndex = 131;
			this.label13.Text = "16 Khz";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// trackBar16000Hz
			// 
			this.trackBar16000Hz.Location = new System.Drawing.Point(468, 48);
			this.trackBar16000Hz.Maximum = 1500;
			this.trackBar16000Hz.Minimum = -1500;
			this.trackBar16000Hz.Name = "trackBar16000Hz";
			this.trackBar16000Hz.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar16000Hz.Size = new System.Drawing.Size(45, 128);
			this.trackBar16000Hz.TabIndex = 132;
			this.trackBar16000Hz.TickFrequency = 150;
			this.trackBar16000Hz.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar16000Hz.Scroll += new System.EventHandler(this.trackBar16000Hz_Scroll);
			// 
			// label12
			// 
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label12.Location = new System.Drawing.Point(420, 24);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(48, 16);
			this.label12.TabIndex = 129;
			this.label12.Text = "14 Khz";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// trackBar14000Hz
			// 
			this.trackBar14000Hz.Location = new System.Drawing.Point(420, 48);
			this.trackBar14000Hz.Maximum = 1500;
			this.trackBar14000Hz.Minimum = -1500;
			this.trackBar14000Hz.Name = "trackBar14000Hz";
			this.trackBar14000Hz.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar14000Hz.Size = new System.Drawing.Size(45, 128);
			this.trackBar14000Hz.TabIndex = 130;
			this.trackBar14000Hz.TickFrequency = 150;
			this.trackBar14000Hz.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar14000Hz.Scroll += new System.EventHandler(this.trackBar14000Hz_Scroll);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(20, 152);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(8, 16);
			this.label10.TabIndex = 128;
			this.label10.Text = "_";
			this.label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(20, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(8, 16);
			this.label2.TabIndex = 127;
			this.label2.Text = "+";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label9.Location = new System.Drawing.Point(372, 24);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(48, 16);
			this.label9.TabIndex = 119;
			this.label9.Text = "12 Khz";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label8.Location = new System.Drawing.Point(324, 24);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(48, 16);
			this.label8.TabIndex = 118;
			this.label8.Text = "6 Khz";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label7.Location = new System.Drawing.Point(276, 24);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(48, 16);
			this.label7.TabIndex = 117;
			this.label7.Text = "3 Khz";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label6.Location = new System.Drawing.Point(228, 24);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(48, 16);
			this.label6.TabIndex = 116;
			this.label6.Text = "1 Khz";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label5.Location = new System.Drawing.Point(180, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(48, 16);
			this.label5.TabIndex = 115;
			this.label5.Text = "600 Hz";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label4.Location = new System.Drawing.Point(132, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(48, 16);
			this.label4.TabIndex = 114;
			this.label4.Text = "310 Hz";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(84, 24);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 16);
			this.label3.TabIndex = 113;
			this.label3.Text = "170 Hz";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// trackBar12000Hz
			// 
			this.trackBar12000Hz.Location = new System.Drawing.Point(372, 48);
			this.trackBar12000Hz.Maximum = 1500;
			this.trackBar12000Hz.Minimum = -1500;
			this.trackBar12000Hz.Name = "trackBar12000Hz";
			this.trackBar12000Hz.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar12000Hz.Size = new System.Drawing.Size(45, 128);
			this.trackBar12000Hz.TabIndex = 126;
			this.trackBar12000Hz.TickFrequency = 150;
			this.trackBar12000Hz.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar12000Hz.Scroll += new System.EventHandler(this.trackBar12000Hz_Scroll);
			// 
			// trackBar6000Hz
			// 
			this.trackBar6000Hz.Location = new System.Drawing.Point(324, 48);
			this.trackBar6000Hz.Maximum = 1500;
			this.trackBar6000Hz.Minimum = -1500;
			this.trackBar6000Hz.Name = "trackBar6000Hz";
			this.trackBar6000Hz.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar6000Hz.Size = new System.Drawing.Size(45, 128);
			this.trackBar6000Hz.TabIndex = 125;
			this.trackBar6000Hz.TickFrequency = 150;
			this.trackBar6000Hz.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar6000Hz.Scroll += new System.EventHandler(this.trackBar6000Hz_Scroll);
			// 
			// trackBar3000Hz
			// 
			this.trackBar3000Hz.Location = new System.Drawing.Point(276, 48);
			this.trackBar3000Hz.Maximum = 1500;
			this.trackBar3000Hz.Minimum = -1500;
			this.trackBar3000Hz.Name = "trackBar3000Hz";
			this.trackBar3000Hz.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar3000Hz.Size = new System.Drawing.Size(45, 128);
			this.trackBar3000Hz.TabIndex = 124;
			this.trackBar3000Hz.TickFrequency = 150;
			this.trackBar3000Hz.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar3000Hz.Scroll += new System.EventHandler(this.trackBar3000Hz_Scroll);
			// 
			// trackBar1000Hz
			// 
			this.trackBar1000Hz.Location = new System.Drawing.Point(228, 48);
			this.trackBar1000Hz.Maximum = 1500;
			this.trackBar1000Hz.Minimum = -1500;
			this.trackBar1000Hz.Name = "trackBar1000Hz";
			this.trackBar1000Hz.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar1000Hz.Size = new System.Drawing.Size(45, 128);
			this.trackBar1000Hz.TabIndex = 123;
			this.trackBar1000Hz.TickFrequency = 150;
			this.trackBar1000Hz.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar1000Hz.Scroll += new System.EventHandler(this.trackBar1000Hz_Scroll);
			// 
			// trackBar600Hz
			// 
			this.trackBar600Hz.Location = new System.Drawing.Point(180, 48);
			this.trackBar600Hz.Maximum = 1500;
			this.trackBar600Hz.Minimum = -1500;
			this.trackBar600Hz.Name = "trackBar600Hz";
			this.trackBar600Hz.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar600Hz.Size = new System.Drawing.Size(45, 128);
			this.trackBar600Hz.TabIndex = 122;
			this.trackBar600Hz.TickFrequency = 150;
			this.trackBar600Hz.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar600Hz.Scroll += new System.EventHandler(this.trackBar600Hz_Scroll);
			// 
			// trackBar310Hz
			// 
			this.trackBar310Hz.Location = new System.Drawing.Point(132, 48);
			this.trackBar310Hz.Maximum = 1500;
			this.trackBar310Hz.Minimum = -1500;
			this.trackBar310Hz.Name = "trackBar310Hz";
			this.trackBar310Hz.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar310Hz.Size = new System.Drawing.Size(45, 128);
			this.trackBar310Hz.TabIndex = 121;
			this.trackBar310Hz.TickFrequency = 150;
			this.trackBar310Hz.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar310Hz.Scroll += new System.EventHandler(this.trackBar310Hz_Scroll);
			// 
			// trackBar170Hz
			// 
			this.trackBar170Hz.Location = new System.Drawing.Point(84, 48);
			this.trackBar170Hz.Maximum = 1500;
			this.trackBar170Hz.Minimum = -1500;
			this.trackBar170Hz.Name = "trackBar170Hz";
			this.trackBar170Hz.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar170Hz.Size = new System.Drawing.Size(45, 128);
			this.trackBar170Hz.TabIndex = 120;
			this.trackBar170Hz.TickFrequency = 150;
			this.trackBar170Hz.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar170Hz.Scroll += new System.EventHandler(this.trackBar170Hz_Scroll);
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label11.Location = new System.Drawing.Point(36, 24);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(48, 16);
			this.label11.TabIndex = 112;
			this.label11.Text = "80 Hz";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// trackBar80Hz
			// 
			this.trackBar80Hz.Location = new System.Drawing.Point(36, 48);
			this.trackBar80Hz.Maximum = 1500;
			this.trackBar80Hz.Minimum = -1500;
			this.trackBar80Hz.Name = "trackBar80Hz";
			this.trackBar80Hz.Orientation = System.Windows.Forms.Orientation.Vertical;
			this.trackBar80Hz.Size = new System.Drawing.Size(45, 128);
			this.trackBar80Hz.TabIndex = 111;
			this.trackBar80Hz.Tag = "";
			this.trackBar80Hz.TickFrequency = 150;
			this.trackBar80Hz.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.trackBar80Hz.Scroll += new System.EventHandler(this.trackBar80Hz_Scroll);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(288, 248);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(104, 24);
			this.buttonCancel.TabIndex = 136;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(160, 248);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(96, 24);
			this.buttonOK.TabIndex = 135;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// FormEqualizer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(544, 294);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.comboBoxPresets);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.trackBar16000Hz);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.trackBar14000Hz);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.trackBar12000Hz);
			this.Controls.Add(this.trackBar6000Hz);
			this.Controls.Add(this.trackBar3000Hz);
			this.Controls.Add(this.trackBar1000Hz);
			this.Controls.Add(this.trackBar600Hz);
			this.Controls.Add(this.trackBar310Hz);
			this.Controls.Add(this.trackBar170Hz);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.trackBar80Hz);
			this.Name = "FormEqualizer";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Equalizer settings";
			this.Load += new System.EventHandler(this.FormEqualizer_Load);
			((System.ComponentModel.ISupportInitialize)(this.trackBar16000Hz)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar14000Hz)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar12000Hz)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar6000Hz)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar3000Hz)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar1000Hz)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar600Hz)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar310Hz)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar170Hz)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBar80Hz)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void ResetEqualizerBands ()
		{
			Int32	nBands = audioSoundEditor1.Effects.EqualizerBandGetCount ();
			for (Int16 i = 0; i < nBands; i++)
			{
				float	fFrequency = audioSoundEditor1.Effects.EqualizerBandGetFrequency (i);
				float	fBandwidth = 0.0f;
				float	fGain = 0.0f;
				audioSoundEditor1.Effects.EqualizerBandGetParams (fFrequency, ref fBandwidth, ref fGain);
				audioSoundEditor1.Effects.EqualizerBandSetParams (fFrequency, fBandwidth, 0.0f);
			}

			trackBar80Hz.Value = 0;
			trackBar170Hz.Value = 0;
			trackBar310Hz.Value = 0;
			trackBar600Hz.Value = 0;
			trackBar1000Hz.Value = 0;
			trackBar3000Hz.Value = 0;
			trackBar6000Hz.Value = 0;
			trackBar12000Hz.Value = 0;
			trackBar14000Hz.Value = 0;
			trackBar16000Hz.Value = 0;
		}

		private void UpdateBandsValues ()
		{
			// update the gain for each band		        
			Int32	nBands = audioSoundEditor1.Effects.EqualizerBandGetCount ();
			for (Int16 i = 0; i < nBands; i++)
			{
				float	fBandwidth = 0.0f;
				float	fGain = 0.0f;
				float	fFrequency = audioSoundEditor1.Effects.EqualizerBandGetFrequency (i);

				// get settings for the specific band, we are interested in knowing the actual "gain"
				audioSoundEditor1.Effects.EqualizerBandGetParams (fFrequency, ref fBandwidth, ref fGain);
				switch (i)
				{
					case 0:	trackBar80Hz.Value = (Int16) fGain * 100;		break;
					case 1:	trackBar170Hz.Value = (Int16) fGain * 100;		break;
					case 2:	trackBar310Hz.Value = (Int16) fGain * 100;		break;
					case 3:	trackBar600Hz.Value = (Int16) fGain * 100;		break;
					case 4:	trackBar1000Hz.Value = (Int16) fGain * 100;		break;
					case 5:	trackBar3000Hz.Value = (Int16) fGain * 100;		break;
					case 6:	trackBar6000Hz.Value = (Int16) fGain * 100;		break;
					case 7:	trackBar12000Hz.Value = (Int16) fGain * 100;	break;
					case 8:	trackBar14000Hz.Value = (Int16) fGain * 100;	break;
					case 9:	trackBar16000Hz.Value = (Int16) fGain * 100;	break;
				}		    
			}
		}

		private void FormEqualizer_Load(object sender, System.EventArgs e)
		{
			// fill the combo box with the available presets
			comboBoxPresets.Items.Add ("None");
			comboBoxPresets.Items.Add ("Classical");
			comboBoxPresets.Items.Add ("Club");
			comboBoxPresets.Items.Add ("Dance");
			comboBoxPresets.Items.Add ("Full Bass");
			comboBoxPresets.Items.Add ("Full Bass Treble");
			comboBoxPresets.Items.Add ("Full Treble");
			comboBoxPresets.Items.Add ("Laptop Speakers");
			comboBoxPresets.Items.Add ("Large Hall");
			comboBoxPresets.Items.Add ("Live");
			comboBoxPresets.Items.Add ("Party");
			comboBoxPresets.Items.Add ("Pop");
			comboBoxPresets.Items.Add ("Reggae");
			comboBoxPresets.Items.Add ("Rock");
			comboBoxPresets.Items.Add ("Ska");
			comboBoxPresets.Items.Add ("Soft");
			comboBoxPresets.Items.Add ("Soft Rock");
			comboBoxPresets.Items.Add ("Techno");

			comboBoxPresets.SelectedIndex = 0;		

			// hide sliders not useful for certain sample rates
			Int32	nSampleRate = audioSoundEditor1.GetFrequency();
			if (nSampleRate <= 11025)
			{
				trackBar6000Hz.Visible = false;
				trackBar12000Hz.Visible = false;
				trackBar14000Hz.Visible = false;
				trackBar16000Hz.Visible = false;
			}
			else if (nSampleRate <= 22050)
			{
				trackBar12000Hz.Visible = false;
				trackBar14000Hz.Visible = false;
				trackBar16000Hz.Visible = false;
			}
			else if (nSampleRate <= 44100)
				trackBar16000Hz.Visible = false;
		    
			// check if equalizer bands have already been created
			Int32	nBands = audioSoundEditor1.Effects.EqualizerBandGetCount ();
			if (nBands == 0)
			{
				audioSoundEditor1.Effects.EqualizerBandAdd (80, 12, 0);
				audioSoundEditor1.Effects.EqualizerBandAdd (170, 12, 0);
				audioSoundEditor1.Effects.EqualizerBandAdd (310, 12, 0);
				audioSoundEditor1.Effects.EqualizerBandAdd (600, 12, 0);
				audioSoundEditor1.Effects.EqualizerBandAdd (1000, 12, 0);
				audioSoundEditor1.Effects.EqualizerBandAdd (3000, 12, 0);
				audioSoundEditor1.Effects.EqualizerBandAdd (6000, 12, 0);
				audioSoundEditor1.Effects.EqualizerBandAdd (12000, 12, 0);
				audioSoundEditor1.Effects.EqualizerBandAdd (14000, 12, 0);
				audioSoundEditor1.Effects.EqualizerBandAdd (16000, 12, 0);
			}
			else
			{
				// update the gain for each band
				UpdateBandsValues ();
			}
		}

		private void comboBoxPresets_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (m_bOpeningEqualizer)
			{
				// ignore the selection made inside the Form_Load function
				m_bOpeningEqualizer = false;
				return;
			}

			if (comboBoxPresets.SelectedIndex == 0)
			{
				// reset equalizer values and exit
				ResetEqualizerBands ();
				return;
			}
		    
			// load preset keeping in mind that the first element is 'None' so we have to decrease the value
			audioSoundEditor1.Effects.EqualizerLoadPresets ((enumEqualizerPresets) (comboBoxPresets.SelectedIndex - 1));
		    
			// update the gain for each band
			UpdateBandsValues ();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			m_bCancel = false;
			Hide ();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			m_bCancel = true;
			Hide ();
		}

		private void trackBar80Hz_Scroll(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.EqualizerBandSetGain (80, ((float) trackBar80Hz.Value) / 100.0f);
		}

		private void trackBar170Hz_Scroll(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.EqualizerBandSetGain (170, ((float) trackBar170Hz.Value) / 100.0f);
		}

		private void trackBar310Hz_Scroll(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.EqualizerBandSetGain (310, ((float) trackBar310Hz.Value) / 100.0f);
		}

		private void trackBar600Hz_Scroll(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.EqualizerBandSetGain (600, ((float) trackBar600Hz.Value) / 100.0f);
		}

		private void trackBar1000Hz_Scroll(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.EqualizerBandSetGain (1000, ((float) trackBar1000Hz.Value) / 100.0f);
		}

		private void trackBar3000Hz_Scroll(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.EqualizerBandSetGain (3000, ((float) trackBar3000Hz.Value) / 100.0f);
		}

		private void trackBar6000Hz_Scroll(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.EqualizerBandSetGain (6000, ((float) trackBar6000Hz.Value) / 100.0f);
		}

		private void trackBar12000Hz_Scroll(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.EqualizerBandSetGain (12000, ((float) trackBar12000Hz.Value) / 100.0f);
		}

		private void trackBar14000Hz_Scroll(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.EqualizerBandSetGain (14000, ((float) trackBar14000Hz.Value) / 100.0f);
		}

		private void trackBar16000Hz_Scroll(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.EqualizerBandSetGain (16000, ((float) trackBar16000Hz.Value) / 100.0f);
		}
	}
}
