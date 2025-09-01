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
	public class JointBoard : Controller
	{
		public const string JBFamilyName = "ATX_JointBoard";
		public const string JBPlaneFamilyName = "ATX_JointBoard_A";

		/// <summary>
		/// Joint Board Family
		/// </summary>
		public static Family JB = null;

		/// <summary>
		/// Joint Board Family which has Angles on the both ends
		/// </summary>
		public static Family JB_Plane = null;

		/// <summary>
		/// Joint Board depth parameter
		/// </summary>
		protected int m_nDepth = 20;
		/// <summary>
		/// Joint Board Side A gap parameter
		/// </summary>
		protected int m_nSideAGap = 2;
		/// <summary>
		/// Joint Board Side B gap parameter
		/// </summary>
		protected int m_nSideBGap = 2;
		/// <summary>
		/// Joint Board overall width parameter
		/// </summary>
		protected int m_nOverallWidth = 200;
		/// <summary>
		/// Joint Board gap between panels parameter
		/// </summary>
		protected int m_nGapBetweenPanels = 1;
		/// <summary>
		/// Part recess depth
		/// </summary>
		protected int m_nPartRecessDepth = 0;
		/// <summary>
		/// Flip side
		/// </summary>
		protected bool m_bFlip = false;
		/// <summary>
		/// Flag to add horizontal Joint Board to Lintel & Sill option
		/// </summary>
		protected bool m_bAddHorzJB = false;
		/// <summary>
		///	Material name to be set to the Joint Board
		/// </summary>
		protected string m_sMaterial = "";

		protected float m_fExcludeMinLimitLength = 0.0f;

		/// <summary>
		/// Joint Board depth parameter
		/// </summary>
		public int JointBoardDepth { get => m_nDepth; set => m_nDepth = value; }
		/// <summary>
		/// Joint Board Side A gap parameter
		/// </summary>
		public int SideAGap { get => m_nSideAGap; set => m_nSideAGap = value; }
		/// <summary>
		/// Joint Board Side B gap parameter
		/// </summary>
		public int SideBGap { get => m_nSideBGap; set => m_nSideBGap = value; }
		/// <summary>
		/// Joint Board overall width parameter
		/// </summary>
		public int OverallWidth { get => m_nOverallWidth; set => m_nOverallWidth = value; }
		/// <summary>
		/// Joint Board gap between panels parameter
		/// </summary>
		public int GapBetweenPanels { get => m_nGapBetweenPanels; set => m_nGapBetweenPanels = value; }
		public int PartRecessDepth { get => m_nPartRecessDepth; set => m_nPartRecessDepth = value; }

		public bool Flip { get => m_bFlip; set => m_bFlip = value; }
		public bool AddHorzJB { get => m_bAddHorzJB; set => m_bAddHorzJB = value; }
		public string Material { get => m_sMaterial; set => m_sMaterial = value; }

		public float ExcludeMinLimitLength { get => m_fExcludeMinLimitLength; set => m_fExcludeMinLimitLength = value; }

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

		protected class JBPart
		{
			public ElementId HostId;
			public ElementId Id;
			public List<Face> Faces;
			public double Thickness;
		}

		protected class SketchLine
		{
			public Curve Curve;
			public List<JBPart> Parts;
			public List<Face> CollidingFaces;
			public List<Curve> CollidingCurves;
			public XYZ Direction;
			public int Reserved = 0;

			public void AddJBPart(JBPart jbPart, Face face, Curve curve)
			{
				int i;
				for (i = 0; i < Parts.Count; i++)
				{
					if (Parts[i].Id == jbPart.Id)
						break;
				}
				if (i == Parts.Count)
				{
					Parts.Add(jbPart);
					CollidingFaces.Add(face);
					CollidingCurves.Add(curve);
				}
			}
		}
		/// <summary>
		/// Sketch Lines
		/// </summary>
		protected List<SketchLine> m_SketchLines = new();

		public class JBInstance
		{
			public Document Doc;
			public ElementId Id;
			public int Flag = 0;
			public double JointBoardDepth = 0.0;
			public double PartRecessDepth = 0.0;
			public double OverallWidth = 0.0;
			public double SideAGap = 0.0;
			public double SideBGap = 0.0;
			public double GapBetweenPanels = 0.0;
			public bool Flip;
		}
		private static readonly List<JBInstance> m_JBInstances = new();

		public List<string> GetMaterialNames()
		{
			List<Material> materials = new List<Material>(
				new FilteredElementCollector(m_uiApp.ActiveUIDocument.Document)
					  .WhereElementIsNotElementType()
					  .OfClass(typeof(Material))
					  .ToElements()
					  .Cast<Material>());

			List<string> names = new List<string>();
			foreach (Material mat in materials)
			{
				names.Add(mat.Name);
			}
			return names;
		}

		/// <summary>
		/// Initialize and prepare the tool
		/// </summary>
		public override void Initialize()
		{
			int i, j;
			Document doc = m_uiApp.ActiveUIDocument.Document;

			//  Initialize Connector controller
			if (!CheckJBFamily())
			{
				return;
			}

			//  Get Part and their parent elements
			m_SketchLines.Clear();

			List<ElementId> hostIds = new();

			List<JBPart> parts = new();
			//	m_Elements is the array of selected parts
			//	The tool gets extra information(host id, face list and thickness of the part) from them and add to jbParts array
			//	We exclude some unnecessary faces which can be ignored from the calculation. (Exterior/interior faces of walls, Top and bottom faces of slabs)
			for (i = 0; i < m_Elements.Count; i++)
			{
				Element host = Util.GetSourceElementOfPart(doc, m_Elements[i] as Part);
				if (!hostIds.Contains(host.Id))
					hostIds.Add(host.Id);

				JBPart part = new()
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

								//	Check with boundary lines of baseFace
								foreach (Curve c in Util.GetOptimizedBoundaryCurves(nMainPart == i ? baseFace1 : baseFace2))
								{
									if (c.Intersect(nMainPart == i ? result1 : result2) == SetComparisonResult.Equal)
									{
										if (Util.IsEqual(c.Length, (nMainPart == i ? result1 : result2).Length))
										{
											AddSketchLine(result1, result2, f1, f2, parts[i], parts[j]);
											break;
										}
										else
										{
											if (nMainPart == i)
												AddSketchLine(c, result2, f1, f2, parts[i], parts[j]);
											else
												AddSketchLine(result1, c, f1, f2, parts[i], parts[j]);
										}
									}
								}
							}
						}
					}
				}
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

					for (j = 0; j < m_SketchLines.Count; j++) {
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
			// 			Transaction trans = new Transaction(doc);
			// 			trans.Start("Show Sketch lines");
			// 			List<ElementId> mcids = new();
			// 			foreach(SketchLine sl in m_SketchLines)
			// 			{
			// 				ModelCurve mc = doc.Create.NewModelCurve(sl.Curve, SketchPlane.Create(doc, Plane.CreateByThreePoints(sl.Curve.GetEndPoint(0), sl.Curve.GetEndPoint(1), new XYZ(0, 0, 0))));
			// 				mcids.Add(mc.Id);
			// 			}
			// 			doc.ActiveView.IsolateElementsTemporary(mcids);
			// 			trans.Commit();
#endif
		}

		private bool AddSketchLine(Curve result1, Curve result2, Face f1, Face f2, JBPart part1, JBPart part2)
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
						Parts = new List<JBPart>() {
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
						m_SketchLines[nStart].AddJBPart(part1, f1, result1);
						m_SketchLines[nStart].AddJBPart(part2, f2, result2);
					}
					else
					{
						m_SketchLines[nEnd].Curve = Line.CreateBound(ptStart, m_SketchLines[nEnd].Curve.GetEndPoint(1));
						m_SketchLines[nEnd].AddJBPart(part1, f1, result1);
						m_SketchLines[nEnd].AddJBPart(part2, f2, result2);
					}
				}
				else
				{
					m_SketchLines[nStart].Curve = Line.CreateBound(m_SketchLines[nStart].Curve.GetEndPoint(0), m_SketchLines[nEnd].Curve.GetEndPoint(1));
					for (k = 0; k < m_SketchLines[nEnd].Parts.Count; k++)
					{
						m_SketchLines[nStart].AddJBPart(m_SketchLines[nEnd].Parts[k], m_SketchLines[nEnd].CollidingFaces[k], m_SketchLines[nEnd].CollidingCurves[k]);
					}

					m_SketchLines[nStart].AddJBPart(part1, f1, result1);
					m_SketchLines[nStart].AddJBPart(part2, f2, result2);
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
			int i, j;
			//	Sort sketch lines by linked part counts
			for (i = 0; i < m_SketchLines.Count; i++)
			{
				for (j = i + 1; j < m_SketchLines.Count; j++)
				{
					if (m_SketchLines[i].Curve.Length < m_SketchLines[j].Curve.Length)
					//if(m_SketchLines[i].JBParts.Count < m_SketchLines[j].JBParts.Count)
					{
						SketchLine sl = m_SketchLines[i];
						m_SketchLines[i] = m_SketchLines[j];
						m_SketchLines[j] = sl;
					}
				}
			}

			//  Remove duplicated sketch lines
			for (i = 0; i < m_SketchLines.Count; i++)
			{
				for (j = i + 1; j < m_SketchLines.Count; j++)
				{
					if (m_SketchLines[i].Curve.Intersect(m_SketchLines[j].Curve, out IntersectionResultArray ira) == SetComparisonResult.Overlap)
					{
						XYZ intersectPt = ira.get_Item(0).XYZPoint;
						if (!Util.IsEqual(intersectPt, m_SketchLines[i].Curve.GetEndPoint(0))
							&& !Util.IsEqual(intersectPt, m_SketchLines[i].Curve.GetEndPoint(1)))
						{
							m_SketchLines.RemoveAt(j);
							j--;
						}
					}
				}
			}

			//  Create instances by Sketch Lines
			for (i = 0; i < m_SketchLines.Count; i++)
			{
				CreateJBInstance(m_SketchLines[i]);
			}
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
						Document doc = m_uiApp.ActiveUIDocument.Document;
						TransactionGroup tg = new(doc);

						tg.Start("Generate JBs");
						int nDump = 0;
						while ((nDump == 0 || m_SketchLines.Count > 0) && nDump++ < 5)
						{
							Initialize();

							if (JB == null || JB_Plane == null)
							{
								TaskDialog.Show("Error", "Can't find the family. Please contact developer.");
								return false;
							}

							GetConnections();
						}

						tg.Assimilate();
						return false;
					default:
						break;
				}
				return true;
			}
			catch(Exception)
			{
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

				//				Initialize();
			}
			catch (Exception ex)
			{
				Console.Write(ex.Message);
			}
		}

		/// <summary>
		/// Create Joint Board Family Instance
		/// </summary>
		protected void CreateJBInstance(SketchLine sl)
		{
			#region Convert Unit of the input parameters
			double fSideAGap = UnitUtils.ConvertToInternalUnits(m_nSideAGap, m_UnitType);
			double fSideBGap = UnitUtils.ConvertToInternalUnits(m_nSideBGap, m_UnitType);
			double fOverallWidth = UnitUtils.ConvertToInternalUnits(m_nOverallWidth, m_UnitType);
			double fGapBetweenPanels = UnitUtils.ConvertToInternalUnits(m_nGapBetweenPanels, m_UnitType);
			double fJointBoardDepth = UnitUtils.ConvertToInternalUnits(m_nDepth, m_UnitType);
			double fPartRecessDepth = UnitUtils.ConvertToInternalUnits(m_nPartRecessDepth, m_UnitType);
			#endregion

			Document doc = m_uiApp.ActiveUIDocument.Document;

			Transaction trans = new(doc);
			trans.Start("Creating JBs");

			FamilyInstance fiJB;

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
				XYZ ptStart = sl.Curve.GetEndPoint(0),
					ptEnd = sl.Curve.GetEndPoint(1);
				location = Line.CreateBound(ptStart, ptEnd);

				XYZ direction = (ptEnd - ptStart).Normalize();

				direction = direction.CrossProduct(normal).Normalize();

				//	Get the thicker thickness
				double fThickness = sl.Parts[0].Thickness;
				foreach (JBPart jbPart in sl.Parts)
				{
					if (jbPart.Thickness > fThickness)
					{
						fThickness = jbPart.Thickness;
						face = Util.GetBaseFaceOfPart(doc, doc.GetElement(jbPart.Id) as Part);
					}
				}

				//	Joint Board
				double fAngle1 = -10.0, fAngle2 = -10.0;

				IntersectionResultArray ira = null;
				foreach (JBPart jbPart in sl.Parts)
				{
					Part part = doc.GetElement(jbPart.Id) as Part;
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

				//	Adjust the location to work with other jbs
				/*Curve minCurve = null;
				foreach (JBPart jbPart in sl.JBParts)
				{
					Part part = doc.GetElement(jbPart.Id) as Part;
					int nDirection = Util.GetDirectionOfPart(doc, location, part);
					foreach (Curve c in Util.GetOptimizedBoundaryCurves(Util.GetBaseFaceOfPart(doc, part)))
					{
						if ((c.Intersect(location) == SetComparisonResult.Equal
							|| c.Intersect(location) == SetComparisonResult.Superset
							|| c.Intersect(location) == SetComparisonResult.Subset)
							&& (minCurve == null || c.Length < minCurve.Length))
						{
							minCurve = c;
						}
					}
				}
				location = minCurve;*/

				//	Get Length1 and Length2
				Curve minCurve = null;
				foreach (JBPart jbPart in sl.Parts)
				{
					Part part = doc.GetElement(jbPart.Id) as Part;
					int nDirection = Util.GetDirectionOfPart(doc, location, part);
					//if (nDirection < 0) continue;
					Face bottomFace = Util.GetBaseFaceOfPart(doc, part, false);
					Plane p = bottomFace.GetSurface() as Plane;
					Curve projLoc = Line.CreateUnbound(p.ProjectOnto(location.GetEndPoint(0)), p.ProjectOnto(location.GetEndPoint(1)) - p.ProjectOnto(location.GetEndPoint(0)));
					foreach (Curve c in Util.GetOptimizedBoundaryCurves(bottomFace))
					{
						if (c.Intersect(projLoc) == SetComparisonResult.Equal
							|| c.Intersect(projLoc) == SetComparisonResult.Superset
							|| c.Intersect(projLoc) == SetComparisonResult.Subset)
						{
							if (minCurve == null)
								minCurve = c;
							else
							{
								if (!Util.IsZero(minCurve.Distance(c.GetEndPoint(0))))
								{
									if (c.GetEndPoint(0).DistanceTo(minCurve.GetEndPoint(0)) < c.GetEndPoint(0).DistanceTo(minCurve.GetEndPoint(1)))
									{
										minCurve = Line.CreateBound(c.GetEndPoint(0), minCurve.GetEndPoint(1));
									}
									else
									{
										minCurve = Line.CreateBound(minCurve.GetEndPoint(0), c.GetEndPoint(0));
									}
								}

								if (!Util.IsZero(minCurve.Distance(c.GetEndPoint(1))))
								{
									if (c.GetEndPoint(1).DistanceTo(minCurve.GetEndPoint(0)) < c.GetEndPoint(1).DistanceTo(minCurve.GetEndPoint(1)))
									{
										minCurve = Line.CreateBound(c.GetEndPoint(1), minCurve.GetEndPoint(1));
									}
									else
									{
										minCurve = Line.CreateBound(minCurve.GetEndPoint(0), c.GetEndPoint(1));
									}
								}
							}
						}
					}
				}
				if (minCurve == null)
				{
					minCurve = location;
				}

				XYZ ptCenter;
				if (!Util.IsEqual((minCurve.GetEndPoint(1) - minCurve.GetEndPoint(0)).Normalize(), (location.GetEndPoint(1) - location.GetEndPoint(0)).Normalize()))
				{
					minCurve = minCurve.CreateReversed();
				}
				ptStart = minCurve.GetEndPoint(0);
				ptEnd = minCurve.GetEndPoint(1);
				ptCenter = minCurve.Project((location.GetEndPoint(1) + location.GetEndPoint(0)) / 2).XYZPoint;

				XYZ pos = (location.GetEndPoint(1) + location.GetEndPoint(0)) / 2;
				FamilyInstance instance = doc.Create.NewFamilyInstance(
					face
					, pos
					, direction
					, (fAngle1 == 0.0 && fAngle2 == 0.0) ? GetFamilySymbol(JB) : GetFamilySymbol(JB_Plane)
					);

				Parameter param = instance.LookupParameter("JointBoardDepth");
				param.Set(fJointBoardDepth);
				param = instance.LookupParameter("PartRecessDepth");
				param.Set(fPartRecessDepth);
				param = instance.LookupParameter("Thickness");
				param.Set(fThickness);
				param = instance.LookupParameter("SideAGap");
				param.Set(fSideAGap);
				param = instance.LookupParameter("SideBGap");
				param.Set(fSideBGap);
				param = instance.LookupParameter("OverallWidth");
				param.Set(fOverallWidth);
				param = instance.LookupParameter("GapBetweenPanels");
				param.Set(fGapBetweenPanels);
				param = instance.LookupParameter("Length");
				param.Set(location.Length);
				if (Util.GetSourceElementOfPart(doc, doc.GetElement(sl.Parts[0].Id) as Part) is Wall)
				{
					param = instance.LookupParameter("Length1");
					param.Set(location.Length / 2);
					param = instance.LookupParameter("Length2");
					param.Set(location.Length / 2);
				}
				else
				{
					param = instance.LookupParameter("Length1");
					param.Set(ptEnd.DistanceTo(ptCenter));
					param = instance.LookupParameter("Length2");
					param.Set(ptStart.DistanceTo(ptCenter));
				}
				param = instance.LookupParameter("Angle1");
				param.Set(fAngle1);
				param = instance.LookupParameter("Angle2");
				param.Set(fAngle2);
				if (m_sMaterial != "")
				{
					param = instance.LookupParameter("JB Material");
					List<Material> materials = new List<Material>(
						new FilteredElementCollector(doc)
						  .WhereElementIsNotElementType()
						  .OfClass(typeof(Material))
						  .ToElements()
						  .Where<Element>(m
						   => m.Name == m_sMaterial)
						  .Cast<Material>());
					if (materials.Count == 1)
					{
						param.Set(materials[0].Id);
					}
				}
				fiJB = instance;
				doc.Regenerate();

				foreach (JBPart jbPart in sl.Parts)
				{
					Part part = doc.GetElement(jbPart.Id) as Part;
					if (InstanceVoidCutUtils.CanBeCutWithVoid(part))
					{
						try
						{
							InstanceVoidCutUtils.AddInstanceVoidCut(doc, part, fiJB);
						}
						catch (Exception e) { TaskDialog.Show("Error", e.Message); }
					}
				}
				trans.Commit();
				if (m_bFlip)
				{
					UpdateParameter(doc
						, fiJB
						, fSideAGap
						, fSideBGap
						, fOverallWidth
						, fGapBetweenPanels
						, fJointBoardDepth
						, fPartRecessDepth
						, param.AsElementId()
						, m_bFlip);
				}
			}
			catch (Exception e)
			{
				TaskDialog.Show("Error", e.Message);
				Debug.WriteLine(e.StackTrace);
				trans.RollBack();
			}
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

		#region Create necessary families
		/// <summary>
		/// Check if Joint Board family is already created
		/// </summary>
		private bool CheckJBFamily()
		{
			Document doc = m_uiApp.ActiveUIDocument.Document;

			if (JB != null && !JB.IsValidObject) JB = null;
			if (JB_Plane != null && !JB_Plane.IsValidObject) JB_Plane = null;

			if (JB != null && JB_Plane != null)
			{
				return true;
			}

			FilteredElementCollector collector = new FilteredElementCollector(doc);
			ICollection<Element> collection = collector.OfClass(typeof(Family)).ToElements();

			foreach (Element e in collection)
			{
				Family f = e as Family;
				if (f.Name == JBFamilyName)
				{
					JB = f;
				}
				else if (f.Name == JBPlaneFamilyName)
				{
					JB_Plane = f;
				}
			}

			if (JB != null && JB_Plane != null)
			{
				return true;
			}
			else
			{
				Transaction trans = new(doc);
				trans.Start("Load Family");

				string url = Assembly.GetExecutingAssembly().Location;

				//	Get the url of the plugin
				url = url.Substring(0, url.LastIndexOf("\\")) + "\\" + JBFamilyName + ".rfa";
				bool bRet1 = doc.LoadFamily(url, out JB);
				url = url.Substring(0, url.LastIndexOf("\\")) + "\\" + JBPlaneFamilyName + ".rfa";
				bool bRet2 = doc.LoadFamily(url, out JB_Plane);
				if (bRet1 && bRet2)
				{
					trans.Commit();
				}
				else
				{
					trans.RollBack();
				}
				return bRet1 && bRet2;
			}
		}
		#endregion

		public static void UpdateParameter(
			Document doc
			, FamilyInstance instance
			, double fSideAGap
			, double fSideBGap
			, double fOverallWidth
			, double fGapBetweenPanels
			, double fJointBoardDepth
			, double fPartRecessDepth
			, ElementId material
			, bool bFlip)
		{
			Parameter param;

			try
			{
				Transaction trans = new(doc);
				trans.Start("Update Parameters");
				if (bFlip)
				{
					FamilySymbol symbol = instance.Symbol;
					LocationPoint lp = instance.Location as LocationPoint;

					param = instance.LookupParameter("Angle1");
					double fAngle1 = Math.PI - param.AsDouble();
					if (Util.IsEqual(fAngle1, Math.PI)) fAngle1 = 0;
					param = instance.LookupParameter("Angle2");
					double fAngle2 = Math.PI - param.AsDouble();
					if (Util.IsEqual(fAngle2, Math.PI)) fAngle2 = 0;
					param = instance.LookupParameter("Length1");
					double fLength1 = param.AsDouble();
					param = instance.LookupParameter("Length2");
					double fLength2 = param.AsDouble();
					param = instance.LookupParameter("Length");
					double fLength = param.AsDouble();
					param = instance.LookupParameter("Thickness");
					double fThickness = param.AsDouble();

					//	Get host
					Reference refer = instance.HostFace;
					Element parent = doc.GetElement(refer);

					PlanarFace hostFace = parent.GetGeometryObjectFromReference(refer) as PlanarFace;
					Face similarFace = null;
					//	Get the corresponding face (Find the most similar face)
					Options options = new()
					{
						ComputeReferences = true
					};
					GeometryElement geomElem = parent.get_Geometry(options);
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
						lp.Point,
						instance.HandOrientation,
						symbol);
					param = newinstance.LookupParameter("Angle1");
					param.Set(fAngle2);
					param = newinstance.LookupParameter("Angle2");
					param.Set(fAngle1);
					param = newinstance.LookupParameter("Length1");
					param.Set(fLength2);
					param = newinstance.LookupParameter("Length2");
					param.Set(fLength1);
					param = newinstance.LookupParameter("Length");
					param.Set(fLength);
					param = newinstance.LookupParameter("Thickness");
					param.Set(fThickness);
					param = newinstance.LookupParameter("JB Material");
					if (material != null)
						param.Set(material);

					ICollection<ElementId> elemIds = InstanceVoidCutUtils.GetElementsBeingCut(instance);
					foreach (ElementId eId in elemIds)
					{
						InstanceVoidCutUtils.AddInstanceVoidCut(doc, doc.GetElement(eId), newinstance);
					}

					doc.Delete(instance.Id);
					instance = newinstance;
				}

				param = instance.LookupParameter("JointBoardDepth");
				param.Set(fJointBoardDepth);
				param = instance.LookupParameter("PartRecessDepth");
				param.Set(fPartRecessDepth);
				param = instance.LookupParameter("SideAGap");
				param.Set(fSideAGap);
				param = instance.LookupParameter("SideBGap");
				param.Set(fSideBGap);
				param = instance.LookupParameter("OverallWidth");
				param.Set(fOverallWidth);
				param = instance.LookupParameter("GapBetweenPanels");
				param.Set(fGapBetweenPanels);
				param = instance.LookupParameter("Flip");
				param.Set(bFlip ? 1 : 0);

				trans.Commit();
			}
			catch (Exception)
			{
			}
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
					for (int i = m_JBInstances.Count - 1; i >= 0; i--)
					{
						JBInstance jb = m_JBInstances[i];
						if (elems.Contains(jb.Id))
						{
							m_JBInstances.Remove(jb);
						}
					}
				}

				elems = args.GetAddedElementIds();
				if (elems.Count > 0)
				{
					foreach (ElementId eId in elems)
					{
						Element e = doc.GetElement(eId);
						if (e is FamilyInstance ins && ins.Name.Contains(JBFamilyName))
						{
							JBInstance jb = new()
							{
								Doc = doc,
								Id = ins.Id,
								JointBoardDepth = ins.LookupParameter("JointBoardDepth").AsDouble(),
								PartRecessDepth = ins.LookupParameter("PartRecessDepth").AsDouble(),
								SideAGap = ins.LookupParameter("SideAGap").AsDouble(),
								SideBGap = ins.LookupParameter("SideBGap").AsDouble(),
								GapBetweenPanels = ins.LookupParameter("GapBetweenPanels").AsDouble(),
								Flip = ins.LookupParameter("Flip").AsInteger() == 1
							};
							m_JBInstances.Add(jb);
						}
					}
				}

				// we check if the element was edited
				ICollection<ElementId> elemIds = args.GetModifiedElementIds();
				if (elemIds.Count > 0)
				{
					foreach (JBInstance jb in m_JBInstances)
					{
						if (elemIds.Contains(jb.Id))
						{
							jb.Flag = 2;
							bHas = true;
						}
					}
				}

				return bHas;
				//if (bHas)
				//{
				//	Application.thisApp.GetUIContApp().Idling += OnIdlingEvent;
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
			//	Remove unnecessary HL instances of closed documents
			for (int i = m_JBInstances.Count - 1; i >= 0; i--)
			{
				if (!m_JBInstances[i].Doc.IsValidObject)
					m_JBInstances.RemoveAt(i);
			}

			Document doc = args.Document;
			if (doc == null)
				return;

			List<FamilyInstance> totalJBs = new(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(FamilyInstance))
					.Where(ins => ins.Name.Contains(JBFamilyName))
					.ToList()
					.Cast<FamilyInstance>()
					);
			foreach (FamilyInstance ins in totalJBs)
			{
				JBInstance jb = new()
				{
					Doc = doc,
					Id = ins.Id,
					JointBoardDepth = ins.LookupParameter("JointBoardDepth").AsDouble(),
					PartRecessDepth = ins.LookupParameter("PartRecessDepth").AsDouble(),
					SideAGap = ins.LookupParameter("SideAGap").AsDouble(),
					SideBGap = ins.LookupParameter("SideBGap").AsDouble(),
					GapBetweenPanels = ins.LookupParameter("GapBetweenPanels").AsDouble(),
					Flip = ins.LookupParameter("Flip").AsInteger() == 1
				};

				m_JBInstances.Add(jb);
			}
		}

		public static void OnIdlingEvent(object sender, IdlingEventArgs e)
		{
			UIApplication uiApp = sender as UIApplication;
			Document doc = uiApp.ActiveUIDocument.Document;

			TransactionGroup tg = new(doc);
			tg.Start("JB Management");
			try
			{
				for (int i = m_JBInstances.Count - 1; i >= 0; i--)
				{
					JBInstance jb = m_JBInstances[i];
					FamilyInstance ins = doc.GetElement(jb.Id) as FamilyInstance;

					if (jb.Flag == 2)
					{
						jb.Flag = 0;
						bool bFlip = jb.Flip != (ins.LookupParameter("Flip").AsInteger() == 1);

						UpdateParameter(doc
							, ins
							, ins.LookupParameter("SideAGap").AsDouble()
							, ins.LookupParameter("SideBGap").AsDouble()
							, ins.LookupParameter("OverallWidth").AsDouble()
							, ins.LookupParameter("GapBetweenPanels").AsDouble()
							, ins.LookupParameter("JointBoardDepth").AsDouble()
							, ins.LookupParameter("PartRecessDepth").AsDouble()
							, ins.LookupParameter("JB Material").AsElementId()
							, bFlip);
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
