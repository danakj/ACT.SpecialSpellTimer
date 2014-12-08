namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Threading;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// ワンポイントテロップWindow
    /// </summary>
    public partial class OnePointTelopWindow : Window
    {
        /// <summary>
        /// ドラッグ開始
        /// </summary>
        private Action<MouseEventArgs> DragOn;

        /// <summary>
        /// ドラッグ終了
        /// </summary>
        private Action<MouseEventArgs> DragOff;

        /// <summary>
        /// ドラッグ中か？
        /// </summary>
        public bool IsDragging { get; private set; }

        /// <summary>フォントのBrush</summary>
        private SolidColorBrush FontBrush { get; set; }

        /// <summary>フォントのアウトラインBrush</summary>
        private SolidColorBrush FontOutlineBrush { get; set; }

        /// <summary>バーのBrush</summary>
        private SolidColorBrush BarBrush { get; set; }

        /// <summary>バーの背景のBrush</summary>
        private SolidColorBrush BarBackBrush { get; set; }

        /// <summary>バーのアウトラインのBrush</summary>
        private SolidColorBrush BarOutlineBrush { get; set; }

        /// <summary>背景色のBrush</summary>
        private SolidColorBrush BackgroundBrush { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OnePointTelopWindow()
        {
            Debug.WriteLine("Telop");
            this.InitializeComponent();

            this.MessageTextBlock.Text = string.Empty;

            this.Loaded += this.OnePointTelopWindow_Loaded;
            this.MouseLeftButtonDown += (s1, e1) => this.DragMove();

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
            this.MessageTextBlock.MouseDown += (s1, e1) => this.DragOn(e1);
            this.MessageTextBlock.MouseUp += (s1, e1) => this.DragOff(e1);
            this.ProgressBarCanvas.MouseDown += (s1, e1) => this.DragOn(e1);
            this.ProgressBarCanvas.MouseUp += (s1, e1) => this.DragOff(e1);
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
            if (this.IsDragging)
            {
                return;
            }

            if (this.DataSource == null)
            {
                this.HideOverlay();
                return;
            }

            // Brushを生成する
            var fontColor = this.DataSource.FontColor.FromHTML().ToWPF();
            var fontOutlineColor = string.IsNullOrWhiteSpace(this.DataSource.FontOutlineColor) ?
                Settings.Default.FontOutlineColor.ToWPF() :
                this.DataSource.FontOutlineColor.FromHTMLWPF();
            var barColor = fontColor;
            var barBackColor = barColor.ChangeBrightness(0.4d);
            var barOutlineColor = fontOutlineColor;
            var c = this.DataSource.BackgroundColor.FromHTML().ToWPF();
            var backGroundColor = Color.FromArgb(
                (byte)this.DataSource.BackgroundAlpha,
                c.R,
                c.G,
                c.B);

            this.FontBrush = this.CreateBrush(this.FontBrush, fontColor);
            this.FontOutlineBrush = this.CreateBrush(this.FontOutlineBrush, fontOutlineColor);
            this.BarBrush = this.CreateBrush(this.BarBrush, barColor);
            this.BarBackBrush = this.CreateBrush(this.BarBackBrush, barBackColor);
            this.BarOutlineBrush = this.CreateBrush(this.BarOutlineBrush, barOutlineColor);
            this.BackgroundBrush = this.CreateBrush(this.BackgroundBrush, backGroundColor);

            Dispatcher.InvokeAsync(new Action(() =>
            {
                if (!this.DataSource.ProgressBarEnabled &&
                    this.ProgressBarCanvas.Children.Count > 0)
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

                var count0AsText = count.ToString("N0");
                var displayTime0AsText = this.DataSource.DisplayTime.ToString("N0");
                count0AsText = count0AsText.PadLeft(displayTime0AsText.Length, '0');

                message = message.Replace("{COUNT}", countAsText);
                message = message.Replace("{COUNT0}", count0AsText);

                if (this.MessageTextBlock.Text != message)
                {
                    this.MessageTextBlock.Text = message;

                    var font = new System.Drawing.Font(
                        this.DataSource.FontFamily,
                        this.DataSource.FontSize,
                        (System.Drawing.FontStyle)this.DataSource.FontStyle);

                    this.MessageTextBlock.FontFamily = font.ToFontFamilyWPF();
                    this.MessageTextBlock.FontSize = font.ToFontSizeWPF();
                    this.MessageTextBlock.FontStyle = font.ToFontStyleWPF();
                    this.MessageTextBlock.FontWeight = font.ToFontWeightWPF();
                    this.MessageTextBlock.Fill = this.FontBrush;
                    this.MessageTextBlock.Stroke = this.FontOutlineBrush;
                    this.MessageTextBlock.StrokeThickness = (this.MessageTextBlock.FontSize / 100d * 2.5d);
                }

                // プログレスバーを表示しない？
                if (!this.DataSource.ProgressBarEnabled ||
                    this.DataSource.DisplayTime <= 0)
                {
                    this.ProgressBarCanvas.Visibility = Visibility.Hidden;
                    return;
                }

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
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        var barRect = this.BarRectangle;
                        barRect.Stroke = this.BarBrush;
                        barRect.Fill = this.BarBrush;
                        barRect.Width = this.MessageTextBlock.ActualWidth * progress;
                        barRect.Height = Settings.Default.ProgressBarSize.Height;
                        barRect.RadiusX = 2.0d;
                        barRect.RadiusY = 2.0d;
                        Canvas.SetLeft(barRect, 0);
                        Canvas.SetTop(barRect, 0);

                        var backRect = this.BarBackRectangle;
                        backRect.Stroke = this.BarBackBrush;
                        backRect.Fill = this.BarBackBrush;
                        backRect.Width = this.MessageTextBlock.ActualWidth;
                        backRect.Height = Settings.Default.ProgressBarSize.Height;
                        backRect.RadiusX = 2.0d;
                        backRect.RadiusY = 2.0d;
                        Canvas.SetLeft(backRect, 0);
                        Canvas.SetTop(backRect, 0);

                        var outlineRect = this.BarOutlineRectangle;
                        outlineRect.Stroke = this.BarOutlineBrush;
                        outlineRect.Width = this.MessageTextBlock.ActualWidth;
                        outlineRect.Height = Settings.Default.ProgressBarSize.Height;
                        outlineRect.RadiusX = 2.0d;
                        outlineRect.RadiusY = 2.0d;
                        Canvas.SetLeft(outlineRect, 0);
                        Canvas.SetTop(outlineRect, 0);

                        this.ProgressBarCanvas.Width = backRect.Width;
                        this.ProgressBarCanvas.Height = backRect.Height;
                    }),
                    DispatcherPriority.Loaded);

                    this.ProgressBarCanvas.Visibility = Visibility.Visible;
                }
            }));

            // 背景色を設定する
            var nowbackground = this.BaseColorRectangle.Fill as SolidColorBrush;
            if (nowbackground == null ||
                nowbackground.Color != this.BackgroundBrush.Color)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.BaseColorRectangle.Fill = this.BackgroundBrush;
                }),
                DispatcherPriority.Loaded);
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
