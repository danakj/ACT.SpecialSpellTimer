namespace ACT.SpecialSpellTimer
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using ACT.SpecialSpellTimer.Properties;

    /// <summary>
    /// Windowの拡張メソッド
    /// </summary>
    public static partial class WindowExtension
    {
        /// <summary>
        /// オーバーレイとして表示する
        /// </summary>
        /// <param name="x">Window</param>
        public static void ShowOverlay(
            this Window x)
        {
            var targetOpacity = (100d - Settings.Default.Opacity) / 100d;
            x.Opacity = targetOpacity;
        }

        /// <summary>
        /// オーバーレイとして非表示にする
        /// </summary>
        /// <param name="x">Window</param>
        public static void HideOverlay(
            this Window x)
        {
            x.Opacity = 0;
        }

        /// <summary>
        /// Brushを生成する
        /// </summary>
        /// <param name="x">Window</param>
        /// <param name="brush">今のBrush</param>
        /// <param name="color">カラー</param>
        /// <returns>Brush</returns>
        public static SolidColorBrush CreateBrush(
            this Window x,
            SolidColorBrush brush,
            Color color)
        {
            if (brush == null || brush.Color != color)
            {
                brush = new SolidColorBrush(color);
                brush.Freeze();
            }

            return brush;
        }

        /// <summary>
        /// Brushを生成する
        /// </summary>
        /// <param name="x">Window</param>
        /// <param name="brush">今のBrush</param>
        /// <param name="color">カラー</param>
        /// <returns>Brush</returns>
        public static SolidColorBrush CreateBrush(
            this UserControl x,
            SolidColorBrush brush,
            Color color)
        {
            if (brush == null || brush.Color != color)
            {
                brush = new SolidColorBrush(color);
                brush.Freeze();
            }

            return brush;
        }
    }
}
