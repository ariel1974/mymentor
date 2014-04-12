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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReadyTexts));
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lblCategory2 = new System.Windows.Forms.Label();
            this.comboCategory2 = new System.Windows.Forms.ComboBox();
            this.comboCategory1 = new System.Windows.Forms.ComboBox();
            this.lblCategory1 = new System.Windows.Forms.Label();
            this.comboCategory3 = new System.Windows.Forms.ComboBox();
            this.lblCategory3 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button3 = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // lblCategory2
            // 
            resources.ApplyResources(this.lblCategory2, "lblCategory2");
            this.lblCategory2.Name = "lblCategory2";
            // 
            // comboCategory2
            // 
            resources.ApplyResources(this.comboCategory2, "comboCategory2");
            this.comboCategory2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategory2.FormattingEnabled = true;
            this.comboCategory2.Name = "comboCategory2";
            // 
            // comboCategory1
            // 
            resources.ApplyResources(this.comboCategory1, "comboCategory1");
            this.comboCategory1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategory1.FormattingEnabled = true;
            this.comboCategory1.Name = "comboCategory1";
            this.comboCategory1.SelectionChangeCommitted += new System.EventHandler(this.comboCategory1_SelectionChangeCommitted);
            // 
            // lblCategory1
            // 
            resources.ApplyResources(this.lblCategory1, "lblCategory1");
            this.lblCategory1.Name = "lblCategory1";
            // 
            // comboCategory3
            // 
            resources.ApplyResources(this.comboCategory3, "comboCategory3");
            this.comboCategory3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCategory3.FormattingEnabled = true;
            this.comboCategory3.Name = "comboCategory3";
            // 
            // lblCategory3
            // 
            resources.ApplyResources(this.lblCategory3, "lblCategory3");
            this.lblCategory3.Name = "lblCategory3";
            // 
            // btnSearch
            // 
            resources.ApplyResources(this.btnSearch, "btnSearch");
            this.btnSearch.Name = "btnSearch";
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
            resources.ApplyResources(this.button3, "button3");
            this.button3.Name = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // listView1
            // 
            resources.ApplyResources(this.listView1, "listView1");
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
            this.listView1.Name = "listView1";
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // FormReadyTexts
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.lblCategory2);
            this.Controls.Add(this.comboCategory2);
            this.Controls.Add(this.comboCategory1);
            this.Controls.Add(this.lblCategory1);
            this.Controls.Add(this.comboCategory3);
            this.Controls.Add(this.lblCategory3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReadyTexts";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
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
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}