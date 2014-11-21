namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Properties;

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

            this.SpellTimerTreeView.AfterSelect += this.SpellTimerTreeView_AfterSelect;
            this.AddButton.Click += this.AddButton_Click;
            this.UpdateButton.Click += this.UpdateButton_Click;
            this.DeleteButton.Click += this.DeleteButton_Click;
        }

        /// <summary>
        /// 初期化 Button
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ShokikaButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Reset();
            Settings.Default.Save();

            PanelSettings.Default.SettingsTable.Clear();
            PanelSettings.Default.Save();

            SpellTimerCore.Default.LayoutPanels();
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
                var nr = SpellTimerTable.Table.NewSpellTimerRow();
                nr.Panel = "General";
                nr.SpellTitle = "New Spell";
                nr.MatchDateTime = DateTime.MinValue;
                SpellTimerTable.Table.AddSpellTimerRow(nr);

                SpellTimerTable.Save();

                this.ShowDetail(nr);
                this.LoadSpellTimerTable();
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
                var src = this.DetailGroupBox.Tag as SpellTimerDataSet.SpellTimerRow;
                if (src != null)
                {
                    src.Panel = this.PanelNameTextBox.Text;
                    src.SpellTitle = this.SpellTitleTextBox.Text;
                    src.Keyword = this.KeywordTextBox.Text;
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
                var src = this.DetailGroupBox.Tag as SpellTimerDataSet.SpellTimerRow;
                if (src != null)
                {
                    SpellTimerTable.Table.RemoveSpellTimerRow(src);
                    SpellTimerTable.Save();
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
                e.Node.Tag as SpellTimerDataSet.SpellTimerRow);
        }

        /// <summary>
        /// 詳細を表示する
        /// </summary>
        /// <param name="dataSource"></param>
        private void ShowDetail(
            SpellTimerDataSet.SpellTimerRow dataSource)
        {
            var src = dataSource;
            if (src == null)
            {
                this.DetailGroupBox.Enabled = false;
                return;
            }

            this.DetailGroupBox.Enabled = true;

            this.PanelNameTextBox.Text = src.Panel;
            this.SpellTitleTextBox.Text = src.SpellTitle;
            this.KeywordTextBox.Text = src.Keyword;
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

            // データソースをタグに突っ込んでおく
            this.DetailGroupBox.Tag = src;
        }

        /// <summary>
        /// スペルタイマテーブルを読み込む
        /// </summary>
        private void LoadSpellTimerTable()
        {
            try
            {
                this.SpellTimerTreeView.SuspendLayout();

                this.SpellTimerTreeView.Nodes.Clear();

                var panels = SpellTimerTable.Table.Select(x => x.Panel).Distinct();
                foreach (var panelName in panels)
                {
                    var children = new List<TreeNode>();
                    var spells = SpellTimerTable.Table.Where(x => x.Panel == panelName);
                    foreach (var spell in spells)
                    {
                        var nc = new TreeNode()
                        {
                            Text = spell.SpellTitle,
                            ToolTipText = spell.Keyword,
                            Tag = spell
                        };

                        children.Add(nc);
                    }

                    var n = new TreeNode(
                        panelName,
                        children.ToArray());

                    this.SpellTimerTreeView.Nodes.Add(n);
                }

                this.SpellTimerTreeView.ExpandAll();
            }
            finally
            {
                this.SpellTimerTreeView.ResumeLayout();
            }
        }
    }
}
