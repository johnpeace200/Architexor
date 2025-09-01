using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using Architexor.Request;
using Architexor.Utils;
using Architexor.Utils.Filter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Document = Autodesk.Revit.DB.Document;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Architexor.Connect.Connectors
{
	public class HalfLap : Controller
	{
		public const string HLMarkerFamilyName = "ATX_HalfLap_Marker";
		public const string HLAFamilyName = "ATX_VoidCut_HalfLapA";
		public const string HLBFamilyName = "ATX_VoidCut_HalfLapB";
		public const string HLLSFamilyName = "ATX_VoidCut_HalfLapLS";

		/// <summary>
		/// Half Lap A Side Family
		/// </summary>
		public static Family HL = null;
		public static Family HLA = null;
		/// <summary>
		/// Half Lap B Side Family
		/// </summary>
		public static Family HLB = null;
		public static Family HLLS = null;

		/// <summary>
		/// Half Lap gap parameter
		/// </summary>
		protected int m_nDepth = 0;
		/// <summary>
		/// Half Lap Side A gap parameter
		/// </summary>
		protected float m_fSideAGap = 1.0f;
		/// <summary>
		/// Half Lap Side B gap parameter
		/// </summary>
		protected float m_fSideBGap = 1.0f;
		/// <summary>
		/// Half Lap width parameter
		/// </summary>
		protected int m_nLapWidth = 50;
		/// <summary>
		/// Half Lap offset of lap parameter
		/// </summary>
		protected float m_fGapBetweenPanels = 0.01f;
		/// <summary>
		/// Flip side
		/// </summary>
		protected bool m_bFlip = false;
		/// <summary>
		/// Flag to add horizontal half lap to Lintel & Sill bearing option
		/// </summary>
		protected bool m_bAddHorzHL = false;

		protected float m_fExcludeMinLimitLength = 0.0f;

		/// <summary>
		/// Half Lap Depth parameter
		/// </summary>
		public int Depth { get => m_nDepth; set => m_nDepth = value; }
		/// <summary>
		/// Half Lap Side A gap parameter
		/// </summary>
		public float SideAGap { get => m_fSideAGap; set => m_fSideAGap = value; }
		/// <summary>
		/// Half Lap Side B gap parameter
		/// </summary>
		public float SideBGap { get => m_fSideBGap; set => m_fSideBGap = value; }
		/// <summary>
		/// Half Lap overall width parameter
		/// </summary>
		public int LapWidth { get => m_nLapWidth; set => m_nLapWidth = value; }
		/// <summary>
		/// Half Lap offset of lap
		/// </summary>
		public float GapBetweenPanels { get => m_fGapBetweenPanels; set => m_fGapBetweenPanels = value; }
		public bool Flip { get => m_bFlip; set => m_bFlip = value; }
		public bool AddHorzHL { get => m_bAddHorzHL; set => m_bAddHorzHL = value; }

		public float ExcludeMinLimitLength { get => m_fExcludeMinLimitLength; set => m_fExcludeMinLimitLength = value; }

		public enum ConnectionLocationType
		{
			Centre = 0x00,
			PushToA = 0x01,
			PushToB = 0x02
		};
		public ConnectionLocationType ConnectionLocation { get; set; }

		//	Variables to use for internal functions
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
		protected ForgeTypeId m_UnitType = UnitTypeId.Millimeters;
#else
        /// <summary>
        /// Display Unit Type
        /// </summary>
        protected DisplayUnitType m_UnitType = DisplayUnitType.DUT_MILLIMETERS;
#endif

		protected struct OpeningBoundary
		{
			public ElementId HostId;
			public Curve Curve;
		}

		protected class HLPart
		{
			public ElementId HostId;
			public ElementId Id;
			public List<Face> Faces;
			public double Thickness;
		}

		protected class SketchLine
		{
			public Curve Curve;
			public List<HLPart> Parts;
			public List<Face> CollidingFaces;
			public List<Curve> CollidingCurves;
			public XYZ Direction;
			public int Reserved = 0;

			public void AddHLPart(HLPart hlPart, Face face, Curve curve)
			{
				int i;
				for (i = 0; i < Parts.Count; i++)
				{
					if (Parts[i].Id == hlPart.Id)
						break;
				}
				if (i == Parts.Count)
				{
					Parts.Add(hlPart);
					CollidingFaces.Add(face);
					CollidingCurves.Add(curve);
				}
			}
		}

		/// <summary>
		/// Sketch Lines
		/// </summary>
		protected List<SketchLine> m_SketchLines = new();

		public class HalfLapInstance
		{
			public Document Doc;
			public ElementId Marker;
			public ElementId A;
			public ElementId B;
			public int Flag = 0;
			public double Depth = 0.0;
			public double LapWidth = 0.0;
			public double SideAGap = 0.0;
			public double SideBGap = 0.0;
			public double GapBetweenPanels = 0.0;
			public double EndA = 0.0;
			public double EndB = 0.0;
			public double Length = 0.0;
			public bool Flip;
			public ConnectionLocationType ConnectionLocation;
		};
		private static readonly List<HalfLapInstance> m_HLInstances = new();

		public class HalfLapLSInstance
		{
			public Document Doc;
			public ElementId Id;
			public Curve Location;
		}
		private static readonly List<HalfLapLSInstance> m_HLLSInstances = new();

		/// <summary>
		/// Initialize and prepare the tool
		/// </summary>
		public override void Initialize()
		{
			int i, j;
			Document doc = m_uiApp.ActiveUIDocument.Document;

			//  Initialize Connector controller
			if (!CheckHLFamily())
			{
				return;
			}

			//  Get Part and their parent elements
			m_SketchLines.Clear();

			List<ElementId> hostIds = new();

			List<HLPart> parts = new();
			//	m_Elements is the array of selected parts
			//	The tool gets extra information(host id, face list and thickness of the part) from them and add to hlParts array
			//	We exclude some unnecessary faces which can be ignored from the calculation. (Exterior/interior faces of walls, Top and bottom faces of slabs)
			for (i = 0; i < m_Elements.Count; i++)
			{
				Element host = Util.GetSourceElementOfPart(doc, m_Elements[i] as Part);
				if (!hostIds.Contains(host.Id))
					hostIds.Add(host.Id);

				HLPart part = new()
				{
					HostId = host.Id,
					Id = m_Elements[i].Id,
					Thickness = Util.GetThicknessOfPart(doc, m_Elements[i] as Part),
					Faces = new List<Face>()
				};

				List<Face> excludeFaces = new();
				if (host is Wall)
				{
					foreach (Reference refer in HostObjectUtils.GetSideFaces(host as HostObject, ShellLayerType.Exterior))
					{
						GeometryObject go = host.GetGeometryObjectFromReference(refer) as Face;
						if (go != null && go is Face)
							excludeFaces.Add(go as Face);
					}
					foreach (Reference refer in HostObjectUtils.GetSideFaces(host as HostObject, ShellLayerType.Interior))
					{
						GeometryObject go = host.GetGeometryObjectFromReference(refer) as Face;
						if (go != null && go is Face)
							excludeFaces.Add(go as Face);
					}
				}
				else
				{
					foreach (Reference refer in HostObjectUtils.GetTopFaces(host as HostObject))
					{
						GeometryObject go = host.GetGeometryObjectFromReference(refer) as Face;
						if (go != null && go is Face)
							excludeFaces.Add(go as Face);
					}
					foreach (Reference refer in HostObjectUtils.GetBottomFaces(host as HostObject))
					{
						GeometryObject go = host.GetGeometryObjectFromReference(refer) as Face;
						if (go != null && go is Face)
							excludeFaces.Add(go as Face);
					}
				}

				GeometryElement geomElem = m_Elements[i].get_Geometry(new Options());
				foreach (GeometryObject geomObject in geomElem)
				{
					if (geomObject is Solid)
					{
						Solid solid = geomObject as Solid;
						FaceArray faceArray = solid.Faces;
						foreach (Face f in faceArray)
						{
							if (f is PlanarFace)
							{
								for (j = 0; j < excludeFaces.Count; j++)
								{
									if (Util.IsEqual((f as PlanarFace).FaceNormal, (excludeFaces[j] as PlanarFace).FaceNormal)
										|| Util.IsEqual((f as PlanarFace).FaceNormal, -(excludeFaces[j] as PlanarFace).FaceNormal))
									{
										break;
									}
								}
								if (j == excludeFaces.Count)
									part.Faces.Add(f);
							}
						}
					}
				}
				parts.Add(part);
			}

			//	Find intersections between parts
			try
			{
				for (i = 0; i < parts.Count; i++)
				{
					Part part1 = doc.GetElement(parts[i].Id) as Part;
					Face baseFace1 = Util.GetBaseFaceOfPart(doc, part1);

					for (j = i + 1; j < parts.Count; j++)
					{
						Part part2 = doc.GetElement(parts[j].Id) as Part;
						Face baseFace2 = Util.GetBaseFaceOfPart(doc, part2);

						foreach (Face f1 in parts[i].Faces)
						{
							foreach (Face f2 in parts[j].Faces)
							{
								if (Util.AbutWith(f1, f2))
								{
									int nMainPart = i;
									//	The algorithm is based on the thicker part
									if (!Util.IsEqual(parts[i].Thickness, parts[j].Thickness))
									{
										if (parts[i].Thickness > parts[j].Thickness)
											nMainPart = j;
									}

									FaceIntersectionFaceResult fifr = f1.Intersect(baseFace1, out Curve result1);
									fifr = f2.Intersect(baseFace2, out Curve result2);
									Plane plane = (nMainPart == i) ? baseFace1.GetSurface() as Plane : baseFace2.GetSurface() as Plane;

									if (Util.IsEqual(plane.ProjectOnto(result1.GetEndPoint(0)), plane.ProjectOnto(result1.GetEndPoint(1)))) continue;
									result1 = Line.CreateBound(plane.ProjectOnto(result1.GetEndPoint(0)), plane.ProjectOnto(result1.GetEndPoint(1)));
									if (Util.IsEqual(plane.ProjectOnto(result2.GetEndPoint(0)), plane.ProjectOnto(result2.GetEndPoint(1)))) continue;
									result2 = Line.CreateBound(plane.ProjectOnto(result2.GetEndPoint(0)), plane.ProjectOnto(result2.GetEndPoint(1)));

									AddSketchLine(result1, result2, f1, f2, parts[i], parts[j]);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				TaskDialog.Show("Error", ex.Message);
			}

			//	Check openings to determine Lintel & Sill options
			List<OpeningBoundary> openingBoundaries = new();
			foreach (ElementId eId in hostIds)
			{
				Element elem = doc.GetElement(eId);
				List<ElementId> openings = GetAssociatedElementIds(elem);

				List<PlanarFace> faceList = new();

				List<Solid> solidList = new();

				Options geomOptions = elem.Document.Application.Create.NewGeometryOptions();

				if (geomOptions != null)
				{
					GeometryElement geoElem = elem.get_Geometry(geomOptions);

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
							if (elem.GetGeneratingElementIds(face)
								.Any(x => openings.Contains(x)))
							{
								faceList.Add(face as PlanarFace);
							}
						}
					}
				}

				List<Face> baseFaces = Util.GetBaseFacesOfElement(doc, elem);
				foreach (PlanarFace pf in faceList)
				{
					foreach (CurveLoop cl in pf.GetEdgesAsCurveLoops())
					{
						foreach (Curve c in cl)
						{
							if (c is Line)
							{
								foreach (Face baseFace in baseFaces)
								{
									Plane plane = baseFace.GetSurface() as Plane;
									if (Util.IsZero(plane.SignedDistanceTo(c.GetEndPoint(0)))
										&& Util.IsZero(plane.SignedDistanceTo(c.GetEndPoint(1))))
									{
										openingBoundaries.Add(new OpeningBoundary() { HostId = eId, Curve = c });
										break;
									}
								}
							}
						}
					}
				}
			}

			for (i = 0; i < m_SketchLines.Count; i++)
			{
				SketchLine sl = m_SketchLines[i];
				if (sl.Parts.Count != 2) continue;
				//				if (sl.HLParts[0].HostId != sl.HLParts[1].HostId) continue;

				Part part1 = doc.GetElement(sl.Parts[0].Id) as Part,
					part2 = doc.GetElement(sl.Parts[1].Id) as Part;
				PlanarFace baseFace1 = Util.GetBaseFaceOfPart(doc, part1) as PlanarFace,
							baseFace2 = Util.GetBaseFaceOfPart(doc, part2) as PlanarFace;
				List<Curve> boundary1 = Util.GetOptimizedBoundaryCurves(baseFace1),
							boundary2 = Util.GetOptimizedBoundaryCurves(baseFace2);
				Element parent = doc.GetElement(sl.Parts[0].HostId);

				bool bFirstCan = boundary1.Count == 4
					, bSecondCan = boundary2.Count == 4;
				if (!bFirstCan && !bSecondCan)
					continue;

				List<Curve> parentBoundary = new();
				foreach (Face f in Util.GetBaseFacesOfElement(doc, parent))
				{
					parentBoundary.AddRange(Util.GetOptimizedBoundaryCurves(f));
				}
				for (j = parentBoundary.Count - 1; j >= 0; j--)
				{
					foreach (OpeningBoundary ob in openingBoundaries)
					{
						if (ob.HostId == parent.Id
							&& ob.Curve.Intersect(parentBoundary[j]) == SetComparisonResult.Equal)
						{
							parentBoundary.RemoveAt(j);
							break;
						}
					}
				}

				Curve horzCurve1 = null
					, horzCurve2 = null;
				for (j = boundary1.Count - 1; j >= 0; j--)
				{
					foreach (OpeningBoundary ob in openingBoundaries)
					{
						if (ob.HostId == parent.Id)
						{
							if (ob.Curve.Intersect(boundary1[j]) == SetComparisonResult.Equal)
							{
								horzCurve1 = boundary1[j];
								break;
							}
						}
					}

					foreach (Curve curve in parentBoundary)
					{
						SetComparisonResult scr = curve.Intersect(boundary1[j]);
						if (scr == SetComparisonResult.Equal)
						{
							boundary1.RemoveAt(j);
							break;
						}
					}
				}

				for (j = boundary2.Count - 1; j >= 0; j--)
				{
					foreach (OpeningBoundary ob in openingBoundaries)
					{
						if (ob.HostId == parent.Id)
						{
							if (ob.Curve.Intersect(boundary2[j]) == SetComparisonResult.Equal)
							{
								horzCurve2 = boundary2[j];
								break;
							}
						}
					}

					foreach (Curve curve in parentBoundary)
					{
						SetComparisonResult scr = curve.Intersect(boundary2[j]);
						if (scr == SetComparisonResult.Equal)
						{
							boundary2.RemoveAt(j);
							break;
						}
					}
				}

				if (boundary1.Count != 3 && boundary2.Count != 3) continue;
				Part part;
				List<Curve> boundary;
				PlanarFace pf;
				Curve horzCurve;
				if (boundary1.Count == 3 && bFirstCan && horzCurve1 != null)
				{
					part = part1;
					boundary = boundary1;
					pf = baseFace1;
					horzCurve = horzCurve1;
				}
				else if (boundary2.Count == 3 && bSecondCan && horzCurve2 != null)
				{
					part = part2;
					boundary = boundary2;
					pf = baseFace2;
					horzCurve = horzCurve2;
				}
				else
				{
					continue;
				}

				if (sl.Curve.Intersect(horzCurve) == SetComparisonResult.Equal)
				{
					m_SketchLines.RemoveAt(i);
					i--;
					continue;
				}

				if (Util.GetDirectionOfPart(doc, sl.Curve, part) > 0)
					sl.Reserved = 1;
				else
					sl.Reserved = 2;
			}

			//	For walls
			//	We decide all horizontal curves are the horizontal curves of Lintel & Sill option
			for (i = 0; i < m_SketchLines.Count; i++)
			{
				SketchLine sl = m_SketchLines[i];
				if (sl.Parts.Count != 2) continue;
				//				if (sl.HLParts[0].HostId != sl.HLParts[1].HostId) continue;

				Part part1 = doc.GetElement(sl.Parts[0].Id) as Part,
					part2 = doc.GetElement(sl.Parts[1].Id) as Part;
				PlanarFace baseFace1 = Util.GetBaseFaceOfPart(doc, part1) as PlanarFace,
							baseFace2 = Util.GetBaseFaceOfPart(doc, part2) as PlanarFace;
				List<Curve> boundary1 = Util.GetOptimizedBoundaryCurves(baseFace1),
							boundary2 = Util.GetOptimizedBoundaryCurves(baseFace2);
				Element parent1 = doc.GetElement(sl.Parts[0].HostId)
					, parent2 = doc.GetElement(sl.Parts[1].HostId);

				if (!(parent1 is Wall) || !(parent2 is Wall))
					continue;

				if (Util.IsEqual(sl.Curve.GetEndPoint(0).Z, sl.Curve.GetEndPoint(1).Z))
				{
					//	Find the middle part
					Part part = part1;
					if (sl.CollidingCurves[0].Length < sl.CollidingCurves[1].Length)
						part = part2;

					for (j = 0; j < m_SketchLines.Count; j++)
					{
						if (j == i) continue;

						if (m_SketchLines[j].Curve.Intersect(sl.Curve, out IntersectionResultArray ira) == SetComparisonResult.Overlap)
						{
							XYZ pt = ira.get_Item(0).XYZPoint;
							if (Util.IsEqual(pt, m_SketchLines[j].Curve.GetEndPoint(0))
								|| Util.IsEqual(pt, m_SketchLines[j].Curve.GetEndPoint(1)))
							{
								if (Util.GetDirectionOfPart(doc, m_SketchLines[j].Curve, part) > 0)
									m_SketchLines[j].Reserved = 1;
								else
									m_SketchLines[j].Reserved = 2;
							}
						}
					}

					m_SketchLines.RemoveAt(i);
					i--;
				}
			}

			for (i = 0; i < m_SketchLines.Count; i++)
			{
				if (m_SketchLines[i].Parts.Count != 2) continue;

				for (j = i + 1; j < m_SketchLines.Count; j++)
				{
					if (m_SketchLines[j].Parts.Count != 2) continue;

					if (!Util.IsEqual(m_SketchLines[i].Curve.GetEndPoint(0).Z, m_SketchLines[j].Curve.GetEndPoint(0).Z)
						|| !Util.IsEqual(m_SketchLines[i].Curve.GetEndPoint(1).Z, m_SketchLines[j].Curve.GetEndPoint(1).Z))
						continue;

					ElementId mainId;
					if (m_SketchLines[i].Parts[0].Id == m_SketchLines[j].Parts[0].Id
						|| m_SketchLines[i].Parts[0].Id == m_SketchLines[j].Parts[1].Id)
						mainId = m_SketchLines[i].Parts[0].Id;
					else if (m_SketchLines[i].Parts[1].Id == m_SketchLines[j].Parts[0].Id
						|| m_SketchLines[i].Parts[1].Id == m_SketchLines[j].Parts[1].Id)
						mainId = m_SketchLines[i].Parts[1].Id;
					else
						continue;

					Curve curve1, curve2;
					try
					{
						curve1 = Line.CreateBound(m_SketchLines[i].Curve.GetEndPoint(0), m_SketchLines[j].Curve.GetEndPoint(0));
						curve2 = Line.CreateBound(m_SketchLines[i].Curve.GetEndPoint(1), m_SketchLines[j].Curve.GetEndPoint(1));
					}
					catch (Exception) { continue; }

					bool bIntersect = false;
					foreach (OpeningBoundary ob in openingBoundaries)
					{
						if (curve1.Intersect(ob.Curve) == SetComparisonResult.Equal
							|| curve2.Intersect(ob.Curve) == SetComparisonResult.Equal)
						{
							bIntersect = true;
							break;
						}
					}
					if (bIntersect)
					{
						if (Util.GetDirectionOfPart(doc, m_SketchLines[j].Curve, doc.GetElement(mainId) as Part) > 0)
							m_SketchLines[j].Reserved = 1;
						else
							m_SketchLines[j].Reserved = 2;
					}
				}
			}

			//	Debug
			//	Show Sketch Lines
#if DEBUG
			Transaction trans = new(doc);
			trans.Start("Show Sketch lines");
			List<ElementId> mcids = new();
			foreach (SketchLine sl in m_SketchLines)
			{
				/*if (sl is HorizontalSketchLineOfLS)
				{
					HorizontalSketchLineOfLS hsl = sl as HorizontalSketchLineOfLS;
					ModelCurve mc = doc.Create.NewModelCurve(hsl.Curve, SketchPlane.Create(doc, Plane.CreateByThreePoints(hsl.Curve.GetEndPoint(0), hsl.Curve.GetEndPoint(1), new XYZ(0, 0, 0))));
					Color color = new Color(255, 0, 0);
					OverrideGraphicSettings ogs = new OverrideGraphicSettings();
					ogs.SetProjectionLineColor(color);
					ogs.SetProjectionLineWeight(16);
					doc.ActiveView.SetElementOverrides(mc.Id, ogs);
					mcids.Add(mc.Id);

					mc = doc.Create.NewModelCurve(hsl.LeftSketchLine.Curve, SketchPlane.Create(doc, Plane.CreateByThreePoints(hsl.LeftSketchLine.Curve.GetEndPoint(0), hsl.LeftSketchLine.Curve.GetEndPoint(1), new XYZ(0, 0, 0))));
					color = new Color(0, 0, 255);
					ogs = new OverrideGraphicSettings();
					ogs.SetProjectionLineColor(color);
					ogs.SetProjectionLineWeight(16);
					doc.ActiveView.SetElementOverrides(mc.Id, ogs);
					mcids.Add(mc.Id);

					mc = doc.Create.NewModelCurve(hsl.RightSketchLine.Curve, SketchPlane.Create(doc, Plane.CreateByThreePoints(hsl.RightSketchLine.Curve.GetEndPoint(0), hsl.RightSketchLine.Curve.GetEndPoint(1), new XYZ(0, 0, 0))));
					color = new Color(0, 0, 128);
					ogs = new OverrideGraphicSettings();
					ogs.SetProjectionLineColor(color);
					ogs.SetProjectionLineWeight(16);
					doc.ActiveView.SetElementOverrides(mc.Id, ogs);
					mcids.Add(mc.Id);
				}
				else
				{
					ModelCurve mc = doc.Create.NewModelCurve(sl.Curve, SketchPlane.Create(doc, Plane.CreateByThreePoints(sl.Curve.GetEndPoint(0), sl.Curve.GetEndPoint(1), new XYZ(0, 0, 0))));
					mcids.Add(mc.Id);
				}*/
				ModelCurve mc = doc.Create.NewModelCurve(sl.Curve, SketchPlane.Create(doc, Plane.CreateByThreePoints(sl.Curve.GetEndPoint(0), sl.Curve.GetEndPoint(1), new XYZ(0, 0, 0))));
				mcids.Add(mc.Id);
			}
			doc.ActiveView.IsolateElementsTemporary(mcids);
			trans.Commit();
#endif
		}

		private bool AddSketchLine(Curve result1, Curve result2, Face f1, Face f2, HLPart part1, HLPart part2)
		{
			try
			{
				int k;

				XYZ direction = (result1.GetEndPoint(1) - result1.GetEndPoint(0)).Normalize();
				if (Util.IsEqual(direction, new XYZ(0, 0, -1)))
					direction = new XYZ(0, 0, 1);
				//	Check if the same direction is already exist
				for (k = 0; k < m_SketchLines.Count; k++)
				{
					if (Util.IsEqual(m_SketchLines[k].Direction, direction)
						|| Util.IsEqual(m_SketchLines[k].Direction, -direction))
					{
						direction = m_SketchLines[k].Direction;
						break;
					}
				}

				if (!Util.IsEqual(direction, (result1.GetEndPoint(1) - result1.GetEndPoint(0)).Normalize()))
				{
					result1 = result1.CreateReversed();
				}
				if (!Util.IsEqual(direction, (result2.GetEndPoint(1) - result2.GetEndPoint(0)).Normalize()))
				{
					result2 = result2.CreateReversed();
				}
				if (Util.IsEqual(result1.GetEndPoint(0), result2.GetEndPoint(1))
					|| Util.IsEqual(result1.GetEndPoint(1), result2.GetEndPoint(0)))
				{
					return false;
				}

				XYZ ptStart, ptEnd;
				if (Util.IsEqual(result1.GetEndPoint(0), result2.GetEndPoint(0)))
					ptStart = result1.GetEndPoint(0);
				else if (Util.IsEqual(direction, (result2.GetEndPoint(0) - result1.GetEndPoint(0)).Normalize()))
				{
					ptStart = result2.GetEndPoint(0);
				}
				else
					ptStart = result1.GetEndPoint(0);
				if (Util.IsEqual(result1.GetEndPoint(1), result2.GetEndPoint(1)))
					ptEnd = result1.GetEndPoint(1);
				else if (Util.IsEqual(direction, (result2.GetEndPoint(1) - result1.GetEndPoint(1)).Normalize()))
				{
					ptEnd = result1.GetEndPoint(1);
				}
				else
					ptEnd = result2.GetEndPoint(1);
				try
				{
					Line.CreateBound(ptStart, ptEnd);
				}
				catch (Exception) { return false; }

				//	We check all previous sketch lines which are added before if this one is linked to another, if so, we don't add new one, but change the information of the previous one.
				int nStart = -1, nEnd = -1;
				for (k = 0; k < m_SketchLines.Count; k++)
				{
					SketchLine sl = m_SketchLines[k];
					if (Util.IsEqual(sl.Direction, (ptEnd - ptStart).Normalize()))
					{
						if (Util.IsEqual(sl.Curve.GetEndPoint(0), ptEnd))
						{
							nEnd = k;
						}
						else if (Util.IsEqual(sl.Curve.GetEndPoint(1), ptStart))
						{
							nStart = k;
						}
					}
				}

				if (nStart < 0 && nEnd < 0)
				{
					SketchLine sl = new()
					{
						Parts = new List<HLPart>() {
												part1, part2
											},
						CollidingFaces = new List<Face>() { f1, f2 },
						CollidingCurves = new List<Curve>() { result1, result2 },
						Curve = Line.CreateBound(ptStart, ptEnd),
						Direction = (ptEnd - ptStart).Normalize()
					};
					//	We ignore the case which is shorter than the limit
					if (sl.Curve.Length > UnitUtils.ConvertToInternalUnits(m_fExcludeMinLimitLength, m_UnitType))
						m_SketchLines.Add(sl);
				}
				else if (nStart < 0 || nEnd < 0)
				{
					if (nStart >= 0)
					{
						m_SketchLines[nStart].Curve = Line.CreateBound(m_SketchLines[nStart].Curve.GetEndPoint(0), ptEnd);
						m_SketchLines[nStart].AddHLPart(part1, f1, result1);
						m_SketchLines[nStart].AddHLPart(part2, f2, result2);
					}
					else
					{
						m_SketchLines[nEnd].Curve = Line.CreateBound(ptStart, m_SketchLines[nEnd].Curve.GetEndPoint(1));
						m_SketchLines[nEnd].AddHLPart(part1, f1, result1);
						m_SketchLines[nEnd].AddHLPart(part2, f2, result2);
					}
				}
				else
				{
					m_SketchLines[nStart].Curve = Line.CreateBound(m_SketchLines[nStart].Curve.GetEndPoint(0), m_SketchLines[nEnd].Curve.GetEndPoint(1));
					for (k = 0; k < m_SketchLines[nEnd].Parts.Count; k++)
					{
						m_SketchLines[nStart].AddHLPart(m_SketchLines[nEnd].Parts[k], m_SketchLines[nEnd].CollidingFaces[k], m_SketchLines[nEnd].CollidingCurves[k]);
					}

					m_SketchLines[nStart].AddHLPart(part1, f1, result1);
					m_SketchLines[nStart].AddHLPart(part2, f2, result2);
					m_SketchLines.RemoveAt(nEnd);
				}
			}
			catch (Exception) { return false; }

			return true;
		}

		private List<ElementId> GetAssociatedElementIds(Element elem)
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			//	Family Instance
			IEnumerable<Element> associatedElements = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
				.Where(m =>
					(m as FamilyInstance).Host != null && (m as FamilyInstance).Host.Id == elem.Id
				);
			List<ElementId> openings = new();

			foreach (Element e in associatedElements)
			{
				openings.Add(e.Id);
			}

			//	Opening
			associatedElements = new FilteredElementCollector(doc).OfClass(typeof(Opening))
				.Where(m =>
					(m as Opening).Host != null && (m as Opening).Host.Id == elem.Id
				);

			foreach (Element e in associatedElements)
			{
				openings.Add(e.Id);
			}
			return openings;
		}

		private void GetConnections()
		{
			int i;

			string sError = "";

			//  Create instances by Sketch Lines
			for (i = 0; i < m_SketchLines.Count; i++)
			{
				if (m_SketchLines[i].Reserved == 1)
				{
					m_bFlip = !m_bFlip;

					if (ConnectionLocation == ConnectionLocationType.PushToA)
						ConnectionLocation = ConnectionLocationType.PushToB;
					else if (ConnectionLocation == ConnectionLocationType.PushToB)
						ConnectionLocation = ConnectionLocationType.PushToA;

					float fTemp = m_fSideAGap;
					m_fSideAGap = m_fSideBGap;
					m_fSideBGap = fTemp;
				}
				string sErr = CreateHLInstance(m_SketchLines[i]);
				if (sErr != "")
					sError += sErr + "\n";
				if (m_SketchLines[i].Reserved == 1)
				{
					m_bFlip = !m_bFlip;

					if (ConnectionLocation == ConnectionLocationType.PushToA)
						ConnectionLocation = ConnectionLocationType.PushToB;
					else if (ConnectionLocation == ConnectionLocationType.PushToB)
						ConnectionLocation = ConnectionLocationType.PushToA;

					float fTemp = m_fSideAGap;
					m_fSideAGap = m_fSideBGap;
					m_fSideBGap = fTemp;
				}
			}
			if (sError != "")
				TaskDialog.Show("Error", sError);
		}

		/// <summary>
		/// Process request handler from FrmConnectSettings
		/// </summary>
		public override bool ProcessRequest(ArchitexorRequestId reqId)
		{
			try
			{
				switch (reqId)
				{
					case ArchitexorRequestId.SelectParentElements:
						SelectElements();
						return false;
					case ArchitexorRequestId.ArrangeConnectors:
						if (HL == null || HLA == null || HLB == null || HLLS == null)
						{
							TaskDialog.Show("Error", "Can't find the family. Please contact developer.");
							return false;
						}

						Document doc = m_uiApp.ActiveUIDocument.Document;
						TransactionGroup tg = new(doc);

						tg.Start("Generate HLs");
						GetConnections();
						tg.Assimilate();
						return true;
					default:
						break;
				}
				return true;
			}
			catch (Exception _)
			{
				TaskDialog.Show("Error", _.Message);
				return false;
			}
		}

		private void SelectElements()
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			Selection sel = m_uiApp.ActiveUIDocument.Selection;

			//  Select a wall
			try
			{
				GlobalHook.Subscribe();
				PartPickFilter selFilter = new(doc);
				IList<Reference> refers = sel.PickObjects(ObjectType.Element, selFilter, "Please select parts.");
				GlobalHook.Unsubscribe();

				if (refers.Count == 0)
					return;
				//	return Result.Cancelled;

				m_Elements.Clear();
				foreach (Reference refer in refers)
				{
					Element elem = doc.GetElement(refer);
					m_Elements.Add(elem);
				}

				Initialize();
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		protected string CreateHLInstance(SketchLine sl)
		{
			#region Convert Unit of the input parameters
			double fSideAGap = UnitUtils.ConvertToInternalUnits(m_fSideAGap, m_UnitType);
			double fSideBGap = UnitUtils.ConvertToInternalUnits(m_fSideBGap, m_UnitType);
			double fLapWidth = UnitUtils.ConvertToInternalUnits(m_nLapWidth, m_UnitType);
			double fDepth = UnitUtils.ConvertToInternalUnits(m_nDepth, m_UnitType);
			double fGap = UnitUtils.ConvertToInternalUnits(m_fGapBetweenPanels, m_UnitType);
			#endregion

			Document doc = m_uiApp.ActiveUIDocument.Document;
			Transaction trans = new(doc);
			trans.Start("Creating HLs");

			FamilyInstance fiHL, fiHLA, fiHLB;

			try
			{
				Face face = Util.GetBaseFaceOfPart(doc, doc.GetElement(sl.Parts[0].Id) as Part);
				XYZ normal = null;
				if (face is PlanarFace)
				{
					normal = (face as PlanarFace).FaceNormal;
				}
				Curve location;

				//	Adjust the location
				//double margin = UnitUtils.ConvertToInternalUnits(100, m_UnitType);
				XYZ ptStart = sl.Curve.GetEndPoint(0),
					ptEnd = sl.Curve.GetEndPoint(1);
				//if(sl.Reserved != 1)
				//{
				//	ptStart-= (ptEnd - ptStart).Normalize() * margin;
				//}
				//if(sl.Reserved != 2)
				//{
				//	ptEnd += (ptEnd - ptStart).Normalize() * margin;
				//}
				location = Line.CreateBound(ptStart, ptEnd);

				XYZ direction = (ptEnd - ptStart).Normalize();

				//	Check for other HLs and Adjust Location (For Lintel & Sill option)
				bool bMoved = false;
				IntersectionResultArray ira = null;
				for (int i = 0; i < sl.Parts.Count; i++)
				{
					Part part = doc.GetElement(sl.Parts[i].Id) as Part;
					List<ElementId> elemIds = (List<ElementId>)InstanceVoidCutUtils.GetCuttingVoidInstances(part);
					foreach (ElementId elemId in elemIds)
					{
						FamilyInstance fi = doc.GetElement(elemId) as FamilyInstance;
						if (fi.Name != HLAFamilyName && fi.Name != HLBFamilyName)
							continue;
						LocationPoint filoc = fi.Location as LocationPoint;
						double fLength = fi.LookupParameter("Length").AsDouble();
						XYZ pt1 = filoc.Point - fi.FacingOrientation * fLength / 2
							, pt2 = filoc.Point + fi.FacingOrientation * fLength / 2;
						Line line = Line.CreateBound(pt1, pt2);
						if (line.Intersect(location, out ira) == SetComparisonResult.Overlap)
						{
							XYZ pt = ira.get_Item(0).XYZPoint;
							if (Util.IsEqual(ptStart, pt))
							{
								ptStart -= direction.Normalize() * fi.LookupParameter("LapWidth").AsDouble();
							}
							else if (Util.IsEqual(ptEnd, pt))
							{
								ptEnd += direction.Normalize() * fi.LookupParameter("LapWidth").AsDouble();
							}
							else if (Util.IsEqual(pt, pt1))
							{
								fi.LookupParameter("Length").Set(fi.LookupParameter("Length").AsDouble() + fLapWidth);
								ElementTransformUtils.MoveElement(doc, fi.Id, -fi.FacingOrientation * fLapWidth / 2);
								bMoved = true;
							}
							else if (Util.IsEqual(pt, pt2))
							{
								fi.LookupParameter("Length").Set(fi.LookupParameter("Length").AsDouble() + fLapWidth);
								ElementTransformUtils.MoveElement(doc, fi.Id, fi.FacingOrientation * fLapWidth / 2);
								bMoved = true;
							}
						}
					}
				}
				if (bMoved)
				{
					face = Util.GetBaseFaceOfPart(doc, doc.GetElement(sl.Parts[0].Id) as Part);
				}
				location.Dispose();
				location = Line.CreateBound(ptStart, ptEnd);

				direction = direction.CrossProduct(normal).Normalize();

				//	Get the thicker thickness
				double fThickness = sl.Parts[0].Thickness;
				foreach (HLPart hlPart in sl.Parts)
				{
					if (hlPart.Thickness > fThickness)
					{
						fThickness = hlPart.Thickness;
						face = Util.GetBaseFaceOfPart(doc, doc.GetElement(hlPart.Id) as Part);
					}
				}

				//	Half Lap
				double fAngle1 = -10.0, fAngle2 = -10.0;

				foreach (HLPart hlPart in sl.Parts)
				{
					Part part = doc.GetElement(hlPart.Id) as Part;
					int nDirection = Util.GetDirectionOfPart(doc, location, part);
					if (nDirection < 0) continue;
					foreach (Curve c in Util.GetOptimizedBoundaryCurves(Util.GetBaseFaceOfPart(doc, part)))
					{
						if (c.Intersect(location, out ira) == SetComparisonResult.Overlap)
						{
							XYZ intersectPt = ira.get_Item(0).XYZPoint;
							if (fAngle2 == -10.0
								&& Util.IsEqual(intersectPt, location.GetEndPoint(0)))
							{
								XYZ vector = c.GetEndPoint(1) - c.GetEndPoint(0);
								fAngle2 = vector.AngleTo(direction);
								XYZ otherPt = c.GetEndPoint(0);
								if (Util.IsEqual(otherPt, intersectPt)) otherPt = c.GetEndPoint(1);
								if (Util.IsEqual(location.Project(otherPt).XYZPoint, location.GetEndPoint(1)))
								{
									fAngle2 = Math.PI - fAngle1;
								}
							}
							else if (fAngle1 == -10.0
								&& Util.IsEqual(intersectPt, location.GetEndPoint(1)))
							{
								XYZ vector = c.GetEndPoint(1) - c.GetEndPoint(0);
								fAngle1 = vector.AngleTo(direction);
								XYZ otherPt = c.GetEndPoint(0);
								if (Util.IsEqual(otherPt, intersectPt)) otherPt = c.GetEndPoint(1);
								if (Util.IsEqual(location.Project(otherPt).XYZPoint, location.GetEndPoint(1)))
								{
									fAngle1 = Math.PI - fAngle1;
								}
							}
						}
					}
				}

				if (fAngle1 == -10.0 || Util.IsEqual(fAngle1, Math.PI) || Util.IsZero(fAngle1)) fAngle1 = 0.0;
				if (fAngle2 == -10.0 || Util.IsEqual(fAngle2, Math.PI) || Util.IsZero(fAngle2)) fAngle2 = 0.0;
				location = Line.CreateBound(
					location.GetEndPoint(0) - (location.GetEndPoint(1) - location.GetEndPoint(0)).Normalize() * fLapWidth * 2 * Math.Abs(Math.Tan(fAngle2)),
					location.GetEndPoint(1) + (location.GetEndPoint(1) - location.GetEndPoint(0)).Normalize() * fLapWidth * 2 * Math.Abs(Math.Tan(fAngle1))
					);

				XYZ pos = (location.GetEndPoint(1) + location.GetEndPoint(0)) / 2;
				FamilyInstance instance = doc.Create.NewFamilyInstance(
					face
					, pos
					, direction
					, GetFamilySymbol(HL));

				Parameter param = instance.LookupParameter("Gap");
				param.Set(fGap);
				param = instance.LookupParameter("Location");
				param.Set((int)ConnectionLocation);
				param = instance.LookupParameter("Thickness");
				param.Set(fThickness);
				param = instance.LookupParameter("Depth");
				if (fDepth >= 0)
					param.Set(fDepth);
				else
					param.Set(fThickness / 2);
				param = instance.LookupParameter("LapWidth");
				param.Set(fLapWidth);
				param = instance.LookupParameter("SideAGap");
				param.Set(fSideAGap);
				param = instance.LookupParameter("SideBGap");
				param.Set(fSideBGap);
				param = instance.LookupParameter("Flip");
				param.Set(0);
				param = instance.LookupParameter("HL Length");
				param.Set(location.Length);
				fiHL = instance;

				instance = doc.Create.NewFamilyInstance(
					face
					, pos
					, direction
					, GetFamilySymbol(HLA));

				param = instance.LookupParameter("Gap");
				param.Set(fGap);
				param = instance.LookupParameter("Offset");
				param.Set(GetOffset());
				param = instance.LookupParameter("Thickness");
				param.Set(fThickness);
				param = instance.LookupParameter("Depth");
				if (fDepth >= 0)
					param.Set(fDepth);
				else
					param.Set(fThickness / 2);
				param = instance.LookupParameter("LapWidth");
				param.Set(fLapWidth);
				param = instance.LookupParameter("Length");
				param.Set(location.Length);
				param = instance.LookupParameter("SideAGap");
				param.Set(fSideAGap);
				//param = instance.LookupParameter("Angle2");
				//param.Set(fAngle2);
				//param = instance.LookupParameter("Angle1");
				//param.Set(fAngle1);
				param = instance.LookupParameter("SketchLine");
#if REVIT2024 || REVIT2025
				param.Set(fiHL.Id.Value);
#else
                param.Set(fiHL.Id.IntegerValue);
#endif
				fiHLA = instance;

				instance = doc.Create.NewFamilyInstance(
					face
					, pos
					, direction
					, GetFamilySymbol(HLB));

				param = instance.LookupParameter("Gap");
				param.Set(fGap);
				param = instance.LookupParameter("Offset");
				param.Set(GetOffset());
				param = instance.LookupParameter("Thickness");
				param.Set(fThickness);
				param = instance.LookupParameter("Depth");
				if (fDepth >= 0)
					param.Set(fDepth);
				else
					param.Set(fThickness / 2);
				param = instance.LookupParameter("LapWidth");
				param.Set(fLapWidth);
				param = instance.LookupParameter("Length");
				param.Set(location.Length);
				param = instance.LookupParameter("SideBGap");
				param.Set(fSideBGap);
				//param = instance.LookupParameter("Angle2");
				//param.Set(fAngle2);
				//param = instance.LookupParameter("Angle1");
				//param.Set(fAngle1);
				param = instance.LookupParameter("SketchLine");
#if REVIT2024 || REVIT2025
				param.Set(fiHL.Id.Value);
#else
                param.Set(fiHL.Id.IntegerValue);
#endif
				fiHLB = instance;

#if DEBUG
#else
				List<ElementId> tmpIds = new()
				{
					fiHLA.Id, fiHLB.Id
				};
				doc.ActiveView.HideElements(tmpIds);
#endif

				doc.Regenerate();

				foreach (HLPart hlPart in sl.Parts)
				{
					Part part = doc.GetElement(hlPart.Id) as Part;
					Face baseFace = Util.GetBaseFaceOfPart(doc, part);
					Plane plane = baseFace.GetSurface() as Plane;
					location = Line.CreateBound(plane.ProjectOnto(location.GetEndPoint(0)), plane.ProjectOnto(location.GetEndPoint(1)));
					if (Util.GetDirectionOfPart(doc, location, part) == 1)
					{
						InstanceVoidCutUtils.AddInstanceVoidCut(doc, part, fiHLB);

						GeometryElement geomElem = part.get_Geometry(new Options());
						foreach (GeometryObject geomObject in geomElem)
						{
							if (geomObject is Solid)
							{
								Solid solid = geomObject as Solid;
								FaceArray faceArray = solid.Faces;
								foreach (Face f in faceArray)
								{
									if (!(f is PlanarFace) || Util.IsEqual((f as PlanarFace).FaceNormal, normal) || Util.IsEqual((f as PlanarFace).FaceNormal, -normal))
									{
										continue;
									}

									//List<Curve> boundary = Util.GetOptimizedBoundaryCurves(f);
									//if(Util.IsPointInsideCurveLoop(boundary, originalLocation.GetEndPoint(0))
									//	&& Util.IsPointInsideCurveLoop(boundary, originalLocation.GetEndPoint(1)))
									if (Util.IsZero(Math.Abs((f.GetSurface() as Plane).SignedDistanceTo((location.GetEndPoint(0) + location.GetEndPoint(1)) / 2))))
									//	&& Util.IsPointInsideCurveLoop(Util.GetOptimizedBoundaryCurves(f), (location.GetEndPoint(0) + location.GetEndPoint(1)) / 2))
									{
										Line line = Line.CreateBound((f.GetSurface() as Plane).ProjectOnto(location.GetEndPoint(0)), (f.GetSurface() as Plane).ProjectOnto(location.GetEndPoint(1)));
										bool bOverlap = false;
										foreach (Curve c in Util.GetOptimizedBoundaryCurves(f))
										{
											SetComparisonResult scr = c.Intersect(line);
											if (scr == SetComparisonResult.Overlap
												|| scr == SetComparisonResult.Equal)
											{
												bOverlap = true;
												break;
											}
										}
										if (bOverlap)
										{
											if (part.CanOffsetFace(f))
											{
												part.SetFaceOffset(f, GetOffset());
											}
											else
											{

											}
											break;
										}
										else
										{

										}
									}
								}
							}
						}
					}
					else
					{
						if (InstanceVoidCutUtils.CanBeCutWithVoid(part))
						{
							InstanceVoidCutUtils.AddInstanceVoidCut(doc, part, fiHLA);
						}

						GeometryElement geomElem = part.get_Geometry(new Options());
						foreach (GeometryObject geomObject in geomElem)
						{
							if (geomObject is Solid)
							{
								Solid solid = geomObject as Solid;
								FaceArray faceArray = solid.Faces;
								foreach (Face f in faceArray)
								{
									if (!(f is PlanarFace) || Util.IsEqual((f as PlanarFace).FaceNormal, normal) || Util.IsEqual((f as PlanarFace).FaceNormal, -normal))
									{
										continue;
									}

									//List<Curve> boundary = Util.GetOptimizedBoundaryCurves(f);
									//if (Util.IsPointInsideCurveLoop(boundary, originalLocation.GetEndPoint(0))
									//	&& Util.IsPointInsideCurveLoop(boundary, originalLocation.GetEndPoint(1)))
									if (Util.IsZero(Math.Abs((f.GetSurface() as Plane).SignedDistanceTo((location.GetEndPoint(0) + location.GetEndPoint(1)) / 2))))
									{
										Line line = Line.CreateBound((f.GetSurface() as Plane).ProjectOnto(location.GetEndPoint(0)), (f.GetSurface() as Plane).ProjectOnto(location.GetEndPoint(1)));
										bool bOverlap = false;
										foreach (Curve c in Util.GetOptimizedBoundaryCurves(f))
										{
											SetComparisonResult scr = c.Intersect(line);
											if (scr == SetComparisonResult.Overlap
												|| scr == SetComparisonResult.Equal)
											{
												bOverlap = true;
												break;
											}
										}
										if (bOverlap)
										{
											if (part.CanOffsetFace(f))
											{
												part.SetFaceOffset(f, fLapWidth - GetOffset());
											}
											else
											{

											}
											break;
										}
										else
										{

										}
									}
								}
							}
						}
					}
				}

				RemoveHLLSAt(location);
				//	Add HLLS if needed
				for (int i = 0; i < sl.Parts.Count; i++)
				{
					if (Util.IsEqual(sl.CollidingCurves[i].Length, location.Length))
						continue;

					Face f = Util.GetBaseFaceOfPart(doc, doc.GetElement(sl.Parts[i].Id) as Part);
					normal = (f as PlanarFace).FaceNormal;
					Part part = doc.GetElement(sl.Parts[i].Id) as Part;

					int nDirection = Util.GetDirectionOfPart(doc, location, part);
					Line newloc;

					try
					{
						if (!Util.IsEqual(sl.CollidingCurves[i].GetEndPoint(0), location.GetEndPoint(0)))
						{
							if (sl.CollidingCurves[i].GetEndPoint(0).DistanceTo(location.GetEndPoint(1)) < location.GetEndPoint(0).DistanceTo(location.GetEndPoint(1)))
							//	location is longer
							{
								newloc = nDirection > 0 ? Line.CreateBound(sl.CollidingCurves[i].GetEndPoint(0), location.GetEndPoint(0))
									: Line.CreateBound(location.GetEndPoint(0), sl.CollidingCurves[i].GetEndPoint(0));
							}
							else
							//	location is shorter
							{
								newloc = nDirection > 0 ? Line.CreateBound(location.GetEndPoint(0), sl.CollidingCurves[i].GetEndPoint(0))
									: Line.CreateBound(sl.CollidingCurves[i].GetEndPoint(0), location.GetEndPoint(0));
								HalfLapLSInstance hlLS = FindHLLSAt(newloc);
								if (hlLS == null)
								{
									instance = GenerateHLLS(newloc, face, (newloc.GetEndPoint(1) - newloc.GetEndPoint(0)).CrossProduct(normal).Normalize(), fThickness, fLapWidth);
									doc.Regenerate();
									InstanceVoidCutUtils.AddInstanceVoidCut(doc, part, instance);
								}
							}
						}
						if (!Util.IsEqual(sl.CollidingCurves[i].GetEndPoint(1), location.GetEndPoint(1)))
						{
							if (sl.CollidingCurves[i].GetEndPoint(1).DistanceTo(location.GetEndPoint(0)) < location.GetEndPoint(1).DistanceTo(location.GetEndPoint(0)))
							//	location is longer
							{
								newloc = nDirection > 0 ? Line.CreateBound(sl.CollidingCurves[i].GetEndPoint(1), location.GetEndPoint(1))
									: Line.CreateBound(location.GetEndPoint(1), sl.CollidingCurves[i].GetEndPoint(1));
							}
							else
							//	location is shorter
							{
								newloc = nDirection > 0 ? Line.CreateBound(sl.CollidingCurves[i].GetEndPoint(1), location.GetEndPoint(1))
									: Line.CreateBound(location.GetEndPoint(1), sl.CollidingCurves[i].GetEndPoint(1));

								HalfLapLSInstance hlLS = FindHLLSAt(newloc);
								if (hlLS == null)
								{
									instance = GenerateHLLS(newloc, face, (newloc.GetEndPoint(1) - newloc.GetEndPoint(0)).CrossProduct(normal).Normalize(), fThickness, fLapWidth);
									doc.Regenerate();
									InstanceVoidCutUtils.AddInstanceVoidCut(doc, part, instance);
								}
							}
						}
					}
					catch (Exception ex) { TaskDialog.Show("Error", ex.Message); }
				}
				trans.Commit();
				if (m_bFlip)
				{
					UpdateParameter(doc
						, fiHL
						, fDepth
						, fLapWidth
						, fSideAGap
						, fSideBGap
						, fGap
						, 0
						, 0
						, -1
						, ConnectionLocation
						, m_bFlip);
				}
				return "";
			}
			catch (Exception e)
			{
				Debug.WriteLine(e.StackTrace);
				trans.RollBack();
				return e.Message;
			}
		}

		private HalfLapLSInstance FindHLLSAt(Curve location)
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			for (int i = m_HLLSInstances.Count - 1; i >= 0; i--)
			{
				HalfLapLSInstance hlLS = m_HLLSInstances[i];
				if (!hlLS.Doc.IsValidObject || !hlLS.Doc.Equals(doc))
					continue;

				SetComparisonResult scr = location.Intersect(hlLS.Location);//, out IntersectionResultArray ra);
				if (scr == SetComparisonResult.Equal)
				{
					return hlLS;
				}
			}
			return null;
		}

		private void RemoveHLLSAt(Curve location)
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			HalfLapLSInstance hlLS = FindHLLSAt(location);
			if (hlLS == null)
				return;

			if (!Util.IsEqual(
				(hlLS.Location.GetEndPoint(1) - hlLS.Location.GetEndPoint(0)).Normalize()
				, (location.GetEndPoint(1) - location.GetEndPoint(0)).Normalize()
				))
			{
				location = location.CreateReversed();
			}

			FamilyInstance ins = doc.GetElement(hlLS.Id) as FamilyInstance;
			if (Util.IsEqual(location.GetEndPoint(0), hlLS.Location.GetEndPoint(0))
				&& Util.IsEqual(location.GetEndPoint(1), hlLS.Location.GetEndPoint(1)))
			{
				doc.Delete(hlLS.Id);
				m_HLLSInstances.Remove(hlLS);
			}
			else if (Util.IsEqual(location.GetEndPoint(0), hlLS.Location.GetEndPoint(0)))
			{
				Line newLocation = Line.CreateBound(location.GetEndPoint(1), hlLS.Location.GetEndPoint(1));
				XYZ newLP = (newLocation.GetEndPoint(1) + newLocation.GetEndPoint(0)) / 2;
				ElementTransformUtils.MoveElement(doc, hlLS.Id, newLP - (ins.Location as LocationPoint).Point);
				ins.LookupParameter("Length").Set(newLocation.Length);
			}
			else if (Util.IsEqual(location.GetEndPoint(1), hlLS.Location.GetEndPoint(1)))
			{
				Line newLocation = Line.CreateBound(location.GetEndPoint(0), hlLS.Location.GetEndPoint(0));
				XYZ newLP = (newLocation.GetEndPoint(1) + newLocation.GetEndPoint(0)) / 2;
				ElementTransformUtils.MoveElement(doc, hlLS.Id, newLP - (ins.Location as LocationPoint).Point);
				ins.LookupParameter("Length").Set(newLocation.Length);
			}
			else if (Util.IsEqual(hlLS.Location.Length, hlLS.Location.GetEndPoint(0).DistanceTo(location.GetEndPoint(0)) + hlLS.Location.GetEndPoint(1).DistanceTo(location.GetEndPoint(0)))
				&& Util.IsEqual(hlLS.Location.Length, hlLS.Location.GetEndPoint(0).DistanceTo(location.GetEndPoint(1)) + hlLS.Location.GetEndPoint(1).DistanceTo(location.GetEndPoint(1))))
			//	location is inside hlLS
			{
				Line newLocation = Line.CreateBound(location.GetEndPoint(0), hlLS.Location.GetEndPoint(0));
				XYZ newLP = (newLocation.GetEndPoint(1) + newLocation.GetEndPoint(0)) / 2;
				ElementTransformUtils.MoveElement(doc, hlLS.Id, newLP - (ins.Location as LocationPoint).Point);
				ins.LookupParameter("Length").Set(newLocation.Length);

				newLocation = Line.CreateBound(location.GetEndPoint(1), hlLS.Location.GetEndPoint(1));
				FamilyInstance anotherIns = GenerateHLLS(newLocation, ins.HostFace, ins.HandOrientation, ins.LookupParameter("Thickness").AsDouble(), ins.LookupParameter("Width").AsDouble());
				foreach (ElementId eId in InstanceVoidCutUtils.GetElementsBeingCut(ins))
				{
					InstanceVoidCutUtils.AddInstanceVoidCut(doc, doc.GetElement(eId), anotherIns);
				}
			}
		}

		private FamilyInstance GenerateHLLS(Curve location, Face face, XYZ direction, double fThickness, double fLapWidth)
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;

			XYZ pos = (location.GetEndPoint(1) + location.GetEndPoint(0)) / 2;
			FamilyInstance instance = doc.Create.NewFamilyInstance(
				face
				, pos
				, direction
				, GetFamilySymbol(HLLS));

			Parameter param = instance.LookupParameter("Thickness");
			param.Set(fThickness);
			param = instance.LookupParameter("Width");
			param.Set(fLapWidth);
			param = instance.LookupParameter("Length");
			param.Set(location.Length);
			//param = instance.LookupParameter("SketchLine");
			//param.Set(fiHL.Id.IntegerValue);
#if DEBUG
#else
			doc.ActiveView.HideElements(new List<ElementId>() { instance.Id });
#endif
			return instance;
		}

		private FamilyInstance GenerateHLLS(Curve location, Reference faceReference, XYZ direction, double fThickness, double fLapWidth)
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;

			XYZ pos = (location.GetEndPoint(1) + location.GetEndPoint(0)) / 2;
			FamilyInstance instance = doc.Create.NewFamilyInstance(
				faceReference
				, pos
				, direction
				, GetFamilySymbol(HLLS));

			Parameter param = instance.LookupParameter("Thickness");
			param.Set(fThickness);
			param = instance.LookupParameter("Width");
			param.Set(fLapWidth);
			param = instance.LookupParameter("Length");
			param.Set(location.Length);
			//param = instance.LookupParameter("SketchLine");
			//param.Set(fiHL.Id.IntegerValue);
#if DEBUG
#else
			doc.ActiveView.HideElements(new List<ElementId>() { instance.Id });
#endif
			return instance;
		}

		private FamilySymbol GetFamilySymbol(Family family)
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;
			ISet<ElementId> familySymbolIds = family.GetFamilySymbolIds();

			foreach (ElementId id in familySymbolIds)
			{
				FamilySymbol familySymbol = doc.GetElement(id) as FamilySymbol;
				if (!familySymbol.IsActive)
					familySymbol.Activate();
				return familySymbol;
			}
			return null;
		}

		private double GetOffset()
		{
			double fLapWidth = UnitUtils.ConvertToInternalUnits(m_nLapWidth, m_UnitType);
			if (ConnectionLocation == ConnectionLocationType.Centre)
				return fLapWidth / 2;
			else if (ConnectionLocation == ConnectionLocationType.PushToB)
			{
				return 0;
			}
			else
				return fLapWidth;
		}

		#region Create necessary families
		/// <summary>
		/// Check if Half Lap family is already loaded
		/// </summary>
		private bool CheckHLFamily()
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;

			HL = null;
			HLA = null;
			HLB = null;
			HLLS = null;
			//if (HLA != null && !HLA.IsValidObject) HLA = null;
			//if (HLB != null && !HLB.IsValidObject) HLB = null;

			//if (HLA != null && HLB != null)
			//{
			//	return true;
			//}

			FilteredElementCollector collector = new(doc);
			ICollection<Element> collection = collector.OfClass(typeof(Family)).ToElements();

			foreach (Element e in collection)
			{
				Family f = e as Family;
				if (f.Name == HLMarkerFamilyName)
				{
					HL = f;
				}
				else if (f.Name == HLAFamilyName)
				{
					HLA = f;
				}
				else if (f.Name == HLBFamilyName)
				{
					HLB = f;
				}
				else if (f.Name == HLLSFamilyName)
				{
					HLLS = f;
				}
			}

			if (HL != null && HLA != null && HLB != null && HLLS != null)
			{
				return true;
			}
			else
			{
				Transaction trans = new(doc);
				trans.Start("Load Family");
				try
				{
					string url = Assembly.GetExecutingAssembly().Location;
					url = url.Substring(0, url.LastIndexOf("\\")) + "\\";

					bool bRet1 = HLA != null || doc.LoadFamily(url + HLAFamilyName + ".rfa", out HLA);
					bool bRet2 = HLB != null || doc.LoadFamily(url + HLBFamilyName + ".rfa", out HLB);
					bool bRet3 = HL != null || doc.LoadFamily(url + HLMarkerFamilyName + ".rfa", out HL);
					bool bRet4 = HLLS != null || doc.LoadFamily(url + HLLSFamilyName + ".rfa", out HLLS);
					if (bRet1 && bRet2 && bRet3 && bRet4)
					{
						trans.Commit();
						return true;
					}
					else
					{
						trans.RollBack();
						return false;
					}
				}
				catch (Exception)
				{
					trans.RollBack();
					return false;
				}
			}
		}
		#endregion

		/// <summary>
		/// Update Parameters
		/// </summary>
		/// <param name="instance"></param>
		public static void UpdateParameter(
			Document doc
			, FamilyInstance instance
			, double fDepth
			, double fLapWidth
			, double fSideAGap
			, double fSideBGap
			, double fGapBetweenPanels
			, double fEndA
			, double fEndB
			, double fLength
			, ConnectionLocationType connectionLocation
			, bool bFlip
			)
		{
			Parameter param;
			ICollection<ElementId> elemIds;
			GeometryElement geomElem;

			if (instance.Name == HLMarkerFamilyName)
			{
				Transaction trans = new(doc);
				trans.Start("Update Parameters");

				double fThickness = instance.LookupParameter("Thickness").AsDouble();
				param = instance.LookupParameter("Depth");
				if (fDepth >= 0)
					param.Set(fDepth);
				else
					param.Set(fThickness / 2);
				instance.LookupParameter("LapWidth").Set(fLapWidth);
				instance.LookupParameter("SideAGap").Set(fSideAGap);
				instance.LookupParameter("SideBGap").Set(fSideBGap);
				instance.LookupParameter("Gap").Set(fGapBetweenPanels);
				instance.LookupParameter("End A").Set(fEndA);
				instance.LookupParameter("End B").Set(fEndB);
				if (fLength > 0)
					instance.LookupParameter("HL Length").Set(fLength);
				instance.LookupParameter("Location").Set((int)connectionLocation);
				instance.LookupParameter("Flip").Set(bFlip ? 1 : 0);
				trans.Commit();
				return;
			}

			try
			{
				Transaction trans = new(doc);
				trans.Start("Update Parameters");

				//	Get the information of the old instance
				FamilySymbol symbol = instance.Symbol;
				LocationPoint lp = instance.Location as LocationPoint;
				param = instance.LookupParameter("Thickness");
				double fThickness = param.AsDouble();
				//param = instance.LookupParameter("Angle1");
				//double fAngle1 = param.AsDouble();
				//param = instance.LookupParameter("Angle2");
				//double fAngle2 = Math.PI - param.AsDouble();
				param = instance.LookupParameter("Length");
				if (fLength < 0)
					fLength = param.AsDouble();
				param = instance.LookupParameter("Offset");
				double fOffset = param.AsDouble();
				param = instance.LookupParameter("LapWidth");
				double fOldLapWidth = param.AsDouble();
				param = instance.LookupParameter("SketchLine");
				int sketchLineId = param.AsInteger();

				//	Get host
				Reference refer = instance.HostFace;
				Element parent = doc.GetElement(refer);

				PlanarFace hostFace = parent.GetGeometryObjectFromReference(refer) as PlanarFace;
				XYZ normal = hostFace.FaceNormal;
				XYZ direction = instance.HandOrientation;
				XYZ locationDirection = direction.CrossProduct(normal);
				Line oldlocation = Line.CreateBound(lp.Point + locationDirection * fLength / 2
					, lp.Point - locationDirection * fLength / 2);
				Line originalLocation = Line.CreateBound(
					oldlocation.GetEndPoint(0),
					oldlocation.GetEndPoint(1)// + (oldlocation.GetEndPoint(1) - oldlocation.GetEndPoint(0)).Normalize() * Math.Tan(fAngle1) * fOffset
					);

				fOffset = fLapWidth;
				if (connectionLocation == ConnectionLocationType.Centre)
					fOffset /= 2;
				else if (connectionLocation == ConnectionLocationType.PushToB)
				{
					fOffset = 0;
				}
				Line location = Line.CreateBound(
					originalLocation.GetEndPoint(0),
					originalLocation.GetEndPoint(1)// - (originalLocation.GetEndPoint(1) - originalLocation.GetEndPoint(0)).Normalize() * Math.Tan(fAngle1) * fOffset
				);
				fLength = location.Length;

				if (bFlip)
				{
					//fAngle1 = Math.PI - fAngle1;
					//fAngle2 = Math.PI - fAngle2;
					//if (Util.IsEqual(fAngle1, Math.PI)) fAngle1 = 0;
					//if (Util.IsEqual(fAngle2, Math.PI)) fAngle2 = 0;
					Face similarFace = null;
					//	Get the corresponding face (Find the most similar face)
					Options options = new()
					{
						ComputeReferences = true
					};
					geomElem = parent.get_Geometry(options);
					foreach (GeometryObject geomObject in geomElem)
					{
						if (geomObject is Solid)
						{
							Solid solid = geomObject as Solid;
							FaceArray faceArray = solid.Faces;
							foreach (Face f in faceArray)
							{
								if (!(f is PlanarFace))
									continue;

								if ((Util.IsEqual((f.GetSurface() as Plane).Normal, -hostFace.FaceNormal)
									|| Util.IsEqual((f.GetSurface() as Plane).Normal, hostFace.FaceNormal))
									&& !Util.IsEqual(f, hostFace))
								{
									if (Util.IsEqual(hostFace.Origin.DistanceTo((f as PlanarFace).Origin), fThickness))
									{
										similarFace = f;
										break;
									}
									if (similarFace == null ||
										Math.Abs(hostFace.Area - similarFace.Area) > Math.Abs(hostFace.Area - f.Area))
									{
										similarFace = f;
									}
								}
							}
						}
					}

					FamilyInstance newinstance = doc.Create.NewFamilyInstance(similarFace,
						(location.GetEndPoint(0) + location.GetEndPoint(1)) / 2,
						instance.HandOrientation,
						symbol);
					//param = newinstance.LookupParameter("Angle1");
					//param.Set(fAngle2);
					//param = newinstance.LookupParameter("Angle2");
					//param.Set(fAngle1);
					param = newinstance.LookupParameter("Length");
					param.Set(fLength);
					param = newinstance.LookupParameter("Thickness");
					param.Set(fThickness);
					param = newinstance.LookupParameter("SketchLine");
					param.Set(sketchLineId);
#if DEBUG
#else
					doc.ActiveView.HideElements(new List<ElementId>() { newinstance.Id });
#endif

					elemIds = InstanceVoidCutUtils.GetElementsBeingCut(instance);
					foreach (ElementId eId in elemIds)
					{
						InstanceVoidCutUtils.AddInstanceVoidCut(doc, doc.GetElement(eId), newinstance);
					}

					doc.Delete(instance.Id);
					instance = newinstance;
				}

				doc.Regenerate();
				elemIds = InstanceVoidCutUtils.GetElementsBeingCut(instance);
				foreach (ElementId eId in elemIds)
				{
					Part p = doc.GetElement(eId) as Part;
					InstanceVoidCutUtils.RemoveInstanceVoidCut(doc, p, instance);
					doc.Regenerate();

					Face baseFace = Util.GetBaseFaceOfPart(doc, p);
					Plane basePlane = baseFace.GetSurface() as Plane;
					Curve oriLoc = Line.CreateBound(basePlane.ProjectOnto(originalLocation.GetEndPoint(0)), basePlane.ProjectOnto(originalLocation.GetEndPoint(1)));

					geomElem = p.get_Geometry(new Options());
					foreach (GeometryObject geomObject in geomElem)
					{
						if (geomObject is Solid)
						{
							Solid solid = geomObject as Solid;
							FaceArray faceArray = solid.Faces;
							foreach (Face f in faceArray)
							{
								if (!(f is PlanarFace) || Util.IsEqual((f as PlanarFace).FaceNormal, normal) || Util.IsEqual((f as PlanarFace).FaceNormal, -normal))
								{
									continue;
								}

								Plane plane = f.GetSurface() as Plane;
								double d1 = Math.Abs(plane.SignedDistanceTo(oriLoc.GetEndPoint(0)))
									, d2 = Math.Abs(plane.SignedDistanceTo(oriLoc.GetEndPoint(1)));
								if ((Util.IsEqual(d1, fOldLapWidth) || d1 <= fOldLapWidth)
									&& (Util.IsEqual(d2, fOldLapWidth) || d2 <= fOldLapWidth))
								{
									Curve loc = Line.CreateBound(plane.ProjectOnto(oriLoc.GetEndPoint(0)), plane.ProjectOnto(oriLoc.GetEndPoint(1)));
									bool bOverlap = false;
									foreach (Curve c in Util.GetOptimizedBoundaryCurves(f))
									{
										SetComparisonResult scr = c.Intersect(loc);
										if (scr == SetComparisonResult.Overlap
											|| scr == SetComparisonResult.Equal)
										{
											bOverlap = true;
											break;
										}
									}
									if (bOverlap)
									{
										p.ResetFaceOffset(f);
									}
								}
								//	&& Util.IsPointInsideCurveLoop(Util.GetOptimizedBoundaryCurves(f), ptProject))
								//	p.ResetFaceOffset(f);
							}
						}
					}
					doc.Regenerate();

					geomElem = p.get_Geometry(new Options());
					foreach (GeometryObject geomObject in geomElem)
					{
						if (geomObject is Solid)
						{
							Solid solid = geomObject as Solid;
							FaceArray faceArray = solid.Faces;
							foreach (Face f in faceArray)
							{
								if (!(f is PlanarFace) || Util.IsEqual((f as PlanarFace).FaceNormal, normal) || Util.IsEqual((f as PlanarFace).FaceNormal, -normal))
								{
									continue;
								}

								//List<Curve> boundary = Util.GetOptimizedBoundaryCurves(f);
								//if (Util.IsPointInsideCurveLoop(boundary, originalLocation.GetEndPoint(0))
								//	&& Util.IsPointInsideCurveLoop(boundary, originalLocation.GetEndPoint(1)))
								if (Util.IsZero(Math.Abs((f.GetSurface() as Plane).SignedDistanceTo(oriLoc.GetEndPoint(0))))
									&& Util.IsZero(Math.Abs((f.GetSurface() as Plane).SignedDistanceTo(oriLoc.GetEndPoint(1)))))
								//&& Util.IsPointInsideCurveLoop(Util.GetOptimizedBoundaryCurves(f), (originalLocation.GetEndPoint(0) + originalLocation.GetEndPoint(1)) / 2))
								{
									bool bOverlap = false;
									foreach (Curve c in Util.GetOptimizedBoundaryCurves(f))
									{
										SetComparisonResult scr = c.Intersect(oriLoc);
										if (scr == SetComparisonResult.Overlap
											|| scr == SetComparisonResult.Equal)
										{
											bOverlap = true;
											break;
										}
									}
									if (bOverlap)
									{
										if (p.CanOffsetFace(f))
										{
											if (instance.Name == HLAFamilyName)
												p.SetFaceOffset(f, fLapWidth - fOffset);
											else
												p.SetFaceOffset(f, fOffset);
										}
										else
										{

										}
										break;
									}
								}
							}
						}
					}
					InstanceVoidCutUtils.AddInstanceVoidCut(doc, p, instance);
				}

				param = instance.LookupParameter("Gap");
				param.Set(fGapBetweenPanels);
				param = instance.LookupParameter("Depth");
				if (fDepth >= 0)
					param.Set(fDepth);
				else
					param.Set(fThickness / 2);
				param = instance.LookupParameter("LapWidth");
				param.Set(fLapWidth);
				param = instance.LookupParameter("SideAGap");
				if (param != null) param.Set(fSideAGap);
				param = instance.LookupParameter("SideBGap");
				if (param != null) param.Set(fSideBGap);
				param = instance.LookupParameter("Offset");
				param.Set(fOffset);
				param = instance.LookupParameter("Length");
				param.Set(fLength);
				param = instance.LookupParameter("End A");
				param.Set(fEndA);
				param = instance.LookupParameter("End B");
				param.Set(fEndB);

				doc.Regenerate();
				ElementTransformUtils.MoveElement(doc, instance.Id, (location.GetEndPoint(1) - location.GetEndPoint(0)) / 2 - (oldlocation.GetEndPoint(1) - oldlocation.GetEndPoint(0)) / 2);

				trans.Commit();
			}
			catch (Exception _)
			{
				TaskDialog.Show("Error", _.Message);
			}
		}

		public static FamilyInstance[] FindPairs(Document doc, FamilyInstance marker)
		{
			FamilyInstance[] ret = new FamilyInstance[2] { null, null };

#if REVIT2024 || REVIT2025
			long nParentId = marker.Id.Value;
#else
            int nParentId = marker.Id.IntegerValue;
#endif

			List<FamilyInstance> hlA = new(
					new FilteredElementCollector(doc)
						.WhereElementIsNotElementType()
						.OfClass(typeof(FamilyInstance))
						.Where(inst => inst.Name == HLAFamilyName && inst.LookupParameter("SketchLine").AsInteger() == nParentId)
						.ToList()
						.Cast<FamilyInstance>()
						);
			if (hlA.Count > 0)
				ret[0] = hlA[0];
			else
			{
				//				TaskDialog.Show("Warning", "Can not find the pairs(A)");
				LogManager.Write(LogManager.WarningLevel.Warning, "HalfLap::FindPairs", "Can not find the pairs(A) of Half Lap instance(" + nParentId.ToString() + ")");
			}
			List<FamilyInstance> hlB = new(
								new FilteredElementCollector(doc)
									.WhereElementIsNotElementType()
									.OfClass(typeof(FamilyInstance))
									.Where(inst => inst.Name == HLBFamilyName && inst.LookupParameter("SketchLine").AsInteger() == nParentId)
									.ToList()
									.Cast<FamilyInstance>()
									);
			if (hlB.Count > 0)
				ret[1] = hlB[0];
			else
			{
				//			TaskDialog.Show("Warning", "Can not find the pairs(B)");
				LogManager.Write(LogManager.WarningLevel.Warning, "HalfLap::FindPairs", "Can not find the pairs(B) of Half Lap instance(" + nParentId.ToString() + ")");
			}
			return ret;
		}

		public static ElementId[] FindPairsId(Document doc, FamilyInstance marker)
		{
			FamilyInstance[] ret = FindPairs(doc, marker);
			ElementId[] eIds = new ElementId[2] { null, null };
			if (ret[0] != null) eIds[0] = ret[0].Id;
			if (ret[1] != null) eIds[1] = ret[1].Id;
			return eIds;
		}

		public static bool DocChangedHandler(DocumentChangedEventArgs args)
		{
			Document doc = args.GetDocument();

			bool bHas = false;
			try
			{
				// first we check if the element was deleted
				ICollection<ElementId> elems = args.GetDeletedElementIds();

				if (elems.Count > 0)
				{
					foreach (HalfLapInstance hl in m_HLInstances)
					{
						if (elems.Contains(hl.Marker))
						{
							hl.Flag = 1;
							bHas = true;
						}
					}
					for (int i = m_HLLSInstances.Count - 1; i >= 0; i--)
					{
						HalfLapLSInstance hlLS = m_HLLSInstances[i];
						if (elems.Contains(hlLS.Id))
						{
							m_HLLSInstances.Remove(hlLS);
						}
					}
				}

				elems = args.GetAddedElementIds();
				if (elems.Count > 0)
				{
					foreach (ElementId eId in elems)
					{
						Element e = doc.GetElement(eId);
						if (e is FamilyInstance ins)
						{
							if (ins.Name == HLMarkerFamilyName)
							{
								ElementId[] pairs = FindPairsId(doc, ins);

								HalfLapInstance hl = new()
								{
									Doc = doc,
									Marker = ins.Id,
									Depth = ins.LookupParameter("Depth").AsDouble(),
									LapWidth = ins.LookupParameter("LapWidth").AsDouble(),
									SideAGap = ins.LookupParameter("SideAGap").AsDouble(),
									SideBGap = ins.LookupParameter("SideBGap").AsDouble(),
									GapBetweenPanels = ins.LookupParameter("Gap").AsDouble(),
									EndA = ins.LookupParameter("End A").AsDouble(),
									EndB = ins.LookupParameter("End B").AsDouble(),
									Length = ins.LookupParameter("HL Length").AsDouble(),
									Flip = ins.LookupParameter("Flip").AsInteger() == 1,
									ConnectionLocation = (ConnectionLocationType)ins.LookupParameter("Location").AsInteger(),
									A = pairs[0],
									B = pairs[1]
								};
								m_HLInstances.Add(hl);
							}
							else if (ins.Name == HLLSFamilyName)
							{
								HalfLapLSInstance hlLS = new()
								{
									Doc = doc,
									Id = ins.Id
								};
								LocationPoint lp = ins.Location as LocationPoint;
								double fLength = ins.LookupParameter("Length").AsDouble();
								XYZ pt1 = lp.Point - ins.FacingOrientation * fLength / 2
									, pt2 = lp.Point + ins.FacingOrientation * fLength / 2;
								hlLS.Location = Line.CreateBound(pt1, pt2);

								m_HLLSInstances.Add(hlLS);
							}
						}
					}
				}

				elems = args.GetModifiedElementIds();
				if (elems.Count > 0)
				{
					foreach (HalfLapInstance hl in m_HLInstances)
					{
						if (elems.Contains(hl.Marker))
						{
							hl.Flag = 2;
							bHas = true;
						}
					}
				}

				return bHas;
				//if (bHas)
				//{
				//Application.thisApp.GetUIContApp().Idling += OnIdlingEvent;
				//}
			}
			catch (Exception ex)
			{
				TaskDialog.Show("Error", ex.Message);
				return false;
			}
		}

		public static void DocOpenedHandler(DocumentOpenedEventArgs args)
		{
			//m_HLInstances.Clear();
			//m_HLLSInstances.Clear();

			//	Remove unnecessary HL instances of closed documents
			for (int i = m_HLInstances.Count - 1; i >= 0; i--)
			{
				if (!m_HLInstances[i].Doc.IsValidObject)
					m_HLInstances.RemoveAt(i);
			}

			Document doc = args.Document;
			if (doc == null)
				return;

			List<FamilyInstance> totalHLs = new(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(FamilyInstance))
					.Where(ins => ins.Name == HLMarkerFamilyName)
					.ToList()
					.Cast<FamilyInstance>()
					);
			List<FamilyInstance> totalHLAs = new(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(FamilyInstance))
					.Where(ins => ins.Name == HLAFamilyName)
					.ToList()
					.Cast<FamilyInstance>()
					);
			List<FamilyInstance> totalHLBs = new(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(FamilyInstance))
					.Where(ins => ins.Name == HLBFamilyName)
					.ToList()
					.Cast<FamilyInstance>()
					);
			foreach (FamilyInstance ins in totalHLs)
			{
				HalfLapInstance hl = new()
				{
					Doc = doc,
					Marker = ins.Id,
					Depth = ins.LookupParameter("Depth").AsDouble(),
					LapWidth = ins.LookupParameter("LapWidth").AsDouble(),
					SideAGap = ins.LookupParameter("SideAGap").AsDouble(),
					SideBGap = ins.LookupParameter("SideBGap").AsDouble(),
					GapBetweenPanels = ins.LookupParameter("Gap").AsDouble(),
					EndA = ins.LookupParameter("End A") != null ? ins.LookupParameter("End A").AsDouble() : 0,
					EndB = ins.LookupParameter("End B") != null ? ins.LookupParameter("End B").AsDouble() : 0,
					Length = ins.LookupParameter("HL Length").AsDouble(),
					Flip = ins.LookupParameter("Flip").AsInteger() == 1,
					ConnectionLocation = (ConnectionLocationType)ins.LookupParameter("Location").AsInteger()
				};

#if REVIT2024 || REVIT2025
				long nParentId = ins.Id.Value;
#else
                int nParentId = ins.Id.IntegerValue;
#endif
				//	Find the pairs
				List<FamilyInstance> tmp = totalHLAs.Where(inst => inst.LookupParameter("SketchLine").AsInteger() == nParentId).ToList();
				if (tmp.Count > 0)
					hl.A = tmp[0].Id;
				tmp = totalHLBs.Where(inst => inst.LookupParameter("SketchLine").AsInteger() == nParentId).ToList();
				if (tmp.Count > 0)
					hl.B = tmp[0].Id;
				//tmp = totalHLLSs.Where(inst => inst.LookupParameter("SketchLine").AsInteger() == nSketchLineId).ToList();
				//if (tmp.Count > 0)
				//	hl.B = tmp[0];
				m_HLInstances.Add(hl);
			}

			List<FamilyInstance> totalHLLSs = new(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(FamilyInstance))
					.Where(ins => ins.Name == HLLSFamilyName)
					.ToList()
					.Cast<FamilyInstance>()
					);
			foreach (FamilyInstance ins in totalHLLSs)
			{
				HalfLapLSInstance hlLS = new()
				{
					Doc = doc,
					Id = ins.Id
				};
				LocationPoint lp = ins.Location as LocationPoint;
				double fLength = ins.LookupParameter("Length").AsDouble();
				XYZ pt1 = lp.Point - ins.FacingOrientation * fLength / 2
					, pt2 = lp.Point + ins.FacingOrientation * fLength / 2;
				hlLS.Location = Line.CreateBound(pt1, pt2);

				m_HLLSInstances.Add(hlLS);
			}
		}

		public static void OnIdlingEvent(object sender, IdlingEventArgs e)
		{
			UIApplication uiApp = sender as UIApplication;
			Document doc = uiApp.ActiveUIDocument.Document;

			TransactionGroup tg = new(doc);
			tg.Start("HL Management");
			try
			{
				for (int i = m_HLInstances.Count - 1; i >= 0; i--)
				{
					HalfLapInstance hl = m_HLInstances[i];
					if (hl.Doc.IsValidObject && !hl.Doc.Equals(doc))
						continue;
					if (hl.Flag == 1)
					{
						Transaction trans = new(doc);
						trans.Start("HL Delete");
						if (hl.A != null && doc.GetElement(hl.A) != null)
						{
							//	Reset Face Offset
							try
							{
								FamilyInstance instance = doc.GetElement(hl.A) as FamilyInstance;
								Parameter param;
								//	Get the information of the old instance
								FamilySymbol symbol = instance.Symbol;
								LocationPoint lp = instance.Location as LocationPoint;
								//param = instance.LookupParameter("Angle1");
								//double fAngle1 = param.AsDouble();
								//param = instance.LookupParameter("Angle2");
								//double fAngle2 = Math.PI - param.AsDouble();
								param = instance.LookupParameter("Length");
								double fLength = param.AsDouble();
								param = instance.LookupParameter("Offset");
								double fOffset = param.AsDouble();
								param = instance.LookupParameter("LapWidth");
								double fOldLapWidth = param.AsDouble();

								//	Get host
								Reference refer = instance.HostFace;
								Element parent = doc.GetElement(refer);

								PlanarFace hostFace = parent.GetGeometryObjectFromReference(refer) as PlanarFace;
								XYZ normal = hostFace.FaceNormal;
								XYZ direction = instance.HandOrientation;
								XYZ locationDirection = direction.CrossProduct(normal);
								Line oldlocation = Line.CreateBound(lp.Point + locationDirection * fLength / 2
									, lp.Point - locationDirection * fLength / 2);
								Line originalLocation = Line.CreateBound(
									oldlocation.GetEndPoint(0),
									oldlocation.GetEndPoint(1)// + (oldlocation.GetEndPoint(1) - oldlocation.GetEndPoint(0)).Normalize() * Math.Tan(fAngle1) * fOffset
									);

								List<ElementId> elemIds = (List<ElementId>)InstanceVoidCutUtils.GetElementsBeingCut(instance);
								foreach (ElementId eId in elemIds)
								{
									Part p = doc.GetElement(eId) as Part;
									InstanceVoidCutUtils.RemoveInstanceVoidCut(doc, p, instance);
									doc.Regenerate();

									GeometryElement geomElem = p.get_Geometry(new Options());
									foreach (GeometryObject geomObject in geomElem)
									{
										if (geomObject is Solid)
										{
											Solid solid = geomObject as Solid;
											FaceArray faceArray = solid.Faces;
											foreach (Face f in faceArray)
											{
												if (!(f is PlanarFace) || Util.IsEqual((f as PlanarFace).FaceNormal, normal) || Util.IsEqual((f as PlanarFace).FaceNormal, -normal))
												{
													continue;
												}

												double d = Math.Abs((f.GetSurface() as Plane).SignedDistanceTo(originalLocation.GetEndPoint(0)));
												if (Util.IsEqual(d, fOldLapWidth) || d <= fOldLapWidth)
													p.ResetFaceOffset(f);
											}
										}
									}
									doc.Regenerate();
								}
							}
							catch (Exception)
							{
							}

							doc.Delete(hl.A);
						}
						if (hl.B != null && doc.GetElement(hl.B) != null)
						{
							try
							{
								FamilyInstance instance = doc.GetElement(hl.B) as FamilyInstance;
								Parameter param;
								//	Get the information of the old instance
								FamilySymbol symbol = instance.Symbol;
								LocationPoint lp = instance.Location as LocationPoint;
								//param = instance.LookupParameter("Angle1");
								//double fAngle1 = param.AsDouble();
								//param = instance.LookupParameter("Angle2");
								//double fAngle2 = Math.PI - param.AsDouble();
								param = instance.LookupParameter("Length");
								double fLength = param.AsDouble();
								param = instance.LookupParameter("Offset");
								double fOffset = param.AsDouble();
								param = instance.LookupParameter("LapWidth");
								double fOldLapWidth = param.AsDouble();

								//	Get host
								Reference refer = instance.HostFace;
								Element parent = doc.GetElement(refer);

								PlanarFace hostFace = parent.GetGeometryObjectFromReference(refer) as PlanarFace;
								XYZ normal = hostFace.FaceNormal;
								XYZ direction = instance.HandOrientation;
								XYZ locationDirection = direction.CrossProduct(normal);
								Line oldlocation = Line.CreateBound(lp.Point + locationDirection * fLength / 2
									, lp.Point - locationDirection * fLength / 2);
								Line originalLocation = Line.CreateBound(
									oldlocation.GetEndPoint(0),
									oldlocation.GetEndPoint(1)// + (oldlocation.GetEndPoint(1) - oldlocation.GetEndPoint(0)).Normalize() * Math.Tan(fAngle1) * fOffset
									);

								List<ElementId> elemIds = (List<ElementId>)InstanceVoidCutUtils.GetElementsBeingCut(instance);
								foreach (ElementId eId in elemIds)
								{
									Part p = doc.GetElement(eId) as Part;
									InstanceVoidCutUtils.RemoveInstanceVoidCut(doc, p, instance);
									doc.Regenerate();

									GeometryElement geomElem = p.get_Geometry(new Options());
									foreach (GeometryObject geomObject in geomElem)
									{
										if (geomObject is Solid)
										{
											Solid solid = geomObject as Solid;
											FaceArray faceArray = solid.Faces;
											foreach (Face f in faceArray)
											{
												if (!(f is PlanarFace) || Util.IsEqual((f as PlanarFace).FaceNormal, normal) || Util.IsEqual((f as PlanarFace).FaceNormal, -normal))
												{
													continue;
												}

												double d = Math.Abs((f.GetSurface() as Plane).SignedDistanceTo(originalLocation.GetEndPoint(0)));
												if (Util.IsEqual(d, fOldLapWidth) || d <= fOldLapWidth)
													p.ResetFaceOffset(f);
											}
										}
									}
									doc.Regenerate();
								}
							}
							catch (Exception)
							{
							}

							doc.Delete(hl.B);
						}
						m_HLInstances.Remove(hl);
						trans.Commit();
					}
					else if (hl.Flag == 2)
					{
						hl.Flag = 0;

						FamilyInstance ins = doc.GetElement(hl.Marker) as FamilyInstance;
						Parameter param = ins.LookupParameter("Gap");
						hl.GapBetweenPanels = param.AsDouble();
						param = ins.LookupParameter("Depth");
						hl.Depth = param.AsDouble();
						param = ins.LookupParameter("LapWidth");
						hl.LapWidth = param.AsDouble();
						param = ins.LookupParameter("SideAGap");
						hl.SideAGap = param.AsDouble();
						param = ins.LookupParameter("SideBGap");
						hl.SideBGap = param.AsDouble();
						param = ins.LookupParameter("End A");
						hl.EndA = param.AsDouble();
						param = ins.LookupParameter("End B");
						hl.EndB = param.AsDouble();
						param = ins.LookupParameter("HL Length");
						hl.Length = param.AsDouble();
						bool bFlip = hl.Flip != (ins.LookupParameter("Flip").AsInteger() == 1);
						hl.ConnectionLocation = (ConnectionLocationType)ins.LookupParameter("Location").AsInteger();
						hl.Flip = ins.LookupParameter("Flip").AsInteger() == 1;

						if (hl.A == null || doc.GetElement(hl.A) == null
							|| hl.B == null || doc.GetElement(hl.B) == null)
						{
							ElementId[] eIds = FindPairsId(doc, ins);
							hl.A = eIds[0];
							hl.B = eIds[1];
						}

						if (hl.A != null && doc.GetElement(hl.A) is FamilyInstance AIns)
						{
							UpdateParameter(doc
								, AIns
								, hl.Depth
								, hl.LapWidth
								, hl.SideAGap
								, hl.SideBGap
								, hl.GapBetweenPanels
								, hl.EndA
								, hl.EndB
								, hl.Length
								, hl.ConnectionLocation
								, bFlip);

							if (bFlip)
							{
								ElementId[] eIds = FindPairsId(doc, doc.GetElement(hl.Marker) as FamilyInstance);
								hl.A = eIds[0];
							}
						}
						if (hl.B != null && doc.GetElement(hl.B) is FamilyInstance BIns)
						{
							UpdateParameter(doc
								, BIns
								, hl.Depth
								, hl.LapWidth
								, hl.SideAGap
								, hl.SideBGap
								, hl.GapBetweenPanels
								, hl.EndA
								, hl.EndB
								, hl.Length
								, hl.ConnectionLocation
								, bFlip);
							if (bFlip)
							{
								ElementId[] eIds = FindPairsId(doc, doc.GetElement(hl.Marker) as FamilyInstance);
								hl.B = eIds[1];
							}
						}
					}
				}
				tg.Assimilate();
			}
			catch (Exception ex)
			{
				tg.RollBack();
				TaskDialog.Show("Error", ex.Message);
			}

			//Application.thisApp.GetUIContApp().Idling -= OnIdlingEvent;
		}
	}
}
