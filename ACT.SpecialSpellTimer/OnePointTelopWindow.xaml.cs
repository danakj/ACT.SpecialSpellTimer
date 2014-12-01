namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// ワンポイントテロップWindow
    /// </summary>
    public partial class OnePointTelopWindow : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OnePointTelopWindow()
        {
            this.InitializeComponent();

            this.MessageTextBlock.Text = string.Empty;

            this.ShowInTaskbar = false;
            this.Topmost = true;

            this.Loaded += this.OnePointTelopWindow_Loaded;
            this.MouseLeftButtonDown += (s1, e1) => this.DragMove();
        }

        /// <summary>
        /// 表示するデータソース
        /// </summary>
        public OnePointTelop DataSource
        {
            get;
            set;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void OnePointTelopWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataSource != null)
            {
                this.Left = this.DataSource.Left;
                this.Top = this.DataSource.Top;
            }

            this.Refresh();
        }

        /// <summary>
        /// 描画を更新する
        /// </summary>
        public void Refresh()
        {
            if (this.DataSource == null)
            {
                this.HideOverlay();
                return;
            }

            try
            {
                this.MessageTextBlock.Visibility = Visibility.Hidden;
                this.ProgressBarCanvas.Visibility = Visibility.Hidden;

                if (!this.DataSource.ProgressBarEnabled)
                {
                    this.ProgressBarCanvas.Children.Clear();
                }

                var message = Settings.Default.TelopAlwaysVisible ?
                    this.DataSource.Message.Replace(",", Environment.NewLine) :
                    this.DataSource.MessageReplaced.Replace(",", Environment.NewLine);

                // カウントダウンプレースホルダを置換する
                var count = (
                    this.DataSource.MatchDateTime.AddSeconds(DataSource.Delay + DataSource.DisplayTime) -
                    DateTime.Now).TotalSeconds;

                if (count < 0.0d)
                {
                    count = 0.0d;
                }

                if (Settings.Default.TelopAlwaysVisible)
                {
                    count = 0.0d;
                }

                var countAsText = count.ToString("N1");
                var displayTimeAsText = this.DataSource.DisplayTime.ToString("N1");
                countAsText = countAsText.PadLeft(displayTimeAsText.Length, '0');
                this.MessageTextBlock.Text = message.Replace("{COUNT}", countAsText);

                var font = new System.Drawing.Font(
                    this.DataSource.FontFamily,
                    this.DataSource.FontSize,
                    (System.Drawing.FontStyle)this.DataSource.FontStyle);

                var fillBrush = new SolidColorBrush(this.DataSource.FontColor.FromHTML().ToWPF());
                var strokeBrush = string.IsNullOrWhiteSpace(this.DataSource.FontOutlineColor) ?
                    new SolidColorBrush(Settings.Default.FontOutlineColor.ToWPF()) :
                    new SolidColorBrush(this.DataSource.FontOutlineColor.FromHTMLWPF());
                this.MessageTextBlock.FontFamily = font.ToFontFamilyWPF();
                this.MessageTextBlock.FontSize = font.ToFontSizeWPF();
                this.MessageTextBlock.FontStyle = font.ToFontStyleWPF();
                this.MessageTextBlock.FontWeight = font.ToFontWeightWPF();
                this.MessageTextBlock.Fill = fillBrush;
                this.MessageTextBlock.Stroke = strokeBrush;
                this.MessageTextBlock.StrokeThickness = (this.MessageTextBlock.FontSize / 100d * 2.5d);

                this.Background = new SolidColorBrush(this.DataSource.BackColor.FromHTML().ToWPF());

                // プログレスバーを表示する？
                if (this.DataSource.ProgressBarEnabled &&
                    this.DataSource.DisplayTime > 0)
                {
                    // 残り表示時間の率を算出する
                    var progress = 1.0d;
                    if (this.DataSource.MatchDateTime > DateTime.MinValue)
                    {
                        // 表示の残り時間を求める
                        var displayTimeRemain = (
                            this.DataSource.MatchDateTime.AddSeconds(this.DataSource.Delay + this.DataSource.DisplayTime) -
                            DateTime.Now).TotalSeconds;

                        if (displayTimeRemain < 0.0d)
                        {
                            displayTimeRemain = 0.0d;
                        }

                        // 率を求める
                        if (this.DataSource.DisplayTime > 0)
                        {
                            progress = displayTimeRemain / this.DataSource.DisplayTime;
                        }
                    }

                    // 常に表示するときは100%表示
                    if (Settings.Default.TelopAlwaysVisible)
                    {
                        progress = 1.0d;
                    }

                    if (progress > 0.0d)
                    {
                        // Fontの95%の明るさで描画する
                        var brush = new SolidColorBrush(this.DataSource
                            .FontColor
                            .FromHTML()
                            .ToWPF()
                            .ChangeBrightness(0.95d));

                        var backBrush = new SolidColorBrush(brush.Color.ChangeBrightness(0.4d));

                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            var barRect = new Rectangle();
                            barRect.Stroke = brush;
                            barRect.Fill = brush;
                            barRect.Width = this.MessageTextBlock.ActualWidth * progress;
                            barRect.Height = Settings.Default.ProgressBarSize.Height;
                            barRect.RadiusX = 2.0d;
                            barRect.RadiusY = 2.0d;
                            Canvas.SetLeft(barRect, 0);
                            Canvas.SetTop(barRect, 0);

                            var backRect = new Rectangle();
                            backRect.Stroke = backBrush;
                            backRect.Fill = backBrush;
                            backRect.Width = this.MessageTextBlock.ActualWidth;
                            backRect.Height = Settings.Default.ProgressBarSize.Height;
                            backRect.RadiusX = 2.0d;
                            backRect.RadiusY = 2.0d;
                            Canvas.SetLeft(backRect, 0);
                            Canvas.SetTop(backRect, 0);

                            var outlineRect = new Rectangle();
                            outlineRect.Stroke = strokeBrush;
                            outlineRect.Width = this.MessageTextBlock.ActualWidth;
                            outlineRect.Height = Settings.Default.ProgressBarSize.Height;
                            outlineRect.RadiusX = 2.0d;
                            outlineRect.RadiusY = 2.0d;
                            Canvas.SetLeft(outlineRect, 0);
                            Canvas.SetTop(outlineRect, 0);

                            this.ProgressBarCanvas.Width = backRect.Width;
                            this.ProgressBarCanvas.Height = backRect.Height;

                            this.ProgressBarCanvas.Children.Clear();
                            this.ProgressBarCanvas.Children.Add(backRect);
                            this.ProgressBarCanvas.Children.Add(barRect);
                            this.ProgressBarCanvas.Children.Add(outlineRect);
                        }),
                        DispatcherPriority.Loaded);

                        this.ProgressBarCanvas.Visibility = Visibility.Visible;
                    }
                }
            }
            finally
            {
                this.MessageTextBlock.Visibility = Visibility.Visible;
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
