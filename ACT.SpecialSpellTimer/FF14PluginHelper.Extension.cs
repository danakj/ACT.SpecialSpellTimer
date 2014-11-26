namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

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

        /// <summary>
        /// パーティの戦闘メンバリストを取得する
        /// </summary>
        /// <returns>パーティの戦闘メンバリスト</returns>
        public static List<Combatant> GetCombatantListParty()
        {
            // 総戦闘メンバリストを取得する（周囲のPC, NPC, MOB等すべて）
            var combatListAll = FF14PluginHelper.GetCombatantList();

            // パーティメンバのIDリストを取得する
            int partyCount;
            var partyListById = FF14PluginHelper.GetCurrentPartyList(out partyCount);

            var combatListParty = new List<Combatant>();

            foreach (var partyMemberId in partyListById)
            {
                if (partyMemberId == 0)
                {
                    continue;
                }

                var partyMember = (
                    from x in combatListAll
                    where
                    x.ID == partyMemberId
                    select
                    x).FirstOrDefault();

                if (partyMember != null)
                {
                    combatListParty.Add(partyMember);
                }
            }

            return combatListParty;
        }
    }
}
