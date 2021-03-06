﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using AudioSoundEditor;
using MyMentor.ParseObjects;
using MyMentor.Resources;
using Parse;
using Security;

namespace MyMentor.Forms
{
    public partial class FormStudio : Form
    {
        private MessageApplyInsertion m_messageInsertion = new MessageApplyInsertion();
        private string m_currentFingerPrint = string.Empty;
        private bool m_bRecAppendMode;
        private bool m_bInPublishProgress;
        private bool m_blSelectionHides = false;
        private bool m_bContinueRecordingAfterPlayback;
        private TimeSpan m_overwriteModeDeletePosition = TimeSpan.Zero;
        private IntPtr m_hWndVuMeterLeft1;
        private IntPtr m_hWndVuMeterRight1;
        private IntPtr m_hWndVuMeterLeft2;
        private IntPtr m_hWndVuMeterRight2;
        private TimeSpan n_hammerLastTimePressed = TimeSpan.Zero;
        private Word m_selectedScheduledWord = null;
        private short m_selectedGraphicItemSelected = -1;
        private short m_pauseOnAnchorGraphicItem = -1;
        private int m_lastAnchorIndexSelected = -1;
        private bool m_selectedStartAnchor = false;
        private bool m_selectedEndAnchor = false;
        private bool m_skipSelectionChange = false;
        private bool m_skipManuallyAnchorsValidation = false;
        private bool m_bClickedWaveAnalyzer = false;
        private int m_waveFormTabIndex = 1;
        private string m_initClip = string.Empty;
        private bool m_admin = false;
        private bool m_loadingParse = true;
        private bool m_whileLoadingClip = false;
        private Stopwatch m_stopWatch = new Stopwatch();
        private IEnumerable<KeyValuePair<string, string>> m_strings;
        private List<Currency> m_currencies;
        private string START_PAUSE_SECTION_ANCHOR;
        private string END_PAUSE_SECTION_ANCHOR;
        List<int> m_LastSelections = new List<int>();
        List<int> m_LastRangeSelections = new List<int>();
        private int m_intRecordingDuration = 0;
        private TimeSpan m_rem_anchorFixRecording = TimeSpan.Zero;

        public static Regex m_regexAll = new Regex(@"(\(\()|(\)\))|(\[\[)|(\]\])|({{)|(}})|(<<)|(>>)", RegexOptions.Compiled | RegexOptions.Multiline);

        private static Regex m_regexRemoveWhiteSpacesParagraphs = new Regex(@"(?<=\{\{)[ ]{1,}|[ ]{1,}(?=\}\})", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexRemoveWhiteSpacesSentences = new Regex(@"(?<=\(\()[ ]{1,}|[ ]{1,}(?=\)\))", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexRemoveWhiteSpacesSections = new Regex(@"(?<=\<\<)[ ]{1,}|[ ]{1,}(?=\>\>)", RegexOptions.Compiled | RegexOptions.Singleline);

        private static Regex m_regexParagraphs = new Regex(@"(.+?)(\[3\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexSentenses = new Regex(@"(.+?)(\[2\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexSections = new Regex(@"(.+?)(\[1\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);
        private static Regex m_regexWords = new Regex(@"(.+?)(\[0\]|$)", RegexOptions.Compiled | RegexOptions.Singleline);

        private bool b_recordIconRed;
        private Graphics rtbMainEditorGraphics;
        private Graphics rtbAlternateEditorGraphics;
        private ParseObject m_contentType;
        private MessageBoxOptions m_msgOptionsRtl;
        private short m_endLineUniqueId = -1;

        private int[] sizes = { 9, 10, 12, 16, 32 };

        public IEnumerable<KeyValuePair<string, string>> Strings
        {
            get { return m_strings; }
        }

        public ParseObject ContentType
        {
            get
            {
                return m_contentType;
            }
        }

        public FormStudio(string initClip)
        {
            CultureInfo newCulture = new CultureInfo(MyMentor.Properties.Settings.Default.CultureInfo);
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = newCulture;

            InitializeComponent();

            if (MyMentor.Properties.Settings.Default.CultureInfo == "he-il")
            {
                mnuTools_UI_Hebrew.Checked = true;
                mnuTools_UI_English.Checked = false;
                m_msgOptionsRtl = MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading;

            }
            else
            {
                mnuTools_UI_Hebrew.Checked = false;
                mnuTools_UI_English.Checked = true;
                m_msgOptionsRtl = 0;

                toolStrip2.Items.Remove(tsbRewind);
                toolStrip2.Items.Remove(tsbMoveStart);
                toolStrip2.Items.Remove(tsbForward);
                toolStrip2.Items.Remove(tsbMoveEnd);
                toolStrip2.Items.Remove(tsbPause);

                toolStrip2.Items.Insert(10, tsbMoveStart);
                toolStrip2.Items.Insert(11, tsbRewind);
                toolStrip2.Items.Insert(12, tsbPause);

                toolStrip2.Items.Insert(14, tsbForward);
                toolStrip2.Items.Insert(15, tsbMoveEnd);
            }

            m_initClip = initClip;

        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (tabControl1.SelectedIndex == 0 && richTextBox1.Focused)
                {
                    if (keyData == (Keys.Control | Keys.A))
                    {
                        textToolstripSelectAll_Click(null, new EventArgs());
                        return true;
                    }

                    if (keyData == (Keys.Control | Keys.C))
                    {
                        textToolstripCopy_Click(null, new EventArgs());
                        return true;
                    }

                    if (keyData == (Keys.Control | Keys.X))
                    {
                        textToolstripCut_Click(null, new EventArgs());
                        return true;
                    }

                    if (keyData == (Keys.Control | Keys.V))
                    {
                        textToolstripPaste_Click(null, new EventArgs());
                        return true;
                    }
                }
                else if (tabControl1.SelectedIndex == 1 || tabControl1.SelectedIndex == 2)
                {
                    if (keyData == (Keys.Delete))
                    {
                        mnuAudioSelectedPart_Delete_Click(null, new EventArgs());
                        return true;
                    }

                    if (keyData == (Keys.Control | Keys.A))
                    {
                        mnuAudioSelectedPart_SelectAll_Click(null, new EventArgs());
                        return true;
                    }

                    if (keyData == (Keys.Control | Keys.C))
                    {
                        mnuAudioSelectedPart_Copy_Click(null, new EventArgs());
                        return true;
                    }

                    if (keyData == (Keys.Control | Keys.X))
                    {
                        mnuAudioSelectedPart_Cut_Click(null, new EventArgs());
                        return true;
                    }

                    if (keyData == (Keys.Control | Keys.V))
                    {
                        mnuAudioSelectedPart_PasteInsertMode_Click(null, new EventArgs());
                        return true;
                    }
                }
            }
            catch
            {

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void HideShowWaveAnylazerSelection(bool hide)
        {
            if (m_selectedGraphicItemSelected > 0 && m_LastSelections.Count() > 0)
            {
                timerSkipAnalyzerSelection.Enabled = true;
                if (hide && !m_blSelectionHides)
                {
                    m_blSelectionHides = true;
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(false, 0, 0);
                }
                else if (!hide && m_blSelectionHides)
                {
                    m_blSelectionHides = false;
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, m_LastSelections[0], m_LastSelections[1]);
                }
            }
        }

        private void FormStudio_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;

            START_PAUSE_SECTION_ANCHOR = ResourceHelper.GetLabel("START_PAUSE_SECTION_ANCHOR");
            END_PAUSE_SECTION_ANCHOR = ResourceHelper.GetLabel("END_PAUSE_SECTION_ANCHOR");

            // init controls
            audioSoundRecorder1.InitRecordingSystem();
            audioSoundRecorder1.SetInputDeviceChannelVolume(0, 0, 100);

            audioSoundRecorder2.InitRecordingSystem();
            audioSoundRecorder2.SetInputDeviceChannelVolume(0, 0, 100);

            audioSoundEditor1.InitEditor();
            audioSoundEditor1.UndoEnable(true);
            ////dj
            audioDjStudio1.InitSoundSystem(1, 0, 0, 0, 0, -1);

            trackBarVolume1.Value = 100;

            // create the recorder's VU-Meter
            audioSoundRecorder1.DisplayVUMeter.Create(IntPtr.Zero);
            m_hWndVuMeterLeft1 = CreateVuMeter(label17, AudioSoundRecorder.enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL);
            m_hWndVuMeterRight1 = CreateVuMeter(label18, AudioSoundRecorder.enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL);

            // create the recorder's VU-Meter
            audioSoundRecorder2.DisplayVUMeter.Create(IntPtr.Zero);
            m_hWndVuMeterLeft2 = CreateVuMeter(label17, AudioSoundRecorder.enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL);
            m_hWndVuMeterRight2 = CreateVuMeter(label18, AudioSoundRecorder.enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL);

            // create the waveform analyzer (always call this function on the end of the form's Load fucntion)
            audioSoundEditor1.DisplayWaveformAnalyzer.Create(panel1.Handle, Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);

            // get the current analyzer wave settings
            AudioSoundEditor.WANALYZER_WAVEFORM_SETTINGS settingsWave = new WANALYZER_WAVEFORM_SETTINGS();
            audioSoundEditor1.DisplayWaveformAnalyzer.SettingsWaveGet(ref settingsWave);

            settingsWave.colorWaveSelBackground = Color.FromArgb(255, 240, 240, 240);
            settingsWave.nSelectionMode = enumWaveformSelectionMode.WAVE_SEL_TRANSPARENT_GLASS;
            settingsWave.colorTransparentGlass = Color.FromArgb(255, 255, 255, 255);
            settingsWave.nTransparentGlassFactor = 128;

            settingsWave.nTransparentGlassType = enumTranspGlassType.TRANSP_GLASS_TYPE_FLAT;
            settingsWave.nTransparentGlassPos3D = 50;
            settingsWave.bTransparentGlassInvert3D = true;

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

            TimerMenuEnabler.Enabled = true;
            tableLayoutPanel4.RowStyles[1].Height = 0;
            tableLayoutPanel4.RowStyles[2].Height = 0;

            rtbMainEditorGraphics = richTextBox1.CreateGraphics();
            rtbAlternateEditorGraphics = richTextBox3.CreateGraphics();

            //set second recorder
            audioSoundRecorder2.EncodeFormats = new AudioSoundRecorder.EncodeFormatsMan();
            audioSoundRecorder2.EncodeFormats.ForRecording = AudioSoundRecorder.enumEncodingFormats.ENCODING_FORMAT_NOFILE;
            AudioSoundRecorder.enumErrorCodes nResult = audioSoundRecorder2.StartFromDirectSoundDevice(0, 0, "");

            audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_MP3;
            audioSoundEditor1.EncodeFormats.MP3.EncodeMode = enumMp3EncodeModes.MP3_ENCODE_CBR;
            audioSoundEditor1.EncodeFormats.MP3.CBR = 128000;

            cbInterval.Text = "1.5";
            dtpReadingDate.MinDate = DateTime.Today;
            trackBarVolume1.Value = Properties.Settings.Default.VolumeLevel == 0 ? 100 : Properties.Settings.Default.VolumeLevel;
            trackBar1.Value = Properties.Settings.Default.RecordingLevel == 0 ? 100 : Properties.Settings.Default.RecordingLevel;
        }

        void ToolBarSettings()
        {
            tbrWord.Image = imageList2.Images[3];
        }

        void mnuItemRemoveAnchors_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
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

        private void TimerReload_Tick(object sender, EventArgs e)
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
                return;
            }

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

        private void TimerMenuEnabler_Tick(object sender, EventArgs e)
        {
            // check if there is a sound inside the system clipboard
            if (audioSoundEditor1.IsSoundAvailableInClipboard())
            {
                mnuAudioSelectedPart_Paste.Enabled = true;

                // check if there is already an available recorded sound
                if (audioSoundEditor1.GetSoundDuration() > 0)
                {
                    mnuAudioSelectedPart_Paste.Text = ResourceHelper.GetLabel("MNU_PASTE_APPEND_MODE");

                    mnuAudioSelectedPart_PasteInsertMode.Visible = true;
                }
                else
                {
                    mnuAudioSelectedPart_Paste.Text = ResourceHelper.GetLabel("MNU_PASTE");

                    mnuAudioSelectedPart_PasteInsertMode.Visible = false;
                }
            }
            else
            {
                mnuAudioSelectedPart_Paste.Enabled = false;
                mnuAudioSelectedPart_PasteInsertMode.Visible = false;
                mnuAudioSelectedPart_Paste.Text = ResourceHelper.GetLabel("MNU_PASTE");
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
                tsbContinueRecord.Enabled = nBeginSelectionInMs == nEndSelectionInMs;

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
                tsbStartRecordNew.Enabled = true;
                tsbContinueRecord.Enabled = false;
                tsbImportFile.Enabled = true;
                tsbSave.Enabled = true;

                // disable Select all
                mnuAudioSelectedPart_SelectAll.Enabled = false;

                // disable editing tools
                mnuAudioOptions_InsertSilent.Enabled = false;
                mnuAudioOptions_ApplyBackgroundSound.Enabled = false;

                // disable special effects
                mnuAudioEffects.Enabled = false;

                // disable zoom
                mnuZoom.Enabled = false;

                //disable buttons
                foreach (var btn in toolStrip2.Items)
                {
                    var tsb = (ToolStripItem)btn;

                    if (
                        (tsb.Tag != null && tsb.Name != "tsbImportFile" && tsb.Name != "tsbStartRecordNew")
                        )
                    {
                        tsb.Enabled = false;
                    }
                }
            }

            // RECORDING 
            if (audioSoundRecorder1.Status == AudioSoundRecorder.enumRecorderStates.RECORD_STATUS_RECORDING
                || audioSoundRecorder1.Status == AudioSoundRecorder.enumRecorderStates.RECORD_STATUS_RECORDING_CLIPBOARD)
            {
                //disable buttons
                foreach (var btn in toolStrip2.Items)
                {
                    var tsb = (ToolStripItem)btn;

                    if (
                        (tsb.Tag != null && tsb.Tag.ToString() == "I")
                        )
                    {
                        tsb.Enabled = false;
                    }
                }

                tsbPlay.Enabled = false;
                tsbPause.Enabled = false;

                if (m_bRecAppendMode && m_bContinueRecordingAfterPlayback)
                {
                    tsbStartRecordNew.Enabled = false;
                    tsbContinueRecord.Enabled = true;
                }
                else
                {
                    tsbContinueRecord.Enabled = false;
                    tsbStartRecordNew.Enabled = true;
                }
            }

                // NOT RECORDING - AND THERE IS CLIP
            else if (audioSoundEditor1.GetSoundDuration() > 0)
            {
                // IN PAUSE OR PLAY MODE
                if (audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PLAYING
                    || audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PAUSED)
                {
                    //disable buttons
                    foreach (var btn in toolStrip2.Items)
                    {
                        var tsb = (ToolStripItem)btn;

                        if (
                            (tsb.Tag != null && tsb.Tag.ToString() == "I" && tsb.Enabled)
                            )
                        {
                            tsb.Enabled = false;
                        }
                    }

                    tsbStartRecordNew.Enabled = false;
                    tsbMoveAnchorForward.Enabled = false;
                    tsbMoveAnchorRewind.Enabled = false;
                    tsbCheckAnchor.Enabled = false;
                    tsbHammer.Enabled = true;

                    if (m_bContinueRecordingAfterPlayback)
                    {
                        tsbContinueRecord.Enabled = true;
                        tsbPlay.Enabled = false;
                        tsbPause.Enabled = false;
                    }
                    else
                    {
                        tsbContinueRecord.Enabled = false;
                        tsbPlay.Enabled = true;
                        tsbPause.Enabled = true;
                    }
                }
                else
                {
                    // NOT PLAYING MODE 
                    //enable all buttons
                    foreach (var btn in toolStrip2.Items)
                    {
                        var tsb = (ToolStripItem)btn;
                        if (
                            (tsb.Tag != null && tsb.Tag.ToString() == "I" && !tsb.Enabled)
                            )
                        {
                            tsb.Enabled = true;
                        }
                    }

                    tsbHammer.Enabled = false;
                    tsbStartRecordNew.Enabled = true;
                    tsbPlay.Enabled = true;
                    tsbPause.Enabled = false;

                    if (m_selectedGraphicItemSelected >= 0 && !tsbMoveAnchorForward.Enabled)
                    {
                        tsbMoveAnchorForward.Enabled = true;
                        tsbMoveAnchorRewind.Enabled = true;
                        tsbCheckAnchor.Enabled = true;
                    }
                    else if (m_selectedGraphicItemSelected == -1 &&
                        tsbMoveAnchorForward.Enabled)
                    {
                        tsbMoveAnchorForward.Enabled = false;
                        tsbMoveAnchorRewind.Enabled = false;
                        tsbCheckAnchor.Enabled = false;
                    }
                }
            }
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

        private void mnuFile_NewClip_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (Clip.Current.IsDirty &&
                    MessageBox.Show(m_strings.Single(a => a.Key == "STD_CLIP_NOT_SAVED_CONFIRM").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl) == System.Windows.Forms.DialogResult.Yes)
                {
                    NewClip();
                }
            }
            else
            {
                NewClip();
            }

            tabControl1.SelectedIndex = 0;
        }

        private void NewClip()
        {
            Clip.Current = null;
            Clip.Current.AutoIncrementVersion = true;
            Clip.Current.Name = ResourceHelper.GetLabel("UNTITLED_CLIP");
            Clip.Current.Version = "1.00";
            Clip.Current.Status = "3DYQsyGZIk";
            Clip.Current.ID = Guid.NewGuid();
            Clip.Current.IsNew = true;
            Clip.Current.RightAlignment = true;
            Clip.Current.User = (string)comboUsers.SelectedValue;
            Clip.Current.FingerPrint = FingerPrint.Value(Clip.Current.User);
            Clip.Current.ClipType = "piL85bMGtR";
            Clip.Current.Expired = null;
            Clip.Current.ContentType = this.ContentType.ObjectId;
            Clip.Current.DefaultSections.paragraph = 1;
            Clip.Current.DefaultSections.sentence = 1;
            Clip.Current.DefaultSections.section = 1;
            Clip.Current.DefaultSections.chapter = 1;
            Clip.Current.DefaultLearningOptions.teacher1 = 1;
            Clip.Current.DefaultLearningOptions.teacherAndStudent = 1;
            Clip.Current.DefaultLearningOptions.teacher2 = 1;
            Clip.Current.DefaultLearningOptions.student = 1;
            
            buttonPublishUpdate.Enabled = false;

            tbrParagraph.Checked = false;
            tbrSection.Checked = false;
            tbrSentense.Checked = false;
            tbrWord.Checked = false;


            this.Text = "MyMentor - " + Clip.Current.Name;

            richTextBox1.Rtf = null;

            audioSoundRecorder1.RecordedSound.FreeMemory();
            audioSoundEditor1.CloseSound();
            TimerReload.Enabled = true;

            Clip.Current.Text = richTextBox1.Text;
            Clip.Current.Devide();

            m_whileLoadingClip = true;
            SetClipProperties();
            RegenerateClipName();
            m_whileLoadingClip = false;

            Clip.Current.IsDirty = false;
        }

        private void RegenerateDatesBox()
        {
            listBoxDates.Items.Clear();
            listBoxDates.Items.AddRange(Clip.Current.ReadingDates.Select(d => (object)d.ToString("dd/MM/yyyy")).ToArray());
        }

        private async void SetClipProperties()
        {
            tbClipRemarks.Text = Clip.Current.Remarks;
            tbClipRemarksEnglish.Text = Clip.Current.RemarksEnglish;
            tbClipDescription.Text = Clip.Current.Description;
            tbClipDescriptionEnglish.Text = Clip.Current.EnglishDescription;
            tbPerformer.Text = Clip.Current.Performer;
            tbPerformerEnglish.Text = Clip.Current.EnglishPerformer;
            comboVoicePrompt.SelectedValue = Clip.Current.VoicePrompts ?? string.Empty;
            comboCategory1.SelectedValue = Clip.Current.Category1 ?? string.Empty;
            mtbVersion.Text = Clip.Current.Version;
            comboCategory3.SelectedValue = Clip.Current.Category3 ?? string.Empty;
            comboCategory4.SelectedValue = Clip.Current.Category4 ?? string.Empty;
            comboClipType.SelectedValue = Clip.Current.ClipType ?? "piL85bMGtR";
            comboUsers.SelectedValue = Clip.Current.User;
            tbKeywords.Text = Clip.Current.Keywords;
            comboStatus.SelectedValue = Clip.Current.Status ?? "3DYQsyGZIk"; //paeel

            tbLastPublished.Text = Clip.Current.LastPublishedOn.HasValue ? Clip.Current.LastPublishedOn.Value.ToString("f") : "-";

            RegenerateDatesBox();

            numericPrice.Value = Clip.Current.Price;

            if (Clip.Current.PriceSupport.HasValue)
            {
                numericPriceSupport.Value = Clip.Current.PriceSupport.Value;
            }
            else
            {
                numericPriceSupport.ResetText();
            }

            def_par.Checked = Clip.Current.DefaultSections.paragraph == 1;
            def_sen.Checked = Clip.Current.DefaultSections.sentence == 1;
            def_sec.Checked = Clip.Current.DefaultSections.section == 1;
            def_wor.Checked = Clip.Current.DefaultSections.chapter == 1;

            loc_par.Checked = Clip.Current.LockedSections.paragraph == 1;
            loc_sen.Checked = Clip.Current.LockedSections.sentence == 1;
            loc_sec.Checked = Clip.Current.LockedSections.section == 1;
            loc_wor.Checked = Clip.Current.LockedSections.chapter == 1;

            sop_teacher1.Checked = Clip.Current.DefaultLearningOptions.teacher1 == 1;
            sop_teacherAndStudent.Checked = Clip.Current.DefaultLearningOptions.teacherAndStudent == 1;
            sop_teacher2.Checked = Clip.Current.DefaultLearningOptions.teacher2 == 1;
            sop_student.Checked = Clip.Current.DefaultLearningOptions.student == 1;

            sop_teacher1l.Checked = Clip.Current.LockedLearningOptions.teacher1 == 1;
            sop_teacherAndStudentl.Checked = Clip.Current.LockedLearningOptions.teacherAndStudent == 1;
            sop_teacher2l.Checked = Clip.Current.LockedLearningOptions.teacher2 == 1;
            sop_studentl.Checked = Clip.Current.LockedLearningOptions.student == 1;

            if (comboCategory1.SelectedValue != null)
            {
                try
                {
                    this.comboCategory2.DisplayMember = MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il" ? "HebrewValue" : "EnglishValue";
                    this.comboCategory2.ValueMember = "ObjectId";
                    this.comboCategory2.DataSource = (await ParseTables.GetCategory2((string)comboCategory1.SelectedValue)).Select(c => new Category
                    {
                        ObjectId = c.ObjectId,
                        HebrewValue = c.ContainsKey("value_he_il") ? c.Get<string>("value_he_il") : string.Empty,
                        EnglishValue = c.ContainsKey("value_en_us") ? c.Get<string>("value_en_us") : string.Empty

                    }).ToList(); ;

                    if (Clip.Current.Category2 != null)
                    {
                        m_whileLoadingClip = true;
                        comboCategory2.SelectedValue = Clip.Current.Category2 ?? string.Empty;
                        m_whileLoadingClip = false;
                    }
                }
                catch(Exception ex)
                {
                    Program.Logger.Error(ex);
                }
            }
            else
            {
                comboCategory2.SelectedValue = string.Empty;
            }

            if (comboCategory3.SelectedIndex >= 0 && ((Category)comboCategory3.SelectedItem).MinPrice > 0)
            {
                lblMinValue.Text = string.Format(ResourceHelper.GetLabel("MIN_PRICE") + " {0:C}", ((Category)comboCategory3.SelectedItem).MinPrice);
                lblMinValue.Visible = true;
            }
            else
            {
                lblMinValue.Visible = false;
            }

            if (string.IsNullOrEmpty(Clip.Current.ContentType))
            {
                Clip.Current.ContentType = this.ContentType.ObjectId;
            }

            comboClipType_SelectionChangeCommitted(null, new EventArgs());
        }

        private void RegenerateClipName()
        {
            RegenerateClipName(false);
        }

        private void RegenerateClipName(bool isDirty)
        {
            var hebrewPattern = (string)comboClipType.SelectedValue == "enaWrne5xe" ?
                m_contentType.Get<string>("sourceTitlePattern_he_il") :
                m_contentType.Get<string>("clipTitlePattern_he_il");

            var englishPattern = (string)comboClipType.SelectedValue == "enaWrne5xe" ?
                m_contentType.Get<string>("sourceTitlePattern_en_us") :
                m_contentType.Get<string>("clipTitlePattern_en_us");

            var portalHebrewPatternPart1 = m_contentType.Get<string>("clipTitlePatternPart1_he_il");
            var portalHebrewPatternPart2 = m_contentType.Get<string>("clipTitlePatternPart2_he_il");

            var portalEnglishPatternPart1 = m_contentType.Get<string>("clipTitlePatternPart1_en_us");
            var portalEnglishPatternPart2 = m_contentType.Get<string>("clipTitlePatternPart2_en_us");

            var clipHebrewTitle = hebrewPattern.SpecialReplace("[category1]", string.Concat("_", comboCategory1.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[category2]", string.Concat("_", comboCategory2.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[category3]", string.Concat("_", comboCategory3.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[category4]", string.Concat("_", comboCategory4.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[description]", string.Concat("_", Clip.Current.Description, "_"))
                .SpecialReplace("[remarks]", string.Concat("_", Clip.Current.Remarks, "_"))
                .SpecialReplace("[firstName]", ParseTables.CurrentUser.ContainsKey("firstName_he_il") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("firstName_he_il"), "_") : string.Empty)
                .SpecialReplace("[lastName]", ParseTables.CurrentUser.ContainsKey("lastName_he_il") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("lastName_he_il"), "_") : string.Empty)
                .SpecialReplace("[cityOfResidence]", ParseTables.CurrentUser.ContainsKey("cityOfResidence_he_il") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("cityOfResidence_he_il"), "_") : string.Empty);

            var portalHebrewTitlePart1 = portalHebrewPatternPart1.SpecialReplace("[category1]", string.Concat("_", comboCategory1.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[category2]", string.Concat("_", comboCategory2.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[category3]", string.Concat("_", comboCategory3.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[category4]", string.Concat("_", comboCategory4.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[description]", string.Concat("_", Clip.Current.Description, "_"))
                .SpecialReplace("[remarks]", string.Concat("_", Clip.Current.Remarks, "_"))
                .SpecialReplace("[firstName]", ParseTables.CurrentUser.ContainsKey("firstName_he_il") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("firstName_he_il"), "_") : string.Empty)
                .SpecialReplace("[lastName]", ParseTables.CurrentUser.ContainsKey("lastName_he_il") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("lastName_he_il"), "_") : string.Empty)
                .SpecialReplace("[cityOfResidence]", ParseTables.CurrentUser.ContainsKey("cityOfResidence_he_il") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("cityOfResidence_he_il"), "_") : string.Empty);

            var portalHebrewTitlePart2 = portalHebrewPatternPart2.SpecialReplace("[category1]", string.Concat("_", comboCategory1.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[category2]", string.Concat("_", comboCategory2.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[category3]", string.Concat("_", comboCategory3.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[category4]", string.Concat("_", comboCategory4.GetHebrewPlaceholderText(), "_"))
                .SpecialReplace("[description]", string.Concat("_", Clip.Current.Description, "_"))
                .SpecialReplace("[remarks]", string.Concat("_", Clip.Current.Remarks, "_"))
                .SpecialReplace("[firstName]", ParseTables.CurrentUser.ContainsKey("firstName_he_il") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("firstName_he_il"), "_") : string.Empty)
                .SpecialReplace("[lastName]", ParseTables.CurrentUser.ContainsKey("lastName_he_il") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("lastName_he_il"), "_") : string.Empty)
                .SpecialReplace("[cityOfResidence]", ParseTables.CurrentUser.ContainsKey("cityOfResidence_he_il") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("cityOfResidence_he_il"), "_") : string.Empty);

            var clipEnglishTitle = englishPattern.SpecialReplace("[category1]", string.Concat("_", comboCategory1.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[category2]", string.Concat("_", comboCategory2.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[category3]", string.Concat("_", comboCategory3.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[category4]", string.Concat("_", comboCategory4.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[description]", string.Concat("_", Clip.Current.EnglishDescription, "_"))
                .SpecialReplace("[remarks]", string.Concat("_", Clip.Current.RemarksEnglish, "_"))
                .SpecialReplace("[firstName]", ParseTables.CurrentUser.ContainsKey("firstName_en_us") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("firstName_en_us"), "_") : string.Empty)
                .SpecialReplace("[lastName]", ParseTables.CurrentUser.ContainsKey("lastName_en_us") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("lastName_en_us"), "_") : string.Empty)
                .SpecialReplace("[cityOfResidence]", ParseTables.CurrentUser.ContainsKey("cityOfResidence_en_us") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("cityOfResidence_en_us"), "_") : string.Empty);

            bool englishRemarkInHebrewTextBox = false;

            if ((!string.IsNullOrEmpty(Clip.Current.Remarks) && Regex.IsMatch(Clip.Current.Remarks, "^[a-zA-Z0-9]"))
                    || (!string.IsNullOrEmpty(Clip.Current.Description) && Regex.IsMatch(Clip.Current.Description, "^[a-zA-Z0-9]")))
            {
                englishRemarkInHebrewTextBox = true;
            }

            var portalEnglishTitlePart1 = portalEnglishPatternPart1.SpecialReplace("[category1]", string.Concat("_", comboCategory1.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[category2]", string.Concat("_", comboCategory2.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[category3]", string.Concat("_", comboCategory3.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[category4]", string.Concat("_", comboCategory4.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[description]", string.Concat("_", Clip.Current.EnglishDescription, "_"))
                .SpecialReplace("[remarks]", string.Concat("_", englishRemarkInHebrewTextBox ? Clip.Current.Remarks : Clip.Current.RemarksEnglish, "_"))
                .SpecialReplace("[firstName]", ParseTables.CurrentUser.ContainsKey("firstName_en_us") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("firstName_en_us"), "_") : string.Empty)
                .SpecialReplace("[lastName]", ParseTables.CurrentUser.ContainsKey("lastName_en_us") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("lastName_en_us"), "_") : string.Empty)
                .SpecialReplace("[cityOfResidence]", ParseTables.CurrentUser.ContainsKey("cityOfResidence_en_us") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("cityOfResidence_en_us"), "_") : string.Empty);

            var portalEnglishTitlePart2 = portalEnglishPatternPart2.SpecialReplace("[category1]", string.Concat("_", comboCategory1.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[category2]", string.Concat("_", comboCategory2.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[category3]", string.Concat("_", comboCategory3.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[category4]", string.Concat("_", comboCategory4.GetEnglishPlaceholderText(), "_"))
                .SpecialReplace("[description]", string.Concat("_", Clip.Current.EnglishDescription, "_"))
                .SpecialReplace("[remarks]", string.Concat("_", englishRemarkInHebrewTextBox ? Clip.Current.Remarks : Clip.Current.RemarksEnglish, "_"))
                .SpecialReplace("[firstName]", ParseTables.CurrentUser.ContainsKey("firstName_en_us") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("firstName_en_us"), "_") : string.Empty)
                .SpecialReplace("[lastName]", ParseTables.CurrentUser.ContainsKey("lastName_en_us") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("lastName_en_us"), "_") : string.Empty)
                .SpecialReplace("[cityOfResidence]", ParseTables.CurrentUser.ContainsKey("cityOfResidence_en_us") ? string.Concat("_", ParseTables.CurrentUser.Get<string>("cityOfResidence_en_us"), "_") : string.Empty);

            clipHebrewTitle = clipHebrewTitle.Replace("_", "");
            clipEnglishTitle = clipEnglishTitle.Replace("_", "");
            portalHebrewTitlePart1 = portalHebrewTitlePart1.Replace("_", "");
            portalHebrewTitlePart2 = portalHebrewTitlePart2.Replace("_", "");
            portalEnglishTitlePart1 = portalEnglishTitlePart1.Replace("_", "");
            portalEnglishTitlePart2 = portalEnglishTitlePart2.Replace("_", "");



            if (MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il")
            {
                if (englishRemarkInHebrewTextBox)
                {
                    this.tbClipTitle.Text = clipEnglishTitle;
                }
                else
                {
                    this.tbClipTitle.Text = clipHebrewTitle;
                }
            }
            else
            {
                this.tbClipTitle.Text = clipEnglishTitle;
            }

            Clip.Current.HebrewTitle = clipHebrewTitle;
            Clip.Current.EnglishTitle = clipEnglishTitle;

            Clip.Current.PortalHebrewTitlePart1 = portalHebrewTitlePart1;
            Clip.Current.PortalHebrewTitlePart2 = portalHebrewTitlePart2;
            Clip.Current.PortalEnglishTitlePart1 = portalEnglishTitlePart1;
            Clip.Current.PortalEnglishTitlePart2 = portalEnglishTitlePart2;
        }

        private void OpenClip()
        {
            OpenClip(null);
        }

        private void OpenClip(string file)
        {
            m_whileLoadingClip = true;

            audioSoundRecorder1.RecordedSound.FreeMemory();
            audioSoundEditor1.CloseSound();
            audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();

            tbrParagraph.Checked = false;
            tbrSection.Checked = false;
            tbrSentense.Checked = false;
            tbrWord.Checked = false;

            TimerReload.Enabled = true;

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

                if (Clip.Current.FingerPrint != m_currentFingerPrint && !m_admin)
                {
                    NewClip();
                    MessageBox.Show(ResourceHelper.GetLabel("FINGERPRINT_ERROR"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }
            }
            catch (ApplicationException ex)
            {
                Program.Logger.Error(ex);

                MessageBox.Show(ex.Message, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                return;
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);

                MessageBox.Show(ResourceHelper.GetLabel("CLIP_FILE_FORMAT_ERROR"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                return;
            }

            this.Text = "MyMentor - " + file;

            //set properties
            richTextBox1.Rtf = Clip.Current.RtfText;

            SetClipProperties();

            tabControl1.SelectedIndex = 0;
            mnuFile_SaveAs.Enabled = true;

            Clip.Current.Devide();

            if (File.Exists(Path.ChangeExtension(file, ".mp3")))
            {
                enumErrorCodes error = audioSoundEditor1.LoadSound(Path.ChangeExtension(file, ".mp3"));

                if (error != enumErrorCodes.ERR_NOERROR)
                {
                    MessageBox.Show(string.Format("{0}", error.ToString()), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                }
            }

            RegenerateClipName();

            m_whileLoadingClip = false;
            Clip.Current.IsDirty = false;
            Clip.Current.IsNew = false;

            buttonPublishUpdate.Enabled = true;

            if (Clip.Current.ContentType != this.ContentType.ObjectId)
            {
                Clip.Current.ContentType = this.ContentType.ObjectId;
                MessageBox.Show(m_strings.Single(a => a.Key == "STD_CONTENT_TYPE_NOT_MATCH").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
            }

        }

        private string GetFileDialog()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyMentor Clips"));

            if (!di.Exists)
            {
                di.Create();
            }

            openFileDialog1.InitialDirectory = di.FullName;
            openFileDialog1.DefaultExt = "mmnt";
            openFileDialog1.Filter = "MyMentor Source Files|*.mmnt";
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

        private void FormStudio_Paint(object sender, PaintEventArgs e)
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
                index = richTextBox2.Find("[0]", index + 2, RichTextBoxFinds.None);

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
            int factor4 = 0;

            var positionMainEditor = richTextBox1.GetPositionFromCharIndex(index);
            richTextBox2.SelectionStart = index;// -3;
            richTextBox2.SelectionLength = 3;

            int width = Convert.ToInt32(rtbMainEditorGraphics.MeasureString(number, richTextBox2.SelectionFont).Width);

            factor1 = (int)(richTextBox2.SelectionFont.Size * 0.98222);
            factor2 = (int)(richTextBox2.SelectionFont.Size * -0.48888);
            factor3 = (int)(richTextBox2.SelectionFont.Size * 0.78555);
            factor4 = (int)(richTextBox2.SelectionFont.Size * -0.19855);

            if (richTextBox1.SelectionFont == null)
                return;

            // rectangle to specify which region to paint too
            Rectangle r1 = new Rectangle();

            // specify dimensions
            r1.X = positionMainEditor.X + (richTextBox1.SelectionAlignment == HorizontalAlignment.Right ? factor1 - width : 0);
            r1.Y = positionMainEditor.Y;
            r1.Width = width + factor2;
            r1.Height =
                Convert.ToInt32(richTextBox1.SelectionFont.Height * richTextBox2.ZoomFactor);

            rtbMainEditorGraphics.DrawRectangle(cp, r1);
            rtbMainEditorGraphics.FillRectangle(cb, r1);

            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(type == AnchorType.Word ? System.Drawing.Color.Black : System.Drawing.Color.White);
            float x = positionMainEditor.X + (richTextBox1.SelectionAlignment == HorizontalAlignment.Right ? factor3 - width : factor4);// (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes ? factor3 - width : 0);
            float y = positionMainEditor.Y;
            System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
            rtbMainEditorGraphics.DrawString(number, richTextBox2.SelectionFont, drawBrush, x, y, drawFormat);

            if (tabControl1.SelectedIndex == 2)
            {
                var positionAlternateEditor = richTextBox3.GetPositionFromCharIndex(index + START_PAUSE_SECTION_ANCHOR.Length);
                r1.X = positionAlternateEditor.X + (richTextBox1.SelectionAlignment == HorizontalAlignment.Right ? factor1 - width : 0);
                r1.Y = positionAlternateEditor.Y;

                //anchor
                if (index + START_PAUSE_SECTION_ANCHOR.Length == richTextBox3.SelectionStart)
                {
                    cb = new SolidBrush(SystemColors.Highlight);
                    drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
                }

                rtbAlternateEditorGraphics.DrawRectangle(cp, r1);
                rtbAlternateEditorGraphics.FillRectangle(cb, r1);

                x = positionAlternateEditor.X + (richTextBox1.SelectionAlignment == HorizontalAlignment.Right ? factor3 - width : 0);
                y = positionAlternateEditor.Y;
                drawFormat = new System.Drawing.StringFormat();
                rtbAlternateEditorGraphics.DrawString(number, richTextBox2.SelectionFont, drawBrush, x, y, drawFormat);
            }
        }


        private async void FormStudio_Shown(object sender, EventArgs e)
        {
            this.Enabled = false;
            m_whileLoadingClip = true;

            if (ParseUser.CurrentUser == null)
            {
                FormLogin frmLogin = new FormLogin();

                var result = frmLogin.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.Cancel)
                {
                    Program.Logger.Info("User cancel login");
                    Application.Exit();
                }
                else
                {
                    if (ParseUser.CurrentUser != null)
                    {
                        lblLoginUser.Text = ResourceHelper.GetLabel("LOGIN_AS") + ParseUser.CurrentUser.Username;
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
            }
            else
            {
                lblLoginUser.Text = ResourceHelper.GetLabel("LOGIN_AS") + ParseUser.CurrentUser.Username;
            }

            FormPleaseWait pleaseWaitFrm = new FormPleaseWait();
            pleaseWaitFrm.Show();
            Application.DoEvents();

            ParseObject contentType = null;

            try
            {
                contentType = await ParseTables.GetContentType();
            }
            catch(Exception ex)
            {
                ParseUser.LogOut();
                Program.Logger.Error(ex);
                MessageBox.Show(ResourceHelper.GetLabel("INVALID_USER") , "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                Application.Exit();
                return;
            }

            pleaseWaitFrm.Progress = 5;

            m_contentType = contentType;

            if (MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il")
            {
                lblWord.Text = string.Concat("עולם:", m_contentType.Get<string>("value_he_il"));
            }
            else
            {
                lblWord.Text = string.Concat("World:", m_contentType.Get<string>("value_en_us"));
            }

            bool readingDates = false;

            if (contentType.ContainsKey("readingDates"))
            {
                readingDates = contentType.Get<bool>("readingDates");
            }

            if (!readingDates)
            {
                dtpReadingDate.Visible = false;
                listBoxDates.Visible = false;
                lblReadingDates.Visible = false;
                lblAsterikReadingDates.Visible = false;
                btnAddDate.Visible = false;
                btnRemoveDate.Visible = false;
            }
            

            string he_il_value_key = "value_he_il";
            string en_us_value_key = "value_en_us";
            string status_key = "status_" + MyMentor.Properties.Settings.Default.CultureInfo.Replace("-", "_");

            try
            {
                m_strings = await ParseTables.GetStrings();

                m_currencies = (await ParseTables.GetCurrencies()).Select(c => new Currency
                {
                    ObjectId = c.ObjectId,
                    Symbol = c.Get<string>("currencySymbol")
                }).ToList();

                pleaseWaitFrm.Progress = 10;

                if (ParseTables.CurrentUser.ContainsKey("lessonCostCurrency"))
                {
                    var costCurrencyId = ParseTables.CurrentUser.Get<ParseObject>("lessonCostCurrency").ObjectId;

                    if (!string.IsNullOrEmpty(costCurrencyId))
                    {
                        lblPriceSupportCurrency.Text = m_currencies.FirstOrDefault(c => c.ObjectId == costCurrencyId).Symbol;
                    }
                }

                this.comboVoicePrompt.DisplayMember = "HebrewValue";
                this.comboVoicePrompt.ValueMember = "ObjectId";
                this.comboVoicePrompt.DataSource = (await ParseTables.GetVoicePrompts()).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    HebrewValue = c.ContainsKey("VoiceType") ? c.Get<string>("VoiceType") : string.Empty
                }).ToList();

                this.comboCategory1.DisplayMember = MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il" ? "HebrewValue" : "EnglishValue";
                this.comboCategory1.ValueMember = "ObjectId";
                this.comboCategory1.DataSource = (await ParseTables.GetCategory1(contentType.ObjectId)).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    HebrewValue = c.ContainsKey(he_il_value_key) ? c.Get<string>(he_il_value_key) : string.Empty,
                    EnglishValue = c.ContainsKey(en_us_value_key) ? c.Get<string>(en_us_value_key) : string.Empty
                }).ToList();

                pleaseWaitFrm.Progress = 15;

                this.comboUsers.DisplayMember = "EnglishValue";
                this.comboUsers.ValueMember = "ObjectId";
                this.comboUsers.DataSource = (await ParseTables.GetUsers()).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    EnglishValue = c.Get<string>("username")
                }).ToList();

                this.comboUsers.SelectedValue = ParseUser.CurrentUser.ObjectId;
                Clip.Current.User = ParseUser.CurrentUser.ObjectId;
                pleaseWaitFrm.Progress = 20;

                this.comboCategory3.DisplayMember = MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il" ? "HebrewValue" : "EnglishValue";
                this.comboCategory3.ValueMember = "ObjectId";
                this.comboCategory3.DataSource = (await ParseTables.GetCategory3(contentType.ObjectId, "HPz65WBzhw")).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    HebrewValue = c.ContainsKey(he_il_value_key) ? c.Get<string>(he_il_value_key) : string.Empty,
                    EnglishValue = c.ContainsKey(en_us_value_key) ? c.Get<string>(en_us_value_key) : string.Empty,
                    MinPrice = (decimal)c.Get<float>("minPrice")
                }).ToList();

                pleaseWaitFrm.Progress = 30;

                this.comboCategory4.DisplayMember = MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il" ? "HebrewValue" : "EnglishValue";
                this.comboCategory4.ValueMember = "ObjectId";
                this.comboCategory4.DataSource = (await ParseTables.GetCategory4(contentType.ObjectId)).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    HebrewValue = c.ContainsKey(he_il_value_key) ? c.Get<string>(he_il_value_key) : string.Empty,
                    EnglishValue = c.ContainsKey(en_us_value_key) ? c.Get<string>(en_us_value_key) : string.Empty
                }).ToList();

                pleaseWaitFrm.Progress = 45;

                this.comboStatus.DisplayMember = MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il" ? "HebrewValue" : "EnglishValue";
                this.comboStatus.ValueMember = "ObjectId";
                this.comboStatus.DataSource = (await ParseTables.GetStatuses()).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    HebrewValue = c.Get<string>(status_key)
                }).ToList();

                pleaseWaitFrm.Progress = 70;

                await ParseTables.GetTypes().ContinueWith((t) =>
                {

                    this.comboClipType.DisplayMember = MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il" ? "HebrewValue" : "EnglishValue";
                    this.comboClipType.ValueMember = "ObjectId";

                    this.comboClipType.DataSource = t.Result.Select(c => new Category
                    {
                        ObjectId = c.ObjectId,
                        HebrewValue = c.ContainsKey(he_il_value_key) ? c.Get<string>(he_il_value_key) : string.Empty,
                        EnglishValue = c.ContainsKey(en_us_value_key) ? c.Get<string>(en_us_value_key) : string.Empty
                    }).ToList();

                    Clip.Current.IsDirty = false;
                });

                pleaseWaitFrm.Progress = 90;

                ParseObject labels = await ParseTables.GetCategoryLabels(contentType.ObjectId);

                if (labels.ContainsKey("category1"))
                {
                    lblCategory1.Text = labels.Get<string>("category1");
                }
                else
                {
                    lblCategory1.Text = "";
                }

                if (labels.ContainsKey("category2"))
                {
                    lblCategory2.Text = labels.Get<string>("category2");
                }
                else
                {
                    lblCategory2.Text = "";
                }

                if (labels.ContainsKey("category3"))
                {
                    lblCategory3.Text = labels.Get<string>("category3");
                }
                else
                {
                    lblCategory3.Text = "";
                }

                if (labels.ContainsKey("category4"))
                {
                    lblCategory4.Text = labels.Get<string>("category4");
                }
                else
                {
                    lblCategory4.Text = "";
                }

            }
            catch (Exception ex)
            {
                ParseUser.LogOut();
                Program.Logger.Error(ex);
                MessageBox.Show(string.Concat(ResourceHelper.GetLabel("UNKNOWN_ERROR") , "\n\n" , ex.Message), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                Application.Exit();
            }
            finally
            {
                if ( string.IsNullOrEmpty(lblCategory1.Text))
                {
                    comboCategory1.Visible = false;
                    lblCategory1.Visible = false;
                    lblAsterikCategory1.Visible = false;
                }

                if (string.IsNullOrEmpty(lblCategory2.Text))
                {
                    comboCategory2.Visible = false;
                    lblCategory2.Visible = false;
                    lblAsterikCategory2.Visible = false;
                }

                if (string.IsNullOrEmpty(lblCategory3.Text))
                {
                    comboCategory3.Visible = false;
                    lblCategory3.Visible = false;
                    lblAsterikCategory3.Visible = false;
                }

                if (string.IsNullOrEmpty(lblCategory4.Text))
                {
                    comboCategory4.Visible = false;
                    lblCategory4.Visible = false;
                    lblAsterikCategory4.Visible = false;
                }

            }

            m_loadingParse = false;

            SetClipProperties();
            RegenerateClipName();

            //check if admin - show extra stuff
            var role = await ParseRole.Query.GetAsync("Au3zBr8rLy");
            var relation = await role.GetRelation<ParseUser>("users").Query.FindAsync();
            var user = relation.FirstOrDefault(usr => usr.ObjectId.Equals(ParseUser.CurrentUser.ObjectId));
            if (user != null)
            {
                m_admin = true;

                comboClipType.Visible = true;
                lblClipType.Visible = true;
                comboUsers.Visible = true;
                lblUsers.Visible = true;
            }
            else
            {
                showDebugDetailToolStripMenuItem.Visible = false;
            }

            pleaseWaitFrm.Progress = 100;

            await Task.Factory.StartNew(() =>
            {
                m_currentFingerPrint = FingerPrint.Value(ParseUser.CurrentUser.ObjectId);

                Debug.WriteLine(m_currentFingerPrint);

                Clip.Current.FingerPrint = m_currentFingerPrint;
            });

            pleaseWaitFrm.Close();
            this.Enabled = true;
            richTextBox1.Focus();


            if (!string.IsNullOrEmpty(m_initClip))
            {
                OpenClip(m_initClip);
            }
            else
            {
                NewClip();
            }


            m_whileLoadingClip = false;
            audioSoundEditor1.DisplayWaveformAnalyzer.Move(Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);

            //test recorder
            if (MyMentor.Properties.Settings.Default.TestSound)
            {
                FormTestSound frmTest = new FormTestSound((short)trackBarVolume1.Value, (short)trackBar1.Value);

                frmTest.ShowDialog();

                trackBar1.Value = frmTest.RecordVolume;
                trackBarVolume1.Value = frmTest.PlayerVolume;
            }
        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            PaintGraphics();

            if (!m_loadingParse)
            {
                Clip.Current.IsDirty = true;
            }

            if (Clip.Current.Chapter != null && Clip.Current.Chapter.Words.Count() > 0
                && !m_skipManuallyAnchorsValidation && tabControl1.SelectedIndex == 0 )
            {
                Regex regexAll = new Regex(@"(.+?)(\[[0123]\]|$)", RegexOptions.Singleline);
                List<SectionMatch> matches = regexAll.Matches(richTextBox1.Text.TrimEnd('\r', '\n')).Cast<Match>()
                            .Select(m => m.Groups[1])
                            .Select(m => new SectionMatch()
                            {
                                CharIndex = m.Index,
                                Length = m.Length,
                                Value = m.Value
                            })
                            .ToList();

                label1.Text = string.Concat(Clip.Current.Chapter.Words.Count().ToString(), "-", matches.Count().ToString());

                if (Clip.Current.Chapter.Words.Count() != matches.Count() &&
                    //for cases there is no anchors
                    (matches.Count() > 0 && Clip.Current.Chapter.Words.Count() > 1))
                {
                    richTextBox1.Rtf = Clip.Current.RtfText;

                    timerShowManuallyAnchorsValidationMessage.Enabled = true;
                    return;
                }
            }

            Clip.Current.RtfText = richTextBox1.Rtf;
            Clip.Current.Text = richTextBox1.Text;

        }

        private void richTextBox1_VScroll(object sender, EventArgs e)
        {
            PaintGraphics();

        }

        private void mnuFile_Open_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (MessageBox.Show(m_strings.Single(a => a.Key == "STD_CLIP_NOT_SAVED_CONFIRM").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl) == System.Windows.Forms.DialogResult.Yes)
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

        private void mnuFile_Save_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void mnuFile_SaveAs_Click(object sender, EventArgs e)
        {
            Save(true);
        }

        private void Save()
        {
            Save(false);
        }

        private void Save(bool isSaveAs)
        {
            Save(isSaveAs, false, null);
        }

        private void Save(bool isSaveAs, bool excludeClipFile)
        {
            Save(isSaveAs, excludeClipFile, null);
        }

        private void Save(bool isSaveAs, bool excludeClipFile, string suggestName)
        {
            SavePropertiesToClip();

            if (Clip.Current.IsNew || (!Clip.Current.IsNew && isSaveAs))
            {
                if (isSaveAs)
                {
                    FileInfo fi = new FileInfo(Clip.Current.FileName);
                    saveFileDialog1.InitialDirectory = fi.DirectoryName;

                    if (!string.IsNullOrEmpty(suggestName))
                    {
                        saveFileDialog1.FileName = suggestName;
                    }
                    else
                    {
                        saveFileDialog1.FileName = fi.Name;//  .FullName;
                    }
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "MyMentor Clips"));

                    if (!di.Exists)
                    {
                        di.Create();
                    }

                    saveFileDialog1.InitialDirectory = di.FullName;

                    string name = MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il" ? Clip.Current.HebrewTitle : Clip.Current.EnglishTitle;

                                //clip type makor
                    if ((string)comboClipType.SelectedValue != "piL85bMGtR")
                    {
                        name = string.Concat(MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il" ? "מקור " : "Source ", name);
                    }
                   
                    foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    {
                        name = name.Replace(c, '_');
                    }

                    saveFileDialog1.FileName = name;
                }
                saveFileDialog1.DefaultExt = "mmnt";
                saveFileDialog1.Filter = "MyMentor Source Files|*.mmnt";

                DialogResult result = saveFileDialog1.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    string oldFileName = string.Empty;

                    if (isSaveAs)
                    {
                        //Clip.Current.ID = Guid.NewGuid();
                        oldFileName = Clip.Current.FileName;
                    }
                    //Clip.Current.FontName = richTextBox1.Font.Name;
                    //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));

                    Clip.Current.Name = saveFileDialog1.FileName;
                    Clip.Current.FileName = saveFileDialog1.FileName;
                    Clip.Current.RtfText = richTextBox1.Rtf;
                    this.Text = "MyMentor - " + Clip.Current.Name;

                    if (!string.IsNullOrEmpty(oldFileName) &&
                        oldFileName != Clip.Current.FileName)
                    {
                        try
                        {
                            FileInfo oldFile = new FileInfo(oldFileName);

                            foreach (FileInfo f in new DirectoryInfo(oldFile.DirectoryName).GetFiles(string.Concat(Path.GetFileNameWithoutExtension(oldFile.Name), ".*")))
                            {
                                if (f.Extension.ToLower() == ".mmnt")
                                {
                                    //f.Delete();
                                }
                                else if (f.Extension.ToLower() == ".mp3")
                                {
                                    FileInfo newFile = new FileInfo(saveFileDialog1.FileName);
                                    f.CopyTo(Path.Combine( newFile.DirectoryName, Path.GetFileNameWithoutExtension(newFile.Name)) + ".mp3");
                                    //f.Delete();
                                }
                            }
                        }
                        catch(Exception ex)
                        {
                            Program.Logger.Error(ex);
                        }
                    }

                    if (excludeClipFile)
                    {
                        Clip.Current.Save(null);
                    }
                    else
                    {
                        Clip.Current.Save(audioSoundEditor1);
                    }

                    if (audioSoundEditor1.GetSoundDuration() <= 0 && !excludeClipFile)
                    {
                        MessageBox.Show(ResourceHelper.GetLabel("CLIP_SAVED_SUCCESSFULLY"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
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

                Clip.Current.Save(!excludeClipFile ? audioSoundEditor1 : null);

                if (audioSoundEditor1.GetSoundDuration() <= 0 && !excludeClipFile)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("CLIP_SAVED_SUCCESSFULLY"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    mnuFile_SaveAs.Enabled = true;
                    Clip.Current.Saved = true;
                }
                else
                {
                    Cursor.Current = Cursors.WaitCursor;
                }
            }
        }

        private void SavePropertiesToClip()
        {
            //set properties
            Clip.Current.HebrewTitle = tbClipTitle.Text;
            Clip.Current.Description = tbClipDescription.Text;
            Clip.Current.EnglishDescription = tbClipDescriptionEnglish.Text;
            Clip.Current.Performer = tbPerformer.Text;
            Clip.Current.EnglishPerformer = tbPerformerEnglish.Text;
            Clip.Current.Remarks = tbClipRemarks.Text;
            Clip.Current.RemarksEnglish = tbClipRemarksEnglish.Text;
            Clip.Current.Status = (string)comboStatus.SelectedValue;
            Clip.Current.VoicePrompts = (string)comboVoicePrompt.SelectedValue;
            Clip.Current.Category1 = (string)comboCategory1.SelectedValue;
            Clip.Current.Category2 = (string)comboCategory2.SelectedValue;
            Clip.Current.Category3 = (string)comboCategory3.SelectedValue;
            Clip.Current.Category4 = (string)comboCategory4.SelectedValue;
            Clip.Current.Keywords = tbKeywords.Text;
            Clip.Current.ClipType = comboClipType.SelectedValue != null ? (string)comboClipType.SelectedValue : "piL85bMGtR";
            Clip.Current.User = (string)comboUsers.SelectedValue;
            Clip.Current.FingerPrint = FingerPrint.Value(Clip.Current.User);
            Clip.Current.Price = numericPrice.Value;

            if (numericPriceSupport.Value > 0)
            {
                Clip.Current.PriceSupport = numericPriceSupport.Value;
            }
            else
            {
                Clip.Current.PriceSupport = null;
            }

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

        private void mnuFile_Exit_Click(object sender, EventArgs e)
        {
            if (Clip.Current.IsDirty)
            {
                if (MessageBox.Show(ResourceHelper.GetLabel("EXIT_CONFIRM"), "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl) == System.Windows.Forms.DialogResult.Yes)
                {
                    Application.Exit();
                }
            }
            else
            {
                Application.Exit();
            }
        }

        private void mnuTools_UI_Hebrew_Click(object sender, EventArgs e)
        {
            MyMentor.Properties.Settings.Default.CultureInfo = "he-il";
            MyMentor.Properties.Settings.Default.Save();
            MessageBox.Show(ResourceHelper.GetLabel("HEBREW_CHANGE_LANGUAGE"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
        }

        private void mnuTools_UI_English_Click(object sender, EventArgs e)
        {
            MyMentor.Properties.Settings.Default.CultureInfo = "en-us";
            MyMentor.Properties.Settings.Default.Save();
            MessageBox.Show(ResourceHelper.GetLabel("ENGLISH_CHANGE_LANGUAGE"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
        }

        private void loginAsOtherUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParseUser.LogOut();

            MessageBox.Show(ResourceHelper.GetLabel("LOGIN_OTHER_USER"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
            Application.Exit();
        }

        private void gotoCharIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new FormGoto())
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

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_skipManuallyAnchorsValidation = true;
            richTextBox2.Rtf = richTextBox1.Rtf;
            richTextBox1.Visible = false;
            RemoveAnchor(AnchorType.Paragraph);
            RemoveAnchor(AnchorType.Sentence);
            RemoveAnchor(AnchorType.Section);
            RemoveAnchor(AnchorType.Word);
            richTextBox1.Rtf = richTextBox2.Rtf;
            richTextBox1.Visible = true;
            tabControl1_SelectedIndexChanged(null, new EventArgs());
            m_skipManuallyAnchorsValidation = false;
        }

        private void clearParagraphAnchorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_skipManuallyAnchorsValidation = true;
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Paragraph);
            richTextBox1.Rtf = richTextBox2.Rtf;
            tabControl1_SelectedIndexChanged(null, new EventArgs());
            m_skipManuallyAnchorsValidation = false;
        }

        private void clearSentensesAnchorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_skipManuallyAnchorsValidation = true;
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Sentence);
            richTextBox1.Rtf = richTextBox2.Rtf;
            tabControl1_SelectedIndexChanged(null, new EventArgs());
            m_skipManuallyAnchorsValidation = false;
        }

        private void clearSectionAnchorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_skipManuallyAnchorsValidation = true;
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Section);
            richTextBox1.Rtf = richTextBox2.Rtf;
            tabControl1_SelectedIndexChanged(null, new EventArgs());
            m_skipManuallyAnchorsValidation = false;
        }

        private void clearWordAnchorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_skipManuallyAnchorsValidation = true;
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Word);
            richTextBox1.Rtf = richTextBox2.Rtf;
            tabControl1_SelectedIndexChanged(null, new EventArgs());
            m_skipManuallyAnchorsValidation = false;
        }

        private void CheckAnchors()
        {
            string[] types = new string[] { Clip.PAR_SIGN, Clip.SEN_SIGN, Clip.SEC_SIGN, Clip.WRD_SIGN };

            foreach (var type in types)
            {
                int index = richTextBox2.Find(type, 0, RichTextBoxFinds.None);

                while (index >= 0)
                {
                    richTextBox2.SelectionStart = index + 3;
                    richTextBox2.SelectionLength = richTextBox2.TextLength - index + 3;

                    if (string.IsNullOrEmpty(richTextBox2.SelectedText.ClearSpacesAndBreakLines()))
                    {
                        richTextBox2.SelectionStart = index;
                        richTextBox2.SelectionLength = 3;
                        richTextBox2.SelectedText = "";

                        break;
                    }

                    index = richTextBox2.Find(type, index + 3, RichTextBoxFinds.None);
                }
            }

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
                ClickAnchor(type, index + 3);

                if (index + 3 < richTextBox2.TextLength)
                {
                    index = richTextBox2.Find(anchor, index + 3, RichTextBoxFinds.None);
                }
                else
                {
                    break;
                }
            }
        }

        private void mnuAudioLoad_Click(object sender, EventArgs e)
        {
            // due to the fact that a Load operation will discard any existing sound,
            // check if there is already a Loading session available
            DialogResult result;
            if (audioSoundEditor1.GetSoundDuration() > 0)
            {
                // ask the user if he wants to go on
                result = MessageBox.Show(ResourceHelper.GetLabel("OVERWRITE_AUDIO_CONFIRM"), "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                if (result == DialogResult.No)
                    return;
            }

            // free the actual editing session
            audioSoundEditor1.CloseSound();

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

            //fix scheduling
            if (Clip.Current.Chapter.Paragraphs != null)
            {
                // buffer
                TimeSpan buffer = new TimeSpan(0, 0, 0, 0, frmSilence.m_nSilenceLengthInMs);

                var inRangeWords = Clip.Current.Chapter.Words.Where(word =>
                    word.StartTime != TimeSpan.Zero && word.StartTime.TotalMilliseconds > nBeginSelectionInMs);

                if (inRangeWords.Count() > 0)
                {
                    foreach (Word word in inRangeWords)
                    {
                        word.StartTime = word.StartTime.Add(buffer);
                    }

                    Clip.Current.Devide();
                }
            }

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
            {
                audioSoundEditor1.SetInsertPos(nBeginSelectionInMs);
                m_messageInsertion.InsertLocation = nBeginSelectionInMs;
            }
            else
            {
                audioSoundEditor1.SetInsertPos(0);
                m_messageInsertion.InsertLocation = 0;
            }

            m_messageInsertion.Active = true;
            m_messageInsertion.OldClipDuration = audioSoundEditor1.GetSoundDuration();

            // set insert mode and get sound data from the chosen file
            audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_INSERT);
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

            int nStart = 0;
            int nEnd = 0;

            audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nStart, ref nEnd);

            m_LastRangeSelections.Clear();
            m_LastRangeSelections.Add(nStart);
            m_LastRangeSelections.Add(nEnd);

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

            int nStart = 0;
            int nEnd = 0;

            audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nStart, ref nEnd);

            m_LastRangeSelections.Clear();
            m_LastRangeSelections.Add(nStart);
            m_LastRangeSelections.Add(nEnd);

            m_LastSelections.Clear();
            m_LastSelections.Add(nBeginSelectionInMs);
            m_LastSelections.Add(nEndSelectionInMs);

            // if a selection is available
            if (bSelectionAvailable)
            {
                // paste at the given position
                audioSoundEditor1.SetInsertPos(nBeginSelectionInMs);
                m_messageInsertion.InsertLocation = nBeginSelectionInMs;
            }
            else
            // paste at position 0
            {
                audioSoundEditor1.SetInsertPos(0);
                m_messageInsertion.InsertLocation = 0;
            }

            // set insert mode
            audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_INSERT);

            m_messageInsertion.Active = true;
            m_messageInsertion.OldClipDuration = audioSoundEditor1.GetSoundDuration();

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
            if (!bSelectionAvailable || nBeginSelectionInMs == nEndSelectionInMs)
                // no selection
                return;

            if (ConfirmRemoveSelectionRange(nBeginSelectionInMs, nEndSelectionInMs))
            {
                int nStart = 0;
                int nEnd = 0;

                audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nStart, ref nEnd);

                m_LastRangeSelections.Clear();
                m_LastRangeSelections.Add(nStart);
                m_LastRangeSelections.Add(nStart);

                m_LastSelections.Clear();
                m_LastSelections.Add(nBeginSelectionInMs);
                m_LastSelections.Add(nBeginSelectionInMs);

                // delete the selected range
                audioSoundEditor1.DeleteRange(nBeginSelectionInMs, nEndSelectionInMs);

                // remove the actual selection
                //audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(false, 0, 0);
            }

        }

        private bool ConfirmRemoveSelectionRange(int nBeginSelectionInMs, int nEndSelectionInMs)
        {
            TimeSpan buffer = new TimeSpan(0, 0, 0, 0, nEndSelectionInMs - nBeginSelectionInMs);

            ////check if we delete some anchors
            if (Clip.Current.Chapter.Paragraphs != null && Clip.Current.Chapter.FirstWord != null)
            {
                var inRangeWords = Clip.Current.Chapter.Words.Where(word =>
                    word.StartTime != TimeSpan.Zero && word.StartTime.TotalMilliseconds > nBeginSelectionInMs && word.StartTime.TotalMilliseconds < nEndSelectionInMs);

                if (inRangeWords.Count() > 0)
                {
                    if (MessageBox.Show(ResourceHelper.GetLabel("CUT_DELETE_CLIP_WORDS_INRANGE_CONFIRM"), "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl) == System.Windows.Forms.DialogResult.No)
                    {
                        return false;
                    }
                    else
                    {
                        Word next = null;
                        int index = 0;

                        foreach (Word word in inRangeWords)
                        {
                            if (word.PreviousWord != null)
                            {
                                index += 3;
                                richTextBox1.SelectionStart = word.RealCharIndex - index;
                                richTextBox1.SelectionLength = 3;
                                richTextBox1.SelectedText = "";
                                word.Index = -1;

                                //decreased index
                                Word internal_next = word.NextWord;
                                while (internal_next != null)
                                {
                                    internal_next.Index = internal_next.Index - 1;
                                    internal_next = internal_next.NextWord;
                                }
                            }
                            else
                            {
                                //set first one
                                word.StartTime = new TimeSpan(0, 0, 0, 0, 100);
                            }

                            //next = word;
                        }

                        //while (next != null)
                        //{
                        //    next.StartTime = next.StartTime.Add(buffer);
                        //    next = next.NextWord;
                        //}

                        Clip.Current.RtfText = richTextBox1.Rtf;
                        Clip.Current.Text = richTextBox1.Text;
                        Clip.Current.Devide();

                        richTextBox2.Rtf = richTextBox1.Rtf;
                        CheckAnchors();
                        richTextBox1.Rtf = richTextBox2.Rtf;
                        richTextBox5.Clear();
                        richTextBox5.Rtf = richTextBox1.Rtf;
                        richTextBox5.RemoveAnchors();
                        richTextBox3.Clear();
                        m_skipSelectionChange = true;
                        richTextBox3.Rtf = richTextBox1.Rtf;

                        //add end anchor
                        richTextBox3.AppendText(END_PAUSE_SECTION_ANCHOR);

                        //add start anchor
                        richTextBox3.SelectionStart = 0;
                        richTextBox3.SelectionLength = 0;
                        richTextBox3.SelectedText = START_PAUSE_SECTION_ANCHOR;
                        m_skipSelectionChange = false;

                        PaintGraphics();
                    }
                }

                //position after selection beign removed
                var afterRangeWords = Clip.Current.Chapter.Words.Where(word =>
                    word.StartTime != TimeSpan.Zero && word.StartTime.TotalMilliseconds > nEndSelectionInMs);

                if (afterRangeWords.Count() > 0)
                {
                    foreach (Word word in afterRangeWords)
                    {
                        word.StartTime = word.StartTime.Subtract(buffer);
                    }

                    Clip.Current.Devide();
                }
            }

            return true;
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

        private void equalizersToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void mnuZoom_Selection_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomToSelection(false);

        }

        private void mnuZoom_AllClip_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomToFullSound();
        }

        private void mnuZoom_In_Click(object sender, EventArgs e)
        {
            toolStripButton2_Click(null, new EventArgs());
        }

        private void mnuZoom_Out_Click(object sender, EventArgs e)
        {
            toolStripButton1_Click(null, new EventArgs());
        }

        private void testAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTestSound frmTest = new FormTestSound((short)trackBarVolume1.Value, (short)trackBar1.Value);
            frmTest.ShowDialog();

            trackBar1.Value = frmTest.RecordVolume;
            trackBarVolume1.Value = frmTest.PlayerVolume;

        }

        private void removeAllSchedulingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clip.Current.Chapter.Paragraphs != null)
            {
                if (MessageBox.Show(ResourceHelper.GetLabel("REMOVE_SCHEDULING_CONFIRM"), "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl) == System.Windows.Forms.DialogResult.Yes)
                {
                    Clip.Current.IsDirty = true;

                    var words = Clip.Current.Chapter.Words;

                    foreach (var word in words)
                    {
                        word.StartTime = TimeSpan.Zero;
                        word.Duration = TimeSpan.Zero;
                    }

                    //remove graphics
                    RemoveWaveFormGraphics();
                    audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();

                    m_endLineUniqueId = -1;
                    m_selectedGraphicItemSelected = -1;

                    richTextBox3.SelectionStart = 0;
                }

            }

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox frm = new AboutBox();
            frm.ShowDialog();

        }

        private void showDebugDetailToolStripMenuItem_Click(object sender, EventArgs e)
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
            RemoveWaveFormGraphics();

            JsonDebugFrm frm = new JsonDebugFrm();
            frm.ShowDialog();

            if (tabControl1.SelectedIndex == 2)
            {
                PaintWaveFormGraphics();
            }
            audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();

        }

        private void tbrNew_Click(object sender, EventArgs e)
        {
            mnuFile_NewClip_Click(null, new EventArgs());
        }

        private void tbrOpen_Click(object sender, EventArgs e)
        {
            mnuFile_Open_Click(null, new EventArgs());
        }

        private void tbrSave_Click(object sender, EventArgs e)
        {
            mnuFile_Save_Click(null, new EventArgs());
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Cut();
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                MessageBox.Show("Unable to copy document content.", "RTE - Copy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Copy();
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                MessageBox.Show("Unable to copy document content.", "RTE - Copy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Paste();
            }
            catch(Exception ex)
            {
                Program.Logger.Error(ex);
                MessageBox.Show("Unable to copy clipboard content to document.", "RTE - Paste", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void tbrSmallerFont_Click(object sender, EventArgs e)
        {
            try
            {
                Font font = richTextBox1.SelectionFont;
                Font font2 = new System.Drawing.Font(font.FontFamily, font.Size - (float)1);

                richTextBox1.SelectionFont = font2;
                PaintGraphics();
            }
            catch
            {

            }

        }

        private void tbrBiggerFont_Click(object sender, EventArgs e)
        {
            try
            {
                Font font = richTextBox1.SelectionFont;
                Font font2 = new System.Drawing.Font(font.FontFamily, font.Size + (float)1);

                richTextBox1.SelectionFont = font2;
                PaintGraphics();
            }
            catch
            {

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
                    richTextBox1.SelectionFont = new Font(fontDialog1.Font, FontStyle.Regular);
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
                //richTextBox1.SelectAll();
                richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
                //richTextBox1.Select(0, 0);
                PaintGraphics();

                Clip.Current.RightAlignment = true;
            }
            catch
            {

            }

        }

        private void tbrLeft_Click(object sender, EventArgs e)
        {
            try
            {
                //richTextBox1.SelectAll();
                richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
                //richTextBox1.Select(0, 0);
                PaintGraphics();

                Clip.Current.RightAlignment = false;
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

        private void tbrParagraph_Click(object sender, EventArgs e)
        {
            if (tbrParagraph.Checked)
            {
                tbrSentense.Checked = false;
                tbrSection.Checked = false;
                tbrWord.Checked = false;
            }
        }

        private void tbrSentense_Click(object sender, EventArgs e)
        {
            if (tbrSentense.Checked)
            {
                tbrParagraph.Checked = false;
                tbrSection.Checked = false;
                tbrWord.Checked = false;
            }
        }

        private void tbrSection_Click(object sender, EventArgs e)
        {
            if (tbrSection.Checked)
            {
                tbrParagraph.Checked = false;
                tbrSentense.Checked = false;
                tbrWord.Checked = false;
            }

        }

        private void tbrWord_Click(object sender, EventArgs e)
        {
            if (tbrWord.Checked)
            {
                tbrParagraph.Checked = false;
                tbrSentense.Checked = false;
                tbrSection.Checked = false;
            }
        }

        private void tbrRemoveAnchors_Click(object sender, EventArgs e)
        {
            m_skipManuallyAnchorsValidation = true;

            if (tbrParagraph.Checked)
            {
                clearParagraphAnchorsToolStripMenuItem_Click(null, new EventArgs());
            }

            if (tbrSentense.Checked)
            {
                clearSentensesAnchorsToolStripMenuItem_Click(null, new EventArgs());
            }

            if (tbrSection.Checked)
            {
                clearSectionAnchorsToolStripMenuItem_Click(null, new EventArgs());
            }

            if (tbrWord.Checked)
            {
                clearWordAnchorsToolStripMenuItem_Click(null, new EventArgs());
            }
            m_skipManuallyAnchorsValidation = false;

        }

        private void FormStudio_Resize(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.Move(Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);
            PaintGraphics();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

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

        private async void comboCategory1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (comboCategory1.SelectedIndex >= 0 && comboCategory2.Visible)
            {
                var remLoading = m_whileLoadingClip;
                var list = (await ParseTables.GetCategory2((string)comboCategory1.SelectedValue)).Where(o => o.Keys.Count() == 4);

                this.comboCategory2.DisplayMember = MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il" ? "HebrewValue" : "EnglishValue";
                this.comboCategory2.ValueMember = "ObjectId";
                this.comboCategory2.DataSource = list.Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    HebrewValue = c.ContainsKey("value_he_il") ? c.Get<string>("value_he_il") : string.Empty,
                    EnglishValue = c.ContainsKey("value_en_us") ? c.Get<string>("value_en_us") : string.Empty
                }).ToList(); ;

                if (Clip.Current.Category2 != null)
                {
                    m_whileLoadingClip = true;
                    this.comboCategory2.SelectedValue = Clip.Current.Category2;
                    m_whileLoadingClip = false;
                }
            }
            else
            {
                this.comboCategory2.DataSource = null;
            }

            RegenerateClipName(true);

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            mnuAudioLoad_Click(null, new EventArgs());

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            Int32 nBeginPosInMs = 0;
            Int32 nEndPosInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            int center = 0;

            if (nBeginSelectionInMs != nEndSelectionInMs)
            {
                center = ((nEndSelectionInMs - nBeginSelectionInMs) / 2) + nBeginSelectionInMs;
            }
            else
            {
                center = nBeginSelectionInMs;
            }

            audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nBeginPosInMs, ref nEndPosInMs);

            int nNewBeginPosInMs = center - (int)((nEndPosInMs - nBeginPosInMs) * 1.35);
            int nNewEndPosInMs = center + (int)((nEndPosInMs - nBeginPosInMs) * 1.35);

            audioSoundEditor1.DisplayWaveformAnalyzer.SetDisplayRange(nNewBeginPosInMs, nNewEndPosInMs);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            Int32 nBeginPosInMs = 0;
            Int32 nEndPosInMs = 0;

            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            int center = 0;

            if (nBeginSelectionInMs != nEndSelectionInMs)
            {
                center = ((nEndSelectionInMs - nBeginSelectionInMs) / 2) + nBeginSelectionInMs;
            }
            else
            {
                center = nBeginSelectionInMs;
            }

            audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nBeginPosInMs, ref nEndPosInMs);

            int nNewBeginPosInMs = center - (int)((nEndPosInMs - nBeginPosInMs) * 0.35);
            int nNewEndPosInMs = center + (int)((nEndPosInMs - nBeginPosInMs) * 0.35);

            audioSoundEditor1.DisplayWaveformAnalyzer.SetDisplayRange(nNewBeginPosInMs, nNewEndPosInMs);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomToFullSound();

        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            bool bSelectionAvailable = false;
            int nBeginSelectionInMs = 0;
            int nEndSelectionInMs = 0;

            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // check if we have simply selected a position (vertical dotted line)
            if (nBeginSelectionInMs != nEndSelectionInMs)
            {
                timerSkipAnalyzerSelection.Enabled = true;
                audioSoundEditor1.DisplayWaveformAnalyzer.ZoomToSelection(false);

                int nWidthInMs = 0;
                int nWidthInPixels = 0;

                audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayWidth(ref nWidthInMs, ref nWidthInPixels);

                var ten_per = nWidthInMs * 0.1;

                audioSoundEditor1.DisplayWaveformAnalyzer.SetDisplayRange((int)(nBeginSelectionInMs - ten_per), (int)(nEndSelectionInMs + ten_per));
            }

        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            var a = audioSoundEditor1.GetSoundDuration();
            //done playing - return position to start
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, a, a);

        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            //done playing - return position to start
            bool bSelectionAvailable = false;
            int nBeginSelectionInMs = 0;
            int nEndSelectionInMs = 0;

            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // check if we have simply selected a position (vertical dotted line)
            if (nBeginSelectionInMs != nEndSelectionInMs)
            {
                audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);
            }
            else
            {
                audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);
            }

        }

        private void toolStripButton10_Click(object sender, EventArgs e)
        {
            if (audioSoundEditor1.UndoIsAvailable())
            {
                audioSoundEditor1.UndoApply();
            }

        }

        private void trackBarWaveZoom_Scroll(object sender, EventArgs e)
        {
            // get the actual analyzer settings
            WANALYZER_GENERAL_SETTINGS settings = new WANALYZER_GENERAL_SETTINGS();
            audioSoundEditor1.DisplayWaveformAnalyzer.SettingsGeneralGet(ref settings);

            // hide the bottom scrollbar
            float fValue = trackBarWaveZoom.Value;
            settings.fVerticalZoomFactor = fValue / 100.0f;

            // apply the new settings
            audioSoundEditor1.DisplayWaveformAnalyzer.SettingsGeneralSet(settings);

        }

        private void buttonStartRecNew_Click(object sender, EventArgs e)
        {
            // check if we already have an editing session in memory
            DialogResult result;
            if (audioSoundEditor1.GetSoundDuration() > 0)
            {
                // ask the user if he wants to go on
                result = MessageBox.Show(ResourceHelper.GetLabel("OVERWRITE_AUDIO_CONFIRM"), "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
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

        private void buttonStopRecording_Click(object sender, EventArgs e)
        {
            timerRecordIcon.Enabled = false;
            // stop recording
            audioSoundRecorder1.Stop();

        }

        private void buttonStartRecAppend_Click(object sender, EventArgs e)
        {
            // create a fresh new recording session
            audioSoundRecorder1.SetRecordingMode(AudioSoundRecorder.enumRecordingModes.REC_MODE_NEW);

            // set the flag for "append" mode
            m_bRecAppendMode = true;

            // start recording in memory from system default input device and input channel
            audioSoundRecorder1.StartFromDirectSoundDevice(0, -1, "");

        }


        private void trackBarVolume1_Scroll(object sender, EventArgs e)
        {

        }

        private void PaintWaveFormGraphics()
        {
            if (Clip.Current.Chapter.Paragraphs != null)
            {
                // Add lines for all anchors
                foreach (Word word in Clip.Current.Chapter.Words)
                {
                    if (word.StartTime.TotalMilliseconds > 0)
                    {
                        word.GraphicItemUnique = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineAdd("WS|" + word.Index, word.Content,
                            (int)word.StartTime.TotalMilliseconds == 0 ? 50 : (int)word.StartTime.TotalMilliseconds,
                            new WANALYZER_VERTICAL_LINE
                            {
                                color = Color.DeepSkyBlue,
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

            if (Clip.Current.Chapter.LastWord != null && Clip.Current.Chapter.LastWord.Duration > TimeSpan.Zero)
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
            audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();

        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveWaveFormGraphics()
        {
            foreach (Word word in Clip.Current.Chapter.Words)
            {
                if (word.GraphicItemUnique > -1)
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(word.GraphicItemUnique);
                    word.GraphicItemUnique = -1;
                }
            }

            //remove guid line & end line
            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(m_endLineUniqueId);
            m_endLineUniqueId = -1;
            audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_selectedScheduledWord = null;
            m_lastAnchorIndexSelected = -1;
            tsm_RemoveAnchor.Enabled = false;

            tbrParagraph.Checked = false;
            tbrSection.Checked = false;
            tbrSentense.Checked = false;
            tbrWord.Checked = false;

            richTextBox2.Rtf = richTextBox1.Rtf;
            CheckAnchors();
            richTextBox1.Rtf = richTextBox2.Rtf;
            Clip.Current.Devide();

            if (tabControl1.SelectedIndex == 0
                || tabControl1.SelectedIndex == 3)
            {
                tableLayoutPanel4.RowStyles[1].Height = 0;
                tableLayoutPanel4.RowStyles[2].Height = 0;

            //    RegenerateClipName();
                RemoveWaveFormGraphics();
                m_waveFormTabIndex = -1;
            }
            else
            {
                tableLayoutPanel4.RowStyles[1].Height = 38;
                tableLayoutPanel4.RowStyles[2].Height = 160;

                if (tabControl1.SelectedIndex == 1)
                {
                    tableLayoutPanel4.ColumnStyles[3].Width = 40;
                    tableLayoutPanel4.ColumnStyles[4].Width = 40;
                    tsbContinueRecord.Visible = true;
                    tsbStartRecordNew.Visible = true;
                    tsbHammer.Visible = false;
                    tsbMoveAnchorRewind.Visible = false;
                    tsbMoveAnchorForward.Visible = false;
                    tsbCheckAnchor.Visible = false;
                    tsbImportFile.Visible = true;
                    lblInterval1.Visible = false;
                    lblInterval2.Visible = false;
                    cbInterval.Visible = false;
                }
                else if (tabControl1.SelectedIndex == 2)
                {
                    tableLayoutPanel4.ColumnStyles[3].Width = 0;
                    tableLayoutPanel4.ColumnStyles[4].Width = 0;
                    tsbContinueRecord.Visible = false;
                    tsbStartRecordNew.Visible = false;
                    tsbHammer.Visible = true;
                    tsbMoveAnchorRewind.Visible = true;
                    tsbMoveAnchorForward.Visible = true;
                    tsbCheckAnchor.Visible = true;
                    tsbImportFile.Visible = false;
                    lblInterval1.Visible = true;
                    lblInterval2.Visible = true;
                    cbInterval.Visible = true;
                }
                audioSoundEditor1.DisplayWaveformAnalyzer.Move(Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);
                audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);
            }

            //check if we have to refresh display
            if (tabControl1.SelectedIndex == 1)
            {
                //copy text and insert start anchor
                richTextBox5.Clear();
                richTextBox5.Rtf = richTextBox1.Rtf;
                richTextBox5.RemoveAnchors();

                if (m_waveFormTabIndex != 1)
                {
                    m_waveFormTabIndex = 1;
                    //audioSoundEditor1.DisplayWaveformAnalyzer.MouseSelectionEnable(true);

                    //remove graphics
                    RemoveWaveFormGraphics();
                }
            }
            //scheduler step
            else if (tabControl1.SelectedIndex == 2)
            {
                //copy text and insert start anchor
                m_skipSelectionChange = true;

                richTextBox3.Clear();
                richTextBox3.Rtf = richTextBox1.Rtf;
                Clip.Current.Devide();

                //add end anchor
                richTextBox3.AppendText(END_PAUSE_SECTION_ANCHOR);

                //add start anchor
                richTextBox3.SelectionStart = 0;
                richTextBox3.SelectionLength = 0;
                richTextBox3.SelectedText = START_PAUSE_SECTION_ANCHOR;
                m_skipSelectionChange = false;

                if (m_waveFormTabIndex != 2)
                {
                    m_waveFormTabIndex = 2;

                    //m_guidLineUniqueId = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineAdd("MyLine", "", 0,
                    //    new WANALYZER_VERTICAL_LINE { color = Color.Yellow, nWidth = 1, nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT, nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DASH_DOT, nHighCap = enumLineCaps.LINE_CAP_SQUARE, nLowCap = enumLineCaps.LINE_CAP_SQUARE, nTranspFactor = 0 });

                    PaintWaveFormGraphics();

                    //audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(false, -1, -1);
                    //audioSoundEditor1.DisplayWaveformAnalyzer.MouseSelectionEnable(false);
                    audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();

                }

                //selects first word
                //richTextBox3.SelectionStart = 0;
                //richTextBox3.SelectionLength = 0;
                cbInterval.Focus();
            }

            PaintGraphics();
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 2 &&
    audioSoundEditor1.GetSoundDuration() <= 0)
            {
                // ask the user if he wants to go on
                MessageBox.Show(ResourceHelper.GetLabel("NO_AUDIO_EXISTS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                e.Cancel = true;
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
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
                    audioDjStudio1.PlaySound(0, nBeginSelectionInMs, nEndSelectionInMs);
                }
                else
                {
                    // play selected range only
                    audioDjStudio1.PlaySound(0, nBeginSelectionInMs, -1);
                }
            }
            else
            {
                audioDjStudio1.PlaySound(0);
            }

        }

        private void buttonPlaySelection_Click(object sender, EventArgs e)
        {
            audioDjStudio1.LoadSoundFromEditingSession(0, audioSoundEditor1.Handle);

            // get the position selected on the waveform analyzer, if any
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            // if a selection is available
            if (bSelectionAvailable)
            {
                // play selected range only
                var error = audioDjStudio1.PlaySound(0, nBeginSelectionInMs, nEndSelectionInMs);

                m_LastSelections.Clear();
                m_LastSelections.Add(nBeginSelectionInMs);
                m_LastSelections.Add(nEndSelectionInMs);
            }

        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            audioDjStudio1.StopSound(0);
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);

        }

        private void timerUpdateDuringPlayback_Tick(object sender, EventArgs e)
        {
            double fPosition = 0;
            audioDjStudio1.SoundPositionGet(0, ref fPosition, false);

            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, (int)fPosition, (int)fPosition);

            int nBeginPosInMs = 0;
            int nEndPosInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nBeginPosInMs, ref nEndPosInMs);
            int soundDuration = audioSoundEditor1.GetSoundDuration();

            var buffer = (nEndPosInMs - nBeginPosInMs) / 100;

            if ((nBeginPosInMs > 0 || nEndPosInMs < soundDuration) &&
                fPosition + buffer > nEndPosInMs)
            {
                if ((int)nEndPosInMs + (nEndPosInMs - nBeginPosInMs) - buffer < soundDuration)
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetDisplayRange((int)nEndPosInMs - buffer, (int)nEndPosInMs + (nEndPosInMs - nBeginPosInMs) - buffer);
                }
                else if (nEndPosInMs < soundDuration)
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetDisplayRange((int)nBeginPosInMs, soundDuration);
                }
            }

            int mm = audioSoundEditor1.GetPlaybackPosition();
            var position = new TimeSpan(0, 0, 0, 0, Math.Max((int)fPosition, 0)); ;

            Debug.WriteLine(string.Format("_Tick: position ({0})", audioSoundEditor1.FromMsToFormattedTime((long)position.TotalMilliseconds, false, true)));

            //check anchor check
            if (m_pauseOnAnchorGraphicItem >= 0)
            {
                int nGraphicItemStart = 0;
                int nGraphicItemEnd = 0;
                audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionGet(m_pauseOnAnchorGraphicItem, ref nGraphicItemStart, ref nGraphicItemEnd);

                if ((int)fPosition + 60 >= nGraphicItemStart)
                {
                    m_pauseOnAnchorGraphicItem = -1;
                    audioDjStudio1.PauseSound(0);
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, nGraphicItemStart, nGraphicItemStart);
                    TimerMenuEnabler.Enabled = false;
                    timerPlayAnchorCheckPause.Enabled = true;
                    return;
                }
            }

            //during playing check if the current position moved over to the next word
            if (Clip.Current.Chapter.Paragraphs != null && Clip.Current.Chapter.FirstWord != null)
            {
                //select start anchor
                if (position < Clip.Current.Chapter.FirstWord.StartTime)
                {
                    Debug.WriteLine(string.Format("select start anchor cause position is less than start time {0}", audioSoundEditor1.FromMsToFormattedTime((long)Clip.Current.Chapter.FirstWord.StartTime.TotalMilliseconds, false, true)));

                    if (!m_selectedStartAnchor)
                    {
                        richTextBox3.SelectionStart = 0;
                    }
                }
                // select end anchor
                else if (position > Clip.Current.Chapter.LastWord.StartTime + Clip.Current.Chapter.LastWord.Duration &&
                    Clip.Current.Chapter.LastWord.StartTime > TimeSpan.Zero &&
                    Clip.Current.Chapter.LastWord.Duration > TimeSpan.Zero)
                {
                    Debug.WriteLine(string.Format("select end anchor cause position is bigger than start time + duration of last word {0}", audioSoundEditor1.FromMsToFormattedTime((long)(Clip.Current.Chapter.LastWord.StartTime + Clip.Current.Chapter.LastWord.Duration).TotalMilliseconds, false, true)));

                    if (!m_selectedEndAnchor)
                    {
                        richTextBox3.SelectionStart = richTextBox3.TextLength;
                    }
                }
                else
                {
                    // between start and end anchors

                    var word = Clip.Current.Chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections)
                        .SelectMany(sc => sc.Words).Where(w => w.StartTime > TimeSpan.Zero && w.StartTime < position).LastOrDefault();

                    if (word != null &&
                        richTextBox3.SelectionStart != word.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length)
                    {
                        Debug.WriteLine(string.Format("word selected '{0}' with start time of {1}", word.Content, audioSoundEditor1.FromMsToFormattedTime((long)word.StartTime.TotalMilliseconds, false, true)));
                        Debug.WriteLine(string.Format("editor selection start : {0} compared to : {1}", richTextBox3.SelectionStart, (word.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length + 1)));

                        richTextBox3.SelectionStart = word.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length + 1;
                    }

                }
            }

            if (m_bRecAppendMode && m_bContinueRecordingAfterPlayback &&
                 soundDuration - fPosition <= 60)
            {
                if (audioSoundRecorder1.Status != AudioSoundRecorder.enumRecorderStates.RECORD_STATUS_RECORDING &&
                    audioSoundRecorder1.Status != AudioSoundRecorder.enumRecorderStates.RECORD_STATUS_RECORDING_MEMORY)
                {
                    // create a fresh new recording session
                    audioSoundRecorder1.SetRecordingMode(AudioSoundRecorder.enumRecordingModes.REC_MODE_APPEND);

                    // start recording in memory from system default input device and input channel
                    audioSoundRecorder1.StartFromDirectSoundDevice(0, -1, "");
                }
            }
        }


        private void timerDisplayWaveform_Tick(object sender, EventArgs e)
        {
            timerDisplayWaveform.Enabled = false;

            // display the full waveform
            audioSoundEditor1.DisplayWaveformAnalyzer.SetDisplayRange(0, -1);

        }

        private void timerRecordIcon_Tick(object sender, EventArgs e)
        {
            if (audioSoundRecorder1.Status == AudioSoundRecorder.enumRecorderStates.RECORD_STATUS_RECORDING ||
                audioSoundRecorder1.Status == AudioSoundRecorder.enumRecorderStates.RECORD_STATUS_RECORDING_MEMORY)
            {
                if (!m_bRecAppendMode)
                {
                    if (b_recordIconRed)
                    {
                        tsbStartRecordNew.Image = imageList1.Images[1];
                    }
                    else
                    {
                        tsbStartRecordNew.Image = imageList1.Images[0];
                    }
                }
                else
                {
                    if (b_recordIconRed)
                    {
                        tsbContinueRecord.Image = imageList1.Images[2];
                    }
                    else
                    {
                        tsbContinueRecord.Image = imageList1.Images[3];
                    }
                }
            }
            else
            {
                if (tabControl1.SelectedIndex == 2)
                {
                    if (b_recordIconRed)
                    {
                        tsbHammer.Image = imageList1.Images[8];
                    }
                    else
                    {
                        tsbHammer.Image = imageList1.Images[9];
                    }
                }
            }
            b_recordIconRed = !b_recordIconRed;

        }

        private void timerRecordingDone_Tick(object sender, EventArgs e)
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

            m_bRecAppendMode = false;
            m_bContinueRecordingAfterPlayback = false;
        }

        private void timerStartRecordingAfterPlayingBuffer_Tick(object sender, EventArgs e)
        {
            timerStartRecordingAfterPlayingBuffer.Enabled = false;

            // create a fresh new recording session
            audioSoundRecorder1.SetRecordingMode(AudioSoundRecorder.enumRecordingModes.REC_MODE_APPEND);

            // start recording in memory from system default input device and input channel
            audioSoundRecorder1.StartFromDirectSoundDevice(0, -1, "");

        }


        private void audioSoundRecorder1_RecordingDuration(object sender, AudioSoundRecorder.RecordingDurationEventArgs e)
        {
            m_intRecordingDuration = e.nDuration;

        }

        private void audioSoundRecorder1_RecordingPaused(object sender, EventArgs e)
        {
            AudioSoundRecorder.enumErrorCodes nResult = audioSoundRecorder2.StartFromDirectSoundDevice(0, 0, "");
            timerRecordIcon.Enabled = false;

        }

        private void audioSoundRecorder1_RecordingResumed(object sender, EventArgs e)
        {
            audioSoundRecorder2.Stop();
            timerRecordIcon.Enabled = true;

        }

        private void audioSoundRecorder1_RecordingStarted(object sender, EventArgs e)
        {
            audioSoundRecorder2.Stop();
            timerRecordIcon.Enabled = true;
        }

        private void audioSoundRecorder1_RecordingStopped(object sender, AudioSoundRecorder.RecordingStoppedEventArgs e)
        {
            AudioSoundRecorder.enumErrorCodes nResult = audioSoundRecorder2.StartFromDirectSoundDevice(0, 0, "");
            // force loading the recording session into the editor
            timerRecordingDone.Enabled = true;
            timerRecordIcon.Enabled = false;

            tsbStartRecordNew.Image = imageList1.Images[0];
            tsbContinueRecord.Image = imageList1.Images[2];
        }

        private void audioSoundRecorder1_VUMeterValueChange(object sender, AudioSoundRecorder.VUMeterValueChangeEventArgs e)
        {
            audioSoundRecorder1.GraphicBarsManager.SetValue(m_hWndVuMeterLeft1, e.nPeakLeft);
            audioSoundRecorder1.GraphicBarsManager.SetValue(m_hWndVuMeterRight1, e.nPeakRight);

        }

        private void audioSoundEditor1_SoundEditDone(object sender, SoundEditDoneEventArgs e)
        {
            LabelStatus.Text = "Status: Idle";
            progressBar1.Visible = false;

            LabelStatus.Refresh();

            if (e.bResult == true)
            {
                // success, force a new analysis of the recorded sound
                TimerReload.Enabled = true;
            }

        }

        private void audioSoundEditor1_SoundEditPerc(object sender, SoundEditPercEventArgs e)
        {
            if (progressBar1.Value == e.nPercentage)
                // no change
                return;

            progressBar1.Value = e.nPercentage;
            LabelStatus.Text = "Status: Editing... " + e.nPercentage.ToString() + "%";

            progressBar1.Refresh();
            LabelStatus.Refresh();

        }

        private void audioSoundEditor1_SoundEditStarted(object sender, SoundEditStartedEventArgs e)
        {
            LabelStatus.Text = "Status: Editing ...";
            progressBar1.Visible = true;

            LabelStatus.Refresh();
            progressBar1.Refresh();

        }

        private void audioSoundEditor1_SoundExportDone(object sender, SoundExportDoneEventArgs e)
        {
            if (m_bInPublishProgress)
            {
                return;
            }

            LabelStatus.Text = "Status: Idle";
            progressBar1.Visible = false;

            LabelStatus.Refresh();

            Cursor.Current = Cursors.Default;

            Clip.Current.Saved = true;

            if (e.nResult != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show(e.nResult.ToString());
            else
            {
                MessageBox.Show(ResourceHelper.GetLabel("CLIP_SAVED_SUCCESSFULLY"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                mnuFile_SaveAs.Enabled = true;
            }

        }

        private void audioSoundEditor1_SoundExportPerc(object sender, SoundExportPercEventArgs e)
        {
            if (progressBar1.Value == e.nPercentage)
                // no change
                return;

            progressBar1.Value = e.nPercentage;
            LabelStatus.Text = "Status: Exporting... " + e.nPercentage.ToString() + "%";

            progressBar1.Refresh();
            LabelStatus.Refresh();

        }

        private void audioSoundEditor1_SoundExportStarted(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.Default;

            LabelStatus.Text = "Status: Exporting...";
            progressBar1.Visible = true;

            LabelStatus.Refresh();

        }

        private void audioSoundEditor1_SoundLoadingDone(object sender, SoundLoadingDoneEventArgs e)
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

                ////check if we delete some anchors
                if (Clip.Current.Chapter.Paragraphs != null && Clip.Current.Chapter.FirstWord != null && m_messageInsertion.Active)
                {
                    // buffer
                    TimeSpan buffer = new TimeSpan (0,0,0,0,audioSoundEditor1.GetSoundDuration() - m_messageInsertion.OldClipDuration);

                    var inRangeWords = Clip.Current.Chapter.Words.Where(word =>
                        word.StartTime != TimeSpan.Zero && word.StartTime.TotalMilliseconds > m_messageInsertion.InsertLocation);

                    if (inRangeWords.Count() > 0)
                    {
                        foreach (Word word in inRangeWords)
                        {
                            word.StartTime = word.StartTime.Add(buffer);
                        }

                        Clip.Current.Devide();
                    }
                    m_messageInsertion.Active = false;
                }

            }
            else
                MessageBox.Show("Sound failed to load with the following error code: " + audioSoundEditor1.LastError.ToString());

            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);

        }

        private void audioSoundEditor1_SoundLoadingPerc(object sender, SoundLoadingPercEventArgs e)
        {
            if (progressBar1.Value == e.nPercentage)
                // no change
                return;

            progressBar1.Value = e.nPercentage;
            LabelStatus.Text = "Status: Loading... " + e.nPercentage.ToString() + "%";

            progressBar1.Refresh();
            LabelStatus.Refresh();

        }

        private void audioSoundEditor1_SoundLoadingStarted(object sender, EventArgs e)
        {
            LabelStatus.Text = "Status: Loading... 0%";
            progressBar1.Value = 0;
            progressBar1.Visible = true;

            progressBar1.Refresh();
            LabelStatus.Refresh();

        }

        private void audioSoundEditor1_SoundPlaybackDone(object sender, EventArgs e)
        {
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            //in case just finished playing anchor fix
            if (m_rem_anchorFixRecording > TimeSpan.Zero)
            {
                m_rem_anchorFixRecording = TimeSpan.Zero;
            }

            if (m_bRecAppendMode && m_bContinueRecordingAfterPlayback)
            {
                timerStartRecordingAfterPlayingBuffer.Enabled = true;
            }

        }

        private void audioSoundEditor1_SoundPlaybackPaused(object sender, EventArgs e)
        {
            LabelStatus.Text = "Status: Playback paused";
            LabelStatus.Refresh();
        }

        private void audioSoundEditor1_SoundPlaybackPlaying(object sender, EventArgs e)
        {
            LabelStatus.Text = "Status: Playing...";
            LabelStatus.Refresh();
        }

        private void audioSoundEditor1_SoundPlaybackStopped(object sender, EventArgs e)
        {
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();
        }

        private void audioSoundEditor1_WaveAnalysisPerc(object sender, WaveAnalysisPercEventArgs e)
        {
            if (progressBar1.Value == e.nPercentage)
                // no change
                return;

            progressBar1.Value = e.nPercentage;
            LabelStatus.Text = "Status: Analyzing waveform... " + e.nPercentage.ToString() + "%";
            progressBar1.Refresh();
            LabelStatus.Refresh();

        }

        private void audioSoundEditor1_WaveAnalysisStart(object sender, EventArgs e)
        {
            LabelStatus.Text = "Status: Analyzing waveform...";
            progressBar1.Value = 0;
            progressBar1.Visible = true;

            progressBar1.Refresh();
            LabelStatus.Refresh();

        }


        private void audioSoundEditor1_WaveAnalysisStop(object sender, AudioSoundEditor.WaveAnalysisStopEventArgs e)
        {
            // force a refresh of the waveform analyzer
            //timerDisplayWaveform.Enabled = true;

            progressBar1.Visible = false;
            LabelStatus.Text = "Status: Idle";
        }


        private void audioSoundEditor1_WaveAnalyzerDisplayRangeChange(object sender, WaveAnalyzerDisplayRangeChangeEventArgs e)
        {

        }

        private void audioSoundEditor1_WaveAnalyzerGraphicItemClick(object sender, WaveAnalyzerGraphItemClickEventArgs e)
        {
            var name = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemNameGet(e.nUniqueID);

            //if (m_blSelectionHides)
            //{
            //    return;
            //}

            if (name.StartsWith("WS|"))
            {
                //other anchor -- get its name WS|12
                int index = int.Parse(name.Split('|')[1]);

                //m_bClickedWaveAnalyzer = true;

                var word = Clip.Current.Chapter.Words.Where(w => w.Index == index).FirstOrDefault();

                if ( word != null )
                {
                    richTextBox3.SelectionStart = word.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length - 1;

                    //var interval = new TimeSpan(0, 0, 0, 0, (int)(float.Parse(cbInterval.Text) * 1000));
                    //var selected = word.StartTime + word.Duration - interval;

                    //draw range
                    //m_nRangeSelection = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemWaveRangeAdd("RANGE", "",
                    //    (int)selected.TotalMilliseconds, (int)selected.TotalMilliseconds + (int)interval.TotalMilliseconds * 2,
                    //    new WANALYZER_WAVE_RANGE
                    //    {
                    //        colorBack = Color.DarkSlateGray,
                    //        colorWaveLinePeak = Color.Green,
                    //        colorWaveLineCenter = Color.LimeGreen
                    //    });

                    m_bClickedWaveAnalyzer = false;

                }
            }
            else if (name == "EndLine")
            {
                if (e.nButton == enumMouseButtons.MOUSE_BTN_LEFT)
                {
                    m_bClickedWaveAnalyzer = true;
                    richTextBox3.SelectionStart = Clip.Current.Chapter.LastWord.RealCharIndex + Clip.Current.Chapter.LastWord.Length + START_PAUSE_SECTION_ANCHOR.Length + 1;
                    m_bClickedWaveAnalyzer = false;
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

        private void audioSoundEditor1_WaveAnalyzerSelectionChange(object sender, WaveAnalyzerSelectionChangeEventArgs e)
        {
            if ((m_selectedGraphicItemSelected > 0)
                && m_LastSelections.Count() > 0
                && audioDjStudio1.GetPlayerStatus(0) != AudioDjStudio.enumPlayerStatus.SOUND_PLAYING
                && !timerSkipAnalyzerSelection.Enabled)
            {
                //check if inside a selection range
                if (e.nBeginPosInMs == e.nEndPosInMs && e.nBeginPosInMs >= m_LastSelections[0] - 1
                    && e.nEndPosInMs <= m_LastSelections[1] + 1)
                {
                    //don't do nothing
                }
                else
                {
                    //remove previous selected color 
                    if (m_selectedGraphicItemSelected >= 0)
                    {
                        // draw previous selected line as blue
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineParamsSet(m_selectedGraphicItemSelected,
                                                    new WANALYZER_VERTICAL_LINE()
                                                    {
                                                        color = m_endLineUniqueId == m_selectedGraphicItemSelected ? Color.PeachPuff : Color.DeepSkyBlue,
                                                        nWidth = 5,
                                                        nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT,
                                                        nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DOT,
                                                        nHighCap = enumLineCaps.LINE_CAP_SQUARE,
                                                        nLowCap = enumLineCaps.LINE_CAP_SQUARE,
                                                        nTranspFactor = 50
                                                    });

                        m_selectedGraphicItemSelected = -1;
                        m_blSelectionHides = false;
                    }

                }
            }
        }

        private void audioDjStudio1_SoundClosed(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            timerUpdateDuringSchedulingPlayback.Enabled = false;
        }

        private void audioDjStudio1_SoundDone(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            timerUpdateDuringSchedulingPlayback.Enabled = false;
            m_stopWatch.Stop();

            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            //after done anchor playback - select the anchor again
            if (m_lastAnchorIndexSelected >= 0)
            {
                richTextBox3.SelectionStart = m_lastAnchorIndexSelected + 1;
                m_lastAnchorIndexSelected = -1;
            }

            //in case just finished playing anchor fix
            if (m_rem_anchorFixRecording > TimeSpan.Zero)
            {
                m_rem_anchorFixRecording = TimeSpan.Zero;
            }

            if (m_bRecAppendMode && m_bContinueRecordingAfterPlayback)
            {
                //timerStartRecordingAfterPlayingBuffer.Enabled = true;
            }
            else
            {
                tsbPlay.Image = imageList1.Images[6];

                if (m_LastSelections.Count() > 0)
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, m_LastSelections[0], m_LastSelections[1]);
                    m_LastSelections.Clear();

                }
                else if (m_LastSelections.Count() > 0)
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, m_LastSelections[0], m_LastSelections[0]);

                    //here we dont want to clear selections
                    //m_LastSelections.Clear();
                }
                else
                {
                    //done playing - return position to start
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);
                }
            }

            //unlock selection
            audioSoundEditor1.DisplayWaveformAnalyzer.MouseSelectionEnable(true);//
            if (tabControl1.SelectedIndex == 2)
            {
                timerRecordIcon.Enabled = false;
                tsbHammer.Image = imageList1.Images[8];
            }
        }

        private void audioDjStudio1_SoundPaused(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            LabelStatus.Text = "Status: Playback paused";
            LabelStatus.Refresh();
            timerUpdateDuringSchedulingPlayback.Enabled = false;

            if (m_bRecAppendMode)
            {
                tsbContinueRecord.Image = imageList1.Images[2];
            }
            else
            {
                tsbPlay.Image = imageList1.Images[6];
            }

        }

        private void audioDjStudio1_SoundPlaying(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            LabelStatus.Text = "Status: Playing...";
            LabelStatus.Refresh();

            timerUpdateDuringSchedulingPlayback.Enabled = true;
        }

        private void audioDjStudio1_SoundStopped(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            timerUpdateDuringSchedulingPlayback.Enabled = false;
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            tsbPlay.Image = imageList1.Images[6];

            // get the position selected on the waveform analyzer, if any
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            //if (m_LastSelections.Count() > 0)
            //{
            //    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, m_LastSelections[0], m_LastSelections[1]);
            //    m_LastSelections.Clear();

            //if (m_nRangeSelection != -1)
            //{
            //    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(m_nRangeSelection);
            //    m_nRangeSelection = -1;
            //}
            //}
            //unlock selection
            audioSoundEditor1.DisplayWaveformAnalyzer.MouseSelectionEnable(true);//

            if (tabControl1.SelectedIndex == 2)
            {
                timerRecordIcon.Enabled = false;
                tsbHammer.Image = imageList1.Images[8];
            }

        }

        private void buttonRestartScheduling_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);
            //timePickerCurrentWord.Value = TimeSpan.Zero;
            richTextBox3.SelectionStart = 4;

        }

        private void richTextBox3_SelectionChanged(object sender, EventArgs e)
        {
            if (m_skipSelectionChange || Clip.Current.Chapter == null)
            {
                return;
            }

            tsm_RemoveAnchor.Enabled = false;

            if (audioDjStudio1.GetPlayerStatus(0) != AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
            {
                m_lastAnchorIndexSelected = -1;
            }

            int selectionIndex = Math.Max(0, richTextBox3.SelectionStart - START_PAUSE_SECTION_ANCHOR.Length);
            var interval = new TimeSpan(0, 0, 0, 0, (int)(float.Parse(cbInterval.Text) * 1000));

            //remove previous selected color 
            if (m_selectedGraphicItemSelected >= 0 && audioDjStudio1.GetPlayerStatus(0) != AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
            {
                // draw previous selected line as blue
                audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineParamsSet(m_selectedGraphicItemSelected,
                                            new WANALYZER_VERTICAL_LINE()
                                            {
                                                color = m_endLineUniqueId == m_selectedGraphicItemSelected ? Color.PeachPuff : Color.DeepSkyBlue,
                                                nWidth = 5,
                                                nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT,
                                                nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DOT,
                                                nHighCap = enumLineCaps.LINE_CAP_SQUARE,
                                                nLowCap = enumLineCaps.LINE_CAP_SQUARE,
                                                nTranspFactor = 50
                                            });

                m_selectedGraphicItemSelected = -1;
            }

            // SELECTED START ANCHOR
            if (selectionIndex == 0)
            {
                m_skipSelectionChange = true;
                richTextBox3.SelectionStart = 0;
                richTextBox3.SelectionLength = START_PAUSE_SECTION_ANCHOR.Length;
                m_skipSelectionChange = false;

                m_lastAnchorIndexSelected = richTextBox3.SelectionStart;

                m_selectedStartAnchor = true;
                m_selectedEndAnchor = false;

                //take first word
                m_selectedScheduledWord = Clip.Current.Chapter.FirstWord;

                if (audioDjStudio1.GetPlayerStatus(0) != AudioDjStudio.enumPlayerStatus.SOUND_PLAYING && m_selectedScheduledWord != null)
                {
                    tsm_RemoveAnchor.Enabled = true;

                    //move line to start grapohic line
                    if (!m_bClickedWaveAnalyzer)
                    {
                        var selected = m_selectedScheduledWord.StartTime - interval;

                        if (m_selectedScheduledWord.StartTime.TotalMilliseconds > 0)
                        {
                            m_LastSelections.Clear();
                            m_LastSelections.Add(Math.Max((int)selected.TotalMilliseconds, 0));
                            m_LastSelections.Add((int)selected.TotalMilliseconds + (int)interval.TotalMilliseconds * 2);

                            timerSkipAnalyzerSelection.Enabled = true;

                            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
                                m_LastSelections[0],
                                m_LastSelections[1]);
                        }
                        else
                        {
                            //audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
                            //    (int)timePickerCurrentWord.Value.TotalMilliseconds,
                            //    (int)timePickerCurrentWord.Value.TotalMilliseconds);
                        }
                    }
                    else
                    {
                        audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds,
                            (int)m_selectedScheduledWord.StartTime.TotalMilliseconds);
                    }

                    // draw selected line with red
                    m_selectedGraphicItemSelected = m_selectedScheduledWord.GraphicItemUnique;

                    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineParamsSet(m_selectedScheduledWord.GraphicItemUnique,
                                                new WANALYZER_VERTICAL_LINE()
                                                {
                                                    color = Color.Red,
                                                    nWidth = 5,
                                                    nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT,
                                                    nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DOT,
                                                    nHighCap = enumLineCaps.LINE_CAP_SQUARE,
                                                    nLowCap = enumLineCaps.LINE_CAP_SQUARE,
                                                    nTranspFactor = 50
                                                });
                }

            }
            //END ANCHOR
            else if (selectionIndex > richTextBox1.TextLength && Clip.Current.Chapter.Paragraphs != null)
            {
                m_skipSelectionChange = true;
                richTextBox3.SelectionStart = richTextBox3.TextLength - END_PAUSE_SECTION_ANCHOR.Length;
                richTextBox3.SelectionLength = END_PAUSE_SECTION_ANCHOR.Length;
                m_skipSelectionChange = false;

                m_lastAnchorIndexSelected = richTextBox3.SelectionStart;

                m_selectedStartAnchor = false;
                m_selectedEndAnchor = true;

                //set as null
                m_selectedScheduledWord = null;

                //clicked wave display - set selection on the graphic line
                m_selectedGraphicItemSelected = m_endLineUniqueId;

                if (audioDjStudio1.GetPlayerStatus(0) != AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
                {
                    // draw selected line with red
                    audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineParamsSet(m_endLineUniqueId,
                                                new WANALYZER_VERTICAL_LINE()
                                                {
                                                    color = Color.Red,
                                                    nWidth = 5,
                                                    nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT,
                                                    nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DOT,
                                                    nHighCap = enumLineCaps.LINE_CAP_SQUARE,
                                                    nLowCap = enumLineCaps.LINE_CAP_SQUARE,
                                                    nTranspFactor = 50
                                                });

                    if (!m_bClickedWaveAnalyzer)
                    {
                        var selected = Clip.Current.Chapter.LastWord.StartTime + Clip.Current.Chapter.LastWord.Duration - interval;

                        if (Clip.Current.Chapter.LastWord.StartTime.TotalMilliseconds > 0)
                        {
                            m_LastSelections.Clear();
                            m_LastSelections.Add((int)selected.TotalMilliseconds);
                            m_LastSelections.Add(Math.Min((int)selected.TotalMilliseconds + (int)interval.TotalMilliseconds * 2,
                                audioSoundEditor1.GetSoundDuration()));

                            timerSkipAnalyzerSelection.Enabled = true;
                            
                            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
                                m_LastSelections[0],
                                m_LastSelections[1]);

                            tsm_RemoveAnchor.Enabled = false;
                        }
                        else
                        {
                            //audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
                            //    (int)timePickerCurrentWord.Value.TotalMilliseconds,
                            //    (int)timePickerCurrentWord.Value.TotalMilliseconds);
                        }
                    }
                    else
                    {

                        //move selection line
                        timerSkipAnalyzerSelection.Enabled = true;

                        audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
                            (int)(Clip.Current.Chapter.LastWord.StartTime + Clip.Current.Chapter.LastWord.Duration).TotalMilliseconds,
                            (int)(Clip.Current.Chapter.LastWord.StartTime + Clip.Current.Chapter.LastWord.Duration).TotalMilliseconds);

                    }
                }
            }
            else if (Clip.Current.Chapter.Paragraphs != null)
            {
                //clicked on any other parts in the clip

                m_selectedStartAnchor = false;
                m_selectedEndAnchor = false;

                var savePreviousWord = m_selectedScheduledWord;

                //check for word selection
                m_selectedScheduledWord = Clip.Current.Chapter.Words.Where(ww => ww.RealCharIndex <= selectionIndex).LastOrDefault();

                if (m_selectedScheduledWord != null)
                {
                    m_skipSelectionChange = true;

                    // ANCHOR SELECTED
                    if (m_selectedScheduledWord.RealCharIndex + m_selectedScheduledWord.Length < selectionIndex
                        && (audioDjStudio1.GetPlayerStatus(0) != AudioDjStudio.enumPlayerStatus.SOUND_PLAYING))
                    {
                        richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + m_selectedScheduledWord.Length + START_PAUSE_SECTION_ANCHOR.Length;
                        richTextBox3.SelectionLength = 3;

                        m_lastAnchorIndexSelected = richTextBox3.SelectionStart;

                        //clicked wave display - set selection on the graphic line
                        if (m_selectedScheduledWord.NextWord != null && m_selectedScheduledWord.NextWord.GraphicItemUnique >= 0)
                        {
                            m_selectedGraphicItemSelected = m_selectedScheduledWord.NextWord.GraphicItemUnique;

                            // draw selected line
                            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineParamsSet(m_selectedScheduledWord.NextWord.GraphicItemUnique,
                                                        new WANALYZER_VERTICAL_LINE()
                                                        {
                                                            color = Color.Red,
                                                            nWidth = 5,
                                                            nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT,
                                                            nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DOT,
                                                            nHighCap = enumLineCaps.LINE_CAP_SQUARE,
                                                            nLowCap = enumLineCaps.LINE_CAP_SQUARE,
                                                            nTranspFactor = 50
                                                        });
                        }

                        // not clicked on wave display
                        if (!m_bClickedWaveAnalyzer)
                        {
                            var selected = m_selectedScheduledWord.StartTime + m_selectedScheduledWord.Duration - interval;

                            if ((m_selectedScheduledWord.StartTime.TotalMilliseconds + m_selectedScheduledWord.Duration.TotalMilliseconds) > 0)
                            {
                                m_LastSelections.Clear();
                                m_LastSelections.Add((int)selected.TotalMilliseconds);
                                m_LastSelections.Add((int)selected.TotalMilliseconds + (int)interval.TotalMilliseconds * 2);

                                timerSkipAnalyzerSelection.Enabled = true;

                                audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
                                    m_LastSelections[0],
                                    m_LastSelections[1]);

                                //draw range
                                //m_nRangeSelection = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemWaveRangeAdd("RANGE", "",
                                //    (int)selected.TotalMilliseconds, (int)selected.TotalMilliseconds + (int)interval.TotalMilliseconds * 2,
                                //    new WANALYZER_WAVE_RANGE
                                //    {
                                //        colorBack = Color.DarkSlateGray,
                                //        colorWaveLinePeak = Color.Green,
                                //        colorWaveLineCenter = Color.LimeGreen
                                //    });
                            }
                            else
                            {
                                //audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
                                //    (int)timePickerCurrentWord.Value.TotalMilliseconds,
                                //    (int)timePickerCurrentWord.Value.TotalMilliseconds);
                            }
                        }
                        else
                        {
                            timerSkipAnalyzerSelection.Enabled = true;

                            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
                                (int)(m_selectedScheduledWord.StartTime.TotalMilliseconds + m_selectedScheduledWord.Duration.TotalMilliseconds),
                                (int)(m_selectedScheduledWord.StartTime.TotalMilliseconds + m_selectedScheduledWord.Duration.TotalMilliseconds));

                        }

                        tsm_RemoveAnchor.Enabled = true;
                    }
                    else
                    {
                        // SELECTED SECTION
                        richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length;
                        richTextBox3.SelectionLength = m_selectedScheduledWord.Length;

                        //in case set start time during scheduling
                        if (n_hammerLastTimePressed != TimeSpan.Zero && audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
                        {
                            m_selectedScheduledWord.StartTime = n_hammerLastTimePressed;

                            Debug.WriteLine(string.Format("Setting '{0}' with start time : {1}", m_selectedScheduledWord.Content, m_selectedScheduledWord.StartTimeText));

                            n_hammerLastTimePressed = TimeSpan.Zero;
                        }
                        else if (audioDjStudio1.GetPlayerStatus(0) != AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
                        {
                            ////set current word start time when clicking word offline
                            //timePickerCurrentWord.Value = m_selectedScheduledWord.StartTime;
                            //LabelCurrentWordDuration.Text = audioSoundEditor1.FromMsToFormattedTime((int)m_selectedScheduledWord.Duration.TotalMilliseconds, true, true);
                            //timePickerCurrentWord.Enabled = true;
                        }

                        //move guid line
                        //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds);

                        if (audioDjStudio1.GetPlayerStatus(0) != AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
                        {
                            timerSkipAnalyzerSelection.Enabled = true;

                            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
                                (int)m_selectedScheduledWord.StartTime.TotalMilliseconds,
                                (int)m_selectedScheduledWord.StartTime.TotalMilliseconds + (int)m_selectedScheduledWord.Duration.TotalMilliseconds);

                        }
                    }

                    m_skipSelectionChange = false;
                }
            }

            PaintGraphics();

        }

        private void tbClipDescription_TextChanged(object sender, EventArgs e)
        {
            Clip.Current.Description = tbClipDescription.Text;
            RegenerateClipName(true);

        }

        private void tbClipRemarks_TextChanged(object sender, EventArgs e)
        {
            Clip.Current.Remarks = tbClipRemarks.Text;
            RegenerateClipName(true);

        }

        private void tbClipDescriptionEnglish_TextChanged(object sender, EventArgs e)
        {
            Clip.Current.EnglishDescription = tbClipDescriptionEnglish.Text;

        }

        private void comboCategory2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RegenerateClipName(true);

        }

        private void comboCategory3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RegenerateClipName(true);

            if (comboCategory3.SelectedIndex >= 0 && ((Category)comboCategory3.SelectedItem).MinPrice > 0)
            {
                lblMinValue.Text = string.Format(ResourceHelper.GetLabel("MIN_PRICE") + " {0:C}", ((Category)comboCategory3.SelectedItem).MinPrice);
                lblMinValue.Visible = true;
            }
            else
            {
                lblMinValue.Visible = false;
            }

        }

        private void comboCategory4_SelectionChangeCommitted(object sender, EventArgs e)
        {
            RegenerateClipName(true);

        }

        private void btnAddDate_Click(object sender, EventArgs e)
        {
            if (Clip.Current.ReadingDates.Contains(DateTime.ParseExact(dtpReadingDate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture)))
            {
                return;
            }

            Clip.Current.IsDirty = true;
            Clip.Current.ReadingDates.Add(DateTime.ParseExact(dtpReadingDate.Value.ToString("dd/MM/yyyy"), "dd/MM/yyyy", CultureInfo.InvariantCulture));
            RegenerateDatesBox();

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

        private async void buttonPublish_Click(object sender, EventArgs e)
        {
            if (audioSoundEditor1.GetSoundDuration() <= 0 && !m_admin)
            {
                // ask the user if he wants to go on
                MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_NO_SCHEDULING_EXISTS").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                return;
            }

            if ((string)comboClipType.SelectedValue != "piL85bMGtR" && !m_admin)
            {
                MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_NO_PERMISSIONS").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                return;
            }

            //clip type shiur
            if ((string)comboClipType.SelectedValue == "piL85bMGtR")
            {
                if (dtpReadingDate.Visible && listBoxDates.Items.Count == 0)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MANDATORY_FIELDS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }
                else if (dtpReadingDate.Visible)
                {
                    foreach (var d in Clip.Current.ReadingDates)
                    {
                        if (d < DateTime.Today)
                        {
                            MessageBox.Show(m_strings.Single(a => a.Key == "STD_READING_DATES_NOT_VALID").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                            return;
                        }
                    }

                }

                if (comboCategory3.Visible && comboCategory3.SelectedIndex < 0)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MANDATORY_FIELDS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }

                if (comboCategory4.Visible && comboCategory4.SelectedIndex < 0)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MANDATORY_FIELDS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }

                if (comboVoicePrompt.SelectedIndex < 0)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MANDATORY_FIELDS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }

                if (comboCategory3.Visible && ((Category)comboCategory3.SelectedItem).MinPrice > numericPrice.Value)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MIN_PRICE_ERROR"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }

                //if (((Category)comboCategory3.SelectedItem).MinPrice > numericPriceSupport.Value)
                //{
                //    MessageBox.Show(ResourceHelper.GetLabel("MIN_PRICE_SUPPORT_ERROR"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                //    return;
                //}

            }

            //if ((string)comboClipType.SelectedValue == "piL85bMGtR")
            //{
            //    if (MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_CONFIRM").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl) == System.Windows.Forms.DialogResult.No)
            //    {
            //        return;
            //    }
            //}
            //else
            //{
            //    if (MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_CONFIRM").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl) == System.Windows.Forms.DialogResult.No)
            //    {
            //        return;
            //    }
            //}

            if (Clip.Current.IsNew)
            {
                // ask the user if he wants to go on
                MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_NOT_SAVED").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                mnuFile_Save_Click(null, new EventArgs());
                return;
            }
            else
            {
                if (!Clip.Current.IsNew && !m_whileLoadingClip && tabControl1.SelectedIndex == 3)
                {
                    Save(true, true, this.tbClipTitle.Text);
                }

            }

            if (MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_UPDATE_CONFIRM").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            m_bInPublishProgress = true;
            groupBox3.Enabled = false;
            pictureBox2.Visible = true;
            progressBar1.Visible = false;
            buttonPublishUpdate.Enabled = false;
            buttonPublishNew.Enabled = false;
            Application.DoEvents();

            SavePropertiesToClip();
            try
            {
                if (Clip.Current.SaveJson(Clip.Current.ExtractJson()) && Clip.Current.ExtractHtml((msg) =>
                {
                    LabelStatus.Invoke((MethodInvoker)(() =>
                    {
                        LabelStatus.Text = msg;
                    }));
                }))
                {
                    if (Clip.Current.Publish(audioSoundEditor1, (msg) =>
                    {
                        LabelStatus.Invoke((MethodInvoker)(() =>
                        {
                            LabelStatus.Text = msg;
                        }));
                    }))
                    {
                        LabelStatus.Text = "Uploading to cloud...please wait";

                        bool result = await Clip.Current.UploadAsync(new Progress<ParseUploadProgressEventArgs>(ev =>
                        {
                            progressBar1.Value = Convert.ToInt32(ev.Progress * 100);
                        }));

                        if (result)
                        {
                            try
                            {
                                Clip.Current.Version = Convert.ToString(Convert.ToDouble(Clip.Current.Version) + 0.01);
                                mtbVersion.Text = Clip.Current.Version;

                                Save(false, true);
                            }
                            catch(Exception ex)
                            {
                                Program.Logger.Error(ex);
                            }

                            LabelStatus.Text = "Status: Idle";
                            progressBar1.Value = 0;
                            buttonPublishUpdate.Enabled = true;
                            buttonPublishNew.Enabled = true;
                            pictureBox2.Visible = false;
                            progressBar1.Visible = true;
                            MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_SUCCESFULLY").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                MessageBox.Show(string.Format("Error :\n\n{0}", ex.ToString()), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
            }
            finally
            {
                pictureBox2.Visible = false;
                progressBar1.Visible = true;
                buttonPublishUpdate.Enabled = true;
                groupBox3.Enabled = true;
                m_bInPublishProgress = false;

            }

        }

        private async void comboClipType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string lessonType = "HPz65WBzhw"; //clip

            // clip
            if (comboClipType.SelectedValue != null && (string)comboClipType.SelectedValue == "enaWrne5xe") //source
            {
                groupBox5.Visible = false;
                groupBox6.Visible = false;

                groupPerformer.Visible = false;
                lblVoicePrompt.Visible = false;
                lblAsterikVoicePrompt.Visible = false;
                comboVoicePrompt.Visible = false;

                lessonType = "0y8A4XTNeR"; //source

                //tbClipDescription.Visible = false;
                //lblDescription.Visible = false;

                numericPrice.Enabled = false;
                numericPrice.Value = 0;
                lblPrice.Visible = false;
                lblPriceSupport.Visible = false;
                lblPriceSupportCurrency.Visible = false;
                lblAsterikPrice.Visible = false;
                numericPrice.Visible = false;
                numericPriceSupport.Visible = false;
                lblMinValue.Visible = false;

                tbKeywords.Visible = false;
                lblKeywords.Visible = false;

                dtpReadingDate.Visible = false;
                lblReadingDates.Visible = false;
                lblAsterikReadingDates.Visible = false;
                btnAddDate.Visible = false;
                btnRemoveDate.Visible = false;
                listBoxDates.Visible = false;

                comboCategory4.Visible = false;
                lblCategory4.Visible = false;
                lblAsterikCategory4.Visible = false;
            }
            else
            {
                groupBox5.Visible = true;
                groupBox6.Visible = true;

                groupPerformer.Visible = true;
                lblVoicePrompt.Visible = true;
                lblAsterikVoicePrompt.Visible = true;
                comboVoicePrompt.Visible = true;

                if (m_contentType.Get<bool>("readingDates"))
                {
                    btnAddDate.Visible = true;
                    btnRemoveDate.Visible = true;
                    listBoxDates.Visible = true;
                    dtpReadingDate.Visible = true;
                    lblAsterikReadingDates.Visible = true;
                    lblReadingDates.Visible = true;
                }

                //tbClipDescription.Visible = true;
                //lblDescription.Visible = true;
                numericPrice.Enabled = true;
                lblPrice.Visible = true;
                lblPriceSupport.Visible = true;
                lblPriceSupportCurrency.Visible = true;
                lblAsterikPrice.Visible = false;
                numericPrice.Visible = true;
                numericPriceSupport.Visible = true;
                lblMinValue.Visible = true;

                tbKeywords.Visible = true;
                lblKeywords.Visible = true;

                if (comboCategory4.Items.Count > 0)
                {
                    comboCategory4.Visible = true;
                    lblCategory4.Visible = true;
                    lblAsterikCategory4.Visible = true;
                }
            }

            bool doDirty = false;

            doDirty = !m_whileLoadingClip;

            this.comboCategory3.DisplayMember = MyMentor.Properties.Settings.Default.CultureInfo.ToLower() == "he-il" ? "HebrewValue" : "EnglishValue";
            this.comboCategory3.ValueMember = "ObjectId";
            this.comboCategory3.DataSource = (await ParseTables.GetCategory3(m_contentType.ObjectId, lessonType)).Select(c => new Category
            {
                ObjectId = c.ObjectId,
                HebrewValue = c.ContainsKey("value_he_il") ? c.Get<string>("value_he_il") : string.Empty,
                EnglishValue = c.ContainsKey("value_en_us") ? c.Get<string>("value_en_us") : string.Empty,
                MinPrice = (decimal)c.Get<float>("minPrice")

            }).ToList();

            if (Clip.Current.Category3 != null)
            {
                comboCategory3.SelectedValue = Clip.Current.Category3;
            }
            else
            {
                comboCategory3.SelectedIndex = -1;
            }

            RegenerateClipName(doDirty);

        }

        private void buttonAutoDevide_Click(object sender, EventArgs e)
        {
            var parDelimiters = comboBoxAutoDevidePar.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(w => w).ToList<string>();
            var senDelimiters = comboBoxAutoDevideSen.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(w => w).ToList<string>();

            if (parDelimiters.Count() == 0 && senDelimiters.Count() == 0 && string.IsNullOrEmpty(comboBoxAutoDevideWor.Text))
            {
                MessageBox.Show(ResourceHelper.GetLabel("SOURCE_NEED_TO_SELECT_OPTION"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                return;
            }

            if (parDelimiters.Exists(a => a == "ENTER") && senDelimiters.Exists(a => a == "ENTER"))
            {
                MessageBox.Show(ResourceHelper.GetLabel("SOURCE_UNLEGAL_OPTION"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                return;
            }

            if (parDelimiters.Exists(a => a == "שני ENTER" || a == "2th ENTER") && senDelimiters.Exists(a => a == "שני ENTER" || a == "2th ENTER"))
            {
                MessageBox.Show(ResourceHelper.GetLabel("SOURCE_UNLEGAL_OPTION"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                return;
            }

            richTextBox2.Rtf = richTextBox1.Rtf;
            m_skipManuallyAnchorsValidation = true;

            try
            {
                int found = 0;
                int devition = 0;

                //sentenses enter
                if (!string.IsNullOrEmpty(comboBoxAutoDevideSen.Text))
                {
                    found = 0;
                    devition = 0;

                    if (parDelimiters.Exists(a => a == "אחרי 2 משפטים" || a == "AFTER 2 SENTENCES"))
                    {
                        devition = 2;
                    }
                    else if (parDelimiters.Exists(a => a == "אחרי 3 משפטים" || a == "AFTER 3 SENTENCES"))
                    {
                        devition = 3;
                    }
                    else if (parDelimiters.Exists(a => a == "אחרי 4 משפטים" || a == "AFTER 4 SENTENCES"))
                    {
                        devition = 4;
                    }

                    string textToFind = string.Empty;

                    switch (comboBoxAutoDevideSen.Text)
                    {
                        case "ENTER":
                            textToFind = "\r";
                            break;
                        case "COLON (:)":
                        case "נקודותיים (:)":
                            textToFind = ":";
                            break;
                        case "DOT (.)":
                        case "נקודה (.)":
                            textToFind = ".";
                            break;
                        default:
                            textToFind = comboBoxAutoDevideSen.Text;
                            break;
                    }

                    richTextBox2.SelectionStart = 0;
                    int idx = richTextBox2.Find(textToFind, 0, RichTextBoxFinds.MatchCase);

                    while (idx >= 0)
                    {
                        found++;

                        if (devition > 0 && found % devition == 0)
                        {
                            AddAnchor(AnchorType.Paragraph, idx + 1, true);
                        }
                        else
                        {
                            AddAnchor(AnchorType.Sentence, idx + 1, true);
                        }

                        if (idx + 4 + textToFind.Length < richTextBox2.TextLength)
                        {
                            idx = richTextBox2.Find(textToFind, idx + textToFind.Length + 4, RichTextBoxFinds.MatchCase);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (textToFind == ":")
                    {
                        textToFind = "׃";
                        richTextBox2.SelectionStart = 0;
                        idx = richTextBox2.Find(textToFind, 0, RichTextBoxFinds.MatchCase);

                        while (idx >= 0)
                        {
                            found++;

                            if (devition > 0 && found % devition == 0)
                            {
                                AddAnchor(AnchorType.Paragraph, idx + 1, true);
                            }
                            else
                            {
                                AddAnchor(AnchorType.Sentence, idx + 1, true);
                            }

                            if (idx + 4 + textToFind.Length < richTextBox2.TextLength)
                            {
                                idx = richTextBox2.Find(textToFind, idx + textToFind.Length + 4, RichTextBoxFinds.MatchCase);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }

                //paragraph enter
                if (!string.IsNullOrEmpty(comboBoxAutoDevidePar.Text) &&
                    !parDelimiters.Exists(a => a == "אחרי 2 משפטים" || a == "AFTER 2 SENTENCES") &&
                    !parDelimiters.Exists(a => a == "אחרי 3 משפטים" || a == "AFTER 3 SENTENCES") &&
                    !parDelimiters.Exists(a => a == "אחרי 4 משפטים" || a == "AFTER 4 SENTENCES"))
                {
                    found = 0;
                    devition = (parDelimiters.Exists(a => a == "שני ENTER" || a == "2th ENTER")) ? 2 : 1;

                    string textToFind = string.Empty;

                    switch (comboBoxAutoDevidePar.Text)
                    {
                        case "ENTER":
                        case "שני ENTER":
                        case "2th ENTER":
                            textToFind = "\r";
                            break;
                        default:
                            textToFind = comboBoxAutoDevidePar.Text;
                            break;
                    }

                    richTextBox2.SelectionStart = 0;
                    int idx = richTextBox2.Find(textToFind, 0, RichTextBoxFinds.MatchCase);

                    while (idx >= 0)
                    {
                        found++;

                        if (found % devition == 0)
                        {
                            AddAnchor(AnchorType.Paragraph, idx + (textToFind == "\r"  ? 0 : textToFind.Trim().Length), true);
                        }

                        if (idx + 3 + textToFind.Length < richTextBox2.TextLength)
                        {
                            idx = richTextBox2.Find(textToFind, idx + textToFind.Length + 3, RichTextBoxFinds.MatchCase);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

         

                //replace words
                if (!string.IsNullOrEmpty(comboBoxAutoDevideWor.Text))
                {
                    string textToFind = comboBoxAutoDevideWor.Text == "רווח" || comboBoxAutoDevideWor.Text == "SPACE" ? " " : comboBoxAutoDevideWor.Text;
                    richTextBox2.SelectionStart = 0;
                    int idx = richTextBox2.Find(textToFind, 0, RichTextBoxFinds.MatchCase);

                    while (idx >= 0)
                    {
                        AddAnchor(AnchorType.Word, idx + textToFind.Length, true);

                        if (idx + textToFind.Length + 3 < richTextBox2.TextLength)
                        {
                            idx = richTextBox2.Find(textToFind, idx + textToFind.Length + 3, RichTextBoxFinds.MatchCase);
                        }
                        else
                        {
                            break;
                        }
                    }
                }


                richTextBox1.Rtf = richTextBox2.Rtf;
            }
            catch(Exception ex)
            {
                Program.Logger.Error(ex);
            }
            finally
            {
                m_skipManuallyAnchorsValidation = false;
            }
        }


        private void SearchStringInText(string textToFind, ref int found, ref int devition)
        {
            richTextBox2.SelectionStart = 0;
            int idx = richTextBox2.Find(textToFind, 0, RichTextBoxFinds.None);

            while (idx >= 0)
            {
                found++;

                if (devition > 0 && found % devition == 0)
                {
                    AddAnchor(AnchorType.Paragraph, idx, true);
                }
                else
                {
                    AddAnchor(AnchorType.Sentence, idx, true);
                }

                if (idx + 3 + textToFind.Length < richTextBox2.TextLength)
                {
                    idx = richTextBox2.Find(textToFind, idx + textToFind.Length + 3, RichTextBoxFinds.None);
                }
                else
                {
                    break;
                }
            }
        }

        private void AddAnchor(AnchorType type, int index)
        {
            AddAnchor(type, index, false);
        }

        private void AddAnchor(AnchorType type, int index, bool isAutoDevide)
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

            string remember = richTextBox2.Text;

            while (index < remember.Length - 1 &&
                remember.Substring(index, 1) != " " &&
                remember.Substring(index, 1) != ":" &&
                remember.Substring(index, 1) != "׃" &&
                remember.Substring(index, 1) != "\n")
            {
                index++;

                if (remember.Substring(index, 1) == "]")
                {
                    index = index + 3;
                    break;
                }
            }

            richTextBox2.SelectionStart = index;
            richTextBox2.SelectionLength = richTextBox2.TextLength - index;
            if (!string.IsNullOrEmpty(richTextBox2.SelectedText.ClearSpacesAndBreakLines()))
            {
                if (index >= 3)
                {
                    richTextBox2.SelectionStart = index - 3;
                    richTextBox2.SelectionLength = 3;

                    if (richTextBox2.SelectedText == "[3]" || richTextBox2.SelectedText == "[2]" || richTextBox2.SelectedText == "[1]" || richTextBox2.SelectedText == "[0]")
                    {
                        if (isAutoDevide && type == AnchorType.Word)
                        {
                            return;
                        }

                        richTextBox2.SelectedText = anchor;
                        return;
                    }
                }

                if (index >= 4)
                {
                    richTextBox2.SelectionStart = index - 4;
                    richTextBox2.SelectionLength = 3;

                    if (richTextBox2.SelectedText == "[3]" || richTextBox2.SelectedText == "[2]" || richTextBox2.SelectedText == "[1]" || richTextBox2.SelectedText == "[0]")
                    {
                        if (isAutoDevide && type == AnchorType.Word)
                        {
                            return;
                        }

                        richTextBox2.SelectedText = anchor;
                        return;
                    }
                }

                if (index >= 5)
                {
                    richTextBox2.SelectionStart = index - 5;
                    richTextBox2.SelectionLength = 3;

                    if (richTextBox2.SelectedText == "[3]" || richTextBox2.SelectedText == "[2]" || richTextBox2.SelectedText == "[1]" || richTextBox2.SelectedText == "[0]")
                    {
                        if (isAutoDevide && type == AnchorType.Word)
                        {
                            return;
                        }

                        richTextBox2.SelectedText = anchor;
                        return;
                    }
                }

                richTextBox2.SelectionStart = index;
                richTextBox2.SelectionLength = 3;

                if (richTextBox2.SelectedText == "[3]" || richTextBox2.SelectedText == "[2]" || richTextBox2.SelectedText == "[1]" || richTextBox2.SelectedText == "[0]")
                {
                    if (isAutoDevide && type == AnchorType.Word)
                    {
                        return;
                    }

                    richTextBox2.SelectedText = anchor;
                }
                else
                {
                    richTextBox2.SelectionLength = 0;
                    richTextBox2.SelectedText = anchor;
                }
            }

        }

        private void tbKeywords_TextChanged(object sender, EventArgs e)
        {
            Clip.Current.Keywords = tbKeywords.Text;
            RegenerateClipName(true);
        }

        private void trackBarVolume1_ValueChanged(object sender, EventArgs e)
        {
            var error = audioDjStudio1.MixerVolumeSet(0, AudioDjStudio.enumComponentTypes.COMPONENTTYPE_SRC_WAVEOUT, trackBarVolume1.Value);

        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            audioSoundRecorder1.SetInputDeviceChannelVolume(0, 0, (Int16)trackBar1.Value);

        }

        private void richTextBox3_VScroll(object sender, EventArgs e)
        {
            PaintGraphics();

        }

        private void tsm_RemoveAnchor_Click(object sender, EventArgs e)
        {
            var selectedWord = m_selectedScheduledWord.NextWord;

            while (selectedWord != null)
            {
                selectedWord.StartTime = TimeSpan.Zero;
                selectedWord.Duration = TimeSpan.Zero;
                selectedWord = selectedWord.NextWord;
            }

            Clip.Current.Chapter.LastWord.Duration = TimeSpan.Zero;

            RemoveWaveFormGraphics();
            PaintWaveFormGraphics();

            tsm_RemoveAnchor.Enabled = false;
        }

        private void timerFirstAnchorScheduling_Tick(object sender, EventArgs e)
        {
            timerFirstAnchorScheduling.Enabled = false;

            if (Clip.Current.Chapter.Paragraphs != null)
            {
                if (Clip.Current.Chapter.FirstWord != null && 
                    Clip.Current.Chapter.FirstWord.StartTime == TimeSpan.Zero)
                {
                    tsbHammer_Click(null, new EventArgs());
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            Int32 nBeginPosInMs = 0;
            Int32 nEndPosInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            int center = 0;

            if (nBeginSelectionInMs != nEndSelectionInMs)
            {
                center = ((nEndSelectionInMs - nBeginSelectionInMs) / 2) + nBeginSelectionInMs;
            }
            else
            {
                center = nBeginSelectionInMs;
            }

            audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nBeginPosInMs, ref nEndPosInMs);

            nBeginPosInMs = center - (int)((nEndPosInMs - nBeginPosInMs) * 0.35);
            nEndPosInMs = center + (int)((nEndPosInMs - nBeginPosInMs) * 0.35);

            audioSoundEditor1.DisplayWaveformAnalyzer.SetDisplayRange(nBeginPosInMs, nEndPosInMs);
        }

        private void tsbStartRecordNew_Click(object sender, EventArgs e)
        {
            if (audioSoundRecorder1.Status == AudioSoundRecorder.enumRecorderStates.RECORD_STATUS_RECORDING ||
                audioSoundRecorder1.Status == AudioSoundRecorder.enumRecorderStates.RECORD_STATUS_RECORDING_MEMORY)
            {
                audioSoundRecorder1.Stop();
                return;
            }


            // check if we already have an editing session in memory
            DialogResult result;
            if (audioSoundEditor1.GetSoundDuration() > 0)
            {
                // ask the user if he wants to go on
                result = MessageBox.Show(ResourceHelper.GetLabel("OVERWRITE_AUDIO_CONFIRM"), "MyMentor", MessageBoxButtons.YesNo);
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

        private void toolStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void RefreshToolstripItemsState()
        {
        }

        private void tsbContinueRecord_Click(object sender, EventArgs e)
        {
            if (audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
            {
                audioDjStudio1.PauseSound(0);
            }
            else if (audioSoundRecorder1.Status == AudioSoundRecorder.enumRecorderStates.RECORD_STATUS_RECORDING ||
                audioSoundRecorder1.Status == AudioSoundRecorder.enumRecorderStates.RECORD_STATUS_RECORDING_CLIPBOARD)
            {
                audioSoundRecorder1.Stop();
            }
            else
            {
                audioDjStudio1.LoadSoundFromEditingSession(0, audioSoundEditor1.Handle);
                tsbContinueRecord.Image = imageList1.Images[4];

                // get the position selected on the waveform analyzer, if any
                bool bSelectionAvailable = false;
                Int32 nBeginSelectionInMs = 0;
                Int32 nEndSelectionInMs = 0;
                audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

                m_bRecAppendMode = true;
                m_bContinueRecordingAfterPlayback = true;

                // play selected range only
                var error = audioDjStudio1.PlaySound(0, nBeginSelectionInMs, -1);
            }
        }

        private void timerPlayerIcon_Tick(object sender, EventArgs e)
        {
            if (b_recordIconRed)
            {
                tsbStartRecordNew.Image = imageList1.Images[1];
            }
            else
            {
                tsbStartRecordNew.Image = imageList1.Images[0];
            }
            b_recordIconRed = !b_recordIconRed;
        }

        private void tsbPlay_Click(object sender, EventArgs e)
        {
            if (audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
            {
                audioDjStudio1.StopSound(0);
                //audioDjStudio1.PauseSound(0);

                if (tabControl1.SelectedIndex == 2)
                {
                    timerRecordIcon.Enabled = false;
                    timerUpdateDuringSchedulingPlayback.Enabled = false;
                }
            }
            else if (audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PAUSED)
            {
                tsbPlay.Image = imageList1.Images[7];

                audioDjStudio1.ResumeSound(0);

                if (tabControl1.SelectedIndex == 2)
                {
                    timerRecordIcon.Enabled = true;
                    timerUpdateDuringSchedulingPlayback.Enabled = true;
                    richTextBox3.Focus();
                }
            }
            else
            {
                m_bContinueRecordingAfterPlayback = false;

                tsbPlay.Image = imageList1.Images[7];

                audioDjStudio1.LoadSoundFromEditingSession(0, audioSoundEditor1.Handle);

                // get the position selected on the waveform analyzer, if any
                bool bSelectionAvailable = true;
                Int32 nBeginSelectionInMs = 0;
                Int32 nEndSelectionInMs = 0;
                audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

                //lock selection
                audioSoundEditor1.DisplayWaveformAnalyzer.MouseSelectionEnable(false);//.SetSelection(false, nBeginSelectionInMs, nEndSelectionInMs);

                // if a selection is available
                if (bSelectionAvailable)
                {
                    //anchor selected
                    if (
                        (m_selectedGraphicItemSelected > 0) 
                        && m_LastSelections.Count() > 0)
                    {

                        //check if inside a selection range
                        if (nBeginSelectionInMs == nEndSelectionInMs && nBeginSelectionInMs >= m_LastSelections[0] - 1
                            && nEndSelectionInMs <= m_LastSelections[1] + 1)
                        {
                            //set end point
                            nEndSelectionInMs = m_LastSelections[1];
                        }
                        else
                        {
                            //grab range selection
                            nBeginSelectionInMs = m_LastSelections[0];
                            nEndSelectionInMs = m_LastSelections[1];
                        }
                    }
                    // normal selection
                    else if (nBeginSelectionInMs != nEndSelectionInMs)
                    {
                        //m_nRangeSelection = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemWaveRangeAdd("RANGE", "",
                        //    nBeginSelectionInMs, nEndSelectionInMs,
                        //    new WANALYZER_WAVE_RANGE
                        //    {
                        //        colorBack = Color.DarkSlateGray,
                        //        colorWaveLinePeak = Color.Green,
                        //        colorWaveLineCenter = Color.LimeGreen
                        //    });

                        audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, nBeginSelectionInMs, nEndSelectionInMs);

                        m_LastSelections.Clear();
                        m_LastSelections.Add(nBeginSelectionInMs);
                        m_LastSelections.Add(nEndSelectionInMs);
                    }

                    // play selected range only
                    var error = audioDjStudio1.PlaySound(0, nBeginSelectionInMs, nEndSelectionInMs == nBeginSelectionInMs ? -1 : nEndSelectionInMs);

                    timerUpdateDuringSchedulingPlayback.Enabled = true;
                }

                //scheduling tabs
                if (tabControl1.SelectedIndex == 2)
                {
                    timerRecordIcon.Enabled = true;
                    n_hammerLastTimePressed = TimeSpan.Zero;
                    richTextBox3.Focus();

                    if (nBeginSelectionInMs == 0)
                    {
                        timerFirstAnchorScheduling.Enabled = true;
                    }
                }
            }

        }

        private void tsbStop_Click(object sender, EventArgs e)
        {
            if (audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
            {
                audioDjStudio1.PauseSound(0);

                if (tabControl1.SelectedIndex == 2)
                {
                    timerRecordIcon.Enabled = false;
                    timerUpdateDuringSchedulingPlayback.Enabled = false;
                }
            }
        }

        private void tsbForward_Click(object sender, EventArgs e)
        {
            int nBeginPosInMs = 0;
            int nEndPosInMs = 0;

            audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nBeginPosInMs, ref nEndPosInMs);

            var buffer = (nEndPosInMs - nBeginPosInMs) / 100;

            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, Math.Min(nBeginSelectionInMs + buffer, audioSoundEditor1.GetSoundDuration()),
                Math.Min(nEndSelectionInMs + buffer, audioSoundEditor1.GetSoundDuration()));
        }

        private void tsbRewind_Click(object sender, EventArgs e)
        {
            int nBeginPosInMs = 0;
            int nEndPosInMs = 0;

            audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nBeginPosInMs, ref nEndPosInMs);

            var buffer = (nEndPosInMs - nBeginPosInMs) / 100;

            bool bSelectionAvailable = false;
            Int32 nBeginSelectionInMs = 0;
            Int32 nEndSelectionInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, Math.Max(nBeginSelectionInMs - buffer, 0), Math.Max(nEndSelectionInMs - buffer, 0));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FormReadyTexts form = new FormReadyTexts(this))
            {
                if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    if (!string.IsNullOrEmpty(richTextBox1.Text.Trim()) &&
                        MessageBox.Show(m_strings.Single(a => a.Key == "STD_SOURCE_TEXT_ARE_YOU_SURE").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }

                    richTextBox1.Rtf = form.SelectedSource.Rtf;

                    if (form.SelectedSource.Category1 != null)
                    {
                        comboCategory1.SelectedValue = form.SelectedSource.Category1.ObjectId;

                        comboCategory1_SelectionChangeCommitted(null, new EventArgs());
                    }

                    if (form.SelectedSource.Category2 != null)
                    {
                        comboCategory2.SelectedValue = form.SelectedSource.Category2.ObjectId;
                    }

                    if (form.SelectedSource.Category3 != null)
                    {
                        comboCategory3.SelectedValue = form.SelectedSource.Category3.ObjectId;
                    }

                    tbClipDescription.Text = form.SelectedSource.Description;
                    tbClipDescriptionEnglish.Text = form.SelectedSource.DescriptionEnglish;

                    tbClipRemarks.Text = form.SelectedSource.Remarks;
                    tbClipRemarksEnglish.Text = form.SelectedSource.RemarksEnglish;

                    MessageBox.Show(m_strings.Single(a => a.Key == "STD_AFTER_SOURCE_SELECTION").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);

                }
            }

        }

        private void menuLanguages_Click(object sender, EventArgs e)
        {
            toolsToolStripMenuItem.ShowDropDown();
            languageToolStripMenuItem.ShowDropDown();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            trackBarPitch1.Value = 0;
        }

        private void trackBarPitch_ValueChanged(object sender, EventArgs e)
        {
            //audioDjStudio1.SetPitch(0, (short)trackBarPitch.Value);
        }

        private void trackBarPitch1_ValueChanged(object sender, EventArgs e)
        {
            audioDjStudio1.SetTempoPerc(0, (float)trackBarPitch1.Value);
            label1.Text = trackBarPitch1.Value.ToString();
        }

        private void ClickAnchor(AnchorType type, int selectionIndex)
        {
            ClickAnchor(type, selectionIndex, false);
        }

        private void ClickAnchor(AnchorType type, int selectionIndex, bool hideChanges)
        {
            string selectedAnchor = string.Empty;
            var othersAnchor = new List<string>();
            Dictionary<int, Word> newAnchorsStack = new Dictionary<int, Word>();

            switch (type)
            {
                case AnchorType.Paragraph:
                    selectedAnchor = "[3]";
                    othersAnchor.AddRange(new string[] { "[2]", "[1]", "[0]" });
                    break;
                case AnchorType.Sentence:
                    selectedAnchor = "[2]";
                    othersAnchor.AddRange(new string[] { "[3]", "[1]", "[0]" });
                    break;
                case AnchorType.Section:
                    selectedAnchor = "[1]";
                    othersAnchor.AddRange(new string[] { "[3]", "[2]", "[0]" });
                    break;
                case AnchorType.Word:
                    selectedAnchor = "[0]";
                    othersAnchor.AddRange(new string[] { "[3]", "[2]", "[1]" });
                    break;

            }

            if (richTextBox2.SelectedText == selectedAnchor)
            {
                if (Clip.Current.Chapter.FirstWord != null && Clip.Current.Chapter.FirstWord.StartTime > TimeSpan.Zero)
                {
                    var lastWord = Clip.Current.Chapter.Words.Where(word =>
                        word.RealCharIndex <= selectionIndex).LastOrDefault();

                    if (lastWord != null && lastWord.NextWord != null)
                    {
                        RemoveWaveFormGraphics();

                        if (lastWord.PreviousWord == Clip.Current.Chapter.FirstWord)
                        {
                            lastWord.PreviousWord.StartTime = TimeSpan.Zero;
                        }

                        lastWord.Index = -1;

                        Word next = lastWord.NextWord;
                        while (next != null)
                        {
                            next.Index = next.Index - 1;
                            next = next.NextWord;
                        }

                        newAnchorsStack = null;
                    }
                    else if (lastWord != null)
                    {
                        RemoveWaveFormGraphics();

                        // add new anchor to the stack
                        newAnchorsStack.Add(lastWord.Index - 1, new Word
                        {
                            StartTime = lastWord.PreviousWord.StartTime,
                            //Duration = lastWord.Duration
                        });

                        //if (lastWord == Clip.Current.Chapter.LastWord && 
                        //    lastWord.Duration > TimeSpan.Zero)
                        //{
                        //    //newAnchorsStack[newAnchorsStack.Count() - 1].Duration = lastWord.Duration;
                        //}

                        lastWord.Index = -1;

                    }
                }

                    richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                    richTextBox1.SelectionLength = 3;
                    richTextBox1.SelectedText = "";
                    Clip.Current.Devide(newAnchorsStack);

            }
            else if (othersAnchor.Contains(  richTextBox2.SelectedText ))
            {
                richTextBox1.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
                richTextBox1.SelectionLength = 3;
                richTextBox1.SelectedText = selectedAnchor;
            }
            else
            {
                if (Clip.Current.Chapter.FirstWord != null && Clip.Current.Chapter.FirstWord.StartTime > TimeSpan.Zero)
                {
                    var lastWord = Clip.Current.Chapter.Words.Where(word =>
                        word.RealCharIndex < selectionIndex).LastOrDefault();

                    if (lastWord != null && lastWord.NextWord != null)
                    {
                        Word next = lastWord.NextWord;
                        while (next != null)
                        {
                            next.Index = next.Index + 1;
                            next = next.NextWord;
                        }

                        // add new anchor to the stack
                        newAnchorsStack.Add(lastWord.Index + 1, new Word
                        {
                            StartTime = lastWord.StartTime.Add(new TimeSpan(0, 0, 0, 0, (int)lastWord.Duration.TotalMilliseconds / 2)),
                            NextWord = new Word()
                        });

                    }
                    else
                    {
                        //last word
                        if (lastWord != null && lastWord == Clip.Current.Chapter.LastWord)
                        {
                            //lastWord.Duration = new TimeSpan(0, 0, 0, 0, (int)(lastWord.Duration.TotalMilliseconds / 2));

                            // add new anchor to the stack
                            newAnchorsStack.Add(lastWord.Index + 1, new Word
                            {
                                StartTime = lastWord.StartTime.Add(new TimeSpan(0, 0, 0, 0, (int)lastWord.Duration.TotalMilliseconds / 2)),
                                Duration = new TimeSpan(0, 0, 0, 0, (int)(lastWord.Duration.TotalMilliseconds / 2)),
                                NextWord = null
                            });
                        }

                    }
                }

                if (!hideChanges)
                {
                    richTextBox1.SelectionStart = selectionIndex;
                    richTextBox1.SelectionLength = 0;
                    richTextBox1.SelectedText = selectedAnchor;

                    Clip.Current.Devide(newAnchorsStack);
                }
            }

            PaintGraphics();
        }

        private void richTextBox1_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionStart == 0 || richTextBox1.TextLength == 0)
            {
                return;
            }

            m_skipManuallyAnchorsValidation = true;
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

                if (selectionIndex > 0 &&
                    remember.Substring(selectionIndex, 1) == "[")
                {
                    selectionIndex = selectionIndex + 3;
                    break;
                }
            }

            //check another or same anchor
            richTextBox2.SelectionStart = selectionIndex - 3 < 0 ? 0 : selectionIndex - 3;
            richTextBox2.SelectionLength = 3;

            if (tbrParagraph.Checked)
            {
                ClickAnchor(AnchorType.Paragraph, selectionIndex);
            }

            if (tbrSentense.Checked)
            {
                ClickAnchor(AnchorType.Sentence, selectionIndex);
            }

            if (tbrSection.Checked)
            {
                ClickAnchor(AnchorType.Section, selectionIndex);
            }

            if (tbrWord.Checked)
            {
                ClickAnchor(AnchorType.Word, selectionIndex);
            }

            //Clip.Current.Devide();
            m_skipManuallyAnchorsValidation = false;

        }

        private void tsbHammer_Click(object sender, EventArgs e)
        {
            // PLAYBACK
            if (audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
            {
                Int32 nBeginSelectionInMs = 0;
                Int32 nEndSelectionInMs = 0;
                bool bSelection = true;

                audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelection, ref nBeginSelectionInMs, ref nEndSelectionInMs);

                n_hammerLastTimePressed = new TimeSpan(0, 0, 0, 0, nBeginSelectionInMs);

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
                    //save duration to last word
                    Clip.Current.Chapter.LastWord.Duration = saveIt - Clip.Current.Chapter.LastWord.StartTime;

                    if (m_endLineUniqueId >= 0)
                    {
                        //set new position
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_endLineUniqueId,
                            (int)saveIt.TotalMilliseconds, (int)saveIt.TotalMilliseconds);
                    }
                    else
                    {
                        //create new end anchor line
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

                    //buttonHammer.Enabled = false;
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
                            color = Color.DeepSkyBlue,
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

        private void richTextBox3_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void richTextBox3_DoubleClick(object sender, EventArgs e)
        {
            toolStripButton5_Click(null, new EventArgs());
        }

        private void tsbMoveAnchorForward_Click(object sender, EventArgs e)
        {
            int nBeginPosInMs = 0;
            int nEndPosInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nBeginPosInMs, ref nEndPosInMs);
            var buffer = (nEndPosInMs - nBeginPosInMs) / 100;

            if (m_selectedGraphicItemSelected >= 0)
            {
                var word = Clip.Current.Chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).SelectMany(sc => sc.Words).Where(w => w.GraphicItemUnique == m_selectedGraphicItemSelected).FirstOrDefault();

                if (word != null)
                {
                    TimeSpan max = TimeSpan.Zero;

                    if (word.NextWord != null && word.NextWord.StartTime > TimeSpan.Zero)
                    {
                        max = word.NextWord.StartTime.Subtract(new TimeSpan(0, 0, 0, 0, 500));

                        if (word.StartTime.Add(new TimeSpan(0, 0, 0, 0, buffer)) < max)
                        {
                            word.StartTime = word.StartTime.Add(new TimeSpan(0, 0, 0, 0, buffer));
                            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_selectedGraphicItemSelected,
                                (int)word.StartTime.TotalMilliseconds,
                                (int)word.StartTime.TotalMilliseconds);
                        }
                    }
                    // last word
                    else if (word.NextWord == null)
                    {
                        max = new TimeSpan(0, 0, 0, 0, audioSoundEditor1.GetSoundDuration());

                        if (word.StartTime.Add(new TimeSpan(0, 0, 0, 0, 500)).Add(word.Duration) < max)
                        {
                            word.StartTime = word.StartTime.Add(new TimeSpan(0, 0, 0, 0, buffer));
                            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_selectedGraphicItemSelected,
                                (int)word.StartTime.TotalMilliseconds,
                                (int)word.StartTime.TotalMilliseconds);
                        }

                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_endLineUniqueId,
                            (int)(word.StartTime + word.Duration).TotalMilliseconds,
                                (int)(word.StartTime + word.Duration).TotalMilliseconds);
                    }
                }
                // end line
                else if (m_selectedGraphicItemSelected == m_endLineUniqueId)
                {
                    var last_word = Clip.Current.Chapter.LastWord;

                    var max = new TimeSpan(0, 0, 0, 0, audioSoundEditor1.GetSoundDuration()).Subtract(new TimeSpan(0, 0, 0, 0, 500));

                    if (last_word.StartTime.Add(new TimeSpan(0, 0, 0, 0, 500)).Add(last_word.Duration) < max)
                    {
                        last_word.Duration = last_word.Duration.Add(new TimeSpan(0, 0, 0, 0, buffer));
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_endLineUniqueId,
                            (int)last_word.StartTime.TotalMilliseconds + (int)last_word.Duration.TotalMilliseconds,
                            (int)last_word.StartTime.TotalMilliseconds + (int)last_word.Duration.TotalMilliseconds);
                    }
                }
            }
        }

        private void tsbMoveAnchorRewind_Click(object sender, EventArgs e)
        {
            int nBeginPosInMs = 0;
            int nEndPosInMs = 0;
            audioSoundEditor1.DisplayWaveformAnalyzer.GetDisplayRange(ref nBeginPosInMs, ref nEndPosInMs);
            var buffer = (nEndPosInMs - nBeginPosInMs) / 100;

            if (m_selectedGraphicItemSelected >= 0)
            {
                var word = Clip.Current.Chapter.Paragraphs.SelectMany(p => p.Sentences).SelectMany(se => se.Sections).SelectMany(sc => sc.Words).Where(w => w.GraphicItemUnique == m_selectedGraphicItemSelected).FirstOrDefault();

                if (word != null)
                {
                    TimeSpan min = TimeSpan.Zero;

                    if (word.PreviousWord != null && word.PreviousWord.StartTime > TimeSpan.Zero)
                    {
                        min = word.PreviousWord.StartTime.Add(new TimeSpan(0, 0, 0, 0, 500));

                        if (word.StartTime.Subtract(new TimeSpan(0, 0, 0, 0, 500)) > min)
                        {
                            word.StartTime = word.StartTime.Subtract(new TimeSpan(0, 0, 0, 0, buffer));
                            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_selectedGraphicItemSelected,
                                (int)word.StartTime.TotalMilliseconds,
                                (int)word.StartTime.TotalMilliseconds);
                        }
                    }
                    else if (word == Clip.Current.Chapter.FirstWord)
                    {
                        min = new TimeSpan(0, 0, 0, 0, 500);

                        if (word.StartTime.Subtract(new TimeSpan(0, 0, 0, 0, 500)) > min)
                        {
                            word.StartTime = word.StartTime.Subtract(new TimeSpan(0, 0, 0, 0, buffer));
                            audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_selectedGraphicItemSelected,
                                (int)word.StartTime.TotalMilliseconds,
                                (int)word.StartTime.TotalMilliseconds);
                        }
                    }

                    //if its last word also
                    if (word.NextWord == null)
                    {
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_endLineUniqueId,
                            (int)(word.StartTime + word.Duration).TotalMilliseconds,
                                (int)(word.StartTime + word.Duration).TotalMilliseconds);
                    }
                }
                else if (m_selectedGraphicItemSelected == m_endLineUniqueId)
                {
                    var last_word = Clip.Current.Chapter.LastWord;

                    var min = new TimeSpan(0, 0, 0, 0, (int)last_word.StartTime.TotalMilliseconds).Add(new TimeSpan(0, 0, 0, 0, 500));

                    if (last_word.StartTime.Subtract(new TimeSpan(0, 0, 0, 0, 500)).Add(last_word.Duration) > min)
                    {
                        last_word.Duration = last_word.Duration.Subtract(new TimeSpan(0, 0, 0, 0, buffer));
                        audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_endLineUniqueId,
                            (int)last_word.StartTime.TotalMilliseconds + (int)last_word.Duration.TotalMilliseconds,
                            (int)last_word.StartTime.TotalMilliseconds + (int)last_word.Duration.TotalMilliseconds);
                    }
                }

            }

        }

        private void audioSoundEditor1_KeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.Write(e.KeyChar);
        }

        private void richTextBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space && tabControl1.SelectedIndex == 2
                && audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
            {
                tsbHammer_Click(null, new EventArgs());
            }
            else
            {
                e.Handled = true;
                e.SuppressKeyPress = false;
            }
        }

        private void richTextBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.Write(e.KeyChar);
        }

        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void tsbCheckAnchor_Click(object sender, EventArgs e)
        {
            var interval = (int)(float.Parse(cbInterval.Text) * 1000);

            if (m_selectedGraphicItemSelected >= 0)
            {
                int nStartPosInMs = 0;
                int nEndPosInMs = 0;

                var selected = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionGet(m_selectedGraphicItemSelected, ref nStartPosInMs, ref nEndPosInMs);
                m_LastSelections.Clear();
                m_LastSelections.Add((int)(nStartPosInMs - interval));
                m_LastSelections.Add((int)(nStartPosInMs + interval));

                //audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
                //    m_LastSelections[0],
                //    m_LastSelections[1]);

                m_pauseOnAnchorGraphicItem = m_selectedGraphicItemSelected;

                tsbPlay_Click(null, new EventArgs());
            }
        }

        private void timerPlayAnchorCheckPause_Tick(object sender, EventArgs e)
        {
            timerPlayAnchorCheckPause.Enabled = false;
            audioDjStudio1.ResumeSound(0);
            TimerMenuEnabler.Enabled = true;
        }

        private void audioSoundRecorder2_VUMeterValueChange(object sender, AudioSoundRecorder.VUMeterValueChangeEventArgs e)
        {
            audioSoundRecorder2.GraphicBarsManager.SetValue(m_hWndVuMeterLeft2, e.nPeakLeft);
            audioSoundRecorder2.GraphicBarsManager.SetValue(m_hWndVuMeterRight2, e.nPeakRight);
        }

        private void FormStudio_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.VolumeLevel = trackBarVolume1.Value;
            Properties.Settings.Default.RecordingLevel = trackBar1.Value;
            Properties.Settings.Default.Save();
        }

        private void richTextBox1_MouseUp(object sender, MouseEventArgs e)
        {
            PaintGraphics();
        }

        private void tsbSave_Click(object sender, EventArgs e)
        {
            mnuFile_Save_Click(null, new EventArgs());

        }

        private void tbClipRemarksEnglish_TextChanged(object sender, EventArgs e)
        {
            Clip.Current.RemarksEnglish = tbClipRemarksEnglish.Text;
            RegenerateClipName(true);
        }

        private void textToolstripSelectAll_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void textToolstripCopy_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void textToolstripPaste_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Paste();
            }
            catch
            {

            }
        }

        private void textToolstripCut_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void textToolstripDeleteSelection_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.SelectedText = string.Empty;
            }
            catch
            {

            }
        }

        private void textToolstripDeselect_Click(object sender, EventArgs e)
        {
            richTextBox1.DeselectAll();
        }

        private void audioSoundEditor1_WaveAnalysisDone(object sender, WaveAnalysisDoneEventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomToFullSound();

            if (tabControl1.SelectedIndex == 2)
            {
                PaintWaveFormGraphics();
                audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();

            }

            if (m_LastRangeSelections.Count() > 0)
            {
                if (m_LastRangeSelections[0] != m_LastRangeSelections[1])
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetDisplayRange(m_LastRangeSelections[0], m_LastRangeSelections[1]);
                }

                if (m_LastSelections.Count() == 0)
                {
                    var center = m_LastRangeSelections[0] + ((m_LastRangeSelections[1] - m_LastRangeSelections[0]) / 2);
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, center, center);
                }
                else
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, m_LastSelections[0], m_LastSelections[1]);
                    m_LastSelections.Clear();
                }

                m_LastRangeSelections.Clear();
            }
        }

        private void tbPerformer_TextChanged(object sender, EventArgs e)
        {
            Clip.Current.Performer = tbPerformer.Text;
            RegenerateClipName(true);
        }

        private void tbPerformerEnglish_TextChanged(object sender, EventArgs e)
        {
            Clip.Current.EnglishPerformer = tbPerformerEnglish.Text;
            RegenerateClipName(true);
        }

        private void timerDeviderBlink_Tick(object sender, EventArgs e)
        {
            if (tbrParagraph.Checked)
            {
                tbrParagraph.BackgroundImageLayout = tbrParagraph.BackgroundImageLayout == ImageLayout.None ? ImageLayout.Tile : ImageLayout.None;
            }
            if (tbrSentense.Checked)
            {
                tbrSentense.BackgroundImageLayout = tbrSentense.BackgroundImageLayout == ImageLayout.None ? ImageLayout.Tile : ImageLayout.None;
            }
            if (tbrSection.Checked)
            {
                tbrSection.BackgroundImageLayout = tbrSection.BackgroundImageLayout == ImageLayout.None ? ImageLayout.Tile : ImageLayout.None;
            }
            if (tbrWord.Checked)
            {
                tbrWord.BackgroundImageLayout = tbrWord.BackgroundImageLayout == ImageLayout.None ? ImageLayout.Tile : ImageLayout.None;
            }
        }

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            //{
            //    e.Handled = true;
            //}
        }

        private void timerSkipAnalyzerSelection_Tick(object sender, EventArgs e)
        {
            timerSkipAnalyzerSelection.Enabled = false;
        }

        private void timerShowManuallyAnchorsValidationMessage_Tick(object sender, EventArgs e)
        {
            timerShowManuallyAnchorsValidationMessage.Enabled = false;

            MessageBox.Show(m_strings.Single(a => a.Key == "STD_MANUALLY_ANCHORS_VALIDATION").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);

        }

        private void FormStudio_KeyDown(object sender, KeyEventArgs e)
        {
            if (tabControl1.SelectedIndex == 1 || tabControl1.SelectedIndex == 2)
            {
                if (e.KeyData == Keys.H)
                {
                    e.SuppressKeyPress = false;
                    e.Handled = true;
                    HideShowWaveAnylazerSelection(true);

                } else if (e.KeyData == Keys.Space && tabControl1.SelectedIndex == 2
                    && audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
                {
                    tsbHammer_Click(null, new EventArgs());
                }
            }
        }

        private void FormStudio_KeyUp(object sender, KeyEventArgs e)
        {
            if (tabControl1.SelectedIndex == 1 || tabControl1.SelectedIndex == 2)
            {
                if (e.KeyData == Keys.H)
                {
                    e.SuppressKeyPress = false;
                    e.Handled = true;
                    HideShowWaveAnylazerSelection(false);
                }
            }
        }

        private void richTextBox3_Enter(object sender, EventArgs e)
        {
            toolStrip2.Focus();
        }

        private async void buttonPublishNew_Click(object sender, EventArgs e)
        {
            if (!Clip.Current.IsNew)
            {
                if (!Clip.Current.LastPublishedOn.HasValue)
                {
                    MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_NO_CHANGE").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }
                else
                {
                    Clip.Current.ID = Guid.NewGuid();
                }
            }

            if (audioSoundEditor1.GetSoundDuration() <= 0 && !m_admin)
            {
                // ask the user if he wants to go on
                MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_NO_SCHEDULING_EXISTS").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                return;
            }

            if ((string)comboClipType.SelectedValue != "piL85bMGtR" && !m_admin)
            {
                MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_NO_PERMISSIONS").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                return;
            }

            //clip type shiur
            if ((string)comboClipType.SelectedValue == "piL85bMGtR")
            {
                if (dtpReadingDate.Visible && listBoxDates.Items.Count == 0)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MANDATORY_FIELDS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }
                else if (dtpReadingDate.Visible)
                {
                    foreach (var d in Clip.Current.ReadingDates)
                    {
                        if (d < DateTime.Today)
                        {
                            MessageBox.Show(m_strings.Single(a => a.Key == "STD_READING_DATES_NOT_VALID").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                            return;
                        }
                    }

                }

                if (comboCategory3.Visible && comboCategory3.SelectedIndex < 0)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MANDATORY_FIELDS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }

                if (comboCategory4.Visible && comboCategory4.SelectedIndex < 0)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MANDATORY_FIELDS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }

                if (comboVoicePrompt.SelectedIndex < 0)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MANDATORY_FIELDS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }

                if (comboCategory3.Visible && ((Category)comboCategory3.SelectedItem).MinPrice > numericPrice.Value)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MIN_PRICE_ERROR"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                    return;
                }

                //if (((Category)comboCategory3.SelectedItem).MinPrice > numericPriceSupport.Value)
                //{
                //    MessageBox.Show(ResourceHelper.GetLabel("MIN_PRICE_SUPPORT_ERROR"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                //    return;
                //}

            }

            if (Clip.Current.IsNew)
            {
                // ask the user if he wants to go on
                MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_NOT_SAVED").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                mnuFile_Save_Click(null, new EventArgs());
                return;
            }
            else
            {
                if (!Clip.Current.IsNew && !m_whileLoadingClip && tabControl1.SelectedIndex == 3)
                {
                    Save(true, true, this.tbClipTitle.Text);
                }

            }

            if (MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_UPDATE_CONFIRM").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, m_msgOptionsRtl) == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            m_bInPublishProgress = true;
            groupBox3.Enabled = false;
            pictureBox2.Visible = true;
            progressBar1.Visible = false;
            buttonPublishUpdate.Enabled = false;
            buttonPublishNew.Enabled = false;
            Application.DoEvents();

            SavePropertiesToClip();
            try
            {
                if (Clip.Current.SaveJson(Clip.Current.ExtractJson()) && Clip.Current.ExtractHtml((msg) =>
                {
                    LabelStatus.Invoke((MethodInvoker)(() =>
                    {
                        LabelStatus.Text = msg;
                    }));
                }))
                {
                    if (Clip.Current.Publish(audioSoundEditor1, (msg) =>
                    {
                        LabelStatus.Invoke((MethodInvoker)(() =>
                        {
                            LabelStatus.Text = msg;
                        }));
                    }))
                    {
                        LabelStatus.Text = "Uploading to cloud...please wait";

                        bool result = await Clip.Current.UploadAsync(new Progress<ParseUploadProgressEventArgs>(ev =>
                        {
                            progressBar1.Value = Convert.ToInt32(ev.Progress * 100);
                        }));

                        if (result)
                        {
                            try
                            {
                                Clip.Current.Version = Convert.ToString(Convert.ToDouble(Clip.Current.Version) + 0.01);
                                mtbVersion.Text = Clip.Current.Version;

                                Save(false, true);
                            }
                            catch (Exception ex)
                            {
                                Program.Logger.Error(ex);
                            }

                            LabelStatus.Text = "Status: Idle";
                            progressBar1.Value = 0;
                            buttonPublishUpdate.Enabled = true;
                            buttonPublishNew.Enabled = true;
                            pictureBox2.Visible = false;
                            progressBar1.Visible = true;
                            MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_SUCCESFULLY").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Logger.Error(ex);
                MessageBox.Show(string.Format("Error :\n\n{0}", ex.ToString()), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, m_msgOptionsRtl);
            }
            finally
            {
                pictureBox2.Visible = false;
                progressBar1.Visible = true;
                buttonPublishUpdate.Enabled = true;
                groupBox3.Enabled = true;
                m_bInPublishProgress = false;

            }
        }

    }
}
