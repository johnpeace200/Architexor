using Architexor.Models.ConnectorTool;
using System;
using System.Windows.Forms;

namespace Architexor.Widgets.ConnectorTool
{
	public partial class UsrTConnectorPosition : UserControl
	{
		//private bool _data;
		//public ButtonData Data
		//{
		//	get { return _data; }
		//	set
		//	{
		//		if (value != _data)
		//		{
		//			// Unhook previous events
		//			if (_data != null)
		//				_data.PropertyChanged -= HandleButtonDataPropertyChanged;
		//			// Set private field
		//			_data = value;
		//			// Hook new events
		//			if (_data != null)
		//				_data.PropertyChanged += HandleButtonDataPropertyChanged;
		//			// Immediate update  since we have a new ButtonData object
		//			if (_data != null)
		//				Update();
		//		}
		//	}
		//}

		//private void HandleButtonDataPropertyChanged(object sender, PropertyChangedEventArgs e)
		//{
		//	// Handle change in ButtonData
		//	Update();
		//}

		//private void Update()
		//{
		//	// Update...
		//}

		public bool HorizontalFixed
		{
			get { return chkPosWidthFixedToCentre.Checked; }
			set
			{
				if (value != chkPosWidthFixedToCentre.Checked)
				{
					chkPosWidthFixedToCentre.Checked = value;
				}
			}
		}

		public bool VerticalFixed {
			get { return chkPosFixedToSecondary.Checked; }
			set
			{
				if(value != chkPosFixedToSecondary.Checked)
				{
					chkPosFixedToSecondary.Checked = value;
				}
			}
		}

		public RecessCase RecessElement
		{
			get {
				if (chkNhM.Checked)
					return RecessCase.Primary;
				else
					return RecessCase.Secondary;
			}
			set {
				if(value == RecessCase.Primary)
				{
					chkNhM.Checked = true;
					chkNhS.Checked = false;
				}
				else
				{
					chkNhM.Checked = false;
					chkNhS.Checked = true;
				}
			}
		}

		public double TopOffset
		{
			get
			{
				return Convert.ToDouble(txtPosVTop.Text);
			}
			set
			{
				txtPosVTop.Text = value.ToString();
				txtPosVBottom.Text = (SecElemHeight - value - Geometry_Connector_Height).ToString();
			}
		}

		public double LeftOffset
		{
			get
			{
				return Convert.ToDouble(txtPosHLeft.Text);
			}
			set
			{
				txtPosHLeft.Text = value.ToString();
				txtPosHRight.Text = (SecElemWidth - value - Geometry_Connector_Width).ToString();
			}
		}

		public double Gap_MainSecondary_Between
		{
			get
			{
				return Convert.ToDouble(txtNhSGapMainSec.Text);
			}
			set
			{
				txtNhSGapMainSec.Text = Gap_MainSecondary_Between.ToString();
			}
		}

		public double Gap_BackPlate_RecessDepth
		{
			get
			{
				return Convert.ToDouble(txtNhMDepth.Text);
			}
			set
			{
				txtNhMDepth.Text = value.ToString();
			}
		}

		public double Gap_BackPlate_Front
		{
			get
			{
				return Convert.ToDouble(txtNhSDepth.Text);
			}
			set
			{
				txtNhSDepth.Text = value.ToString();
			}
		}

		public double Gap_BackPlate_Top
		{
			get
			{
				if (chkNhM.Checked)
				{
					return Convert.ToDouble(txtNhMTop.Text);
				}
				else
				{
					return Convert.ToDouble(txtNhSTop.Text);
				}
			}
			set
			{
				txtNhMTop.Text = value.ToString();
				txtNhSTop.Text = value.ToString();
			}
		}

		public double Gap_BackPlate_Bottom
		{
			get
			{
				if (chkNhM.Checked)
				{
					return Convert.ToDouble(txtNhMBottom.Text);
				}
				else
				{
					return Convert.ToDouble(txtNhSBottom.Text);
				}
			}
			set
			{
				txtNhMBottom.Text = value.ToString();
				txtNhSBottom.Text = value.ToString();
			}
		}

		public double Gap_BackPlate_Left
		{
			get
			{
				if (chkNhM.Checked)
				{
					return Convert.ToDouble(txtNhMSideB.Text);
				}
				else
				{
					return Convert.ToDouble(txtNhSSideA.Text);
				}
			}
			set
			{
				txtNhMSideB.Text = value.ToString();
				txtNhSSideA.Text = value.ToString();
			}
		}

		public double Gap_BackPlate_Right
		{
			get
			{
				if (chkNhM.Checked)
				{
					return Convert.ToDouble(txtNhMSideA.Text);
				}
				else
				{
					return Convert.ToDouble(txtNhSSideB.Text);
				}
			}
			set
			{
				txtNhMSideA.Text = value.ToString();
				txtNhSSideB.Text = value.ToString();
			}
		}

		public double SecElemHeight { get; set; } = 600;
		public double SecElemWidth { get; set; } = 300;

		public double Geometry_Connector_Width { get; set; }

		public double Geometry_Connector_Height { get; set; }

		public TConPositionParam TConPositionParams
		{
			get
			{
				return new TConPositionParam()
				{
					RecessCase = RecessElement,
					HorizontalFixed = HorizontalFixed,
					VerticalFixed = VerticalFixed,
					TopOffset = TopOffset,
					LeftOffset = LeftOffset,
					Gap_BackPlate_RecessDepth = Gap_BackPlate_RecessDepth,
					Gap_BackPlate_Front = Gap_BackPlate_Front,
					Gap_BackPlate_SideA = Gap_BackPlate_Left,
					Gap_BackPlate_SideB = Gap_BackPlate_Right,
					Gap_BackPlate_Top = Gap_BackPlate_Top,
					Gap_BackPlate_Bottom = Gap_BackPlate_Bottom,
					Gap_MainSecondary_Between = Gap_MainSecondary_Between,
					Secondary_Width = SecElemWidth,
					Secondary_Height = SecElemHeight
				};
			}
		}

		public UsrTConnectorPosition()
		{
			InitializeComponent();
		}

		public void UpdateParameters(TConPositionParam value, double Connector_Width, double Connector_Height)
		{
			RecessElement = value.RecessCase;
			HorizontalFixed = value.HorizontalFixed;
			VerticalFixed = value.VerticalFixed;
			TopOffset = value.TopOffset;
			LeftOffset = value.LeftOffset;
			Gap_BackPlate_Front = value.Gap_BackPlate_Front;
			Gap_BackPlate_RecessDepth = value.Gap_BackPlate_RecessDepth;
			Gap_BackPlate_Left = value.Gap_BackPlate_SideA;
			Gap_BackPlate_Right = value.Gap_BackPlate_SideB;
			Gap_BackPlate_Top = value.Gap_BackPlate_Top;
			Gap_BackPlate_Bottom = value.Gap_BackPlate_Bottom;
			Gap_MainSecondary_Between = value.Gap_MainSecondary_Between;
			SecElemWidth = value.Secondary_Width;
			SecElemHeight = value.Secondary_Height;
			Geometry_Connector_Width = Connector_Width;
			Geometry_Connector_Height = Connector_Height;
		}

		private void ChkPosFixedToSecondary_CheckedChanged(object sender, EventArgs e)
		{
			//groPosition.Enabled = !chkPosFixedToSecondary.Checked;
			VerticalFixed = chkPosFixedToSecondary.Checked;
			txtPosVBottom.Enabled = !chkPosFixedToSecondary.Checked;
			txtPosVTop.Enabled = !chkPosFixedToSecondary.Checked;
		}

		private void ChkPosWidthFixedToCentre_CheckedChanged(object sender, EventArgs e)
		{
			//groHostPri.Enabled = !chkPosWidthFixedToCentre.Checked;
			HorizontalFixed = chkPosWidthFixedToCentre.Checked;

			txtPosHLeft.Enabled = !chkPosWidthFixedToCentre.Checked;
			txtPosHRight.Enabled = !chkPosWidthFixedToCentre.Checked;
		}

		private void TxtPosVTop_TextChanged(object sender, EventArgs e)
		{
			if (txtPosVTop.Text != "")
			{
				if (SecElemHeight > 0 && SecElemHeight > TopOffset + Geometry_Connector_Height)
					txtPosVBottom.Text = (SecElemHeight - TopOffset - Geometry_Connector_Height).ToString();
				else if (SecElemHeight > 0)
				{
					MessageBox.Show("The offset is bigger than height of secondary element.");
					TopOffset = 50;
				}
			}
			else
			{
				TopOffset = 0;
			}
		}

		private void TxtPosVBottom_TextChanged(object sender, EventArgs e)
		{
			double offset;
			if (txtPosVBottom.Text != "")
			{
				offset = Convert.ToDouble(txtPosVBottom.Text);
				
				if (SecElemHeight > offset + Geometry_Connector_Height)
					TopOffset = SecElemHeight - offset - Geometry_Connector_Height;
				else if (SecElemHeight > 0)
				{
					TopOffset = SecElemHeight - 50 - Geometry_Connector_Height;
				}
			}
			else
			{
				TopOffset = SecElemHeight;
			}
		}

		private void TxtPosHLeft_TextChanged(object sender, EventArgs e)
		{
			if (txtPosHLeft.Text != "")
			{
				if (SecElemWidth > 0 && SecElemWidth > LeftOffset + Geometry_Connector_Width)
					txtPosHRight.Text = (SecElemWidth - LeftOffset - Geometry_Connector_Width).ToString();
				else if (SecElemWidth > 0)
				{
					MessageBox.Show("The offset is bigger than width of secondary element.");
					LeftOffset = 50;
				}
			}
			else LeftOffset = 0;
		}

		private void TxtPosHRight_TextChanged(object sender, EventArgs e)
		{
			double offset;
			if (txtPosHRight.Text != "")
			{
				offset = Convert.ToDouble(txtPosHRight.Text);
				if (SecElemWidth > offset + Geometry_Connector_Width)
					LeftOffset = SecElemWidth - offset - Geometry_Connector_Width;
				else if (SecElemWidth > 0)
				{
					MessageBox.Show("The offset is bigger than width of secondary element.");
					LeftOffset = SecElemWidth - 50 - Geometry_Connector_Width;
				}
			}
			else LeftOffset = SecElemWidth;
		}

		private void TxtChangingRecessToMainParams(object sender, EventArgs e)
		{
			if (sender is TextBox textBox && textBox.Text == "")
				textBox.Text = "0";
		}

		private void ChkNhS_CheckedChanged(object sender, EventArgs e)
		{
			if (chkNhS.Focused)
			{
				chkNhM.Checked = !chkNhS.Checked;
				SetStateGroHostPri(chkNhM.Checked);
				SetStateGroHostSec(chkNhS.Checked);
			}
		}

		private void ChkNhM_CheckedChanged(object sender, EventArgs e)
		{
			if (chkNhM.Focused)
			{
				chkNhS.Checked = !chkNhM.Checked;
				SetStateGroHostPri(chkNhM.Checked);
				SetStateGroHostSec(chkNhS.Checked);
			}
		}

		private void SetStateGroHostPri(bool bSet)
		{
			txtNhMTop.Enabled = bSet;
			txtNhMBottom.Enabled = bSet;
			txtNhMDepth.Enabled = bSet;
			txtNhMSideA.Enabled = bSet;
			txtNhMSideB.Enabled = bSet;
		}

		private void SetStateGroHostSec(bool bSet)
		{
			txtNhSTop.Enabled = bSet;
			txtNhSBottom.Enabled = bSet;
			txtNhSDepth.Enabled = bSet;
			txtNhSSideA.Enabled = bSet;
			txtNhSSideB.Enabled = bSet;
		}
	}
}
