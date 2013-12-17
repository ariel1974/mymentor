namespace MyMentor.Forms
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
            this.label20 = new System.Windows.Forms.Label();
            this.comboWorldContentType = new System.Windows.Forms.ComboBox();
            this.comboKria = new System.Windows.Forms.ComboBox();
            this.label21 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Arial", 12F);
            this.label20.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label20.Location = new System.Drawing.Point(28, 50);
            this.label20.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(63, 18);
            this.label20.TabIndex = 63;
            this.label20.Text = "עולם תוכן";
            // 
            // comboWorldContentType
            // 
            this.comboWorldContentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboWorldContentType.FormattingEnabled = true;
            this.comboWorldContentType.Location = new System.Drawing.Point(120, 50);
            this.comboWorldContentType.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboWorldContentType.Name = "comboWorldContentType";
            this.comboWorldContentType.Size = new System.Drawing.Size(181, 26);
            this.comboWorldContentType.TabIndex = 62;
            // 
            // comboKria
            // 
            this.comboKria.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboKria.FormattingEnabled = true;
            this.comboKria.Location = new System.Drawing.Point(120, 16);
            this.comboKria.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboKria.Name = "comboKria";
            this.comboKria.Size = new System.Drawing.Size(181, 26);
            this.comboKria.TabIndex = 61;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Arial", 12F);
            this.label21.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label21.Location = new System.Drawing.Point(44, 15);
            this.label21.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(47, 18);
            this.label21.TabIndex = 60;
            this.label21.Text = "קריאה";
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Font = new System.Drawing.Font("Arial", 12F);
            this.button2.Location = new System.Drawing.Point(572, 349);
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
            this.button1.Location = new System.Drawing.Point(671, 349);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(92, 40);
            this.button1.TabIndex = 64;
            this.button1.Text = "ביטול";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FormReadyTexts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 402);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.comboWorldContentType);
            this.Controls.Add(this.comboKria);
            this.Controls.Add(this.label21);
            this.Font = new System.Drawing.Font("Arial", 12F);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReadyTexts";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "חיפוש טקסט";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.ComboBox comboWorldContentType;
        private System.Windows.Forms.ComboBox comboKria;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
    }
}