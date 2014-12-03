namespace ACT.SpecialSpellTimer
{
    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// SpellTimerControl
    /// </summary>
    public partial class SpellTimerControl : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SpellTimerControl()
        {
            Debug.WriteLine("Spell");
            this.InitializeComponent();
            this.Background = Brushes.Transparent;
        }

        /// <summary>
        /// スペルのTitle
        /// </summary>
        public string SpellTitle { get; set; }

        /// <summary>
        /// 残りリキャストTime(秒数)
        /// </summary>
        public double RecastTime { get; set; }

        /// <summary>
        /// リキャストの進捗率
        /// </summary>
        public double Progress { get; set; }

        /// <summary>
        /// プログレスバーを逆にするか？
        /// </summary>
        public bool IsReverse { get; set; }

        /// <summary>
        /// バーの色
        /// </summary>
        public string BarColor { get; set; }

        /// <summary>
        /// バーOutlineの色
        /// </summary>
        public string BarOutlineColor { get; set; }

        /// <summary>
        /// バーの幅
        /// </summary>
        public int BarWidth { get; set; }
        /// <summary>
        /// バーの高さ
        /// </summary>
        public int BarHeight { get; set; }

        /// <summary>
        /// フォントファミリー
        /// </summary>
        public string TextFontFamily { get; set; }

        /// <summary>
        /// フォントサイズ
        /// </summary>
        public float TextFontSize { get; set; }

        /// <summary>
        /// フォントスタイル
        /// </summary>
        public int TextFontStyle { get; set; }

        /// <summary>
        /// Fontの色
        /// </summary>
        public string FontColor { get; set; }

        /// <summary>
        /// FontOutlineの色
        /// </summary>
        public string FontOutlineColor { get; set; }

        /// <summary>
        /// 描画を更新する
        /// </summary>
        public void Refresh()
        {
#if false
            var sw = Stopwatch.StartNew();
#endif
            this.Width = this.BarWidth;

            var tb = default(OutlineTextBlock);
            var font = new System.Drawing.Font(this.TextFontFamily, this.TextFontSize, (System.Drawing.FontStyle)this.TextFontStyle);
            var fontBrush = new SolidColorBrush(Settings.Default.FontColor.ToWPF());
            var fontOutline = new SolidColorBrush(Settings.Default.FontOutlineColor.ToWPF());

            if (!string.IsNullOrWhiteSpace(this.FontColor))
            {
                fontBrush.Color = this.FontColor.FromHTMLWPF();
            }

            if (!string.IsNullOrWhiteSpace(this.FontOutlineColor))
            {
                fontOutline.Color = this.FontOutlineColor.FromHTMLWPF();
            }

            // Titleを描画する
            tb = this.SpellTitleTextBlock;
            tb.Text = string.IsNullOrWhiteSpace(this.SpellTitle) ? "　" : this.SpellTitle;
            tb.FontFamily = font.ToFontFamilyWPF();
            tb.FontSize = font.ToFontSizeWPF();
            tb.FontStyle = font.ToFontStyleWPF();
            tb.FontWeight = font.ToFontWeightWPF();
            tb.Fill = fontBrush;
            tb.Stroke = fontOutline;
            tb.StrokeThickness = 0.2d;

            // リキャスト時間を描画する
            tb = this.RecastTimeTextBlock;
            tb.Text = this.RecastTime > 0 ?
                this.RecastTime.ToString("N1") :
                this.IsReverse ? "Over" : "Ready";
            tb.FontFamily = font.ToFontFamilyWPF();
            tb.FontSize = font.ToFontSizeWPF();
            tb.FontStyle = font.ToFontStyleWPF();
            tb.FontWeight = font.ToFontWeightWPF();
            tb.Fill = fontBrush;
            tb.Stroke = fontOutline;
            tb.StrokeThickness = 0.2d;

            // ProgressBarを描画する
            var foreBrush = new SolidColorBrush(Settings.Default.ProgressBarColor.ToWPF());
            if (!string.IsNullOrWhiteSpace(this.BarColor))
            {
                foreBrush.Color = this.BarColor.FromHTMLWPF();
            }

            var backBrush = new SolidColorBrush(foreBrush.Color.ChangeBrightness(0.4d));

            var foreRect = new Rectangle();
            foreRect.Stroke = foreBrush;
            foreRect.Fill = foreBrush;
            foreRect.Width = this.IsReverse ?
                (double)(this.BarWidth * (1.0d - this.Progress)) :
                (double)(this.BarWidth * this.Progress);
            foreRect.Height = this.BarHeight;
            foreRect.RadiusX = 2.0d;
            foreRect.RadiusY = 2.0d;
            Canvas.SetLeft(foreRect, 0);
            Canvas.SetTop(foreRect, 0);

            var backRect = new Rectangle();
            backRect.Stroke = backBrush;
            backRect.Fill = backBrush;
            backRect.Width = this.BarWidth;
            backRect.Height = foreRect.Height;
            backRect.RadiusX = 2.0d;
            backRect.RadiusY = 2.0d;
            Canvas.SetLeft(backRect, 0);
            Canvas.SetTop(backRect, 0);

            var outlineBrush = new SolidColorBrush(Settings.Default.ProgressBarOutlineColor.ToWPF());
            if (!string.IsNullOrWhiteSpace(this.BarOutlineColor))
            {
                outlineBrush.Color = this.BarOutlineColor.FromHTMLWPF();
            }
            else
            {
                outlineBrush.Color = fontOutline.Color;
            }

            var outlineRect = new Rectangle();
            outlineRect.Stroke = outlineBrush;
            outlineRect.Width = backRect.Width;
            outlineRect.Height = foreRect.Height;
            outlineRect.RadiusX = 2.0d;
            outlineRect.RadiusY = 2.0d;
            Canvas.SetLeft(outlineRect, 0);
            Canvas.SetTop(outlineRect, 0);

            this.ProgressBarCanvas.Width = backRect.Width;
            this.ProgressBarCanvas.Height = backRect.Height;

            this.ProgressBarCanvas.Children.Clear();
            this.ProgressBarCanvas.Children.Add(backRect);
            this.ProgressBarCanvas.Children.Add(foreRect);
            this.ProgressBarCanvas.Children.Add(outlineRect);

#if false
            sw.Stop();
            Debug.WriteLine("Spell Refresh -> " + sw.ElapsedMilliseconds.ToString("N0") + "ms");
#endif
        }
    }
}
