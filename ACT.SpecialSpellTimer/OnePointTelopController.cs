namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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
        /// 位置を設定する
        /// </summary>
        /// <param name="telopTitle">設定するテロップのTitle</param>
        /// <param name="left">Left</param>
        /// <param name="top">Top</param>
        public static void SetLocation(
            string telopTitle,
            double left,
            double top)
        {
            if (telopWindowList != null)
            {
                var telop = telopWindowList.Where(x => x.DataSource.Title == telopTitle).FirstOrDefault();
                if (telop != null)
                {
                    telop.Left = left;
                    telop.Top = top;
                }
            }
        }

        /// <summary>
        /// 位置を取得する
        /// </summary>
        /// <param name="telopTitle">設定するテロップのTitle</param>
        /// <param name="left">Left</param>
        /// <param name="top">Top</param>
        public static void GettLocation(
            string telopTitle,
            out double left,
            out double top)
        {
            left = 0;
            top = 0;

            if (telopWindowList != null)
            {
                var telop = telopWindowList.Where(x => x.DataSource.Title == telopTitle).FirstOrDefault();
                if (telop != null)
                {
                    left = telop.Left;
                    top = telop.Top;
                }
            }
        }

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
        /// ログとマッチングする
        /// </summary>
        /// <param name="logLines">ログ行</param>
        public static void Match(
            string[] logLines)
        {
            var telops = OnePointTelopTable.Default.EnabledTable;

            foreach (var telop in telops.AsParallel())
            {
                var regex = telop.Regex;
                var regexToHide = telop.RegexToHide;
                var isForceHide = false;

                foreach (var log in logLines)
                {
                    // 通常マッチ
                    if (regex == null)
                    {
                        var keyword = LogBuffer.MakeKeyword(telop.Keyword);
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            if (log.ToUpper().Contains(
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
                                telop.MatchedLog = log;

                                SoundController.Default.Play(telop.MatchSound);
                                SoundController.Default.Play(telop.MatchTextToSpeak);
                            }
                        }
                    }

                    // 正規表現マッチ
                    if (regex != null)
                    {
                        if (regex.IsMatch(log))
                        {
                            if (!telop.AddMessageEnabled)
                            {
                                telop.MessageReplaced = regex.Replace(log, telop.Message);
                            }
                            else
                            {
                                telop.MessageReplaced += string.IsNullOrWhiteSpace(telop.MessageReplaced) ?
                                    regex.Replace(log, telop.Message) :
                                    Environment.NewLine + regex.Replace(log, telop.Message);
                            }

                            telop.MatchDateTime = DateTime.Now;
                            telop.Delayed = false;
                            telop.MatchedLog = log;

                            SoundController.Default.Play(telop.MatchSound);
                            if (!string.IsNullOrWhiteSpace(telop.MatchTextToSpeak))
                            {
                                var tts = regex.Replace(log, telop.MatchTextToSpeak);
                                SoundController.Default.Play(tts);
                            }
                        }
                    }

                    // 通常マッチ(強制非表示)
                    if (regexToHide == null)
                    {
                        var keyword = LogBuffer.MakeKeyword(telop.KeywordToHide);
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            if (log.ToUpper().Contains(
                                keyword.ToUpper()))
                            {
                                isForceHide = true;
                            }
                        }
                    }

                    // 正規表現マッチ(強制非表示)
                    if (regexToHide != null)
                    {
                        if (regexToHide.IsMatch(log))
                        {
                            isForceHide = true;
                        }
                    }
                }   // end loop logLines

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
                    // ドラッグ中じゃない？
                    if (!w.IsDragging)
                    {
                        w.Refresh();
                        w.ShowOverlay();
                    }

                    continue;
                }

                if (telop.MatchDateTime > DateTime.MinValue)
                {
                    var start = telop.MatchDateTime.AddSeconds(telop.Delay);
                    var end = telop.MatchDateTime.AddSeconds(telop.Delay + telop.DisplayTime);

                    if (start <= DateTime.Now && DateTime.Now <= end)
                    {
                        w.Refresh();
                        w.ShowOverlay();
                    }
                    else
                    {
                        w.HideOverlay();

                        if (DateTime.Now > end)
                        {
                            telop.MatchDateTime = DateTime.MinValue;
                            telop.MessageReplaced = string.Empty;
                        }
                    }

                    if (isForceHide)
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
            }   // end loop telops
        }
    }
}
