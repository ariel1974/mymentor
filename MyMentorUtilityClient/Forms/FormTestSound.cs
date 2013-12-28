using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using AudioSoundRecorder;

namespace MyMentor
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class FormTestSound : System.Windows.Forms.Form
	{
        private AudioSoundRecorder.AudioSoundRecorder audioSoundRecorder1;
        private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ComboBox comboInputChannels;
		private System.Windows.Forms.ComboBox comboInputDevices;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button buttonStartRecording;
		private System.Windows.Forms.Label labelVuMeterRight;
        private System.Windows.Forms.Label labelVuMeterLeft;
        private IContainer components;

		private	string	m_strOutputPathname;
		private	string	m_strOutputExtension;
		private	Int16	m_nCurrInputDevice = 0;
		private	Int16	m_nCurrInputChannel = 0;
		private IntPtr	m_hWndVuMeterLeft;
        private Timer timer1;
        private Button button1;
        private Label label30;
        private TrackBar trackBarVolume1;
        private Button button2;
        private CheckBox checkBox1;
        private Label label2;
        private PictureBox pictureBox2;
        private Label label1;
        private TrackBar trackBar1;
        private Timer timer2;
		private IntPtr	m_hWndVuMeterRight;

		public FormTestSound()
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
				if (components != null) 
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTestSound));
            this.audioSoundRecorder1 = new AudioSoundRecorder.AudioSoundRecorder();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label30 = new System.Windows.Forms.Label();
            this.trackBarVolume1 = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboInputChannels = new System.Windows.Forms.ComboBox();
            this.comboInputDevices = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonStartRecording = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.labelVuMeterRight = new System.Windows.Forms.Label();
            this.labelVuMeterLeft = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume1)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // audioSoundRecorder1
            // 
            this.audioSoundRecorder1.EncodeAacCustomString = null;
            this.audioSoundRecorder1.EncodeAacMode = AudioSoundRecorder.enumAacEncodeModes.AAC_ENCODE_VBR_QUALITY;
            this.audioSoundRecorder1.EncodeAacQuality = 0F;
            this.audioSoundRecorder1.EncodeAacWrapInMP4 = false;
            this.audioSoundRecorder1.EncodeFormatForCdRipping = AudioSoundRecorder.enumEncodingFormats.ENCODING_FORMAT_WAV;
            this.audioSoundRecorder1.EncodeFormatForExporting = AudioSoundRecorder.enumEncodingFormats.ENCODING_FORMAT_WAV;
            this.audioSoundRecorder1.EncodeFormatForRecording = AudioSoundRecorder.enumEncodingFormats.ENCODING_FORMAT_WAV;
            this.audioSoundRecorder1.EncodeMp3ABR = 0;
            this.audioSoundRecorder1.EncodeMp3CBR = 0;
            this.audioSoundRecorder1.EncodeMp3CustomString = null;
            this.audioSoundRecorder1.EncodeMp3Downmix = false;
            this.audioSoundRecorder1.EncodeMp3Mode = AudioSoundRecorder.enumMp3EncodeModes.MP3_ENCODE_PRESETS;
            this.audioSoundRecorder1.EncodeMp3Presets = AudioSoundRecorder.enumMp3EncodePresets.MP3_PRESET_MEDIUM;
            this.audioSoundRecorder1.EncodeOggBitrate = 0;
            this.audioSoundRecorder1.EncodeOggCustomString = null;
            this.audioSoundRecorder1.EncodeOggDownmix = false;
            this.audioSoundRecorder1.EncodeOggMode = AudioSoundRecorder.enumOggEncodeModes.OGG_ENCODE_QUALITY;
            this.audioSoundRecorder1.EncodeOggQuality = 0F;
            this.audioSoundRecorder1.EncodeOggResampleFreq = 0;
            this.audioSoundRecorder1.EncodeWmaCBR = -1;
            this.audioSoundRecorder1.EncodeWmaMode = AudioSoundRecorder.enumWmaEncodeModes.WMA_ENCODE_VBR_QUALITY;
            this.audioSoundRecorder1.EncodeWmaVBRQuality = 100;
            this.audioSoundRecorder1.Location = new System.Drawing.Point(10, -17);
            this.audioSoundRecorder1.Name = "audioSoundRecorder1";
            this.audioSoundRecorder1.SilenceThreshold = ((short)(0));
            this.audioSoundRecorder1.Size = new System.Drawing.Size(48, 48);
            this.audioSoundRecorder1.TabIndex = 0;
            this.audioSoundRecorder1.RecordingStarted += new AudioSoundRecorder.AudioSoundRecorder.EventHandler(this.audioSoundRecorder1_RecordingStarted);
            this.audioSoundRecorder1.RecordingStopped += new AudioSoundRecorder.AudioSoundRecorder.RecordingStoppedEventHandler(this.audioSoundRecorder1_RecordingStopped);
            this.audioSoundRecorder1.RecordingSize += new AudioSoundRecorder.AudioSoundRecorder.RecordingSizeEventHandler(this.audioSoundRecorder1_RecordingSize);
            this.audioSoundRecorder1.RecordingDuration += new AudioSoundRecorder.AudioSoundRecorder.RecordingDurationEventHandler(this.audioSoundRecorder1_RecordingDuration);
            this.audioSoundRecorder1.VUMeterValueChange += new AudioSoundRecorder.AudioSoundRecorder.VUMeterValueChangeEventHandler(this.audioSoundRecorder1_VUMeterValueChange);
            this.audioSoundRecorder1.SoundPlaybackDone += new AudioSoundRecorder.AudioSoundRecorder.EventHandler(this.audioSoundRecorder1_SoundPlaybackDone);
            this.audioSoundRecorder1.SoundUploadDone += new AudioSoundRecorder.AudioSoundRecorder.SoundUploadDoneEventHandler(this.audioSoundRecorder1_SoundUploadDone);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.pictureBox2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.trackBar1);
            this.groupBox2.Controls.Add(this.label30);
            this.groupBox2.Controls.Add(this.trackBarVolume1);
            this.groupBox2.Location = new System.Drawing.Point(343, 76);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(272, 254);
            this.groupBox2.TabIndex = 73;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "הקלטה";
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(6, 103);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(93, 18);
            this.label30.TabIndex = 74;
            this.label30.Text = "עוצמת שמע";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBarVolume1
            // 
            this.trackBarVolume1.Location = new System.Drawing.Point(32, 126);
            this.trackBarVolume1.Maximum = 100;
            this.trackBarVolume1.Name = "trackBarVolume1";
            this.trackBarVolume1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarVolume1.Size = new System.Drawing.Size(45, 112);
            this.trackBarVolume1.TabIndex = 73;
            this.trackBarVolume1.TickFrequency = 10;
            this.trackBarVolume1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarVolume1.Scroll += new System.EventHandler(this.trackBarVolume1_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboInputChannels);
            this.groupBox1.Controls.Add(this.comboInputDevices);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.buttonStartRecording);
            this.groupBox1.Location = new System.Drawing.Point(25, 76);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 254);
            this.groupBox1.TabIndex = 70;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "הגדרות התקני קלט";
            // 
            // comboInputChannels
            // 
            this.comboInputChannels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInputChannels.Location = new System.Drawing.Point(9, 130);
            this.comboInputChannels.Name = "comboInputChannels";
            this.comboInputChannels.Size = new System.Drawing.Size(248, 26);
            this.comboInputChannels.TabIndex = 3;
            this.comboInputChannels.SelectedIndexChanged += new System.EventHandler(this.comboInputChannels_SelectedIndexChanged);
            // 
            // comboInputDevices
            // 
            this.comboInputDevices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboInputDevices.Location = new System.Drawing.Point(9, 61);
            this.comboInputDevices.Name = "comboInputDevices";
            this.comboInputDevices.Size = new System.Drawing.Size(248, 26);
            this.comboInputDevices.TabIndex = 2;
            this.comboInputDevices.SelectedIndexChanged += new System.EventHandler(this.comboInputDevices_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(9, 103);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(248, 28);
            this.label12.TabIndex = 1;
            this.label12.Text = "ערוצי שמע להתקני קלט נבחר";
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(9, 38);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(248, 20);
            this.label11.TabIndex = 0;
            this.label11.Text = " התקני קלט זמינים";
            // 
            // buttonStartRecording
            // 
            this.buttonStartRecording.Location = new System.Drawing.Point(172, 171);
            this.buttonStartRecording.Name = "buttonStartRecording";
            this.buttonStartRecording.Size = new System.Drawing.Size(85, 67);
            this.buttonStartRecording.TabIndex = 71;
            this.buttonStartRecording.Text = "התחל הקלטה";
            this.buttonStartRecording.Click += new System.EventHandler(this.buttonStartRecording_Click);
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(22, 9);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(512, 50);
            this.label10.TabIndex = 69;
            this.label10.Text = "שלום רב ! במסך זה אנחנו נבדוק את התקני השמע המותקנים במחשב זה ע\"י נסיון הקלטה קצר" +
    "ה של 5 שניות והשמעתה מיד.";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelVuMeterRight
            // 
            this.labelVuMeterRight.BackColor = System.Drawing.Color.Black;
            this.labelVuMeterRight.Location = new System.Drawing.Point(321, 79);
            this.labelVuMeterRight.Name = "labelVuMeterRight";
            this.labelVuMeterRight.Size = new System.Drawing.Size(16, 251);
            this.labelVuMeterRight.TabIndex = 82;
            this.labelVuMeterRight.Visible = false;
            // 
            // labelVuMeterLeft
            // 
            this.labelVuMeterLeft.BackColor = System.Drawing.Color.Black;
            this.labelVuMeterLeft.Location = new System.Drawing.Point(305, 79);
            this.labelVuMeterLeft.Name = "labelVuMeterLeft";
            this.labelVuMeterLeft.Size = new System.Drawing.Size(16, 251);
            this.labelVuMeterLeft.TabIndex = 81;
            this.labelVuMeterLeft.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 5000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(450, 339);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(164, 40);
            this.button1.TabIndex = 72;
            this.button1.Text = "הכל בסדר, בוא נמשיך";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(380, 339);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(64, 40);
            this.button2.TabIndex = 83;
            this.button2.Text = "דלג ";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(25, 349);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(276, 22);
            this.checkBox1.TabIndex = 84;
            this.checkBox1.Text = "להבא אל תציג מסך זה בפתיחת המערכת";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(114, 103);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 18);
            this.label1.TabIndex = 76;
            this.label1.Text = "עוצמת הקלטה";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(149, 126);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar1.Size = new System.Drawing.Size(45, 112);
            this.trackBar1.TabIndex = 75;
            this.trackBar1.TickFrequency = 10;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::MyMentor.Properties.Resources._1388144426_arrow_sans_right;
            this.pictureBox2.Location = new System.Drawing.Point(238, 136);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(32, 32);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 78;
            this.pictureBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(81, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(171, 65);
            this.label2.TabIndex = 79;
            this.label2.Text = "מומלץ להגיע למצב שבו הנך נמצא באיזור החץ גם במצב הקלטה וגם במצב נגינה";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 200;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // FormTestSound
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
            this.ClientSize = new System.Drawing.Size(628, 387);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.labelVuMeterRight);
            this.Controls.Add(this.labelVuMeterLeft);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.audioSoundRecorder1);
            this.Font = new System.Drawing.Font("Arial", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormTestSound";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "בדיקת הקלטה";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTestSound_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

	
		private IntPtr CreateVuMeter (Label ctrlPosition)
		{
			// create a new graphic bar
			IntPtr hWnd = audioSoundRecorder1.GraphicBarsManager.Create (this.Handle, ctrlPosition.Left, ctrlPosition.Top,
				ctrlPosition.Width, ctrlPosition.Height);

			// set graphic bar range
			audioSoundRecorder1.GraphicBarsManager.SetRange (hWnd, 0, 32767);

			// disable automatic drop and set vertical orientation
			GRAPHIC_BAR_SETTINGS	settings = new GRAPHIC_BAR_SETTINGS ();
			audioSoundRecorder1.GraphicBarsManager.GetGraphicalSettings (hWnd, ref settings);
			settings.bAutomaticDrop = false;
			settings.nOrientation = enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL;
			audioSoundRecorder1.GraphicBarsManager.SetGraphicalSettings (hWnd, settings);

			return hWnd;
		}

		private void UpdateInputCombos ()
		{
			// list the available input channels for the chosend input device
			comboInputChannels.Items.Clear ();
			Int16	nInputChannels = audioSoundRecorder1.GetInputDeviceChannelsCount(m_nCurrInputDevice);
			for (Int16 i = 0; i < nInputChannels; i++)
			{
				string	strInputChannel = audioSoundRecorder1.GetInputDeviceChannelDesc(m_nCurrInputDevice, i);
				comboInputChannels.Items.Add (strInputChannel);
			}
			// select the actual system default input channel for the chosen input device
			m_nCurrInputChannel = audioSoundRecorder1.GetInputDeviceChannelDefault(m_nCurrInputDevice);
			comboInputChannels.SelectedIndex = m_nCurrInputChannel;
		}


		private void Form1_Load(object sender, System.EventArgs e)
		{
			Int16	nInputDevices = audioSoundRecorder1.GetInputDevicesCount();
			if (nInputDevices == 0)
			{
				MessageBox.Show ("No input device detected and/or connected: the program will now close. Please, try to plug a microphone into the sound card or an external audio device into the Line-In before launching again the program.");
                Application.Exit();
			}
			// init the control
			audioSoundRecorder1.InitRecordingSystem ();

			// list the available input devices
			for (Int16 i = 0; i < nInputDevices; i++)
			{
				string	strInputDevice = audioSoundRecorder1.GetInputDeviceDesc(i);
				comboInputDevices.Items.Add (strInputDevice);
			}
            trackBarVolume1.Value = 100;
            audioSoundRecorder1.EncodeFormats = new EncodeFormatsMan();
            audioSoundRecorder1.EncodeFormats.MP3.EncodeMode = enumMp3EncodeModes.MP3_ENCODE_PRESETS;
            audioSoundRecorder1.EncodeFormats.MP3.Preset = enumMp3EncodePresets.MP3_PRESET_STANDARD;

			// select the actual system default input device
			m_nCurrInputDevice = 0;
			comboInputDevices.SelectedIndex = m_nCurrInputDevice;

			// update the input channels combo and the input formats combo
			UpdateInputCombos ();

			// enable generating VU-Meter events passing 0
			audioSoundRecorder1.DisplayVUMeter.Create (IntPtr.Zero);

			// create some fancy VU-Meter
			m_hWndVuMeterLeft = CreateVuMeter(labelVuMeterLeft);
			m_hWndVuMeterRight = CreateVuMeter(labelVuMeterRight);		
		}

		private void comboInputDevices_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Int16	nCurrInputDevice = (Int16) comboInputDevices.SelectedIndex;
			if (audioSoundRecorder1.VerifyDirectSoundInputDevice(nCurrInputDevice) == enumErrorCodes.ERR_NOERROR)
			{
				m_nCurrInputDevice = (Int16) comboInputDevices.SelectedIndex;

				// list the available input channels for the chosend input device
				UpdateInputCombos ();
			}
			else
			{
				MessageBox.Show ("The selected device is not accessible");
				comboInputDevices.SelectedIndex = m_nCurrInputDevice;
			}				
		}

		private void comboInputChannels_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			m_nCurrInputChannel = (Int16) comboInputChannels.SelectedIndex;
			audioSoundRecorder1.SetInputDeviceChannelDefault(m_nCurrInputDevice, m_nCurrInputChannel);		
		}

		private void buttonStartRecording_Click(object sender, System.EventArgs e)
		{
            timer1.Enabled = true;
            // use custom resampling format 44100 Hz Stereo
			//audioSoundRecorder1.EncodeFormats.ResampleMode = enumResampleModes.RESAMPLE_MODE_NATIVE_FORMAT;
		    
			// start recording session
			enumErrorCodes	nResult = audioSoundRecorder1.StartFromDirectSoundDevice(m_nCurrInputDevice, m_nCurrInputChannel, "");
			if (nResult != enumErrorCodes.ERR_NOERROR)
				MessageBox.Show (string.Format("{0} Cannot start recording: probably a parameter is not compatible with the current resample format", nResult.ToString()));
		}

		private void buttonStopRecording_Click(object sender, System.EventArgs e)
		{
			audioSoundRecorder1.Stop ();
		}

		private void audioSoundRecorder1_VUMeterValueChange(object sender, AudioSoundRecorder.VUMeterValueChangeEventArgs e)
		{
			audioSoundRecorder1.GraphicBarsManager.SetValue (m_hWndVuMeterLeft, e.nPeakLeft);
			audioSoundRecorder1.GraphicBarsManager.SetValue (m_hWndVuMeterRight, e.nPeakRight);		
		}

		private void audioSoundRecorder1_RecordingDuration(object sender, AudioSoundRecorder.RecordingDurationEventArgs e)
		{
			//labelDuration.Text = "Recording duration: " + audioSoundRecorder1.GetFormattedTime (e.nDuration, false, true);
		}

		private void audioSoundRecorder1_RecordingSize(object sender, AudioSoundRecorder.RecordingSizeEventArgs e)
		{
			//labelSize.Text = "Recording size in bytes: " + e.nDataSize.ToString ();
		}

		private void audioSoundRecorder1_RecordingStarted(object sender, System.EventArgs e)
		{
		}

		private void audioSoundRecorder1_RecordingStopped(object sender, AudioSoundRecorder.RecordingStoppedEventArgs e)
		{
		}

		private void buttonPlay_Click(object sender, System.EventArgs e)
		{
				audioSoundRecorder1.RecordedSound.Play ();
		}

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            audioSoundRecorder1.Stop();
            buttonStartRecording.Enabled = false;
            audioSoundRecorder1.OutputVolumeSet((short)trackBarVolume1.Value, enumVolumeScales.SCALE_LINEAR);
            audioSoundRecorder1.RecordedSound.Play();
        }

        private void trackBarVolume1_Scroll(object sender, EventArgs e)
        {
            audioSoundRecorder1.OutputVolumeSet((short)trackBarVolume1.Value, enumVolumeScales.SCALE_LINEAR);

        }

        private void audioSoundRecorder1_SoundUploadDone(object sender, SoundUploadDoneEventArgs e)
        {
            buttonStartRecording.Enabled = true ;

        }

        private void audioSoundRecorder1_SoundPlaybackDone(object sender, EventArgs e)
        {
            buttonStartRecording.Enabled = true;
            button1.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormTestSound_FormClosing(object sender, FormClosingEventArgs e)
        {
            MyMentor.Properties.Settings.Default.TestSound = !checkBox1.Checked;
            MyMentor.Properties.Settings.Default.Save();

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            audioSoundRecorder1.SetInputDeviceChannelVolume(m_nCurrInputDevice, m_nCurrInputChannel, (Int16)trackBar1.Value);

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            // get the current volume level for the selected input channel
            int fCurrVolume = audioSoundRecorder1.GetInputDeviceChannelVolume(m_nCurrInputDevice, m_nCurrInputChannel);
            if (fCurrVolume >= 0)
                trackBar1.Value = fCurrVolume;
            else
                trackBar1.Value = 0;

        }

	}
}
