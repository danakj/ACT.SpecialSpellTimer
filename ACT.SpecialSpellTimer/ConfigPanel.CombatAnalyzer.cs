namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// 設定Panel
    /// </summary>
    public partial class ConfigPanel : UserControl
    {
        private int timerCount;

        /// <summary>
        /// ロード
        /// </summary>
        private void LoadCombatAnalyzer()
        {
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

            var changeButtonCaption = new Action(() =>
            {
                var text = string.Empty;

                if (CombatAnalyzer.Default.IsAnalyzing)
                {
                    text += "分析を停止する" + Environment.NewLine;
                    text += "現在の状態 -> 分析中";
                }
                else
                {
                    text += "分析を開始する";
                }

                this.SwitchCombatAnalyzerButton.Text = text;
                Application.DoEvents();
            });

            changeButtonCaption();
            this.SwitchCombatAnalyzerButton.Click += (s, e) =>
            {
                if (!CombatAnalyzer.Default.IsAnalyzing)
                {
                    CombatAnalyzer.Default.Start();
                }
                else
                {
                    this.SwitchCombatAnalyzerButton.Enabled = false;
                    this.CombatAnalyzingLabel.Visible = true;
                    this.CombatAnalyzingTimer.Start();

                    Task.Run(() =>
                    {
                        CombatAnalyzer.Default.Stop();
                    }).ContinueWith((t) =>
                    {
                        if (t != null)
                        {
                            t.Dispose();
                        }

                        this.ShowCombatLog();
                    });

                    Thread.Sleep(500);
                }

                changeButtonCaption();
            };
        }

        /// <summary>
        /// 戦闘ログを表示する
        /// </summary>
        public void ShowCombatLog()
        {
            var action = new Action(() =>
            {
                try
                {
                    this.CombatLogDataGridView.Visible = false;
                    this.CombatLogDataGridView.SuspendLayout();

                    this.CombatLogDataGridView.AutoGenerateColumns = false;
                    this.CombatLogDataGridView.DataSource = CombatAnalyzer.Default.CurrentCombatLogList;
                    this.SwitchCombatAnalyzerButton.Enabled = true;
                    this.CombatAnalyzingLabel.Visible = false;
                    this.CombatAnalyzingTimer.Stop();
                }
                finally
                {
                    this.CombatLogDataGridView.ResumeLayout();
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
