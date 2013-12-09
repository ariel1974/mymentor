using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SoundStudio
{
	/// <summary>
	/// Summary description for FormBalance.
	/// </summary>
	public class FormBalance : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonAboutBox;
		private System.Windows.Forms.TrackBar trackBarBalanceExternal;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public bool		m_bUseInternal;
		public Int16	m_nBalancePercentage;
		public bool		m_bCancel;
		public Int32	m_idDspBalanceExternal;
		
		internal AudioSoundEditor.AudioSoundEditor	audioSoundEditor1;

		public FormBalance()
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
			this.buttonAboutBox = new System.Windows.Forms.Button();
			this.trackBarBalanceExternal = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.trackBarBalanceExternal)).BeginInit();
			this.SuspendLayout();
			// 
			// buttonAboutBox
			// 
			this.buttonAboutBox.Location = new System.Drawing.Point(104, 128);
			this.buttonAboutBox.Name = "buttonAboutBox";
			this.buttonAboutBox.Size = new System.Drawing.Size(144, 32);
			this.buttonAboutBox.TabIndex = 7;
			this.buttonAboutBox.Text = "Display DSP\'s About box";
			this.buttonAboutBox.Click += new System.EventHandler(this.buttonAboutBox_Click);
			// 
			// trackBarBalanceExternal
			// 
			this.trackBarBalanceExternal.Location = new System.Drawing.Point(72, 72);
			this.trackBarBalanceExternal.Maximum = 100;
			this.trackBarBalanceExternal.Minimum = -100;
			this.trackBarBalanceExternal.Name = "trackBarBalanceExternal";
			this.trackBarBalanceExternal.Size = new System.Drawing.Size(208, 45);
			this.trackBarBalanceExternal.SmallChange = 5;
			this.trackBarBalanceExternal.TabIndex = 6;
			this.trackBarBalanceExternal.TickFrequency = 10;
			this.trackBarBalanceExternal.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
			this.trackBarBalanceExternal.Scroll += new System.EventHandler(this.trackBarBalanceExternal_Scroll);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(328, 16);
			this.label1.TabIndex = 8;
			this.label1.Text = "This sample demonstrates how to apply a Balance custom DSP";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(188, 200);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(104, 24);
			this.buttonCancel.TabIndex = 10;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(60, 200);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(96, 24);
			this.buttonOK.TabIndex = 9;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(80, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 16);
			this.label2.TabIndex = 11;
			this.label2.Text = "Left";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(224, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(48, 16);
			this.label3.TabIndex = 12;
			this.label3.Text = "Right";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// FormBalance
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(352, 238);
			this.ControlBox = false;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonAboutBox);
			this.Controls.Add(this.trackBarBalanceExternal);
			this.Name = "FormBalance";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Balance DSP settings";
			this.Load += new System.EventHandler(this.FormBalance_Load);
			((System.ComponentModel.ISupportInitialize)(this.trackBarBalanceExternal)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void FormBalance_Load(object sender, System.EventArgs e)
		{
			if (m_bUseInternal)
				buttonAboutBox.Visible = false;
		}

		private void trackBarBalanceExternal_Scroll(object sender, System.EventArgs e)
		{
			if (m_bUseInternal)
				// set balance parameter directly
				m_nBalancePercentage = (Int16) trackBarBalanceExternal.Value;
			else
			{
				// send balance parameter to the external DSP
				BALANCE_PARAMETERS	paramsBalance = new BALANCE_PARAMETERS ();
				paramsBalance.nBalancePercentage = (Int16) trackBarBalanceExternal.Value;

				IntPtr	ptrParamsBalance = Marshal.AllocHGlobal(Marshal.SizeOf(paramsBalance));
				Marshal.StructureToPtr (paramsBalance, ptrParamsBalance, true);
				audioSoundEditor1.Effects.CustomDspExternalSetParameters (m_idDspBalanceExternal, ptrParamsBalance);
				Marshal.FreeHGlobal(ptrParamsBalance);
			}
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
			// request the DSP to display its own "about box"
			audioSoundEditor1.Effects.CustomDspExternalSendCommand (m_idDspBalanceExternal, this.Handle, "AboutBox");
		}
	}
	// define the data structure containing the parameters that will be passed to the external Balance DSP
	// it's important that this data structure reflects exactly (also in terms of bytes length) the data structure
	// used inside the DLL contaning the external Balance DSP
	[StructLayout(LayoutKind.Sequential)]
	public struct BALANCE_PARAMETERS
	{
		public Int16	nBalancePercentage;
	}
}
