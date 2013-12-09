using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SoundStudio
{
	/// <summary>
	/// Summary description for FormChooseBackgroundFile.
	/// </summary>
	public class FormChooseBackgroundFile : System.Windows.Forms.Form
	{
		public System.Windows.Forms.ToolTip ToolTip1;
		public System.Windows.Forms.Label Label1;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		public System.Windows.Forms.Button buttonOK;
		public System.Windows.Forms.CheckBox checkboxLoop;
		public System.Windows.Forms.Button buttonBrowse;
		public System.Windows.Forms.TextBox textboxPathname;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Button buttonCancel;

		public string	m_strPathname;
		public bool		m_bLoop;

		public FormChooseBackgroundFile()
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
			this.components = new System.ComponentModel.Container();
			this.buttonOK = new System.Windows.Forms.Button();
			this.checkboxLoop = new System.Windows.Forms.CheckBox();
			this.buttonBrowse = new System.Windows.Forms.Button();
			this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.textboxPathname = new System.Windows.Forms.TextBox();
			this.Label1 = new System.Windows.Forms.Label();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.BackColor = System.Drawing.SystemColors.Control;
			this.buttonOK.Cursor = System.Windows.Forms.Cursors.Default;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.buttonOK.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonOK.Location = new System.Drawing.Point(144, 88);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.buttonOK.Size = new System.Drawing.Size(96, 25);
			this.buttonOK.TabIndex = 9;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// checkboxLoop
			// 
			this.checkboxLoop.BackColor = System.Drawing.SystemColors.Control;
			this.checkboxLoop.Checked = true;
			this.checkboxLoop.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkboxLoop.Cursor = System.Windows.Forms.Cursors.Default;
			this.checkboxLoop.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.checkboxLoop.ForeColor = System.Drawing.SystemColors.ControlText;
			this.checkboxLoop.Location = new System.Drawing.Point(8, 52);
			this.checkboxLoop.Name = "checkboxLoop";
			this.checkboxLoop.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.checkboxLoop.Size = new System.Drawing.Size(125, 21);
			this.checkboxLoop.TabIndex = 8;
			this.checkboxLoop.Text = "Apply in loop";
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.BackColor = System.Drawing.SystemColors.Control;
			this.buttonBrowse.Cursor = System.Windows.Forms.Cursors.Default;
			this.buttonBrowse.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.buttonBrowse.ForeColor = System.Drawing.SystemColors.ControlText;
			this.buttonBrowse.Location = new System.Drawing.Point(420, 24);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.buttonBrowse.Size = new System.Drawing.Size(76, 25);
			this.buttonBrowse.TabIndex = 7;
			this.buttonBrowse.Text = "Browse...";
			this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
			// 
			// textboxPathname
			// 
			this.textboxPathname.AcceptsReturn = true;
			this.textboxPathname.AutoSize = false;
			this.textboxPathname.BackColor = System.Drawing.SystemColors.Window;
			this.textboxPathname.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.textboxPathname.Font = new System.Drawing.Font("Arial", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.textboxPathname.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textboxPathname.Location = new System.Drawing.Point(8, 24);
			this.textboxPathname.MaxLength = 0;
			this.textboxPathname.Name = "textboxPathname";
			this.textboxPathname.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.textboxPathname.Size = new System.Drawing.Size(409, 21);
			this.textboxPathname.TabIndex = 5;
			this.textboxPathname.Text = "";
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
			this.Label1.Size = new System.Drawing.Size(377, 17);
			this.Label1.TabIndex = 6;
			this.Label1.Text = "Enter sound file\'s full pathname or press the Browse button";
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(264, 88);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(96, 25);
			this.buttonCancel.TabIndex = 10;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// FormChooseBackgroundFile
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(504, 121);
			this.ControlBox = false;
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.checkboxLoop);
			this.Controls.Add(this.buttonBrowse);
			this.Controls.Add(this.textboxPathname);
			this.Controls.Add(this.Label1);
			this.Controls.Add(this.buttonOK);
			this.Name = "FormChooseBackgroundFile";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Choose background file";
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			m_strPathname = textboxPathname.Text;
			m_bLoop = checkboxLoop.Checked;

			Close ();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Close ();
		}

		private void buttonBrowse_Click(object sender, System.EventArgs e)
		{
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
			DialogResult	result = openFileDialog1.ShowDialog();
			if (result == DialogResult.Cancel)
				return;
			
			textboxPathname.Text = openFileDialog1.FileName;		
		}
	}
}
