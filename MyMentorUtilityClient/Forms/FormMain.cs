using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Linq;
using AudioSoundEditor;
using MyMentorUtilityClient;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace SoundStudio
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class FormMain : System.Windows.Forms.Form
    {
        #region Controls
        public System.Windows.Forms.GroupBox Frame4;
        public System.Windows.Forms.Label Label2;
        public System.Windows.Forms.Label Label3;
        public System.Windows.Forms.Label Label4;
        public System.Windows.Forms.Label Label5;
        public System.Windows.Forms.Label Label6;
        public System.Windows.Forms.Label LabelSelectionBegin;
        public System.Windows.Forms.Label LabelSelectionEnd;
        public System.Windows.Forms.Label LabelSelectionDuration;
        public System.Windows.Forms.Label LabelRangeBegin;
        public System.Windows.Forms.Label LabelRangeEnd;
        public System.Windows.Forms.Label LabelRangeDuration;
        public System.Windows.Forms.Label LabelTotalDuration;
        public System.Windows.Forms.Label Label8;
        public System.Windows.Forms.PictureBox Picture1;
        public System.Windows.Forms.Label LabelStatus;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        public System.Windows.Forms.Timer TimerReload;
        public System.Windows.Forms.Timer TimerMenuEnabler;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.IContainer components;

        public System.Windows.Forms.Button buttonPlay;
        public System.Windows.Forms.Button buttonStop;
        public System.Windows.Forms.Button buttonPlaySelection;
        private System.Windows.Forms.Timer timerDisplayWaveform;
        private System.Windows.Forms.Button buttonPause;

        private AudioSoundEditor.AudioSoundEditor audioSoundEditor1;
        public System.Windows.Forms.Button buttonStopRecording;
        public System.Windows.Forms.Button buttonStartRecNew;
        public System.Windows.Forms.Button buttonStartRecAppend;
        private AudioSoundRecorder.AudioSoundRecorder audioSoundRecorder1;
        #endregion

        #region Audio SetUp
        private byte[] m_byteBuffer = null;
        const int VOLUME_FLAT = 0;
        const int VOLUME_SLIDING = 1;

        // calback functions
        private AudioSoundEditor.DSPCallbackFunction addrReverbCallback;
        private AudioSoundEditor.DSPCallbackFunction addrBalanceCallback;

        // unique identifiers for DSPs
        public Int32 m_idDspReverbInternal;
        public Int32 m_idDspBalanceInternal;
        public Int32 m_idDspReverbExternal;
        public Int32 m_idDspBalanceExternal;
        public Int32 m_idDspBassBoostExternal;

        // unique identifiers for VSTs
        public Int32 m_idVstKarmaFxEq;
        public Int32 m_idVstFromFile;

        // Reverb internal DSP variables
        private Int32 BUFFERLEN = 1200;	// length of the reverb's buffer
        private float[] m_buffReverbLeft;	// buffer for left channel reverb
        private float[] m_buffReverbRight;	// buffer for right channel reverb
        private Int32 m_posReverb;		// current position in buffers

        // Balance internal DSP variables
        private Int16 m_nBalancePercentageInternal = 0;

        // Bass boost parameters
        private BASSBOOST_PARAMETERS m_paramBassBoostExternal;
        public System.Windows.Forms.GroupBox framePlayback;
        private System.Windows.Forms.Timer timerRecordingDone;
        public System.Windows.Forms.GroupBox FrameRecording;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        #endregion

        #region More Controls
        private bool m_bRecAppendMode;

        private IntPtr m_hWndVuMeterLeft;
        private IntPtr m_hWndVuMeterRight;
        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem menuFile;
        private ToolStripMenuItem mnuAudio;
        private ToolStripMenuItem mnuAudioSelectedPart;
        private ToolStripMenuItem mnuAudioSelectedPart_Cut;
        private ToolStripMenuItem mnuAudioSelectedPart_Copy;
        private ToolStripMenuItem mnuAudioSelectedPart_Paste;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem mnuAudioSelectedPart_Delete;
        private ToolStripMenuItem mnuAudioSelectedPart_Reduce;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem mnuAudioSelectedPart_SelectAll;
        private ToolStripMenuItem mnuAudioSelectedPart_Remove;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem mnuAudioOptions;
        private ToolStripMenuItem mnuAudioOptions_InsertSilent;
        private ToolStripSeparator toolStripMenuItem7;
        private ToolStripMenuItem mnuAudioOptions_ApplyBackgroundSound;
        private ToolStripSeparator toolStripMenuItem8;
        private ToolStripMenuItem mnuAudioOptions_AppendSoundFile;
        private ToolStripMenuItem mnuAudioOptions_InsertSoundFile;
        private ToolStripSeparator toolStripMenuItem5;
        private ToolStripMenuItem mnuAudioEffects;
        private ToolStripMenuItem mnuAudioEffects_Equalizer;
        private ToolStripMenuItem mnuAudioEffects_Tempo;
        private ToolStripMenuItem mnuAudioEffects_Pitch;
        private ToolStripMenuItem mnuAudioEffects_PlaybackRate;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem mnuZoom;
        private ToolStripMenuItem mnuZoom_Selection;
        private ToolStripMenuItem mnuZoom_AllClip;
        private ToolStripMenuItem mnuZoom_In;
        private ToolStripMenuItem mnuZoom_Out;
        private ToolStripMenuItem mnuAudioSelectedPart_PasteInsertMode;
        private ToolStripMenuItem mnuAudioLoad;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mnuFile_NewClip;
        private ToolStripSeparator toolStripMenuItem9;
        private ToolStripMenuItem mnuFile_Open;
        private ToolStripMenuItem mnuFile_Save;
        private ToolStripMenuItem mnuFile_SaveAs;
        private ToolStripSeparator toolStripMenuItem10;
        private ToolStripMenuItem mnuFile_Properties;
        private ToolStripMenuItem mnuFile_Parse;
        private ToolStripSeparator toolStripMenuItem11;
        private ToolStripMenuItem mnuFile_Publish;
        private ToolStripSeparator toolStripMenuItem12;
        private ToolStripMenuItem mnuFile_Exit;
        private TableLayoutPanel tableLayoutPanel2;
        internal ToolStrip ToolStrip1;
        internal ToolStripButton tbrNew;
        internal ToolStripButton tbrOpen;
        internal ToolStripButton tbrSave;
        internal ToolStripSeparator ToolStripSeparator1;
        private ToolStripButton tbrSmallerFont;
        private ToolStripButton tbrBiggerFont;
        internal ToolStripButton tbrFont;
        internal ToolStripSeparator ToolStripSeparator4;
        internal ToolStripButton tbrRight;
        internal ToolStripButton tbrLeft;
        internal ToolStripSeparator ToolStripSeparator2;
        internal ToolStripButton tbrBold;
        internal ToolStripButton tbrItalic;
        internal ToolStripButton tbrUnderline;
        internal ToolStripSeparator ToolStripSeparator3;
        private ToolStripButton tbrParagraph;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton tbrSentense;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton tbrSection;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton tbrWord;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripButton tbrProperties;
        private ToolStripButton tbrParse;
        private ToolStripButton tbrPublish;
        public RichTextBox richTextBox1;
        private RichTextBox richTextBox2;
        #endregion

        #region Clip SetUp

        private TimeSpan m_setStartTime = TimeSpan.Zero;
        private Word m_selectedScheduledWord = null;
        private bool m_selectedAnchor = false;
        private bool m_skipSelectionChange = false;
        private Chapter m_chapter = null;
        private string m_initClip = string.Empty;
        public static Regex m_regexAll = new Regex(@"(\(\()|(\)\))|(\[\[)|(\]\])|({{)|(}})|(<<)|(>>)", RegexOptions.Compiled | RegexOptions.Multiline);

        private static Regex m_regexRemoveWhiteSpacesParagraphs = new Regex(@"(?<=\{\{)[ ]{1,}|[ ]{1,}(?=\}\})", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexRemoveWhiteSpacesSentences = new Regex(@"(?<=\(\()[ ]{1,}|[ ]{1,}(?=\)\))", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexRemoveWhiteSpacesSections = new Regex(@"(?<=\<\<)[ ]{1,}|[ ]{1,}(?=\>\>)", RegexOptions.Compiled | RegexOptions.Singleline);

        private static Regex m_regexParagraphs = new Regex(@"(.+?)(\[3\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexSentenses = new Regex(@"(.+?)(\[2\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexSections = new Regex(@"(.+?)(\[1\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexWords = new Regex(@"(.+?)(\[0\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);

        private Graphics rtbMainEditorGraphics;
        private Graphics rtbAlternateEditorGraphics;

        private int sizeIndex = 3;
        private int[] sizes = { 9, 10, 12, 16, 32 };

        #endregion
        private ToolStripMenuItem עזרהToolStripMenuItem;
        private ToolStripMenuItem mnuHelp_About;
        private ToolStripSeparator toolStripMenuItem13;
        private ToolStripMenuItem mnuHelp_ShowJSON;
        private ToolStripMenuItem טקסטToolStripMenuItem;
        private ToolStripMenuItem mnuText_Goto;
        private SaveFileDialog saveFileDialog1;
        private FontDialog fontDialog1;
        private TabPage tabPage3;
        private TableLayoutPanel tableLayoutPanel3;
        public RichTextBox richTextBox3;
        private TabPage tabPage4;
        public Label label1;
        private Panel panel4;
        public PictureBox pictureBox1;
        private AudioDjStudio.AudioDjStudio audioDjStudio1;
        public GroupBox groupBox1;
        public Button buttonStartDJPlay;
        private Label label7;
        private Timer djLineTimer;
        private GroupBox groupBox2;
        private MyMentorUtilityClient.TimeSpinner.TimePickerSpinner timePickerSpinner1;
        private Label label9;
        public Button buttonHammer;
        private ToolStripMenuItem mnuLoadTest;
        private Timer timerUpdateTimePickerSpinner;

        private string m_strExportPathname;

        // Reverb internal DSP
        private void ReverbCallback(IntPtr bufferSamples, Int32 bufferSamplesLength, Int32 nUserData)
        {
            float[] buffTemp = new float[bufferSamplesLength / 4];
            Int32 index;

            Marshal.Copy(bufferSamples, buffTemp, 0, bufferSamplesLength / 4);

            for (index = 0; index < (bufferSamplesLength / 4); index += 2)
            {
                float left, right;
                left = buffTemp[index] + (m_buffReverbLeft[m_posReverb] / 2);
                right = buffTemp[index + 1] + (m_buffReverbRight[m_posReverb] / 2);

                m_buffReverbLeft[m_posReverb] = left;
                buffTemp[index] = left;
                m_buffReverbRight[m_posReverb] = right;
                buffTemp[index + 1] = right;

                m_posReverb++;
                if (m_posReverb == BUFFERLEN)
                    m_posReverb = 0;
            }

            Marshal.Copy(buffTemp, 0, bufferSamples, bufferSamplesLength / 4);
        }

        // Balance internal DSP
        private void BalanceCallback(IntPtr bufferSamples, Int32 bufferSamplesLength, Int32 nUserData)
        {
            if (m_nBalancePercentageInternal == 0)
                return;

            float[] buffTemp = new float[bufferSamplesLength / 4];

            Marshal.Copy(bufferSamples, buffTemp, 0, bufferSamplesLength / 4);

            Int32 length = bufferSamplesLength;
            Int32 index = 0;
            do
            {
                if (m_nBalancePercentageInternal < 0)
                    buffTemp[index + 1] = buffTemp[index + 1] * (100 + m_nBalancePercentageInternal) / 100;
                else
                    buffTemp[index] = buffTemp[index] * (100 - m_nBalancePercentageInternal) / 100;

                length = length - 8;
                index = index + 2;
            }
            while (length > 0);

            Marshal.Copy(buffTemp, 0, bufferSamples, bufferSamplesLength / 4);
        }


        public FormMain()
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
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.Frame4 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.LabelSelectionBegin = new System.Windows.Forms.Label();
            this.LabelSelectionEnd = new System.Windows.Forms.Label();
            this.LabelSelectionDuration = new System.Windows.Forms.Label();
            this.LabelRangeBegin = new System.Windows.Forms.Label();
            this.LabelRangeEnd = new System.Windows.Forms.Label();
            this.LabelRangeDuration = new System.Windows.Forms.Label();
            this.LabelTotalDuration = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.framePlayback = new System.Windows.Forms.GroupBox();
            this.buttonPause = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.buttonPlaySelection = new System.Windows.Forms.Button();
            this.Picture1 = new System.Windows.Forms.PictureBox();
            this.LabelStatus = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.TimerReload = new System.Windows.Forms.Timer(this.components);
            this.TimerMenuEnabler = new System.Windows.Forms.Timer(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timerDisplayWaveform = new System.Windows.Forms.Timer(this.components);
            this.audioSoundEditor1 = new AudioSoundEditor.AudioSoundEditor();
            this.FrameRecording = new System.Windows.Forms.GroupBox();
            this.buttonStartRecAppend = new System.Windows.Forms.Button();
            this.buttonStopRecording = new System.Windows.Forms.Button();
            this.buttonStartRecNew = new System.Windows.Forms.Button();
            this.audioSoundRecorder1 = new AudioSoundRecorder.AudioSoundRecorder();
            this.timerRecordingDone = new System.Windows.Forms.Timer(this.components);
            this.label18 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.ToolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbrNew = new System.Windows.Forms.ToolStripButton();
            this.tbrOpen = new System.Windows.Forms.ToolStripButton();
            this.tbrSave = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbrSmallerFont = new System.Windows.Forms.ToolStripButton();
            this.tbrBiggerFont = new System.Windows.Forms.ToolStripButton();
            this.tbrFont = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tbrRight = new System.Windows.Forms.ToolStripButton();
            this.tbrLeft = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbrBold = new System.Windows.Forms.ToolStripButton();
            this.tbrItalic = new System.Windows.Forms.ToolStripButton();
            this.tbrUnderline = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tbrParagraph = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tbrSentense = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tbrSection = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.tbrWord = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.tbrProperties = new System.Windows.Forms.ToolStripButton();
            this.tbrParse = new System.Windows.Forms.ToolStripButton();
            this.tbrPublish = new System.Windows.Forms.ToolStripButton();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonHammer = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonStartDJPlay = new System.Windows.Forms.Button();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.timePickerSpinner1 = new MyMentorUtilityClient.TimeSpinner.TimePickerSpinner();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_NewClip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFile_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFile_Properties = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_Parse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFile_Publish = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFile_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.טקסטToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuText_Goto = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudio = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAudioOptions_InsertSilent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAudioOptions_ApplyBackgroundSound = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAudioOptions_AppendSoundFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioOptions_InsertSoundFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAudioSelectedPart = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioSelectedPart_Cut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioSelectedPart_Copy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioSelectedPart_Paste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioSelectedPart_PasteInsertMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAudioSelectedPart_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioSelectedPart_Reduce = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAudioSelectedPart_SelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioSelectedPart_Remove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAudioEffects = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioEffects_Equalizer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioEffects_Tempo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioEffects_Pitch = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAudioEffects_PlaybackRate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuZoom = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoom_Selection = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoom_AllClip = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoom_In = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuZoom_Out = new System.Windows.Forms.ToolStripMenuItem();
            this.עזרהToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp_About = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelp_ShowJSON = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLoadTest = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.audioDjStudio1 = new AudioDjStudio.AudioDjStudio();
            this.djLineTimer = new System.Windows.Forms.Timer(this.components);
            this.timerUpdateTimePickerSpinner = new System.Windows.Forms.Timer(this.components);
            this.Frame4.SuspendLayout();
            this.framePlayback.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Picture1)).BeginInit();
            this.FrameRecording.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.ToolStrip1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Frame4
            // 
            this.Frame4.BackColor = System.Drawing.SystemColors.Control;
            this.Frame4.Controls.Add(this.label1);
            this.Frame4.Controls.Add(this.Label2);
            this.Frame4.Controls.Add(this.Label3);
            this.Frame4.Controls.Add(this.Label4);
            this.Frame4.Controls.Add(this.Label5);
            this.Frame4.Controls.Add(this.Label6);
            this.Frame4.Controls.Add(this.LabelSelectionBegin);
            this.Frame4.Controls.Add(this.LabelSelectionEnd);
            this.Frame4.Controls.Add(this.LabelSelectionDuration);
            this.Frame4.Controls.Add(this.LabelRangeBegin);
            this.Frame4.Controls.Add(this.LabelRangeEnd);
            this.Frame4.Controls.Add(this.LabelRangeDuration);
            this.Frame4.Controls.Add(this.LabelTotalDuration);
            this.Frame4.Controls.Add(this.Label8);
            this.Frame4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Frame4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Frame4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame4.Location = new System.Drawing.Point(3, 215);
            this.Frame4.Name = "Frame4";
            this.Frame4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tableLayoutPanel1.SetRowSpan(this.Frame4, 2);
            this.Frame4.Size = new System.Drawing.Size(493, 206);
            this.Frame4.TabIndex = 17;
            this.Frame4.TabStop = false;
            this.Frame4.Text = "מיקום";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.Location = new System.Drawing.Point(116, 38);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(154, 21);
            this.label1.TabIndex = 26;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.SystemColors.Control;
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label2.Location = new System.Drawing.Point(413, 106);
            this.Label2.Name = "Label2";
            this.Label2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label2.Size = new System.Drawing.Size(65, 17);
            this.Label2.TabIndex = 25;
            this.Label2.Text = "בחירה";
            // 
            // Label3
            // 
            this.Label3.BackColor = System.Drawing.SystemColors.Control;
            this.Label3.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label3.Location = new System.Drawing.Point(413, 149);
            this.Label3.Name = "Label3";
            this.Label3.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label3.Size = new System.Drawing.Size(65, 17);
            this.Label3.TabIndex = 24;
            this.Label3.Text = "תחום גלוי";
            // 
            // Label4
            // 
            this.Label4.BackColor = System.Drawing.SystemColors.Control;
            this.Label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label4.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label4.Location = new System.Drawing.Point(338, 85);
            this.Label4.Name = "Label4";
            this.Label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label4.Size = new System.Drawing.Size(59, 17);
            this.Label4.TabIndex = 23;
            this.Label4.Text = "התחלה";
            // 
            // Label5
            // 
            this.Label5.BackColor = System.Drawing.SystemColors.Control;
            this.Label5.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label5.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label5.Location = new System.Drawing.Point(242, 85);
            this.Label5.Name = "Label5";
            this.Label5.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label5.Size = new System.Drawing.Size(41, 17);
            this.Label5.TabIndex = 22;
            this.Label5.Text = "סוף";
            // 
            // Label6
            // 
            this.Label6.BackColor = System.Drawing.SystemColors.Control;
            this.Label6.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label6.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label6.Location = new System.Drawing.Point(116, 85);
            this.Label6.Name = "Label6";
            this.Label6.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label6.Size = new System.Drawing.Size(53, 17);
            this.Label6.TabIndex = 21;
            this.Label6.Text = "משך";
            // 
            // LabelSelectionBegin
            // 
            this.LabelSelectionBegin.BackColor = System.Drawing.Color.White;
            this.LabelSelectionBegin.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelSelectionBegin.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelSelectionBegin.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSelectionBegin.ForeColor = System.Drawing.Color.Black;
            this.LabelSelectionBegin.Location = new System.Drawing.Point(289, 105);
            this.LabelSelectionBegin.Name = "LabelSelectionBegin";
            this.LabelSelectionBegin.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelSelectionBegin.Size = new System.Drawing.Size(108, 22);
            this.LabelSelectionBegin.TabIndex = 20;
            this.LabelSelectionBegin.Text = "00:00:00.000";
            this.LabelSelectionBegin.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LabelSelectionEnd
            // 
            this.LabelSelectionEnd.BackColor = System.Drawing.Color.White;
            this.LabelSelectionEnd.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelSelectionEnd.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelSelectionEnd.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSelectionEnd.ForeColor = System.Drawing.Color.Black;
            this.LabelSelectionEnd.Location = new System.Drawing.Point(175, 105);
            this.LabelSelectionEnd.Name = "LabelSelectionEnd";
            this.LabelSelectionEnd.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelSelectionEnd.Size = new System.Drawing.Size(108, 22);
            this.LabelSelectionEnd.TabIndex = 19;
            this.LabelSelectionEnd.Text = "00:00:00.000";
            this.LabelSelectionEnd.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LabelSelectionDuration
            // 
            this.LabelSelectionDuration.BackColor = System.Drawing.Color.White;
            this.LabelSelectionDuration.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelSelectionDuration.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelSelectionDuration.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelSelectionDuration.ForeColor = System.Drawing.Color.Black;
            this.LabelSelectionDuration.Location = new System.Drawing.Point(61, 105);
            this.LabelSelectionDuration.Name = "LabelSelectionDuration";
            this.LabelSelectionDuration.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelSelectionDuration.Size = new System.Drawing.Size(108, 22);
            this.LabelSelectionDuration.TabIndex = 18;
            this.LabelSelectionDuration.Text = "00:00:00.000";
            this.LabelSelectionDuration.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LabelRangeBegin
            // 
            this.LabelRangeBegin.BackColor = System.Drawing.Color.White;
            this.LabelRangeBegin.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelRangeBegin.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelRangeBegin.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelRangeBegin.ForeColor = System.Drawing.Color.Black;
            this.LabelRangeBegin.Location = new System.Drawing.Point(289, 148);
            this.LabelRangeBegin.Name = "LabelRangeBegin";
            this.LabelRangeBegin.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelRangeBegin.Size = new System.Drawing.Size(108, 22);
            this.LabelRangeBegin.TabIndex = 17;
            this.LabelRangeBegin.Text = "00:00:00.000";
            this.LabelRangeBegin.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LabelRangeEnd
            // 
            this.LabelRangeEnd.BackColor = System.Drawing.Color.White;
            this.LabelRangeEnd.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelRangeEnd.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelRangeEnd.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelRangeEnd.ForeColor = System.Drawing.Color.Black;
            this.LabelRangeEnd.Location = new System.Drawing.Point(175, 148);
            this.LabelRangeEnd.Name = "LabelRangeEnd";
            this.LabelRangeEnd.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelRangeEnd.Size = new System.Drawing.Size(108, 22);
            this.LabelRangeEnd.TabIndex = 16;
            this.LabelRangeEnd.Text = "00:00:00.000";
            this.LabelRangeEnd.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LabelRangeDuration
            // 
            this.LabelRangeDuration.BackColor = System.Drawing.Color.White;
            this.LabelRangeDuration.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelRangeDuration.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelRangeDuration.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelRangeDuration.ForeColor = System.Drawing.Color.Black;
            this.LabelRangeDuration.Location = new System.Drawing.Point(61, 148);
            this.LabelRangeDuration.Name = "LabelRangeDuration";
            this.LabelRangeDuration.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelRangeDuration.Size = new System.Drawing.Size(108, 22);
            this.LabelRangeDuration.TabIndex = 15;
            this.LabelRangeDuration.Text = "00:00:00.000";
            this.LabelRangeDuration.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // LabelTotalDuration
            // 
            this.LabelTotalDuration.BackColor = System.Drawing.Color.White;
            this.LabelTotalDuration.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelTotalDuration.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelTotalDuration.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelTotalDuration.ForeColor = System.Drawing.Color.Black;
            this.LabelTotalDuration.Location = new System.Drawing.Point(292, 38);
            this.LabelTotalDuration.Name = "LabelTotalDuration";
            this.LabelTotalDuration.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelTotalDuration.Size = new System.Drawing.Size(108, 22);
            this.LabelTotalDuration.TabIndex = 14;
            this.LabelTotalDuration.Text = "00:00:00.000";
            this.LabelTotalDuration.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Label8
            // 
            this.Label8.BackColor = System.Drawing.SystemColors.Control;
            this.Label8.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label8.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label8.Location = new System.Drawing.Point(413, 38);
            this.Label8.Name = "Label8";
            this.Label8.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label8.Size = new System.Drawing.Size(53, 21);
            this.Label8.TabIndex = 13;
            this.Label8.Text = "אורך הקלטה";
            // 
            // framePlayback
            // 
            this.framePlayback.BackColor = System.Drawing.SystemColors.Control;
            this.framePlayback.Controls.Add(this.buttonPause);
            this.framePlayback.Controls.Add(this.buttonPlay);
            this.framePlayback.Controls.Add(this.buttonStop);
            this.framePlayback.Controls.Add(this.buttonPlaySelection);
            this.framePlayback.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.framePlayback.ForeColor = System.Drawing.SystemColors.ControlText;
            this.framePlayback.Location = new System.Drawing.Point(616, 321);
            this.framePlayback.Name = "framePlayback";
            this.framePlayback.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.framePlayback.Size = new System.Drawing.Size(409, 76);
            this.framePlayback.TabIndex = 15;
            this.framePlayback.TabStop = false;
            this.framePlayback.Text = "ניגון";
            // 
            // buttonPause
            // 
            this.buttonPause.Enabled = false;
            this.buttonPause.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPause.Location = new System.Drawing.Point(218, 27);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(85, 28);
            this.buttonPause.TabIndex = 10;
            this.buttonPause.Text = "השהה";
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.BackColor = System.Drawing.SystemColors.Control;
            this.buttonPlay.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonPlay.Enabled = false;
            this.buttonPlay.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlay.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonPlay.Location = new System.Drawing.Point(309, 27);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonPlay.Size = new System.Drawing.Size(94, 28);
            this.buttonPlay.TabIndex = 9;
            this.buttonPlay.Text = "נגן";
            this.buttonPlay.UseVisualStyleBackColor = false;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStop.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStop.Enabled = false;
            this.buttonStop.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStop.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStop.Location = new System.Drawing.Point(6, 27);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStop.Size = new System.Drawing.Size(101, 28);
            this.buttonStop.TabIndex = 8;
            this.buttonStop.Text = "עצור";
            this.buttonStop.UseVisualStyleBackColor = false;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // buttonPlaySelection
            // 
            this.buttonPlaySelection.BackColor = System.Drawing.SystemColors.Control;
            this.buttonPlaySelection.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonPlaySelection.Enabled = false;
            this.buttonPlaySelection.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlaySelection.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonPlaySelection.Location = new System.Drawing.Point(113, 27);
            this.buttonPlaySelection.Name = "buttonPlaySelection";
            this.buttonPlaySelection.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonPlaySelection.Size = new System.Drawing.Size(99, 28);
            this.buttonPlaySelection.TabIndex = 7;
            this.buttonPlaySelection.Text = "נגן בחירה";
            this.buttonPlaySelection.UseVisualStyleBackColor = false;
            this.buttonPlaySelection.Click += new System.EventHandler(this.buttonPlaySelection_Click);
            // 
            // Picture1
            // 
            this.Picture1.BackColor = System.Drawing.Color.Black;
            this.Picture1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Picture1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Picture1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Picture1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Picture1.Location = new System.Drawing.Point(0, 0);
            this.Picture1.Name = "Picture1";
            this.Picture1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Picture1.Size = new System.Drawing.Size(1022, 206);
            this.Picture1.TabIndex = 13;
            this.Picture1.TabStop = false;
            this.Picture1.Visible = false;
            // 
            // LabelStatus
            // 
            this.LabelStatus.BackColor = System.Drawing.SystemColors.Control;
            this.LabelStatus.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelStatus.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LabelStatus.Location = new System.Drawing.Point(160, 0);
            this.LabelStatus.Name = "LabelStatus";
            this.LabelStatus.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelStatus.Size = new System.Drawing.Size(201, 18);
            this.LabelStatus.TabIndex = 16;
            this.LabelStatus.Text = "Status: Idle";
            this.LabelStatus.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // TimerReload
            // 
            this.TimerReload.Tick += new System.EventHandler(this.TimerReload_Tick);
            // 
            // TimerMenuEnabler
            // 
            this.TimerMenuEnabler.Tick += new System.EventHandler(this.TimerMenuEnabler_Tick);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(90, 21);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(340, 14);
            this.progressBar1.TabIndex = 19;
            this.progressBar1.Visible = false;
            // 
            // timerDisplayWaveform
            // 
            this.timerDisplayWaveform.Tick += new System.EventHandler(this.timerDisplayWaveform_Tick);
            // 
            // audioSoundEditor1
            // 
            this.audioSoundEditor1.Location = new System.Drawing.Point(519, 473);
            this.audioSoundEditor1.Name = "audioSoundEditor1";
            this.audioSoundEditor1.Size = new System.Drawing.Size(48, 48);
            this.audioSoundEditor1.TabIndex = 23;
            this.audioSoundEditor1.WaveAnalysisStart += new AudioSoundEditor.AudioSoundEditor.EventHandler(this.audioSoundEditor1_WaveAnalysisStart);
            this.audioSoundEditor1.WaveAnalysisPerc += new AudioSoundEditor.AudioSoundEditor.WaveAnalysisPercEventHandler(this.audioSoundEditor1_WaveAnalysisPerc);
            this.audioSoundEditor1.WaveAnalysisStop += new AudioSoundEditor.AudioSoundEditor.WaveAnalysisStopEventHandler(this.audioSoundEditor1_WaveAnalysisStop);
            this.audioSoundEditor1.SoundEditStarted += new AudioSoundEditor.AudioSoundEditor.SoundEditStartedEventHandler(this.audioSoundEditor1_SoundEditStarted);
            this.audioSoundEditor1.SoundEditPerc += new AudioSoundEditor.AudioSoundEditor.SoundEditPercEventHandler(this.audioSoundEditor1_SoundEditPerc);
            this.audioSoundEditor1.SoundEditDone += new AudioSoundEditor.AudioSoundEditor.SoundEditDoneEventHandler(this.audioSoundEditor1_SoundEditDone);
            this.audioSoundEditor1.WaveAnalyzerSelectionChange += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerSelectionChangeEventHandler(this.audioSoundEditor1_WaveAnalyzerSelectionChange);
            this.audioSoundEditor1.WaveAnalyzerDisplayRangeChange += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerDisplayRangeChangeEventHandler(this.audioSoundEditor1_WaveAnalyzerDisplayRangeChange);
            this.audioSoundEditor1.SoundExportStarted += new AudioSoundEditor.AudioSoundEditor.EventHandler(this.audioSoundEditor1_SoundExportStarted);
            this.audioSoundEditor1.SoundExportPerc += new AudioSoundEditor.AudioSoundEditor.SoundExportPercEventHandler(this.audioSoundEditor1_SoundExportPerc);
            this.audioSoundEditor1.SoundExportDone += new AudioSoundEditor.AudioSoundEditor.SoundExportDoneEventHandler(this.audioSoundEditor1_SoundExportDone);
            this.audioSoundEditor1.SoundPlaybackDone += new AudioSoundEditor.AudioSoundEditor.EventHandler(this.audioSoundEditor1_SoundPlaybackDone);
            this.audioSoundEditor1.SoundPlaybackStopped += new AudioSoundEditor.AudioSoundEditor.EventHandler(this.audioSoundEditor1_SoundPlaybackStopped);
            this.audioSoundEditor1.SoundPlaybackPaused += new AudioSoundEditor.AudioSoundEditor.EventHandler(this.audioSoundEditor1_SoundPlaybackPaused);
            this.audioSoundEditor1.SoundPlaybackPlaying += new AudioSoundEditor.AudioSoundEditor.EventHandler(this.audioSoundEditor1_SoundPlaybackPlaying);
            this.audioSoundEditor1.SoundLoadingStarted += new AudioSoundEditor.AudioSoundEditor.EventHandler(this.audioSoundEditor1_SoundLoadingStarted);
            this.audioSoundEditor1.SoundLoadingPerc += new AudioSoundEditor.AudioSoundEditor.SoundLoadingPercEventHandler(this.audioSoundEditor1_SoundLoadingPerc);
            this.audioSoundEditor1.SoundLoadingDone += new AudioSoundEditor.AudioSoundEditor.SoundLoadingDoneEventHandler(this.audioSoundEditor1_SoundLoadingDone);
            this.audioSoundEditor1.WaveAnalyzerMouseNotification += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerMouseNotificationEventHandler(this.audioSoundEditor1_WaveAnalyzerMouseNotification);
            this.audioSoundEditor1.WaveAnalyzerLineMoving += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerLineMovingEventHandler(this.audioSoundEditor1_WaveAnalyzerLineMoving);
            this.audioSoundEditor1.WaveAnalyzerHorzLineMoving += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerHorzLineMovingEventHandler(this.audioSoundEditor1_WaveAnalyzerHorzLineMoving);
            // 
            // FrameRecording
            // 
            this.FrameRecording.BackColor = System.Drawing.SystemColors.Control;
            this.FrameRecording.Controls.Add(this.buttonStartRecAppend);
            this.FrameRecording.Controls.Add(this.buttonStopRecording);
            this.FrameRecording.Controls.Add(this.buttonStartRecNew);
            this.FrameRecording.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FrameRecording.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FrameRecording.Location = new System.Drawing.Point(616, 215);
            this.FrameRecording.Name = "FrameRecording";
            this.FrameRecording.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.FrameRecording.Size = new System.Drawing.Size(409, 80);
            this.FrameRecording.TabIndex = 24;
            this.FrameRecording.TabStop = false;
            this.FrameRecording.Text = "הקלטה";
            // 
            // buttonStartRecAppend
            // 
            this.buttonStartRecAppend.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStartRecAppend.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStartRecAppend.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartRecAppend.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStartRecAppend.Location = new System.Drawing.Point(170, 32);
            this.buttonStartRecAppend.Name = "buttonStartRecAppend";
            this.buttonStartRecAppend.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStartRecAppend.Size = new System.Drawing.Size(107, 28);
            this.buttonStartRecAppend.TabIndex = 6;
            this.buttonStartRecAppend.Text = "התחל הוספה";
            this.buttonStartRecAppend.UseVisualStyleBackColor = false;
            this.buttonStartRecAppend.Click += new System.EventHandler(this.buttonStartRecAppend_Click);
            // 
            // buttonStopRecording
            // 
            this.buttonStopRecording.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStopRecording.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStopRecording.Enabled = false;
            this.buttonStopRecording.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStopRecording.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStopRecording.Location = new System.Drawing.Point(6, 32);
            this.buttonStopRecording.Name = "buttonStopRecording";
            this.buttonStopRecording.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStopRecording.Size = new System.Drawing.Size(160, 28);
            this.buttonStopRecording.TabIndex = 5;
            this.buttonStopRecording.Text = "עצור הקלטה";
            this.buttonStopRecording.UseVisualStyleBackColor = false;
            this.buttonStopRecording.Click += new System.EventHandler(this.buttonStopRecording_Click);
            // 
            // buttonStartRecNew
            // 
            this.buttonStartRecNew.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStartRecNew.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStartRecNew.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartRecNew.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStartRecNew.Location = new System.Drawing.Point(283, 32);
            this.buttonStartRecNew.Name = "buttonStartRecNew";
            this.buttonStartRecNew.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStartRecNew.Size = new System.Drawing.Size(120, 28);
            this.buttonStartRecNew.TabIndex = 4;
            this.buttonStartRecNew.Text = "התחל חדש";
            this.buttonStartRecNew.UseVisualStyleBackColor = false;
            this.buttonStartRecNew.Click += new System.EventHandler(this.buttonStartRecNew_Click);
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
            this.audioSoundRecorder1.EncodeOggBitrate = -1;
            this.audioSoundRecorder1.EncodeOggCustomString = "";
            this.audioSoundRecorder1.EncodeOggDownmix = false;
            this.audioSoundRecorder1.EncodeOggMode = AudioSoundRecorder.enumOggEncodeModes.OGG_ENCODE_QUALITY;
            this.audioSoundRecorder1.EncodeOggQuality = 3F;
            this.audioSoundRecorder1.EncodeOggResampleFreq = -1;
            this.audioSoundRecorder1.EncodeWmaCBR = -1;
            this.audioSoundRecorder1.EncodeWmaMode = AudioSoundRecorder.enumWmaEncodeModes.WMA_ENCODE_VBR_QUALITY;
            this.audioSoundRecorder1.EncodeWmaVBRQuality = 100;
            this.audioSoundRecorder1.Location = new System.Drawing.Point(407, 437);
            this.audioSoundRecorder1.Name = "audioSoundRecorder1";
            this.audioSoundRecorder1.SilenceThreshold = ((short)(0));
            this.audioSoundRecorder1.Size = new System.Drawing.Size(48, 48);
            this.audioSoundRecorder1.TabIndex = 25;
            this.audioSoundRecorder1.RecordingStarted += new AudioSoundRecorder.AudioSoundRecorder.EventHandler(this.audioSoundRecorder1_RecordingStarted);
            this.audioSoundRecorder1.RecordingStopped += new AudioSoundRecorder.AudioSoundRecorder.RecordingStoppedEventHandler(this.audioSoundRecorder1_RecordingStopped);
            this.audioSoundRecorder1.VUMeterValueChange += new AudioSoundRecorder.AudioSoundRecorder.VUMeterValueChangeEventHandler(this.audioSoundRecorder1_VUMeterValueChange);
            // 
            // timerRecordingDone
            // 
            this.timerRecordingDone.Tick += new System.EventHandler(this.timerRecordingDone_Tick);
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.Black;
            this.label18.Location = new System.Drawing.Point(3, 15);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(10, 190);
            this.label18.TabIndex = 60;
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(13, 15);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(10, 191);
            this.label17.TabIndex = 59;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Arial Narrow", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(0, 24);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.RightToLeftLayout = true;
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1042, 526);
            this.tabControl1.TabIndex = 61;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 46);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1034, 476);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "1 - טקסט";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 134F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 148F));
            this.tableLayoutPanel2.Controls.Add(this.ToolStrip1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.richTextBox1, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.richTextBox2, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1028, 470);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // ToolStrip1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.ToolStrip1, 2);
            this.ToolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbrNew,
            this.tbrOpen,
            this.tbrSave,
            this.ToolStripSeparator1,
            this.tbrSmallerFont,
            this.tbrBiggerFont,
            this.tbrFont,
            this.ToolStripSeparator4,
            this.tbrRight,
            this.tbrLeft,
            this.ToolStripSeparator2,
            this.tbrBold,
            this.tbrItalic,
            this.tbrUnderline,
            this.ToolStripSeparator3,
            this.tbrParagraph,
            this.toolStripSeparator5,
            this.tbrSentense,
            this.toolStripSeparator6,
            this.tbrSection,
            this.toolStripSeparator7,
            this.tbrWord,
            this.toolStripSeparator8,
            this.tbrProperties,
            this.tbrParse,
            this.tbrPublish});
            this.ToolStrip1.Location = new System.Drawing.Point(282, 40);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.Size = new System.Drawing.Size(746, 28);
            this.ToolStrip1.TabIndex = 32;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // tbrNew
            // 
            this.tbrNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrNew.Image = ((System.Drawing.Image)(resources.GetObject("tbrNew.Image")));
            this.tbrNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrNew.Name = "tbrNew";
            this.tbrNew.Size = new System.Drawing.Size(23, 25);
            this.tbrNew.Text = "New";
            this.tbrNew.ToolTipText = "חדש";
            this.tbrNew.Click += new System.EventHandler(this.tbrNew_Click);
            // 
            // tbrOpen
            // 
            this.tbrOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrOpen.Image = ((System.Drawing.Image)(resources.GetObject("tbrOpen.Image")));
            this.tbrOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrOpen.Name = "tbrOpen";
            this.tbrOpen.Size = new System.Drawing.Size(23, 25);
            this.tbrOpen.Text = "Open";
            this.tbrOpen.ToolTipText = "פתח";
            this.tbrOpen.Click += new System.EventHandler(this.tbrOpen_Click);
            // 
            // tbrSave
            // 
            this.tbrSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrSave.Image = ((System.Drawing.Image)(resources.GetObject("tbrSave.Image")));
            this.tbrSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrSave.Name = "tbrSave";
            this.tbrSave.Size = new System.Drawing.Size(23, 25);
            this.tbrSave.Text = "Save";
            this.tbrSave.ToolTipText = "שמור";
            this.tbrSave.Click += new System.EventHandler(this.tbrSave_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 28);
            // 
            // tbrSmallerFont
            // 
            this.tbrSmallerFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrSmallerFont.Image = ((System.Drawing.Image)(resources.GetObject("tbrSmallerFont.Image")));
            this.tbrSmallerFont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrSmallerFont.Name = "tbrSmallerFont";
            this.tbrSmallerFont.Size = new System.Drawing.Size(24, 25);
            this.tbrSmallerFont.Text = "-A";
            this.tbrSmallerFont.Click += new System.EventHandler(this.tbrSmallerFont_Click);
            // 
            // tbrBiggerFont
            // 
            this.tbrBiggerFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrBiggerFont.Image = ((System.Drawing.Image)(resources.GetObject("tbrBiggerFont.Image")));
            this.tbrBiggerFont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrBiggerFont.Name = "tbrBiggerFont";
            this.tbrBiggerFont.Size = new System.Drawing.Size(27, 25);
            this.tbrBiggerFont.Text = "+A";
            this.tbrBiggerFont.ToolTipText = "הגדל טקסט";
            this.tbrBiggerFont.Click += new System.EventHandler(this.tbrBiggerFont_Click);
            // 
            // tbrFont
            // 
            this.tbrFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrFont.Image = ((System.Drawing.Image)(resources.GetObject("tbrFont.Image")));
            this.tbrFont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrFont.Name = "tbrFont";
            this.tbrFont.Size = new System.Drawing.Size(23, 25);
            this.tbrFont.Text = "Font";
            this.tbrFont.ToolTipText = "גופן";
            this.tbrFont.Click += new System.EventHandler(this.tbrFont_Click);
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(6, 28);
            // 
            // tbrRight
            // 
            this.tbrRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrRight.Image = ((System.Drawing.Image)(resources.GetObject("tbrRight.Image")));
            this.tbrRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrRight.Name = "tbrRight";
            this.tbrRight.Size = new System.Drawing.Size(23, 25);
            this.tbrRight.Text = "Right";
            this.tbrRight.ToolTipText = "ישר לימין";
            this.tbrRight.Click += new System.EventHandler(this.tbrRight_Click);
            // 
            // tbrLeft
            // 
            this.tbrLeft.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrLeft.Image = ((System.Drawing.Image)(resources.GetObject("tbrLeft.Image")));
            this.tbrLeft.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrLeft.Name = "tbrLeft";
            this.tbrLeft.Size = new System.Drawing.Size(23, 25);
            this.tbrLeft.Text = "Left";
            this.tbrLeft.ToolTipText = "יישר לשמאל";
            this.tbrLeft.Click += new System.EventHandler(this.tbrLeft_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 28);
            // 
            // tbrBold
            // 
            this.tbrBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrBold.Image = ((System.Drawing.Image)(resources.GetObject("tbrBold.Image")));
            this.tbrBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrBold.Name = "tbrBold";
            this.tbrBold.Size = new System.Drawing.Size(23, 25);
            this.tbrBold.Text = "Bold";
            this.tbrBold.ToolTipText = "מודגש";
            this.tbrBold.Click += new System.EventHandler(this.tbrBold_Click);
            // 
            // tbrItalic
            // 
            this.tbrItalic.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrItalic.Image = ((System.Drawing.Image)(resources.GetObject("tbrItalic.Image")));
            this.tbrItalic.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrItalic.Name = "tbrItalic";
            this.tbrItalic.Size = new System.Drawing.Size(23, 25);
            this.tbrItalic.Text = "Italic";
            this.tbrItalic.ToolTipText = "נטוי";
            this.tbrItalic.Click += new System.EventHandler(this.tbrItalic_Click);
            // 
            // tbrUnderline
            // 
            this.tbrUnderline.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrUnderline.Image = ((System.Drawing.Image)(resources.GetObject("tbrUnderline.Image")));
            this.tbrUnderline.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrUnderline.Name = "tbrUnderline";
            this.tbrUnderline.Size = new System.Drawing.Size(23, 25);
            this.tbrUnderline.Text = "Underline";
            this.tbrUnderline.ToolTipText = "קו תחתי";
            this.tbrUnderline.Click += new System.EventHandler(this.tbrUnderline_Click);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(6, 28);
            // 
            // tbrParagraph
            // 
            this.tbrParagraph.BackColor = System.Drawing.Color.Red;
            this.tbrParagraph.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrParagraph.ForeColor = System.Drawing.Color.White;
            this.tbrParagraph.Image = ((System.Drawing.Image)(resources.GetObject("tbrParagraph.Image")));
            this.tbrParagraph.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrParagraph.Name = "tbrParagraph";
            this.tbrParagraph.Size = new System.Drawing.Size(25, 25);
            this.tbrParagraph.Text = "[3]";
            this.tbrParagraph.ToolTipText = "חלק פסקה";
            this.tbrParagraph.Click += new System.EventHandler(this.tbrParagraph_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 28);
            // 
            // tbrSentense
            // 
            this.tbrSentense.BackColor = System.Drawing.Color.Violet;
            this.tbrSentense.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrSentense.ForeColor = System.Drawing.Color.White;
            this.tbrSentense.Image = ((System.Drawing.Image)(resources.GetObject("tbrSentense.Image")));
            this.tbrSentense.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrSentense.Name = "tbrSentense";
            this.tbrSentense.Size = new System.Drawing.Size(25, 25);
            this.tbrSentense.Text = "[2]";
            this.tbrSentense.ToolTipText = "חלק משפט";
            this.tbrSentense.Click += new System.EventHandler(this.tbrSentense_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 28);
            // 
            // tbrSection
            // 
            this.tbrSection.BackColor = System.Drawing.Color.LimeGreen;
            this.tbrSection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrSection.ForeColor = System.Drawing.Color.White;
            this.tbrSection.Image = ((System.Drawing.Image)(resources.GetObject("tbrSection.Image")));
            this.tbrSection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrSection.Name = "tbrSection";
            this.tbrSection.Size = new System.Drawing.Size(25, 25);
            this.tbrSection.Text = "[1]";
            this.tbrSection.ToolTipText = "חלק קטע";
            this.tbrSection.Click += new System.EventHandler(this.tbrSection_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 28);
            // 
            // tbrWord
            // 
            this.tbrWord.BackColor = System.Drawing.Color.Yellow;
            this.tbrWord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrWord.Image = ((System.Drawing.Image)(resources.GetObject("tbrWord.Image")));
            this.tbrWord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrWord.Name = "tbrWord";
            this.tbrWord.Size = new System.Drawing.Size(25, 25);
            this.tbrWord.Text = "[0]";
            this.tbrWord.ToolTipText = "התחל מילה";
            this.tbrWord.Click += new System.EventHandler(this.tbrWord_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 28);
            // 
            // tbrProperties
            // 
            this.tbrProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrProperties.Image = ((System.Drawing.Image)(resources.GetObject("tbrProperties.Image")));
            this.tbrProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrProperties.Name = "tbrProperties";
            this.tbrProperties.Size = new System.Drawing.Size(23, 25);
            this.tbrProperties.Text = "toolStripButton11";
            this.tbrProperties.ToolTipText = "מאפייני שיעור";
            // 
            // tbrParse
            // 
            this.tbrParse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrParse.Image = ((System.Drawing.Image)(resources.GetObject("tbrParse.Image")));
            this.tbrParse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrParse.Name = "tbrParse";
            this.tbrParse.Size = new System.Drawing.Size(23, 25);
            this.tbrParse.Text = "toolStripButton10";
            this.tbrParse.ToolTipText = "בדוק תקינות";
            this.tbrParse.Click += new System.EventHandler(this.tbrParse_Click);
            // 
            // tbrPublish
            // 
            this.tbrPublish.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrPublish.Image = ((System.Drawing.Image)(resources.GetObject("tbrPublish.Image")));
            this.tbrPublish.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrPublish.Name = "tbrPublish";
            this.tbrPublish.Size = new System.Drawing.Size(23, 25);
            this.tbrPublish.Text = "toolStripButton9";
            this.tbrPublish.ToolTipText = "פרסם";
            // 
            // richTextBox1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.richTextBox1, 5);
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.richTextBox1.Location = new System.Drawing.Point(4, 72);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox1.Name = "richTextBox1";
            this.tableLayoutPanel2.SetRowSpan(this.richTextBox1, 2);
            this.richTextBox1.Size = new System.Drawing.Size(1020, 394);
            this.richTextBox1.TabIndex = 31;
            this.richTextBox1.Text = "";
            this.richTextBox1.SelectionChanged += new System.EventHandler(this.richTextBox1_SelectionChanged);
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(552, 3);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(100, 34);
            this.richTextBox2.TabIndex = 33;
            this.richTextBox2.Text = "";
            this.richTextBox2.Visible = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 46);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1034, 476);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "2 - הקלטה";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Frame4, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.FrameRecording, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.framePlayback, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1028, 470);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.panel1, 3);
            this.panel1.Controls.Add(this.Picture1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1022, 206);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label18);
            this.panel2.Controls.Add(this.label17);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(502, 215);
            this.panel2.Name = "panel2";
            this.tableLayoutPanel1.SetRowSpan(this.panel2, 2);
            this.panel2.Size = new System.Drawing.Size(24, 206);
            this.panel2.TabIndex = 18;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.progressBar1);
            this.panel3.Controls.Add(this.LabelStatus);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(532, 427);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(493, 40);
            this.panel3.TabIndex = 25;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tableLayoutPanel3);
            this.tabPage3.Location = new System.Drawing.Point(4, 46);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1034, 476);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "3 - תזמון";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.groupBox1, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.richTextBox3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.panel4, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.groupBox2, 1, 3);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1028, 470);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.buttonHammer);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.buttonStartDJPlay);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(556, 373);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(469, 92);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ניגון";
            // 
            // buttonHammer
            // 
            this.buttonHammer.BackColor = System.Drawing.SystemColors.Control;
            this.buttonHammer.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonHammer.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHammer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonHammer.Location = new System.Drawing.Point(217, 37);
            this.buttonHammer.Name = "buttonHammer";
            this.buttonHammer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonHammer.Size = new System.Drawing.Size(94, 28);
            this.buttonHammer.TabIndex = 11;
            this.buttonHammer.Text = "פטישון";
            this.buttonHammer.UseVisualStyleBackColor = false;
            this.buttonHammer.Click += new System.EventHandler(this.buttonHammer_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 18);
            this.label7.TabIndex = 10;
            this.label7.Text = "label7";
            // 
            // buttonStartDJPlay
            // 
            this.buttonStartDJPlay.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStartDJPlay.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStartDJPlay.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartDJPlay.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStartDJPlay.Location = new System.Drawing.Point(342, 37);
            this.buttonStartDJPlay.Name = "buttonStartDJPlay";
            this.buttonStartDJPlay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStartDJPlay.Size = new System.Drawing.Size(94, 28);
            this.buttonStartDJPlay.TabIndex = 9;
            this.buttonStartDJPlay.Text = "נגן";
            this.buttonStartDJPlay.UseVisualStyleBackColor = false;
            this.buttonStartDJPlay.Click += new System.EventHandler(this.buttonStartDJPlay_Click);
            // 
            // richTextBox3
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.richTextBox3, 2);
            this.richTextBox3.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox3.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
            this.richTextBox3.HideSelection = false;
            this.richTextBox3.Location = new System.Drawing.Point(4, 4);
            this.richTextBox3.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.ReadOnly = true;
            this.tableLayoutPanel3.SetRowSpan(this.richTextBox3, 2);
            this.richTextBox3.Size = new System.Drawing.Size(1020, 192);
            this.richTextBox3.TabIndex = 32;
            this.richTextBox3.Text = "";
            this.richTextBox3.SelectionChanged += new System.EventHandler(this.richTextBox3_SelectionChanged);
            // 
            // panel4
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.panel4, 2);
            this.panel4.Controls.Add(this.pictureBox1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(3, 203);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1022, 164);
            this.panel4.TabIndex = 33;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Black;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pictureBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.pictureBox1.Size = new System.Drawing.Size(1022, 164);
            this.pictureBox1.TabIndex = 14;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.timePickerSpinner1);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(95, 373);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(416, 94);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "תזמון";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(244, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(48, 18);
            this.label9.TabIndex = 24;
            this.label9.Text = "בחירה";
            // 
            // timePickerSpinner1
            // 
            this.timePickerSpinner1.Location = new System.Drawing.Point(53, 19);
            this.timePickerSpinner1.Margin = new System.Windows.Forms.Padding(4);
            this.timePickerSpinner1.Name = "timePickerSpinner1";
            this.timePickerSpinner1.Size = new System.Drawing.Size(181, 36);
            this.timePickerSpinner1.TabIndex = 22;
            this.timePickerSpinner1.Value = System.TimeSpan.Parse("00:00:00");
            this.timePickerSpinner1.ValueChanged += new System.EventHandler(this.timePickerSpinner1_ValueChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 46);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1034, 476);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "4 - פרסום";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.טקסטToolStripMenuItem,
            this.mnuAudio,
            this.עזרהToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1042, 24);
            this.menuStrip1.TabIndex = 62;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile_NewClip,
            this.toolStripMenuItem9,
            this.mnuFile_Open,
            this.mnuFile_Save,
            this.mnuFile_SaveAs,
            this.toolStripMenuItem10,
            this.mnuFile_Properties,
            this.mnuFile_Parse,
            this.toolStripMenuItem11,
            this.mnuFile_Publish,
            this.toolStripMenuItem12,
            this.mnuFile_Exit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(46, 20);
            this.menuFile.Text = "קובץ";
            // 
            // mnuFile_NewClip
            // 
            this.mnuFile_NewClip.Name = "mnuFile_NewClip";
            this.mnuFile_NewClip.Size = new System.Drawing.Size(177, 22);
            this.mnuFile_NewClip.Text = "שיעור חדש";
            this.mnuFile_NewClip.Click += new System.EventHandler(this.mnuFile_NewClip_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(174, 6);
            // 
            // mnuFile_Open
            // 
            this.mnuFile_Open.Name = "mnuFile_Open";
            this.mnuFile_Open.Size = new System.Drawing.Size(177, 22);
            this.mnuFile_Open.Text = "פתח שיעור";
            // 
            // mnuFile_Save
            // 
            this.mnuFile_Save.Name = "mnuFile_Save";
            this.mnuFile_Save.Size = new System.Drawing.Size(177, 22);
            this.mnuFile_Save.Text = "שמור שיעור";
            this.mnuFile_Save.Click += new System.EventHandler(this.mnuFile_Save_Click);
            // 
            // mnuFile_SaveAs
            // 
            this.mnuFile_SaveAs.Name = "mnuFile_SaveAs";
            this.mnuFile_SaveAs.Size = new System.Drawing.Size(177, 22);
            this.mnuFile_SaveAs.Text = "שמור שיעור בשם...";
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(174, 6);
            // 
            // mnuFile_Properties
            // 
            this.mnuFile_Properties.Name = "mnuFile_Properties";
            this.mnuFile_Properties.Size = new System.Drawing.Size(177, 22);
            this.mnuFile_Properties.Text = "מאפיינים";
            // 
            // mnuFile_Parse
            // 
            this.mnuFile_Parse.Name = "mnuFile_Parse";
            this.mnuFile_Parse.Size = new System.Drawing.Size(177, 22);
            this.mnuFile_Parse.Text = "בדוק תקינות";
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(174, 6);
            // 
            // mnuFile_Publish
            // 
            this.mnuFile_Publish.Name = "mnuFile_Publish";
            this.mnuFile_Publish.Size = new System.Drawing.Size(177, 22);
            this.mnuFile_Publish.Text = "פרסם";
            // 
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(174, 6);
            // 
            // mnuFile_Exit
            // 
            this.mnuFile_Exit.Name = "mnuFile_Exit";
            this.mnuFile_Exit.Size = new System.Drawing.Size(177, 22);
            this.mnuFile_Exit.Text = "יציאה";
            this.mnuFile_Exit.Click += new System.EventHandler(this.mnuFile_Exit_Click);
            // 
            // טקסטToolStripMenuItem
            // 
            this.טקסטToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuText_Goto});
            this.טקסטToolStripMenuItem.Name = "טקסטToolStripMenuItem";
            this.טקסטToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.טקסטToolStripMenuItem.Text = "טקסט";
            // 
            // mnuText_Goto
            // 
            this.mnuText_Goto.Name = "mnuText_Goto";
            this.mnuText_Goto.Size = new System.Drawing.Size(150, 22);
            this.mnuText_Goto.Text = "גש למספר תו";
            this.mnuText_Goto.Click += new System.EventHandler(this.mnuText_Goto_Click);
            // 
            // mnuAudio
            // 
            this.mnuAudio.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAudioOptions,
            this.toolStripMenuItem4,
            this.mnuAudioSelectedPart,
            this.toolStripMenuItem5,
            this.mnuAudioEffects,
            this.toolStripMenuItem6,
            this.mnuZoom});
            this.mnuAudio.Name = "mnuAudio";
            this.mnuAudio.Size = new System.Drawing.Size(90, 20);
            this.mnuAudio.Text = "הקלטת שמע";
            // 
            // mnuAudioOptions
            // 
            this.mnuAudioOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAudioLoad,
            this.toolStripMenuItem1,
            this.mnuAudioOptions_InsertSilent,
            this.toolStripMenuItem7,
            this.mnuAudioOptions_ApplyBackgroundSound,
            this.toolStripMenuItem8,
            this.mnuAudioOptions_AppendSoundFile,
            this.mnuAudioOptions_InsertSoundFile});
            this.mnuAudioOptions.Name = "mnuAudioOptions";
            this.mnuAudioOptions.Size = new System.Drawing.Size(131, 22);
            this.mnuAudioOptions.Text = "אפשרויות";
            // 
            // mnuAudioLoad
            // 
            this.mnuAudioLoad.Name = "mnuAudioLoad";
            this.mnuAudioLoad.Size = new System.Drawing.Size(197, 22);
            this.mnuAudioLoad.Text = "טען קובץ שמע";
            this.mnuAudioLoad.Click += new System.EventHandler(this.mnuAudioLoad_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(194, 6);
            // 
            // mnuAudioOptions_InsertSilent
            // 
            this.mnuAudioOptions_InsertSilent.Name = "mnuAudioOptions_InsertSilent";
            this.mnuAudioOptions_InsertSilent.Size = new System.Drawing.Size(197, 22);
            this.mnuAudioOptions_InsertSilent.Text = "הכנס קטע שקט";
            this.mnuAudioOptions_InsertSilent.Click += new System.EventHandler(this.mnuAudioOptions_InsertSilent_Click);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(194, 6);
            // 
            // mnuAudioOptions_ApplyBackgroundSound
            // 
            this.mnuAudioOptions_ApplyBackgroundSound.Name = "mnuAudioOptions_ApplyBackgroundSound";
            this.mnuAudioOptions_ApplyBackgroundSound.Size = new System.Drawing.Size(197, 22);
            this.mnuAudioOptions_ApplyBackgroundSound.Text = "שלב קובץ שמע ברקע";
            this.mnuAudioOptions_ApplyBackgroundSound.Click += new System.EventHandler(this.mnuAudioOptions_ApplyBackgroundSound_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(194, 6);
            // 
            // mnuAudioOptions_AppendSoundFile
            // 
            this.mnuAudioOptions_AppendSoundFile.Name = "mnuAudioOptions_AppendSoundFile";
            this.mnuAudioOptions_AppendSoundFile.Size = new System.Drawing.Size(197, 22);
            this.mnuAudioOptions_AppendSoundFile.Text = "הוסף קובץ שמע";
            this.mnuAudioOptions_AppendSoundFile.Click += new System.EventHandler(this.mnuAudioOptions_AppendSoundFile_Click);
            // 
            // mnuAudioOptions_InsertSoundFile
            // 
            this.mnuAudioOptions_InsertSoundFile.Name = "mnuAudioOptions_InsertSoundFile";
            this.mnuAudioOptions_InsertSoundFile.Size = new System.Drawing.Size(197, 22);
            this.mnuAudioOptions_InsertSoundFile.Text = "הכנס קובץ שמע";
            this.mnuAudioOptions_InsertSoundFile.Click += new System.EventHandler(this.mnuAudioOptions_InsertSoundFile_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(128, 6);
            // 
            // mnuAudioSelectedPart
            // 
            this.mnuAudioSelectedPart.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAudioSelectedPart_Cut,
            this.mnuAudioSelectedPart_Copy,
            this.mnuAudioSelectedPart_Paste,
            this.mnuAudioSelectedPart_PasteInsertMode,
            this.toolStripMenuItem2,
            this.mnuAudioSelectedPart_Delete,
            this.mnuAudioSelectedPart_Reduce,
            this.toolStripMenuItem3,
            this.mnuAudioSelectedPart_SelectAll,
            this.mnuAudioSelectedPart_Remove});
            this.mnuAudioSelectedPart.Name = "mnuAudioSelectedPart";
            this.mnuAudioSelectedPart.Size = new System.Drawing.Size(131, 22);
            this.mnuAudioSelectedPart.Text = "קטע נבחר";
            // 
            // mnuAudioSelectedPart_Cut
            // 
            this.mnuAudioSelectedPart_Cut.Name = "mnuAudioSelectedPart_Cut";
            this.mnuAudioSelectedPart_Cut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.mnuAudioSelectedPart_Cut.Size = new System.Drawing.Size(183, 22);
            this.mnuAudioSelectedPart_Cut.Text = "גזור";
            this.mnuAudioSelectedPart_Cut.Click += new System.EventHandler(this.mnuAudioSelectedPart_Cut_Click);
            // 
            // mnuAudioSelectedPart_Copy
            // 
            this.mnuAudioSelectedPart_Copy.Name = "mnuAudioSelectedPart_Copy";
            this.mnuAudioSelectedPart_Copy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mnuAudioSelectedPart_Copy.Size = new System.Drawing.Size(183, 22);
            this.mnuAudioSelectedPart_Copy.Text = "העתק";
            this.mnuAudioSelectedPart_Copy.Click += new System.EventHandler(this.mnuAudioSelectedPart_Copy_Click);
            // 
            // mnuAudioSelectedPart_Paste
            // 
            this.mnuAudioSelectedPart_Paste.Name = "mnuAudioSelectedPart_Paste";
            this.mnuAudioSelectedPart_Paste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.mnuAudioSelectedPart_Paste.Size = new System.Drawing.Size(183, 22);
            this.mnuAudioSelectedPart_Paste.Text = "הדבק";
            this.mnuAudioSelectedPart_Paste.Click += new System.EventHandler(this.mnuAudioSelectedPart_Paste_Click);
            // 
            // mnuAudioSelectedPart_PasteInsertMode
            // 
            this.mnuAudioSelectedPart_PasteInsertMode.Name = "mnuAudioSelectedPart_PasteInsertMode";
            this.mnuAudioSelectedPart_PasteInsertMode.Size = new System.Drawing.Size(183, 22);
            this.mnuAudioSelectedPart_PasteInsertMode.Text = "הדבק לקטע הנבחר";
            this.mnuAudioSelectedPart_PasteInsertMode.Visible = false;
            this.mnuAudioSelectedPart_PasteInsertMode.Click += new System.EventHandler(this.mnuAudioSelectedPart_PasteInsertMode_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(180, 6);
            // 
            // mnuAudioSelectedPart_Delete
            // 
            this.mnuAudioSelectedPart_Delete.Name = "mnuAudioSelectedPart_Delete";
            this.mnuAudioSelectedPart_Delete.Size = new System.Drawing.Size(183, 22);
            this.mnuAudioSelectedPart_Delete.Text = "מחק בחירה";
            this.mnuAudioSelectedPart_Delete.Click += new System.EventHandler(this.mnuAudioSelectedPart_Delete_Click);
            // 
            // mnuAudioSelectedPart_Reduce
            // 
            this.mnuAudioSelectedPart_Reduce.Name = "mnuAudioSelectedPart_Reduce";
            this.mnuAudioSelectedPart_Reduce.Size = new System.Drawing.Size(183, 22);
            this.mnuAudioSelectedPart_Reduce.Text = "צמצם לבחירה";
            this.mnuAudioSelectedPart_Reduce.Click += new System.EventHandler(this.mnuAudioSelectedPart_Reduce_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(180, 6);
            // 
            // mnuAudioSelectedPart_SelectAll
            // 
            this.mnuAudioSelectedPart_SelectAll.Name = "mnuAudioSelectedPart_SelectAll";
            this.mnuAudioSelectedPart_SelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mnuAudioSelectedPart_SelectAll.Size = new System.Drawing.Size(183, 22);
            this.mnuAudioSelectedPart_SelectAll.Text = "בחר הכל";
            this.mnuAudioSelectedPart_SelectAll.Click += new System.EventHandler(this.mnuAudioSelectedPart_SelectAll_Click);
            // 
            // mnuAudioSelectedPart_Remove
            // 
            this.mnuAudioSelectedPart_Remove.Name = "mnuAudioSelectedPart_Remove";
            this.mnuAudioSelectedPart_Remove.Size = new System.Drawing.Size(183, 22);
            this.mnuAudioSelectedPart_Remove.Text = "הסר בחירה";
            this.mnuAudioSelectedPart_Remove.Click += new System.EventHandler(this.mnuAudioSelectedPart_Remove_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(128, 6);
            // 
            // mnuAudioEffects
            // 
            this.mnuAudioEffects.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAudioEffects_Equalizer,
            this.mnuAudioEffects_Tempo,
            this.mnuAudioEffects_Pitch,
            this.mnuAudioEffects_PlaybackRate});
            this.mnuAudioEffects.Name = "mnuAudioEffects";
            this.mnuAudioEffects.Size = new System.Drawing.Size(131, 22);
            this.mnuAudioEffects.Text = "אפקטים";
            // 
            // mnuAudioEffects_Equalizer
            // 
            this.mnuAudioEffects_Equalizer.Name = "mnuAudioEffects_Equalizer";
            this.mnuAudioEffects_Equalizer.Size = new System.Drawing.Size(175, 22);
            this.mnuAudioEffects_Equalizer.Text = "Equalizer";
            this.mnuAudioEffects_Equalizer.Click += new System.EventHandler(this.mnuAudioEffects_Equalizer_Click);
            // 
            // mnuAudioEffects_Tempo
            // 
            this.mnuAudioEffects_Tempo.Name = "mnuAudioEffects_Tempo";
            this.mnuAudioEffects_Tempo.Size = new System.Drawing.Size(175, 22);
            this.mnuAudioEffects_Tempo.Text = "ערוך Tempo";
            this.mnuAudioEffects_Tempo.Click += new System.EventHandler(this.mnuAudioEffects_Tempo_Click);
            // 
            // mnuAudioEffects_Pitch
            // 
            this.mnuAudioEffects_Pitch.Name = "mnuAudioEffects_Pitch";
            this.mnuAudioEffects_Pitch.Size = new System.Drawing.Size(175, 22);
            this.mnuAudioEffects_Pitch.Text = "ערוך Pitch";
            this.mnuAudioEffects_Pitch.Click += new System.EventHandler(this.mnuAudioEffects_Pitch_Click);
            // 
            // mnuAudioEffects_PlaybackRate
            // 
            this.mnuAudioEffects_PlaybackRate.Name = "mnuAudioEffects_PlaybackRate";
            this.mnuAudioEffects_PlaybackRate.Size = new System.Drawing.Size(175, 22);
            this.mnuAudioEffects_PlaybackRate.Text = "ערוך Playback Rate";
            this.mnuAudioEffects_PlaybackRate.Click += new System.EventHandler(this.mnuAudioEffects_PlaybackRate_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(128, 6);
            // 
            // mnuZoom
            // 
            this.mnuZoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuZoom_Selection,
            this.mnuZoom_AllClip,
            this.mnuZoom_In,
            this.mnuZoom_Out});
            this.mnuZoom.Name = "mnuZoom";
            this.mnuZoom.Size = new System.Drawing.Size(131, 22);
            this.mnuZoom.Text = "זום";
            // 
            // mnuZoom_Selection
            // 
            this.mnuZoom_Selection.Name = "mnuZoom_Selection";
            this.mnuZoom_Selection.Size = new System.Drawing.Size(166, 22);
            this.mnuZoom_Selection.Text = "זום לקטע הנבחר";
            this.mnuZoom_Selection.Click += new System.EventHandler(this.mnuZoom_Selection_Click);
            // 
            // mnuZoom_AllClip
            // 
            this.mnuZoom_AllClip.Name = "mnuZoom_AllClip";
            this.mnuZoom_AllClip.Size = new System.Drawing.Size(166, 22);
            this.mnuZoom_AllClip.Text = "זום לכל הקטע";
            this.mnuZoom_AllClip.Click += new System.EventHandler(this.mnuZoom_AllClip_Click);
            // 
            // mnuZoom_In
            // 
            this.mnuZoom_In.Name = "mnuZoom_In";
            this.mnuZoom_In.Size = new System.Drawing.Size(166, 22);
            this.mnuZoom_In.Text = "זום פנימה";
            this.mnuZoom_In.Click += new System.EventHandler(this.mnuZoom_In_Click);
            // 
            // mnuZoom_Out
            // 
            this.mnuZoom_Out.Name = "mnuZoom_Out";
            this.mnuZoom_Out.Size = new System.Drawing.Size(166, 22);
            this.mnuZoom_Out.Text = "זום החוצה";
            this.mnuZoom_Out.Click += new System.EventHandler(this.mnuZoom_Out_Click);
            // 
            // עזרהToolStripMenuItem
            // 
            this.עזרהToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelp_About,
            this.toolStripMenuItem13,
            this.mnuHelp_ShowJSON,
            this.mnuLoadTest});
            this.עזרהToolStripMenuItem.Name = "עזרהToolStripMenuItem";
            this.עזרהToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.עזרהToolStripMenuItem.Text = "עזרה";
            // 
            // mnuHelp_About
            // 
            this.mnuHelp_About.Name = "mnuHelp_About";
            this.mnuHelp_About.Size = new System.Drawing.Size(165, 22);
            this.mnuHelp_About.Text = "אודות";
            this.mnuHelp_About.Click += new System.EventHandler(this.mnuHelp_About_Click);
            // 
            // toolStripMenuItem13
            // 
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Size = new System.Drawing.Size(162, 6);
            // 
            // mnuHelp_ShowJSON
            // 
            this.mnuHelp_ShowJSON.Name = "mnuHelp_ShowJSON";
            this.mnuHelp_ShowJSON.Size = new System.Drawing.Size(165, 22);
            this.mnuHelp_ShowJSON.Text = "הצג נתוני DEBUG";
            this.mnuHelp_ShowJSON.Click += new System.EventHandler(this.mnuHelp_ShowJSON_Click);
            // 
            // mnuLoadTest
            // 
            this.mnuLoadTest.Name = "mnuLoadTest";
            this.mnuLoadTest.Size = new System.Drawing.Size(165, 22);
            this.mnuLoadTest.Text = "טען קובץ Test";
            this.mnuLoadTest.Click += new System.EventHandler(this.mnuLoadTest_Click);
            // 
            // audioDjStudio1
            // 
            this.audioDjStudio1.Fader = ((AudioDjStudio.FaderObject)(resources.GetObject("audioDjStudio1.Fader")));
            this.audioDjStudio1.LastError = AudioDjStudio.enumErrorCodes.ERR_NOERROR;
            this.audioDjStudio1.Location = new System.Drawing.Point(567, 0);
            this.audioDjStudio1.Name = "audioDjStudio1";
            this.audioDjStudio1.Size = new System.Drawing.Size(48, 48);
            this.audioDjStudio1.TabIndex = 63;
            this.audioDjStudio1.SoundDone += new AudioDjStudio.AudioDjStudio.PlayerEventHandler(this.audioDjStudio1_SoundDone);
            this.audioDjStudio1.SilenceDetectionStateChange += new AudioDjStudio.AudioDjStudio.SilenceDetectionStateChangeEventHandler(this.audioDjStudio1_SilenceDetectionStateChange);
            // 
            // djLineTimer
            // 
            this.djLineTimer.Interval = 50;
            this.djLineTimer.Tick += new System.EventHandler(this.djLineTimer_Tick);
            // 
            // timerUpdateTimePickerSpinner
            // 
            this.timerUpdateTimePickerSpinner.Interval = 50;
            this.timerUpdateTimePickerSpinner.Tick += new System.EventHandler(this.timerUpdateSpinnerControl_Tick);
            // 
            // FormMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 19);
            this.ClientSize = new System.Drawing.Size(1042, 550);
            this.Controls.Add(this.audioDjStudio1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.audioSoundEditor1);
            this.Controls.Add(this.audioSoundRecorder1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MyMentor";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.ResizeEnd += new System.EventHandler(this.FormMain_ResizeEnd);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormMain_Paint);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.Frame4.ResumeLayout(false);
            this.framePlayback.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Picture1)).EndInit();
            this.FrameRecording.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion


        public static bool CheckKeyPress(TextBox textbox, Int32 key)
        {
            // allow valid floating point characters
            if (key >= 48 && key <= 57)
                return false;

            // obtain the decimal separator for the current locale
            char strDecimalSeparator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            int nDecimalSeparator = Convert.ToInt32(strDecimalSeparator);
            if (key == nDecimalSeparator ||
                key == 8 ||
                key == 45)
            {
                if (key == nDecimalSeparator)
                {
                    // check if the decimal separator has already been entered
                    if (textbox.Text.IndexOf(strDecimalSeparator) != -1)
                        // discard further decimal separators
                        return true;
                }
                return false;
            }

            // character not valid for floating point number, discard it
            return true;
        }

        private void GetSelectedRange(ref Int32 nBeginSelectionInMs, ref Int32 nEndSelectionInMs)
        {
            bool bSelectionAvailable = false;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // if a selection is available
            if (!bSelectionAvailable)
            {
                // apply the effect on the whole sound
                nBeginSelectionInMs = 0;
                nEndSelectionInMs = -1;
            }

            // check if we have simply selected a position (vertical dotted line)
            if (nBeginSelectionInMs == nEndSelectionInMs)
                // apply the effect till the end of sound
                nEndSelectionInMs = -1;
        }

        private IntPtr CreateVuMeter(Label ctrlPosition, AudioSoundRecorder.enumGraphicBarOrientations nOrientation)
        {
            // create a new graphic bar
            IntPtr hWnd = audioSoundRecorder1.GraphicBarsManager.Create(panel2.Handle, ctrlPosition.Left, ctrlPosition.Top,
                ctrlPosition.Width, ctrlPosition.Height);

            // set graphic bar range
            audioSoundRecorder1.GraphicBarsManager.SetRange(hWnd, 0, 32767);

            // enable automatic drop and set the requested orientation
            AudioSoundRecorder.GRAPHIC_BAR_SETTINGS settings = new AudioSoundRecorder.GRAPHIC_BAR_SETTINGS();
            audioSoundRecorder1.GraphicBarsManager.GetGraphicalSettings(hWnd, ref settings);
            settings.bAutomaticDrop = true;
            settings.nOrientation = nOrientation;
            audioSoundRecorder1.GraphicBarsManager.SetGraphicalSettings(hWnd, settings);

            return hWnd;
        }

        private void FormMain_Load(object sender, System.EventArgs e)
        {
            rtbMainEditorGraphics = richTextBox1.CreateGraphics();
            rtbAlternateEditorGraphics = richTextBox3.CreateGraphics();

            // init controls
            audioSoundRecorder1.InitRecordingSystem();
            audioSoundEditor1.InitEditor();

            // create the recorder's VU-Meter
            audioSoundRecorder1.DisplayVUMeter.Create(IntPtr.Zero);
            m_hWndVuMeterLeft = CreateVuMeter(label17, AudioSoundRecorder.enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL);
            m_hWndVuMeterRight = CreateVuMeter(label18, AudioSoundRecorder.enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL);

            // create the waveform analyzer (always call this function on the end of the form's Load fucntion)
            audioSoundEditor1.DisplayWaveformAnalyzer.Create(panel1.Handle, Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);

            // get the analyzer's current settings
            WANALYZER_SCROLLBARS_SETTINGS settingsScrollbars = new WANALYZER_SCROLLBARS_SETTINGS();
            audioSoundEditor1.DisplayWaveformAnalyzer.SettingsScrollbarsGet(ref settingsScrollbars);

            // hide the bottom scrollbar
            settingsScrollbars.bVisibleBottom = false;

            // apply the new settings
            audioSoundEditor1.DisplayWaveformAnalyzer.SettingsScrollbarsSet(settingsScrollbars);

            // init unique identifiers for DSPs
            m_idDspReverbInternal = 0;
            m_idDspBalanceInternal = 0;
            m_idDspReverbExternal = 0;
            m_idDspBalanceExternal = 0;
            m_idDspBassBoostExternal = 0;

            // init unique identifiers for VSTs
            m_idVstKarmaFxEq = 0;

            TimerMenuEnabler.Enabled = true;
        }

        private void buttonPlay_Click(object sender, System.EventArgs e)
        {
            audioDjStudio1.LoadSoundFromEditingSession(0, audioSoundEditor1.Handle);

            audioSoundEditor1.PlaySound();
        }

        private void buttonPlaySelection_Click(object sender, System.EventArgs e)
        {
            // get the position selected on the waveform analyzer, if any
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // if a selection is available
            if (bSelectionAvailable)
                // play selected range only
                audioSoundEditor1.PlaySoundRange(nBeginSelectionInMs, nEndSelectionInMs);
        }

        private void buttonPause_Click(object sender, System.EventArgs e)
        {
            if (buttonPause.Text == "השהה")
            {
                audioSoundEditor1.PauseSound();
            }
            else
                audioSoundEditor1.ResumeSound();
        }

        private void buttonStop_Click(object sender, System.EventArgs e)
        {
            audioSoundEditor1.StopSound();
        }

        private void LoadAudioFromMemory(object sender, System.EventArgs e)
        {
            // due to the fact that a Load operation will discard any existing sound,
            // check if there is already a Loading session available
            DialogResult result;
            if (audioSoundEditor1.GetSoundDuration() > 0)
            {
                // ask the user if he wants to go on
                result = MessageBox.Show("קיים קובץ שמע בזיכרון, האם בכל זאת ליצור חדש ?", "MyMentor", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
            }

            // free the actual session
            audioSoundEditor1.CloseSound();

            // reset formatted strings
            LabelRangeBegin.Text = "00:00:00.000";
            LabelRangeEnd.Text = "00:00:00.000";
            LabelRangeDuration.Text = "00:00:00.000";
            LabelSelectionBegin.Text = "00:00:00.000";
            LabelSelectionEnd.Text = "00:00:00.000";
            LabelSelectionDuration.Text = "00:00:00.000";
            LabelTotalDuration.Text = "00:00:00.000";

            openFileDialog1.Filter =
                "Supported Sounds (*.mp3;*.mp2;*.wav;*.ogg;*.aiff;*.wma;*.wmv;*.asx;*.asf;" +
                "*.m4a;*.mp4;*.flac;*.aac;*.ac3;*.wv;" +
                "*.au;*.aif;*.w64;*.voc;*.sf;*.paf;*.pvf;*.caf;*.svx ;" +
                "*.it;*.xm;*.s3m;*.mod;*.mtm;*.mo3;*.cda)|" +
                "*.mp3;*.mp2;*.wav;*.ogg;*.aiff;*.wma;*.wmv;*.asx;*.asf;" +
                "*.m4a;*.mp4;*.flac;*.aac;*.ac3;*.wv;" +
                "*.au;*.aif;*.w64;*.voc;*.sf;*.paf;*.pvf;*.caf;*.svx ;" +
                "*.it;*.xm;*.s3m;*.mod;*.mtm;*.mo3;*.cda|" +
                "MP3 and MP2 sounds (*.mp3;*.mp2)|*.mp3;*.mp2|" +
                "AAC and MP4 sounds (*.aac;*.mp4)|*.aac;*.mp4|" +
                "WAV sounds (*.wav)|*.wav|" +
                "OGG Vorbis sounds (*.ogg)|*.ogg|" +
                "AIFF sounds (*.aiff)|*.aiff|" +
                "Windows Media sounds (*.wma;*.wmv;*.asx;*.asf)|*.wma;*.wmv;*.asx;*.asf|" +
                "AC3 sounds (*.ac3)|*.ac3;|" +
                "ALAC sounds (*.m4a)|*.ac3;|" +
                "FLAC sounds (*.flac)|*.flac;|" +
                "WavPack sounds (*.wv)|*.wv;|" +
                "All files (*.*)|*.*";
            openFileDialog1.Title = "Load a sound file";
            result = openFileDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            // read the song file into a memory buffer
            FileStream streamFile = new FileStream(openFileDialog1.FileName, FileMode.Open);
            BinaryReader binReader = new BinaryReader(streamFile);

            m_byteBuffer = new byte[streamFile.Length];
            int read = binReader.Read(m_byteBuffer, 0, (int)streamFile.Length);

            // load the new song from the memory buffer
            audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_NEW);
            if (audioSoundEditor1.LoadSoundFromMemory(m_byteBuffer, (Int32)streamFile.Length) != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show("Cannot load file " + openFileDialog1.FileName);

            binReader.Close();
            streamFile.Close();
        }

        private void ExportAudioFile(object sender, System.EventArgs e)
        {
            FormExport frmExport = new FormExport();
            frmExport.audioSoundEditor1 = this.audioSoundEditor1;
            frmExport.ShowDialog(this);
            m_strExportPathname = frmExport.m_strExportPathname;
        }

        private void TimerMenuEnabler_Tick(object sender, System.EventArgs e)
        {
            // check if there is a sound inside the system clipboard
            if (audioSoundEditor1.IsSoundAvailableInClipboard())
            {
                mnuAudioSelectedPart_Paste.Enabled = true;

                // check if there is already an available recorded sound
                if (audioSoundEditor1.GetSoundDuration() > 0)
                {
                    mnuAudioSelectedPart_Paste.Text = "הדבק ב- '&Append mode'";

                    mnuAudioSelectedPart_PasteInsertMode.Visible = true;
                }
                else
                {
                    mnuAudioSelectedPart_Paste.Text = "&הדבק";

                    mnuAudioSelectedPart_PasteInsertMode.Visible = false;
                }
            }
            else
            {
                mnuAudioSelectedPart_Paste.Enabled = false;
                mnuAudioSelectedPart_PasteInsertMode.Visible = false;
                mnuAudioSelectedPart_Paste.Text = "הדבק";
            }

            // check if there is a selection on the analyzer
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // if a selection is available
            if (bSelectionAvailable)
            {
                // enable copy & cut
                mnuAudioSelectedPart_Cut.Enabled = true;
                mnuAudioSelectedPart_Copy.Enabled = true;

                // enable selection deletion
                mnuAudioSelectedPart_Delete.Enabled = true;

                // enable reduction to selection
                mnuAudioSelectedPart_Reduce.Enabled = true;

                // enable selection removal
                mnuAudioSelectedPart_Remove.Enabled = true;

                // enable zoom to selection
                mnuZoom_Selection.Enabled = true;
            }
            else
            {
                // disable copy & cut
                mnuAudioSelectedPart_Cut.Enabled = false;
                mnuAudioSelectedPart_Copy.Enabled = false;

                // disable selection deletion
                mnuAudioSelectedPart_Delete.Enabled = false;

                // disable reduction to selection
                mnuAudioSelectedPart_Reduce.Enabled = false;

                // disable selection removal
                mnuAudioSelectedPart_Remove.Enabled = false;

                // disable zoom to selection
                mnuZoom_Selection.Enabled = false;
            }

            // check if there is a sound available
            if (audioSoundEditor1.GetSoundDuration() > 0)
            {
                // enable Select all
                mnuAudioSelectedPart_SelectAll.Enabled = true;

                // enable editing tools
                mnuAudioOptions_InsertSilent.Enabled = true;
                mnuAudioOptions_ApplyBackgroundSound.Enabled = true;

                // enable special effects
                mnuAudioEffects.Enabled = true;

                // enable zoom
                mnuZoom.Enabled = true;
            }
            else
            {
                // disable Select all
                mnuAudioSelectedPart_SelectAll.Enabled = false;

                // disable editing tools
                mnuAudioOptions_InsertSilent.Enabled = false;
                mnuAudioOptions_ApplyBackgroundSound.Enabled = false;

                // disable special effects
                mnuAudioEffects.Enabled = false;

                // disable zoom
                mnuZoom.Enabled = false;
            }
        }

        private void TimerReload_Tick(object sender, System.EventArgs e)
        {
            // reset the timer
            TimerReload.Enabled = false;

            // get updated sound duration
            Int32 nDurationInMs = audioSoundEditor1.GetSoundDuration();
            if (nDurationInMs == 0)
            {
                // the whole song was deleted, let's free all
                audioSoundEditor1.CloseSound();

                // reset formatted strings
                LabelRangeBegin.Text = "00:00:00.000";
                LabelRangeEnd.Text = "00:00:00.000";
                LabelRangeDuration.Text = "00:00:00.000";
                LabelSelectionBegin.Text = "00:00:00.000";
                LabelSelectionEnd.Text = "00:00:00.000";
                LabelSelectionDuration.Text = "00:00:00.000";
                LabelTotalDuration.Text = "00:00:00.000";
                return;
            }

            // display updated sound duration
            LabelTotalDuration.Text = audioSoundEditor1.GetFormattedTime(nDurationInMs, true, true);
            LabelTotalDuration.Refresh();

            // check if the song was loaded from a previous recording session
            if (audioSoundRecorder1.RecordedSound.GetDuration() > 0)
                // free recorder contents
                audioSoundRecorder1.RecordedSound.FreeMemory();

            // request full waveform analysis using different resolutions depending upon the song's duration
            // remember that the higher the resolution, the slower the time required to perform it
            enumAnalyzerResolutions nResolution;
            if (nDurationInMs <= 5000)
                // less than 5 seconds
                nResolution = enumAnalyzerResolutions.WAVEANALYZER_RES_MAXIMUM;
            else if ((nDurationInMs > 5000) && (nDurationInMs <= 10000))
                // between 5 and 10 seconds
                nResolution = enumAnalyzerResolutions.WAVEANALYZER_RES_HIGH;
            else if ((nDurationInMs > 10000) && (nDurationInMs <= 300000))
                // between 10 seconds and 5 minutes
                nResolution = enumAnalyzerResolutions.WAVEANALYZER_RES_MIDDLE;
            else
                // above 5 minutes
                nResolution = enumAnalyzerResolutions.WAVEANALYZER_RES_LOW;

            // get the mix analyzer's current settings
            WANALYZER_GENERAL_SETTINGS settingsGeneral = new WANALYZER_GENERAL_SETTINGS();
            audioSoundEditor1.DisplayWaveformAnalyzer.SettingsGeneralGet(ref settingsGeneral);

            // apply the new resolution
            settingsGeneral.nResolution = nResolution;
            audioSoundEditor1.DisplayWaveformAnalyzer.SettingsGeneralSet(settingsGeneral);

            // start waveform analysis
            audioSoundEditor1.DisplayWaveformAnalyzer.AnalyzeFullSound();

            // some debugging message
            enumStoreModes nStoreMode = audioSoundEditor1.GetStoreMode();
            if (nStoreMode == enumStoreModes.STORE_MODE_MEMORY_BUFFER)
                Debug.WriteLine("Song loaded inside a memory buffer whose total size is " + audioSoundEditor1.GetMemorySize() + " bytes");
            else if (nStoreMode == enumStoreModes.STORE_MODE_TEMP_FILE)
            {
                Debug.WriteLine("Song loaded inside a temporary file whose pathname is " + audioSoundEditor1.GetTempFilePathname());
                Debug.WriteLine("and whose size is " + audioSoundEditor1.GetTempFileSize() + " bytes");
            }
        }

        private void timerDisplayWaveform_Tick(object sender, System.EventArgs e)
        {
            timerDisplayWaveform.Enabled = false;

            // display the full waveform
            audioSoundEditor1.DisplayWaveformAnalyzer.SetDisplayRange(0, -1);
        }

        private void audioSoundEditor1_SoundExportStarted(object sender, System.EventArgs e)
        {
            LabelStatus.Text = "Status: Exporting...";
            progressBar1.Visible = true;

            LabelStatus.Refresh();
        }

        private void audioSoundEditor1_SoundExportPerc(object sender, AudioSoundEditor.SoundExportPercEventArgs e)
        {
            if (progressBar1.Value == e.nPercentage)
                // no change
                return;

            progressBar1.Value = e.nPercentage;
            LabelStatus.Text = "Status: Exporting... " + e.nPercentage.ToString() + "%";

            progressBar1.Refresh();
            LabelStatus.Refresh();
        }

        private void audioSoundEditor1_SoundExportDone(object sender, AudioSoundEditor.SoundExportDoneEventArgs e)
        {
            LabelStatus.Text = "Status: Idle";
            progressBar1.Visible = false;

            LabelStatus.Refresh();

            if (e.nResult != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show("Export failed with the following error code: " + e.nResult.ToString());
            else
                MessageBox.Show("Editing session exported to " + m_strExportPathname);
        }

        private void audioSoundEditor1_SoundEditStarted(object sender, AudioSoundEditor.SoundEditStartedEventArgs e)
        {
            LabelStatus.Text = "Status: Editing ...";
            progressBar1.Visible = true;

            LabelStatus.Refresh();
            progressBar1.Refresh();
        }

        private void audioSoundEditor1_SoundEditPerc(object sender, AudioSoundEditor.SoundEditPercEventArgs e)
        {
            if (progressBar1.Value == e.nPercentage)
                // no change
                return;

            progressBar1.Value = e.nPercentage;
            LabelStatus.Text = "Status: Editing... " + e.nPercentage.ToString() + "%";

            progressBar1.Refresh();
            LabelStatus.Refresh();
        }

        private void audioSoundEditor1_SoundEditDone(object sender, AudioSoundEditor.SoundEditDoneEventArgs e)
        {
            LabelStatus.Text = "Status: Idle";
            progressBar1.Visible = false;

            LabelStatus.Refresh();

            if (e.bResult == true)
            {
                // success, force a new analysis of the recorded sound
                TimerReload.Enabled = true;

                Int32 nDuration = audioSoundEditor1.GetSoundDuration();
                LabelTotalDuration.Text = audioSoundEditor1.GetFormattedTime(nDuration, true, true);
                LabelTotalDuration.Refresh();
            }
        }

        private void audioSoundEditor1_SoundPlaybackDone(object sender, System.EventArgs e)
        {
            buttonPause.Text = "השהה";
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            buttonStartDJPlay.Text = "נגן";
            timePickerSpinner1.Enabled = true;

            FrameRecording.Enabled = true;
        }

        private void audioSoundEditor1_SoundPlaybackPlaying(object sender, System.EventArgs e)
        {
            buttonPause.Text = "השהה";
            LabelStatus.Text = "Status: Playing...";
            LabelStatus.Refresh();

            FrameRecording.Enabled = false;
        }

        private void audioSoundEditor1_SoundPlaybackPaused(object sender, System.EventArgs e)
        {
            buttonPause.Text = "המשך";
            LabelStatus.Text = "Status: Playback paused";
            LabelStatus.Refresh();

            int mm = audioSoundEditor1.GetPlaybackPosition();
            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_uniqueLine, mm, mm);

        }

        private void audioSoundEditor1_SoundPlaybackStopped(object sender, System.EventArgs e)
        {
            buttonPause.Text = "השהה";
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            FrameRecording.Enabled = true;
        }

        private void audioSoundEditor1_WaveAnalysisStart(object sender, System.EventArgs e)
        {
            LabelStatus.Text = "Status: Analyzing waveform...";
            progressBar1.Value = 0;
            progressBar1.Visible = true;

            progressBar1.Refresh();
            LabelStatus.Refresh();
        }

        private void audioSoundEditor1_WaveAnalysisPerc(object sender, AudioSoundEditor.WaveAnalysisPercEventArgs e)
        {
            if (progressBar1.Value == e.nPercentage)
                // no change
                return;

            progressBar1.Value = e.nPercentage;
            LabelStatus.Text = "Status: Analyzing waveform... " + e.nPercentage.ToString() + "%";
            progressBar1.Refresh();
            LabelStatus.Refresh();
        }

        private void audioSoundEditor1_WaveAnalysisStop(object sender, AudioSoundEditor.WaveAnalysisStopEventArgs e)
        {
            // force a refresh of the waveform analyzer
            timerDisplayWaveform.Enabled = true;

            progressBar1.Visible = false;
            LabelStatus.Text = "Status: Idle";

            buttonPlay.Enabled = true;
            buttonPause.Enabled = true;
            buttonStop.Enabled = true;

            m_uniqueLine = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineAdd("MyLine", "KooKoo", 0, new WANALYZER_VERTICAL_LINE { color = Color.White, nWidth = 1, nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT, nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DASH_DOT, nHighCap = enumLineCaps.LINE_CAP_SQUARE, nLowCap = enumLineCaps.LINE_CAP_SQUARE, nTranspFactor = 0 });
        }

        private void audioSoundEditor1_WaveAnalyzerDisplayRangeChange(object sender, AudioSoundEditor.WaveAnalyzerDisplayRangeChangeEventArgs e)
        {
            // display formatted strings
            LabelRangeBegin.Text = audioSoundEditor1.GetFormattedTime(e.nBeginPosInMs, true, true);
            LabelRangeEnd.Text = audioSoundEditor1.GetFormattedTime(e.nEndPosInMs, true, true);
            LabelRangeDuration.Text = audioSoundEditor1.GetFormattedTime(e.nEndPosInMs - e.nBeginPosInMs, true, true);
        }

        private void audioSoundEditor1_WaveAnalyzerSelectionChange(object sender, AudioSoundEditor.WaveAnalyzerSelectionChangeEventArgs e)
        {
            if (e.bSelectionAvailable)
            {
                // check if this is not only a position selection
                if ((e.nEndPosInMs - e.nBeginPosInMs) > 0)
                    // selection can be played
                    buttonPlaySelection.Enabled = true;
                else
                    // selection cannot be played because it's simply a position selection
                    buttonPlaySelection.Enabled = false;

                // display formatted strings
                LabelSelectionBegin.Text = audioSoundEditor1.GetFormattedTime(e.nBeginPosInMs, true, true);
                LabelSelectionEnd.Text = audioSoundEditor1.GetFormattedTime(e.nEndPosInMs, true, true);
                LabelSelectionDuration.Text = audioSoundEditor1.GetFormattedTime(e.nEndPosInMs - e.nBeginPosInMs, true, true);

                //timePickerSpinner1.Value = TimeSpan.Parse(audioSoundEditor1.GetFormattedTime(e.nBeginPosInMs, true, true));
            }
            else
            {
                // no selection to play
                buttonPlaySelection.Enabled = false;

                // display formatted strings
                LabelSelectionBegin.Text = "00:00:00.000";
                LabelSelectionEnd.Text = "00:00:00.000";
                LabelSelectionDuration.Text = "00:00:00.000";
            }
        }


        private void audioSoundEditor1_SoundLoadingStarted(object sender, System.EventArgs e)
        {
            LabelStatus.Text = "Status: Loading... 0%";
            progressBar1.Value = 0;
            progressBar1.Visible = true;

            progressBar1.Refresh();
            LabelStatus.Refresh();
        }

        private void audioSoundEditor1_SoundLoadingPerc(object sender, AudioSoundEditor.SoundLoadingPercEventArgs e)
        {
            if (progressBar1.Value == e.nPercentage)
                // no change
                return;

            progressBar1.Value = e.nPercentage;
            LabelStatus.Text = "Status: Loading... " + e.nPercentage.ToString() + "%";

            progressBar1.Refresh();
            LabelStatus.Refresh();
        }

        private void audioSoundEditor1_SoundLoadingDone(object sender, AudioSoundEditor.SoundLoadingDoneEventArgs e)
        {
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();
            progressBar1.Visible = false;

            // force analysis of the loaded sound
            if (e.bResult == true)
                TimerReload.Enabled = true;
            else
                MessageBox.Show("Sound failed to load with the following error code: " + audioSoundEditor1.LastError.ToString());
        }


        private void buttonStartRecNew_Click(object sender, System.EventArgs e)
        {
            // check if we already have an editing session in memory
            DialogResult result;
            if (audioSoundEditor1.GetSoundDuration() > 0)
            {
                // ask the user if he wants to go on
                result = MessageBox.Show("An editing session is actually in memory: do you want to create a new one?", "Sound Editor", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
            }

            // set the flag for "new" mode
            m_bRecAppendMode = false;

            // create a fresh new recording session
            audioSoundRecorder1.SetRecordingMode(AudioSoundRecorder.enumRecordingModes.REC_MODE_NEW);

            // start recording in memory from system default input device and input channel
            audioSoundRecorder1.StartFromDirectSoundDevice(0, -1, "");
        }

        private void buttonStartRecAppend_Click(object sender, System.EventArgs e)
        {
            // create a fresh new recording session
            audioSoundRecorder1.SetRecordingMode(AudioSoundRecorder.enumRecordingModes.REC_MODE_NEW);

            // set the flag for "append" mode
            m_bRecAppendMode = true;

            // start recording in memory from system default input device and input channel
            audioSoundRecorder1.StartFromDirectSoundDevice(0, -1, "");
        }

        private void buttonStopRecording_Click(object sender, System.EventArgs e)
        {
            // stop recording
            audioSoundRecorder1.Stop();
        }

        private void audioSoundRecorder1_RecordingStarted(object sender, System.EventArgs e)
        {
            buttonStartRecNew.Enabled = false;
            buttonStartRecAppend.Enabled = false;
            buttonStopRecording.Enabled = true;

            framePlayback.Enabled = false;
        }

        private void audioSoundRecorder1_RecordingStopped(object sender, AudioSoundRecorder.RecordingStoppedEventArgs e)
        {
            // force loading the recording session into the editor
            timerRecordingDone.Enabled = true;

            buttonStartRecNew.Enabled = true;
            buttonStartRecAppend.Enabled = true;
            buttonStopRecording.Enabled = false;

            framePlayback.Enabled = true;
        }

        private void audioSoundRecorder1_VUMeterValueChange(object sender, AudioSoundRecorder.VUMeterValueChangeEventArgs e)
        {
            audioSoundRecorder1.GraphicBarsManager.SetValue(m_hWndVuMeterLeft, e.nPeakLeft);
            audioSoundRecorder1.GraphicBarsManager.SetValue(m_hWndVuMeterRight, e.nPeakRight);
        }

        private void timerRecordingDone_Tick(object sender, System.EventArgs e)
        {
            timerRecordingDone.Enabled = false;

            // check if the latest recording session must replace or appended to the previous editing session
            if (m_bRecAppendMode)
                // append mode
                audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_APPEND);
            else
                // discard previous editing session and start a new one
                audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_NEW);

            // load recorded sound inside the editor
            audioSoundEditor1.LoadSoundFromRecordingSession(audioSoundRecorder1.Handle);
        }


        private void mnuAudioEffects_Equalizer_Click(object sender, EventArgs e)
        {
            FormEqualizer frmEqualizer = new FormEqualizer();
            frmEqualizer.audioSoundEditor1 = this.audioSoundEditor1;
            frmEqualizer.ShowDialog(this);
            if (frmEqualizer.m_bCancel)
                return;

            // check if there is a selection on the analyzer
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            GetSelectedRange(ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // apply equalizer 
            audioSoundEditor1.Effects.EqualizerApply(nBeginSelectionInMs, nEndSelectionInMs);
        }

        private void mnuAudioEffects_Tempo_Click(object sender, EventArgs e)
        {
            FormTempoRate frmTempo = new FormTempoRate();
            frmTempo.m_bIsChangingTempo = true;
            frmTempo.ShowDialog(this);
            if (frmTempo.m_bCancel)
                return;

            // check if we have a selected range
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            GetSelectedRange(ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // apply Tempo change
            audioSoundEditor1.Effects.TempoApply(nBeginSelectionInMs, nEndSelectionInMs, frmTempo.m_fChangePercentage);

        }

        private void mnuAudioEffects_PlaybackRate_Click(object sender, EventArgs e)
        {
            FormTempoRate frmPlaybackRate = new FormTempoRate();
            frmPlaybackRate.m_bIsChangingTempo = false;
            frmPlaybackRate.ShowDialog(this);
            if (frmPlaybackRate.m_bCancel)
                return;

            // check if we have a selected range
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            GetSelectedRange(ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // apply Playback Rate change
            audioSoundEditor1.Effects.PlaybackRateApply(nBeginSelectionInMs, nEndSelectionInMs, frmPlaybackRate.m_fChangePercentage);

        }

        private void mnuAudioEffects_Pitch_Click(object sender, EventArgs e)
        {
            FormPitch frmPitch = new FormPitch();
            frmPitch.ShowDialog(this);
            if (frmPitch.m_bCancel)
                return;

            // check if we have a selected range
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            GetSelectedRange(ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // apply Pitch change
            audioSoundEditor1.Effects.PitchApply(nBeginSelectionInMs, nEndSelectionInMs, frmPitch.m_fChangeValue);

        }

        private void mnuAudioOptions_AppendSoundFile_Click(object sender, EventArgs e)
        {
            DialogResult result;
            openFileDialog1.Filter =
                "Supported Sounds (*.mp3;*.mp2;*.wav;*.ogg;*.aiff;*.wma;*.wmv;*.asx;*.asf;" +
                "*.m4a;*.mp4;*.flac;*.aac;*.ac3;*.wv;" +
                "*.au;*.aif;*.w64;*.voc;*.sf;*.paf;*.pvf;*.caf;*.svx ;" +
                "*.it;*.xm;*.s3m;*.mod;*.mtm;*.mo3;*.cda)|" +
                "*.mp3;*.mp2;*.wav;*.ogg;*.aiff;*.wma;*.wmv;*.asx;*.asf;" +
                "*.m4a;*.mp4;*.flac;*.aac;*.ac3;*.wv;" +
                "*.au;*.aif;*.w64;*.voc;*.sf;*.paf;*.pvf;*.caf;*.svx ;" +
                "*.it;*.xm;*.s3m;*.mod;*.mtm;*.mo3;*.cda|" +
                "MP3 and MP2 sounds (*.mp3;*.mp2)|*.mp3;*.mp2|" +
                "AAC and MP4 sounds (*.aac;*.mp4)|*.aac;*.mp4|" +
                "WAV sounds (*.wav)|*.wav|" +
                "OGG Vorbis sounds (*.ogg)|*.ogg|" +
                "AIFF sounds (*.aiff)|*.aiff|" +
                "Windows Media sounds (*.wma;*.wmv;*.asx;*.asf)|*.wma;*.wmv;*.asx;*.asf|" +
                "AC3 sounds (*.ac3)|*.ac3;|" +
                "ALAC sounds (*.m4a)|*.ac3;|" +
                "FLAC sounds (*.flac)|*.flac;|" +
                "WavPack sounds (*.wv)|*.wv;|" +
                "All files (*.*)|*.*";
            openFileDialog1.Title = "Load a sound file";
            openFileDialog1.FileName = "";

            result = openFileDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            // set append mode and get sound data from the chosen file
            audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_APPEND);
            if (audioSoundEditor1.LoadSound(openFileDialog1.FileName) != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show("Cannot load file " + openFileDialog1.FileName);

        }

        private void mnuAudioLoad_Click(object sender, EventArgs e)
        {
            // due to the fact that a Load operation will discard any existing sound,
            // check if there is already a Loading session available
            DialogResult result;
            if (audioSoundEditor1.GetSoundDuration() > 0)
            {
                // ask the user if he wants to go on
                result = MessageBox.Show("קיים קובץ שמע בזיכרון, האם בכל זאת ליצור חדש ?", "MyMentor", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
            }

            // free the actual editing session
            audioSoundEditor1.CloseSound();

            // reset formatted strings
            LabelRangeBegin.Text = "00:00:00.000";
            LabelRangeEnd.Text = "00:00:00.000";
            LabelRangeDuration.Text = "00:00:00.000";
            LabelSelectionBegin.Text = "00:00:00.000";
            LabelSelectionEnd.Text = "00:00:00.000";
            LabelSelectionDuration.Text = "00:00:00.000";
            LabelTotalDuration.Text = "00:00:00.000";

            openFileDialog1.Filter =
                "Supported Sounds (*.mp3;*.mp2;*.wav;*.ogg;*.aiff;*.wma;*.wmv;*.asx;*.asf;" +
                "*.m4a;*.mp4;*.flac;*.aac;*.ac3;*.wv;" +
                "*.au;*.aif;*.w64;*.voc;*.sf;*.paf;*.pvf;*.caf;*.svx ;" +
                "*.it;*.xm;*.s3m;*.mod;*.mtm;*.mo3;*.cda)|" +
                "*.mp3;*.mp2;*.wav;*.ogg;*.aiff;*.wma;*.wmv;*.asx;*.asf;" +
                "*.m4a;*.mp4;*.flac;*.aac;*.ac3;*.wv;" +
                "*.au;*.aif;*.w64;*.voc;*.sf;*.paf;*.pvf;*.caf;*.svx ;" +
                "*.it;*.xm;*.s3m;*.mod;*.mtm;*.mo3;*.cda|" +
                "MP3 and MP2 sounds (*.mp3;*.mp2)|*.mp3;*.mp2|" +
                "AAC and MP4 sounds (*.aac;*.mp4)|*.aac;*.mp4|" +
                "WAV sounds (*.wav)|*.wav|" +
                "OGG Vorbis sounds (*.ogg)|*.ogg|" +
                "AIFF sounds (*.aiff)|*.aiff|" +
                "Windows Media sounds (*.wma;*.wmv;*.asx;*.asf)|*.wma;*.wmv;*.asx;*.asf|" +
                "AC3 sounds (*.ac3)|*.ac3;|" +
                "ALAC sounds (*.m4a)|*.ac3;|" +
                "FLAC sounds (*.flac)|*.flac;|" +
                "WavPack sounds (*.wv)|*.wv;|" +
                "All files (*.*)|*.*";
            openFileDialog1.Title = "Load a sound file";
            openFileDialog1.FileName = "";

            result = openFileDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            // load the new song
            audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_NEW);
            if (audioSoundEditor1.LoadSound(openFileDialog1.FileName) != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show("Cannot load file " + openFileDialog1.FileName);

        }

        private void mnuAudioSelectedPart_Cut_Click(object sender, EventArgs e)
        {
            // check if we have a selected range
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // if a selection is available
            if (!bSelectionAvailable)
                // no selection
                return;

            // check if we have simply selected a position (vertical dotted line)
            if (nBeginSelectionInMs == nEndSelectionInMs)
                // no selection
                return;

            // copy the selected range into the clipboard
            mnuAudioSelectedPart_Copy_Click(sender, e);

            // delete selected range
            mnuAudioSelectedPart_Delete_Click(sender, e);

        }

        private void mnuAudioSelectedPart_Copy_Click(object sender, EventArgs e)
        {
            // check if we have a selected range
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // if a selection is available
            if (!bSelectionAvailable)
                // no selection
                return;

            // check if we have simply selected a position (vertical dotted line)
            if (nBeginSelectionInMs == nEndSelectionInMs)
                // no selection
                return;

            // copy the selected range into the clipboard
            audioSoundEditor1.CopyRangeToClipboard(nBeginSelectionInMs, nEndSelectionInMs);

        }

        private void mnuAudioSelectedPart_Paste_Click(object sender, EventArgs e)
        {
            // check if we already have a Loading session in memory
            if (audioSoundEditor1.GetSoundDuration() > 0)
                // set append mode
                audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_APPEND);
            else
                // set create new mode
                audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_NEW);

            // paste clipboard contents
            if (audioSoundEditor1.LoadSoundFromClipboard() != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show("Cannot load from system clipboard");

        }

        private void mnuAudioSelectedPart_PasteInsertMode_Click(object sender, EventArgs e)
        {
            // check if we have a selected range
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // if a selection is available
            if (bSelectionAvailable)
                // paste at the given position
                audioSoundEditor1.SetInsertPos(nBeginSelectionInMs);
            else
                // paste at position 0
                audioSoundEditor1.SetInsertPos(0);

            // set insert mode
            audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_INSERT);

            // paste clipboard contents
            if (audioSoundEditor1.LoadSoundFromClipboard() != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show("Cannot paste from system clipboard");

        }

        private void mnuAudioSelectedPart_Delete_Click(object sender, EventArgs e)
        {
            // check if we have a selected range
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // if a selection is available
            if (!bSelectionAvailable)
                // no selection
                return;

            // delete the selected range
            audioSoundEditor1.DeleteRange(nBeginSelectionInMs, nEndSelectionInMs);

            // remove the actual selection
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(false, 0, 0);

        }

        private void mnuAudioSelectedPart_Reduce_Click(object sender, EventArgs e)
        {
            // check if we have a selected range
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // if a selection is available
            if (!bSelectionAvailable)
                // no selection
                return;

            // remove the actual selection
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(false, 0, 0);

            // reduce the actual session to the given range
            audioSoundEditor1.ReduceToRange(nBeginSelectionInMs, nEndSelectionInMs);

        }

        private void mnuAudioSelectedPart_SelectAll_Click(object sender, EventArgs e)
        {
            // select the whole sound on the waveform analyzer
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, -1);
        }

        private void mnuAudioSelectedPart_Remove_Click(object sender, EventArgs e)
        {
            // remove the selection from the waveform analyzer
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(false, 0, -1);

        }

        private void mnuAudioOptions_ApplyBackgroundSound_Click(object sender, EventArgs e)
        {
            FormChooseBackgroundFile frmChooseBackgroundFile = new FormChooseBackgroundFile();
            DialogResult result;
            result = frmChooseBackgroundFile.ShowDialog(this);
            if ((result == DialogResult.Cancel) || (frmChooseBackgroundFile.m_strPathname == ""))
                return;

            // set Loading in "mix" mode
            audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_MIX);

            // check if we have a selected range
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            if (!bSelectionAvailable)
            {
                // no selection, apply the background sound to the whole Loading session
                nBeginSelectionInMs = 0;
                nEndSelectionInMs = -1;
            }
            else
            {
                // check if we have simply selected a position (vertical dotted line)
                if (nBeginSelectionInMs == nEndSelectionInMs)
                    // apply background sound from the selected position to the end of the actual Loading session
                    nEndSelectionInMs = -1;
            }

            // set the mixing parameters
            audioSoundEditor1.SetMixingPos(nBeginSelectionInMs, nEndSelectionInMs);
            audioSoundEditor1.SetMixingParams(false, frmChooseBackgroundFile.m_bLoop, 100, 100, enumVolumeScales.SCALE_LINEAR);

            // start Loading from the given file
            if (audioSoundEditor1.LoadSound(frmChooseBackgroundFile.m_strPathname) != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show("Cannot load file");
        }

        private void mnuAudioOptions_InsertSilent_Click(object sender, EventArgs e)
        {
            FormSilence frmSilence = new FormSilence();
            DialogResult result;
            result = frmSilence.ShowDialog(this);
            if ((result == DialogResult.Cancel) || (frmSilence.m_nSilenceLengthInMs == -1))
                return;

            // check if there is a selection on the analyzer
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // remove the actual selection
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(false, 0, 0);

            // insert the requested silence
            if (bSelectionAvailable)
                audioSoundEditor1.InsertSilence(nBeginSelectionInMs, frmSilence.m_nSilenceLengthInMs);
            else
                audioSoundEditor1.InsertSilence(0, frmSilence.m_nSilenceLengthInMs);
        }

        private void mnuAudioOptions_InsertSoundFile_Click(object sender, EventArgs e)
        {
            DialogResult result;
            openFileDialog1.Filter =
                "Supported Sounds (*.mp3;*.mp2;*.wav;*.ogg;*.aiff;*.wma;*.wmv;*.asx;*.asf;" +
                "*.m4a;*.mp4;*.flac;*.aac;*.ac3;*.wv;" +
                "*.au;*.aif;*.w64;*.voc;*.sf;*.paf;*.pvf;*.caf;*.svx ;" +
                "*.it;*.xm;*.s3m;*.mod;*.mtm;*.mo3;*.cda)|" +
                "*.mp3;*.mp2;*.wav;*.ogg;*.aiff;*.wma;*.wmv;*.asx;*.asf;" +
                "*.m4a;*.mp4;*.flac;*.aac;*.ac3;*.wv;" +
                "*.au;*.aif;*.w64;*.voc;*.sf;*.paf;*.pvf;*.caf;*.svx ;" +
                "*.it;*.xm;*.s3m;*.mod;*.mtm;*.mo3;*.cda|" +
                "MP3 and MP2 sounds (*.mp3;*.mp2)|*.mp3;*.mp2|" +
                "AAC and MP4 sounds (*.aac;*.mp4)|*.aac;*.mp4|" +
                "WAV sounds (*.wav)|*.wav|" +
                "OGG Vorbis sounds (*.ogg)|*.ogg|" +
                "AIFF sounds (*.aiff)|*.aiff|" +
                "Windows Media sounds (*.wma;*.wmv;*.asx;*.asf)|*.wma;*.wmv;*.asx;*.asf|" +
                "AC3 sounds (*.ac3)|*.ac3;|" +
                "ALAC sounds (*.m4a)|*.ac3;|" +
                "FLAC sounds (*.flac)|*.flac;|" +
                "WavPack sounds (*.wv)|*.wv;|" +
                "All files (*.*)|*.*";
            openFileDialog1.Title = "Load a sound file";
            openFileDialog1.FileName = "";

            result = openFileDialog1.ShowDialog();
            if (result == DialogResult.Cancel)
                return;

            // check if there is a selection on the analyzer
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // predispose the insert position
            if (bSelectionAvailable)
                audioSoundEditor1.SetInsertPos(nBeginSelectionInMs);
            else
                audioSoundEditor1.SetInsertPos(0);

            // set insert mode and get sound data from the chosen file
            audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_INSERT);
            if (audioSoundEditor1.LoadSound(openFileDialog1.FileName) != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show("Cannot load file " + openFileDialog1.FileName);
        }

        private void mnuZoom_Selection_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomToSelection(true);

        }

        private void mnuZoom_AllClip_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomToFullSound();
        }

        private void mnuZoom_In_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomIn();
        }

        private void mnuZoom_Out_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomOut();

        }

        private void FormMain_Resize(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.Destroy();

            if (tabControl1.SelectedIndex == 2)
            {
                // create the waveform analyzer (always call this function on the end of the form's Load fucntion)
                audioSoundEditor1.DisplayWaveformAnalyzer.Create(panel4.Handle, pictureBox1.Left, pictureBox1.Top, panel4.Width, panel4.Height);
            }
            else
            {
                // create the waveform analyzer (always call this function on the end of the form's Load fucntion)
                audioSoundEditor1.DisplayWaveformAnalyzer.Create(panel1.Handle, Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);
            }

            TimerReload.Enabled = true;

            PaintGraphics();
        }

        private void FormMain_ResizeEnd(object sender, EventArgs e)
        {
        }

        private void tbrOpen_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (MessageBox.Show("השיעור לא נשמר מהשינויים האחרונים.\n\nהאם אתה בטוח להמשיך?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    OpenClip();
                    mnuFile_SaveAs.Enabled = true;
                }
            }
            else
            {
                OpenClip();
                mnuFile_SaveAs.Enabled = true;
            }

        }

        private void tbrSave_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void DevideText()
        {
            try
            {
                var paragraphs_local = new List<Paragraph>();
                int paragraphIndex = -1;
                int sectionIndex = -1;
                int sentenceIndex = -1;
                int wordIndex = -1;

                int innerSentenceIndex = -1;
                int innerSectionIndex = -1;

                TimeSpan nextStartTime = TimeSpan.Zero;
                TimeSpan nextParagraphDuration = TimeSpan.Zero;
                TimeSpan nextSentenceDuration = TimeSpan.Zero;
                TimeSpan nextSectionDuration = TimeSpan.Zero;

                IEnumerable<Word> allWords = null;

                if (m_chapter != null && m_chapter.Paragraphs != null)
                {
                    allWords = m_chapter.Paragraphs.FlattenWords();
                }


                int bufferIndex = 0;

                List<SectionMatch> matchesParagraphs = m_regexParagraphs.Matches(richTextBox1.Text).Cast<Match>()
                            .Select(m => m.Groups[1])
                            .Select(m => new SectionMatch()
                            {
                                CharIndex = m.Index,
                                Length = m.Length,
                                Value = m.Value
                            })
                            .ToList();

                if (matchesParagraphs.Count == 0)
                {
                    matchesParagraphs.Add(new SectionMatch()
                    {
                        CharIndex = 0,
                        Length = richTextBox1.Text.Length,
                        Value = richTextBox1.Text
                    });
                }

                foreach (SectionMatch matchParagraph in matchesParagraphs)
                {
                    paragraphIndex++;
                    innerSentenceIndex = -1;

                    TimeSpan start = TimeSpan.Zero;
                    TimeSpan duration = TimeSpan.Zero;
                    bool startManually = false;
                    bool durationManually = false;
                    Paragraph ex_paragraph = null;

                    //check for saved schedule
                    if (m_chapter != null && m_chapter.Paragraphs != null)
                    {
                        ex_paragraph = m_chapter.Paragraphs.Where(p => p.Index == paragraphIndex).FirstOrDefault();
                    }

                    if (ex_paragraph != null)
                    {
                        start = ex_paragraph.StartTime;
                        duration = ex_paragraph.Duration;
                        startManually = ex_paragraph.ManuallyStartDate;
                        durationManually = ex_paragraph.ManuallyDuration;
                    }

                    paragraphs_local.Add(new Paragraph
                    {
                        Content = matchParagraph.StrippedValue,
                        RealCharIndex = matchParagraph.CharIndex,
                        CharIndex = matchParagraph.CharIndex - bufferIndex,
                        ManuallyStartDate = startManually,
                        ManuallyDuration = durationManually,
                        Index = paragraphIndex,
                        Sentences = new List<Sentence>(),
                        Words = new List<Word>(),
                        StartTime = start,
                        Duration = duration,
                    });

                    bufferIndex += 3;

                    List<SectionMatch> matchesSentenses = m_regexSentenses.Matches(matchParagraph.Value).Cast<Match>()
                        .Select(m => m.Groups[1])
                        .Select(m => new SectionMatch()
                        {
                            CharIndex = m.Index,
                            Length = m.Length,
                            Value = m.Value
                        })
                        .ToList();

                    if (matchesSentenses.Count == 0)
                    {
                        matchesSentenses.Add(matchParagraph);
                    }

                    foreach (SectionMatch matchSentense in matchesSentenses)
                    {
                        start = TimeSpan.Zero;
                        duration = TimeSpan.Zero;
                        startManually = false;
                        durationManually = false;
                        Sentence ex_sentence = null;

                        sentenceIndex++;
                        innerSentenceIndex++;
                        innerSectionIndex = -1;

                        //check for saved schedule
                        if (m_chapter != null && m_chapter.Paragraphs != null)
                        {
                            ex_sentence = m_chapter.Paragraphs.SelectMany(s => s.Sentences).Where(p => p.Index == sentenceIndex).FirstOrDefault();
                        }

                        if (ex_sentence != null)
                        {
                            start = ex_sentence.StartTime;
                            duration = ex_sentence.Duration;
                            startManually = ex_sentence.ManuallyStartDate;
                            durationManually = ex_sentence.ManuallyDuration;
                        }

                        int sectionsOffset = 0;
                        int wordsOffset = 0;

                        if (innerSentenceIndex > 0)
                        {
                            sectionsOffset = Math.Max(0, paragraphs_local[paragraphIndex].Sentences.Take(innerSentenceIndex).SelectMany(p => p.Sections).Count() * 3 - 3);
                            wordsOffset = Math.Max(0, paragraphs_local[paragraphIndex].Sentences.Take(innerSentenceIndex).SelectMany(p => p.Sections).SelectMany(w => w.Words).Count() * 3);
                        }

                        paragraphs_local[paragraphIndex].Sentences.Add(new Sentence
                        {   //                     5                               +           15    - 4   - (4 * 1) - 2
                            Content = matchSentense.StrippedValue,
                            RealCharIndex = paragraphs_local[paragraphIndex].RealCharIndex + matchSentense.CharIndex,
                            CharIndex = paragraphs_local[paragraphIndex].CharIndex + matchSentense.CharIndex - wordsOffset,// - Math.Max(0, 3 * innerSentenceIndex ),
                            Index = sentenceIndex,
                            ManuallyDuration = durationManually,
                            ManuallyStartDate = startManually,
                            Sections = new List<Section>(),
                            Words = new List<Word>(),
                            StartTime = start,
                            Duration = duration
                        });

                        bufferIndex += innerSentenceIndex == 0 ? 0 : 3;

                        List<SectionMatch> matchesSections = m_regexSections.Matches(matchSentense.Value).Cast<Match>()
                            .Select(m => m.Groups[1])
                            .Select(m => new SectionMatch()
                            {
                                CharIndex = m.Index,
                                Length = m.Length,
                                Value = m.Value
                            })
                            .ToList();

                        if (matchesSentenses.Count == 0)
                        {
                            matchesSentenses.Add(matchSentense);
                        }

                        foreach (SectionMatch matchSection in matchesSections)
                        {
                            int innerWordIndex = -1;
                            sectionIndex++;
                            innerSectionIndex++;

                            start = TimeSpan.Zero;
                            duration = TimeSpan.Zero;
                            startManually = false;
                            durationManually = false;
                            Section ex_section = null;

                            //check for saved schedule
                            if (m_chapter != null && m_chapter.Paragraphs != null)
                            {
                                ex_section = m_chapter.Paragraphs.SelectMany(s => s.Sentences).SelectMany(se => se.Sections).Where(p => p.Index == sectionIndex).FirstOrDefault();
                            }

                            if (ex_section != null)
                            {
                                start = ex_section.StartTime;
                                duration = ex_section.Duration;
                                startManually = ex_section.ManuallyStartDate;
                                durationManually = ex_section.ManuallyDuration;
                            }

                            int groupWordsBuffer = Math.Max(0, paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections.SelectMany(s => s.Words).Count() * 3);

                            paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections.Add(new Section
                            {
                                Content = matchSection.StrippedValue,
                                ManuallyDuration = durationManually,
                                ManuallyStartDate = startManually,
                                RealCharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].RealCharIndex + matchSection.CharIndex,
                                CharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].CharIndex + matchSection.CharIndex
                                    //- (3 * innerSectionIndex) 
                                - groupWordsBuffer,
                                Index = sectionIndex,
                                Words = new List<Word>(),
                                StartTime = start,
                                Duration = duration
                            });

                            bufferIndex += innerSectionIndex == 0 ? 0 : 3;

                            List<SectionMatch> matchesWords = m_regexWords.Matches(matchSection.Value).Cast<Match>()
                                .Select(m => m.Groups[1])
                                .Select(m => new SectionMatch()
                                {
                                    CharIndex = m.Index,
                                    Length = m.Length,
                                    Value = m.Value
                                })
                                .ToList();

                            if (matchesWords.Count == 0)
                            {
                                matchesWords.Add(matchSection);
                            }

                            foreach (SectionMatch matchWord in matchesWords)
                            {
                                wordIndex++;
                                innerWordIndex++;

                                paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Words.Add(new Word
                                {
                                    RealCharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].RealCharIndex + matchWord.CharIndex,
                                    CharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].CharIndex + matchWord.CharIndex
                                        // - (3 * Math.Max( 0, innerWordIndex)),
                                    - (3 * Math.Max(0, paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Words.Count())),
                                    IsInGroup = true, //matchWord.Groups["group"].Success,
                                    Index = wordIndex,
                                    Content = matchWord.Value,
                                    StartTime = start,
                                    Duration = duration
                                });

                                bufferIndex += innerWordIndex == 0 ? 0 : 3;
                            }
                        }
                    }
                }

                m_chapter = new Chapter();
                m_chapter.Words = new List<Word>();
                m_chapter.Content = m_regexAll.Replace(richTextBox1.Text, string.Empty);
                m_chapter.Paragraphs = paragraphs_local;
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }


        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            PaintGraphics();
            Clip.Current.IsDirty = true;

        }

        private void tbrParagraph_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionStart == 0)
            {
                return;
            }

            richTextBox2.Rtf = richTextBox1.Rtf;

            string remember = richTextBox1.Text;
            var selectionIndex = richTextBox1.SelectionStart;

            while (selectionIndex > 0 &&
                selectionIndex < remember.Length &&
                remember.Substring(selectionIndex, 1) != " " &&
                remember.Substring(selectionIndex, 1) != ":" &&
                remember.Substring(selectionIndex, 1) != "\n")
            {
                selectionIndex--;
            }

            //check another or same anchor
            richTextBox2.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
            richTextBox2.SelectionLength = 3;

            if (richTextBox2.SelectedText == "[3]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "";
                return;
            }
            else if (richTextBox2.SelectedText == "[2]" || richTextBox2.SelectedText == "[1]" || richTextBox2.SelectedText == "[0]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "[3]";
            }
            else
            {
                richTextBox1.SelectionStart = selectionIndex;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectedText = "[3]";
            }

            PaintGraphics();


        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            PaintGraphics();

        }


        private void PaintGraphics()
        {
            richTextBox1.Refresh();
            richTextBox3.Refresh();
            richTextBox2.Rtf = richTextBox1.Rtf;

            int index = richTextBox2.Find("[3]", 0, RichTextBoxFinds.None);

            while (index >= 0)
            {
                PaintAnchor(AnchorType.Paragraph, index);
                index = richTextBox2.Find("[3]", index + 3, RichTextBoxFinds.None);

                if (index + 3 >= richTextBox2.TextLength)
                {
                    break;
                }
            }

            index = richTextBox2.Find("[2]", 0, RichTextBoxFinds.None);

            while (index >= 0)
            {
                PaintAnchor(AnchorType.Sentence, index);
                index = richTextBox2.Find("[2]", index + 3, RichTextBoxFinds.None);

                if (index + 3 >= richTextBox2.TextLength)
                {
                    break;
                }
            }

            index = richTextBox2.Find("[1]", 0, RichTextBoxFinds.None);

            while (index >= 0)
            {
                PaintAnchor(AnchorType.Section, index);
                index = richTextBox2.Find("[1]", index + 3, RichTextBoxFinds.None);

                if (index + 3 >= richTextBox2.TextLength)
                {
                    break;
                }
            }

            index = richTextBox2.Find("[0]", 0, RichTextBoxFinds.None);

            while (index >= 0)
            {
                PaintAnchor(AnchorType.Word, index);
                index = richTextBox2.Find("[0]", index + 3, RichTextBoxFinds.None);

                if (index + 3 >= richTextBox2.TextLength)
                {
                    break;
                }
            }
        }

        private void PaintAnchor(AnchorType type, int index)
        {
            Color c;
            Pen cp = new Pen(Color.Beige);
            string number = "";

            switch (type)
            {
                case AnchorType.Paragraph:
                    c = Color.Red;
                    number = "[3]";
                    break;
                case AnchorType.Sentence:
                    c = Color.Violet;
                    number = "[2]";
                    break;
                case AnchorType.Section:
                    c = Color.LimeGreen;
                    number = "[1]";
                    break;
                case AnchorType.Word:
                    c = Color.Yellow;
                    number = "[0]";
                    break;
                default:
                    c = Color.Black;
                    number = "";
                    break;
            }

            SolidBrush cb = new SolidBrush(c);
            int factor1 = 0;
            int factor2 = 0;
            int factor3 = 0;

            var positionMainEditor = richTextBox1.GetPositionFromCharIndex(index);
            richTextBox2.SelectionStart = index;// -3;
            richTextBox2.SelectionLength = 3;

            int width = Convert.ToInt32(rtbMainEditorGraphics.MeasureString(number, richTextBox2.SelectionFont).Width);

            if (richTextBox2.SelectionFont.Size >= 32)
            {
                factor1 = 25;
                factor2 = -12;
                factor3 = 20;
            }
            else if (richTextBox2.SelectionFont.Size >= 16 && richTextBox2.SelectionFont.Size < 32)
            {
                factor1 = 13;
                factor2 = -7;
                factor3 = 10;
            }
            else if (richTextBox2.SelectionFont.Size < 16)
            {
                factor1 = 11;
                factor2 = -6;
                factor3 = 8;
            }

            // rectangle to specify which region to paint too
            Rectangle r1 = new Rectangle();

            // specify dimensions
            r1.X = positionMainEditor.X + factor1 - width;
            r1.Y = positionMainEditor.Y;
            r1.Width = width + factor2;
            r1.Height =
                Convert.ToInt32(richTextBox2.SelectionFont.Height * richTextBox2.ZoomFactor);

            rtbMainEditorGraphics.DrawRectangle(cp, r1);
            rtbMainEditorGraphics.FillRectangle(cb, r1);

            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(type == AnchorType.Word ? System.Drawing.Color.Black : System.Drawing.Color.White);
            float x = positionMainEditor.X + factor3 - width;
            float y = positionMainEditor.Y;
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
            rtbMainEditorGraphics.DrawString(number, richTextBox2.SelectionFont, drawBrush, x, y, drawFormat);

            if (tabControl1.SelectedIndex == 2)
            {
                var positionAlternateEditor = richTextBox3.GetPositionFromCharIndex(index + 3);
                r1.X = positionAlternateEditor.X + factor1 - width;
                r1.Y = positionAlternateEditor.Y;

                //anchor
                if (index + 3 == richTextBox3.SelectionStart)
                {
                    cb = new SolidBrush(SystemColors.Highlight);
                    drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                }

                rtbAlternateEditorGraphics.DrawRectangle(cp, r1);
                rtbAlternateEditorGraphics.FillRectangle(cb, r1);

                x = positionAlternateEditor.X + factor3 - width;
                y = positionAlternateEditor.Y;
                drawFormat = new System.Drawing.StringFormat();
                rtbAlternateEditorGraphics.DrawString(number, richTextBox2.SelectionFont, drawBrush, x, y, drawFormat);
            }
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            PaintGraphics();

        }

        private void tbrSentense_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionStart == 0)
            {
                return;
            }

            string remember = richTextBox1.Text;
            var selectionIndex = richTextBox1.SelectionStart;

            while (selectionIndex > 0 &&
                 selectionIndex < remember.Length &&
               remember.Substring(selectionIndex, 1) != " " &&
                remember.Substring(selectionIndex, 1) != ":" &&
                remember.Substring(selectionIndex, 1) != "\n")
            {
                selectionIndex--;
            }

            //check another or same anchor
            richTextBox2.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
            richTextBox2.SelectionLength = 3;

            if (richTextBox2.SelectedText == "[2]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "";
                return;
            }
            else if (richTextBox2.SelectedText == "[3]" || richTextBox2.SelectedText == "[1]" || richTextBox2.SelectedText == "[0]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "[2]";
            }
            else
            {
                richTextBox1.SelectionStart = selectionIndex;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectedText = "[2]";
            }

            PaintGraphics();
        }

        private void tbrSection_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionStart == 0)
            {
                return;
            }

            string remember = richTextBox1.Text;
            var selectionIndex = richTextBox1.SelectionStart;

            while (selectionIndex > 0 &&
                  selectionIndex < remember.Length &&
              remember.Substring(selectionIndex, 1) != " " &&
                remember.Substring(selectionIndex, 1) != ":" &&
                remember.Substring(selectionIndex, 1) != "\n")
            {
                selectionIndex--;
            }

            //check another or same anchor
            richTextBox2.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
            richTextBox2.SelectionLength = 3;

            if (richTextBox2.SelectedText == "[1]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "";
                return;
            }
            else if (richTextBox2.SelectedText == "[3]" || richTextBox2.SelectedText == "[2]" || richTextBox2.SelectedText == "[0]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "[1]";
            }
            else
            {
                richTextBox1.SelectionStart = selectionIndex;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectedText = "[1]";
            }

            PaintGraphics();
        }

        private void tbrBiggerFont_Click(object sender, EventArgs e)
        {
            try
            {
                if (sizeIndex >= 0 && sizeIndex < sizes.Length)
                {
                    sizeIndex++;
                    richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, sizes[sizeIndex]);
                }
            }
            catch
            {

            }
        }

        private void tbrSmallerFont_Click(object sender, EventArgs e)
        {
            try
            {
                if (sizeIndex > 0)
                {
                    sizeIndex--;
                    richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, sizes[sizeIndex]);
                }
            }
            catch
            {

            }
        }

        private void tbrWord_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionStart == 0)
            {
                return;
            }

            string remember = richTextBox1.Text;
            var selectionIndex = richTextBox1.SelectionStart;

            while (selectionIndex > 0 &&
                   selectionIndex < remember.Length &&
             remember.Substring(selectionIndex, 1) != " " &&
                remember.Substring(selectionIndex, 1) != ":" &&
                remember.Substring(selectionIndex, 1) != "\n")
            {
                selectionIndex--;
            }

            //check another or same anchor
            richTextBox2.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
            richTextBox2.SelectionLength = 3;

            if (richTextBox2.SelectedText == "[0]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "";
                return;
            }
            else if (richTextBox2.SelectedText == "[3]" || richTextBox2.SelectedText == "[2]" || richTextBox2.SelectedText == "[1]")
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = "[0]";
            }
            else
            {
                richTextBox1.SelectionStart = selectionIndex;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectedText = "[0]";
            }

            PaintGraphics();
        }

        private void mnuHelp_ShowJSON_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsNew)
            {
                MessageBox.Show("Please save clip first");
                return;
            }

            DevideText();

            //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
            //Clip.Current.FontName = richTextBox1.Font.Name;
            Clip.Current.Chapter = m_chapter;
            Clip.Current.Save();

            JsonDebugFrm frm = new JsonDebugFrm();
            frm.ShowDialog();

        }

        private void mnuFile_Save_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            Save(false);
        }

        private void Save(bool isSaveAs)
        {
            if (Clip.Current.IsNew || (!Clip.Current.IsNew && isSaveAs))
            {
                if (isSaveAs)
                {
                    FileInfo fi = new FileInfo(Clip.Current.FileName);
                    saveFileDialog1.InitialDirectory = fi.DirectoryName;
                    saveFileDialog1.FileName = fi.Name;//  .FullName;
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyMentor Clips"));

                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    saveFileDialog1.InitialDirectory = di.FullName;
                    saveFileDialog1.FileName = Clip.Current.Title.ToValidFileName();
                }
                saveFileDialog1.DefaultExt = "mmnx";
                saveFileDialog1.Filter = "MyMentor Source Files|*.mmnx";

                DialogResult result = saveFileDialog1.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    if (isSaveAs)
                    {
                        Clip.Current.ID = Guid.NewGuid();
                    }

                    //Clip.Current.FontName = richTextBox1.Font.Name;
                    //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
                    Clip.Current.FileName = saveFileDialog1.FileName;
                    Clip.Current.RtfText = richTextBox1.Rtf;
                    this.Text = "MyMentor - " + Clip.Current.FileName;
                    Clip.Current.Chapter = m_chapter;
                    Clip.Current.Save();

                    MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    mnuFile_SaveAs.Enabled = true;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Clip.Current.RtfText = richTextBox1.Rtf;
                Clip.Current.Chapter = m_chapter;
                //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
                //Clip.Current.FontName = richTextBox1.Font.Name;
                Clip.Current.Save();

                MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
        }

        private void tbrParse_Click(object sender, EventArgs e)
        {
            DevideText();
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            richTextBox1.Focus();

            //if (ParseUser.CurrentUser == null)
            //{
            //    LoginForm frmLogin = new LoginForm(this);
            //    frmLogin.ShowDialog();
            //}
            //else
            //{
            //    lblLoginUser.Text = "מחובר כ-" + ParseUser.CurrentUser.Username;
            //}

            if (!string.IsNullOrEmpty(m_initClip))
            {
                OpenClip(m_initClip);
            }
            else
            {
                Clip.Current.Title = "שיעור 1";
                Clip.Current.IsDirty = false;
                Clip.Current.IsNew = true;
                Clip.Current.ID = Guid.NewGuid();
                this.Text = "MyMentor - " + Clip.Current.Title;
            }

        }

        private void OpenClip()
        {
            OpenClip(null);
        }

        private string GetFileDialog()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyMentor Clips"));

            if (!di.Exists)
            {
                di.Create();
            }

            openFileDialog1.InitialDirectory = di.FullName;
            openFileDialog1.DefaultExt = "mmnx";
            openFileDialog1.Filter = "MyMentor Source Files|*.mmnx";
            openFileDialog1.FileName = "";

            DialogResult result = openFileDialog1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return openFileDialog1.FileName;
            }
            else
            {
                return null;
            }
        }

        private void OpenClip(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                file = GetFileDialog();
            }

            if (string.IsNullOrEmpty(file))
            {
                return;
            }

            m_chapter = null;

            try
            {
                Clip.Load(file);
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("קובץ שיעור אינו תקין", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            this.Text = "MyMentor - " + file;

            richTextBox1.Rtf = Clip.Current.RtfText;

            if (Clip.Current.Chapter != null)
            {
                m_chapter = Clip.Current.Chapter;
            }
            //for old version
            else if (Clip.Current.Paragraphs != null)
            {
                m_chapter = new Chapter();
                m_chapter.Paragraphs = Clip.Current.Paragraphs;
            }

            DevideText();

            Clip.Current.IsDirty = false;
        }

        private void mnuText_Goto_Click(object sender, EventArgs e)
        {
            using (var form = new GotoForm())
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tabControl1.SelectedIndex = 0;
                    richTextBox1.SelectionStart = form.CharIndex;
                    richTextBox1.SelectionLength = 0;
                    richTextBox1.Focus();
                }
            }

        }

        private void mnuFile_Exit_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (MessageBox.Show("השיעור לא נשמר מהשינויים האחרונים.\n\nהאם אתה בטוח לצאת?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }

        }

        private void mnuHelp_About_Click(object sender, EventArgs e)
        {
            AboutBox frm = new AboutBox();
            frm.ShowDialog();

        }

        private short m_uniqueLine = 0;

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.Destroy();

            //copy text and insert pause anchor
            richTextBox3.Clear();
            richTextBox3.Rtf = richTextBox1.Rtf;
            richTextBox3.SelectionStart = 0;
            richTextBox3.SelectionLength = 0;
            richTextBox3.SelectedText = "[p]";

            m_selectedAnchor = false;

            //scheduler step
            if (tabControl1.SelectedIndex == 2)
            {
                DevideText();

                if (m_chapter.Paragraphs != null)
                {
                    m_selectedScheduledWord = m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(sc => sc.Sections)
                    .SelectMany(w => w.Words).FirstOrDefault();

                    if (m_selectedScheduledWord != null)
                    {
                        m_skipSelectionChange = true;
                        richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + 3;
                        richTextBox3.SelectionLength = m_selectedScheduledWord.Length;
                        m_skipSelectionChange = false;
                    }
                }

                //audioDjStudio1.InitSoundSystem(1, 0, 0, 0, 0);
                //AudioDjStudio.enumErrorCodes error = audioDjStudio1.LoadSoundFromEditingSession(0, audioSoundEditor1.Handle);

                // create the waveform analyzer (always call this function on the end of the form's Load fucntion)
                audioSoundEditor1.DisplayWaveformAnalyzer.Create(panel4.Handle, pictureBox1.Left, pictureBox1.Top, panel4.Width, panel4.Height);
                audioSoundEditor1.DisplayWaveformAnalyzer.MouseSelectionEnable(false);
            }
            else
            {
                // create the waveform analyzer (always call this function on the end of the form's Load fucntion)
                audioSoundEditor1.DisplayWaveformAnalyzer.Create(panel1.Handle, Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);
            }

            PaintGraphics();
            TimerReload.Enabled = true;
        }

        private void mnuFile_NewClip_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (Clip.Current.IsDirty &&
                    MessageBox.Show("השיעור לא נשמר מהשינויים האחרונים.\n\nהאם אתה בטוח להמשיך?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    NewClip();
                }
            }
            else
            {
                NewClip();
            }
        }

        private void NewClip()
        {
            Clip.Current = null;
            Clip.Current.AutoIncrementVersion = true;
            Clip.Current.Title = "שיעור 1";
            Clip.Current.Version = "1.00";
            Clip.Current.Status = "PENDING";
            Clip.Current.ID = Guid.NewGuid();
            Clip.Current.IsNew = true;
            Clip.Current.RightAlignment = true;

            this.Text = "MyMentor - " + Clip.Current.Title;

            richTextBox1.Rtf = null;
            Clip.Current.IsDirty = false;

            DevideText();
        }

        private void tbrNew_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (Clip.Current.IsDirty &&
                    MessageBox.Show("השיעור לא נשמר מהשינויים האחרונים.\n\nהאם אתה בטוח להמשיך?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    NewClip();
                }
            }
            else
            {
                NewClip();
            }
        }

        private void tbrFont_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(richTextBox1.SelectionFont == null))
                {
                    fontDialog1.Font = richTextBox1.SelectionFont;
                }
                else
                {
                    fontDialog1.Font = null;
                }
                fontDialog1.ShowApply = true;

                if (fontDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    richTextBox1.SelectionFont = fontDialog1.Font;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void tbrRight_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
            }
            catch
            {

            }
            //richTextBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            //Clip.Current.RightAlignment = true;
        }

        private void tbrLeft_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
            }
            catch
            {

            }
        }

        private void tbrBold_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(richTextBox1.SelectionFont == null))
                {
                    System.Drawing.Font currentFont = richTextBox1.SelectionFont;
                    System.Drawing.FontStyle newFontStyle;

                    newFontStyle = richTextBox1.SelectionFont.Style ^ FontStyle.Bold;

                    richTextBox1.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);

                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        private void tbrItalic_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(richTextBox1.SelectionFont == null))
                {
                    System.Drawing.Font currentFont = richTextBox1.SelectionFont;
                    System.Drawing.FontStyle newFontStyle;

                    newFontStyle = richTextBox1.SelectionFont.Style ^ FontStyle.Italic;

                    richTextBox1.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        private void tbrUnderline_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(richTextBox1.SelectionFont == null))
                {
                    System.Drawing.Font currentFont = richTextBox1.SelectionFont;
                    System.Drawing.FontStyle newFontStyle;

                    newFontStyle = richTextBox1.SelectionFont.Style ^ FontStyle.Underline;

                    richTextBox1.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, newFontStyle);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString(), "Error");
            }
        }

        private void richTextBox3_SelectionChanged(object sender, EventArgs e)
        {
            if (m_skipSelectionChange)
            {
                return;
            }

            int selectionIndex = Math.Max(0, richTextBox3.SelectionStart - 3);

            if (selectionIndex == 0) // pause
            {
                richTextBox3.SelectionStart = 0;
                richTextBox3.SelectionLength = 3;
            }
            else if (m_chapter.Paragraphs != null)
            {
                //check for word selection
                m_selectedScheduledWord = m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(sc => sc.Sections)
                    .SelectMany(w => w.Words).Where(ww => ww.RealCharIndex <= selectionIndex).LastOrDefault();

                if (m_selectedScheduledWord != null)
                {
                    m_skipSelectionChange = true;

                    //check for anchor
                    if (m_selectedScheduledWord.RealCharIndex + m_selectedScheduledWord.Length < selectionIndex)
                    {
                        richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + m_selectedScheduledWord.Length + 3;
                        richTextBox3.SelectionLength = 3;

                        m_selectedAnchor = true;
                    }
                    else
                    {
                        richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + 3;
                        richTextBox3.SelectionLength = m_selectedScheduledWord.Length;

                        //in case set start time during scheduling
                        if (m_setStartTime != TimeSpan.Zero)
                        {
                            m_selectedScheduledWord.StartTime = m_setStartTime;
                            m_setStartTime = TimeSpan.Zero;
                        }
                        else if (audioSoundEditor1.GetPlaybackStatus() != enumPlaybackStatus.PLAYBACK_PLAYING)
                        {
                            timePickerSpinner1.Value = m_selectedScheduledWord.StartTime;
                        }
                        
                        //move line
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_uniqueLine,(int) m_selectedScheduledWord.StartTime.TotalMilliseconds,(int) m_selectedScheduledWord.StartTime.TotalMilliseconds);
                        m_selectedAnchor = false;
                    }

                    m_skipSelectionChange = false;
                }
            }

            PaintGraphics();
        }

        private void audioDjStudio1_SilenceDetectionStateChange(object sender, AudioDjStudio.SilenceDetectionStateChangeEventArgs e)
        {
            label7.Text = string.Format("{0}:{1}", e.nPositionInMs, e.bIsSilent);
        }

        private void buttonStartDJPlay_Click(object sender, EventArgs e)
        {
            Int32 nDurationInMs = audioSoundEditor1.GetSoundDuration();
            if (nDurationInMs == 0)
            {
                return;
            }

            //audioDjStudio1.SilenceDetectionRealTimeEnable(0, true);
            //audioDjStudio1.SilenceDetectionRealTimeParamsSet(0, 800, 100);
            //AudioDjStudio.enumErrorCodes error1 = audioDjStudio1.PlaySound(0);
            if (buttonStartDJPlay.Text == "נגן")
            {
                audioSoundEditor1.PlaySoundRange( (int)timePickerSpinner1.Value.TotalMilliseconds, -1 );
                buttonStartDJPlay.Text = "עצור";

                timerUpdateTimePickerSpinner.Enabled = true;
                timePickerSpinner1.Enabled = false;
            }
            else if (buttonStartDJPlay.Text == "עצור")
            {
                timerUpdateTimePickerSpinner.Enabled = false;
                audioSoundEditor1.StopSound();
                buttonStartDJPlay.Text = "נגן";
            }

            //djLineTimer.Enabled = true;
        }

        private void djLineTimer_Tick(object sender, EventArgs e)
        {
            //double position = 0;
            //audioDjStudio1.SoundPositionGet(0, ref position, false);
            //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_uniqueLine, (int)position, (int)position);
        }

        private void audioDjStudio1_SoundDone(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            //djLineTimer.Enabled = false;
        }

        private void buttonHammer_Click(object sender, EventArgs e)
        {
            if (audioSoundEditor1.GetPlaybackStatus() == enumPlaybackStatus.PLAYBACK_PLAYING)
            {
                m_setStartTime = timePickerSpinner1.Value;

                richTextBox3.SelectionStart = richTextBox3.SelectionStart + richTextBox3.SelectionLength + 3;
            }
        }

        private void timePickerSpinner1_ValueChanged(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_uniqueLine, (int)timePickerSpinner1.Value.TotalMilliseconds, (int)timePickerSpinner1.Value.TotalMilliseconds);
        }


        private void audioSoundEditor1_WaveAnalyzerLineMoving(object sender, WaveAnalyzerLineMovingEventArgs e)
        {
            if (e.nUniqueID == m_uniqueLine)
            {
                timePickerSpinner1.Value = new TimeSpan(0, 0, 0, 0, e.nPosInMs);
            }
        }

        private void audioSoundEditor1_WaveAnalyzerHorzLineMoving(object sender, WaveAnalyzerHorzLineMovingEventArgs e)
        {

        }

        private void mnuLoadTest_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.LoadSound(@"C:\Users\Administrator\Documents\7923.mp3");
        }

        private void audioSoundEditor1_WaveAnalyzerMouseNotification(object sender, WaveAnalyzerMouseNotificationEventArgs e)
        {
            if (e.nAction == enumMouseActions.MOUSE_ACTION_LEFT_CLICK)
            {

            }
        }

        private void timerUpdateSpinnerControl_Tick(object sender, EventArgs e)
        {
            int mm = audioSoundEditor1.GetPlaybackPosition();
            var position = new TimeSpan(0, 0, 0, 0, mm); ;

            timePickerSpinner1.Value = position;

            //during playing check if the current position moved over to the next word
            if (m_chapter.Paragraphs != null && m_selectedScheduledWord != null && !m_selectedAnchor)
            {
                var nextWord = m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections)
                    .SelectMany(sc => sc.Words).FirstOrDefault(w => 
                        position >= w.StartTime
                        && w.StartTime > m_selectedScheduledWord.StartTime  );

                if (nextWord != null && nextWord != m_selectedScheduledWord)
                {
                    richTextBox3.SelectionStart = nextWord.RealCharIndex + 3;
                    //richTextBox3.SelectionLength = m_selectedScheduledWord.Length;

                }
            }
        }


    }

    public struct SectionMatch
    {
        private string m_value;

        public int CharIndex { get; set; }
        public int Length { get; set; }
        public AnchorType Type { get; set; }

        public string StrippedValue
        {
            get
            {
                return m_value.Replace("[0]", "").Replace("[1]", "").Replace("[2]", "").Replace("[3]", "");
            }
        }

        public string Value
        {
            get
            {
                return m_value;
            }

            set
            {
                m_value = value;
            }
        }
    }

    // define the data structure containing the parameters that will be passed to the external DSP
    // it's important that this data structure reflects exactly (also in terms of bytes length) the data structure
    // used inside the DLL contaning the external DSP
    [StructLayout(LayoutKind.Sequential)]
    public struct BASSBOOST_PARAMETERS
    {
        public Int32 nSampleRate;
        public Int16 nFrequencyHz;
        public Int16 nBoostdB;
    }

    // data structure for obtaining info about the DSP's user interface
    [StructLayout(LayoutKind.Sequential)]
    public struct DSP_EDITOR_WINDOW_INFO
    {
        public IntPtr hWnd;
        public byte bIsVisible;
        public Int32 nLeftPosPx;
        public Int32 nTopPosPx;
        public Int32 nWidthPx;
        public Int32 nHeightPx;
    }
}
