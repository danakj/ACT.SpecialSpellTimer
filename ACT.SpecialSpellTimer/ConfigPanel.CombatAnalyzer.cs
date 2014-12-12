namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Collections.Generic;

    using ACT.SpecialSpellTimer.Properties;

    /// <summary>
    /// 設定Panel
    /// </summary>
    public partial class ConfigPanel : UserControl
    {
        private int timerCount;

        private List<CombatLog> bindedCombatLogList = new List<CombatLog>();

        /// <summary>
        /// ロード
        /// </summary>
        private void LoadCombatAnalyzer()
        {
            this.CombatLogDataGridView.AutoGenerateColumns = false;

            this.CombatLogEnabledCheckBox.Checked = Settings.Default.CombatLogEnabled;
            this.CombatLogBufferSizeNumericUpDown.Value = Settings.Default.CombatLogBufferSize;

            var saveSettings = new Action(() =>
            {
                Settings.Default.CombatLogEnabled = this.CombatLogEnabledCheckBox.Checked;
                Settings.Default.CombatLogBufferSize = (long)this.CombatLogBufferSizeNumericUpDown.Value;
                Settings.Default.Save();
            });

            this.CombatLogEnabledCheckBox.CheckedChanged += (s, e) => saveSettings();
            this.CombatLogBufferSizeNumericUpDown.ValueChanged += (s, e) => saveSettings();

            this.CombatAnalyzingLabel.Text = string.Empty;
            this.CombatAnalyzingLabel.Visible = false;

            this.CombatAnalyzingTimer.Tick += (s, e) =>
            {
                this.timerCount++;

                if (this.timerCount >= 4)
                {
                    this.timerCount = 0;
                }

                switch (this.timerCount)
                {
                    case 0:
                        this.CombatAnalyzingLabel.Text = "分析しています";
                        break;
                    case 1:
                        this.CombatAnalyzingLabel.Text = "分析しています .";
                        break;
                    case 2:
                        this.CombatAnalyzingLabel.Text = "分析しています . .";
                        break;
                    case 3:
                        this.CombatAnalyzingLabel.Text = "分析しています . . .";
                        break;
                }

                Application.DoEvents();
            };

            this.AnalyzeCombatButton.Click += (s, e) =>
            {
                this.AnalyzeCombatButton.Enabled = false;
                this.CombatAnalyzingLabel.Visible = true;
                this.CombatAnalyzingTimer.Start();

                Task.Run(() =>
                {
                    CombatAnalyzer.Default.AnalyzeLog();
                }).ContinueWith((t) =>
                {
                    if (t != null)
                    {
                        t.Dispose();
                    }

                    this.ShowCombatLog();
                });
            };

            this.CombatLogDataGridView.CellValueNeeded += (s, e) =>
            {
                // データソースを取り出す
                var ds = this.CombatLogDataGridView.Rows[e.RowIndex].Tag as CombatLog;
                if (ds == null)
                {
                    return;
                }

                if (e.ColumnIndex == this.TimeStampColumn.Index)
                {
                    e.Value = ds.TimeStamp;
                }

                if (e.ColumnIndex == this.ElapsedColumn.Index)
                {
                    e.Value = ds.TimeStampElapted;
                }

                if (e.ColumnIndex == this.LogTypeColumn.Index)
                {
                    e.Value = ds.LogTypeName;
                }

                if (e.ColumnIndex == this.ActorColumn.Index)
                {
                    e.Value = ds.Actor;
                }

                if (e.ColumnIndex == this.ActionColumn.Index)
                {
                    e.Value = ds.Action;
                }

                if (e.ColumnIndex == this.SpanColumn.Index)
                {
                    e.Value = ds.Span;
                }

                if (e.ColumnIndex == this.LogColumn.Index)
                {
                    e.Value = ds.Raw;
                }
            };
        }

        /// <summary>
        /// 戦闘ログを表示する
        /// </summary>
        public void ShowCombatLog()
        {
            this.bindedCombatLogList.Clear();
            this.bindedCombatLogList.AddRange(CombatAnalyzer.Default.CurrentCombatLogList);

            var action = new Action(() =>
            {
                try
                {
                    this.CombatLogDataGridView.Visible = false;
                    this.CombatLogDataGridView.Enabled = false;
                    this.CombatLogDataGridView.SuspendLayout();
                    this.CombatLogDataGridView.RowCount = this.bindedCombatLogList.Count;

                    for (int i = 0; i < this.CombatLogDataGridView.Rows.Count; i++)
                    {
                        this.CombatLogDataGridView.Rows[i].HeaderCell.Value = (i + 1).ToString("N0");
                        this.CombatLogDataGridView.Rows[i].Tag = this.bindedCombatLogList[i];
                    }

                    this.AnalyzeCombatButton.Enabled = true;
                    this.CombatAnalyzingLabel.Visible = false;
                    this.CombatAnalyzingTimer.Stop();
                }
                finally
                {
                    this.CombatLogDataGridView.ResumeLayout();
                    this.CombatLogDataGridView.Enabled = true;
                    this.CombatLogDataGridView.Visible = true;
                }
            });

            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    action();
                });
            }
            else
            {
                action();
            }
        }
    }
}
