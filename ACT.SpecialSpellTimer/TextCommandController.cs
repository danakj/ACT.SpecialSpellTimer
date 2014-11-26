namespace ACT.SpecialSpellTimer
{
    using System.Text.RegularExpressions;

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
            @".*/spespe (?<command>.*) (?<target>.*)( (?<windowname>"".*"") (?<value>.*))*",
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
                                break;

                            case "telops":
                                OnePointTelopController.CloseTelops();
                                break;
                        }

                        break;

                    case "changeenabled":
                        switch (target)
                        {
                            case "spells":
                                foreach (var spell in SpellTimerTable.Table)
                                {
                                    if (spell.Panel.Trim() == windowname.Trim() ||
                                        spell.SpellTitle.Trim() == windowname.Trim())
                                    {
                                        spell.Enabled = value;
                                    }
                                }

                                ActInvoker.Invoke(() =>
                                {
                                    SpecialSpellTimerPlugin.ConfigPanel.LoadSpellTimerTable();
                                });

                                break;

                            case "telops":
                                foreach (var telop in OnePointTelopTable.Default.Table)
                                {
                                    if (telop.Title.Trim() == windowname.Trim())
                                    {
                                        telop.Enabled = value;
                                    }
                                }

                                ActInvoker.Invoke(() =>
                                {
                                    SpecialSpellTimerPlugin.ConfigPanel.LoadTelopTable();
                                });

                                break;
                        }

                        break;
                }
            }
        }
    }
}
