namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;

    using ACT.SpecialSpellTimer.Sound;

    /// <summary>
    /// SpellTimerテーブル
    /// </summary>
    public static class SpellTimerTable
    {
        /// <summary>
        /// SpellTimerデータテーブル
        /// </summary>
        private static List<SpellTimer> table;

        /// <summary>
        /// SpellTimerデータテーブル
        /// </summary>
        public static List<SpellTimer> Table
        {
            get
            {
                if (table == null)
                {
                    table = new List<SpellTimer>();
                    Load();
                }

                return table;
            }
        }

        /// <summary>
        /// 有効なSpellTimerデータテーブル
        /// </summary>
        public static SpellTimer[] EnabledTable
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

                // ジョブフィルタをかける
                var spellsFilteredJob = new List<SpellTimer>();
                foreach (var spell in spells)
                {
                    var player = FF14PluginHelper.GetPlayer();

                    if (player == null ||
                        string.IsNullOrWhiteSpace(spell.JobFilter))
                    {
                        spellsFilteredJob.Add(spell);
                        continue;
                    }

                    var jobs = spell.JobFilter.Split(',');
                    if (jobs.Any(x => x == player.Job.ToString()))
                    {
                        spellsFilteredJob.Add(spell);
                    }
                }

                // コンパイル済みの正規表現をセットする
                foreach (var spell in spellsFilteredJob)
                {
                    if (!spell.RegexEnabled)
                    {
                        spell.RegexPattern = string.Empty;
                        spell.Regex = null;
                        continue;
                    }

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

                return spellsFilteredJob.ToArray();
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

                row.MatchSound = !string.IsNullOrWhiteSpace(row.MatchSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(row.MatchSound)) :
                    string.Empty;
                row.OverSound = !string.IsNullOrWhiteSpace(row.OverSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(row.OverSound)) :
                    string.Empty;
                row.TimeupSound = !string.IsNullOrWhiteSpace(row.TimeupSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(row.TimeupSound)) :
                    string.Empty;
            }
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

                // 旧フォーマットを置換する
                var content = File.ReadAllText(file, new UTF8Encoding(false)).Replace(
                    "DocumentElement",
                    "ArrayOfSpellTimer");
                File.WriteAllText(file, content, new UTF8Encoding(false));

                using (var sr = new StreamReader(file, new UTF8Encoding(false)))
                {
                    var xs = new XmlSerializer(table.GetType());
                    table = xs.Deserialize(sr) as List<SpellTimer>; ;
                }

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
            var dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            foreach (var item in table)
            {
                item.MatchSound = !string.IsNullOrWhiteSpace(item.MatchSound) ?
                    Path.GetFileName(item.MatchSound) :
                    string.Empty;
                item.OverSound = !string.IsNullOrWhiteSpace(item.OverSound) ?
                    Path.GetFileName(item.OverSound) :
                    string.Empty;
                item.TimeupSound = !string.IsNullOrWhiteSpace(item.TimeupSound) ?
                    Path.GetFileName(item.TimeupSound) :
                    string.Empty;
            }

            using (var sw = new StreamWriter(file, false, new UTF8Encoding(false)))
            {
                var xs = new XmlSerializer(table.GetType());
                xs.Serialize(sw, table);
            }

            foreach (var item in table)
            {
                item.MatchSound = !string.IsNullOrWhiteSpace(item.MatchSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(item.MatchSound)) :
                    string.Empty;
                item.OverSound = !string.IsNullOrWhiteSpace(item.OverSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(item.OverSound)) :
                    string.Empty;
                item.TimeupSound = !string.IsNullOrWhiteSpace(item.TimeupSound) ?
                    Path.Combine(SoundController.Default.WaveDirectory, Path.GetFileName(item.TimeupSound)) :
                    string.Empty;
            }
        }

        /// <summary>
        /// 正規表現用のキーワードを生成する
        /// </summary>
        /// <param name="pattern">元のパターン</param>
        /// <returns>マッチ用キーワード</returns>
        public static string MakeKeyword(
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

    /// <summary>
    /// スペルタイマ
    /// </summary>
    [Serializable]
    public class SpellTimer
    {
        public SpellTimer()
        {
            this.Panel = string.Empty;
            this.SpellTitle = string.Empty;
            this.Keyword = string.Empty;
            this.MatchSound = string.Empty;
            this.MatchTextToSpeak = string.Empty;
            this.OverSound = string.Empty;
            this.OverTextToSpeak = string.Empty;
            this.TimeupSound = string.Empty;
            this.TimeupTextToSpeak = string.Empty;
            this.FontColor = string.Empty;
            this.BarColor = string.Empty;
            this.JobFilter = string.Empty;
            this.SpellTitleReplaced = string.Empty;
            this.MatchedLog = string.Empty;
            this.RegexPattern = string.Empty;
        }

        public string Panel { get; set; }
        public string SpellTitle { get; set; }
        public string Keyword { get; set; }
        public long RecastTime { get; set; }
        public bool RepeatEnabled { get; set; }
        public bool ProgressBarVisible { get; set; }
        public string MatchSound { get; set; }
        public string MatchTextToSpeak { get; set; }
        public string OverSound { get; set; }
        public string OverTextToSpeak { get; set; }
        public long OverTime { get; set; }
        public string TimeupSound { get; set; }
        public string TimeupTextToSpeak { get; set; }
        public DateTime MatchDateTime { get; set; }
        public bool TimeupHide { get; set; }
        public bool IsReverse { get; set; }
        public string FontColor { get; set; }
        public string BarColor { get; set; }
        public bool DontHide { get; set; }
        public bool RegexEnabled { get; set; }
        public string JobFilter { get; set; }
        public bool Enabled { get; set; }

        [XmlIgnore]
        public bool OverDone { get; set; }
        [XmlIgnore]
        public bool TimeupDone { get; set; }
        [XmlIgnore]
        public string SpellTitleReplaced { get; set; }
        [XmlIgnore]
        public string MatchedLog { get; set; }
        public long DisplayNo { get; set; }
        [XmlIgnore]
        public Regex Regex { get; set; }
        [XmlIgnore]
        public string RegexPattern { get; set; }
    }
}
