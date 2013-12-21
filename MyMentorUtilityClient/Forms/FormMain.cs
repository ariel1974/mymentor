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
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Parse;
using MyMentor;
using System.Globalization;
using System.Resources;
using System.Threading.Tasks;
using MyMentor.ParseObjects;

namespace MyMentor
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
        private bool m_bRecOverwriteMode;
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
        private ToolStripMenuItem mnuFile;
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
        private ToolStripMenuItem mnuFile_Parse;
        private ToolStripSeparator toolStripMenuItem11;
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
        private ToolStripButton tbrParse;
        public RichTextBox richTextBox1;
        private RichTextBox richTextBox2;
        #endregion

        #region Clip SetUp

        private TimeSpan n_hammerLastTimePressed = TimeSpan.Zero;
        private Word m_selectedScheduledWord = null;
        private bool m_selectedAnchor = false;
        private bool m_selectedStartAnchor = false;
        private bool m_selectedEndAnchor = false;
        private bool m_skipSelectionChange = false;
        private int m_waveFormTabIndex = 1;
        private string m_initClip = string.Empty;
        private bool m_admin = false;
        private bool m_loadingParse = true;
        private bool m_whileLoadingClip = false;

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
        private const string START_PAUSE_SECTION_ANCHOR = "[התחלה]";
        private const string END_PAUSE_SECTION_ANCHOR = "[סוף]";
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
        private TabPage tabPage4;
        public Label label1;
        private AudioDjStudio.AudioDjStudio audioDjStudio1;
        public GroupBox groupBox1;
        public Button buttonStartSchedulingPlayback;
        private Timer djLineTimer;
        private GroupBox groupBox2;
        private Label label9;
        public Button buttonHammer;
        private Timer timerUpdateTimePickerSpinner;
        private TableLayoutPanel tableLayoutPanel4;
        private ToolStripMenuItem תזמוןToolStripMenuItem;
        private ToolStripMenuItem mnuRemoveSchedule;
        public Label LabelCurrentSchedulingTimer;
        private Label label10;
        private TimeSpinner.TimePickerSpinner timePickerCurrentWord;
        public Button buttonRestartScheduling;
        private Panel panel5;
        private Label label7;
        private NumericUpDown numericUpDownInterval;
        public Button buttonScheduleAnchor;
        private Label label11;
        private ToolStripButton tbrProperties;
        public Label lblLoginUser;
        private ToolStripMenuItem mnuLoginDifferentUser;
        private ImageList imageList1;
        private Timer timerRecordIcon;
        private bool b_recordIconRed;
        private TableLayoutPanel tableLayoutPanel5;
        private GroupBox groupBox3;
        private TextBox textBox7;
        private Label label12;
        private GroupBox groupBox5;
        private CheckBox sop_teacher1l;
        private CheckBox sop_studentl;
        private CheckBox sop_teacher2l;
        private CheckBox sop_teacherAndStudentl;
        private GroupBox groupBox4;
        private CheckBox sop_teacher1;
        private CheckBox sop_student;
        private CheckBox sop_teacher2;
        private CheckBox sop_teacherAndStudent;
        private GroupBox groupBox6;
        private CheckBox loc_par;
        private CheckBox loc_wor;
        private CheckBox loc_sec;
        private CheckBox loc_sen;
        private GroupBox groupBox7;
        private CheckBox def_sen;
        private CheckBox def_par;
        private CheckBox def_wor;
        private CheckBox def_sec;
        private Label label13;
        private TextBox tbClipDescription;
        private Label label15;
        private Label label16;
        private Label label19;
        private TextBox tbKeywords;
        private Label lblCategory3;
        private Label label22;
        private TextBox tbClipName;
        private MaskedTextBox maskedTextBox1;
        private Label label23;
        private Panel panel4;
        private Button buttonPublish;
        private Panel panel6;
        public RichTextBox richTextBox3;
        private Panel panel7;
        private GroupBox groupBox8;
        private ToolStripSeparator toolStripMenuItem14;
        private ToolStripMenuItem mnuAnchors;
        private ToolStripMenuItem mnuAnchors_RemoveAll;
        private ToolStripSeparator toolStripMenuItem15;
        private ToolStripMenuItem mnuAnchors_RemoveParagraphs;
        private ToolStripMenuItem mnuAnchors_RemoveSentenses;
        private ToolStripMenuItem mnuAnchors_RemoveSections;
        private ToolStripMenuItem mnuAnchors_RemoveWords;
        public Label label14;
        private PresentationControls.CheckBoxComboBox comboBoxAutoDevideSen;
        public Label label24;
        public Button buttonAutoDevide;
        private PresentationControls.CheckBoxComboBox comboBoxAutoDevidePar;
        private RichTextBox richTextBox4;
        private string m_strExportPathname;
        private Label label25;
        private GroupBox groupBox9;
        private Label label26;
        private Label label27;
        private NumericUpDown numericUpDownBufferRecord;
        public Button buttonRecOverwritePlayback;
        private PictureBox pictureBox1;
        private Timer timerStartRecordingAfterPlayingBuffer;
        private TimeSpan m_rem_anchorFixRecording = TimeSpan.Zero;
        private DmitryBrant.CustomControls.SevenSegment sevenSegment1;
        private Timer timerRefreshLedDisplay;
        public Label LabelCurrentWordDuration;
        private Label label29;
        private ComboBox comboCategory3;
        private Panel panel8;
        private GroupBox groupBox10;
        private Label label28;
        private LinkLabel linkLabel1;
        private ComboBox comboStatus;
        public Label LabelTotalDuration2;
        private PictureBox pictureBox2;
        private Label label30;
        private TrackBar trackBarVolume1;
        private ToolStripSeparator toolStripMenuItem12;
        private ToolStripMenuItem mnuAudio_Test;
        private ToolStripMenuItem כליםToolStripMenuItem;
        private ToolStripMenuItem שפתממשקToolStripMenuItem;
        private ToolStripMenuItem mnuTools_UI_Hebrew;
        private ToolStripMenuItem mnuTools_UI_English;
        private ToolStripSeparator toolStripMenuItem16;
        private ComboBox comboCategory4;
        private Label lblCategory4;
        private Label lblCategory2;
        private ComboBox comboCategory2;
        private ComboBox comboCategory1;
        private Label lblCategory1;
        private int m_intRecordingDuration = 0;
        private Button btnRemoveDate;
        private Button btnAddDate;
        private ListBox listBoxDates;
        private Label label20;
        private DateTimePicker dtpReadingDate;
        private Timer timerPreStartFixPlayback;
        private Label lblClipType;
        private ComboBox comboClipType;
        private string m_clipTitlePattern;

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
            PresentationControls.CheckBoxProperties checkBoxProperties1 = new PresentationControls.CheckBoxProperties();
            PresentationControls.CheckBoxProperties checkBoxProperties2 = new PresentationControls.CheckBoxProperties();
            this.Frame4 = new System.Windows.Forms.GroupBox();
            this.label30 = new System.Windows.Forms.Label();
            this.trackBarVolume1 = new System.Windows.Forms.TrackBar();
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
            this.LabelStatus = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.TimerReload = new System.Windows.Forms.Timer(this.components);
            this.TimerMenuEnabler = new System.Windows.Forms.Timer(this.components);
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.timerDisplayWaveform = new System.Windows.Forms.Timer(this.components);
            this.audioSoundEditor1 = new AudioSoundEditor.AudioSoundEditor();
            this.FrameRecording = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.numericUpDownBufferRecord = new System.Windows.Forms.NumericUpDown();
            this.buttonRecOverwritePlayback = new System.Windows.Forms.Button();
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.buttonAutoDevide = new System.Windows.Forms.Button();
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.richTextBox4 = new System.Windows.Forms.RichTextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label28 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.LabelCurrentWordDuration = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.LabelCurrentSchedulingTimer = new System.Windows.Forms.Label();
            this.buttonRestartScheduling = new System.Windows.Forms.Button();
            this.buttonHammer = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonStartSchedulingPlayback = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownInterval = new System.Windows.Forms.NumericUpDown();
            this.buttonScheduleAnchor = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.dtpReadingDate = new System.Windows.Forms.DateTimePicker();
            this.btnRemoveDate = new System.Windows.Forms.Button();
            this.btnAddDate = new System.Windows.Forms.Button();
            this.listBoxDates = new System.Windows.Forms.ListBox();
            this.label20 = new System.Windows.Forms.Label();
            this.comboCategory4 = new System.Windows.Forms.ComboBox();
            this.lblCategory4 = new System.Windows.Forms.Label();
            this.lblCategory2 = new System.Windows.Forms.Label();
            this.comboCategory2 = new System.Windows.Forms.ComboBox();
            this.comboCategory1 = new System.Windows.Forms.ComboBox();
            this.lblCategory1 = new System.Windows.Forms.Label();
            this.LabelTotalDuration2 = new System.Windows.Forms.Label();
            this.comboStatus = new System.Windows.Forms.ComboBox();
            this.comboCategory3 = new System.Windows.Forms.ComboBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.sop_teacher1l = new System.Windows.Forms.CheckBox();
            this.sop_studentl = new System.Windows.Forms.CheckBox();
            this.sop_teacher2l = new System.Windows.Forms.CheckBox();
            this.sop_teacherAndStudentl = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.sop_teacher1 = new System.Windows.Forms.CheckBox();
            this.sop_student = new System.Windows.Forms.CheckBox();
            this.sop_teacher2 = new System.Windows.Forms.CheckBox();
            this.sop_teacherAndStudent = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.loc_par = new System.Windows.Forms.CheckBox();
            this.loc_wor = new System.Windows.Forms.CheckBox();
            this.loc_sec = new System.Windows.Forms.CheckBox();
            this.loc_sen = new System.Windows.Forms.CheckBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.def_sen = new System.Windows.Forms.CheckBox();
            this.def_par = new System.Windows.Forms.CheckBox();
            this.def_wor = new System.Windows.Forms.CheckBox();
            this.def_sec = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tbClipDescription = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.tbKeywords = new System.Windows.Forms.TextBox();
            this.lblCategory3 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.tbClipName = new System.Windows.Forms.TextBox();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.buttonPublish = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.Picture1 = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_NewClip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFile_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFile_Parse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFile_Exit = new System.Windows.Forms.ToolStripMenuItem();
            this.כליםToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.שפתממשקToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools_UI_Hebrew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools_UI_English = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuLoginDifferentUser = new System.Windows.Forms.ToolStripMenuItem();
            this.טקסטToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuText_Goto = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAnchors = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAnchors_RemoveAll = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAnchors_RemoveParagraphs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAnchors_RemoveSentenses = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAnchors_RemoveSections = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAnchors_RemoveWords = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAudio_Test = new System.Windows.Forms.ToolStripMenuItem();
            this.תזמוןToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemoveSchedule = new System.Windows.Forms.ToolStripMenuItem();
            this.עזרהToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp_About = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelp_ShowJSON = new System.Windows.Forms.ToolStripMenuItem();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.audioDjStudio1 = new AudioDjStudio.AudioDjStudio();
            this.djLineTimer = new System.Windows.Forms.Timer(this.components);
            this.timerUpdateTimePickerSpinner = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.lblLoginUser = new System.Windows.Forms.Label();
            this.timerRecordIcon = new System.Windows.Forms.Timer(this.components);
            this.timerStartRecordingAfterPlayingBuffer = new System.Windows.Forms.Timer(this.components);
            this.timerRefreshLedDisplay = new System.Windows.Forms.Timer(this.components);
            this.timerPreStartFixPlayback = new System.Windows.Forms.Timer(this.components);
            this.comboBoxAutoDevidePar = new PresentationControls.CheckBoxComboBox();
            this.comboBoxAutoDevideSen = new PresentationControls.CheckBoxComboBox();
            this.sevenSegment1 = new DmitryBrant.CustomControls.SevenSegment();
            this.timePickerCurrentWord = new MyMentor.TimeSpinner.TimePickerSpinner();
            this.comboClipType = new System.Windows.Forms.ComboBox();
            this.lblClipType = new System.Windows.Forms.Label();
            this.Frame4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume1)).BeginInit();
            this.framePlayback.SuspendLayout();
            this.FrameRecording.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBufferRecord)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.ToolStrip1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.panel8.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).BeginInit();
            this.panel6.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Picture1)).BeginInit();
            this.panel3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // Frame4
            // 
            this.Frame4.BackColor = System.Drawing.SystemColors.Control;
            this.Frame4.Controls.Add(this.label30);
            this.Frame4.Controls.Add(this.trackBarVolume1);
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
            this.Frame4.Font = new System.Drawing.Font("Arial", 12F);
            this.Frame4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Frame4.Location = new System.Drawing.Point(3, 25);
            this.Frame4.Name = "Frame4";
            this.Frame4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Frame4.Size = new System.Drawing.Size(551, 208);
            this.Frame4.TabIndex = 17;
            this.Frame4.TabStop = false;
            this.Frame4.Text = "מיקום";
            // 
            // label30
            // 
            this.label30.Location = new System.Drawing.Point(16, 46);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(65, 18);
            this.label30.TabIndex = 34;
            this.label30.Text = "Volume";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBarVolume1
            // 
            this.trackBarVolume1.Location = new System.Drawing.Point(27, 69);
            this.trackBarVolume1.Maximum = 100;
            this.trackBarVolume1.Name = "trackBarVolume1";
            this.trackBarVolume1.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarVolume1.Size = new System.Drawing.Size(45, 112);
            this.trackBarVolume1.TabIndex = 33;
            this.trackBarVolume1.TickFrequency = 10;
            this.trackBarVolume1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarVolume1.Scroll += new System.EventHandler(this.trackBarVolume1_Scroll);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Arial", 12F);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(185, 49);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(154, 21);
            this.label1.TabIndex = 26;
            // 
            // Label2
            // 
            this.Label2.BackColor = System.Drawing.SystemColors.Control;
            this.Label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label2.Font = new System.Drawing.Font("Arial", 12F);
            this.Label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label2.Location = new System.Drawing.Point(482, 117);
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
            this.Label3.Font = new System.Drawing.Font("Arial", 12F);
            this.Label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label3.Location = new System.Drawing.Point(482, 160);
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
            this.Label4.Font = new System.Drawing.Font("Arial", 12F);
            this.Label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label4.Location = new System.Drawing.Point(407, 96);
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
            this.Label5.Font = new System.Drawing.Font("Arial", 12F);
            this.Label5.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label5.Location = new System.Drawing.Point(311, 96);
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
            this.Label6.Font = new System.Drawing.Font("Arial", 12F);
            this.Label6.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label6.Location = new System.Drawing.Point(185, 96);
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
            this.LabelSelectionBegin.Font = new System.Drawing.Font("Arial", 12F);
            this.LabelSelectionBegin.ForeColor = System.Drawing.Color.Black;
            this.LabelSelectionBegin.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelSelectionBegin.Location = new System.Drawing.Point(358, 116);
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
            this.LabelSelectionEnd.Font = new System.Drawing.Font("Arial", 12F);
            this.LabelSelectionEnd.ForeColor = System.Drawing.Color.Black;
            this.LabelSelectionEnd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelSelectionEnd.Location = new System.Drawing.Point(244, 116);
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
            this.LabelSelectionDuration.Font = new System.Drawing.Font("Arial", 12F);
            this.LabelSelectionDuration.ForeColor = System.Drawing.Color.Black;
            this.LabelSelectionDuration.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelSelectionDuration.Location = new System.Drawing.Point(130, 116);
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
            this.LabelRangeBegin.Font = new System.Drawing.Font("Arial", 12F);
            this.LabelRangeBegin.ForeColor = System.Drawing.Color.Black;
            this.LabelRangeBegin.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelRangeBegin.Location = new System.Drawing.Point(358, 159);
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
            this.LabelRangeEnd.Font = new System.Drawing.Font("Arial", 12F);
            this.LabelRangeEnd.ForeColor = System.Drawing.Color.Black;
            this.LabelRangeEnd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelRangeEnd.Location = new System.Drawing.Point(244, 159);
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
            this.LabelRangeDuration.Font = new System.Drawing.Font("Arial", 12F);
            this.LabelRangeDuration.ForeColor = System.Drawing.Color.Black;
            this.LabelRangeDuration.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelRangeDuration.Location = new System.Drawing.Point(130, 159);
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
            this.LabelTotalDuration.Font = new System.Drawing.Font("Arial", 12F);
            this.LabelTotalDuration.ForeColor = System.Drawing.Color.Black;
            this.LabelTotalDuration.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelTotalDuration.Location = new System.Drawing.Point(361, 49);
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
            this.Label8.Font = new System.Drawing.Font("Arial", 12F);
            this.Label8.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Label8.Location = new System.Drawing.Point(482, 49);
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
            this.framePlayback.Font = new System.Drawing.Font("Arial", 12F);
            this.framePlayback.ForeColor = System.Drawing.SystemColors.ControlText;
            this.framePlayback.Location = new System.Drawing.Point(3, 250);
            this.framePlayback.Name = "framePlayback";
            this.framePlayback.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.framePlayback.Size = new System.Drawing.Size(551, 95);
            this.framePlayback.TabIndex = 15;
            this.framePlayback.TabStop = false;
            this.framePlayback.Text = "ניגון";
            // 
            // buttonPause
            // 
            this.buttonPause.Enabled = false;
            this.buttonPause.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonPause.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonPause.Location = new System.Drawing.Point(349, 25);
            this.buttonPause.Name = "buttonPause";
            this.buttonPause.Size = new System.Drawing.Size(85, 43);
            this.buttonPause.TabIndex = 10;
            this.buttonPause.Text = "השהה";
            this.buttonPause.Click += new System.EventHandler(this.buttonPause_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.BackColor = System.Drawing.SystemColors.Control;
            this.buttonPlay.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonPlay.Enabled = false;
            this.buttonPlay.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonPlay.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonPlay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonPlay.Location = new System.Drawing.Point(440, 25);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonPlay.Size = new System.Drawing.Size(90, 43);
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
            this.buttonStop.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonStop.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStop.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonStop.Location = new System.Drawing.Point(137, 25);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStop.Size = new System.Drawing.Size(101, 43);
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
            this.buttonPlaySelection.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonPlaySelection.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonPlaySelection.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonPlaySelection.Location = new System.Drawing.Point(244, 25);
            this.buttonPlaySelection.Name = "buttonPlaySelection";
            this.buttonPlaySelection.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonPlaySelection.Size = new System.Drawing.Size(99, 43);
            this.buttonPlaySelection.TabIndex = 7;
            this.buttonPlaySelection.Text = "נגן בחירה";
            this.buttonPlaySelection.UseVisualStyleBackColor = false;
            this.buttonPlaySelection.Click += new System.EventHandler(this.buttonPlaySelection_Click);
            // 
            // LabelStatus
            // 
            this.LabelStatus.BackColor = System.Drawing.SystemColors.Control;
            this.LabelStatus.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelStatus.Font = new System.Drawing.Font("Arial", 8F);
            this.LabelStatus.ForeColor = System.Drawing.SystemColors.ControlText;
            this.LabelStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelStatus.Location = new System.Drawing.Point(156, 2);
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
            this.progressBar1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.progressBar1.Location = new System.Drawing.Point(20, 22);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(457, 13);
            this.progressBar1.TabIndex = 19;
            this.progressBar1.Visible = false;
            // 
            // timerDisplayWaveform
            // 
            this.timerDisplayWaveform.Tick += new System.EventHandler(this.timerDisplayWaveform_Tick);
            // 
            // audioSoundEditor1
            // 
            this.audioSoundEditor1.Location = new System.Drawing.Point(519, 0);
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
            this.audioSoundEditor1.WaveAnalyzerLineMoving += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerLineMovingEventHandler(this.audioSoundEditor1_WaveAnalyzerLineMoving);
            this.audioSoundEditor1.WaveAnalyzerGraphicItemClick += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerGraphicItemClickEventHandler(this.audioSoundEditor1_WaveAnalyzerGraphicItemClick);
            // 
            // FrameRecording
            // 
            this.FrameRecording.BackColor = System.Drawing.SystemColors.Control;
            this.FrameRecording.Controls.Add(this.pictureBox1);
            this.FrameRecording.Controls.Add(this.groupBox9);
            this.FrameRecording.Controls.Add(this.buttonStartRecAppend);
            this.FrameRecording.Controls.Add(this.buttonStopRecording);
            this.FrameRecording.Controls.Add(this.buttonStartRecNew);
            this.FrameRecording.Font = new System.Drawing.Font("Arial", 12F);
            this.FrameRecording.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FrameRecording.Location = new System.Drawing.Point(608, 25);
            this.FrameRecording.Name = "FrameRecording";
            this.tableLayoutPanel1.SetRowSpan(this.FrameRecording, 2);
            this.FrameRecording.Size = new System.Drawing.Size(551, 315);
            this.FrameRecording.TabIndex = 24;
            this.FrameRecording.TabStop = false;
            this.FrameRecording.Text = "הקלטה";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox1.Image = global::MyMentor.Properties.Resources._1386908112_Record_Button1;
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(26, 35);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(36, 36);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 33;
            this.pictureBox1.TabStop = false;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.sevenSegment1);
            this.groupBox9.Controls.Add(this.label26);
            this.groupBox9.Controls.Add(this.label27);
            this.groupBox9.Controls.Add(this.numericUpDownBufferRecord);
            this.groupBox9.Controls.Add(this.buttonRecOverwritePlayback);
            this.groupBox9.Location = new System.Drawing.Point(247, 94);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(298, 123);
            this.groupBox9.TabIndex = 32;
            this.groupBox9.TabStop = false;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label26.Location = new System.Drawing.Point(89, 89);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(28, 18);
            this.label26.TabIndex = 36;
            this.label26.Text = "שנ\'";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label27.Location = new System.Drawing.Point(168, 89);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(124, 18);
            this.label27.TabIndex = 35;
            this.label27.Text = "היסט עליון נגינה של";
            // 
            // numericUpDownBufferRecord
            // 
            this.numericUpDownBufferRecord.Location = new System.Drawing.Point(123, 87);
            this.numericUpDownBufferRecord.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.numericUpDownBufferRecord.Name = "numericUpDownBufferRecord";
            this.numericUpDownBufferRecord.Size = new System.Drawing.Size(33, 26);
            this.numericUpDownBufferRecord.TabIndex = 34;
            this.numericUpDownBufferRecord.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownBufferRecord.ValueChanged += new System.EventHandler(this.numericUpDownBufferRecord_ValueChanged);
            // 
            // buttonRecOverwritePlayback
            // 
            this.buttonRecOverwritePlayback.BackColor = System.Drawing.SystemColors.Control;
            this.buttonRecOverwritePlayback.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonRecOverwritePlayback.Enabled = false;
            this.buttonRecOverwritePlayback.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonRecOverwritePlayback.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonRecOverwritePlayback.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonRecOverwritePlayback.Location = new System.Drawing.Point(141, 25);
            this.buttonRecOverwritePlayback.Name = "buttonRecOverwritePlayback";
            this.buttonRecOverwritePlayback.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonRecOverwritePlayback.Size = new System.Drawing.Size(138, 43);
            this.buttonRecOverwritePlayback.TabIndex = 33;
            this.buttonRecOverwritePlayback.Text = "תקן במקום הנבחר";
            this.buttonRecOverwritePlayback.UseVisualStyleBackColor = false;
            this.buttonRecOverwritePlayback.Click += new System.EventHandler(this.buttonRecOverwritePlayback_Click);
            // 
            // buttonStartRecAppend
            // 
            this.buttonStartRecAppend.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStartRecAppend.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStartRecAppend.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonStartRecAppend.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStartRecAppend.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonStartRecAppend.Location = new System.Drawing.Point(109, 31);
            this.buttonStartRecAppend.Name = "buttonStartRecAppend";
            this.buttonStartRecAppend.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStartRecAppend.Size = new System.Drawing.Size(184, 43);
            this.buttonStartRecAppend.TabIndex = 6;
            this.buttonStartRecAppend.Text = "המשך הקלטה מסוף הקטע";
            this.buttonStartRecAppend.UseVisualStyleBackColor = false;
            this.buttonStartRecAppend.Click += new System.EventHandler(this.buttonStartRecAppend_Click);
            // 
            // buttonStopRecording
            // 
            this.buttonStopRecording.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStopRecording.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStopRecording.Enabled = false;
            this.buttonStopRecording.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonStopRecording.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStopRecording.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonStopRecording.Location = new System.Drawing.Point(299, 31);
            this.buttonStopRecording.Name = "buttonStopRecording";
            this.buttonStopRecording.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStopRecording.Size = new System.Drawing.Size(127, 43);
            this.buttonStopRecording.TabIndex = 5;
            this.buttonStopRecording.Text = "עצור הקלטה";
            this.buttonStopRecording.UseVisualStyleBackColor = false;
            this.buttonStopRecording.Click += new System.EventHandler(this.buttonStopRecording_Click);
            // 
            // buttonStartRecNew
            // 
            this.buttonStartRecNew.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStartRecNew.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonStartRecNew.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStartRecNew.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonStartRecNew.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStartRecNew.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonStartRecNew.Location = new System.Drawing.Point(437, 31);
            this.buttonStartRecNew.Name = "buttonStartRecNew";
            this.buttonStartRecNew.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.buttonStartRecNew.Size = new System.Drawing.Size(111, 42);
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
            this.audioSoundRecorder1.Location = new System.Drawing.Point(448, 0);
            this.audioSoundRecorder1.Name = "audioSoundRecorder1";
            this.audioSoundRecorder1.SilenceThreshold = ((short)(0));
            this.audioSoundRecorder1.Size = new System.Drawing.Size(48, 48);
            this.audioSoundRecorder1.TabIndex = 25;
            this.audioSoundRecorder1.RecordingStarted += new AudioSoundRecorder.AudioSoundRecorder.EventHandler(this.audioSoundRecorder1_RecordingStarted);
            this.audioSoundRecorder1.RecordingPaused += new AudioSoundRecorder.AudioSoundRecorder.EventHandler(this.audioSoundRecorder1_RecordingPaused);
            this.audioSoundRecorder1.RecordingResumed += new AudioSoundRecorder.AudioSoundRecorder.EventHandler(this.audioSoundRecorder1_RecordingResumed);
            this.audioSoundRecorder1.RecordingStopped += new AudioSoundRecorder.AudioSoundRecorder.RecordingStoppedEventHandler(this.audioSoundRecorder1_RecordingStopped);
            this.audioSoundRecorder1.RecordingDuration += new AudioSoundRecorder.AudioSoundRecorder.RecordingDurationEventHandler(this.audioSoundRecorder1_RecordingDuration);
            this.audioSoundRecorder1.VUMeterValueChange += new AudioSoundRecorder.AudioSoundRecorder.VUMeterValueChangeEventHandler(this.audioSoundRecorder1_VUMeterValueChange);
            // 
            // timerRecordingDone
            // 
            this.timerRecordingDone.Tick += new System.EventHandler(this.timerRecordingDone_Tick);
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.Black;
            this.label18.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label18.Location = new System.Drawing.Point(3, 5);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(15, 194);
            this.label18.TabIndex = 60;
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.Black;
            this.label17.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label17.Location = new System.Drawing.Point(20, 5);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(15, 194);
            this.label17.TabIndex = 59;
            // 
            // tabControl1
            // 
            this.tableLayoutPanel4.SetColumnSpan(this.tabControl1, 2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Arial Narrow", 24F);
            this.tabControl1.ImageList = this.imageList1;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.RightToLeftLayout = true;
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1176, 454);
            this.tabControl1.TabIndex = 61;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            this.tabControl1.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabControl1_Selecting);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.tableLayoutPanel2);
            this.tabPage1.Location = new System.Drawing.Point(4, 46);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1168, 404);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "1 - טקסט";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85.12881F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.87119F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 129F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel2.Controls.Add(this.ToolStrip1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.richTextBox1, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.panel7, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel8, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1162, 398);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // ToolStrip1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.ToolStrip1, 4);
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
            this.tbrParse});
            this.ToolStrip1.Location = new System.Drawing.Point(0, 80);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.Size = new System.Drawing.Size(1162, 27);
            this.ToolStrip1.TabIndex = 32;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // tbrNew
            // 
            this.tbrNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrNew.Image = ((System.Drawing.Image)(resources.GetObject("tbrNew.Image")));
            this.tbrNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrNew.Name = "tbrNew";
            this.tbrNew.Size = new System.Drawing.Size(23, 24);
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
            this.tbrOpen.Size = new System.Drawing.Size(23, 24);
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
            this.tbrSave.Size = new System.Drawing.Size(23, 24);
            this.tbrSave.Text = "Save";
            this.tbrSave.ToolTipText = "שמור";
            this.tbrSave.Click += new System.EventHandler(this.tbrSave_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // tbrSmallerFont
            // 
            this.tbrSmallerFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrSmallerFont.Image = ((System.Drawing.Image)(resources.GetObject("tbrSmallerFont.Image")));
            this.tbrSmallerFont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrSmallerFont.Name = "tbrSmallerFont";
            this.tbrSmallerFont.Size = new System.Drawing.Size(24, 24);
            this.tbrSmallerFont.Text = "-A";
            this.tbrSmallerFont.Click += new System.EventHandler(this.tbrSmallerFont_Click);
            // 
            // tbrBiggerFont
            // 
            this.tbrBiggerFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrBiggerFont.Image = ((System.Drawing.Image)(resources.GetObject("tbrBiggerFont.Image")));
            this.tbrBiggerFont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrBiggerFont.Name = "tbrBiggerFont";
            this.tbrBiggerFont.Size = new System.Drawing.Size(27, 24);
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
            this.tbrFont.Size = new System.Drawing.Size(23, 24);
            this.tbrFont.Text = "Font";
            this.tbrFont.ToolTipText = "גופן";
            this.tbrFont.Click += new System.EventHandler(this.tbrFont_Click);
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // tbrRight
            // 
            this.tbrRight.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrRight.Image = ((System.Drawing.Image)(resources.GetObject("tbrRight.Image")));
            this.tbrRight.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrRight.Name = "tbrRight";
            this.tbrRight.Size = new System.Drawing.Size(23, 24);
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
            this.tbrLeft.Size = new System.Drawing.Size(23, 24);
            this.tbrLeft.Text = "Left";
            this.tbrLeft.ToolTipText = "יישר לשמאל";
            this.tbrLeft.Click += new System.EventHandler(this.tbrLeft_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // tbrBold
            // 
            this.tbrBold.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrBold.Image = ((System.Drawing.Image)(resources.GetObject("tbrBold.Image")));
            this.tbrBold.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrBold.Name = "tbrBold";
            this.tbrBold.Size = new System.Drawing.Size(23, 24);
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
            this.tbrItalic.Size = new System.Drawing.Size(23, 24);
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
            this.tbrUnderline.Size = new System.Drawing.Size(23, 24);
            this.tbrUnderline.Text = "Underline";
            this.tbrUnderline.ToolTipText = "קו תחתי";
            this.tbrUnderline.Click += new System.EventHandler(this.tbrUnderline_Click);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // tbrParagraph
            // 
            this.tbrParagraph.BackColor = System.Drawing.Color.Red;
            this.tbrParagraph.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrParagraph.ForeColor = System.Drawing.Color.White;
            this.tbrParagraph.Image = ((System.Drawing.Image)(resources.GetObject("tbrParagraph.Image")));
            this.tbrParagraph.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrParagraph.Name = "tbrParagraph";
            this.tbrParagraph.Size = new System.Drawing.Size(25, 24);
            this.tbrParagraph.Text = "[3]";
            this.tbrParagraph.ToolTipText = "חלק פסקה";
            this.tbrParagraph.Click += new System.EventHandler(this.tbrParagraph_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // tbrSentense
            // 
            this.tbrSentense.BackColor = System.Drawing.Color.Violet;
            this.tbrSentense.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrSentense.ForeColor = System.Drawing.Color.White;
            this.tbrSentense.Image = ((System.Drawing.Image)(resources.GetObject("tbrSentense.Image")));
            this.tbrSentense.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrSentense.Name = "tbrSentense";
            this.tbrSentense.Size = new System.Drawing.Size(25, 24);
            this.tbrSentense.Text = "[2]";
            this.tbrSentense.ToolTipText = "חלק משפט";
            this.tbrSentense.Click += new System.EventHandler(this.tbrSentense_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 27);
            // 
            // tbrSection
            // 
            this.tbrSection.BackColor = System.Drawing.Color.LimeGreen;
            this.tbrSection.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrSection.ForeColor = System.Drawing.Color.White;
            this.tbrSection.Image = ((System.Drawing.Image)(resources.GetObject("tbrSection.Image")));
            this.tbrSection.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrSection.Name = "tbrSection";
            this.tbrSection.Size = new System.Drawing.Size(25, 24);
            this.tbrSection.Text = "[1]";
            this.tbrSection.ToolTipText = "חלק קטע";
            this.tbrSection.Click += new System.EventHandler(this.tbrSection_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 27);
            // 
            // tbrWord
            // 
            this.tbrWord.BackColor = System.Drawing.Color.Yellow;
            this.tbrWord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbrWord.Image = ((System.Drawing.Image)(resources.GetObject("tbrWord.Image")));
            this.tbrWord.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrWord.Name = "tbrWord";
            this.tbrWord.Size = new System.Drawing.Size(25, 24);
            this.tbrWord.Text = "[0]";
            this.tbrWord.ToolTipText = "התחל מילה";
            this.tbrWord.Click += new System.EventHandler(this.tbrWord_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(6, 27);
            // 
            // tbrProperties
            // 
            this.tbrProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrProperties.Image = ((System.Drawing.Image)(resources.GetObject("tbrProperties.Image")));
            this.tbrProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrProperties.Name = "tbrProperties";
            this.tbrProperties.Size = new System.Drawing.Size(23, 24);
            this.tbrProperties.Text = "toolStripButton11";
            this.tbrProperties.ToolTipText = "מאפייני שיעור";
            // 
            // tbrParse
            // 
            this.tbrParse.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbrParse.Image = ((System.Drawing.Image)(resources.GetObject("tbrParse.Image")));
            this.tbrParse.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbrParse.Name = "tbrParse";
            this.tbrParse.Size = new System.Drawing.Size(23, 24);
            this.tbrParse.Text = "toolStripButton10";
            this.tbrParse.ToolTipText = "בדוק תקינות";
            this.tbrParse.Click += new System.EventHandler(this.tbrParse_Click);
            // 
            // richTextBox1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.richTextBox1, 5);
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Font = new System.Drawing.Font("Arial", 15.75F);
            this.richTextBox1.Location = new System.Drawing.Point(4, 111);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox1.Name = "richTextBox1";
            this.tableLayoutPanel2.SetRowSpan(this.richTextBox1, 2);
            this.richTextBox1.Size = new System.Drawing.Size(1154, 283);
            this.richTextBox1.TabIndex = 31;
            this.richTextBox1.Text = "";
            this.richTextBox1.SelectionChanged += new System.EventHandler(this.richTextBox1_SelectionChanged);
            this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.groupBox8);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(456, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(703, 74);
            this.panel7.TabIndex = 34;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.comboBoxAutoDevidePar);
            this.groupBox8.Controls.Add(this.buttonAutoDevide);
            this.groupBox8.Controls.Add(this.richTextBox2);
            this.groupBox8.Controls.Add(this.richTextBox4);
            this.groupBox8.Controls.Add(this.label24);
            this.groupBox8.Controls.Add(this.comboBoxAutoDevideSen);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Font = new System.Drawing.Font("Arial", 12F);
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox8.Size = new System.Drawing.Size(711, 68);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "חלוקה אוטומטית";
            // 
            // buttonAutoDevide
            // 
            this.buttonAutoDevide.BackColor = System.Drawing.SystemColors.Control;
            this.buttonAutoDevide.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonAutoDevide.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonAutoDevide.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonAutoDevide.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonAutoDevide.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonAutoDevide.Location = new System.Drawing.Point(20, 19);
            this.buttonAutoDevide.Name = "buttonAutoDevide";
            this.buttonAutoDevide.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.buttonAutoDevide.Size = new System.Drawing.Size(85, 42);
            this.buttonAutoDevide.TabIndex = 18;
            this.buttonAutoDevide.Text = "קבע עוגנים";
            this.buttonAutoDevide.UseVisualStyleBackColor = false;
            this.buttonAutoDevide.Click += new System.EventHandler(this.buttonAutoDevide_Click);
            // 
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(134, 13);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(26, 23);
            this.richTextBox2.TabIndex = 33;
            this.richTextBox2.Text = "";
            this.richTextBox2.Visible = false;
            // 
            // richTextBox4
            // 
            this.richTextBox4.Location = new System.Drawing.Point(166, 13);
            this.richTextBox4.Name = "richTextBox4";
            this.richTextBox4.Size = new System.Drawing.Size(28, 23);
            this.richTextBox4.TabIndex = 35;
            this.richTextBox4.Text = "";
            this.richTextBox4.Visible = false;
            // 
            // label24
            // 
            this.label24.BackColor = System.Drawing.SystemColors.Control;
            this.label24.Cursor = System.Windows.Forms.Cursors.Default;
            this.label24.Font = new System.Drawing.Font("Arial", 12F);
            this.label24.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label24.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label24.Location = new System.Drawing.Point(317, 34);
            this.label24.Name = "label24";
            this.label24.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label24.Size = new System.Drawing.Size(75, 21);
            this.label24.TabIndex = 17;
            this.label24.Text = "סוף משפט";
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.SystemColors.Control;
            this.label14.Cursor = System.Windows.Forms.Cursors.Default;
            this.label14.Font = new System.Drawing.Font("Arial", 12F);
            this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(625, 31);
            this.label14.Name = "label14";
            this.label14.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label14.Size = new System.Drawing.Size(75, 21);
            this.label14.TabIndex = 14;
            this.label14.Text = "סוף פסקה";
            // 
            // panel8
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.panel8, 3);
            this.panel8.Controls.Add(this.groupBox10);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(3, 3);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(447, 74);
            this.panel8.TabIndex = 35;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.linkLabel1);
            this.groupBox10.Controls.Add(this.label28);
            this.groupBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox10.Font = new System.Drawing.Font("Arial", 12F);
            this.groupBox10.Location = new System.Drawing.Point(0, 0);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(447, 74);
            this.groupBox10.TabIndex = 0;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "טקסטים מוכנים";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(51, 34);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(55, 18);
            this.linkLabel1.TabIndex = 1;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "לחץ כאן";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(101, 34);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(311, 18);
            this.label28.TabIndex = 0;
            this.label28.Text = "באפשרותך להתחיל ע\"י הבאת טקסט מוכן, לחיפוש ";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel1);
            this.tabPage2.Location = new System.Drawing.Point(4, 46);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1168, 404);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "2 - הקלטה";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.Frame4, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.FrameRecording, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.framePlayback, 2, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 63.92046F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 29.82955F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1162, 398);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.label18);
            this.panel2.Controls.Add(this.label17);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(560, 25);
            this.panel2.Name = "panel2";
            this.tableLayoutPanel1.SetRowSpan(this.panel2, 2);
            this.panel2.Size = new System.Drawing.Size(42, 208);
            this.panel2.TabIndex = 18;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.tableLayoutPanel3);
            this.tabPage3.Location = new System.Drawing.Point(4, 46);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(1168, 404);
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
            this.tableLayoutPanel3.Controls.Add(this.groupBox2, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.panel6, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1162, 398);
            this.tableLayoutPanel3.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.LabelCurrentWordDuration);
            this.groupBox1.Controls.Add(this.label29);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.timePickerCurrentWord);
            this.groupBox1.Controls.Add(this.LabelCurrentSchedulingTimer);
            this.groupBox1.Controls.Add(this.buttonRestartScheduling);
            this.groupBox1.Controls.Add(this.buttonHammer);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.buttonStartSchedulingPlayback);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 12F);
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(605, 256);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(554, 139);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "תזמון קטעים";
            // 
            // LabelCurrentWordDuration
            // 
            this.LabelCurrentWordDuration.BackColor = System.Drawing.Color.White;
            this.LabelCurrentWordDuration.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelCurrentWordDuration.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelCurrentWordDuration.Font = new System.Drawing.Font("Arial", 12F);
            this.LabelCurrentWordDuration.ForeColor = System.Drawing.Color.Black;
            this.LabelCurrentWordDuration.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelCurrentWordDuration.Location = new System.Drawing.Point(71, 108);
            this.LabelCurrentWordDuration.Name = "LabelCurrentWordDuration";
            this.LabelCurrentWordDuration.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelCurrentWordDuration.Size = new System.Drawing.Size(108, 22);
            this.LabelCurrentWordDuration.TabIndex = 29;
            this.LabelCurrentWordDuration.Text = "00:00:00.000";
            this.LabelCurrentWordDuration.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label29.Location = new System.Drawing.Point(198, 109);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(103, 18);
            this.label29.TabIndex = 28;
            this.label29.Text = "אורך קטע נבחר";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(195, 80);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(122, 18);
            this.label10.TabIndex = 26;
            this.label10.Text = "התחלה קטע נבחר";
            // 
            // LabelCurrentSchedulingTimer
            // 
            this.LabelCurrentSchedulingTimer.BackColor = System.Drawing.Color.White;
            this.LabelCurrentSchedulingTimer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelCurrentSchedulingTimer.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelCurrentSchedulingTimer.Font = new System.Drawing.Font("Arial", 12F);
            this.LabelCurrentSchedulingTimer.ForeColor = System.Drawing.Color.Black;
            this.LabelCurrentSchedulingTimer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelCurrentSchedulingTimer.Location = new System.Drawing.Point(342, 104);
            this.LabelCurrentSchedulingTimer.Name = "LabelCurrentSchedulingTimer";
            this.LabelCurrentSchedulingTimer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelCurrentSchedulingTimer.Size = new System.Drawing.Size(108, 22);
            this.LabelCurrentSchedulingTimer.TabIndex = 27;
            this.LabelCurrentSchedulingTimer.Text = "00:00:00.000";
            this.LabelCurrentSchedulingTimer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // buttonRestartScheduling
            // 
            this.buttonRestartScheduling.BackColor = System.Drawing.SystemColors.Control;
            this.buttonRestartScheduling.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonRestartScheduling.Enabled = false;
            this.buttonRestartScheduling.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonRestartScheduling.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonRestartScheduling.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonRestartScheduling.Location = new System.Drawing.Point(311, 25);
            this.buttonRestartScheduling.Name = "buttonRestartScheduling";
            this.buttonRestartScheduling.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonRestartScheduling.Size = new System.Drawing.Size(131, 41);
            this.buttonRestartScheduling.TabIndex = 12;
            this.buttonRestartScheduling.Text = "חזור להתחלה";
            this.buttonRestartScheduling.UseVisualStyleBackColor = false;
            this.buttonRestartScheduling.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonHammer
            // 
            this.buttonHammer.BackColor = System.Drawing.SystemColors.Control;
            this.buttonHammer.BackgroundImage = global::MyMentor.Properties.Resources._1386909293_auction_hammer_gavel;
            this.buttonHammer.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonHammer.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonHammer.Enabled = false;
            this.buttonHammer.Font = new System.Drawing.Font("Arial", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))));
            this.buttonHammer.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonHammer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonHammer.Location = new System.Drawing.Point(261, 25);
            this.buttonHammer.Name = "buttonHammer";
            this.buttonHammer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.buttonHammer.Size = new System.Drawing.Size(44, 41);
            this.buttonHammer.TabIndex = 11;
            this.buttonHammer.UseVisualStyleBackColor = false;
            this.buttonHammer.Click += new System.EventHandler(this.buttonHammer_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(469, 105);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 18);
            this.label9.TabIndex = 24;
            this.label9.Text = "מיקום נוכחי";
            // 
            // buttonStartSchedulingPlayback
            // 
            this.buttonStartSchedulingPlayback.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStartSchedulingPlayback.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStartSchedulingPlayback.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonStartSchedulingPlayback.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStartSchedulingPlayback.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonStartSchedulingPlayback.Location = new System.Drawing.Point(448, 25);
            this.buttonStartSchedulingPlayback.Name = "buttonStartSchedulingPlayback";
            this.buttonStartSchedulingPlayback.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStartSchedulingPlayback.Size = new System.Drawing.Size(94, 41);
            this.buttonStartSchedulingPlayback.TabIndex = 9;
            this.buttonStartSchedulingPlayback.Text = "התחל";
            this.buttonStartSchedulingPlayback.UseVisualStyleBackColor = false;
            this.buttonStartSchedulingPlayback.Click += new System.EventHandler(this.buttonStartSchedulingPlayback_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label25);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numericUpDownInterval);
            this.groupBox2.Controls.Add(this.buttonScheduleAnchor);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 12F);
            this.groupBox2.Location = new System.Drawing.Point(225, 256);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(353, 139);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "בדיקת מעברים";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Arial", 9F);
            this.label25.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label25.Location = new System.Drawing.Point(104, 108);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(236, 15);
            this.label25.TabIndex = 29;
            this.label25.Text = "* יש להקליק על העוגנים בטקסט לאפשור הבדיקה";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(19, 49);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(28, 18);
            this.label11.TabIndex = 28;
            this.label11.Text = "שנ\'";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(108, 48);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 18);
            this.label7.TabIndex = 27;
            this.label7.Text = "היסט עליון/תחתון";
            // 
            // numericUpDownInterval
            // 
            this.numericUpDownInterval.Location = new System.Drawing.Point(53, 44);
            this.numericUpDownInterval.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownInterval.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDownInterval.Name = "numericUpDownInterval";
            this.numericUpDownInterval.Size = new System.Drawing.Size(49, 26);
            this.numericUpDownInterval.TabIndex = 11;
            this.numericUpDownInterval.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // buttonScheduleAnchor
            // 
            this.buttonScheduleAnchor.BackColor = System.Drawing.SystemColors.Control;
            this.buttonScheduleAnchor.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonScheduleAnchor.Enabled = false;
            this.buttonScheduleAnchor.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonScheduleAnchor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonScheduleAnchor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonScheduleAnchor.Location = new System.Drawing.Point(229, 38);
            this.buttonScheduleAnchor.Name = "buttonScheduleAnchor";
            this.buttonScheduleAnchor.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonScheduleAnchor.Size = new System.Drawing.Size(94, 38);
            this.buttonScheduleAnchor.TabIndex = 10;
            this.buttonScheduleAnchor.Text = "התחל";
            this.buttonScheduleAnchor.UseVisualStyleBackColor = false;
            this.buttonScheduleAnchor.Click += new System.EventHandler(this.buttonScheduleAnchor_Click);
            // 
            // panel6
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.panel6, 2);
            this.panel6.Controls.Add(this.richTextBox3);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel6.Location = new System.Drawing.Point(3, 3);
            this.panel6.Name = "panel6";
            this.tableLayoutPanel3.SetRowSpan(this.panel6, 3);
            this.panel6.Size = new System.Drawing.Size(1156, 247);
            this.panel6.TabIndex = 36;
            // 
            // richTextBox3
            // 
            this.richTextBox3.Cursor = System.Windows.Forms.Cursors.Default;
            this.richTextBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox3.Font = new System.Drawing.Font("Arial", 15.75F);
            this.richTextBox3.HideSelection = false;
            this.richTextBox3.Location = new System.Drawing.Point(0, 0);
            this.richTextBox3.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox3.Name = "richTextBox3";
            this.richTextBox3.ReadOnly = true;
            this.richTextBox3.Size = new System.Drawing.Size(1156, 247);
            this.richTextBox3.TabIndex = 33;
            this.richTextBox3.Text = "";
            this.richTextBox3.SelectionChanged += new System.EventHandler(this.richTextBox3_SelectionChanged);
            this.richTextBox3.MouseMove += new System.Windows.Forms.MouseEventHandler(this.richTextBox3_MouseMove);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tableLayoutPanel5);
            this.tabPage4.Location = new System.Drawing.Point(4, 46);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1168, 404);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "4 - פרסום";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel5.ColumnCount = 2;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85.78767F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.21233F));
            this.tableLayoutPanel5.Controls.Add(this.groupBox3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.panel4, 1, 0);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 2;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 97.77228F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 2.227723F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(1168, 404);
            this.tableLayoutPanel5.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox3.Controls.Add(this.lblClipType);
            this.groupBox3.Controls.Add(this.comboClipType);
            this.groupBox3.Controls.Add(this.dtpReadingDate);
            this.groupBox3.Controls.Add(this.btnRemoveDate);
            this.groupBox3.Controls.Add(this.btnAddDate);
            this.groupBox3.Controls.Add(this.listBoxDates);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.comboCategory4);
            this.groupBox3.Controls.Add(this.lblCategory4);
            this.groupBox3.Controls.Add(this.lblCategory2);
            this.groupBox3.Controls.Add(this.comboCategory2);
            this.groupBox3.Controls.Add(this.comboCategory1);
            this.groupBox3.Controls.Add(this.lblCategory1);
            this.groupBox3.Controls.Add(this.LabelTotalDuration2);
            this.groupBox3.Controls.Add(this.comboStatus);
            this.groupBox3.Controls.Add(this.comboCategory3);
            this.groupBox3.Controls.Add(this.textBox7);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.Controls.Add(this.groupBox7);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.tbClipDescription);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.tbKeywords);
            this.groupBox3.Controls.Add(this.lblCategory3);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.tbClipName);
            this.groupBox3.Controls.Add(this.maskedTextBox1);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 12F);
            this.groupBox3.Location = new System.Drawing.Point(171, 4);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(993, 387);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            // 
            // dtpReadingDate
            // 
            this.dtpReadingDate.CustomFormat = "";
            this.dtpReadingDate.DropDownAlign = System.Windows.Forms.LeftRightAlignment.Right;
            this.dtpReadingDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpReadingDate.Location = new System.Drawing.Point(399, 162);
            this.dtpReadingDate.Name = "dtpReadingDate";
            this.dtpReadingDate.Size = new System.Drawing.Size(118, 26);
            this.dtpReadingDate.TabIndex = 72;
            // 
            // btnRemoveDate
            // 
            this.btnRemoveDate.Location = new System.Drawing.Point(325, 194);
            this.btnRemoveDate.Name = "btnRemoveDate";
            this.btnRemoveDate.Size = new System.Drawing.Size(31, 31);
            this.btnRemoveDate.TabIndex = 71;
            this.btnRemoveDate.Text = "-";
            this.btnRemoveDate.UseVisualStyleBackColor = true;
            this.btnRemoveDate.Click += new System.EventHandler(this.btnRemoveDate_Click);
            // 
            // btnAddDate
            // 
            this.btnAddDate.Location = new System.Drawing.Point(362, 162);
            this.btnAddDate.Name = "btnAddDate";
            this.btnAddDate.Size = new System.Drawing.Size(31, 26);
            this.btnAddDate.TabIndex = 70;
            this.btnAddDate.Text = "+";
            this.btnAddDate.UseVisualStyleBackColor = true;
            this.btnAddDate.Click += new System.EventHandler(this.btnAddDate_Click);
            // 
            // listBoxDates
            // 
            this.listBoxDates.FormattingEnabled = true;
            this.listBoxDates.ItemHeight = 18;
            this.listBoxDates.Location = new System.Drawing.Point(362, 194);
            this.listBoxDates.Name = "listBoxDates";
            this.listBoxDates.Size = new System.Drawing.Size(155, 94);
            this.listBoxDates.TabIndex = 69;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Arial", 12F);
            this.label20.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label20.Location = new System.Drawing.Point(524, 194);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(91, 18);
            this.label20.TabIndex = 68;
            this.label20.Text = "תאריכי קריאה";
            // 
            // comboCategory4
            // 
            this.comboCategory4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategory4.FormattingEnabled = true;
            this.comboCategory4.Location = new System.Drawing.Point(646, 254);
            this.comboCategory4.Name = "comboCategory4";
            this.comboCategory4.Size = new System.Drawing.Size(220, 26);
            this.comboCategory4.TabIndex = 67;
            this.comboCategory4.SelectionChangeCommitted += new System.EventHandler(this.comboCategory4_SelectionChangeCommitted);
            // 
            // lblCategory4
            // 
            this.lblCategory4.AutoSize = true;
            this.lblCategory4.Font = new System.Drawing.Font("Arial", 12F);
            this.lblCategory4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCategory4.Location = new System.Drawing.Point(886, 257);
            this.lblCategory4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCategory4.Name = "lblCategory4";
            this.lblCategory4.Size = new System.Drawing.Size(43, 18);
            this.lblCategory4.TabIndex = 66;
            this.lblCategory4.Text = "טוען...";
            // 
            // lblCategory2
            // 
            this.lblCategory2.AutoSize = true;
            this.lblCategory2.Font = new System.Drawing.Font("Arial", 12F);
            this.lblCategory2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCategory2.Location = new System.Drawing.Point(886, 189);
            this.lblCategory2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCategory2.Name = "lblCategory2";
            this.lblCategory2.Size = new System.Drawing.Size(43, 18);
            this.lblCategory2.TabIndex = 65;
            this.lblCategory2.Text = "טוען...";
            // 
            // comboCategory2
            // 
            this.comboCategory2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategory2.FormattingEnabled = true;
            this.comboCategory2.Location = new System.Drawing.Point(646, 186);
            this.comboCategory2.Name = "comboCategory2";
            this.comboCategory2.Size = new System.Drawing.Size(220, 26);
            this.comboCategory2.TabIndex = 64;
            this.comboCategory2.SelectionChangeCommitted += new System.EventHandler(this.comboCategory2_SelectionChangeCommitted);
            // 
            // comboCategory1
            // 
            this.comboCategory1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategory1.FormattingEnabled = true;
            this.comboCategory1.Location = new System.Drawing.Point(646, 154);
            this.comboCategory1.Name = "comboCategory1";
            this.comboCategory1.Size = new System.Drawing.Size(220, 26);
            this.comboCategory1.TabIndex = 63;
            this.comboCategory1.SelectionChangeCommitted += new System.EventHandler(this.comboCategory1_SelectionChangeCommitted);
            // 
            // lblCategory1
            // 
            this.lblCategory1.AutoSize = true;
            this.lblCategory1.Font = new System.Drawing.Font("Arial", 12F);
            this.lblCategory1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCategory1.Location = new System.Drawing.Point(886, 157);
            this.lblCategory1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCategory1.Name = "lblCategory1";
            this.lblCategory1.Size = new System.Drawing.Size(43, 18);
            this.lblCategory1.TabIndex = 62;
            this.lblCategory1.Text = "טוען...";
            // 
            // LabelTotalDuration2
            // 
            this.LabelTotalDuration2.BackColor = System.Drawing.Color.White;
            this.LabelTotalDuration2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.LabelTotalDuration2.Cursor = System.Windows.Forms.Cursors.Default;
            this.LabelTotalDuration2.Font = new System.Drawing.Font("Arial", 12F);
            this.LabelTotalDuration2.ForeColor = System.Drawing.Color.Black;
            this.LabelTotalDuration2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.LabelTotalDuration2.Location = new System.Drawing.Point(758, 358);
            this.LabelTotalDuration2.Name = "LabelTotalDuration2";
            this.LabelTotalDuration2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LabelTotalDuration2.Size = new System.Drawing.Size(108, 22);
            this.LabelTotalDuration2.TabIndex = 61;
            this.LabelTotalDuration2.Text = "00:00:00.000";
            this.LabelTotalDuration2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // comboStatus
            // 
            this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboStatus.FormattingEnabled = true;
            this.comboStatus.Location = new System.Drawing.Point(646, 324);
            this.comboStatus.Name = "comboStatus";
            this.comboStatus.Size = new System.Drawing.Size(220, 26);
            this.comboStatus.TabIndex = 60;
            // 
            // comboCategory3
            // 
            this.comboCategory3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategory3.FormattingEnabled = true;
            this.comboCategory3.Location = new System.Drawing.Point(646, 221);
            this.comboCategory3.Name = "comboCategory3";
            this.comboCategory3.Size = new System.Drawing.Size(220, 26);
            this.comboCategory3.TabIndex = 57;
            this.comboCategory3.SelectionChangeCommitted += new System.EventHandler(this.comboCategory3_SelectionChangeCommitted);
            // 
            // textBox7
            // 
            this.textBox7.Font = new System.Drawing.Font("Arial", 12F);
            this.textBox7.Location = new System.Drawing.Point(465, 384);
            this.textBox7.Margin = new System.Windows.Forms.Padding(4);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(333, 26);
            this.textBox7.TabIndex = 55;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(838, 387);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(60, 18);
            this.label12.TabIndex = 54;
            this.label12.Text = "פורסם ב";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.sop_teacher1l);
            this.groupBox5.Controls.Add(this.sop_studentl);
            this.groupBox5.Controls.Add(this.sop_teacher2l);
            this.groupBox5.Controls.Add(this.sop_teacherAndStudentl);
            this.groupBox5.Font = new System.Drawing.Font("Arial", 9F);
            this.groupBox5.Location = new System.Drawing.Point(16, 37);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(124, 161);
            this.groupBox5.TabIndex = 53;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "נעילות לימוד";
            // 
            // sop_teacher1l
            // 
            this.sop_teacher1l.AutoSize = true;
            this.sop_teacher1l.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.sop_teacher1l.Location = new System.Drawing.Point(50, 26);
            this.sop_teacher1l.Name = "sop_teacher1l";
            this.sop_teacher1l.Size = new System.Drawing.Size(59, 19);
            this.sop_teacher1l.TabIndex = 46;
            this.sop_teacher1l.Text = "מורה 1";
            this.sop_teacher1l.UseVisualStyleBackColor = true;
            // 
            // sop_studentl
            // 
            this.sop_studentl.AutoSize = true;
            this.sop_studentl.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.sop_studentl.Location = new System.Drawing.Point(53, 110);
            this.sop_studentl.Name = "sop_studentl";
            this.sop_studentl.Size = new System.Drawing.Size(56, 19);
            this.sop_studentl.TabIndex = 47;
            this.sop_studentl.Text = "תלמיד";
            this.sop_studentl.UseVisualStyleBackColor = true;
            // 
            // sop_teacher2l
            // 
            this.sop_teacher2l.AutoSize = true;
            this.sop_teacher2l.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.sop_teacher2l.Location = new System.Drawing.Point(50, 82);
            this.sop_teacher2l.Name = "sop_teacher2l";
            this.sop_teacher2l.Size = new System.Drawing.Size(59, 19);
            this.sop_teacher2l.TabIndex = 48;
            this.sop_teacher2l.Text = "מורה 2";
            this.sop_teacher2l.UseVisualStyleBackColor = true;
            // 
            // sop_teacherAndStudentl
            // 
            this.sop_teacherAndStudentl.AutoSize = true;
            this.sop_teacherAndStudentl.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.sop_teacherAndStudentl.Location = new System.Drawing.Point(24, 54);
            this.sop_teacherAndStudentl.Name = "sop_teacherAndStudentl";
            this.sop_teacherAndStudentl.Size = new System.Drawing.Size(85, 19);
            this.sop_teacherAndStudentl.TabIndex = 49;
            this.sop_teacherAndStudentl.Text = "מורה ותלמיד";
            this.sop_teacherAndStudentl.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.sop_teacher1);
            this.groupBox4.Controls.Add(this.sop_student);
            this.groupBox4.Controls.Add(this.sop_teacher2);
            this.groupBox4.Controls.Add(this.sop_teacherAndStudent);
            this.groupBox4.Font = new System.Drawing.Font("Arial", 9F);
            this.groupBox4.Location = new System.Drawing.Point(16, 219);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(124, 154);
            this.groupBox4.TabIndex = 52;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "אפשרויות לימוד";
            // 
            // sop_teacher1
            // 
            this.sop_teacher1.AutoSize = true;
            this.sop_teacher1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.sop_teacher1.Location = new System.Drawing.Point(50, 30);
            this.sop_teacher1.Name = "sop_teacher1";
            this.sop_teacher1.Size = new System.Drawing.Size(59, 19);
            this.sop_teacher1.TabIndex = 39;
            this.sop_teacher1.Text = "מורה 1";
            this.sop_teacher1.UseVisualStyleBackColor = true;
            // 
            // sop_student
            // 
            this.sop_student.AutoSize = true;
            this.sop_student.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.sop_student.Location = new System.Drawing.Point(53, 114);
            this.sop_student.Name = "sop_student";
            this.sop_student.Size = new System.Drawing.Size(56, 19);
            this.sop_student.TabIndex = 40;
            this.sop_student.Text = "תלמיד";
            this.sop_student.UseVisualStyleBackColor = true;
            // 
            // sop_teacher2
            // 
            this.sop_teacher2.AutoSize = true;
            this.sop_teacher2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.sop_teacher2.Location = new System.Drawing.Point(50, 86);
            this.sop_teacher2.Name = "sop_teacher2";
            this.sop_teacher2.Size = new System.Drawing.Size(59, 19);
            this.sop_teacher2.TabIndex = 41;
            this.sop_teacher2.Text = "מורה 2";
            this.sop_teacher2.UseVisualStyleBackColor = true;
            // 
            // sop_teacherAndStudent
            // 
            this.sop_teacherAndStudent.AutoSize = true;
            this.sop_teacherAndStudent.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.sop_teacherAndStudent.Location = new System.Drawing.Point(24, 58);
            this.sop_teacherAndStudent.Name = "sop_teacherAndStudent";
            this.sop_teacherAndStudent.Size = new System.Drawing.Size(85, 19);
            this.sop_teacherAndStudent.TabIndex = 42;
            this.sop_teacherAndStudent.Text = "מורה ותלמיד";
            this.sop_teacherAndStudent.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.loc_par);
            this.groupBox6.Controls.Add(this.loc_wor);
            this.groupBox6.Controls.Add(this.loc_sec);
            this.groupBox6.Controls.Add(this.loc_sen);
            this.groupBox6.Font = new System.Drawing.Font("Arial", 9F);
            this.groupBox6.Location = new System.Drawing.Point(166, 37);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(129, 161);
            this.groupBox6.TabIndex = 51;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "נעילות מקטעים";
            // 
            // loc_par
            // 
            this.loc_par.AutoSize = true;
            this.loc_par.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.loc_par.Location = new System.Drawing.Point(44, 26);
            this.loc_par.Name = "loc_par";
            this.loc_par.Size = new System.Drawing.Size(54, 19);
            this.loc_par.TabIndex = 34;
            this.loc_par.Text = "פסקה";
            this.loc_par.UseVisualStyleBackColor = true;
            // 
            // loc_wor
            // 
            this.loc_wor.AutoSize = true;
            this.loc_wor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.loc_wor.Location = new System.Drawing.Point(46, 110);
            this.loc_wor.Name = "loc_wor";
            this.loc_wor.Size = new System.Drawing.Size(52, 19);
            this.loc_wor.TabIndex = 35;
            this.loc_wor.Text = "מילים";
            this.loc_wor.UseVisualStyleBackColor = true;
            // 
            // loc_sec
            // 
            this.loc_sec.AutoSize = true;
            this.loc_sec.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.loc_sec.Location = new System.Drawing.Point(52, 82);
            this.loc_sec.Name = "loc_sec";
            this.loc_sec.Size = new System.Drawing.Size(46, 19);
            this.loc_sec.TabIndex = 36;
            this.loc_sec.Text = "קטע";
            this.loc_sec.UseVisualStyleBackColor = true;
            // 
            // loc_sen
            // 
            this.loc_sen.AutoSize = true;
            this.loc_sen.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.loc_sen.Location = new System.Drawing.Point(43, 54);
            this.loc_sen.Name = "loc_sen";
            this.loc_sen.Size = new System.Drawing.Size(55, 19);
            this.loc_sen.TabIndex = 37;
            this.loc_sen.Text = "משפט";
            this.loc_sen.UseVisualStyleBackColor = true;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.def_sen);
            this.groupBox7.Controls.Add(this.def_par);
            this.groupBox7.Controls.Add(this.def_wor);
            this.groupBox7.Controls.Add(this.def_sec);
            this.groupBox7.Font = new System.Drawing.Font("Arial", 9F);
            this.groupBox7.Location = new System.Drawing.Point(166, 219);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(129, 151);
            this.groupBox7.TabIndex = 50;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "מקטעי ברירת מחדל";
            // 
            // def_sen
            // 
            this.def_sen.AutoSize = true;
            this.def_sen.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.def_sen.Location = new System.Drawing.Point(43, 58);
            this.def_sen.Name = "def_sen";
            this.def_sen.Size = new System.Drawing.Size(55, 19);
            this.def_sen.TabIndex = 32;
            this.def_sen.Text = "משפט";
            this.def_sen.UseVisualStyleBackColor = true;
            // 
            // def_par
            // 
            this.def_par.AutoSize = true;
            this.def_par.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.def_par.Location = new System.Drawing.Point(44, 30);
            this.def_par.Name = "def_par";
            this.def_par.Size = new System.Drawing.Size(54, 19);
            this.def_par.TabIndex = 29;
            this.def_par.Text = "פסקה";
            this.def_par.UseVisualStyleBackColor = true;
            // 
            // def_wor
            // 
            this.def_wor.AutoSize = true;
            this.def_wor.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.def_wor.Location = new System.Drawing.Point(46, 114);
            this.def_wor.Name = "def_wor";
            this.def_wor.Size = new System.Drawing.Size(52, 19);
            this.def_wor.TabIndex = 30;
            this.def_wor.Text = "מילים";
            this.def_wor.UseVisualStyleBackColor = true;
            // 
            // def_sec
            // 
            this.def_sec.AutoSize = true;
            this.def_sec.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.def_sec.Location = new System.Drawing.Point(52, 86);
            this.def_sec.Name = "def_sec";
            this.def_sec.Size = new System.Drawing.Size(46, 19);
            this.def_sec.TabIndex = 31;
            this.def_sec.Text = "קטע";
            this.def_sec.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 12F);
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(886, 120);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(41, 18);
            this.label13.TabIndex = 43;
            this.label13.Text = "תיאור";
            // 
            // tbClipDescription
            // 
            this.tbClipDescription.Font = new System.Drawing.Font("Arial", 12F);
            this.tbClipDescription.Location = new System.Drawing.Point(313, 117);
            this.tbClipDescription.Margin = new System.Windows.Forms.Padding(4);
            this.tbClipDescription.Multiline = true;
            this.tbClipDescription.Name = "tbClipDescription";
            this.tbClipDescription.Size = new System.Drawing.Size(553, 30);
            this.tbClipDescription.TabIndex = 44;
            this.tbClipDescription.TextChanged += new System.EventHandler(this.tbClipDescription_TextChanged);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(887, 358);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(74, 18);
            this.label15.TabIndex = 21;
            this.label15.Text = "משך שיעור";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial", 12F);
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(886, 328);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(56, 18);
            this.label16.TabIndex = 16;
            this.label16.Text = "סטאטוס";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Arial", 12F);
            this.label19.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label19.Location = new System.Drawing.Point(886, 294);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(84, 18);
            this.label19.TabIndex = 14;
            this.label19.Text = "מילות מפתח";
            // 
            // tbKeywords
            // 
            this.tbKeywords.Font = new System.Drawing.Font("Arial", 12F);
            this.tbKeywords.Location = new System.Drawing.Point(646, 291);
            this.tbKeywords.Margin = new System.Windows.Forms.Padding(4);
            this.tbKeywords.Name = "tbKeywords";
            this.tbKeywords.Size = new System.Drawing.Size(220, 26);
            this.tbKeywords.TabIndex = 15;
            this.tbKeywords.TextChanged += new System.EventHandler(this.tbKeywords_TextChanged);
            // 
            // lblCategory3
            // 
            this.lblCategory3.AutoSize = true;
            this.lblCategory3.Font = new System.Drawing.Font("Arial", 12F);
            this.lblCategory3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCategory3.Location = new System.Drawing.Point(886, 224);
            this.lblCategory3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCategory3.Name = "lblCategory3";
            this.lblCategory3.Size = new System.Drawing.Size(43, 18);
            this.lblCategory3.TabIndex = 10;
            this.lblCategory3.Text = "טוען...";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Arial", 12F);
            this.label22.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label22.Location = new System.Drawing.Point(886, 40);
            this.label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(66, 18);
            this.label22.TabIndex = 1;
            this.label22.Text = "שם שיעור";
            // 
            // tbClipName
            // 
            this.tbClipName.Font = new System.Drawing.Font("Arial", 12F);
            this.tbClipName.Location = new System.Drawing.Point(313, 37);
            this.tbClipName.Margin = new System.Windows.Forms.Padding(4);
            this.tbClipName.Name = "tbClipName";
            this.tbClipName.ReadOnly = true;
            this.tbClipName.Size = new System.Drawing.Size(553, 26);
            this.tbClipName.TabIndex = 2;
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Font = new System.Drawing.Font("Arial", 12F);
            this.maskedTextBox1.Location = new System.Drawing.Point(789, 73);
            this.maskedTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.maskedTextBox1.Mask = "0.00";
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.ReadOnly = true;
            this.maskedTextBox1.Size = new System.Drawing.Size(77, 26);
            this.maskedTextBox1.TabIndex = 3;
            this.maskedTextBox1.Text = "100";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Arial", 12F);
            this.label23.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label23.Location = new System.Drawing.Point(886, 76);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(41, 18);
            this.label23.TabIndex = 4;
            this.label23.Text = "גרסה";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.pictureBox2);
            this.panel4.Controls.Add(this.buttonPublish);
            this.panel4.Location = new System.Drawing.Point(4, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(160, 389);
            this.panel4.TabIndex = 11;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(14, 292);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(138, 20);
            this.pictureBox2.TabIndex = 20;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // buttonPublish
            // 
            this.buttonPublish.BackgroundImage = global::MyMentor.Properties.Resources._1386909646_519838_50_Cloud_Arrow_Up;
            this.buttonPublish.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonPublish.Font = new System.Drawing.Font("Arial", 12F);
            this.buttonPublish.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonPublish.Location = new System.Drawing.Point(13, 329);
            this.buttonPublish.Name = "buttonPublish";
            this.buttonPublish.Size = new System.Drawing.Size(139, 42);
            this.buttonPublish.TabIndex = 0;
            this.buttonPublish.Text = "פרסם שיעור";
            this.buttonPublish.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonPublish.UseVisualStyleBackColor = true;
            this.buttonPublish.Click += new System.EventHandler(this.buttonPublish_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "1386908105_Record Button2.png");
            this.imageList1.Images.SetKeyName(1, "1386908112_Record Button1.png");
            // 
            // panel1
            // 
            this.tableLayoutPanel4.SetColumnSpan(this.panel1, 2);
            this.panel1.Controls.Add(this.Picture1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 463);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1176, 154);
            this.panel1.TabIndex = 0;
            // 
            // Picture1
            // 
            this.Picture1.BackColor = System.Drawing.Color.Black;
            this.Picture1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Picture1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Picture1.Font = new System.Drawing.Font("Arial", 8F);
            this.Picture1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Picture1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.Picture1.Location = new System.Drawing.Point(0, 0);
            this.Picture1.Name = "Picture1";
            this.Picture1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Picture1.Size = new System.Drawing.Size(1176, 154);
            this.Picture1.TabIndex = 13;
            this.Picture1.TabStop = false;
            this.Picture1.Visible = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.progressBar1);
            this.panel3.Controls.Add(this.LabelStatus);
            this.panel3.Location = new System.Drawing.Point(686, 623);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(493, 39);
            this.panel3.TabIndex = 25;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.כליםToolStripMenuItem,
            this.טקסטToolStripMenuItem,
            this.mnuAudio,
            this.תזמוןToolStripMenuItem,
            this.עזרהToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1182, 24);
            this.menuStrip1.TabIndex = 62;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile_NewClip,
            this.toolStripMenuItem9,
            this.mnuFile_Open,
            this.mnuFile_Save,
            this.mnuFile_SaveAs,
            this.toolStripMenuItem10,
            this.mnuFile_Parse,
            this.toolStripMenuItem11,
            this.mnuFile_Exit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(46, 20);
            this.mnuFile.Text = "קובץ";
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
            this.mnuFile_Open.Click += new System.EventHandler(this.mnuFile_Open_Click);
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
            this.mnuFile_SaveAs.Enabled = false;
            this.mnuFile_SaveAs.Name = "mnuFile_SaveAs";
            this.mnuFile_SaveAs.Size = new System.Drawing.Size(177, 22);
            this.mnuFile_SaveAs.Text = "שמור שיעור בשם...";
            this.mnuFile_SaveAs.Click += new System.EventHandler(this.mnuFile_SaveAs_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(174, 6);
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
            // mnuFile_Exit
            // 
            this.mnuFile_Exit.Name = "mnuFile_Exit";
            this.mnuFile_Exit.Size = new System.Drawing.Size(177, 22);
            this.mnuFile_Exit.Text = "יציאה";
            this.mnuFile_Exit.Click += new System.EventHandler(this.mnuFile_Exit_Click);
            // 
            // כליםToolStripMenuItem
            // 
            this.כליםToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.שפתממשקToolStripMenuItem,
            this.toolStripMenuItem16,
            this.mnuLoginDifferentUser});
            this.כליםToolStripMenuItem.Name = "כליםToolStripMenuItem";
            this.כליםToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.כליםToolStripMenuItem.Text = "כלים";
            // 
            // שפתממשקToolStripMenuItem
            // 
            this.שפתממשקToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTools_UI_Hebrew,
            this.mnuTools_UI_English});
            this.שפתממשקToolStripMenuItem.Name = "שפתממשקToolStripMenuItem";
            this.שפתממשקToolStripMenuItem.Size = new System.Drawing.Size(200, 22);
            this.שפתממשקToolStripMenuItem.Text = "שפת ממשק";
            // 
            // mnuTools_UI_Hebrew
            // 
            this.mnuTools_UI_Hebrew.Checked = true;
            this.mnuTools_UI_Hebrew.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuTools_UI_Hebrew.Name = "mnuTools_UI_Hebrew";
            this.mnuTools_UI_Hebrew.Size = new System.Drawing.Size(112, 22);
            this.mnuTools_UI_Hebrew.Text = "עברית";
            this.mnuTools_UI_Hebrew.Click += new System.EventHandler(this.mnuTools_UI_Hebrew_Click);
            // 
            // mnuTools_UI_English
            // 
            this.mnuTools_UI_English.Checked = true;
            this.mnuTools_UI_English.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuTools_UI_English.Name = "mnuTools_UI_English";
            this.mnuTools_UI_English.Size = new System.Drawing.Size(112, 22);
            this.mnuTools_UI_English.Text = "English";
            this.mnuTools_UI_English.Click += new System.EventHandler(this.mnuTools_UI_English_Click);
            // 
            // toolStripMenuItem16
            // 
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            this.toolStripMenuItem16.Size = new System.Drawing.Size(197, 6);
            // 
            // mnuLoginDifferentUser
            // 
            this.mnuLoginDifferentUser.Name = "mnuLoginDifferentUser";
            this.mnuLoginDifferentUser.Size = new System.Drawing.Size(200, 22);
            this.mnuLoginDifferentUser.Text = "התחבר כמשתמש אחר";
            this.mnuLoginDifferentUser.Click += new System.EventHandler(this.mnuLoginDifferentUser_Click);
            // 
            // טקסטToolStripMenuItem
            // 
            this.טקסטToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuText_Goto,
            this.toolStripMenuItem14,
            this.mnuAnchors});
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
            // toolStripMenuItem14
            // 
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Size = new System.Drawing.Size(147, 6);
            // 
            // mnuAnchors
            // 
            this.mnuAnchors.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAnchors_RemoveAll,
            this.toolStripMenuItem15,
            this.mnuAnchors_RemoveParagraphs,
            this.mnuAnchors_RemoveSentenses,
            this.mnuAnchors_RemoveSections,
            this.mnuAnchors_RemoveWords});
            this.mnuAnchors.Name = "mnuAnchors";
            this.mnuAnchors.Size = new System.Drawing.Size(150, 22);
            this.mnuAnchors.Text = "עוגנים";
            // 
            // mnuAnchors_RemoveAll
            // 
            this.mnuAnchors_RemoveAll.Name = "mnuAnchors_RemoveAll";
            this.mnuAnchors_RemoveAll.Size = new System.Drawing.Size(177, 22);
            this.mnuAnchors_RemoveAll.Text = "נקה כל העוגנים";
            this.mnuAnchors_RemoveAll.Click += new System.EventHandler(this.mnuAnchors_RemoveAll_Click);
            // 
            // toolStripMenuItem15
            // 
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Size = new System.Drawing.Size(174, 6);
            // 
            // mnuAnchors_RemoveParagraphs
            // 
            this.mnuAnchors_RemoveParagraphs.Name = "mnuAnchors_RemoveParagraphs";
            this.mnuAnchors_RemoveParagraphs.Size = new System.Drawing.Size(177, 22);
            this.mnuAnchors_RemoveParagraphs.Text = "הסר עוגני פסקאות";
            this.mnuAnchors_RemoveParagraphs.Click += new System.EventHandler(this.mnuAnchors_RemoveParagraphs_Click);
            // 
            // mnuAnchors_RemoveSentenses
            // 
            this.mnuAnchors_RemoveSentenses.Name = "mnuAnchors_RemoveSentenses";
            this.mnuAnchors_RemoveSentenses.Size = new System.Drawing.Size(177, 22);
            this.mnuAnchors_RemoveSentenses.Text = "הסר עוגני משפטים";
            this.mnuAnchors_RemoveSentenses.Click += new System.EventHandler(this.mnuAnchors_RemoveSentenses_Click);
            // 
            // mnuAnchors_RemoveSections
            // 
            this.mnuAnchors_RemoveSections.Name = "mnuAnchors_RemoveSections";
            this.mnuAnchors_RemoveSections.Size = new System.Drawing.Size(177, 22);
            this.mnuAnchors_RemoveSections.Text = "הסר עוגני קטעים";
            this.mnuAnchors_RemoveSections.Click += new System.EventHandler(this.mnuAnchors_RemoveSections_Click);
            // 
            // mnuAnchors_RemoveWords
            // 
            this.mnuAnchors_RemoveWords.Name = "mnuAnchors_RemoveWords";
            this.mnuAnchors_RemoveWords.Size = new System.Drawing.Size(177, 22);
            this.mnuAnchors_RemoveWords.Text = "הסר עוגני מילים";
            this.mnuAnchors_RemoveWords.Click += new System.EventHandler(this.mnuAnchors_RemoveWords_Click);
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
            this.mnuZoom,
            this.toolStripMenuItem12,
            this.mnuAudio_Test});
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
            this.mnuAudioOptions.Size = new System.Drawing.Size(210, 22);
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
            this.toolStripMenuItem4.Size = new System.Drawing.Size(207, 6);
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
            this.mnuAudioSelectedPart.Size = new System.Drawing.Size(210, 22);
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
            this.toolStripMenuItem5.Size = new System.Drawing.Size(207, 6);
            // 
            // mnuAudioEffects
            // 
            this.mnuAudioEffects.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAudioEffects_Equalizer,
            this.mnuAudioEffects_Tempo,
            this.mnuAudioEffects_Pitch,
            this.mnuAudioEffects_PlaybackRate});
            this.mnuAudioEffects.Name = "mnuAudioEffects";
            this.mnuAudioEffects.Size = new System.Drawing.Size(210, 22);
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
            this.toolStripMenuItem6.Size = new System.Drawing.Size(207, 6);
            // 
            // mnuZoom
            // 
            this.mnuZoom.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuZoom_Selection,
            this.mnuZoom_AllClip,
            this.mnuZoom_In,
            this.mnuZoom_Out});
            this.mnuZoom.Name = "mnuZoom";
            this.mnuZoom.Size = new System.Drawing.Size(210, 22);
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
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(207, 6);
            // 
            // mnuAudio_Test
            // 
            this.mnuAudio_Test.Name = "mnuAudio_Test";
            this.mnuAudio_Test.Size = new System.Drawing.Size(210, 22);
            this.mnuAudio_Test.Text = "בדיקת התקני קלט ושמע";
            this.mnuAudio_Test.Click += new System.EventHandler(this.mnuAudio_Test_Click);
            // 
            // תזמוןToolStripMenuItem
            // 
            this.תזמוןToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuRemoveSchedule});
            this.תזמוןToolStripMenuItem.Name = "תזמוןToolStripMenuItem";
            this.תזמוןToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.תזמוןToolStripMenuItem.Text = "תזמון";
            // 
            // mnuRemoveSchedule
            // 
            this.mnuRemoveSchedule.Name = "mnuRemoveSchedule";
            this.mnuRemoveSchedule.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.mnuRemoveSchedule.Size = new System.Drawing.Size(192, 22);
            this.mnuRemoveSchedule.Text = "נקה תזמונים";
            this.mnuRemoveSchedule.Click += new System.EventHandler(this.mnuRemoveSchedule_Click);
            // 
            // עזרהToolStripMenuItem
            // 
            this.עזרהToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelp_About,
            this.toolStripMenuItem13,
            this.mnuHelp_ShowJSON});
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
            // audioDjStudio1
            // 
            this.audioDjStudio1.Fader = ((AudioDjStudio.FaderObject)(resources.GetObject("audioDjStudio1.Fader")));
            this.audioDjStudio1.LastError = AudioDjStudio.enumErrorCodes.ERR_NOERROR;
            this.audioDjStudio1.Location = new System.Drawing.Point(594, 0);
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
            this.timerUpdateTimePickerSpinner.Tick += new System.EventHandler(this.timerCheckPlaybackDuringScheduling_Tick);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.panel3, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.panel5, 1, 2);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1182, 665);
            this.tableLayoutPanel4.TabIndex = 64;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.lblLoginUser);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(3, 623);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(585, 39);
            this.panel5.TabIndex = 62;
            // 
            // lblLoginUser
            // 
            this.lblLoginUser.BackColor = System.Drawing.SystemColors.Control;
            this.lblLoginUser.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLoginUser.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblLoginUser.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold);
            this.lblLoginUser.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblLoginUser.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblLoginUser.Location = new System.Drawing.Point(340, 9);
            this.lblLoginUser.Name = "lblLoginUser";
            this.lblLoginUser.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblLoginUser.Size = new System.Drawing.Size(229, 24);
            this.lblLoginUser.TabIndex = 26;
            this.lblLoginUser.Text = "הנך מחובר כ";
            // 
            // timerRecordIcon
            // 
            this.timerRecordIcon.Interval = 500;
            this.timerRecordIcon.Tick += new System.EventHandler(this.timerRecordIcon_Tick);
            // 
            // timerStartRecordingAfterPlayingBuffer
            // 
            this.timerStartRecordingAfterPlayingBuffer.Interval = 50;
            this.timerStartRecordingAfterPlayingBuffer.Tick += new System.EventHandler(this.timerStartRecordingAfterPlayingBuffer_Tick);
            // 
            // timerRefreshLedDisplay
            // 
            this.timerRefreshLedDisplay.Interval = 1000;
            this.timerRefreshLedDisplay.Tick += new System.EventHandler(this.timerRefreshLedDisplay_Tick);
            // 
            // timerPreStartFixPlayback
            // 
            this.timerPreStartFixPlayback.Tick += new System.EventHandler(this.timerPreStartFixPlayback_Tick);
            // 
            // comboBoxAutoDevidePar
            // 
            checkBoxProperties1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBoxAutoDevidePar.CheckBoxProperties = checkBoxProperties1;
            this.comboBoxAutoDevidePar.DisplayMemberSingleItem = "";
            this.comboBoxAutoDevidePar.FormattingEnabled = true;
            this.comboBoxAutoDevidePar.Items.AddRange(new object[] {
            "ENTER",
            "שני ENTER",
            "אחרי 2 משפטים",
            "אחרי 3 משפטים",
            "אחרי 4 משפטים"});
            this.comboBoxAutoDevidePar.Location = new System.Drawing.Point(431, 31);
            this.comboBoxAutoDevidePar.Name = "comboBoxAutoDevidePar";
            this.comboBoxAutoDevidePar.Size = new System.Drawing.Size(188, 26);
            this.comboBoxAutoDevidePar.TabIndex = 19;
            // 
            // comboBoxAutoDevideSen
            // 
            checkBoxProperties2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.comboBoxAutoDevideSen.CheckBoxProperties = checkBoxProperties2;
            this.comboBoxAutoDevideSen.DisplayMemberSingleItem = "";
            this.comboBoxAutoDevideSen.FormattingEnabled = true;
            this.comboBoxAutoDevideSen.Items.AddRange(new object[] {
            "נקודותיים (:)",
            "נקודה (.)",
            "ENTER"});
            this.comboBoxAutoDevideSen.Location = new System.Drawing.Point(120, 31);
            this.comboBoxAutoDevideSen.Name = "comboBoxAutoDevideSen";
            this.comboBoxAutoDevideSen.Size = new System.Drawing.Size(191, 26);
            this.comboBoxAutoDevideSen.TabIndex = 16;
            // 
            // sevenSegment1
            // 
            this.sevenSegment1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sevenSegment1.ColorBackground = System.Drawing.Color.DarkGray;
            this.sevenSegment1.ColorDark = System.Drawing.Color.DimGray;
            this.sevenSegment1.ColorLight = System.Drawing.Color.Lime;
            this.sevenSegment1.CustomPattern = 107;
            this.sevenSegment1.DecimalOn = false;
            this.sevenSegment1.DecimalShow = true;
            this.sevenSegment1.ElementWidth = 10;
            this.sevenSegment1.ItalicFactor = 0F;
            this.sevenSegment1.Location = new System.Drawing.Point(18, 25);
            this.sevenSegment1.Name = "sevenSegment1";
            this.sevenSegment1.Padding = new System.Windows.Forms.Padding(4);
            this.sevenSegment1.Size = new System.Drawing.Size(53, 82);
            this.sevenSegment1.TabIndex = 37;
            this.sevenSegment1.TabStop = false;
            this.sevenSegment1.Value = "5";
            // 
            // timePickerCurrentWord
            // 
            this.timePickerCurrentWord.Location = new System.Drawing.Point(7, 72);
            this.timePickerCurrentWord.Margin = new System.Windows.Forms.Padding(4);
            this.timePickerCurrentWord.Name = "timePickerCurrentWord";
            this.timePickerCurrentWord.Size = new System.Drawing.Size(181, 36);
            this.timePickerCurrentWord.TabIndex = 25;
            this.timePickerCurrentWord.Value = System.TimeSpan.Parse("00:00:00");
            this.timePickerCurrentWord.ValueChanged += new System.EventHandler(this.timePickerSpinner1_ValueChanged);
            // 
            // comboClipType
            // 
            this.comboClipType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboClipType.FormattingEnabled = true;
            this.comboClipType.Location = new System.Drawing.Point(362, 300);
            this.comboClipType.Name = "comboClipType";
            this.comboClipType.Size = new System.Drawing.Size(155, 26);
            this.comboClipType.TabIndex = 73;
            this.comboClipType.Visible = false;
            // 
            // lblClipType
            // 
            this.lblClipType.AutoSize = true;
            this.lblClipType.Font = new System.Drawing.Font("Arial", 12F);
            this.lblClipType.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblClipType.Location = new System.Drawing.Point(524, 303);
            this.lblClipType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClipType.Name = "lblClipType";
            this.lblClipType.Size = new System.Drawing.Size(63, 18);
            this.lblClipType.TabIndex = 74;
            this.lblClipType.Text = "סוג שיעור";
            this.lblClipType.Visible = false;
            // 
            // FormMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 19);
            this.ClientSize = new System.Drawing.Size(1182, 689);
            this.Controls.Add(this.tableLayoutPanel4);
            this.Controls.Add(this.audioDjStudio1);
            this.Controls.Add(this.audioSoundEditor1);
            this.Controls.Add(this.audioSoundRecorder1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Arial Narrow", 12F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MyMentor";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.ResizeEnd += new System.EventHandler(this.FormMain_ResizeEnd);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormMain_Paint);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.Frame4.ResumeLayout(false);
            this.Frame4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarVolume1)).EndInit();
            this.framePlayback.ResumeLayout(false);
            this.FrameRecording.ResumeLayout(false);
            this.FrameRecording.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBufferRecord)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownInterval)).EndInit();
            this.panel6.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Picture1)).EndInit();
            this.panel3.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
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
            ApplyLanguageUI();

            rtbMainEditorGraphics = richTextBox1.CreateGraphics();
            rtbAlternateEditorGraphics = richTextBox3.CreateGraphics();

            // init controls
            audioSoundRecorder1.InitRecordingSystem();
            audioSoundEditor1.InitEditor();
            trackBarVolume1.Value = 100;

            // create the recorder's VU-Meter
            audioSoundRecorder1.DisplayVUMeter.Create(IntPtr.Zero);
            m_hWndVuMeterLeft = CreateVuMeter(label17, AudioSoundRecorder.enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL);
            m_hWndVuMeterRight = CreateVuMeter(label18, AudioSoundRecorder.enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL);

            // create the waveform analyzer (always call this function on the end of the form's Load fucntion)
            audioSoundEditor1.DisplayWaveformAnalyzer.Create(panel1.Handle, Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);
            // get the current analyzer wave settings
            WANALYZER_WAVEFORM_SETTINGS settingsWave = new WANALYZER_WAVEFORM_SETTINGS();
            audioSoundEditor1.DisplayWaveformAnalyzer.SettingsWaveGet(ref settingsWave);
            settingsWave.nStereoVisualizationMode = enumWaveformStereoModes.STEREO_MODE_CHANNELS_MIXED;

            // apply the new settings
            audioSoundEditor1.DisplayWaveformAnalyzer.SettingsWaveSet(settingsWave);

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
            tableLayoutPanel4.RowStyles[1].Height = 0;

            audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_MP3;
            audioSoundEditor1.EncodeFormats.MP3.EncodeMode = enumMp3EncodeModes.MP3_ENCODE_PRESETS;
            audioSoundEditor1.EncodeFormats.MP3.Preset = enumMp3EncodePresets.MP3_PRESET_STANDARD;

        }

        private void buttonPlay_Click(object sender, System.EventArgs e)
        {
            audioDjStudio1.LoadSoundFromEditingSession(0, audioSoundEditor1.Handle);

             bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // if a selection is available
            if (bSelectionAvailable)
            {
                if (nEndSelectionInMs > nBeginSelectionInMs)
                {
                    // play selected range only
                    audioSoundEditor1.PlaySoundRange(nBeginSelectionInMs, nEndSelectionInMs);
                }
                else
                {
                    // play selected range only
                    audioSoundEditor1.PlaySoundRange(nBeginSelectionInMs, -1);
                }
            }
            else
            {
                audioSoundEditor1.PlaySound();
            }
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

            mnuFile_NewClip.Enabled = true;
            mnuFile_Open.Enabled = true;
            mnuFile_Save.Enabled = true;

            tbrNew.Enabled = true;
            tbrOpen.Enabled = true;
            tbrSave.Enabled = true;

            if (Clip.Current.Saved)
            {
                mnuFile_SaveAs.Enabled = true;
            }

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
                LabelTotalDuration2.Text = "00:00:00.000";
                return;
            }

            // display updated sound duration
            LabelTotalDuration.Text = audioSoundEditor1.GetFormattedTime(nDurationInMs, true, true);
            LabelTotalDuration.Refresh();

            LabelTotalDuration2.Text = audioSoundEditor1.GetFormattedTime(nDurationInMs, true, true);
            LabelTotalDuration2.Refresh();

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
            Cursor.Current = Cursors.Default;

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

            Cursor.Current = Cursors.Default;

            Clip.Current.Saved = true;

            if (e.nResult != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show(e.nResult.ToString());
            else
            {
                MessageBox.Show("השיעור נשמר בהצלחה!", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                mnuFile_SaveAs.Enabled = true;
            }

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

                LabelTotalDuration2.Text = audioSoundEditor1.GetFormattedTime(nDuration, true, true);
                LabelTotalDuration2.Refresh();
            }
        }

        private void audioSoundEditor1_SoundPlaybackDone(object sender, System.EventArgs e)
        {
            buttonPause.Text = "השהה";
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            buttonStartSchedulingPlayback.Text = "התחל";
            buttonRestartScheduling.Enabled = true;
            timePickerCurrentWord.Enabled = true;

            if (m_selectedAnchor)
            {
                buttonScheduleAnchor.Enabled = true;
            }

            //in case just finished playing anchor fix
            if (m_rem_anchorFixRecording > TimeSpan.Zero)
            {
                timePickerCurrentWord.Value = m_rem_anchorFixRecording;
                m_rem_anchorFixRecording = TimeSpan.Zero;
            }

            FrameRecording.Enabled = true;

            if (m_bRecOverwriteMode)
            {
                timerStartRecordingAfterPlayingBuffer.Enabled = true;
            }
        }

        private void audioSoundEditor1_SoundPlaybackPlaying(object sender, System.EventArgs e)
        {
            buttonPause.Text = "השהה";
            LabelStatus.Text = "Status: Playing...";
            LabelStatus.Refresh();

            timePickerCurrentWord.Enabled = false;
        }

        private void audioSoundEditor1_SoundPlaybackPaused(object sender, System.EventArgs e)
        {
            buttonPause.Text = "המשך";
            LabelStatus.Text = "Status: Playback paused";
            LabelStatus.Refresh();

            int mm = audioSoundEditor1.GetPlaybackPosition();
            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, mm, mm);

        }

        private void audioSoundEditor1_SoundPlaybackStopped(object sender, System.EventArgs e)
        {
            buttonPause.Text = "השהה";
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            FrameRecording.Enabled = true;

            timerUpdateTimePickerSpinner.Enabled = false;
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
                {
                    // selection can be played
                    buttonPlaySelection.Enabled = true;
                    buttonRecOverwritePlayback.Enabled = false;
                }
                else
                {
                    // selection cannot be played because it's simply a position selection
                    buttonPlaySelection.Enabled = false;
                    buttonRecOverwritePlayback.Enabled = true;
                }

                // display formatted strings
                LabelSelectionBegin.Text = audioSoundEditor1.GetFormattedTime(e.nBeginPosInMs, true, true);
                LabelSelectionEnd.Text = audioSoundEditor1.GetFormattedTime(e.nEndPosInMs, true, true);
                LabelSelectionDuration.Text = audioSoundEditor1.GetFormattedTime(e.nEndPosInMs - e.nBeginPosInMs, true, true);

                TimeSpan tp = new TimeSpan(0, 0, 0, 0, e.nBeginPosInMs);

                if (new TimeSpan(0, 0, 0, 0, 1000 * (int)numericUpDownBufferRecord.Value) > new TimeSpan(0, 0, (int)tp.TotalSeconds))
                {
                    timerPreStartFixPlayback.Interval = (int)(new TimeSpan(0, 0, 0, 0, 1000 * (int)numericUpDownBufferRecord.Value) - new TimeSpan(0, 0, (int)tp.TotalSeconds)).TotalMilliseconds;
                }
                else
                {
                    timerPreStartFixPlayback.Interval = 1;
                }
                //check 
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

            mnuFile_NewClip.Enabled = false;
            mnuFile_Open.Enabled = false;
            mnuFile_Save.Enabled = false;

            tbrNew.Enabled = false;
            tbrOpen.Enabled = false;
            tbrSave.Enabled = false;
            mnuFile_SaveAs.Enabled = false;

            // force analysis of the loaded sound
            if (e.bResult == true)
            {
                audioSoundEditor1.OutputVolumeSet((short)trackBarVolume1.Value, enumVolumeScales.SCALE_LINEAR);

                TimerReload.Enabled = true;
            }
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
                result = MessageBox.Show("קיימת הקלטה בזיכרון, האם אתה בטוח ליצור חדש ?", "MyMentor", MessageBoxButtons.YesNo);
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
            timerRecordIcon.Enabled = false;
            // stop recording
            audioSoundRecorder1.Stop();
        }

        private void audioSoundRecorder1_RecordingStarted(object sender, System.EventArgs e)
        {
            buttonStartRecNew.Enabled = false;
            buttonStartRecAppend.Enabled = false;
            buttonStopRecording.Enabled = true;
            timerRecordIcon.Enabled = true;
            framePlayback.Enabled = false;
        }

        private void audioSoundRecorder1_RecordingStopped(object sender, AudioSoundRecorder.RecordingStoppedEventArgs e)
        {
            // force loading the recording session into the editor
            timerRecordingDone.Enabled = true;
            timerRecordIcon.Enabled = false;

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
            if (m_bRecOverwriteMode)
            {
                m_bRecOverwriteMode = false;
                TimeSpan startSelectionTime = TimeSpan.Parse(LabelSelectionBegin.Text);
                TimeSpan endSelectionTime = TimeSpan.Parse(LabelSelectionEnd.Text);

                // overwrite mode
                audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_OVERWRITE);
                audioSoundEditor1.SetOverwritePos((int)startSelectionTime.TotalMilliseconds, -1);// (int)endSelectionTime.TotalMilliseconds);
            }
            else if (m_bRecAppendMode)
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
            LabelTotalDuration2.Text = "00:00:00.000";

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

            audioSoundEditor1.DisplayWaveformAnalyzer.Move(Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);


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



        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            PaintGraphics();
            Clip.Current.IsDirty = true;

            Clip.Current.RtfText = richTextBox1.Rtf;
            Clip.Current.Text = richTextBox1.Text;
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
                var positionAlternateEditor = richTextBox3.GetPositionFromCharIndex(index + START_PAUSE_SECTION_ANCHOR.Length);
                r1.X = positionAlternateEditor.X + factor1 - width;
                r1.Y = positionAlternateEditor.Y;

                //anchor
                if (index + START_PAUSE_SECTION_ANCHOR.Length == richTextBox3.SelectionStart)
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

            Clip.Current.Text = richTextBox1.Text;
            Clip.Current.Devide();

            //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
            //Clip.Current.FontName = richTextBox1.Font.Name;

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

        private void SavePropertiesToClip()
        {
            //set properties
            Clip.Current.Name = tbClipName.Text;
            Clip.Current.Description = tbClipDescription.Text;
            Clip.Current.Status = (string)comboStatus.SelectedValue;
            Clip.Current.Category1 = (string)comboCategory1.SelectedValue;
            Clip.Current.Category2 = (string)comboCategory2.SelectedValue;
            Clip.Current.Category3 = (string)comboCategory3.SelectedValue;
            Clip.Current.Category4 = (string)comboCategory4.SelectedValue;
            Clip.Current.Keywords = tbKeywords.Text;
            Clip.Current.ClipType = comboClipType.SelectedValue != null ? (string)comboClipType.SelectedValue : "piL85bMGtR";

            Clip.Current.DefaultSections.paragraph = def_par.Checked ? 1 : 0;
            Clip.Current.DefaultSections.sentence = def_sen.Checked ? 1 : 0;
            Clip.Current.DefaultSections.section = def_sec.Checked ? 1 : 0;
            Clip.Current.DefaultSections.chapter = def_wor.Checked ? 1 : 0;

            Clip.Current.LockedSections.paragraph = loc_par.Checked ? 1 : 0;
            Clip.Current.LockedSections.sentence = loc_sen.Checked ? 1 : 0;
            Clip.Current.LockedSections.section = loc_sec.Checked ? 1 : 0;
            Clip.Current.LockedSections.chapter = loc_wor.Checked ? 1 : 0;

            Clip.Current.DefaultLearningOptions.teacher1 = sop_teacher1.Checked ? 1 : 0;
            Clip.Current.DefaultLearningOptions.teacherAndStudent = sop_teacherAndStudent.Checked ? 1 : 0;
            Clip.Current.DefaultLearningOptions.teacher2 = sop_teacher2.Checked ? 1 : 0;
            Clip.Current.DefaultLearningOptions.student = sop_student.Checked ? 1 : 0;

            Clip.Current.LockedLearningOptions.teacher1 = sop_teacher1l.Checked ? 1 : 0;
            Clip.Current.LockedLearningOptions.teacherAndStudent = sop_teacherAndStudentl.Checked ? 1 : 0;
            Clip.Current.LockedLearningOptions.teacher2 = sop_teacher2l.Checked ? 1 : 0;
            Clip.Current.LockedLearningOptions.student = sop_studentl.Checked ? 1 : 0;

        }

        private void Save(bool isSaveAs)
        {
            SavePropertiesToClip();

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
                    saveFileDialog1.FileName = Clip.Current.Name.ToValidFileName();
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

                    Clip.Current.Save(audioSoundEditor1);

                    if (audioSoundEditor1.GetSoundDuration() <= 0)
                    {
                        MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                        mnuFile_SaveAs.Enabled = true;
                        Clip.Current.Saved = true;
                    }
                    else
                    {
                        Cursor.Current = Cursors.WaitCursor;
                    }
                }
                else
                {
                    return;
                }
            }
            else
            {
                //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
                //Clip.Current.FontName = richTextBox1.Font.Name;

                Clip.Current.Save(audioSoundEditor1);

                if (audioSoundEditor1.GetSoundDuration() <= 0)
                {
                    MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    mnuFile_SaveAs.Enabled = true;
                    Clip.Current.Saved = true;
                }
                else
                {
                    Cursor.Current = Cursors.WaitCursor;
                }
            }
        }

        private void tbrParse_Click(object sender, EventArgs e)
        {
            Clip.Current.Text = richTextBox1.Text;
            Clip.Current.Devide();
        }

        private async void FormMain_Shown(object sender, EventArgs e)
        {
            richTextBox1.Focus();

            if (ParseUser.CurrentUser == null)
            {
                LoginForm frmLogin = new LoginForm();

                var result = frmLogin.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    Application.Exit();
                }
                else
                {
                    if (ParseUser.CurrentUser != null)
                    {
                        lblLoginUser.Text = "הנך מחובר כ-" + ParseUser.CurrentUser.Username;
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
            else
            {
                lblLoginUser.Text = "הנך מחובר כ-" + ParseUser.CurrentUser.Username;
            }

            if (!string.IsNullOrEmpty(m_initClip))
            {
                OpenClip(m_initClip);
            }
            else
            {
                NewClip();
            }

            WorldContentType contentType = await ParseTables.GetContentType();

            m_clipTitlePattern = contentType.ClipTitlePattern;

            this.comboCategory1.DisplayMember = "Value";
            this.comboCategory1.ValueMember = "ObjectId";
            this.comboCategory1.DataSource = (await ParseTables.GetCategory1(contentType.ObjectId)).Select(c => new Category
            {
                ObjectId = c.ObjectId,
                Value = c.Get<string>("value")
            }).ToList();

            this.comboCategory3.DisplayMember = "Value";
            this.comboCategory3.ValueMember = "ObjectId";
            this.comboCategory3.DataSource = (await ParseTables.GetCategory3(contentType.ObjectId)).Select(c => new Category
            {
                ObjectId = c.ObjectId,
                Value = c.Get<string>("value")
            }).ToList();

            this.comboCategory4.DisplayMember = "Value";
            this.comboCategory4.ValueMember = "ObjectId";
            this.comboCategory4.DataSource = (await ParseTables.GetCategory4(contentType.ObjectId)).Select(c => new Category
            {
                ObjectId = c.ObjectId,
                Value = c.Get<string>("value")
            }).ToList();

            this.comboStatus.DisplayMember = "Value";
            this.comboStatus.ValueMember = "ObjectId";
            this.comboStatus.DataSource = (await ParseTables.GetStatuses()).Select(c => new Category
            {
                ObjectId = c.ObjectId,
                Value = c.Get<string>("status_" + MyMentor.Properties.Settings.Default.CultureInfo.Replace("-","_"))
            }).ToList();

            this.comboClipType.DisplayMember = "Value";
            this.comboClipType.ValueMember = "ObjectId";
            this.comboClipType.DataSource = (await ParseTables.GetTypes()).Select(c => new Category
            {
                ObjectId = c.ObjectId,
                Value = c.Get<string>("value")
            }).ToList();

            ParseObject labels = await ParseTables.GetCategoryLabels(contentType.ObjectId);

            lblCategory1.Text = labels.Get<string>("category1");
            lblCategory2.Text = labels.Get<string>("category2");
            lblCategory3.Text = labels.Get<string>("category3");
            lblCategory4.Text = labels.Get<string>("category4");

            SetClipProperties();
            RegenerateClipName();

            m_loadingParse = false;

            //test recorder
            if (MyMentor.Properties.Settings.Default.TestSound)
            {
                FormTestSound frmTest = new FormTestSound();

                frmTest.ShowDialog();
            }

            //check if admin - show extra stuff
            var role = await ParseRole.Query.GetAsync("Au3zBr8rLy");
            var relation = await role.GetRelation<ParseUser>("users").Query.FindAsync();
            var user = relation.FirstOrDefault(usr => usr.ObjectId.Equals( ParseUser.CurrentUser.ObjectId ));
            if (user != null)
            {
                m_admin = true;

                comboClipType.Visible = true;
                lblClipType.Visible = true;
            }
        }

        public string[] GetCategoriesLabels()
        {
            return new string[] { lblCategory1.Text, lblCategory2.Text, lblCategory3.Text, lblCategory4.Text };
        }

        public IEnumerable<Category> GetCategory1()
        {
            return this.comboCategory1.DataSource as IEnumerable<Category>;
        }

        public IEnumerable<Category> GetCategory3()
        {
            return this.comboCategory3.DataSource as IEnumerable<Category>;
        }

        public IEnumerable<Category> GetCategory4()
        {
            return this.comboCategory4.DataSource as IEnumerable<Category>;
        }

        private void RegenerateClipName()
        {
            RegenerateClipName(false);
        }

        private void RegenerateClipName(bool isDirty)
        {
            if (m_loadingParse)
            {
                return;
            }

            Debug.WriteLine(string.Format("ClipPattern:{0}", m_clipTitlePattern));

            var clipName = m_clipTitlePattern.Replace("[category1]", comboCategory1.Text ?? string.Empty)
                .Replace("[category2]", comboCategory2.Text ?? string.Empty)
                .Replace("[category3]", comboCategory3.Text ?? string.Empty)
                .Replace("[category4]", comboCategory4.Text ?? string.Empty)
                .Replace("[description]", Clip.Current.Description ?? string.Empty)
                .Replace("[firstName]", ParseTables.CurrentUser.Get<string>("firstName") ?? string.Empty)
                .Replace("[lastName]", ParseTables.CurrentUser.Get<string>("lastName") ?? string.Empty)
                .Replace("[cityOfResidence]", ParseTables.CurrentUser.Get<string>("cityOfResidence") ?? string.Empty);

            this.tbClipName.Text = clipName;
            Clip.Current.Name = clipName;

            if (isDirty && !m_whileLoadingClip)
            {
                Clip.Current.IsDirty = true;
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
            m_whileLoadingClip = true;

            if (string.IsNullOrEmpty(file))
            {
                file = GetFileDialog();
            }

            if (string.IsNullOrEmpty(file))
            {
                return;
            }

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

            //set properties
            richTextBox1.Rtf = Clip.Current.RtfText;

            SetClipProperties();
            RegenerateClipName();

            tabControl1.SelectedIndex = 0;
            mnuFile_SaveAs.Enabled = true;

            Clip.Current.Devide();

            if (File.Exists(Path.ChangeExtension(file, ".mp3")))
            {
                enumErrorCodes error = audioSoundEditor1.LoadSound(Path.ChangeExtension(file, ".mp3"));

                if (error != enumErrorCodes.ERR_NOERROR)
                {
                    MessageBox.Show(string.Format("נסיון טעינה של קובץ המוזיקה נכשל בגלל הסיבה :{0}\n\nאנא נסה שוב מאוחר יותר", error.ToString() ), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                }
            }

            Clip.Current.IsDirty = false;
            Clip.Current.IsNew = false;
            m_whileLoadingClip = false;

        }

        private async void SetClipProperties()
        {
            tbClipDescription.Text = Clip.Current.Description;
            comboCategory1.SelectedValue = Clip.Current.Category1 ?? string.Empty;

            comboCategory3.SelectedValue = Clip.Current.Category3 ?? string.Empty;
            comboCategory4.SelectedValue = Clip.Current.Category4 ?? string.Empty;
            comboClipType.SelectedValue = Clip.Current.ClipType ?? "piL85bMGtR";
            tbKeywords.Text = Clip.Current.Keywords;
            comboStatus.SelectedValue = Clip.Current.Status ?? string.Empty;
            RegenerateDatesBox();

            if (comboCategory1.SelectedValue != null)
            {
                this.comboCategory2.DisplayMember = "Value";
                this.comboCategory2.ValueMember = "ObjectId";
                this.comboCategory2.DataSource = (await ParseTables.GetCategory2((string)comboCategory1.SelectedValue)).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    Value = c.Get<string>("value")
                }).ToList(); ;

                if (Clip.Current.Category2 != null)
                {
                    comboCategory2.SelectedValue = Clip.Current.Category2 ?? string.Empty;
                }
            }
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

        private short m_guidLineUniqueId = -1;
        private short m_endLineUniqueId = -1;

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //copy text and insert start anchor
            richTextBox3.Clear();
            richTextBox3.Rtf = richTextBox1.Rtf;

            //add end anchor
            m_skipSelectionChange = true;
            richTextBox3.AppendText(END_PAUSE_SECTION_ANCHOR);

            //add start anchor
            richTextBox3.SelectionStart = 0;
            richTextBox3.SelectionLength = 0;
            richTextBox3.SelectedText = START_PAUSE_SECTION_ANCHOR;
            m_skipSelectionChange = false;

            richTextBox3.SelectionStart = 0;
            richTextBox3.SelectionLength = 0;

            m_selectedScheduledWord = null;
            m_selectedAnchor = false;

            if (tabControl1.SelectedIndex == 0
                || tabControl1.SelectedIndex == 3)
            {
                tableLayoutPanel4.RowStyles[1].Height = 0;
            }
            else
            {
                tableLayoutPanel4.RowStyles[1].Height = 160;
            }

            //check if we have to refresh display
            if (tabControl1.SelectedIndex == 1
                && m_waveFormTabIndex != 1)
            {
                m_guidLineUniqueId = -1;
                m_waveFormTabIndex = 1;
                audioSoundEditor1.DisplayWaveformAnalyzer.MouseSelectionEnable(true);

                //remove graphics
                foreach (Word word in Clip.Current.Chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(sc => sc.Sections)
                   .SelectMany(w => w.Words))
                {
                    if (word.GraphicItemUnique > -1)
                    {
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(word.GraphicItemUnique);
                    }
                }

                //remove guid line & end line
                audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(m_endLineUniqueId);
                audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(m_guidLineUniqueId);
                audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();
            }
            //scheduler step
            else if (tabControl1.SelectedIndex == 2)
            {
                Clip.Current.Devide();

                richTextBox3.SelectionStart = 0;

                if (m_waveFormTabIndex != 2)
                {
                    m_waveFormTabIndex = 2;

                    m_guidLineUniqueId = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineAdd("MyLine", "", 0,
                        new WANALYZER_VERTICAL_LINE { color = Color.Yellow, nWidth = 1, nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT, nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DASH_DOT, nHighCap = enumLineCaps.LINE_CAP_SQUARE, nLowCap = enumLineCaps.LINE_CAP_SQUARE, nTranspFactor = 0 });

                    if (Clip.Current.Chapter.Paragraphs != null)
                    {
                        // Add lines for all anchors
                        foreach (Word word in Clip.Current.Chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).SelectMany(sc => sc.Words))
                        {
                            if (word.StartTime.TotalMilliseconds > 0)
                            {
                                word.GraphicItemUnique = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineAdd("WS|" + word.Index, word.Content,
                                    (int)word.StartTime.TotalMilliseconds == 0 ? 50 : (int)word.StartTime.TotalMilliseconds,
                                    new WANALYZER_VERTICAL_LINE
                                    {
                                        color = Color.White,
                                        nWidth = 5,
                                        nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT,
                                        nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DOT,
                                        nHighCap = enumLineCaps.LINE_CAP_SQUARE,
                                        nLowCap = enumLineCaps.LINE_CAP_SQUARE,
                                        nTranspFactor = 50
                                    });
                            }
                        }
                    }

                    if (Clip.Current.Chapter.LastWord != null && Clip.Current.Chapter.LastWord.EndTime > TimeSpan.Zero)
                    {
                        m_endLineUniqueId = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineAdd("EndLine", "",
                            (int)(Clip.Current.Chapter.LastWord.StartTime.TotalMilliseconds + Clip.Current.Chapter.LastWord.Duration.TotalMilliseconds),
                                                new WANALYZER_VERTICAL_LINE
                                                {
                                                    color = Color.PeachPuff,
                                                    nWidth = 5,
                                                    nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT,
                                                    nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DOT,
                                                    nHighCap = enumLineCaps.LINE_CAP_SQUARE,
                                                    nLowCap = enumLineCaps.LINE_CAP_SQUARE,
                                                    nTranspFactor = 50
                                                });
                        ;
                    }

                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(false, -1, -1);
                    audioSoundEditor1.DisplayWaveformAnalyzer.MouseSelectionEnable(false);
                    audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();

                }

            }

            PaintGraphics();
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
            Clip.Current.Name = "ללא שם";
            Clip.Current.Version = "1.00";
            Clip.Current.Status = "bXhtWFZznn";
            Clip.Current.ID = Guid.NewGuid();
            Clip.Current.IsNew = true;
            Clip.Current.RightAlignment = true;
            Clip.Current.ClipType = "piL85bMGtR";

            this.Text = "MyMentor - " + Clip.Current.Name;

            richTextBox1.Rtf = null;
            Clip.Current.IsDirty = false;

            audioSoundRecorder1.RecordedSound.FreeMemory();
            audioSoundEditor1.CloseSound();
            TimerReload.Enabled = true;

            Clip.Current.Text = richTextBox1.Text;
            Clip.Current.Devide();

            SetClipProperties();
            RegenerateClipName();
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
            if (m_skipSelectionChange || Clip.Current.Chapter == null)
            {
                return;
            }

            int selectionIndex = Math.Max(0, richTextBox3.SelectionStart - START_PAUSE_SECTION_ANCHOR.Length);

            if (selectionIndex == 0 && m_selectedScheduledWord != null) // start anchor
            {
                //set current word start time when clicking word offline
                timePickerCurrentWord.Value = TimeSpan.Zero;
                timePickerCurrentWord.Enabled = false;

                m_skipSelectionChange = true;
                richTextBox3.SelectionStart = 0;
                richTextBox3.SelectionLength = START_PAUSE_SECTION_ANCHOR.Length;
                m_skipSelectionChange = false;

                m_selectedAnchor = false;
                m_selectedStartAnchor = true;
                m_selectedEndAnchor = false;

                //take first word
                m_selectedScheduledWord = Clip.Current.Chapter.FirstWord;
                LabelCurrentWordDuration.Text = audioSoundEditor1.FromMsToFormattedTime((int)m_selectedScheduledWord.StartTime.TotalMilliseconds, true, true);

                //move line to start
                audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, (int)timePickerCurrentWord.Value.TotalMilliseconds, (int)timePickerCurrentWord.Value.TotalMilliseconds);

            }
            //end anchor
            else if (selectionIndex > richTextBox1.TextLength && Clip.Current.Chapter.Paragraphs != null)
            {
                m_skipSelectionChange = true;
                richTextBox3.SelectionStart = richTextBox3.TextLength - END_PAUSE_SECTION_ANCHOR.Length;
                richTextBox3.SelectionLength = END_PAUSE_SECTION_ANCHOR.Length;
                m_skipSelectionChange = false;

                m_selectedAnchor = false;
                m_selectedStartAnchor = false;
                m_selectedEndAnchor = true;

                //set current word start time when clicking word offline
                timePickerCurrentWord.Value = Clip.Current.Chapter.LastWord.StartTime + Clip.Current.Chapter.LastWord.Duration;
                timePickerCurrentWord.Enabled = true;

                LabelCurrentWordDuration.Text = audioSoundEditor1.FromMsToFormattedTime(
                    (int)audioSoundEditor1.GetSoundDuration() - (int)timePickerCurrentWord.Value.TotalMilliseconds, //rest of the file
                    true, true);

                //set as null
                m_selectedScheduledWord = null;

                //move line
                audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, 
                    (int)timePickerCurrentWord.Value.TotalMilliseconds,
                    (int)timePickerCurrentWord.Value.TotalMilliseconds);
            }
            else if (Clip.Current.Chapter.Paragraphs != null)
            {
                //clicked on any other parts in the clip

                m_selectedStartAnchor = false;
                m_selectedEndAnchor = false;

                var savePreviousWord = m_selectedScheduledWord;

                //check for word selection
                m_selectedScheduledWord = Clip.Current.Chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(sc => sc.Sections)
                    .SelectMany(w => w.Words).Where(ww => ww.RealCharIndex <= selectionIndex).LastOrDefault();

                if (m_selectedScheduledWord != null)
                {
                    m_skipSelectionChange = true;

                    //check for anchor
                    if (m_selectedScheduledWord.RealCharIndex + m_selectedScheduledWord.Length < selectionIndex)
                    {
                        richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + m_selectedScheduledWord.Length + START_PAUSE_SECTION_ANCHOR.Length;
                        richTextBox3.SelectionLength = 3;

                        var interval = new TimeSpan(0, 0, (int)numericUpDownInterval.Value);

                        //set current word start time when clicking word offline
                        if ((m_selectedScheduledWord.StartTime + m_selectedScheduledWord.Duration).TotalSeconds >= interval.TotalSeconds)
                        {
                            timePickerCurrentWord.Value = m_selectedScheduledWord.StartTime + m_selectedScheduledWord.Duration - interval;
                            timePickerCurrentWord.Enabled = false;
                            LabelCurrentWordDuration.Text = "00:00:00.000";
                        }

                        //move line
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, (int)timePickerCurrentWord.Value.TotalMilliseconds, (int)timePickerCurrentWord.Value.TotalMilliseconds);

                        m_selectedAnchor = true;

                        buttonScheduleAnchor.Enabled = true;
                    }
                    else
                    {
                        //select the word
                        richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length;
                        richTextBox3.SelectionLength = m_selectedScheduledWord.Length;

                        //in case set start time during scheduling
                        if (n_hammerLastTimePressed != TimeSpan.Zero && audioSoundEditor1.GetPlaybackStatus() == enumPlaybackStatus.PLAYBACK_PLAYING)
                        {
                            m_selectedScheduledWord.StartTime = n_hammerLastTimePressed;

                            Debug.WriteLine(string.Format("Setting '{0}' with start time : {1}", m_selectedScheduledWord.Content, m_selectedScheduledWord.StartTimeText));

                            n_hammerLastTimePressed = TimeSpan.Zero;
                        }
                        else if (audioSoundEditor1.GetPlaybackStatus() != enumPlaybackStatus.PLAYBACK_PLAYING)
                        {
                            //set current word start time when clicking word offline
                            timePickerCurrentWord.Value = m_selectedScheduledWord.StartTime;
                            LabelCurrentWordDuration.Text = audioSoundEditor1.FromMsToFormattedTime((int)m_selectedScheduledWord.Duration.TotalMilliseconds, true, true);
                            timePickerCurrentWord.Enabled = true;
                        }

                        //move guid line
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds);
                        m_selectedAnchor = false;
                        buttonScheduleAnchor.Enabled = false;
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

        private void buttonStartSchedulingPlayback_Click(object sender, EventArgs e)
        {
            Int32 nDurationInMs = audioSoundEditor1.GetSoundDuration();
            if (nDurationInMs == 0)
            {
                return;
            }

            if (buttonStartSchedulingPlayback.Text == "התחל" || buttonStartSchedulingPlayback.Text == "המשך")
            {
                buttonHammer.Enabled = true;
                audioSoundEditor1.PlaySoundRange((int)TimeSpan.Parse(LabelCurrentSchedulingTimer.Text).TotalMilliseconds, -1);
                buttonStartSchedulingPlayback.Text = "עצור";

                buttonRestartScheduling.Enabled = false;
                timerUpdateTimePickerSpinner.Enabled = true;
                timePickerCurrentWord.Enabled = false;
            }
            else if (buttonStartSchedulingPlayback.Text == "עצור")
            {
                buttonHammer.Enabled = false;
                buttonRestartScheduling.Enabled = true;
                timePickerCurrentWord.Enabled = true;
                timerUpdateTimePickerSpinner.Enabled = false;
                audioSoundEditor1.StopSound();
                buttonStartSchedulingPlayback.Text = "המשך";
            }

            //djLineTimer.Enabled = true;
        }

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

        private void djLineTimer_Tick(object sender, EventArgs e)
        {
            //double position = 0;
            //audioDjStudio1.SoundPositionGet(0, ref position, false);
            //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, (int)position, (int)position);
        }

        private void audioDjStudio1_SoundDone(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            //djLineTimer.Enabled = false;
        }

        /// <summary>
        /// Push the fucking hammer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHammer_Click(object sender, EventArgs e)
        {
            //
            // PLAYBACK
            if (audioSoundEditor1.GetPlaybackStatus() == enumPlaybackStatus.PLAYBACK_PLAYING)
            {
                n_hammerLastTimePressed = TimeSpan.Parse(LabelCurrentSchedulingTimer.Text);// timePickerCurrentWord.Value;

                //it resets when it pass the selected event
                var saveIt = n_hammerLastTimePressed;

                //in case current on silent part
                if (m_selectedStartAnchor)
                {
                    //goto to first word in chapter
                    richTextBox3.SelectionStart = START_PAUSE_SECTION_ANCHOR.Length + 1;
                }
                else if (m_selectedEndAnchor)
                {
                    // do nothing for now
                }
                else
                {
                    //goto next word
                    richTextBox3.SelectionStart = richTextBox3.SelectionStart + richTextBox3.SelectionLength + 3;
                }

                //in case line exists
                if (m_selectedScheduledWord != null && m_selectedScheduledWord.GraphicItemUnique > 0)
                {
                    //set new position
                    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_selectedScheduledWord.GraphicItemUnique,
                        (int)saveIt.TotalMilliseconds == 0 ? 50 : (int)saveIt.TotalMilliseconds, (int)saveIt.TotalMilliseconds == 0 ? 50 : (int)saveIt.TotalMilliseconds);
                }
                else if (m_selectedScheduledWord == null && m_selectedEndAnchor)
                {
                    Clip.Current.Chapter.LastWord.Duration = saveIt - Clip.Current.Chapter.LastWord.StartTime;

                    if (m_endLineUniqueId >= 0)
                    {
                        //set new position
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_endLineUniqueId,
                            (int)saveIt.TotalMilliseconds, (int)saveIt.TotalMilliseconds);
                    }
                    else
                    {
                        //create new anchor line
                        m_endLineUniqueId = audioSoundEditor1.DisplayWaveformAnalyzer
                            .GraphicItemVerticalLineAdd("EndLine", "",
                            (int)saveIt.TotalMilliseconds,
                            new WANALYZER_VERTICAL_LINE
                            {
                                color = Color.PeachPuff,
                                nWidth = 5,
                                nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT,
                                nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DOT,
                                nHighCap = enumLineCaps.LINE_CAP_SQUARE,
                                nLowCap = enumLineCaps.LINE_CAP_SQUARE,
                                nTranspFactor = 50
                            });
                    }
                    buttonHammer.Enabled = false;
                }
                else
                {
                    //create new anchor line
                    m_selectedScheduledWord.GraphicItemUnique =
                        audioSoundEditor1.DisplayWaveformAnalyzer
                        .GraphicItemVerticalLineAdd("WS|" + m_selectedScheduledWord.Index, m_selectedScheduledWord.Content,

                        (int)saveIt.TotalMilliseconds == 0 ? 50 : (int)saveIt.TotalMilliseconds,
                        new WANALYZER_VERTICAL_LINE
                        {
                            color = Color.White,
                            nWidth = 5,
                            nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT,
                            nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DOT,
                            nHighCap = enumLineCaps.LINE_CAP_SQUARE,
                            nLowCap = enumLineCaps.LINE_CAP_SQUARE,
                            nTranspFactor = 50
                        });
                }


            }
        }

        private void timePickerSpinner1_ValueChanged(object sender, EventArgs e)
        {
            if (!m_selectedAnchor && !m_selectedStartAnchor
                && audioSoundEditor1.GetPlaybackStatus() != enumPlaybackStatus.PLAYBACK_PLAYING)
            {
                //end anchor
                if (m_selectedEndAnchor && Clip.Current.Chapter.LastWord != null)
                {
                    if (timePickerCurrentWord.Value < Clip.Current.Chapter.LastWord.StartTime.Add(new TimeSpan(0, 0, 0, 0, 500)))
                    {
                        timePickerCurrentWord.Value = Clip.Current.Chapter.LastWord.StartTime.Add(new TimeSpan(0, 0, 0, 0, 500));
                        return;
                    }

                    //set duration to last word
                    Clip.Current.Chapter.LastWord.Duration = timePickerCurrentWord.Value - Clip.Current.Chapter.LastWord.StartTime;

                    //move relevet line
                    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_endLineUniqueId,
                        (int)timePickerCurrentWord.Value.TotalMilliseconds, (int)timePickerCurrentWord.Value.TotalMilliseconds);
                }
                else
                {
                    if (m_selectedScheduledWord != null)
                    {
                        if (m_selectedScheduledWord.PreviousWord != null &&
                            timePickerCurrentWord.Value <= m_selectedScheduledWord.PreviousWord.StartTime + new TimeSpan(0, 0, 0, 0, 500))
                        {
                            timePickerCurrentWord.Value = m_selectedScheduledWord.PreviousWord.StartTime + new TimeSpan(0, 0, 0, 0, 500);
                            return;
                        }

                        if (m_selectedScheduledWord.NextWord != null &&
                            timePickerCurrentWord.Value >= m_selectedScheduledWord.NextWord.StartTime + new TimeSpan(0, 0, 0, 0, 500))
                        {
                            timePickerCurrentWord.Value = m_selectedScheduledWord.NextWord.StartTime + new TimeSpan(0, 0, 0, 0, 500);
                            return;
                        }

                        //in all other cases we talk about start time
                        m_selectedScheduledWord.StartTime = timePickerCurrentWord.Value;

                        //move relevet line
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_selectedScheduledWord.GraphicItemUnique,
                            (int)timePickerCurrentWord.Value.TotalMilliseconds, (int)timePickerCurrentWord.Value.TotalMilliseconds);

                        //if its the last word move the red line also
                        if (m_selectedScheduledWord.NextWord == null)
                        {
                            //move relevet line
                            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_endLineUniqueId,
                                (int)(timePickerCurrentWord.Value.TotalMilliseconds + m_selectedScheduledWord.Duration.TotalMilliseconds) ,
                                 (int)(timePickerCurrentWord.Value.TotalMilliseconds + m_selectedScheduledWord.Duration.TotalMilliseconds));
                        }

                    }
                }
            }
        }


        private void audioSoundEditor1_WaveAnalyzerLineMoving(object sender, WaveAnalyzerLineMovingEventArgs e)
        {
            var name = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemNameGet(e.nUniqueID);

            if (e.bMovedByCode)
                return;

            if (name == "MyLine")
            {
                //timePickerCurrentWord.Value = new TimeSpan(0, 0, 0, 0, e.nPosInMs);
            }
            else if (name == "EndLine")
            {
                var pos = Clip.Current.Chapter.LastWord.StartTime.Add(new TimeSpan(0, 0, 0, 0, 500));

                if (Clip.Current.Chapter.LastWord != null &&
                        new TimeSpan(0, 0, 0, 0, e.nPosInMs) <= pos)
                {
                    Clip.Current.Chapter.LastWord.Duration = pos - Clip.Current.Chapter.LastWord.StartTime;

                    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(e.nUniqueID,
                        (int)pos.TotalMilliseconds, (int)pos.TotalMilliseconds);
                }
                else
                {
                    Clip.Current.Chapter.LastWord.Duration = new TimeSpan(0, 0, 0, 0, e.nPosInMs) - Clip.Current.Chapter.LastWord.StartTime;
                }

            }
            else if (name.StartsWith("WS|"))
            {
                //other anchor -- get its name WS|12
                int index = int.Parse(audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemNameGet(e.nUniqueID).Split('|')[1]);

                var word = Clip.Current.Chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).SelectMany(sc => sc.Words).Where(w => w.Index == index).FirstOrDefault();

                //if the user tried to move the line illegally
                if (word.NextWord != null &&
                    new TimeSpan(0, 0, 0, 0, e.nPosInMs) >= word.NextWord.StartTime.Subtract(new TimeSpan(0, 0, 0, 0, 500)))
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(e.nUniqueID, (int)word.NextWord.StartTime.TotalMilliseconds - 500, (int)word.NextWord.StartTime.TotalMilliseconds - 500);
                    word.StartTime = new TimeSpan(0, 0, 0, 0, (int)word.NextWord.StartTime.TotalMilliseconds - 500);
                }
                else if (word.PreviousWord != null &&
                    new TimeSpan(0, 0, 0, 0, e.nPosInMs) <= word.PreviousWord.StartTime.Add(new TimeSpan(0, 0, 0, 0, 500)))
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(e.nUniqueID, (int)word.PreviousWord.StartTime.TotalMilliseconds + 500, (int)word.PreviousWord.StartTime.TotalMilliseconds + 500);
                    word.StartTime = new TimeSpan(0, 0, 0, 0, (int)word.PreviousWord.StartTime.TotalMilliseconds + 500);
                }
                else
                {
                    //set word new start time
                    word.StartTime = new TimeSpan(0, 0, 0, 0, e.nPosInMs);

                    //if this is the last word move its duration also
                    if (word.NextWord == null)
                    {
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_endLineUniqueId,
                            (int)(word.StartTime + word.Duration).TotalMilliseconds,
                             (int)(word.StartTime + word.Duration).TotalMilliseconds);
                    }
                }
            }
        }

        private void timerCheckPlaybackDuringScheduling_Tick(object sender, EventArgs e)
        {
            if (audioSoundEditor1.GetPlaybackStatus() != enumPlaybackStatus.PLAYBACK_PLAYING) // in case anchor fixing
            {
                return;
            }

            int mm = audioSoundEditor1.GetPlaybackPosition();
            var position = new TimeSpan(0, 0, 0, 0, Math.Max(mm, 0)); ;

            LabelCurrentSchedulingTimer.Text = audioSoundEditor1.FromMsToFormattedTime((long)mm, true, true);// GetFormattedTime(e.nBeginPosInMs, true, true);

            //during playing check if the current position moved over to the next word
            if (Clip.Current.Chapter.Paragraphs != null && !m_selectedAnchor)
            {
                //in case start part
                if (m_selectedStartAnchor)
                {
                    if (position >= Clip.Current.Chapter.FirstWord.StartTime && Clip.Current.Chapter.FirstWord.StartTime > TimeSpan.Zero)
                    {
                        //set time picker
                        timePickerCurrentWord.Value = position;
                        richTextBox3.SelectionStart = Clip.Current.Chapter.FirstWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length + 1;
                    }
                }
                else if (m_selectedEndAnchor)
                {
                    //Do nothing for now
                }
                else
                {
                    //get next word in case timer moving and pass next start time word
                    //catch all the words until the last one
                    if (m_selectedScheduledWord != null &&
                        m_selectedScheduledWord.NextWord != null
                        && position >= m_selectedScheduledWord.NextWord.StartTime
                        && m_selectedScheduledWord.NextWord.NextWord != null
                        && position < m_selectedScheduledWord.NextWord.NextWord.StartTime)
                    {
                        //set time picker
                        timePickerCurrentWord.Value = m_selectedScheduledWord.NextWord.StartTime;// position;

                        richTextBox3.SelectionStart = m_selectedScheduledWord.NextWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length;
                    }
                    // catch the last word as the last one
                    else if (m_selectedScheduledWord != null &&
                        m_selectedScheduledWord.NextWord != null
                        && position >= m_selectedScheduledWord.NextWord.StartTime
                        && position < m_selectedScheduledWord.NextWord.StartTime + m_selectedScheduledWord.NextWord.Duration
                        && m_selectedScheduledWord.NextWord.NextWord == null
                        )
                    {
                        //set time picker
                        timePickerCurrentWord.Value = m_selectedScheduledWord.NextWord.StartTime;// position;

                        richTextBox3.SelectionStart = m_selectedScheduledWord.NextWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length;
                    }
                    // catch when passed the red line end border 
                    else if (m_selectedScheduledWord != null &&
                        m_selectedScheduledWord.NextWord == null &&
                        m_selectedScheduledWord.Duration > TimeSpan.Zero
                        && position >= m_selectedScheduledWord.StartTime + m_selectedScheduledWord.Duration
                        )
                    {
                        //set time picker
                        timePickerCurrentWord.Value = m_selectedScheduledWord.StartTime + m_selectedScheduledWord.Duration;

                        richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length + m_selectedScheduledWord.Length + 1;
                    }
                }
            }
        }

        private void mnuRemoveSchedule_Click(object sender, EventArgs e)
        {
            if (Clip.Current.Chapter.Paragraphs != null)
            {
                if (MessageBox.Show("האם אתה בטוח להסיר את התזמונים לשיעור זה ? ", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    Clip.Current.IsDirty = true;

                    var words = Clip.Current.Chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).SelectMany(w => w.Words);

                    foreach (var word in words)
                    {
                        word.StartTime = TimeSpan.Zero;
                        word.Duration = TimeSpan.Zero;
                    }

                    //remove graphics
                    foreach (Word word in words)
                    {
                        if (word.GraphicItemUnique > -1)
                        {
                            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(word.GraphicItemUnique);
                            word.GraphicItemUnique = -1;
                        }
                    }

                    //remove guid line
                    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(m_endLineUniqueId);
                    audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, 0, 0);
            LabelCurrentSchedulingTimer.Text = audioSoundEditor1.FromMsToFormattedTime(0, true, true);// GetFormattedTime(e.nBeginPosInMs, true, true);
            //timePickerCurrentWord.Value = TimeSpan.Zero;
            richTextBox3.SelectionStart = 4;
            buttonStartSchedulingPlayback.Text = "התחל";
            buttonHammer.Enabled = true;
        }

        private void buttonScheduleAnchor_Click(object sender, EventArgs e)
        {
            Int32 nDurationInMs = audioSoundEditor1.GetSoundDuration();
            if (nDurationInMs == 0)
            {
                return;
            }

            buttonScheduleAnchor.Enabled = false;

            var interval = new TimeSpan(0, 0, (int)(numericUpDownInterval.Value * 2));

            timerUpdateTimePickerSpinner.Enabled = true;

            m_rem_anchorFixRecording = timePickerCurrentWord.Value;

            //play the range
            audioSoundEditor1.PlaySoundRange((int)timePickerCurrentWord.Value.TotalMilliseconds,
                    (int)timePickerCurrentWord.Value.TotalMilliseconds + (int)interval.TotalMilliseconds);

        }

        private void audioSoundEditor1_WaveAnalyzerGraphicItemClick(object sender, WaveAnalyzerGraphItemClickEventArgs e)
        {
            var name = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemNameGet(e.nUniqueID);

            if (name.StartsWith("WS|"))
            {
                //other anchor -- get its name WS|12
                int index = int.Parse(name.Split('|')[1]);

                var word = Clip.Current.Chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).SelectMany(sc => sc.Words).Where(w => w.Index == index).FirstOrDefault();

                if (e.nButton == enumMouseButtons.MOUSE_BTN_RIGHT)
                {
                    richTextBox3.SelectionStart = word.RealCharIndex + 2;
                }
                else
                {
                    richTextBox3.SelectionStart = word.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length + 1;
                }
            }
            else if (name == "EndLine")
            {
                if (e.nButton == enumMouseButtons.MOUSE_BTN_LEFT)
                {
                    richTextBox3.SelectionStart = Clip.Current.Chapter.LastWord.RealCharIndex + Clip.Current.Chapter.LastWord.Length + START_PAUSE_SECTION_ANCHOR.Length + 1;
                }
            }
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 2 &&
                audioSoundEditor1.GetSoundDuration() <= 0)
            {
                // ask the user if he wants to go on
                MessageBox.Show("לא קיימת הקלטה לביצוע תזמון", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                e.Cancel = true;
            }
            //else if (e.TabPageIndex == 3 &&
            //    audioSoundEditor1.GetSoundDuration() <= 0)
            //{
            //    // ask the user if he wants to go on
            //    MessageBox.Show("יש לבצע הקלטה ותזמון לשיעור לפני פרסומו", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            //    e.Cancel = true;
            //}
        }

        private void buttonStartRecOverwrite_Click(object sender, EventArgs e)
        {
        }

        private void mnuLoginDifferentUser_Click(object sender, EventArgs e)
        {
            LoginForm form = new LoginForm();
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (ParseUser.CurrentUser != null)
                {
                    lblLoginUser.Text = "הנך מחובר כ-" + ParseUser.CurrentUser.Username;
                }
                else
                {
                    Application.Exit();
                }
            }

        }

        private void timerRecordIcon_Tick(object sender, EventArgs e)
        {
            if (b_recordIconRed)
            {
                pictureBox1.Image = imageList1.Images[1];
                pictureBox1.Refresh();
            }
            else
            {
                pictureBox1.Image = imageList1.Images[0];
                pictureBox1.Refresh();
            }
            b_recordIconRed = !b_recordIconRed;
        }

        private void richTextBox3_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void richTextBox3_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void mnuAnchors_RemoveAll_Click(object sender, EventArgs e)
        {
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Paragraph);
            RemoveAnchor(AnchorType.Sentence);
            RemoveAnchor(AnchorType.Section);
            RemoveAnchor(AnchorType.Word);
            richTextBox1.Rtf = richTextBox2.Rtf;

        }

        private void mnuAnchors_RemoveSentenses_Click(object sender, EventArgs e)
        {
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Sentence);
            richTextBox1.Rtf = richTextBox2.Rtf;
        }


        private void AddAnchor(AnchorType type, int index)
        {
            string anchor = "";

            switch (type)
            {
                case AnchorType.Paragraph:
                    anchor = Clip.PAR_SIGN;
                    break;
                case AnchorType.Sentence:
                    anchor = Clip.SEN_SIGN;
                    break;
                case AnchorType.Section:
                    anchor = Clip.SEC_SIGN;
                    break;
                case AnchorType.Word:
                    anchor = Clip.WRD_SIGN;
                    break;
            }

            richTextBox2.SelectionStart = index;
            richTextBox2.SelectionLength = 0;
            richTextBox2.SelectedText = anchor;

        }

        private void RemoveAnchor(AnchorType type)
        {
            string anchor = "";

            switch (type)
            {
                case AnchorType.Paragraph:
                    anchor = Clip.PAR_SIGN;
                    break;
                case AnchorType.Sentence:
                    anchor = Clip.SEN_SIGN;
                    break;
                case AnchorType.Section:
                    anchor = Clip.SEC_SIGN;
                    break;
                case AnchorType.Word:
                    anchor = Clip.WRD_SIGN;
                    break;
            }

            int index = richTextBox2.Find(anchor, 0, RichTextBoxFinds.None);

            while (index >= 0)
            {
                richTextBox2.SelectionStart = index;
                richTextBox2.SelectionLength = 3;
                richTextBox2.SelectedText = "";

                index = richTextBox2.Find(anchor, index + 3, RichTextBoxFinds.None);
            }
        }

        private void mnuAnchors_RemoveParagraphs_Click(object sender, EventArgs e)
        {
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Paragraph);
            richTextBox1.Rtf = richTextBox2.Rtf;
        }

        private void mnuAnchors_RemoveSections_Click(object sender, EventArgs e)
        {
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Section);
            richTextBox1.Rtf = richTextBox2.Rtf;
        }

        private void mnuAnchors_RemoveWords_Click(object sender, EventArgs e)
        {
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Word);
            richTextBox1.Rtf = richTextBox2.Rtf;
        }

        private void buttonAutoDevide_Click(object sender, EventArgs e)
        {
            var parDelimiters = comboBoxAutoDevidePar.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim()).ToList<string>();
            var senDelimiters = comboBoxAutoDevideSen.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim()).ToList<string>();

            if (parDelimiters.Count() == 0 && senDelimiters.Count() == 0)
            {
                MessageBox.Show("יש לבחור לפחות אפשרות עוגן פסקה אחת או אפשרות עוגן משפט אחת", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            //if (senDelimiters.Count() == 0)
            //{
            //    MessageBox.Show("יש לבחור לפחות אפשרות עוגן משפט אחת", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            //    return;
            //}

            if (parDelimiters.Exists(a => a == "ENTER") && senDelimiters.Exists(a => a == "ENTER"))
            {
                MessageBox.Show("בחירה לא חוקית", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            if (parDelimiters.Exists(a => a == "שני ENTER") && senDelimiters.Exists(a => a == "שני ENTER"))
            {
                MessageBox.Show("בחירה לא חוקית", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            richTextBox2.Rtf = richTextBox1.Rtf;

            RemoveAnchor(AnchorType.Paragraph);
            RemoveAnchor(AnchorType.Sentence);
            RemoveAnchor(AnchorType.Section);
            RemoveAnchor(AnchorType.Word);

            int index = 0;
            int enterKeys = 0;
            int sentenses = 0;
            int charactersFromLastAnchor = 0;

            while (index < richTextBox2.TextLength)
            {
                richTextBox2.SelectionStart = index;
                richTextBox2.SelectionLength = 1;

                index++;

                //enter
                if (richTextBox2.SelectedText.ToCharArray()[0] == (char)10)
                {
                    enterKeys++;

                    if (parDelimiters.Exists(a => a == "ENTER") && charactersFromLastAnchor > 0)
                    {
                        AddAnchor(AnchorType.Paragraph, index - 1);
                        charactersFromLastAnchor = 0;
                        index += 4;
                        continue;
                    }

                    if (parDelimiters.Exists(a => a == "שני ENTER") && enterKeys % 2 == 0 && charactersFromLastAnchor > 0)
                    {
                        AddAnchor(AnchorType.Paragraph, index - 1);
                        charactersFromLastAnchor = 0;
                        index += 4;
                        continue;
                    }

                    if (senDelimiters.Exists(a => a == "ENTER"))
                    {
                        sentenses += 1;

                        if (charactersFromLastAnchor > 0 && (sentenses == 2 && parDelimiters.Exists(a => a == "אחרי 2 משפטים")) ||
                            (sentenses == 3 && parDelimiters.Exists(a => a == "אחרי 3 משפטים")) ||
                            (sentenses == 4 && parDelimiters.Exists(a => a == "אחרי 4 משפטים")))
                        {
                            AddAnchor(AnchorType.Paragraph, index - 1);
                            charactersFromLastAnchor = 0;
                            index += 4;
                            sentenses = 0;
                            continue;
                        }

                        AddAnchor(AnchorType.Sentence, index - 1);
                        charactersFromLastAnchor = 0;
                        index += 4;
                        continue;
                    }

                    if (senDelimiters.Exists(a => a == "שני ENTER") && enterKeys % 2 == 0 && charactersFromLastAnchor > 0)
                    {
                        sentenses += 1;

                        if ((sentenses == 2 && parDelimiters.Exists(a => a == "אחרי 2 משפטים")) ||
                            (sentenses == 3 && parDelimiters.Exists(a => a == "אחרי 3 משפטים")) ||
                            (sentenses == 4 && parDelimiters.Exists(a => a == "אחרי 4 משפטים")))
                        {
                            AddAnchor(AnchorType.Paragraph, index - 1);
                            charactersFromLastAnchor = 0;
                            index += 4;
                            sentenses = 0;
                            continue;
                        }

                        AddAnchor(AnchorType.Sentence, index - 1);
                        charactersFromLastAnchor = 0;
                        index += 4;
                        continue;
                    }
                }

                if (richTextBox2.SelectedText == ":" && senDelimiters.Exists(a => a == "נקודותיים (:)") && charactersFromLastAnchor > 0)
                {
                    sentenses += 1;

                    if ((sentenses == 2 && parDelimiters.Exists(a => a == "אחרי 2 משפטים")) ||
                        (sentenses == 3 && parDelimiters.Exists(a => a == "אחרי 3 משפטים")) ||
                        (sentenses == 4 && parDelimiters.Exists(a => a == "אחרי 4 משפטים")))
                    {
                        AddAnchor(AnchorType.Paragraph, index);
                        charactersFromLastAnchor = 0;
                        index += 4;
                        sentenses = 0;
                        continue;
                    }

                    AddAnchor(AnchorType.Sentence, index);
                    charactersFromLastAnchor = 0;
                    index += 4;
                    continue;
                }

                if (richTextBox2.SelectedText == "." && senDelimiters.Exists(a => a == "נקודה (.)") && charactersFromLastAnchor > 0)
                {
                    sentenses += 1;

                    if ((sentenses == 2 && parDelimiters.Exists(a => a == "אחרי 2 משפטים")) ||
                        (sentenses == 3 && parDelimiters.Exists(a => a == "אחרי 3 משפטים")) ||
                        (sentenses == 4 && parDelimiters.Exists(a => a == "אחרי 4 משפטים")))
                    {
                        AddAnchor(AnchorType.Paragraph, index);
                        charactersFromLastAnchor = 0;
                        index += 4;
                        sentenses = 0;
                        continue;
                    }

                    AddAnchor(AnchorType.Sentence, index);
                    charactersFromLastAnchor = 0;
                    index += 4;
                    continue;
                }

                charactersFromLastAnchor++;

            }
            richTextBox1.Rtf = richTextBox2.Rtf;

        }

        private void mnuRemoveNikud_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = richTextBox1.Text.RemoveNikud();
        }

        private void mnuFile_Open_Click(object sender, EventArgs e)
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

        private void mnuFile_SaveAs_Click(object sender, EventArgs e)
        {
            Save(true);
        }

        private void mnuRemoveTeamim_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = richTextBox1.Text.RemoveTeamim();
        }

        private void buttonRecOverwritePlayback_Click(object sender, EventArgs e)
        {
            buttonRecOverwritePlayback.Enabled = false;
            timerRefreshLedDisplay.Enabled = true;

            timerPreStartFixPlayback.Enabled = true;
        }

        private void timerStartRecordingAfterPlayingBuffer_Tick(object sender, EventArgs e)
        {
            timerStartRecordingAfterPlayingBuffer.Enabled = false;

            if (m_bRecOverwriteMode)
            {
                // create a fresh new recording session
                audioSoundRecorder1.SetRecordingMode(AudioSoundRecorder.enumRecordingModes.REC_MODE_OVERWRITE);
                TimeSpan startSelectionTime = TimeSpan.Parse(LabelSelectionBegin.Text);

                audioSoundRecorder1.SetOverwritePos((int)startSelectionTime.TotalMilliseconds);

                // start recording in memory from system default input device and input channel
                audioSoundRecorder1.StartFromDirectSoundDevice(0, -1, "");

            }
        }

        private void audioSoundRecorder1_RecordingPaused(object sender, EventArgs e)
        {
            timerRecordIcon.Enabled = false;
        }

        private void audioSoundRecorder1_RecordingResumed(object sender, EventArgs e)
        {
            timerRecordIcon.Enabled = true;

        }

        private void audioSoundRecorder1_RecordingDuration(object sender, AudioSoundRecorder.RecordingDurationEventArgs e)
        {
            m_intRecordingDuration = e.nDuration;
        }

        private void timerRefreshLedDisplay_Tick(object sender, EventArgs e)
        {
            int val = int.Parse(sevenSegment1.Value);

            sevenSegment1.Value = (val - 1).ToString();

            if (val - 1 == 0)
            {
                timerRefreshLedDisplay.Enabled = false;
            }
        }

        private void numericUpDownBufferRecord_ValueChanged(object sender, EventArgs e)
        {
            sevenSegment1.Value = numericUpDownBufferRecord.Value.ToString();
        }

        private async void buttonPublish_Click(object sender, EventArgs e)
        {
            if (audioSoundEditor1.GetSoundDuration() <= 0)
            {
                // ask the user if he wants to go on
                MessageBox.Show("יש לבצע הקלטה ותזמון לשיעור לפני פרסומו", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }
            
            if (MessageBox.Show("האם אתה בטוח לפרסם שיעור זה ?", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            groupBox3.Enabled = false;
            pictureBox2.Visible = true;
            progressBar1.Visible = false;
            buttonPublish.Enabled = false;
            Application.DoEvents();

            SavePropertiesToClip();
            try
            {
                if (Clip.Current.SaveJson(Clip.Current.ExtractJson()) && Clip.Current.ExtractHtml())
                {
                    if (Clip.Current.Publish(audioSoundEditor1))
                    {
                        LabelStatus.Text = "מעלה...אנא המתן";

                        bool result = await Clip.Current.UploadAsync(new Progress<ParseUploadProgressEventArgs>(ev =>
                        {
                            progressBar1.Value = Convert.ToInt32(ev.Progress * 100);
                        }));

                        if (result)
                        {
                            if (Clip.Current.AutoIncrementVersion)
                            {
                                try
                                {
                                    Clip.Current.Version = Convert.ToString(Convert.ToDouble(Clip.Current.Version) + 0.01);
                                }
                                catch
                                {

                                }

                                //Clip.Current.Save();
                            }

                            LabelStatus.Text = "Status: Idle";
                            progressBar1.Value = 0;
                            buttonPublish.Enabled = true;
                            pictureBox2.Visible = false;
                            progressBar1.Visible = true;
                            MessageBox.Show("הסרטון פורסם בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                pictureBox2.Visible = false;
                progressBar1.Visible = true;
                buttonPublish.Enabled = true;
                groupBox3.Enabled = true;
            }
        }

        private void trackBarVolume1_Scroll(object sender, EventArgs e)
        {
            audioSoundEditor1.OutputVolumeSet((short)trackBarVolume1.Value, enumVolumeScales.SCALE_LINEAR);
        }

        private void mnuAudio_Test_Click(object sender, EventArgs e)
        {
            FormTestSound f = new FormTestSound();
            f.ShowDialog();
        }

        private void ApplyLanguageUI()
        {
            if (MyMentor.Properties.Settings.Default.CultureInfo == "he-il")
            {
                mnuTools_UI_Hebrew.Checked = true;
                mnuTools_UI_English.Checked = false;

                this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                this.RightToLeftLayout = true;

                tabControl1.RightToLeftLayout = true;
                //groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                //groupBox2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                //groupBox3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                //groupBox4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                //groupBox5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                //groupBox6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                //groupBox7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                //groupBox8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                //groupBox9.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
                //groupBox10.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            }
            else
            {
                this.RightToLeft = System.Windows.Forms.RightToLeft.No;
                this.RightToLeftLayout = false;

                mnuTools_UI_Hebrew.Checked = false;
                mnuTools_UI_English.Checked = true;

                tabControl1.RightToLeftLayout = false;
                //groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
                //groupBox2.RightToLeft = System.Windows.Forms.RightToLeft.No;
                //groupBox3.RightToLeft = System.Windows.Forms.RightToLeft.No;
                //groupBox4.RightToLeft = System.Windows.Forms.RightToLeft.No;
                //groupBox5.RightToLeft = System.Windows.Forms.RightToLeft.No;
                //groupBox6.RightToLeft = System.Windows.Forms.RightToLeft.No;
                //groupBox7.RightToLeft = System.Windows.Forms.RightToLeft.No;
                //groupBox8.RightToLeft = System.Windows.Forms.RightToLeft.No;
                //groupBox9.RightToLeft = System.Windows.Forms.RightToLeft.No;
                //groupBox10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            }

            CultureInfo newCulture = new CultureInfo(MyMentor.Properties.Settings.Default.CultureInfo);
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;

            ResourceManager rm = new ResourceManager("MyMentor.Resources.Strings",
                                      typeof(FormMain).Assembly);
            mnuFile.Text = rm.GetString("mnuFile");
            tabControl1.TabPages[0].Text = rm.GetString("tabControl1.tab0");
            tabControl1.TabPages[1].Text = rm.GetString("tabControl1.tab1");
            tabControl1.TabPages[2].Text = rm.GetString("tabControl1.tab2");
            tabControl1.TabPages[3].Text = rm.GetString("tabControl1.tab3");

        }

        private void mnuTools_UI_English_Click(object sender, EventArgs e)
        {
            MyMentor.Properties.Settings.Default.CultureInfo = "en-us";
            MyMentor.Properties.Settings.Default.Save();
            MessageBox.Show("You have to restart MyMentor to apply changes", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

        }

        private void mnuTools_UI_Hebrew_Click(object sender, EventArgs e)
        {
            MyMentor.Properties.Settings.Default.CultureInfo = "he-il";
            MyMentor.Properties.Settings.Default.Save();
            MessageBox.Show("יש לצאת ולהכנס מהמערכת על מנת להחיל את השינויים", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }


        private void tbKeywords_TextChanged(object sender, EventArgs e)
        {
            Clip.Current.Keywords = tbKeywords.Text;
            RegenerateClipName(true);

        }

        private void tbClipDescription_TextChanged(object sender, EventArgs e)
        {
            Clip.Current.Description = tbClipDescription.Text;
            RegenerateClipName(true);
        }

        private void btnAddDate_Click(object sender, EventArgs e)
        {
            if (Clip.Current.ReadingDates.Contains( DateTime.ParseExact(  dtpReadingDate.Value.ToString("dd/MM/yyyy") , "dd/MM/yyyy", CultureInfo.InvariantCulture )))
            {
                return;
            }

            Clip.Current.IsDirty = true;
            Clip.Current.ReadingDates.Add(DateTime.ParseExact(dtpReadingDate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture));
            RegenerateDatesBox();
        }

        private void RegenerateDatesBox()
        {
            listBoxDates.Items.Clear();
            listBoxDates.Items.AddRange(Clip.Current.ReadingDates.Select( d => (object)d.ToString("dd/MM/yyyy")).ToArray());
        }

        private async void comboCategory1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboCategory1.SelectedIndex >= 0)
            {
                this.comboCategory2.DisplayMember = "Value";
                this.comboCategory2.ValueMember = "ObjectId";
                this.comboCategory2.DataSource = (await ParseTables.GetCategory2((string)comboCategory1.SelectedValue)).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    Value = c.Get<string>("value")
                }).ToList(); ;

                if (Clip.Current.Category2 != null)
                {
                    this.comboCategory2.SelectedValue = Clip.Current.Category2;
                }
            }
            else
            {
                this.comboCategory2.DataSource = null;
            }

            RegenerateClipName(true);

        }

        private void comboCategory2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RegenerateClipName(true);
        }

        private void comboCategory3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RegenerateClipName(true);
        }

        private void comboCategory4_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RegenerateClipName(true);
        }

        private void btnRemoveDate_Click(object sender, EventArgs e)
        {
            if (listBoxDates.SelectedIndex >= 0)
            {
                Clip.Current.IsDirty = true;
                Clip.Current.ReadingDates.RemoveAt(listBoxDates.SelectedIndex);
                RegenerateDatesBox();
            }
        }

        private void timerPreStartFixPlayback_Tick(object sender, EventArgs e)
        {
            timerPreStartFixPlayback.Enabled = false;

            // get the position selected on the waveform analyzer, if any
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            TimeSpan buffer = new TimeSpan(0, 0, (int)numericUpDownBufferRecord.Value);

            int nBeginPlaying = Math.Max(0, nBeginSelectionInMs - (int)buffer.TotalMilliseconds);

            // if a selection is available
            if (bSelectionAvailable)
            {
                // play selected range only
                audioSoundEditor1.PlaySoundRange(nBeginPlaying, nBeginSelectionInMs);
                m_bRecOverwriteMode = true;
            }

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FormReadyTexts form = new FormReadyTexts(this))
            {
                if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    richTextBox1.Text = form.SelectedText;
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
