using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using AudioSoundEditor;

namespace SoundStudio
{
	/// <summary>
	/// Summary description for FormOptions.
	/// </summary>
	public class FormOptions : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonMemoryBuffer;
		private System.Windows.Forms.RadioButton radioButtonTempFile;

		internal AudioSoundEditor.AudioSoundEditor	audioSoundEditor1;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormOptions()
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
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonTempFile = new System.Windows.Forms.RadioButton();
			this.radioButtonMemoryBuffer = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(24, 144);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(88, 32);
			this.buttonOK.TabIndex = 0;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(136, 144);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(96, 32);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonTempFile);
			this.groupBox1.Controls.Add(this.radioButtonMemoryBuffer);
			this.groupBox1.Location = new System.Drawing.Point(20, 24);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(216, 96);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Sound storage mode";
			// 
			// radioButtonTempFile
			// 
			this.radioButtonTempFile.Location = new System.Drawing.Point(24, 56);
			this.radioButtonTempFile.Name = "radioButtonTempFile";
			this.radioButtonTempFile.Size = new System.Drawing.Size(176, 24);
			this.radioButtonTempFile.TabIndex = 1;
			this.radioButtonTempFile.Text = "Inside temporary file";
			// 
			// radioButtonMemoryBuffer
			// 
			this.radioButtonMemoryBuffer.Location = new System.Drawing.Point(24, 24);
			this.radioButtonMemoryBuffer.Name = "radioButtonMemoryBuffer";
			this.radioButtonMemoryBuffer.Size = new System.Drawing.Size(184, 24);
			this.radioButtonMemoryBuffer.TabIndex = 0;
			this.radioButtonMemoryBuffer.Text = "Inside memory buffer";
			// 
			// FormOptions
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(256, 190);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Name = "FormOptions";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Options";
			this.Load += new System.EventHandler(this.FormOptions_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormOptions_Load(object sender, System.EventArgs e)
		{
			// get the current storage settings
			enumStoreModes	nStoreMode = audioSoundEditor1.GetStoreMode ();
			if (nStoreMode == enumStoreModes.STORE_MODE_MEMORY_BUFFER)
			{
				radioButtonMemoryBuffer.Checked = true;
				radioButtonTempFile.Checked = false;
			}
			else if (nStoreMode == enumStoreModes.STORE_MODE_TEMP_FILE)
			{
				radioButtonTempFile.Checked = true;
				radioButtonMemoryBuffer.Checked = false;
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			// set the new storage settings
			if (radioButtonMemoryBuffer.Checked == true)
				audioSoundEditor1.SetStoreMode (enumStoreModes.STORE_MODE_MEMORY_BUFFER);
			else if (radioButtonTempFile.Checked == true)
				audioSoundEditor1.SetStoreMode (enumStoreModes.STORE_MODE_TEMP_FILE);
			Close ();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Close ();
		}
	}
}
