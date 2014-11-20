namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Timers;
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
            // SpellTimerPanelを生成する
            this.SpellTimerPanels = new List<SpellTimerListWindow>();
            var panelNames = SpellTimerTable.Table.Select(x => x.Panel.Trim()).Distinct();
            foreach (var name in panelNames)
            {
                var w = new SpellTimerListWindow()
                {
                    PanelName = name,
                    SpellTimers = (
                        from x in SpellTimerTable.Table
                        where
                        x.Panel.Trim() == name
                        select
                        x).ToArray()
                };

                this.SpellTimerPanels.Add(w);

                w.Show();
            }

            // ACTのログ読取りにイベントを仕込む
            ActGlobals.oFormActMain.OnLogLineRead += this.oFormActMain_OnLogLineRead;

            // Refreshタイマを開始する
            this.RefreshTimer = new Timer()
            {
                AutoReset = false,
                Enabled = false,
                Interval = Settings.Default.RefreshInterval
            };

            this.RefreshTimer.Elapsed += this.RefreshTimer_Elapsed;
            this.RefreshTimer.Start();
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

            // 設定を保存する
            Settings.Default.Save();
            SpellTimerTable.Save();

            // 全てのPanelを閉じる
            foreach (var panel in this.SpellTimerPanels)
            {
                panel.Close();
            }

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
            lock (SpellTimerTable.Table)
            {
                try
                {
                    // インポートならば何もしない
                    if (isImport)
                    {
                        return;
                    }

                    Debug.WriteLine(
                        logInfo.detectedTime.ToString("yyyy-MM-dd HH:mm:ss.fff") + " " +
                        logInfo.logLine);

                    // プレイヤ情報を取得する
                    var player = this.GetPlayer();

                    // プレイヤ情報が取得できなければ全てのWindowを隠す
                    if (player == null)
                    {
                        foreach (var panel in this.SpellTimerPanels)
                        {
                            panel.Visibility = Visibility.Hidden;
                        }

                        this.RefreshTimer.Interval = 5000;
                        return;
                    }

                    // Spellリストとマッチングする
                    Parallel.ForEach(SpellTimerTable.Table, (spell) =>
                    {
                        var keyword = spell.Keyword.Trim();

                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            // <me>代名詞を置き換える
                            keyword = keyword.Replace("<me>", player.Name.Trim());

                            var regex = new Regex(
                                ".*" + keyword + ".*",
                                RegexOptions.IgnoreCase | RegexOptions.Singleline);

                            if (regex.IsMatch(logInfo.logLine.Trim()))
                            {
                                spell.MatchDateTime = DateTime.Now;
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    ActGlobals.oFormActMain.WriteExceptionLog(
                        ex,
                        "ACT.SpecialSpellTimer ログの解析で例外が発生しました。");
                }
            }
        }

        /// <summary>
        /// Refreshタイマ Elapsed
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (SpellTimerTable.Table)
            {
                try
                {
                    this.RefreshTimer.Stop();

                    // ACTが起動していない？
                    if (!ActGlobals.oFormActMain.Visible)
                    {
                        foreach (var panel in this.SpellTimerPanels)
                        {
                            panel.Visibility = Visibility.Hidden;
                        }

                        this.RefreshTimer.Interval = 5000;
                        return;
                    }

                    // FF14が起動していない？
                    if (FF14PluginHelper.GetFFXIVProcess == null)
                    {
                        foreach (var panel in this.SpellTimerPanels)
                        {
                            panel.Visibility = Visibility.Hidden;
                        }

                        this.RefreshTimer.Interval = 5000;
                        return;
                    }

                    // Repeat指定のSpellを更新する
                    Parallel.ForEach(SpellTimerTable.Table, (spell) =>
                    {
                        if (spell.RepeatEnabled &&
                            spell.MatchDateTime > DateTime.MinValue)
                        {
                            if (DateTime.Now >= spell.MatchDateTime.AddSeconds(spell.RecastTime))
                            {
                                spell.MatchDateTime = DateTime.Now;
                            }
                        }
                    });

                    // Windowを表示する
                    foreach (var panel in this.SpellTimerPanels)
                    {
                        panel.SpellTimers = (
                            from x in SpellTimerTable.Table
                            where
                            x.Panel == panel.Name
                            select
                            x).ToArray();

                        panel.RefreshSpellTimer();
                    }

                    // タイマの間隔を初期化する
                    this.RefreshTimer.Interval = Settings.Default.RefreshInterval;
                }
                catch (Exception ex)
                {
                    ActGlobals.oFormActMain.WriteExceptionLog(
                        ex,
                        "ACT.SpecialSpellTimer スペルタイマWindowのRefreshで例外が発生しました。");
                }
                finally
                {
                    this.RefreshTimer.Start();
                }
            }
        }

        /// <summary>
        /// プレイヤ情報を取得する
        /// </summary>
        /// <returns>プレイヤ情報</returns>
        private Combatant GetPlayer()
        {
            var list = FF14PluginHelper.GetCombatantList();

            if (list.Count > 0)
            {
                return list[0];
            }
            else
            {
                return null;
            }
        }
    }
}
