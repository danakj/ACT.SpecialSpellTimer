namespace ACT.SpecialSpellTimer
{
    partial class SelectJobForm
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
            this.JobsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.OKButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.AllONButton = new System.Windows.Forms.Button();
            this.AllOFFButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // JobsCheckedListBox
            // 
            this.JobsCheckedListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.JobsCheckedListBox.BackColor = System.Drawing.SystemColors.Control;
            this.JobsCheckedListBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.JobsCheckedListBox.CheckOnClick = true;
            this.JobsCheckedListBox.FormattingEnabled = true;
            this.JobsCheckedListBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.JobsCheckedListBox.Location = new System.Drawing.Point(16, 44);
            this.JobsCheckedListBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.JobsCheckedListBox.MultiColumn = true;
            this.JobsCheckedListBox.Name = "JobsCheckedListBox";
            this.JobsCheckedListBox.Size = new System.Drawing.Size(637, 272);
            this.JobsCheckedListBox.TabIndex = 0;
            this.JobsCheckedListBox.ThreeDCheckBoxes = true;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(387, 348);
            this.OKButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(129, 35);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CloseButton.Location = new System.Drawing.Point(524, 348);
            this.CloseButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(129, 35);
            this.CloseButton.TabIndex = 2;
            this.CloseButton.Text = "キャンセル";
            this.CloseButton.UseVisualStyleBackColor = true;
            // 
            // AllONButton
            // 
            this.AllONButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AllONButton.Location = new System.Drawing.Point(16, 348);
            this.AllONButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AllONButton.Name = "AllONButton";
            this.AllONButton.Size = new System.Drawing.Size(100, 35);
            this.AllONButton.TabIndex = 3;
            this.AllONButton.Text = "全てON";
            this.AllONButton.UseVisualStyleBackColor = true;
            // 
            // AllOFFButton
            // 
            this.AllOFFButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AllOFFButton.Location = new System.Drawing.Point(124, 348);
            this.AllOFFButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AllOFFButton.Name = "AllOFFButton";
            this.AllOFFButton.Size = new System.Drawing.Size(100, 35);
            this.AllOFFButton.TabIndex = 4;
            this.AllOFFButton.Text = "全てOFF";
            this.AllOFFButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "※対象とするジョブを限定したい場合はチェックをONにしてください";
            // 
            // SelectJobForm
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CloseButton;
            this.ClientSize = new System.Drawing.Size(669, 398);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AllOFFButton);
            this.Controls.Add(this.AllONButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.JobsCheckedListBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectJobForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "対象のジョブを選択してください";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox JobsCheckedListBox;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button AllONButton;
        private System.Windows.Forms.Button AllOFFButton;
        private System.Windows.Forms.Label label1;
    }
}