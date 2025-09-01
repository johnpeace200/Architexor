using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using View = Autodesk.Revit.DB.View;

namespace Architexor.Custom.Helpers
{
	public struct DimensioningWall
	{
		public Wall Wall;
		public Reference StartReference;
		public Reference EndReference;
	}
	public struct DimensioningElement
	{
		public Element Element;
		public Reference StartReference;
		public Reference EndReference;
	}

	public struct DimensioningWallGroup
	{
		public string Side;
		public bool IsOuter;
		public XYZ Direction;
		public XYZ Normal;
		public List<DimensioningWall> Walls;
		public List<DimensioningElement> Instances;

		//	Temporary
		public List<ElementId> IgnoredWalls;
	}

	public struct DimensionReferenceOption
	{
		public string ApplyTo; //	Outside|Inside|None																							//	Works
		public string ReferenceType; //	Wall Centerline|Wall Face|Core Centerline|Core Face				//	Only works for Wall Centerline|Wall Face
		public bool ConsiderOpenings;																															//	Works
		public string OpeningReferenceType;  //	Width|Center																			//	Works
		public bool ConsiderIntersections;																												//	Works
		public bool ConsiderGrids;																																//	Works
	}

	public struct Element2dInfo
	{
		public Element Instance;
		public ElementId Id; // Id of wall
		public XYZ Direction; // Direction of wall
		public XYZ Normal; // Normal of wall
		public UV Min; // Minimum point of the rectangular of wall bounding box projected onto the plan
		public UV Max; // Maximum point of the rectangular of wall bounding box projected onto the plan
		public bool IsOuter;

		public XYZ GetPointByRatio(int pos, double tol)
		{
			XYZ max = new XYZ(Max.U, Max.V, 0);
			XYZ min = new XYZ(Min.U, Min.V, 0);

			return pos == 0 ? min + (max - min).Normalize() * tol : max - (max - min).Normalize() * tol;
		}

		public XYZ GetMidPoint()
		{
			XYZ max = new XYZ(Max.U, Max.V, 0);
			XYZ min = new XYZ(Min.U, Min.V, 0);

			return (max + min) / 2;
		}
	}

	internal static class Dimensioning
	{
		/// <summary>
		/// Returns the DimensionType with the given name, or the first available if not found.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static DimensionType GetDimensionType(Document doc, string name)
		{
			List<DimensionType> types = new FilteredElementCollector(doc)
				.OfClass(typeof(DimensionType))
				.Cast<DimensionType>()
				.ToList();

			foreach (DimensionType dt in types)
			{
				Parameter param = dt.get_Parameter(BuiltInParameter.SYMBOL_NAME_PARAM);
				if (param != null && param.AsString() == name)
					return dt;
			}
			return types.Count == 0 ? null : types[0];
		}
	
		/// <summary>
    /// Gets references from a set of walls for dimensioning, considering openings, intersections, and grids.
    /// </summary>
    /// <param name="view">The Revit view.</param>
    /// <param name="dwg">List of wall reference objects (each with a 'ref' property of type Wall).</param>
    /// <param name="refType">Reference type: "Wall Centerline" or "Wall Face".</param>
    /// <param name="stats">A dictionary for debug/statistics output.</param>
    /// <param name="considerOpenings">Whether to consider openings (windows/doors).</param>
    /// <param name="openingRefType">Type of opening reference, e.g., "Center".</param>
    /// <param name="considerIntersections">Whether to consider intersecting walls.</param>
    /// <param name="considerGrids">Whether to consider grids (not implemented).</param>
    /// <returns>List of references for dimensioning.</returns>
    public static List<Reference> GetWallReferences(
				View view,
				DimensioningWallGroup dwg,
				string refType,
				bool considerOpenings,
				string openingRefType,
				bool considerIntersections = true,
				bool considerGrids = true)
		{
			var references = new List<Reference>();
			if (dwg.Walls == null || dwg.Walls.Count == 0)
				return references;

			// Assume all wall directions in the group are the same
			Wall firstWall = dwg.Walls[0].Wall;
			XYZ wallDirection = Geometric.GetWallDirection(firstWall);

			Document doc = firstWall.Document;

			foreach (DimensioningWall dwall in dwg.Walls)
			{
				Wall wall = dwall.Wall;

				//	Find all openings associated to this wall(We will filter by their host later)
				ICollection<ElementId> hostIds = wall.FindInserts(true, true, true, true);

				Options opts = new Options { ComputeReferences = true, View = view, IncludeNonVisibleObjects = false };
				GeometryElement geo = wall.get_Geometry(opts);

				foreach (GeometryObject obj in geo)
				{
					if (obj is Solid solid)
					{
						foreach (Face face in solid.Faces)
						{
							if (!(face is PlanarFace))
								//	We support PlanarFace only for now
								continue;
							
							// Filter by direction
							if ((face as PlanarFace).FaceNormal.IsAlmostEqualTo(wallDirection) ||
							(face as PlanarFace).FaceNormal.IsAlmostEqualTo(-wallDirection))
							{
								ICollection<ElementId> generatingIds = wall.GetGeneratingElementIds(face);

								// Exclude faces that belong to openings (e.g., windows, doors)
								if (generatingIds.Count == 1 && hostIds.Contains(generatingIds.First()))
								{
									// If an opening generates this face, generatingIds will include the opening id only
									continue;
								}
								else if(generatingIds.Count == 1)
								{
									// This is not an opening reference
									if(face.Reference != null)
									{
										//	Some faces don't provide Reference
										//	Exception: Why null?
										references.Add(face.Reference);
									}
									
								}
							}
						}
					}
				}

				if(considerOpenings)
				{
					foreach (ElementId openingId in hostIds)
					{
						Element opening = doc.GetElement(openingId);
						//	Check host
						if(opening is FamilyInstance fi && fi.Host.Id != wall.Id)
						{
							continue;
						}

						if (openingRefType == "Center")
						{
							Reference openingRef = Geometric.GetOpeningReference(opening, 1);
							if (openingRef != null)
								references.Add(openingRef);
						}
						else
						{
							Reference openingRef = Geometric.GetOpeningReference(opening, 0);
							if (openingRef != null)
								references.Add(openingRef);

							openingRef = Geometric.GetOpeningReference(opening, 2);
							if (openingRef != null)
								references.Add(openingRef);
						}
					}
				}

				//	Get the start and end reference lines
				// Get all intersecting walls
				List<Wall> intersectingWalls = Geometric.GetIntersectingWalls(wall, view);

				// Get joined elements
				List<ElementId> joinedElems = new List<ElementId>();
				LocationCurve location = wall.Location as LocationCurve;
					
				if (location != null)
				{
					//	Find joined elements at start and end points
					for (int k = 0; k < 100; k++)
					{
						try
						{
							ElementArray joined = location.get_ElementsAtJoin(k);
							if (joined == null || joined.Size == 0)
								break;

							if (!considerIntersections && k > 1)
								break;

							foreach (Element j in joined)
							{
								if (j.Id != wall.Id &&
									!joinedElems.Contains(j.Id) &&	//	Already checked element
									intersectingWalls.FindIndex(x => x.Id == j.Id) >= 0	//	Ignore walls that are not visible in view
									)
								{
									XYZ normal = Geometric.GetWallNormal(j as Wall);
									if (!(normal.IsAlmostEqualTo(wallDirection) || normal.IsAlmostEqualTo(-wallDirection)))
										//	Consider perpendicular walls only
										continue;

									joinedElems.Add(j.Id);

									if (refType == "Wall Centerline")
									{
										var refer = Geometric.GetWallCenterlineReference(j as Wall);
										if (refer != null) references.Add(refer);
									}
									else if (refType == "Wall Face")
									{
										var refer = Geometric.GetWallSideReference(j as Wall, ShellLayerType.Exterior);
										if (refer != null) references.Add(refer);
										refer = Geometric.GetWallSideReference(j as Wall, ShellLayerType.Interior);
										if (refer != null) references.Add(refer);
									}
								}
							}
						}
						catch
						{
							break;
						}
					}

					if (considerIntersections)
					{
						//	Find other intersecting walls by BoundingBox check
						foreach (Wall intersectingWall in intersectingWalls)
						{
							XYZ normal = Geometric.GetWallNormal(intersectingWall);
							if (!(normal.IsAlmostEqualTo(wallDirection) || normal.IsAlmostEqualTo(-wallDirection)))
								//	Consider perpendicular walls only
								continue;

							if (joinedElems.Contains(intersectingWall.Id))
								continue;

							joinedElems.Add(intersectingWall.Id);

							if (refType == "Wall Centerline")
							{
								// Use location curve
								Reference refer = Geometric.GetWallCenterlineReference(intersectingWall);
								if (refer != null)
									references.Add(refer);
							}
							else if (refType == "Wall Face")
							{
								Reference refer = Geometric.GetWallSideReference(intersectingWall, ShellLayerType.Exterior);
								if (refer != null) references.Add(refer);
								refer = Geometric.GetWallSideReference(intersectingWall, ShellLayerType.Interior);
								if (refer != null) references.Add(refer);
							}
						}
					}
				}

				if (considerGrids)
				{
					XYZ wallNormal = Geometric.GetWallNormal(firstWall);
					//	Get all location curves
					List<Curve> curves = new List<Curve>();
					foreach(DimensioningWall dWall in dwg.Walls)
					{
						curves.Add((dWall.Wall.Location as LocationCurve).Curve);
					}

					List<Grid> grids = new FilteredElementCollector(doc, view.Id).OfClass(typeof(Grid)).Cast<Grid>().ToList();

					foreach(Grid grid in grids)
					{
						//	Check direction
						if (!((grid.Curve as Line).Direction.IsAlmostEqualTo(wallNormal)
							|| (grid.Curve as Line).Direction.IsAlmostEqualTo(-wallNormal)))
							continue;

						if (grid.Curve == null)
							continue;

						//	Check Intersection
						foreach(Curve curve in curves)
						{
							SetComparisonResult res = curve.Intersect(grid.Curve);
							if (res == SetComparisonResult.Overlap || res == SetComparisonResult.Subset)
							{
								Reference r = new Reference(grid);
								references.Add(r);
							}
						}
					}
				}
			}

			// TODO: considerGrids is not implemented for now
			return references;
		}

		/// <summary>
		/// Get references of walls inside in view
		/// </summary>
		/// <param name="view"></param>
		/// <param name="dwg"></param>
		/// <param name="refType"></param>
		/// <param name="considerOpenings"></param>
		/// <param name="openingRefType"></param>
		/// <param name="considerIntersections"></param>
		/// <param name="considerGrids"></param>
		/// <returns></returns>
		public static List<Reference> GetInsideWallReferences(
				View view,
				DimensioningWallGroup dwg,
				string refType,
				bool considerOpenings,
				string openingRefType,
				bool considerIntersections = true,
				bool considerGrids = true)
		{
			var references = new List<Reference>();
			if (dwg.Walls == null || dwg.Walls.Count == 0)
				return references;

			// Assume all wall directions in the group are the same
			Wall firstWall = dwg.Walls[0].Wall;
			XYZ wallDirection = Geometric.GetWallDirection(firstWall);
			XYZ wallNormal = Geometric.GetWallNormal(firstWall);

			Document doc = firstWall.Document;

			foreach (DimensioningWall dwall in dwg.Walls)
			{
				Wall wall = dwall.Wall;

				//	Find all openings associated to this wall(We will filter by their host later)
				ICollection<ElementId> hostIds = wall.FindInserts(true, true, true, true);

				if (refType == "Wall Centerline")
				{
					// Use location curve
					Reference refer = Geometric.GetWallCenterlineReference(wall);
					if (refer != null)
						references.Add(refer);
				}
				else if (refType == "Wall Face")
				{
					Reference refer = Geometric.GetWallSideReference(wall, ShellLayerType.Exterior);
					if (refer != null) references.Add(refer);
					refer = Geometric.GetWallSideReference(wall, ShellLayerType.Interior);
					if (refer != null) references.Add(refer);
				}
			}

			var instanceReferences = new List<Reference>();
			// Add all references of instances
			if (dwg.Instances.Any())
			{
				foreach(DimensioningElement dElem in dwg.Instances)
				{
					var elem = dElem.Element;
					var elemReferences = Geometric.GetReferencesFromElement(elem);
					if (!elemReferences.Any()) continue;

					//foreach (var reference in elemReferences)
					//{
					//	XYZ dir = Geometric.GetReferenceDirection(doc, reference);
					//	if(dir.IsAlmostEqualTo(wallNormal) || dir.IsAlmostEqualTo(-wallNormal))
					//	{
					//instanceReferences.Add(elemReferences[0]);
					//	}
					//}
					//break;
				}

				// Filter instanceReferences
				if(instanceReferences.Count > 0)
				{
					references.AddRange(instanceReferences);
				}
			}

			return references;
		}

		/// <summary>
		/// Creates a dimension line at a given offset from a start point, in a specified direction and normal.
		/// </summary>
		/// <param name="start">The start point of the dimension line.</param>
		/// <param name="direction">The direction vector of the dimension line.</param>
		/// <param name="normal">The normal vector to offset the line.</param>
		/// <param name="offset">The offset distance from the start point along the normal.</param>
		/// <returns>A Line object representing the dimension line.</returns>
		public static Line CreateDimensionLine(XYZ start, XYZ direction, XYZ normal, double offset)
		{
			XYZ offsetStart = start.Add(normal.Multiply(offset));
			XYZ offsetEnd = offsetStart.Add(direction.Multiply(100)); // 100 units in the direction
			return Line.CreateBound(offsetStart, offsetEnd);
		}

		/// <summary>
		/// Creates a dimension in the given view using the specified references and dimension line.
		/// </summary>
		/// <param name="doc">The Revit document.</param>
		/// <param name="view">The view to place the dimension in.</param>
		/// <param name="references">The list of references to dimension.</param>
		/// <param name="line">The dimension line.</param>
		/// <param name="dimensionTypeName">The name of the dimension type to use.</param>
		/// <returns>The created Dimension element, or null if creation failed.</returns>
		public static Dimension CreateDimension(Document doc, View view, List<Reference> references, Line line)
		{
			DimensionType dimType = GetDimensionType(doc, "dimension_type string here");

			ReferenceArray refArr = new ReferenceArray();
			foreach (var r in references)
			{
				refArr.Append(r);
			}

			return doc.Create.NewDimension(view, line, refArr, dimType);
		}

		public static XYZ GetStartPoint(View view, DimensioningWallGroup wallGroup)
		{
			//	Check all boundingboxes of wallGroup.Walls.Wall
			//	Find the one with the max coordinate by wallGroup.Normal
			if (wallGroup.Walls == null || wallGroup.Walls.Count == 0)
				return null;

			double maxDot = double.MinValue;
			XYZ maxPoint = null;

			foreach (DimensioningWall dimWall in wallGroup.Walls)
			{
				Wall wall = dimWall.Wall;

				BoundingBoxXYZ bbox = Geometric.SafeWorldBoundingBox(wall, view);

				// Check all 8 corners of the bounding box
				List<XYZ> corners = new List<XYZ>
										{
											new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Min.Z),
											new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Max.Z),
											new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z),
											new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Max.Z),
											new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Min.Z),
											new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Max.Z),
											new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Min.Z),
											new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Max.Z)
										};

				foreach (XYZ pt in corners)
				{
					double dot = pt.DotProduct(wallGroup.Normal);
					if (dot > maxDot)
					{
						maxDot = dot;
						maxPoint = pt;
					}
				}
			}

			return maxPoint;
		}
	}
}
