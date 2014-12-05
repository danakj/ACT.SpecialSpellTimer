namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Windows.Forms;

    public partial class VisualSettingControlBackgoundColorForm : Form
    {
        public int Alpha { get; set; }

        public VisualSettingControlBackgoundColorForm()
        {
            this.InitializeComponent();

            this.OpacityNumericUpDown.ValueChanged += (s1, e1) =>
            {
                this.AlphaRateLabel.Text = 
                    (this.OpacityNumericUpDown.Value / 255m * 100m).ToString("N0") + "%";
            };

            this.Load += (s, e) =>
            {
                this.OpacityNumericUpDown.Value = this.Alpha;
                this.OpacityNumericUpDown.Focus();
            };
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Alpha = (int)this.OpacityNumericUpDown.Value;
        }
    }
}
