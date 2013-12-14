namespace MyMentorUtilityClient.TimeSpinner
{
    partial class TimePickerSpinner
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.minutes = new MyMentorUtilityClient.TimeSpinner.NumericUpDownEx();
            this.seconds = new MyMentorUtilityClient.TimeSpinner.NumericUpDownEx();
            this.milliseconds = new MyMentorUtilityClient.TimeSpinner.NumericUpDownEx();
            ((System.ComponentModel.ISupportInitialize)(this.minutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.seconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.milliseconds)).BeginInit();
            this.SuspendLayout();
            // 
            // minutes
            // 
            this.minutes.Format = null;
            this.minutes.Location = new System.Drawing.Point(4, 3);
            this.minutes.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
            this.minutes.Name = "minutes";
            this.minutes.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.minutes.Size = new System.Drawing.Size(33, 20);
            this.minutes.TabIndex = 7;
            this.minutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.minutes.ValueChanged += new System.EventHandler(this.minutes_ValueChanged);
            // 
            // seconds
            // 
            this.seconds.Format = null;
            this.seconds.Location = new System.Drawing.Point(43, 3);
            this.seconds.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.seconds.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.seconds.Name = "seconds";
            this.seconds.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.seconds.Size = new System.Drawing.Size(34, 20);
            this.seconds.TabIndex = 6;
            this.seconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.seconds.ValueChanged += new System.EventHandler(this.seconds_ValueChanged);
            // 
            // milliseconds
            // 
            this.milliseconds.Format = "{0:000}";
            this.milliseconds.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.milliseconds.Location = new System.Drawing.Point(83, 3);
            this.milliseconds.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.milliseconds.Name = "milliseconds";
            this.milliseconds.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.milliseconds.Size = new System.Drawing.Size(32, 20);
            this.milliseconds.TabIndex = 5;
            this.milliseconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.milliseconds.ValueChanged += new System.EventHandler(this.milliseconds_ValueChanged);
            // 
            // TimePickerSpinner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.minutes);
            this.Controls.Add(this.seconds);
            this.Controls.Add(this.milliseconds);
            this.Name = "TimePickerSpinner";
            this.Size = new System.Drawing.Size(128, 27);
            ((System.ComponentModel.ISupportInitialize)(this.minutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.seconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.milliseconds)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private NumericUpDownEx milliseconds;
        private NumericUpDownEx seconds;
        private NumericUpDownEx minutes;
    }
}
