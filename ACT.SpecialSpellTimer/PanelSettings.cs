namespace ACT.SpecialSpellTimer
{
    using System;
    using System.IO;

    /// <summary>
    /// Panel設定
    /// </summary>
    public class PanelSettings
    {
        /// <summary>
        /// 唯一のinstance
        /// </summary>
        private static PanelSettings instance;

        /// <summary>
        /// 唯一のinstance
        /// </summary>
        public static PanelSettings Default
        {
            get
            {
                if (instance == null)
                {
                    instance = new PanelSettings();
                    instance.Load();
                }

                return instance;
            }
        }

        /// <summary>
        /// Panel設定データテーブル
        /// </summary>
        private SpellTimerDataSet.PanelSettingsDataTable settingsTable = new SpellTimerDataSet.PanelSettingsDataTable();

        /// <summary>
        /// Panel設定データテーブル
        /// </summary>
        public SpellTimerDataSet.PanelSettingsDataTable SettingsTable
        {
            get
            {
                return this.settingsTable;
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
                    @"anoyetta\ACT\ACT.SpecialSpellTimer.Panels.xml");

                return r;
            }
        }

        /// <summary>
        /// ロード
        /// </summary>
        public void Load()
        {
            if (File.Exists(this.DefaultFile))
            {
                this.settingsTable.Clear();
                this.settingsTable.ReadXml(this.DefaultFile);
            }
        }

        /// <summary>
        /// セーブ
        /// </summary>
        public void Save()
        {
            this.settingsTable.AcceptChanges();

            if (this.settingsTable == null ||
                this.settingsTable.Count < 1)
            {
                return;
            }

            var dir = Path.GetDirectoryName(this.DefaultFile);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            this.settingsTable.WriteXml(this.DefaultFile);
        }

        /// <summary>
        /// テーブルファイルをバックアップする
        /// </summary>
        public void Backup()
        {
            var file = this.DefaultFile;

            if (File.Exists(file))
            {
                var backupFile = Path.Combine(
                    Path.GetDirectoryName(file),
                    Path.GetFileNameWithoutExtension(file) + "." + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".bak");

                File.Copy(
                    file,
                    backupFile,
                    true);

                // 古いバックアップを消す
                foreach (var bak in Directory.GetFiles(Path.GetDirectoryName(file), "*.bak"))
                {
                    var timeStamp = File.GetCreationTime(bak);
                    if ((DateTime.Now - timeStamp).TotalDays >= 3.0d)
                    {
                        File.Delete(bak);
                    }
                }
            }
        }
    }
}
