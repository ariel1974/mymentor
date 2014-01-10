namespace MyMentor
{
    partial class FormReadyTexts
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lblCategory2 = new System.Windows.Forms.Label();
            this.comboCategory2 = new System.Windows.Forms.ComboBox();
            this.comboCategory1 = new System.Windows.Forms.ComboBox();
            this.lblCategory1 = new System.Windows.Forms.Label();
            this.comboCategory3 = new System.Windows.Forms.ComboBox();
            this.lblCategory3 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Enabled = false;
            this.button2.Font = new System.Drawing.Font("Arial", 12F);
            this.button2.Location = new System.Drawing.Point(493, 349);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 40);
            this.button2.TabIndex = 65;
            this.button2.Text = "אישור";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Font = new System.Drawing.Font("Arial", 12F);
            this.button1.Location = new System.Drawing.Point(592, 349);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 40);
            this.button1.TabIndex = 64;
            this.button1.Text = "ביטול";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lblCategory2
            // 
            this.lblCategory2.Font = new System.Drawing.Font("Arial", 12F);
            this.lblCategory2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCategory2.Location = new System.Drawing.Point(16, 43);
            this.lblCategory2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCategory2.Name = "lblCategory2";
            this.lblCategory2.Size = new System.Drawing.Size(81, 18);
            this.lblCategory2.TabIndex = 77;
            this.lblCategory2.Text = "טוען...";
            // 
            // comboCategory2
            // 
            this.comboCategory2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategory2.FormattingEnabled = true;
            this.comboCategory2.Location = new System.Drawing.Point(119, 43);
            this.comboCategory2.Name = "comboCategory2";
            this.comboCategory2.Size = new System.Drawing.Size(220, 26);
            this.comboCategory2.TabIndex = 76;
            // 
            // comboCategory1
            // 
            this.comboCategory1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategory1.FormattingEnabled = true;
            this.comboCategory1.Location = new System.Drawing.Point(119, 11);
            this.comboCategory1.Name = "comboCategory1";
            this.comboCategory1.Size = new System.Drawing.Size(220, 26);
            this.comboCategory1.TabIndex = 75;
            this.comboCategory1.SelectionChangeCommitted += new System.EventHandler(this.comboCategory1_SelectionChangeCommitted);
            // 
            // lblCategory1
            // 
            this.lblCategory1.Font = new System.Drawing.Font("Arial", 12F);
            this.lblCategory1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCategory1.Location = new System.Drawing.Point(16, 11);
            this.lblCategory1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCategory1.Name = "lblCategory1";
            this.lblCategory1.Size = new System.Drawing.Size(81, 18);
            this.lblCategory1.TabIndex = 74;
            this.lblCategory1.Text = "טוען...";
            // 
            // comboCategory3
            // 
            this.comboCategory3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategory3.FormattingEnabled = true;
            this.comboCategory3.Location = new System.Drawing.Point(119, 75);
            this.comboCategory3.Name = "comboCategory3";
            this.comboCategory3.Size = new System.Drawing.Size(220, 26);
            this.comboCategory3.TabIndex = 73;
            // 
            // lblCategory3
            // 
            this.lblCategory3.Font = new System.Drawing.Font("Arial", 12F);
            this.lblCategory3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblCategory3.Location = new System.Drawing.Point(16, 75);
            this.lblCategory3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCategory3.Name = "lblCategory3";
            this.lblCategory3.Size = new System.Drawing.Size(81, 18);
            this.lblCategory3.TabIndex = 68;
            this.lblCategory3.Text = "טוען...";
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 18;
            this.listBox1.Location = new System.Drawing.Point(16, 121);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(668, 202);
            this.listBox1.TabIndex = 80;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("Arial", 12F);
            this.btnSearch.Location = new System.Drawing.Point(364, 61);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(91, 40);
            this.btnSearch.TabIndex = 81;
            this.btnSearch.Text = "חיפוש";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 2000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Arial", 12F);
            this.button3.Location = new System.Drawing.Point(364, 13);
            this.button3.Margin = new System.Windows.Forms.Padding(4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(91, 40);
            this.button3.TabIndex = 82;
            this.button3.Text = "נקה";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // FormReadyTexts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(702, 402);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.lblCategory2);
            this.Controls.Add(this.comboCategory2);
            this.Controls.Add(this.comboCategory1);
            this.Controls.Add(this.lblCategory1);
            this.Controls.Add(this.comboCategory3);
            this.Controls.Add(this.lblCategory3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Font = new System.Drawing.Font("Arial", 12F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReadyTexts";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "חיפוש טקסט";
            this.Load += new System.EventHandler(this.FormReadyTexts_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblCategory2;
        private System.Windows.Forms.ComboBox comboCategory2;
        private System.Windows.Forms.ComboBox comboCategory1;
        private System.Windows.Forms.Label lblCategory1;
        private System.Windows.Forms.ComboBox comboCategory3;
        private System.Windows.Forms.Label lblCategory3;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button3;
    }
}