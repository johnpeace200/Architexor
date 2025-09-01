#region Namespaces
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Architexor.Utils;
using Color = Autodesk.Revit.DB.Color;
using View = Autodesk.Revit.DB.View;
#endregion // Namespaces

namespace Architexor.Commands
{
	[Transaction(TransactionMode.Manual)]
	public class CmdProfile : IExternalCommand
	{
		void SetModelCurvesColor(
		  ModelCurveArray modelCurves,
		  View view,
		  Color color)
		{
			foreach (var curve in modelCurves
			  .Cast<ModelCurve>())
			{
				var overrides = view.GetElementOverrides(
				  curve.Id);

				overrides.SetProjectionLineColor(color);

				view.SetElementOverrides(curve.Id, overrides);
			}
		}

		/// <summary>
		/// Improved implementation by Alexander Ignatovich
		/// supporting curved wall with curved window, 
		/// second attempt, published April 10, 2015:
		/// </summary>
		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			//  Get application and document objects
			UIApplication uiapp = commandData.Application;
			UIDocument uidoc = uiapp.ActiveUIDocument;
			Autodesk.Revit.ApplicationServices.Application app = uiapp.Application;
			Document doc = uidoc.Document;
			View view = doc.ActiveView;

			Autodesk.Revit.Creation.Application creapp
			  = app.Create;

			Autodesk.Revit.Creation.Document credoc
			  = doc.Create;

			Creator creator = new Creator(doc);

			List<Element> walls = new List<Element>();
			List<Element> floors = new List<Element>();
			List<Element> roofs = new List<Element>();

			if (!Util.GetSelectedElementsOrAll(
			  walls, uidoc, typeof(Wall))
				&& !Util.GetSelectedElementsOrAll(
			  floors, uidoc, typeof(Floor))
				&& !Util.GetSelectedElementsOrAll(
			  roofs, uidoc, typeof(RoofBase)))
			{
				Selection sel = uidoc.Selection;
				message = (0 < sel.GetElementIds().Count)
				  ? "Please select some wall/slab elements."
				  : "No wall/slab elements found.";

				return Result.Failed;
			}

			floors.AddRange(roofs);

			using (Transaction tx = new Transaction(doc))
			{
				tx.Start("Wall Profile");
				foreach (Element e in walls)
				{
					Wall wall = e as Wall;
					// Get the external wall face for the profile

					Reference sideFaceReference
					  = HostObjectUtils.GetSideFaces(
						wall, ShellLayerType.Exterior)
						  .First();

					Face face = wall.GetGeometryObjectFromReference(
					  sideFaceReference) as Face;

					// The plane and normal of the wall external face.

					XYZ normal = wall.Orientation.Normalize();
					Transform ftx = face.ComputeDerivatives(UV.Zero);
					XYZ forigin = ftx.Origin;
					XYZ fnormal = ftx.BasisZ;

					Debug.Print(
					  "wall orientation {0}, face origin {1}, face normal {2}",
					  Util.PointString(normal),
					  Util.PointString(forigin),
					  Util.PointString(fnormal));

					// Offset distance.

					double d = 5;

					// Offset curve copies for visibility.

					XYZ voffset = d * normal;
					Transform offset = Transform.CreateTranslation(
					  voffset);

					// If the curve loop direction is counter-
					// clockwise, change its color to RED.

					Color colorRed = new Color(255, 0, 0);

					// Get edge loops as curve loops.

					IList<CurveLoop> curveLoops
					  = face.GetEdgesAsCurveLoops();

					foreach (var curveLoop in curveLoops)
					{
						//CurveLoop curveLoopOffset = CurveLoop.CreateViaOffset(
						//  curveLoop, d, normal );

						CurveArray curves = creapp.NewCurveArray();

						foreach (Curve curve in curveLoop)
							curves.Append(curve.CreateTransformed(
							  offset));

						var isCounterClockwize = curveLoop
						  .IsCounterclockwise(normal);

						// Create model lines for an curve loop if it is made 

						Curve wallCurve = ((LocationCurve)wall.Location).Curve;

						if (wallCurve is Line)
						{
							//Plane plane = creapp.NewPlane( curves ); // 2016

							//Plane plane = curveLoopOffset.GetPlane(); // 2017

							Plane plane = Plane.CreateByNormalAndOrigin( // 2019
							  normal, forigin + voffset);

							Debug.Print(
							  "plane origin {0}, plane normal {1}",
							  Util.PointString(plane.Origin),
							  Util.PointString(plane.Normal));

							SketchPlane sketchPlane
							  = SketchPlane.Create(doc, plane);

							ModelCurveArray curveElements = credoc
							  .NewModelCurveArray(curves, sketchPlane);

							if (isCounterClockwize)
							{
								SetModelCurvesColor(curveElements,
								  view, colorRed);
							}
						}
						else
						{
							foreach (var curve in curves.Cast<Curve>())
							{
								var curveElements = creator.CreateModelCurves(curve);
								if (isCounterClockwize)
								{
									SetModelCurvesColor(curveElements, view, colorRed);
								}
							}
						}
					}
				}

				foreach (Element e in floors)
				{
					List<Reference> sideFaceReferences
					  = (List<Reference>)HostObjectUtils.GetTopFaces(
						e as HostObject);

					foreach (Reference sideFaceReference in sideFaceReferences)
					{
						Face face = e.GetGeometryObjectFromReference(
						  sideFaceReference) as Face;
						if (face == null)
							continue;

						// The plane and normal of the wall external face.

						XYZ normal = (face as PlanarFace).FaceNormal;
						Transform ftx = face.ComputeDerivatives(UV.Zero);
						XYZ forigin = ftx.Origin;
						XYZ fnormal = ftx.BasisZ;

						Debug.Print(
						  "wall orientation {0}, face origin {1}, face normal {2}",
						  Util.PointString(normal),
						  Util.PointString(forigin),
						  Util.PointString(fnormal));

						// Offset distance.

						double d = 5;

						// Offset curve copies for visibility.

						XYZ voffset = d * normal;
						Transform offset = Transform.CreateTranslation(
						  voffset);

						// If the curve loop direction is counter-
						// clockwise, change its color to RED.

						Color colorRed = new Color(255, 0, 0);

						// Get edge loops as curve loops.

						IList<CurveLoop> curveLoops
						  = face.GetEdgesAsCurveLoops();

						foreach (var curveLoop in curveLoops)
						{
							//CurveLoop curveLoopOffset = CurveLoop.CreateViaOffset(
							//  curveLoop, d, normal );

							CurveArray curves = creapp.NewCurveArray();

							foreach (Curve curve in curveLoop)
								curves.Append(curve.CreateTransformed(
								  offset));

							var isCounterClockwize = curveLoop
							  .IsCounterclockwise(normal);

							foreach (var curve in curves.Cast<Curve>())
							{
								var curveElements = creator.CreateModelCurves(curve);
								if (isCounterClockwize)
								{
									SetModelCurvesColor(curveElements, view, colorRed);
								}
							}
						}
					}
				}
				tx.Commit();
			}
			return Result.Succeeded;
		}
	}
}
