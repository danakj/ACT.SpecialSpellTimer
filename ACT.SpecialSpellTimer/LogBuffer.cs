namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using Advanced_Combat_Tracker;

    /// <summary>
    /// ログのバッファ
    /// </summary>
    public class LogBuffer : IDisposable
    {
        /// <summary>
        /// 内部バッファ
        /// </summary>
        private List<KeyValuePair<long, string>> buffer = new List<KeyValuePair<long, string>>();

        /// <summary>
        /// 既読キー
        /// </summary>
        private long allreadyKey;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LogBuffer()
        {
            ActGlobals.oFormActMain.OnLogLineRead += this.oFormActMain_OnLogLineRead;
            ActGlobals.oFormActMain.OnCombatEnd += this.oFormActMain_OnCombatEnd;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ActGlobals.oFormActMain.OnLogLineRead -= this.oFormActMain_OnLogLineRead;
            ActGlobals.oFormActMain.OnCombatEnd -= this.oFormActMain_OnCombatEnd;
            this.Clear();
        }

        /// <summary>
        /// 新しいログ行を返す
        /// </summary>
        /// <returns>
        /// 新しいログ行の配列</returns>
        public string[] GetNewLogLines()
        {
            var newLines =
                from x in this.buffer
                where
                x.Key > this.allreadyKey
                select
                x;

            this.allreadyKey = newLines.Any() ? newLines.Max(x => x.Key) : 0;

            return newLines.Select(x => x.Value).ToArray();
        }

        /// <summary>
        /// バッファをクリアする
        /// </summary>
        public void Clear()
        {
            lock (this.buffer)
            {
                this.buffer.Clear();
                this.allreadyKey = 0;
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
                this.buffer.Add(new KeyValuePair<long, string>(
                    this.buffer.Count + 1,
                    logInfo.logLine.Trim()));
            }
        }

        /// <summary>
        /// 戦闘終了
        /// </summary>
        /// <param name="isImport">Importか？</param>
        /// <param name="encounterInfo">ログ情報</param>
        private void oFormActMain_OnCombatEnd(bool isImport, CombatToggleEventArgs encounterInfo)
        {
            if (isImport)
            {
                return;
            }

            this.Clear();
        }
    }
}
