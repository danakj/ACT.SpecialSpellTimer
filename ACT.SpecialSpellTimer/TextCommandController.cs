namespace ACT.SpecialSpellTimer
{
    using System.Text.RegularExpressions;
    using ACT.SpecialSpellTimer.Sound;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// テキストコマンド Controller
    /// </summary>
    public static class TextCommandController
    {
        /// <summary>
        /// コマンド解析用の正規表現
        /// </summary>
        private static Regex regexCommand = new Regex(
            @".*/spespe (?<command>refresh|changeenabled) (?<target>spells|telops|me|pt)( (?<windowname>"".*""|all) (?<value>.*))*",
            RegexOptions.Compiled |
            RegexOptions.IgnoreCase);

        /// <summary>
        /// Commandとマッチングする
        /// </summary>
        /// <param name="logLines">
        /// ログ行</param>
        public static void MatchCommand(
            string[] logLines)
        {
            foreach (var log in logLines)
            {
                // 正規表現の前にキーワードがなければ抜けてしまう
                if (!log.ToLower().Contains("/spespe"))
                {
                    continue;
                }

                var match = regexCommand.Match(log);
                if (!match.Success)
                {
                    continue;
                }

                var command = match.Groups["command"].ToString().ToLower();
                var target = match.Groups["target"].ToString().ToLower();
                var windowname = match.Groups["windowname"].ToString().ToLower().Replace(@"""", string.Empty);
                var valueAsText = match.Groups["value"].ToString().ToLower();
                var value = false;
                if (!bool.TryParse(valueAsText, out value))
                {
                    value = false;
                }

                switch (command)
                {
                    case "refresh":
                        switch (target)
                        {
                            case "spells":
                                SpellTimerCore.Default.ClosePanels();
                                SoundController.Default.Play("リフレッシュ'スペル。");
                                break;

                            case "telops":
                                OnePointTelopController.CloseTelops();
                                SoundController.Default.Play("リフレッシュ'テロップ。");
                                break;

                            case "me":
                                FF14PluginHelper.RefreshPlayer();
                                SoundController.Default.Play("リフレッシュ'ミー。");
                                break;

                            case "pt":
                                LogBuffer.RefreshPTList();
                                SoundController.Default.Play("リフレッシュ'パーティー。");
                                break;
                        }

                        break;

                    case "changeenabled":
                        var changed = false;
                        switch (target)
                        {
                            case "spells":
                                foreach (var spell in SpellTimerTable.Table)
                                {
                                    if (spell.Panel.Trim().ToLower() == windowname.Trim().ToLower() ||
                                        spell.SpellTitle.Trim().ToLower() == windowname.Trim().ToLower() ||
                                        windowname.Trim().ToLower() == "all")
                                    {
                                        changed = true;
                                        spell.Enabled = value;
                                    }
                                }

                                if (changed)
                                {
                                    ActInvoker.Invoke(() =>
                                    {
                                        SpecialSpellTimerPlugin.ConfigPanel.LoadSpellTimerTable();
                                    });

                                    SoundController.Default.Play("チェインジ'スペル。");
                                }

                                break;

                            case "telops":
                                foreach (var telop in OnePointTelopTable.Default.Table)
                                {
                                    if (telop.Title.Trim().ToLower() == windowname.Trim().ToLower() ||
                                        windowname.Trim().ToLower() == "all")
                                    {
                                        changed = true;
                                        telop.Enabled = value;
                                    }
                                }

                                if (changed)
                                {
                                    ActInvoker.Invoke(() =>
                                    {
                                        SpecialSpellTimerPlugin.ConfigPanel.LoadTelopTable();
                                    });

                                    SoundController.Default.Play("チェインジ'テロップ。");
                                }

                                break;
                        }

                        break;
                }
            }
        }
    }
}
