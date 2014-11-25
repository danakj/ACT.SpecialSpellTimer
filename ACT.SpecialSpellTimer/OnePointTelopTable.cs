namespace ACT.SpecialSpellTimer
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// ワンポイントテレロップ設定テーブル
    /// </summary>
    public class OnePointTelopTable
    {
        /// <summary>
        /// シングルトンinstance
        /// </summary>
        private static OnePointTelopTable instance;

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        public static OnePointTelopTable Default
        {
            get
            {
                if (instance == null)
                {
                    instance = new OnePointTelopTable();
                }

                return instance;
            }
        }

        /// <summary>
        /// データテーブル
        /// </summary>
        private SpellTimerDataSet.OnePointTelopDataTable table = new SpellTimerDataSet.OnePointTelopDataTable();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OnePointTelopTable()
        {
            this.Load();
        }

        /// <summary>
        /// 生のテーブル
        /// </summary>
        public SpellTimerDataSet.OnePointTelopDataTable Table
        {
            get
            {
                return this.table;
            }
        }

        /// <summary>
        /// 有効なエントリのリスト
        /// </summary>
        public SpellTimerDataSet.OnePointTelopRow[] EnabledTable
        {
            get
            {
                var spells =
                    from x in this.table
                    where
                    x.Enabled
                    orderby
                    x.MatchDateTime ascending
                    select
                    x;

                // コンパイル済みの正規表現をセットする
                foreach (var spell in spells)
                {
                    if (!spell.RegexEnabled)
                    {
                        spell.RegexPattern = string.Empty;
                        spell.Regex = null;
                        spell.RegexPatternToHide = string.Empty;
                        spell.RegexToHide = null;
                        continue;
                    }

                    var pattern = SpellTimerTable.MakeKeyword(spell.Keyword);

                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        if (spell.IsRegexNull() ||
                            spell.Regex == null ||
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

                    var patternToHide = SpellTimerTable.MakeKeyword(spell.KeywordToHide);

                    if (!string.IsNullOrWhiteSpace(patternToHide))
                    {
                        if (spell.IsRegexToHideNull() ||
                            spell.RegexToHide == null ||
                            spell.RegexPatternToHide != patternToHide)
                        {
                            spell.RegexPatternToHide = patternToHide;
                            spell.RegexToHide = new Regex(
                                patternToHide,
                                RegexOptions.Compiled);
                        }
                    }
                    else
                    {
                        spell.RegexPatternToHide = string.Empty;
                        spell.RegexToHide = null;
                    }
                }

                return spells.ToArray();
            }
        }


        /// <summary>
        /// デフォルトのファイル
        /// </summary>
        public string DefaultFile
        {
            get
            {
                var r = string.Empty;

                r = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"anoyetta\ACT\ACT.SpecialSpellTimer.Telops.xml");

                return r;
            }
        }

        /// <summary>
        /// マッチ状態をリセットする
        /// </summary>
        public void Reset()
        {
            foreach (var row in this.table)
            {
                row.MatchDateTime = DateTime.MinValue;
                row.Regex = null;
                row.RegexPattern = string.Empty;
            }

            this.table.AcceptChanges();
        }

        /// <summary>
        /// Load
        /// </summary>
        public void Load()
        {
            this.Load(this.DefaultFile, true);
        }

        /// <summary>
        /// Load
        /// </summary>
        /// <param name="file">ファイル</param>
        /// <param name="isClear">クリアしてから取り込むか？</param>
        public void Load(
            string file,
            bool isClear)
        {
            if (File.Exists(file))
            {
                if (isClear)
                {
                    this.table.Clear();
                }

                this.table.ReadXml(file);
            }
        }

        /// <summary>
        /// Save
        /// </summary>
        public void Save()
        {
            this.Save(this.DefaultFile);
        }

        /// <summary>
        /// Save
        /// </summary>
        /// <param name="file">ファイル</param>
        public void Save(
            string file)
        {
            this.table.AcceptChanges();

            var copy = this.table as SpellTimerDataSet.OnePointTelopDataTable;

            var dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            foreach (var item in copy)
            {
                item.MessageReplaced = string.Empty;
                item.MatchedLog = string.Empty;
                item.MatchDateTime = DateTime.MinValue;
                item.Regex = null;
                item.RegexPattern = string.Empty;
            }

            copy.AcceptChanges();
            copy.WriteXml(file);
        }
    }
}
