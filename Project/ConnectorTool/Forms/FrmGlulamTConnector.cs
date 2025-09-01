using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ConnectorTool.Engine;
using ConnectorTool.Information;
using Control = System.Windows.Forms.Control;
using Architexor.Models.ConnectorTool;
using Architexor.Request;
using Architexor.Utils;
using Architexor.Forms;

namespace ConnectorTool.Forms
{
	public partial class FrmGlulamTConnector : System.Windows.Forms.Form, IExternal
	{
		// This is the enum for controlling the groups of Connection Parameter.
		public enum ConnectionParamId
		{
			ConnectorPosition = 0,
			ConnectorFinFilletCover,
			ConnectorFixings,
		}
		// The dialog owns the handler and the event objects,
		// but it is not a requirement. They may as well be static properties
		// of the application.
		protected ArchitexorRequestHandler m_Handler;
		protected ExternalEvent m_ExEvent;

		public ArchitexorRequestHandler Handler { get => m_Handler; }

		public ArchitexorRequestId LastRequestId { get; set; } = ArchitexorRequestId.None;

		// The ConnectionParameter's index
		private ConnectionParamId m_connectionParamId = ConnectionParamId.ConnectorPosition;

		// This is the ControlList to control the ConnectionParameter's user controls.
		private readonly List<UserControl> groConParamControls;

		// This is Connector's Basic Information(Primary, Secondary, host face) list to create the connectors.
		private List<TConElemInfo> m_conElemInfoList;

		// This is the created connector's list
		private readonly List<TConInfo> m_conInfoList;

		// This is the index of connector in the connection list
		private int m_conIndex;

		private enum State
		{
			Init = 0x00,
			ElementsSelected = 0x01
		}
		private State m_State = State.Init;

		/// <summary>
		/// Form initialize.
		/// </summary>
		public FrmGlulamTConnector(ArchitexorRequestId reqId, UIApplication uiApp)
		{
			InitializeComponent();

			// A new handler to handle request posting by the dialog
			m_Handler = new ArchitexorRequestHandler(Architexor.ConnectorTool.Application.thisApp, reqId, uiApp);

			// External Event for the dialog to use (to post requests)
			m_ExEvent = ExternalEvent.Create(m_Handler);

			// ConnectionParameters UserControls
			groConParamControls = new List<UserControl>();

			m_conElemInfoList = new List<TConElemInfo>();

			m_conInfoList = new List<TConInfo>();

			m_conIndex = 0;

			ConnectorBuilder cb = (ConnectorBuilder)m_Handler.Instance;
			//	Check if all necessary families exist
			//if (m_bCanWake = cb.GetConnectorData().CanWakeUp)
			//	WakeUp();
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
			foreach (Control ctrl in Controls)
			{
				ctrl.Enabled = status;
			}
			if (!status)
			{
				btnCancel.Enabled = true;
			}
		}

		/// <summary>
		/// initialize controls
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CreateTConnectorForm_Load(object sender, EventArgs e)
		{
			txtVersion.Text = string.Format("{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

			string sCategory = "GlulamTConnector";
			List<Preset> presets = PresetManager.GetPresetsByCategory(sCategory);
			foreach (Preset preset in presets)
			{
				cmbPreset.Items.Add(preset.Properties[0].Value);
			}

			ConnectorBuilder cb = (ConnectorBuilder)(m_Handler.Instance);
			RefreshConnectionFamilyListControl(cmbType, cb.GetConnectorData().ConnectionTypeMgr);
			RefreshConnectionSymbolListControl(cmbModel, cb.GetConnectorData().ConnectionTypeMgr, cb.GetConnectorData().ConnectionFamily);

			// Set the Bolt and Screw List in UsrTConnectorDrilltoMain Dialog and UsrTConnectorDrilltoSec Dialog
			RefreshFixingListControl(cb.GetConnectorData());

			//	Create an UsrControlList to control the Button Changing Event
			groConParamControls.Add(groPosition);
			groConParamControls.Add(groFinFilletCover);
			groConParamControls.Add(groFixing);

			foreach (Control groControl in groConParamControls)
			{
				groControl.Visible = false;
			}
		}

		private void BtnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		/// <summary>
		/// Picking Elements.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnSelect_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectElementsForTConnector);
		}

		/// <summary>
		/// Remove Elements
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void BtnRemove_Click(object sender, EventArgs e)
		{
			ConnectorBuilder ins = (ConnectorBuilder)(m_Handler.Instance);
			ins.Elements.Clear();
			btnSelectElement.Text = "Select";
			foreach (Control control in groConParamControls)
			{
				control.Visible = false;
			}
			m_State = State.Init;
			SetControlsState();
		}

		/// <summary>
		/// refresh the FamilyListControl's data source
		/// </summary>
		/// <param name="list">FamilyListControl to be refreshed</param>
		/// <param name="manager"> Type Manager to contain family and symbol information </param>
		private static void RefreshConnectionFamilyListControl(ListControl list, TypeManager manager)
		{
			// refresh control's data
			list.DataSource = null;
			list.DataSource = manager.Families;
			list.DisplayMember = "Name";
			if (manager.Families.Count > 0)
				list.SelectedIndex = 0;
		}

		/// <summary>
		/// refresh the FamilyListControl's data source
		/// </summary>
		/// <param name="nameList"> Type Manager to contain family and symbol information </param>
		private void RefreshFixingListControl(ConnectorData data)
		{
			groFixing.RefreshFixingListControl(data.FixingCandidatesList);
		}

		/// <summary>
		/// refresh the SymbolListControl's data source
		/// </summary>
		/// <param name="list">The ListControl of family</param>
		/// <param name="manager">The TypeManager of Connector</param>
		/// <param name="family">The family of Connector</param>
		private static void RefreshConnectionSymbolListControl(ListControl list, TypeManager manager, Family family)
		{
			// refresh control's data
			List<FamilySymbol> symbols = new List<FamilySymbol>();
			list.DataSource = null;
			foreach (FamilySymbol symbol in manager.Symbols)
			{
				if (family != null)
				{
					if (symbol.FamilyName == family.Name)
					{
						symbols.Add(symbol);
					}
				}
			}
			list.DataSource = symbols;
			list.DisplayMember = "Name";
			if (symbols.Count > 0)
				list.SelectedIndex = 0;
		}

		private void CmbModel_SelectedIndexChanged(object sender, EventArgs e)
		{
			ConnectorBuilder cb = (ConnectorBuilder)(m_Handler.Instance);
			cb.GetConnectorData().SetConnectionSymbol(cmbModel.SelectedItem as FamilySymbol);
			foreach (TConInfo conInfo in m_conInfoList)
			{
				conInfo.ConSymbol = cb.GetConnectorData().ConnectionSymbol;
			}
		}

		private void CmbType_SelectedIndexChanged(object sender, EventArgs e)
		{
			object obj = cmbType.SelectedItem;
			if (obj is Family family)
			{
				ConnectorBuilder cb = (ConnectorBuilder)(m_Handler.Instance);

				RefreshConnectionSymbolListControl(cmbModel, cb.GetConnectorData().ConnectionTypeMgr, family);
			}
		}

		private void BtnOK_Click(object sender, EventArgs e)
		{
			ConnectorBuilder cb = (ConnectorBuilder)(m_Handler.Instance);
			// if the user modify the last connector, changing parameters of this connector will not assign this connector because modify
			// method is only called when you select the previous and next button. so we call this method again here.
			if (!chkApplytoAll.Checked)
				ModifyConnectionParameters();
			else
			{
				m_conInfoList.Clear();
				CreateConnections();
			}

			cb.GetConnectorData().ConInfoList = m_conInfoList;

			MakeRequest(ArchitexorRequestId.ArrangeTConnectors);
		}

		private void BtnHelp_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.futuretimber.com/");
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

		public void WakeUp(bool bFinish = false)
		{
			if (bFinish)
			{
				Close();
				return;
			}
			EnableCommands(true);

			ConnectorBuilder ins = (ConnectorBuilder)(m_Handler.Instance);

			if (ins.Elements.Count == 0)
			{
				btnSelectElement.Text = "Select";
				m_State = State.Init;
			}
			else
			{
				m_State = State.ElementsSelected;
				btnSelectElement.Text = "Select(" + ins.Elements.Count.ToString() + ")";
				m_conElemInfoList.Clear();
				if (!ins.GetConnectorData().GetConElemInfoList(ins.Elements))
				{
					Activate();
					return;
				}
				m_conElemInfoList = ins.GetConnectorData().ConElemInfoList;
				if (m_conElemInfoList.Count > 1)
				{
					// Initialize Form Controls when the elements are selected.
					btnNext.Enabled = true;
					chkApplytoAll.Enabled = true;
				}
				if (m_conElemInfoList.Count > 0)
				{
					groConParamControls[(short)ConnectionParamId.ConnectorPosition].Visible = true;
					ins.SelectElementsLinkWithConnector(ins.GetConnectorData().ConElemInfoList[0]);
					CreateConnections();
				}
			}

			SetControlsState();

			Activate();
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

		private void BtnPresetSelect_Click(object sender, EventArgs e)
		{
			string sCategory = "GlulamTConnector";

			if (cmbPreset.SelectedIndex < 0)
				return;

			Preset preset = PresetManager.GetPreset(sCategory, cmbPreset.SelectedItem.ToString());
			foreach (Property p in preset.Properties)
			{
				switch (p.Key)
				{
					case "Type":
						foreach (object item in cmbType.Items)
						{
							if (item.ToString().Equals(p.Value.ToString()))
							{
								cmbType.SelectedItem = item;
							}
						}
						break;
					case "Model":
						foreach (object item in cmbModel.Items)
						{
							if (item.ToString().Equals(p.Value.ToString()))
							{
								cmbModel.SelectedItem = item;
							}
						}
						break;
					default:
						break;
				}
			}
		}

		private void BtnPresetSave_Click(object sender, EventArgs e)
		{
			string sCategory = "GlulamTConnector";

			if (cmbPreset.Text == "")
				return;

			if (PresetManager.IsPresetKeyValid(sCategory, cmbPreset.Text))
			{
				Preset preset = new Preset()
				{
					Category = sCategory,
					Properties = new List<Property>()
					{
						new Property() { Key = "Name", Value = cmbPreset.Text },
						new Property() { Key = "Type", Value = cmbType.SelectedIndex >= 0 ? cmbType.SelectedItem.ToString() : "" },
						new Property() { Key = "Model", Value = cmbModel.SelectedIndex >= 0 ? cmbModel.SelectedItem.ToString() : "" }
					}
				};
				PresetManager.AddPreset(sCategory, preset);
				cmbPreset.Items.Add(cmbPreset.Text);
			}
			else
			{
				MessageBox.Show("Please choose another name.");
			}
		}

		private void BtnPresetDelete_Click(object sender, EventArgs e)
		{
			if (cmbPreset.SelectedIndex < 0)
				return;

			string sCategory = "GlulamTConnector";
			cmbPreset.Text = cmbPreset.SelectedItem.ToString();
			PresetManager.RemovePreset(sCategory, cmbPreset.Text);
			cmbPreset.Items.RemoveAt(cmbPreset.SelectedIndex);
		}

		private void SetControlsState()
		{
			if (m_State == State.Init)
			{
				btnOK.Enabled = false;
				btnConPosition.Enabled = false;
				btnSetFinFilletCover.Enabled = false;
				btnSetFixings.Enabled = false;
				btnPreview.Enabled = false;
				btnPrevious.Enabled = false;
				btnNext.Enabled = false;
				chkApplytoAll.Enabled = false;
				picPreviewFront.Invalidate();
				picPreviewPlan.Invalidate();
			}
			else
			{
				btnOK.Enabled = true;
				btnConPosition.Enabled = true;
				btnSetFinFilletCover.Enabled = true;
				btnSetFixings.Enabled = true;
				//btnDrillingToMainElem.Enabled = true;
				//btnDrillingToSecondaryElem.Enabled = true;
				btnPreview.Enabled = true;
				chkApplytoAll.Enabled = true;
			}
		}

		private void BtnConnectionParametersChanged(object sender, EventArgs e)
		{
			Button btn = sender as Button;
			UserControl control = groConParamControls[(short)m_connectionParamId];

			// Hide the last groConnectionParamControl
			control.Visible = false;

			// Show the Selected groConnectionParamControl
			m_connectionParamId = (ConnectionParamId)Convert.ToInt32(btn.Tag.ToString());
			groConParamControls[(short)m_connectionParamId].Visible = true;
		}

		private void BtnNextandPrevious(object sender, EventArgs e)
		{
			// This method is to create, modify the connectors and interact with connection parameter's forms
			Button button = sender as Button;
			ConnectorBuilder ins = (ConnectorBuilder)(m_Handler.Instance);

			if (m_conElemInfoList.Count > 0)
			{
				if (button.Tag.ToString() == "Previous")
				{
					if (m_conInfoList[m_conIndex] != null)
						ModifyConnectionParameters();
					if (m_conIndex > 0)
						m_conIndex--;
					if (m_conInfoList[m_conIndex] != null)
						SetConnectionParameters();
				}
				else if (button.Tag.ToString() == "Next")
				{
					if (m_conInfoList[m_conIndex] != null)
						ModifyConnectionParameters();
					if (m_conIndex < m_conInfoList.Count - 1)
						m_conIndex++;
					if (m_conInfoList[m_conIndex] != null)
						SetConnectionParameters();
				}
				ins.SelectElementsLinkWithConnector(m_conElemInfoList[m_conIndex]);
				if (m_conIndex < 1)
				{
					btnPrevious.Enabled = false;
					btnNext.Enabled = true;
				}
				else if (m_conIndex > m_conElemInfoList.Count - 2)
				{
					btnPrevious.Enabled = true;
					btnNext.Enabled = false;
				}
				else
				{
					btnPrevious.Enabled = true;
					btnNext.Enabled = true;
				}
			}
			else
			{
				return;
			}
			picPreviewFront.Invalidate();
			picPreviewPlan.Invalidate();
		}

		public void CreateConnections()
		{
			ConnectorBuilder cb = (ConnectorBuilder)(m_Handler.Instance);

			if (chkApplytoAll.Checked)
			{
				for (int index = 0; index < m_conElemInfoList.Count; index++)
				{
					TConInfo conInfo = new TConInfo
					{
						ConElemInfo = m_conElemInfoList[index],
						ConPositionParam = groPosition.TConPositionParams,
						ConFinFCParam = groFinFilletCover.ConFinFCParam,
						ConFixingParam = groFixing.ConFixingParam,
						ConSymbol = cb.GetConnectorData().ConnectionSymbol,
					};
					m_conInfoList.Add(conInfo);
				}
			}
			else
			{
				for (int index = 0; index < m_conElemInfoList.Count; index++)
				{
					TConInfo conInfo = new TConInfo
					{
						ConElemInfo = m_conElemInfoList[index],
						ConPositionParam = groPosition.TConPositionParams,
						ConFinFCParam = groFinFilletCover.ConFinFCParam,
						ConFixingParam = groFixing.ConFixingParam,
						ConSymbol = cb.GetConnectorData().ConnectionSymbol,
					};
					groPosition.UpdateParameters(new TConPositionParam
					{
						Secondary_Height = m_conElemInfoList[index].Secondary_Element_Height,
						Secondary_Width = m_conElemInfoList[index].Secondary_Element_Width,
					},
						conInfo.ConTypeParam.Geometry_TConnector_Width,
						conInfo.ConTypeParam.Geometry_TConnector_Height);
					groFinFilletCover.UpdateParameters(new TConFinFCParam());
					groFixing.UpdateParameters(new TConFixingParam());
					m_conInfoList.Add(conInfo);
				}
			}
		}

		public void ModifyConnectionParameters()
		{
			if (m_conIndex < m_conInfoList.Count)
			{
				TConInfo conInfo = m_conInfoList[m_conIndex];
				conInfo.ConPositionParam = groPosition.TConPositionParams;
				conInfo.ConFinFCParam = groFinFilletCover.ConFinFCParam;
				conInfo.ConFixingParam = groFixing.ConFixingParam;
				m_conInfoList[m_conIndex] = conInfo;
			}
		}

		public void SetConnectionParameters()
		{
			if (m_conIndex < m_conInfoList.Count)
			{
				TConInfo conInfo = m_conInfoList[m_conIndex];
				groPosition.UpdateParameters(conInfo.ConPositionParam
					, conInfo.ConTypeParam.Geometry_TConnector_Width
					, conInfo.ConTypeParam.Geometry_TConnector_Height);
				groFinFilletCover.UpdateParameters(conInfo.ConFinFCParam);
				groFixing.UpdateParameters(conInfo.ConFixingParam);
			}
		}

		private void BtnPreview_Click(object sender, EventArgs e)
		{
			ModifyConnectionParameters();
			if (m_conIndex < m_conInfoList.Count)
			{
				picPreviewFront.Invalidate();
				picPreviewPlan.Invalidate();
			}

		}

		private void ChkApplytoAll_CheckedChanged(object sender, EventArgs e)
		{
			btnPrevious.Enabled = !chkApplytoAll.Checked;
			btnNext.Enabled = !chkApplytoAll.Checked;
		}

		private void picPreviewFront_Paint(object sender, PaintEventArgs e)
		{
			if (m_conIndex >= 0 && m_conIndex < m_conInfoList.Count)
			{
				Size size = picPreviewFront.Size;
				m_conInfoList[m_conIndex].DrawPreview(e.Graphics, size, 1);
			}
			else
			{
				e.Graphics.Clear(System.Drawing.Color.White);
			}
		}

		private void picPreviewPlan_Paint(object sender, PaintEventArgs e)
		{
			if (m_conIndex >= 0 && m_conIndex < m_conInfoList.Count)
			{
				Size size = picPreviewPlan.Size;
				m_conInfoList[m_conIndex].DrawPreview(e.Graphics, size, 0);
			}
			else
			{
				e.Graphics.Clear(System.Drawing.Color.White);
			}
		}
	}
}
