namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// Configパネル オプション
    /// </summary>
    public partial class ConfigPanel
    {
        /// <summary>
        /// オプションのLoad
        /// </summary>
        private void LoadOption()
        {
            this.LoadSettingsOption();

            this.SwitchOverlayButton.Click += (s1, e1) =>
            {
                Settings.Default.OverlayVisible = !Settings.Default.OverlayVisible;
                Settings.Default.Save();
                this.LoadSettingsOption();

                if (Settings.Default.OverlayVisible)
                {
                    SpellTimerCore.Default.ActivatePanels();
                    OnePointTelopController.ActivateTelops();
                }
            };

            this.SwitchTelopButton.Click += (s1, e1) =>
            {
                Settings.Default.TelopAlwaysVisible = !Settings.Default.TelopAlwaysVisible;
                Settings.Default.Save();
                this.LoadSettingsOption();
            };

            Action action = new Action(() =>
            {
                if (Settings.Default.OverlayVisible)
                {
                    this.SwitchOverlayButton.Text =
                        "オーバーレイの表示スイッチ" + Environment.NewLine +
                        "現在の状態 -> ON";
                }
                else
                {
                    this.SwitchOverlayButton.Text =
                        "オーバーレイの表示スイッチ" + Environment.NewLine +
                        "現在の状態 -> OFF";
                }
            });

            this.OptionTabPage.MouseHover += (s1, e1) => action();
            this.SwitchOverlayButton.MouseHover += (s1, e1) => action();
        }

        /// <summary>
        /// 初期化 Button
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void ShokikaButton_Click(object sender, EventArgs e)
        {
            Settings.Default.Reset();
            Settings.Default.Save();

            PanelSettings.Default.SettingsTable.Clear();
            PanelSettings.Default.Save();

            foreach (var telop in OnePointTelopTable.Default.Table)
            {
                telop.Left = 10.0d;
                telop.Top = 10.0d;
            }

            OnePointTelopTable.Default.Save();

            this.LoadSettingsOption();
            SpellTimerCore.Default.LayoutPanels();
        }

        /// <summary>
        /// 適用する Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void TekiyoButton_Click(object sender, EventArgs e)
        {
            this.ApplySettingsOption();

            // Windowを一旦すべて閉じる
            SpellTimerCore.Default.ClosePanels();
            OnePointTelopController.CloseTelops();
        }

        /// <summary>
        /// オプション設定をロードする
        /// </summary>
        private void LoadSettingsOption()
        {
            if (Settings.Default.OverlayVisible)
            {
                this.SwitchOverlayButton.Text =
                    "オーバーレイの表示スイッチ" + Environment.NewLine +
                    "現在の状態 -> ON";
            }
            else
            {
                this.SwitchOverlayButton.Text =
                    "オーバーレイの表示スイッチ" + Environment.NewLine +
                    "現在の状態 -> OFF";
            }

            if (Settings.Default.TelopAlwaysVisible)
            {
                this.SwitchTelopButton.Text =
                    "テロップの表示スイッチ" + Environment.NewLine +
                    "現在の状態 -> 常に表示(位置調整向け)";
            }
            else
            {
                this.SwitchTelopButton.Text =
                    "テロップの表示スイッチ" + Environment.NewLine +
                    "現在の状態 -> 通常";
            }

            this.DefaultVisualSetting.BarSize = Settings.Default.ProgressBarSize;
            this.DefaultVisualSetting.BarColor = Settings.Default.ProgressBarColor;
            this.DefaultVisualSetting.BarOutlineColor = Settings.Default.ProgressBarOutlineColor;
            this.DefaultVisualSetting.TextFont = Settings.Default.Font;
            this.DefaultVisualSetting.FontColor = Settings.Default.FontColor;
            this.DefaultVisualSetting.FontOutlineColor = Settings.Default.FontOutlineColor;
            this.DefaultVisualSetting.BackgroundColor = Settings.Default.BackgroundColor;
            this.DefaultVisualSetting.RefreshSampleImage();
            
            this.OpacityNumericUpDown.Value = Settings.Default.Opacity;
            this.ClickThroughCheckBox.Checked = Settings.Default.ClickThroughEnabled;
            this.AutoSortCheckBox.Checked = Settings.Default.AutoSortEnabled;
            this.AutoSortReverseCheckBox.Checked = Settings.Default.AutoSortReverse;
            this.TimeOfHideNumericUpDown.Value = (decimal)Settings.Default.TimeOfHideSpell;
            this.RefreshIntervalNumericUpDown.Value = Settings.Default.RefreshInterval;
            this.EnabledPTPlaceholderCheckBox.Checked = Settings.Default.EnabledPartyMemberPlaceholder;
        }

        /// <summary>
        /// 設定を適用する
        /// </summary>
        private void ApplySettingsOption()
        {
            Settings.Default.ProgressBarSize = this.DefaultVisualSetting.BarSize;
            Settings.Default.ProgressBarColor = this.DefaultVisualSetting.BarColor;
            Settings.Default.ProgressBarOutlineColor = this.DefaultVisualSetting.BarOutlineColor;
            Settings.Default.Font = this.DefaultVisualSetting.TextFont;
            Settings.Default.FontColor = this.DefaultVisualSetting.FontColor;
            Settings.Default.FontOutlineColor = this.DefaultVisualSetting.FontOutlineColor;
            Settings.Default.BackgroundColor = this.DefaultVisualSetting.BackgroundColor;

            Settings.Default.Opacity = (int)this.OpacityNumericUpDown.Value;
            Settings.Default.ClickThroughEnabled = this.ClickThroughCheckBox.Checked;
            Settings.Default.AutoSortEnabled = this.AutoSortCheckBox.Checked;
            Settings.Default.AutoSortReverse = this.AutoSortReverseCheckBox.Checked;
            Settings.Default.TimeOfHideSpell = (double)this.TimeOfHideNumericUpDown.Value;
            Settings.Default.RefreshInterval = (long)this.RefreshIntervalNumericUpDown.Value;
            Settings.Default.EnabledPartyMemberPlaceholder = this.EnabledPTPlaceholderCheckBox.Checked;

            // 設定を保存する
            Settings.Default.Save();
        }
    }
}
