namespace ACT.SpecialSpellTimer.Utility
{
    using System;
    using System.Windows.Forms;

    using Advanced_Combat_Tracker;

    /// <summary>
    /// ActInvoker
    /// </summary>
    public static class ActInvoker
    {
        /// <summary>
        /// ACTメインフォームでInvokeする
        /// </summary>
        /// <param name="action">実行するアクション</param>
        public static void Invoke(Action action)
        {
            if (ActGlobals.oFormActMain != null &&
                ActGlobals.oFormActMain.IsHandleCreated &&
                !ActGlobals.oFormActMain.IsDisposed)
            {
                if (ActGlobals.oFormActMain.InvokeRequired)
                {
                    ActGlobals.oFormActMain.Invoke((MethodInvoker)delegate
                    {
                        action();
                    });
                }
                else
                {
                    action();
                }
            }
        }
    }
}
