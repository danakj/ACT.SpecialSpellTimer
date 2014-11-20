namespace ACT.SpecialSpellTimer
{
    using System;
    using System.IO;

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
        /// デフォルトのファイル
        /// </summary>
        public static string DefaultFile
        {
            get
            {
                var r = string.Empty;

                r = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"anoyetta\ACT\ACT.SpecialSpellTimerTable.xml");

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
            }
        }

        /// <summary>
        /// 読み込む
        /// </summary>
        public static void Load()
        {
            Load(DefaultFile);
        }

        /// <summary>
        /// 読み込む
        /// </summary>
        /// <param name="file">ファイルパス</param>
        public static void Load(
            string file)
        {
            if (File.Exists(file))
            {
                Table.Clear();
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
            var dir = Path.GetDirectoryName(file);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            Table.WriteXml(file);
        }
    }
}
