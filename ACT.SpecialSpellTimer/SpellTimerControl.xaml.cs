namespace ACT.SpecialSpellTimer
{
    using System.Diagnostics;
    using System.Windows.Controls;
    using System.Windows.Media;

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

        /// <summary>Cacheされたフォント</summary>
        private System.Drawing.Font CachedFont { get; set; }

        /// <summary>
        /// 描画を更新する
        /// </summary>
        public void Refresh()
        {
#if false
            var sw = Stopwatch.StartNew();
#endif
            this.Width = this.BarWidth;

            // Brushを生成する
            var fontColor = string.IsNullOrWhiteSpace(this.FontColor) ?
                Settings.Default.FontColor.ToWPF() :
                this.FontColor.FromHTMLWPF();
            var fontOutlineColor = string.IsNullOrWhiteSpace(this.FontOutlineColor) ?
                Settings.Default.FontOutlineColor.ToWPF() :
                this.FontOutlineColor.FromHTMLWPF();
            var barColor = string.IsNullOrWhiteSpace(this.BarColor) ?
                Settings.Default.ProgressBarColor.ToWPF() :
                this.BarColor.FromHTMLWPF();
            var barBackColor = barColor.ChangeBrightness(0.4d);
            var barOutlineColor = string.IsNullOrWhiteSpace(this.BarOutlineColor) ?
                Settings.Default.ProgressBarOutlineColor.ToWPF() :
                this.BarOutlineColor.FromHTMLWPF();

            this.FontBrush = this.CreateBrush(this.FontBrush, fontColor);
            this.FontOutlineBrush = this.CreateBrush(this.FontOutlineBrush, fontOutlineColor);
            this.BarBrush = this.CreateBrush(this.BarBrush, barColor);
            this.BarBackBrush = this.CreateBrush(this.BarBackBrush, barBackColor);
            this.BarOutlineBrush = this.CreateBrush(this.BarOutlineBrush, barOutlineColor);

            // フォントを生成する
            if (this.CachedFont == null ||
                this.CachedFont.Name != this.TextFontFamily ||
                this.CachedFont.Size != this.TextFontSize ||
                this.CachedFont.Style != (System.Drawing.FontStyle)this.TextFontStyle)
            {
                this.CachedFont = new System.Drawing.Font(
                    this.TextFontFamily,
                    this.TextFontSize,
                    (System.Drawing.FontStyle)this.TextFontStyle);
            }

            var tb = default(OutlineTextBlock);
            var font = this.CachedFont;

            // Titleを描画する
            tb = this.SpellTitleTextBlock;
            var title = string.IsNullOrWhiteSpace(this.SpellTitle) ? "　" : this.SpellTitle;
            if (tb.Text != title)
            {
                tb.Text = title;
                tb.FontFamily = font.ToFontFamilyWPF();
                tb.FontSize = font.ToFontSizeWPF();
                tb.FontStyle = font.ToFontStyleWPF();
                tb.FontWeight = font.ToFontWeightWPF();
                tb.Fill = this.FontBrush;
                tb.Stroke = this.FontOutlineBrush;
                tb.StrokeThickness = 0.2d;
            }

            // リキャスト時間を描画する
            tb = this.RecastTimeTextBlock;
            var recast = this.RecastTime > 0 ?
                this.RecastTime.ToString("N1") :
                this.IsReverse ? "Over" : "Ready";
            if (tb.Text != recast)
            {
                tb.Text = recast;
                tb.FontFamily = font.ToFontFamilyWPF();
                tb.FontSize = font.ToFontSizeWPF();
                tb.FontStyle = font.ToFontStyleWPF();
                tb.FontWeight = font.ToFontWeightWPF();
                tb.Fill = this.FontBrush;
                tb.Stroke = this.FontOutlineBrush;
                tb.StrokeThickness = 0.2d;
            }

            // ProgressBarを描画する
            var foreRect = this.BarRectangle;
            foreRect.Stroke = this.BarBrush;
            foreRect.Fill = this.BarBrush;
            foreRect.Width = this.IsReverse ?
                (double)(this.BarWidth * (1.0d - this.Progress)) :
                (double)(this.BarWidth * this.Progress);
            foreRect.Height = this.BarHeight;
            foreRect.RadiusX = 2.0d;
            foreRect.RadiusY = 2.0d;
            Canvas.SetLeft(foreRect, 0);
            Canvas.SetTop(foreRect, 0);

            var backRect = this.BarBackRectangle;
            backRect.Stroke = this.BarBackBrush;
            backRect.Fill = this.BarBackBrush;
            backRect.Width = this.BarWidth;
            backRect.Height = foreRect.Height;
            backRect.RadiusX = 2.0d;
            backRect.RadiusY = 2.0d;
            Canvas.SetLeft(backRect, 0);
            Canvas.SetTop(backRect, 0);

            var outlineRect = this.BarOutlineRectangle;
            outlineRect.Stroke = this.BarOutlineBrush;
            outlineRect.Width = backRect.Width;
            outlineRect.Height = foreRect.Height;
            outlineRect.RadiusX = 2.0d;
            outlineRect.RadiusY = 2.0d;
            Canvas.SetLeft(outlineRect, 0);
            Canvas.SetTop(outlineRect, 0);

            // バーのエフェクトの色を設定する
            this.BarEffect.Color = this.BarBrush.Color.ChangeBrightness(1.05d);

            this.ProgressBarCanvas.Width = backRect.Width;
            this.ProgressBarCanvas.Height = backRect.Height;

#if false
            sw.Stop();
            Debug.WriteLine("Spell Refresh -> " + sw.ElapsedMilliseconds.ToString("N0") + "ms");
#endif
        }
    }
}
