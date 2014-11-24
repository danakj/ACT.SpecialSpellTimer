namespace ACT.SpecialSpellTimer
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// SpellTimerテーブル
    /// </summary>
    public static class SpellTimerTable
    {
        /// <summary>
        /// SpellTimerデータテーブル
        /// </summary>
        private static SpellTimerDataSet.SpellTimerDataTable table;

        /// <summary>
        /// SpellTimerデータテーブル
        /// </summary>
        public static SpellTimerDataSet.SpellTimerDataTable Table
        {
            get
            {
                if (table == null)
                {
                    table = new SpellTimerDataSet.SpellTimerDataTable();
                    Load();
                }

                return table;
            }
        }

        /// <summary>
        /// 有効なSpellTimerデータテーブル
        /// </summary>
        public static SpellTimerDataSet.SpellTimerRow[] EnabledTable
        {
            get
            {
                var spells =
                    from x in Table
                    where
                    x.Enabled
                    orderby
                    x.DisplayNo
                    select
                    x;

                // コンパイル済みの正規表現をセットする
                foreach (var spell in spells)
                {
                    var pattern = MakeKeyword(spell.Keyword);

                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        if (spell.Regex == null ||
                            spell.RegexPattern != pattern)
                        {
                            spell.RegexPattern = pattern;
                            spell.Regex = new Regex(
                                pattern,
                                RegexOptions.Compiled);
                        }
                    }
                    else
                    {
                        spell.RegexPattern = string.Empty;
                        spell.Regex = null;
                    }
                }

                return spells.ToArray();
            }
        }

        /// <summary>
        /// デフォルトのファイル
        /// </summary>
        public static string DefaultFile
        {
            get
            {
                var r = string.Empty;

                r = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"anoyetta\ACT\ACT.SpecialSpellTimer.Spells.xml");

                return r;
            }
        }

        /// <summary>
        /// カウントをリセットする
        /// </summary>
        public static void Reset()
        {
            foreach (var row in Table)
            {
                row.MatchDateTime = DateTime.MinValue;
                row.Regex = null;
                row.RegexPattern = string.Empty;
            }

            Table.AcceptChanges();
        }

        /// <summary>
        /// 読み込む
        /// </summary>
        public static void Load()
        {
            Load(DefaultFile, true);
        }

        /// <summary>
        /// 読み込む
        /// </summary>
        /// <param name="file">ファイルパス</param>
        /// <param name="isClear">消去してからロードする？</param>
        public static void Load(
            string file,
            bool isClear)
        {
            if (File.Exists(file))
            {
                if (isClear)
                {
                    Table.Clear();
                }

                Table.ReadXml(file);
                Reset();
            }
        }

        /// <summary>
        /// 保存する
        /// </summary>
        public static void Save()
        {
            Save(DefaultFile);
        }

        /// <summary>
        /// 保存する
        /// </summary>
        /// <param name="file">ファイルパス</param>

        public static void Save(
            string file)
        {
            Table.AcceptChanges();

            var copy = Table as SpellTimerDataSet.SpellTimerDataTable;

            var dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            foreach (var item in copy)
            {
                item.SpellTitleReplaced = string.Empty;
                item.MatchedLog = string.Empty;
                item.MatchDateTime = DateTime.MinValue;
                item.Regex = null;
                item.RegexPattern = string.Empty;
            }

            copy.AcceptChanges();
            copy.WriteXml(file);
        }

        /// <summary>
        /// 正規表現用のキーワードを生成する
        /// </summary>
        /// <param name="pattern">元のパターン</param>
        /// <returns>マッチ用キーワード</returns>
        private static string MakeKeyword(
            string pattern)
        {
            var keyword = pattern.Trim();

            var player = FF14PluginHelper.GetPlayer();
            if (player != null)
            {
                keyword = keyword.Replace("<me>", player.Name.Trim());
            }

            return string.IsNullOrWhiteSpace(keyword) ?
                string.Empty :
                ".*" + keyword + ".*";
        }
    }
}
