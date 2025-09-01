using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Architexor.Custom.Helpers;
using Architexor.Request;
using Autodesk.Revit.UI.Selection;
using View = Autodesk.Revit.DB.View;

namespace Architexor.Controllers
{
	public class Tagging : Controller
	{
		private List<ViewPlan> mAllViews;
		private List<ViewPlan> mViews;

		public List<ViewPlan> AllViews { get => mAllViews; set { mAllViews = value; } }

		private bool m_bConsiderLink = false;
		public bool ConsiderLink { get => m_bConsiderLink; set { m_bConsiderLink = value; } }

		private bool m_bAvoidOverlap = false;
		public bool AvoidOverlap { get => m_bAvoidOverlap; set { m_bAvoidOverlap = value; } }

		private bool m_bMultiHosting = true;
		public bool MultiHosting { get => m_bMultiHosting; set { m_bMultiHosting = value; } }

		private bool m_bMergeLeaders = true;
		public bool MergeLeaders { get => m_bMergeLeaders; set { m_bMergeLeaders = value; } }

		private List<string> m_OverlapConsiderCategories = new List<string>();
		public List<string> OverlapConsiderCategories { get => m_OverlapConsiderCategories; set { m_OverlapConsiderCategories = value; } }

		public List<TagHostingOption> TagHostingOptions { get; set; }

		private static Dictionary<ElementId, ViewSpecificCache> _VIEW_CACHE = new Dictionary<ElementId, ViewSpecificCache>();

		public List<(string, List<(ElementId, ElementId)>)> DoTagging()
		{
			Document doc = GetDocument();
			// Parse the input state to the configuration
			//var cfg = ParseConfig(state);
			//if (cfg == null || cfg.Count == 0)
			//	return;

			var res = new List<(string, List<(ElementId, ElementId)>)>();
			if (TagHostingOptions == null)
				return res;

			foreach (TagHostingOption option in TagHostingOptions)
			{
				List<(ElementId, ElementId)> generated = DoTaggingPerCategory(option);
				res.Add((option.Category.Name, generated));
			}
			return res;
		}

		/// <summary>
		/// This function will be run per each targeted category
		/// In an order where they are arranged by their priority.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="option"></param>
		/// <returns></returns>
		public List<(ElementId, ElementId)> DoTaggingPerCategory(TagHostingOption option)
		{
			List<(ElementId, ElementId)> pairs = new List<(ElementId, ElementId)>();

			Document doc = GetDocument();
			
			TagOrientation orientation = option.Orientation;
			TagEndType endType = option.EndType;
			TagHeadingDirection headingDirection = option.HeadingDirection;
			bool leader_visible = option.LeaderVisible;
			BuiltInCategory targetCategory = option.Category.BuiltInCategory;
			
			if(option.TagTypeName == "")
			{
#if DEBUG
				Debug.Assert(false);
#endif
				return pairs;
			}

			// Get TagType and Get The ActualSize of it (No Scaled)
			FamilySymbol tagType = new FilteredElementCollector(doc)
				.WhereElementIsElementType()
				.OfClass(typeof(FamilySymbol))
				.Where(x => x.Name == option.TagTypeName)
				.Cast<FamilySymbol>()
				.First();
			BuiltInCategory tagCategory = tagType.Category.BuiltInCategory;

			bool isSpatial = Custom.Helpers.Tagging.IsSpatialTag(tagCategory);

			//	This does not work.
			//XY tagSize = Custom.Helpers.Tagging.GetTagSymbolSize(doc, tagType);
			
			// Do Tagging per view
			foreach (View view in mViews)
			{
				// TODO: ensure that maxoffset is calculated as paper-size.
				double maxOffset = Geometric.Paper2RealSizeInView(view, option.MaxOffset, Geometric.UnitOption.Inch);
				//XY tagSizeInView = tagSize.ScaledOne(view.Scale);

				// Get Target Elements
				// TODO: ensure the elements should be in the view Crop Region.
				// Skip partially included elements in View depending on the categories.
				List<Element> target_elements = Custom.Helpers.Tagging.GetTargetElements(doc, view, option);
				//Element test = target_elements.Find(x => x.Id.Value == 1594273);
				
				// Filter out already tagged
				HashSet<ElementId> tagged_element_ids = Custom.Helpers.Tagging.GetAlreadyTagged(doc, view.Id, tagCategory);
				int index = target_elements.Count;
				while (index > 0)
				{
					index--;
					if (tagged_element_ids.Contains(target_elements[index].Id))
					{
						target_elements.RemoveAt(index);
					}
				}

				// If multihosting is enabled then group the elements by possibility of multi hosting -- distance and spatial layout
				List<List<Element>> tagging_groups = null;
				if (m_bMultiHosting)
				{
					tagging_groups = Custom.Helpers.Tagging.GroupElementsForMultiHosting(doc, view.Id, targetCategory, target_elements);
				}
				else
				{
					tagging_groups = target_elements.Select(e => new List<Element> { e }).ToList();
				}

				int nGroup = tagging_groups.Count;
				for (int i = 0; i < nGroup; i++)
				{
					int nTarget = tagging_groups[i].Count;

					if (nTarget > 1)
					{
						// Always not in this case - multi hosting tagging is not available at the moment.
					}
					else if (nTarget == 1)
					{
						Element target = tagging_groups[i][0];
						if (target == null)
						{
#if DEBUG
							Debug.Assert(false);
#endif
							continue;
						}

						//double tagAngle = 0;
						Element tagElem = null;
						switch (targetCategory)
						{
							case BuiltInCategory.OST_Walls:
								tagElem = TagToWall();
								break;
							case BuiltInCategory.OST_Rooms:
								tagElem = TagToRoom();
								break;
							case BuiltInCategory.OST_Doors:
								tagElem = TagToDoor();
								break;
							case BuiltInCategory.OST_Windows:
								tagElem = TagToWindow();
								break;
							default:
								tagElem = TagToOther();
								break;
						}

						if (tagElem == null)
						{

							pairs.Add((target.Id, ElementId.InvalidElementId));
							continue;
						}
						pairs.Add((target.Id, tagElem.Id));

						Element TagToWall()
						{
							Wall wall = target as Wall;

							//if(wall.Id.Value != 1920460)
							//{
							//return null;
							//}

							// Get Possible target curves for the wall
							// consider the view crop
							List<Curve> possible_curves = Custom.Helpers.Tagging.GetWallValidCurves(doc, view, wall, 0);// tagSizeInView.Min);

							possible_curves = Geometric.GetInboundCurvesInViewLight(possible_curves, view);
							if (possible_curves == null || possible_curves.Count < 1)
							{
								pairs.Add((wall.Id, ElementId.InvalidElementId));
							}
							possible_curves = possible_curves.OrderByDescending(c => c.Length).ToList();
							Curve maincurve = possible_curves[0];
							
							//Curve maincurve = (wall.Location as LocationCurve).Curve;

							Element wallType = doc.GetElement(wall.GetTypeId());
							bool isOuter = wallType.get_Parameter(BuiltInParameter.FUNCTION_PARAM)?.AsInteger() == (int)WallFunction.Exterior;
							
							XYZ dirNorm = Geometric.GetWallNormal(wall);
							dirNorm = dirNorm.Negate();
							double offset = 4;// leader_visible ? tagSizeInView.Min : 10;
							XYZ centerPt = maincurve.Evaluate(0.5, true);
							XYZ basePt = centerPt;// + dirNorm.Multiply(offset);
							if(!m_bAvoidOverlap)
							{
								//	Apply offset
								basePt += dirNorm.Multiply(3.0);
							}
							
							IndependentTag tag = IndependentTag.Create(doc, tagType.Id, view.Id, new Reference(target), false, orientation, basePt);

							//	Apply Avoid overlap logic
							List<APIObject> geometries = Geometric.GetElementGeometryOnView(tag, view);
							if (m_bAvoidOverlap)
							{
								BoundingBoxXYZ bb = geometries[0] as BoundingBoxXYZ;
								double width = bb.Max.X - bb.Min.X, height = bb.Max.Y - bb.Min.Y;
								XYZ pt = _VIEW_CACHE[view.Id].FindIdealPoint(basePt, width, height, "Linear", maxOffset, possible_curves, doc, view);
								tag.TagHeadPosition = pt;
								doc.Regenerate();
								geometries = Geometric.GetElementGeometryOnView(tag, view);
							}
							_VIEW_CACHE[view.Id].ApplyGeometryToMap(geometries[0]);

							//	We add the leader after the tag creation, otherwise the point goes wrong.
							tag.HasLeader = leader_visible;

							tag.TagOrientation = orientation;
							if (endType == TagEndType.AttachedEnd)
								tag.LeaderEndCondition = LeaderEndCondition.Attached;
							else if (endType == TagEndType.FreeEnd)
								tag.LeaderEndCondition = LeaderEndCondition.Free;

								return tag;
						}

						Element TagToRoom()
						{
							return TagToOther();
						}

						Element TagToDoor()
						{
							return TagToOther();
						}

						Element TagToWindow()
						{
							return TagToOther();
						}

						Element TagToOther()
						{
							BoundingBoxXYZ tagBbox = null;

							// get basepont for the leader
							// For tags without leaders, basePt is the position of the tag head. For tags with leaders, this point is the 
							// end of the leader, and a leader of default length will be created from this point to the tag head.
							(XYZ basePt, XYZ dirVec) = Custom.Helpers.Tagging.DetermineBasePoint(target, view, true, 0, isSpatial);
							
							// Tag one element and register its occupied space.
							if (isSpatial) // SpatialElementTag Creation
							{
								// TODO: get location point
								UV baseUV = new UV(basePt.X, basePt.Y);

								SpatialElement spatialElem = target as SpatialElement;
								// Spatial Type
								if (spatialElem == null)
								{
									Debug.Assert(false);
								}
								
								SpatialElementTag tag = null;
								switch (spatialElem.SpatialElementType)
								{
									case SpatialElementType.Room:
										tag = doc.Create.NewRoomTag(new LinkElementId(target.Id), baseUV, view.Id);
										break;
									case SpatialElementType.Area:
										if (view is ViewPlan plan)
										{
											tag = doc.Create.NewAreaTag(plan, (Area)target, baseUV);
										}
										break;
									case SpatialElementType.Space:
										tag = doc.Create.NewSpaceTag((Space)target, baseUV, view);
										break;
									case SpatialElementType.ElectricalLoadArea:
										// Implement dedicated logic here
										break;
									default:
										break;
								}

								//tagBbox = tag.HasLeader
								//	? Helpers.Tagging.GetTagHeadBBox(tag, view)
								//	: Geometric.SafeWorldBoundingBox(tag, view);

								//	Apply Avoid overlap logic
								List<APIObject> geometries = Geometric.GetElementGeometryOnView(tag, view);
								if (m_bAvoidOverlap)
								{
									//	Find the room boundaries
									List<Curve> boundaries = Geometric.GetBoundaryCurves(spatialElem);

									BoundingBoxXYZ bb = geometries[0] as BoundingBoxXYZ;
									double width = bb.Max.X - bb.Min.X, height = bb.Max.Y - bb.Min.Y;
									XYZ pt = _VIEW_CACHE[view.Id].FindIdealPoint(basePt, width, height, "Polygonal", maxOffset, boundaries);
									try
									{
										tag.TagHeadPosition = pt;
									}
									catch(Exception ex) {
										//	Draw debug curve
										//Curve c = Line.CreateBound(new XYZ(pt.X - 10, pt.Y, pt.Z), new XYZ(pt.X + 10, pt.Y, pt.Z));
										//doc.Create.NewDetailCurve(view, c);
										//c = Line.CreateBound(new XYZ(pt.X, pt.Y - 10, pt.Z), new XYZ(pt.X, pt.Y + 10, pt.Z));
										//doc.Create.NewDetailCurve(view, c);
									}
									doc.Regenerate();
									geometries = Geometric.GetElementGeometryOnView(tag, view);
								}
								_VIEW_CACHE[view.Id].ApplyGeometryToMap(geometries[0]);

								tag.HasLeader = leader_visible;
								if (orientation == TagOrientation.Horizontal)
									tag.TagOrientation = SpatialElementTagOrientation.Horizontal;
								else if (orientation == TagOrientation.Vertical)
									tag.TagOrientation = SpatialElementTagOrientation.Vertical;
								else
									tag.TagOrientation = SpatialElementTagOrientation.Model;

								return tag;
							}
							else // IndependentTag Creation
							{
								Reference reference = null;
								if (m_bConsiderLink && target is RevitLinkInstance linkTarget)
								{
									// Considering Link is not yet implemented.
								}
								else
								{
									reference = new Reference(target);
								}
								IndependentTag tag = IndependentTag.Create(doc, tagType.Id, view.Id, reference, false, orientation, basePt);
								if (tag == null)
									return null;

								//	Apply Avoid overlap logic
								List<APIObject> geometries = Geometric.GetElementGeometryOnView(tag, view);
								if (m_bAvoidOverlap)
								{
									BoundingBoxXYZ bb = geometries[0] as BoundingBoxXYZ;
									double width = bb.Max.X - bb.Min.X, height = bb.Max.Y - bb.Min.Y;
									XYZ pt = _VIEW_CACHE[view.Id].FindIdealPoint(basePt, width * 0.8, height * 0.8, "Radial", maxOffset, null);
									tag.TagHeadPosition = pt;
									doc.Regenerate();
									geometries = Geometric.GetElementGeometryOnView(tag, view);
								}
								_VIEW_CACHE[view.Id].ApplyGeometryToMap(geometries[0]);

								tag.HasLeader = leader_visible;

								tag.TagOrientation = orientation;
								if (endType == TagEndType.AttachedEnd)
									tag.LeaderEndCondition = LeaderEndCondition.Attached;
								else if (endType == TagEndType.FreeEnd)
									tag.LeaderEndCondition = LeaderEndCondition.Free;

								return tag;
							}
						}
					}
					else
					{
#if DEBUG
						Debug.Assert(false);
#endif
						continue;
					}
				}

				switch (endType)
				{
					case TagEndType.AttachedEnd:
						break;
					case TagEndType.FreeEnd:
						break;
					default:
#if DEBUG
						Debug.Assert(false);
#endif
						break;
				}
			}

			return pairs;
		}

		/// <summary>
		/// Draws a single-elbow leader connecting the tag to the base point without disturbing the anchor.
		/// Compatible with IndependentTag (Revit 2022+) and SpatialElementTag.
		/// </summary>
		/*public void ApplyLeaderGeometry(
			Element tag,
			XYZ xyzBase,
			View view,
			int leaderAngle = 0,
			bool alignFlag = true)
		{
			// 1) Ensure the tag has a leader
			try
			{
				if (tag is IndependentTag indTag && !indTag.HasLeader)
					indTag.HasLeader = true;
				else if (tag is SpatialElementTag spatTag && !spatTag.HasLeader)
					spatTag.HasLeader = true;
			}
			catch { /* Some tags may have read-only HasLeader * / }

			// 2) Get current tag-head position
			XYZ headXyz = null;
			try
			{
				if (tag is IndependentTag indTag)
					headXyz = indTag.TagHeadPosition;
				else if (tag is SpatialElementTag spatTag)
					headXyz = spatTag.TagHeadPosition;
			}
			catch
			{
				var bb = Custom.Helpers.Geometric.SafeWorldBoundingBox(tag, view);
				if (bb != null)
					headXyz = (bb.Min + bb.Max) / 2.0;
				else
					return;
			}
			if (headXyz == null)
				return;

			// 3) Compute the elbow position
			XYZ v = xyzBase - headXyz;
			var (rt, up) = Custom.Helpers.Geometric.ViewAxes(view);
			double dx = v.DotProduct(rt);
			double dy = v.DotProduct(up);
			double dxy = Math.Sqrt(dx * dx + dy * dy);

			XYZ elbowXyz;
			if (dxy < Custom.Helpers.Geometric.TOL)
			{
				elbowXyz = headXyz;
			}
			else if (leaderAngle == 30 || leaderAngle == 45 || leaderAngle == 90)
			{
				double ang = (leaderAngle) * Math.PI / 180.0;
				double cos = Math.Cos(ang), sin = Math.Sin(ang);
				double rx = dx * cos - dy * sin;
				double ry = dx * sin + dy * cos;
				double scale = dxy / 3.0;
				elbowXyz = new XYZ(
					headXyz.X + (rx / dxy) * scale,
					headXyz.Y + (ry / dxy) * scale,
					headXyz.Z
				);
			}
			else if (alignFlag == true)
			{
				elbowXyz = new XYZ(xyzBase.X, headXyz.Y, headXyz.Z);
			}
			else
			{
				elbowXyz = new XYZ(
					(headXyz.X + xyzBase.X) / 2.0,
					(headXyz.Y + xyzBase.Y) / 2.0,
					headXyz.Z
				);
			}

			// Optionally override elbow for overlap avoidance
			if (m_bAvoidOverlap)
				elbowXyz = xyzBase;

			// 4) Apply leader geometry
			try
			{
				if (tag is IndependentTag indTag)
				{
					IList<Reference> refs = null;
					try { refs = indTag.GetTaggedReferences(); }
					catch { }
					if (refs == null || refs.Count == 0)
						return;

					foreach (var r in refs)
					{
						indTag.SetLeaderElbow(r, elbowXyz);
						if (indTag.LeaderEndCondition == LeaderEndCondition.Free)
							indTag.SetLeaderEnd(r, xyzBase);
					}
				}
				else if (tag is SpatialElementTag spatTag)
				{
					spatTag.LeaderElbow = elbowXyz;
					spatTag.LeaderEnd = xyzBase;
				}
			}
			catch
			{
				// Swallow errors so tagging continues cleanly
			}
		}*/

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

		private void InitRectCache()
		{
			Document doc = GetDocument();
			foreach(View view in mViews)
			{
				ViewSpecificCache _cache = null;
				if (!_VIEW_CACHE.TryGetValue(view.Id, out _cache))
				{
					_cache = new ViewSpecificCache(view);
					_VIEW_CACHE[view.Id] = _cache;
				}

				//	Refresh the crop region
				_cache.Initialize();

				//	Rebuild the map
				_cache.InitCacheToAvoidOverlap(m_OverlapConsiderCategories);
			}
		}

		public override bool ProcessRequest(ArchitexorRequestId reqId)
		{
			bool bFinish = false;
			switch (reqId)
			{
				case ArchitexorRequestId.InitRectCache:
					{
						InitRectCache();
					}
					break;
				case ArchitexorRequestId.GetTaggingElements:
					{
						Selection sel = m_uiApp.ActiveUIDocument.Selection;
						Document doc = GetDocument();
						View view = doc.ActiveView;

						Transaction trans = new Transaction(doc, "Test");
						trans.Start();
						foreach (ElementId eId in sel.GetElementIds())
						{
							List<APIObject> geometries = Geometric.GetElementGeometryOnView(doc.GetElement(eId), view);

							foreach (APIObject geometry in geometries)
							{
								if (geometry is Line l)
								{
									doc.Create.NewDetailCurve(view, l);
								}
							}
						}
						trans.Commit();
					}

						break;
				case ArchitexorRequestId.ShowCache:
					{
						Document doc = GetDocument();
						ViewSpecificCache _cache = _VIEW_CACHE[doc.ActiveView.Id];

						Transaction trans = new Transaction(doc, "Add Visual Curves");
						trans.Start();
						int nGridMaxX = _cache.mGrid.Length,
							nGridMaxY = _cache.mGrid[0].Length;
						double Z = doc.ActiveView.GenLevel.Elevation;
						XYZ minPt = new XYZ(_cache.mBoundingBox.Min.X, _cache.mBoundingBox.Min.Y, Z);
						double step = _cache.CELL_SIZE;
						bool[][] grid = _cache.mGrid;
						for (int i = 0; i < nGridMaxX; i++)
						{
							for (int j = 0; j < nGridMaxY; j++)
							{
								//	Draw grid boundaries
								double x0 = minPt.X + i * step, x1 = x0 + step,
								y0 = minPt.Y + j * step, y1 = y0 + step;

								y0 -= 100;
								y1 -= 100;
								try
								{
									if (grid[i][j])
									{
										DetailCurve dc = doc.Create.NewDetailCurve(doc.ActiveView, Line.CreateBound(new XYZ(x0, y0, Z), new XYZ(x1, y1, Z)));
									}
								}
								catch (Exception ex)
								{
								}
							}
						}
						trans.Commit();
					}
					break;
				case ArchitexorRequestId.SelectTaggingGroup:
					break;
				case ArchitexorRequestId.GenerateTags:
					{
						InitRectCache();

						Document doc = GetDocument();
						Transaction trans = new Transaction(doc, "DoTagging");
						trans.Start();
						List<(string, List<(ElementId, ElementId)>)> res = DoTagging();
						trans.Commit();

						List<ElementId> generated = new List<ElementId>();
						foreach(var elems in res)
						{
							foreach(var pair in elems.Item2)
							{
								if (pair.Item2 != ElementId.InvalidElementId)
									generated.Add(pair.Item2);
							}
						}
						Selection sel = m_uiApp.ActiveUIDocument.Selection;
						sel.SetElementIds(generated);
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