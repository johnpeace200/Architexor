using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Architexor.Custom.Helpers;
using Architexor.Request;
using Architexor;
using View = Autodesk.Revit.DB.View;

namespace Architexor.Controllers
{
	public class Dimensioning : Controller
	{
		private List<ViewPlan> mAllViews;
		private List<ViewPlan> mViews;

		public List<ViewPlan> AllViews { get => mAllViews; set { mAllViews = value; } }

		/// <summary>
		/// Extract user settings into a clean dictionary.
		/// </summary>
		/// <param name="state">Dictionary of user settings.</param>
		/// <returns>Dictionary with parsed configuration.</returns>
		public static Dictionary<string, object> ParseConfig(Dictionary<string, object> state)
		{
			return new Dictionary<string, object>
				{
					// View Selection Node
					{ "view_mode", state.ContainsKey("view_mode") ? state["view_mode"] : "Current View" },
					{ "views", state.ContainsKey("views") ? state["views"] : new List<object>() },

					// Placement Options - (node) User Input -
					{ "direction", state.ContainsKey("direction") ? state["direction"] : "All" },
					{ "offset_from_origin", state.ContainsKey("offset_from_origin") ? state["offset_from_origin"] : 0.5 },		//	Work
					{ "gap_to_baseline", state.ContainsKey("gap_to_baseline") ? state["gap_to_baseline"] : 2 },								//	Work
					{ "chain_dimension", state.ContainsKey("chain_dimension") ? state["chain_dimension"] : true },
					{ "orientation", state.ContainsKey("orientation") ? state["orientation"] : "Aligned" },		//	Aligned|Axis
					{ "text_position", state.ContainsKey("text_position") ? state["text_position"] : "Up" },	//	Auto|Up			//	Work
					{ "avoid_overlap", state.ContainsKey("avoid_overlap") ? state["avoid_overlap"] : true },
					{ "leader_visible", state.ContainsKey("leader_visible") ? state["leader_visible"] : true },								//	Work
					{ "skip_tiny", state.ContainsKey("skip_tiny") ? state["skip_tiny"] : true },															//	Work
					{ "tiny_size", state.ContainsKey("tiny_size") ? state["tiny_size"] : 1 },																	//	Work

					// Wall Dimension Referencing Options
					{ "dim_wall", state.ContainsKey("dim_wall") ? state["dim_wall"] : true },
					{ "dim_wall_ref_options", state.ContainsKey("dim_wall_ref_options") ? state["dim_wall_ref_options"] : new List<Dictionary<string, object>>
						{
							new Dictionary<string, object>
							{
								{ "name", "tier 1" },
								{ "apply_to", "Outside" },
								{ "ref_type", "Wall Centerline" },
								{ "consider_openings", true },
								{ "opening_ref_type", "Center" },
								{ "consider_intersections", true },
								{ "consider_grids", true }
							},
							new Dictionary<string, object>
							{
								{ "apply_to", "Inside" },
								{ "ref_type", "Wall Face" },
								{ "consider_openings", true },
								{ "opening_ref_type", "Width" },
								{ "consider_intersections", false },
								{ "consider_grids", false }
							},
							new Dictionary<string, object>
							{
								{ "name", "tier 3" },
								{ "apply_to", "All" },
								{ "ref_type", "Wall Face" },
								{ "consider_openings", true },
								{ "opening_ref_type", "Center" },
								{ "consider_intersections", false },
								{ "consider_grids", false }
							},
							new Dictionary<string, object>
							{
								{ "apply_to", "None" },
								{ "ref_type", "Wall Face" },
								{ "consider_openings", true },
								{ "opening_ref_type", "Width" },
								{ "consider_intersections", false },
								{ "consider_grids", false }
							},
							new Dictionary<string, object>
							{
								{ "name", "tier 5" },
								{ "apply_to", "All" },
								{ "ref_type", "Wall Face" },
								{ "consider_openings", false },
								{ "opening_ref_type", "Center" },
								{ "consider_intersections", false },
								{ "consider_grids", false }
							}
						}
					}
				};
		}

		private static void GetDimensioningWallGroups(Document doc, List<DimensioningWallGroup> wallGroups, List<ElementId> candidates, ElementId wallId, int nDepth = 0)
		{
			Wall wall = doc.GetElement(wallId) as Wall;
			DimensioningWallGroup dimWallGroup;

			XYZ direction = Geometric.GetWallDirection(wall);
			XYZ normal = Geometric.GetWallNormal(wall);

			if (nDepth == 0)//wallGroups.Count == 0)
			{
				dimWallGroup = new DimensioningWallGroup
				{
					Side = "Outside",
					Direction = direction,
					Normal = normal,
					Walls = new List<DimensioningWall> { },
					IgnoredWalls = new List<ElementId> { },
				};
				wallGroups.Add(dimWallGroup);
			}
			else
			{
				dimWallGroup = wallGroups[wallGroups.Count - 1];
			}

			if(candidates.Contains(wallId)
				&& (direction.IsAlmostEqualTo(dimWallGroup.Direction) || direction.IsAlmostEqualTo(-dimWallGroup.Direction))
				&& normal.IsAlmostEqualTo(dimWallGroup.Normal))
			{
				dimWallGroup.Walls.Add(new DimensioningWall()
				{
					Wall = wall,
					StartReference = null,
					EndReference = null,
				});
				candidates.Remove(wallId);
			}
			dimWallGroup.IgnoredWalls.Add(wallId);

			//	Find all joined walls (at End Points only)
			LocationCurve locationCurve = wall.Location as LocationCurve;
			ElementArray joinedWalls = locationCurve.get_ElementsAtJoin(0);
			foreach (Element elem in joinedWalls)
			{
				if (elem is not Wall)
					continue;

				Wall adjacent = elem as Wall;

				//if (!candidates.Contains(adjacent))
				//if (candidates.Find(x => x.Id == adjacent.Id) == null)
				//	continue;

				if (dimWallGroup.IgnoredWalls.Contains(adjacent.Id))
					continue;

				GetDimensioningWallGroups(doc, wallGroups, candidates, adjacent.Id, nDepth + 1);
			}

			joinedWalls = locationCurve.get_ElementsAtJoin(1);
			foreach (Element elem in joinedWalls)
			{
				if (elem is not Wall)
					continue;

				Wall adjacent = elem as Wall;

				//if (!candidates.Contains(adjacent))
				//if (candidates.Find(x => x.Id == adjacent.Id) == null)
				//	continue;

				if (dimWallGroup.IgnoredWalls.Contains(adjacent.Id))
					continue;

				GetDimensioningWallGroups(doc, wallGroups, candidates, adjacent.Id, nDepth + 1);
			}
		}

		public void DoDimensioningWall(Document doc, Dictionary<string, object> state)
		{
			var cfg = ParseConfig(state);

			//if ((string)cfg["view_mode"] == "Selected Views")
			//	return null;

			List<Dictionary<string, object>> tiers = (List<Dictionary<string, object>>)cfg["dim_wall_ref_options"];

			//	Statistic Variables
			int nCreatedWallDims = 0, nSkippedWalls = 0;
			List<string> debugs = [];

			foreach (var view in mViews)
			{
				List<Wall> walls = new FilteredElementCollector(doc, view.Id)
						.OfClass(typeof(Wall))
						.Cast<Wall>()
						.ToList();

				bool outside = (string)cfg["direction"] == "Outside" || (string)cfg["direction"] == "All";
				bool inside = (string)cfg["direction"] == "Inside" || (string)cfg["direction"] == "All";

				List<DimensioningWallGroup> dimWallGroups = new List<DimensioningWallGroup>();

				if (outside)
				{
					List<ElementId> exteriorWalls = Geometric.GetRealExteriorWalls(view as ViewPlan).Select(x => x.Id).ToList();

					while (exteriorWalls.Count > 0)
					{
						GetDimensioningWallGroups(doc, dimWallGroups, exteriorWalls, exteriorWalls[0]);
					}
				}

				if (inside)
				{
					// Placeholder for inside wall group logic
				}

				foreach (DimensioningWallGroup dimWallGroup in dimWallGroups)
				{
					XYZ direction = dimWallGroup.Direction;
					XYZ normal = dimWallGroup.Normal;
					double offset = Convert.ToDouble(cfg["offset_from_origin"]);

					if (dimWallGroup.Side == "Inside")
						offset = 0;

					int prevRefCnt = 0;

					foreach (var tier in tiers)
					{
						var applied = (string)tier["apply_to"];
						if (!(applied == "All" || applied == dimWallGroup.Side))
							continue;

						var references = Custom.Helpers.Dimensioning.GetWallReferences(
								view,
								dimWallGroup,
								(string)tier["ref_type"],
								Convert.ToBoolean(tier["consider_openings"]),
								(string)tier["opening_ref_type"],
								Convert.ToBoolean(tier["consider_intersections"]),
								Convert.ToBoolean(tier["consider_grids"])
						);

						if (prevRefCnt == references.Count)
							//	Ignore this tier if no changes between the previous tier detected
							continue;

						prevRefCnt = references.Count;

						if (references == null || references.Count < 2)
						{
							nSkippedWalls++;
							debugs.Add("Not enough references(" + tier + $") found to create dimensions for wall({dimWallGroup.Walls[0].Wall.Id}).");
							continue;
						}

						references = Geometric.SortReferencesByDirection(doc, references, direction);

						XYZ start = Custom.Helpers.Dimensioning.GetStartPoint(view, dimWallGroup);
						Line dimLine = Custom.Helpers.Dimensioning.CreateDimensionLine(start, direction, normal, offset);
						Line unboundDimLine = dimLine.Clone() as Line;
						unboundDimLine.MakeUnbound();

						//	Remove duplicated references
						for (var i = 1; i < references.Count; i++)
						{
							XYZ pt1 = Geometric.GetPointFromReference(doc, references[i - 1])
								, pt2 = Geometric.GetPointFromReference(doc, references[i]);
							pt1 = unboundDimLine.Project(pt1).XYZPoint;
							pt2 = unboundDimLine.Project(pt2).XYZPoint;

							double d = pt1.DistanceTo(pt2);

							if (d < Geometric.TOL)
							{
								references.RemoveAt(i);
								i--;
								continue;
							}

							if ((bool) cfg["skip_tiny"])
							{
								double tinySize = Convert.ToDouble(cfg["tiny_size"]);
								//	assume the unit of tinySize is inch
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
								d = UnitUtils.ConvertFromInternalUnits(d, UnitTypeId.Inches);
#else
								d = UnitUtils.ConvertFromInternalUnits(d, DisplayUnitType.DUT_MILLIMETERES);
#endif
								// tiny gap merging logic here
								if (d < tinySize)
								{
									references.RemoveAt(i);
									i--;
								}
							}
						}

						bool chain = (bool)cfg["chain_dimension"];

						Dimension dim = Custom.Helpers.Dimensioning.CreateDimension(doc, view, references, dimLine);
						offset += Convert.ToDouble(cfg["gap_to_baseline"]);

						if(!(bool)cfg["leader_visible"])
						{
							dim.HasLeader = false;
						}
						
						if ((string)cfg["text_position"] == "Auto")
						{
							// Move dimension text logic
							foreach (DimensionSegment ds in dim.Segments)
							{
								if (ds.IsTextPositionAdjustable())
								{
									if (normal.IsAlmostEqualTo(-XYZ.BasisY) || normal.IsAlmostEqualTo(XYZ.BasisX))
									{
										XYZ currentTextPos = ds.TextPosition;

										DimensionType dt = dim.DimensionType;
										// Calculate textOffset by considering Text Offset and Text Size of DimensionType
										double textSize = 0.0;
										double textOffsetParam = 0.0;

										// Try to get text size and text offset from the dimension type parameters
										Parameter textSizeParam = dt.get_Parameter(BuiltInParameter.TEXT_SIZE);
										if (textSizeParam != null && textSizeParam.StorageType == StorageType.Double)
										{
											textSize = textSizeParam.AsDouble();
										}

										Parameter textOffsetParamObj = dt.get_Parameter(BuiltInParameter.TEXT_DIST_TO_LINE);
										if (textOffsetParamObj != null && textOffsetParamObj.StorageType == StorageType.Double)
										{
											textOffsetParam = textOffsetParamObj.AsDouble();
										}

										// Fallback to a default if not set
										if (textSize == 0.0)
											textSize = 0.02 * view.Scale;
										if (textOffsetParam == 0.0)
											textOffsetParam = 0.01 * view.Scale;

										// Combine text size and offset for the final textOffset
										double textOffset = (textSize + textOffsetParam) * 25;

										XYZ moveDir = normal.Normalize();
										XYZ newTextPos = currentTextPos + moveDir * textOffset;
										ds.TextPosition = newTextPos;
									}
								}
							}
						}
						
						if ((bool)cfg["avoid_overlap"])
						{
							// Avoid overlap logic
						}
					}

					nCreatedWallDims++;
				}
			}
		}

		public static void DoDimensioningInside(Document doc, Dictionary<string, object> state)
		{
			var cfg = ParseConfig(state);

			if (!(bool)cfg["dim_wall"])
				return;

			List<View> views = new List<View>()
			{
					doc.ActiveView
			};

			if (views.Count == 0 || doc.ActiveView == null || !(doc.ActiveView is ViewPlan))
				throw new Exception("Open a view before running.");

			//if ((string)cfg["view_mode"] == "Selected Views")
			//	return null;

			List<Dictionary<string, object>> tiers = (List<Dictionary<string, object>>)cfg["dim_wall_ref_options"];

			//	Statistic Variables
			int nCreatedWallDims = 0;
			List<string> debugs = [];

			foreach (var view in views)
			{
				if(!(view is ViewPlan)) continue;

				var (origin, size) = Geometric.GetCropView(view as ViewPlan);
				var center = new XYZ(size.Width / 2, size.Height / 2, 0);
				var wall2dList = GetWall2dInfoInView(view as ViewPlan);
				var instance2dList = GetInstance2dInfoInView(view as ViewPlan);

				// HashSet to track unique detects
				var uniqueDetects = new HashSet<string>();
				var outerWallGroups = new List<DimensioningWallGroup>();
				var innerWallGroups = new List<DimensioningWallGroup>();

				foreach (var wall2d in wall2dList)
				{
					if (wall2d.IsOuter) // Whether thsi wall is outer wall
					{
						// Set the refPoint0 to 0.1 of diagonal vector of rectangular of wall2d
						XYZ refPoint0 = wall2d.GetPointByRatio(0, 0.5);
						GetDimensioningGroups(wall2dList, instance2dList, wall2d, center, refPoint0, outerWallGroups, uniqueDetects);

						// Set the refPoint1 to 0.9 of diagonal vector of rectangular of wall2d
						XYZ refPoint1 = wall2d.GetPointByRatio(1, 0.5);
						GetDimensioningGroups(wall2dList, instance2dList, wall2d, center, refPoint1, outerWallGroups, uniqueDetects);
					}
					else
					{
						XYZ refPoint = wall2d.GetMidPoint();
						GetDimensioningGroups(wall2dList, instance2dList, wall2d, center, refPoint, innerWallGroups, uniqueDetects);
					}
				}

				// Do dimensioning for outer walls
				var uniqueRefPairs = new HashSet<string>();
				foreach (var dimWallGroup in outerWallGroups)
				{
					if (DoDimensioning(dimWallGroup, cfg, view as ViewPlan, uniqueRefPairs))
					{
						nCreatedWallDims++;
					}
				}

				// Do dimensioning for inner walls
				innerWallGroups = innerWallGroups.OrderByDescending(x => x.Walls.Count).ToList();
				var count = innerWallGroups.Count;
				for (int i = 0; i < count; i++)
				{
					var dimGroup = innerWallGroups[i];
					if (DoDimensioning(dimGroup, cfg, view as ViewPlan, uniqueRefPairs))
					{
						nCreatedWallDims++;
					}
				}
			}
		}

		/// <summary>
		/// Get wall 2d informations in view
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static List<Element2dInfo> GetWall2dInfoInView(ViewPlan view)
		{
			if (view == null)
				throw new ArgumentNullException("Document and View must not be null.");

			Document doc = view.Document;

			// Collect all wall elements in the view
			List<Wall> walls = new FilteredElementCollector(doc, view.Id)
					.OfClass(typeof(Wall))
					.Cast<Wall>()
					.ToList();

			// Get outer walls in the view
			var outerWalls = Geometric.GetOuterWalls(view);

			// Get the cropped view's origin and size in the view's coordinate system
			var (origin, size) = Geometric.GetCropView(view);

			var wall2dList = new List<Element2dInfo>();

			foreach (var wall in walls)
			{
				// Get world-aligned bounding box for each wall
				var bbox = Geometric.SafeWorldBoundingBox(wall, view);

				// Get the wall's direction vector in 3D space
				var dir = Geometric.GetWallDirection(wall);

				// Convert bounding box corners into 2D rect based on view's coordinate system
				var (xmin, ymin, xmax, ymax) = Geometric.RectFromBbox(
						bbox,
						view.Origin,
						view.RightDirection,
						view.UpDirection);

				// Convert points into local 2D coordinates relative to the crop origin
				wall2dList.Add(new Element2dInfo(){
						Id = wall.Id,
						Instance = wall,
						Direction = Geometric.GetWallDirection(wall),
						Normal = Geometric.GetWallNormal(wall),
						Min = new UV(xmin - origin.X, ymin - origin.Y),
						Max = new UV(xmax - origin.X, ymax - origin.Y),
						IsOuter = outerWalls.Where(w => w.Id == wall.Id).Any(), // Check if this wall is outer
					}
				);
			}

			return wall2dList;
		}

		/// <summary>
		/// Get 2d information of all the instances in view
		/// </summary>
		/// <param name="view"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static List<Element2dInfo> GetInstance2dInfoInView(ViewPlan view)
		{
			if (view == null)
				throw new ArgumentNullException("Document and View must not be null.");

			Document doc = view.Document;

			// Collect all wall elements in the view
			List<Element> elements = new FilteredElementCollector(doc, view.Id)
					.WhereElementIsNotElementType()
					.ToElements()
					.Where(e =>
						e.Category != null &&
						e.Category.BuiltInCategory != BuiltInCategory.OST_Walls &&
						e.Category.BuiltInCategory != BuiltInCategory.OST_Floors &&
						e.Category.BuiltInCategory != BuiltInCategory.OST_Rooms &&
						e.Category.BuiltInCategory != BuiltInCategory.OST_Doors &&
						e.Category.BuiltInCategory != BuiltInCategory.OST_Windows &&
						e.Category.CategoryType == CategoryType.Model &&
						!e.IsHidden(view) &&
						e.get_Geometry(new Options() { 
							ComputeReferences = true,
							IncludeNonVisibleObjects = true
						}) != null
					)
					.ToList();

			// Get the cropped view's origin and size in the view's coordinate system
			var (origin, size) = Geometric.GetCropView(view);

			var element2dList = new List<Element2dInfo>();

			foreach (var element in elements)
			{
				// Get world-aligned bounding box for each wall
				var bbox = Geometric.SafeWorldBoundingBox(element, view);

				// Convert bounding box corners into 2D rect based on view's coordinate system
				var (xmin, ymin, xmax, ymax) = Geometric.RectFromBbox(
						bbox,
						view.Origin,
						view.RightDirection,
						view.UpDirection);

				// Convert points into local 2D coordinates relative to the crop origin
				element2dList.Add(new Element2dInfo() {
						Id = element.Id,
						Instance = element,
						Min = new UV(xmin - origin.X, ymin - origin.Y),
						Max = new UV(xmax - origin.X, ymax - origin.Y)
					}
				);
			}

			return element2dList;
		}

		/// <summary>
		/// Get dimensioning groups from elements detected from raycasting of refPoint along normal of source.
		/// </summary>
		/// <param name="wall2dList">Wall 2d information</param>
		/// <param name="instance2dList">Element 2d information</param>
		/// <param name="center">Center of view</param>
		/// <param name="source">Source element to be used for detection</param>
		/// <param name="refPoint">Point on the rectangular of source element</param>
		/// <param name="dimGroups">Dimensioning groups to be used for creating dimensions</param>
		/// <param name="uniqueDetects">Unique detect sets to filter the same detected element sets</param>
		private static void GetDimensioningGroups(List<Element2dInfo> wall2dList, List<Element2dInfo> instance2dList, Element2dInfo source, XYZ center, XYZ refPoint, List<DimensioningWallGroup> dimGroups, HashSet<string> uniqueDetects)
		{
			if(source.Direction == null || source.Normal == null) return;
			var direction = source.Direction;
			var normal = source.Normal;

			var detectedWalls = Geometric.GetWallsByRay(wall2dList, direction, refPoint, normal);
			if (detectedWalls.Count <= 1) return;

			var detectedInstances = Geometric.GetElementsByRay(instance2dList, refPoint, normal);

			// Use a string hash to identify duplicates
#if REVIT2024 || REVIT2025
			string detectKey = string.Join(",", detectedWalls.OrderBy(d => d.Id.Value).Select(d => d.Id.Value.ToString()));
#else
			string detectKey = string.Join(",", detectedWalls.OrderBy(d => d.Id.IntegerValue).Select(d => d.Id.IntegerValue.ToString()));
#endif

			// Skip if already processed
			if (!uniqueDetects.Add(detectKey)) return;

			var dimWalls = new List<DimensioningWall>();
			foreach (var detect in detectedWalls)
			{
				dimWalls.Add(new DimensioningWall()
				{
					Wall = detect.Instance as Wall,
					StartReference = null,
					EndReference = null
				});
			}

			var dimInstances = new List<DimensioningElement>();
			foreach (var detect in detectedInstances)
			{
				dimInstances.Add(new DimensioningElement()
				{
					Element = detect.Instance,
					StartReference = null,
					EndReference = null
				});
			}

			// To get group from walls where perpedicular to the target wall, so the direction is normal of target wall.
			var dimWallGroup = new DimensioningWallGroup
			{
				Side = "Inside",
				IsOuter = source.IsOuter,
				Direction = normal,
				Normal = Geometric.AdjustVectorDirection(center, refPoint, direction),
				Walls = dimWalls,
				Instances = dimInstances
			};

			dimGroups.Add(dimWallGroup);
		}

		/// <summary>
		/// Do dimensioning using specified dimensioning group
		/// </summary>
		/// <param name="dimGroup"></param>
		/// <param name="cfg"></param>
		/// <param name="view"></param>
		/// <param name="uniqueRefPairs"></param>
		/// <returns></returns>
		private static bool DoDimensioning(DimensioningWallGroup dimGroup, Dictionary<string, object> cfg, ViewPlan view,
				HashSet<string> uniqueRefPairs)
		{
			Document doc = view.Document;
			XYZ direction = dimGroup.Direction;
			XYZ normal = dimGroup.Normal;
			var (origin, size) = Geometric.GetCropView(view);
			double offset = Convert.ToDouble(cfg["offset_from_origin"]);
			int prevRefCnt = 0;

			List<Dictionary<string, object>> tiers = (List<Dictionary<string, object>>)cfg["dim_wall_ref_options"];
			foreach (var tier in tiers)
			{
				var applied = (string)tier["apply_to"];
				if (!(applied == "All" || applied == dimGroup.Side))
					continue;

				var references = new List<Reference>();

				references = Custom.Helpers.Dimensioning.GetInsideWallReferences(
						view,
						dimGroup,
						(string)tier["ref_type"],
						Convert.ToBoolean(tier["consider_openings"]),
						(string)tier["opening_ref_type"],
						Convert.ToBoolean(tier["consider_intersections"]),
						Convert.ToBoolean(tier["consider_grids"])
					);

				if ((bool)cfg["skip_tiny"])
				{
					double tinySize = Convert.ToDouble(cfg["tiny_size"]);
					// Implement tiny gap merging logic here
				}

				if (prevRefCnt == references.Count)
					//	Ignore this tier if no changes between the previous tier detected
					continue;

				prevRefCnt = references.Count;

				// Filter references by its id
				//references = references.GroupBy(r => r.ElementId).Select(g => g.FirstOrDefault()).OrderBy(x => x.ElementId.Value).ToList();

				if (references == null || references.Count < 2)
				{
					return false;
				}

				references = Geometric.SortReferencesByDirection(doc, references, direction);

				XYZ start = new XYZ(0, 0, 0);

				if (dimGroup.IsOuter)
				{
					start = Custom.Helpers.Dimensioning.GetStartPoint(view, dimGroup);

					// Refine start because its value should be get positive
					start = new XYZ(Math.Max(start.X, origin.X + 0.5), Math.Max(start.Y, origin.Y + 0.5), 0);
					start = new XYZ(Math.Min(start.X, origin.X + size.Width), Math.Min(start.Y, origin.Y + size.Height), 0);
				}
				else
				{
					// Get the endpoint of wall which is the smallest
					var smallest = dimGroup.Walls.OrderBy(w => Geometric.GetWallLength(w.Wall)).FirstOrDefault();
					start = (smallest.Wall.Location as LocationCurve).Curve.GetEndPoint(0);
				}

				bool chain = (bool)cfg["chain_dimension"];

				if(dimGroup.IsOuter)
				{
					if (references.Count <= 1) continue;

					// Update unique reference pairs using references

					for(int j = 0; j < references.Count - 1; j++)
					{
#if REVIT2024 || REVIT2025
						long id1 = references[j].ElementId.Value;
						long id2 = references[j + 1].ElementId.Value;
#else
						long id1 = references[j].ElementId.IntegerValue;
						long id2 = references[j + 1].ElementId.IntegerValue;
#endif
						string pairKey = id1 < id2 ? $"{id1}_{id2}" : $"{id2}_{id1}";

						uniqueRefPairs.Add(pairKey);
					}

					Line dimLine = Custom.Helpers.Dimensioning.CreateDimensionLine(start, direction, normal, offset);
					Dimension dim = Custom.Helpers.Dimensioning.CreateDimension(doc, view, references, dimLine);
					offset += Convert.ToDouble(cfg["gap_to_baseline"]);

					if ((string)cfg["text_position"] == "Down")
					{
						// Move dimension text logic
					}

					if ((bool)cfg["avoid_overlap"])
					{
						// Avoid overlap logic
					}
				}
				else
				{
					var splitedReferenceGroup = GetSplitedReferences(references, uniqueRefPairs);

					foreach (var refs in splitedReferenceGroup)
					{
						if (refs.Count <= 1) continue;

						Line dimLine = Custom.Helpers.Dimensioning.CreateDimensionLine(start, direction, normal, -offset);
						Dimension dim = Custom.Helpers.Dimensioning.CreateDimension(doc, view, refs, dimLine);
						offset += Convert.ToDouble(cfg["gap_to_baseline"]);

						if ((string)cfg["text_position"] == "Down")
						{
							// Move dimension text logic
						}

						if ((bool)cfg["avoid_overlap"])
						{
							// Avoid overlap logic
						}
					}

				}
			}

			return true;
		}

		/// <summary>
		/// Split references and remove duplicated with unique reference pairs and get remained references
		/// </summary>
		/// <param name="references"></param>
		/// <param name="uniqueRefPairs"></param>
		/// <returns></returns>
		private static List<List<Reference>> GetSplitedReferences(List<Reference> references, HashSet<string> uniqueRefPairs)
		{
			// Get rid of reference set once it is used before
			var splitReferenceGroup = new List<List<Reference>>();
			var temp = new List<Reference>();

			for (int j = 0; j < references.Count - 1; j++)
			{
				var ref1 = references[j];
				var ref2 = references[j + 1];

				temp.Add(ref1);
#if REVIT2024 || REVIT2025
				long id1 = ref1.ElementId.Value;
				long id2 = ref2.ElementId.Value;
#else
				long id1 = ref1.ElementId.IntegerValue;
				long id2 = ref2.ElementId.IntegerValue;
#endif
				string pairKey = id1 < id2 ? $"{id1}_{id2}" : $"{id2}_{id1}";

				// Split if this pair was already processed
				if (!uniqueRefPairs.Add(pairKey))
				{
					splitReferenceGroup.Add(new List<Reference>(temp));
					temp.Clear();
				}
			}

			// Ensure the final segment is added if any references remain
			if (temp.Count > 0)
			{
				temp.Add(references[references.Count - 1]); // Add the last reference
				splitReferenceGroup.Add(temp);
			}

			return splitReferenceGroup;
		}

		public override void Initialize()
		{
			//	Get Views
			Document doc = m_uiApp.ActiveUIDocument.Document;

			// Get all plan views in the document and sort by ViewType and Name
			List<ViewPlan> planViews = new FilteredElementCollector(doc)
				.OfClass(typeof(ViewPlan))
				.Cast<ViewPlan>()
				.Where(vp => !vp.IsTemplate)
				.OrderBy(vp => vp.ViewType)
				.ThenBy(vp => vp.Name)
				.ToList();

			mAllViews = planViews;
		}

		public override bool ProcessRequest(ArchitexorRequestId reqId)
		{
			bool bFinish = false;
			switch(reqId)
			{
				case ArchitexorRequestId.DetectAndSelectExteriorWalls:
					if (mViews.Count == 0) {
						throw new Exception("Please select at least one view.");
					}

					Document doc = GetDocument();
					foreach (ViewPlan view in mViews)
					{
						List<Wall> walls = new FilteredElementCollector(doc, view.Id)
								.OfClass(typeof(Wall))
								.Cast<Wall>()
								.ToList();

						//bool outside = (string)cfg["direction"] == "Outside" || (string)cfg["direction"] == "All";
						//bool inside = (string)cfg["direction"] == "Inside" || (string)cfg["direction"] == "All";

						//List<DimensioningWallGroup> dimWallGroups = new List<DimensioningWallGroup>();
						Selection sel = m_uiApp.ActiveUIDocument.Selection;

						Transaction trans = new Transaction(doc, "GetRealExteriorWalls");
						trans.Start();
						//List<ElementId> exteriorWalls = Geometric.GetRealExteriorWalls(view).Select(x => x.Id).ToList();
						sel.SetElementIds(Geometric.GetLocationCurve(doc, view, sel.GetElementIds().ToList()));
						trans.Commit();

//						sel.SetElementIds(exteriorWalls);
					}
					break;
				default:
					break;
			}

			return bFinish;
		}

		public void SetSelectedViews(List<ViewPlan> views)
		{
			mViews = views;
		}
	}
}