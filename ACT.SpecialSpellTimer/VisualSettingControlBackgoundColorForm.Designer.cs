namespace ACT.SpecialSpellTimer
{
    partial class VisualSettingControlBackgoundColorForm
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
            this.TojiruButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.OpacityNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.AlphaRateLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.OpacityNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // TojiruButton
            // 
            this.TojiruButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TojiruButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.TojiruButton.Location = new System.Drawing.Point(128, 88);
            this.TojiruButton.Margin = new System.Windows.Forms.Padding(4);
            this.TojiruButton.Name = "TojiruButton";
            this.TojiruButton.Size = new System.Drawing.Size(100, 31);
            this.TojiruButton.TabIndex = 2;
            this.TojiruButton.Text = "CancelButton";
            this.TojiruButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(20, 88);
            this.OKButton.Margin = new System.Windows.Forms.Padding(4);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(100, 31);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OKButton";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // OpacityNumericUpDown
            // 
            this.OpacityNumericUpDown.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.OpacityNumericUpDown.Location = new System.Drawing.Point(16, 23);
            this.OpacityNumericUpDown.Margin = new System.Windows.Forms.Padding(4);
            this.OpacityNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.OpacityNumericUpDown.Name = "OpacityNumericUpDown";
            this.OpacityNumericUpDown.Size = new System.Drawing.Size(72, 22);
            this.OpacityNumericUpDown.TabIndex = 0;
            this.OpacityNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.OpacityNumericUpDown.ThousandsSeparator = true;
            this.OpacityNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(96, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "OpacityDescription";
            // 
            // AlphaRateLabel
            // 
            this.AlphaRateLabel.Location = new System.Drawing.Point(17, 48);
            this.AlphaRateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.AlphaRateLabel.Name = "AlphaRateLabel";
            this.AlphaRateLabel.Size = new System.Drawing.Size(57, 32);
            this.AlphaRateLabel.TabIndex = 4;
            this.AlphaRateLabel.Text = "__100%";
            this.AlphaRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // VisualSettingControlBackgoundColorForm
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.TojiruButton;
            this.ClientSize = new System.Drawing.Size(244, 135);
            this.Controls.Add(this.AlphaRateLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpacityNumericUpDown);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.TojiruButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "VisualSettingControlBackgoundColorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BackgroundColorAlphaTitle";
            ((System.ComponentModel.ISupportInitialize)(this.OpacityNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TojiruButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.NumericUpDown OpacityNumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label AlphaRateLabel;
    }
}