namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

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
        private static List<string> ptmember = new List<string>();

        /// <summary>
        /// 内部バッファ
        /// </summary>
        private List<string> buffer = new List<string>();

        /// <summary>
        /// 前のゾーン名
        /// </summary>
        private string previousZoneName;

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
                if (!Settings.Default.EnabledPartyMemberPlaceholder ||
                    ptmember.Count <= 1)
                {
                    var logLines = this.buffer.ToArray();
                    this.buffer.Clear();
                    return logLines;
                }

                // PTメンバの名前をプレースホルダに置き換える
                var logLinesReplaced = new List<string>();
                foreach (var logLine in this.buffer)
                {
                    var logLineReplaced = logLine;

                    for (int i = 1; i < ptmember.Count; i++)
                    {
                        logLineReplaced = logLineReplaced.Replace(
                            ptmember[i],
                            "<" + (i + 1).ToString() + ">");
                    }

                    logLinesReplaced.Add(logLineReplaced);
                }

                this.buffer.Clear();
                return logLinesReplaced.ToArray();
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

            Debug.WriteLine(logInfo.logLine);

            lock (this.buffer)
            {
                if (ActGlobals.oFormActMain.CurrentZone != this.previousZoneName)
                {
                    this.ZoneChanged();
                }

                this.previousZoneName = ActGlobals.oFormActMain.CurrentZone;

                this.buffer.Add(logInfo.logLine.Trim());
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
            ptmember.Clear();

            if (Settings.Default.EnabledPartyMemberPlaceholder)
            {
                // PTメンバの名前を記録しておく
                if (Settings.Default.EnabledPartyMemberPlaceholder)
                {
                    var partyList = FF14PluginHelper.GetCombatantListParty();
                    foreach (var member in partyList)
                    {
                        ptmember.Add(member.Name.Trim());
                    }
                }
            }
        }
    }
}
