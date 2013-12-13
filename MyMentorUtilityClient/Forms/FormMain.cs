﻿using System;
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
using Parse;

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

        private TimeSpan m_setStartTime = TimeSpan.Zero;
        private Word m_selectedScheduledWord = null;
        private bool m_selectedAnchor = false;
        private bool m_selectedSilentAnchor = false;
        private bool m_skipSelectionChange = false;
        private int m_waveFormTabIndex = 1;
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
        private const string START_PAUSE_SECTION_ANCHOR = "[התחלה]";
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
        public Button buttonStartDJPlay;
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
        private MyMentorUtilityClient.TimeSpinner.TimePickerSpinner timePickerCurrentWord;
        public Button buttonRestartScheduling;
        private Panel panel5;
        private Label label7;
        private NumericUpDown numericUpDownInterval;
        public Button buttonScheduleAnchor;
        private Label label11;
        private ToolStripButton tbrProperties;
        public Label lblLoginUser;
        public Button buttonStartRecOverwrite;
        private ToolStripMenuItem mnuLoginDifferentUser;
        private ImageList imageList1;
        private Timer timerRecordIcon;
        private bool b_recordIconRed;
        private TableLayoutPanel tableLayoutPanel5;
        private GroupBox groupBox3;
        private TextBox textBox8;
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
        private TextBox textBox3;
        private Label label15;
        private ComboBox comboBox1;
        private Label label16;
        private Label label19;
        private TextBox textBox6;
        private Label label20;
        private TextBox textBox5;
        private Label label21;
        private TextBox textBox4;
        private Label label22;
        private TextBox textBox1;
        private MaskedTextBox maskedTextBox1;
        private Label label23;
        private Panel panel4;
        private Button button1;
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
        private ToolStripMenuItem mnuRemoveNikud;
        private ToolStripSeparator toolStripMenuItem12;
        private ToolStripMenuItem mnuRemoveTeamim;
        private RichTextBox richTextBox4;
        private string m_strExportPathname;
        private Label label25;
        private TimeSpan m_rem_anchorFixRecording = TimeSpan.Zero;

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
            this.buttonStartRecOverwrite = new System.Windows.Forms.Button();
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
            this.richTextBox2 = new System.Windows.Forms.RichTextBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.buttonAutoDevide = new System.Windows.Forms.Button();
            this.label24 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.richTextBox4 = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.LabelCurrentSchedulingTimer = new System.Windows.Forms.Label();
            this.buttonRestartScheduling = new System.Windows.Forms.Button();
            this.buttonHammer = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonStartDJPlay = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDownInterval = new System.Windows.Forms.NumericUpDown();
            this.buttonScheduleAnchor = new System.Windows.Forms.Button();
            this.panel6 = new System.Windows.Forms.Panel();
            this.richTextBox3 = new System.Windows.Forms.RichTextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
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
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Picture1 = new System.Windows.Forms.PictureBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_NewClip = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFile_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_Save = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_SaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFile_Parse = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuLoginDifferentUser = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile_Exit = new System.Windows.Forms.ToolStripMenuItem();
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
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuRemoveNikud = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemoveTeamim = new System.Windows.Forms.ToolStripMenuItem();
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
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.timerRecordIcon = new System.Windows.Forms.Timer(this.components);
            this.comboBoxAutoDevidePar = new PresentationControls.CheckBoxComboBox();
            this.comboBoxAutoDevideSen = new PresentationControls.CheckBoxComboBox();
            this.timePickerCurrentWord = new MyMentorUtilityClient.TimeSpinner.TimePickerSpinner();
            this.label25 = new System.Windows.Forms.Label();
            this.Frame4.SuspendLayout();
            this.framePlayback.SuspendLayout();
            this.FrameRecording.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.ToolStrip1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.groupBox8.SuspendLayout();
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
            this.Frame4.Location = new System.Drawing.Point(3, 135);
            this.Frame4.Name = "Frame4";
            this.Frame4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tableLayoutPanel1.SetRowSpan(this.Frame4, 2);
            this.Frame4.Size = new System.Drawing.Size(551, 214);
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
            this.framePlayback.Location = new System.Drawing.Point(736, 245);
            this.framePlayback.Name = "framePlayback";
            this.framePlayback.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.framePlayback.Size = new System.Drawing.Size(423, 95);
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
            this.buttonPlay.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlay.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonPlay.Location = new System.Drawing.Point(309, 27);
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
            this.buttonStop.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStop.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStop.Location = new System.Drawing.Point(6, 27);
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
            this.buttonPlaySelection.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonPlaySelection.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonPlaySelection.Location = new System.Drawing.Point(113, 27);
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
            this.LabelStatus.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelStatus.ForeColor = System.Drawing.SystemColors.ControlText;
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
            this.audioSoundEditor1.WaveAnalyzerMouseNotification += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerMouseNotificationEventHandler(this.audioSoundEditor1_WaveAnalyzerMouseNotification);
            this.audioSoundEditor1.WaveAnalyzerLineMoving += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerLineMovingEventHandler(this.audioSoundEditor1_WaveAnalyzerLineMoving);
            this.audioSoundEditor1.WaveAnalyzerHorzLineMoving += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerHorzLineMovingEventHandler(this.audioSoundEditor1_WaveAnalyzerHorzLineMoving);
            this.audioSoundEditor1.WaveAnalyzerGraphicItemClick += new AudioSoundEditor.AudioSoundEditor.WaveAnalyzerGraphicItemClickEventHandler(this.audioSoundEditor1_WaveAnalyzerGraphicItemClick);
            // 
            // FrameRecording
            // 
            this.FrameRecording.BackColor = System.Drawing.SystemColors.Control;
            this.FrameRecording.Controls.Add(this.buttonStartRecOverwrite);
            this.FrameRecording.Controls.Add(this.buttonStartRecAppend);
            this.FrameRecording.Controls.Add(this.buttonStopRecording);
            this.FrameRecording.Controls.Add(this.buttonStartRecNew);
            this.FrameRecording.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FrameRecording.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FrameRecording.Location = new System.Drawing.Point(608, 135);
            this.FrameRecording.Name = "FrameRecording";
            this.FrameRecording.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.FrameRecording.Size = new System.Drawing.Size(551, 80);
            this.FrameRecording.TabIndex = 24;
            this.FrameRecording.TabStop = false;
            this.FrameRecording.Text = "הקלטה";
            // 
            // buttonStartRecOverwrite
            // 
            this.buttonStartRecOverwrite.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStartRecOverwrite.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStartRecOverwrite.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartRecOverwrite.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStartRecOverwrite.Location = new System.Drawing.Point(288, 31);
            this.buttonStartRecOverwrite.Name = "buttonStartRecOverwrite";
            this.buttonStartRecOverwrite.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStartRecOverwrite.Size = new System.Drawing.Size(142, 43);
            this.buttonStartRecOverwrite.TabIndex = 7;
            this.buttonStartRecOverwrite.Text = "התחל ממיקום נבחר";
            this.buttonStartRecOverwrite.UseVisualStyleBackColor = false;
            this.buttonStartRecOverwrite.Click += new System.EventHandler(this.buttonStartRecOverwrite_Click);
            // 
            // buttonStartRecAppend
            // 
            this.buttonStartRecAppend.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStartRecAppend.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStartRecAppend.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartRecAppend.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStartRecAppend.Location = new System.Drawing.Point(175, 31);
            this.buttonStartRecAppend.Name = "buttonStartRecAppend";
            this.buttonStartRecAppend.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStartRecAppend.Size = new System.Drawing.Size(107, 43);
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
            this.buttonStopRecording.Location = new System.Drawing.Point(42, 31);
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
            this.buttonStartRecNew.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonStartRecNew.BackgroundImage")));
            this.buttonStartRecNew.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.buttonStartRecNew.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStartRecNew.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartRecNew.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStartRecNew.Location = new System.Drawing.Point(432, 32);
            this.buttonStartRecNew.Name = "buttonStartRecNew";
            this.buttonStartRecNew.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.buttonStartRecNew.Size = new System.Drawing.Size(116, 42);
            this.buttonStartRecNew.TabIndex = 4;
            this.buttonStartRecNew.Text = "התחל חדש";
            this.buttonStartRecNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
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
            this.label18.Location = new System.Drawing.Point(3, 1);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(15, 207);
            this.label18.TabIndex = 60;
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(20, 1);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(15, 207);
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
            this.tabControl1.Font = new System.Drawing.Font("Arial Narrow", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
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
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 82.68793F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 17.31207F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 134F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 154F));
            this.tableLayoutPanel2.Controls.Add(this.ToolStrip1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.richTextBox1, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.richTextBox2, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel7, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.richTextBox4, 2, 0);
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
            this.richTextBox1.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
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
            // richTextBox2
            // 
            this.richTextBox2.Location = new System.Drawing.Point(337, 3);
            this.richTextBox2.Name = "richTextBox2";
            this.richTextBox2.Size = new System.Drawing.Size(100, 34);
            this.richTextBox2.TabIndex = 33;
            this.richTextBox2.Text = "";
            this.richTextBox2.Visible = false;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.groupBox8);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel7.Location = new System.Drawing.Point(443, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(716, 74);
            this.panel7.TabIndex = 34;
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.comboBoxAutoDevidePar);
            this.groupBox8.Controls.Add(this.buttonAutoDevide);
            this.groupBox8.Controls.Add(this.label24);
            this.groupBox8.Controls.Add(this.comboBoxAutoDevideSen);
            this.groupBox8.Controls.Add(this.label14);
            this.groupBox8.Font = new System.Drawing.Font("Arial", 12F);
            this.groupBox8.Location = new System.Drawing.Point(3, 3);
            this.groupBox8.Name = "groupBox8";
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
            this.buttonAutoDevide.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonAutoDevide.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonAutoDevide.Image = global::MyMentor.Properties.Resources._1386947558_anchor;
            this.buttonAutoDevide.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonAutoDevide.Location = new System.Drawing.Point(23, 20);
            this.buttonAutoDevide.Name = "buttonAutoDevide";
            this.buttonAutoDevide.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.buttonAutoDevide.Size = new System.Drawing.Size(113, 42);
            this.buttonAutoDevide.TabIndex = 18;
            this.buttonAutoDevide.Text = "קבע עוגנים";
            this.buttonAutoDevide.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonAutoDevide.UseVisualStyleBackColor = false;
            this.buttonAutoDevide.Click += new System.EventHandler(this.buttonAutoDevide_Click);
            // 
            // label24
            // 
            this.label24.BackColor = System.Drawing.SystemColors.Control;
            this.label24.Cursor = System.Windows.Forms.Cursors.Default;
            this.label24.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label24.Location = new System.Drawing.Point(350, 34);
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
            this.label14.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label14.Location = new System.Drawing.Point(625, 31);
            this.label14.Name = "label14";
            this.label14.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label14.Size = new System.Drawing.Size(75, 21);
            this.label14.TabIndex = 14;
            this.label14.Text = "סוף פסקה";
            // 
            // richTextBox4
            // 
            this.richTextBox4.Location = new System.Drawing.Point(210, 3);
            this.richTextBox4.Name = "richTextBox4";
            this.richTextBox4.Size = new System.Drawing.Size(76, 34);
            this.richTextBox4.TabIndex = 35;
            this.richTextBox4.Text = "";
            this.richTextBox4.Visible = false;
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
            this.tableLayoutPanel1.Controls.Add(this.framePlayback, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 37.5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 31.25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1162, 398);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.label18);
            this.panel2.Controls.Add(this.label17);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(560, 135);
            this.panel2.Name = "panel2";
            this.tableLayoutPanel1.SetRowSpan(this.panel2, 2);
            this.panel2.Size = new System.Drawing.Size(42, 214);
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
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.timePickerCurrentWord);
            this.groupBox1.Controls.Add(this.LabelCurrentSchedulingTimer);
            this.groupBox1.Controls.Add(this.buttonRestartScheduling);
            this.groupBox1.Controls.Add(this.buttonHammer);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.buttonStartDJPlay);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.groupBox1.Location = new System.Drawing.Point(605, 256);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.groupBox1.Size = new System.Drawing.Size(554, 139);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "תזמון קטעים";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(195, 104);
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
            this.LabelCurrentSchedulingTimer.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelCurrentSchedulingTimer.ForeColor = System.Drawing.Color.Black;
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
            this.buttonRestartScheduling.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonRestartScheduling.ForeColor = System.Drawing.SystemColors.ControlText;
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
            this.buttonHammer.Font = new System.Drawing.Font("Arial", 12F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonHammer.ForeColor = System.Drawing.SystemColors.ControlText;
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
            this.label9.Location = new System.Drawing.Point(469, 105);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(76, 18);
            this.label9.TabIndex = 24;
            this.label9.Text = "מיקום נוכחי";
            // 
            // buttonStartDJPlay
            // 
            this.buttonStartDJPlay.BackColor = System.Drawing.SystemColors.Control;
            this.buttonStartDJPlay.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonStartDJPlay.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStartDJPlay.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonStartDJPlay.Location = new System.Drawing.Point(448, 25);
            this.buttonStartDJPlay.Name = "buttonStartDJPlay";
            this.buttonStartDJPlay.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonStartDJPlay.Size = new System.Drawing.Size(94, 41);
            this.buttonStartDJPlay.TabIndex = 9;
            this.buttonStartDJPlay.Text = "התחל";
            this.buttonStartDJPlay.UseVisualStyleBackColor = false;
            this.buttonStartDJPlay.Click += new System.EventHandler(this.buttonStartDJPlay_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label25);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.numericUpDownInterval);
            this.groupBox2.Controls.Add(this.buttonScheduleAnchor);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(225, 256);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(353, 139);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "בדיקת מעברים";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(18, 43);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(28, 18);
            this.label11.TabIndex = 28;
            this.label11.Text = "שנ\'";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(107, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 18);
            this.label7.TabIndex = 27;
            this.label7.Text = "היסט עליון/תחתון";
            // 
            // numericUpDownInterval
            // 
            this.numericUpDownInterval.Location = new System.Drawing.Point(52, 38);
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
            this.buttonScheduleAnchor.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonScheduleAnchor.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonScheduleAnchor.Location = new System.Drawing.Point(229, 38);
            this.buttonScheduleAnchor.Name = "buttonScheduleAnchor";
            this.buttonScheduleAnchor.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonScheduleAnchor.Size = new System.Drawing.Size(94, 28);
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
            this.richTextBox3.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(177)));
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
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80.30822F));
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 19.69178F));
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
            this.groupBox3.Controls.Add(this.textBox8);
            this.groupBox3.Controls.Add(this.textBox7);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.Controls.Add(this.groupBox7);
            this.groupBox3.Controls.Add(this.label13);
            this.groupBox3.Controls.Add(this.textBox3);
            this.groupBox3.Controls.Add(this.label15);
            this.groupBox3.Controls.Add(this.comboBox1);
            this.groupBox3.Controls.Add(this.label16);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.textBox6);
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.textBox5);
            this.groupBox3.Controls.Add(this.label21);
            this.groupBox3.Controls.Add(this.textBox4);
            this.groupBox3.Controls.Add(this.label22);
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Controls.Add(this.maskedTextBox1);
            this.groupBox3.Controls.Add(this.label23);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(239, 4);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(925, 387);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            // 
            // textBox8
            // 
            this.textBox8.Font = new System.Drawing.Font("Arial", 12F);
            this.textBox8.Location = new System.Drawing.Point(560, 327);
            this.textBox8.Margin = new System.Windows.Forms.Padding(4);
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(237, 26);
            this.textBox8.TabIndex = 56;
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
            this.groupBox5.Location = new System.Drawing.Point(47, 25);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(169, 161);
            this.groupBox5.TabIndex = 53;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "נעילות לימוד";
            // 
            // sop_teacher1l
            // 
            this.sop_teacher1l.AutoSize = true;
            this.sop_teacher1l.Location = new System.Drawing.Point(62, 35);
            this.sop_teacher1l.Name = "sop_teacher1l";
            this.sop_teacher1l.Size = new System.Drawing.Size(71, 22);
            this.sop_teacher1l.TabIndex = 46;
            this.sop_teacher1l.Text = "מורה 1";
            this.sop_teacher1l.UseVisualStyleBackColor = true;
            // 
            // sop_studentl
            // 
            this.sop_studentl.AutoSize = true;
            this.sop_studentl.Location = new System.Drawing.Point(68, 119);
            this.sop_studentl.Name = "sop_studentl";
            this.sop_studentl.Size = new System.Drawing.Size(65, 22);
            this.sop_studentl.TabIndex = 47;
            this.sop_studentl.Text = "תלמיד";
            this.sop_studentl.UseVisualStyleBackColor = true;
            // 
            // sop_teacher2l
            // 
            this.sop_teacher2l.AutoSize = true;
            this.sop_teacher2l.Location = new System.Drawing.Point(62, 91);
            this.sop_teacher2l.Name = "sop_teacher2l";
            this.sop_teacher2l.Size = new System.Drawing.Size(71, 22);
            this.sop_teacher2l.TabIndex = 48;
            this.sop_teacher2l.Text = "מורה 2";
            this.sop_teacher2l.UseVisualStyleBackColor = true;
            // 
            // sop_teacherAndStudentl
            // 
            this.sop_teacherAndStudentl.AutoSize = true;
            this.sop_teacherAndStudentl.Location = new System.Drawing.Point(30, 63);
            this.sop_teacherAndStudentl.Name = "sop_teacherAndStudentl";
            this.sop_teacherAndStudentl.Size = new System.Drawing.Size(103, 22);
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
            this.groupBox4.Location = new System.Drawing.Point(47, 207);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(169, 154);
            this.groupBox4.TabIndex = 52;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "אפשרויות לימוד";
            // 
            // sop_teacher1
            // 
            this.sop_teacher1.AutoSize = true;
            this.sop_teacher1.Location = new System.Drawing.Point(62, 35);
            this.sop_teacher1.Name = "sop_teacher1";
            this.sop_teacher1.Size = new System.Drawing.Size(71, 22);
            this.sop_teacher1.TabIndex = 39;
            this.sop_teacher1.Text = "מורה 1";
            this.sop_teacher1.UseVisualStyleBackColor = true;
            // 
            // sop_student
            // 
            this.sop_student.AutoSize = true;
            this.sop_student.Location = new System.Drawing.Point(68, 119);
            this.sop_student.Name = "sop_student";
            this.sop_student.Size = new System.Drawing.Size(65, 22);
            this.sop_student.TabIndex = 40;
            this.sop_student.Text = "תלמיד";
            this.sop_student.UseVisualStyleBackColor = true;
            // 
            // sop_teacher2
            // 
            this.sop_teacher2.AutoSize = true;
            this.sop_teacher2.Location = new System.Drawing.Point(62, 91);
            this.sop_teacher2.Name = "sop_teacher2";
            this.sop_teacher2.Size = new System.Drawing.Size(71, 22);
            this.sop_teacher2.TabIndex = 41;
            this.sop_teacher2.Text = "מורה 2";
            this.sop_teacher2.UseVisualStyleBackColor = true;
            // 
            // sop_teacherAndStudent
            // 
            this.sop_teacherAndStudent.AutoSize = true;
            this.sop_teacherAndStudent.Location = new System.Drawing.Point(30, 63);
            this.sop_teacherAndStudent.Name = "sop_teacherAndStudent";
            this.sop_teacherAndStudent.Size = new System.Drawing.Size(103, 22);
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
            this.groupBox6.Location = new System.Drawing.Point(244, 25);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(164, 161);
            this.groupBox6.TabIndex = 51;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "נעילות מקטעים";
            // 
            // loc_par
            // 
            this.loc_par.AutoSize = true;
            this.loc_par.Location = new System.Drawing.Point(52, 28);
            this.loc_par.Name = "loc_par";
            this.loc_par.Size = new System.Drawing.Size(64, 22);
            this.loc_par.TabIndex = 34;
            this.loc_par.Text = "פסקה";
            this.loc_par.UseVisualStyleBackColor = true;
            // 
            // loc_wor
            // 
            this.loc_wor.AutoSize = true;
            this.loc_wor.Location = new System.Drawing.Point(56, 112);
            this.loc_wor.Name = "loc_wor";
            this.loc_wor.Size = new System.Drawing.Size(60, 22);
            this.loc_wor.TabIndex = 35;
            this.loc_wor.Text = "מילים";
            this.loc_wor.UseVisualStyleBackColor = true;
            // 
            // loc_sec
            // 
            this.loc_sec.AutoSize = true;
            this.loc_sec.Location = new System.Drawing.Point(63, 84);
            this.loc_sec.Name = "loc_sec";
            this.loc_sec.Size = new System.Drawing.Size(53, 22);
            this.loc_sec.TabIndex = 36;
            this.loc_sec.Text = "קטע";
            this.loc_sec.UseVisualStyleBackColor = true;
            // 
            // loc_sen
            // 
            this.loc_sen.AutoSize = true;
            this.loc_sen.Location = new System.Drawing.Point(50, 56);
            this.loc_sen.Name = "loc_sen";
            this.loc_sen.Size = new System.Drawing.Size(66, 22);
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
            this.groupBox7.Location = new System.Drawing.Point(244, 207);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(164, 151);
            this.groupBox7.TabIndex = 50;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "מקטעי ברירת מחדל";
            // 
            // def_sen
            // 
            this.def_sen.AutoSize = true;
            this.def_sen.Location = new System.Drawing.Point(50, 60);
            this.def_sen.Name = "def_sen";
            this.def_sen.Size = new System.Drawing.Size(66, 22);
            this.def_sen.TabIndex = 32;
            this.def_sen.Text = "משפט";
            this.def_sen.UseVisualStyleBackColor = true;
            // 
            // def_par
            // 
            this.def_par.AutoSize = true;
            this.def_par.Location = new System.Drawing.Point(52, 32);
            this.def_par.Name = "def_par";
            this.def_par.Size = new System.Drawing.Size(64, 22);
            this.def_par.TabIndex = 29;
            this.def_par.Text = "פסקה";
            this.def_par.UseVisualStyleBackColor = true;
            // 
            // def_wor
            // 
            this.def_wor.AutoSize = true;
            this.def_wor.Location = new System.Drawing.Point(56, 116);
            this.def_wor.Name = "def_wor";
            this.def_wor.Size = new System.Drawing.Size(60, 22);
            this.def_wor.TabIndex = 30;
            this.def_wor.Text = "מילים";
            this.def_wor.UseVisualStyleBackColor = true;
            // 
            // def_sec
            // 
            this.def_sec.AutoSize = true;
            this.def_sec.Location = new System.Drawing.Point(63, 88);
            this.def_sec.Name = "def_sec";
            this.def_sec.Size = new System.Drawing.Size(53, 22);
            this.def_sec.TabIndex = 31;
            this.def_sec.Text = "קטע";
            this.def_sec.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial", 12F);
            this.label13.Location = new System.Drawing.Point(837, 110);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(41, 18);
            this.label13.TabIndex = 43;
            this.label13.Text = "תיאור";
            // 
            // textBox3
            // 
            this.textBox3.Font = new System.Drawing.Font("Arial", 12F);
            this.textBox3.Location = new System.Drawing.Point(465, 107);
            this.textBox3.Margin = new System.Windows.Forms.Padding(4);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(332, 70);
            this.textBox3.TabIndex = 44;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(836, 328);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(37, 18);
            this.label15.TabIndex = 21;
            this.label15.Text = "משך";
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Arial", 12F);
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "PENDING",
            "PUBLISHED"});
            this.comboBox1.Location = new System.Drawing.Point(618, 293);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(4);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(180, 26);
            this.comboBox1.TabIndex = 17;
            this.comboBox1.Text = "PENDING";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial", 12F);
            this.label16.Location = new System.Drawing.Point(837, 295);
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
            this.label19.Location = new System.Drawing.Point(838, 262);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(37, 18);
            this.label19.TabIndex = 14;
            this.label19.Text = "תגים";
            // 
            // textBox6
            // 
            this.textBox6.Font = new System.Drawing.Font("Arial", 12F);
            this.textBox6.Location = new System.Drawing.Point(520, 259);
            this.textBox6.Margin = new System.Windows.Forms.Padding(4);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(278, 26);
            this.textBox6.TabIndex = 15;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Arial", 12F);
            this.label20.Location = new System.Drawing.Point(838, 226);
            this.label20.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(80, 18);
            this.label20.TabIndex = 12;
            this.label20.Text = "תת קטגוריה";
            // 
            // textBox5
            // 
            this.textBox5.Font = new System.Drawing.Font("Arial", 12F);
            this.textBox5.Location = new System.Drawing.Point(520, 223);
            this.textBox5.Margin = new System.Windows.Forms.Padding(4);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(278, 26);
            this.textBox5.TabIndex = 13;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Arial", 12F);
            this.label21.Location = new System.Drawing.Point(838, 190);
            this.label21.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(56, 18);
            this.label21.TabIndex = 10;
            this.label21.Text = "קטגוריה";
            // 
            // textBox4
            // 
            this.textBox4.Font = new System.Drawing.Font("Arial", 12F);
            this.textBox4.Location = new System.Drawing.Point(520, 187);
            this.textBox4.Margin = new System.Windows.Forms.Padding(4);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(278, 26);
            this.textBox4.TabIndex = 11;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Arial", 12F);
            this.label22.Location = new System.Drawing.Point(836, 30);
            this.label22.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(66, 18);
            this.label22.TabIndex = 1;
            this.label22.Text = "שם שיעור";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Arial", 12F);
            this.textBox1.Location = new System.Drawing.Point(550, 27);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(246, 26);
            this.textBox1.TabIndex = 2;
            // 
            // maskedTextBox1
            // 
            this.maskedTextBox1.Font = new System.Drawing.Font("Arial", 12F);
            this.maskedTextBox1.Location = new System.Drawing.Point(742, 63);
            this.maskedTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.maskedTextBox1.Mask = "0.00";
            this.maskedTextBox1.Name = "maskedTextBox1";
            this.maskedTextBox1.Size = new System.Drawing.Size(55, 26);
            this.maskedTextBox1.TabIndex = 3;
            this.maskedTextBox1.Text = "100";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Arial", 12F);
            this.label23.Location = new System.Drawing.Point(836, 66);
            this.label23.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(41, 18);
            this.label23.TabIndex = 4;
            this.label23.Text = "גרסה";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button1);
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(224, 389);
            this.panel4.TabIndex = 11;
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::MyMentor.Properties.Resources._1386909646_519838_50_Cloud_Arrow_Up;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.button1.Font = new System.Drawing.Font("Arial", 12F);
            this.button1.Location = new System.Drawing.Point(36, 327);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(124, 44);
            this.button1.TabIndex = 0;
            this.button1.Text = "פרסם שיעור";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = true;
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
            this.Picture1.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Picture1.ForeColor = System.Drawing.SystemColors.WindowText;
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
            this.menuFile,
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
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile_NewClip,
            this.toolStripMenuItem9,
            this.mnuFile_Open,
            this.mnuFile_Save,
            this.mnuFile_SaveAs,
            this.toolStripMenuItem10,
            this.mnuFile_Parse,
            this.toolStripMenuItem11,
            this.mnuLoginDifferentUser,
            this.mnuFile_Exit});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(46, 20);
            this.menuFile.Text = "קובץ";
            // 
            // mnuFile_NewClip
            // 
            this.mnuFile_NewClip.Name = "mnuFile_NewClip";
            this.mnuFile_NewClip.Size = new System.Drawing.Size(200, 22);
            this.mnuFile_NewClip.Text = "שיעור חדש";
            this.mnuFile_NewClip.Click += new System.EventHandler(this.mnuFile_NewClip_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(197, 6);
            // 
            // mnuFile_Open
            // 
            this.mnuFile_Open.Name = "mnuFile_Open";
            this.mnuFile_Open.Size = new System.Drawing.Size(200, 22);
            this.mnuFile_Open.Text = "פתח שיעור";
            this.mnuFile_Open.Click += new System.EventHandler(this.mnuFile_Open_Click);
            // 
            // mnuFile_Save
            // 
            this.mnuFile_Save.Name = "mnuFile_Save";
            this.mnuFile_Save.Size = new System.Drawing.Size(200, 22);
            this.mnuFile_Save.Text = "שמור שיעור";
            this.mnuFile_Save.Click += new System.EventHandler(this.mnuFile_Save_Click);
            // 
            // mnuFile_SaveAs
            // 
            this.mnuFile_SaveAs.Name = "mnuFile_SaveAs";
            this.mnuFile_SaveAs.Size = new System.Drawing.Size(200, 22);
            this.mnuFile_SaveAs.Text = "שמור שיעור בשם...";
            this.mnuFile_SaveAs.Click += new System.EventHandler(this.mnuFile_SaveAs_Click);
            // 
            // toolStripMenuItem10
            // 
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Size = new System.Drawing.Size(197, 6);
            // 
            // mnuFile_Parse
            // 
            this.mnuFile_Parse.Name = "mnuFile_Parse";
            this.mnuFile_Parse.Size = new System.Drawing.Size(200, 22);
            this.mnuFile_Parse.Text = "בדוק תקינות";
            // 
            // toolStripMenuItem11
            // 
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Size = new System.Drawing.Size(197, 6);
            // 
            // mnuLoginDifferentUser
            // 
            this.mnuLoginDifferentUser.Name = "mnuLoginDifferentUser";
            this.mnuLoginDifferentUser.Size = new System.Drawing.Size(200, 22);
            this.mnuLoginDifferentUser.Text = "התחבר כמשתמש אחר";
            this.mnuLoginDifferentUser.Click += new System.EventHandler(this.mnuLoginDifferentUser_Click);
            // 
            // mnuFile_Exit
            // 
            this.mnuFile_Exit.Name = "mnuFile_Exit";
            this.mnuFile_Exit.Size = new System.Drawing.Size(200, 22);
            this.mnuFile_Exit.Text = "יציאה";
            this.mnuFile_Exit.Click += new System.EventHandler(this.mnuFile_Exit_Click);
            // 
            // טקסטToolStripMenuItem
            // 
            this.טקסטToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuText_Goto,
            this.toolStripMenuItem14,
            this.mnuAnchors,
            this.toolStripMenuItem12,
            this.mnuRemoveNikud,
            this.mnuRemoveTeamim});
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
            // toolStripMenuItem12
            // 
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Size = new System.Drawing.Size(147, 6);
            // 
            // mnuRemoveNikud
            // 
            this.mnuRemoveNikud.Name = "mnuRemoveNikud";
            this.mnuRemoveNikud.Size = new System.Drawing.Size(150, 22);
            this.mnuRemoveNikud.Text = "הסר ניקוד";
            this.mnuRemoveNikud.Click += new System.EventHandler(this.mnuRemoveNikud_Click);
            // 
            // mnuRemoveTeamim
            // 
            this.mnuRemoveTeamim.Name = "mnuRemoveTeamim";
            this.mnuRemoveTeamim.Size = new System.Drawing.Size(150, 22);
            this.mnuRemoveTeamim.Text = "הסר טעמים";
            this.mnuRemoveTeamim.Click += new System.EventHandler(this.mnuRemoveTeamim_Click);
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
            this.timerUpdateTimePickerSpinner.Tick += new System.EventHandler(this.timerUpdateSpinnerControl_Tick);
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
            this.lblLoginUser.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLoginUser.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblLoginUser.Location = new System.Drawing.Point(340, 9);
            this.lblLoginUser.Name = "lblLoginUser";
            this.lblLoginUser.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblLoginUser.Size = new System.Drawing.Size(229, 24);
            this.lblLoginUser.TabIndex = 26;
            this.lblLoginUser.Text = "הנך מחובר כ";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "1386908105_Record Button2.png");
            this.imageList1.Images.SetKeyName(1, "1386908112_Record Button1.png");
            // 
            // timerRecordIcon
            // 
            this.timerRecordIcon.Interval = 500;
            this.timerRecordIcon.Tick += new System.EventHandler(this.timerRecordIcon_Tick);
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
            this.comboBoxAutoDevideSen.Location = new System.Drawing.Point(153, 31);
            this.comboBoxAutoDevideSen.Name = "comboBoxAutoDevideSen";
            this.comboBoxAutoDevideSen.Size = new System.Drawing.Size(191, 26);
            this.comboBoxAutoDevideSen.TabIndex = 16;
            // 
            // timePickerCurrentWord
            // 
            this.timePickerCurrentWord.Location = new System.Drawing.Point(7, 96);
            this.timePickerCurrentWord.Margin = new System.Windows.Forms.Padding(4);
            this.timePickerCurrentWord.Name = "timePickerCurrentWord";
            this.timePickerCurrentWord.Size = new System.Drawing.Size(181, 36);
            this.timePickerCurrentWord.TabIndex = 25;
            this.timePickerCurrentWord.Value = System.TimeSpan.Parse("00:00:00");
            this.timePickerCurrentWord.ValueChanged += new System.EventHandler(this.timePickerSpinner1_ValueChanged);
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Arial", 9F);
            this.label25.Location = new System.Drawing.Point(104, 108);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(236, 15);
            this.label25.TabIndex = 29;
            this.label25.Text = "* יש להקליק על העוגנים בטקסט לאפשור הבדיקה";
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
            this.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MyMentor";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.Shown += new System.EventHandler(this.FormMain_Shown);
            this.ResizeEnd += new System.EventHandler(this.FormMain_ResizeEnd);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FormMain_Paint);
            this.Resize += new System.EventHandler(this.FormMain_Resize);
            this.Frame4.ResumeLayout(false);
            this.framePlayback.ResumeLayout(false);
            this.FrameRecording.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
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


            //audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_MP3;
            //audioSoundEditor1.EncodeFormats.MP3.EncodeMode = enumMp3EncodeModes.MP3_ENCODE_PRESETS;
            //audioSoundEditor1.EncodeFormats.MP3.Preset = enumMp3EncodePresets.MP3_PRESET_STANDARD;

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
            }
        }

        private void audioSoundEditor1_SoundPlaybackDone(object sender, System.EventArgs e)
        {
            buttonPause.Text = "השהה";
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            buttonStartDJPlay.Text = "התחל";
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
            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_uniqueLine, mm, mm);

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
                result = MessageBox.Show("קיימת הקלטה בזיכרון, האם אתה בטוח ליצור חדש ?", "MyMentor", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                    return;
            }

            //start record icon toggle timer
            timerRecordIcon.Enabled = true;

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
            if (m_bRecOverwriteMode)
            {
                m_bRecOverwriteMode = false;
                TimeSpan startSelectionTime = TimeSpan.Parse(LabelSelectionBegin.Text);
                //TimeSpan endSelectionTime = TimeSpan.Parse(LabelSelectionEnd.Text);

                // overwrite mode
                audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_OVERWRITE);
                audioSoundEditor1.SetOverwritePos((int)startSelectionTime.TotalMilliseconds, audioDjStudio1.GetSoundDuration(0));
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

                Word previousWord = null;
                Word firstWord = null;
                Word lastWord = null;

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

                    paragraphs_local.Add(new Paragraph
                    {
                        Content = matchParagraph.StrippedValue,
                        RealCharIndex = matchParagraph.CharIndex,
                        CharIndex = matchParagraph.CharIndex - bufferIndex,
                        Index = paragraphIndex,
                        Sentences = new List<Sentence>()
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

                        sentenceIndex++;
                        innerSentenceIndex++;
                        innerSectionIndex = -1;

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
                            Sections = new List<Section>()
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

                            int groupWordsBuffer = Math.Max(0, paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections.SelectMany(s => s.Words).Count() * 3);

                            paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections.Add(new Section
                            {
                                Content = matchSection.StrippedValue,
                                RealCharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].RealCharIndex + matchSection.CharIndex,
                                CharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].CharIndex + matchSection.CharIndex
                                - groupWordsBuffer,
                                Index = sectionIndex,
                                Words = new List<Word>(),
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

                                Word ex_word = null;

                                if (m_chapter != null &&
                                    m_chapter.Paragraphs != null)
                                {
                                    ex_word = m_chapter.Paragraphs.SelectMany(s => s.Sentences).SelectMany(se => se.Sections).SelectMany(sc => sc.Words).FirstOrDefault(w => w.Index == wordIndex);

                                    if (ex_word != null)
                                    {
                                        start = ex_word.StartTime;

                                        if (ex_word.NextWord == null)
                                        {
                                            duration = ex_word.Duration;
                                        }
                                    }
                                }

                                var newWord = new Word
                                {
                                    RealCharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].RealCharIndex + matchWord.CharIndex,
                                    CharIndex = paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].CharIndex + matchWord.CharIndex
                                    - (3 * Math.Max(0, paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Words.Count())),
                                    Index = wordIndex,
                                    Content = matchWord.Value,
                                    StartTime = start
                                };

                                //in case last word grab the duration
                                if (ex_word != null && ex_word.NextWord == null)
                                {
                                    newWord.Duration = duration;
                                }

                                //set previous word
                                if (previousWord != null)
                                {
                                    newWord.PreviousWord = previousWord;
                                }

                                paragraphs_local[paragraphIndex].Sentences[innerSentenceIndex].Sections[innerSectionIndex].Words.Add(newWord);

                                //set next word to previous one
                                if (wordIndex > 0)
                                {
                                    previousWord.NextWord = newWord;
                                }
                                else
                                {
                                    firstWord = newWord;
                                }

                                previousWord = newWord;
                                lastWord = newWord;
                                //save first word

                                bufferIndex += innerWordIndex == 0 ? 0 : 3;
                            }
                        }
                    }
                }

                //set is last to the last word (for duration manually);
                m_chapter = new Chapter();
                m_chapter.FirstWord = firstWord;
                m_chapter.LastWord = lastWord;
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

            DevideText();

            //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
            //Clip.Current.FontName = richTextBox1.Font.Name;
            Clip.Current.Chapter = m_chapter;
            Clip.Current.Save(audioSoundEditor1);

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
                    Clip.Current.Save(audioSoundEditor1);

                    if (audioSoundEditor1.GetSoundDuration() <= 0)
                    {
                        MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                        mnuFile_SaveAs.Enabled = true;
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
                Clip.Current.RtfText = richTextBox1.Rtf;
                Clip.Current.Chapter = m_chapter;
                //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
                //Clip.Current.FontName = richTextBox1.Font.Name;
                Clip.Current.Save(audioSoundEditor1);

                if (audioSoundEditor1.GetSoundDuration() <= 0)
                {
                    MessageBox.Show("השיעור נשמר בהצלחה !", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    mnuFile_SaveAs.Enabled = true;
                }
                else
                {
                    Cursor.Current = Cursors.WaitCursor;
                }
            }
        }

        private void tbrParse_Click(object sender, EventArgs e)
        {
            DevideText();
        }

        private void FormMain_Shown(object sender, EventArgs e)
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

            mnuRemoveNikud.Checked = true;
            mnuRemoveTeamim.Checked = true;

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

            audioSoundEditor1.DeleteRange(0, -1);

            DevideText();

            if (File.Exists(Path.ChangeExtension(file, ".mp3")))
            {
                audioSoundEditor1.LoadSound(Path.ChangeExtension(file, ".mp3"));
            }

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

        private short m_uniqueLine = -1;

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //copy text and insert pause anchor
            richTextBox3.Clear();
            richTextBox3.Rtf = richTextBox1.Rtf;
            richTextBox3.SelectionStart = 0;
            richTextBox3.SelectionLength = 0;
            richTextBox3.SelectedText = START_PAUSE_SECTION_ANCHOR;

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
                m_uniqueLine = -1;
                m_waveFormTabIndex = 1;
                audioSoundEditor1.DisplayWaveformAnalyzer.MouseSelectionEnable(true);

                //remove graphics
                foreach (Word word in m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(sc => sc.Sections)
                   .SelectMany(w => w.Words))
                {
                    if (word.GraphicItemUnique > -1)
                    {
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(word.GraphicItemUnique);
                    }
                }

                //remove guid line
                audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(m_uniqueLine);
                audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();
            }
            //scheduler step
            else if (tabControl1.SelectedIndex == 2)
            {
                DevideText();

                richTextBox3.SelectionStart = 0;
                //richTextBox3.SelectionLength = m_selectedScheduledWord.Length;

                //if (m_chapter.Paragraphs != null)
                //{
                //    m_selectedScheduledWord = m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(sc => sc.Sections)
                //    .SelectMany(w => w.Words).FirstOrDefault();

                //    if (m_selectedScheduledWord != null)
                //    {
                //        m_skipSelectionChange = true;
                //        richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length;
                //        richTextBox3.SelectionLength = m_selectedScheduledWord.Length;
                //        m_skipSelectionChange = false;
                //    }
                //}

                if (m_waveFormTabIndex != 2)
                {
                    m_waveFormTabIndex = 2;

                    m_uniqueLine = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineAdd("MyLine", "KooKoo", 0,
                        new WANALYZER_VERTICAL_LINE { color = Color.Yellow, nWidth = 1, nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT, nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DASH_DOT, nHighCap = enumLineCaps.LINE_CAP_SQUARE, nLowCap = enumLineCaps.LINE_CAP_SQUARE, nTranspFactor = 0 });

                    if (m_chapter.Paragraphs != null)
                    {
                        // Add lines for all anchors
                        foreach (Word word in m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).SelectMany(sc => sc.Words))
                        {
                            if (word.StartTime.TotalMilliseconds > 0)
                            {
                                word.GraphicItemUnique = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineAdd("W|" + word.Index, word.Content,
                                    (int)word.StartTime.TotalMilliseconds == 0 ? 50 : (int)word.StartTime.TotalMilliseconds,
                                    new WANALYZER_VERTICAL_LINE
                                    {
                                        color = Color.White,
                                        nWidth = 2,
                                        nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT,
                                        nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DOT,
                                        nHighCap = enumLineCaps.LINE_CAP_SQUARE,
                                        nLowCap = enumLineCaps.LINE_CAP_SQUARE,
                                        nTranspFactor = 50
                                    });
                            }
                        }
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
            mnuRemoveNikud.Checked = true;
            mnuRemoveTeamim.Checked = true;

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
            if (m_skipSelectionChange || m_chapter == null)
            {
                return;
            }

            int selectionIndex = Math.Max(0, richTextBox3.SelectionStart - START_PAUSE_SECTION_ANCHOR.Length);

            if (selectionIndex == 0) // pause anchor
            {
                //set current word start time when clicking word offline
                timePickerCurrentWord.Value = TimeSpan.Zero;
                timePickerCurrentWord.Enabled = false;

                richTextBox3.SelectionStart = 0;
                richTextBox3.SelectionLength = START_PAUSE_SECTION_ANCHOR.Length;

                m_selectedAnchor = false;
                m_selectedSilentAnchor = true;

                //take first work
                m_selectedScheduledWord = m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(sc => sc.Sections)
                    .SelectMany(w => w.Words).Where(ww => ww.Index == 0).FirstOrDefault();

                //move line to start
                audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_uniqueLine, (int)timePickerCurrentWord.Value.TotalMilliseconds, (int)timePickerCurrentWord.Value.TotalMilliseconds);

            }
            else if (m_chapter.Paragraphs != null)
            {
                m_selectedSilentAnchor = false;

                var savePreviousWord = m_selectedScheduledWord;

                //check for word selection
                m_selectedScheduledWord = m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(sc => sc.Sections)
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
                        }

                        //move line
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_uniqueLine, (int)timePickerCurrentWord.Value.TotalMilliseconds, (int)timePickerCurrentWord.Value.TotalMilliseconds);

                        m_selectedAnchor = true;

                        buttonScheduleAnchor.Enabled = true;
                    }
                    else
                    {
                        //in case last word
                        if (savePreviousWord == m_selectedScheduledWord
                            && m_selectedScheduledWord == m_chapter.LastWord)
                        {
                            if (audioSoundEditor1.GetPlaybackStatus() == enumPlaybackStatus.PLAYBACK_PLAYING)
                            {
                                buttonHammer.Enabled = false;
                                m_selectedScheduledWord.Duration = m_setStartTime - m_selectedScheduledWord.StartTime;

                                Debug.WriteLine(string.Format("Setting '{0}' with duration : {1}", m_selectedScheduledWord.Content, m_selectedScheduledWord.DurationText));
                            }
                            else
                            {
                                //set current word start time when clicking word offline
                                timePickerCurrentWord.Value = m_selectedScheduledWord.StartTime;
                                timePickerCurrentWord.Enabled = true;
                            }
                        }
                        else
                        {
                            //select the word
                            richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length;
                            richTextBox3.SelectionLength = m_selectedScheduledWord.Length;

                            //in case set start time during scheduling
                            if (m_setStartTime != TimeSpan.Zero && audioSoundEditor1.GetPlaybackStatus() == enumPlaybackStatus.PLAYBACK_PLAYING)
                            {
                                m_selectedScheduledWord.StartTime = m_setStartTime;

                                Debug.WriteLine(string.Format("Setting '{0}' with start time : {1}", m_selectedScheduledWord.Content, m_selectedScheduledWord.StartTimeText));

                                m_setStartTime = TimeSpan.Zero;
                            }
                            else if (audioSoundEditor1.GetPlaybackStatus() != enumPlaybackStatus.PLAYBACK_PLAYING)
                            {
                                //set current word start time when clicking word offline
                                timePickerCurrentWord.Value = m_selectedScheduledWord.StartTime;
                                timePickerCurrentWord.Enabled = true;
                            }
                        }

                        //move guid line
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_uniqueLine, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds);
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

        private void buttonStartDJPlay_Click(object sender, EventArgs e)
        {
            Int32 nDurationInMs = audioSoundEditor1.GetSoundDuration();
            if (nDurationInMs == 0)
            {
                return;
            }

            if (buttonStartDJPlay.Text == "התחל" || buttonStartDJPlay.Text == "המשך")
            {
                buttonHammer.Enabled = true;
                audioSoundEditor1.PlaySoundRange((int)timePickerCurrentWord.Value.TotalMilliseconds, -1);
                buttonStartDJPlay.Text = "עצור";

                buttonRestartScheduling.Enabled = false;
                timerUpdateTimePickerSpinner.Enabled = true;
                timePickerCurrentWord.Enabled = false;
            }
            else if (buttonStartDJPlay.Text == "עצור")
            {
                buttonHammer.Enabled = false;
                buttonRestartScheduling.Enabled = true;
                timePickerCurrentWord.Enabled = true;
                timerUpdateTimePickerSpinner.Enabled = false;
                audioSoundEditor1.StopSound();
                buttonStartDJPlay.Text = "המשך";
            }

            //djLineTimer.Enabled = true;
        }

        [DllImport("user32.dll")]
        static extern bool HideCaret(IntPtr hWnd);

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

        /// <summary>
        /// Push the fucking hammer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHammer_Click(object sender, EventArgs e)
        {
            if (audioSoundEditor1.GetPlaybackStatus() == enumPlaybackStatus.PLAYBACK_PLAYING)
            {
                m_setStartTime = timePickerCurrentWord.Value;

                var saveIt = m_setStartTime;

                //in case current on silent part
                if (m_selectedSilentAnchor)
                {
                    //goto to first word in chapter
                    richTextBox3.SelectionStart = START_PAUSE_SECTION_ANCHOR.Length + 1;
                }
                else
                {
                    //goto next word
                    richTextBox3.SelectionStart = richTextBox3.SelectionStart + richTextBox3.SelectionLength + 3;
                }

                //in case the last word we dont want to draw another ending line
                if (m_selectedScheduledWord != m_chapter.LastWord)
                {
                    //add line 
                    m_selectedScheduledWord.GraphicItemUnique = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineAdd("W|" + m_selectedScheduledWord.Index, m_selectedScheduledWord.Content,
                        (int)saveIt.TotalMilliseconds == 0 ? 50 : (int)saveIt.TotalMilliseconds,
                        new WANALYZER_VERTICAL_LINE
                        {
                            color = Color.White,
                            nWidth = 2,
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

            if (m_selectedScheduledWord != null && !m_selectedAnchor && !m_selectedSilentAnchor)
            {
                m_selectedScheduledWord.StartTime = timePickerCurrentWord.Value;

                //move relevet line
                audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_selectedScheduledWord.GraphicItemUnique,
                    (int)timePickerCurrentWord.Value.TotalMilliseconds, (int)timePickerCurrentWord.Value.TotalMilliseconds);
            }
        }


        private void audioSoundEditor1_WaveAnalyzerLineMoving(object sender, WaveAnalyzerLineMovingEventArgs e)
        {
            var name = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemNameGet(e.nUniqueID);

            if (e.bMovedByCode)
                return;

            if (name == "MyLine")
            {
                timePickerCurrentWord.Value = new TimeSpan(0, 0, 0, 0, e.nPosInMs);
            }
            else if (name.StartsWith("W|"))
            {
                //other anchor -- get its name W|12
                int index = int.Parse(audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemNameGet(e.nUniqueID).Split('|')[1]);

                var word = m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).SelectMany(sc => sc.Words).Where(w => w.Index == index).FirstOrDefault();

                if (word.NextWord != null &&
                    new TimeSpan(0, 0, 0, 0, e.nPosInMs) >= word.NextWord.StartTime.Subtract(new TimeSpan(0,0,0,0,500)))
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(e.nUniqueID, (int) word.NextWord.StartTime.TotalMilliseconds - 500, (int) word.NextWord.StartTime.TotalMilliseconds - 500);
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
                    word.StartTime = new TimeSpan(0, 0, 0, 0, e.nPosInMs);
                }
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
            if (audioSoundEditor1.GetPlaybackStatus() != enumPlaybackStatus.PLAYBACK_PLAYING) // in case anchor fixing
            {
                return;
            }

            int mm = audioSoundEditor1.GetPlaybackPosition();
            var position = new TimeSpan(0, 0, 0, 0, Math.Max(mm, 0)); ;

            LabelCurrentSchedulingTimer.Text = audioSoundEditor1.FromMsToFormattedTime((long)mm, true, true);// GetFormattedTime(e.nBeginPosInMs, true, true);

            //during playing check if the current position moved over to the next word
            if (m_chapter.Paragraphs != null && !m_selectedAnchor)
            {
                //in case slient part
                if (m_selectedSilentAnchor)
                {
                    if (position >= m_chapter.FirstWord.StartTime && m_chapter.FirstWord.StartTime > TimeSpan.Zero)
                    {
                        //set time picker
                        timePickerCurrentWord.Value = position;
                        richTextBox3.SelectionStart = m_chapter.FirstWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length + 1;
                    }
                }
                else
                {
                    //get next word
                    if (m_selectedScheduledWord!=null && 
                        m_selectedScheduledWord.NextWord != null
                        && position >= m_selectedScheduledWord.NextWord.StartTime
                        && m_selectedScheduledWord.NextWord.NextWord != null
                        && position < m_selectedScheduledWord.NextWord.NextWord.StartTime)
                    {
                        //set time picker
                        timePickerCurrentWord.Value = position;

                        richTextBox3.SelectionStart = m_selectedScheduledWord.NextWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length;
                    }
                    else
                    {

                    }
                }
            }
        }

        private void mnuRemoveSchedule_Click(object sender, EventArgs e)
        {
            if (m_chapter.Paragraphs != null)
            {
                if (MessageBox.Show("האם אתה בטוח להסיר את התזמונים לשיעור זה ? ", "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign) == System.Windows.Forms.DialogResult.Yes)
                {
                    Clip.Current.IsDirty = true;

                    var words = m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).SelectMany(w => w.Words);

                    foreach (var word in words)
                    {
                        word.StartTime = TimeSpan.Zero;
                        //word.Duration = TimeSpan.Zero;
                    }
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_uniqueLine, 0, 0);
            LabelCurrentSchedulingTimer.Text = audioSoundEditor1.FromMsToFormattedTime(0, true, true);// GetFormattedTime(e.nBeginPosInMs, true, true);
            //timePickerCurrentWord.Value = TimeSpan.Zero;
            richTextBox3.SelectionStart = 4;
            buttonStartDJPlay.Text = "התחל";
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

            if (name.StartsWith("W|"))
            {
                //other anchor -- get its name W|12
                int index = int.Parse(name.Split('|')[1]);

                var word = m_chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).SelectMany(sc => sc.Words).Where(w => w.Index == index).FirstOrDefault();

                if (e.nButton == enumMouseButtons.MOUSE_BTN_RIGHT)
                {
                    richTextBox3.SelectionStart = word.RealCharIndex + 2;
                }
                else
                {
                    richTextBox3.SelectionStart = word.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length + 1;
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
            else if (e.TabPageIndex == 3 &&
                audioSoundEditor1.GetSoundDuration() <= 0)
            {
                // ask the user if he wants to go on
                MessageBox.Show("יש לבצע הקלטה ותזמון לשיעור לפני פרסומו", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                e.Cancel = true;
            }
        }

        private void buttonStartRecOverwrite_Click(object sender, EventArgs e)
        {
            // create a fresh new recording session
            audioSoundRecorder1.SetRecordingMode(AudioSoundRecorder.enumRecordingModes.REC_MODE_OVERWRITE);

            // set the flag for "append" mode
            m_bRecOverwriteMode = true;
            TimeSpan startSelectionTime = TimeSpan.Parse(LabelSelectionBegin.Text);

            audioSoundRecorder1.SetOverwritePos((int)startSelectionTime.TotalMilliseconds);

            // start recording in memory from system default input device and input channel
            audioSoundRecorder1.StartFromDirectSoundDevice(0, -1, "");
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
                buttonStartRecNew.BackgroundImage = imageList1.Images[1];
                buttonStartRecNew.Refresh();
            }
            else
            {
                buttonStartRecNew.BackgroundImage = imageList1.Images[0];
                buttonStartRecNew.Refresh();
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
            var parDelimiters = comboBoxAutoDevidePar.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select( w => w.Trim()).ToList<string>();
            var senDelimiters = comboBoxAutoDevideSen.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim()).ToList<string>();

            if (parDelimiters.Count() == 0)
            {
                MessageBox.Show("יש לבחור לפחות אפשרות עוגן פסקה אחת", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            if (senDelimiters.Count() == 0)
            {
                MessageBox.Show("יש לבחור לפחות אפשרות עוגן משפט אחת", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

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
