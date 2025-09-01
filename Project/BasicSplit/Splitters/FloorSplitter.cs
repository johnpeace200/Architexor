using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Architexor.Request;
using Architexor.Utils;
using Architexor.Utils.Filter;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Architexor.BasicSplit.Splitters
{
	public class FloorSplitter : Splitter
	{
		public override bool ProcessRequest(ArchitexorRequestId reqId)
		{
			switch (reqId)
			{
				case ArchitexorRequestId.SelectSplitElements:
					Document doc = m_uiApp.ActiveUIDocument.Document;
					Selection sel = m_uiApp.ActiveUIDocument.Selection;

					//  Select slab/part(s)
					try
					{
						GlobalHook.Subscribe();
						SlabsAndPartsPickFilter selFilter = new SlabsAndPartsPickFilter(doc);
						IList<Reference> refers = sel.PickObjects(ObjectType.Element, selFilter, "Please select roof(s) or floor(s).");
						GlobalHook.Unsubscribe();

						m_Elements.Clear();
						foreach (Reference refer in refers)
						{
							m_Elements.Add(doc.GetElement(refer));
						}

						TransactionGroup ts = new TransactionGroup(doc);
						ts.Start();
						Initialize();
						ts.Assimilate();
					}
					catch(Exception _) {
						m_Elements.Clear();
						AssociatedOpenings.Clear();
						m_SplitElements.Clear();
						Debug.WriteLine(_.Message + "\r\n" + _.StackTrace);
					}
					break;
				default:
					break;
			}
			return base.ProcessRequest(reqId);
		}

		public override void Initialize()
		{
			AssociatedOpenings.Clear();
			m_SplitElements.Clear();
			AdjacentElements.Clear();

			return;
			for (int i = 0; i < m_Elements.Count; i++)
			{
				Element e = m_Elements[i];

				CSplitSlabElement se = new CSplitSlabElement(this, e);

				if (se.BoundaryCurves.Count >= 2)
				{
					Document doc = GetDocument();
					ICollection<ElementId> parts = new List<ElementId>();
					List<ElementId> elemList = new List<ElementId> { se.Element.Id };

					Transaction trans = new Transaction(doc);
					trans.Start("Split");

					if (PartUtils.AreElementsValidForCreateParts(doc, elemList))
					{
						PartUtils.CreateParts(doc, elemList);
						doc.Regenerate();
						parts = PartUtils.GetAssociatedParts(doc, se.Element.Id, false, false);
					}
					else
					{
						if (e is Part)
						{
							parts.Add(e.Id);
						}
						else
						{
							parts = PartUtils.GetAssociatedParts(doc, e.Id, false, false);
						}
					}

					if (parts.Count > 1)
					{
						m_Elements.RemoveAt(i);
						i--;
						foreach (ElementId eId in parts)
						{
							m_Elements.Add(doc.GetElement(eId));
						}
					}
					else
					{
						List<Curve> lstCurves = new List<Curve>();
						foreach(CurveLoop cl in se.BoundaryCurves)
						{
							foreach(Curve c in cl)
							{
								lstCurves.Add(
									Line.CreateBound(
										se.SketchPlane.ProjectOnto(c.GetEndPoint(0)),
										se.SketchPlane.ProjectOnto(c.GetEndPoint(1))
									)
								);
							}
						}

						List<ElementId> intersectionElementIds = new List<ElementId>();
						//	Divide Part
						SketchPlane sketchPlane = SketchPlane.Create(doc, se.SketchPlane);
						PartMaker _ = PartUtils.DivideParts(doc, parts, intersectionElementIds, lstCurves, sketchPlane.Id);
						doc.Regenerate();

						ICollection<ElementId> generatedParts = PartUtils.GetAssociatedParts(doc, e.Id, true, true);
						foreach (ElementId eId in parts)
							generatedParts.Remove(eId);

						m_Elements.RemoveAt(i);
						i--;
						foreach (ElementId eId in generatedParts)
						{
							m_Elements.Add(doc.GetElement(eId));
						}
					}

					trans.Commit();
				}
				else
				{
					m_SplitElements.Add(se);
				}
			}

			AdjacentElements.Clear();
		}
	}
}
