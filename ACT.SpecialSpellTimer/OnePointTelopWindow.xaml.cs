namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Windows;
    using System.Windows.Media;

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

            this.ShowInTaskbar = false;
            this.Topmost = true;

            this.Loaded += this.OnePointTelopWindow_Loaded;
            this.MouseLeftButtonDown += (s1, e1) => this.DragMove();
        }

        /// <summary>
        /// 表示するデータソース
        /// </summary>
        public SpellTimerDataSet.OnePointTelopRow DataSource
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
                this.Visibility = Visibility.Hidden;
                return;
            }

            // 透明度を設定する
            this.Opacity = (100d - Settings.Default.Opacity) / 100d;

            this.MessageTextBlock.Text = !Settings.Default.TelopAlwaysVisible ?
                this.DataSource.MessageReplaced.Replace(",", Environment.NewLine) :
                this.DataSource.Message.Replace(",", Environment.NewLine);

            var font = new System.Drawing.Font(
                this.DataSource.FontFamily,
                this.DataSource.FontSize,
                (System.Drawing.FontStyle)this.DataSource.FontStyle);

            this.MessageTextBlock.FontFamily = font.ToFontFamilyWPF();
            this.MessageTextBlock.FontSize = font.ToFontSizeWPF();
            this.MessageTextBlock.FontStyle = font.ToFontStyleWPF();
            this.MessageTextBlock.FontWeight = font.ToFontWeightWPF();
            this.MessageTextBlock.Foreground = new SolidColorBrush(this.DataSource.FontColor.FromHTML().ToWPF());
            this.MessageTextBlock.Background = new SolidColorBrush(this.DataSource.BackColor.FromHTML().ToWPF());
        }
    }
}
