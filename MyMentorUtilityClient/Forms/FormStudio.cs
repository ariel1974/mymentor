using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private string m_currentFingerPrint = string.Empty;
        private bool m_bRecAppendMode;
        private bool m_bRecOverwriteMode;
        private TimeSpan m_overwriteModeDeletePosition = TimeSpan.Zero;
        private IntPtr m_hWndVuMeterLeft;
        private IntPtr m_hWndVuMeterRight;
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
        private Stopwatch m_stopWatch = new Stopwatch();
        private IEnumerable<KeyValuePair<string, string>> m_strings;
        private string START_PAUSE_SECTION_ANCHOR;
        private string END_PAUSE_SECTION_ANCHOR;
        List<int> m_LastSelections = new List<int>();
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
        private WorldContentType m_contentType;
        private short m_endLineUniqueId = -1;
        private int m_targetStartFixInMs = 0;

        private int sizeIndex = 3;
        private int[] sizes = { 9, 10, 12, 16, 32 };

        public IEnumerable<KeyValuePair<string, string>> Strings
        {
            get { return m_strings; }
        }

        public WorldContentType ContentType
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
            }
            else
            {
                mnuTools_UI_Hebrew.Checked = false;
                mnuTools_UI_English.Checked = true;
            }

            m_initClip = initClip;

        }

        private void FormStudio_Load(object sender, EventArgs e)
        {
            START_PAUSE_SECTION_ANCHOR = ResourceHelper.GetLabel("START_PAUSE_SECTION_ANCHOR");
            END_PAUSE_SECTION_ANCHOR = ResourceHelper.GetLabel("END_PAUSE_SECTION_ANCHOR");

            // init controls
            audioSoundRecorder1.InitRecordingSystem();
            audioSoundRecorder1.SetInputDeviceChannelVolume(0, 0, 100);

            audioSoundEditor1.InitEditor();
            audioSoundEditor1.UndoEnable(true);
            ////dj
            audioDjStudio1.InitSoundSystem(1, 0, 0, 0, 0, -1);

            trackBarVolume1.Value = 100;

            // create the recorder's VU-Meter
            audioSoundRecorder1.DisplayVUMeter.Create(IntPtr.Zero);
            m_hWndVuMeterLeft = CreateVuMeter(label17, AudioSoundRecorder.enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL);
            m_hWndVuMeterRight = CreateVuMeter(label18, AudioSoundRecorder.enumGraphicBarOrientations.GRAPHIC_BAR_ORIENT_VERTICAL);

            // create the waveform analyzer (always call this function on the end of the form's Load fucntion)
            audioSoundEditor1.DisplayWaveformAnalyzer.Create(panel1.Handle, Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);

            // get the current analyzer wave settings
            AudioSoundEditor.WANALYZER_WAVEFORM_SETTINGS settingsWave = new WANALYZER_WAVEFORM_SETTINGS();
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

            TimerMenuEnabler.Enabled = true;
            tableLayoutPanel4.RowStyles[1].Height = 0;
            tableLayoutPanel4.RowStyles[2].Height = 0;

            rtbMainEditorGraphics = richTextBox1.CreateGraphics();
            rtbAlternateEditorGraphics = richTextBox3.CreateGraphics();

            audioSoundEditor1.EncodeFormats.FormatToUse = enumEncodingFormats.ENCODING_FORMAT_MP3;
            audioSoundEditor1.EncodeFormats.MP3.EncodeMode = enumMp3EncodeModes.MP3_ENCODE_CBR;
            audioSoundEditor1.EncodeFormats.MP3.CBR = 128000;

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
                    MessageBox.Show(m_strings.Single(a => a.Key == "STD_CLIP_NOT_SAVED_CONFIRM").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0)) == System.Windows.Forms.DialogResult.Yes)
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
            Clip.Current.Name = ResourceHelper.GetLabel("UNTITLED_CLIP");
            Clip.Current.Version = "1.00";
            Clip.Current.Status = "3DYQsyGZIk";
            Clip.Current.ID = Guid.NewGuid();
            Clip.Current.IsNew = true;
            Clip.Current.RightAlignment = true;
            Clip.Current.FingerPrint = m_currentFingerPrint;
            Clip.Current.ClipType = "piL85bMGtR";
            Clip.Current.Expired = null;
            Clip.Current.ContentType = this.ContentType.ObjectId;

            this.Text = "MyMentor - " + Clip.Current.Name;

            richTextBox1.Rtf = null;

            audioSoundRecorder1.RecordedSound.FreeMemory();
            audioSoundEditor1.CloseSound();
            TimerReload.Enabled = true;

            Clip.Current.Text = richTextBox1.Text;
            Clip.Current.Devide();

            SetClipProperties();
            RegenerateClipName();

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
            tbClipDescription.Text = Clip.Current.Description;
            tbClipDescriptionEnglish.Text = Clip.Current.EnglishDescription;
            comboCategory1.SelectedValue = Clip.Current.Category1 ?? string.Empty;
            mtbVersion.Text = Clip.Current.Version;
            comboCategory3.SelectedValue = Clip.Current.Category3 ?? string.Empty;
            comboCategory4.SelectedValue = Clip.Current.Category4 ?? string.Empty;
            comboClipType.SelectedValue = Clip.Current.ClipType ?? "piL85bMGtR";
            tbKeywords.Text = Clip.Current.Keywords;
            comboStatus.SelectedValue = Clip.Current.Status ?? "3DYQsyGZIk"; //paeel

            if (Clip.Current.Expired == null)
            {
                dateTimeExpired.Checked = false;
            }
            else
            {
                dateTimeExpired.Checked = true;
                dateTimeExpired.Value = Clip.Current.Expired.Value;
            }

            RegenerateDatesBox();

            numericPrice.Value = Clip.Current.Price;
            numericPriceSupport.Value = Clip.Current.PriceSupport;

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
            if (m_loadingParse)
            {
                return;
            }

            Debug.WriteLine(string.Format("ClipPattern:{0}", m_contentType.ClipTitlePattern));
            Debug.WriteLine(string.Format("SourcePattern:{0}", m_contentType.SourceTitlePattern));

            var pattern = (string)comboClipType.SelectedValue == "enaWrne5xe" ? m_contentType.SourceTitlePattern : m_contentType.ClipTitlePattern;
            var clipTitle = pattern.SpecialReplace("[category1]", comboCategory1.Text)
                .SpecialReplace("[category2]", comboCategory2.Text)
                .SpecialReplace("[category3]", comboCategory3.Text)
                .SpecialReplace("[category4]", comboCategory4.Text)
                .SpecialReplace("[description]", Clip.Current.Description)
                .SpecialReplace("[remarks]", Clip.Current.Remarks)
                .SpecialReplace("[firstName]", ParseTables.CurrentUser.ContainsKey("firstName") ? ParseTables.CurrentUser.Get<string>("firstName") : string.Empty)
                .SpecialReplace("[lastName]", ParseTables.CurrentUser.ContainsKey("lastName") ? ParseTables.CurrentUser.Get<string>("lastName") : string.Empty)
                .SpecialReplace("[cityOfResidence]", ParseTables.CurrentUser.ContainsKey("cityOfResidence") ? ParseTables.CurrentUser.Get<string>("cityOfResidence") : string.Empty);

            this.tbClipTitle.Text = clipTitle;
            Clip.Current.Title = clipTitle;

            if (isDirty && !m_whileLoadingClip)
            {
                Clip.Current.IsDirty = true;
            }
        }

        private void OpenClip()
        {
            OpenClip(null);
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

                if (Clip.Current.FingerPrint != m_currentFingerPrint)
                {
                    NewClip();
                    MessageBox.Show(ResourceHelper.GetLabel("FINGERPRINT_ERROR"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    return;
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ResourceHelper.GetLabel("CLIP_FILE_FORMAT_ERROR"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
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
                    MessageBox.Show(string.Format("נסיון טעינה של קובץ המוזיקה נכשל בגלל הסיבה :{0}\n\nאנא נסה שוב מאוחר יותר", error.ToString()), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                }
            }

            m_whileLoadingClip = false;
            Clip.Current.IsDirty = false;
            Clip.Current.IsNew = false;

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
            r1.X = positionMainEditor.X + ( this.RightToLeft == System.Windows.Forms.RightToLeft.Yes ? factor1 - width : 0 );
            r1.Y = positionMainEditor.Y;
            r1.Width = width + factor2;
            r1.Height =
                Convert.ToInt32(richTextBox2.SelectionFont.Height * richTextBox2.ZoomFactor);

            rtbMainEditorGraphics.DrawRectangle(cp, r1);
            rtbMainEditorGraphics.FillRectangle(cb, r1);

            System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(type == AnchorType.Word ? System.Drawing.Color.Black : System.Drawing.Color.White);
            float x = positionMainEditor.X + ( this.RightToLeft == System.Windows.Forms.RightToLeft.Yes ? factor3 - width : 0 );
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

        private async void FormStudio_Shown(object sender, EventArgs e)
        {
            this.Enabled = false;

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

            PleaseWaitForm form = new PleaseWaitForm();
            form.Show();
            Application.DoEvents();

            WorldContentType contentType = await ParseTables.GetContentType();

            m_contentType = contentType;

            string value_key = "value_" + MyMentor.Properties.Settings.Default.CultureInfo.Replace("-", "_");
            string status_key = "status_" + MyMentor.Properties.Settings.Default.CultureInfo.Replace("-", "_");

            try
            {
                m_strings = await ParseTables.GetStrings();

                this.comboCategory1.DisplayMember = "Value";
                this.comboCategory1.ValueMember = "ObjectId";
                this.comboCategory1.DataSource = (await ParseTables.GetCategory1(contentType.ObjectId)).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    Value = c.ContainsKey(value_key) ? c.Get<string>(value_key) : string.Empty
                }).ToList();

                this.comboCategory3.DisplayMember = "Value";
                this.comboCategory3.ValueMember = "ObjectId";
                this.comboCategory3.DataSource = (await ParseTables.GetCategory3(contentType.ObjectId, "HPz65WBzhw")).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    Value = c.ContainsKey(value_key) ? c.Get<string>(value_key) : string.Empty,
                    MinPrice = (decimal)c.Get<float>("minPrice")
                }).ToList();

                this.comboCategory4.DisplayMember = "Value";
                this.comboCategory4.ValueMember = "ObjectId";
                this.comboCategory4.DataSource = (await ParseTables.GetCategory4(contentType.ObjectId)).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    Value = c.ContainsKey(value_key) ? c.Get<string>(value_key) : string.Empty
                }).ToList();

                this.comboStatus.DisplayMember = "Value";
                this.comboStatus.ValueMember = "ObjectId";
                this.comboStatus.DataSource = (await ParseTables.GetStatuses()).Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    Value = c.Get<string>(status_key)
                }).ToList();


                await ParseTables.GetTypes().ContinueWith((t) => {

                    this.comboClipType.DisplayMember = "Value";
                    this.comboClipType.ValueMember = "ObjectId";
                
                    this.comboClipType.DataSource = t.Result.Select(c => new Category
                    {
                        ObjectId = c.ObjectId,
                        Value = c.ContainsKey(value_key) ? c.Get<string>(value_key) : string.Empty
                    }).ToList();

                    Clip.Current.IsDirty = false;
                });
                

                ParseObject labels = await ParseTables.GetCategoryLabels(contentType.ObjectId);

                lblCategory1.Text = labels.Get<string>("category1");
                lblCategory2.Text = labels.Get<string>("category2");
                lblCategory3.Text = labels.Get<string>("category3");
                lblCategory4.Text = labels.Get<string>("category4");
            }
            catch (Exception ex)
            {

            }

            m_loadingParse = false;
            m_whileLoadingClip = true;

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
            }

            await Task.Factory.StartNew(() =>
            {
                m_currentFingerPrint = FingerPrint.Value(ParseUser.CurrentUser.ObjectId);

                Debug.WriteLine(m_currentFingerPrint);

                Clip.Current.FingerPrint = m_currentFingerPrint;
            });

            form.Close();
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

            //test recorder
            if (MyMentor.Properties.Settings.Default.TestSound)
            {
                FormTestSound frmTest = new FormTestSound();

                frmTest.ShowDialog();
            }


            m_whileLoadingClip = false;

        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            PaintGraphics();

            int selectionIndex = richTextBox1.SelectionStart;
            string remember = richTextBox1.Text;

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

            if (richTextBox2.SelectedText == "[0]")
            {
                tbrWord.Checked = true;
            }
            else
            {
                tbrWord.Checked = false;
            }

            if (richTextBox2.SelectedText == "[1]")
            {
                tbrSection.Checked = true;
            }
            else
            {
                tbrSection.Checked = false;
            }

            if (richTextBox2.SelectedText == "[2]")
            {
                tbrSentense.Checked = true;
            }
            else
            {
                tbrSentense.Checked = false;
            }

            if (richTextBox2.SelectedText == "[3]")
            {
                tbrParagraph.Checked = true;
            }
            else
            {
                tbrParagraph.Checked = false;
            }

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            PaintGraphics();

            if (!m_loadingParse)
            {
                Clip.Current.IsDirty = true;
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
                if (MessageBox.Show(m_strings.Single(a => a.Key == "STD_CLIP_NOT_SAVED_CONFIRM").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,(this.RightToLeft == System.Windows.Forms.RightToLeft.Yes ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0)) == System.Windows.Forms.DialogResult.Yes)
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
            Save(isSaveAs, false);
        }

        private void Save(bool isSaveAs, bool excludeClipFile)
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
                    string name = "";
                    if (comboCategory1.SelectedItem != null)
                    {
                        name = ((Category)comboCategory1.SelectedItem).Value;
                    }
                    if (comboCategory2.SelectedItem != null)
                    {
                        name += " " + ((Category)comboCategory2.SelectedItem).Value;
                    }
                    if (comboCategory3.SelectedItem != null)
                    {
                        name += " " + ((Category)comboCategory3.SelectedItem).Value;
                    }

                    saveFileDialog1.FileName = name;
                }
                saveFileDialog1.DefaultExt = "mmnt";
                saveFileDialog1.Filter = "MyMentor Source Files|*.mmnt";

                DialogResult result = saveFileDialog1.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    if (isSaveAs)
                    {
                        Clip.Current.ID = Guid.NewGuid();
                    }

                    //Clip.Current.FontName = richTextBox1.Font.Name;
                    //Clip.Current.FontSize = float.Parse(toolStripComboBox1.Text.Replace("pt", string.Empty));
                    Clip.Current.Name = saveFileDialog1.FileName;
                    Clip.Current.FileName = saveFileDialog1.FileName;
                    Clip.Current.RtfText = richTextBox1.Rtf;
                    this.Text = "MyMentor - " + Clip.Current.Name;

                    Clip.Current.Save(audioSoundEditor1);

                    if (audioSoundEditor1.GetSoundDuration() <= 0 && !excludeClipFile)
                    {
                        MessageBox.Show(ResourceHelper.GetLabel("CLIP_SAVED_SUCCESSFULLY"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
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
                    MessageBox.Show(ResourceHelper.GetLabel("CLIP_SAVED_SUCCESSFULLY"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
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
            Clip.Current.Title = tbClipTitle.Text;
            Clip.Current.Description = tbClipDescription.Text;
            Clip.Current.Remarks = tbClipRemarks.Text;

            if (dateTimeExpired.Checked)
            {
                Clip.Current.Expired = dateTimeExpired.Value;
            }
            else
            {
                Clip.Current.Expired = null;
            }

            Clip.Current.Status = (string)comboStatus.SelectedValue;
            Clip.Current.Category1 = (string)comboCategory1.SelectedValue;
            Clip.Current.Category2 = (string)comboCategory2.SelectedValue;
            Clip.Current.Category3 = (string)comboCategory3.SelectedValue;
            Clip.Current.Category4 = (string)comboCategory4.SelectedValue;
            Clip.Current.Keywords = tbKeywords.Text;
            Clip.Current.ClipType = comboClipType.SelectedValue != null ? (string)comboClipType.SelectedValue : "piL85bMGtR";
            Clip.Current.Price = numericPrice.Value;
            Clip.Current.PriceSupport = numericPriceSupport.Value;

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
                if (MessageBox.Show(ResourceHelper.GetLabel("EXIT_CONFIRM"), "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,(this.RightToLeft == System.Windows.Forms.RightToLeft.Yes ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0)) == System.Windows.Forms.DialogResult.Yes)
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
            MessageBox.Show(ResourceHelper.GetLabel("HEBREW_CHANGE_LANGUAGE"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
        }

        private void mnuTools_UI_English_Click(object sender, EventArgs e)
        {
            MyMentor.Properties.Settings.Default.CultureInfo = "en-us";
            MyMentor.Properties.Settings.Default.Save();
            MessageBox.Show(ResourceHelper.GetLabel("ENGLISH_CHANGE_LANGUAGE"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        private void loginAsOtherUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParseUser.LogOut();

            MessageBox.Show(ResourceHelper.GetLabel("LOGIN_OTHER_USER"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            Application.Exit();
        }

        private void gotoCharIndexToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Paragraph);
            RemoveAnchor(AnchorType.Sentence);
            RemoveAnchor(AnchorType.Section);
            RemoveAnchor(AnchorType.Word);
            richTextBox1.Rtf = richTextBox2.Rtf;
        }

        private void clearParagraphAnchorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Paragraph);
            richTextBox1.Rtf = richTextBox2.Rtf;
        }

        private void clearSentensesAnchorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Sentence);
            richTextBox1.Rtf = richTextBox2.Rtf;
        }

        private void clearSectionAnchorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Section);
            richTextBox1.Rtf = richTextBox2.Rtf;
        }

        private void clearWordAnchorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox2.Rtf = richTextBox1.Rtf;
            RemoveAnchor(AnchorType.Word);
            richTextBox1.Rtf = richTextBox2.Rtf;
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
                result = MessageBox.Show(ResourceHelper.GetLabel("OVERWRITE_AUDIO_CONFIRM"), "MyMentor", MessageBoxButtons.YesNo);
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
                audioSoundEditor1.SetInsertPos(nBeginSelectionInMs);
            else
                audioSoundEditor1.SetInsertPos(0);

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

        private void testAudioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormTestSound f = new FormTestSound();
            f.ShowDialog();

        }

        private void removeAllSchedulingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Clip.Current.Chapter.Paragraphs != null)
            {
                if (MessageBox.Show(ResourceHelper.GetLabel("REMOVE_SCHEDULING_CONFIRM"), "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,(this.RightToLeft == System.Windows.Forms.RightToLeft.Yes ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0)) == System.Windows.Forms.DialogResult.Yes)
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
                    m_endLineUniqueId = -1;

                    LabelCurrentSchedulingTimer.Text = audioSoundEditor1.FromMsToFormattedTime(0, true, true);// GetFormattedTime(e.nBeginPosInMs, true, true);
                    richTextBox3.SelectionStart = 4;
                    buttonStartSchedulingPlayback.Text = ResourceHelper.GetLabel("START");
                    buttonHammer.Enabled = true;

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

            JsonDebugFrm frm = new JsonDebugFrm();
            frm.ShowDialog();

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
            catch (Exception)
            {
                MessageBox.Show("Unable to copy document content.", "RTE - Copy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Copy();
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to copy document content.", "RTE - Copy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.Paste();
            }
            catch
            {
                MessageBox.Show("Unable to copy clipboard content to document.", "RTE - Paste", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void tbrSmallerFont_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.SelectionFont.Size > 8)
                {
                    richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, richTextBox1.SelectionFont.Size - 2);
                }
            }
            catch
            {

            }

        }

        private void tbrBiggerFont_Click(object sender, EventArgs e)
        {
            try
            {
                if (richTextBox1.SelectionFont.Size < 32)
                {
                    richTextBox1.SelectionFont = new Font(richTextBox1.SelectionFont.FontFamily, richTextBox1.SelectionFont.Size + 2);
                }
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
                richTextBox1.SelectAll();
                richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
                richTextBox1.Select(0, 0);
            }
            catch
            {

            }

        }

        private void tbrLeft_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox1.SelectAll();
                richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
                richTextBox1.Select(0, 0);
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

        private void tbrRemoveAnchors_Click(object sender, EventArgs e)
        {
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

        }

        private void FormStudio_Resize(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.Move(Picture1.Left, Picture1.Top, panel1.Width, panel1.Height);


            PaintGraphics();

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (FormReadyTexts form = new FormReadyTexts(this))
            {
                if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    richTextBox1.Text = form.SelectedSource.Text;

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

                    MessageBox.Show(m_strings.Single(a => a.Key == "STD_AFTER_SOURCE_SELECTION").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                }
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

        private async void comboCategory1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string value_key = "value_" + MyMentor.Properties.Settings.Default.CultureInfo.Replace("-", "_");

            if (comboCategory1.SelectedIndex >= 0)
            {
                var list = (await ParseTables.GetCategory2((string)comboCategory1.SelectedValue)).Where( o => o.Keys.Count() == 4);

                this.comboCategory2.DisplayMember = "Value";
                this.comboCategory2.ValueMember = "ObjectId";
                this.comboCategory2.DataSource = list.Select(c => new Category
                {
                    ObjectId = c.ObjectId,
                    Value = c.ContainsKey(value_key) ? c.Get<string>(value_key) : string.Empty
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

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            mnuAudioLoad_Click(null, new EventArgs());

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomOut();

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomIn();

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
                audioSoundEditor1.DisplayWaveformAnalyzer.ZoomToSelection(true);
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
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);

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

        private void trackBarPitch1_Scroll(object sender, EventArgs e)
        {
            audioDjStudio1.SetRatePerc(0, (short)trackBarPitch1.Value);

        }

        private void buttonStartRecNew_Click(object sender, EventArgs e)
        {
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

        private void buttonRecOverwritePlayback_Click(object sender, EventArgs e)
        {
            audioDjStudio1.LoadSoundFromEditingSession(0, audioSoundEditor1.Handle);

            buttonRecOverwritePlayback.Enabled = false;

            m_stopWatch.Reset();
            m_stopWatch.Start();

            timerRefreshLedDisplay.Enabled = true;
            timerPreStartFixPlayback.Enabled = true;

        }

        private void numericUpDownBufferRecord_ValueChanged(object sender, EventArgs e)
        {
            sevenSegmentArray1.Value = numericUpDownBufferRecord.Value.ToString("0.0");

        }

        private void trackBarVolume1_Scroll(object sender, EventArgs e)
        {

        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_selectedScheduledWord = null;
            m_selectedAnchor = false;

            if (tabControl1.SelectedIndex == 0
                || tabControl1.SelectedIndex == 3)
            {
                tableLayoutPanel4.RowStyles[1].Height = 0;
                tableLayoutPanel4.RowStyles[2].Height = 0;

                RegenerateClipName();
            }
            else
            {
                tableLayoutPanel4.RowStyles[1].Height = 38;
                tableLayoutPanel4.RowStyles[2].Height = 160;
            }

            //check if we have to refresh display
            if (tabControl1.SelectedIndex == 1)
            {
                //copy text and insert start anchor
                richTextBox5.Clear();
                richTextBox5.Rtf = richTextBox1.Rtf;
                richTextBox5.Text = richTextBox5.Text.RemoveAnchors();

                if (m_waveFormTabIndex != 1)
                {
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
                    //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemRemove(m_guidLineUniqueId);
                    audioSoundEditor1.DisplayWaveformAnalyzer.Refresh();
                }
            }
            //scheduler step
            else if (tabControl1.SelectedIndex == 2)
            {
                //copy text and insert start anchor
                richTextBox3.Clear();
                richTextBox3.Rtf = richTextBox1.Rtf;
                Clip.Current.Devide();

                //add end anchor
                m_skipSelectionChange = true;
                richTextBox3.AppendText(END_PAUSE_SECTION_ANCHOR);

                //add start anchor
                richTextBox3.SelectionStart = 0;
                richTextBox3.SelectionLength = 0;
                richTextBox3.SelectedText = START_PAUSE_SECTION_ANCHOR;
                m_skipSelectionChange = false;

                //selects first word
                richTextBox3.SelectionStart = 0;
                richTextBox3.SelectionLength = 0;

                if (m_waveFormTabIndex != 2)
                {
                    m_waveFormTabIndex = 2;

                    //m_guidLineUniqueId = audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemVerticalLineAdd("MyLine", "", 0,
                    //    new WANALYZER_VERTICAL_LINE { color = Color.Yellow, nWidth = 1, nDashCap = enumLineDashCaps.LINE_DASH_CAP_FLAT, nDashStyle = enumWaveformLineDashStyles.LINE_DASH_STYLE_DASH_DOT, nHighCap = enumLineCaps.LINE_CAP_SQUARE, nLowCap = enumLineCaps.LINE_CAP_SQUARE, nTranspFactor = 0 });

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

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == 2 &&
    audioSoundEditor1.GetSoundDuration() <= 0)
            {
                // ask the user if he wants to go on
                MessageBox.Show(ResourceHelper.GetLabel("NO_AUDIO_EXISTS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
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

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (buttonPause.Text == ResourceHelper.GetLabel("PAUSE"))
            {
                audioDjStudio1.PauseSound(0);
            }
            else
            {
                audioDjStudio1.ResumeSound(0);
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

            //int mm = audioSoundEditor1.GetPlaybackPosition();
            var position = new TimeSpan(0, 0, 0, 0, Math.Max((int)fPosition, 0)); ;

            LabelCurrentSchedulingTimer.Text = audioSoundEditor1.FromMsToFormattedTime((long)fPosition, true, true);// GetFormattedTime(e.nBeginPosInMs, true, true);

            //during playing check if the current position moved over to the next word
            if (Clip.Current.Chapter.Paragraphs != null)// && !m_selectedAnchor)
            {
                //in case start part
                if (m_selectedStartAnchor)
                {
                    Debug.WriteLine(string.Format("_Tick: m_selectedStartAnchor is true"));

                    if (position >= Clip.Current.Chapter.FirstWord.StartTime && Clip.Current.Chapter.FirstWord.StartTime > TimeSpan.Zero)
                    {
                        Debug.WriteLine(string.Format("_Tick: position ({0}) is equal or bigger than first word start time ({1}) AND start time > 0", audioSoundEditor1.FromMsToFormattedTime((long)position.TotalMilliseconds, false, true), audioSoundEditor1.FromMsToFormattedTime((long)Clip.Current.Chapter.FirstWord.StartTime.TotalMilliseconds, false, true)));

                        //set time picker
                        timePickerCurrentWord.Value = position;

                        Debug.WriteLine(string.Format("_Tick: set timePickerCurrentWord to position ({0})", audioSoundEditor1.FromMsToFormattedTime((long)position.TotalMilliseconds, false, true)));

                        Debug.WriteLine(string.Format("_Tick: moving selection start to {0}", Clip.Current.Chapter.FirstWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length + 1));
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
                        Debug.WriteLine(string.Format("_Tick: moving to next word start time : {0}", audioSoundEditor1.FromMsToFormattedTime((long)m_selectedScheduledWord.NextWord.StartTime.TotalMilliseconds, false, true)));

                        //set time picker
                        timePickerCurrentWord.Value = m_selectedScheduledWord.NextWord.StartTime;// position;

                        Debug.WriteLine(string.Format("_Tick: moving to char index {0}", m_selectedScheduledWord.NextWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length));
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
                        Debug.WriteLine(string.Format("_Tick: !!last word!! : moving to next word start time : {0}", audioSoundEditor1.FromMsToFormattedTime((long)m_selectedScheduledWord.NextWord.StartTime.TotalMilliseconds, false, true)));
                        //set time picker
                        timePickerCurrentWord.Value = m_selectedScheduledWord.NextWord.StartTime;// position;

                        Debug.WriteLine(string.Format("_Tick: moving to char index {0}", m_selectedScheduledWord.NextWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length));
                        richTextBox3.SelectionStart = m_selectedScheduledWord.NextWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length;
                    }
                    // catch when passed the red line end border 
                    else if (m_selectedScheduledWord != null &&
                        m_selectedScheduledWord.NextWord == null &&
                        m_selectedScheduledWord.Duration > TimeSpan.Zero
                        && position >= m_selectedScheduledWord.StartTime + m_selectedScheduledWord.Duration
                        )
                    {
                        Debug.WriteLine(string.Format("_Tick: !!Passed the end line!! on time : {0}", audioSoundEditor1.FromMsToFormattedTime((long)(m_selectedScheduledWord.StartTime + m_selectedScheduledWord.Duration).TotalMilliseconds, false, true)));

                        //set time picker
                        timePickerCurrentWord.Value = m_selectedScheduledWord.StartTime + m_selectedScheduledWord.Duration;

                        richTextBox3.SelectionStart = m_selectedScheduledWord.RealCharIndex + START_PAUSE_SECTION_ANCHOR.Length + m_selectedScheduledWord.Length + 1;
                    }
                }
            }

        }

        private void timerRefreshLedDisplay_Tick(object sender, EventArgs e)
        {
            if (audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
            {
                bool bSelectionAvailable = false;
                Int32 nBeginSelectionInMs = 0;
                Int32 nEndSelectionInMs = 0;
                audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelectionAvailable, ref nBeginSelectionInMs, ref nEndSelectionInMs);

                if (m_targetStartFixInMs > nBeginSelectionInMs)
                {
                    double d = (double)((m_targetStartFixInMs - nBeginSelectionInMs) / (double)1000);

                    // playing music
                    sevenSegmentArray1.Value = d.ToString("0.0");
                }
            }
            else
            {
                // in silent pre playback
                sevenSegmentArray1.Value = ((double)(numericUpDownBufferRecord.Value * 1000 - m_stopWatch.ElapsedMilliseconds) / (double)1000).ToString("0.0");
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

        private void timerRecordingDone_Tick(object sender, EventArgs e)
        {
            timerRecordingDone.Enabled = false;

            // check if the latest recording session must replace or appended to the previous editing session
            if (m_bRecOverwriteMode)
            {
                m_bRecOverwriteMode = false;
                TimeSpan startSelectionTime = TimeSpan.Parse(LabelSelectionBegin.Text);
                TimeSpan endSelectionTime = TimeSpan.Parse(LabelSelectionEnd.Text);

                audioSoundEditor1.UseThreadsInSyncMode(true);
                audioSoundEditor1.DeleteRange((int)m_overwriteModeDeletePosition.TotalMilliseconds, -1);
                audioSoundEditor1.SetLoadingMode(enumLoadingModes.LOAD_MODE_APPEND);
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

        private void timerStartRecordingAfterPlayingBuffer_Tick(object sender, EventArgs e)
        {
            timerStartRecordingAfterPlayingBuffer.Enabled = false;

            if (m_bRecOverwriteMode)
            {
                TimeSpan startSelectionTime = TimeSpan.Parse(LabelSelectionBegin.Text);

                m_overwriteModeDeletePosition = startSelectionTime;

                // create a fresh new recording session
                audioSoundRecorder1.SetRecordingMode(AudioSoundRecorder.enumRecordingModes.REC_MODE_NEW);

                // start recording in memory from system default input device and input channel
                audioSoundRecorder1.StartFromDirectSoundDevice(0, -1, "");

            }

        }

        private void timerPreStartFixPlayback_Tick(object sender, EventArgs e)
        {
            timerPreStartFixPlayback.Enabled = false;
            m_stopWatch.Stop();

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
                audioDjStudio1.PlaySound(0, nBeginPlaying, nBeginSelectionInMs);
                m_bRecOverwriteMode = true;
            }

        }

        private void timerFixRichText_Tick(object sender, EventArgs e)
        {
            timerFixRichText.Enabled = false;
            richTextBox5.SelectionStart = 0;
            richTextBox5.SelectionLength = 0;
            richTextBox5.Refresh();

        }

        private void audioSoundRecorder1_RecordingDuration(object sender, AudioSoundRecorder.RecordingDurationEventArgs e)
        {
            m_intRecordingDuration = e.nDuration;

        }

        private void audioSoundRecorder1_RecordingPaused(object sender, EventArgs e)
        {
            timerRecordIcon.Enabled = false;

        }

        private void audioSoundRecorder1_RecordingResumed(object sender, EventArgs e)
        {
            timerRecordIcon.Enabled = true;

        }

        private void audioSoundRecorder1_RecordingStarted(object sender, EventArgs e)
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

        private void audioSoundEditor1_SoundEditDone(object sender, SoundEditDoneEventArgs e)
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
            LabelStatus.Text = "Status: Idle";
            progressBar1.Visible = false;

            LabelStatus.Refresh();

            Cursor.Current = Cursors.Default;

            Clip.Current.Saved = true;

            if (e.nResult != enumErrorCodes.ERR_NOERROR)
                MessageBox.Show(e.nResult.ToString());
            else
            {
                MessageBox.Show(ResourceHelper.GetLabel("CLIP_SAVED_SUCCESSFULLY"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
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
            }
            else
                MessageBox.Show("Sound failed to load with the following error code: " + audioSoundEditor1.LastError.ToString());

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
            buttonPause.Text = ResourceHelper.GetLabel("PAUSE");
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            buttonStartSchedulingPlayback.Text = ResourceHelper.GetLabel("START");
            buttonRestartScheduling.Enabled = true;
            timePickerCurrentWord.Enabled = true;

            if (m_selectedAnchor)
            {
                buttonScheduleAnchor.Enabled = true;
                buttonZoomTestableArea.Enabled = true;
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

        private void audioSoundEditor1_SoundPlaybackPaused(object sender, EventArgs e)
        {
            buttonPause.Text = ResourceHelper.GetLabel("CONTINUE");
            LabelStatus.Text = "Status: Playback paused";
            LabelStatus.Refresh();


        }

        private void audioSoundEditor1_SoundPlaybackPlaying(object sender, EventArgs e)
        {
            buttonPause.Text = ResourceHelper.GetLabel("PAUSE");
            LabelStatus.Text = "Status: Playing...";
            LabelStatus.Refresh();

            timePickerCurrentWord.Enabled = false;

        }

        private void audioSoundEditor1_SoundPlaybackStopped(object sender, EventArgs e)
        {
            buttonPause.Text = ResourceHelper.GetLabel("PAUSE");
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            FrameRecording.Enabled = true;

            timerUpdateTimePickerSpinner.Enabled = false;

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
            timerDisplayWaveform.Enabled = true;

            progressBar1.Visible = false;
            LabelStatus.Text = "Status: Idle";

            buttonPlay.Enabled = true;
            buttonPause.Enabled = true;
            buttonStop.Enabled = true;

        }


        private void audioSoundEditor1_WaveAnalyzerDisplayRangeChange(object sender, WaveAnalyzerDisplayRangeChangeEventArgs e)
        {
            // display formatted strings
            LabelRangeBegin.Text = audioSoundEditor1.GetFormattedTime(e.nBeginPosInMs, true, true);
            LabelRangeEnd.Text = audioSoundEditor1.GetFormattedTime(e.nEndPosInMs, true, true);
            LabelRangeDuration.Text = audioSoundEditor1.GetFormattedTime(e.nEndPosInMs - e.nBeginPosInMs, true, true);

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

                if (audioDjStudio1.GetPlayerStatus(0) != AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
                {

                    TimeSpan tp = new TimeSpan(0, 0, 0, 0, e.nBeginPosInMs);

                    if (new TimeSpan(0, 0, 0, 0, 1000 * (int)numericUpDownBufferRecord.Value) > new TimeSpan(0, 0, (int)tp.TotalSeconds))
                    {
                        timerPreStartFixPlayback.Interval = (int)(new TimeSpan(0, 0, 0, 0, 1000 * (int)numericUpDownBufferRecord.Value) - new TimeSpan(0, 0, (int)tp.TotalSeconds)).TotalMilliseconds;
                    }
                    else
                    {
                        timerPreStartFixPlayback.Interval = 1;
                    }

                    m_targetStartFixInMs = e.nBeginPosInMs;
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

        private void audioDjStudio1_SoundClosed(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            timerUpdateDuringPlayback.Enabled = false;
        }

        private void audioDjStudio1_SoundDone(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            timerUpdateDuringPlayback.Enabled = false;
            m_stopWatch.Stop();

            buttonPause.Text = ResourceHelper.GetLabel("PAUSE");
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            buttonStartSchedulingPlayback.Text = ResourceHelper.GetLabel("START");
            buttonRestartScheduling.Enabled = true;
            timePickerCurrentWord.Enabled = true;

            if (m_selectedAnchor)
            {
                buttonScheduleAnchor.Enabled = true;
                buttonZoomTestableArea.Enabled = true;
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
            else
            {
                if (m_LastSelections.Count() > 0)
                {
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, m_LastSelections[0], m_LastSelections[1]);

                    m_LastSelections.Clear();
                }
                else
                {
                    //done playing - return position to start
                    audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);
                }
            }

        }

        private void audioDjStudio1_SoundPaused(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            buttonPause.Text = ResourceHelper.GetLabel("CONTINUE");
            LabelStatus.Text = "Status: Playback paused";
            LabelStatus.Refresh();
            timerUpdateDuringPlayback.Enabled = false;

        }

        private void audioDjStudio1_SoundPlaying(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            buttonPause.Text = ResourceHelper.GetLabel("PAUSE");
            LabelStatus.Text = "Status: Playing...";
            LabelStatus.Refresh();

            timerUpdateDuringPlayback.Enabled = true;
            timePickerCurrentWord.Enabled = false;

        }

        private void audioDjStudio1_SoundStopped(object sender, AudioDjStudio.PlayerEventArgs e)
        {
            timerUpdateDuringPlayback.Enabled = false;
            buttonPause.Text = ResourceHelper.GetLabel("PAUSE");
            LabelStatus.Text = "Status: Idle";
            LabelStatus.Refresh();

            FrameRecording.Enabled = true;

            timerUpdateTimePickerSpinner.Enabled = false;

        }

        private void buttonStartSchedulingPlayback_Click(object sender, EventArgs e)
        {
            Int32 nDurationInMs = audioSoundEditor1.GetSoundDuration();
            if (nDurationInMs == 0)
            {
                return;
            }

            if (buttonStartSchedulingPlayback.Text == ResourceHelper.GetLabel("START") || buttonStartSchedulingPlayback.Text == ResourceHelper.GetLabel("CONTINUE"))
            {
                Int32 nBeginSelectionInMs = 0;
                Int32 nEndSelectionInMs = 0;
                bool bSelection = true;
                n_hammerLastTimePressed = TimeSpan.Zero;

                audioSoundEditor1.DisplayWaveformAnalyzer.GetSelection(ref bSelection, ref nBeginSelectionInMs, ref nEndSelectionInMs);//   .GraphicItemHorzPositionGet(m_guidLineUniqueId, ref nBeginSelectionInMs, ref nEndSelectionInMs);
                //audioSoundEditor1.PlaySoundRange(nBeginSelectionInMs, -1);

                if (buttonStartSchedulingPlayback.Text == ResourceHelper.GetLabel("CONTINUE"))
                {
                    audioDjStudio1.LoadSoundFromEditingSession(0, audioSoundEditor1.Handle);
                    audioDjStudio1.PlaySound(0, nBeginSelectionInMs, -1);
                }
                else
                {
                    audioDjStudio1.LoadSoundFromEditingSession(0, audioSoundEditor1.Handle);
                    audioDjStudio1.PlaySound(0, nBeginSelectionInMs, -1);
                }


                buttonHammer.Enabled = true;
                //audioSoundEditor1.PlaySoundRange((int)TimeSpan.Parse(LabelCurrentSchedulingTimer.Text).TotalMilliseconds, -1);
                buttonStartSchedulingPlayback.Text = ResourceHelper.GetLabel("STOP");

                buttonRestartScheduling.Enabled = false;
                timerUpdateTimePickerSpinner.Enabled = true;
                timePickerCurrentWord.Enabled = false;
            }
            else if (buttonStartSchedulingPlayback.Text == ResourceHelper.GetLabel("STOP"))
            {
                n_hammerLastTimePressed = TimeSpan.Zero;
                buttonHammer.Enabled = false;
                buttonRestartScheduling.Enabled = true;
                timePickerCurrentWord.Enabled = true;
                timerUpdateTimePickerSpinner.Enabled = false;
                //audioSoundEditor1.StopSound();
                audioDjStudio1.StopSound(0);
                buttonStartSchedulingPlayback.Text = ResourceHelper.GetLabel("CONTINUE");
            }

        }

        private void buttonRestartScheduling_Click(object sender, EventArgs e)
        {
            //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, 0, 0);
            LabelCurrentSchedulingTimer.Text = audioSoundEditor1.FromMsToFormattedTime(0, true, true);// GetFormattedTime(e.nBeginPosInMs, true, true);

            //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, 0, 0);
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);
            //timePickerCurrentWord.Value = TimeSpan.Zero;
            richTextBox3.SelectionStart = 4;
            buttonStartSchedulingPlayback.Text = ResourceHelper.GetLabel("START");
            buttonHammer.Enabled = true;

        }

        private void buttonHammer_Click(object sender, EventArgs e)
        {
            //
            // PLAYBACK
            if (audioDjStudio1.GetPlayerStatus(0) == AudioDjStudio.enumPlayerStatus.SOUND_PLAYING)
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

        private void timePickerCurrentWord_ValueChanged(object sender, EventArgs e)
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
                    //we have selected section with graphical line exists
                    if (m_selectedScheduledWord != null && m_selectedScheduledWord.GraphicItemUnique > 0)
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
                                (int)(timePickerCurrentWord.Value.TotalMilliseconds + m_selectedScheduledWord.Duration.TotalMilliseconds),
                                 (int)(timePickerCurrentWord.Value.TotalMilliseconds + m_selectedScheduledWord.Duration.TotalMilliseconds));
                        }

                    }
                }
            }

        }

        private void buttonScheduleAnchor_Click(object sender, EventArgs e)
        {
            Int32 nDurationInMs = audioSoundEditor1.GetSoundDuration();
            if (nDurationInMs == 0)
            {
                return;
            }

            buttonScheduleAnchor.Enabled = false;
            buttonZoomTestableArea.Enabled = false;

            var interval = new TimeSpan(0, 0, (int)(numericUpDownInterval.Value * 2));

            timerUpdateTimePickerSpinner.Enabled = true;

            m_rem_anchorFixRecording = timePickerCurrentWord.Value;

            //play the range
            //audioSoundEditor1.PlaySoundRange((int)timePickerCurrentWord.Value.TotalMilliseconds,
            //        (int)timePickerCurrentWord.Value.TotalMilliseconds + (int)interval.TotalMilliseconds);
            audioDjStudio1.LoadSoundFromEditingSession(0, audioSoundEditor1.Handle);
            audioDjStudio1.PlaySound(0, (int)timePickerCurrentWord.Value.TotalMilliseconds,
                    (int)timePickerCurrentWord.Value.TotalMilliseconds + (int)interval.TotalMilliseconds);

        }

        private void buttonZoomTestableArea_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, (int)timePickerCurrentWord.Value.TotalMilliseconds,
    (int)timePickerCurrentWord.Value.TotalMilliseconds + ((int)numericUpDownInterval.Value * 2 * 1000));
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomToSelection(true);

        }

        private void buttonZoomAllClip_Click(object sender, EventArgs e)
        {
            audioSoundEditor1.DisplayWaveformAnalyzer.ZoomToFullSound();

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
                audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, 0, 0);

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
                //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId,
                //    (int)timePickerCurrentWord.Value.TotalMilliseconds,
                //    (int)timePickerCurrentWord.Value.TotalMilliseconds);
                audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true,
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
                        //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, (int)timePickerCurrentWord.Value.TotalMilliseconds, (int)timePickerCurrentWord.Value.TotalMilliseconds);
                        audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, (int)timePickerCurrentWord.Value.TotalMilliseconds, (int)timePickerCurrentWord.Value.TotalMilliseconds);

                        m_selectedAnchor = true;

                        buttonScheduleAnchor.Enabled = true;
                        buttonZoomTestableArea.Enabled = true;
                    }
                    else
                    {
                        //select the word
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
                            //set current word start time when clicking word offline
                            timePickerCurrentWord.Value = m_selectedScheduledWord.StartTime;
                            LabelCurrentWordDuration.Text = audioSoundEditor1.FromMsToFormattedTime((int)m_selectedScheduledWord.Duration.TotalMilliseconds, true, true);
                            timePickerCurrentWord.Enabled = true;
                        }

                        //move guid line
                        //audioSoundEditor1.DisplayWaveformAnalyzer.GraphicItemHorzPositionSet(m_guidLineUniqueId, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds);
                        audioSoundEditor1.DisplayWaveformAnalyzer.SetSelection(true, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds, (int)m_selectedScheduledWord.StartTime.TotalMilliseconds);
                        m_selectedAnchor = false;
                        buttonScheduleAnchor.Enabled = false;
                        buttonZoomTestableArea.Enabled = false;
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
                MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_NO_SCHEDULING_EXISTS").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            if (Clip.Current.IsNew)
            {
                // ask the user if he wants to go on
                MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_NOT_SAVED").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            if ((string)comboClipType.SelectedValue != "piL85bMGtR" && !m_admin)
            {
                MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_NO_PERMISSIONS").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                return;
            }

            //clip type shiur
            if ((string)comboClipType.SelectedValue == "piL85bMGtR")
            {
                if (dateTimeExpired.Checked && dateTimeExpired.Value <= DateTime.Today)
                {
                    MessageBox.Show( ResourceHelper.GetLabel("EXPIRED_DATE_ERROR") , "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    return;
                }

                if (comboCategory3.SelectedIndex < 0)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MANDATORY_FIELDS"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    return;
                }

                if (((Category)comboCategory3.SelectedItem).MinPrice > numericPrice.Value)
                {
                    MessageBox.Show(ResourceHelper.GetLabel("MIN_PRICE_ERROR"), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                    return;
                }

                //if (((Category)comboCategory3.SelectedItem).MinPrice > numericPriceSupport.Value)
                //{
                //    MessageBox.Show("מחיר השיעור כולל תמיכה קטן מהמינימום המותר לסוג זה", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                //    return;
                //}

            }

            if ((string)comboClipType.SelectedValue == "piL85bMGtR")
            {
                if (MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_CONFIRM").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,(this.RightToLeft == System.Windows.Forms.RightToLeft.Yes ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0)) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }
            else
            {
                if (MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_CONFIRM").Value, "MyMentor", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,(this.RightToLeft == System.Windows.Forms.RightToLeft.Yes ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0)) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            groupBox3.Enabled = false;
            pictureBox2.Visible = true;
            progressBar1.Visible = false;
            buttonPublish.Enabled = false;
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
                            catch
                            {

                            }

                            LabelStatus.Text = "Status: Idle";
                            progressBar1.Value = 0;
                            buttonPublish.Enabled = true;
                            pictureBox2.Visible = false;
                            progressBar1.Visible = true;
                            MessageBox.Show(m_strings.Single(a => a.Key == "STD_PUBLISH_SUCCESFULLY").Value, "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error :\n\n{0}", ex.ToString()), "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
            }
            finally
            {
                pictureBox2.Visible = false;
                progressBar1.Visible = true;
                buttonPublish.Enabled = true;
                groupBox3.Enabled = true;
            }

        }

        private async void comboClipType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string lessonType = "HPz65WBzhw"; //clip

            // clip
            if (comboClipType.SelectedValue != null && (string)comboClipType.SelectedValue == "enaWrne5xe") //source
            {
                groupBox4.Visible = false;
                groupBox5.Visible = false;
                groupBox6.Visible = false;
                groupBox7.Visible = false;

                lessonType = "0y8A4XTNeR"; //source

                //tbClipDescription.Visible = false;
                //lblDescription.Visible = false;

                dateTimeExpired.Enabled = false;
                dateTimeExpired.Checked = false;
                dateTimeExpired.Visible = false;

                btnAddDate.Visible = false;
                btnRemoveDate.Visible = false;
                listBoxDates.Visible = false;

                numericPrice.Enabled = false;
                numericPrice.Value = 0;
                lblPrice.Visible = false;
                numericPrice.Visible = false;
                lblMinValue.Visible = false;

                tbKeywords.Visible = false;
                lblKeywords.Visible = false;

                lblExpired.Visible = false;
                dateTimeExpired.Visible = false;
                dtpReadingDate.Visible = false;
                lblReadingDates.Visible = false;

                comboCategory4.Visible = false;
                lblCategory4.Visible = false;
            }
            else
            {
                groupBox4.Visible = true;
                groupBox5.Visible = true;
                groupBox6.Visible = true;
                groupBox7.Visible = true;

                dateTimeExpired.Enabled = true;
                dateTimeExpired.Visible = true;
                btnAddDate.Visible = true;
                btnRemoveDate.Visible = true;
                listBoxDates.Visible = true;

                //tbClipDescription.Visible = true;
                //lblDescription.Visible = true;
                numericPrice.Enabled = true;
                lblPrice.Visible = true;
                numericPrice.Visible = true;
                lblMinValue.Visible = true;
                lblReadingDates.Visible = true;

                tbKeywords.Visible = true;
                lblKeywords.Visible = true;

                lblExpired.Visible = true;
                dateTimeExpired.Visible = true;
                dtpReadingDate.Visible = true;

                comboCategory4.Visible = true;
                lblCategory4.Visible = true;
            }

            bool doDirty = false;

            doDirty = !m_whileLoadingClip;
            string value_key = "value_" + MyMentor.Properties.Settings.Default.CultureInfo.Replace("-", "_");

            this.comboCategory3.DisplayMember = "Value";
            this.comboCategory3.ValueMember = "ObjectId";
            this.comboCategory3.DataSource = (await ParseTables.GetCategory3(m_contentType.ObjectId, lessonType)).Select(c => new Category
            {
                ObjectId = c.ObjectId,
                Value = c.Get<string>(value_key),
                MinPrice = (decimal)c.Get<float>("minPrice")

            }).ToList();

            if (Clip.Current.Category3 != null)
            {
                comboCategory3.SelectedItem = Clip.Current.Category3;
            }

            RegenerateClipName(doDirty);

        }

        private void buttonAutoDevide_Click(object sender, EventArgs e)
        {
            var parDelimiters = comboBoxAutoDevidePar.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim()).ToList<string>();
            var senDelimiters = comboBoxAutoDevideSen.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim()).ToList<string>();
            var worDelimiters = comboBoxAutoDevideWor.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(w => w.Trim()).ToList<string>();

            if (parDelimiters.Count() == 0 && senDelimiters.Count() == 0 && worDelimiters.Count() == 0)
            {
                MessageBox.Show("יש לבחור לפחות אפשרות עוגן אחת", "MyMentor", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
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

            try
            {
                RemoveAnchor(AnchorType.Paragraph);
                RemoveAnchor(AnchorType.Sentence);
                RemoveAnchor(AnchorType.Section);
                RemoveAnchor(AnchorType.Word);
                int anchors = 0;
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

                        if (parDelimiters.Exists(a => a == "ENTER") && charactersFromLastAnchor == 0 && anchors > 0)
                        {
                            //remove other anchor
                            richTextBox2.SelectionStart = index - 4;
                            richTextBox2.SelectionLength = 3;
                            richTextBox2.SelectedText = "";

                            AddAnchor(AnchorType.Paragraph, index - 4);
                            anchors++;
                            charactersFromLastAnchor = 0;
                            index += 3;
                            continue;
                        }

                        if (parDelimiters.Exists(a => a == "ENTER") && charactersFromLastAnchor > 0)
                        {
                            AddAnchor(AnchorType.Paragraph, index - 1);
                            anchors++;
                            charactersFromLastAnchor = 0;
                            index += 3;
                            continue;
                        }

                        if (parDelimiters.Exists(a => a == "שני ENTER") && enterKeys % 2 == 0 && charactersFromLastAnchor > 0)
                        {
                            AddAnchor(AnchorType.Paragraph, index - 1);
                            anchors++;
                            charactersFromLastAnchor = 0;
                            index += 3;
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
                                anchors++;
                                charactersFromLastAnchor = 0;
                                index += 3;
                                sentenses = 0;
                                continue;
                            }

                            AddAnchor(AnchorType.Sentence, index - 1);
                            anchors++;
                            charactersFromLastAnchor = 0;
                            index += 3;
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
                                anchors++;
                                charactersFromLastAnchor = 0;
                                index += 3;
                                sentenses = 0;
                                continue;
                            }

                            AddAnchor(AnchorType.Sentence, index - 1);
                            anchors++;
                            charactersFromLastAnchor = 0;
                            index += 3;
                            continue;
                        }
                    }


                    if (parDelimiters.Exists(a => a == richTextBox2.SelectedText) && charactersFromLastAnchor > 0)
                    {
                        AddAnchor(AnchorType.Paragraph, index);
                        anchors++;
                        charactersFromLastAnchor = 0;
                        index += 3;
                        continue;
                    }

                    if ((richTextBox2.SelectedText == ":" || richTextBox2.SelectedText == "׃")
                        && senDelimiters.Exists(a => a == "נקודותיים (:)") && charactersFromLastAnchor > 0)
                    {
                        sentenses += 1;

                        if ((sentenses == 2 && parDelimiters.Exists(a => a == "אחרי 2 משפטים")) ||
                            (sentenses == 3 && parDelimiters.Exists(a => a == "אחרי 3 משפטים")) ||
                            (sentenses == 4 && parDelimiters.Exists(a => a == "אחרי 4 משפטים")))
                        {
                            AddAnchor(AnchorType.Paragraph, index);
                            anchors++;
                            charactersFromLastAnchor = 0;
                            index += 3;
                            sentenses = 0;
                            continue;
                        }

                        AddAnchor(AnchorType.Sentence, index);
                        anchors++;
                        charactersFromLastAnchor = 0;
                        index += 3;
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
                            anchors++;
                            charactersFromLastAnchor = 0;
                            index += 3;
                            sentenses = 0;
                            continue;
                        }

                        AddAnchor(AnchorType.Sentence, index);
                        anchors++;
                        charactersFromLastAnchor = 0;
                        index += 3;
                        continue;
                    }

                    if (senDelimiters.Exists(a => a == richTextBox2.SelectedText) && charactersFromLastAnchor > 0)
                    {
                        sentenses += 1;

                        if ((sentenses == 2 && parDelimiters.Exists(a => a == "אחרי 2 משפטים")) ||
                            (sentenses == 3 && parDelimiters.Exists(a => a == "אחרי 3 משפטים")) ||
                            (sentenses == 4 && parDelimiters.Exists(a => a == "אחרי 4 משפטים")))
                        {
                            AddAnchor(AnchorType.Paragraph, index);
                            anchors++;
                            charactersFromLastAnchor = 0;
                            index += 3;
                            sentenses = 0;
                            continue;
                        }

                        AddAnchor(AnchorType.Sentence, index);
                        anchors++;
                        charactersFromLastAnchor = 0;
                        index += 3;
                        continue;
                    }

                    if ((richTextBox2.SelectedText == " " && worDelimiters.Exists(a => a == "רווח") && charactersFromLastAnchor > 0)
                        ||
                        (worDelimiters.Exists(a => a == richTextBox2.SelectedText) && charactersFromLastAnchor > 0))
                    {
                        AddAnchor(AnchorType.Word, index);
                        anchors++;
                        charactersFromLastAnchor = 0;
                        index += 3;
                        continue;
                    }

                    charactersFromLastAnchor++;

                }
                richTextBox1.Rtf = richTextBox2.Rtf;
            }
            catch
            {

            }
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
    }
}
