namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;

    using ACT.SpecialSpellTimer.Properties;
    using Advanced_Combat_Tracker;

    /// <summary>
    /// SpellTimerの中核
    /// </summary>
    public class SpellTimerCore
    {
        /// <summary>
        /// シングルトンinstance
        /// </summary>
        private static SpellTimerCore instance;

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        public static SpellTimerCore Default
        {
            get
            {
                if (instance == null)
                {
                    instance = new SpellTimerCore();
                }

                return instance;
            }
        }

        /// <summary>
        /// Refreshタイマ
        /// </summary>
        private Timer RefreshTimer;

        /// <summary>
        /// 次の更新タイミング
        /// </summary>
        private long NextRefresh;

        /// <summary>
        /// ログバッファ
        /// </summary>
        private List<string> LogBuffer = new List<string>();

        /// <summary>
        /// SpellTimerのPanelリスト
        /// </summary>
        private List<SpellTimerListWindow> SpellTimerPanels
        {
            get;
            set;
        }

        /// <summary>
        /// 開始する
        /// </summary>
        public void Begin()
        {
            // Panelリストを生成する
            this.SpellTimerPanels = new List<SpellTimerListWindow>();

            // ACTのログ読取りにイベントを仕込む
            ActGlobals.oFormActMain.OnLogLineRead += this.oFormActMain_OnLogLineRead;

            // Refreshタイマを開始する
            this.RefreshTimer = new Timer(new TimerCallback((state) =>
            {
                try
                {
                    this.RefreshWindow();
                }
                catch (Exception ex)
                {
                    ActGlobals.oFormActMain.WriteExceptionLog(
                        ex,
                        "ACT.SpecialSpellTimer スペルタイマWindowのRefreshで例外が発生しました。");
                }
                finally
                {
                    this.RefreshTimer.Change(
                        this.NextRefresh,
                        Timeout.Infinite);
                }
            }),
            null,
            Timeout.Infinite,
            Timeout.Infinite);

            this.NextRefresh = Settings.Default.RefreshInterval;

            this.RefreshTimer.Change(
                this.NextRefresh,
                Timeout.Infinite);
        }

        /// <summary>
        /// 終了する
        /// </summary>
        public void End()
        {
            // ACTからイベントを外す
            ActGlobals.oFormActMain.OnLogLineRead -= this.oFormActMain_OnLogLineRead;

            // 監視を止める
            if (this.RefreshTimer != null)
            {
                this.RefreshTimer.Dispose();
                this.RefreshTimer = null;
            }

            // 全てのPanelを閉じる
            this.ClosePanels();

            // 設定を保存する
            Settings.Default.Save();
            SpellTimerTable.Save();

            // instanceを消去する
            instance = null;
        }

        /// <summary>
        /// ACT OnLogLineRead
        /// </summary>
        /// <param name="isImport">インポートログか？</param>
        /// <param name="logInfo">ログ情報</param>
        private void oFormActMain_OnLogLineRead(
            bool isImport,
            LogLineEventArgs logInfo)
        {
            if (isImport)
            {
                return;
            }

#if false
            Debug.WriteLine(
                logInfo.detectedTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " +
                logInfo.logLine);
#endif

            // LogLineReadはかなりの頻度で発生するためここではログを溜めておくのみとする
            this.LogBuffer.Add(logInfo.logLine.Trim());
        }

        /// <summary>
        /// Windowを更新する
        /// </summary>
        private void RefreshWindow()
        {
            // 有効なSpellリストを取得する
            var spellArray = SpellTimerTable.EnabledTable;

            // 不要なWindowを閉じる
            if (this.SpellTimerPanels != null)
            {
                var removeList = new List<SpellTimerListWindow>();
                foreach (var panel in this.SpellTimerPanels)
                {
                    if (!spellArray.Any(x => x.Panel == panel.PanelName))
                    {
                        ActGlobals.oFormActMain.Invoke((System.Windows.Forms.MethodInvoker)delegate
                        {
                            panel.Close();
                        });

                        removeList.Add(panel);
                    }
                }

                foreach (var item in removeList)
                {
                    this.SpellTimerPanels.Remove(item);
                }
            }

            // ACTが起動していない？
            if (ActGlobals.oFormActMain == null ||
                !ActGlobals.oFormActMain.Visible)
            {
                this.HidePanels();
                this.NextRefresh = 1000;
                return;
            }

#if !DEBUG
                // FF14が起動していない？
                if (FF14PluginHelper.GetFFXIVProcess == null)
                {
                    this.HidePanels();
                    this.NextRefresh = 1000;
                    return;
                }
#endif

            // タイマの間隔を標準に戻す
            this.NextRefresh = Settings.Default.RefreshInterval;

            // ログを取り出す
            var logLines = new string[0];
            logLines = this.LogBuffer.ToArray();
            this.LogBuffer.Clear();

            // Spellを舐める
            foreach (var spell in spellArray)
            {
                var keyword = this.MakeKeyword(spell.Keyword);

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    continue;
                }

                // キーワードに対する正規表現を生成する
                var regex = new Regex(
                    keyword,
                    RegexOptions.IgnoreCase | RegexOptions.Singleline);

                // マッチする？
                foreach (var logLine in logLines.AsParallel())
                {
                    if (regex.IsMatch(logLine))
                    {
                        // ヒットしたログを格納する
                        spell.MatchedLog = logLine;

                        // 置換したスペル名を格納する
                        spell.SpellTitleReplaced = regex.Replace(
                            logLine.Trim(),
                            spell.SpellTitle);

                        spell.MatchDateTime = DateTime.Now;
                        spell.OverDone = false;
                        spell.TimeupDone = false;

                        // マッチ時点のサウンドを再生する
                        this.Play(spell.MatchSound);

                        if (!string.IsNullOrWhiteSpace(spell.MatchTextToSpeak))
                        {
                            var tts = Regex.Replace(
                                logLine.Trim(),
                                keyword,
                                spell.MatchTextToSpeak);
                            this.Play(tts);
                        }
                    }
                }

                // Repeat対象のSpellを更新する
                if (spell.RepeatEnabled &&
                    spell.MatchDateTime > DateTime.MinValue)
                {
                    if (DateTime.Now >= spell.MatchDateTime.AddSeconds(spell.RecastTime))
                    {
                        spell.MatchDateTime = DateTime.Now;
                        spell.OverDone = false;
                        spell.TimeupDone = false;
                    }
                }

                // ｎ秒後のSoundを再生する
                if (spell.OverTime > 0 &&
                    !spell.OverDone &&
                    spell.MatchDateTime > DateTime.MinValue)
                {
                    var over = spell.MatchDateTime.AddSeconds(spell.OverTime);

                    if (DateTime.Now >= over)
                    {
                        this.Play(spell.OverSound);
                        if (!string.IsNullOrWhiteSpace(spell.OverTextToSpeak))
                        {
                            var tts = regex.Replace(
                                spell.MatchedLog,
                                spell.OverTextToSpeak);
                            this.Play(tts);
                        }

                        spell.OverDone = true;
                    }
                }

                // リキャスト完了のSoundを再生する
                if (spell.RecastTime > 0 &&
                    !spell.TimeupDone &&
                    spell.MatchDateTime > DateTime.MinValue)
                {
                    var recast = spell.MatchDateTime.AddSeconds(spell.RecastTime);
                    if (DateTime.Now >= recast)
                    {
                        this.Play(spell.TimeupSound);
                        if (!string.IsNullOrWhiteSpace(spell.TimeupTextToSpeak))
                        {
                            var tts = regex.Replace(
                                spell.MatchedLog,
                                spell.TimeupTextToSpeak);
                            this.Play(tts);
                        }

                        spell.TimeupDone = true;
                    }
                }
            }

            // オーバーレイが非表示？
            if (!Settings.Default.OverlayVisible)
            {
                this.HidePanels();
                return;
            }

            // Windowを表示する
            var panelNames = spellArray.Select(x => x.Panel.Trim()).Distinct();
            foreach (var name in panelNames)
            {
                ActGlobals.oFormActMain.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    var w = this.SpellTimerPanels.Where(x => x.PanelName == name).FirstOrDefault();
                    if (w == null)
                    {
                        w = new SpellTimerListWindow()
                        {
                            Title = "SpecialSpellTimer - " + name,
                            PanelName = name,
                        };

                        this.SpellTimerPanels.Add(w);

                        // クリックスルー？
                        if (Settings.Default.ClickThroughEnabled)
                        {
                            w.ToTransparentWindow();
                        }

                        w.Show();
                    }

                    w.SpellTimers = (
                        from x in spellArray
                        where
                        x.Panel.Trim() == name
                        select
                        x).ToArray();

                    w.RefreshSpellTimer();
                });
            }
        }

        /// <summary>
        /// Panelをアクティブ化する
        /// </summary>
        public void ActivatePanels()
        {
            if (this.SpellTimerPanels != null)
            {
                if (ActGlobals.oFormActMain != null &&
                    ActGlobals.oFormActMain.Visible)
                {
                    ActGlobals.oFormActMain.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        foreach (var panel in this.SpellTimerPanels)
                        {
                            if (panel.Visibility == Visibility.Hidden)
                            {
                                panel.Visibility = Visibility.Visible;
                            }

                            panel.Activate();
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Panelを表示する
        /// </summary>
        public void VisiblePanels()
        {
            if (this.SpellTimerPanels != null)
            {
                if (ActGlobals.oFormActMain != null &&
                    ActGlobals.oFormActMain.Visible)
                {
                    ActGlobals.oFormActMain.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        foreach (var panel in this.SpellTimerPanels)
                        {
                            panel.Visibility = Visibility.Visible;
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Panelを隠す
        /// </summary>
        public void HidePanels()
        {
            if (this.SpellTimerPanels != null)
            {
                if (ActGlobals.oFormActMain != null &&
                    ActGlobals.oFormActMain.Visible)
                {
                    ActGlobals.oFormActMain.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        foreach (var panel in this.SpellTimerPanels)
                        {
                            panel.Visibility = Visibility.Hidden;
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Panelを閉じる
        /// </summary>
        public void ClosePanels()
        {
            if (this.SpellTimerPanels != null)
            {
                // Panelの位置を保存する
                PanelSettings.Default.SettingsTable.Clear();
                foreach (var panel in this.SpellTimerPanels)
                {
                    var setting = PanelSettings.Default.SettingsTable.NewPanelSettingsRow();
                    setting.PanelName = panel.PanelName;
                    setting.Left = panel.Left;
                    setting.Top = panel.Top;
                    PanelSettings.Default.SettingsTable.AddPanelSettingsRow(setting);
                }

                PanelSettings.Default.Save();

                // Panelを閉じる
                ActGlobals.oFormActMain.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    foreach (var panel in this.SpellTimerPanels)
                    {
                        panel.Close();
                    }

                    this.SpellTimerPanels.Clear();
                });
            }
        }

        /// <summary>
        /// Panelの位置を設定する
        /// </summary>
        public void LayoutPanels()
        {
            if (this.SpellTimerPanels != null)
            {
                ActGlobals.oFormActMain.Invoke((System.Windows.Forms.MethodInvoker)delegate
                {
                    foreach (var panel in this.SpellTimerPanels)
                    {
                        var setting = PanelSettings.Default.SettingsTable
                            .Where(x => x.PanelName == panel.PanelName)
                            .FirstOrDefault();

                        if (setting != null)
                        {
                            panel.Left = setting.Left;
                            panel.Top = setting.Top;
                        }
                        else
                        {
                            panel.Left = 10.0d;
                            panel.Top = 10.0d;
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 再生する
        /// </summary>
        /// <param name="source">再生するSource</param>
        private void Play(
            string source)
        {
            ACT.SpecialSpellTimer.Sound.SoundController.Default.Play(source);
        }

        /// <summary>
        /// 正規表現用のキーワードを生成する
        /// </summary>
        /// <param name="pattern">元のパターン</param>
        /// <returns>マッチ用キーワード</returns>
        private string MakeKeyword(
            string pattern)
        {
            var keyword = pattern.Trim();

            var player = FF14PluginHelper.GetPlayer();
            if (player != null)
            {
                keyword = keyword.Replace("<me>", player.Name.Trim());
            }

            return string.IsNullOrWhiteSpace(keyword) ?
                string.Empty :
                ".*" + keyword + ".*";
        }
    }
}
