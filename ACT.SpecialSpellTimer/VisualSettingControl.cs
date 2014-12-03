namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml.Serialization;

    using ACT.SpecialSpellTimer.Properties;
    using ACT.SpecialSpellTimer.Utility;

    /// <summary>
    /// 見た目設定用コントロール
    /// </summary>
    public partial class VisualSettingControl : UserControl
    {
        private bool barEnabled;

        public VisualSettingControl()
        {
            this.InitializeComponent();

            this.TextFont = Settings.Default.Font;
            this.FontColor = Settings.Default.FontColor;
            this.FontOutlineColor = Settings.Default.FontOutlineColor;
            this.BarColor = Settings.Default.ProgressBarColor;
            this.BarOutlineColor = Settings.Default.ProgressBarOutlineColor;
            this.BarSize = Settings.Default.ProgressBarSize;

            this.BarEnabled = true;

            this.Load += this.VisualSettingControl_Load;
        }

        private void VisualSettingControl_Load(object sender, EventArgs e)
        {

            var asm = Assembly.GetEntryAssembly();
            if (asm != null)
            {
                var colorDirectory = Path.Combine(
                    asm.Location,
                    @"resources\color");

                if (Directory.Exists(colorDirectory))
                {
                    this.OpenFileDialog.InitialDirectory = colorDirectory;
                    this.SaveFileDialog.InitialDirectory = colorDirectory;
                }
            }

            this.WidthNumericUpDown.Value = this.BarSize.Width;
            this.HeightNumericUpDown.Value = this.BarSize.Height;

            this.RefreshSampleImage();

            this.ChangeFontItem.Click += (s1, e1) =>
            {
                this.FontDialog.Font = this.TextFont;
                if (this.FontDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.TextFont = this.FontDialog.Font;
                    this.RefreshSampleImage();
                }
            };

            this.ChangeFontColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.FontColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.FontColor = this.ColorDialog.Color;
                    this.RefreshSampleImage();
                }
            };

            this.ChangeFontOutlineColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.FontOutlineColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.FontOutlineColor = this.ColorDialog.Color;
                    this.RefreshSampleImage();
                }
            };

            this.ChangeBarColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.BarColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.BarColor = this.ColorDialog.Color;
                    this.RefreshSampleImage();
                }
            };

            this.ChangeBarOutlineColorItem.Click += (s1, e1) =>
            {
                this.ColorDialog.Color = this.BarOutlineColor;
                if (this.ColorDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    this.BarOutlineColor = this.ColorDialog.Color;
                    this.RefreshSampleImage();
                }
            };

            this.WidthNumericUpDown.ValueChanged += (s1, e1) =>
            {
                this.RefreshSampleImage();
            };

            this.HeightNumericUpDown.ValueChanged += (s1, e1) =>
            {
                this.RefreshSampleImage();
            };

            this.LoadColorSetItem.Click += (s1, e1) =>
            {
                if (this.OpenFileDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    using (var sr = new StreamReader(this.OpenFileDialog.FileName, new UTF8Encoding(false)))
                    {
                        var xs = new XmlSerializer(typeof(ColorSet));
                        var colorSet = xs.Deserialize(sr) as ColorSet;
                        if (colorSet != null)
                        {
                            this.FontColor = colorSet.FontColor.FromHTML();
                            this.FontOutlineColor = colorSet.FontOutlineColor.FromHTML();
                            this.BarColor = colorSet.BarColor.FromHTML();
                            this.BarOutlineColor = colorSet.BarOutlineColor.FromHTML();

                            this.RefreshSampleImage();

                            // カラーパレットに登録する
                            this.ColorDialog.CustomColors = new int[]
                            {
                                ColorTranslator.ToOle(colorSet.FontColor.FromHTML()),
                                ColorTranslator.ToOle(colorSet.FontOutlineColor.FromHTML()),
                                ColorTranslator.ToOle(colorSet.BarColor.FromHTML()),
                                ColorTranslator.ToOle(colorSet.BarOutlineColor.FromHTML()),
                            };
                        }
                    }
                }
            };

            this.SaveColorSetItem.Click += (s1, e1) =>
            {
                if (string.IsNullOrWhiteSpace(this.SaveFileDialog.FileName))
                {
                    this.SaveFileDialog.FileName = "スペスペ配色セット.xml";
                }

                if (this.SaveFileDialog.ShowDialog(this) != DialogResult.Cancel)
                {
                    var colorSet = new ColorSet()
                    {
                        FontColor = this.FontColor.ToHTML(),
                        FontOutlineColor = this.FontOutlineColor.ToHTML(),
                        BarColor = this.BarColor.ToHTML(),
                        BarOutlineColor = this.BarOutlineColor.ToHTML()
                    };

                    using (var sw = new StreamWriter(this.SaveFileDialog.FileName, false, new UTF8Encoding(false)))
                    {
                        var xs = new XmlSerializer(typeof(ColorSet));
                        xs.Serialize(sw, colorSet);
                    }
                }
            };
        }

        public Font TextFont { get; set; }

        public Color FontColor { get; set; }

        public Color FontOutlineColor { get; set; }

        public Color BarColor { get; set; }

        public Color BarOutlineColor { get; set; }

        public Size BarSize 
        { 
            get
            {
                return new Size(
                    (int)this.WidthNumericUpDown.Value,
                    (int)this.HeightNumericUpDown.Value);
            }
            set 
            {
                this.WidthNumericUpDown.Value = value.Width;
                this.HeightNumericUpDown.Value = value.Height;
            }
        }

        public bool BarEnabled
        {
            get { return this.barEnabled; }
            set
            {
                this.barEnabled = value;

                if (this.barEnabled)
                {
                    this.WidthNumericUpDown.Visible = true;
                    this.HeightNumericUpDown.Visible = true;
                    this.BarSizeLabel.Visible = true;
                    this.BarSizeXLabel.Visible = true;
                    this.ChangeBarColorItem.Enabled = true;
                    this.ChangeBarOutlineColorItem.Enabled = true;
                }
                else
                {
                    this.WidthNumericUpDown.Visible = false;
                    this.HeightNumericUpDown.Visible = false;
                    this.BarSizeLabel.Visible = false;
                    this.BarSizeXLabel.Visible = false;
                    this.ChangeBarColorItem.Enabled = false;
                    this.ChangeBarOutlineColorItem.Enabled = false;
                }

                this.RefreshSampleImage();
            }
        }

        /// <summary>
        /// サンプルイメージを描画する
        /// </summary>
        public void RefreshSampleImage()
        {
            var font = this.TextFont;
            var fontColor = this.FontColor;
            var fontOutlineColor = this.FontOutlineColor;
            var barColor = this.BarColor;
            var barOutlineColor = this.BarOutlineColor;
            var barSize = this.BarEnabled ?
                this.BarSize :
                this.SamplePictureBox.Size;
            var barLocation = new Point(
                (this.SamplePictureBox.Width / 2) - (barSize.Width / 2),
                this.SamplePictureBox.Height - barSize.Height - 12);

            var bmp = new Bitmap(this.SamplePictureBox.Width, this.SamplePictureBox.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

                if (this.BarEnabled)
                {
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

                    // 後片付け
                    backBrush.Dispose();
                    foreBrush.Dispose();
                    outlinePen.Dispose();
                }

                // フォントのペンを生成する
                var fontBrush = new SolidBrush(fontColor);
                var fontOutlinePen = new Pen(fontOutlineColor, 0.2f);
                var fontRect = new Rectangle(
                    barLocation.X,
                    6,
                    barSize.Width,
                    barLocation.Y - 2);

                if (!this.BarEnabled)
                {
                    fontRect = new Rectangle(
                        barLocation.X,
                        6,
                        barSize.Width,
                        this.SamplePictureBox.Height - 6);
                }

                // フォントを描く
                var spellSf = new StringFormat()
                {
                    Alignment = StringAlignment.Near
                };

                var recastSf = new StringFormat()
                {
                    Alignment = StringAlignment.Far
                };

                var telopSf = new StringFormat()
                {
                    Alignment = StringAlignment.Center
                };

                var path = new GraphicsPath();

                if (this.BarEnabled)
                {
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
                }
                else
                {
                    path.AddString(
                        "サンプルテロップ",
                        font.FontFamily,
                        (int)font.Style,
                        (float)font.ToFontSizeWPF(),
                        fontRect,
                        telopSf);
                }

                g.FillPath(fontBrush, path);
                g.DrawPath(fontOutlinePen, path);

                // まとめて後片付け
                fontOutlinePen.Dispose();
                path.Dispose();
                spellSf.Dispose();
                recastSf.Dispose();
                telopSf.Dispose();
            }

            if (this.SamplePictureBox.Image != null)
            {
                this.SamplePictureBox.Image.Dispose();
                this.SamplePictureBox.Image = null;
            }

            this.SamplePictureBox.Image = bmp;
        }
    }

    [Serializable]
    public class ColorSet
    {
        public string FontColor { get; set; }
        public string FontOutlineColor { get; set; }
        public string BarColor { get; set; }
        public string BarOutlineColor { get; set; }
    }
}
