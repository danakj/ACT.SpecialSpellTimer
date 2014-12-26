namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Sound;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// Configパネル ワンポイントテロップ
    /// </summary>
    public partial class ConfigPanel
    {
        /// <summary>
        /// ワンポイントテロップのLoad
        /// </summary>
        private void LoadOnePointTelop()
        {
            // テロップテーブルをロードする
            this.LoadTelopTable();

            this.TelopDetailGroupBox.Visible = false;

            // コンボボックスにアイテムを装填する
            this.TelopMatchSoundComboBox.DataSource = SoundController.Default.EnumlateWave();
            this.TelopMatchSoundComboBox.ValueMember = "FullPath";
            this.TelopMatchSoundComboBox.DisplayMember = "Name";

            this.TelopDelaySoundComboBox.DataSource = SoundController.Default.EnumlateWave();
            this.TelopDelaySoundComboBox.ValueMember = "FullPath";
            this.TelopDelaySoundComboBox.DisplayMember = "Name";

            this.TelopPlay1Button.Click += (s1, e1) =>
            {
                SoundController.Default.Play((string)this.TelopMatchSoundComboBox.SelectedValue ?? string.Empty);
            };

            this.TelopPlay2Button.Click += (s1, e1) =>
            {
                SoundController.Default.Play((string)this.TelopDelaySoundComboBox.SelectedValue ?? string.Empty);
            };

            this.TelopSpeak1Button.Click += (s1, e1) =>
            {
                SoundController.Default.Play(this.TelopMatchTTSTextBox.Text);
            };

            this.TelopSpeak2Button.Click += (s1, e1) =>
            {
                SoundController.Default.Play(this.TelopDelayTTSTextBox.Text);
            };

            this.TelopTreeView.AfterCheck += (s1, e1) =>
            {
                var source = e1.Node.Tag as OnePointTelop;
                if (source != null)
                {
                    source.Enabled = e1.Node.Checked;
                }
            };

            this.TelopTreeView.AfterSelect += (s1, e1) =>
            {
                this.ShowTelopDetail(
                    e1.Node.Tag as OnePointTelop);
            };

            this.TelopSelectJobButton.Click += (s1, e1) =>
            {
                var src = this.TelopDetailGroupBox.Tag as OnePointTelop;
                if (src != null)
                {
                    using (var f = new SelectJobForm())
                    {
                        f.JobFilter = src.JobFilter;
                        if (f.ShowDialog(this) == DialogResult.OK)
                        {
                            src.JobFilter = f.JobFilter;
                        }
                    }
                }
            };

            this.TelopExportButton.Click += this.TelopExportButton_Click;
            this.TelopImportButton.Click += this.TelopImportButton_Click;
            this.TelopClearAllButton.Click += this.TelopClearAllButton_Click;
            this.TelopAddButton.Click += this.TelopAddButton_Click;
            this.TelopUpdateButton.Click += this.TelopUpdateButton_Click;
            this.TelopDeleteButton.Click += this.TelopDeleteButton_Click;
        }

        /// <summary>
        /// テロップエクスポート Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TelopExportButton_Click(object sender, EventArgs e)
        {
            this.SaveFileDialog.FileName = "ACT.SpecialSpellTimer.Telops.xml";
            if (this.SaveFileDialog.ShowDialog(this) != DialogResult.Cancel)
            {
                OnePointTelopTable.Default.Save(
                    this.SaveFileDialog.FileName);
            }
        }

        /// <summary>
        /// テロップインポート Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TelopImportButton_Click(object sender, EventArgs e)
        {
            this.OpenFileDialog.FileName = "ACT.SpecialSpellTimer.Telops.xml";
            if (this.OpenFileDialog.ShowDialog(this) != DialogResult.Cancel)
            {
                OnePointTelopTable.Default.Load(
                    this.OpenFileDialog.FileName,
                    false);

                this.LoadTelopTable();
            }
        }

        /// <summary>
        /// テロップ全て削除 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TelopClearAllButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this,
                Translate.Get("TelopClearAllPrompt"),
                "ACT.SpecialSpellTimer",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                lock (OnePointTelopTable.Default.Table)
                {
                    this.TelopDetailGroupBox.Visible = false;
                    OnePointTelopTable.Default.Table.Clear();
                }

                OnePointTelopController.CloseTelops();
                this.LoadTelopTable();
            }
        }

        /// <summary>
        /// テロップ追加 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TelopAddButton_Click(object sender, EventArgs e)
        {
            var nr = new OnePointTelop();

            nr.ID = OnePointTelopTable.Default.Table.Any() ?
                OnePointTelopTable.Default.Table.Max(x => x.ID) + 1 :
                1;
            nr.Title = Translate.Get("NewTelop");
            nr.DisplayTime = 3;
            nr.FontColor = Settings.Default.FontColor.ToHTML();
            nr.FontOutlineColor = Settings.Default.FontOutlineColor.ToHTML();
            nr.FontFamily = Settings.Default.Font.Name;
            nr.FontSize = Settings.Default.Font.Size;
            nr.FontStyle = (int)Settings.Default.Font.Style;
            nr.BackgroundColor = Settings.Default.BackgroundColor.ToHTML();
            nr.Left = 10.0d;
            nr.Top = 10.0d;
            nr.JobFilter = string.Empty;

            // 現在選択しているノードの情報を一部コピーする
            if (this.TelopTreeView.SelectedNode != null)
            {
                var baseRow = this.TelopTreeView.SelectedNode.Tag != null ?
                    this.TelopTreeView.SelectedNode.Tag as OnePointTelop :
                    this.TelopTreeView.SelectedNode.Nodes[0].Tag as OnePointTelop;

                if (baseRow != null)
                {
                    nr.Title = baseRow.Title + " New";
                    nr.Message = baseRow.Message;
                    nr.Keyword = baseRow.Keyword;
                    nr.KeywordToHide = baseRow.KeywordToHide;
                    nr.RegexEnabled = baseRow.RegexEnabled;
                    nr.Delay = baseRow.Delay;
                    nr.DisplayTime = baseRow.DisplayTime;
                    nr.AddMessageEnabled = baseRow.AddMessageEnabled;
                    nr.ProgressBarEnabled = baseRow.ProgressBarEnabled;
                    nr.FontColor = baseRow.FontColor;
                    nr.FontOutlineColor = baseRow.FontOutlineColor;
                    nr.FontFamily = baseRow.FontFamily;
                    nr.FontSize = baseRow.FontSize;
                    nr.FontStyle = baseRow.FontStyle;
                    nr.BackgroundColor = baseRow.BackgroundColor;
                    nr.BackgroundAlpha = baseRow.BackgroundAlpha;
                    nr.Left = baseRow.Left;
                    nr.Top = baseRow.Top;
                    nr.JobFilter = baseRow.JobFilter;
                }
            }

            nr.MatchDateTime = DateTime.MinValue;
            nr.Enabled = true;
            nr.Regex = null;
            nr.RegexPattern = string.Empty;
            nr.RegexToHide = null;
            nr.RegexPatternToHide = string.Empty;

            OnePointTelopTable.Default.Table.Add(nr);

            OnePointTelopTable.Default.Save();

            // 新しいノードを生成する
            var node = new TreeNode(nr.Title)
            {
                Tag = nr,
                ToolTipText = nr.Message,
                Checked = nr.Enabled
            };

            this.TelopTreeView.Nodes.Add(node);
            this.TelopTreeView.SelectedNode = node;
        }

        /// <summary>
        /// テロップ更新 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TelopUpdateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.TelopTitleTextBox.Text))
            {
                MessageBox.Show(
                    this,
                    Translate.Get("UpdateTelopNameTitle"),
                    "ACT.SpecialSpellTimer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            var src = this.TelopDetailGroupBox.Tag as OnePointTelop;
            if (src != null)
            {
                src.Title = this.TelopTitleTextBox.Text;
                src.Message = this.TelopMessageTextBox.Text;
                src.Keyword = this.TelopKeywordTextBox.Text;
                src.KeywordToHide = this.TelopKeywordToHideTextBox.Text;
                src.RegexEnabled = this.TelopRegexEnabledCheckBox.Checked;
                src.Delay = (long)this.TelopDelayNumericUpDown.Value;
                src.DisplayTime = (long)this.DisplayTimeNumericUpDown.Value;
                src.AddMessageEnabled = this.EnabledAddMessageCheckBox.Checked;
                src.ProgressBarEnabled = this.TelopProgressBarEnabledCheckBox.Checked;
                src.FontColor = this.TelopVisualSetting.FontColor.ToHTML();
                src.FontOutlineColor = this.TelopVisualSetting.FontOutlineColor.ToHTML();
                src.FontFamily = this.TelopVisualSetting.TextFont.Name;
                src.FontSize = this.TelopVisualSetting.TextFont.Size;
                src.FontStyle = (int)this.TelopVisualSetting.TextFont.Style;
                src.BackgroundColor = this.TelopVisualSetting.BackgroundColor.ToHTML();
                src.BackgroundAlpha = this.TelopVisualSetting.BackgroundColor.A;
                src.Left = (double)this.TelopLeftNumericUpDown.Value;
                src.Top = (double)this.TelopTopNumericUpDown.Value;
                src.MatchSound = (string)this.TelopMatchSoundComboBox.SelectedValue ?? string.Empty;
                src.MatchTextToSpeak = this.TelopMatchTTSTextBox.Text;
                src.DelaySound = (string)this.TelopDelaySoundComboBox.SelectedValue ?? string.Empty;
                src.DelayTextToSpeak = this.TelopDelayTTSTextBox.Text;

                if ((int)this.TelopLeftNumericUpDown.Tag != src.Left ||
                    (int)this.TelopTopNumericUpDown.Tag != src.Top)
                {
                    OnePointTelopController.SetLocation(
                        src.ID,
                        src.Left,
                        src.Top);
                }

                OnePointTelopTable.Default.Save();
                this.LoadTelopTable();

                // 一度全てのテロップを閉じる
                OnePointTelopController.CloseTelops();

                foreach (TreeNode node in this.TelopTreeView.Nodes)
                {
                    var ds = node.Tag as OnePointTelop;
                    if (ds != null)
                    {
                        if (ds.ID == src.ID)
                        {
                            this.TelopTreeView.SelectedNode = node;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// テロップ削除 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TelopDeleteButton_Click(object sender, EventArgs e)
        {
            lock (OnePointTelopTable.Default.Table)
            {
                var src = this.TelopDetailGroupBox.Tag as OnePointTelop;
                if (src != null)
                {
                    OnePointTelopTable.Default.Table.Remove(src);
                    OnePointTelopTable.Default.Save();

                    OnePointTelopController.CloseTelops();

                    this.TelopDetailGroupBox.Visible = false;
                }
            }

            // 今の選択ノードを取り出す
            var targetNode = this.TelopTreeView.SelectedNode;
            if (targetNode != null)
            {
                // 1個前のノードを取り出しておく
                var prevNode = targetNode.PrevNode;

                targetNode.Remove();

                if (prevNode != null)
                {
                    this.TelopTreeView.SelectedNode = prevNode;
                }
            }
        }

        /// <summary>
        /// テロップテーブルをツリーにロードする
        /// </summary>
        public void LoadTelopTable()
        {
            try
            {
                this.TelopTreeView.SuspendLayout();

                this.TelopTreeView.Nodes.Clear();

                var telops = OnePointTelopTable.Default.Table.OrderBy(x => x.Title);
                foreach (var telop in telops)
                {
                    var n = new TreeNode();

                    n.Tag = telop;
                    n.Text = telop.Title;
                    n.ToolTipText = telop.Message;
                    n.Checked = telop.Enabled;

                    this.TelopTreeView.Nodes.Add(n);
                }

                this.TelopTreeView.ExpandAll();
            }
            finally
            {
                this.TelopTreeView.ResumeLayout();
            }
        }

        /// <summary>
        /// 詳細を表示する
        /// </summary>
        /// <param name="dataSource"></param>
        private void ShowTelopDetail(
            OnePointTelop dataSource)
        {
            var src = dataSource;
            if (src == null)
            {
                this.TelopDetailGroupBox.Visible = false;
                return;
            }

            this.TelopDetailGroupBox.Visible = true;

            this.TelopTitleTextBox.Text = src.Title;
            this.TelopMessageTextBox.Text = src.Message;
            this.TelopKeywordTextBox.Text = src.Keyword;
            this.TelopKeywordToHideTextBox.Text = src.KeywordToHide;
            this.TelopRegexEnabledCheckBox.Checked = src.RegexEnabled;
            this.TelopDelayNumericUpDown.Value = src.Delay;
            this.DisplayTimeNumericUpDown.Value = src.DisplayTime;
            this.EnabledAddMessageCheckBox.Checked = src.AddMessageEnabled;
            this.TelopProgressBarEnabledCheckBox.Checked = src.ProgressBarEnabled;

            this.TelopVisualSetting.FontColor = src.FontColor.FromHTML();
            this.TelopVisualSetting.FontOutlineColor = src.FontOutlineColor.FromHTML();
            this.TelopVisualSetting.FontColor = src.FontColor.FromHTML();
            this.TelopVisualSetting.TextFont = new Font(
                src.FontFamily,
                src.FontSize,
                (FontStyle)src.FontStyle);
            this.TelopVisualSetting.BackgroundColor = string.IsNullOrWhiteSpace(src.BackgroundColor) ?
                Settings.Default.BackgroundColor :
                Color.FromArgb(src.BackgroundAlpha, src.BackgroundColor.FromHTML());

            this.TelopVisualSetting.RefreshSampleImage();

            var left = (int)src.Left;
            var top = (int)src.Top;

            double x, y;
            OnePointTelopController.GettLocation(
                src.ID,
                out x,
                out y);

            if (x != 0)
            {
                left = (int)x;
            }

            if (y != 0)
            {
                top = (int)y;
            }

            this.TelopLeftNumericUpDown.Value = left;
            this.TelopLeftNumericUpDown.Tag = left;
            this.TelopTopNumericUpDown.Value = top;
            this.TelopTopNumericUpDown.Tag = top;

            this.TelopMatchSoundComboBox.SelectedValue = src.MatchSound;
            this.TelopMatchTTSTextBox.Text = src.MatchTextToSpeak;

            this.TelopDelaySoundComboBox.SelectedValue = src.DelaySound;
            this.TelopDelayTTSTextBox.Text = src.DelayTextToSpeak;

            // データソースをタグに突っ込んでおく
            this.TelopDetailGroupBox.Tag = src;
        }
    }
}
