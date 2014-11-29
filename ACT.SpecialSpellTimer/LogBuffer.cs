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
        }

        /// <summary>
        /// ログを一行読取った
        /// </summary>
        /// <param name="isImport">Importか？</param>
        /// <param name="logInfo">ログ情報</param>
        private void oFormActMain_OnLogLineRead(bool isImport, LogLineEventArgs logInfo)
        {
            try
            {
                if (isImport)
                {
                    return;
                }

#if false
                Debug.WriteLine(logInfo.logLine);
#endif

                var logLine = logInfo.logLine.Trim();

                // パーティに変化あり？
                if (ptmember == null ||
                    logLine.Contains("パーティを解散しました。") ||
                    logLine.Contains("がパーティに参加しました。") ||
                    logLine.Contains("がパーティから離脱しました。") ||
                    logLine.Contains("をパーティから離脱させました。"))
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

                if (Settings.Default.EnabledPartyMemberPlaceholder)
                {
                    if (ptmember != null)
                    {
                        for (int i = 0; i < ptmember.Count; i++)
                        {
                            logLine = logLine.Replace(
                                ptmember[i],
                                "<" + (i + 2).ToString() + ">");
                        }
                    }
                }

                // テロップとマッチングする
                OnePointTelopController.Match(logLine);

                // スペルとマッチングする
                SpellTimerCore.Default.Match(logLine);

                // テキストコマンドとマッチングする
                TextCommandController.MatchCommand(logLine);
            }
            catch (Exception ex)
            {
                ActGlobals.oFormActMain.WriteExceptionLog(
                ex,
                "ACT.SpecialSpellTimer ログの解析で例外が発生ました。" + Environment.NewLine +
                logInfo.logLine);
            }
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
