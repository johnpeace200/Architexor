#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Architexor.Core;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Architexor.Request;
using Architexor.Utils;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
#endregion // Namespaces

namespace Architexor.Commands
{
	/*[Transaction(TransactionMode.Manual)]
	public class Commands : IExternalCommand
	{
		/// <summary>
		/// </summary>
		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			return Result.Failed;
		}
	}*/
	/*
	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class SelectWallOnLevel : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			Document document = commandData.Application.ActiveUIDocument.Document;
			Selection sel = commandData.Application.ActiveUIDocument.Selection;

			FilteredElementCollector collector = new(document);
			ICollection<Element> levels = collector.OfClass(typeof(Level)).ToElements();
			var query = from element in collector where element.Name == "Level 0" select element;// Linq query

			// Get the level id which will be used to match elements
			List<Element> level1 = query.ToList<Element>();
			ElementId levelId = level1[0].Id;

			// Find all walls on level one
			ElementLevelFilter level1Filter = new(levelId);
			collector = new FilteredElementCollector(document);
			ICollection<ElementId> allWallsOnLevel1 = collector.OfClass(typeof(Wall)).WherePasses(level1Filter).ToElementIds();

			sel.SetElementIds(allWallsOnLevel1);
			// Find all rooms not on level one: use an inverted ElementLevelFilter to match all elements not on the target level
			//			ElementLevelFilter notOnLevel1Filter = new ElementLevelFilter(levelId, true); // Inverted filter
			//			collector = new FilteredElementCollector(document);
			//			IList<Element> allRoomsNotOnLevel1 = collector.WherePasses(new WallFilter()).WherePasses(notOnLevel1Filter).ToElements();

			return Result.Succeeded;
		}
	}*/

	/*
	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class MergeParts : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			Result result = Result.Succeeded;

			try
			{
				//  Get application and document objects
				int i;
				UIApplication uiapp = commandData.Application;
				Document doc = uiapp.ActiveUIDocument.Document;
				Selection sel = uiapp.ActiveUIDocument.Selection;

				ICollection<ElementId> partIds = sel.GetElementIds();
				IEnumerator<ElementId> enumerator = partIds.GetEnumerator();
				List<Part> parts = new List<Part>();
				for (i = 0; i < partIds.Count; i++)
				{
					enumerator.MoveNext();
					parts.Add(doc.GetElement(enumerator.Current) as Part);
				}
				enumerator.MoveNext();

				//Element parent = Util.GetSourceElementOfPart(doc, parts[0]);
				List<ElementId> parents = new List<ElementId>();
				foreach (Part p in parts)
				{
					ElementId id = Util.GetSourceElementOfPart(doc, p).Id;
					if (!parents.Contains(id))
					{
						parents.Add(id);
					}
				}

				Transaction trans = new Transaction(doc);

				trans.Start("Retrieve Sketch Elements");
				ICollection<ElementId> ids = doc.Delete(parents);// parent.Id);
				trans.RollBack();

				trans.Start("Merge Parts");

				List<Element> a = new List<Element>();
				enumerator = ids.GetEnumerator();
				for (i = 0; i < ids.Count; i++)
				{
					enumerator.MoveNext();
					a.Add(doc.GetElement(enumerator.Current));
				}

				List<List<Part>> mergeArray = new List<List<Part>>();

				//	Generate new split lines
				List<Curve> newCurves = new List<Curve>();
				foreach (Element e in a.FindAll(e => e is ModelLine))
				{
					List<Part> partsArray = new List<Part>();

					Curve c = (e as ModelLine).GeometryCurve;

					int nIntersectCount = 0;
					foreach (Part p in parts)
					{
						GeometryElement geomElem = p.get_Geometry(new Options());
						foreach (GeometryObject geomObject in geomElem)
						{
							if (geomObject is Solid)
							{
								Solid solid = geomObject as Solid;
								FaceArray faceArray = solid.Faces;
								foreach (Face f in faceArray)
								{
									foreach (CurveLoop cl in f.GetEdgesAsCurveLoops())
									{
										foreach (Curve c1 in cl)
										{
											if (c.Intersect(c1) == SetComparisonResult.Equal)
											{
												nIntersectCount++;
												partsArray.Add(p);
												goto Break;
											}
										}
									}
								}

							}
						}
					Break:
						//	Do nothing
						nIntersectCount += 0;
					}
					if (nIntersectCount < 2)
						newCurves.Add(c);
					else
					{
						mergeArray.Add(partsArray);
					}
				}

				foreach (List<Part> partsArray in mergeArray)
				{
					List<ElementId> elemList = new List<ElementId>();
					foreach (Part p in partsArray)
					{
						elemList.Add(p.Id);
					}

					PartUtils.CreateMergedPart(doc, elemList);
				}

				/*doc.Delete(PartUtils.GetAssociatedPartMaker(doc, parent.Id).Id);

				//	Get original sketch plane
				SketchPlane sketchPlane = a.FindAll(e => e is SketchPlane)[0] as SketchPlane;

				//	Generate new sketch plane
				sketchPlane = SketchPlane.Create(doc, sketchPlane.GetPlane());

				//  Create Part
				List<ElementId> elemList = new List<ElementId>();
				elemList.Add(parent.Id);
				ICollection<ElementId> newPartIds = new List<ElementId>();
				if (PartUtils.AreElementsValidForCreateParts(doc, elemList))
				{
					PartUtils.CreateParts(doc, elemList);
					doc.Regenerate();

					newPartIds = PartUtils.GetAssociatedParts(doc, parent.Id, false, false);
				}
				List<ElementId> intersectionElementsIds = new List<ElementId>();
				//  Divide Part
				PartMaker partDivide = PartUtils.DivideParts(doc, newPartIds, intersectionElementsIds, newCurves, sketchPlane.Id);* /

				trans.Commit();
			}
			catch (Exception ex)
			{
				message = ex.Message;
				return Result.Failed;
			}

			return result;
		}
	}*/


	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class Annotation : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			try
			{
				// Get the active document and view
				UIDocument revitDoc = commandData.Application.ActiveUIDocument;
				Autodesk.Revit.DB.View view = revitDoc.Document.ActiveView;
				Document doc = revitDoc.Document;

				//  Get MultiReferenceAnnotationType
				List<MultiReferenceAnnotationType> types = new List<MultiReferenceAnnotationType>(
						new FilteredElementCollector(doc)
								//.WhereElementIsNotElementType()
								.OfClass(typeof(MultiReferenceAnnotationType))
								//.Where(ins => ins.Name == ConnectorBuilder.sMarker)
								.Where(ins => ins.Name == "Columns Section-Stirrups Dis without dim text or tricks")
								.ToList()
								.Cast<MultiReferenceAnnotationType>()
								);
				if (types.Count == 0)
				{
					message = "Can not find the Annotation type!";
					return Autodesk.Revit.UI.Result.Failed;
				}
				MultiReferenceAnnotationType type = types[0];

				//  Get Rebars
				List<Rebar> rebars = new List<Rebar>(
						new FilteredElementCollector(doc, view.Id)
								.OfClass(typeof(Rebar))
								.ToList()
								.Cast<Rebar>()
						);

				TransactionGroup tg = new TransactionGroup(doc);
				tg.Start();

				foreach (Rebar rebar in rebars)
				{
					Autodesk.Revit.DB.Curve curve = rebar.GetCenterlineCurves(false, false, false, MultiplanarOption.IncludeAllMultiplanarCurves, 0)[0];

					// create a rebar tag at the first end point of the first curve
					/*using( Transaction t = new Transaction(revitDoc.Document))
					{
							t.Start("Create new tag");

							IndependentTag tag = IndependentTag.Create(revitDoc.Document, view.Id, new Reference(rebar), true,
									Autodesk.Revit.DB.TagMode.TM_ADDBY_CATEGORY,//TM_ADDBY_CATEGORY,
									Autodesk.Revit.DB.HeadOrientation.Horizontal, curve.GetEndPoint(0));
							t.Commit();
					}*/

					try
					{
						MultiReferenceAnnotationOptions options = new MultiReferenceAnnotationOptions(type);
						options.TagHeadPosition = new XYZ(curve.GetEndPoint(0).X * 0.9, 0, curve.GetEndPoint(0).Z);
						options.DimensionLineOrigin = new XYZ(curve.GetEndPoint(0).X * 0.9, 0, curve.GetEndPoint(0).Z);
						options.DimensionLineDirection = new XYZ(1, 0, 0);
						options.DimensionPlaneNormal = view.ViewDirection;
						options.SetElementsToDimension(new List<ElementId>() { rebar.Id });
						using (Transaction tran = new Transaction(revitDoc.Document, "Create_Rebar_Vertical"))
						{
							tran.Start();
							var mra = MultiReferenceAnnotation.Create(revitDoc.Document, view.Id, options);
							var dimension = revitDoc.Document.GetElement(mra.DimensionId) as Dimension;

							//List<ElementId> eIds = new List<ElementId>();
							//eIds.Add(mra.Id);
							//eIds.Add(dimension.Id);

							//revitDoc.Selection.SetElementIds(eIds);
							tran.Commit();
						}
					}
					catch (Exception) { }
				}
				tg.Assimilate();
				return Autodesk.Revit.UI.Result.Succeeded;
			}
			catch (Exception e)
			{
				message = e.Message;
				return Autodesk.Revit.UI.Result.Failed;
			}
		}
	}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class AutoTag: IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{

			UIApplication uiapp = commandData.Application;
			Architexor.Custom.Application.thisApp.DoRequest(uiapp, ArchitexorRequestId.AutoTagging);
			return Result.Succeeded;

			//Tagging.DoTagging(doc, new Dictionary<string, object> { });
		}
	}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class AutoDimension : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			Architexor.Custom.Application.thisApp.DoRequest(uiapp, ArchitexorRequestId.AutoDimensioning);
			return Result.Succeeded;
		}
	}
}
