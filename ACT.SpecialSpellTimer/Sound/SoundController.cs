namespace ACT.SpecialSpellTimer.Sound
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using ACT.SpecialSpellTimer.Utility;
    using Advanced_Combat_Tracker;

    /// <summary>
    /// Soundコントローラ
    /// </summary>
    public class SoundController
    {
        /// <summary>
        /// ロックオブジェクト
        /// </summary>
        private static object lockObject = new object();

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        private static SoundController instance;

        /// <summary>
        /// シングルトンinstance
        /// </summary>
        public static SoundController Default
        {
            get
            {
                lock (lockObject)
                {
                    instance = new SoundController();
                    instance.LoadTTSYukkuri();
                }

                return instance;
            }
        }

        /// <summary>
        /// TTSYukkuriプラグインinstance
        /// </summary>
        private dynamic ttsYukkuriPlugin;

        /// <summary>
        /// ゆっくりが有効かどうか？
        /// </summary>
        public bool EnabledYukkuri
        {
            get
            {
                this.LoadTTSYukkuri();
                return this.ttsYukkuriPlugin != null;
            }
        }

        public string WaveDirectory
        {
            get
            {
                // ACTのパスを取得する
                var baseDir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                return Path.Combine(
                    baseDir,
                    @"resources\wav");
            }
        }

        /// <summary>
        /// Waveファイルを列挙する
        /// </summary>
        /// <returns>
        /// Waveファイルのコレクション</returns>
        public WaveFile[] EnumlateWave()
        {
            var list = new List<WaveFile>();

            // 未選択用のダミーをセットしておく
            list.Add(new WaveFile()
            {
                FullPath = string.Empty
            });

            if (Directory.Exists(this.WaveDirectory))
            {
                foreach (var wave in Directory.GetFiles(this.WaveDirectory, "*.wav")
                    .OrderBy(x => x)
                    .ToArray())
                {
                    list.Add(new WaveFile()
                    {
                        FullPath = wave
                    });
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// TTSYukkuriをロードする
        /// </summary>
        public void LoadTTSYukkuri()
        {
            lock (lockObject)
            {
                if (this.ttsYukkuriPlugin == null)
                {
                    if (ActGlobals.oFormActMain.Visible)
                    {
                        foreach (var item in ActGlobals.oFormActMain.ActPlugins)
                        {
                            if (item.pluginFile.Name.ToUpper() == "ACT.TTSYukkuri.dll".ToUpper() &&
                                item.lblPluginStatus.Text.ToUpper() == "Plugin Started".ToUpper())
                            {
                                this.ttsYukkuriPlugin = item.pluginObj;
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 再生する
        /// </summary>
        /// <param name="source">
        /// 再生する対象</param>
        public void Play(
            string source)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(source))
                {
                    return;
                }

                if (this.EnabledYukkuri)
                {
                    this.ttsYukkuriPlugin.Speak(source);
                }
                else
                {
                    Task.Run(() =>
                    {
                        // wav？
                        if (source.EndsWith(".wav"))
                        {
                            // ファイルが存在する？
                            if (File.Exists(source))
                            {
                                ActInvoker.Invoke(() =>
                                {
                                    ActGlobals.oFormActMain.PlaySound(source);
                                });
                            }
                        }
                        else
                        {
                            ActInvoker.Invoke(() =>
                            {
                                ActGlobals.oFormActMain.TTS(source);
                            });
                        }
                    }).ContinueWith((t) =>
                    {
                        if (t != null)
                        {
                            t.Dispose();
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                ActGlobals.oFormActMain.WriteExceptionLog(
                    ex,
                    "ACT.SpecialSpellTimer Soundの再生で例外が発生しました。");
            }
        }

        /// <summary>
        /// Waveファイル
        /// </summary>
        public class WaveFile
        {
            /// <summary>
            /// フルパス
            /// </summary>
            public string FullPath { get; set; }

            /// <summary>
            /// ファイル名
            /// </summary>
            public string Name
            {
                get
                {
                    return !string.IsNullOrWhiteSpace(this.FullPath) ?
                        Path.GetFileName(this.FullPath) :
                        string.Empty;
                }
            }

            /// <summary>
            /// ToString()
            /// </summary>
            /// <returns>一般化された文字列</returns>
            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}
