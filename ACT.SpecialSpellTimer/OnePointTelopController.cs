namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Sound;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// ワンポイントテレロップ Controller
    /// </summary>
    public class OnePointTelopController
    {
        /// <summary>
        /// テロップWindowのリスト
        /// </summary>
        private static List<OnePointTelopWindow> telopWindowList = new List<OnePointTelopWindow>();

        /// <summary>
        /// テロップを閉じる
        /// </summary>
        public static void CloseTelops()
        {
            if (telopWindowList != null)
            {
                ActInvoker.Invoke(() =>
                {
                    foreach (var telop in telopWindowList)
                    {
                        telop.DataSource.Left = telop.Left;
                        telop.DataSource.Top = telop.Top;

                        telop.Close();
                    }
                });

                OnePointTelopTable.Default.Save();

                telopWindowList.Clear();
            }
        }

        /// <summary>
        /// テロップを隠す
        /// </summary>
        public static void HideTelops()
        {
            if (telopWindowList != null)
            {
                ActInvoker.Invoke(() =>
                {
                    foreach (var telop in telopWindowList)
                    {
                        telop.HideOverlay();
                    }
                });
            }
        }

        /// <summary>
        /// テロップをActive化する
        /// </summary>
        public static void ActivateTelops()
        {
            if (telopWindowList != null)
            {
                ActInvoker.Invoke(() =>
                {
                    foreach (var telop in telopWindowList)
                    {
                        telop.Activate();
                    }
                });
            }
        }

        /// <summary>
        /// Windowをリフレッシュする
        /// </summary>
        public static void Refresh()
        {
            var telops = OnePointTelopTable.Default.EnabledTable;
            var player = FF14PluginHelper.GetPlayer();

            foreach (var telop in telops)
            {
                try
                {
                    telop.BeginEdit();

                    var regex = telop.Regex as Regex;

                    // ディレイ時間が経過した？
                    if (!telop.Delayed &&
                        telop.MatchDateTime > DateTime.MinValue &&
                        telop.Delay > 0)
                    {
                        var delayed = telop.MatchDateTime.AddSeconds(telop.Delay);
                        if (DateTime.Now >= delayed)
                        {
                            telop.Delayed = true;
                            SoundController.Default.Play(telop.DelaySound);
                            var tts = regex != null && !string.IsNullOrWhiteSpace(telop.DelayTextToSpeak) ?
                                regex.Replace(telop.MatchedLog, telop.DelayTextToSpeak) :
                                telop.DelayTextToSpeak;
                            SoundController.Default.Play(tts);
                        }
                    }

                    var w = telopWindowList.Where(x => x.DataSource.ID == telop.ID).FirstOrDefault();
                    if (w == null)
                    {
                        w = new OnePointTelopWindow()
                        {
                            Title = "OnePointTelop - " + telop.Title,
                            DataSource = telop
                        };

                        if (Settings.Default.ClickThroughEnabled)
                        {
                            w.ToTransparentWindow();
                        }

                        w.Opacity = 0;
                        w.Show();

                        telopWindowList.Add(w);
                    }

                    w.Refresh();

                    // telopの位置を保存する
                    if (DateTime.Now.Second == 0)
                    {
                        telop.Left = w.Left;
                        telop.Top = w.Top;
                        OnePointTelopTable.Default.Save();
                    }

                    if (Settings.Default.OverlayVisible &&
                        Settings.Default.TelopAlwaysVisible)
                    {
                        w.ShowOverlay();
                        continue;
                    }

                    if (telop.MatchDateTime > DateTime.MinValue)
                    {
                        if (telop.MatchDateTime.AddSeconds(telop.Delay) <= DateTime.Now &&
                            DateTime.Now <= telop.MatchDateTime.AddSeconds(telop.Delay + telop.DisplayTime))
                        {
                            w.ShowOverlay();
                        }
                        else
                        {
                            w.HideOverlay();
                            telop.MatchDateTime = DateTime.MinValue;
                            telop.MessageReplaced = string.Empty;
                        }

                        if (telop.ForceHide)
                        {
                            w.HideOverlay();
                            telop.MatchDateTime = DateTime.MinValue;
                            telop.MessageReplaced = string.Empty;
                        }
                    }
                    else
                    {
                        w.HideOverlay();
                        telop.MessageReplaced = string.Empty;
                    }
                }
                finally
                {
                    telop.EndEdit();
                }
            }   // end loop telops
        }

        /// <summary>
        /// ログとマッチングする
        /// </summary>
        /// <param name="logLine">ログ行</param>
        public static void Match(
            string logLine)
        {
            var telops = OnePointTelopTable.Default.EnabledTable;
            var player = FF14PluginHelper.GetPlayer();

            foreach (var telop in telops)
            {
                try
                {
                    telop.BeginEdit();

                    var regex = telop.Regex as Regex;
                    var regexToHide = telop.RegexToHide as Regex;

                    // 通常マッチ
                    if (regex == null)
                    {
                        var keyword = player != null ?
                            telop.Keyword.Trim().Replace("<me>", player.Name) :
                            telop.Keyword.Trim();

                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            if (logLine.ToUpper().Contains(
                                keyword.ToUpper()))
                            {
                                if (!telop.AddMessageEnabled)
                                {
                                    telop.MessageReplaced = telop.Message;
                                }
                                else
                                {
                                    telop.MessageReplaced += string.IsNullOrWhiteSpace(telop.MessageReplaced) ?
                                        telop.Message :
                                        Environment.NewLine + telop.Message;
                                }

                                telop.MatchDateTime = DateTime.Now;
                                telop.Delayed = false;
                                telop.ForceHide = false;
                                telop.MatchedLog = logLine;

                                SoundController.Default.Play(telop.MatchSound);
                                SoundController.Default.Play(telop.MatchTextToSpeak);
                            }
                        }
                    }

                    // 正規表現マッチ
                    if (regex != null)
                    {
                        if (regex.IsMatch(logLine))
                        {
                            if (!telop.AddMessageEnabled)
                            {
                                telop.MessageReplaced = regex.Replace(logLine, telop.Message);
                            }
                            else
                            {
                                telop.MessageReplaced += string.IsNullOrWhiteSpace(telop.MessageReplaced) ?
                                    regex.Replace(logLine, telop.Message) :
                                    Environment.NewLine + regex.Replace(logLine, telop.Message);
                            }

                            telop.MatchDateTime = DateTime.Now;
                            telop.Delayed = false;
                            telop.ForceHide = false;
                            telop.MatchedLog = logLine;

                            SoundController.Default.Play(telop.MatchSound);
                            if (!string.IsNullOrWhiteSpace(telop.MatchTextToSpeak))
                            {
                                var tts = regex.Replace(logLine, telop.MatchTextToSpeak);
                                SoundController.Default.Play(tts);
                            }
                        }
                    }

                    // 通常マッチ(強制非表示)
                    if (regexToHide == null)
                    {
                        var keyword = player != null ?
                            telop.KeywordToHide.Trim().Replace("<me>", player.Name) :
                            telop.KeywordToHide.Trim();

                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            if (logLine.ToUpper().Contains(
                                keyword.ToUpper()))
                            {
                                telop.ForceHide = true;
                            }
                        }
                    }

                    // 正規表現マッチ(強制非表示)
                    if (regexToHide != null)
                    {
                        if (regexToHide.IsMatch(logLine))
                        {
                            telop.ForceHide = true;
                        }
                    }
                }
                finally
                {
                    telop.EndEdit();
                }
            }
        }
    }
}
