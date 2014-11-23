namespace ACT.SpecialSpellTimer
{
    using System;

    /// <summary>
    /// FF14Pluginヘルパーの拡張部分
    /// </summary>
    public static partial class FF14PluginHelper
    {
        /// <summary>
        /// プレイヤー情報
        /// </summary>
        private static Combatant player;

        /// <summary>
        /// プレイヤー情報を最後に取得した日時
        /// </summary>
        private static DateTime lastPlayerDateTime;

        /// <summary>
        /// プレイヤー情報を取得する
        /// </summary>
        /// <returns>プレイヤー情報</returns>
        public static Combatant GetPlayer()
        {
            if (player == null)
            {
                var list = FF14PluginHelper.GetCombatantList();

                if (list.Count > 0)
                {
                    player = list[0];
                    lastPlayerDateTime = DateTime.Now;
                }
            }
            else
            {
                // 3分以上経過した？
                if ((DateTime.Now - lastPlayerDateTime).TotalMinutes >= 3.0d)
                {
                    var list = FF14PluginHelper.GetCombatantList();

                    if (list.Count > 0)
                    {
                        player = list[0];
                        lastPlayerDateTime = DateTime.Now;
                    }
                }
            }

            return player;
        }
    }
}
