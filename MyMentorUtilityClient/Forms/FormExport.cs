using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using AudioSoundEditor;

namespace SoundStudio
{
	/// <summary>
	/// Summary description for FormExport.
	/// </summary>
	public class FormExport : System.Windows.Forms.Form
	{
		public System.Windows.Forms.GroupBox Frame2;
		public System.Windows.Forms.GroupBox Frame1;
		public System.Windows.Forms.GroupBox Frame5;
		public System.Windows.Forms.GroupBox Frame4;
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.RadioButton radioButton16bits;
		public System.Windows.Forms.RadioButton radioButton8bits;
		public System.Windows.Forms.RadioButton radioButtonStereo;
		public System.Windows.Forms.RadioButton radioButtonMono;
		public System.Windows.Forms.RadioButton radioButton48000;
		public System.Windows.Forms.RadioButton radioButton44100;
		public System.Windows.Forms.RadioButton radioButton22050;
		public System.Windows.Forms.RadioButton radioButton11025;
		public System.Windows.Forms.RadioButton radioButtonExportSelection;
		public System.Windows.Forms.RadioButton radioButtonExportFull;
		public System.Windows.Forms.Button buttonCancel;
		public System.Windows.Forms.Button buttonOK;
		public System.Windows.Forms.GroupBox FrameResampleSettings;
		private System.Windows.Forms.ComboBox comboBoxFormats;
		public System.Windows.Forms.RadioButton radioButton32bits;
		public System.Windows.Forms.RadioButton radioButton32bitsFloat;
		public System.Windows.Forms.GroupBox FrameBitsPerSample;

		internal AudioSoundEditor.AudioSoundEditor audioSoundEditor1;

		public string	m_strExportPathname;

		protected Int32 m_nBeginSelectionInMs;
		protected Int32 m_nEndSelectionInMs;
		private	Int16	m_ACMCodec;
		private	Int16	m_ACMCodecFormat;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormExport()
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
			this.FrameResampleSettings = new System.Windows.Forms.GroupBox();
			this.FrameBitsPerSample = new System.Windows.Forms.GroupBox();
			this.radioButton16bits = new System.Windows.Forms.RadioButton();
			this.radioButton8bits = new System.Windows.Forms.RadioButton();
			this.Frame2 = new System.Windows.Forms.GroupBox();
			this.radioButtonStereo = new System.Windows.Forms.RadioButton();
			this.radioButtonMono = new System.Windows.Forms.RadioButton();
			this.Frame1 = new System.Windows.Forms.GroupBox();
			this.radioButton48000 = new System.Windows.Forms.RadioButton();
			this.radioButton44100 = new System.Windows.Forms.RadioButton();
			this.radioButton22050 = new System.Windows.Forms.RadioButton();
			this.radioButton11025 = new System.Windows.Forms.RadioButton();
			this.Frame5 = new System.Windows.Forms.GroupBox();
			this.radioButtonExportSelection = new System.Windows.Forms.RadioButton();
			this.radioButtonExportFull = new System.Windows.Forms.RadioButton();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.Frame4 = new System.Windows.Forms.GroupBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.Label1 = new System.Windows.Forms.Label();
			this.comboBoxFormats = new System.Windows.Forms.ComboBox();
			this.radioButton32bits = new System.Windows.Forms.RadioButton();
			this.radioButton32bitsFloat = new System.Windows.Forms.RadioButton();
			this.FrameResampleSettings.SuspendLayout();
			this.FrameBitsPerSample.SuspendLayout();
			this.Frame2.SuspendLayout();
			this.Frame1.SuspendLayout();
			this.Frame5.SuspendLayout();
			this.Frame4.SuspendLayout();
			this.SuspendLayout();
			// 
			// FrameResampleSettings
			// 
			this.FrameResampleSettings.BackColor = System.Drawing.SystemColors.Control;
			this.FrameResampleSettings.Controls.Add(this.FrameBitsPerSample);
			this.FrameResampleSettings.Controls.Add(this.Frame2);
			this.FrameResampleSettings.Controls.Add(this.Frame1);
			this.FrameResampleSettings.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FrameResampleSettings.ForeColor = System.Drawing.SystemColors.ControlText;
			this.FrameResampleSettings.Location = new System.Drawing.Point(21, 148);
			this.FrameResampleSettings.Name = "FrameResampleSettings";
			this.FrameResampleSettings.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.FrameResampleSettings.Size = new System.Drawing.Size(382, 133);
			this.FrameResampleSettings.TabIndex = 17;
			this.FrameResampleSettings.TabStop = false;
			this.FrameResampleSettings.Text = "Resample settings";
			// 
			// FrameBitsPerSample
			// 
			this.FrameBitsPerSample.BackColor = System.Drawing.SystemColors.Control;
			this.FrameBitsPerSample.Controls.Add(this.radioButton32bitsFloat);
			this.FrameBitsPerSample.Controls.Add(this.radioButton32bits);
			this.FrameBitsPerSample.Controls.Add(this.radioButton16bits);
			this.FrameBitsPerSample.Controls.Add(this.radioButton8bits);
			this.FrameBitsPerSample.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FrameBitsPerSample.ForeColor = System.Drawing.SystemColors.ControlText;
			this.FrameBitsPerSample.Location = new System.Drawing.Point(263, 20);
			this.FrameBitsPerSample.Name = "FrameBitsPerSample";
			this.FrameBitsPerSample.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.FrameBitsPerSample.Size = new System.Drawing.Size(105, 101);
			this.FrameBitsPerSample.TabIndex = 19;
			this.FrameBitsPerSample.TabStop = false;
			this.FrameBitsPerSample.Text = "Bits per sample";
			// 
			// radioButton16bits
			// 
			this.radioButton16bits.BackColor = System.Drawing.SystemColors.Control;
			this.radioButton16bits.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButton16bits.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButton16bits.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButton16bits.Location = new System.Drawing.Point(12, 38);
			this.radioButton16bits.Name = "radioButton16bits";
			this.radioButton16bits.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButton16bits.Size = new System.Drawing.Size(77, 13);
			this.radioButton16bits.TabIndex = 21;
			this.radioButton16bits.TabStop = true;
			this.radioButton16bits.Text = "16 bits";
			// 
			// radioButton8bits
			// 
			this.radioButton8bits.BackColor = System.Drawing.SystemColors.Control;
			this.radioButton8bits.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButton8bits.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButton8bits.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButton8bits.Location = new System.Drawing.Point(12, 20);
			this.radioButton8bits.Name = "radioButton8bits";
			this.radioButton8bits.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButton8bits.Size = new System.Drawing.Size(77, 13);
			this.radioButton8bits.TabIndex = 20;
			this.radioButton8bits.TabStop = true;
			this.radioButton8bits.Text = "8 bits";
			// 
			// Frame2
			// 
			this.Frame2.BackColor = System.Drawing.SystemColors.Control;
			this.Frame2.Controls.Add(this.radioButtonStereo);
			this.Frame2.Controls.Add(this.radioButtonMono);
			this.Frame2.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Frame2.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame2.Location = new System.Drawing.Point(141, 20);
			this.Frame2.Name = "Frame2";
			this.Frame2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame2.Size = new System.Drawing.Size(100, 101);
			this.Frame2.TabIndex = 16;
			this.Frame2.TabStop = false;
			this.Frame2.Text = "Channels";
			// 
			// radioButtonStereo
			// 
			this.radioButtonStereo.BackColor = System.Drawing.SystemColors.Control;
			this.radioButtonStereo.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButtonStereo.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButtonStereo.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButtonStereo.Location = new System.Drawing.Point(8, 20);
			this.radioButtonStereo.Name = "radioButtonStereo";
			this.radioButtonStereo.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButtonStereo.Size = new System.Drawing.Size(77, 17);
			this.radioButtonStereo.TabIndex = 18;
			this.radioButtonStereo.TabStop = true;
			this.radioButtonStereo.Text = "Stereo";
			// 
			// radioButtonMono
			// 
			this.radioButtonMono.BackColor = System.Drawing.SystemColors.Control;
			this.radioButtonMono.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButtonMono.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButtonMono.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButtonMono.Location = new System.Drawing.Point(8, 38);
			this.radioButtonMono.Name = "radioButtonMono";
			this.radioButtonMono.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButtonMono.Size = new System.Drawing.Size(69, 17);
			this.radioButtonMono.TabIndex = 17;
			this.radioButtonMono.TabStop = true;
			this.radioButtonMono.Text = "Mono";
			// 
			// Frame1
			// 
			this.Frame1.BackColor = System.Drawing.SystemColors.Control;
			this.Frame1.Controls.Add(this.radioButton48000);
			this.Frame1.Controls.Add(this.radioButton44100);
			this.Frame1.Controls.Add(this.radioButton22050);
			this.Frame1.Controls.Add(this.radioButton11025);
			this.Frame1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Frame1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame1.Location = new System.Drawing.Point(19, 20);
			this.Frame1.Name = "Frame1";
			this.Frame1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame1.Size = new System.Drawing.Size(100, 101);
			this.Frame1.TabIndex = 12;
			this.Frame1.TabStop = false;
			this.Frame1.Text = "Frequencies";
			// 
			// radioButton48000
			// 
			this.radioButton48000.BackColor = System.Drawing.SystemColors.Control;
			this.radioButton48000.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButton48000.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButton48000.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButton48000.Location = new System.Drawing.Point(8, 20);
			this.radioButton48000.Name = "radioButton48000";
			this.radioButton48000.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButton48000.Size = new System.Drawing.Size(53, 17);
			this.radioButton48000.TabIndex = 22;
			this.radioButton48000.TabStop = true;
			this.radioButton48000.Text = "48000";
			// 
			// radioButton44100
			// 
			this.radioButton44100.BackColor = System.Drawing.SystemColors.Control;
			this.radioButton44100.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButton44100.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButton44100.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButton44100.Location = new System.Drawing.Point(8, 38);
			this.radioButton44100.Name = "radioButton44100";
			this.radioButton44100.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButton44100.Size = new System.Drawing.Size(61, 17);
			this.radioButton44100.TabIndex = 15;
			this.radioButton44100.TabStop = true;
			this.radioButton44100.Text = "44100";
			// 
			// radioButton22050
			// 
			this.radioButton22050.BackColor = System.Drawing.SystemColors.Control;
			this.radioButton22050.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButton22050.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButton22050.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButton22050.Location = new System.Drawing.Point(8, 55);
			this.radioButton22050.Name = "radioButton22050";
			this.radioButton22050.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButton22050.Size = new System.Drawing.Size(61, 17);
			this.radioButton22050.TabIndex = 14;
			this.radioButton22050.TabStop = true;
			this.radioButton22050.Text = "22050";
			// 
			// radioButton11025
			// 
			this.radioButton11025.BackColor = System.Drawing.SystemColors.Control;
			this.radioButton11025.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButton11025.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButton11025.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButton11025.Location = new System.Drawing.Point(8, 72);
			this.radioButton11025.Name = "radioButton11025";
			this.radioButton11025.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButton11025.Size = new System.Drawing.Size(61, 17);
			this.radioButton11025.TabIndex = 13;
			this.radioButton11025.TabStop = true;
			this.radioButton11025.Text = "11025";
			// 
			// Frame5
			// 
			this.Frame5.BackColor = System.Drawing.SystemColors.Control;
			this.Frame5.Controls.Add(this.radioButtonExportSelection);
			this.Frame5.Controls.Add(this.radioButtonExportFull);
			this.Frame5.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Frame5.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame5.Location = new System.Drawing.Point(21, 60);
			this.Frame5.Name = "Frame5";
			this.Frame5.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame5.Size = new System.Drawing.Size(161, 84);
			this.Frame5.TabIndex = 16;
			this.Frame5.TabStop = false;
			this.Frame5.Text = "Export range";
			// 
			// radioButtonExportSelection
			// 
			this.radioButtonExportSelection.BackColor = System.Drawing.SystemColors.Control;
			this.radioButtonExportSelection.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButtonExportSelection.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButtonExportSelection.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButtonExportSelection.Location = new System.Drawing.Point(8, 44);
			this.radioButtonExportSelection.Name = "radioButtonExportSelection";
			this.radioButtonExportSelection.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButtonExportSelection.Size = new System.Drawing.Size(125, 17);
			this.radioButtonExportSelection.TabIndex = 10;
			this.radioButtonExportSelection.TabStop = true;
			this.radioButtonExportSelection.Text = "Selection only";
			// 
			// radioButtonExportFull
			// 
			this.radioButtonExportFull.BackColor = System.Drawing.SystemColors.Control;
			this.radioButtonExportFull.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButtonExportFull.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButtonExportFull.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButtonExportFull.Location = new System.Drawing.Point(8, 24);
			this.radioButtonExportFull.Name = "radioButtonExportFull";
			this.radioButtonExportFull.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButtonExportFull.Size = new System.Drawing.Size(137, 13);
			this.radioButtonExportFull.TabIndex = 9;
			this.radioButtonExportFull.TabStop = true;
			this.radioButtonExportFull.Text = "Full sound";
			// 
			// buttonCancel
			// 
			this.buttonCancel.BackColor = System.Drawing.SystemColors.Control;
			this.buttonCancel.Cursor = System.Windows.Forms.Cursors.Default;
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.buttonCancel.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonCancel.Location = new System.Drawing.Point(222, 292);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.buttonCancel.Size = new System.Drawing.Size(117, 25);
			this.buttonCancel.TabIndex = 15;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// Frame4
			// 
			this.Frame4.BackColor = System.Drawing.SystemColors.Control;
			this.Frame4.Controls.Add(this.comboBoxFormats);
			this.Frame4.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Frame4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Frame4.Location = new System.Drawing.Point(189, 60);
			this.Frame4.Name = "Frame4";
			this.Frame4.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Frame4.Size = new System.Drawing.Size(214, 84);
			this.Frame4.TabIndex = 14;
			this.Frame4.TabStop = false;
			this.Frame4.Text = "Output format";
			// 
			// buttonOK
			// 
			this.buttonOK.BackColor = System.Drawing.SystemColors.Control;
			this.buttonOK.Cursor = System.Windows.Forms.Cursors.Default;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.buttonOK.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonOK.Location = new System.Drawing.Point(86, 292);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.buttonOK.Size = new System.Drawing.Size(117, 25);
			this.buttonOK.TabIndex = 12;
			this.buttonOK.Text = "Export start";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// Label1
			// 
			this.Label1.BackColor = System.Drawing.SystemColors.Control;
			this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
			this.Label1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.Label1.Location = new System.Drawing.Point(8, 8);
			this.Label1.Name = "Label1";
			this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Label1.Size = new System.Drawing.Size(409, 45);
			this.Label1.TabIndex = 13;
			this.Label1.Text = "By default, the sound will be resampled using the given settings, then it will be" +
				" exported into EditorTest.xxx where \'xxx\' will be replaced by the default ext" +
				"ension of the output format of choice, like \'wav\' for WAV format or \'mp3\' for MP" +
				"3 format.";
			this.Label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// comboBoxFormats
			// 
			this.comboBoxFormats.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxFormats.Location = new System.Drawing.Point(8, 32);
			this.comboBoxFormats.Name = "comboBoxFormats";
			this.comboBoxFormats.Size = new System.Drawing.Size(196, 22);
			this.comboBoxFormats.TabIndex = 0;
			this.comboBoxFormats.SelectedIndexChanged += new System.EventHandler(this.comboBoxFormats_SelectedIndexChanged);
			// 
			// radioButton32bits
			// 
			this.radioButton32bits.BackColor = System.Drawing.SystemColors.Control;
			this.radioButton32bits.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButton32bits.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButton32bits.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButton32bits.Location = new System.Drawing.Point(12, 55);
			this.radioButton32bits.Name = "radioButton32bits";
			this.radioButton32bits.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButton32bits.Size = new System.Drawing.Size(77, 13);
			this.radioButton32bits.TabIndex = 22;
			this.radioButton32bits.TabStop = true;
			this.radioButton32bits.Text = "32 bits";
			// 
			// radioButton32bitsFloat
			// 
			this.radioButton32bitsFloat.BackColor = System.Drawing.SystemColors.Control;
			this.radioButton32bitsFloat.Cursor = System.Windows.Forms.Cursors.Default;
			this.radioButton32bitsFloat.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.radioButton32bitsFloat.ForeColor = System.Drawing.SystemColors.ControlText;
			this.radioButton32bitsFloat.Location = new System.Drawing.Point(12, 72);
			this.radioButton32bitsFloat.Name = "radioButton32bitsFloat";
			this.radioButton32bitsFloat.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.radioButton32bitsFloat.Size = new System.Drawing.Size(80, 13);
			this.radioButton32bitsFloat.TabIndex = 23;
			this.radioButton32bitsFloat.TabStop = true;
			this.radioButton32bitsFloat.Text = "32 bits float";
			// 
			// FormExport
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(424, 329);
			this.ControlBox = false;
			this.Controls.Add(this.FrameResampleSettings);
			this.Controls.Add(this.Frame5);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.Frame4);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.Label1);
			this.Name = "FormExport";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Export recorded sound to file";
			this.Load += new System.EventHandler(this.FormExport_Load);
			this.FrameResampleSettings.ResumeLayout(false);
			this.FrameBitsPerSample.ResumeLayout(false);
			this.Frame2.ResumeLayout(false);
			this.Frame1.ResumeLayout(false);
			this.Frame5.ResumeLayout(false);
			this.Frame4.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormExport_Load(object sender, System.EventArgs e)
		{
			// set default values
			radioButton44100.Checked = true;
			radioButtonStereo.Checked = true;
			radioButton16bits.Checked = true;
		    
			// get the position selected on the waveform analyzer, if any
			bool	bSelectionAvailable = false;
			audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection (ref bSelectionAvailable, ref m_nBeginSelectionInMs, ref m_nEndSelectionInMs);

			// if a selection is available
			if (bSelectionAvailable)
			{
				radioButtonExportFull.Checked = false;
				radioButtonExportSelection.Enabled = true;
				radioButtonExportSelection.Checked = true;
			}
			else
			{
				radioButtonExportFull.Checked = true;
				radioButtonExportSelection.Enabled = false;
				radioButtonExportSelection.Checked = false;
				m_nBeginSelectionInMs = 0;
				m_nEndSelectionInMs = -1;
			}

			// add some of the supported output formats
			comboBoxFormats.Items.Add ("Microsoft WAV");
			comboBoxFormats.Items.Add ("MP3");
			comboBoxFormats.Items.Add ("OGG Vorbis");
			comboBoxFormats.Items.Add ("Windows Media Audio (WMA)");
			comboBoxFormats.Items.Add ("AAC/MP4");
			comboBoxFormats.Items.Add ("Audio Compression Manager codec");
			comboBoxFormats.Items.Add ("Apple/SGI AIFF");
			comboBoxFormats.Items.Add ("Sun/NeXT AU");
			comboBoxFormats.SelectedIndex = 0;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			Int32	nBeginSelectionInMs;
			Int32	nEndSelectionInMs;
			if (radioButtonExportFull.Checked)
			{
				nBeginSelectionInMs = 0;
				nEndSelectionInMs = -1;
			}
			else
			{
				nBeginSelectionInMs = m_nBeginSelectionInMs;
				nEndSelectionInMs = m_nEndSelectionInMs;
			}

			Int32	nResampleFrequency = 44100;
			Int32	nResampleChannels = 2;
			
			// get the actual selected settings
			if (radioButton48000.Checked)
				nResampleFrequency = 48000;
			if (radioButton44100.Checked)
				nResampleFrequency = 44100;
			if (radioButton22050.Checked)
				nResampleFrequency = 22050;
			if (radioButton11025.Checked)
				nResampleFrequency = 11025;

			if (radioButtonStereo.Checked)
				nResampleChannels = 2;
			if (radioButtonMono.Checked)
				nResampleChannels = 1;

			// compose the outputh pathname
			string	strPathname = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal) + @"\EditorTest";
			switch (comboBoxFormats.SelectedIndex)
			{
			case 0:
				audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_WAV;
				if (radioButton8bits.Checked)
					audioSoundEditor1.EncodeFormats.WAV.EncodeMode = enumWavEncodeModes.WAV_ENCODE_PCM_U8;
				else if (radioButton16bits.Checked)
					audioSoundEditor1.EncodeFormats.WAV.EncodeMode = enumWavEncodeModes.WAV_ENCODE_PCM_S16;
				else if (radioButton32bits.Checked)
					audioSoundEditor1.EncodeFormats.WAV.EncodeMode = enumWavEncodeModes.WAV_ENCODE_PCM_S32;
				else
					audioSoundEditor1.EncodeFormats.WAV.EncodeMode = enumWavEncodeModes.WAV_ENCODE_FLOAT32;
				strPathname += ".wav";
				break;
			case 1:
				audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_MP3;
				audioSoundEditor1.EncodeFormats.MP3.EncodeMode = enumMp3EncodeModes.MP3_ENCODE_PRESETS;
				audioSoundEditor1.EncodeFormats.MP3.Preset = enumMp3EncodePresets.MP3_PRESET_STANDARD;
				strPathname += ".mp3";
				break;
			case 2:
				audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_OGG;
				audioSoundEditor1.EncodeFormats.OGG.EncodeMode = enumOggEncodeModes.OGG_ENCODE_QUALITY;
				audioSoundEditor1.EncodeFormats.OGG.Quality = 5;
				strPathname += ".ogg";
				break;
			case 3:
				audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_WMA;
				audioSoundEditor1.EncodeFormats.WMA.EncodeMode = enumWmaEncodeModes.WMA_ENCODE_VBR_QUALITY;
				audioSoundEditor1.EncodeFormats.WMA.Quality = 50;
				strPathname += ".wma";
				break;
			case 4:
				audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_AAC;
				audioSoundEditor1.EncodeFormats.AAC.EncodeMode = enumAacEncodeModes.AAC_ENCODE_VBR_QUALITY;
				audioSoundEditor1.EncodeFormats.AAC.Quality = 100;
				strPathname += ".aac";
				break;
			case 5:
				audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_ACM;
				audioSoundEditor1.EncodeFormats.ACM.EncodeMode = enumAcmEncodeModes.ACM_ENCODE_USE_CODEC_INDEX;
				audioSoundEditor1.EncodeFormats.ACM.CodecIndex = m_ACMCodec;
				audioSoundEditor1.EncodeFormats.ACM.CodecFormatIndex = m_ACMCodecFormat;
				strPathname += ".wav";
				break;
			case 6:
				audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_AIFF;
				if (radioButton8bits.Checked)
					audioSoundEditor1.EncodeFormats.AIFF.EncodeMode = enumAIFFEncodeModes.AIFF_ENCODE_PCM_U8;
				else if (radioButton16bits.Checked)
					audioSoundEditor1.EncodeFormats.AIFF.EncodeMode = enumAIFFEncodeModes.AIFF_ENCODE_PCM_S16;
				else if (radioButton32bits.Checked)
					audioSoundEditor1.EncodeFormats.AIFF.EncodeMode = enumAIFFEncodeModes.AIFF_ENCODE_PCM_S32;
				else
					audioSoundEditor1.EncodeFormats.AIFF.EncodeMode = enumAIFFEncodeModes.AIFF_ENCODE_FLOAT32;
				strPathname += ".aiff";
				break;
			case 7:
				audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_AU;
				if (radioButton8bits.Checked)
					audioSoundEditor1.EncodeFormats.AU.EncodeMode = enumAUEncodeModes.AU_ENCODE_PCM_S8;
				else if (radioButton16bits.Checked)
					audioSoundEditor1.EncodeFormats.AU.EncodeMode = enumAUEncodeModes.AU_ENCODE_PCM_S16;
				else if (radioButton32bits.Checked)
					audioSoundEditor1.EncodeFormats.AU.EncodeMode = enumAUEncodeModes.AU_ENCODE_PCM_S32;
				else
					audioSoundEditor1.EncodeFormats.AU.EncodeMode = enumAUEncodeModes.AU_ENCODE_FLOAT32;
				strPathname += ".au";
				break;
			}

			// resample and export sound to the given file
			m_strExportPathname = strPathname;
			audioSoundEditor1.ExportToFile (nResampleFrequency, nResampleChannels,
				0, nBeginSelectionInMs, nEndSelectionInMs, strPathname);
		                                            
			// we can close the form: when the export session will be completed
			// the SoundExportDone event will be fired
			Close ();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			m_strExportPathname = "";
			Close ();		
		}

		private void comboBoxFormats_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch (comboBoxFormats.SelectedIndex)
			{
			case 0:
				// WAV
				radioButton48000.Enabled = true;
				radioButton22050.Enabled = true;
				radioButton11025.Enabled = true;
				radioButton8bits.Enabled = true;
				radioButtonMono.Enabled = true;
				FrameResampleSettings.Visible = true;
				FrameBitsPerSample.Visible = true;
				break;
			case 1:
				// MP3
				radioButton48000.Enabled = true;
				radioButton22050.Enabled = true;
				radioButton11025.Enabled = true;
				radioButton8bits.Enabled = true;
				radioButtonMono.Enabled = true;
				FrameResampleSettings.Visible = true;
				FrameBitsPerSample.Visible = false;
				break;
			case 2:
				// OGG
				radioButton48000.Enabled = true;
				radioButton22050.Enabled = true;
				radioButton11025.Enabled = true;
				radioButton8bits.Enabled = true;
				radioButtonMono.Enabled = true;
				FrameResampleSettings.Visible = true;
				FrameBitsPerSample.Visible = false;
				break;
			case 3:
				// WMA
				// disable frequencies not supported by actual WMA settings
				radioButton48000.Enabled = false;
				radioButton22050.Enabled = false;
				radioButton11025.Enabled = false;
				radioButton8bits.Enabled = false;
				radioButtonMono.Enabled = false;
		
				radioButton44100.Checked = true;
				radioButtonStereo.Checked = true;
				radioButton16bits.Checked = true;
				FrameResampleSettings.Visible = true;
				FrameBitsPerSample.Visible = false;
				break;
			case 4:
				// AAC
				radioButton48000.Enabled = true;
				radioButton22050.Enabled = true;
				radioButton11025.Enabled = true;
				radioButton8bits.Enabled = true;
				FrameResampleSettings.Visible = true;
				FrameBitsPerSample.Visible = false;
				break;

			case 6:
				//AIFF
				radioButton48000.Enabled = true;
				radioButton22050.Enabled = true;
				radioButton11025.Enabled = true;
				radioButton8bits.Enabled = true;
				FrameResampleSettings.Visible = true;
				FrameBitsPerSample.Visible = true;
				break;
			case 7:
				// AU
				radioButton48000.Enabled = true;
				radioButton22050.Enabled = true;
				radioButton11025.Enabled = true;
				radioButton8bits.Enabled = true;
				FrameResampleSettings.Visible = true;
				FrameBitsPerSample.Visible = true;
				break;
			}
		}
	}
}
