using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace MyMentor
{
	/// <summary>
	/// Summary description for FormSilence.
	/// </summary>
	public class FormSilence : System.Windows.Forms.Form
	{
		public System.Windows.Forms.Label Label1;
		public System.Windows.Forms.Button buttonOK;
		public System.Windows.Forms.TextBox textboxSilenceLength;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        public Label label2;

		public Int32	m_nSilenceLengthInMs;

		[DllImport("user32.dll")]
		public static extern int SetWindowLong( IntPtr window, int index, int value);
		[DllImport("user32.dll")]
		public static extern int GetWindowLong( IntPtr window, int index);

		const int GWL_STYLE = -16;
		const int ES_NUMBER = 0x2000;

		public FormSilence()
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
            this.textboxSilenceLength = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.BackColor = System.Drawing.SystemColors.Control;
            this.buttonOK.Cursor = System.Windows.Forms.Cursors.Default;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonOK.Location = new System.Drawing.Point(99, 106);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonOK.Size = new System.Drawing.Size(84, 29);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "אישור";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // textboxSilenceLength
            // 
            this.textboxSilenceLength.AcceptsReturn = true;
            this.textboxSilenceLength.BackColor = System.Drawing.SystemColors.Window;
            this.textboxSilenceLength.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textboxSilenceLength.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textboxSilenceLength.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textboxSilenceLength.Location = new System.Drawing.Point(83, 45);
            this.textboxSilenceLength.MaxLength = 0;
            this.textboxSilenceLength.Name = "textboxSilenceLength";
            this.textboxSilenceLength.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.textboxSilenceLength.Size = new System.Drawing.Size(85, 26);
            this.textboxSilenceLength.TabIndex = 4;
            this.textboxSilenceLength.Text = "1000";
            // 
            // Label1
            // 
            this.Label1.BackColor = System.Drawing.SystemColors.Control;
            this.Label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.Label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Label1.Location = new System.Drawing.Point(12, 9);
            this.Label1.Name = "Label1";
            this.Label1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Label1.Size = new System.Drawing.Size(185, 23);
            this.Label1.TabIndex = 3;
            this.Label1.Text = "הקלד אורך קטע שקט";
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCancel.Location = new System.Drawing.Point(189, 106);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(85, 29);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "ביטול";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Cursor = System.Windows.Forms.Cursors.Default;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.Location = new System.Drawing.Point(174, 48);
            this.label2.Name = "label2";
            this.label2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.label2.Size = new System.Drawing.Size(70, 23);
            this.label2.TabIndex = 7;
            this.label2.Text = "מיל\' שניות";
            // 
            // FormSilence
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 19);
            this.ClientSize = new System.Drawing.Size(283, 147);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textboxSilenceLength);
            this.Controls.Add(this.Label1);
            this.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FormSilence";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "אורך קטע שקט";
            this.Load += new System.EventHandler(this.FormSilence_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			m_nSilenceLengthInMs = Convert.ToInt32 (textboxSilenceLength.Text);
			Close ();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Close ();
		}

		private void FormSilence_Load(object sender, System.EventArgs e)
		{
			// set the numeric style for the textboxSilenceLength textbox
			// getting the current style then adding ES_NUMBER to the style
			Int32   nStyle;
			nStyle = GetWindowLong(textboxSilenceLength.Handle, GWL_STYLE);
			SetWindowLong (textboxSilenceLength.Handle, GWL_STYLE, nStyle | ES_NUMBER);

			m_nSilenceLengthInMs = -1;
		}
	}
}
