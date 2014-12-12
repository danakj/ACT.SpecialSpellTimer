namespace ACT.SpecialSpellTimer
{
    using System.Media;
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
            @".*/spespe (?<command>refresh|changeenabled|analyze) (?<target>spells|telops|me|pt|pet|on|off)( (?<windowname>"".*""|all) (?<value>.*))*",
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
            var commandDone = false;
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
                    case "analyze":
                        switch (target)
                        {
                            case "on":
                                SpecialSpellTimerPlugin.ConfigPanel.CombatAnalyzerEnabled = true;
                                commandDone = true;
                                break;

                            case "off":
                                SpecialSpellTimerPlugin.ConfigPanel.CombatAnalyzerEnabled = false;
                                commandDone = true;
                                break;
                        }

                        break;

                    case "refresh":
                        switch (target)
                        {
                            case "spells":
                                SpellTimerCore.Default.ClosePanels();
                                commandDone = true;
                                break;

                            case "telops":
                                OnePointTelopController.CloseTelops();
                                commandDone = true;
                                break;

                            case "me":
                                FF14PluginHelper.RefreshPlayer();
                                commandDone = true;
                                break;

                            case "pt":
                                LogBuffer.RefreshPTList();
                                commandDone = true;
                                break;

                            case "pet":
                                LogBuffer.RefreshPetID();
                                commandDone = true;
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

                                    commandDone = true;
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

                                    commandDone = true;
                                }

                                break;
                        }

                        break;
                }
            }   // loop end logLines

            // コマンドを実行したらシステム音を鳴らす
            if (commandDone)
            {
                SystemSounds.Asterisk.Play();
            }
        }   // method end
    }
}
