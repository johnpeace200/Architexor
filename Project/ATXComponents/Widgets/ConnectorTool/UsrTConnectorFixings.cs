using Architexor.Models.ConnectorTool;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Architexor.Widgets.ConnectorTool
{
	public partial class UsrTConnectorFixings : UserControl
	{
		public TConFixingParam ConFixingParam
		{
			get
			{
				return new TConFixingParam()
				{
					PriFixing = PriFixing,
					BPriBoltHole = ShowPriBoltHole,
					PriBoltHoleDiameter = PriBoltHoleDiameter,
					PriPlug = PriPlug,
					SecFixing = SecFixing,
					BSecHole = ShowSecHole,
					SecHoleDiameter = SecHoleDiameter,
					BSecReducedHole = ShowSecReducedHole,
					SecReducedSideA = SecReducedSideA,
					SecReducedSideB = SecReducedSideB,
					SecPlugSideType = SecPlugSideType,
					SecPlug = SecPlug
				};
			}
		}

		private static List<FixingCandidate> m_FixingCandidates = null;

		public bool ShowPriBoltHole
		{
			get
			{
				return chkDrMBoltHole.Checked;
			}
			set
			{
				chkDrMBoltHole.Checked = value;
				txtDrMBoltHoleDia.Enabled = value;
			}
		}

		public double PriBoltHoleDiameter
		{
			get
			{
				return Convert.ToDouble(txtDrMBoltHoleDia.Text);
			}
			set
			{
				txtDrMBoltHoleDia.Text = value.ToString();
			}
		}

		public Plug PriPlug
		{
			get
			{
				if (!chkDrMPlugging.Checked)
					return null;
				else
					return new Plug()
						{
							valid = chkDrMPlugging.Checked,
							show = chkDrMPlugShow.Checked,
							diameter = Convert.ToDouble(txtDrMPlugDia.Text),
							depth = Convert.ToDouble(txtDrMPlugDepth.Text)
						};
			}
			set
			{
				if (value != null)
				{
					chkDrMPlugging.Checked = value.valid;
					chkDrMPlugShow.Checked = value.show;
					txtDrMPlugDia.Text = value.diameter.ToString();
					txtDrMPlugDepth.Text = value.depth.ToString();

					chkDrMPlugShow.Enabled = value.valid;
					txtDrMPlugDia.Enabled = value.valid;
					txtDrMPlugDepth.Enabled = value.valid;
				}
				else
				{
					chkDrMPlugging.Checked = false;
					chkDrMPlugShow.Enabled = false;
					txtDrMPlugDia.Enabled = false;
					txtDrMPlugDepth.Enabled = false;
				}
			}
		}

		public Plug SecPlug
		{
			get
			{
				if (!chkDrSPlugging.Checked)
					return null;
				else
					return new Plug()
					{
						valid = chkDrSPlugging.Checked,
						show = chkDrSPlugShow.Checked,
						diameter = Convert.ToDouble(txtDrSPlugDia.Text),
						depth = Convert.ToDouble(txtDrSPlugDepth.Text)
					};
			}
			set
			{
				if(value != null)
				{
					chkDrSPlugging.Checked = value.valid;
					chkDrSPlugShow.Checked = value.show;
					txtDrSPlugDia.Text = value.diameter.ToString();
					txtDrSPlugDepth.Text = value.depth.ToString();

					chkDrSPlugShow.Enabled = value.valid;
					cmbDrSPlugSide.Enabled = value.valid;
					txtDrSPlugDia.Enabled = value.valid;
					txtDrSPlugDepth.Enabled = value.valid;
				}
				else
				{
					chkDrSPlugShow.Enabled = false;
					cmbDrSPlugSide.Enabled = false;
					txtDrSPlugDia.Enabled = false;
					txtDrSPlugDepth.Enabled = false;
				}
			}
		}

		public FixingCandidate PriFixing
		{
			get
			{
				if (cmbDrMFixName.SelectedItem is FixingCandidate fc)
					return fc;
				else
					return null;
			}
			set
			{
				if (value == null)
				{
					cmbDrMFixName.SelectedItem = null;
				}
				else
				{
					if (value.Name != null)
						cmbDrMFixName.SelectedItem = value;
					//				else if (cmbDrMFixName.SelectedItem is Fixing fixing)
					//						value = fixing;

					txtDrMFixingDia.Text = value.Diameter.ToString();
					txtDrMFixingLength.Text = value.Length.ToString();
				}
			}
		}

		public FixingCandidate SecFixing
		{
			get
			{
				if (cmbDrSFixName.SelectedItem is FixingCandidate fc)
					return fc;
				else
					return null;
			}
			set
			{
				if (value == null)
				{
					cmbDrSFixName.SelectedItem = null;
				}
				else
				{
					if (value.Name != null)
						cmbDrSFixName.SelectedItem = value;
					//				else if (cmbDrSFixName.SelectedItem is Fixing fixing)
					//						value = fixing;

					txtDrSFixingDia.Text = value.Diameter.ToString();
					txtDrSFixingLength.Text = value.Length.ToString();
				}
			}
		}

		public bool ShowSecHole
		{
			get
			{
				return chkDrSFixingHole.Checked;
			}
			set
			{
				chkDrSFixingHole.Checked = value;
				txtDrSFixingHoleDia.Enabled = value;
			}
		}

		public bool ShowSecReducedHole
		{
			get
			{
				return chkDrSReduced.Checked;
			}
			set
			{
				chkDrSReduced.Checked = value;
				txtDrSReducedSideA.Enabled = value;
				txtDrSReducedSideB.Enabled = value;

				RefreshPlugSide();
			}
		}

		public double SecReducedSideA
		{
			get
			{
				return Convert.ToDouble(txtDrSReducedSideA.Text);
			}
			set
			{
				txtDrSReducedSideA.Text = value.ToString();

				RefreshPlugSide();
			}
		}

		public double SecReducedSideB
		{
			get
			{
				return Convert.ToDouble(txtDrSReducedSideB.Text);
			}
			set
			{
				txtDrSReducedSideB.Text = value.ToString();

				RefreshPlugSide();
			}
		}

		public PluggingSideType SecPlugSideType
		{
			get
			{
				switch (cmbDrSPlugSide.SelectedItem)
				{
					case "SideA":
						return PluggingSideType.SideA;
					case "SideB":
						return PluggingSideType.SideB;
					case "Both":
					default:
						return PluggingSideType.Both;
				}
			}
			set
			{
				switch (value)
				{
					case PluggingSideType.SideA:
						cmbDrSPlugSide.SelectedItem = "SideA";
						break;
					case PluggingSideType.SideB:
						cmbDrSPlugSide.SelectedItem = "SideB";
						break;
					case PluggingSideType.Both:
						cmbDrSPlugSide.SelectedItem = "Both";
						break;
					default:
						break;
				}
			}
		}

		public double SecHoleDiameter
		{
			get
			{
				return double.Parse(txtDrSFixingHoleDia.Text);
			}
			set
			{
				txtDrSFixingHoleDia.Text = value.ToString();
			}
		}

		public UsrTConnectorFixings()
		{
			InitializeComponent();
		}

		public void UpdateParameters(TConFixingParam value)
		{
			PriFixing = value.PriFixing;
			ShowPriBoltHole = value.BPriBoltHole;
			PriBoltHoleDiameter = value.PriBoltHoleDiameter;
			PriPlug = value.PriPlug;
			SecFixing = value.SecFixing;
			ShowSecHole = value.BSecHole;
			SecHoleDiameter = value.SecHoleDiameter;
			ShowSecReducedHole = value.BSecReducedHole;
			SecReducedSideA = value.SecReducedSideA;
			SecReducedSideB = value.SecReducedSideB;
			SecPlugSideType = value.SecPlugSideType;
			SecPlug = value.SecPlug;
		}

		private void CmbDrMDrillType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string text = (sender as ComboBox).Text;

			if (text == "BOLT")
			{
				ShowPriBoltHole = true;
				chkDrMBoltHole.Enabled = true;
				chkDrMPlugging.Enabled = true;
			}
			else
			{
				ShowPriBoltHole = false;
				chkDrMBoltHole.Enabled = false;
				PriPlug = null;
				chkDrMPlugging.Enabled = false;
			}

			RefreshFixingListControl(true);
		}

		public void RefreshFixingListControl(List<FixingCandidate> candidates)
		{
			m_FixingCandidates = candidates;

			cmbDrMFixingType.SelectedIndex = 0;
			cmbDrSFixingType.SelectedIndex = 0;
			//foreach(FixingCandidate fc in m_FixingCandidates)
			//	{
			//		if(fc.ConnectionType == ConnectionType.None)
			//			lstFixingCandidates.Items.Add(fc);
			//	}
		}

		private void RefreshFixingListControl(bool bMain)
		{
			if (m_FixingCandidates == null)
				return;

			// refresh control's data
			if (bMain)
			{
				cmbDrMFixName.Items.Clear();
				cmbDrMFixName.Text = "";
				txtDrMFixingDia.Text = "0";
				txtDrMFixingLength.Text = "0";
				txtDrMFixingDia.ReadOnly = true;
				txtDrMFixingLength.ReadOnly = true;
				foreach (FixingCandidate fc in m_FixingCandidates)
				{
					if ((fc.ConnectionType == ConnectionType.Bolt && cmbDrMFixingType.Text == "BOLT")
						|| (fc.ConnectionType == ConnectionType.Screw && cmbDrMFixingType.Text == "SCREW"))
					{
						cmbDrMFixName.Items.Add(fc);
					}
				}

				if (cmbDrMFixName.Items.Count > 0)
					cmbDrMFixName.SelectedIndex = 0;
			}
			else
			{
				cmbDrSFixName.Items.Clear();
				cmbDrSFixName.Text = "";
				txtDrSFixingDia.Text = "0";
				txtDrSFixingLength.Text = "0";
				txtDrSFixingDia.ReadOnly = true;
				txtDrSFixingLength.ReadOnly = true;
				foreach (FixingCandidate fc in m_FixingCandidates)
				{
					if ((fc.ConnectionType == ConnectionType.Bolt && cmbDrSFixingType.Text == "BOLT")
						|| (fc.ConnectionType == ConnectionType.Dowel && cmbDrSFixingType.Text == "DOWEL"))
						cmbDrSFixName.Items.Add(fc);
				}

				if (cmbDrSFixName.Items.Count > 0)
					cmbDrSFixName.SelectedIndex = 0;
			}
		}

		private void TxtChangingDrillToMainElemGeoParams(object sender, EventArgs e)
		{
			if (sender is TextBox textBox)
			{
				if (textBox.Text == "")
					textBox.Text = "0";

				if (textBox.Name == "txtDrSReducedSideA")
				{
					SecReducedSideA = Convert.ToDouble(textBox.Text);
				}
				else if(textBox.Name == "txtDrSReducedSideB")
				{
					SecReducedSideB = Convert.ToDouble(textBox.Text);
				}
			}
		}

		private void CmbDrSFixingType_SelectedIndexChanged(object sender, EventArgs e)
		{
			string text = (sender as ComboBox).Text;
			if (text == "BOLT")
			{
				ShowSecReducedHole = false;
				chkDrSReduced.Enabled = false;
			}
			else
			{
				ShowSecHole = true;
				chkDrSReduced.Enabled = true;
			}

			RefreshFixingListControl(false);
		}

		private void CmbDrMFixName_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmbDrMFixName.SelectedItem is FixingCandidate fc && fc != null)
			{
				if (fc.Diameter > 0 && fc.Length > 0)
				{
					txtDrMFixingDia.Text = fc.Diameter.ToString();
					txtDrMFixingLength.Text = fc.Length.ToString();
					txtDrMFixingDia.ReadOnly = true;
					txtDrMFixingLength.ReadOnly = true;
				}
				else
				{
					txtDrMFixingDia.Text = "0";
					txtDrMFixingLength.Text = "0";
					txtDrMFixingDia.ReadOnly = false;
					txtDrMFixingLength.ReadOnly = false;
				}
			}
		}

		private void CmbDrSFixName_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmbDrSFixName.SelectedItem is FixingCandidate fc && fc != null)
			{
				if (fc.Diameter > 0 && fc.Length > 0)
				{
					txtDrSFixingDia.Text = fc.Diameter.ToString();
					txtDrSFixingLength.Text = fc.Length.ToString();
					txtDrSFixingDia.ReadOnly = true;
					txtDrSFixingLength.ReadOnly = true;
				}
				else
				{
					txtDrSFixingDia.Text = "0";
					txtDrSFixingLength.Text = "0";
					txtDrSFixingDia.ReadOnly = false;
					txtDrSFixingLength.ReadOnly = false;
				}
			}
		}

		private void chkDrMBoltHole_CheckedChanged(object sender, EventArgs e)
		{
			ShowPriBoltHole = ((CheckBox)sender).Checked;
		}

		private void chkDrMPlugging_CheckedChanged(object sender, EventArgs e)
		{
			PriPlug = new Plug()
			{
				valid = ((CheckBox)sender).Checked
			};
		}

		private void chkDrSFixingHole_CheckedChanged(object sender, EventArgs e)
		{
			ShowSecHole = ((CheckBox)sender).Checked;
		}

		private void chkDrSReduced_CheckedChanged(object sender, EventArgs e)
		{
			ShowSecReducedHole = ((CheckBox)sender).Checked;
		}

		private void chkDrSPlugging_CheckedChanged(object sender, EventArgs e)
		{
			SecPlug = new Plug()
			{
				valid = ((CheckBox)sender).Checked
			};
		}

		private void RefreshPlugSide()
		{
			cmbDrSPlugSide.Items.Clear();
			if (ShowSecReducedHole)
			{
				if (SecReducedSideA == 0 && SecReducedSideB == 0)
				{
					cmbDrSPlugSide.Items.Add("SideA");
					cmbDrSPlugSide.Items.Add("SideB");
					cmbDrSPlugSide.Items.Add("Both");
				}
				else if (SecReducedSideA == 0)
				{
					cmbDrSPlugSide.Items.Add("SideA");
				}
				else
				{
					cmbDrSPlugSide.Items.Add("SideB");
				}
			}
			else
			{
				cmbDrSPlugSide.Items.Add("SideA");
				cmbDrSPlugSide.Items.Add("SideB");
				cmbDrSPlugSide.Items.Add("Both");
			}
			cmbDrSPlugSide.SelectedIndex = 0;
		}

		private void lstFixingCandidates_SelectedIndexChanged(object sender, EventArgs e)
		{
			//FixingCandidate candidate = (FixingCandidate)lstFixingCandidates.SelectedItem;
			//if (candidate == null)
			//	return;

			//cmbDiameterParameter.Items.Clear();
			//cmbLengthParameter.Items.Clear();
			//foreach (string s in candidate.TypeParameters)
			//{
			//	cmbDiameterParameter.Items.Add(s);
			//	cmbLengthParameter.Items.Add(s);
			//}
			//foreach (string s in candidate.InstanceParameters)
			//{
			//	cmbDiameterParameter.Items.Add(s);
			//	cmbLengthParameter.Items.Add(s);
			//}
			//cmbDiameterParameter.Text = "";
			//cmbLengthParameter.Text = "";
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			//if (cmbDiameterParameter.SelectedIndex < 0
			//	|| cmbLengthParameter.SelectedIndex < 0
			//	|| lstFixingCandidates.SelectedIndex < 0
			//	|| cmbFixingType.SelectedIndex < 0)
			//	return;

			//FixingCandidate candidate = lstFixingCandidates.SelectedItem as FixingCandidate;
			//switch (cmbFixingType.Text)
			//{
			//	case "BOLT":
			//		candidate.ConnectionType = ConnectionType.Bolt;
			//		break;
			//	case "DOWEL":
			//		candidate.ConnectionType = ConnectionType.Dowel;
			//		break;
			//	case "SCREW":
			//		candidate.ConnectionType = ConnectionType.Screw;
			//		break;
			//	default:
			//		candidate.ConnectionType = ConnectionType.None;
			//		break;
			//}

			//lstFixingCandidates.Items.Remove(candidate);
			//lstFixings.Items.Add(candidate);
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			//if (lstFixings.SelectedItem == null)
			//	return;

			//FixingCandidate candidate = lstFixings.SelectedItem as FixingCandidate;
			//candidate.ConnectionType = ConnectionType.None;
			//lstFixings.Items.Remove(candidate);
			//lstFixingCandidates.Items.Add(candidate);
		}

		private void cmbFixingType_SelectedIndexChanged(object sender, EventArgs e)
		{
			//lstFixings.Items.Clear();
			//foreach(FixingCandidate fc in m_FixingCandidates)
			//{
			//	if((cmbFixingType.Text == "BOLT" && fc.ConnectionType == ConnectionType.Bolt)
			//		|| (cmbFixingType.Text == "DOWEL" && fc.ConnectionType == ConnectionType.Dowel)
			//		|| (cmbFixingType.Text == "SCREW" && fc.ConnectionType == ConnectionType.Screw)
			//		)
			//	{
			//		lstFixings.Items.Add(fc);
			//	}
			//}
		}

		private void UsrTConnectorFixings_Load(object sender, EventArgs e)
		{
		}
	}
}
