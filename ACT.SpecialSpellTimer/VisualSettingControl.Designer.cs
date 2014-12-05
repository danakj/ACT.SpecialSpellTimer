namespace ACT.SpecialSpellTimer
{
    partial class VisualSettingControl
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.SamplePictureBox = new System.Windows.Forms.PictureBox();
            this.VisualSettingContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ChangeFontItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeFontColorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeFontOutlineColorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeBarColorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeBarOutlineColorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeBackgoundColorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ChangeBackgroundAlphaItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ResetSpellFontItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResetSpellBarSizeItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResetSpellColorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ResetTelopFontItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ResetTelopColorItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.LoadColorSetItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveColorSetItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BarSizeLabel = new System.Windows.Forms.Label();
            this.WidthNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.HeightNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.BarSizeXLabel = new System.Windows.Forms.Label();
            this.ColorDialog = new System.Windows.Forms.ColorDialog();
            this.FontDialog = new System.Windows.Forms.FontDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.SamplePictureBox)).BeginInit();
            this.VisualSettingContextMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WidthNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // SamplePictureBox
            // 
            this.SamplePictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SamplePictureBox.BackColor = System.Drawing.Color.Transparent;
            this.SamplePictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SamplePictureBox.Location = new System.Drawing.Point(3, 3);
            this.SamplePictureBox.Name = "SamplePictureBox";
            this.SamplePictureBox.Size = new System.Drawing.Size(300, 50);
            this.SamplePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.SamplePictureBox.TabIndex = 0;
            this.SamplePictureBox.TabStop = false;
            // 
            // VisualSettingContextMenuStrip
            // 
            this.VisualSettingContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChangeFontItem,
            this.ChangeFontColorItem,
            this.ChangeFontOutlineColorItem,
            this.ChangeBarColorItem,
            this.ChangeBarOutlineColorItem,
            this.ChangeBackgoundColorItem,
            this.ChangeBackgroundAlphaItem,
            this.toolStripSeparator1,
            this.ResetSpellFontItem,
            this.ResetSpellBarSizeItem,
            this.ResetSpellColorItem,
            this.toolStripSeparator2,
            this.ResetTelopFontItem,
            this.ResetTelopColorItem,
            this.toolStripSeparator3,
            this.LoadColorSetItem,
            this.SaveColorSetItem});
            this.VisualSettingContextMenuStrip.Name = "ContextMenuStrip";
            this.VisualSettingContextMenuStrip.Size = new System.Drawing.Size(287, 330);
            // 
            // ChangeFontItem
            // 
            this.ChangeFontItem.Name = "ChangeFontItem";
            this.ChangeFontItem.Size = new System.Drawing.Size(286, 22);
            this.ChangeFontItem.Text = "フォントを変更する";
            // 
            // ChangeFontColorItem
            // 
            this.ChangeFontColorItem.Name = "ChangeFontColorItem";
            this.ChangeFontColorItem.Size = new System.Drawing.Size(286, 22);
            this.ChangeFontColorItem.Text = "フォントの色を変更する";
            // 
            // ChangeFontOutlineColorItem
            // 
            this.ChangeFontOutlineColorItem.Name = "ChangeFontOutlineColorItem";
            this.ChangeFontOutlineColorItem.Size = new System.Drawing.Size(286, 22);
            this.ChangeFontOutlineColorItem.Text = "フォントのアウトラインの色を変更する";
            // 
            // ChangeBarColorItem
            // 
            this.ChangeBarColorItem.Name = "ChangeBarColorItem";
            this.ChangeBarColorItem.Size = new System.Drawing.Size(286, 22);
            this.ChangeBarColorItem.Text = "バーの色を変更する";
            // 
            // ChangeBarOutlineColorItem
            // 
            this.ChangeBarOutlineColorItem.Name = "ChangeBarOutlineColorItem";
            this.ChangeBarOutlineColorItem.Size = new System.Drawing.Size(286, 22);
            this.ChangeBarOutlineColorItem.Text = "バーのアウトラインの色を変更する";
            // 
            // ChangeBackgoundColorItem
            // 
            this.ChangeBackgoundColorItem.Name = "ChangeBackgoundColorItem";
            this.ChangeBackgoundColorItem.Size = new System.Drawing.Size(286, 22);
            this.ChangeBackgoundColorItem.Text = "背景色を変更する";
            // 
            // ChangeBackgroundAlphaItem
            // 
            this.ChangeBackgroundAlphaItem.Name = "ChangeBackgroundAlphaItem";
            this.ChangeBackgroundAlphaItem.Size = new System.Drawing.Size(286, 22);
            this.ChangeBackgroundAlphaItem.Text = "背景色のアルファチャンネルを変更する";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(283, 6);
            // 
            // ResetSpellFontItem
            // 
            this.ResetSpellFontItem.Name = "ResetSpellFontItem";
            this.ResetSpellFontItem.Size = new System.Drawing.Size(286, 22);
            this.ResetSpellFontItem.Text = "スペルをこのフォントで統一する";
            // 
            // ResetSpellBarSizeItem
            // 
            this.ResetSpellBarSizeItem.Name = "ResetSpellBarSizeItem";
            this.ResetSpellBarSizeItem.Size = new System.Drawing.Size(286, 22);
            this.ResetSpellBarSizeItem.Text = "スペルをこのバーサイズで統一する";
            // 
            // ResetSpellColorItem
            // 
            this.ResetSpellColorItem.Name = "ResetSpellColorItem";
            this.ResetSpellColorItem.Size = new System.Drawing.Size(286, 22);
            this.ResetSpellColorItem.Text = "スペルをこの配色で統一する";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(283, 6);
            // 
            // ResetTelopFontItem
            // 
            this.ResetTelopFontItem.Name = "ResetTelopFontItem";
            this.ResetTelopFontItem.Size = new System.Drawing.Size(286, 22);
            this.ResetTelopFontItem.Text = "テロップをこのフォントで統一する";
            // 
            // ResetTelopColorItem
            // 
            this.ResetTelopColorItem.Name = "ResetTelopColorItem";
            this.ResetTelopColorItem.Size = new System.Drawing.Size(286, 22);
            this.ResetTelopColorItem.Text = "テロップをこの配色で統一する";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(283, 6);
            // 
            // LoadColorSetItem
            // 
            this.LoadColorSetItem.Name = "LoadColorSetItem";
            this.LoadColorSetItem.Size = new System.Drawing.Size(286, 22);
            this.LoadColorSetItem.Text = "配色を読み込む";
            // 
            // SaveColorSetItem
            // 
            this.SaveColorSetItem.Name = "SaveColorSetItem";
            this.SaveColorSetItem.Size = new System.Drawing.Size(286, 22);
            this.SaveColorSetItem.Text = "配色を保存する";
            // 
            // BarSizeLabel
            // 
            this.BarSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BarSizeLabel.AutoSize = true;
            this.BarSizeLabel.Location = new System.Drawing.Point(110, 62);
            this.BarSizeLabel.Name = "BarSizeLabel";
            this.BarSizeLabel.Size = new System.Drawing.Size(60, 12);
            this.BarSizeLabel.TabIndex = 2;
            this.BarSizeLabel.Text = "ﾊﾞｰのｻｲｽﾞ";
            this.BarSizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // WidthNumericUpDown
            // 
            this.WidthNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.WidthNumericUpDown.Location = new System.Drawing.Point(180, 59);
            this.WidthNumericUpDown.Maximum = new decimal(new int[] {
            1920,
            0,
            0,
            0});
            this.WidthNumericUpDown.Name = "WidthNumericUpDown";
            this.WidthNumericUpDown.Size = new System.Drawing.Size(47, 19);
            this.WidthNumericUpDown.TabIndex = 3;
            this.WidthNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.WidthNumericUpDown.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // HeightNumericUpDown
            // 
            this.HeightNumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HeightNumericUpDown.Location = new System.Drawing.Point(256, 59);
            this.HeightNumericUpDown.Maximum = new decimal(new int[] {
            30,
            0,
            0,
            0});
            this.HeightNumericUpDown.Name = "HeightNumericUpDown";
            this.HeightNumericUpDown.Size = new System.Drawing.Size(47, 19);
            this.HeightNumericUpDown.TabIndex = 4;
            this.HeightNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.HeightNumericUpDown.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
            // 
            // BarSizeXLabel
            // 
            this.BarSizeXLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.BarSizeXLabel.AutoSize = true;
            this.BarSizeXLabel.Location = new System.Drawing.Point(233, 62);
            this.BarSizeXLabel.Name = "BarSizeXLabel";
            this.BarSizeXLabel.Size = new System.Drawing.Size(17, 12);
            this.BarSizeXLabel.TabIndex = 5;
            this.BarSizeXLabel.Text = "×";
            // 
            // ColorDialog
            // 
            this.ColorDialog.AnyColor = true;
            this.ColorDialog.FullOpen = true;
            // 
            // FontDialog
            // 
            this.FontDialog.AllowVerticalFonts = false;
            this.FontDialog.ShowEffects = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.DodgerBlue;
            this.label3.Location = new System.Drawing.Point(3, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "※右クリックで変更";
            // 
            // SaveFileDialog
            // 
            this.SaveFileDialog.DefaultExt = "xml";
            this.SaveFileDialog.Filter = "XMLファイル (*.xml)|*.xml|全てのファイル (*.*)|*.*";
            this.SaveFileDialog.RestoreDirectory = true;
            // 
            // OpenFileDialog
            // 
            this.OpenFileDialog.DefaultExt = "xml";
            this.OpenFileDialog.Filter = "XMLファイル (*.xml)|*.xml|全てのファイル (*.*)|*.*";
            this.OpenFileDialog.RestoreDirectory = true;
            // 
            // VisualSettingControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ContextMenuStrip = this.VisualSettingContextMenuStrip;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BarSizeXLabel);
            this.Controls.Add(this.HeightNumericUpDown);
            this.Controls.Add(this.WidthNumericUpDown);
            this.Controls.Add(this.BarSizeLabel);
            this.Controls.Add(this.SamplePictureBox);
            this.Name = "VisualSettingControl";
            this.Size = new System.Drawing.Size(306, 85);
            ((System.ComponentModel.ISupportInitialize)(this.SamplePictureBox)).EndInit();
            this.VisualSettingContextMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.WidthNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.HeightNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox SamplePictureBox;
        private System.Windows.Forms.ContextMenuStrip VisualSettingContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem ChangeFontItem;
        private System.Windows.Forms.ToolStripMenuItem ChangeFontColorItem;
        private System.Windows.Forms.ToolStripMenuItem ChangeFontOutlineColorItem;
        private System.Windows.Forms.ToolStripMenuItem ChangeBarColorItem;
        private System.Windows.Forms.ToolStripMenuItem ChangeBarOutlineColorItem;
        private System.Windows.Forms.Label BarSizeLabel;
        private System.Windows.Forms.NumericUpDown WidthNumericUpDown;
        private System.Windows.Forms.NumericUpDown HeightNumericUpDown;
        private System.Windows.Forms.Label BarSizeXLabel;
        private System.Windows.Forms.ColorDialog ColorDialog;
        private System.Windows.Forms.FontDialog FontDialog;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripMenuItem SaveColorSetItem;
        private System.Windows.Forms.ToolStripMenuItem LoadColorSetItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.SaveFileDialog SaveFileDialog;
        private System.Windows.Forms.OpenFileDialog OpenFileDialog;
        private System.Windows.Forms.ToolStripMenuItem ChangeBackgoundColorItem;
        private System.Windows.Forms.ToolStripMenuItem ChangeBackgroundAlphaItem;
        private System.Windows.Forms.ToolStripMenuItem ResetSpellFontItem;
        private System.Windows.Forms.ToolStripMenuItem ResetSpellBarSizeItem;
        private System.Windows.Forms.ToolStripMenuItem ResetSpellColorItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ResetTelopFontItem;
        private System.Windows.Forms.ToolStripMenuItem ResetTelopColorItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    }
}
