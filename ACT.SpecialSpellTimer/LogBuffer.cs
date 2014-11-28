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
        /// 内部バッファ
        /// </summary>
        private List<string> buffer = new List<string>();

        /// <summary>
        /// パーティメンバ
        /// </summary>
        private List<string> ptmember = new List<string>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LogBuffer()
        {
            ActGlobals.oFormActMain.OnLogLineRead += this.oFormActMain_OnLogLineRead;
            ActGlobals.oFormActMain.OnCombatStart += this.oFormActMain_OnCombatStart;
            ActGlobals.oFormActMain.OnCombatEnd += this.oFormActMain_OnCombatEnd;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ActGlobals.oFormActMain.OnLogLineRead -= this.oFormActMain_OnLogLineRead;
            ActGlobals.oFormActMain.OnCombatStart -= this.oFormActMain_OnCombatStart;
            ActGlobals.oFormActMain.OnCombatEnd -= this.oFormActMain_OnCombatEnd;
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
                if (!Settings.Default.EnabledPartyMemberPlaceholder)
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

                    for (int i = 1; i < this.ptmember.Count; i++)
                    {
                        logLineReplaced = logLineReplaced.Replace(
                            this.ptmember[i],
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
                this.buffer.Add(logInfo.logLine.Trim());
            }
        }

        /// <summary>
        /// 戦闘がスタートした
        /// </summary>
        /// <param name="isImport">Importか？</param>
        /// <param name="encounterInfo">遭遇情報</param>
        private void oFormActMain_OnCombatStart(bool isImport, CombatToggleEventArgs encounterInfo)
        {
            if (isImport)
            {
                return;
            }

            // PTメンバの名前を記録しておく
            if (Settings.Default.EnabledPartyMemberPlaceholder)
            {
                this.ptmember.Clear();

                var partyList = FF14PluginHelper.GetCombatantListParty();
                foreach (var member in partyList)
                {
                    this.ptmember.Add(member.Name.Trim());
                }
            }
        }

        /// <summary>
        /// 戦闘がエンドした
        /// </summary>
        /// <param name="isImport">Importか？</param>
        /// <param name="encounterInfo">遭遇情報</param>
        private void oFormActMain_OnCombatEnd(bool isImport, CombatToggleEventArgs encounterInfo)
        {
            if (isImport)
            {
                return;
            }

            this.ptmember.Clear();
        }
    }
}
