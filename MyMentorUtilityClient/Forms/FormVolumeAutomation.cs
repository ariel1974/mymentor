using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using AudioSoundEditor;

namespace MyMentor
{
	/// <summary>
	/// Summary description for FormVolumeAutomation.
	/// </summary>
	public class FormVolumeAutomation : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.GroupBox groupBoxChannels;
		private System.Windows.Forms.RadioButton radioButtonRight;
		private System.Windows.Forms.RadioButton radioButtonLeft;
		private System.Windows.Forms.RadioButton radioButtonBoth;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textBoxPositionPoint0;
		private System.Windows.Forms.TextBox textBoxVolumePoint0;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.TextBox textBoxVolumePoint1;
		private System.Windows.Forms.TextBox textBoxPositionPoint1;
		private System.Windows.Forms.TextBox textBoxVolumePoint2;
		private System.Windows.Forms.TextBox textBoxPositionPoint2;
		private System.Windows.Forms.TextBox textBoxVolumePoint3;
		private System.Windows.Forms.TextBox textBoxPositionPoint3;
		private System.Windows.Forms.TextBox textBoxVolumePoint4;
		private System.Windows.Forms.TextBox textBoxPositionPoint4;
		private System.Windows.Forms.TextBox textBoxVolumePoint5;
		private System.Windows.Forms.TextBox textBoxPositionPoint5;
		
		internal AudioSoundEditor.AudioSoundEditor	audioSoundEditor1;

		public enumChannels	m_nAffectedChannels;
		public bool			m_bCancel;

		[DllImport("user32.dll")]
		public static extern int SetWindowLong( IntPtr window, int index, int value);
		[DllImport("user32.dll")]
		public static extern int GetWindowLong( IntPtr window, int index);

		const int GWL_STYLE = -16;
		const int ES_NUMBER = 0x2000;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FormVolumeAutomation()
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
			this.groupBoxChannels = new System.Windows.Forms.GroupBox();
			this.radioButtonRight = new System.Windows.Forms.RadioButton();
			this.radioButtonLeft = new System.Windows.Forms.RadioButton();
			this.radioButtonBoth = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textBoxVolumePoint0 = new System.Windows.Forms.TextBox();
			this.textBoxPositionPoint0 = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.textBoxVolumePoint1 = new System.Windows.Forms.TextBox();
			this.textBoxPositionPoint1 = new System.Windows.Forms.TextBox();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.textBoxVolumePoint2 = new System.Windows.Forms.TextBox();
			this.textBoxPositionPoint2 = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.textBoxVolumePoint3 = new System.Windows.Forms.TextBox();
			this.textBoxPositionPoint3 = new System.Windows.Forms.TextBox();
			this.label13 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.textBoxVolumePoint4 = new System.Windows.Forms.TextBox();
			this.textBoxPositionPoint4 = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.label16 = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.textBoxVolumePoint5 = new System.Windows.Forms.TextBox();
			this.textBoxPositionPoint5 = new System.Windows.Forms.TextBox();
			this.label17 = new System.Windows.Forms.Label();
			this.label18 = new System.Windows.Forms.Label();
			this.groupBoxChannels.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(360, 344);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(104, 24);
			this.buttonCancel.TabIndex = 9;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(360, 304);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(104, 24);
			this.buttonOK.TabIndex = 8;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// groupBoxChannels
			// 
			this.groupBoxChannels.Controls.Add(this.radioButtonRight);
			this.groupBoxChannels.Controls.Add(this.radioButtonLeft);
			this.groupBoxChannels.Controls.Add(this.radioButtonBoth);
			this.groupBoxChannels.Location = new System.Drawing.Point(348, 168);
			this.groupBoxChannels.Name = "groupBoxChannels";
			this.groupBoxChannels.Size = new System.Drawing.Size(128, 112);
			this.groupBoxChannels.TabIndex = 7;
			this.groupBoxChannels.TabStop = false;
			this.groupBoxChannels.Text = "Affected channels";
			// 
			// radioButtonRight
			// 
			this.radioButtonRight.Location = new System.Drawing.Point(16, 80);
			this.radioButtonRight.Name = "radioButtonRight";
			this.radioButtonRight.Size = new System.Drawing.Size(96, 16);
			this.radioButtonRight.TabIndex = 2;
			this.radioButtonRight.Text = "Right channel";
			// 
			// radioButtonLeft
			// 
			this.radioButtonLeft.Location = new System.Drawing.Point(16, 56);
			this.radioButtonLeft.Name = "radioButtonLeft";
			this.radioButtonLeft.Size = new System.Drawing.Size(96, 16);
			this.radioButtonLeft.TabIndex = 1;
			this.radioButtonLeft.Text = "Left channel";
			// 
			// radioButtonBoth
			// 
			this.radioButtonBoth.Location = new System.Drawing.Point(16, 32);
			this.radioButtonBoth.Name = "radioButtonBoth";
			this.radioButtonBoth.Size = new System.Drawing.Size(96, 16);
			this.radioButtonBoth.TabIndex = 0;
			this.radioButtonBoth.Text = "Both channels";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(480, 40);
			this.label1.TabIndex = 10;
			this.label1.Text = @"This is a small sample which demonstrates how to create volume automation. In this sample we will create 6 volume points at 6 different percentage positions: it's important to note that inside your application you could create as many volume points you need.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(480, 40);
			this.label2.TabIndex = 11;
			this.label2.Text = "For each given volume point you can modify both the position\'s percentage (relate" +
				"d to the whole loaded sound or to a selected range) and the percentage of volume" +
				" level using the schema below: ";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(56, 96);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(408, 16);
			this.label3.TabIndex = 12;
			this.label3.Text = "- 0% means silence";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(56, 112);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(408, 16);
			this.label4.TabIndex = 13;
			this.label4.Text = "- Values betwenn 1% and 99% will cause an attenuation of the original sound";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(56, 128);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(408, 16);
			this.label5.TabIndex = 14;
			this.label5.Text = "- 100% keeps the original volume level";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(56, 144);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(408, 16);
			this.label6.TabIndex = 15;
			this.label6.Text = "- Values higher than 100% will cause an amplification of the original sound";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textBoxVolumePoint0);
			this.groupBox1.Controls.Add(this.textBoxPositionPoint0);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Location = new System.Drawing.Point(16, 168);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(312, 45);
			this.groupBox1.TabIndex = 16;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Volume point 0";
			// 
			// textBoxVolumePoint0
			// 
			this.textBoxVolumePoint0.Location = new System.Drawing.Point(248, 16);
			this.textBoxVolumePoint0.Name = "textBoxVolumePoint0";
			this.textBoxVolumePoint0.Size = new System.Drawing.Size(40, 20);
			this.textBoxVolumePoint0.TabIndex = 3;
			this.textBoxVolumePoint0.Text = "0";
			// 
			// textBoxPositionPoint0
			// 
			this.textBoxPositionPoint0.Location = new System.Drawing.Point(88, 16);
			this.textBoxPositionPoint0.Name = "textBoxPositionPoint0";
			this.textBoxPositionPoint0.Size = new System.Drawing.Size(40, 20);
			this.textBoxPositionPoint0.TabIndex = 2;
			this.textBoxPositionPoint0.Text = "0";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(144, 18);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(96, 16);
			this.label8.TabIndex = 1;
			this.label8.Text = "Volume level (%)";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(8, 18);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(72, 16);
			this.label7.TabIndex = 0;
			this.label7.Text = "Position (%)";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.textBoxVolumePoint1);
			this.groupBox2.Controls.Add(this.textBoxPositionPoint1);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Location = new System.Drawing.Point(16, 216);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(312, 45);
			this.groupBox2.TabIndex = 17;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Volume point 1";
			// 
			// textBoxVolumePoint1
			// 
			this.textBoxVolumePoint1.Location = new System.Drawing.Point(248, 16);
			this.textBoxVolumePoint1.Name = "textBoxVolumePoint1";
			this.textBoxVolumePoint1.Size = new System.Drawing.Size(40, 20);
			this.textBoxVolumePoint1.TabIndex = 3;
			this.textBoxVolumePoint1.Text = "100";
			// 
			// textBoxPositionPoint1
			// 
			this.textBoxPositionPoint1.Location = new System.Drawing.Point(88, 16);
			this.textBoxPositionPoint1.Name = "textBoxPositionPoint1";
			this.textBoxPositionPoint1.Size = new System.Drawing.Size(40, 20);
			this.textBoxPositionPoint1.TabIndex = 2;
			this.textBoxPositionPoint1.Text = "20";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(144, 18);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(96, 16);
			this.label9.TabIndex = 1;
			this.label9.Text = "Volume level (%)";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(8, 18);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(72, 16);
			this.label10.TabIndex = 0;
			this.label10.Text = "Position (%)";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.textBoxVolumePoint2);
			this.groupBox3.Controls.Add(this.textBoxPositionPoint2);
			this.groupBox3.Controls.Add(this.label11);
			this.groupBox3.Controls.Add(this.label12);
			this.groupBox3.Location = new System.Drawing.Point(16, 264);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(312, 45);
			this.groupBox3.TabIndex = 18;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Volume point 2";
			// 
			// textBoxVolumePoint2
			// 
			this.textBoxVolumePoint2.Location = new System.Drawing.Point(248, 16);
			this.textBoxVolumePoint2.Name = "textBoxVolumePoint2";
			this.textBoxVolumePoint2.Size = new System.Drawing.Size(40, 20);
			this.textBoxVolumePoint2.TabIndex = 3;
			this.textBoxVolumePoint2.Text = "400";
			// 
			// textBoxPositionPoint2
			// 
			this.textBoxPositionPoint2.Location = new System.Drawing.Point(88, 16);
			this.textBoxPositionPoint2.Name = "textBoxPositionPoint2";
			this.textBoxPositionPoint2.Size = new System.Drawing.Size(40, 20);
			this.textBoxPositionPoint2.TabIndex = 2;
			this.textBoxPositionPoint2.Text = "40";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(144, 18);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(96, 16);
			this.label11.TabIndex = 1;
			this.label11.Text = "Volume level (%)";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(8, 18);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(72, 16);
			this.label12.TabIndex = 0;
			this.label12.Text = "Position (%)";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.textBoxVolumePoint3);
			this.groupBox4.Controls.Add(this.textBoxPositionPoint3);
			this.groupBox4.Controls.Add(this.label13);
			this.groupBox4.Controls.Add(this.label14);
			this.groupBox4.Location = new System.Drawing.Point(16, 312);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(312, 45);
			this.groupBox4.TabIndex = 19;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "Volume point 3";
			// 
			// textBoxVolumePoint3
			// 
			this.textBoxVolumePoint3.Location = new System.Drawing.Point(248, 16);
			this.textBoxVolumePoint3.Name = "textBoxVolumePoint3";
			this.textBoxVolumePoint3.Size = new System.Drawing.Size(40, 20);
			this.textBoxVolumePoint3.TabIndex = 3;
			this.textBoxVolumePoint3.Text = "20";
			// 
			// textBoxPositionPoint3
			// 
			this.textBoxPositionPoint3.Location = new System.Drawing.Point(88, 16);
			this.textBoxPositionPoint3.Name = "textBoxPositionPoint3";
			this.textBoxPositionPoint3.Size = new System.Drawing.Size(40, 20);
			this.textBoxPositionPoint3.TabIndex = 2;
			this.textBoxPositionPoint3.Text = "60";
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(144, 18);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(96, 16);
			this.label13.TabIndex = 1;
			this.label13.Text = "Volume level (%)";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label14
			// 
			this.label14.Location = new System.Drawing.Point(8, 18);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(72, 16);
			this.label14.TabIndex = 0;
			this.label14.Text = "Position (%)";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.textBoxVolumePoint4);
			this.groupBox5.Controls.Add(this.textBoxPositionPoint4);
			this.groupBox5.Controls.Add(this.label15);
			this.groupBox5.Controls.Add(this.label16);
			this.groupBox5.Location = new System.Drawing.Point(16, 360);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(312, 45);
			this.groupBox5.TabIndex = 20;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Volume point 4";
			// 
			// textBoxVolumePoint4
			// 
			this.textBoxVolumePoint4.Location = new System.Drawing.Point(248, 16);
			this.textBoxVolumePoint4.Name = "textBoxVolumePoint4";
			this.textBoxVolumePoint4.Size = new System.Drawing.Size(40, 20);
			this.textBoxVolumePoint4.TabIndex = 3;
			this.textBoxVolumePoint4.Text = "100";
			// 
			// textBoxPositionPoint4
			// 
			this.textBoxPositionPoint4.Location = new System.Drawing.Point(88, 16);
			this.textBoxPositionPoint4.Name = "textBoxPositionPoint4";
			this.textBoxPositionPoint4.Size = new System.Drawing.Size(40, 20);
			this.textBoxPositionPoint4.TabIndex = 2;
			this.textBoxPositionPoint4.Text = "80";
			// 
			// label15
			// 
			this.label15.Location = new System.Drawing.Point(144, 18);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(96, 16);
			this.label15.TabIndex = 1;
			this.label15.Text = "Volume level (%)";
			this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(8, 18);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(72, 16);
			this.label16.TabIndex = 0;
			this.label16.Text = "Position (%)";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.textBoxVolumePoint5);
			this.groupBox6.Controls.Add(this.textBoxPositionPoint5);
			this.groupBox6.Controls.Add(this.label17);
			this.groupBox6.Controls.Add(this.label18);
			this.groupBox6.Location = new System.Drawing.Point(16, 408);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(312, 45);
			this.groupBox6.TabIndex = 21;
			this.groupBox6.TabStop = false;
			this.groupBox6.Text = "Volume point 5";
			// 
			// textBoxVolumePoint5
			// 
			this.textBoxVolumePoint5.Location = new System.Drawing.Point(248, 16);
			this.textBoxVolumePoint5.Name = "textBoxVolumePoint5";
			this.textBoxVolumePoint5.Size = new System.Drawing.Size(40, 20);
			this.textBoxVolumePoint5.TabIndex = 3;
			this.textBoxVolumePoint5.Text = "0";
			// 
			// textBoxPositionPoint5
			// 
			this.textBoxPositionPoint5.Location = new System.Drawing.Point(88, 16);
			this.textBoxPositionPoint5.Name = "textBoxPositionPoint5";
			this.textBoxPositionPoint5.Size = new System.Drawing.Size(40, 20);
			this.textBoxPositionPoint5.TabIndex = 2;
			this.textBoxPositionPoint5.Text = "100";
			// 
			// label17
			// 
			this.label17.Location = new System.Drawing.Point(144, 18);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(96, 16);
			this.label17.TabIndex = 1;
			this.label17.Text = "Volume level (%)";
			this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(8, 18);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(72, 16);
			this.label18.TabIndex = 0;
			this.label18.Text = "Position (%)";
			this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormVolumeAutomation
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(496, 462);
			this.ControlBox = false;
			this.Controls.Add(this.groupBox6);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.groupBoxChannels);
			this.Name = "FormVolumeAutomation";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Volume automation settings";
			this.Load += new System.EventHandler(this.FormVolumeAutomation_Load);
			this.groupBoxChannels.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void FormVolumeAutomation_Load(object sender, System.EventArgs e)
		{
			// set the numeric style for the available textboxes
			// getting the current style then adding ES_NUMBER to the style
			Int32   nStyle = GetWindowLong(textBoxPositionPoint0.Handle, GWL_STYLE);
			SetWindowLong (textBoxPositionPoint0.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxPositionPoint1.Handle, GWL_STYLE);
			SetWindowLong (textBoxPositionPoint1.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxPositionPoint2.Handle, GWL_STYLE);
			SetWindowLong (textBoxPositionPoint2.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxPositionPoint3.Handle, GWL_STYLE);
			SetWindowLong (textBoxPositionPoint3.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxPositionPoint4.Handle, GWL_STYLE);
			SetWindowLong (textBoxPositionPoint4.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxPositionPoint5.Handle, GWL_STYLE);
			SetWindowLong (textBoxPositionPoint5.Handle, GWL_STYLE, nStyle | ES_NUMBER);

			nStyle = GetWindowLong(textBoxVolumePoint0.Handle, GWL_STYLE);
			SetWindowLong (textBoxVolumePoint0.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxVolumePoint1.Handle, GWL_STYLE);
			SetWindowLong (textBoxVolumePoint1.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxVolumePoint2.Handle, GWL_STYLE);
			SetWindowLong (textBoxVolumePoint2.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxVolumePoint3.Handle, GWL_STYLE);
			SetWindowLong (textBoxVolumePoint3.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxVolumePoint4.Handle, GWL_STYLE);
			SetWindowLong (textBoxVolumePoint4.Handle, GWL_STYLE, nStyle | ES_NUMBER);
			nStyle = GetWindowLong(textBoxVolumePoint5.Handle, GWL_STYLE);
			SetWindowLong (textBoxVolumePoint5.Handle, GWL_STYLE, nStyle | ES_NUMBER);

			if (audioSoundEditor1.GetChannels() == 2)
			{
				groupBoxChannels.Enabled = true;
				radioButtonBoth.Checked = true;
			}
			else
			{
				groupBoxChannels.Enabled = false;
				radioButtonBoth.Enabled = false;
				radioButtonLeft.Enabled = false;
				radioButtonRight.Enabled = false;
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			// reset the current volume points
			audioSoundEditor1.Effects.VolumeAutomationReset ();
		    
			// add the new volume points
			float	fPosition = Convert.ToSingle (textBoxPositionPoint0.Text);
			float	fVolumeLevel = Convert.ToSingle (textBoxVolumePoint0.Text);
			audioSoundEditor1.Effects.VolumeAutomationPointAddNew (fPosition, fVolumeLevel, enumVolumeCurves.VOLUME_CURVE_NONE, 0);

			fPosition = Convert.ToSingle (textBoxPositionPoint1.Text);
			fVolumeLevel = Convert.ToSingle (textBoxVolumePoint1.Text);
			audioSoundEditor1.Effects.VolumeAutomationPointAddNew (fPosition, fVolumeLevel, enumVolumeCurves.VOLUME_CURVE_NONE, 0);

			fPosition = Convert.ToSingle (textBoxPositionPoint2.Text);
			fVolumeLevel = Convert.ToSingle (textBoxVolumePoint2.Text);
			audioSoundEditor1.Effects.VolumeAutomationPointAddNew (fPosition, fVolumeLevel, enumVolumeCurves.VOLUME_CURVE_NONE, 0);

			fPosition = Convert.ToSingle (textBoxPositionPoint3.Text);
			fVolumeLevel = Convert.ToSingle (textBoxVolumePoint3.Text);
			audioSoundEditor1.Effects.VolumeAutomationPointAddNew (fPosition, fVolumeLevel, enumVolumeCurves.VOLUME_CURVE_NONE, 0);

			fPosition = Convert.ToSingle (textBoxPositionPoint4.Text);
			fVolumeLevel = Convert.ToSingle (textBoxVolumePoint4.Text);
			audioSoundEditor1.Effects.VolumeAutomationPointAddNew (fPosition, fVolumeLevel, enumVolumeCurves.VOLUME_CURVE_NONE, 0);

			fPosition = Convert.ToSingle (textBoxPositionPoint5.Text);
			fVolumeLevel = Convert.ToSingle (textBoxVolumePoint5.Text);
			audioSoundEditor1.Effects.VolumeAutomationPointAddNew (fPosition, fVolumeLevel, enumVolumeCurves.VOLUME_CURVE_NONE, 0);

			if (radioButtonBoth.Checked)
				m_nAffectedChannels = enumChannels.CHANNELS_BOTH;
			else if (radioButtonLeft.Checked)
				m_nAffectedChannels = enumChannels.CHANNELS_LEFT;
			else if (radioButtonRight.Checked)
				m_nAffectedChannels = enumChannels.CHANNELS_RIGHT;

			m_bCancel = false;
			Close ();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			m_bCancel = true;
			Close ();
		}
	}
}
