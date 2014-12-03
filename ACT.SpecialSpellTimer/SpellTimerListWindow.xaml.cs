namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;

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
            Debug.WriteLine("SpellList");
            this.InitializeComponent();

            this.ShowInTaskbar = false;
            this.Topmost = true;
            this.Background = Brushes.Transparent;

            this.SpellTimerControls = new Dictionary<long, SpellTimerControl>();

            this.Loaded += this.SpellTimerListWindow_Loaded;
            this.MouseLeftButtonDown += (s1, e1) => this.DragMove();
            this.Closed += (s1, e1) =>
            {
                if (this.SpellTimerControls != null)
                {
                    this.SpellTimerControls.Clear();
                }
            };

            this.DragOn = new Action<MouseEventArgs>((mouse) =>
            {
                if (mouse.LeftButton == MouseButtonState.Pressed)
                {
                    this.IsDragging = true;
                    Debug.WriteLine("Drag On");
                }
            });

            this.DragOff = new Action<MouseEventArgs>((mouse) =>
            {
                if (mouse.LeftButton == MouseButtonState.Released)
                {
                    this.IsDragging = false;
                    Debug.WriteLine("Drag Off");
                }
            });

            this.MouseDown += (s1, e1) => this.DragOn(e1);
            this.MouseUp += (s1, e1) => this.DragOff(e1);
        }

        /// <summary>
        /// このPanelの名前
        /// </summary>
        public string PanelName { get; set; }

        /// <summary>
        /// 扱うSpellTimerのリスト
        /// </summary>
        public SpellTimer[] SpellTimers { get; set; }

        /// <summary>
        /// 扱っているスペルタイマコントロールのリスト
        /// </summary>
        public Dictionary<long, SpellTimerControl> SpellTimerControls { get; private set; }

        /// <summary>
        /// ドラッグ中か？
        /// </summary>
        private bool IsDragging;

        /// <summary>
        /// ドラッグ開始
        /// </summary>
        private Action<MouseEventArgs> DragOn;

        /// <summary>
        /// ドラッグ終了
        /// </summary>
        private Action<MouseEventArgs> DragOff;

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
            if (this.IsDragging)
            {
                return;
            }

            // 表示するものがなければ何もしない
            if (this.SpellTimers == null ||
                !this.SpellTimers.Any(x => x.ProgressBarVisible))
            {
                this.HideOverlay();
                return;
            }

            // 表示対象だけに絞る
            var spells =
                from x in this.SpellTimers
                where
                x.ProgressBarVisible
                select
                x;

            // タイムアップしたものを除外する
            if (Settings.Default.TimeOfHideSpell > 0.0d)
            {
                spells =
                    from x in spells
                    where
                    x.DontHide ||
                    (DateTime.Now - x.MatchDateTime.AddSeconds(x.RecastTime)).TotalSeconds <= Settings.Default.TimeOfHideSpell
                    select
                    x;
            }

            // リキャストの近いもの順でソートする
            if (Settings.Default.AutoSortEnabled)
            {
                // 昇順？
                if (!Settings.Default.AutoSortReverse)
                {
                    spells =
                        from x in spells
                        orderby
                        x.MatchDateTime.AddSeconds(x.RecastTime),
                        x.DisplayNo
                        select
                        x;
                }
                else
                {
                    spells =
                        from x in spells
                        orderby
                        x.MatchDateTime.AddSeconds(x.RecastTime) descending,
                        x.DisplayNo
                        select
                        x;
                }
            }

            // スペルタイマコントロールのリストを生成する
            var displayList = new List<SpellTimerControl>();
            foreach (var spell in spells)
            {
                SpellTimerControl c;
                if (this.SpellTimerControls.ContainsKey(spell.ID))
                {
                    c = this.SpellTimerControls[spell.ID];
                }
                else
                {
                    c = new SpellTimerControl();
                    this.SpellTimerControls.Add(spell.ID, c);

                    c.Visibility = Visibility.Hidden;
                    c.MouseDown += (s, e) => this.DragOn(e);
                    c.MouseUp += (s, e) => this.DragOff(e);

                    c.HorizontalAlignment = HorizontalAlignment.Left;
                    c.VerticalAlignment = VerticalAlignment.Top;
                    c.Margin = new Thickness();

                    this.BaseGrid.RowDefinitions.Add(new RowDefinition());
                    this.BaseGrid.Children.Add(c);

                    c.SetValue(Grid.ColumnProperty, 0);
                    c.SetValue(Grid.RowProperty, this.BaseGrid.Children.Count - 1);
                }

                c.SpellTitle = string.IsNullOrWhiteSpace(spell.SpellTitleReplaced) ?
                    spell.SpellTitle :
                    spell.SpellTitleReplaced;
                c.IsReverse = spell.IsReverse;
                c.RecastTime = 0;
                c.Progress = 1.0d;

                c.BarColor = spell.BarColor;
                c.BarOutlineColor = spell.BarOutlineColor;
                c.BarWidth = spell.BarWidth;
                c.BarHeight = spell.BarHeight;
                c.TextFontFamily = spell.FontFamily;
                c.TextFontSize = spell.FontSize;
                c.TextFontStyle = spell.FontStyle;
                c.FontColor = spell.FontColor;
                c.FontOutlineColor = spell.FontOutlineColor;

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

                displayList.Add(c);
            }

            // 今回表示しないスペルを隠す
            foreach (var c in this.SpellTimerControls)
            {
                if (!spells.Any(x => x.ID == c.Key))
                {
                    c.Value.Visibility = Visibility.Hidden;
                }
            }

            // スペルの表示順を設定する
            var index = 0;
            foreach (var displaySpell in displayList)
            {
                displaySpell.SetValue(Grid.RowProperty, index);
                displaySpell.Visibility = Visibility.Visible;
                index++;
            }

            if (spells.Count() > 0)
            {
                this.ShowOverlay();
                this.Topmost = true;
            }
        }

        #region フォーカスを奪わない対策

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE, GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }

        #endregion
    }
}
