namespace ACT.SpecialSpellTimer
{
    using System;

    /// <summary>
    /// 戦闘ログ
    /// </summary>
    public class CombatLog
    {
        public CombatLog()
        {
            this.Raw = string.Empty;
            this.Actor = string.Empty;
            this.Action = string.Empty;
            this.Target = string.Empty;
            this.LogTypeName = string.Empty;
        }

        public DateTime TimeStamp { get; set; }
        public double TimeStampElapted { get; set; }
        public string Raw { get; set; }
        public CombatLogType LogType { get; set; }
        public string LogTypeName { get; set; }
        public string Actor { get; set; }
        public string Target { get; set; }
        public string Action { get; set; }
        public double CastTime { get; set; }
        public double Span { get; set; }
        public decimal HPRate { get; set; }
    }

    /// <summary>
    /// 戦闘ログの種類
    /// </summary>
    public enum CombatLogType
    {
        /// <summary>0:分析開始</summary>
        AnalyzeStart = 0,

        /// <summary>1:分析終了</summary>
        AnalyzeEnd = 1,
        
        /// <summary>2:詠唱開始</summary>
        CastStart = 2,
        
        /// <summary>3:アクション(発動)</summary>
        Action = 3,
        
        /// <summary>4:Add</summary>
        Added = 4,
        
        /// <summary>5:残HP率の記録</summary>
        HPRate = 5
    }
}
