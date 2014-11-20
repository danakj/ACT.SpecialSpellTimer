namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using ACT.SpecialSpellTimer.Properties;

    /// <summary>
    /// SpellTimerList Window
    /// </summary>
    public partial class SpellTimerListWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SpellTimerListWindow()
        {
            this.InitializeComponent();

            this.Loaded += this.SpellTimerListWindow_Loaded;
        }

        /// <summary>
        /// このPanelの名前
        /// </summary>
        public string PanelName { get; set; }

        /// <summary>
        /// 扱うSpellTimerのリスト
        /// </summary>
        public SpellTimerDataSet.SpellTimerRow[] SpellTimers { get; set; }

        /// <summary>
        /// Loaded
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void SpellTimerListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.RefreshSpellTimer();
        }

        /// <summary>
        /// SpellTimerの描画をRefreshする
        /// </summary>
        public void RefreshSpellTimer()
        {
            // コントロールを消去する
            this.ControlCanvas.Children.Clear();

            // 表示するものがなければ何もしない
            if (!this.SpellTimers.Any(x => x.ProgressBarVisible))
            {
                this.Visibility = Visibility.Hidden;
                return;
            }

            // 透明度を設定する
            this.Opacity = (100d - Settings.Default.Opacity) / 100d;

            var top = 0.0d;
            foreach (var spell in this.SpellTimers)
            {
                if (!spell.ProgressBarVisible)
                {
                    continue;
                }

                var c = new SpellTimerControl();
                c.SpellTitle = spell.SpellTitle;

                if (spell.MatchDateTime > DateTime.MinValue)
                {
                    var nextDateTime = spell.MatchDateTime.AddSeconds(spell.RecastTime);

                    c.RecastTime = (nextDateTime - DateTime.Now).TotalSeconds;
                    c.Progress = spell.RecastTime != 0 ?
                        c.RecastTime / spell.RecastTime :
                        1.0d;
                }

                c.Refresh();

                Canvas.SetLeft(c, 0);
                Canvas.SetTop(c, top);
                this.ControlCanvas.Children.Add(c);

                top += c.Height;

                this.Width = c.Width;
                this.Height = top;
            }
        }
    }
}
