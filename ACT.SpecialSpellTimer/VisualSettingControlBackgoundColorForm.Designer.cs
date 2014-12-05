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
            this.TojiruButton.Location = new System.Drawing.Point(96, 66);
            this.TojiruButton.Name = "TojiruButton";
            this.TojiruButton.Size = new System.Drawing.Size(75, 23);
            this.TojiruButton.TabIndex = 2;
            this.TojiruButton.Text = "キャンセル";
            this.TojiruButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(15, 66);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
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
            this.OpacityNumericUpDown.Location = new System.Drawing.Point(12, 17);
            this.OpacityNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.OpacityNumericUpDown.Name = "OpacityNumericUpDown";
            this.OpacityNumericUpDown.Size = new System.Drawing.Size(54, 19);
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
            this.label1.Location = new System.Drawing.Point(72, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "0:透明, 255:不透明";
            // 
            // AlphaRateLabel
            // 
            this.AlphaRateLabel.Location = new System.Drawing.Point(13, 36);
            this.AlphaRateLabel.Name = "AlphaRateLabel";
            this.AlphaRateLabel.Size = new System.Drawing.Size(43, 24);
            this.AlphaRateLabel.TabIndex = 4;
            this.AlphaRateLabel.Text = "100%";
            this.AlphaRateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // VisualSettingControlBackgoundColorForm
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.TojiruButton;
            this.ClientSize = new System.Drawing.Size(183, 101);
            this.Controls.Add(this.AlphaRateLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.OpacityNumericUpDown);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.TojiruButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "VisualSettingControlBackgoundColorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "背景色のアルファチャンネル";
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