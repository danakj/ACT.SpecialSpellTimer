namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
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

            lock (this.buffer)
            {
                var logLine = logInfo.logLine.Trim();

                // パーティに変化あり？
                if (ptmember == null ||
                    logLine.Contains("パーティを解散しました。") ||
                    logLine.Contains("がパーティに参加しました。") ||
                    logLine.Contains("がパーティから離脱しました。") ||
                    logLine.Contains("をパーティから離脱させました。"))
                {
                    Thread.Sleep(3 * 1000);
                    RefreshPTList();
                }

                if (Settings.Default.EnabledPartyMemberPlaceholder)
                {
                    for (int i = 0; i < ptmember.Count; i++)
                    {
                        logLine = logLine.Replace(
                            ptmember[i],
                            "<" + (i + 2).ToString() + ">");
                    }
                }

                this.buffer.Add(logLine);
            }
        }

        /// <summary>
        /// ゾーンが変わった？
        /// </summary>
        private void ZoneChanged()
        {
            // プレイヤ情報を更新する
            FF14PluginHelper.RefreshPlayer();

            RefreshPTList();
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
