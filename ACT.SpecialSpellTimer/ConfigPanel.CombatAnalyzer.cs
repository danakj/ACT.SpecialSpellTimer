namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;

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
            this.CombatLogEnabledCheckBox.Checked = Settings.Default.CombatLogEnabled;
            this.CombatLogBufferSizeNumericUpDown.Value = Settings.Default.CombatLogBufferSize;

            var saveSettings = new Action(() =>
            {
                Settings.Default.CombatLogEnabled = this.CombatLogEnabledCheckBox.Checked;
                Settings.Default.CombatLogBufferSize = (long)this.CombatLogBufferSizeNumericUpDown.Value;
                Settings.Default.Save();
            });

            this.CombatLogEnabledCheckBox.CheckedChanged += (s, e) =>
            {
                saveSettings();

                if (!Settings.Default.CombatLogEnabled)
                {
                    CombatAnalyzer.Default.ClearLogBuffer();
                }
            };

            this.CombatLogBufferSizeNumericUpDown.ValueChanged += (s, e) => saveSettings();

            this.CombatAnalyzingLabel.Text = string.Empty;
            this.CombatAnalyzingLabel.Visible = false;

            this.CombatAnalyzingTimer.Tick += (s, e) =>
            {
                this.timerCount++;

                if (this.timerCount >= 5)
                {
                    this.timerCount = 0;
                }

                switch (this.timerCount)
                {
                    case 0:
                        this.CombatAnalyzingLabel.Text = "Please Wait";
                        break;
                    case 1:
                        this.CombatAnalyzingLabel.Text = "Please Wait .";
                        break;
                    case 2:
                        this.CombatAnalyzingLabel.Text = "Please Wait . .";
                        break;
                    case 3:
                        this.CombatAnalyzingLabel.Text = "Please Wait . . .";
                        break;
                    case 4:
                        this.CombatAnalyzingLabel.Text = "Please Wait . . . .";
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

            // コンテキストメニューを設定する
            this.CASelectAllItem.Click += (s, e) => this.SelectAll();
            this.CACopyLogItem.Click += (s, e) => this.CopyLog();
            this.CACopyLogDetailItem.Click += (s, e) => this.CopyLogDetail();
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
                    this.CombatLogListView.Visible = false;
                    this.CombatLogListView.SuspendLayout();

                    this.CombatLogListView.Items.Clear();

                    for (int i = 0; i < this.bindedCombatLogList.Count; i++)
                    {
                        var ds = this.bindedCombatLogList[i];

                        var values = new string[]
                    {
                        string.Empty,
                        (i + 1).ToString("N0"),
                        ds.TimeStamp.ToString("yy/MM/dd HH:mm:ss.fff"),
                        ds.TimeStampElapted.ToString("N0"),
                        ds.LogTypeName,
                        ds.Actor,
                        ds.Action,
                        ds.Span.ToString("N0"),
                        ds.Raw
                    };

                        var item = new ListViewItem(values)
                        {
                            Tag = ds
                        };

                        this.CombatLogListView.Items.Add(item);
                    }

                    this.CombatLogListView.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.HeaderSize);
                    this.CombatLogListView.AutoResizeColumn(4, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(5, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(6, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(7, ColumnHeaderAutoResizeStyle.HeaderSize);
                    this.CombatLogListView.AutoResizeColumn(8, ColumnHeaderAutoResizeStyle.ColumnContent);

                    this.CombatAnalyzingTimer.Stop();
                    this.AnalyzeCombatButton.Enabled = true;
                    this.CombatAnalyzingLabel.Visible = false;
                }
                finally
                {
                    this.CombatLogListView.ResumeLayout();
                    this.CombatLogListView.Visible = true;
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

        /// <summary>
        /// 全て選択する
        /// </summary>
        private void SelectAll()
        {
            foreach (ListViewItem item in this.CombatLogListView.Items)
            {
                item.Selected = true;
            }
        }

        /// <summary>
        /// ログをコピーする
        /// </summary>
        private void CopyLog()
        {
            var sb = new StringBuilder();
            foreach (ListViewItem item in this.CombatLogListView.SelectedItems)
            {
                var log = item.Tag as CombatLog;
                if (log != null)
                {
                    sb.AppendLine(log.Raw);
                }
            }

            Clipboard.SetText(sb.ToString());
        }

        /// <summary>
        /// ログの詳細をコピーする
        /// </summary>
        private void CopyLogDetail()
        {
            var sb = new StringBuilder();
            foreach (ListViewItem item in this.CombatLogListView.SelectedItems)
            {
                var list = new List<string>();
                var i = 0;
                foreach (ListViewItem.ListViewSubItem sub in item.SubItems)
                {
                    if (i != 0)
                    {
                        list.Add(sub.Text);
                    }

                    i++;
                }

                sb.AppendLine(string.Join("\t", list.ToArray()));
            }

            Clipboard.SetText(sb.ToString());
        }
    }
}
