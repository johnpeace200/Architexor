using Architexor.Core.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Architexor.Forms;
using Architexor.Request;
using Architexor.Utils;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Splitter = Architexor.BasicSplit.Splitters.Splitter;
using Color = System.Drawing.Color;
using Settings = Architexor.BasicSplit.Base.Settings;
using Control = System.Windows.Forms.Control;
using Architexor.BasicSplit.Splitters;
using ATXComponents.Widgets;
using Newtonsoft.Json.Linq;
using Architexor.Core;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using Architexor.Base;

namespace Architexor.BasicSplit.Forms
{
	public partial class FrmSplit : System.Windows.Forms.Form, IExternal
	{
		// The dialog owns the handler and the event objects,
		// but it is not a requirement. They may as well be static properties
		// of the application.
		protected ArchitexorRequestHandler m_Handler;
		protected ExternalEvent m_ExEvent;
		public ArchitexorRequestHandler Handler { get => m_Handler; }

		public ArchitexorRequestId LastRequestId { get; set; } = ArchitexorRequestId.None;

		private int m_nCurrentElement = 0;

		protected PointF m_HighlightPos = new PointF(0, 0);

		public FrmSplit(ArchitexorRequestId reqId, UIApplication uiApp)
		{
			InitializeComponent();

			//	Temp
			btnHelp.Enabled = false;
#if RELEASE
			grpDebug.Visible = false;
#endif

			InitStatisticControls();

			// A new handler to handle request posting by the dialog
			m_Handler = new ArchitexorRequestHandler(PanelTool.Application.thisApp, reqId, uiApp);

			// External Event for the dialog to use (to post requests)
			m_ExEvent = ExternalEvent.Create(m_Handler);

			Settings.Initialize();

			WakeUp();
		}

		protected void InitStatisticControls()
		{
			//	Add statistic labels
			Label label = new Label();
			label.Text = "Net area";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpIndividualArea.Controls.Add(label, 0, 0);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpIndividualArea.Controls.Add(label, 1, 0);
			//Control ctrl = tlpIndividualArea.GetControlFromPosition(0, 0);

			label = new Label();
			label.Text = "Gross Area";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpIndividualArea.Controls.Add(label, 0, 1);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpIndividualArea.Controls.Add(label, 1, 1);


			label = new Label();
			label.Text = "Bounding Area";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpIndividualArea.Controls.Add(label, 0, 2);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpIndividualArea.Controls.Add(label, 1, 2);

			tlpIndividualArea.BackColor = Color.White;
			tlpIndividualArea.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;


			label = new Label();
			label.Text = "Part count";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpIndividualPart.Controls.Add(label, 0, 0);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpIndividualPart.Controls.Add(label, 1, 0);

			tlpIndividualPart.BackColor = Color.White;
			tlpIndividualPart.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			Padding padding = new Padding(0);
			Accordion accIndividual = new Accordion();
			panelIndividualStats.Controls.Add(accIndividual);
			tlpIndividualArea.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			accIndividual.Add(tlpIndividualArea, "Area individual(m²)", "", 0, true, addResizeBar: false, contentPadding: padding, contentMargin: padding);
			tlpIndividualPart.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			accIndividual.Add(tlpIndividualPart, "Part individual", "", 0, true, addResizeBar: false, contentPadding: padding, contentMargin: padding);

			tlpTotalSelectedElements.BackColor = Color.White;
			tlpTotalSelectedElements.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			label = new Label();
			label.Text = "Elements to split";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalSelectedElements.Controls.Add(label, 0, 0);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalSelectedElements.Controls.Add(label, 1, 0);


			label = new Label();
			label.Text = "Part length";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalSelectedElements.Controls.Add(label, 0, 1);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalSelectedElements.Controls.Add(label, 1, 1);

			label = new Label();
			label.Text = "Lintel and Sill";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalSelectedOpenings.Controls.Add(label, 0, 0);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalSelectedOpenings.Controls.Add(label, 1, 0);

			label = new Label();
			label.Text = "Lintel bearing length";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalSelectedOpenings.Controls.Add(label, 0, 1);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalSelectedOpenings.Controls.Add(label, 1, 1);

			label = new Label();
			label.Text = "Sill bearing length";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalSelectedOpenings.Controls.Add(label, 0, 2);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalSelectedOpenings.Controls.Add(label, 1, 2);

			label = new Label();
			label.Text = "Around Centre";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalSelectedOpenings.Controls.Add(label, 0, 3);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalSelectedOpenings.Controls.Add(label, 1, 3);

			label = new Label();
			label.Text = "Around centre length";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalSelectedOpenings.Controls.Add(label, 0, 4);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalSelectedOpenings.Controls.Add(label, 1, 4);

			label = new Label();
			label.Text = "Centre";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalSelectedOpenings.Controls.Add(label, 0, 5);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalSelectedOpenings.Controls.Add(label, 1, 5);

			label = new Label();
			label.Text = "Equal distance between";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalSelectedOpenings.Controls.Add(label, 0, 6);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalSelectedOpenings.Controls.Add(label, 1, 6);

			label = new Label();
			label.Text = "Adjacent walls/slabs";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalSelectedOpenings.Controls.Add(label, 0, 7);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalSelectedOpenings.Controls.Add(label, 1, 7);

			tlpTotalSelectedOpenings.BackColor = Color.White;
			tlpTotalSelectedOpenings.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			label = new Label();
			label.Text = "Net area";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalArea.Controls.Add(label, 0, 0);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalArea.Controls.Add(label, 1, 0);

			label = new Label();
			label.Text = "Gross Area";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalArea.Controls.Add(label, 0, 1);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalArea.Controls.Add(label, 1, 1);

			label = new Label();
			label.Text = "Bounding Area";
			label.TextAlign = ContentAlignment.MiddleRight;
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			tlpTotalArea.Controls.Add(label, 0, 2);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalArea.Controls.Add(label, 1, 2);

			tlpTotalArea.BackColor = Color.White;
			tlpTotalArea.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			label = new Label();
			label.Text = "Part count";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			tlpTotalPart.Controls.Add(label, 0, 0);

			label = new Label();
			label.Text = "0";
			label.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
			label.TextAlign = ContentAlignment.MiddleRight;
			label.AutoSize = true;
			tlpTotalPart.Controls.Add(label, 1, 0);

			tlpTotalPart.BackColor = Color.White;
			tlpTotalPart.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

			Accordion accTotal = new Accordion();
			panelTotalStats.Controls.Add(accTotal);
			tlpTotalSelectedElements.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			accTotal.Add(tlpTotalSelectedElements, "Selected Elements && Parameters", "", 0, true, addResizeBar: false, contentPadding: padding, contentMargin: padding);
			tlpTotalSelectedOpenings.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			accTotal.Add(tlpTotalSelectedOpenings, "Selected Openings && Parameters", "", 0, true, addResizeBar: false, contentPadding: padding, contentMargin: padding);
			tlpTotalArea.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			accTotal.Add(tlpTotalArea, "Area total(m²)", "", 0, true, addResizeBar: false, contentPadding: padding, contentMargin: padding);
			tlpTotalPart.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			accTotal.Add(tlpTotalPart, "Part total", "", 0, true, addResizeBar: false, contentPadding: padding, contentMargin: padding);
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
			btnHelp.Enabled = false;
			btnAIPick.Enabled = false;
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

			Splitter ins = (Splitter)(m_Handler.Instance);

			Activate();

			if (LastRequestId == ArchitexorRequestId.SelectStartPoint)
			{
				if (ins.SplitElements[m_nCurrentElement].StartPoint == null)
				{
					ins.SplitElements[m_nCurrentElement].MustSelectStartPoint = false;
					rdoPickStartPt.Checked = false;
					rdoLeftRight.Checked = true;
				}
				else
				{
					ins.SplitElements[m_nCurrentElement].MustSelectStartPoint = true;
				}
			}

			if (LastRequestId == ArchitexorRequestId.SelectSplitElements)
			{
				//if(ins.SplitElements.Count == ins.Elements.Count)
				//{
				//	Clicked Cancel
				//}
				//else {
				panelThumnails.Controls.Clear();

				if (ins.Elements.Count == 0)
					return;

				//	Show Progress Bar
				ProgressDialog progressDialog = new();

				//	Read Elements
				progressDialog.SetMessage("Reading Elements");
				progressDialog.SetMaximum(ins.Elements.Count);

				if (ins is WallSplitter)
				{
					Task.Run(() =>
					{
						int nIndex = 0, nCount = ins.Elements.Count;
						ins.Failures.Clear();

						//	Get exact scrollbar width
						int scrollBarWidth = SystemInformation.VerticalScrollBarWidth;
						if (nCount < panelThumnails.Height / 130)
						{
							scrollBarWidth = 0;
						}

						for (int i = 0; i < ins.Elements.Count; i++)
						{
							Element e = ins.Elements[i];
							progressDialog.SetDescription("Processing " + (i + 1 + ins.Failures.Count) + " of " + nCount + " elements");

							CSplitElement se;
							try
							{
								se = new CSplitWallElement(ins, e);
								ins.SplitElements.Add(se);
							}
							catch (Exception ex)
							{
								ins.Failures.Add(e.Id);
								ins.Elements.RemoveAt(i);
								i--;
								continue;
							}

							CThumbnail thumb = new CThumbnail();
							thumb.Image = se.Thumbnail;
							thumb.Width = panelThumnails.Width - scrollBarWidth - 2;
							thumb.Index = nIndex++;
							panelThumnails.Invoke(new Action(() => panelThumnails.Controls.Add(thumb)));
							thumb.Disposed += OnElementDisposed;

							thumb.Click += onThumbnail_Click;

							const int nLimitation = 1200;
							if (nIndex >= nLimitation)
							{
								Util.InfoMsg("Our plugin supports up to " + nLimitation + " elements to split at this moment due to the performance issue.\nSorry for the inconvenience.\nWe work hard to extend the limitation in the future upgrades.");
								ins.Elements.RemoveRange(nLimitation, ins.Elements.Count - nLimitation);
								break;
							}

							progressDialog.UpdateProgress(nIndex);
						}

						m_nCurrentElement = -1;
						progressDialog.Invoke(new Action(() =>
						{
							progressDialog.Close();

							UpdateTotalStats();

							//	Force to select the first item
							if (m_nCurrentElement == -1)
								SelectElement();

							SetControlsState();

							if (ins.Failures.Count > 0)
							{
								MakeRequest(ArchitexorRequestId.ShowFailureList);
							}
						}));
					});
					Activate();
					progressDialog.ShowDialog(this);
				}
				else
				{
					int nIndex = 0, nCount = ins.Elements.Count;
					ins.Failures.Clear();

					//	Get exact scrollbar width
					int scrollBarWidth = SystemInformation.VerticalScrollBarWidth;
					if (nCount < panelThumnails.Height / 130)
					{
						scrollBarWidth = 0;
					}

					for (int i = 0; i < ins.Elements.Count; i++)
					{
						Element e = ins.Elements[i];
						progressDialog.SetDescription("Processing " + (i + 1 + ins.Failures.Count) + " of " + nCount + " elements");

						CSplitElement se;
						try
						{
							se = new CSplitSlabElement(ins, e);
							ins.SplitElements.Add(se);
						}
						catch (Exception ex)
						{
							ins.Failures.Add(e.Id);
							ins.Elements.RemoveAt(i);
							i--;
							continue;
						}

						CThumbnail thumb = new CThumbnail();
						thumb.Image = se.Thumbnail;
						thumb.Width = panelThumnails.Width - scrollBarWidth - 2;
						thumb.Index = nIndex++;
						panelThumnails.Invoke(new Action(() => panelThumnails.Controls.Add(thumb)));
						thumb.Disposed += OnElementDisposed;

						thumb.Click += onThumbnail_Click;

						const int nLimitation = 100;
						if (nIndex >= nLimitation)
						{
							Util.InfoMsg("Our plugin supports up to " + nLimitation + " elements to split at this moment due to the performance issue.\nSorry for the inconvenience.\nWe work hard to extend the limitation in the future upgrades.");
							ins.Elements.RemoveRange(nLimitation, ins.Elements.Count - nLimitation);
							break;
						}

						progressDialog.UpdateProgress(nIndex);
					}

					m_nCurrentElement = -1;
					//progressDialog.Invoke(new Action(() =>
					//{
						progressDialog.Close();

						UpdateTotalStats();

						//	Force to select the first item
						if (m_nCurrentElement == -1)
							SelectElement();

						SetControlsState();

						if (ins.Failures.Count > 0)
						{
							MakeRequest(ArchitexorRequestId.ShowFailureList);
						}
					//}));

					Activate();
				}
			}
			else if (ins.SplitElements.Count > 0)
			{
				DrawPreview();
				UpdateIndividualStats();
				UpdateTotalStats();
			}

			((Label)tlpTotalSelectedElements.GetControlFromPosition(1, 0)).Text =
				ins.SplitElements.Count.ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 0)).Text =
				ins.GetOpeningCount(OpeningSelectOption.LintelAndSill).ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 3)).Text =
				ins.GetOpeningCount(OpeningSelectOption.AroundCentreOfOpening).ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 6)).Text =
				ins.GetOpeningCount(OpeningSelectOption.EqualDistanceBetweenOpenings).ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 5)).Text =
				ins.GetOpeningCount(OpeningSelectOption.CentreOfOpening).ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 7)).Text =
				ins.AdjacentElements.Count.ToString();
		}

		private void onThumbnail_Click(object sender, EventArgs e)
		{
			CThumbnail thumb = ((CThumbnail)sender);
			m_nCurrentElement = thumb.Index;

			Splitter ins = (Splitter)(m_Handler.Instance);
			ins.CurrentElement = m_nCurrentElement;

			SelectElement();
		}

		private void SelectElement()
		{
			Splitter splitter = ((Splitter)(m_Handler.Instance));

			if (splitter.SplitElements.Count > 0
				&& m_nCurrentElement < 0)
			{
				m_nCurrentElement = 0;
			}
			else if (m_nCurrentElement >= splitter.SplitElements.Count)
			{
				m_nCurrentElement = splitter.SplitElements.Count - 1;
			}
			splitter.CurrentElement = m_nCurrentElement;

			//	Deselect Other elements
			foreach (Control c in panelThumnails.Controls)
			{
				((CThumbnail)c).SetSelected(false);
			}

			if (m_nCurrentElement >= 0)
			{
				btnViewInRevit.Visible = true;

				CThumbnail thumb = (CThumbnail)panelThumnails.Controls[m_nCurrentElement];
				thumb.SetSelected(true);

				DrawPreview();

				UpdateIndividualStats();

				CSplitElement se = splitter.SplitElements[m_nCurrentElement];
				//	Show Parameters
				txtWidth.Text = se.StandardSplitWidth.ToString();
				txtLintel.Text = se.LintelBearing.ToString();
				txtCill.Text = se.CillBearing.ToString();
				txtCentredWidth.Text = se.AroundCentreSplitWidth.ToString();
				rdoLeftRight.Checked = se.SplitFromLeftToRight;
				rdoRightLeft.Checked = !se.SplitFromLeftToRight;
				rdoPickStartPt.Checked = se.MustSelectStartPoint;
			}
			else
			{
				picPreview.Image = null;
				//Graphics graphics = picPreview.CreateGraphics();
				//graphics.Clear(Color.White);
			}
		}

		private void Initialize()
		{
			Splitter ins = (Splitter)(m_Handler.Instance);
			ins.Elements.Clear();
			ins.SplitElements.Clear();
			ins.ClearOpenings(OpeningSelectOption.None);

			//	Remove Thumbnails
			m_nCurrentElement = -1;
			panelThumnails.Controls.Clear();

			btnViewInRevit.Visible = false;

			SelectElement();

			SetControlsState();
		}

		private bool Serialize()
		{
			Splitter ins = (Splitter)(m_Handler.Instance);
			if (ins.SplitElements.Count == 0)
			{
				MessageBox.Show("Please select elements to split.");
				return false;
			}

			//	Validation check for the input parameters
			/*if (!int.TryParse(txtWidth.Text, out int nTemp) || nTemp <= 0)
			{
				MessageBox.Show("Please input width correctly.");
				return false;
			}

			ins.StandardSplitWidth = nTemp;

			int.TryParse(txtLintel.Text, out nTemp);
			ins.LintelBearing = nTemp;
			int.TryParse(txtCill.Text, out nTemp);
			ins.CillBearing = nTemp;

			if (ins.GetOpeningCount(OpeningSelectOption.AroundCentreOfOpening) > 0
				&& (!int.TryParse(txtCentredWidth.Text, out nTemp) || nTemp <= 0))
			{
				MessageBox.Show("Please input around centre width correctly.");
				return false;
			}
			int.TryParse(txtCentredWidth.Text, out nTemp);
			ins.AroundCentreSplitWidth = nTemp;

			ins.MustSelectStartPoint = rdoPickStartPt.Checked;
			ins.SplitFromLeftToRight = rdoLeftRight.Checked;*/

			return true;
		}

		private void UpdateTotalStats()
		{
			Splitter splitter = ((Splitter)(m_Handler.Instance));
			int nTotalPanelCount = 0;
			double fNetAreaTotal = 0.0, fGrossAreaTotal = 0.0, fBoundingAreaTotal = 0.0;
			foreach (CSplitElement se in splitter.SplitElements)
			{
				nTotalPanelCount += se.Panels.Count;
				fNetAreaTotal += UnitUtils.ConvertFromInternalUnits(
							se.GetArea(0)
							, CCommonSettings.SquareUnit) / 1000000;
				fGrossAreaTotal += UnitUtils.ConvertFromInternalUnits(
							se.GetArea(1)
							, CCommonSettings.SquareUnit) / 1000000;
				fBoundingAreaTotal += UnitUtils.ConvertFromInternalUnits(
							se.GetArea(2)
							, CCommonSettings.SquareUnit) / 1000000;
			}
			((Label)tlpTotalPart.GetControlFromPosition(1, 0)).Text =
					nTotalPanelCount.ToString();
			((Label)tlpTotalArea.GetControlFromPosition(1, 0)).Text =
				Math.Round(fNetAreaTotal, 3).ToString();
			((Label)tlpTotalArea.GetControlFromPosition(1, 1)).Text =
				Math.Round(fGrossAreaTotal, 3).ToString();
			((Label)tlpTotalArea.GetControlFromPosition(1, 2)).Text =
				Math.Round(fBoundingAreaTotal, 3).ToString();

			((Label)tlpTotalSelectedElements.GetControlFromPosition(1, 0)).Text =
					splitter.SplitElements.Count.ToString();
			((Label)tlpTotalSelectedElements.GetControlFromPosition(1, 1)).Text =
					splitter.StandardSplitWidth.ToString();

			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 0)).Text =
					splitter.GetOpeningCount(OpeningSelectOption.LintelAndSill).ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 1)).Text =
					splitter.LintelBearing.ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 2)).Text =
					splitter.CillBearing.ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 3)).Text =
					splitter.GetOpeningCount(OpeningSelectOption.AroundCentreOfOpening).ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 4)).Text =
					splitter.AroundCentreSplitWidth.ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 5)).Text =
					splitter.GetOpeningCount(OpeningSelectOption.CentreOfOpening).ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 6)).Text =
					splitter.GetOpeningCount(OpeningSelectOption.EqualDistanceBetweenOpenings).ToString();
			((Label)tlpTotalSelectedOpenings.GetControlFromPosition(1, 7)).Text =
					splitter.AdjacentElements.Count.ToString();
		}

		private void UpdateIndividualStats()
		{
			Splitter splitter = ((Splitter)(m_Handler.Instance));
			CSplitElement se = splitter.SplitElements[m_nCurrentElement];
			((Label)tlpIndividualPart.GetControlFromPosition(1, 0)).Text =
				se.Panels.Count.ToString();
			((Label)tlpIndividualArea.GetControlFromPosition(1, 0)).Text =
				Math.Round(
					UnitUtils.ConvertFromInternalUnits(
						se.GetArea(0)
						, CCommonSettings.SquareUnit) / 1000000, 3).ToString();
			((Label)tlpIndividualArea.GetControlFromPosition(1, 1)).Text =
				Math.Round(
					UnitUtils.ConvertFromInternalUnits(
						se.GetArea(1)
						, CCommonSettings.SquareUnit) / 1000000, 3).ToString();
			((Label)tlpIndividualArea.GetControlFromPosition(1, 2)).Text =
				Math.Round(
					UnitUtils.ConvertFromInternalUnits(
						se.GetArea(2)
						, CCommonSettings.SquareUnit) / 1000000, 3).ToString();
		}

		private void DrawPreview()
		{
			if (m_nCurrentElement < 0)
				return;

			Splitter ins = (Splitter)(m_Handler.Instance);
			ins.SplitElements[m_nCurrentElement].DrawPreview();
			picPreview.Image = ins.SplitElements[m_nCurrentElement].PreviewBitmap;
		}

		private PointF ConvertToFormPoint(XYZ pt, double fFactor, double fMaxWidth)
		{
			int nPadding = 10,
				nCanvasWidth = picPreview.Width - nPadding * 2,
				nCanvasHeight = picPreview.Height - nPadding * 2;
			return new PointF()
			{
				X = (float)(pt.X * fFactor + nPadding + (nCanvasWidth - fMaxWidth * fFactor) / 2),
				Y = (float)(nCanvasHeight - pt.Y * fFactor)
			};
		}

		private void SetControlsState()
		{
			Splitter ins = (Splitter)(m_Handler.Instance);

			if (ins.SplitElements.Count >= 1)
			{
				rdoPickStartPt.Enabled = true;
				rdoLeftRight.Enabled = true;
				rdoRightLeft.Enabled = true;

				btnAIPick.Enabled = false;
				btnSelectLC.Enabled = true;
				btnClearLC.Enabled = true;
				btnSelectAroundCentre.Enabled = true;
				btnClearAroundCentre.Enabled = true;
				btnSelectCentre.Enabled = true;
				btnClearCentre.Enabled = true;
				btnSelectEqual.Enabled = true;
				btnClearEqual.Enabled = true;
				btnSelectAdjacent.Enabled = true;
				btnClearAdjacent.Enabled = true;
				if (int.TryParse(txtWidth.Text, out int nTemp) && nTemp > 0)
				{
					btnOK.Enabled = true;
					btnApply.Enabled = true;
				}
				else
				{
					btnOK.Enabled = false;
					btnApply.Enabled = false;
				}
			}
			else
			{
				rdoPickStartPt.Enabled = false;
				rdoPickStartPt.Checked = false;
				rdoLeftRight.Enabled = false;
				rdoRightLeft.Enabled = false;

				btnAIPick.Enabled = false;
				btnSelectLC.Enabled = false;
				btnClearLC.Enabled = false;
				btnSelectAroundCentre.Enabled = false;
				btnClearAroundCentre.Enabled = false;
				btnSelectCentre.Enabled = false;
				btnClearCentre.Enabled = false;
				btnSelectEqual.Enabled = false;
				btnClearEqual.Enabled = false;
				btnSelectAdjacent.Enabled = false;
				btnClearAdjacent.Enabled = false;
				btnApply.Enabled = false;
				btnOK.Enabled = false;
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

		#region Event Handlers
		private void btnSelectLC_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectOpeningsForLC);
		}

		private void btnClearLC_Click(object sender, EventArgs e)
		{
			((Splitter)(m_Handler.Instance)).ClearOpenings(OpeningSelectOption.LintelAndSill);
		}

		private void btnSelectAroundCentre_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectOpeningsForAroundCentre);
		}

		private void btnClearAroundCentre_Click(object sender, EventArgs e)
		{
			((Splitter)(m_Handler.Instance)).ClearOpenings(OpeningSelectOption.AroundCentreOfOpening);
		}

		private void btnSelectCentre_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectOpeningsForCentre);
		}

		private void btnClearCentre_Click(object sender, EventArgs e)
		{
			((Splitter)(m_Handler.Instance)).ClearOpenings(OpeningSelectOption.CentreOfOpening);
		}

		private void btnSelectEqual_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectOpeningsForEqualDistanceBetween);
		}

		private void btnClearEqual_Click(object sender, EventArgs e)
		{
			((Splitter)(m_Handler.Instance)).ClearOpenings(OpeningSelectOption.EqualDistanceBetweenOpenings);
		}

		private void btnSelectAdjacent_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectAdjacentElements);
		}

		private void btnClearAdjacent_Click(object sender, EventArgs e)
		{
			((Splitter)(m_Handler.Instance)).ClearAdjacentElements();
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			if (Serialize())
			{
				MakeRequest(ArchitexorRequestId.PrepareSplit);
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (Serialize())
			{
				MakeRequest(ArchitexorRequestId.Split);
			}
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void btnHelp_Click(object sender, EventArgs e)
		{

		}

		private void rdoNet_CheckedChanged(object sender, EventArgs e)
		{
			if (rdoNet.Checked)
			{
				Settings.PreviewSettings.AreaType = 0;
				DrawPreview();
			}
		}

		private void rdoGross_CheckedChanged(object sender, EventArgs e)
		{
			if (rdoGross.Checked)
			{
				Settings.PreviewSettings.AreaType = 1;
				DrawPreview();
			}
		}

		private void rdoBounding_CheckedChanged(object sender, EventArgs e)
		{
			if (rdoBounding.Checked)
			{
				Settings.PreviewSettings.AreaType = 2;
				DrawPreview();
			}
		}

		private void btnSelect_Click(object sender, EventArgs e)
		{
			MakeRequest(ArchitexorRequestId.SelectSplitElements);
		}

		private async void btnAIPick_Click(object sender, EventArgs e)
		{
			Splitter ins = (Splitter)(m_Handler.Instance);

			//	Clear Selected Openings
			ins.ClearOpenings(OpeningSelectOption.None);

			JArray elements = new JArray();
			JObject element = new JObject();
			JArray openings;
			int nIndex, i;
			CSplitElement se;

			int nStart = 0, nEnd = ins.SplitElements.Count;
			for (nIndex = nStart; nIndex < nEnd; nIndex++)
			{
				se = ins.SplitElements[nIndex];
				double nLength = Math.Round(UnitUtils.ConvertFromInternalUnits(se.LocationCurve.Length, CCommonSettings.Unit), 3);

				JObject wall = new JObject();
				JArray coordinates = new JArray();
				foreach(Curve c in se.BoundaryCurves[0])
				{
					XYZ pt = c.GetEndPoint(0);

					JArray coordinate = new JArray();
					coordinate.Add(Math.Round(UnitUtils.ConvertFromInternalUnits(se.GetPoint2D(pt).X, CCommonSettings.Unit), 3));
					coordinate.Add(Math.Round(UnitUtils.ConvertFromInternalUnits(se.GetPoint2D(pt).Y, CCommonSettings.Unit), 3));
					coordinates.Add(coordinate);
				}
				wall.Add("coordinates", coordinates);

				element = new JObject();
				element.Add("wall", wall);

				openings = new JArray();
				foreach (COpening opening in se.AssociatedOpenings)
				{
					if (opening.Width == 0
						|| opening.Height == 0
						|| opening.Location == null)
						continue;

					JObject jOpening = new JObject();
#if REVIT2024 || REVIT2025
					jOpening.Add("id", opening.Id.Value);
#else
					jOpening.Add("id", opening.Id.IntegerValue);
#endif
					double x, y, width, height;
					width = Math.Round(UnitUtils.ConvertFromInternalUnits(opening.Width, CCommonSettings.Unit), 3);
					height = Math.Round(UnitUtils.ConvertFromInternalUnits(opening.Height, CCommonSettings.Unit), 3);
					x = Math.Round(UnitUtils.ConvertFromInternalUnits(se.GetPoint2D(opening.Location).X, CCommonSettings.Unit), 3);
					y = Math.Round(UnitUtils.ConvertFromInternalUnits(se.GetPoint2D(opening.Location).Y, CCommonSettings.Unit), 3);
					coordinates = new JArray();

					JArray coordinate = new JArray();
					coordinate.Add(x);	coordinate.Add(y);
					coordinates.Add(coordinate);

					coordinate = new JArray();
					coordinate.Add(x + width); coordinate.Add(y);
					coordinates.Add(coordinate);

					coordinate = new JArray();
					coordinate.Add(x + width); coordinate.Add(y + height);
					coordinates.Add(coordinate);

					coordinate = new JArray();
					coordinate.Add(x); coordinate.Add(y + height);
					coordinates.Add(coordinate);

					jOpening.Add("coordinates", coordinates);

					openings.Add(jOpening);
				}

				element.Add("openings", openings);
				elements.Add(element);
			}

			string _apiKey = "";
			HttpClient _httpClient = new HttpClient
			{
				BaseAddress = new Uri("https://api.openai.com/v1/")
			};
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
			string _systemPrompt = @"You are panelizing timber panels for construction.
The wall should be split into multiple panels based on optimized splitting methods. The goal is to minimize the number of panels while ensuring structural feasibility.

Splitting Options:
- Standard Split Width – The wall is split into equal-width panels.
- Lintel & Sill Bearing – The wall is split at openings, considering each of lintel and cill bearing lengths.
- Around Centre of Opening – The wall is split near the center of openings with a configurable offset.
- Center of Opening – The wall is split exactly at the center of the opening.
- Equal Distance Between Openings – Splits are placed to maintain equal distances between multiple openings.
- Split Direction – The split progresses either from left to right or right to left.

Input Format:
Wall Geometry: Provided as a list of polylines defining the wall boundary.
Openings: Provided as a list of polylines with unique Element IDs.
Configuration Values:
- Standard panel width
- Lintel/Sill bearing length
- Distance from the center of the opening
- Split direction

Expected JSON Output Format:
The output must contain:
Optimized split strategy.
Opening references by ID (instead of full geometry).
Empty arrays for unused splitting methods.

Example output:
{
    ""standard_split_width"": {
      ""width"": 10
    },
    ""lintel_sill_bearing"": [],
    ""center_of_opening"": [],
    ""around_centre_of_opening"": [
      {
        ""opening_id"": 1,
        ""distance_from_centre"": 3
      }
    ],
    ""equal_distance_between_openings"": [],
    ""split_direction"": ""left_to_right""
  }
}
Notes:
- Only one split method is preferred to be applied per opening (to avoid redundancy, but not must).
- Unused split options should be empty lists ([]).
- Include JSON only in the output.";

			var requestPayload = new
			{
				model = "gpt-4-0613",
				messages = new[]
				{
					new { role = "system", content = _systemPrompt },
					new { role = "user", content = element.ToString() }
				},
			};

			try
			{
				var response = await _httpClient.PostAsync(
					"chat/completions",
					new StringContent(JsonSerializer.Serialize(requestPayload), Encoding.UTF8, "application/json")
				);

				var responseString = await response.Content.ReadAsStringAsync();

				response.EnsureSuccessStatusCode();

				var responseObject = JsonSerializer.Deserialize<ChatGPTResponse>(responseString);

			}
			catch (Exception ex)
			{
			}
			/*try
			{
				string sResponse = ApiService.PostSync(Constants.AI_ENGINE_ENDPOINT + "timber/partsplit/predict", elements.ToString());

				elements = JArray.Parse(sResponse);

				for (nIndex = nStart; nIndex < nEnd; nIndex++)
				{
					element = (JObject)elements[nIndex - nStart];

					se = ins.SplitElements[nIndex];
					se.StandardSplitWidth = int.Parse(element.GetValue("standard_split_length").ToString());
					if (m_nCurrentElement == nIndex)
					{
						txtWidth.Text = se.StandardSplitWidth.ToString();
					}

					openings = (JArray)element.GetValue("openings");

					//	Find Associated Opening Index
					foreach (JObject opening in openings)
					{
						for (i = 0; i < ins.AssociatedOpenings.Count; i++)
						{
#if REVIT2024 || REVIT2025
							if (ins.AssociatedOpenings[i].Id.Value == int.Parse(opening.GetValue("id").ToString()))
#else
							if (ins.AssociatedOpenings[i].Id.IntegerValue == int.Parse(opening.GetValue("id").ToString()))
#endif
							{
								string option = opening.GetValue("option").ToString();
								switch (option)
								{
									case "ls":
										ins.AssociatedOpenings[i].Option = OpeningSelectOption.LintelAndSill;
										se.LintelBearing = int.Parse(opening.GetValue("lintel_bearing_length").ToString());
										se.CillBearing = int.Parse(opening.GetValue("cill_bearing_length").ToString());

										if (m_nCurrentElement == nIndex)
										{
											txtLintel.Text = se.LintelBearing.ToString();
											txtCill.Text = se.CillBearing.ToString();
										}
										break;
									case "around":
										ins.AssociatedOpenings[i].Option = OpeningSelectOption.LintelAndSill;
										se.AroundCentreSplitWidth = int.Parse(opening.GetValue("around_centre_split_width").ToString());

										if (m_nCurrentElement == nIndex)
										{
											txtCentredWidth.Text = se.AroundCentreSplitWidth.ToString();
										}
										break;
									case "centre":
										ins.AssociatedOpenings[i].Option = OpeningSelectOption.CentreOfOpening;
										break;
									case "equal":
										ins.AssociatedOpenings[i].Option = OpeningSelectOption.EqualDistanceBetweenOpenings;
										break;
									default:
										break;
								}
								if (option != "none")
								{
									ins.SelectedOpenings.Add(ins.AssociatedOpenings[i]);
									se.SelectedOpenings.Add(ins.AssociatedOpenings[i]);
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{

			}*/
		}

		private void FrmSplit_Load(object sender, EventArgs e)
		{
			Initialize();

			//	Load Presets
			if (m_Handler.Instance is FloorSplitter)
			{
				Text = "PART SPLIT - SLABS";
				//lblLS.Text = "Overlap Top and Bottom";
			}
			else
			{
				Text = "PART SPLIT - WALLS";
			}

			//string sCategory = "SplitWall";
			//if (Text.IndexOf("SLABS") > 0)
			//	sCategory = "SplitFloor";
			//List<Preset> presets = PresetManager.GetPresetsByCategory(sCategory);
			//foreach (Preset preset in presets)
			//{
			//	cmbPreset.Items.Add(preset.Properties[0].Value);
			//}

			lblVersion.Text = string.Format("{0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
		}

		private void txtWidth_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}
		}

		private void txtLintel_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}
		}

		private void txtCill_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}
		}

		private void txtCentredWidth_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
			{
				e.Handled = true;
			}
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			Initialize();
		}

		private void rdoPickStartPt_CheckedChanged(object sender, EventArgs e)
		{
			if (rdoPickStartPt.Checked)
			{
				MakeRequest(ArchitexorRequestId.SelectStartPoint);
			}
		}

		private void chkDebug_SplitLineNumbers_CheckedChanged(object sender, EventArgs e)
		{
			if (chkDebug_SplitLineNumbers.Checked)
				Settings.PreviewSettings.SplitLineNumberColor = Color.Black;
			else
				Settings.PreviewSettings.SplitLineNumberColor = Color.Transparent;

			DrawPreview();
		}

		private void chkDebug_SplitPoints_CheckedChanged(object sender, EventArgs e)
		{
			if (chkDebug_SplitPoints.Checked)
				Settings.PreviewSettings.SplitPointColor = Color.Black;
			else
				Settings.PreviewSettings.SplitPointColor = Color.Transparent;

			DrawPreview();
		}

		private void chkDebug_LocationCurve_CheckedChanged(object sender, EventArgs e)
		{
			if (chkDebug_LocationCurve.Checked)
				Settings.PreviewSettings.LocationCurveColor = Color.Black;
			else
				Settings.PreviewSettings.LocationCurveColor = Color.Transparent;

			DrawPreview();
		}

		private void chkDebug_BoundaryPoints_CheckedChanged(object sender, EventArgs e)
		{
			if (chkDebug_BoundaryPoints.Checked)
				Settings.PreviewSettings.BoundaryPointColor = Color.Black;
			else
				Settings.PreviewSettings.BoundaryPointColor = Color.Transparent;

			DrawPreview();
		}

		private void btnViewInRevit_Click(object sender, EventArgs e)
		{
			Splitter ins = (Splitter)(m_Handler.Instance);

			MakeRequest(ArchitexorRequestId.ViewElementInRevit);
		}

		private void txtWidth_TextChanged(object sender, EventArgs e)
		{
			SetStandardSplitWidth();
		}

		private void SetStandardSplitWidth()
		{
			if (int.TryParse(txtWidth.Text, out int result))
			{
				Splitter ins = (Splitter)(m_Handler.Instance);
				if (m_nCurrentElement < 0 || m_nCurrentElement >= ins.SplitElements.Count)
					return;

				ins.SplitElements[m_nCurrentElement].StandardSplitWidth = result;
			}
		}

		private void txtLintel_TextChanged(object sender, EventArgs e)
		{
			SetLintelBearing();
		}

		private void SetLintelBearing()
		{
			if (int.TryParse(txtLintel.Text, out int result))
			{
				Splitter ins = (Splitter)(m_Handler.Instance);
				if (m_nCurrentElement < 0 || m_nCurrentElement >= ins.SplitElements.Count)
					return;

				ins.SplitElements[m_nCurrentElement].LintelBearing = result;
			}
		}

		private void txtCill_TextChanged(object sender, EventArgs e)
		{
			SetCillBearing();
		}

		private void SetCillBearing()
		{
			if (int.TryParse(txtCill.Text, out int result))
			{
				Splitter ins = (Splitter)(m_Handler.Instance);
				if (m_nCurrentElement < 0 || m_nCurrentElement >= ins.SplitElements.Count)
					return;

				ins.SplitElements[m_nCurrentElement].CillBearing = result;
			}
		}

		private void txtCentredWidth_TextChanged(object sender, EventArgs e)
		{
			SetCentredWidth();
		}

		private void SetCentredWidth()
		{
			if (int.TryParse(txtCentredWidth.Text, out int result))
			{
				Splitter ins = (Splitter)(m_Handler.Instance);
				if (m_nCurrentElement < 0 || m_nCurrentElement >= ins.SplitElements.Count)
					return;

				ins.SplitElements[m_nCurrentElement].AroundCentreSplitWidth = result;
			}
		}

		private void rdoLeftRight_CheckedChanged(object sender, EventArgs e)
		{
			SetSplitFromLeftToRight();
		}

		private void rdoRightLeft_CheckedChanged(object sender, EventArgs e)
		{
			SetSplitFromLeftToRight();
		}

		private void SetSplitFromLeftToRight()
		{
			Splitter ins = (Splitter)(m_Handler.Instance);
			ins.SplitElements[m_nCurrentElement].SplitFromLeftToRight = rdoLeftRight.Checked;
		}

		private void btnSetting_Click(object sender, EventArgs e)
		{
			FrmSplitParams frm = new FrmSplitParams();
			frm.ShowDialog();
		}

		protected void OnElementDisposed(object sender, EventArgs e)
		{
			CThumbnail thumb = (CThumbnail)sender;
			if (m_Handler == null)
			{
				return;
			}

			Splitter ins = (Splitter)(m_Handler.Instance);
			int nIndex = thumb.Index;
			ins.SplitElements.RemoveAt(nIndex);

			UpdateTotalStats();

			if (m_nCurrentElement == nIndex)
			{
				if (m_nCurrentElement == ins.SplitElements.Count)
					m_nCurrentElement--;

				if (m_nCurrentElement < 0)
				{
					btnClear.PerformClick();
					return;
				}

				UpdateIndividualStats();
			}

			//	Update Index of Others
			foreach (Control control in panelThumnails.Controls)
			{
				thumb = (CThumbnail)control;
				if (thumb.Index > nIndex)
				{
					thumb.Index--;
				}
			}
		}

		private void btnTrain_Click(object sender, EventArgs e)
		{
			Splitter ins = (Splitter)(m_Handler.Instance);
			JObject project = new JObject();
			project.Add("guid", ins.GetDocument().ProjectInformation.UniqueId);
			project.Add("user", Constants.thisUser.Id);
			project.Add("name", "");
			project.Add("country_code", "");
			project.Add("address", "");
			project.Add("keywords", "");
			project.Add("comments", "");
			project.Add("overall_building_area", "0");
			JArray elements = new JArray();
			foreach (CSplitElement se in ins.SplitElements)
			{
				JObject element = new JObject();
				element.Add("type", 1);

#if REVIT2024 || REVIT2025
				element.Add("element_id", se.Element.Id.Value);
#else
				element.Add("element_id", se.Element.Id.IntegerValue);
#endif
				element.Add("height", (int)UnitUtils.ConvertFromInternalUnits(se.MaxHeight, CCommonSettings.Unit));
				element.Add("length", (int)Math.Round(UnitUtils.ConvertFromInternalUnits(se.LocationCurve.Length, CCommonSettings.Unit), 3));
				element.Add("split_length", se.StandardSplitWidth);
				//JArray points = new JArray();
				string points = "";
				foreach (Curve c in se.BoundaryCurves[0])
				{
					points += (int)UnitUtils.ConvertFromInternalUnits(se.GetPoint2D(c.GetEndPoint(0)).X, CCommonSettings.Unit) + "," + (int)UnitUtils.ConvertFromInternalUnits(se.GetPoint2D(c.GetEndPoint(0)).Y, CCommonSettings.Unit) + ",";
				}
				element.Add("points", points);

				JArray openings = new JArray();
				foreach (COpening opening in se.AssociatedOpenings)
				{
					try
					{
						if (opening.Width == 0
							|| opening.Height == 0
							|| opening.Location == null)
							continue;

						JObject jOpening = new JObject();
#if REVIT2024 || REVIT2025
						jOpening.Add("opening_id", opening.Id.Value);
#else
						jOpening.Add("opening_id", opening.Id.IntegerValue);
#endif
						jOpening.Add("width", (int)Math.Round(UnitUtils.ConvertFromInternalUnits(opening.Width, CCommonSettings.Unit), 3));
						jOpening.Add("height", (int)Math.Round(UnitUtils.ConvertFromInternalUnits(opening.Height, CCommonSettings.Unit), 3));
						jOpening.Add("x", (int)Math.Round(UnitUtils.ConvertFromInternalUnits(se.GetPoint2D(opening.Location).X, CCommonSettings.Unit), 3));
						jOpening.Add("y", (int)Math.Round(UnitUtils.ConvertFromInternalUnits(se.GetPoint2D(opening.Location).Y, CCommonSettings.Unit), 3));
						string[] options = ["none", "ls", "around", "centre", "equal"];
						jOpening.Add("option", options[(int)opening.Option]);
						if (opening.Option == OpeningSelectOption.LintelAndSill)
						{
							jOpening.Add("lintel_length", se.LintelBearing);
							jOpening.Add("cill_length", se.CillBearing);
						}
						if (opening.Option == OpeningSelectOption.AroundCentreOfOpening)
							jOpening.Add("around_length", se.AroundCentreSplitWidth);

						openings.Add(jOpening);
					}
					catch (Exception ex)
					{
					}
				}

				element.Add("openings", openings);
				elements.Add(element);
			}
			project.Add("elements", elements);

			try
			{
				ApiService.PostSync(Constants.AI_ENGINE_ENDPOINT + "timber/project", project.ToString());//"http://localhost:8080/"
				//ApiService.GetResponse(Constants.AI_ENGINE_ENDPOINT + "timber/partsplit/train", "");
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error");
			}
		}
		#endregion

		private void btnApplyToAll_Click(object sender, EventArgs e)
		{
			Splitter ins = (Splitter)(m_Handler.Instance);
			int 
				nStandardSplitWidth = 0,
				nLintelBearing = 0,
				nCillBearing = 0,
				nCentredWidth = 0;
			int.TryParse(txtWidth.Text, out nStandardSplitWidth);
			int.TryParse(txtLintel.Text, out nLintelBearing);
			int.TryParse(txtCill.Text, out nCillBearing);
			int.TryParse(txtCentredWidth.Text, out nCentredWidth);

			foreach (CSplitElement se in ins.SplitElements)
			{
				se.StandardSplitWidth = nStandardSplitWidth;
				se.LintelBearing = nLintelBearing;
				se.CillBearing = nCillBearing;
				se.AroundCentreSplitWidth = nCentredWidth;
				se.SplitFromLeftToRight = rdoLeftRight.Checked;
			}
		}
	}

	//	Strongly-typed classes for deserialization
	public class ChatGPTResponse
	{
		public List<Choice> choices { get; set; }
	}

	public class Choice
	{
		public int index { get; set; }

		public ChatMessage message { get; set; }
	}

	public class ChatMessage
	{
		public string role { get; set; }
		public string content { get; set; }
	}
}
