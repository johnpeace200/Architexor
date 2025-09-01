using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Architexor.Utils;
using View = Autodesk.Revit.DB.View;

namespace Architexor.Custom.Helpers
{
	internal static class Geometric
	{
		/// Object helpers
		#region Wall related functions
		public static bool IsWallExterior(Wall wall)
		{
			Element wallType = wall.Document.GetElement(wall.GetTypeId());
			Parameter pFunc = wallType.get_Parameter(BuiltInParameter.FUNCTION_PARAM);
			if (pFunc != null && pFunc.AsInteger() == (int)WallFunction.Exterior)
				return true;
			return false;
		}

		//	This function does not work
		//public static bool IsElementVisibleInView(Document doc, View view, Element element)
		//{
		//	//if element.IsVisibleInView(view))
		//	//	return false;

		//	bool isHiddenByCategory = false;
		//	bool isHiddenByElement = false;
		//	try
		//	{
		//		isHiddenByCategory = view.GetCategoryHidden(element.Category.Id);
		//	}
		//	catch { }
		//	try
		//	{
		//		isHiddenByElement = view.GetElementOverrides(element.Id).IsHiddenElement;
		//	}
		//	catch { }
		//	if (isHiddenByCategory || isHiddenByElement)
		//		return false;
		//	return true;
		//}

		/// <summary>
		/// Returns the normalized direction vector of the given wall
		/// </summary>
		/// <param name="wall"></param>
		/// <returns></returns>
		public static XYZ GetWallDirection(Wall wall)
		{
			Curve wallCurve = ((LocationCurve)wall.Location).Curve;
			return (wallCurve.GetEndPoint(1) - wallCurve.GetEndPoint(0)).Normalize();
		}

		public static double GetWallLength(Wall wall)
		{
			if(wall == null) return 0;

			if(wall.Location is LocationCurve locationCurve && locationCurve.Curve != null)
			{
				return locationCurve.Curve.Length;
			}

			return 0;
		}

		public static Face GetWallExteriorFace(Wall wall)
		{
			List<Reference> extRefers = (List<Reference>)HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
			foreach (Reference refer in extRefers)
			{
				GeometryObject go = wall.GetGeometryObjectFromReference(refer);
				if (go is Face face)
					return face;
			}
			return null;
		}

		public static Reference GetWallSideReference(Wall wall, ShellLayerType type)
		{
			List<Reference> extRefers = (List<Reference>)HostObjectUtils.GetSideFaces(wall, type);
			foreach (Reference refer in extRefers)
			{
				GeometryObject go = wall.GetGeometryObjectFromReference(refer);
				if (go is Face)
					return refer;
			}
			return null;
		}

		/// <summary>
		/// Return a unit vector in the XY-plane that is perpendicular
		/// to the wall's center-line (positive = exterior face).
		/// Works in any 2-D view.
		/// 
		/// Uses Wall.Orientation, which Revit keeps valid for *all*
		/// straight, curved, slanted, and copy-monitored walls.
		/// </summary>
		/// <param name="wall"></param>
		/// <returns></returns>
		public static XYZ GetWallNormal(Wall wall)
		{
			try
			{
				XYZ n = wall.Orientation;	//	already perpendicular to the wall
				if (n != null && n.GetLength() > TOL)
				{
					//if (wall.Flipped)
					//	n = n.Negate();
					return Unit(n);
				}
			}
			catch { }

			//	legacy fallback (rare)
			LocationCurve loc = wall.Location as LocationCurve;
			if (loc == null)
				return XYZ.BasisY;
			XYZ axis = loc.Curve.ComputeDerivatives(0.5, true).BasisX;
			if (axis.GetLength() < TOL)
				return XYZ.BasisY;
			XYZ n2 = XYZ.BasisZ.CrossProduct(axis).Normalize();
			if (n2.GetLength() < TOL)
				n2 = new XYZ(-axis.Y, axis.X, axis.Z).Normalize();
			if (wall.Flipped)
				n2 = new XYZ(-n2.X, -n2.Y, -n2.Z);
			return Unit(n2);
		}

		/// <summary>
		/// TODO: Change this method to "Check floor areas"
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		public static List<XYZ> GetExteriorWallPoints(Document doc)
		{
			List<Element> walls = (List<Element>)new FilteredElementCollector(doc)
					.OfCategory(BuiltInCategory.OST_Walls)
					.WhereElementIsNotElementType()
					.ToElements();
			List<XYZ> exteriorPoints = new List<XYZ>();
			foreach (Element wall in walls)
			{
				if (!(wall is Wall w)) continue;
				Element wallType = doc.GetElement(w.GetTypeId());

				//	Check if this wall is exterior
				if (wallType.LookupParameter("Function").AsInteger() != 1)
					continue;

				LocationCurve location = w.Location as LocationCurve;
				if (location != null)
				{
					Curve curve = location.Curve;
					exteriorPoints.Add(curve.GetEndPoint(0));
					exteriorPoints.Add(curve.GetEndPoint(1));
				}
			}
			return exteriorPoints;
		}

		///	Functions to get real exterior walls
		//	Group Structure
		class PolygonGroup
		{
			public List<XYZ> Boundary;
		}

		//	Equality comparer for 2D points
		class XYZEqualityComparer2D : IEqualityComparer<XYZ>
		{
			private const double TOL = 1e-4;
			public bool Equals(XYZ a, XYZ b)
			{
				return Math.Abs(a.X - b.X) < TOL && Math.Abs(a.Y - b.Y) < TOL;
			}

			public int GetHashCode(XYZ obj)
			{
				return (obj.X.GetHashCode() * 397) ^ obj.Y.GetHashCode();
			}
		}

		//	Geometrical information of wall on Plan View
		//	All Z information here are meaningless
		public struct WallGeoInfo
		{
			public ElementId Id;

			public Curve Curve;

			//	Projected information on the plan
			public BoundingBoxXYZ BoundingBox;
			public XYZ[] Points;
		}

		public static Solid GetWallSolid(Wall wall, View view)
		{
			Options options = new Options { ComputeReferences = true, View = view };
			GeometryElement geomElem = wall.get_Geometry(options);

			foreach (GeometryObject obj in geomElem)
			{
				if (obj is Solid solid && solid.Volume > 0)
					return solid;

				if (obj is GeometryInstance inst)
				{
					foreach (GeometryObject nested in inst.GetInstanceGeometry())
					{
						if (nested is Solid nestedSolid && nestedSolid.Volume > 0)
							return nestedSolid;
					}
				}
			}
			return null;
		}

		public static List<ElementId> GetLocationCurve(Document doc, ViewPlan view, List<ElementId> ids)
		{
			List<ElementId> generated = new List<ElementId>();

			PlanViewRange pvr = view.GetViewRange();
			Level cutLevel = doc.GetElement(pvr.GetLevelId(PlanViewPlane.CutPlane)) as Level;
			Level topLevel = doc.GetElement(pvr.GetLevelId(PlanViewPlane.TopClipPlane)) as Level;

			List<Wall> walls = new FilteredElementCollector(doc, view.Id)
												.OfClass(typeof(Wall))
												.Cast<Wall>()
												.ToList();
			generated = walls.Where(w => doc.GetElement(w.GetTypeId()).get_Parameter(BuiltInParameter.FUNCTION_PARAM).AsInteger() == 1).Select(x => x.Id).ToList();//WallFunction.Exterior
			return generated;

			double offset = pvr.GetOffset(PlanViewPlane.CutPlane);

			double topZ = cutLevel.Elevation + offset;

			// Construct the top plane (XY at topZ)
			XYZ origin = new XYZ(0, 0, topZ);
			XYZ normal = XYZ.BasisZ;
			Plane plane = Plane.CreateByNormalAndOrigin(normal, origin);

			SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

			foreach (ElementId id in ids) {
				Wall wall = doc.GetElement(id) as Wall;
				if(wall == null)
				{
					continue;
				}

//				Parameter param = wall.get_Parameter(BuiltInParameter.WALL_KEY_REF_PARAM);
//				param.AsInteger() == WallLocationLine.

				LocationCurve locationCurve = wall.Location as LocationCurve;
				Curve curve = locationCurve.Curve;
				XYZ p0 = curve.GetEndPoint(0), p1 = curve.GetEndPoint(1);
				XYZ direction = (p1 - p0).Normalize();

				//	Get Joined walls
				/*ElementArray elems = locationCurve.get_ElementsAtJoin(0);
				foreach(Element elem in elems)
				{
					Wall otherWall = elem as Wall;
					if (otherWall == null || otherWall.Id == wall.Id)
						continue;

					Curve c = (otherWall.Location as LocationCurve).Curve;
					XYZ p2 = c.GetEndPoint(0), p3 = c.GetEndPoint(1);
					XYZ dir = (p3 - p2).Normalize();
					if (dir.IsAlmostEqualTo(direction) || dir.IsAlmostEqualTo(direction.Negate()))
					{
						// Get the point where wall and joinedWall meet
						XYZ closestEnd = p0.DistanceTo(p2) < p1.DistanceTo(p2) ? p0 : p1;

						// Determine which end to extend (p0 or p1)
						bool extendStart = p0.DistanceTo(closestEnd) < p1.DistanceTo(closestEnd);

						double width = p2.DistanceTo(p0) < p3.DistanceTo(p0) ? (p2 - p0).GetLength(): (p3 - p0).GetLength();
						if(!extendStart)
						{
							width = p2.DistanceTo(p0) < p3.DistanceTo(p0) ? (p2 - p1).GetLength() : (p3 - p1).GetLength();
						}
						
						//if (extendStart)
						//	p0 = p0 + (p0 - p1).Normalize().Multiply(width);
						//else
						//	p1 = p1 + (p1 - p0).Normalize().Multiply(width);
					}
					else
					{
						double width = otherWall.Width / 2;

						// Get the point where wall and joinedWall meet
						XYZ closestEnd = p0.DistanceTo(p2) < p1.DistanceTo(p2) ? p0 : p1;

						// Determine which end to extend (p0 or p1)
						bool extendStart = p0.DistanceTo(closestEnd) < p1.DistanceTo(closestEnd);

						// Compute perpendicular direction (rotate 90° in plane)
						XYZ wallNormal = direction.CrossProduct(XYZ.BasisZ); // Assuming walls are on XY plane
						XYZ perpToJoined = dir.CrossProduct(XYZ.BasisZ);

						// Choose which side to extend based on alignment
						XYZ extendDir = perpToJoined.DotProduct(wallNormal) > 0 ? perpToJoined.Negate() : perpToJoined;

						if (extendStart)
							p0 = p0 + (p0 - p1).Normalize().Multiply(width);
						else
							p1 = p1 + (p1 - p0).Normalize().Multiply(width);
					}
				}
				elems = locationCurve.get_ElementsAtJoin(1);
				foreach (Element elem in elems)
				{
					Wall otherWall = elem as Wall;
					if (otherWall == null || otherWall.Id == wall.Id)
						continue;

					Curve c = (otherWall.Location as LocationCurve).Curve;
					XYZ p2 = c.GetEndPoint(0), p3 = c.GetEndPoint(1);
					XYZ dir = (p3 - p2).Normalize();
					if (dir.IsAlmostEqualTo(direction) || dir.IsAlmostEqualTo(direction.Negate()))
					{
						// Get the point where wall and joinedWall meet
						XYZ closestEnd = p0.DistanceTo(p2) < p1.DistanceTo(p2) ? p0 : p1;

						// Determine which end to extend (p0 or p1)
						bool extendStart = p0.DistanceTo(closestEnd) < p1.DistanceTo(closestEnd);

						double width = p2.DistanceTo(p0) < p3.DistanceTo(p0) ? (p2 - p0).GetLength() : (p3 - p0).GetLength();
						if (!extendStart)
						{
							width = p2.DistanceTo(p0) < p3.DistanceTo(p0) ? (p2 - p1).GetLength() : (p3 - p1).GetLength();
						}

						//if (extendStart)
						//	p0 = p0 + (p0 - p1).Normalize().Multiply(width);
						//else
						//	p1 = p1 + (p1 - p0).Normalize().Multiply(width);
					}
					else
					{
						double width = otherWall.Width / 2;

						// Get the point where wall and joinedWall meet
						XYZ closestEnd = p0.DistanceTo(p2) < p0.DistanceTo(p2) ? p0 : p1;

						// Determine which end to extend (p0 or p1)
						bool extendStart = p0.DistanceTo(closestEnd) < p1.DistanceTo(closestEnd);

						// Compute perpendicular direction (rotate 90° in plane)
						XYZ wallNormal = direction.CrossProduct(XYZ.BasisZ); // Assuming walls are on XY plane
						XYZ perpToJoined = dir.CrossProduct(XYZ.BasisZ);

						// Choose which side to extend based on alignment
						XYZ extendDir = perpToJoined.DotProduct(wallNormal) > 0 ? perpToJoined.Negate() : perpToJoined;

						if (extendStart)
							p0 = p0 + (p0 - p1).Normalize().Multiply(width);
						else
							p1 = p1 + (p1 - p0).Normalize().Multiply(width);
					}
				}*/

				XYZ pt1 = curve.GetEndPoint(0), pt2 = curve.GetEndPoint(1);
				pt1 = plane.ProjectOnto(p0);
				pt2 = plane.ProjectOnto(p1);
				if ((pt2 - pt1).Normalize().IsAlmostEqualTo(XYZ.BasisZ))
				{
					continue;
				}

				BoundingBoxXYZ bb = wall.get_BoundingBox(view)
					, bb1 = wall.get_BoundingBox(null);
				
				DetailCurve mc = doc.Create.NewDetailCurve(view, Line.CreateBound(pt1, pt2));
				generated.Add(mc.Id);
			}
			return generated;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="plan"></param>
		/// <returns></returns>
		public static List<Wall> GetRealExteriorWalls(ViewPlan plan)
		{
			List<ElementId> exteriorWallIds = new List<ElementId>();

			Document doc = plan.Document;

			PlanViewRange pvr = plan.GetViewRange();
			Level cutLevel = doc.GetElement(pvr.GetLevelId(PlanViewPlane.CutPlane)) as Level;
			Level topLevel = doc.GetElement(pvr.GetLevelId(PlanViewPlane.TopClipPlane)) as Level;

			double offset = pvr.GetOffset(PlanViewPlane.CutPlane);

			double topZ = cutLevel.Elevation + offset;

			// Construct the top plane (XY at topZ)
			XYZ origin = new XYZ(0, 0, topZ);
			XYZ normal = XYZ.BasisZ;
			Plane plane = Plane.CreateByNormalAndOrigin(normal, origin);

			//	Get all candidates
			List<Wall> walls = new FilteredElementCollector(doc, plan.Id)
												.OfClass(typeof(Wall))
												.Cast<Wall>()
												//.Where(
												//		w => //w.WallType.Function == WallFunction.Exterior &&
												//				 w.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE) != null &&
												//				 w.LevelId == plan.GenLevel.Id)
												//		w.Location is LocationCurve)
												.ToList();
			List<ElementId> curves = new List<ElementId>();
			SketchPlane sketchPlane = SketchPlane.Create(doc, plane);

			foreach(Wall wall in walls)
			{
				Curve curve = (wall.Location as LocationCurve).Curve;
				XYZ pt1 = curve.GetEndPoint(0), pt2 = curve.GetEndPoint(1);
				pt1 = plane.ProjectOnto(pt1);
				pt2 = plane.ProjectOnto(pt2);
				if((pt2 - pt1).Normalize().IsAlmostEqualTo(XYZ.BasisZ))
				{
					continue;
				}
				ModelCurve mc = doc.Create.NewModelCurve(Line.CreateBound(pt1, pt2), sketchPlane);
				curves.Add(mc.Id);
			}

			return new List<Wall>();

			//	Get geometrical information of all walls for future calculation
			List<WallGeoInfo> geoInfos = new List<WallGeoInfo>();

			BoundingBoxXYZ gBB = null;
			foreach(Wall wall in walls)
			{
				if (wall.Location is LocationCurve locationCurve && locationCurve.Curve != null)
				{
					//	Get all projected points
					Curve c = locationCurve.Curve;
					List<XYZ> pts = new List<XYZ>();
					try
					{
						int i = 0;
						while (true)
						{
							XYZ pt = c.GetEndPoint(i++);
							pts.Add(plane.ProjectOnto(pt));
						}
					}
					catch { }
					
					BoundingBoxXYZ bb = new BoundingBoxXYZ();
					bb.Min = pts[0];	bb.Max = pts[0];
					foreach(XYZ pt in pts)
					{
						bb.ExpandToContain(pt);
					}
					c = Line.CreateBound(bb.Min, bb.Max);

					if(gBB == null)
					{
						//	Clone the first one
						gBB = new BoundingBoxXYZ();
						gBB.Min = bb.Min;
						gBB.Max = bb.Max;
					}
					else
					{
						gBB.ExpandToContain(bb);
					}

					WallGeoInfo info = new WallGeoInfo
					{
						Id = wall.Id,
						Curve = locationCurve.Curve,
						BoundingBox = bb,
						Points = pts.ToArray(),
					};

					geoInfos.Add(info);
				}
			}

			//	Get all rays
			List<double> xs = new List<double>(), ys = new List<double>();
			foreach(WallGeoInfo geoInfo in geoInfos)
			{
				if (!Util.IsEqual(geoInfo.Points[0].X, geoInfo.Points[1].X))
				{
					//	Horizontal wall
					xs.Add((geoInfo.Points[0].X + geoInfo.Points[1].X) / 2);
				}
				if (!Util.IsEqual(geoInfo.Points[0].Y, geoInfo.Points[1].Y))
				{
					ys.Add((geoInfo.Points[0].Y + geoInfo.Points[1].Y) / 2);
				}
			}
			xs = xs.OrderBy(x => x).ToList();
			ys = ys.OrderBy(x => x).ToList();

			foreach (WallGeoInfo geoInfo in geoInfos)
			{
				Line line = Line.CreateBound(geoInfo.Points[0], geoInfo.Points[1]);

				if (!Util.IsEqual(geoInfo.Points[0].X, geoInfo.Points[1].X))
				{
					//	Horizontal wall
					foreach (double x in xs)
					{
						Line ray = Line.CreateBound(new XYZ(x, gBB.Min.Y, gBB.Min.Z), new XYZ(x, gBB.Max.Y, gBB.Min.Z));

						//	Check intersection with ray
						if(ray.Intersect(line, out IntersectionResultArray ra) != SetComparisonResult.Overlap)
						{
							continue;
						}

						//if(geoInfo.Id.Value == 1474830)
						//{

						//}

						XYZ intersectPt = ra.get_Item(0).XYZPoint;
						double distanceFromMax = gBB.Max.Y - intersectPt.Y;

						bool bNegative = false, bPositive = false;
						foreach (WallGeoInfo geoInfo1 in geoInfos)
						{
							if (geoInfo.Id == geoInfo1.Id)
								continue;

							XYZ pt1 = geoInfo1.Points[0], pt2 = geoInfo1.Points[1];
							//	Make it a bit longer
							double distance = pt1.DistanceTo(pt2);
							XYZ direction = (pt2 - pt1).Normalize();
							pt1 = pt1.Subtract(direction.Multiply(distance / 10));
							pt2 = pt2.Add(direction.Multiply(distance / 10));
							Line curve = Line.CreateBound(pt1, pt2);

							if (ray.Intersect(curve, out IntersectionResultArray ra1) == SetComparisonResult.Overlap)
							{
								XYZ pt = ra1.get_Item(0).XYZPoint;

								try
								{
									//	Check if it really intersects with Solid
									Solid otherSolid = GetWallSolid(doc.GetElement(geoInfo1.Id) as Wall, plan);
									if (otherSolid == null) continue;

									var result = otherSolid.IntersectWithCurve(ray, new SolidCurveIntersectionOptions());
									if (result != null && result.SegmentCount > 0)
									{
										// If intersection is beyond the current wall, it's an interior wall
										foreach (XYZ point in result.GetCurveSegment(0).Tessellate())
										{
											double d = gBB.Max.DistanceTo(pt);
											if (!bNegative && d < distanceFromMax)
											{
												bNegative = true;
											}
											if (!bPositive && d > distanceFromMax)
											{
												bPositive = true;
											}
										}
									}
								}
								catch { }
								//double d = gBB.Max.Y - pt.Y;
								//if (!bNegative && d < distanceFromMax)
								//{
								//	bNegative = true;
								//}
								//if (!bPositive && d > distanceFromMax)
								//{
								//	bPositive = true;
								//}

								if (bNegative && bPositive)
									break;
							}
						}
						if (!bNegative || !bPositive)
						{
							//if (minId.Value == 1300708)
							//{

							//}
							exteriorWallIds.Add(geoInfo.Id);
							break;
						}
					}
				}
				if (!Util.IsEqual(geoInfo.Points[0].Y, geoInfo.Points[1].Y))
				{

				}
			}

			return exteriorWallIds.Select(x => doc.GetElement(x)).Cast<Wall>().ToList();
		}

		//public static List<Wall> GetRealExteriorWalls(ViewPlan plan)
		//{
		//	//	Collect all Floor elements in the plan view.
		//	//	Get each floor’s projected boundary curves(outer profile).
		//	//	Group overlapping/ adjacent floors into clusters(floor groups).
		//	//	For each group, build a merged polygon.
		//	//	Collect all walls on the same level.
		//	//	Test if the wall location line lies on the outer boundary of a group polygon.
		//	//	Return such walls as true exterior walls.
		//	List<Wall> exteriorWalls = new List<Wall>();

		//	Document doc = plan.Document;
		//	Level level = doc.GetElement(plan.GenLevel.Id) as Level;
		//	if (level == null) return exteriorWalls;

		//	//	1. Collect all floors at this level
		//	List<Floor> floors = new FilteredElementCollector(doc, plan.Id)
		//									.OfClass(typeof(Floor))
		//									.Cast<Floor>()
		//									.Where(f => f.get_Parameter(BuiltInParameter.LEVEL_PARAM)?.AsElementId() == level.Id)
		//									.ToList();

		//	//	2. Convert floor boundary loops to flattened 2D polygons(projected to XY)
		//	List<List<XYZ>> floorPolygons = new List<List<XYZ>>();
		//	foreach (Floor floor in floors)
		//	{
		//		Options opt = new Options
		//		{
		//			ComputeReferences = false,
		//			IncludeNonVisibleObjects = false,
		//			DetailLevel = ViewDetailLevel.Fine
		//		};
		//		GeometryElement geo = floor.get_Geometry(opt);

		//		foreach (GeometryObject obj in geo)
		//		{
		//			if (obj is Solid solid && solid.Faces.Size > 0)
		//			{
		//				Face bottomFace = GetBottomFace(solid);
		//				if (bottomFace != null)
		//				{
		//					Mesh mesh = bottomFace.Triangulate();
		//					List<XYZ> points = mesh.Vertices.Distinct(new XYZEqualityComparer2D()).ToList();
		//					if (points.Count >= 3)
		//					{
		//						floorPolygons.Add(points);
		//					}
		//				}
		//			}
		//		}
		//	}

		//	//	3. Merge floor polygons into unified boundary groups
		//	List<PolygonGroup> groups = GroupAndMergePolygons(floorPolygons);

		//	//	4. Collect all candidate walls on this level
		//	List<Wall> walls = new FilteredElementCollector(doc, plan.Id)
		//										.OfClass(typeof(Wall))
		//										.Cast<Wall>()
		//										.Where(
		//												w => //w.WallType.Function == WallFunction.Exterior &&
		//												//w.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE) != null &&
		//												//w.LevelId == level.Id &&
		//												w.Location is LocationCurve)
		//										.ToList();
		//	Wall test = walls.Find(x => x.Id == new ElementId(678361));
		//	test = doc.GetElement(new ElementId(678361)) as Wall;

		//	foreach(Wall wall in walls)
		//	{
		//		Curve curve = (wall.Location as LocationCurve).Curve;
		//		XYZ mid = curve.Evaluate(0.5, true);
		//		XYZ mid2D = new XYZ(mid.X, mid.Y, 0);

		//		foreach(var group in groups)
		//		{
		//			if(IsPointOnPolygonEdge(group.Boundary, mid2D, 0.5))  // 0.5ft tolerance
		//			{
		//				exteriorWalls.Add(wall);
		//				break;
		//			}
		//		}
		//	}

		//	List<ElementId> ids = exteriorWalls.Select(x => x.Id).ToList();

		//	return exteriorWalls;
		//}

		/// <summary>
		/// Get outer walls in the cropped view
		/// </summary>
		/// <param name="plan"></param>
		/// <returns></returns>
		public static List<Wall> GetOuterWalls(ViewPlan plan)
		{
			//	Collect all rooms in the plan view.
			//	Get each room’s projected boundary curves.
			//	Collect all walls on the plan view.
			//	Test if the wall location line lies on the outer boundary of a group polygon.
			//	Return such walls as true exterior walls.
			List<Wall> outerWalls = new List<Wall>();

			Document doc = plan.Document;
			// 1. Collect all Rooms in the view
			var collector = new FilteredElementCollector(doc, plan.Id)
					.OfCategory(BuiltInCategory.OST_Rooms)
					.WhereElementIsNotElementType();

			if(collector.Count() == 0)
			{
				return outerWalls;
			}

			// 2. Get polygons of room
			SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions
			{
				SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish,
				StoreFreeBoundaryFaces = false
			};

			List<List<XYZ>> roomPolygons = new List<List<XYZ>>();

			foreach(Room room in collector)
			{
				if (room.Area <= 0 || room.Location == null) continue;

				IList<IList<BoundarySegment>> boundaries = room.GetBoundarySegments(options);
				if (boundaries == null) continue;

				foreach(var boundaryLoop in boundaries)
				{
					List<XYZ> polygon = new List<XYZ>();
					foreach(var segment in boundaryLoop)
					{
						polygon.Add(segment.GetCurve().GetEndPoint(0));
					}
					roomPolygons.Add(polygon); // Each loop represents a polygon
				}
			}

			//	3. Merge room polygons into unified boundary groups
			List<PolygonGroup> groups = GroupAndMergePolygons(roomPolygons);

			// 4. Get a single perimeter polygon that contains all room polygons
			var points2D = new List<XYZ>();
			foreach(var poly in roomPolygons)
			{
				foreach(var pt in poly)
				{
					points2D.Add(new XYZ(pt.X, pt.Y, 0)); // Project to 2D
				}
			}
			var hull = ComputeConvexHull(points2D);
			var perimeterPolygon = hull.Select(pt => new XYZ(pt.Item1, pt.Item2, 0)).ToList();

			// 5. Collect all candidate walls on the plan
			List<Wall> walls = new FilteredElementCollector(doc, plan.Id)
												.OfClass(typeof(Wall))
												.Cast<Wall>()
												.Where(
														w => w.Location is LocationCurve)
												.ToList();

			foreach (Wall wall in walls)
			{
				Curve curve = (wall.Location as LocationCurve).Curve;
				XYZ mid = curve.Evaluate(0.5, true);
				XYZ mid2D = new XYZ(mid.X, mid.Y, 0);

				if (IsCurveOnPolygonEdge(perimeterPolygon, curve, 0.5) || IsPointOnPolygonEdge(perimeterPolygon, mid2D, 0.5))  // 0.5ft tolerance
				{
					outerWalls.Add(wall);
				}
			}

			return outerWalls;
		}

		/// <summary>
		/// 	Extract bottom face of a floor
		/// </summary>
		/// <param name="solid"></param>
		/// <returns></returns>
		private static Face GetBottomFace(Solid solid)
		{
			Face lowestFace = null;
			double minZ = double.MaxValue;

			foreach(Face face in solid.Faces)
			{
				BoundingBoxUV uvBox = face.GetBoundingBox();
				UV centerUV = (uvBox.Min + uvBox.Max) * 0.5;
				XYZ normal = face.ComputeNormal(centerUV);
				XYZ origin = face.Evaluate(centerUV);

				if(normal.Z > 0.99 && origin.Z < minZ)
				{
					minZ = origin.Z;
					lowestFace = face;
				}
			}
			return lowestFace;
		}

		/// <summary>
		/// Check if curve is pass polygon edge
		/// </summary>
		/// <param name="poly">Polygon vertices</param>
		/// <param name="curve">Curve to check</param>
		/// <param name="tolerance">Distance tolerance</param>
		/// <returns>True if curve lies on any polygone edge</returns>
		private static bool IsCurveOnPolygonEdge(List<XYZ> poly, Curve curve, double tolerance)
		{
			if (curve == null || !curve.IsBound) return false;

			var curveStart = curve.GetEndPoint(0);
			var curveEnd = curve.GetEndPoint(1);

			int count = poly.Count;
			for (int i = 0; i < count; i++)
			{
				XYZ a = poly[i];
				XYZ b = poly[(i + 1) % count];

				if(IsPointNearLineSegment2D(curveStart, curveEnd, a, tolerance) &&
					IsPointNearLineSegment2D(curveStart, curveEnd, b, tolerance))
				{
					return true;
				}

			}

			return false;
		}


		/// <summary>
		/// Check if point is on polygon edge
		/// </summary>
		/// <param name="poly"></param>
		/// <param name="point"></param>
		/// <param name="tolerance"></param>
		/// <returns></returns>
		private static bool IsPointOnPolygonEdge(List<XYZ> poly, XYZ point, double tolerance)
		{
			int count = poly.Count;
			for (int i = 0; i < count; i++)
			{
				XYZ a = poly[i];
				XYZ b = poly[(i + 1) % count];
				if (IsPointNearLineSegment2D(a, b, point, tolerance))
					return true;
			}

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <param name="p"></param>
		/// <param name="tol"></param>
		/// <returns></returns>
		private static bool IsPointNearLineSegment2D(XYZ a, XYZ b, XYZ p, double tol)
		{
			double dx = b.X - a.X;
			double dy = b.Y - a.Y;
			double lengthSq = dx * dx + dy * dy;
			if (lengthSq == 0) return (a - p).GetLength() < tol;

			double t = ((p.X - a.X) * dx + (p.Y - a.Y) * dy) / lengthSq;
			t = Math.Max(0, Math.Min(1, t));
			double projX = a.X + t * dx;
			double projY = a.Y + t * dy;

			double dist = Math.Sqrt(Math.Pow(p.X - projX, 2) + Math.Pow(p.Y - projY, 2));

			return dist <= tol;
		}

		/// <summary>
		/// Grouping/merging logic: naive union (no geometry lib used here)
		/// TODO
		/// </summary>
		/// <param name="polygons"></param>
		/// <returns></returns>
		private static List<PolygonGroup> GroupAndMergePolygons(List<List<XYZ>> polygons)
		{
			var groups = new List<List<List<XYZ>>>();

			foreach(var poly in polygons)
			{
				bool added = false;
				foreach(var group in groups)
				{
					if(group.Any(other => PolygonsIntersect(poly, other)))
					{
						group.Add(poly);
						added = true;
						break;
					}
				}
				if (!added)
				{
					groups.Add(new List<List<XYZ>> { poly });
				}
			}

			List<PolygonGroup> merged = new List<PolygonGroup>();
			foreach(var group in groups)
			{
				var allPoints = group.SelectMany(p => p).ToList();
				merged.Add(new PolygonGroup { Boundary = ConvexHull2D(allPoints) });
			}

			return merged;
		}

		/// <summary>
		/// Check if two polygons intersect
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static bool PolygonsIntersect(List<XYZ> a, List<XYZ> b)
		{
			if (!BoundingBoxesOverlap(a, b))
				return false;

			return a.Any(pt => IsPointInPolygon(pt, b)) || b.Any(pt => IsPointInPolygon(pt, a));
		}

		/// <summary>
		/// Check if two bounding boxes of two polygons overlap
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		private static bool BoundingBoxesOverlap(List<XYZ> a, List<XYZ> b)
		{
			var (minAx, maxAx, minAy, maxAy) = GetBounds(a);
			var (minBx, maxBx, minBy, maxBy) = GetBounds(b);

			return !(maxAx < minBx || maxBx < minAx || maxAy < minBy || maxBy < minAy);
		}

		private static (double minX, double maxX, double minY, double maxY) GetBounds(List<XYZ> pts)
		{
			var xs = pts.Select(p => p.X);
			var ys = pts.Select(p => p.Y);
			return (xs.Min(), xs.Max(), ys.Min(), ys.Max());
		}

		/// <summary>
		/// Simple gift-wrapping algorithm
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		private static List<XYZ> ConvexHull2D(List<XYZ> points)
		{
			List<XYZ> unique = points.Distinct(new XYZEqualityComparer2D()).ToList();
			if (unique.Count < 3) return unique;

			List<XYZ> hull = new List<XYZ>();
			XYZ start = unique.OrderBy(p => p.X).ThenBy(p => p.Y).First();
			XYZ current = start;

			XYZEqualityComparer2D comparer = new XYZEqualityComparer2D();
			do
			{
				hull.Add(current);
				XYZ next = unique[0];
				foreach (XYZ candidate in unique)
				{
					if (comparer.Equals(candidate, current)) continue;
					double cross = Cross(current, next, candidate);
					if (next.Equals(current) || cross < 0 ||
						(Math.Abs(cross) < 1e-6 && Distance2D(current, candidate) > Distance2D(current, next)))
					{
						next = candidate;
					}
				}
				current = next;
			} while (!comparer.Equals(current, start));

			return hull;
		}

		private static double Cross(XYZ o, XYZ a, XYZ b)
		{
			return (a.X - o.X) * (b.Y - o.Y) - (a.Y - o.Y) * (b.X - o.X);
		}

		private static double Distance2D(XYZ a, XYZ b)
		{
			return Distance2DTuple((a.X, b.X), (a.Y, b.Y));
		}

		private static double Distance2DTuple((double, double) a, (double, double) b)
		{
			return Math.Sqrt(Math.Pow(a.Item1 - b.Item1, 2) + Math.Pow(a.Item2 - b.Item2, 2));
		}
		///

		/// <summary>
		/// Logic to determine if the face belongs to an intersection
		/// Example: Check if the face is near another wall's geometry
		/// </summary>
		/// <param name="face"></param>
		/// <param name="wall"></param>
		/// <returns></returns>
		public static bool IsIntersectionFace(Face face, Wall wall)
		{
			List<Wall> intersectingElements = (List<Wall>)new FilteredElementCollector(wall.Document)
					.OfClass(typeof(Wall)).ToElements();
			foreach (Wall otherWall in intersectingElements)
			{
				if (otherWall.Id == wall.Id) continue;

				Options opts = new Options { ComputeReferences = true };
				GeometryElement geo = otherWall.get_Geometry(opts);
				foreach (GeometryObject obj in geo)
				{
					if (obj is Solid solid)
					{
						foreach (Face otherFace in solid.Faces)
						{
							if (face.Intersect(otherFace) == FaceIntersectionFaceResult.Intersecting)
								return true;
						}
					}
				}
			}
			return false;
		}

		public static Reference GetWallCenterlineReference(Wall wall)
		{
			// Try stable representation first
			string format = "{0}:{1}:{2}";
			ElementId uid = wall.Id;
			string nines = "LINEAR";
			string refString = string.Format(format, uid, nines, 1);

			try
			{
				return Reference.ParseFromStableRepresentation(wall.Document, refString);
			}
			catch {
				return null;
			}
		}

		/// <summary>
		/// Finds walls that intersect with the given wall in the specified view.
		/// </summary>
		/// <param name="wall">The wall to check intersections for.</param>
		/// <param name="view">The Revit view.</param>
		/// <returns>List of intersecting Wall elements.</returns>
		public static List<Wall> GetIntersectingWalls(Wall wall, View view)
		{
			Document doc = wall.Document;
			BoundingBoxXYZ wallBbox = wall.get_BoundingBox(null);
			if (wallBbox == null)
				return new List<Wall>();

			List<Wall> walls = new FilteredElementCollector(doc, view.Id)
								.OfClass(typeof(Wall))
								.Cast<Wall>()
								.ToList();

			var intersectingWalls = new List<Wall>();
			foreach (Wall otherWall in walls)
			{
				if (otherWall.Id == wall.Id)
					continue;

				BoundingBoxXYZ otherBbox = otherWall.get_BoundingBox(null);
				if (otherBbox == null)
					continue;

				if (BoundingBoxesIntersect(wallBbox, otherBbox))
					intersectingWalls.Add(otherWall);
			}
			return intersectingWalls;
		}
		#endregion

		public static SetComparisonResult IsIntersectWith(Element e1, Element e2)
		{
			GeometryElement geomElem1 = e1.get_Geometry(new Options());
			GeometryElement geomElem2 = e2.get_Geometry(new Options());
			foreach (GeometryObject geomObject1 in geomElem1)
			{
				if (geomObject1 is Solid solid1)
				{
					foreach (Face f1 in solid1.Faces)
					{
						foreach (GeometryObject geomObject2 in geomElem2)
						{
							if (geomObject2 is Solid solid2)
							{
								foreach (Face f2 in solid2.Faces)
								{
									var scr = f1.Intersect(f2);
									if (scr == FaceIntersectionFaceResult.Intersecting)
										return SetComparisonResult.Overlap;
								}
							}
						}
					}
				}
			}
			return SetComparisonResult.Disjoint;
		}

		#region Geometric helpers
		public const double TOL = 1e-9;
		public static readonly XYZ _Z = XYZ.BasisZ;

		/// <summary>
		/// Compute convex hull of this points array
		/// </summary>
		/// <param name="points"></param>
		/// <returns></returns>
		public static List<(double, double)> ComputeConvexHull(IEnumerable<XYZ> points)
		{
			var pts = points.Select(p => (p.X, p.Y)).Distinct().OrderBy(p => p.Item1).ThenBy(p => p.Item2).ToList();

			double Cross((double, double) o, (double, double) a, (double, double) b)
					=> (a.Item1 - o.Item1) * (b.Item2 - o.Item2) - (a.Item2 - o.Item2) * (b.Item1 - o.Item1);

			var lower = new List<(double, double)>();
			foreach (var p in pts)
			{
				while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
					lower.RemoveAt(lower.Count - 1);
				lower.Add(p);
			}

			var upper = new List<(double, double)>();
			for (int i = pts.Count - 1; i >= 0; i--)
			{
				var p = pts[i];
				while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
					upper.RemoveAt(upper.Count - 1);
				upper.Add(p);
			}

			lower.RemoveAt(lower.Count - 1);
			upper.RemoveAt(upper.Count - 1);
			lower.AddRange(upper);
			return lower;
		}

		// --- Global cache: compute once per run --------------------------------------
		//private static List<(double x1, double y1, double x2, double y2, RectangleF rect)> _SEG_CACHE = null;	//	list of (x1, y1, x2, y2, RectangleF)
		//private static Dictionary<(int gx, int gy), List<(double x1, double y1, double x2, double y2, RectangleF rect)>> _SEG_GRID = null;	//	{(gx, gy): [segments]} - spatial index (built once)
		//private const double _GRID_SIZE = 12.0; // grid cell width/height in internal ft (≈ 1200 mm)

		/// <summary>
		/// Build the flat segment list and a very light-weight grid
		/// so that later overlap tests only look at nearby segments.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="view"></param>
		//public static void InitSegmentCache2(Document doc, View view)
		//{
		//	_SEG_CACHE = new List<(double, double, double, double, RectangleF)>();
		//	_SEG_GRID = new Dictionary<(int, int), List<(double, double, double, double, RectangleF)>>();

		//	List<(double x1, double y1, double x2, double y2)> raw = ExtractLineSegments(doc, view);
		//	double gs = _GRID_SIZE;

		//	foreach (var seg in raw)
		//	{
		//		var (x1, y1, x2, y2) = seg;
		//		double xmin = Math.Min(x1, x2);
		//		double xmax = Math.Max(x1, x2);
		//		double ymin = Math.Min(y1, y2);
		//		double ymax = Math.Max(y1, y2);

		//		RectangleF rect = new RectangleF((float)xmin, (float)ymin, (float)(xmax - xmin), (float)(ymax - ymin));
		//		var seg5 = (x1, y1, x2, y2, rect);
		//		_SEG_CACHE.Add(seg5);

		//		int gx_min = (int)Math.Floor(xmin / gs);
		//		int gx_max = (int)Math.Floor(xmax / gs);
		//		int gy_min = (int)Math.Floor(ymin / gs);
		//		int gy_max = (int)Math.Floor(ymax / gs);

		//		for (int gx = gx_min; gx <= gx_max; gx++)
		//		{
		//			for (int gy = gy_min; gy <= gy_max; gy++)
		//			{
		//				var key = (gx, gy);
		//				if (!_SEG_GRID.ContainsKey(key))
		//					_SEG_GRID[key] = new List<(double, double, double, double, RectangleF)>();
		//				_SEG_GRID[key].Add(seg5);
		//			}
		//		}
		//	}
		//}

		/// <summary>
		/// Collect all visible linear segments in view and return a list of
		/// tuples (x1, y1, x2, y2) already projected into the view plane.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		/// TODO: Need to be updated to filter the source elements by targeted categories
//		private static List<(double x1, double y1, double x2, double y2)> ExtractLineSegments(Document doc, View view)
//		{
//			XYZ origin = view.Origin;
//			XYZ right = view.RightDirection;
//			XYZ up = view.UpDirection;

//			double short_tol = doc.Application.ShortCurveTolerance * 1.05;
//			double short_tol2 = short_tol * short_tol;	//	compare **squared** lengths

//			List<(double, double, double, double)> segs = new List<(double, double, double, double)>();
//			HashSet<string> seen = new HashSet<string>();

//			void ProcessCurve(Curve curve, Transform trf)
//			{
//				//	Tessellate *curve* (already world-transformed) and add segments
//				if (curve == null) return;

//				try
//				{
//					Curve curve_w = (trf != null && !trf.IsIdentity) ? curve.CreateTransformed(trf) : curve;
//					List<XYZ> pts = (List<XYZ>)curve_w.Tessellate();
//					for (int i = 0; i < pts.Count - 1; i++)
//					{
//						XYZ p1 = pts[i];
//						XYZ p2 = pts[i + 1];

//						//	--- NEW: skip segments shorter than ShortCurveTolerance ---
//						double dx = p2.X - p1.X;
//						double dy = p2.Y - p1.Y;
//						double dz = p2.Z - p1.Z;
//						if (dx * dx + dy * dy + dz * dz < short_tol2)
//							continue;

//						var (x1, y1) = ProjectToViewPlane(p1, origin, right, up);
//						var (x2, y2) = ProjectToViewPlane(p2, origin, right, up);

//						string key = $"{Math.Round(x1, 6)},{Math.Round(y1, 6)},{Math.Round(x2, 6)},{Math.Round(y2, 6)}";
//						string rev = $"{Math.Round(x2, 6)},{Math.Round(y2, 6)},{Math.Round(x1, 6)},{Math.Round(y1, 6)}";
//						if (seen.Contains(key) || seen.Contains(rev))
//							continue;
//						seen.Add(key);

//						segs.Add((x1, y1, x2, y2));
//					}
//				}
//				catch { }
//			}

//			void AddEdge(XYZ p1, XYZ p2, Transform trf)
//			{
//				if (p1.DistanceTo(p2) > short_tol)
//				{
//					try
//					{
//						var ln = Line.CreateBound(p1, p2);
//						ProcessCurve(ln, trf);
//					}
//					catch { }
//				}
//			}

//			// 1) direct CurveElement lines (model/detail lines)
//			foreach (CurveElement ce in new FilteredElementCollector(doc, view.Id)
//					.OfClass(typeof(CurveElement))
//					.WhereElementIsNotElementType())
//			{
//				ProcessCurve(ce.GeometryCurve, Transform.Identity);
//			}

//			// 2) geometry from all other visible elements
//			Options opts = new Options { View = view };
//			HashSet<BuiltInCategory> skip = new HashSet<BuiltInCategory>
//						{
//								BuiltInCategory.OST_Topography,
//								BuiltInCategory.OST_Planting,
//								BuiltInCategory.OST_Entourage,
//								BuiltInCategory.OST_Mass,
//								BuiltInCategory.OST_CurtainWallMullions,
//						};

//			var stack = new Stack<(GeometryElement, Transform)>();
//			foreach (Element el in new FilteredElementCollector(doc, view.Id).WhereElementIsNotElementType())
//			{
//#if REVIT2024 || REVIT2025
//				if (el.Category != null && skip.Contains((BuiltInCategory)el.Category.Id.Value))
//#else
//				if (el.Category != null && skip.Contains((BuiltInCategory)el.Category.Id.IntegerValue))
//#endif
//					continue;

//				if (el is CurveElement || el is IndependentTag)
//					continue;
//				var ge = el.get_Geometry(opts);
//				if (ge != null)
//					stack.Push((ge, Transform.Identity));
//			}

//			while (stack.Count > 0)
//			{
//				var (geomElem, trfAccum) = stack.Pop();
//				foreach (GeometryObject obj in geomElem)
//				{
//					if (obj is GeometryInstance gi)
//					{
//						Transform inst_trf = trfAccum.Multiply(gi.Transform);
//						stack.Push((gi.GetSymbolGeometry(), inst_trf));
//					}
//					else if (obj is Solid solid)
//					{
//						foreach (Edge e in solid.Edges)
//							ProcessCurve(e.AsCurve(), trfAccum);
//					}
//					else if (obj is Curve curveObj)
//					{
//						ProcessCurve(curveObj, trfAccum);
//					}
//				}
//			}

//			return segs;
//		}

		/// <summary>
		/// Return *True* if bbox *cand* intersects ANY visible line segment.
		/// 
		/// Uses the grid built in InitSegmentCache so we only check
		/// segments that live in the same (or adjacent) grid tiles.
		/// </summary>
		/// <param name="cand"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		//public static bool OverlapsModel(BoundingBoxXYZ cand, View view)
		//{
		//	if (_SEG_CACHE == null)
		//		InitSegmentCache(view.Document, view);

		//	XYZ origin = view.Origin;
		//	XYZ right = view.RightDirection;
		//	XYZ up = view.UpDirection;
		//	RectangleF rect = RectFromBboxCached(cand, origin, right, up);
		//	float xmin = rect.Left, ymin = rect.Top, xmax = rect.Right, ymax = rect.Bottom;

		//	double gs = _GRID_SIZE;
		//	int gx_min = (int)Math.Floor(xmin / gs), gx_max = (int)Math.Floor(xmax / gs);
		//	int gy_min = (int)Math.Floor(ymin / gs), gy_max = (int)Math.Floor(ymax / gs);

		//	var seen = new HashSet<(double, double, double, double, RectangleF)>();
		//	for (int gx = gx_min; gx <= gx_max; gx++)
		//	{
		//		for (int gy = gy_min; gy <= gy_max; gy++)
		//		{
		//			var key = (gx, gy);
		//			if (!_SEG_GRID.ContainsKey(key)) continue;
		//			foreach (var seg in _SEG_GRID[key])
		//			{
		//				if (seen.Contains(seg)) continue;
		//				seen.Add(seg);
		//				if (SegmentHitsRect(seg, rect))
		//					return true;
		//			}
		//		}
		//	}
		//	return false;
		//}

		//public static bool OverlapsAny(BoundingBoxXYZ cand, IEnumerable<BoundingBoxXYZ> boxes, View view)
		//{
		//	if (boxes == null)
		//		return false;
		//	// Check cand against a collection of bounding boxes in 2D.
		//	XYZ origin = view.Origin;
		//	XYZ right = view.RightDirection;
		//	XYZ up = view.UpDirection;
		//	RectangleF candRect = RectFromBboxCached(cand, origin, right, up);
		//	foreach (BoundingBoxXYZ bb in boxes)
		//	{
		//		if (candRect.IntersectsWith(RectFromBboxCached(bb, origin, right, up)))
		//			return true;
		//	}
		//	return false;
		//}

#if DEBUG
		//private static readonly Dictionary<ElementId, BoundingBoxXYZ> _Bbox_View_Cache = new Dictionary<ElementId, BoundingBoxXYZ>();
#endif

		/// <summary>
		/// Returns tag-center candidates on concentric arcs in the view-plane.
		/// </summary>
		/// <param name="bb"></param>
		/// <param name="rings"></param>
		/// <param name="spacing"></param>
		/// <param name="dirVec"></param>
		/// <param name="view"></param>
		/// <param name="directions"></param>
		/// <param name="otherHalfOfCircle"></param>
		/// <returns></returns>
		public static IEnumerable<XYZ> RadialPoints(
				BoundingBoxXYZ bb,
				int rings,
				double spacing,
				XYZ dirVec,
				View view,
				int directions = 16,
				bool otherHalfOfCircle = false)
		{
			// Only half-circle by default
			bool fullCircle = false;
			// Center of bounding box
			XYZ c = (bb.Min + bb.Max) / 2.0;

			// Get view axes
			var (rt, up) = ViewAxes(view);

			// Flatten direction vector to view plane
			XYZ d2 = FlattenToPlane(dirVec, view);
			if (d2.GetLength() < TOL)
				d2 = up; // default "sheet-up"
			double ax = d2.DotProduct(rt);
			double ay = d2.DotProduct(up);

			double ang0 = Math.Atan2(ay, ax);

			// If other_half_of_circle is True, we want points behind the direction vector
			if (otherHalfOfCircle)
				ang0 += Math.PI;	//	Rotate 180 degrees to get points behind

			double half = fullCircle ? Math.PI : Math.PI / 2.0;
			double stepAng = (2 * half) / (directions - 1);

			List<double> offsets = new List<double> { 0.0 };
			for (int k = 1; k < directions; k++)
			{
				int n = (k + 1) / 2;
				int s = (k % 2 == 1) ? 1 : -1;
				double off = s * n * stepAng;
				if (Math.Abs(off) <= half + TOL)
					offsets.Add(off);
			}

			for (int r = 1; r <= rings; r++)
			{
				double d = r * spacing;
				foreach (double off in offsets)
				{
					double ang = ang0 + off;
					XYZ vec = rt.Multiply(d * Math.Cos(ang)).Add(up.Multiply(d * Math.Sin(ang)));
					yield return new XYZ(c.X + vec.X, c.Y + vec.Y, c.Z + vec.Z);
				}
			}
		}

		/// <summary>
		/// Returns an XYZ unit vector that points where the tag *ought* to move for clarity, given the element's category and handedness.
		/// 
		/// • Exterior walls .......... inward   (keep façades clean)
		/// • Interior walls .......... whichever side has more room (fallback = up)
		/// • Doors ................... inward   (tags sit inside the room)
		/// • Windows ................. outward  (outside the glazing line)
		/// • Everything else ......... view-“up”
		/// </summary>
		/// <param name="el"></param>
		/// <returns></returns>
		public static XYZ LogicalOffsetDirection(Element el)
		{
#if REVIT2024 || REVIT2025
			long cat = el.Category.Id.Value;
#else
			int cat = el.Category.Id.IntegerValue;
#endif

			// --- walls ---
			if (el.Category.Name == "Walls")
			{
				XYZ norm = GetWallNormal((Wall)el);
				// flip once more so EXTERIOR walls push *into* the building
				if (IsWallExterior((Wall)el))
					norm = new XYZ(-norm.X, -norm.Y, 0.0);
				return norm;
			}

			// --- doors ---
			if (el.Category.Name == "Doors")
			{
				// FacingOrientation is a property on FamilyInstance
				FamilyInstance fi = el as FamilyInstance;
				if (fi != null)
				{
					XYZ norm = fi.FacingOrientation;
					// always tag to the INSIDE of the room
					if (norm != null)
						return new XYZ(-norm.X, -norm.Y, -norm.Z);
				}
			}

			// --- windows ---
			if (el.Category.Name == "Windows")
			{
				FamilyInstance fi = el as FamilyInstance;
				if (fi != null)
				{
					XYZ dirVec = fi.FacingOrientation;
					if (fi.FacingFlipped)
						dirVec = new XYZ(-dirVec.X, -dirVec.Y, -dirVec.Z);
					return dirVec;
				}
			}

			// --- everything else: straight “sheet-up” ---
			var (_, up) = ViewAxes(el.Document.ActiveView);
			return up;
		}

		/// <summary>
		/// Point-in-Polygon Check
		/// </summary>
		/// <param name="xyPoint"></param>
		/// <param name="polygonXY"></param>
		/// <returns></returns>
		public static bool IsPointInPolygon((double, double) xyPoint, List<(double, double)> polygonXY)
		{
			double x = xyPoint.Item1, y = xyPoint.Item2;
			bool inside = false;
			int n = polygonXY.Count, j = n - 1;
			for (int i = 0; i < n; i++)
			{
				var (xi, yi) = polygonXY[i];
				var (xj, yj) = polygonXY[j];
				if (((yi > y) != (yj > y)) &&
						(x < (xj - xi) * (y - yi) / ((yj - yi) + TOL) + xi))
					inside = !inside;
				j = i;
			}
			return inside;
		}

		public static bool IsPointInPolygon(XYZ pt, List<XYZ> polygon)
		{
			double x = pt.X, y = pt.Y;
			List<(double, double)> polygonXY = polygon.Select(x => (x.X, x.Y)).ToList();
			
			return IsPointInPolygon((x, y), polygonXY);
		}

		/// <summary>
		/// Returns a new XYZ moved by d along the given unit vector.
		/// </summary>
		/// <param name="p">The original point.</param>
		/// <param name="vUnit">The unit vector direction.</param>
		/// <param name="d">The distance to move.</param>
		/// <returns>The offset point.</returns>
		public static XYZ OffsetAlongPlane(XYZ p, XYZ vUnit, double d)
		{
			return new XYZ(
				p.X + vUnit.X * d,
				p.Y + vUnit.Y * d,
				p.Z + vUnit.Z * d
			);
		}

		#endregion

		#region Opening related functions
		/// <summary>
		/// Get Opening Reference
		/// </summary>
		/// <param name="opening"></param>
		/// <param name="nToken">0 => Left, 1 => Center, 2 => Right</param>
		/// <returns></returns>
		public static Reference GetOpeningReference(Element opening, int nToken)
		{
			string format = "{0}:{1}:{2}:{3}:{4}";
			ElementId uid = opening.Id;
			ElementId typeuid = opening.GetTypeId();
			string refString = string.Format(format, uid, "0", "INSTANCE", typeuid, nToken);

			try
			{
				return Reference.ParseFromStableRepresentation(opening.Document, refString);
			}
			catch
			{
				return null;
			}
		}
		#endregion

		#region BoundingBox related functions
		/// <summary>
		/// Returns a new BoundingBoxXYZ translated by vector v.
		/// </summary>
		/// <param name="bb">The original bounding box.</param>
		/// <param name="v">The translation vector.</param>
		/// <returns>The shifted bounding box.</returns>
		public static BoundingBoxXYZ ShiftBoundingBox(BoundingBoxXYZ bb, XYZ v)
		{
			BoundingBoxXYZ outBox = new BoundingBoxXYZ
			{
				Min = new XYZ(bb.Min.X + v.X, bb.Min.Y + v.Y, bb.Min.Z + v.Z),
				Max = new XYZ(bb.Max.X + v.X, bb.Max.Y + v.Y, bb.Max.Z + v.Z)
			};
			return outBox;
		}

		/// <summary>
		/// Inflates a bounding box by dx, dy (in view's Right/Up directions) and dz (in Z direction).
		/// 
		/// Grow *bb* by (dx, dy) along the view's Right / Up directions
		/// (and by dz along Z if you pass one - default 0).
		/// 
		/// Returns a world-aligned BoundingBoxXYZ that encloses the result,
		/// so the rest of the code can keep using simple axis-aligned boxes.
		/// </summary>
		/// <param name="bb">The original bounding box.</param>
		/// <param name="dx">Amount to grow along view's right direction.</param>
		/// <param name="dy">Amount to grow along view's up direction.</param>
		/// <param name="view">The view for direction context.</param>
		/// <param name="dz">Amount to grow along Z direction (optional, default 0.0).</param>
		/// <returns>The inflated bounding box.</returns>
		public static BoundingBoxXYZ InflateBoundingBox(BoundingBoxXYZ bb, double dx, double dy, View view, double dz = 0.0)
		{
			var (rt, up) = ViewAxes(view);
			XYZ d1 = rt.Multiply(dx);
			XYZ d2 = up.Multiply(dy);
			XYZ d3 = view.ViewDirection.Normalize().Multiply(dz);

			XYZ minpt = bb.Min;
			XYZ maxpt = bb.Max;
			//	eight original corners
			List<XYZ> corners = new List<XYZ>();
			foreach (double x in new[] { minpt.X, maxpt.X })
				foreach (double y in new[] { minpt.Y, maxpt.Y })
					foreach (double z in new[] { minpt.Z, maxpt.Z })
						corners.Add(new XYZ(x, y, z));

			//	displaced by every ±dx and ±dy combo inside the view plane
			List<XYZ> inflated = new List<XYZ>();
			foreach (var c in corners)
			{
				foreach (int sx in new[] { -1, 1 })
					foreach (int sy in new[] { -1, 1 })
						inflated.Add(c.Add(d1.Multiply(sx)).Add(d2.Multiply(sy)).Add(d3));
			}

			double xmin = inflated.Min(p => p.X);
			double xmax = inflated.Max(p => p.X);
			double ymin = inflated.Min(p => p.Y);
			double ymax = inflated.Max(p => p.Y);
			double zmin = inflated.Min(p => p.Z);
			double zmax = inflated.Max(p => p.Z);

			var outBox = new BoundingBoxXYZ
			{
				Min = new XYZ(xmin, ymin, zmin),
				Max = new XYZ(xmax, ymax, zmax)
			};
			return outBox;
		}

		/// <summary>
		/// Checks if two bounding boxes intersect.
		/// </summary>
		public static bool BoundingBoxesIntersect(BoundingBoxXYZ bbox1, BoundingBoxXYZ bbox2)
		{
			return !(bbox1.Max.X < bbox2.Min.X || bbox1.Min.X > bbox2.Max.X ||
			bbox1.Max.Y < bbox2.Min.Y || bbox1.Min.Y > bbox2.Max.Y ||
			bbox1.Max.Z < bbox2.Min.Z || bbox1.Min.Z > bbox2.Max.Z);
		}

		// --- BoundingBoxXYZ world-aligned cache and helpers ---
		private static readonly Dictionary<ElementId, BoundingBoxXYZ> _bboxCache = new Dictionary<ElementId, BoundingBoxXYZ>();

		/// <summary>
		/// Returns the element's world-aligned bounding box or null if not available.
		/// Caches the result for performance.
		/// </summary>
		public static BoundingBoxXYZ SafeWorldBoundingBox(Element el, View view)
		{
			if (el == null || view == null) return null;
			//if (_bboxCache.TryGetValue(el.Id, out BoundingBoxXYZ bb))
			//	return bb;
			BoundingBoxXYZ bb;
			try
			{
				var bb0 = el.get_BoundingBox(view);
				if (bb0 != null)
				{
					bb = WorldBoundingBox(bb0);
					_bboxCache[el.Id] = bb;
					return bb;
				}
			}
			catch
			{
				// zombie tag or invalid element
			}
			return null;
		}

		/// <summary>
		/// Returns a new BoundingBoxXYZ whose Min/Max are in world space.
		/// </summary>
		public static BoundingBoxXYZ WorldBoundingBox(BoundingBoxXYZ bb)
		{
			if (bb == null) return null;
			var tf = bb.Transform;
			if (tf != null && !tf.IsIdentity)
			{
				var pts = new List<XYZ>
										{
											tf.OfPoint(bb.Min),
											tf.OfPoint(new XYZ(bb.Min.X, bb.Min.Y, bb.Max.Z)),
											tf.OfPoint(new XYZ(bb.Min.X, bb.Max.Y, bb.Min.Z)),
											tf.OfPoint(new XYZ(bb.Min.X, bb.Max.Y, bb.Max.Z)),
											tf.OfPoint(new XYZ(bb.Max.X, bb.Min.Y, bb.Min.Z)),
											tf.OfPoint(new XYZ(bb.Max.X, bb.Min.Y, bb.Max.Z)),
											tf.OfPoint(new XYZ(bb.Max.X, bb.Max.Y, bb.Min.Z)),
											tf.OfPoint(bb.Max)
										};
				double xmin = pts.Min(p => p.X);
				double xmax = pts.Max(p => p.X);
				double ymin = pts.Min(p => p.Y);
				double ymax = pts.Max(p => p.Y);
				double zmin = pts.Min(p => p.Z);
				double zmax = pts.Max(p => p.Z);
				return new BoundingBoxXYZ
				{
					Min = new XYZ(xmin, ymin, zmin),
					Max = new XYZ(xmax, ymax, zmax)
				};
			}
			return bb; // already world-aligned
		}

		/// <summary>
		/// Returns the lower-left corner of the active crop box in world coordinates,
		/// or the view.Origin if no crop box is active.
		/// </summary>
		public static ((double X, double Y), (double Width, double Height)) GetCropView(ViewPlan view)
		{
			//	TODO : Fix
			RectangleF cand = new RectangleF();

			if (!view.CropBoxActive)
			{
				BoundingBoxXYZ bbox = view.get_BoundingBox(view);
				//cand = RectFromBboxCached(bbox, view.Origin, view.RightDirection, view.UpDirection);
			}
			else
			{
				BoundingBoxXYZ cropBox = view.CropBox;
				//cand = RectFromBboxCached(cropBox, view.Origin, view.RightDirection, view.UpDirection);
			}

			return ((cand.X, cand.Y), (cand.Width, cand.Height));
		}

		/// <summary>
		/// Return all the wall 2d informations which intersect with a ray given by an origin and a direction
		/// </summary>
		/// <param name="wall2dList">sources to be detected</param>
		/// <param name="dir">direction of wall to be compared</param>
		/// <param name="origin">Ray's origin</param>
		/// <param name="ray">Ray's direction</param>
		/// <returns></returns>
		public static List<Element2dInfo> GetWallsByRay(List<Element2dInfo> wall2dList, XYZ dir, XYZ origin, XYZ ray)
		{
			List<Element2dInfo> res = new List<Element2dInfo>();

			var hits = new List<ElementId>();
			hits.AddRange(Obstructions(wall2dList, origin, ray));
			hits.AddRange(Obstructions(wall2dList, origin, ray.Negate()));

			// Filter by hits and dir
			foreach (var wall in wall2dList)
			{
				if(hits.Contains(wall.Id) && (dir.IsAlmostEqualTo(wall.Direction) || dir.IsAlmostEqualTo(-wall.Direction)))
				{
					res.Add(wall);
				}
			}

			return res;
		}

		/// <summary>
		/// Return all the wall 2d informations which intersect with a ray given by an origin and a direction
		/// </summary>
		/// <param name="element2dList">sources to be detected</param>
		/// <param name="dir">direction of wall to be compared</param>
		/// <param name="origin">Ray's origin</param>
		/// <param name="ray">Ray's direction</param>
		/// <returns></returns>
		public static List<Element2dInfo> GetElementsByRay(List<Element2dInfo> element2dList, XYZ origin, XYZ ray)
		{
			List<Element2dInfo> res = new List<Element2dInfo>();

			var hits = new List<ElementId>();
			hits.AddRange(Obstructions(element2dList, origin, ray));
			hits.AddRange(Obstructions(element2dList, origin, ray.Negate()));

			// Filter by hits and dir
			foreach (var element in element2dList)
			{
				if (hits.Contains(element.Id))
				{
					res.Add(element);
				}
			}

			return res;
		}

		/// <summary>
		/// Return all the obstructions which intersect with a ray given by an origin and a direction
		/// </summary>
		/// <param name="walls">sources to be detected</param>
		/// <param name="origin">Ray's origin</param>
		/// <param name="dir">Ray's direction</param>
		/// <returns>Obstructions intersected with the given ray</returns>
		private static List<ElementId> Obstructions(List<Element2dInfo> walls, XYZ origin, XYZ dir)
		{
			var hits = new List<ElementId>();

			// Ray origin and direction in plan-space
			double x0 = origin.X;
			double y0 = origin.Y;
			double dx = dir.X;
			double dy = dir.Y;

			foreach(var w in walls)
			{
				// Rectangle extents
				double uMin = w.Min.U, uMax = w.Max.U;
				double vMin = w.Min.V, vMax = w.Max.V;

				// Compute the t-interval for X slabs
				double tminX, tmaxX;
				if(Math.Abs(dx) < TOL)
				{
					// Ray parallel to X: must lie within [uMin, uMax]
					if (x0 < uMin || x0 > uMax)
						continue;
					tminX = double.NegativeInfinity;
					tmaxX = double.PositiveInfinity;
				}
				else
				{
					double tx1 = (uMin - x0) / dx;
					double tx2 = (uMax - x0) / dx;

					tminX = Math.Min(tx1, tx2);
					tmaxX = Math.Max(tx1, tx2);
				}

				// Compute the t-interval for Y slabs
				double tminY, tmaxY;
				if( Math.Abs(dy) < TOL)
				{
					// Ray parallel to Y: must lie within [vMin, vMax]
					if (y0 < vMin || y0 > vMax)
						continue;
					tminY = double.NegativeInfinity;
					tmaxY = double.PositiveInfinity;
				}
				else
				{
					double ty1 = (vMin - y0) / dy;
					double ty2 = (vMax - y0) / dy;
					tminY = Math.Min(ty1, ty2);
					tmaxY = Math.Max(ty1, ty2);
				}

				// Find the intersection of the two intervals
				double tEnter = Math.Max(tminX, tminY);
				double tExit = Math.Min(tmaxX, tmaxY);

				// If there is overlap and it occurs at or after the ray origin
				// we have a hit
				if (tExit >= Math.Max(tEnter, 0.0))
				{
					hits.Add(w.Id);
				}
			}

			return hits;
		}

		/// <summary>
		/// Returns (xmin, ymin, xmax, ymax) of BoundingBoxXYZ in view coordinates.
		/// </summary>
		public static (double xmin, double ymin, double xmax, double ymax) RectFromBbox(BoundingBoxXYZ bb, XYZ origin, XYZ right, XYZ up)
		{
			var tf = bb.Transform ?? Transform.Identity;
			var minpt = bb.Min;
			var maxpt = bb.Max;
			var corners = new List<XYZ>
									{
										new XYZ(minpt.X, minpt.Y, minpt.Z),
										new XYZ(minpt.X, minpt.Y, maxpt.Z),
										new XYZ(minpt.X, maxpt.Y, minpt.Z),
										new XYZ(minpt.X, maxpt.Y, maxpt.Z),
										new XYZ(maxpt.X, minpt.Y, minpt.Z),
										new XYZ(maxpt.X, minpt.Y, maxpt.Z),
										new XYZ(maxpt.X, maxpt.Y, minpt.Z),
										new XYZ(maxpt.X, maxpt.Y, maxpt.Z)
									};
			var xs = new List<double>();
			var ys = new List<double>();
			foreach (var c in corners)
			{
				var pt = tf.OfPoint(c);
				var (x, y) = ProjectToViewPlane(pt, origin, right, up);
				xs.Add(x);
				ys.Add(y);
			}
			return (xs.Min(), ys.Min(), xs.Max(), ys.Max());
		}
		#endregion

		#region Room related functions
		/// <summary>
		/// Finds the room at the given point, or null if none.
		/// </summary>
		/// <param name="doc">The Revit document.</param>
		/// <param name="point">The point to check.</param>
		/// <returns>The room at the point, or null.</returns>
		public static Element FindRoomAtPoint(Document doc, XYZ point)
		{
			List<Element> rooms = (List<Element>)new FilteredElementCollector(doc)
				.OfCategory(BuiltInCategory.OST_Rooms)
				.WhereElementIsNotElementType()
				.ToElements();
			foreach (Element room in rooms)
			{
				// Room.IsPointInRoom is only available on Autodesk.Revit.DB.Architecture.Room
				if (room is Autodesk.Revit.DB.Architecture.Room r)
				{
					if (r.IsPointInRoom(point))
						return r;
				}
			}
			return null;
		}

		public static List<Curve> GetBoundaryCurves(SpatialElement elem)
		{
			Document doc = elem.Document;
			List<Curve> curves = new List<Curve>();

			SpatialElementBoundaryOptions options = new SpatialElementBoundaryOptions()
			{
				SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish
			};

			var boundarySegments = elem.GetBoundarySegments(options);
			if (boundarySegments == null)
				return curves;

			foreach(var segmentList in elem.GetBoundarySegments(options))
			{
				foreach(BoundarySegment segment in segmentList)
				{
					curves.Add(segment.GetCurve());
				}
			}

			return curves;
		}
		#endregion

		#region Point related
		/// <summary>
		/// Projects a 3D point to the 2D view plane using the view's origin, right, and up vectors.
		/// </summary>
		/// <param name="pt"></param>
		/// <param name="origin"></param>
		/// <param name="right"></param>
		/// <param name="up"></param>
		/// <returns></returns>
		private static (double, double) ProjectToViewPlane(XYZ pt, XYZ origin, XYZ right, XYZ up)
		{
			double x = (pt - origin).DotProduct(right);
			double y = (pt - origin).DotProduct(up);
			return (x, y);
		}

		/// <summary>
		/// Component of vector *v* that lies in the view-plane.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public static XYZ FlattenToPlane(XYZ v, View view)
		{
			XYZ n = view.ViewDirection.Normalize();
			n = view.ViewDirection;
			n.Normalize();

			return v.Subtract(n.Multiply(v.DotProduct(n)));
		}

		/// <summary>
		/// Unit Right / Up directions that work in every version
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public static (XYZ, XYZ) ViewAxes(View view)
		{
			return (view.RightDirection.Normalize(), view.UpDirection.Normalize());
		}
		#endregion

		#region Reference related functions
		// Replace the relevant part of GetPointFromReference to handle Opening with nToken (Left, Center, Right)
		public static XYZ GetPointFromReference(Document doc, Reference reference)
		{
			Element elem = doc.GetElement(reference.ElementId);
			GeometryObject geoObj = elem.GetGeometryObjectFromReference(reference);

			if (geoObj == null)
			{
				if (elem is FamilyInstance fi)
				{
					// Special handling for Opening: try to parse nToken from stable representation
					if (new List<BuiltInCategory> {
						BuiltInCategory.OST_Windows
						, BuiltInCategory.OST_Doors
					}.Contains(fi.Category.BuiltInCategory))
					{
						// Try to parse nToken from the stable representation
						string stable = reference.ConvertToStableRepresentation(doc);
						// Format: <elementId>:0:INSTANCE:<typeId>:<nToken>
						var parts = stable.Split(':');
						if (parts.Length >= 5 && int.TryParse(parts[4], out int nToken))
						{
							// Get the opening's location point
							if (elem.Location is LocationPoint lp1 && lp1.Point != null)
							{
								XYZ basePoint = lp1.Point;
								// Try to get the width vector if possible (for left/right)
								Parameter widthParam = fi.LookupParameter("Width");
								if(widthParam == null)
								{
									Element type = doc.GetElement(fi.GetTypeId());
									widthParam = type.LookupParameter("Width");
								}
								double width = widthParam != null ? widthParam.AsDouble() : 10.0;
								XYZ facing = fi.FacingOrientation.CrossProduct(XYZ.BasisZ);

								// Default: center
								XYZ pt = basePoint;
								if (width > 0 && facing != null)
								{
									XYZ dir = facing.Normalize();
									switch (nToken)
									{
										case 0: // Left
											pt = basePoint.Subtract(dir.Multiply(width / 2.0));
											break;
										case 2: // Right
											pt = basePoint.Add(dir.Multiply(width / 2.0));
											break;
										default: // Center
											pt = basePoint;
											break;
									}
								}
								return pt;
							}
						}
					}

					if (fi.Location is LocationPoint lp)
						return lp.Point;
					else if (fi.Location is LocationCurve lc)
						return lc.Curve.Evaluate(0.5, true);
					else
						return XYZ.Zero;
				}
				else if (elem is Grid)
				{
					return (elem as Grid).Curve.Evaluate(0.5, true);
				}
				else
					return XYZ.Zero;
			}

			//	If the reference is to a face, return the centroid of the face
			if (geoObj is Face face1)
			{
				BoundingBoxUV bb = face1.GetBoundingBox();
				UV mid = (bb.Min + bb.Max) / 2;
				return face1.Evaluate(mid);
			}

			//	If the reference is to an edge, return the midpoint of the edge
			if (geoObj is Edge edge1)
			{
				return edge1.Evaluate(0.5);
			}

			//	If the reference is to a curve (line, arc, etc.)
			if (geoObj is Curve curveObj)
			{
				return curveObj.Evaluate(0.5, true); //	true = normalized parameter
			}

			//	If the geometry is a geometry instance, use its origin as fallback
			if (geoObj is GeometryInstance gi)
			{
				return gi.Transform.Origin;
			}

			if (geoObj is GeometryObject)
			{
				Options opt = new Options
				{
					ComputeReferences = true,
					IncludeNonVisibleObjects = true
				};

				GeometryElement geoElem = elem.get_Geometry(opt);

				foreach (GeometryObject gObj in geoElem)
				{
					if (gObj is Solid solid)
					{
						foreach (Face face in solid.Faces)
						{
							if (face.Reference != null &&
									face.Reference.ElementId == reference.ElementId &&
									face.Reference.ConvertToStableRepresentation(doc) == reference.ConvertToStableRepresentation(doc))
							{
								BoundingBoxUV bb = face.GetBoundingBox();
								UV mid = (bb.Min + bb.Max) / 2;
								return face.Evaluate(mid);
							}
						}

						foreach (Edge edge in solid.Edges)
						{
							if (edge.Reference != null &&
									edge.Reference.ElementId == reference.ElementId &&
									edge.Reference.ConvertToStableRepresentation(doc) == reference.ConvertToStableRepresentation(doc))
							{
								return edge.Evaluate(0.5);
							}
						}
					}
				}
			}

			return XYZ.Zero;
		}

		/// <summary>
		/// Sorts references by their projection along the given direction vector.
		/// </summary>
		/// <param name="references">List of references to sort.</param>
		/// <param name="direction">Direction vector for projection.</param>
		/// <returns>Sorted list of references.</returns>
		public static List<Reference> SortReferencesByDirection(Document doc, List<Reference> references, XYZ direction)
		{
			return references
					.OrderBy(r => GetPointFromReference(doc, r).DotProduct(direction))
					.ToList();
		}

		/// <summary>
		/// Get Reference Direction
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="ref1"></param>
		/// <returns> the direction perpendicular to reference</returns>
		/// <returns>XYZ.Zero on error</returns>
		public static XYZ GetReferenceDirection(Document doc, Reference ref1)
		{
			XYZ res = XYZ.Zero;
			if(ref1.ElementId == ElementId.InvalidElementId)
				return res;

			Element element = doc.GetElement(ref1.ElementId);

			if(ref1.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_SURFACE ||
				ref1.ElementReferenceType == ElementReferenceType.REFERENCE_TYPE_LINEAR)
			{
				GeometryObject geoObj = element.GetGeometryObjectFromReference(ref1);
				if (geoObj is Face face)
				{
					res = face.ComputeNormal(ref1.UVPoint);
				}
				else if (geoObj is Edge edge)
				{
					res = edge.Evaluate(0.5);
				}
			}

			return res;
		}
		#endregion

		//	Etc
		// --- NEW: always-safe normalisation (Revit 2022-2025) ------------------------
		public static XYZ Unit(XYZ v)
		{
#if REVIT2025
			v.Normalize();
			return v;
#else
			return v.Normalize();
#endif
		}

		public enum UnitOption
		{
			Millimeter,
			Inch,
			Feet
		}

		/// <summary>
		/// Conver the papersize value to the real size
		/// </summary>
		/// <param name="view"></param>
		/// <param name="sizeInSheet"></param>
		/// <param name="unit"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static double Paper2RealSizeInView(View view, double sizeInSheet, UnitOption unit)
		{
			if (view == null)
				throw new ArgumentNullException(nameof(view));

			double scale = view.Scale;
			double realSize;

			switch (unit)
			{
				case UnitOption.Millimeter:
					// 1 mm = 0.00328084 feet
					realSize = sizeInSheet * scale * 0.00328084;
					break;
				case UnitOption.Inch:
					// 1 inch = 1/12 feet
					realSize = sizeInSheet * scale / 12.0;
					break;
				default:
					// Assume input is already in feet
					realSize = sizeInSheet * scale;
					break;
			}

			return realSize;
		}

		/// <summary>
		/// Returns the (width, height) of the bounding box projected into the view plane.
		/// </summary>
		/// <param name="bb">The bounding box.</param>
		/// <param name="view">The view.</param>
		/// <returns>Tuple of (width, height) in view plane.</returns>
		public static (double width, double height) ViewAlignedSize(BoundingBoxXYZ bb, View view)
		{
			var origin = view.Origin;
			var rt = view.RightDirection;
			var up = view.UpDirection;
			var corners = new List<XYZ>
			{
				bb.Min,
				new XYZ(bb.Min.X, bb.Min.Y, bb.Max.Z),
				new XYZ(bb.Min.X, bb.Max.Y, bb.Min.Z),
				new XYZ(bb.Min.X, bb.Max.Y, bb.Max.Z),
				new XYZ(bb.Max.X, bb.Min.Y, bb.Min.Z),
				new XYZ(bb.Max.X, bb.Min.Y, bb.Max.Z),
				new XYZ(bb.Max.X, bb.Max.Y, bb.Min.Z),
				bb.Max
			};
			var xs = new List<double>();
			var ys = new List<double>();
			foreach (var corner in corners)
			{
				var (x, y) = ProjectToViewPlane(corner, origin, rt, up);
				xs.Add(x);
				ys.Add(y);
			}
			double width = Math.Abs(xs.Max() - xs.Min());
			double height = Math.Abs(ys.Max() - ys.Min());
			return (width, height);
		}

		/// <summary>
		/// Crop Boundary Polygon List Dictionary for caching them per view (Key: View Id)
		/// </summary>
		private static Dictionary<ElementId, List<List<(double, double)>>> _VIEWCROP_CACHE = new Dictionary<ElementId, List<List<(double, double)>>>();

		/// <summary>
		/// Get ViewCropRegion Boundary Polygon List
		/// The CropRegion Boundary can be multi-poly
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		public static List<List<(double, double)>> ViewCropRegionBoundary2D(View view)
		{
			if (_VIEWCROP_CACHE.ContainsKey(view.Id))
			{
				return _VIEWCROP_CACHE[view.Id];
			}

			List<List<(double, double)>> cropRegions = new List<List<(double, double)>>();
			if (!view.CropBoxActive)
				return cropRegions;

			// Get the crop region shape manager
			ViewCropRegionShapeManager cropShape = view.GetCropRegionShapeManager();
			if (cropShape == null)
				return cropRegions;

			// Crop region can have multiple loops
			IList<CurveLoop> curveLoops = cropShape.GetCropShape();
			if (curveLoops == null || curveLoops.Count == 0)
				return cropRegions;

			foreach (var curveLoop in curveLoops)
			{
				List<(double, double)> poly = new List<(double, double)>();
				foreach (Curve curve in curveLoop)
				{
					poly.Add((curve.GetEndPoint(0).X, curve.GetEndPoint(0).Y));
				}
				if (poly.Count > 2)
				{
					cropRegions.Add(poly);
				}
			}

			_VIEWCROP_CACHE.Add(view.Id, cropRegions);
			return cropRegions;
		}



		/// <summary>
		/// Computes the intersection point(s) between two Revit curves (in 3D).
		/// Returns a list of intersection points (empty if none).
		/// </summary>
		/// <param name="curve1">First curve.</param>
		/// <param name="curve2">Second curve.</param>
		/// <returns>List of intersection points (XYZ).</returns>
		public static List<XYZ> GetCurveIntersections(Curve curve1, Curve curve2)
		{
			var result = new List<XYZ>();
			if (curve1 == null || curve2 == null)
				return result;

			IntersectionResultArray ira;
			SetComparisonResult res = curve1.Intersect(curve2, out ira);

			if (res == SetComparisonResult.Overlap && ira != null)
			{
				foreach (IntersectionResult ir in ira)
				{
					if (ir.XYZPoint != null)
						result.Add(ir.XYZPoint);
				}
			}
			return result;
		}

		public static List<Curve> GetInboundCurvesInView(List<Curve> curves, View view)
		{
			if (curves == null || curves.Count == 0 || view == null)
				return new List<Curve>();

			// Get the crop region boundary polygons (in 2D)
			var cropRegions = ViewCropRegionBoundary2D(view);
			if (cropRegions == null || cropRegions.Count == 0)
				return curves; // no crop, return all

			var result = new List<Curve>();

			foreach (var curve in curves)
			{
				// Tessellate the curve for segment-wise processing
				var tess = curve.Tessellate();
				if (tess.Count < 2)
					continue;

				List<XYZ> insidePoints = new List<XYZ>();

				// For each segment, check if both endpoints are inside crop
				for (int i = 0; i < tess.Count - 1; i++)
				{
					XYZ p0 = tess[i];
					XYZ p1 = tess[i + 1];

					// Project to 2D
					var p0_2d = (p0.X, p0.Y);
					var p1_2d = (p1.X, p1.Y);

					bool p0In = cropRegions.Any(poly => IsPointInPolygon(p0_2d, poly));
					bool p1In = cropRegions.Any(poly => IsPointInPolygon(p1_2d, poly));

					if (p0In && p1In)
					{
						// Both inside, keep this segment
						result.Add(Line.CreateBound(p0, p1));
					}
					else if (p0In != p1In)
					{
						// One in, one out: find intersection with crop boundary
						foreach (var poly in cropRegions)
						{
							// For each edge of the polygon
							for (int j = 0; j < poly.Count; j++)
							{
								var a = poly[j];
								var b = poly[(j + 1) % poly.Count];

								// Check intersection between (p0_2d, p1_2d) and (a, b)
								if (TryGet2DSegmentIntersection(p0_2d, p1_2d, a, b, out var inter))
								{
									XYZ inter3d = new XYZ(inter.Item1, inter.Item2, p0.Z + (p1.Z - p0.Z) * (Distance2DTuple(p0_2d, inter) / Distance2DTuple(p0_2d, p1_2d)));
									if (p0In)
										result.Add(Line.CreateBound(p0, inter3d));
									else
										result.Add(Line.CreateBound(inter3d, p1));
									break;
								}
							}
						}
					}
					// else: both out, skip
				}
			}

			return result;

			// --- Helper: 2D segment intersection ---
			bool TryGet2DSegmentIntersection((double, double) p1, (double, double) p2, (double, double) q1, (double, double) q2, out (double, double) intersection)
			{
				intersection = (0, 0);
				double x1 = p1.Item1, y1 = p1.Item2, x2 = p2.Item1, y2 = p2.Item2;
				double x3 = q1.Item1, y3 = q1.Item2, x4 = q2.Item1, y4 = q2.Item2;

				double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
				if (Math.Abs(denom) < TOL)
					return false; // parallel

				double px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denom;
				double py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denom;

				// Check if intersection is within both segments
				if (IsBetween(px, x1, x2) && IsBetween(py, y1, y2) && IsBetween(px, x3, x4) && IsBetween(py, y3, y4))
				{
					intersection = (px, py);
					return true;
				}
				return false;
			}

			bool IsBetween(double val, double a, double b)
			{
				return (val >= Math.Min(a, b) - TOL) && (val <= Math.Max(a, b) + TOL);
			}
		}

		/// <summary>
		/// Returns a list of Curve objects (or Line) that are inside the view's crop region.
		//	Only segments fully or partially inside the crop region are returned, clipped as needed.
		/// </summary>
		/// <param name="curves"></param>
		/// <param name="view"></param>
		/// <returns></returns>
		public static List<Curve> GetInboundCurvesInViewLight(List<Curve> curves, View view)
		{
			if (curves == null || curves.Count == 0 || view == null)
				return new List<Curve>();

			// Get the crop region boundary polygons (in 2D)
			var cropRegions = ViewCropRegionBoundary2D(view);
			if (cropRegions == null || cropRegions.Count == 0)
				return curves; // no crop, return all

			var result = new List<Curve>();

			foreach (var curve in curves)
			{
				if (!curve.IsBound)
					continue;

				XYZ p0 = curve.GetEndPoint(0);
				XYZ p1 = curve.GetEndPoint(1);

				// Ignore Z: project to 2D
				var p0_2d = (p0.X, p0.Y);
				var p1_2d = (p1.X, p1.Y);

				bool p0In = cropRegions.Any(poly => IsPointInPolygon(p0_2d, poly));
				bool p1In = cropRegions.Any(poly => IsPointInPolygon(p1_2d, poly));

				if (p0In && p1In)
				{
					// Both inside, keep this segment
					result.Add(Line.CreateBound(p0, p1));
				}
				else if (p0In != p1In)
				{
					// One in, one out: find intersection with crop boundary
					foreach (var poly in cropRegions)
					{
						for (int j = 0; j < poly.Count; j++)
						{
							var a = poly[j];
							var b = poly[(j + 1) % poly.Count];

							if (TryGet2DSegmentIntersection(p0_2d, p1_2d, a, b, out var inter))
							{
								// Ignore Z: use p0.Z for both endpoints
								XYZ inter3d = new XYZ(inter.Item1, inter.Item2, p0.Z);

								if (p0In)
									result.Add(Line.CreateBound(p0, inter3d));
								else
									result.Add(Line.CreateBound(inter3d, p1));
								break;
							}
						}
					}
				}
				// else: both out, skip
			}

			return result;

			// --- Helper: 2D segment intersection ---
			bool TryGet2DSegmentIntersection((double, double) p1, (double, double) p2, (double, double) q1, (double, double) q2, out (double, double) intersection)
			{
				intersection = (0, 0);
				double x1 = p1.Item1, y1 = p1.Item2, x2 = p2.Item1, y2 = p2.Item2;
				double x3 = q1.Item1, y3 = q1.Item2, x4 = q2.Item1, y4 = q2.Item2;

				double denom = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
				if (Math.Abs(denom) < TOL)
					return false; // parallel

				double px = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denom;
				double py = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denom;

				// Check if intersection is within both segments
				if (IsBetween(px, x1, x2) && IsBetween(py, y1, y2) && IsBetween(px, x3, x4) && IsBetween(py, y3, y4))
				{
					intersection = (px, py);
					return true;
				}
				return false;
			}

			bool IsBetween(double val, double a, double b)
			{
				return (val >= Math.Min(a, b) - TOL) && (val <= Math.Max(a, b) + TOL);
			}
		}

		#region VISUAL_DEBUG

		public static void DebugViewPolygonVisualization(Document doc, View view, string lineStyle = "Thin Lines")
		{
			if (view == null) return;
			List<List<(double, double)>> multipoly = null;
			if (_VIEWCROP_CACHE.ContainsKey(view.Id))
			{
				multipoly = _VIEWCROP_CACHE[view.Id];
			}
			if (multipoly == null)
			{
				multipoly = ViewCropRegionBoundary2D(view);
			}

			foreach (var poly in multipoly)
			{
				DebugDrawPolyInView2D(doc, view, poly, lineStyle);
			}
		}

		public static void DebugBoundingBoxVisualization(Document doc, View view, BoundingBoxXYZ bbox, string lineStyle = "Wide Lines")
		{
			var p1 = (bbox.Min.X, bbox.Min.Y);
			var p2 = (bbox.Max.X, bbox.Min.Y);
			var p3 = (bbox.Max.X, bbox.Max.Y);
			var p4 = (bbox.Min.X, bbox.Max.Y);
			DebugDrawPolyInView2D(doc, view, new List<(double, double)> { p1, p2, p3, p4 }, lineStyle);
		}

		private static void DebugDrawPolyInView2D(Document doc, View view, List<(double, double)> polygon2D, string styleName)
		{
			if (polygon2D == null)
				return;

			int nCount = polygon2D.Count;
			if (nCount < 2)
				return;

			// Find or create the line style
			Category lineCategory = doc.Settings.Categories.get_Item(BuiltInCategory.OST_Lines);
			styleName = "<" + styleName + ">";
			if (!lineCategory.SubCategories.Contains(styleName))
			{
				return;
			}

			var lineStyle = lineCategory.SubCategories.get_Item(styleName);
			var graphic = lineStyle.GetGraphicsStyle(GraphicsStyleType.Projection);

			for (int i = 0; i < nCount; i++)
			{
				XYZ p1 = new XYZ(polygon2D[i].Item1, polygon2D[i].Item2, 0);
				XYZ p2 = new XYZ(polygon2D[(i + 1) % nCount].Item1, polygon2D[(i + 1) % nCount].Item2, 0);

				Line line = Line.CreateBound(p1, p2);
				DetailCurve curve = doc.Create.NewDetailCurve(view, line);
				curve.LineStyle = graphic;
			}
		}

		/// <summary>
		/// Adjusts the given vector direction based on the angle between it and a vector from the origin to a target point.
		/// If the angle is greater than 90 degrees, the reversed vector is returned.
		/// </summary>
		/// <param name="origin">The center of the rectangular area.</param>
		/// <param name="targetPoint">The reference point to form a vector from the origin.</param>
		/// <param name="direction">The original direction vector to compare.</param>
		/// <returns>The adjusted vector direction.</returns>
		public static XYZ AdjustVectorDirection(XYZ origin, XYZ targetPoint, XYZ direction)
		{
			if (origin == null || targetPoint == null || direction == null)
				throw new ArgumentNullException("Input vectors must not be null.");

			XYZ toTarget = (targetPoint - origin).Normalize();
			XYZ dirNorm = direction.Normalize();

			double angle = toTarget.AngleTo(dirNorm);

			// If angle is greater than 90 degrees, reverse the direction
			return angle > Math.PI / 2 ? -dirNorm : dirNorm;
		}

		/// <summary>
		/// Get references from element
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static List<Reference> GetReferencesFromElement(Element element)
		{
			var solid = GetSolidFromElement(element);

			List<Reference> references = new List<Reference>();

			if (solid == null) return references;

			foreach(Face face in solid.Faces)
			{
				if(face.Reference != null)
				{
					references.Add(face.Reference);
				}
			}

			return references;
		}

		/// <summary>
		/// Get solid from eleemnt
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public static Solid GetSolidFromElement(Element element)
		{
			if (element == null)
				return null;

			// Options for retrieving geometry
			Options options = new Options
			{
				ComputeReferences = true,
				IncludeNonVisibleObjects = true
			};

			// Get the geometry of the FamilyInstance
			GeometryElement geometryElement = element.get_Geometry(options);
			if (geometryElement == null)
				return null;

			foreach (GeometryObject geometryObject in geometryElement)
			{
				if (geometryObject is Solid solid && solid.Volume > 0)
				{
					// Return the solid if found
					return solid;
				}
				else if (geometryObject is GeometryInstance geometryInstance)
				{
					// Get the instance geometry
					GeometryElement instanceGeometry = geometryInstance.GetInstanceGeometry();

					foreach (GeometryObject instanceObject in instanceGeometry)
					{
						if (instanceObject is Solid instanceSolid && instanceSolid.Volume > 0)
						{
							return instanceSolid;
						}
					}
				}
			}

			// No valid solid found
			return null;
		}
		#endregion

		public static List<APIObject> GetElementGeometryOnView(Element element, View view)
		{
			Document doc = element.Document;
			
			if(element is IndependentTag tag)
			{
				return GetIndependentTagGeometryOnView(tag, view);
			}
			else if (element is RoomTag roomTag)
			{
				return GetRoomTagGeometryOnView(roomTag, view);
			}
			else if (element is AreaTag areaTag)
			{
				return GetAreaTagGeometryOnView(areaTag, view);
			}
			else if(element is Wall wall)
			{
				return GetWallGeometryOnView(wall, view);
			}
			else if (element is Stairs stairs)
			{
				return GetStairsGeometryOnView(stairs, view);
			}
			else if(element is FamilyInstance fi)
			{
				return GetFamilyInstanceGeometryOnView(fi, view);
			}
			else if(element is Floor floor)
			{
				return GetFloorGeometryOnView(floor, view);
			}
			else
			{
				throw new NotImplementedException(element.Category.BuiltInCategory.ToString() + " - " + element.Id.ToString());
			}
		}

		public static List<APIObject> GetIndependentTagGeometryOnView(IndependentTag tag, View view)
		{
			Document doc = tag.Document;

			Transaction trans = null;
			Options opts = new Options
			{
				View = view,
			};

			List<APIObject> geometries = new List<APIObject>();

			bool bLeader = false;
			bLeader = tag.HasLeader;
			if (bLeader)
			{
				XYZ tagHeadPosition = tag.TagHeadPosition;
				{
					foreach (Reference reference in tag.GetTaggedReferences())
					{
						XYZ leaderEnd = null;
						if (tag.LeaderEndCondition == LeaderEndCondition.Free)
							leaderEnd = tag.GetLeaderEnd(reference);
						else
						{
							Element host = doc.GetElement(reference.ElementId);
							if (host.Location is LocationPoint lp)
							{
								leaderEnd = lp.Point;
							}
							else
							{
								throw new NotImplementedException("Could not get the leader geometry of " + tag.Id + "\nCould not get the LocationPoint of Element(" + host.Id + ")");
							}
						}
						if (leaderEnd == null)
							//	Skip this leader
							throw new NotImplementedException("Could not get the leader geometry of " + tag.Id);

						if (tag.HasLeaderElbow(reference))
						{
							XYZ leaderElbow = tag.GetLeaderElbow(reference);
							geometries.Add(Line.CreateBound(tag.TagHeadPosition, leaderElbow));
							geometries.Add(Line.CreateBound(leaderEnd, leaderElbow));
						}
						else
						{
							geometries.Add(Line.CreateBound(tag.TagHeadPosition, leaderEnd));
						}
					}
				}

				trans = new Transaction(doc, "Temp");
				trans.Start();
				tag.HasLeader = false;
				doc.Regenerate();
			}

			BoundingBoxXYZ bb = tag.get_BoundingBox(view);
			geometries.Add(bb);

			if (bLeader)
			{
				tag.HasLeader = bLeader;
				trans.RollBack(); //	Cancel the transaction
			}

			return geometries;
		}

		public static List<APIObject> GetRoomTagGeometryOnView(RoomTag tag, View view)
		{
			Document doc = tag.Document;

			Options opts = new Options
			{
				View = view,
			};

			List<APIObject> geometries = new List<APIObject>();

			//	Ignore Leader Line geometry for Room Tags
			bool bLeader = false;
			bLeader = tag.HasLeader;

			Transaction trans = null;
			if (bLeader)
			{
				trans = new Transaction(doc, "Temp");
				trans.Start();

				tag.HasLeader = false;
			}

			BoundingBoxXYZ bb = tag.get_BoundingBox(view);
			if (bLeader)
			{
				tag.HasLeader = bLeader;
				trans.RollBack(); //	Cancel the transaction
			}

			geometries.Add(bb);

			return geometries;
		}

		public static List<APIObject> GetAreaTagGeometryOnView(AreaTag tag, View view)
		{
			Document doc = tag.Document;

			Options opts = new Options
			{
				View = view,
			};

			List<APIObject> geometries = new List<APIObject>();

			//	Ignore Leader Line geometry for Room Tags
			bool bLeader = false;
			bLeader = tag.HasLeader;

			Transaction trans = null;
			if (bLeader)
			{
				trans = new Transaction(doc, "Temp");
				trans.Start();

				tag.HasLeader = false;
			}

			BoundingBoxXYZ bb = tag.get_BoundingBox(view);
			if (bLeader)
			{
				tag.HasLeader = bLeader;
				trans.RollBack(); //	Cancel the transaction
			}

			//geometries.Add(bb);

			return geometries;
		}

		public static List<APIObject> GetWallGeometryOnView(Wall wall, View view)
		{
			List<APIObject> geometries = new List<APIObject>();
			Document doc = wall.Document;

			Options opts = new Options() { View = view };
			GeometryElement geoElem = wall.get_Geometry(opts);

			foreach (GeometryObject geoObj in geoElem)
			{
				if (geoObj is Solid solid)
				{
					foreach (Face face in solid.Faces)
					{
						if (face is PlanarFace pFace)
						{
							if (!pFace.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ))
								continue;

							foreach (CurveLoop cl in face.GetEdgesAsCurveLoops())
							{
								foreach (Curve c in cl)
								{
									if (c is Line line)
									{
										geometries.Add(line);
									}
									else
									{
										List<XYZ> points = (List<XYZ>)c.Tessellate();
										for (int i = 0; i < points.Count - 1; i++)
										{
											geometries.Add(Line.CreateBound(points[i], points[i + 1]));
										}
									}
								}
							}
						}
						else
						{
							//throw new NotImplementedException("Wall(" + wall.Id + ") has curved faces.");
						}
					}
				}
			}

			return geometries;
		}

		public static List<APIObject> GetFamilyInstanceGeometryOnView(FamilyInstance fi, View view)
		{
			List<APIObject> geometries = new List<APIObject>();
			Document doc = fi.Document;

			Options opts = new Options() { View = view };
			GeometryElement geoElem = fi.get_Geometry(opts);
			if (geoElem == null)
			{
				throw new NotImplementedException("FamilyInstance(" + fi.Id + ") does not have GeometryElement");
			}

			foreach (GeometryObject geoObj in geoElem)
			{
				if (geoObj is GeometryInstance geoInstance)
				{
					GeometryElement symbolGeometry = geoInstance.GetInstanceGeometry();
					foreach (GeometryObject symbolObj in symbolGeometry)
					{
						if (symbolObj is Curve curve)
						{
							if (curve is Line line)
							{
								geometries.Add(line);
							}
							else
							{
								List<XYZ> points = (List<XYZ>)curve.Tessellate();
								for (int i = 0; i < points.Count - 1; i++)
								{
									geometries.Add(Line.CreateBound(points[i], points[i + 1]));
								}
							}
						}
						else if (symbolObj is Solid solid)
						{
							foreach (Face face in solid.Faces)
							{
								if (face is PlanarFace pFace)
								{
									if (!pFace.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ))
										continue;

									foreach (CurveLoop cl in face.GetEdgesAsCurveLoops())
									{
										foreach (Curve c in cl)
										{
											if (c is Line line)
											{
												geometries.Add(line);
											}
											else
											{
												List<XYZ> points = (List<XYZ>)c.Tessellate();
												for (int i = 0; i < points.Count - 1; i++)
												{
													geometries.Add(Line.CreateBound(points[i], points[i + 1]));
												}
											}
										}
									}
								}
								else
								{
									throw new NotImplementedException("FamilyInstance(" + fi.Id + ") has curved faces.");
								}
							}
						}
						else if (symbolObj is Mesh mesh)
						{

						}
						else
						{

						}
					}
				}
			}

			return geometries;
		}

		public static List<APIObject> GetStairsGeometryOnView(Stairs stairs, View view)
		{
			List<APIObject> geometries = new List<APIObject>();
			Document doc = stairs.Document;

			Options opts = new Options() { View = view };
			GeometryElement geoElem = stairs.get_Geometry(opts);
			if (geoElem == null)
			{
				throw new NotImplementedException("Stairs(" + stairs.Id + ") does not have GeometryElement");
			}

			BoundingBoxXYZ bbox = stairs.get_BoundingBox(view);

			foreach (GeometryObject geoObj in geoElem)
			{
				if (geoObj is Curve curve)
				{
					if (curve is Line line)
					{
						geometries.Add(line);
					}
					else
					{
						List<XYZ> points = (List<XYZ>)curve.Tessellate();
						for (int i = 0; i < points.Count - 1; i++)
						{
							geometries.Add(Line.CreateBound(points[i], points[i + 1]));
						}
					}
				}
				else if (geoObj is Solid solid)
				{
					foreach (Face face in solid.Faces)
					{
						if (face is PlanarFace pFace)
						{
							if (!pFace.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ))
								continue;

							foreach (CurveLoop cl in face.GetEdgesAsCurveLoops())
							{
								foreach (Curve c in cl)
								{
									if (c is Line line)
									{
										geometries.Add(line);
									}
									else
									{
										List<XYZ> points = (List<XYZ>)c.Tessellate();
										for (int i = 0; i < points.Count - 1; i++)
										{
											geometries.Add(Line.CreateBound(points[i], points[i + 1]));
										}
									}
								}
							}
						}
						else
						{
							throw new NotImplementedException("Stairs(" + stairs.Id + " has curved faces.");
						}
					}
				}
				else if (geoObj is Mesh mesh)
				{

				}
				else
				{

				}
			}

			return geometries;
		}

		public static List<APIObject> GetFloorGeometryOnView(Floor floor, View view)
		{
			List<APIObject> geometries = new List<APIObject>();
			Document doc = floor.Document;

			Options opts = new Options() { View = view };
			GeometryElement geoElem = floor.get_Geometry(opts);
			if (geoElem == null)
			{
				throw new NotImplementedException("Floor(" + floor.Id + ") does not have GeometryElement");
			}

			BoundingBoxXYZ bbox = floor.get_BoundingBox(view);

			foreach (GeometryObject geoObj in geoElem)
			{
				if (geoObj is Curve curve)
				{
					if (curve is Line line)
					{
						geometries.Add(line);
					}
					else
					{
						List<XYZ> points = (List<XYZ>)curve.Tessellate();
						for (int i = 0; i < points.Count - 1; i++)
						{
							geometries.Add(Line.CreateBound(points[i], points[i + 1]));
						}
					}
				}
				else if (geoObj is Solid solid)
				{
					foreach (Face face in solid.Faces)
					{
						if (face is PlanarFace pFace)
						{
							//if (!pFace.FaceNormal.IsAlmostEqualTo(XYZ.BasisZ))
							if(pFace.FaceNormal.Z > 0.8)
								continue;

							foreach (CurveLoop cl in face.GetEdgesAsCurveLoops())
							{
								foreach (Curve c in cl)
								{
									if (c is Line line)
									{
										geometries.Add(line);
									}
									else
									{
										List<XYZ> points = (List<XYZ>)c.Tessellate();
										for (int i = 0; i < points.Count - 1; i++)
										{
											geometries.Add(Line.CreateBound(points[i], points[i + 1]));
										}
									}
								}
							}
						}
						else
						{
							//throw new NotImplementedException("Floor(" + floor.Id + " has curved faces.");
						}
					}
				}
				else if (geoObj is Mesh mesh)
				{

				}
				else
				{

				}
			}

			return geometries;
		}
	}
}
