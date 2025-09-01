using Autodesk.Revit.UI;
using Architexor.Forms;
using Architexor.Request;
using System.Collections.Generic;
using System.Windows.Forms;
using Control = System.Windows.Forms.Control;
using HalfLap = Architexor.Connect.Connectors.HalfLap;
using System;
using Architexor.Utils;

namespace Architexor.Connect.Forms
{
	public partial class FrmHalfLap : System.Windows.Forms.Form, IExternal
	{
		// The dialog owns the handler and the event objects,
		// but it is not a requirement. They may as well be static properties
		// of the application.
		protected ArchitexorRequestHandler m_Handler;
		protected ExternalEvent m_ExEvent;

		public ArchitexorRequestHandler Handler { get => m_Handler; }

		public ArchitexorRequestId LastRequestId { get; set; } = ArchitexorRequestId.None;

		private enum State
		{
			Init = 0x00,
			ParentElementsSelected = 0x01
		};
		private State m_State = State.Init;

		public FrmHalfLap(ArchitexorRequestId reqId, UIApplication uiApp)
		{
			InitializeComponent();

			optCentred.Checked = true;

			// A new handler to handle request posting by the dialog
			m_Handler = new ArchitexorRequestHandler(PanelTool.Application.thisApp, reqId, uiApp);

			// External Event for the dialog to use (to post requests)
			m_ExEvent = ExternalEvent.Create(m_Handler);

			WakeUp();
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			// we own both the event and the handler
			// we should dispose it before we are closed
			m_ExEvent.Dispose();
			m_ExEvent = null;
			m_Handler = null;

			// do not forget to call the base class
			base.OnFormClosed(e);
		}

		private void EnableCommands(bool status)
		{
			foreach (Control ctrl in this.Controls)
			{
				ctrl.Enabled = status;
			}
			if (!status)
			{
				btnCancel.Enabled = true;
			}
		}

		//	WakeUp -> enable all controls
		public void WakeUp(bool bFinish = false)
		{
			if (bFinish)
			{
				Close();
				return;
			}
			EnableCommands(true);

			HalfLap ins = (HalfLap)(m_Handler.Instance);

			if (ins.Elements.Count == 0)
			{
				m_State = State.Init;
				btnSelectElement.Text = "Select";
			}
			else
			{
				m_State = State.ParentElementsSelected;
				btnSelectElement.Text = "Select(" + ins.Elements.Count.ToString() + ")";
			}

			SetControlsState();

			Activate();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}

		private bool Serialize()
		{
			HalfLap ins = (HalfLap)(m_Handler.Instance);

			try
			{
				ins.Depth = txtHLDepth.Text == "" ? -1 : int.Parse(txtHLDepth.Text);
				ins.SideAGap = float.Parse(txtSideAGap.Text);
				ins.SideBGap = float.Parse(txtSideBGap.Text);
				ins.LapWidth = int.Parse(txtLapWidth.Text);
				ins.GapBetweenPanels = float.Parse(txtGapBetweenPanels.Text);
				ins.Flip = chkFlip.Checked;
				ins.AddHorzHL = chkAddHorzHL.Checked;
				ins.ExcludeMinLimitLength = float.Parse(txtMinLengthLimit.Text);
				ins.ConnectionLocation = optCentred.Checked ? HalfLap.ConnectionLocationType.Centre : (optPushToA.Checked ? HalfLap.ConnectionLocationType.PushToA : HalfLap.ConnectionLocationType.PushToB);
			}
			catch (Exception)
			{
				MessageBox.Show("Please input value correctly.");
				return false;
			}

			return true;
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{
			if (Serialize())
			{
				MakeRequest(ArchitexorRequestId.ArrangeConnectors);
			}
		}

		private void txtDepth_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}
		}

		private void txtSideAGap_KeyPress(object sender, KeyPressEventArgs e)
		{

		}

		private void txtSideBGap_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}
		}

		private void txtLapWidth_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}
		}

		private void txtGapBetweenPanels_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}
		}

		private void txtMinLimitLength_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void FrmHalfLap_Load(object sender, EventArgs e)
		{
			string sCategory = "HalfLap";
			List<Preset> presets = PresetManager.GetPresetsByCategory(sCategory);
			foreach (Preset preset in presets)
			{
				cmbPreset.Items.Add(preset.Properties[0].Value);
			}

			ToolTip tooltip = new();
			tooltip.SetToolTip(chkFlip, "Half Laps will be placed on the top side (exterior side) of the slab(wall). If you want to place them on the bottom(interior) side, please check here.");

			txtVersion.Text = string.Format("{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
		}

		private void btnHelp_Click(object sender, EventArgs e)
		{
			//System.Diagnostics.Process.Start(Application.SITE);
			MessageBox.Show("Important Update: Plugin Migration and New Free Version Coming Soon\n\n" +
					"We are excited to inform you that we are in the process of migrating our plugin to a new, enhanced version, which will be available for free starting in November. This update is part of our ongoing commitment to improving your experience with our tools.\n\n" +
					"As we work on this migration, you may encounter some bugs or issues. We sincerely apologize for any inconvenience this may cause and appreciate your understanding during this transition period.\n\n" +
					"If you find any bugs or need support, please don't hesitate to reach out to the Product Manager - mailto:chris@futuretimber.com or the Plugin Developer - mailto:xiaoliangzon01@gmail.com. Your feedback is invaluable in helping us ensure the best possible experience for all users.\n\n" +
					"Thank you for your continued support and patience!");
		}

		private void btnPresetSelect_Click(object sender, EventArgs e)
		{
			string sCategory = "HalfLap";

			if (cmbPreset.SelectedIndex < 0)
				return;

			Preset preset = PresetManager.GetPreset(sCategory, cmbPreset.SelectedItem.ToString());
			foreach (Property p in preset.Properties)
			{
				switch (p.Key)
				{
					case "Depth":
						txtHLDepth.Text = p.Value.ToString();
						break;
					case "Lap Width":
						txtLapWidth.Text = p.Value.ToString();
						break;
					case "Side A Gap":
						txtSideAGap.Text = p.Value.ToString();
						break;
					case "Side B Gap":
						txtSideBGap.Text = p.Value.ToString();
						break;
					case "Gap Between Panels":
						txtGapBetweenPanels.Text = p.Value.ToString();
						break;
					case "Flip":
						if (p.Value.ToString() == "0")
						{
							chkFlip.Checked = false;
						}
						else
						{
							chkFlip.Checked = true;
						}
						break;
					case "Add Horizontal HL":
						if (p.Value.ToString() == "0")
						{
							chkAddHorzHL.Checked = false;
						}
						else
						{
							chkAddHorzHL.Checked = true;
						}
						break;
					case "ExcludeMinLimitLength":
						txtMinLengthLimit.Text = p.Value.ToString();
						break;
					default:
						break;
				}
			}
		}

		private void btnPresetSave_Click(object sender, EventArgs e)
		{
			string sCategory = "HalfLap";

			if (cmbPreset.Text == "")
				return;

			if (PresetManager.IsPresetKeyValid(sCategory, cmbPreset.Text))
			{
				Preset preset = new()
				{
					Category = sCategory,
					Properties = new List<Property>()
					{
						new Property() { Key = "Name", Value = cmbPreset.Text },
						new Property() { Key = "Depth", Value = txtHLDepth.Text },
						new Property() { Key = "Lap Width", Value = txtLapWidth.Text },
						new Property() { Key = "Side A Gap", Value = txtSideAGap.Text },
						new Property() { Key = "Side B Gap", Value = txtSideBGap.Text },
						new Property() { Key = "Gap Between Panels", Value = txtGapBetweenPanels.Text },
						new Property() { Key = "Flip", Value = chkFlip.Checked ? "1" : "0" },
						new Property() { Key = "Add Horizontal HL", Value = chkAddHorzHL.Checked ? "1" : "0" },
						new Property() { Key = "ExcludeMinLimitLength", Value = txtMinLengthLimit.Text }
					}
				};
				PresetManager.AddPreset(sCategory, preset);
				cmbPreset.Items.Add(cmbPreset.Text);
			}
			else
			{
				Util.InfoMsg("Please choose another name.");
			}
		}

		private void btnPresetDelete_Click(object sender, EventArgs e)
		{
			if (cmbPreset.SelectedIndex < 0)
				return;

			string sCategory = "HalfLap";
			cmbPreset.Text = cmbPreset.SelectedItem.ToString();
			PresetManager.RemovePreset(sCategory, cmbPreset.Text);
			cmbPreset.Items.RemoveAt(cmbPreset.SelectedIndex);
		}

		private void btnSelectElement_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectParentElements);
		}

		private void btnClearElement_Click(object sender, EventArgs e)
		{
			HalfLap ins = (HalfLap)(m_Handler.Instance);
			ins.Elements.Clear();
			btnSelectElement.Text = "Select";

			m_State = State.Init;
			SetControlsState();
		}

		private void SetControlsState()
		{
			if (m_State == State.Init)
			{
				btnOK.Enabled = false;
			}
			else
			{
				btnOK.Enabled = true;
			}
		}

		public ArchitexorRequestId GetRequestId()
		{
			if (m_Handler == null)
			{
				return ArchitexorRequestId.None;
			}
			return m_Handler.RequestId;
		}

		public void MakeRequest(ArchitexorRequestId request)
		{
			LastRequestId = request;

			m_Handler.Request.Make(request);
			m_ExEvent.Raise();
			DozeOff();
		}

		public void DozeOff()
		{
			EnableCommands(false);
		}

		public void IClose()
		{
			Close();
		}

		public bool IVisible()
		{
			return Visible;
		}

		public bool IIsDisposed()
		{
			return IsDisposed;
		}

		public void IShow()
		{
			Show();
		}
	}
}
