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

            this.ShowInTaskbar = false;
            this.Topmost = true;

            this.Loaded += this.SpellTimerListWindow_Loaded;
            this.MouseLeftButtonDown += (s1, e1) => this.DragMove();
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
            // Panelの位置を復元する
            var setting = PanelSettings.Default.SettingsTable
                .Where(x => x.PanelName == this.PanelName)
                .FirstOrDefault();

            if (setting != null)
            {
                this.Left = setting.Left;
                this.Top = setting.Top;
            }

            this.RefreshSpellTimer();
        }

        /// <summary>
        /// SpellTimerの描画をRefreshする
        /// </summary>
        public void RefreshSpellTimer()
        {
            // コントロールを消去する
            this.BaseGrid.Children.Clear();
            this.BaseGrid.RowDefinitions.Clear();

            // 表示するものがなければ何もしない
            if (this.SpellTimers == null ||
                !this.SpellTimers.Any(x => x.ProgressBarVisible))
            {
                this.Visibility = Visibility.Hidden;
                return;
            }

            this.Width = Settings.Default.ProgressBarSize.Width;

            // 透明度を設定する
            this.Opacity = (100d - Settings.Default.Opacity) / 100d;

            // スペルタイマコントロールのリストを生成する
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
                    if (c.RecastTime < 0)
                    {
                        c.RecastTime = 0;
                    }

                    c.Progress = spell.RecastTime != 0 ?
                        (spell.RecastTime - c.RecastTime) / spell.RecastTime :
                        1.0d;
                    if (c.Progress > 1.0d)
                    {
                        c.Progress = 1.0d;
                    }
                }

                c.Refresh();

                this.BaseGrid.RowDefinitions.Add(new RowDefinition());
                this.BaseGrid.Children.Add(c);

                c.SetValue(Grid.ColumnProperty, 0);
                c.SetValue(Grid.RowProperty, this.BaseGrid.Children.Count - 1);
            }

            if (this.BaseGrid.Children.Count > 0)
            {
                this.Visibility = Visibility.Visible;
            }
        }
    }
}
