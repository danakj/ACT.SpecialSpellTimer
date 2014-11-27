namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Windowの拡張メソッド
    /// </summary>
    public static partial class WindowExtension
    {
        public const int WS_EX_TRANSPARENT = 0x00000020;
        public const int GWL_EXSTYLE = (-20);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        /// <summary>
        /// Click透過させる
        /// </summary>
        /// <param name="x">対象のWindow</param>
        public static void ToTransparentWindow(this Window x)
        {
            x.SourceInitialized += (s1, e1) =>
            {
                // Get this window's handle
                IntPtr hwnd = new WindowInteropHelper(x).Handle;

                // Change the extended window style to include WS_EX_TRANSPARENT
                int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);

                SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
            };
        }
    }
}
