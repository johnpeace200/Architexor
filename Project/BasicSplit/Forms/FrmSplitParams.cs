using Architexor.BasicSplit.Base;
using System;
using System.Windows.Forms;
using Splitter = Architexor.BasicSplit.Splitters.Splitter;

namespace Architexor.BasicSplit.Forms
{
	public partial class FrmSplitParams : Form
	{
		public FrmSplitParams()
		{
			InitializeComponent();

			btnElementColor.BackColor = Settings.PreviewSettings.ElementColor;
			btnSplitLineColor.BackColor = Settings.PreviewSettings.SplitLineColor;
			btnOpeningColor.BackColor = Settings.PreviewSettings.OpeningColor;
			btnDimensionColor.BackColor = Settings.PreviewSettings.DimensionColor;
			btnPanelBoundaryColor.BackColor = Settings.PreviewSettings.PanelBoundaryColor;

			txtMinPanelWidth.Text = Settings.MinimumPanelWidth.ToString();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnElementColor_Click(object sender, EventArgs e)
		{
			if(colorDialog1.ShowDialog() == DialogResult.OK)
			{
				btnElementColor.BackColor = colorDialog1.Color;
			}
		}

		private void FrmSplitParams_Load(object sender, EventArgs e)
		{
		}

		private void btnOpeningColor_Click(object sender, EventArgs e)
		{
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				btnOpeningColor.BackColor = colorDialog1.Color;
			}
		}

		private void btnSplitLineColor_Click(object sender, EventArgs e)
		{
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				btnSplitLineColor.BackColor = colorDialog1.Color;
			}
		}

		private void btnDimensionColor_Click(object sender, EventArgs e)
		{
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				btnDimensionColor.BackColor = colorDialog1.Color;
			}
		}

		private void btnPanelBoundaryColor_Click(object sender, EventArgs e)
		{
			if (colorDialog1.ShowDialog() == DialogResult.OK)
			{
				btnPanelBoundaryColor.BackColor = colorDialog1.Color;
			}
		}

		private void FrmSplitParams_FormClosing(object sender, FormClosingEventArgs e)
		{
			Settings.PreviewSettings.ElementColor = btnElementColor.BackColor;
			Settings.PreviewSettings.SplitLineColor = btnSplitLineColor.BackColor;
			Settings.PreviewSettings.OpeningColor = btnOpeningColor.BackColor;
			Settings.PreviewSettings.DimensionColor = btnDimensionColor.BackColor;
			Settings.PreviewSettings.PanelBoundaryColor = btnPanelBoundaryColor.BackColor;

			if (int.TryParse(txtMinPanelWidth.Text, out int minPanelWidth))
				Settings.MinimumPanelWidth = minPanelWidth;
		}
	}
}
