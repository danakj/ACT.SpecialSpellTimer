namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Reflection;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Properties;
    using Advanced_Combat_Tracker;

    /// <summary>
    /// SpecialSpellTimer Plugin
    /// </summary>
    public class SpecialSpellTimerPlugin : IActPluginV1
    {
        /// <summary>
        /// 設定パネル
        /// </summary>
        public static ConfigPanel ConfigPanel { get; private set; }

        /// <summary>
        /// 表示切り替えボタン
        /// </summary>
        private static Button SwitchVisibleButton { get; set; }

        /// <summary>
        /// 自身の場所
        /// </summary>
        public static string Location
        {
            get;
            private set;
        }

        /// <summary>
        /// プラグインステータス表示ラベル
        /// </summary>
        private Label PluginStatusLabel
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public SpecialSpellTimerPlugin()
        {
            // このDLLの配置場所とACT標準のPluginディレクトリも解決の対象にする
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                try
                {
                    var asm = new AssemblyName(e.Name);

                    var plugin = ActGlobals.oFormActMain.PluginGetSelfData(this);
                    if (plugin != null)
                    {
                        var thisDirectory = plugin.pluginFile.DirectoryName;
                        var path1 = Path.Combine(thisDirectory, asm.Name + ".dll");
                        if (File.Exists(path1))
                        {
                            return Assembly.LoadFrom(path1);
                        }
                    }

                    var pluginDirectory = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        @"Advanced Combat Tracker\Plugins");

                    var path = Path.Combine(pluginDirectory, asm.Name + ".dll");

                    if (File.Exists(path))
                    {
                        return Assembly.LoadFrom(path);
                    }
                }
                catch (Exception ex)
                {
                    ActGlobals.oFormActMain.WriteExceptionLog(
                        ex,
                        Utility.Translate.Get("ACTAssemblyError"));
                }

                return null;
            };
        }

        /// <summary>
        /// 初期化する
        /// </summary>
        /// <param name="pluginScreenSpace">Pluginタブ</param>
        /// <param name="pluginStatusText">Pluginステータスラベル</param>
        void IActPluginV1.InitPlugin(
            TabPage pluginScreenSpace,
            Label pluginStatusText)
        {
            try
            {
                pluginScreenSpace.Text = "SpecialSpellTimer(スペスペ)";
                this.PluginStatusLabel = pluginStatusText;

                // アップデートを確認する
                this.Update();

                // 自身の場所を格納しておく
                var plugin = ActGlobals.oFormActMain.PluginGetSelfData(this);
                if (plugin != null)
                {
                    SpecialSpellTimerPlugin.Location = plugin.pluginFile.DirectoryName;
                }

                // 設定Panelを追加する
                ConfigPanel = new ConfigPanel();
                pluginScreenSpace.Controls.Add(ConfigPanel);
                ConfigPanel.Size = pluginScreenSpace.Size;
                ConfigPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

                // 設定ファイルのバックアップを作成する
                SpellTimerTable.Backup();
                OnePointTelopTable.Default.Backup();
                PanelSettings.Default.Backup();

                // 本体を開始する
                SpellTimerCore.Default.Begin();

                this.SetSwitchVisibleButton();
                this.PluginStatusLabel.Text = "Plugin Started";
            }
            catch (Exception ex)
            {
                ActGlobals.oFormActMain.WriteExceptionLog(
                    ex,
                    Utility.Translate.Get("ACTPluginStartError"));

                this.PluginStatusLabel.Text = "Plugin Initialize Error";
            }
        }

        /// <summary>
        /// 後片付けをする
        /// </summary>
        void IActPluginV1.DeInitPlugin()
        {
            try
            {
                SpellTimerCore.Default.End();
                this.RemoveSwitchVisibleButton();
                this.PluginStatusLabel.Text = "Plugin Exited";
            }
            catch (Exception ex)
            {
                ActGlobals.oFormActMain.WriteExceptionLog(
                    ex,
                    Utility.Translate.Get("ACTPluginStopError"));

                this.PluginStatusLabel.Text = "Plugin Exited Error";
            }
        }

        /// <summary>
        /// 表示切り替えボタンを配置する
        /// </summary>
        private void SetSwitchVisibleButton()
        {
            var changeColor = new Action<Button>((button) =>
            {
                if (Settings.Default.OverlayVisible)
                {
                    button.BackColor = Color.OrangeRed;
                    button.ForeColor = Color.WhiteSmoke;
                }
                else
                {
                    button.BackColor = SystemColors.Control;
                    button.ForeColor = Color.Black;
                }
            });

            SwitchVisibleButton = new Button();
            SwitchVisibleButton.Name = "SpecialSpellTimerSwitchVisibleButton";
            SwitchVisibleButton.Size = new Size(90, 24);
            SwitchVisibleButton.Text = Utility.Translate.Get("SupeSupe");
            SwitchVisibleButton.TextAlign = ContentAlignment.MiddleCenter;
            SwitchVisibleButton.UseVisualStyleBackColor = true;
            SwitchVisibleButton.Click += (s, e) =>
            {
                var button = s as Button;

                Settings.Default.OverlayVisible = !Settings.Default.OverlayVisible;
                Settings.Default.Save();

                SpellTimerCore.Default.ClosePanels();
                OnePointTelopController.CloseTelops();

                FF14PluginHelper.RefreshPlayer();
                LogBuffer.RefreshPTList();
                LogBuffer.RefreshPetID();

                if (Settings.Default.OverlayVisible)
                {
                    SpellTimerCore.Default.ActivatePanels();
                    OnePointTelopController.ActivateTelops();
                }

                changeColor(s as Button);
            };

            changeColor(SwitchVisibleButton);

            ActGlobals.oFormActMain.Resize += this.oFormActMain_Resize;
            ActGlobals.oFormActMain.Controls.Add(SwitchVisibleButton);
            ActGlobals.oFormActMain.Controls.SetChildIndex(SwitchVisibleButton, 1);

            this.oFormActMain_Resize(this, null);
        }

        /// <summary>
        /// 表示切り替えボタンを除去する
        /// </summary>
        private void RemoveSwitchVisibleButton()
        {
            if (SwitchVisibleButton != null)
            {
                ActGlobals.oFormActMain.Resize -= this.oFormActMain_Resize;
                ActGlobals.oFormActMain.Controls.Remove(SwitchVisibleButton);
            }
        }

        /// <summary>
        /// ACTメインフォーム Resize
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void oFormActMain_Resize(object sender, EventArgs e)
        {
            SwitchVisibleButton.Location = new Point(
                ActGlobals.oFormActMain.Width - 533,
                0);
        }

        /// <summary>
        /// アップデートを行う
        /// </summary>
        private void Update()
        {
            if ((DateTime.Now - Settings.Default.LastUpdateDateTime).TotalHours >= 6d)
            {
                var message = UpdateChecker.Update();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    ActGlobals.oFormActMain.WriteExceptionLog(
                        new Exception(),
                        message);
                }

                Settings.Default.LastUpdateDateTime = DateTime.Now;
                Settings.Default.Save();
            }
        }
    }
}
