namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using ACT.SpecialSpellTimer.Properties;

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
                }
            };

            this.SwitchTelopButton.Click += (s1, e1) =>
            {
                Settings.Default.TelopAlwaysVisible = !Settings.Default.TelopAlwaysVisible;
                Settings.Default.Save();
                this.LoadSettingsOption();
            };
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
        }

        /// <summary>
        /// BarColor Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void BarColorButton_Click(object sender, EventArgs e)
        {
            this.ColorDialog.Color = this.PreviewLabel.BackColor;
            if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
            {
                this.PreviewLabel.BackColor = this.ColorDialog.Color;
            }
        }

        /// <summary>
        /// Font Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void FontButton_Click(object sender, EventArgs e)
        {
            this.FontDialog.Font = this.PreviewLabel.Font;
            if (this.FontDialog.ShowDialog(this) != DialogResult.Cancel)
            {
                this.PreviewLabel.Font = this.FontDialog.Font;
            }
        }

        /// <summary>
        /// FontColor Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void FontColorButton_Click(object sender, EventArgs e)
        {
            this.ColorDialog.Color = this.PreviewLabel.ForeColor;
            if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
            {
                this.PreviewLabel.ForeColor = this.ColorDialog.Color;
            }
        }

        /// <summary>
        /// オプション設定をロードする
        /// </summary>
        private void LoadSettingsOption()
        {
            if (Settings.Default.OverlayVisible)
            {
                this.SwitchOverlayButton.Text = 
                    "スペルリストの表示スイッチ" + Environment.NewLine +
                    "現在の状態 -> ON";
            }
            else
            {
                this.SwitchOverlayButton.Text =
                    "スペルリストの表示スイッチ" + Environment.NewLine +
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

            this.BarWidthNumericUpDown.Value = Settings.Default.ProgressBarSize.Width;
            this.BarHeightNumericUpDown.Value = Settings.Default.ProgressBarSize.Height;
            this.PreviewLabel.BackColor = Settings.Default.ProgressBarColor;
            this.PreviewLabel.Font = Settings.Default.Font;
            this.PreviewLabel.ForeColor = Settings.Default.FontColor;
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
            Settings.Default.ProgressBarSize = new Size(
                (int)this.BarWidthNumericUpDown.Value,
                (int)this.BarHeightNumericUpDown.Value);
            Settings.Default.ProgressBarColor = this.PreviewLabel.BackColor;
            Settings.Default.Font = this.PreviewLabel.Font;
            Settings.Default.FontColor = this.PreviewLabel.ForeColor;
            Settings.Default.Opacity = (int)this.OpacityNumericUpDown.Value;
            Settings.Default.ClickThroughEnabled = this.ClickThroughCheckBox.Checked;
            Settings.Default.AutoSortEnabled = this.AutoSortCheckBox.Checked;
            Settings.Default.AutoSortReverse = this.AutoSortReverseCheckBox.Checked;
            Settings.Default.TimeOfHideSpell = (double)this.TimeOfHideNumericUpDown.Value;
            Settings.Default.RefreshInterval = (long)this.RefreshIntervalNumericUpDown.Value;
            Settings.Default.EnabledPartyMemberPlaceholder = this.EnabledPTPlaceholderCheckBox.Checked;

            // 設定を保存する
            Settings.Default.Save();

            // Windowを一旦すべて閉じる
            SpellTimerCore.Default.ClosePanels();
            OnePointTelopController.CloseTelops();
        }
    }
}
