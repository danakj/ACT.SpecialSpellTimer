namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;

    using Advanced_Combat_Tracker;

    /// <summary>
    /// コンバットアナライザ
    /// </summary>
    public class CombatAnalyzer
    {
        private static readonly string[] CastKeywords = new string[] { "を唱えた。", "の構え。" };
        private static readonly string[] ActionKeywords = new string[] { "「", "」" };
        private static readonly string[] HPRateKeywords = new string[] { "HP at" };
        private static readonly string[] AddedKeywords = new string[] { "Added new combatant" };

        private static readonly Regex CastRegex = new Regex(
            @"\[.+?\] \d\d:.+?:(?<actor>.+?)は「(?<skill>.+?)」(を唱えた。|の構え。)$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex ActionRegex = new Regex(
            @"\[.+?\] \d\d:.+?:(?<actor>.+?)の「(?<skill>.+?)」$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex HPRateRegex = new Regex(
            @"\[.+?\] \d\d:(?<actor>.+?) HP at (?<hprate>\d+?)%",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        private static readonly Regex AddedRegex = new Regex(
            @"\[.+?\] \d\d:Added new combatant (?<actor>.+)\.  ",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        /// <summary>
        /// シングルトンInstance
        /// </summary>
        private static CombatAnalyzer instance;

        /// <summary>
        /// シングルトンInstance
        /// </summary>
        public static CombatAnalyzer Default
        {
            get
            {
                if (instance == null)
                {
                    instance = new CombatAnalyzer();
                }

                return instance;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombatAnalyzer()
        {
            this.CurrentCombatLogList = new List<CombatLog>();
        }

        /// <summary>
        /// 分析中？
        /// </summary>
        public bool IsAnalyzing { get; private set; }

        /// <summary>
        /// 戦闘ログのリスト
        /// </summary>
        public List<CombatLog> CurrentCombatLogList { get; private set; }

        /// <summary>
        /// 分析を開始したタイムスタンプ
        /// </summary>
        private DateTime StartAnalyzeTimeStamp { get; set; }

        /// <summary>
        /// 分析を開始する
        /// </summary>
        public void Start()
        {
            this.CurrentCombatLogList.Clear();
            this.CurrentCombatLogList.Add(new CombatLog()
            {
                TimeStamp = DateTime.Now,
                TimeStampElapted = 0,
                LogType = CombatLogType.AnalyzeStart,
                Action = "分析開始"
            });

            ActGlobals.oFormActMain.OnLogLineRead += this.oFormActMain_OnLogLineRead;
            this.IsAnalyzing = true;
        }

        /// <summary>
        /// 分析を停止する
        /// </summary>
        public void Stop()
        {
            if (this.IsAnalyzing)
            {
                ActGlobals.oFormActMain.OnLogLineRead -= this.oFormActMain_OnLogLineRead;
                this.IsAnalyzing = false;

                if (this.CurrentCombatLogList != null &&
                    this.CurrentCombatLogList.Count > 0)
                {
                    var now = DateTime.Now;
                    this.CurrentCombatLogList.Add(new CombatLog()
                    {
                        TimeStamp = now,
                        TimeStampElapted = (now - this.StartAnalyzeTimeStamp).TotalSeconds,
                        LogType = CombatLogType.AnalyzeEnd,
                        Action = "分析終了"
                    });

                    // 分析する
                    this.AnalyzeLog(this.CurrentCombatLogList);
                }
            }
        }

        /// <summary>
        /// ログを分析する
        /// </summary>
        /// <param name="logList">ログのリスト</param>
        private void AnalyzeLog(
            List<CombatLog> logList)
        {
            if (logList == null ||
                logList.Count < 1)
            {
                return;
            }

            var previouseAction = new Dictionary<string, DateTime>();

            foreach (var log in logList.OrderBy(x => x.TimeStamp))
            {
                Thread.Sleep(10);

                switch (log.LogType)
                {
                    case CombatLogType.AnalyzeStart:
                        log.LogTypeName = "開始";
                        break;

                    case CombatLogType.AnalyzeEnd:
                        log.LogTypeName = "終了";
                        break;

                    case CombatLogType.CastStart:
                        log.LogTypeName = "準備動作";
                        break;

                    case CombatLogType.Action:
                        log.LogTypeName = "アクション";
                        break;

                    case CombatLogType.Added:
                        log.LogTypeName = "Added";
                        break;

                    case CombatLogType.HPRate:
                        log.LogTypeName = "残HP率";
                        break;
                }

                if (log.LogType == CombatLogType.AnalyzeStart ||
                    log.LogType == CombatLogType.AnalyzeEnd ||
                    log.LogType == CombatLogType.HPRate)
                {
                    continue;
                }

                var key = log.LogType.ToString() + "-" + log.Actor + "-" + log.Action;

                // 直前の同じログを探す
                if (previouseAction.ContainsKey(key))
                {
                    log.Span = (log.TimeStamp - previouseAction[key]).TotalSeconds;
                }

                // 記録しておく
                previouseAction[key] = log.TimeStamp;
            }
        }

        /// <summary>
        /// ログを1行読取った
        /// </summary>
        /// <param name="isImport">Importか？</param>
        /// <param name="logInfo">ログ情報</param>
        private void oFormActMain_OnLogLineRead(
            bool isImport,
            LogLineEventArgs logInfo)
        {
            if (!this.IsAnalyzing)
            {
                return;
            }

            if (this.CurrentCombatLogList == null ||
                this.CurrentCombatLogList.Count < 1)
            {
                return;
            }

            // プレイヤ情報とパーティリストを取得する
#if !DEBUG
            var player = FF14PluginHelper.GetPlayer();
            var ptlist = LogBuffer.GetPTMember();

            if (player == null ||
                ptlist == null)
            {
                return;
            }
            // ログにプレイヤ名が含まれている？
            if (logInfo.logLine.Contains(player.Name))
            {
                return;
            }
#endif

            // ログにペットが含まれている？
            if (logInfo.logLine.Contains("・エギ") ||
                logInfo.logLine.Contains("フェアリー・") ||
                logInfo.logLine.Contains("カーバンクル・"))
            {
                return;
            }

            // ログにパーティメンバ名が含まれている？
#if !DEBUG
            foreach (var name in ptlist)
            {
                if (logInfo.logLine.Contains(name))
                {
                    return;
                }
            }
#endif

            // キャストのキーワードが含まれている？
            foreach (var keyword in CastKeywords)
            {
                if (logInfo.logLine.Contains(keyword))
                {
                    this.StoreCastLog(logInfo);
                    return;
                }
            }

            // アクションのキーワードが含まれている？
            foreach (var keyword in ActionKeywords)
            {
                if (logInfo.logLine.Contains(keyword))
                {
                    this.StoreActionLog(logInfo);
                    return;
                }
            }

            // 残HP率のキーワードが含まれている？
            foreach (var keyword in HPRateKeywords)
            {
                if (logInfo.logLine.Contains(keyword))
                {
                    this.StoreHPRateLog(logInfo);
                    return;
                }
            }

            // Addedのキーワードが含まれている？
            foreach (var keyword in AddedKeywords)
            {
                if (logInfo.logLine.Contains(keyword))
                {
                    this.StoreAddedLog(logInfo);
                    return;
                }
            }
        }

        /// <summary>
        /// キャストログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreCastLog(
            LogLineEventArgs logInfo)
        {
            var match = CastRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            if (this.CurrentCombatLogList.Count <= 1)
            {
                this.StartAnalyzeTimeStamp = logInfo.detectedTime;
            }

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                TimeStampElapted = (logInfo.detectedTime - this.StartAnalyzeTimeStamp).TotalSeconds,
                Raw = logInfo.logLine,
                Actor = match.Groups["actor"].ToString(),
                Action = match.Groups["skill"].ToString() + " の準備動作",
                LogType = CombatLogType.CastStart
            };

            this.CurrentCombatLogList.Add(log);
        }

        /// <summary>
        /// アクションログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreActionLog(
            LogLineEventArgs logInfo)
        {
            var match = ActionRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            if (this.CurrentCombatLogList.Count <= 1)
            {
                this.StartAnalyzeTimeStamp = logInfo.detectedTime;
            }

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                TimeStampElapted = (logInfo.detectedTime - this.StartAnalyzeTimeStamp).TotalSeconds,
                Raw = logInfo.logLine,
                Actor = match.Groups["actor"].ToString(),
                Action = match.Groups["skill"].ToString() + " の発動",
                LogType = CombatLogType.Action
            };

            this.CurrentCombatLogList.Add(log);
        }

        /// <summary>
        /// HP率のログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreHPRateLog(
            LogLineEventArgs logInfo)
        {
            var match = HPRateRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            if (this.CurrentCombatLogList.Count <= 1)
            {
                this.StartAnalyzeTimeStamp = logInfo.detectedTime;
            }

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                TimeStampElapted = (logInfo.detectedTime - this.StartAnalyzeTimeStamp).TotalSeconds,
                Raw = logInfo.logLine,
                Actor = match.Groups["actor"].ToString(),
                HPRate = decimal.Parse(match.Groups["hprate"].ToString()),
                Action = "HP " + match.Groups["hprate"].ToString().PadLeft(3, ' ') + "%",
                LogType = CombatLogType.HPRate
            };

            this.CurrentCombatLogList.Add(log);
        }

        /// <summary>
        /// Addedのログを格納する
        /// </summary>
        /// <param name="logInfo">ログ情報</param>
        private void StoreAddedLog(
            LogLineEventArgs logInfo)
        {
            var match = AddedRegex.Match(logInfo.logLine);
            if (!match.Success)
            {
                return;
            }

            if (this.CurrentCombatLogList.Count <= 1)
            {
                this.StartAnalyzeTimeStamp = logInfo.detectedTime;
            }

            var log = new CombatLog()
            {
                TimeStamp = logInfo.detectedTime,
                TimeStampElapted = (logInfo.detectedTime - this.StartAnalyzeTimeStamp).TotalSeconds,
                Raw = logInfo.logLine,
                Actor = match.Groups["actor"].ToString(),
                Action = "Added",
                LogType = CombatLogType.Added
            };

            this.CurrentCombatLogList.Add(log);
        }
    }
}
