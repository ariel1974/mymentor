using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using AudioSoundEditor;

namespace SoundStudio
{
	/// <summary>
	/// Summary description for FormBassBoost.
	/// </summary>
	public class FormBassBoost : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonAboutBox;
		private System.Windows.Forms.Label labelDspUIPosition;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		internal AudioSoundEditor.AudioSoundEditor	audioSoundEditor1;

		internal Int32	m_idDspBassBoostExternal;
		public bool		m_bCancel;

		public FormBassBoost()
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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonAboutBox = new System.Windows.Forms.Button();
			this.labelDspUIPosition = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(240, 232);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(104, 24);
			this.buttonCancel.TabIndex = 14;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(112, 232);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(96, 24);
			this.buttonOK.TabIndex = 13;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(392, 32);
			this.label1.TabIndex = 12;
			this.label1.Text = "This sample demonstrates how to apply a custom DSP (Bass Boost), contained inside" +
				" an external DLL, which comes with its own User Interface";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonAboutBox
			// 
			this.buttonAboutBox.Location = new System.Drawing.Point(152, 176);
			this.buttonAboutBox.Name = "buttonAboutBox";
			this.buttonAboutBox.Size = new System.Drawing.Size(144, 32);
			this.buttonAboutBox.TabIndex = 11;
			this.buttonAboutBox.Text = "Display DSP\'s About box";
			this.buttonAboutBox.Click += new System.EventHandler(this.buttonAboutBox_Click);
			// 
			// labelDspUIPosition
			// 
			this.labelDspUIPosition.Location = new System.Drawing.Point(24, 72);
			this.labelDspUIPosition.Name = "labelDspUIPosition";
			this.labelDspUIPosition.Size = new System.Drawing.Size(80, 32);
			this.labelDspUIPosition.TabIndex = 15;
			this.labelDspUIPosition.Text = "label2";
			this.labelDspUIPosition.Visible = false;
			// 
			// FormBassBoost
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(456, 270);
			this.ControlBox = false;
			this.Controls.Add(this.labelDspUIPosition);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonAboutBox);
			this.Name = "FormBassBoost";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Bass Boost settings";
			this.Load += new System.EventHandler(this.FormBassBoost_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormBassBoost_Load(object sender, System.EventArgs e)
		{
		    // request the DSP to display its own User Interface
			audioSoundEditor1.Effects.CustomDspExternalEditorShow (m_idDspBassBoostExternal, true,
				this.Handle, labelDspUIPosition.Left, labelDspUIPosition.Top);
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			m_bCancel = false;
			Close ();		
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			m_bCancel = true;
			Close ();		
		}

		private void buttonAboutBox_Click(object sender, System.EventArgs e)
		{
			// request the DSP to display its own "about box" through the custom "AboutBox" command
			audioSoundEditor1.Effects.CustomDspExternalSendCommand (m_idDspBassBoostExternal, this.Handle, "AboutBox");
		}
	}
}
