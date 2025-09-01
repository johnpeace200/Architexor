#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Architexor.Utils;
using View = Autodesk.Revit.DB.View;
#endregion // Namespaces

namespace Architexor.Commands
{
	[Transaction(TransactionMode.Manual)]
	class CmdOpeningProfiles : IExternalCommand
	{
		/// <summary>
		/// Retrieve all planar faces belonging to the 
		/// specified opening in the given wall.
		/// </summary>
		static List<PlanarFace> GetOpeningPlanarFaces(
		  HostObject obj,
		  ElementId openingId)
		{
			List<PlanarFace> faceList = new();

			List<Solid> solidList = new();

			Options geomOptions = obj.Document.Application.Create.NewGeometryOptions();

			if (geomOptions != null)
			{
				//geomOptions.ComputeReferences = true; // expensive, avoid if not needed
				//geomOptions.DetailLevel = ViewDetailLevel.Fine;
				//geomOptions.IncludeNonVisibleObjects = false;

				GeometryElement geoElem = obj.get_Geometry(geomOptions);

				if (geoElem != null)
				{
					foreach (GeometryObject geomObj in geoElem)
					{
						if (geomObj is Solid)
						{
							solidList.Add(geomObj as Solid);
						}
					}
				}
			}

			foreach (Solid solid in solidList)
			{
				foreach (Face face in solid.Faces)
				{
					if (face is PlanarFace)
					{
						if (obj.GetGeneratingElementIds(face)
						  .Any(x => x == openingId))
						{
							faceList.Add(face as PlanarFace);
						}
					}
				}
			}
			return faceList;
		}

		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Document doc = uidoc.Document;
			Result commandResult = Result.Succeeded;
			Categories cats = doc.Settings.Categories;

			ElementId catDoorsId = cats.get_Item(
			  BuiltInCategory.OST_Doors).Id;

			ElementId catWindowsId = cats.get_Item(
			  BuiltInCategory.OST_Windows).Id;

			try
			{
				List<ElementId> selectedIds = uidoc.Selection
				  .GetElementIds().ToList();

				TransactionGroup tg = new(doc);
				tg.Start("Transaction Group");

				List<ElementId> newIds = new();

				foreach (ElementId selectedId in selectedIds)
				{
					if (doc.GetElement(selectedId) is HostObject element)
					{
						List<Face> baseFaces = Util.GetBaseFacesOfElement(doc, element);

						List<PlanarFace> faceList = new();

						List<ElementId> insertIds = element.FindInserts(
							true, false, false, false).ToList();

						IEnumerable<Element> associatedElements = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
						.Where(m =>
							(m as FamilyInstance).Host != null && (m as FamilyInstance).Host.Id == element.Id
						);

						insertIds.Clear();
						foreach (Element e in associatedElements)
						{
							insertIds.Add(e.Id);
						}
						associatedElements = new FilteredElementCollector(doc).OfClass(typeof(Opening))
						.Where(m =>
							(m as Opening).Host != null && (m as Opening).Host.Id == element.Id
						);
						foreach (Element e in associatedElements)
						{
							insertIds.Add(e.Id);
						}

						foreach (ElementId insertId in insertIds)
						{
							Element elem = doc.GetElement(insertId);

							if (elem is FamilyInstance)
							{
								FamilyInstance inst = elem as FamilyInstance;

								CategoryType catType = inst.Category
									.CategoryType;

								Category cat = inst.Category;
								//
								//	&& (cat.Id == catDoorsId
								//	|| cat.Id == catWindowsId)
								if (catType == CategoryType.Model)
								{
									//	faceList.AddRange(
									//		GetOpeningPlanarFaces(
									//		element, insertId));
									faceList = GetOpeningPlanarFaces(element, insertId);
									Transaction trans = new(doc);
									trans.Start("Face");
									foreach (PlanarFace pf in faceList)
									{
										foreach (CurveLoop cl in pf.GetEdgesAsCurveLoops())
										{
											foreach (Curve c in cl)
											{
												if (c is Line)
												{
													foreach (Face f in baseFaces)
													{
														if (f is PlanarFace)
														{
															Plane plane = f.GetSurface() as Plane;
															if (Util.IsZero(plane.SignedDistanceTo(c.GetEndPoint(0)))
																&& Util.IsZero(plane.SignedDistanceTo(c.GetEndPoint(1))))
															{
																SketchPlane sketchPlane
								= SketchPlane.Create(doc, plane);

																ModelCurve modelCurve = doc.Create
																	.NewModelCurve(c, sketchPlane);

																newIds.Add(modelCurve.Id);
																break;
															}
														}
													}
												}
											}
										}
									}
									trans.Commit();
								}
							}
							else if (elem is Opening)
							{
								//faceList.AddRange(
								//  GetWallOpeningPlanarFaces(
								//	element, insertId));

								Transaction trans = new(doc);
								trans.Start("Get Model Lines");
								ICollection<ElementId> eIds = doc.Delete(insertId);
								trans.RollBack();

								trans = new Transaction(doc);
								trans.Start("Face");

								SketchPlane sp = null;
								foreach (ElementId eId in eIds)
								{
									if (doc.GetElement(eId) is SketchPlane)
										sp = doc.GetElement(eId) as SketchPlane;
								}

								foreach (ElementId eId in eIds)
								{
									if (doc.GetElement(eId) is ModelLine)
									{
										ModelLine ml = doc.GetElement(eId) as ModelLine;
										ModelCurve modelCurve = doc.Create
											.NewModelCurve(ml.GeometryCurve, sp);
										newIds.Add(modelCurve.Id);
									}
								}
								trans.Commit();
							}
						}
					}
				}

				if (newIds.Count > 0)
				{
					Transaction trans = new(doc);
					trans.Start("Isolate");
					View activeView = uidoc.ActiveGraphicalView;
					activeView.IsolateElementsTemporary(newIds);
					trans.Commit();
				}
				tg.Assimilate();
			}

			#region Exception Handling

			catch (Autodesk.Revit.Exceptions
			  .ExternalApplicationException e)
			{
				message = e.Message;
				Debug.WriteLine(
				  "Exception Encountered (Application)\n"
				  + e.Message + "\nStack Trace: "
				  + e.StackTrace);

				commandResult = Result.Failed;
			}
			catch (Autodesk.Revit.Exceptions
			  .OperationCanceledException e)
			{
				Debug.WriteLine("Operation cancelled. "
				  + e.Message);

				message = "Operation cancelled.";

				commandResult = Result.Succeeded;
			}
			catch (Exception e)
			{
				message = e.Message;
				Debug.WriteLine(
				  "Exception Encountered (General)\n"
				  + e.Message + "\nStack Trace: "
				  + e.StackTrace);

				commandResult = Result.Failed;
			}

			#endregion

			return commandResult;
		}
	}
}
