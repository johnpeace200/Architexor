using ATXComponents.Widgets;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Architexor.Request;
using Architexor.Utils;
using Architexor.Utils.Filter;
using System;
using System.Collections.Generic;
using View = Autodesk.Revit.DB.View;
using Architexor.Base;

namespace Architexor.BasicSplit.Splitters
{
	abstract public class Splitter : Controller
	{
		//	Elements to split (Walls, Slabs, Parts)
		protected List<CSplitElement> m_SplitElements = new List<CSplitElement>();
		public List<CSplitElement> SplitElements { get => m_SplitElements; set => m_SplitElements = value; }

		//	Input Parameters
		/// <summary>
		/// Standard panel split width
		/// </summary>
		protected int m_nStandardSplitWidth = 0;
		/// <summary>
		/// Lintel Bearing
		/// </summary>
		protected int m_nLintelBearing = 0;
		/// <summary>
		/// Sill Bearing
		/// </summary>
		protected int m_nCillBearing = 0;
		/// <summary>
		/// Width for Around Centre option
		/// </summary>
		protected int m_nAroundCentreSplitWidth = 0;

		/// <summary>
		/// Parameter to determine to select the start point or not
		/// This parameter tells the tool will split from left to right or right to left when user selected multiple elements.
		///		false : right to left
		///		true : left to right
		/// </summary>
		protected bool m_bMustSelectStartPoint = false;
		/// <summary>
		/// The point which indicates where start the split from
		/// </summary>
		protected XYZ m_ptStartPoint = null;

		/// <summary>
		/// Array of the selected openings for each option (index and option only)
		/// </summary>
		protected List<COpening> m_SelectedOpenings;
		public List<COpening> SelectedOpenings { get => m_SelectedOpenings; set => m_SelectedOpenings = value; }
		/// <summary>
		/// Array of the all associated openings to the selected elements
		/// </summary>
		protected List<COpening> m_AssociatedOpenings;
		public List<COpening> AssociatedOpenings { get => m_AssociatedOpenings; set => m_AssociatedOpenings = value; }


		public int StandardSplitWidth { get => m_nStandardSplitWidth; set => m_nStandardSplitWidth = value; }
		public int LintelBearing { get => m_nLintelBearing; set => m_nLintelBearing = value; }
		public int CillBearing { get => m_nCillBearing; set => m_nCillBearing = value; }
		public int AroundCentreSplitWidth { get => m_nAroundCentreSplitWidth; set => m_nAroundCentreSplitWidth = value; }
		public bool MustSelectStartPoint { get => m_bMustSelectStartPoint; set => m_bMustSelectStartPoint = value; }
		public XYZ StartPoint { get => m_ptStartPoint; set => m_ptStartPoint = value; }
		public bool SplitFromLeftToRight { get; set; }

		public List<Element> AdjacentElements { get; set; }

		public int CurrentElement { get; set; } = -1;
		public bool IsBatchAction { get; set; } = false;


		public List<ElementId> Failures { get; set; } = new List<ElementId>();

		//	Initialize
		public Splitter()
		{
			m_SelectedOpenings = new List<COpening>();
			m_AssociatedOpenings = new List<COpening>();
			AdjacentElements = new List<Element>();
		}

		public List<COpening> GetAssociatedOpenings(ElementId nId)
		{
			List<COpening> openings = new();
			foreach (COpening opening in m_AssociatedOpenings)
			{
				if (opening.Parent == nId)
				{
					openings.Add(opening);
				}
			}
			return openings;
		}

		/// <summary>
		///		Function to get openings for each options
		/// </summary>
		/// <param name="opt"></param>
		public void SelectOpenings(OpeningSelectOption opt)
		{
			//	Get all openings attached to the element
			List<ElementId> candidates = new List<ElementId>();
			foreach (COpening o in AssociatedOpenings)
			{
				if(!IsBatchAction && o.Parent != SplitElements[CurrentElement].Element.Id)
				{
					continue;
				}

				bool bExist = false;
				foreach (COpening oi in m_SelectedOpenings)
				{
					if (oi.Option == opt && oi.Id == o.Id)
					{
						bExist = true;
						break;
					}
				}
				if (!bExist)
				{
					switch (opt)
					{
						case OpeningSelectOption.LintelAndSill:
							//	This opening should not be selected as Around Centre and Centre of Opening Options
							foreach (COpening oi in m_SelectedOpenings)
							{
								if ((oi.Option == OpeningSelectOption.AroundCentreOfOpening
									|| oi.Option == OpeningSelectOption.CentreOfOpening)
									&& oi.Id == o.Id)
								{
									bExist = true;
									break;
								}
							}
							break;
						case OpeningSelectOption.AroundCentreOfOpening:
							//	This opening should not be selected as Lintel & Cill bearing and Centre of Opening Options
							foreach (COpening oi in m_SelectedOpenings)
							{
								if ((oi.Option == OpeningSelectOption.LintelAndSill
									|| oi.Option == OpeningSelectOption.CentreOfOpening)
									&& oi.Id == o.Id)
								{
									bExist = true;
									break;
								}
							}
							break;
						case OpeningSelectOption.CentreOfOpening:
							//	This opening should not be selected as Lintel & Cill bearing and Around Centre Options
							foreach (COpening oi in m_SelectedOpenings)
							{
								if ((oi.Option == OpeningSelectOption.LintelAndSill
									|| oi.Option == OpeningSelectOption.AroundCentreOfOpening)
									&& oi.Id == o.Id)
								{
									bExist = true;
									break;
								}
							}
							break;
						default:
							break;
					}
				}

				if (!bExist)
				{
					candidates.Add(o.Id);
				}
			}

			//	Select Openings for Lintel & Cill bearing split option(Except openings for Around Center split option)
			Selection sel = m_uiApp.ActiveUIDocument.Selection;
			InArrayPickFilter pf = new InArrayPickFilter(candidates);

			//	Don't set pre-select array because user can be confused
			try
			{
				GlobalHook.Subscribe();
				List<Reference> refers = (List<Reference>)sel.PickObjects(ObjectType.Element, pf, "Please select windows & doors.");//, canLCOpenings);
				GlobalHook.Unsubscribe();

				//	Remove all openings which is selected for the current option previously
				//foreach (SOpeningParameter oi in m_SelectedOpenings)
				//{
				//	if (oi.Option == opt)
				//	{
				//		m_SelectedOpenings.Remove(oi);
				//	}
				//}

				foreach (Reference r in refers)
				{
					for (int i = 0; i < AssociatedOpenings.Count; i++)
					{
						if (AssociatedOpenings[i].Id == r.ElementId)
						{
							AssociatedOpenings[i].Option = opt;
							m_SelectedOpenings.Add(AssociatedOpenings[i]);

							m_SplitElements.Find(x => x.Element.Id == AssociatedOpenings[i].Parent).SelectedOpenings.Add(AssociatedOpenings[i]);
							break;
						}
					}
				}
			}
			catch (Autodesk.Revit.Exceptions.OperationCanceledException) { }
		}

		public void ClearOpenings(OpeningSelectOption opt)
		{
			if (opt != OpeningSelectOption.None)
			{
				if(IsBatchAction)
				{
					foreach(CSplitElement se in SplitElements)
					{
						for (int i = se.SelectedOpenings.Count - 1; i >= 0; i--)
						{
							if (se.SelectedOpenings[i].Option == opt)
							{
								se.SelectedOpenings.RemoveAt(i);
							}
						}
					}
				}
				else
				{
					for (int i = SplitElements[CurrentElement].SelectedOpenings.Count - 1; i >= 0; i--)
					{
						if (SplitElements[CurrentElement].SelectedOpenings[i].Option == opt)
						{
							SplitElements[CurrentElement].SelectedOpenings.RemoveAt(i);
						}
					}
				}
				for (int i = m_SelectedOpenings.Count - 1; i >= 0; i--)
				{
					if (m_SelectedOpenings[i].Option == opt)
					{
						m_SelectedOpenings.RemoveAt(i);
					}
				}
			}
			else
			{
				m_SelectedOpenings.Clear();
			}
		}

		public bool SelectStartPoint()
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			Selection sel = m_uiApp.ActiveUIDocument.Selection;
			CSplitElement se = m_SplitElements[CurrentElement];

			Reference r;
			try
			{
				Transaction trans = new Transaction(doc);
				trans.Start("Create LocationCurve line");

				ModelCurve mc = doc.Create.NewModelCurve(se.LocationCurve, SketchPlane.Create(doc, se.SketchPlane));
				doc.Regenerate();
				r = sel.PickObject(ObjectType.PointOnElement, new PointOnElementFilter(mc), "Please pick a start point");
				se.StartPoint = r.GlobalPoint;

				trans.RollBack();
			}
			catch (Autodesk.Revit.Exceptions.OperationCanceledException)
			{
				se.StartPoint = null;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Split function
		/// </summary>
		public bool PrepareSplit()
		{
			try
			{
				Document doc = m_uiApp.ActiveUIDocument.Document;

				#region Digitalize the information of the elements and openings
				for (int i = 0; i < m_SplitElements.Count; i++)
				{
					CSplitElement cElem = m_SplitElements[i];

					//	If this element is already split, we simply ignore
					if(!cElem.IsSplitable())
					{
						continue;
					}

					Element elem = cElem.Element;

					cElem.Prepare();

					#region Determine split points based on the Openings
					cElem.GetSplitPoints();
					#endregion

					#region Get Split lines based on split points
					cElem.GetSplitLines();
					#endregion

					cElem.GetPanels();
				}
				#endregion
			}
			catch (Exception ex)
			{
				Util.ErrorMsg(ex.Message);
				return false;
			}

			return true;
		}

		public void SplitParts()
		{
			try
			{
				Document doc = m_uiApp.ActiveUIDocument.Document;
				//  Start the transaction
				using (TransactionGroup tg = new TransactionGroup(doc))
				{
					tg.Start("Action");

					//	Save current visibility
					Transaction trans = new Transaction(doc);
					trans.Start("Change PartsVisibility Option");
					PartsVisibility tmpVisibility = doc.ActiveView.PartsVisibility;
					doc.ActiveView.PartsVisibility = PartsVisibility.ShowPartsOnly;
					trans.Commit();

					foreach (CSplitElement cElem in m_SplitElements)
					{
						//	If this element is already split, we simply ignore the element
						if(!cElem.IsSplitable())
						{
							continue;
						}

						trans = new Transaction(doc);
						trans.Start("Split Part");
						FailureHandlingOptions failureHandlingOptions = trans.GetFailureHandlingOptions();
						failureHandlingOptions.SetFailuresPreprocessor(new MyFailureHandler());
						trans.SetFailureHandlingOptions(failureHandlingOptions);

						cElem.Split();
						trans.Commit();
					}
					//	Restore parts visibility property
					trans = new Transaction(doc);
					trans.Start("Change PartsVisibility Option back");
					doc.ActiveView.PartsVisibility = tmpVisibility;
					trans.Commit();

					tg.Assimilate();
					//tg.RollBack();
				}
			}
			catch (Exception ex)
			{
				Util.ErrorMsg(ex.Message);
			}
		}

		public double ConvertToInternalUnits(double fVal)
		{
			return UnitUtils.ConvertToInternalUnits(fVal, CCommonSettings.Unit);
		}

		public int GetOpeningCount(OpeningSelectOption option)
		{
			if (option == OpeningSelectOption.None)
				return AssociatedOpenings.Count;
			else
			{
				int nCount = 0;
				foreach (COpening sO in m_SelectedOpenings)
				{
					if (sO.Option == option)
					{
						nCount++;
					}
				}
				return nCount;
			}
		}

		public override bool ProcessRequest(ArchitexorRequestId reqId)
		{
			bool bFinish = false;
			switch (reqId)
			{
				case ArchitexorRequestId.None:
					return bFinish;  // no request at this time -> we can leave immediately
				case ArchitexorRequestId.SelectOpeningsForLC:
					SelectOpenings(OpeningSelectOption.LintelAndSill);
					break;
				case ArchitexorRequestId.SelectOpeningsForAroundCentre:
					SelectOpenings(OpeningSelectOption.AroundCentreOfOpening);
					break;
				case ArchitexorRequestId.SelectOpeningsForCentre:
					SelectOpenings(OpeningSelectOption.CentreOfOpening);
					break;
				case ArchitexorRequestId.SelectOpeningsForEqualDistanceBetween:
					SelectOpenings(OpeningSelectOption.EqualDistanceBetweenOpenings);
					break;
				case ArchitexorRequestId.SelectAdjacentElements:
					SelectAdjacentElements();
					break;
				case ArchitexorRequestId.Split:
					if (PrepareSplit())
					{
						SplitParts();
					}
					bFinish = true;
					break;
				case ArchitexorRequestId.PrepareSplit:
					PrepareSplit();
					break;
				case ArchitexorRequestId.SelectStartPoint:
					SelectStartPoint();
					break;
				case ArchitexorRequestId.ViewElementInRevit:
					ViewCurrentElementInRevit();
					break;
				case ArchitexorRequestId.ShowFailureList:
					ShowFailureList();
					break;
				default:
					// some kind of a warning here should
					// notify us about an unexpected request 
					break;
			}
			return bFinish;
		}

		private void SelectAdjacentElements()
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			Selection sel = m_uiApp.ActiveUIDocument.Selection;

			try
			{
				using (Transaction trans = new Transaction(doc))
				{
					trans.Start("Panel Split");

					//	Save current visibility
					PartsVisibility tmpVisibility = doc.ActiveView.PartsVisibility;
					doc.ActiveView.PartsVisibility = PartsVisibility.ShowPartsAndOriginal;

					WallsPickFilter pf = new WallsPickFilter();
					GlobalHook.Subscribe();
					List<Reference> refers = (List<Reference>)sel.PickObjects(ObjectType.Element, "Please select adjacent elements.");//pf, 
					GlobalHook.Unsubscribe();

					AdjacentElements.Clear();
					foreach (Reference refer in refers)
					{
						Element e = doc.GetElement(refer);
						if (e is Part)
							e = Util.GetSourceElementOfPart(doc, e as Part);
						AdjacentElements.Add(e);
					}

					doc.ActiveView.PartsVisibility = tmpVisibility;

					trans.Commit();

					foreach (CSplitElement se in m_SplitElements)
					{
						Element e = se.Element;
						if (e is Part)
						{
							e = Util.GetSourceElementOfPart(doc, e as Part);
						}
						se.AdjacentElements.Clear();

						foreach (Element ae in AdjacentElements)
						{
							if (e.Id == ae.Id)
								continue;

							if (ae is Wall)
							{
								if (Util.IntersectWith(se.Element, ae) == SetComparisonResult.Superset)
								{
									se.AdjacentElements.Add(ae);
								}
							}
#if REVIT2024 || REVIT2025
							else if (ae is FamilyInstance &&
									(e.Category.Id.Value.Equals((int)BuiltInCategory.OST_Roofs))
											|| (e.Category.Id.Value.Equals((int)BuiltInCategory.OST_Floors)))    //	Beams
#else
                            else if (ae is FamilyInstance &&
								(e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Roofs))
									|| (e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors)))	//	Beams
#endif
							{
								BoundingBoxXYZ bbxyz = ae.get_BoundingBox(doc.ActiveView);

								if (ae.Location is LocationCurve lc)
								{
									XYZ pt1 = lc.Curve.GetEndPoint(0),
										pt2 = lc.Curve.GetEndPoint(1);

									Face bottomFace = Util.GetBaseFacesOfElement(doc, e, false)[0];
									Plane plane = bottomFace.GetSurface() as Plane;
									double d = Math.Abs(plane.SignedDistanceTo(pt1));
									d = UnitUtils.ConvertFromInternalUnits(d, CCommonSettings.Unit);
									d = Math.Abs(plane.SignedDistanceTo(pt2));
									d = UnitUtils.ConvertFromInternalUnits(d, CCommonSettings.Unit);
									if (bbxyz != null)
									{
										if (Math.Abs(plane.SignedDistanceTo(pt1)) < bbxyz.Max.Z - bbxyz.Min.Z
											&& Math.Abs(plane.SignedDistanceTo(pt2)) < bbxyz.Max.Z - bbxyz.Min.Z)
										{
											se.AdjacentElements.Add(ae);
										}
									}
									else
									{
										if (Util.IsZero(plane.SignedDistanceTo(pt1))
											&& Util.IsZero(plane.SignedDistanceTo(pt2)))
										{
											se.AdjacentElements.Add(ae);
										}
									}
								}
							}
						}
					}
				}
			}
			catch (Autodesk.Revit.Exceptions.OperationCanceledException)
			{

			}
		}

		public void ClearAdjacentElements()
		{
			AdjacentElements.Clear();
			for (int i = 0; i < m_SplitElements.Count; i++)
				m_SplitElements[i].AdjacentElements.Clear();
		}

		private void ViewCurrentElementInRevit()
		{
			Document doc = GetDocument();
			View view = doc.ActiveView;
			Element element = m_Elements[CurrentElement];
			BoundingBoxXYZ boundingBox = element.get_BoundingBox(view);

			if (boundingBox != null)
			{
				XYZ min = boundingBox.Min;
				XYZ max = boundingBox.Max;

				// Get the UIView corresponding to the current active view
				IList<UIView> uiViews = m_uiApp.ActiveUIDocument.GetOpenUIViews();
				UIView activeUIView = null;

				foreach (UIView uiView in uiViews)
				{
					if (uiView.ViewId == doc.ActiveView.Id)
					{
						activeUIView = uiView;
						break;
					}
				}

				if (activeUIView != null)
				{
					// Use ZoomAndFitToRectangle to zoom to the bounding box of the element
					activeUIView.ZoomAndCenterRectangle(min, max);
				}
			}

			// Select the element
			ICollection<ElementId> elementIds = new List<ElementId> { element.Id };
			m_uiApp.ActiveUIDocument.Selection.SetElementIds(elementIds);
		}

		private void ShowFailureList()
		{
			Document doc = GetDocument();
			using (Transaction trans = new Transaction(doc, "Error Message"))
			{
				trans.Start();

				// Add the failure message to the transaction
				FailureDefinitionId failDefId = BuiltInFailures.GeneralFailures.GenericWarning;
				FailureMessage fm = new FailureMessage(failDefId);
				fm.SetFailingElements(Failures);  // Link the failing element
				doc.PostFailure(fm);  // Post the failure to Revit's system

				trans.Commit();
				/*TransactionStatus status = trans.Commit();
				if (status == TransactionStatus.RolledBack)
				{
					//	Clicked cancel
					m_Elements.Clear();
					m_SplitElements.Clear();
				}*/
			}
		}
}

	public class MyFailureHandler : IFailuresPreprocessor
	{
		public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
		{
			foreach (var failureMessageAccessor in failuresAccessor.GetFailureMessages())
			{
				FailureDefinitionId failureDefinitionId = failureMessageAccessor.GetFailureDefinitionId();
				// Here, you can add another Failure if you want
				if (failureDefinitionId == BuiltInFailures.InaccurateFailures.InaccurateSketchLine)
				{
					failuresAccessor.DeleteWarning(failureMessageAccessor);
				}
			}
			return FailureProcessingResult.Continue;
		}
	}
}
