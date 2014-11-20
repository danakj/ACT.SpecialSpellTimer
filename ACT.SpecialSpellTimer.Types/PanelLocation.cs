namespace ACT.SpecialSpellTimer.Types
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Panelの位置
    /// </summary>
    [Serializable]
    public class PanelLocation
    {
        /// <summary>
        /// Panel名
        /// </summary>
        public string PanelName { get; set; }

        /// <summary>
        /// Panelの位置
        /// </summary>
        public Point Location { get; set; }
    }
}
