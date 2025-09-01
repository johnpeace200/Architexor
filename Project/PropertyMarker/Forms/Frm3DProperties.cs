using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Architexor.Controllers;
using Architexor.Request;
using Architexor.Utils;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TextBox = System.Windows.Forms.TextBox;

namespace Architexor.Forms
{
	public partial class Frm3DProperties : System.Windows.Forms.Form, IExternal
	{
		// The dialog owns the handler and the event objects,
		// but it is not a requirement. They may as well be static properties
		// of the application.
		protected ArchitexorRequestHandler m_Handler;
		protected ExternalEvent m_ExEvent;

		public ArchitexorRequestHandler Handler { get => m_Handler; }

		public ArchitexorRequestId LastRequestId { get; set; } = ArchitexorRequestId.None;

		private struct ListItem
		{
			public Autodesk.Revit.DB.Parameter Data;
			public override string ToString()
			{
				if (Data == null)
					return "Structural Span Arrow";

#if DEBUG
				Autodesk.Revit.DB.InternalDefinition id = Data.Definition as Autodesk.Revit.DB.InternalDefinition;
				return Data.Definition.Name + "(" + id.BuiltInParameter.ToString() + ")";
#else
				return Data.Definition.Name;
#endif
			}
		}

		private enum State
		{
			Init = 0x00,
			ElementsSelected = 0x01
		}
		private State m_State = State.Init;

		public Frm3DProperties(ArchitexorRequestId reqId, UIApplication uiApp)
		{
			InitializeComponent();

			// A new handler to handle request posting by the dialog
			m_Handler = new ArchitexorRequestHandler(PropertyMarker.Application.thisApp, reqId, uiApp);

			// External Event for the dialog to use (to post requests)
			m_ExEvent = ExternalEvent.Create(m_Handler);

			WakeUp();
		}

		public void DozeOff()
		{
			EnableCommands(false);
		}

		public ArchitexorRequestId GetRequestId()
		{
			if (m_Handler == null)
			{
				return ArchitexorRequestId.None;
			}
			return m_Handler.RequestId;
		}

		public void IClose()
		{
			Close();
		}

		public bool IIsDisposed()
		{
			return IsDisposed;
		}

		public void IShow()
		{
			Show();
		}

		public bool IVisible()
		{
			return Visible;
		}

		public void MakeRequest(ArchitexorRequestId request)
		{
			LastRequestId = request;

			m_Handler.Request.Make(request);
			m_ExEvent.Raise();
			DozeOff();
		}

		public void WakeUp(bool bFinish = false)
		{
			if (bFinish)
			{
				Close();
				return;
			}
			EnableCommands(true);

			Controllers.PropertyMarker ins = (Controllers.PropertyMarker)(m_Handler.Instance);

			if (ins.Elements.Count == 0)
			{
				btnSelectElement.Text = "Select";
				m_State = State.Init;
			}
			else
			{
				if (LastRequestId == ArchitexorRequestId.SelectPropertyElements
					|| LastRequestId == ArchitexorRequestId.SelectPropertyElementsToUpdate)
				{
					m_State = State.ElementsSelected;
					btnSelectElement.Text = "Select(" + ins.Elements.Count.ToString() + ")";

					RefreshParameterList();

					if (ins.PropertyMarkers.Count > 0)
					{
						string sTopParameter = "", sBottomParameter = "";
						int bShowSSA = 0;
						for (int i = 0; i < ins.PropertyMarkers.Count; i++)
						{
							FamilyInstance fi = ins.PropertyMarkers[i];
							string sTop = fi.LookupParameter("ATX_GUID_Top").AsString()
								, sBottom = fi.LookupParameter("ATX_GUID_Bottom").AsString();
							int isShowSSA = fi.LookupParameter("ATX_Show_Structural Span Direction Arrow").AsInteger();
							if (i == 0)
							{
								sTopParameter = sTop;
								sBottomParameter = sBottom;
								bShowSSA = isShowSSA;
							}
							else if (sTopParameter != sTop
								&& sBottomParameter != sBottom
								&& bShowSSA != isShowSSA)
							{
								sTopParameter = "";
								sBottomParameter = "";
								bShowSSA = 1;
								break;
							}
						}

						if (bShowSSA == 1)
							chkIncludeArrow.Checked = true;

						lstSelectedParameters.Items.Clear();
						if (sTopParameter != "")
						{
							string[] arrParams = sTopParameter.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
							for (int i = 0; i < arrParams.Length; i += 2)
							{
								foreach (Parameter p in ins.PublicParams)
								{
#if REVIT2024 || REVIT2025
									if (p.Id.Value == int.Parse(arrParams[i]))
#else
									if (p.Id.IntegerValue == int.Parse(arrParams[i]))
#endif
									{
										lstSelectedParameters.Items.Add(new ListItem() { Data = p });
									}
								}
							}
						}
						lstSelectedParameters.Items.Add(new ListItem() { Data = null });
						if (sBottomParameter != "")
						{
							string[] arrParams = sBottomParameter.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
							for (int i = 0; i < arrParams.Length; i += 2)
							{
								foreach (Parameter p in ins.PublicParams)
								{
#if REVIT2024 || REVIT2025
									if (p.Id.Value == int.Parse(arrParams[i]))
#else
                                    if (p.Id.IntegerValue == int.Parse(arrParams[i]))
#endif
									{
										lstSelectedParameters.Items.Add(new ListItem() { Data = p });
									}
								}
							}
						}
					}
				}
			}

			SetControlsState();

			Activate();
		}

		private void EnableCommands(bool status)
		{
			foreach (System.Windows.Forms.Control ctrl in this.Controls)
			{
				ctrl.Enabled = status;
			}
			if (!status)
			{
				btnCancel.Enabled = true;
			}
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

		private void Frm3DProperties_Load(object sender, EventArgs e)
		{
			txtVersion.Text = string.Format("{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

			txtSearch.Text = "Search";
			txtSearch.GotFocus += new System.EventHandler(RemoveText);
			txtSearch.LostFocus += new System.EventHandler(AddText);

			lstSelectedParameters.Items.Add(new ListItem() { Data = null });

			//if(m_Handler.RequestId == ArchitexorRequestId.PropertyMarkerUpdate)
			//{
			//chkFlipSide.Visible = false;
			//}
		}

		private void RemoveText(object sender, EventArgs e)
		{
			TextBox txt = (TextBox)sender;
			if (txt.Text == "Search")
			{
				txt.Text = "";
			}
		}

		private void AddText(object sender, EventArgs e)
		{
			TextBox txt = (TextBox)sender;
			if (string.IsNullOrWhiteSpace(txt.Text))
				txt.Text = "Search";
		}

		private void Frm3DProperties_FormClosed(object sender, FormClosedEventArgs e)
		{
			// we own both the event and the handler
			// we should dispose it before we are closed
			//m_ExEvent.Dispose();
			//m_ExEvent = null;
			//m_Handler = null;

			// do not forget to call the base class
			base.OnFormClosed(e);
		}

		private bool Serialize()
		{
			Controllers.PropertyMarker ins = (Controllers.PropertyMarker)(m_Handler.Instance);

			try
			{
				ins.Thickness = int.Parse(txtThickness.Text);
				ins.Scale = int.Parse(txtScale.Text);
				ins.SelectedTopParams.Clear();
				ins.SelectedBottomParams.Clear();
				if (lstSelectedParameters.Items.Count <= 1)
				{
					MessageBox.Show("Please select at least one parameter.");
					return false;
				}
				bool bTop = true;
				foreach (object obj in lstSelectedParameters.Items)
				{
					if (((ListItem)obj).Data != null)
					{
						if (bTop)
							ins.SelectedTopParams.Add(((ListItem)obj).Data);
						else
							ins.SelectedBottomParams.Add(((ListItem)obj).Data);
					}
					else
						bTop = false;
				}
				ins.Flip = chkFlipSide.Checked;
				ins.IncludeArrow = chkIncludeArrow.Checked;
			}
			catch (Exception)
			{
				MessageBox.Show("Please input value correctly.");
				return false;
			}

			return true;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (Serialize())
			{
				if (m_Handler.RequestId == ArchitexorRequestId.PropertyMarker)
					MakeRequest(ArchitexorRequestId.ArrangePropertyMarkers);
				else if (m_Handler.RequestId == ArchitexorRequestId.PropertyMarkerUpdate)
					MakeRequest(ArchitexorRequestId.UpdatePropertyMarkers);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnHelp_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("https://www.futuretimber.com/");
		}

		private void btnSelectElement_Click(object sender, EventArgs e)
		{
			if (m_Handler.RequestId == ArchitexorRequestId.PropertyMarker)
				MakeRequest(ArchitexorRequestId.SelectPropertyElements);
			else if (m_Handler.RequestId == ArchitexorRequestId.PropertyMarkerUpdate)
				MakeRequest(ArchitexorRequestId.SelectPropertyElementsToUpdate);
		}

		private void btnClearElement_Click(object sender, EventArgs e)
		{
			Controllers.PropertyMarker ins = (Controllers.PropertyMarker)(m_Handler.Instance);
			ins.Elements.Clear();
			btnSelectElement.Text = "Select";

			lstParameters.Items.Clear();
			lstSelectedParameters.Items.Clear();
			lstSelectedParameters.Items.Add(new ListItem() { Data = null });

			m_State = State.Init;
			SetControlsState();
		}

		private void btnAddParameter_Click(object sender, EventArgs e)
		{
			foreach (object obj in lstParameters.SelectedItems)
			{
				ListItem item = (ListItem)obj;

				bool bExist = false;
				foreach (object obj1 in lstSelectedParameters.Items)
				{
					ListItem item1 = (ListItem)obj1;
					if (item1.Data != null && item1.Data.Id == item.Data.Id)
					{
						bExist = true;
						break;
					}
				}
				if (!bExist)
					lstSelectedParameters.Items.Add(item);
			}
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			List<ListItem> items = new List<ListItem>();
			foreach (object obj in lstSelectedParameters.SelectedItems)
			{
				if (((ListItem)obj).Data != null)
					items.Add((ListItem)obj);
			}
			foreach (ListItem item in items)
			{
				lstSelectedParameters.Items.Remove(item);
			}
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectNextPropertyElement);
		}

		private void btnPrev_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectPreviousPropertyElement);
		}

		private void btnUp_Click(object sender, EventArgs e)
		{
			MoveItem(-1);
		}

		public void MoveItem(int direction)
		{
			// Checking selected item
			if (lstSelectedParameters.SelectedItem == null || lstSelectedParameters.SelectedIndex < 0)
				return; // No selected item - nothing to do

			// Calculate new index using move direction
			int newIndex = lstSelectedParameters.SelectedIndex + direction;

			// Checking bounds of the range
			if (newIndex < 0 || newIndex >= lstSelectedParameters.Items.Count)
				return; // Index out of range - nothing to do

			object selected = lstSelectedParameters.SelectedItem;

			// Removing removable element
			lstSelectedParameters.Items.Remove(selected);
			// Insert it in new position
			lstSelectedParameters.Items.Insert(newIndex, selected);
			// Restore selection
			lstSelectedParameters.SetSelected(newIndex, true);
		}

		private void btnDown_Click(object sender, EventArgs e)
		{
			MoveItem(1);
		}

		private void txtSearch_TextChanged(object sender, EventArgs e)
		{
			RefreshParameterList();
		}

		private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
		{
			RefreshParameterList();
		}

		private void RefreshParameterList()
		{
			string sKeyword = "";
			if (txtSearch.Text != "Search")
				sKeyword = txtSearch.Text.Trim();

			lstParameters.Items.Clear();

			Controllers.PropertyMarker ins = (Controllers.PropertyMarker)(m_Handler.Instance);
			if (ins.PrivateInstanceParams.Count == 0)
				return;

			List<Parameter> paras = null;
			switch (cmbFilter.Text)
			{
				case "Instance":
					paras = ins.PrivateInstanceParams[ins.CurrentElement];
					break;
				case "Type":
					paras = ins.PrivateTypeParams[ins.CurrentElement];
					break;
				case "Both":
					paras = new List<Parameter>();
					foreach (Parameter p in ins.PrivateInstanceParams[ins.CurrentElement])
					{
						paras.Add(p);
					}
					foreach (Parameter p in ins.PrivateTypeParams[ins.CurrentElement])
					{
						bool bExist = false;
						foreach (Parameter p1 in paras)
						{
							if (p.Id == p1.Id)
							{
								bExist = true;
								break;
							}
						}
						if (!bExist)
							paras.Add(p);
					}
					break;
				default:
					paras = ins.PublicParams;
					break;
			}

			for (int i = 0; i < paras.Count; i++)
			{
				if (paras[i].Definition.Name.ToLower().Contains(sKeyword.ToLower()))
					lstParameters.Items.Add(new ListItem() { Data = paras[i] });
			}
		}

		private void lstParameters_DoubleClick(object sender, EventArgs e)
		{
			btnAddParameter_Click(null, null);
		}
	}
}
