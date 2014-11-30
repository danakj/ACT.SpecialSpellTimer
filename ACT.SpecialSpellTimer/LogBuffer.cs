namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    using ACT.SpecialSpellTimer.Properties;
    using Advanced_Combat_Tracker;

    /// <summary>
    /// ログのバッファ
    /// </summary>
    public class LogBuffer : IDisposable
    {
        /// <summary>
        /// パーティメンバ
        /// </summary>
        private static List<string> ptmember;

        /// <summary>
        /// 内部バッファ
        /// </summary>
        private List<string> buffer = new List<string>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LogBuffer()
        {
            ActGlobals.oFormActMain.OnLogLineRead += this.oFormActMain_OnLogLineRead;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ActGlobals.oFormActMain.OnLogLineRead -= this.oFormActMain_OnLogLineRead;
            this.Clear();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// ログ行を返す
        /// </summary>
        /// <returns>
        /// ログ行の配列</returns>
        public string[] GetLogLines()
        {
            lock (this.buffer)
            {
                var logLines = this.buffer.ToArray();
                this.buffer.Clear();
                return logLines;
            }
        }

        /// <summary>
        /// バッファをクリアする
        /// </summary>
        public void Clear()
        {
            lock (this.buffer)
            {
                this.buffer.Clear();
                ptmember.Clear();
                Debug.WriteLine("Logをクリアしました");
            }
        }

        /// <summary>
        /// ログを一行読取った
        /// </summary>
        /// <param name="isImport">Importか？</param>
        /// <param name="logInfo">ログ情報</param>
        private void oFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            if (isImport)
            {
                return;
            }

#if false
            Debug.WriteLine(logInfo.logLine);
#endif

            var logLine = logInfo.logLine.Trim();

            // ジョブに変化あり？
            if (logLine.Contains("にチェンジした。"))
            {
                FF14PluginHelper.RefreshPlayer();
            }

            // パーティに変化あり？
            if (ptmember == null ||
                logLine.Contains("パーティを解散しました。") ||
                logLine.Contains("がパーティに参加しました。") ||
                logLine.Contains("がパーティから離脱しました。") ||
                logLine.Contains("をパーティから離脱させました。") ||
                logLine.Contains("の攻略を開始した。") ||
                logLine.Contains("の攻略を終了した。"))
            {
                Task.Run(() =>
                {
                    Thread.Sleep(5 * 1000);
                    RefreshPTList();
                }).ContinueWith((t) =>
                {
                    t.Dispose();
                });
            }

            lock (this.buffer)
            {
                this.buffer.Add(logLine);
            }
        }

        /// <summary>
        /// マッチングキーワードを生成する
        /// </summary>
        /// <param name="keyword">元のキーワード</param>
        /// <returns>生成したキーワード</returns>
        public static string MakeKeyword(
            string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return keyword.Trim();
            }

            keyword = keyword.Trim();

            var player = FF14PluginHelper.GetPlayer();
            if (player != null)
            {
                keyword = keyword.Replace("<me>", player.Name.Trim());
            }

            if (Settings.Default.EnabledPartyMemberPlaceholder)
            {
                if (ptmember != null)
                {
                    for (int i = 0; i < ptmember.Count; i++)
                    {
                        keyword = keyword.Replace(
                            "<" + (i + 2).ToString() + ">",
                            ptmember[i].Trim());
                    }
                }
            }

            return keyword;
        }

        /// <summary>
        /// 正規表現向けマッチングキーワードを生成する
        /// </summary>
        /// <param name="keyword">元のキーワード</param>
        /// <returns>生成したキーワード</returns>
        public static string MakeKeywordToRegex(
            string keyword)
        {
            keyword = MakeKeyword(keyword);

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return keyword.Trim();
            }

            return ".*" + keyword + ".*";
        }

        /// <summary>
        /// PTメンバーリストを返す
        /// </summary>
        /// <returns>PTメンバーリスト</returns>
        public static string[] GetPTMember()
        {
            return ptmember.ToArray();
        }

        /// <summary>
        /// パーティリストを更新する
        /// </summary>
        public static void RefreshPTList()
        {
            if (ptmember == null)
            {
                ptmember = new List<string>();
            }
            else
            {
                ptmember.Clear();
            }

            if (Settings.Default.EnabledPartyMemberPlaceholder)
            {
                Debug.WriteLine("PT: Refresh");

                // プレイヤー情報を取得する
                var player = FF14PluginHelper.GetPlayer();
                if (player == null)
                {
                    return;
                }

                // PTメンバの名前を記録しておく
                if (Settings.Default.EnabledPartyMemberPlaceholder)
                {
                    var partyList = FF14PluginHelper.GetCombatantListParty();
                    foreach (var member in partyList)
                    {
                        if (member.ID != player.ID)
                        {
                            ptmember.Add(member.Name.Trim());
                            Debug.WriteLine("<-  " + member.Name);
                        }
                    }
                }
            }
        }
    }
}
