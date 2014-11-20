namespace ACT.SpecialSpellTimer.Types
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Panelの位置のコレクション
    /// </summary>
    [Serializable]
    public class PanelLocations
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PanelLocations()
        {
            this.Locations = new List<PanelLocation>();
        }

        /// <summary>
        /// Panelの位置のリスト
        /// </summary>
        public List<PanelLocation> Locations { get; set; }
    }
}
