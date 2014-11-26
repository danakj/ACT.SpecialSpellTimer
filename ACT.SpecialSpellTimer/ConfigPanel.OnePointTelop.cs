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

            // イベントを設定する
            this.TelopBackColorTranceparentCheckBox.CheckedChanged += (s1, e1) =>
            {
                this.TelopBackColorButton.Enabled =
                    !this.TelopBackColorTranceparentCheckBox.Checked;

                if (this.TelopBackColorTranceparentCheckBox.Checked)
                {
                    this.TelopSampleLabel.BackColor = Color.Transparent;
                }
            };

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
                var source = e1.Node.Tag as SpellTimerDataSet.OnePointTelopRow;
                if (source != null)
                {
                    source.Enabled = e1.Node.Checked;
                }
            };

            this.TelopTreeView.AfterSelect += (s1, e1) =>
            {
                this.ShowTelopDetail(
                    e1.Node.Tag as SpellTimerDataSet.OnePointTelopRow);
            };

            this.TelopBackColorButton.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.TelopSampleLabel.BackColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.TelopSampleLabel.BackColor = this.ColorDialog.Color;
                }
            };

            this.TelopFontButton.Click += (s1, e1) =>
            {
                var maxSizeBack = this.FontDialog.MaxSize;

                this.FontDialog.MaxSize = 0;
                this.FontDialog.Font = this.TelopSampleLabel.Font;
                if (this.FontDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.TelopSampleLabel.Font = this.FontDialog.Font;
                }

                this.FontDialog.MaxSize = maxSizeBack;
            };

            this.TelopFontColorButton.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.TelopSampleLabel.ForeColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.TelopSampleLabel.ForeColor = this.ColorDialog.Color;
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
        }

        /// <summary>
        /// テロップインポート Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TelopImportButton_Click(object sender, EventArgs e)
        {
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
                "全てのテロップを削除してよろしいですか？",
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
            var nr = OnePointTelopTable.Default.Table.NewOnePointTelopRow();

            nr.ID = OnePointTelopTable.Default.Table.Any() ?
                OnePointTelopTable.Default.Table.Max(x => x.ID) + 1 :
                1;
            nr.Title = "New Telop";
            nr.DisplayTime = 3;
            nr.BackColor = Color.Transparent.ToHTML();
            nr.FontColor = Settings.Default.FontColor.ToHTML();
            nr.FontFamily = Settings.Default.Font.Name;
            nr.FontSize = Settings.Default.Font.Size;
            nr.FontStyle = (int)Settings.Default.Font.Style;
            nr.Left = 10.0d;
            nr.Top = 10.0d;

            // 現在選択しているノードの情報を一部コピーする
            if (this.TelopTreeView.SelectedNode != null)
            {
                var baseRow = this.TelopTreeView.SelectedNode.Tag != null ?
                    this.TelopTreeView.SelectedNode.Tag as SpellTimerDataSet.OnePointTelopRow :
                    this.TelopTreeView.SelectedNode.Nodes[0].Tag as SpellTimerDataSet.OnePointTelopRow;

                if (baseRow != null)
                {
                    nr.Title = baseRow.Title + " New";
                    nr.Message = baseRow.Message;
                    nr.Keyword = baseRow.Keyword;
                    nr.KeywordToHide = baseRow.KeywordToHide;
                    nr.RegexEnabled = baseRow.RegexEnabled;
                    nr.Delay = baseRow.Delay;
                    nr.DisplayTime = baseRow.DisplayTime;
                    nr.BackColor = baseRow.BackColor;
                    nr.FontColor = baseRow.FontColor;
                    nr.FontFamily = baseRow.FontFamily;
                    nr.FontSize = baseRow.FontSize;
                    nr.FontStyle = baseRow.FontStyle;
                    nr.Left = baseRow.Left;
                    nr.Top = baseRow.Top;
                }
            }

            nr.MatchDateTime = DateTime.MinValue;
            nr.Enabled = true;
            nr.Regex = null;
            nr.RegexPattern = string.Empty;
            nr.RegexToHide = null;
            nr.RegexPatternToHide = string.Empty;

            OnePointTelopTable.Default.Table.AddOnePointTelopRow(nr);

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
                    "テロップの名前を入力してください",
                    "ACT.SpecialSpellTimer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }

            var src = this.TelopDetailGroupBox.Tag as SpellTimerDataSet.OnePointTelopRow;
            if (src != null)
            {
                src.BeginEdit();

                src.Title = this.TelopTitleTextBox.Text;
                src.Message = this.TelopMessageTextBox.Text;
                src.Keyword = this.TelopKeywordTextBox.Text;
                src.KeywordToHide = this.TelopKeywordToHideTextBox.Text;
                src.Delay = (long)this.TelopDelayNumericUpDown.Value;
                src.DisplayTime = (long)this.DisplayTimeNumericUpDown.Value;
                src.RegexEnabled = this.TelopRegexEnabledCheckBox.Checked;
                src.BackColor = this.TelopSampleLabel.BackColor.ToHTML();
                src.FontColor = this.TelopSampleLabel.ForeColor.ToHTML();
                src.FontFamily = this.TelopSampleLabel.Font.Name;
                src.FontSize = this.TelopSampleLabel.Font.Size;
                src.FontStyle = (int)this.TelopSampleLabel.Font.Style;
                src.Left = (double)this.TelopLeftNumericUpDown.Value;
                src.Top = (double)this.TelopTopNumericUpDown.Value;
                src.MatchSound = (string)this.TelopMatchSoundComboBox.SelectedValue;
                src.MatchTextToSpeak = this.TelopMatchTTSTextBox.Text;
                src.DelaySound = (string)this.TelopDelaySoundComboBox.SelectedValue;
                src.DelayTextToSpeak = this.TelopDelayTTSTextBox.Text;

                src.EndEdit();
                OnePointTelopTable.Default.Save();
                this.LoadTelopTable();

                foreach (TreeNode node in this.TelopTreeView.Nodes)
                {
                    var ds = node.Tag as SpellTimerDataSet.OnePointTelopRow;
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
                var src = this.TelopDetailGroupBox.Tag as SpellTimerDataSet.OnePointTelopRow;
                if (src != null)
                {
                    OnePointTelopTable.Default.Table.RemoveOnePointTelopRow(src);
                    OnePointTelopTable.Default.Save();

                    OnePointTelopController.CloseTelops();

                    this.TelopDetailGroupBox.Visible = false;
                }
            }

            this.LoadTelopTable();
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
            SpellTimerDataSet.OnePointTelopRow dataSource)
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
            this.TelopDelayNumericUpDown.Value = src.Delay;
            this.DisplayTimeNumericUpDown.Value = src.DisplayTime;
            this.TelopRegexEnabledCheckBox.Checked = src.RegexEnabled;

            this.TelopSampleLabel.BackColor = src.BackColor.FromHTML();
            this.TelopSampleLabel.ForeColor = src.FontColor.FromHTML();
            this.TelopSampleLabel.Font = new Font(
                src.FontFamily,
                src.FontSize,
                (FontStyle)src.FontStyle);
            this.TelopBackColorTranceparentCheckBox.Checked =
                this.TelopSampleLabel.BackColor == Color.Transparent;
            this.TelopBackColorButton.Enabled =
                !this.TelopBackColorTranceparentCheckBox.Checked;

            this.TelopLeftNumericUpDown.Value = (int)src.Left;
            this.TelopTopNumericUpDown.Value = (int)src.Top;

            this.TelopMatchSoundComboBox.SelectedValue = src.MatchSound;
            this.TelopMatchTTSTextBox.Text = src.MatchTextToSpeak;

            this.TelopDelaySoundComboBox.SelectedValue = src.DelaySound;
            this.TelopDelayTTSTextBox.Text = src.DelayTextToSpeak;

            // データソースをタグに突っ込んでおく
            this.TelopDetailGroupBox.Tag = src;
        }
    }
}
