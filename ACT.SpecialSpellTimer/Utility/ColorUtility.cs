namespace ACT.SpecialSpellTimer.Utility
{
    /// <summary>
    /// Colorに関するUtility
    /// </summary>
    public static class ColorUtility
    {
        /// <summary>
        /// WPF向けのColorに変換する
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>WPF向けカラー</returns>
        public static System.Windows.Media.Color ToWPF(
            this System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(
                color.A,
                color.R,
                color.G,
                color.B);
        }

        /// <summary>
        /// HTMLColorに変換する
        /// </summary>
        /// <param name="color">Color</param>
        /// <returns>HTMLカラー</returns>
        public static string ToHTML(
            this System.Drawing.Color color)
        {
            return System.Drawing.ColorTranslator.ToHtml(color);
        }

        /// <summary>
        /// HTMLからカラーに
        /// </summary>
        /// <param name="color">HTMLカラー</param>
        /// <returns>カラー</returns>
        public static System.Drawing.Color FromHTML(
            this string color)
        {
            return System.Drawing.ColorTranslator.FromHtml(color);
        }

        /// <summary>
        /// HTMLからカラーに
        /// </summary>
        /// <param name="color">HTMLカラー</param>
        /// <returns>カラー</returns>
        public static System.Windows.Media.Color FromHTMLWPF(
            this string color)
        {
            return color.FromHTML().ToWPF();
        }

        /// <summary>
        /// 明るさを変更する
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="brightness">明るさ</param>
        /// <returns>Color</returns>
        public static System.Drawing.Color ChangeBrightness(
            this System.Drawing.Color color,
            double brightness)
        {
            int r = (int)(color.R * brightness);
            int g = (int)(color.G * brightness);
            int b = (int)(color.B * brightness);

            if (r < 0)
            {
                r = 0;
            }

            if (r > 255)
            {
                r = 255;
            }

            if (g < 0)
            {
                g = 0;
            }

            if (g > 255)
            {
                g = 255;
            }

            if (b < 0)
            {
                b = 0;
            }

            if (b > 255)
            {
                b = 255;
            }

            return System.Drawing.Color.FromArgb(
                color.A,
                (byte)r,
                (byte)g,
                (byte)b);
        }

        /// <summary>
        /// 明るさを変更する
        /// </summary>
        /// <param name="color">Color</param>
        /// <param name="brightness">明るさ</param>
        /// <returns>Color</returns>
        public static System.Windows.Media.Color ChangeBrightness(
            this System.Windows.Media.Color color,
            double brightness)
        {
            int r = (int)(color.R * brightness);
            int g = (int)(color.G * brightness);
            int b = (int)(color.B * brightness);

            if (r < 0)
            {
                r = 0;
            }

            if (r > 255)
            {
                r = 255;
            }

            if (g < 0)
            {
                g = 0;
            }

            if (g > 255)
            {
                g = 255;
            }

            if (b < 0)
            {
                b = 0;
            }

            if (b > 255)
            {
                b = 255;
            }

            return System.Windows.Media.Color.FromArgb(
                color.A,
                (byte)r,
                (byte)g,
                (byte)b);
        }
    }
}
