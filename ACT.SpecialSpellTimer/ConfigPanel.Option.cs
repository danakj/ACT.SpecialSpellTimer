namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
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

            // 配色を統一する
            this.HaishokuToitsuButton.Click += (s1, e1) =>
            {
                if (MessageBox.Show(
                    this,
                    "スペルとテロップの配色を統一しますか？",
                    "ACT.SpecialSpellTimer",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.OK)
                {
                    this.ApplySettingsOption();

                    foreach (var spell in SpellTimerTable.Table)
                    {
                        spell.FontColor = Settings.Default.FontColor.ToHTML();
                        spell.FontOutlineColor = Settings.Default.FontOutlineColor.ToHTML();
                        spell.BarColor = Settings.Default.ProgressBarColor.ToHTML();
                        spell.BarOutlineColor = Settings.Default.ProgressBarOutlineColor.ToHTML();
                    }

                    foreach (var telop in OnePointTelopTable.Default.Table)
                    {
                        telop.FontColor = Settings.Default.FontColor.ToHTML();
                        telop.FontOutlineColor = Settings.Default.FontOutlineColor.ToHTML();
                    }

                    SpellTimerTable.Save();
                    OnePointTelopTable.Default.Save();

                    // Windowを一度閉じてリフレッシュする
                    SpellTimerCore.Default.ClosePanels();
                    OnePointTelopController.CloseTelops();
                }
            };

            this.BarWidthNumericUpDown.ValueChanged += (s1, e1) => this.DrawSampleImage();
            this.BarHeightNumericUpDown.ValueChanged += (s1, e1) => this.DrawSampleImage();
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

                // サンプル画像を描画する
                this.DrawSampleImage();
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

                // サンプル画像を描画する
                this.DrawSampleImage();
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

                // サンプル画像を描画する
                this.DrawSampleImage();
            }
        }

        /// <summary>
        /// BarOutlineColor Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void BarOutlineColorButton_Click(object sender, EventArgs e)
        {
            this.ColorDialog.Color = this.BarOutlineColorButton.BackColor;
            if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
            {
                this.BarOutlineColorButton.BackColor = this.ColorDialog.Color;

                // サンプル画像を描画する
                this.DrawSampleImage();
            }
        }

        /// <summary>
        /// FontOutlineColor Click
        /// </summary>
        /// <param name="sender">イベント発生元</param>
        /// <param name="e">イベント引数</param>
        private void FontOutlineColorButton_Click(object sender, EventArgs e)
        {
            this.ColorDialog.Color = this.FontOutlineColorButton.BackColor;
            if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
            {
                this.FontOutlineColorButton.BackColor = this.ColorDialog.Color;

                // サンプル画像を描画する
                this.DrawSampleImage();
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

            this.BarWidthNumericUpDown.Value = Settings.Default.ProgressBarSize.Width;
            this.BarHeightNumericUpDown.Value = Settings.Default.ProgressBarSize.Height;
            this.PreviewLabel.BackColor = Settings.Default.ProgressBarColor;
            this.PreviewLabel.Font = Settings.Default.Font;
            this.PreviewLabel.ForeColor = Settings.Default.FontColor;
            this.FontOutlineColorButton.BackColor = Settings.Default.FontOutlineColor;
            this.BarOutlineColorButton.BackColor = Settings.Default.ProgressBarOutlineColor;
            this.OpacityNumericUpDown.Value = Settings.Default.Opacity;
            this.ClickThroughCheckBox.Checked = Settings.Default.ClickThroughEnabled;
            this.AutoSortCheckBox.Checked = Settings.Default.AutoSortEnabled;
            this.AutoSortReverseCheckBox.Checked = Settings.Default.AutoSortReverse;
            this.TimeOfHideNumericUpDown.Value = (decimal)Settings.Default.TimeOfHideSpell;
            this.RefreshIntervalNumericUpDown.Value = Settings.Default.RefreshInterval;
            this.EnabledPTPlaceholderCheckBox.Checked = Settings.Default.EnabledPartyMemberPlaceholder;

            // サンプル画像を描画する
            this.DrawSampleImage();
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
            Settings.Default.FontOutlineColor = this.FontOutlineColorButton.BackColor;
            Settings.Default.ProgressBarOutlineColor = this.BarOutlineColorButton.BackColor;
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

        /// <summary>
        /// サンプルイメージを描画する
        /// </summary>
        private void DrawSampleImage()
        {
            var font = this.PreviewLabel.Font;
            var fontColor = this.PreviewLabel.ForeColor;
            var fontOutlineColor = this.FontOutlineColorButton.BackColor;
            var barColor = this.PreviewLabel.BackColor;
            var barOutlineColor = this.BarOutlineColorButton.BackColor;
            var barSize = new Size(
                (int)this.BarWidthNumericUpDown.Value,
                (int)this.BarHeightNumericUpDown.Value);
            var barLocation = new Point(
                (this.SamplePictureBox.Width / 2) - (barSize.Width / 2),
                this.SamplePictureBox.Height - barSize.Height - 25);

            var bmp = new Bitmap(this.SamplePictureBox.Width, this.SamplePictureBox.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                // バーの暗を描く
                var backRect = new Rectangle(barLocation, barSize);
                var backBrush = new SolidBrush(barColor.ChangeBrightness(0.4d));
                g.FillRectangle(backBrush, backRect);

                // バーの明を描く
                var foreRect = new Rectangle(barLocation, new Size((int)(barSize.Width * 0.6), barSize.Height));
                var foreBrush = new SolidBrush(barColor);
                g.FillRectangle(foreBrush, foreRect);

                // バーのアウトラインを描く
                var outlineRect = new Rectangle(barLocation, barSize);
                var outlinePen = new Pen(barOutlineColor, 1.0f);
                g.DrawRectangle(outlinePen, outlineRect);

                // フォントのペンを生成する
                var fontBrush = new SolidBrush(fontColor);
                var fontOutlinePen = new Pen(fontOutlineColor, 0.2f);
                var fontRect = new Rectangle(
                    barLocation.X,
                    22,
                    barSize.Width,
                    this.SamplePictureBox.Height - 22);

                // フォントを描く
                var spellSf = new StringFormat()
                {
                    Alignment = StringAlignment.Near
                };

                var recastSf = new StringFormat()
                {
                    Alignment = StringAlignment.Far
                };

                var path = new GraphicsPath();
                path.AddString(
                    "サンプルスペル",
                    font.FontFamily,
                    (int)font.Style,
                    (float)font.ToFontSizeWPF(),
                    fontRect,
                    spellSf);

                path.AddString(
                    "120.0",
                    font.FontFamily,
                    (int)font.Style,
                    (float)font.ToFontSizeWPF(),
                    fontRect,
                    recastSf);

                g.FillPath(fontBrush, path);
                g.DrawPath(fontOutlinePen, path);

                // まとめて後片付け
                backBrush.Dispose();
                foreBrush.Dispose();
                outlinePen.Dispose();
                fontOutlinePen.Dispose();
                path.Dispose();
                spellSf.Dispose();
                recastSf.Dispose();
            }

            if (this.SamplePictureBox.Image != null)
            {
                this.SamplePictureBox.Image.Dispose();
                this.SamplePictureBox.Image = null;
            }

            this.SamplePictureBox.Image = bmp;
        }
    }
}
