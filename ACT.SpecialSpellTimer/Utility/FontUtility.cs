namespace ACT.SpecialSpellTimer.Utility
{
    using System.Windows;

    /// <summary>
    /// フォントに関するUtility
    /// </summary>
    public static class FontUtility
    {
        /// <summary>
        /// WPF向けFontFamilyに変換する
        /// </summary>
        /// <param name="font">Font</param>
        /// <returns>FontFamily</returns>
        public static System.Windows.Media.FontFamily ToFontFamilyWPF(
            this System.Drawing.Font font)
        {
            return new System.Windows.Media.FontFamily(font.Name);
        }

        /// <summary>
        /// WPF向けFontSizeに変換する
        /// </summary>
        /// <param name="font">Font</param>
        /// <returns>FontSize</returns>
        public static double ToFontSizeWPF(
            this System.Drawing.Font font)
        {
            return (double)font.Size / 72.0d * 96.0d;
        }

        /// <summary>
        /// WPF向けFontStyleに変換する
        /// </summary>
        /// <param name="font">Font</param>
        /// <returns>FontStyle</returns>
        public static System.Windows.FontStyle ToFontStyleWPF(
            this System.Drawing.Font font)
        {
            return (font.Style & System.Drawing.FontStyle.Italic) != 0 ? FontStyles.Italic : FontStyles.Normal;
        }

        /// <summary>
        /// WPF向けFontWeightに変換する
        /// </summary>
        /// <param name="font">Font</param>
        /// <returns>FontWeight</returns>
        public static System.Windows.FontWeight ToFontWeightWPF(
            this System.Drawing.Font font)
        {
            return (font.Style & System.Drawing.FontStyle.Bold) != 0 ? FontWeights.Bold : FontWeights.Normal;
        }
    }
}
