using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using AudioSoundEditor;

namespace SoundStudio
{
	/// <summary>
	/// Summary description for FormHostVstEditor.
	/// </summary>
	public class FormHostVstEditor : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonHide;
		private System.Windows.Forms.Label labelVstEditorPosition;
		private System.Windows.Forms.Label labelEffectName;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal AudioSoundEditor.AudioSoundEditor	audioSoundEditor1;

		public Int32		m_idVst;
		private System.Windows.Forms.ComboBox comboBoxVstPrograms;
		private System.Windows.Forms.Label label3;
		public bool			m_bCancel;

		public FormHostVstEditor()
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
			this.buttonApply = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonHide = new System.Windows.Forms.Button();
			this.labelEffectName = new System.Windows.Forms.Label();
			this.labelVstEditorPosition = new System.Windows.Forms.Label();
			this.comboBoxVstPrograms = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonApply
			// 
			this.buttonApply.Location = new System.Drawing.Point(16, 8);
			this.buttonApply.Name = "buttonApply";
			this.buttonApply.Size = new System.Drawing.Size(136, 40);
			this.buttonApply.TabIndex = 0;
			this.buttonApply.Text = "Apply VST";
			this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Location = new System.Drawing.Point(168, 8);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(128, 40);
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonHide
			// 
			this.buttonHide.Location = new System.Drawing.Point(376, 8);
			this.buttonHide.Name = "buttonHide";
			this.buttonHide.Size = new System.Drawing.Size(160, 40);
			this.buttonHide.TabIndex = 2;
			this.buttonHide.Text = "Hide VST\'s User Interface";
			this.buttonHide.Click += new System.EventHandler(this.buttonHide_Click);
			// 
			// labelEffectName
			// 
			this.labelEffectName.Location = new System.Drawing.Point(16, 64);
			this.labelEffectName.Name = "labelEffectName";
			this.labelEffectName.Size = new System.Drawing.Size(352, 16);
			this.labelEffectName.TabIndex = 3;
			this.labelEffectName.Text = "- ";
			// 
			// labelVstEditorPosition
			// 
			this.labelVstEditorPosition.Location = new System.Drawing.Point(8, 104);
			this.labelVstEditorPosition.Name = "labelVstEditorPosition";
			this.labelVstEditorPosition.Size = new System.Drawing.Size(360, 32);
			this.labelVstEditorPosition.TabIndex = 4;
			this.labelVstEditorPosition.Text = "Reference label for VST\'s editor placement";
			this.labelVstEditorPosition.Visible = false;
			// 
			// comboBoxVstPrograms
			// 
			this.comboBoxVstPrograms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxVstPrograms.Location = new System.Drawing.Point(376, 72);
			this.comboBoxVstPrograms.Name = "comboBoxVstPrograms";
			this.comboBoxVstPrograms.Size = new System.Drawing.Size(192, 21);
			this.comboBoxVstPrograms.TabIndex = 52;
			this.comboBoxVstPrograms.SelectedIndexChanged += new System.EventHandler(this.comboBoxVstPrograms_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(376, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(192, 16);
			this.label3.TabIndex = 51;
			this.label3.Text = "Choose a VST effect program:";
			// 
			// FormHostVstEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(715, 506);
			this.ControlBox = false;
			this.Controls.Add(this.comboBoxVstPrograms);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labelVstEditorPosition);
			this.Controls.Add(this.labelEffectName);
			this.Controls.Add(this.buttonHide);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonApply);
			this.Name = "FormHostVstEditor";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "VST editor";
			this.Load += new System.EventHandler(this.FormHostVstEditor_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormHostVstEditor_Load(object sender, System.EventArgs e)
		{
			// get the name and the vendor of the VST effect
			string	strEffectName = audioSoundEditor1.Effects.VstGetInfoString (m_idVst, enumVstInfo.VST_INFO_EFFECT_NAME);
			string	strVendorName = audioSoundEditor1.Effects.VstGetInfoString (m_idVst, enumVstInfo.VST_INFO_VENDOR_NAME);

			// get the version of the effect
			AudioSoundEditor.VstEffectInfo	info = new AudioSoundEditor.VstEffectInfo ();
			audioSoundEditor1.Effects.VstGetInfo (m_idVst, ref info);
			string	strVstVersion = info.nVersion.ToString ();

			// update the information on the user interface
			labelEffectName.Text = strEffectName;
			labelEffectName.Text += " ver. ";
			labelEffectName.Text += strVstVersion;
			labelEffectName.Text += " developed by ";
			labelEffectName.Text += strVendorName;
    		
			// enumerate programs available inside the VST effect and add them to the combo box
			Int16	 nPrograms = audioSoundEditor1.Effects.VstProgramsGetCount (m_idVst);
			for (Int16 index = 0; index < nPrograms; index++)
				comboBoxVstPrograms.Items.Add (audioSoundEditor1.Effects.VstProgramNameGet (m_idVst, index));
			comboBoxVstPrograms.SelectedIndex = 0;

			// check if there is enough room on the form in order to display the editor
			AudioSoundEditor.VstEditorInfo	infoEditor = new VstEditorInfo ();
			audioSoundEditor1.Effects.VstEditorGetInfo (m_idVst, ref infoEditor);
			if (infoEditor.nEditorWidth > this.ClientRectangle.Width)
			{
				int	nWidthDiff = this.Width - this.ClientRectangle.Width;
				this.Width = (labelVstEditorPosition.Location.X * 3) + infoEditor.nEditorWidth + nWidthDiff;
			}
			if (infoEditor.nEditorHeight > (this.ClientRectangle.Height - labelVstEditorPosition.Location.Y))
			{
				int	nHeightDiff = this.Height - this.ClientRectangle.Height;
				this.Height = labelVstEditorPosition.Location.Y + infoEditor.nEditorHeight + nHeightDiff + 10;
			}

			// request the VST to display its own User Interface
			audioSoundEditor1.Effects.VstEditorShow (m_idVst, true,
				this.Handle, labelVstEditorPosition.Left, labelVstEditorPosition.Top);			
			buttonHide.Text = "Hide VST's User Interface";
		}

		private void buttonApply_Click(object sender, System.EventArgs e)
		{
			m_bCancel = false;
			Close ();		
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			m_bCancel = true;
			Close ();		
		}

		private void buttonHide_Click(object sender, System.EventArgs e)
		{
			// check editor visibility
			AudioSoundEditor.VstEditorInfo	infoEditor = new VstEditorInfo ();
			audioSoundEditor1.Effects.VstEditorGetInfo (m_idVst, ref infoEditor);
			if (!infoEditor.bIsEditorVisible)
			{
				// request the VST to display its own User Interface
				audioSoundEditor1.Effects.VstEditorShow (m_idVst, true, this.Handle, labelVstEditorPosition.Left, labelVstEditorPosition.Top);
				buttonHide.Text = "Hide VST's User Interface";
			}
			else
			{
				// request the DSP to hide its own User Interface
				audioSoundEditor1.Effects.VstEditorShow (m_idVst, false, this.Handle, labelVstEditorPosition.Left, labelVstEditorPosition.Top);
				buttonHide.Text = "Display VST's User Interface";
			}
		}

		private void comboBoxVstPrograms_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			audioSoundEditor1.Effects.VstProgramSetCurrent (m_idVst, (Int16) comboBoxVstPrograms.SelectedIndex);
		}
	}
}
