namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Sound;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// 設定Panel
    /// </summary>
    public partial class ConfigPanel : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfigPanel()
        {
            this.InitializeComponent();

            this.Load += this.ConfigPanel_Load;
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ConfigPanel_Load(object sender, EventArgs e)
        {
            this.LoadSpellTimerTable();

            this.DetailGroupBox.Visible = false;

            // コンボボックスにアイテムを装填する
            this.MatchSoundComboBox.DataSource = SoundController.Default.EnumlateWave();
            this.MatchSoundComboBox.ValueMember = "FullPath";
            this.MatchSoundComboBox.DisplayMember = "Name";

            this.OverSoundComboBox.DataSource = SoundController.Default.EnumlateWave();
            this.OverSoundComboBox.ValueMember = "FullPath";
            this.OverSoundComboBox.DisplayMember = "Name";

            this.TimeupSoundComboBox.DataSource = SoundController.Default.EnumlateWave();
            this.TimeupSoundComboBox.ValueMember = "FullPath";
            this.TimeupSoundComboBox.DisplayMember = "Name";

            // イベントを設定する
            this.SpellTimerTreeView.AfterSelect += this.SpellTimerTreeView_AfterSelect;
            this.AddButton.Click += this.AddButton_Click;
            this.UpdateButton.Click += this.UpdateButton_Click;
            this.DeleteButton.Click += this.DeleteButton_Click;

            this.Play1Button.Click += (s1, e1) =>
            {
                SoundController.Default.Play((string)this.MatchSoundComboBox.SelectedValue ?? string.Empty);
            };

            this.Play2Button.Click += (s1, e1) =>
            {
                SoundController.Default.Play((string)this.OverSoundComboBox.SelectedValue ?? string.Empty);
            };

            this.Play3Button.Click += (s1, e1) =>
            {
                SoundController.Default.Play((string)this.TimeupSoundComboBox.SelectedValue ?? string.Empty);
            };

            this.Speak1Button.Click += (s1, e1) =>
            {
                SoundController.Default.Play(this.MatchTextToSpeakTextBox.Text);
            };

            this.Speak2Button.Click += (s1, e1) =>
            {
                SoundController.Default.Play(this.OverTextToSpeakTextBox.Text);
            };

            this.Speak3Button.Click += (s1, e1) =>
            {
                SoundController.Default.Play(this.TimeupTextToSpeakTextBox.Text);
            };

            this.SpellTimerTreeView.AfterCheck += (s1, e1) =>
            {
                var source = e1.Node.Tag as SpellTimer;
                if (source != null)
                {
                    source.Enabled = e1.Node.Checked;
                }
                else
                {
                    foreach (TreeNode node in e1.Node.Nodes)
                    {
                        var sourceChild = node.Tag as SpellTimer;
                        if (sourceChild != null)
                        {
                            sourceChild.Enabled = e1.Node.Checked;
                        }

                        node.Checked = e1.Node.Checked;
                    }
                }
            };

            this.OneBarColorButton.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.SampleLabel.BackColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.SampleLabel.BackColor = this.ColorDialog.Color;
                }
            };

            this.OneFontColorButton.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.SampleLabel.ForeColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.SampleLabel.ForeColor = this.ColorDialog.Color;
                }
            };

            this.SelectJobButton.Click += (s1, e1) =>
            {
                var src = this.DetailGroupBox.Tag as SpellTimer;
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

            // オプションのロードメソッドを呼ぶ
            this.LoadOption();

            // ワンポイントテロップのロードメソッドを呼ぶ
            this.LoadOnePointTelop();
        }

        /// <summary>
        /// 追加 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void AddButton_Click(object sender, EventArgs e)
        {
            lock (SpellTimerTable.Table)
            {
                var nr = new SpellTimer();

                nr.Panel = "General";
                nr.SpellTitle = "New Spell";
                nr.ProgressBarVisible = true;
                nr.BarColor = Settings.Default.ProgressBarColor.ToHTML();
                nr.FontColor = Settings.Default.FontColor.ToHTML();

                // 現在選択しているノードの情報を一部コピーする
                if (this.SpellTimerTreeView.SelectedNode != null)
                {
                    var baseRow = this.SpellTimerTreeView.SelectedNode.Tag != null ?
                        this.SpellTimerTreeView.SelectedNode.Tag as SpellTimer :
                        this.SpellTimerTreeView.SelectedNode.Nodes[0].Tag as SpellTimer;

                    if (baseRow != null)
                    {
                        nr.Panel = baseRow.Panel;
                        nr.SpellTitle = baseRow.SpellTitle + " New";
                        nr.Keyword = baseRow.Keyword;
                        nr.RegexEnabled = baseRow.RegexEnabled;
                        nr.RecastTime = baseRow.RecastTime;
                        nr.RepeatEnabled = baseRow.RepeatEnabled;
                        nr.ProgressBarVisible = baseRow.ProgressBarVisible;
                        nr.IsReverse = baseRow.IsReverse;
                        nr.BarColor = baseRow.BarColor;
                        nr.FontColor = baseRow.FontColor;
                        nr.DontHide = baseRow.DontHide;
                    }
                }

                nr.MatchDateTime = DateTime.MinValue;
                nr.Enabled = true;
                nr.DisplayNo = SpellTimerTable.Table.Any() ?
                    SpellTimerTable.Table.Max(x => x.DisplayNo) + 1 :
                    50;
                nr.Regex = null;
                nr.RegexPattern = string.Empty;
                SpellTimerTable.Table.Add(nr);

                SpellTimerTable.Save();

                // 新しいノードを生成する
                var node = new TreeNode(nr.SpellTitle)
                {
                    Tag = nr,
                    ToolTipText = nr.Keyword,
                    Checked = nr.Enabled
                };

                // 親ノードがあれば追加する
                foreach (TreeNode item in this.SpellTimerTreeView.Nodes)
                {
                    if (item.Text == nr.Panel)
                    {
                        item.Nodes.Add(node);
                        this.SpellTimerTreeView.SelectedNode = node;
                        break;
                    }
                }

                // 親ノードがなかった
                if (this.SpellTimerTreeView.SelectedNode != node)
                {
                    var parentNode = new TreeNode(nr.Panel, new TreeNode[] { node })
                    {
                        Checked = true
                    };

                    this.SpellTimerTreeView.Nodes.Add(parentNode);
                    this.SpellTimerTreeView.SelectedNode = node;
                }
            }
        }

        /// <summary>
        /// 更新 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            lock (SpellTimerTable.Table)
            {
                if (string.IsNullOrWhiteSpace(this.PanelNameTextBox.Text))
                {
                    MessageBox.Show(
                        this,
                        "パネル名を入力してください",
                        "ACT.SpecialSpellTimer",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                if (string.IsNullOrWhiteSpace(this.SpellTitleTextBox.Text))
                {
                    MessageBox.Show(
                        this,
                        "スペル名を入力してください",
                        "ACT.SpecialSpellTimer",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation);
                    return;
                }

                var src = this.DetailGroupBox.Tag as SpellTimer;
                if (src != null)
                {
                    src.Panel = this.PanelNameTextBox.Text;
                    src.SpellTitle = this.SpellTitleTextBox.Text;
                    src.DisplayNo = (int)this.DisplayNoNumericUpDown.Value;
                    src.Keyword = this.KeywordTextBox.Text;
                    src.RegexEnabled = this.RegexEnabledCheckBox.Checked;
                    src.RecastTime = (int)this.RecastTimeNumericUpDown.Value;
                    src.RepeatEnabled = this.RepeatCheckBox.Checked;
                    src.ProgressBarVisible = this.ShowProgressBarCheckBox.Checked;

                    src.MatchSound = (string)this.MatchSoundComboBox.SelectedValue ?? string.Empty;
                    src.MatchTextToSpeak = this.MatchTextToSpeakTextBox.Text;

                    src.OverSound = (string)this.OverSoundComboBox.SelectedValue ?? string.Empty;
                    src.OverTextToSpeak = this.OverTextToSpeakTextBox.Text;
                    src.OverTime = (int)this.OverTimeNumericUpDown.Value;

                    src.TimeupSound = (string)this.TimeupSoundComboBox.SelectedValue ?? string.Empty;
                    src.TimeupTextToSpeak = this.TimeupTextToSpeakTextBox.Text;

                    src.IsReverse = this.IsReverseCheckBox.Checked;
                    src.BarColor = this.SampleLabel.BackColor.ToHTML();
                    src.FontColor = this.SampleLabel.ForeColor.ToHTML();
                    src.DontHide = this.DontHideCheckBox.Checked;

                    SpellTimerTable.Save();
                }
            }

            this.LoadSpellTimerTable();
        }

        /// <summary>
        /// 削除 Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            lock (SpellTimerTable.Table)
            {
                var src = this.DetailGroupBox.Tag as SpellTimer;
                if (src != null)
                {
                    SpellTimerTable.Table.Remove(src);
                    SpellTimerTable.Save();

                    this.DetailGroupBox.Visible = false;
                }
            }

            this.LoadSpellTimerTable();
        }

        /// <summary>
        /// スペルタイマツリー AfterSelect
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void SpellTimerTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.ShowDetail(
                e.Node.Tag as SpellTimer);
        }

        /// <summary>
        /// スペルタイマテーブルを読み込む
        /// </summary>
        public void LoadSpellTimerTable()
        {
            try
            {
                this.SpellTimerTreeView.SuspendLayout();

                this.SpellTimerTreeView.Nodes.Clear();

                var panels = SpellTimerTable.Table
                    .OrderBy(x => x.Panel)
                    .Select(x => x.Panel)
                    .Distinct();
                foreach (var panelName in panels)
                {
                    var children = new List<TreeNode>();
                    var spells = SpellTimerTable.Table
                        .OrderBy(x => x.DisplayNo)
                        .Where(x => x.Panel == panelName);
                    foreach (var spell in spells)
                    {
                        var nc = new TreeNode()
                        {
                            Text = spell.SpellTitle,
                            ToolTipText = spell.Keyword,
                            Checked = spell.Enabled,
                            Tag = spell,
                        };

                        children.Add(nc);
                    }

                    var n = new TreeNode(
                        panelName,
                        children.ToArray());

                    n.Checked = children.Any(x => x.Checked);

                    this.SpellTimerTreeView.Nodes.Add(n);
                }

                this.SpellTimerTreeView.ExpandAll();
            }
            finally
            {
                this.SpellTimerTreeView.ResumeLayout();
            }
        }

        /// <summary>
        /// 詳細を表示する
        /// </summary>
        /// <param name="dataSource">データソース</param>
        private void ShowDetail(
            SpellTimer dataSource)
        {
            var src = dataSource;
            if (src == null)
            {
                this.DetailGroupBox.Visible = false;
                return;
            }

            this.DetailGroupBox.Visible = true;

            this.PanelNameTextBox.Text = src.Panel;
            this.SpellTitleTextBox.Text = src.SpellTitle;
            this.DisplayNoNumericUpDown.Value = src.DisplayNo;
            this.KeywordTextBox.Text = src.Keyword;
            this.RegexEnabledCheckBox.Checked = src.RegexEnabled;
            this.RecastTimeNumericUpDown.Value = src.RecastTime;
            this.RepeatCheckBox.Checked = src.RepeatEnabled;
            this.ShowProgressBarCheckBox.Checked = src.ProgressBarVisible;

            this.MatchSoundComboBox.SelectedValue = src.MatchSound;
            this.MatchTextToSpeakTextBox.Text = src.MatchTextToSpeak;

            this.OverSoundComboBox.SelectedValue = src.OverSound;
            this.OverTextToSpeakTextBox.Text = src.OverTextToSpeak;
            this.OverTimeNumericUpDown.Value = src.OverTime;

            this.TimeupSoundComboBox.SelectedValue = src.TimeupSound;
            this.TimeupTextToSpeakTextBox.Text = src.TimeupTextToSpeak;

            this.IsReverseCheckBox.Checked = src.IsReverse;
            this.SampleLabel.BackColor = string.IsNullOrWhiteSpace(src.BarColor) ?
                Settings.Default.ProgressBarColor :
                src.BarColor.FromHTML();
            this.SampleLabel.ForeColor = string.IsNullOrWhiteSpace(src.FontColor) ?
                Settings.Default.FontColor :
                src.FontColor.FromHTML();
            this.SampleLabel.Font = Settings.Default.Font;
            this.DontHideCheckBox.Checked = src.DontHide;

            // データソースをタグに突っ込んでおく
            this.DetailGroupBox.Tag = src;
        }

        /// <summary>
        /// エクスポート Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ExportButton_Click(object sender, EventArgs e)
        {
            this.SaveFileDialog.FileName = "ACT.SpecialSpellTimer.Spells.xml";
            if (this.SaveFileDialog.ShowDialog(this) != DialogResult.Cancel)
            {
                SpellTimerTable.Save(
                    this.SaveFileDialog.FileName);
            }
        }

        /// <summary>
        /// インポート Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ImportButton_Click(object sender, EventArgs e)
        {
            this.OpenFileDialog.FileName = "ACT.SpecialSpellTimer.Spells.xml";
            if (this.OpenFileDialog.ShowDialog(this) != DialogResult.Cancel)
            {
                SpellTimerTable.Load(
                    this.OpenFileDialog.FileName,
                    false);

                this.LoadSpellTimerTable();
            }
        }

        /// <summary>
        /// 全て削除
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ClearAllButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                this,
                "全てのスペルを削除してよろしいですか？",
                "ACT.SpecialSpellTimer",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                lock (SpellTimerTable.Table)
                {
                    this.DetailGroupBox.Visible = false;
                    SpellTimerTable.Table.Clear();
                }

                this.LoadSpellTimerTable();
            }
        }
    }
}
