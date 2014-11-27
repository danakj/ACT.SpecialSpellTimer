namespace ACT.SpecialSpellTimer
{
    using System.Windows;

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
            x.Opacity = (100d - Settings.Default.Opacity) / 100d;
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
    }
}
