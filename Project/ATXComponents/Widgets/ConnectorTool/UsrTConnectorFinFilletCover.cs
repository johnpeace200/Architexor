using Architexor.Models.ConnectorTool;
using System;
using System.Windows.Forms;

namespace Architexor.Widgets.ConnectorTool
{
	public partial class UsrTConnectorFinFilletCover : UserControl
	{
		public TConFinFCParam ConFinFCParam
		{
			get
			{
				return new TConFinFCParam()
				{
					Gap_FinPlate_Front = Gap_FinPlate_Front,
					Gap_FinPlate_Top = Gap_FinPlate_Top,
					Gap_FinPlate_Bottom = Gap_FinPlate_Bottom,
					Gap_FinPlate_Sides = Gap_FinPlate_Sides,
					BFilletNotch = ShowFilletNotch,
					Gap_Fillet_Top = GapFillTop,
					Gap_Fillet_Bottom = GapFillBottom,
					Gap_Fillet_Width = GapFillSide,
					Gap_Fillet_Depth = GapFillEnd,
					BCoverPlate = ShowCoverPlate,
					Gap_CoverBoard_Depth = Gap_CoverBoard_Depth,
					Gap_CoverBoard_Length = Gap_CoverBoard_Length,
					Gap_CoverBoard_Width = Gap_CoverBoard_Width
				};
			}
		}

		//	parameters of Fin plate notching
		public double Gap_FinPlate_Front
		{
			get
			{
				return Convert.ToDouble(txtNhSFinFrontGap.Text);
			}
			set
			{
				txtNhSFinFrontGap.Text = value.ToString();
			}
		}

		public double Gap_FinPlate_Top
		{
			get
			{
				return Convert.ToDouble(txtNhSFinTopGap.Text);
			}
			set
			{
				txtNhSFinTopGap.Text = value.ToString();
			}
		}

		public double Gap_FinPlate_Bottom
		{
			get
			{
				return Convert.ToDouble(txtNhSFinBtmGap.Text);
			}
			set
			{
				txtNhSFinBtmGap.Text = value.ToString();
			}
		}

		public double Gap_FinPlate_Sides
		{
			get
			{
				return Convert.ToDouble(txtNhSFinSideA.Text);
			}
			set
			{
				txtNhSFinSideA.Text = value.ToString();
			}
		}

		//	parameters of Fillet notch plate notching
		public bool ShowFilletNotch
		{
			get
			{
				return chkNhSWeldNch.Checked;
			}
			set
			{
				chkNhSWeldNch.Checked = value;
			}
		}

		public double GapFillTop
		{
			get
			{
				return Convert.ToDouble(txtNhSWNchTop.Text);
			}
			set
			{
				txtNhSWNchTop.Text = value.ToString();
			}
		}

		public double GapFillBottom
		{
			get
			{
				return Convert.ToDouble(txtNhSWNchBtm.Text);
			}
			set
			{
				txtNhSWNchBtm.Text = value.ToString();
			}
		}

		public double GapFillSide
		{
			get
			{
				return Convert.ToDouble(txtNhSWNchSide.Text);
			}
			set
			{
				txtNhSWNchSide.Text = value.ToString();
			}
		}

		public double GapFillEnd
		{
			get
			{
				return Convert.ToDouble(txtNhSWNchEnd.Text);
			}
			set
			{
				txtNhSWNchEnd.Text = value.ToString();
			}
		}

		//	parameters of Bottom cover plate notching
		public bool ShowCoverPlate
		{
			get
			{
				return chkNhSPlate.Checked;
			}
			set
			{
				chkNhSPlate.Checked = value;
			}
		}

		public double Gap_CoverBoard_Depth
		{
			get
			{
				return Convert.ToDouble(txtNhSPDepth.Text);
			}
			set
			{
				txtNhSPDepth.Text = value.ToString();
			}
		}

		public double Gap_CoverBoard_Length
		{
			get
			{
				return Convert.ToDouble(txtNhSPLength.Text);
			}
			set
			{
				txtNhSPLength.Text = value.ToString();
			}
		}

		public double Gap_CoverBoard_Width
		{
			get
			{
				return Convert.ToDouble(txtNhSPWidth.Text);
			}
			set
			{
				txtNhSPWidth.Text = value.ToString();
			}
		}

		public UsrTConnectorFinFilletCover()
		{
			InitializeComponent();
		}

		public void UpdateParameters(TConFinFCParam value)
		{
			Gap_FinPlate_Front = value.Gap_FinPlate_Front;
			Gap_FinPlate_Top = value.Gap_FinPlate_Top;
			Gap_FinPlate_Bottom = value.Gap_FinPlate_Bottom;
			Gap_FinPlate_Sides = value.Gap_FinPlate_Sides;
			ShowFilletNotch = value.BFilletNotch;
			GapFillTop = value.Gap_Fillet_Top;
			GapFillBottom = value.Gap_Fillet_Bottom;
			GapFillSide = value.Gap_Fillet_Width;
			GapFillEnd = value.Gap_Fillet_Depth;
			ShowCoverPlate = value.BCoverPlate;
			Gap_CoverBoard_Depth = value.Gap_CoverBoard_Depth;
			Gap_CoverBoard_Length = value.Gap_CoverBoard_Length;
			Gap_CoverBoard_Width = value.Gap_CoverBoard_Width;
		}

		private void TxtChangingParams(object sender, EventArgs e)
		{
			if (sender is TextBox textBox && textBox.Text == "")
				textBox.Text = "0";
		}

		private void ChkNhSWeldNch_CheckedChanged(object sender, EventArgs e)
		{
			picNhSWeld.Enabled = chkNhSWeldNch.Checked;
			txtNhSWNchTop.Enabled = chkNhSWeldNch.Checked;
			txtNhSWNchBtm.Enabled = chkNhSWeldNch.Checked;
			txtNhSWNchEnd.Enabled = chkNhSWeldNch.Checked;
			txtNhSWNchSide.Enabled = chkNhSWeldNch.Checked;
		}

		private void ChkNhSPlate_CheckedChanged(object sender, EventArgs e)
		{
			picNhSCover.Enabled = chkNhSPlate.Checked;
			txtNhSPDepth.Enabled = chkNhSPlate.Checked;
			txtNhSPLength.Enabled = chkNhSPlate.Checked;
			txtNhSPWidth.Enabled = chkNhSPlate.Checked;
		}
	}
}
