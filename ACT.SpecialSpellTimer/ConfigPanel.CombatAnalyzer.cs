namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading;
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
        /// 戦闘アナライザの有効性
        /// </summary>
        public bool CombatAnalyzerEnabled
        {
            get
            {
                return this.CombatLogEnabledCheckBox.Checked;
            }
            set
            {
                this.CombatLogEnabledCheckBox.Checked = value;
            }
        }

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
                        this.CombatAnalyzingLabel.Text = Utility.Translate.Get("PleaseWait1");
                        break;
                    case 1:
                        this.CombatAnalyzingLabel.Text = Utility.Translate.Get("PleaseWait2");
                        break;
                    case 2:
                        this.CombatAnalyzingLabel.Text = Utility.Translate.Get("PleaseWait3");
                        break;
                    case 3:
                        this.CombatAnalyzingLabel.Text = Utility.Translate.Get("PleaseWait4");
                        break;
                    case 4:
                        this.CombatAnalyzingLabel.Text = Utility.Translate.Get("PleaseWait5");
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

                    this.ShowCombatLog(CombatAnalyzer.Default.CurrentCombatLogList);
                });
            };

            // コンテキストメニューを設定する
            this.CASelectAllItem.Click += (s, e) => this.SelectAll();
            this.CACopyLogItem.Click += (s, e) => this.CopyLog();
            this.CACopyLogDetailItem.Click += (s, e) => this.CopyLogDetail();

            // 経過秒の起点を変える
            this.CASetOriginItem.Click += (s, e) =>
            {
                if (this.CombatLogListView.SelectedItems.Count < 1)
                {
                    return;
                }

                var ds = this.CombatLogListView.SelectedItems[0].Tag as CombatLog;
                if (ds == null)
                {
                    return;
                }

                // 起点のタイムスタンプを取り出す
                var originTimeStamp = ds.TimeStamp;

                this.AnalyzeCombatButton.Enabled = false;
                this.CombatAnalyzingLabel.Visible = true;
                this.CombatAnalyzingTimer.Start();

                Task.Run(() =>
                {
                    // 経過秒を計算し直す
                    foreach (var log in this.bindedCombatLogList.AsParallel())
                    {
                        if ((log.TimeStamp.Ticks % 10) == 0)
                        {
                            Thread.Sleep(1);
                        }

                        log.IsOrigin = false;
                        log.TimeStampElapted = (log.TimeStamp - originTimeStamp).TotalSeconds;
                    }

                    // 今回の起点だけマークする
                    ds.IsOrigin = true;
                }).ContinueWith((t) =>
                {
                    if (t != null)
                    {
                        t.Dispose();
                    }

                    this.ShowCombatLog();
                });
            };
        }

        /// <summary>
        /// 戦闘ログを表示する
        /// </summary>
        public void ShowCombatLog()
        {
            this.ShowCombatLog(null);
        }

        /// <summary>
        /// 戦闘ログを表示する
        /// </summary>
        /// <param name="combatLogList">表示するコンバットログのリスト</param>
        public void ShowCombatLog(
            List<CombatLog> combatLogList)
        {
            if (combatLogList != null)
            {
                this.bindedCombatLogList.Clear();
                this.bindedCombatLogList.AddRange(combatLogList);
            }

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
                            ds.HPRate.ToString("N0") + "%",
                            ds.Action,
                            ds.Span.ToString("N0"),
                            ds.Raw
                        };

                        var item = new ListViewItem(values)
                        {
                            Tag = ds
                        };

                        item.ForeColor = SystemColors.WindowText;
                        switch (ds.LogType)
                        {
                            case CombatLogType.CastStart:
                                item.BackColor = Color.LightPink;
                                break;

                            case CombatLogType.Added:
                                item.BackColor = Color.Gold;
                                break;

                            case CombatLogType.HPRate:
                                item.BackColor = Color.LightGray;
                                break;
                        }

                        if (ds.IsOrigin)
                        {
                            item.BackColor = Color.MediumBlue;
                            item.ForeColor = Color.AliceBlue;
                        }

                        this.CombatLogListView.Items.Add(item);
                    }

                    this.CombatLogListView.AutoResizeColumn(this.NoColumnHeader.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(this.TimeStampColumnHeader.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(this.ElapsedColumnHeader.Index, ColumnHeaderAutoResizeStyle.HeaderSize);
                    this.CombatLogListView.AutoResizeColumn(this.LogTypeColumnHeader.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(this.ActorColumnHeader.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(this.HPRateColumnHeader.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(this.ActionColumnHeader.Index, ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.CombatLogListView.AutoResizeColumn(this.SpanColumnHeader.Index, ColumnHeaderAutoResizeStyle.HeaderSize);
                    this.CombatLogListView.AutoResizeColumn(this.LogColumnHeader.Index, ColumnHeaderAutoResizeStyle.ColumnContent);

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
