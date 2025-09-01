using Architexor.Core;
using Autodesk.Revit.DB;
using Architexor.Base;
using Architexor.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Color = System.Drawing.Color;
using Settings = Architexor.BasicSplit.Base.Settings;
using Splitter = Architexor.BasicSplit.Splitters.Splitter;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Architexor.BasicSplit
{
	abstract public class CSplitElement : CElement
	{
		//	Input Parameters
		/// <summary>
		/// Standard panel split width
		/// </summary>
		protected int m_nStandardSplitWidth = 0;
		/// <summary>
		/// Lintel Bearing
		/// </summary>
		protected int m_nLintelBearing = 0;
		/// <summary>
		/// Sill Bearing
		/// </summary>
		protected int m_nCillBearing = 0;
		/// <summary>
		/// Width for Around Centre option
		/// </summary>
		protected int m_nAroundCentreSplitWidth = 0;

		/// <summary>
		/// Parameter to determine to select the start point or not
		/// This parameter tells the tool will split from left to right or right to left when user selected multiple elements.
		///		false : right to left
		///		true : left to right
		/// </summary>
		protected bool m_bMustSelectStartPoint = false;
		/// <summary>
		/// The point which indicates where start the split from
		/// </summary>
		protected XYZ m_ptStartPoint = null;

		public int StandardSplitWidth { get => m_nStandardSplitWidth; set => m_nStandardSplitWidth = value; }
		public int LintelBearing { get => m_nLintelBearing; set => m_nLintelBearing = value; }
		public int CillBearing { get => m_nCillBearing; set => m_nCillBearing = value; }
		public int AroundCentreSplitWidth { get => m_nAroundCentreSplitWidth; set => m_nAroundCentreSplitWidth = value; }
		public bool MustSelectStartPoint { get => m_bMustSelectStartPoint; set => m_bMustSelectStartPoint = value; }
		public XYZ StartPoint { get => m_ptStartPoint; set => m_ptStartPoint = value; }
		public bool SplitFromLeftToRight { get; set; } = true;

		/// <summary>
		/// Splitter instance handler
		/// </summary>
		protected Splitter m_Splitter;
		/// <summary>
		/// Plane which the sketch lines will be placed on
		/// </summary>
		public Plane SketchPlane { get; set; }
		/// <summary>
		/// Faces which the sketch lines will be placed on
		/// Wall => Exterior face
		/// Slab => Top faces
		/// </summary>
		public Face[] BaseFaces { get; set; }
		/// <summary>
		/// BoundaryCurves of the BaseFaces
		/// </summary>
		public List<CurveLoop> BoundaryCurves { get; set; } = new List<CurveLoop>();
		public List<CurveLoop> BoundaryCurvesOfOpenings { get; set; } = new List<CurveLoop>();
		/// <summary>
		/// Location Curve which indicates the start point and end point of the base line
		/// </summary>
		public Curve LocationCurve { get; set; }
		/// <summary>
		/// Perpendicular Vector to the LocationCurve(Normalized)
		/// </summary>
		public XYZ PV { get; set; }
		/// <summary>
		/// This will be used to determine how long the split lines are
		/// We get this value to get the maximum height which is perpendicular to the LocationCurve.
		/// </summary>
		public double MaxHeight { get; set; }
#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024 || REVIT2025)
		protected double SplitMargin { get; set; } = UnitUtils.ConvertToInternalUnits(300.0, UnitTypeId.Millimeters);
#else
		protected double SplitMargin { get; set; } = UnitUtils.ConvertToInternalUnits(300.0, DisplayUnitType.DUT_MILLIMETERS);
#endif

		public List<Element> AdjacentElements { get; set; } = new List<Element>();
		public List<COpening> AssociatedOpenings { get; set; } = new List<COpening>();
		/// <summary>
		/// Openings array associated to this element and selected to split
		/// </summary>
		public List<COpening> SelectedOpenings { get; set; } = new List<COpening>();
		/// <summary>
		/// Split points array
		/// </summary>
		public List<SSplitPoint> SplitPoints { get; set; } = new List<SSplitPoint>();
		/// <summary>
		/// Split lines array
		/// </summary>
		public List<SSplitLine> SplitLines { get; set; } = new List<SSplitLine>();
		/// <summary>
		/// Array of the panels split
		/// </summary>
		public List<CPanel> Panels { get; set; } = new List<CPanel>();

		public double DifferenceBetweenNetAndGross { get; set; }

		public Bitmap PreviewBitmap = null;
		public Image Thumbnail = null;

		protected int m_nOriginalPartCount = 0;

		protected CSplitElement() { }

		public CSplitElement(Splitter splitter, Element e)
		{
			Element = e;

			m_Splitter = splitter;

			if(!GetBaseFace())
			{
				throw new Exception("Can't get base face.");
			}
			GetSketchPlane();
			GetBoundaryCurves();
			GetLocationCurve();
			if(LocationCurve.Length < UnitUtils.ConvertToInternalUnits(Settings.MinimumPanelWidth, CCommonSettings.Unit))
			{
				//	TODO : Use user-defined variable
				throw new Exception("We ignore small elements.");
			}
			GetAssociatedElements();

			//	Extract information if this element is already split
			GetExistingPanels();
			ExtractSplitInformation();

			//	Draw Preview
			DrawPreview();
		}

		protected const int PREVIEW_WIDTH = 800;
		protected const int PREVIEW_HEIGHT = 500;

		private PointF ConvertTo2D(XYZ pt, double fFactor, double fMaxWidth)
		{
			int nPadding = 10;
			return new PointF()
			{
				X = (float)(pt.X * fFactor + nPadding + (PREVIEW_WIDTH - fMaxWidth * fFactor) / 2),
				Y = (float)(PREVIEW_HEIGHT - pt.Y * fFactor - 25)
			};
		}

		public bool IsSplitable()
		{
			return m_nOriginalPartCount < 2;
		}

		public void DrawPreview()
		{
			PreviewBitmap = new Bitmap(PREVIEW_WIDTH, PREVIEW_HEIGHT);
			using (Graphics g = Graphics.FromImage(PreviewBitmap))
			{
				g.Clear(Color.White);

				Font dbgFont = new Font(Constants.FontFamily, 8);
				Brush brush;

				int i;
				Pen pen = new Pen(Settings.PreviewSettings.ElementColor);

				double fMaxWidth = (LocationCurve.GetEndPoint(1) - LocationCurve.GetEndPoint(0)).GetLength();
				//	Get Factor
				double fFactor = (double)PREVIEW_WIDTH / fMaxWidth;
				if (fFactor > (double)PREVIEW_HEIGHT / MaxHeight)
				{
					fFactor = (double)PREVIEW_HEIGHT / MaxHeight;
				}
				fFactor *= 0.8;

				try
				{
					//	Draw panel backgrounds
					brush = new SolidBrush(Color.FromArgb(242, 220, 189));
					if (Settings.PreviewSettings.AreaType == 1)
					{
						//	Gross
#if REVIT2019
						PointF[] pts = new PointF[BoundaryCurves[0].Count()];
#else
						PointF[] pts = new PointF[BoundaryCurves[0].NumberOfCurves()];
#endif
						int j = 0;
						foreach (Curve c in BoundaryCurves[0])
						{
							pts[j++] = ConvertTo2D(GetPoint2D(c.GetEndPoint(0)), fFactor, fMaxWidth);
						}
						g.FillPolygon(brush, pts);

						(brush as SolidBrush).Color = Color.White;
						//	Get Associated openings
						foreach (COpening sO in SelectedOpenings)
						{
							if (sO.Option == OpeningSelectOption.LintelAndSill)
							{
								COpening co = sO;
								pts = new PointF[co.Points.Length];
								for (j = 0; j < co.Points.Length; j++)
								{
									pts[j] = ConvertTo2D(GetPoint2D(co.Points[j]), fFactor, fMaxWidth);
								}
								g.FillPolygon(brush, pts);
							}
						}
					}
					else
					{
						if (Panels.Count > 0)
						{
							for (i = 0; i < Panels.Count; i++)
							{
								CPanel panel = Panels[i];
								if (Settings.PreviewSettings.AreaType == 2)
								{
									PointF min = ConvertTo2D(panel.MinPoint2D, fFactor, fMaxWidth),
										max = ConvertTo2D(panel.MaxPoint2D, fFactor, fMaxWidth);

									g.FillRectangle(brush,
										min.X, max.Y,
										max.X - min.X, min.Y - max.Y
										);
								}
								else
								{
									PointF[] pts = new PointF[panel.Points.Length];
									for (int j = 0; j < panel.Points.Length; j++)
									{
										pts[j] = ConvertTo2D(panel.Point2Ds[j], fFactor, fMaxWidth);
									}
									g.FillPolygon(brush, pts);
								}
							}
						}

						if (Settings.PreviewSettings.AreaType == 0)
						{
							(brush as SolidBrush).Color = Color.White;
							//	Get Associated openings
							foreach (COpening co in AssociatedOpenings)
							{
								PointF[] pts = new PointF[co.Points.Length];
								for (int j = 0; j < co.Points.Length; j++)
								{
									pts[j] = ConvertTo2D(GetPoint2D(co.Points[j]), fFactor, fMaxWidth);
								}
								g.FillPolygon(brush, pts);
							}
						}
					}

					//	Draw boundary curves
					int nNo = 0;
					foreach (CurveLoop cl in BoundaryCurves)
					{
						foreach (Curve c in cl)
						{
							PointF pt1 = ConvertTo2D(GetPoint2D(c.GetEndPoint(0))
								, fFactor, fMaxWidth)
								, pt2 = ConvertTo2D(GetPoint2D(c.GetEndPoint(1)), fFactor, fMaxWidth);
							g.DrawLine(pen, pt1, pt2);

							if (Settings.PreviewSettings.BoundaryPointColor != Color.Transparent)
							{
								nNo++;
								pt1 = ConvertTo2D(GetPoint2D(c.GetEndPoint(0)), fFactor, fMaxWidth);

								g.DrawString(nNo.ToString(), dbgFont, new SolidBrush(Settings.PreviewSettings.BoundaryPointColor), pt1);
							}
						}
					}

					if (Settings.PreviewSettings.LocationCurveColor != Color.Transparent)
					{
						pen.Width = 2;
						pen.Color = Settings.PreviewSettings.LocationCurveColor;
						brush = new SolidBrush(Settings.PreviewSettings.LocationCurveColor);

						XYZ ptStart = LocationCurve.GetEndPoint(0),
							ptEnd = LocationCurve.GetEndPoint(1);
						ptEnd -= (ptEnd - ptStart) * 0.1;

						g.DrawLine(pen
							, ConvertTo2D(GetPoint2D(LocationCurve.GetEndPoint(0)), fFactor, fMaxWidth)
							, ConvertTo2D(GetPoint2D(LocationCurve.GetEndPoint(1)), fFactor, fMaxWidth));

						g.DrawString("   Start"
							, dbgFont
							, brush
							, ConvertTo2D(GetPoint2D(ptStart), fFactor, fMaxWidth));
						g.DrawString("   End(" + Util.RoundToRealWorld(UnitUtils.ConvertFromInternalUnits(LocationCurve.Length, CCommonSettings.Unit)) + ")"
							, dbgFont
							, brush
							, ConvertTo2D(GetPoint2D(ptEnd), fFactor, fMaxWidth));
					}

					//	Draw the openings
					pen.Width = 1;
					pen.Color = Settings.PreviewSettings.OpeningColor;
					foreach (CurveLoop cl in BoundaryCurvesOfOpenings)
					{
						foreach (Curve c in cl)
						{
							PointF pt1 = ConvertTo2D(GetPoint2D(c.GetEndPoint(0))
								, fFactor, fMaxWidth)
								, pt2 = ConvertTo2D(GetPoint2D(c.GetEndPoint(1)), fFactor, fMaxWidth);
							g.DrawLine(pen, pt1, pt2);
						}
					}

					//	Draw the split lines
					brush = new SolidBrush(Settings.PreviewSettings.DimensionColor);
					Font font = new Font(Constants.FontFamily, 10);
					if (SplitLines.Count > 0)
					{
						//pen.DashStyle = DashStyle.Dot;
						pen.Color = Settings.PreviewSettings.SplitLineColor;
						for (i = 0; i < SplitLines.Count; i++)
						{
							SSplitLine sSL = SplitLines[i];
							PointF ptStart = ConvertTo2D(GetPoint2D(sSL.Points[0]), fFactor, fMaxWidth)
								, ptEnd = ConvertTo2D(GetPoint2D(sSL.Points[1]), fFactor, fMaxWidth);
							if (!Util.IsEqual(sSL.Points[0], LocationCurve.GetEndPoint(1)))
							{
								//g.DrawLine(pen, ptStart, ptEnd);
								//	Write the Line order;
								if (Settings.PreviewSettings.SplitLineNumberColor != Color.Transparent)
								{
									g.DrawString(i.ToString()
										, dbgFont
										, new SolidBrush(Settings.PreviewSettings.SplitLineNumberColor)
										, (ptStart.X + ptEnd.X) / 2
										, (ptStart.Y + ptEnd.Y) / 2);

									if (sSL.ReferenceToSplitPoint == -1)
									{
										pen.Color = Color.Blue;
									}
									pen.Width = 3;
									g.DrawLine(pen
										, ptStart
										, ptEnd);
									pen.Width = 1;
									pen.Color = Color.Green;
								}
							}
						}
					}
					//	Draw Split Points
					if (Settings.PreviewSettings.SplitPointColor != Color.Transparent)
					{
						for (i = 0; i < SplitPoints.Count; i++)
						{
							g.DrawString(i.ToString()
								, dbgFont
								, new SolidBrush(Settings.PreviewSettings.SplitPointColor)
								, ConvertTo2D(GetPoint2D(SplitPoints[i].Point), fFactor, fMaxWidth));
						}
					}

					//	Draw Panel boundary curves
					pen = new Pen(Settings.PreviewSettings.PanelBoundaryColor);
					if (Panels.Count > 0)
					{
						int nHighlightIndex = -1;
						for (i = 0; i < Panels.Count; i++)
						{
							CPanel panel = Panels[i];
							PointF min = ConvertTo2D(panel.MinPoint2D, fFactor, fMaxWidth),
								max = ConvertTo2D(panel.MaxPoint2D, fFactor, fMaxWidth);

							//if (m_HighlightPos.X > min.X && m_HighlightPos.X < max.X
							//	&& m_HighlightPos.Y > max.Y && m_HighlightPos.Y < min.Y)
							//{
							//	nHighlightIndex = i;
							//}

							if (Settings.PreviewSettings.AreaType == 2)
							{
								g.DrawRectangle(pen,
									min.X, max.Y,
									max.X - min.X, min.Y - max.Y
									);
							}
							else
							{
								PointF[] pts = new PointF[panel.Points.Length];
								for (int j = 0; j < panel.Points.Length; j++)
								{
									pts[j] = ConvertTo2D(panel.Point2Ds[j], fFactor, fMaxWidth);
								}
								g.DrawPolygon(pen, pts);

								if (Settings.PreviewSettings.PanelPointColor != Color.Transparent)
								{
									//if (int.Parse(txtDebug_PanelIndex.Text) != i)
									//	continue;

									for (int j = 0; j < panel.Points.Length; j++)
									{
										g.DrawString(j.ToString(), dbgFont, new SolidBrush(Settings.PreviewSettings.PanelPointColor), pts[j]);
									}
								}
							}
						}

						if (nHighlightIndex >= 0)
						{
							//	Draw the dimensions
							int R = Settings.PreviewSettings.PanelBoundaryColor.R + 50;
							int G = Settings.PreviewSettings.PanelBoundaryColor.G + 50;
							int B = Settings.PreviewSettings.PanelBoundaryColor.B + 50;
							R = (R > 255) ? 255 : R;
							G = (G > 255) ? 255 : G;
							B = (B > 255) ? 255 : B;
							pen.Color = Color.FromArgb(R
								, G
								, B);

							CPanel panel = Panels[nHighlightIndex];
							PointF min = ConvertTo2D(panel.MinPoint2D, fFactor, fMaxWidth),
								max = ConvertTo2D(panel.MaxPoint2D, fFactor, fMaxWidth);

							//						if (m_HighlightPos.X > min.X && m_HighlightPos.X < max.X
							//							&& m_HighlightPos.Y > max.Y && m_HighlightPos.Y < min.Y)
							//						{
							//							nHighlightIndex = i;
							//						}

							if (Settings.PreviewSettings.AreaType == 2)
							{
								g.DrawRectangle(pen,
									min.X, max.Y,
									max.X - min.X, min.Y - max.Y
									);

								int nLength = Util.RoundToRealWorld(UnitUtils.ConvertFromInternalUnits(
									(max.X - min.X) / fFactor
									, CCommonSettings.Unit));
								g.DrawString(nLength.ToString()
									, dbgFont
									, brush
									, (max.X + min.X) / 2
									, min.Y);
								nLength = Util.RoundToRealWorld(UnitUtils.ConvertFromInternalUnits(
									(min.Y - max.Y) / fFactor
									, CCommonSettings.Unit));
								g.DrawString(nLength.ToString()
									, dbgFont
									, brush
									, max.X
									, (max.Y + min.Y) / 2);
							}
							else
							{
								PointF[] pts = new PointF[panel.Points.Length];
								for (int j = 0; j < panel.Points.Length; j++)
								{
									pts[j] = ConvertTo2D(panel.Point2Ds[j], fFactor, fMaxWidth);
								}
								g.DrawPolygon(pen, pts);
								for (int j = 0; j < pts.Length; j++)
								{
									PointF ptStart;
									if (j == 0) ptStart = pts[pts.Length - 1];
									else ptStart = pts[j - 1];

									int nLength = Util.RoundToRealWorld(UnitUtils.ConvertFromInternalUnits(Math.Sqrt((pts[j].X - ptStart.X) * (pts[j].X - ptStart.X) +
										(pts[j].Y - ptStart.Y) * (pts[j].Y - ptStart.Y))
										/ fFactor, CCommonSettings.Unit));
									g.DrawString(nLength.ToString(), dbgFont, brush
										, (ptStart.X + pts[j].X) / 2
										, (ptStart.Y + pts[j].Y) / 2);
								}
							}
						}
					}

					PreviewBitmap.MakeTransparent(Color.White);
					if (Thumbnail == null)
					{
						//	Move Thumbnail to the centre
						Bitmap tmpBmp = new Bitmap(PREVIEW_WIDTH, PREVIEW_HEIGHT);
						using(Graphics g1 = Graphics.FromImage(tmpBmp))
						{
							g1.Clear(Color.Transparent);
							g1.DrawImage(PreviewBitmap, 0, 25 - (PREVIEW_HEIGHT - (int)(MaxHeight * fFactor)) / 2
								, PREVIEW_WIDTH, PREVIEW_HEIGHT);
						}

						Thumbnail = tmpBmp.GetThumbnailImage(200, 125, null, IntPtr.Zero);
					}
					//m_HighlightPos.X = 0; m_HighlightPos.Y = 0;
				}
				catch (Exception ex)
				{
					Util.InfoMsg(ex.Message);
				}
			}
		}

		protected abstract bool GetBaseFace();

		protected abstract void GetSketchPlane();

		protected void GetBoundaryCurves()
		{
			List<CurveLoop> cls = new List<CurveLoop>();
			foreach (Face f in BaseFaces)
			{
				foreach (CurveLoop cl in f.GetEdgesAsCurveLoops())
				{
					if (cl.IsCounterclockwise((f as PlanarFace).FaceNormal))
					{
						cls.Add(cl);
					}
					else
					{
						BoundaryCurvesOfOpenings.Add(cl);
					}
					/*int i;
					if (cls.Count == 0)
					{
						cls.Add(cl);
					}
					else
					{
						for (i = 0; i < cls.Count; i++)
						{
							//	Check if cl in cls[i]
							if (IsCurveLoopInsideCurveLoop(cl, cls[i]))
							{
								//	This is a boundary curve loop for opening
								BoundaryCurvesOfOpenings.Add(cl);
								i = -1;
								break;
							}
							else if (IsCurveLoopInsideCurveLoop(cls[i], cl))
							{
								//	The previous one is a boundary curve loop for opening
								
								cls.RemoveAt(i);
								i--;
							}
						}
						if (i >= 0)
						{
							cls.Add(cl);
						}
					}*/
				}
			}

			//	Remove unnecessary points
			foreach (CurveLoop cl in cls)
			{
				CurveLoop cl_optimized = new CurveLoop();
				Curve prev = null;
				List<XYZ> pts = new List<XYZ>();
				foreach (Curve c in cl)
				{
					if (!(c is Line))
					{
						continue;
					}
					if (pts.Count >= 2)
					{
						if (Util.IsZero(prev.Distance(c.GetEndPoint(1))))
						{
							pts[pts.Count - 1] = c.GetEndPoint(1);
						}
						else
						{
							pts.Add(c.GetEndPoint(1));
						}
					}
					else
					{
						pts.Add(c.GetEndPoint(1));
					}

					if (prev != null) prev.Dispose();
					prev = c.Clone();
					prev.MakeUnbound();
				}
				//	Check if the last point is necessary
				if (pts.Count < 2)
					continue;
				if (Util.IsZero(prev.Distance(pts[0])))
				{
					pts.RemoveAt(pts.Count - 1);
				}
				prev.Dispose();
				pts.Add(pts[0]);
				XYZ lastPt = pts[0];
				for (int i = 0; i < pts.Count - 1; i++)
				{
					try
					{
						cl_optimized.Append(Line.CreateBound(lastPt, pts[i + 1]));
						lastPt = pts[i + 1];
					}
					catch (Exception e) { TaskDialog.Show("Error", e.Message); }
				}
				pts.Clear();
				BoundaryCurves.Add(cl_optimized);
			}
		}

		protected void GetAssociatedElements()
		{
			Document doc = m_Splitter.GetDocument();

			ElementId parent = Element.Id;
			Element elem = doc.GetElement(parent);
			if (elem is Part)
				parent = Util.GetSourceElementOfPart(doc, Element as Part).Id;

			//	Get All Openings and detailed information of them
			List<COpening> openings = new List<COpening>();

			//	Family Instance
#if REVIT2024 || REVIT2025
			IEnumerable<Element> associatedElements = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
				.Where(m =>
					((m as FamilyInstance).Symbol.Category.Id.Value.Equals((int)BuiltInCategory.OST_Doors)
					|| (m as FamilyInstance).Symbol.Category.Id.Value.Equals((int)BuiltInCategory.OST_Windows)
					)
					&& (m as FamilyInstance).Host != null
					&& (m as FamilyInstance).Host.Id == parent
				);
#else
			IEnumerable<Element> associatedElements = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
				.Where(m =>
					((m as FamilyInstance).Symbol.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Doors)
					|| (m as FamilyInstance).Symbol.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Windows)
					)
					&& (m as FamilyInstance).Host != null
					&& (m as FamilyInstance).Host.Id == parent
				);
#endif

			foreach (Element e in associatedElements)
			{
				if (!openings.Exists(x => x.Id == e.Id))
					openings.Add(new COpening() { Parent = Element.Id, Id = e.Id });
			}

			//	Opening
			associatedElements = new FilteredElementCollector(doc).OfClass(typeof(Opening))
				.Where(m =>
					(m as Opening).Host != null && (m as Opening).Host.Id == parent
				);

			foreach (Element e in associatedElements)
			{
				if (!openings.Exists(x => x.Id == e.Id))
					openings.Add(new COpening() { Parent = Element.Id, Id = e.Id });
			}

			foreach (COpening opening in openings)
			{
				GetDetailedInformationOfOpening(opening);
				AssociatedOpenings.Add(opening);
				m_Splitter.AssociatedOpenings.Add(opening);
			}
		}

		protected abstract void GetLocationCurve();

		/// <summary>
		//	Adjust the LocationCurve to include all points and not exceed more
		/// </summary>
		protected void AdjustLocationCurve()
		{
			//	This step will consider the direction of LocationCurve only.
			XYZ normal = SketchPlane.Normal;
			XYZ ptStart = SketchPlane.ProjectOnto(LocationCurve.GetEndPoint(0))
				, ptEnd = SketchPlane.ProjectOnto(LocationCurve.GetEndPoint(1));
			Line l = Line.CreateUnbound(ptStart, ptEnd - ptStart);
			ptStart = null;
			ptEnd = null;
			foreach (CurveLoop cl in BoundaryCurves)
			{
				foreach (Curve c in cl)
				{
					XYZ pt = l.Project(c.GetEndPoint(0)).XYZPoint;
					if (ptStart == null)
					{
						ptStart = pt;
						ptEnd = pt;
					}
					else if (!Util.IsEqual(
						ptStart.DistanceTo(ptEnd)
						, pt.DistanceTo(ptStart) + pt.DistanceTo(ptEnd)
						))
					{
						if (pt.DistanceTo(ptStart) < pt.DistanceTo(ptEnd))
						{
							ptStart = pt;
						}
						else
						{
							ptEnd = pt;
						}
					}
				}
			}
			if (!Util.IsEqual(
				(LocationCurve.GetEndPoint(1) - LocationCurve.GetEndPoint(0)).Normalize(),
				(ptEnd - ptStart).Normalize()
				))
			{
				XYZ pt = ptEnd;
				ptEnd = ptStart;
				ptStart = pt;
			}

			//	Get the Perpendicular Vector
			PV = (ptEnd - ptStart).CrossProduct(normal).Normalize();

			//	Consider the vertical direction
			double fMax = 0;
			foreach (CurveLoop cl in BoundaryCurves)
			{
				foreach (Curve c in cl)
				{
					XYZ pt = l.Project(c.GetEndPoint(0)).XYZPoint;
					if (!Util.IsEqual((c.GetEndPoint(0) - pt).Normalize(), PV)
						&& c.GetEndPoint(0).DistanceTo(pt) > fMax)
					{
						fMax = c.GetEndPoint(0).DistanceTo(pt);
					}
				}
			}
			LocationCurve = Line.CreateBound(ptStart - PV * fMax
					, ptEnd - PV * fMax);

			//	Get maximum height
			fMax = 0;
			foreach (CurveLoop cl in BoundaryCurves)
			{
				foreach (Curve c in cl)
				{
					double fDistance = LocationCurve.Distance(c.GetEndPoint(0));
					if (fDistance > fMax)
						fMax = fDistance;
				}
			}
			MaxHeight = fMax;
		}

		/// <summary>
		//	Check if cl1 is inside cl2
		//	This function will work when both CurveLoops are on one plane
		/// </summary>
		protected bool IsCurveLoopInsideCurveLoop(CurveLoop cl1, CurveLoop cl2)
		{
			foreach (Curve c in cl1)
			{
				if (!(c is Line))
					return false;

				if (!Util.IsPointInsideCurveLoop(cl2, c.GetEndPoint(0)))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Check if an opening is associated to this element
		/// </summary>
		public bool IsOpeningAssociatedWith(Element opening)
		{
			ElementId hostId;
			if (opening is FamilyInstance)
				hostId = (opening as FamilyInstance).Host.Id;
			else if (opening is Opening)
				hostId = (opening as Opening).Host.Id;
			else
			{
				LogManager.Write(LogManager.WarningLevel.Error, "CSplitElement::IsOpeningAssociatedWith", "This kind of opening is not supported yet.");
				throw new NotSupportedException();
			}

			if (Element is Part)
			{
				Document doc = m_Splitter.GetDocument();
				if (hostId == Util.GetSourceElementOfPart(doc, Element as Part).Id)
				{
					BoundingBoxXYZ bbxyz = opening.get_BoundingBox(doc.ActiveView);
					if (bbxyz == null)
						return false;

					XYZ min = bbxyz.Min, max = bbxyz.Max;
					min = SketchPlane.ProjectOnto(min);
					max = SketchPlane.ProjectOnto(max);

					if (Util.IsPointInsideCurveLoop(BoundaryCurves[0], min, true)
						|| Util.IsPointInsideCurveLoop(BoundaryCurves[0], max, true))
					{
						return true;
					}

					return false;
				}
				return false;
			}
			else
			{
				return hostId == Element.Id;
			}
		}

		public void AddAssociatedOpening(COpening sO)
		{
			AssociatedOpenings.Add(sO);
		}

		/// <summary>
		/// Add Opening with its parameter to SelectedOpenings array
		/// This function check its position and insert into appropriate index
		/// </summary>
		/// <param name="sOP"></param>
		public void AddOpening(COpening sO)
		{
			//	Check if this opening is intersecting with the boundary edges
			int j;
			//	If at least one boundary point of the opening is on the boundary edge of the element or outside the element, tool decides this opening is intersecting with the boundary edges
			for (j = 0; j < sO.Points.Length; j++)
			{
				sO.IntersectWithBoundaryEdge = false;
				foreach (CurveLoop cl in BoundaryCurves)
				{
					int nCount = 0;
					foreach (Curve c in cl)
					{
						if (Util.IsZero(c.Distance(sO.Points[j])))
						{
							sO.IntersectWithBoundaryEdge = true;
							nCount = 0;
							break;
						}
						Curve c1 = Line.CreateBound(sO.Points[j], sO.Points[j] + PV * MaxHeight * 2);
						if (c.Intersect(c1) == SetComparisonResult.Overlap)
						{
							nCount++;
						}
					}
					if (nCount % 2 == 0)
					{
						sO.IntersectWithBoundaryEdge = true;
					}
					if (sO.IntersectWithBoundaryEdge)
						break;
				}
			}
			double tempDist = LocationCurve.GetEndPoint(0).DistanceTo(sO.ProjectedLocation);
			int i;
			for (i = 0; i < SelectedOpenings.Count; i++)
				if (tempDist < LocationCurve.GetEndPoint(0).DistanceTo(SelectedOpenings[i].ProjectedLocation))
				{
					SelectedOpenings.Insert(i, sO);
					break;
				}
			if (i == SelectedOpenings.Count) SelectedOpenings.Add(sO);
		}

		/// <summary>
		/// Add Split Point to SplitPoints array
		/// This function checks its position and insert into appropriate index
		/// </summary>
		/// <param name="sSP"></param>
		public void AddSplitPoint(SSplitPoint sSP, int nIndex = -1)
		{
			if (nIndex >= 0)
			{
				SplitPoints.Insert(nIndex, sSP);
				return;
			}

			XYZ ptStart = LocationCurve.GetEndPoint(0);

			double tempDist = LocationCurve.GetEndPoint(0).DistanceTo(sSP.Point);
			int i;
			for (i = 0; i < SplitPoints.Count; i++)
			{
				if (Util.IsEqual(tempDist, ptStart.DistanceTo(SplitPoints[i].Point))
					&& SplitPoints[i].ReferenceToOpening >= 0
					&& sSP.ReferenceToOpening >= 0)
				{
					int nOpening1 = SplitPoints[i].ReferenceToOpening,
						nOpening2 = sSP.ReferenceToOpening;
					if (ptStart.DistanceTo(SelectedOpenings[nOpening1].Location)
						> ptStart.DistanceTo(SelectedOpenings[nOpening2].Location))
					{
						SplitPoints.Insert(i, sSP);
						break;
					}
				}
				else if (tempDist < ptStart.DistanceTo(SplitPoints[i].Point))
				{
					SplitPoints.Insert(i, sSP);
					break;
				}
			}
			if (i == SplitPoints.Count) SplitPoints.Add(sSP);
		}

		/// <summary>
		/// Determine split points based on the Openings
		/// </summary>
		public void GetSplitPoints()
		{
			#region Convert Unit of the input parameters
			double fAroundCentreSplitWidth = UnitUtils.ConvertToInternalUnits(AroundCentreSplitWidth, CCommonSettings.Unit);
			#endregion

			m_ptStartPoint = LocationCurve.Project(m_ptStartPoint).XYZPoint;

			int i, j;
			SSplitPoint sSP;
			XYZ NV = (LocationCurve.GetEndPoint(1) - LocationCurve.GetEndPoint(0)).Normalize();
			for (i = 0; i < SelectedOpenings.Count; i++)
			{
				COpening sO = SelectedOpenings[i];
				switch (sO.Option)
				{
					case OpeningSelectOption.LintelAndSill:
						XYZ ptTmp = SketchPlane.ProjectOnto(sO.Location) - PV * sO.Height / 2;
						//	TODO
						//	Add split point for Sill
						if (IsOnBaseFace(ptTmp) && !Util.IsZero(LocationCurve.Distance(ptTmp)))
						{
							sSP = new SSplitPoint
							{
								ReferenceToOpening = i,
								Reserved = 0,
								Point = sO.ProjectedLocation - NV * sO.Width / 2
							};
							AddSplitPoint(sSP);
						}

						//	Add split point for Lintel
						ptTmp = SketchPlane.ProjectOnto(sO.Location) + PV * sO.Height / 2;
						if (IsOnBaseFace(ptTmp))
						{
							sSP = new SSplitPoint
							{
								ReferenceToOpening = i,
								Reserved = 1,
								Point = sO.ProjectedLocation - NV * sO.Width / 2
							};
							AddSplitPoint(sSP);
						}
						break;
					case OpeningSelectOption.AroundCentreOfOpening:
						sSP = new SSplitPoint
						{
							ReferenceToOpening = i,
							Reserved = 0,
							Point = sO.ProjectedLocation - NV * fAroundCentreSplitWidth / 2
						};
						AddSplitPoint(sSP);

						sSP = new SSplitPoint
						{
							ReferenceToOpening = i,
							Reserved = 1,
							Point = sO.ProjectedLocation + NV * fAroundCentreSplitWidth / 2
						};
						AddSplitPoint(sSP);
						break;
					case OpeningSelectOption.CentreOfOpening:
						sSP = new SSplitPoint
						{
							ReferenceToOpening = i,
							Reserved = 0,
							Point = sO.ProjectedLocation
						};
						AddSplitPoint(sSP);
						break;
					case OpeningSelectOption.EqualDistanceBetweenOpenings:
						//	Find the next Opening
						for (j = i + 1; j < SelectedOpenings.Count; j++)
						{
							COpening sO1 = SelectedOpenings[j];
							if (sO1.Option == OpeningSelectOption.EqualDistanceBetweenOpenings)
							{
								sSP = new SSplitPoint
								{
									ReferenceToOpening = i,
									Reserved = j,
									Point = (sO.ProjectedLocation + sO1.ProjectedLocation) / 2
								};
								AddSplitPoint(sSP);
								break;
							}
						}
						break;
					default:
						break;
				}
			}
			//	Add the split point for the start point and the end point
			sSP = new SSplitPoint
			{
				ReferenceToOpening = -1,
				Point = m_ptStartPoint
			};
			AddSplitPoint(sSP);
			if (!Util.IsEqual(m_ptStartPoint, LocationCurve.GetEndPoint(0)))
			{
				sSP = new SSplitPoint
				{
					ReferenceToOpening = -1,
					Point = LocationCurve.GetEndPoint(0)
				};
				AddSplitPoint(sSP, 0);
			}
			if (!Util.IsEqual(m_ptStartPoint, LocationCurve.GetEndPoint(1)))
			{
				sSP = new SSplitPoint
				{
					ReferenceToOpening = -1,
					Point = LocationCurve.GetEndPoint(1)
				};
				AddSplitPoint(sSP, SplitPoints.Count);
			}
		}

		/// <summary>
		//	Check if point is on BaseFace of this element
		/// </summary>
		protected bool IsOnBaseFace(XYZ point)
		{
			if (Util.IsEqual(SketchPlane.ProjectOnto(point), point))
			{
				Curve c = Line.CreateBound(point, point + PV * MaxHeight);
				int nIntersectCnt = 0;
				foreach (CurveLoop cl in BoundaryCurves)
				{
					foreach (Curve c1 in cl)
					{
						if (c.Intersect(c1, out IntersectionResultArray ira) != SetComparisonResult.Disjoint)
						{
							if (Util.IsEqual(ira.get_Item(0).XYZPoint, point))
								return true;
							nIntersectCnt++;
						}
					}
				}
				if (nIntersectCnt % 2 == 1)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Add Split Line to SplitLines array
		///		pt0 is nearer to the LocationCurve than pt1
		///		We extend the line to the Boundary Curves according to the bCutByEdge0 and bCutByEdge1 parameters
		/// </summary>
		public void AddSplitLine(int nIndex, int nReferenceToSplitPoint, XYZ pt0, XYZ pt1, OpeningSelectOption opt, bool bCutByEdge0 = false, bool bCutByEdge1 = false, int nReserved = -1)
		{
			/*bool bIn0 = false, bIn1 = false;
			foreach(CurveLoop cl in BoundaryCurves)
			{
				if (Util.IsPointInsideCurveLoop(cl, pt0))
				{
					bIn0 = true;
				}
				if (Util.IsPointInsideCurveLoop(cl, pt1))
				{
					bIn1 = true;
				}
				if (bIn0 && bIn1)
					break;
			}
			if (!bIn0 && !bIn1)
			{
				return;
			}*/

			SSplitLine sSL = new SSplitLine
			{
				ReferenceToSplitPoint = nReferenceToSplitPoint,
				Points = new XYZ[2],
				IsCutByEdge = bCutByEdge1,
				IsCutByLocationCurve = bCutByEdge0,
				Option = opt,
				Reserved = nReserved
			};
			if (bCutByEdge0)
			{
				/*Curve curve = Line.CreateUnbound(pt0, PV);
				curve.Intersect(LocationCurve, out IntersectionResultArray ira);
				pt0 = ira.get_Item(0).XYZPoint;
				curve.Dispose();*/
				//	Get the farest intersect point by the direction
				Curve curve = Line.CreateBound(pt0, pt0 - PV * MaxHeight);
				XYZ intersectPt = null;
				foreach (CurveLoop cl in BoundaryCurves)
				{
					foreach (Curve c in cl)
					{
						if (c.Intersect(curve, out IntersectionResultArray resultArray) == SetComparisonResult.Overlap)
						{
							XYZ pt = resultArray.get_Item(0).XYZPoint;
							if (intersectPt == null || pt0.DistanceTo(pt) > pt1.DistanceTo(intersectPt))
							{
								intersectPt = pt;
							}
						}
					}
				}
				curve.Dispose();

				if (intersectPt == null)
				{
					curve = Line.CreateBound(pt0, pt0 + PV * MaxHeight);
					//	Get the nearest intersect point by the opposite direction
					foreach (CurveLoop cl in BoundaryCurves)
					{
						foreach (Curve c in cl)
						{
							if (c.Intersect(curve, out IntersectionResultArray resultArray) == SetComparisonResult.Overlap)
							{
								XYZ pt = resultArray.get_Item(0).XYZPoint;
								if (intersectPt == null || pt0.DistanceTo(pt) < pt1.DistanceTo(intersectPt))
								{
									intersectPt = pt;
								}
							}
						}
					}
					curve.Dispose();

					if (intersectPt == null)
					{
#if RELEASE
						Util.ErrorMsg("Can't find the intersect point");
#endif
						pt0 = pt1 - PV * MaxHeight;
					}
					else
					{
						pt0 = intersectPt;
					}
				}
				else
				{
					pt0 = intersectPt;
				}
			}
			if (bCutByEdge1)
			{
				//	Get the farest intersect point by the direction
				Curve curve = Line.CreateBound(pt1, pt1 + PV * MaxHeight);
				XYZ intersectPt = null;
				foreach (CurveLoop cl in BoundaryCurves)
				{
					foreach (Curve c in cl)
					{
						if (c.Intersect(curve, out IntersectionResultArray resultArray) == SetComparisonResult.Overlap)
						{
							XYZ pt = resultArray.get_Item(0).XYZPoint;
							if (intersectPt == null || pt1.DistanceTo(pt) > pt1.DistanceTo(intersectPt))
							{
								intersectPt = pt;
							}
						}
					}
				}
				curve.Dispose();

				if (intersectPt == null)
				{
					curve = Line.CreateBound(pt1, pt1 - PV * MaxHeight);
					//	Get the nearest intersect point by the opposite direction
					foreach (CurveLoop cl in BoundaryCurves)
					{
						foreach (Curve c in cl)
						{
							if (c.Intersect(curve, out IntersectionResultArray resultArray) == SetComparisonResult.Overlap)
							{
								XYZ pt = resultArray.get_Item(0).XYZPoint;
								if (intersectPt == null || pt1.DistanceTo(pt) < pt1.DistanceTo(intersectPt))
								{
									intersectPt = pt;
								}
							}
						}
					}
					curve.Dispose();

					if (intersectPt == null)
					{
#if RELEASE
						Util.ErrorMsg("Can't find the intersect point");
#endif
						pt1 += PV * MaxHeight;
					}
					else
					{
						pt1 = intersectPt;
					}
				}
				else
				{
					pt1 = intersectPt;
				}
			}
			sSL.Points[0] = pt0;
			sSL.Points[1] = pt1;
			if (bCutByEdge0)
			{
				if (MaxHeight > 2)
					pt0 -= PV * MaxHeight;
				else
					pt0 -= PV * 2;
			}
			if (bCutByEdge1)
			{
				if (MaxHeight > 2)
					pt1 += PV * MaxHeight;
				else
					pt1 += PV * 2;
			}
			sSL.SplitCurveWithMargin = Line.CreateBound(pt0, pt1);

			if (nIndex >= 0)
			{
				SplitLines.Insert(nIndex, sSL);
			}
			else if (nIndex == -1)
			{
				SplitLines.Add(sSL);
			}
		}

		/// <summary>
		/// Get Split lines based on split points
		/// </summary>
		public void GetSplitLines()
		{
			SplitLines.Clear();

			#region Convert Unit of the input parameters
			double fStandardSplitWidth = UnitUtils.ConvertToInternalUnits(StandardSplitWidth, CCommonSettings.Unit);
			double fLintel = UnitUtils.ConvertToInternalUnits(LintelBearing, CCommonSettings.Unit);
			double fCill = UnitUtils.ConvertToInternalUnits(CillBearing, CCommonSettings.Unit);
			#endregion

			int i, j;
			XYZ ptTmp;

			if (AdjacentElements.Count > 0)
			{
				//	Draw adjacent elements
				double fWallWidthLimit = UnitUtils.ConvertToInternalUnits(400, CCommonSettings.Unit);
				Document doc = m_Splitter.GetDocument();
				List<Face> bottomFaces = Util.GetBaseFacesOfElement(doc, Element, false);
				List<Face> topFaces = Util.GetBaseFacesOfElement(doc, Element, true);
				foreach (Element e in AdjacentElements)
				{
					XYZ ptStart, ptEnd;
					if (e is Wall)
					{
						if (Element is Wall ||
							(Element is Part && Util.GetSourceElementOfPart(doc, Element as Part) is Wall))
						{
							//	If this element is a wall and the adjacent element is also a wall, find the intersecting shape (curves array) located on base face of this element
							XYZ normal = SketchPlane.Normal;
							GeometryElement geomElem = e.get_Geometry(new Options());
							List<Curve> curves = new List<Curve>();
							//	Find the intersecting lines
							foreach (GeometryObject geomObject in geomElem)
							{
								if (geomObject is Solid)
								{
									Solid solid = geomObject as Solid;
									FaceArray faceArray = solid.Faces;
									//	Check interior face first
									foreach (Face f in faceArray)
									{
										if (Util.IntersectWith(f, bottomFaces[0]) == SetComparisonResult.Overlap)
										{
											f.Intersect(bottomFaces[0], out Curve c);
											curves.Add(c);
										}
									}
									//	If we don't find any intersection with interior face, we try to check exterior face
									if (curves.Count == 0)
									{
										foreach (Face f in faceArray)
										{
											if (Util.IntersectWith(f, topFaces[0]) == SetComparisonResult.Overlap)
											{
												f.Intersect(topFaces[0], out Curve c);
												curves.Add(c);
											}
										}
									}
								}
							}

							//	Find the longest curve and use it to get split line
							Curve maxCurve = null;
							foreach (Curve c in curves)
							{
								if (maxCurve == null
									|| c.Length > maxCurve.Length)
								{
									maxCurve = c;
								}
							}
							for (i = curves.Count - 1; i >= 0; i--)
							{
								if (curves[i].Length < maxCurve.Length / 2
									|| !Util.IsParallel(curves[i].GetEndPoint(1) - curves[i].GetEndPoint(0),
										maxCurve.GetEndPoint(1) - maxCurve.GetEndPoint(0)))
								{
									curves.RemoveAt(i);
								}
							}
							if (curves.Count == 0)
							{
								throw new NotSupportedException();
							}
							else if (curves.Count == 1)
							{
								ptStart = curves[0].GetEndPoint(0);
								ptEnd = curves[0].GetEndPoint(1);
							}
							else if (curves.Count == 2)
							{
								ptStart = curves[0].GetEndPoint(0);
								ptEnd = curves[0].GetEndPoint(1);
								Line l = Line.CreateUnbound(ptStart, ptEnd - ptStart);
								XYZ pt = l.Project(curves[1].GetEndPoint(0)).XYZPoint;
								ptStart += (curves[1].GetEndPoint(0) - pt) / 2;
								ptEnd += (curves[1].GetEndPoint(0) - pt) / 2;
							}
							else
							{
								throw new NotSupportedException();
							}
						}
						else
						{
							//	If this element is a floor/slab and the adjacent element is a wall, we split the floor/slab with the location curve of the adjacent wall
							ptStart = SketchPlane.ProjectOnto(
									(e.Location as LocationCurve).Curve.GetEndPoint(0)
									);
							ptEnd = SketchPlane.ProjectOnto(
									(e.Location as LocationCurve).Curve.GetEndPoint(1)
									);
						}
					}
					else
					{
						//	If the adjacent element is a beam, we split this element with the location curve of the adjacent beam
						LocationCurve lc = e.Location as LocationCurve;
						ptStart = SketchPlane.ProjectOnto(lc.Curve.GetEndPoint(0));
						ptEnd = SketchPlane.ProjectOnto(lc.Curve.GetEndPoint(1));
					}

					//ptStart -= (ptEnd - ptStart).Normalize();
					//ptEnd += (ptEnd - ptStart).Normalize();

					SSplitLine sSL = new SSplitLine
					{
#if REVIT2024 || REVIT2025
						ReferenceToSplitPoint = (int)e.Id.Value,
#else
						ReferenceToSplitPoint = e.Id.IntegerValue,
#endif
						Points = new XYZ[2] { ptStart, ptEnd },
						IsCutByEdge = false,
						IsCutByLocationCurve = false,
						Option = OpeningSelectOption.None,
						SplitCurveWithMargin = Line.CreateBound(ptStart, ptEnd)
					};

					//	Check if it intersects with boundary edge
					foreach (Curve c in BoundaryCurves[0])
					{
						if (c.Intersect(sSL.SplitCurveWithMargin, out IntersectionResultArray ira) == SetComparisonResult.Overlap)
						{
							XYZ intersectPt = ira.get_Item(0).XYZPoint;
							if (Util.IsEqual(intersectPt, ptStart)
								|| Util.IsEqual(intersectPt, ptEnd))
								break;

							if (Util.IsPointInsideCurveLoop(BoundaryCurves[0], ptStart))
							{
								sSL.Points[1] = intersectPt;
							}
							else
							{
								Curve tmpLine = Line.CreateBound(intersectPt, ptStart);

								int nIntersectCnt = 0;
								foreach (Curve c1 in BoundaryCurves[0])
								{
									if (!Util.IsEqual(c as Line, c1 as Line))
									{
										if (tmpLine.Intersect(c1) == SetComparisonResult.Overlap)
											nIntersectCnt++;
									}
								}
								if (nIntersectCnt % 2 == 0)
								{
									sSL.Points[0] = intersectPt;
								}
								else
								{
									sSL.Points[1] = intersectPt;
								}
							}
						}
					}

					SplitLines.Add(sSL);
				}

				//	Check all split lines
				for (i = 0; i < SplitLines.Count; i++)
				{
					SSplitLine sSL1 = SplitLines[i];
					for (j = i + 1; j < SplitLines.Count; j++)
					{
						SSplitLine sSL2 = SplitLines[j];
						if (sSL1.SplitCurveWithMargin.Intersect(
							sSL2.SplitCurveWithMargin, out IntersectionResultArray ira) == SetComparisonResult.Overlap)
						{
							XYZ intersectPt = ira.get_Item(0).XYZPoint;
							if (intersectPt.DistanceTo(sSL1.Points[0]) > intersectPt.DistanceTo(sSL1.Points[1]))
							{
								if (fWallWidthLimit > intersectPt.DistanceTo(sSL1.Points[1]))
									sSL1.Points[1] = intersectPt;
							}
							else
							{
								if (fWallWidthLimit * 1.5 > intersectPt.DistanceTo(sSL1.Points[0]))
									sSL1.Points[0] = intersectPt;
							}
							if (intersectPt.DistanceTo(sSL2.Points[0]) > intersectPt.DistanceTo(sSL2.Points[1]))
							{
								if (fWallWidthLimit * 1.5 > intersectPt.DistanceTo(sSL2.Points[1]))
									sSL2.Points[1] = intersectPt;
							}
							else
							{
								if (fWallWidthLimit * 1.5 > intersectPt.DistanceTo(sSL2.Points[0]))
									sSL2.Points[0] = intersectPt;
							}
						}
					}
				}

				for (i = 0; i < SplitLines.Count; i++)
				{
					SSplitLine sSL0 = SplitLines[i];
					Curve c = Line.CreateBound(sSL0.Points[0], sSL0.Points[1]);
					c.MakeUnbound();

					//	Get all intersection point
					List<XYZ> pts = new List<XYZ>();
					for (j = 0; j < SplitLines.Count; j++)
					{
						if (i == j)
							continue;

						SSplitLine sSL1 = SplitLines[j];
						if (c.Intersect(sSL1.SplitCurveWithMargin, out IntersectionResultArray ira) == SetComparisonResult.Overlap)
						{
							pts.Add(ira.get_Item(0).XYZPoint);
						}
					}
					foreach (Curve bc in BoundaryCurves[0])
					{
						if (c.Intersect(bc, out IntersectionResultArray ira) == SetComparisonResult.Overlap)
						{
							pts.Add(ira.get_Item(0).XYZPoint);
						}
					}
					XYZ ptStart = null, ptEnd = null;
					foreach (XYZ pt in pts)
					{
						if (Util.IsEqual(pt, sSL0.Points[0]))
							ptStart = sSL0.Points[0];
						if (Util.IsEqual(pt, sSL0.Points[1]))
							ptEnd = sSL0.Points[1];

						if (Util.IsEqual(sSL0.Points[0].DistanceTo(sSL0.Points[1]), pt.DistanceTo(sSL0.Points[0]) + pt.DistanceTo(sSL0.Points[1])))
							continue;

						if ((ptStart == null
							|| pt.DistanceTo(sSL0.Points[0]) < ptStart.DistanceTo(sSL0.Points[0]))
							&& (pt.DistanceTo(sSL0.Points[0]) < pt.DistanceTo(sSL0.Points[1])))
						{
							ptStart = pt;
						}
						if ((ptEnd == null
							|| pt.DistanceTo(sSL0.Points[1]) < ptEnd.DistanceTo(sSL0.Points[1]))
							&& (pt.DistanceTo(sSL0.Points[0]) > pt.DistanceTo(sSL0.Points[1])))
						{
							ptEnd = pt;
						}
					}
					if (ptStart == null || ptEnd == null)
					{

					}
					else
					{
						sSL0.Points[0] = ptStart;
						sSL0.Points[1] = ptEnd;
					}
				}

				for (i = 0; i < SplitLines.Count; i++)
				{
					SSplitLine sSL = SplitLines[i];
					XYZ ptStart = sSL.Points[0],
						ptEnd = sSL.Points[1];

					ptStart -= (ptEnd - ptStart).Normalize();
					ptEnd += (ptEnd - ptStart).Normalize();
					sSL.SplitCurveWithMargin = Line.CreateBound(ptStart, ptEnd);
					SplitLines[i] = sSL;
				}

				return;
			}

			//	Get the normal vector of the vector which links the start point and end point of the edge location
			XYZ gNV = (LocationCurve.GetEndPoint(1) - LocationCurve.GetEndPoint(0)).Normalize();
			int nSplitLineIndexToAdd = 0;
			//	Add Start Line
			//	TODO: Not sure if we need this
			AddSplitLine(nSplitLineIndexToAdd++, 0,
				LocationCurve.GetEndPoint(0),
				LocationCurve.GetEndPoint(0),
				OpeningSelectOption.None,
				true,
				true);
			for (i = 0; i < SplitPoints.Count - 1; i++)
			{
				SSplitPoint sSP = SplitPoints[i];
				XYZ lPtStart = sSP.Point
					, lPtEnd = SplitPoints[i + 1].Point;

				XYZ lNV = gNV;
				XYZ lVectorForStandard = lNV * fStandardSplitWidth;

				//	Add split lines for the Split Point
				COpening sOP;
				COpening sO;
				if (sSP.ReferenceToOpening >= 0
					&& (sOP = SelectedOpenings[sSP.ReferenceToOpening]).Option == OpeningSelectOption.LintelAndSill)
				{
					sO = sOP;
					ptTmp = SketchPlane.ProjectOnto(sO.Location) - PV * sO.Height / 2;

					if (SplitPoints[i + 1].ReferenceToOpening == sSP.ReferenceToOpening)
					{
						//  Draw Bottom Lines
						AddSplitLine(nSplitLineIndexToAdd++, i
							, ptTmp - lNV * (sO.Width / 2 + fCill)
							, ptTmp - lNV * (sO.Width / 2 + fCill)
							, OpeningSelectOption.LintelAndSill
							, true
							, false
							, 0);
						AddSplitLine(nSplitLineIndexToAdd++, i
							, ptTmp - lNV * (sO.Width / 2 + fCill)
							, ptTmp + lNV * (sO.Width / 2 + fCill)
							, OpeningSelectOption.LintelAndSill
							, false
							, false
							, 1);
						AddSplitLine(nSplitLineIndexToAdd, i
							, ptTmp + lNV * (sO.Width / 2 + fCill)
							, ptTmp + lNV * (sO.Width / 2 + fCill)
							, OpeningSelectOption.LintelAndSill
							, true
							, false
							, 2);

						ptTmp += PV * sO.Height;
						//  Draw top lines
						AddSplitLine(nSplitLineIndexToAdd++, i + 1
							, ptTmp - lNV * (sO.Width / 2 + fLintel)
							, ptTmp - lNV * (sO.Width / 2 + fLintel)
							, OpeningSelectOption.LintelAndSill
							, false
							, true
							, 3);
						AddSplitLine(nSplitLineIndexToAdd++, i + 1
							, ptTmp - lNV * (sO.Width / 2 + fLintel)
							, ptTmp + lNV * (sO.Width / 2 + fLintel)
							, OpeningSelectOption.LintelAndSill
							, false
							, false
							, 4);
						AddSplitLine(nSplitLineIndexToAdd, i + 1
							, ptTmp + lNV * (sO.Width / 2 + fLintel)
							, ptTmp + lNV * (sO.Width / 2 + fLintel)
							, OpeningSelectOption.LintelAndSill
							, false
							, true
							, 5);
						nSplitLineIndexToAdd += 2;
						continue;
					}
					else if (i > 0 && SplitPoints[i - 1].ReferenceToOpening == sSP.ReferenceToOpening)
					{
						lPtStart += lNV * sO.Width;
					}
					else
					{
						//	In case we have only Lintel or Cill
						if (sSP.Reserved == 0)	//	Cill only
						{
							//  Draw Bottom Lines
							AddSplitLine(nSplitLineIndexToAdd++, i
								, ptTmp - lNV * (sO.Width / 2 + fCill)
								, ptTmp - lNV * (sO.Width / 2 + fCill)
								, OpeningSelectOption.LintelAndSill
								, true
								, false
								, 0);
							AddSplitLine(nSplitLineIndexToAdd++, i
								, ptTmp - lNV * (sO.Width / 2 + fCill)
								, ptTmp + lNV * (sO.Width / 2 + fCill)
								, OpeningSelectOption.LintelAndSill
								, false
								, false
								, 1);
							AddSplitLine(nSplitLineIndexToAdd, i
								, ptTmp + lNV * (sO.Width / 2 + fCill)
								, ptTmp + lNV * (sO.Width / 2 + fCill)
								, OpeningSelectOption.LintelAndSill
								, true
								, false
								, 2);
						}

						ptTmp += PV * sO.Height;
						if (sSP.Reserved == 1)	//	Lintel only
						{
							//  Draw top lines
							AddSplitLine(nSplitLineIndexToAdd++, i
								, ptTmp - lNV * (sO.Width / 2 + fLintel)
								, ptTmp - lNV * (sO.Width / 2 + fLintel)
								, OpeningSelectOption.LintelAndSill
								, false
								, true
								, 3);
							AddSplitLine(nSplitLineIndexToAdd++, i
								, ptTmp - lNV * (sO.Width / 2 + fLintel)
								, ptTmp + lNV * (sO.Width / 2 + fLintel)
								, OpeningSelectOption.LintelAndSill
								, false
								, false
								, 4);
							AddSplitLine(nSplitLineIndexToAdd, i
								, ptTmp + lNV * (sO.Width / 2 + fLintel)
								, ptTmp + lNV * (sO.Width / 2 + fLintel)
								, OpeningSelectOption.LintelAndSill
								, false
								, true
								, 5);
						}
						nSplitLineIndexToAdd++;
						lPtStart += lNV * sO.Width;
					}
				}
				else if (i > 0)
				{
					if (sSP.ReferenceToOpening >= 0)
					{
						AddSplitLine(nSplitLineIndexToAdd++, i
									, sSP.Point
									, sSP.Point
									, SelectedOpenings[sSP.ReferenceToOpening].Option
									, true
									, true);
					}
					else
					{
						AddSplitLine(nSplitLineIndexToAdd++, i
									, sSP.Point
									, sSP.Point
									, OpeningSelectOption.None
									, true
									, true);
					}
				}

				//	Exceptions
				if (sSP.ReferenceToOpening >= 0 && (sOP = SelectedOpenings[sSP.ReferenceToOpening]).Option == OpeningSelectOption.AroundCentreOfOpening
					&& i < SplitPoints.Count - 1
					&& sSP.ReferenceToOpening == SplitPoints[i + 1].ReferenceToOpening
					&& SelectedOpenings[SplitPoints[i + 1].ReferenceToOpening].Option == OpeningSelectOption.AroundCentreOfOpening)
				{
					//	We don't add more lines between split points in this case.
				}
				else if (sSP.ReferenceToOpening >= 0 && (sOP = SelectedOpenings[sSP.ReferenceToOpening]).Option == OpeningSelectOption.EqualDistanceBetweenOpenings
					&& i < SplitPoints.Count - 1
					&& sSP.Reserved == SplitPoints[i + 1].ReferenceToOpening
					&& SelectedOpenings[SplitPoints[i + 1].ReferenceToOpening].Option == OpeningSelectOption.EqualDistanceBetweenOpenings)
				{

				}
				else
				{
					int nIndexDelta = 0;
					double fLength = lPtStart.DistanceTo(lPtEnd);
					//	TODO: Not sure if we need to split into these two conditions
					if (SplitFromLeftToRight)
					{
						if (fLength > fStandardSplitWidth && !Util.IsEqual(fLength, fStandardSplitWidth))
						{
							if (i > 0
								&& sSP.ReferenceToOpening != -1
								&& SelectedOpenings[sSP.ReferenceToOpening].Option == OpeningSelectOption.EqualDistanceBetweenOpenings)
							{
								lPtStart += lVectorForStandard;
								AddSplitLine(nSplitLineIndexToAdd++, i
									, lPtStart
									, lPtStart
									, OpeningSelectOption.None
									, true
									, true);
								fLength -= fStandardSplitWidth;
							}

							if (SplitPoints[i + 1].ReferenceToOpening != -1
								&& SelectedOpenings[SplitPoints[i + 1].ReferenceToOpening].Option == OpeningSelectOption.EqualDistanceBetweenOpenings)
							{
								lPtEnd -= lVectorForStandard;
								AddSplitLine(nSplitLineIndexToAdd, i
									, lPtEnd
									, lPtEnd
									, OpeningSelectOption.None
									, true
									, true);
								nIndexDelta++;
								fLength -= fStandardSplitWidth;
							}
						}
					}
					else
					{
						if (fLength > fStandardSplitWidth && !Util.IsEqual(fLength, fStandardSplitWidth))
						{
							if (SplitPoints[i + 1].ReferenceToOpening != -1
								&& SelectedOpenings[SplitPoints[i + 1].ReferenceToOpening].Option == OpeningSelectOption.EqualDistanceBetweenOpenings)
							{
								lPtEnd -= lVectorForStandard;
								AddSplitLine(nSplitLineIndexToAdd, i
									, lPtEnd
									, lPtEnd
									, OpeningSelectOption.None
									, true
									, true);
								nIndexDelta++;
								fLength -= fStandardSplitWidth;
							}

							if (i > 0
								&& sSP.ReferenceToOpening != -1
								&& SelectedOpenings[sSP.ReferenceToOpening].Option == OpeningSelectOption.EqualDistanceBetweenOpenings)
							{
								lPtStart += lVectorForStandard;
								AddSplitLine(nSplitLineIndexToAdd++, i
									, lPtStart
									, lPtStart
									, OpeningSelectOption.None
									, true
									, true);
								fLength -= fStandardSplitWidth;
							}
						}
					}
					//	Add regular split lines for one segment
					int nLineCount = (int)(fLength / fStandardSplitWidth);
					if (Util.IsEqual(fLength, fStandardSplitWidth * nLineCount))
					{
						nLineCount--;
					}
					if (!MustSelectStartPoint)
					{
						for (j = 0; j < nLineCount; j++)
						{
							if (SplitFromLeftToRight)
							{
								lPtStart += lVectorForStandard;
								AddSplitLine(nSplitLineIndexToAdd++, i
									, lPtStart
									, lPtStart
									, OpeningSelectOption.None
									, true
									, true);
							}
							else
							{
								lPtEnd -= lVectorForStandard;
								AddSplitLine(nSplitLineIndexToAdd, i
									, lPtEnd
									, lPtEnd
									, OpeningSelectOption.None
									, true
									, true);
								nIndexDelta++;
							}
						}
					}
					else
					{
						for (j = 0; j < nLineCount; j++)
						{
							if (lPtStart.DistanceTo(m_ptStartPoint) < lPtEnd.DistanceTo(m_ptStartPoint))
							{
								lPtStart += lVectorForStandard;
								AddSplitLine(nSplitLineIndexToAdd++, i
									, lPtStart
									, lPtStart
									, OpeningSelectOption.None
									, true
									, true);
							}
							else
							{
								lPtEnd -= lVectorForStandard;
								AddSplitLine(nSplitLineIndexToAdd, i
									, lPtEnd
									, lPtEnd
									, OpeningSelectOption.None
									, true
									, true);
								nIndexDelta++;
							}
						}
					}
					nSplitLineIndexToAdd += nIndexDelta;
				}
			}

			//	Add end line
			if (!Util.IsEqual(SplitLines[SplitLines.Count - 1].Points[0], LocationCurve.GetEndPoint(1)))
			{
				AddSplitLine(nSplitLineIndexToAdd++, 0,
					LocationCurve.GetEndPoint(1),
					LocationCurve.GetEndPoint(1),
					OpeningSelectOption.None,
					true,
					true);
			}

			//	Check for Lintel & Sill
			//	Merge two neighbor lintels or sills together.
			for (i = 0; i < SplitLines.Count; i++)
			{
				SSplitLine sSL = SplitLines[i];
				if (sSL.Option == OpeningSelectOption.LintelAndSill
					&& sSL.Reserved % 3 == 1
				)
				{
					//	Horizontal line
					for (j = i + 1; j < SplitLines.Count; j++)
					{
						SSplitLine sSL1 = SplitLines[j];
						if (sSL1.Reserved % 3 != 1)
							continue;
						if (Util.IsEqual(sSL.Points[1], sSL1.Points[0]))
						{
							sSL.Points[1] = sSL1.Points[1];
							sSL.SplitCurveWithMargin.Dispose();
							sSL.SplitCurveWithMargin = Line.CreateBound(sSL.Points[0], sSL.Points[1]);
							SplitLines[i] = sSL;
							SplitLines.RemoveAt(j);
							SplitLines.RemoveAt(j - 1);
							int tmp = i + 1;
							while (tmp < SplitLines.Count)
							{
								if (SplitLines[tmp].ReferenceToSplitPoint == sSL.ReferenceToSplitPoint
									&& SplitLines[tmp].Reserved == sSL.Reserved + 1)
								{
									SplitLines.RemoveAt(tmp);
									break;
								}
								tmp++;
							}
							i--;
							break;
						}
					}
				}
			}
			//	Merge two overlapped lintels or sills together.
			for (i = 0; i < SplitLines.Count; i++)
			{
				SSplitLine sSL = SplitLines[i];
				if (sSL.Option == OpeningSelectOption.LintelAndSill
					&& sSL.Reserved % 3 == 1
				)
				{
					//	Horizontal line
					for (j = 0; j < SplitLines.Count; j++)
					{
						if (i == j)
							continue;

						SSplitLine sSL1 = SplitLines[j];
						if ((sSL.ReferenceToSplitPoint != sSL1.ReferenceToSplitPoint
							|| sSL1.Reserved < 0)
							&& sSL.SplitCurveWithMargin.Intersect(sSL1.SplitCurveWithMargin, out IntersectionResultArray ira) == SetComparisonResult.Overlap)
						{
							if (sSL1.Option != OpeningSelectOption.LintelAndSill)
							{
								if (sSL.Reserved >= 3)  //	Lintel
								{
									sSL1.Points[1] = ira.get_Item(0).XYZPoint;
									sSL1.SplitCurveWithMargin = Line.CreateBound(sSL1.SplitCurveWithMargin.GetEndPoint(0), sSL1.Points[1]);
								}
								else    //	Sill
								{
									sSL1.Points[0] = ira.get_Item(0).XYZPoint;
									sSL1.SplitCurveWithMargin = Line.CreateBound(sSL1.Points[0], sSL1.SplitCurveWithMargin.GetEndPoint(1));
								}
								SplitLines[j] = sSL1;
							}
							else
							{
								if (sSL.Reserved >= 3)  //	Lintel
								{
									if (Util.IsEqual(sSL1.Points[0], ira.get_Item(0).XYZPoint)
										&& !Util.IsEqual(sSL.Points[1], sSL1.Points[0]))
									{
										sSL.Points[1] = SplitLines[j + 1].Points[1];
										sSL.SplitCurveWithMargin = Line.CreateBound(sSL.Points[0], sSL.Points[1]);
										SplitLines[i] = sSL;
										if (sSL1.Reserved % 3 == 0)
										{
											SplitLines.RemoveAt(j + 1);
											SplitLines.RemoveAt(j);
										}
										else
										{
											SplitLines.RemoveAt(j);
											int tmp = j - 1;
											while (tmp > 0)
											{
												if (SplitLines[tmp].ReferenceToSplitPoint == sSL1.ReferenceToSplitPoint
													&& SplitLines[tmp].Reserved == sSL1.Reserved - 1)
												{
													SplitLines.RemoveAt(tmp);
													break;
												}
												tmp--;
											}
										}
									}
								}
								else    //	Sill
								{
									if (Util.IsEqual(sSL1.Points[1], ira.get_Item(0).XYZPoint)
										&& !Util.IsEqual(sSL.Points[1], sSL1.Points[1]))
									{
										sSL.Points[1] = SplitLines[j + 1].Points[1];
										sSL.SplitCurveWithMargin = Line.CreateBound(sSL.Points[0], sSL.Points[1]);
										SplitLines[i] = sSL;
										if (sSL1.Reserved % 3 == 0)
										{
											SplitLines.RemoveAt(j + 1);
											SplitLines.RemoveAt(j);
										}
										else
										{
											SplitLines.RemoveAt(j);
											int tmp = j - 1;
											while (tmp > 0)
											{
												if (SplitLines[tmp].ReferenceToSplitPoint == sSL1.ReferenceToSplitPoint
													&& SplitLines[tmp].Reserved == sSL1.Reserved - 1)
												{
													SplitLines.RemoveAt(tmp);
													break;
												}
												tmp--;
											}
										}
										j--;
									}
								}
							}
						}
					}
				}
			}

			for (i = 0; i < SplitLines.Count; i++)
			{
				SSplitLine sSL = SplitLines[i];
				if (sSL.Option == OpeningSelectOption.LintelAndSill
					&& sSL.Reserved % 3 == 1
				)
				{
					//	Horizontal line
					for (j = 0; j < SplitLines.Count; j++)
					{
						if (i == j)
							continue;

						SSplitLine sSL1 = SplitLines[j];
						if (sSL1.Option == OpeningSelectOption.LintelAndSill
							&& (sSL1.Reserved == sSL.Reserved + 1
								|| sSL1.Reserved == sSL.Reserved - 1)
							&& sSL.SplitCurveWithMargin.Intersect(sSL1.SplitCurveWithMargin, out IntersectionResultArray ira) == SetComparisonResult.Overlap
							)
						{
							if (!Util.IsEqual(sSL.Points[0], ira.get_Item(0).XYZPoint)
								&& !Util.IsEqual(sSL.Points[1], ira.get_Item(0).XYZPoint))
							{
								SplitLines.RemoveAt(j);
								j--;
							}
						}
					}
				}
			}

			//	The case which two openings are on one point(horizontally)
			for (i = 2; i < SplitPoints.Count; i++)
			{
				if (SplitPoints[i - 2].ReferenceToOpening >= 0
					&& SelectedOpenings[SplitPoints[i - 2].ReferenceToOpening].Option == OpeningSelectOption.LintelAndSill
					&& SplitPoints[i].ReferenceToOpening >= 0
					&& SelectedOpenings[SplitPoints[i].ReferenceToOpening].Option == OpeningSelectOption.LintelAndSill
					&& SplitPoints[i - 2].Reserved == 0
					&& Util.IsEqual(SplitPoints[i - 2].Point, SplitPoints[i].Point))
				{
					//	Get the last split line index for this split point
					for (j = SplitLines.Count - 1; j >= 0; j--)
					{
						if (SplitLines[j].ReferenceToSplitPoint == i)
							break;
					}
					SplitLines[j - 9].Points[0] = SplitLines[j - 9].Points[1];
					SplitLines[j - 7].Points[0] = SplitLines[j - 7].Points[1];
					SSplitLine sSL = SplitLines[j - 5];
					sSL.Points[0] = sSL.Points[0].Add(SplitLines[j - 8].Points[0] - sSL.Points[0]);
					sSL.IsCutByLocationCurve = false;
					sSL.SplitCurveWithMargin = Line.CreateBound(sSL.Points[0], sSL.Points[1]);
					SplitLines[j - 5] = sSL;

					sSL = SplitLines[j];
					sSL.Points[0] = SplitLines[j - 8].Points[1];
					sSL.IsCutByLocationCurve = false;
					sSL.SplitCurveWithMargin = Line.CreateBound(sSL.Points[0], sSL.Points[1]);
					SplitLines[j] = sSL;
				}
			}
		}

		//	Convert the 3D Point to 2D Point
		//		Project to the SketchPlane. Range is based on LocationCurve and MaxHeight
		public XYZ GetPoint2D(XYZ pt)
		{
			pt -= LocationCurve.GetEndPoint(0);
			double l = pt.GetLength();
			double angle = pt.AngleTo(LocationCurve.GetEndPoint(1) - LocationCurve.GetEndPoint(0));

			return new XYZ(l * Math.Cos(angle),
				l * Math.Sin(angle),
				0.0);
		}

		public void Prepare()
		{
			//AssociatedOpenings.Clear();
			//SelectedOpenings.Clear();
			SplitLines.Clear();
			SplitPoints.Clear();
			Panels.Clear();

			#region Determine the Start Point
			if (!m_bMustSelectStartPoint)
			{
				if (!SplitFromLeftToRight)
					m_ptStartPoint = LocationCurve.GetEndPoint(0);
				else
					m_ptStartPoint = LocationCurve.GetEndPoint(1);
			}
			#endregion
		}

		public ICollection<ElementId> Split()
		{
			Document doc = m_Splitter.GetDocument();

			SketchPlane sketchPlane = Autodesk.Revit.DB.SketchPlane.Create(doc, SketchPlane);

			List<Curve> lstCurves = new List<Curve>();
			if (AdjacentElements.Count > 0)
			{
				for (int i = 0; i < SplitLines.Count; i++)
				{
					lstCurves.Add(SplitLines[i].SplitCurveWithMargin);
				}
			}
			else
			{
				for (int i = 1; i < SplitLines.Count - 1; i++)
				{
					if (!Util.IsEqual(SplitLines[i].Points[0], SplitLines[i].Points[1]))
						lstCurves.Add(SplitLines[i].SplitCurveWithMargin);
				}
			}

			List<ElementId> elemList;
			ICollection<ElementId> parts;
			if (lstCurves.Count == 0)
			{
				elemList = new List<ElementId> { Element.Id };
				parts = new List<ElementId>();
				if (PartUtils.AreElementsValidForCreateParts(doc, elemList))
				{
					PartUtils.CreateParts(doc, elemList);
					doc.Regenerate();

					parts = PartUtils.GetAssociatedParts(doc, Element.Id, false, false);
				}
				else
				{
					if (Element is Part)
					{
						parts.Add(Element.Id);
					}
					else
					{
						parts = PartUtils.GetAssociatedParts(doc, Element.Id, false, false);
					}
				}
				return parts;
			}

			//	To DEBUG
			//	Enable these statements to view the split lines
			//CurveArray ca = new CurveArray();
			//foreach(SSplitLine sSL in SplitLines)
			//	ca.Append(sSL.SplitCurveWithMargin);
			//doc.Create.NewModelCurveArray(ca, sketchPlane);

			//	Create Part
			elemList = new List<ElementId> { Element.Id };
			parts = new List<ElementId>();
			if (PartUtils.AreElementsValidForCreateParts(doc, elemList))
			{
				PartUtils.CreateParts(doc, elemList);
				doc.Regenerate();

				parts = PartUtils.GetAssociatedParts(doc, Element.Id, false, false);
			}
			else
			{
				if (Element is Part)
				{
					parts.Add(Element.Id);
				}
				else
				{
					parts = PartUtils.GetAssociatedParts(doc, Element.Id, false, false);
				}
			}

			List<ElementId> intersectionElementIds = new List<ElementId>();
			//	Divide Part
			PartMaker _ = PartUtils.DivideParts(doc, parts, intersectionElementIds, lstCurves, sketchPlane.Id);
			doc.Regenerate();
			ICollection<ElementId> generatedParts = PartUtils.GetAssociatedParts(doc, Element.Id, true, true);
			if (generatedParts.Count == 0)
				foreach (ElementId eId in parts)
					generatedParts.Add(eId);
			else
				foreach (ElementId eId in parts)
					generatedParts.Remove(eId);
			return generatedParts;
		}

		public void GetPanels()
		{
			Document doc = m_Splitter.GetDocument();
			int i;

			ICollection<ElementId> parts;
			Transaction trans = new Transaction(doc);
			trans.Start("Split Part");

			//	Backup ElementId
			ElementId id = Element.Id;

			if ((parts = Split()) != null)
			{
				foreach (ElementId eId in parts)
				{
					Element e = doc.GetElement(eId);
					if (e != null && e is Part)
					{
						Face f = Util.GetBaseFacesOfElement(doc, e)[0];
						IList<CurveLoop> lstCurveloop = f.GetEdgesAsCurveLoops();
						//	Find the boundary CurveLoop(the longest one)
						CurveLoop boundary = null;
						double fMax = 0.0;
						foreach (CurveLoop cl in lstCurveloop)
						{
							if (fMax < cl.GetExactLength())
							{
								boundary = cl;
								fMax = cl.GetExactLength();
							}
						}

						List<XYZ> pts = new List<XYZ>();
						//	Remove Unnecessary points
						Curve prev = null;
						foreach (Curve c in boundary)
						{
							if (!(c is Line))
							{
								continue;
							}

							if (pts.Count >= 2)
							{
								if (Util.IsZero(prev.Distance(c.GetEndPoint(1))))
								{
									pts[pts.Count - 1] = c.GetEndPoint(1);
								}
								else
								{
									pts.Add(c.GetEndPoint(1));
								}
							}
							else
							{
								pts.Add(c.GetEndPoint(1));
							}

							if (prev != null) prev.Dispose();
							prev = c.Clone();
							prev.MakeUnbound();
						}
						//	Check if the last point is necessary
						if (Util.IsZero(prev.Distance(pts[0])))
						{
							pts.RemoveAt(pts.Count - 1);
						}
						prev.Dispose();

						CPanel panel = new CPanel();
						XYZ maxPt = null, minPt = null;
						panel.Points = new XYZ[pts.Count];
						panel.Point2Ds = new XYZ[pts.Count];
						i = 0;
						foreach (XYZ pt in pts)
						{
							panel.Points[i] = pt;
							panel.Point2Ds[i] = GetPoint2D(pt);
							if (maxPt == null)
							{
								minPt = panel.Point2Ds[i];
								maxPt = panel.Point2Ds[i];
							}
							else
							{
								if (panel.Point2Ds[i].X < minPt.X)
									minPt = new XYZ(panel.Point2Ds[i].X, minPt.Y, 0.0);
								if (panel.Point2Ds[i].X > maxPt.X)
									maxPt = new XYZ(panel.Point2Ds[i].X, maxPt.Y, 0.0);
								if (panel.Point2Ds[i].Y < minPt.Y)
									minPt = new XYZ(minPt.X, panel.Point2Ds[i].Y, 0.0);
								if (panel.Point2Ds[i].Y > maxPt.Y)
									maxPt = new XYZ(maxPt.X, panel.Point2Ds[i].Y, 0.0);
							}
							i++;
						}
						panel.MaxPoint2D = maxPt;
						panel.MinPoint2D = minPt;
						panel.ComputeNetArea();
						panel.BoundingArea = (maxPt.X - minPt.X) * (maxPt.Y - minPt.Y);
						Panels.Add(panel);
					}
				}
			}

			FailureHandlingOptions opt = trans.GetFailureHandlingOptions();
			opt = opt.SetDelayedMiniWarnings(false);
			opt = opt.SetForcedModalHandling(false);
			opt = opt.SetClearAfterRollback(true);
			trans.RollBack(opt);

			if (!Element.IsValidObject)
			{
				Element = doc.GetElement(id);
			}
		}

		public double GetArea(int nType)
		{
			double fArea;
			int i = 0;

			fArea = 0.0;
			if (nType == 2)
			{
				for (i = 0; i < Panels.Count; i++)
				{
					fArea += Panels[i].BoundingArea;
				}
			}
			else if (nType == 0)
			{
				Parameter param = Element.LookupParameter("Area");
				//Parameter param = Element.get_Parameter(BuiltInParameter.HOST_AREA_COMPUTED);
				if (param == null)
					return 0;
				return param.AsDouble();
			}
			else
			{
				double fNetArea = 0.0;
				Parameter param = Element.LookupParameter("Area");
				if (param != null)
					fNetArea = param.AsDouble();

#if REVIT2019
				XYZ[] points = new XYZ[BoundaryCurves[0].Count()];
#else
				XYZ[] points = new XYZ[BoundaryCurves[0].NumberOfCurves()];
#endif

				foreach (Curve c in BoundaryCurves[0])
				{
					points[i++] = c.GetEndPoint(0);
				}
				fArea = Util.ComputeArea(points);

				foreach (COpening sO in SelectedOpenings)
				{
					if (sO.Option == OpeningSelectOption.LintelAndSill)
					{
						bool bIn = false;
						foreach (XYZ pt in sO.Points)
						{
							if (Util.IsPointInsideCurveLoop(BoundaryCurves[0], pt, false))
							{
								bIn = true;
								break;
							}
						}
						if (bIn)
							fArea -= sO.Area;
					}
				}

				if (fArea < fNetArea)
					fArea = fNetArea;
			}

			return fArea;
		}

		/********	Functions related to analysis	********/
		public void GetExistingPanels()
		{
			Document doc = m_Splitter.GetDocument();
			int i;

			ICollection<ElementId> parts = PartUtils.GetAssociatedParts(doc, Element.Id, true, true);

			m_nOriginalPartCount = parts.Count;

			Panels.Clear();
			//	Backup ElementId
			ElementId id = Element.Id;

			foreach (ElementId eId in parts)
			{
				Element e = doc.GetElement(eId);

				//	Check if this part has child parts
				if(PartUtils.GetAssociatedParts(doc, eId, true, true).Count > 1)
				{
					continue;
				}

				IList<Curve> lstCurves = PartUtils.GetSplittingCurves(doc, eId);
				foreach (Curve c in lstCurves)
				{
					if (!SplitLines.Exists(x => Util.IsEqual(x.Points[0], c.GetEndPoint(0)) && Util.IsEqual(x.Points[1], c.GetEndPoint(1))))
						AddSplitLine(0, -1, c.GetEndPoint(0), c.GetEndPoint(1), new OpeningSelectOption(), false, false, -1);
				}

				if (e != null && e is Part)
				{
					Face f = Util.GetBaseFacesOfElement(doc, e)[0];//BaseFaces[0];//
					IList<CurveLoop> lstCurveloop = f.GetEdgesAsCurveLoops();
					//	Find the boundary CurveLoop(the longest one)
					CurveLoop boundary = null;
					double fMax = 0.0;
					foreach (CurveLoop cl in lstCurveloop)
					{
						if (fMax < cl.GetExactLength())
						{
							boundary = cl;
							fMax = cl.GetExactLength();
						}
					}

					List<XYZ> pts = new List<XYZ>();
					//	Remove Unnecessary points
					Curve prev = null;
					foreach (Curve c in boundary)
					{
						if (!(c is Line))
						{
							continue;
						}

						if (pts.Count >= 2)
						{
							if (Util.IsZero(prev.Distance(c.GetEndPoint(1))))
							{
								pts[pts.Count - 1] = c.GetEndPoint(1);
							}
							else
							{
								pts.Add(c.GetEndPoint(1));
							}
						}
						else
						{
							pts.Add(c.GetEndPoint(1));
						}

						if (prev != null) prev.Dispose();
						prev = c.Clone();
						prev.MakeUnbound();
					}
					//	Check if the last point is necessary
					if (Util.IsZero(prev.Distance(pts[0])))
					{
						pts.RemoveAt(pts.Count - 1);
					}
					prev.Dispose();

					CPanel panel = new CPanel();
					XYZ maxPt = null, minPt = null;
					panel.Points = new XYZ[pts.Count];
					panel.Point2Ds = new XYZ[pts.Count];
					i = 0;
					foreach (XYZ pt in pts)
					{
						panel.Points[i] = pt;
						panel.Point2Ds[i] = GetPoint2D(pt);
						if (maxPt == null)
						{
							minPt = panel.Point2Ds[i];
							maxPt = panel.Point2Ds[i];
						}
						else
						{
							if (panel.Point2Ds[i].X < minPt.X)
								minPt = new XYZ(panel.Point2Ds[i].X, minPt.Y, 0.0);
							if (panel.Point2Ds[i].X > maxPt.X)
								maxPt = new XYZ(panel.Point2Ds[i].X, maxPt.Y, 0.0);
							if (panel.Point2Ds[i].Y < minPt.Y)
								minPt = new XYZ(minPt.X, panel.Point2Ds[i].Y, 0.0);
							if (panel.Point2Ds[i].Y > maxPt.Y)
								maxPt = new XYZ(maxPt.X, panel.Point2Ds[i].Y, 0.0);
						}
						i++;
					}
					panel.MaxPoint2D = maxPt;
					panel.MinPoint2D = minPt;
					panel.ComputeNetArea();
					panel.BoundingArea = (maxPt.X - minPt.X) * (maxPt.Y - minPt.Y);
					Panels.Add(panel);
				}
			}
		}

		protected void AddSelectedOpening(COpening opening)
		{
			SelectedOpenings.Add(opening);
			m_Splitter.SelectedOpenings.Add(opening);
		}

		public void ExtractSplitInformation()
		{
			int i, j, slIndex;
			Document doc = m_Splitter.GetDocument();
			List<COpening> openings = m_Splitter.GetAssociatedOpenings(Element.Id);

			Curve unboundLocationCurve = LocationCurve.Clone();
			unboundLocationCurve.MakeUnbound();

			//	Cut Split lines first
			for (slIndex = 0; slIndex < SplitLines.Count; slIndex++)
			{
				SSplitLine splitLine = SplitLines[slIndex];
				XYZ ptStart = splitLine.Points[0]
					, ptEnd = splitLine.Points[1];

				bool isVertical = Util.IsEqual(unboundLocationCurve.Project(ptStart).XYZPoint, unboundLocationCurve.Project(ptEnd).XYZPoint);

				//	If these split lines are not created by our tool, they can be located on different face.
				//	So we will project the points to our sketchplane
				ptStart = SketchPlane.ProjectOnto(ptStart);
				ptEnd = SketchPlane.ProjectOnto(ptEnd);

				//	Make the start point to the bottom, left point
				if(
					(!isVertical
						&& LocationCurve.GetEndPoint(0).DistanceTo(ptStart) > LocationCurve.GetEndPoint(0).DistanceTo(ptEnd)) //	Horizontal Line
					|| (isVertical
						&& !Util.IsEqual((ptEnd - unboundLocationCurve.Project(ptEnd).XYZPoint).Normalize(), PV))										//	Vertical line which the second point is under the LocationCurve
					|| (isVertical
						&& Util.IsEqual((ptEnd - unboundLocationCurve.Project(ptEnd).XYZPoint).Normalize(), PV)
						&& LocationCurve.GetEndPoint(0).DistanceTo(ptStart) > LocationCurve.GetEndPoint(0).DistanceTo(ptEnd))
					)
				{
					XYZ ptTmp = ptStart;
					ptStart = ptEnd;
					ptEnd = ptTmp;
				}
				splitLine.Points[0] = ptStart;
				splitLine.Points[1] = ptEnd;

				splitLine.SplitCurveWithMargin = Line.CreateBound(ptStart, ptEnd);
				
				//	We don't cut the horizontal lines
				if (isVertical
					&& !Util.IsPointInsideCurveLoop(BoundaryCurves[0], ptStart))
				{
					splitLine.IsCutByLocationCurve = true;
					//	Find all intersect points
					List<XYZ> points = new();
					foreach(Curve c in BoundaryCurves[0])
					{
						if(c.Intersect(splitLine.SplitCurveWithMargin, out IntersectionResultArray ira) == SetComparisonResult.Overlap)
						{
							points.Add(ira.get_Item(0).XYZPoint);
						}
					}

					if (points.Count > 0)
					{
						//	Find the nearest point
						XYZ nearest = points[0];
						foreach (XYZ pt in points)
						{
							if (nearest.DistanceTo(ptStart) > pt.DistanceTo(ptStart))
								nearest = pt;
						}
						splitLine.Points[0] = nearest;
					}
				}

				if (isVertical
					&& !Util.IsPointInsideCurveLoop(BoundaryCurves[0], ptEnd))
				{
					splitLine.IsCutByEdge = true;
					//	Find all intersect points
					List<XYZ> points = new();
					foreach (Curve c in BoundaryCurves[0])
					{
						if (c.Intersect(splitLine.SplitCurveWithMargin, out IntersectionResultArray ira) == SetComparisonResult.Overlap)
						{
							points.Add(ira.get_Item(0).XYZPoint);
						}
					}

					//	Find the nearest point
					if (points.Count > 0)
					{
						XYZ nearest = points[0];
						foreach (XYZ pt in points)
						{
							if (nearest.DistanceTo(ptEnd) > pt.DistanceTo(ptEnd))
								nearest = pt;
						}
						splitLine.Points[1] = nearest;
					}
				}

				SplitLines[slIndex] = splitLine;
			}

			//	Sort split lines from the start point to the end point of LocationCurve
			XYZ gPtStart = LocationCurve.GetEndPoint(0);
			for (i = 0; i < SplitLines.Count; i++)
			{
				for (j = i + 1; j < SplitLines.Count; j++)
				{
					double[] fDistancesToFirst = [gPtStart.DistanceTo(unboundLocationCurve.Project(SplitLines[i].Points[0]).XYZPoint), gPtStart.DistanceTo(unboundLocationCurve.Project(SplitLines[j].Points[0]).XYZPoint)];
					if (fDistancesToFirst[0] > fDistancesToFirst[1]) {
						//	Normal vertical split lines
						SSplitLine sl = SplitLines[i];
						SplitLines[i] = SplitLines[j];
						SplitLines[j] = sl;
					}
				}
			}

			//	Find LS split lines first
			for (i = 0; i < SplitLines.Count; i++)
			{
				//	Check horizontal lines
				if (Util.IsEqual(unboundLocationCurve.Project(SplitLines[i].Points[0]).XYZPoint
						, unboundLocationCurve.Project(SplitLines[i].Points[1]).XYZPoint))
				{
					continue;
				}

				int nLintelLeft = 0, nLintelRight = 0, nCillLeft = 0, nCillRight = 0;
				for (; nLintelLeft < SplitLines.Count; nLintelLeft++)
				{
					if (nLintelLeft == i)
						continue;

					if (Util.IsEqual(SplitLines[nLintelLeft].Points[0], SplitLines[i].Points[0]))
						break;
				}
				for (; nLintelRight < SplitLines.Count; nLintelRight++)
				{
					if (nLintelRight == i || nLintelRight == nLintelLeft)
						continue;

					if (Util.IsEqual(SplitLines[nLintelRight].Points[0], SplitLines[i].Points[1]))
						break;
				}
				for (; nCillLeft < SplitLines.Count; nCillLeft++)
				{
					if (nCillLeft == i)
						continue;

					if (Util.IsEqual(SplitLines[nCillLeft].Points[1], SplitLines[i].Points[0]))
						break;
				}
				for (; nCillRight < SplitLines.Count; nCillRight++)
				{
					if (nCillRight == i || nCillRight == nCillLeft)
						continue;

					if (Util.IsEqual(SplitLines[nCillRight].Points[1], SplitLines[i].Points[1]))
						break;
				}

				if (nLintelLeft < SplitLines.Count
					&& nLintelRight < SplitLines.Count)
				{
					//AddSplitPoint(new SSplitPoint
					//{
					//	Point = SplitLines[i].Points[0],
					//	//ReferenceToOpening = SelectedOpenings.Count - 1,
					//	Reserved = 0
					//});

					SSplitLine tmp = SplitLines[i];
					//tmp.Option = OpeningSelectOption.LintelAndSill;
					//tmp.Reserved = 1;
					//tmp.ReferenceToSplitPoint = SplitPoints.Count - 1;
					SplitLines[i] = tmp;

					tmp = SplitLines[nLintelLeft];
					//tmp.Option = OpeningSelectOption.LintelAndSill;
					//tmp.Reserved = 0;
					//tmp.ReferenceToSplitPoint = SplitPoints.Count - 1;
					SplitLines[nLintelLeft] = tmp;

					tmp = SplitLines[nLintelRight];
					//tmp.Option = OpeningSelectOption.LintelAndSill;
					//tmp.Reserved = 2;
					//tmp.ReferenceToSplitPoint = SplitPoints.Count - 1;
					SplitLines[nLintelRight] = tmp;

					//	Swap elements
					SSplitLine tmpLeft = SplitLines[nLintelLeft]
									, tmpRight = SplitLines[nLintelRight];
					SplitLines.Remove(tmpLeft);
					SplitLines.Remove(tmpRight);
					if (nLintelLeft < i)
						i--;
					if (nLintelRight < i)
						i--;
					SplitLines.Insert(i + 1, tmpRight);
					SplitLines.Insert(i, tmpLeft);

					i++;
				}
				else if (nCillLeft < SplitLines.Count
					&& nCillRight < SplitLines.Count)
				{
					//AddSplitPoint(new SSplitPoint
					//{
					//	Point = SplitLines[i].Points[0],
						//ReferenceToOpening = SelectedOpenings.Count - 1,
					//	Reserved = 1
					//});

					SSplitLine tmp = SplitLines[i];
					//tmp.Option = OpeningSelectOption.LintelAndSill;
					//tmp.Reserved = 4;
					//tmp.ReferenceToSplitPoint = SplitPoints.Count - 1;
					SplitLines[i] = tmp;

					tmp = SplitLines[nCillLeft];
					//tmp.Option = OpeningSelectOption.LintelAndSill;
					//tmp.Reserved = 3;
					//tmp.ReferenceToSplitPoint = SplitPoints.Count - 1;
					SplitLines[nCillLeft] = tmp;

					tmp = SplitLines[nCillRight];
					//tmp.Option = OpeningSelectOption.LintelAndSill;
					//tmp.Reserved = 5;
					//tmp.ReferenceToSplitPoint = SplitPoints.Count - 1;
					SplitLines[nCillRight] = tmp;

					//	Swap elements
					SSplitLine tmpLeft = SplitLines[nCillLeft]
									, tmpRight = SplitLines[nCillRight];
					SplitLines.Remove(tmpLeft);
					SplitLines.Remove(tmpRight);
					if (nCillLeft < i)
						i--;
					if (nCillRight < i)
						i--;
					SplitLines.Insert(i + 1, tmpRight);
					SplitLines.Insert(i, tmpLeft);

					i++;
				}
			}

			//	Analyse split lines
			for (slIndex = 0; slIndex < SplitLines.Count; slIndex++)
			{
				SSplitLine splitline = SplitLines[slIndex];

				if (splitline.ReferenceToSplitPoint >= 0)
				{
					//	Check if this split line is already recognized
					continue;
				}

				XYZ ptStart = GetPoint2D(splitline.Points[0]),
						ptEnd = GetPoint2D(splitline.Points[1]);

				//	Lintel and Cill
				//	Use case for both Lintel and Cill exists
				if (slIndex < SplitLines.Count - 5)
				{
					SSplitLine sl1 = SplitLines[slIndex + 1],
							sl2 = SplitLines[slIndex + 2],
							sl3 = SplitLines[slIndex + 3],
							sl4 = SplitLines[slIndex + 4],
							sl5 = SplitLines[slIndex + 5];

					if (Util.IsEqual(splitline.Points[0], sl1.Points[0])
						&& Util.IsEqual(sl1.Points[1], sl2.Points[0])
						&& Util.IsEqual(sl3.Points[0], sl4.Points[0])
						&& Util.IsEqual(sl4.Points[1], sl5.Points[0]))
					{
						//	Find the appropriate opening
						foreach (COpening opening in openings)
						{
							Line topLine = Line.CreateBound(opening.Points[0], opening.Points[1]);
							Line bottomLine = Line.CreateBound(opening.Points[2], opening.Points[3]);
							SetComparisonResult irTop = sl1.SplitCurveWithMargin.Intersect(topLine);
							SetComparisonResult irBottom = sl1.SplitCurveWithMargin.Intersect(bottomLine);
							if ((irTop == SetComparisonResult.Equal
								|| irTop == SetComparisonResult.Subset
								|| irTop == SetComparisonResult.Superset)
								&& (irBottom == SetComparisonResult.Equal
								|| irBottom == SetComparisonResult.Subset
								|| irBottom == SetComparisonResult.Superset))
							{
								//	Lintel
								opening.Option = OpeningSelectOption.LintelAndSill;
								AddSelectedOpening(opening);

								AddSplitPoint(new SSplitPoint
								{
									Point = opening.Points[0],
									ReferenceToOpening = SelectedOpenings.Count - 1,
									Reserved = 0
								});

								splitline.ReferenceToSplitPoint = SplitPoints.Count - 1;
								splitline.Reserved = 3;
								splitline.Option = OpeningSelectOption.LintelAndSill;

								sl1.ReferenceToSplitPoint = SplitPoints.Count - 1;
								sl1.Reserved = 4;
								sl1.Option = OpeningSelectOption.LintelAndSill;
								SplitLines[slIndex + 1] = sl1;

								sl2.ReferenceToSplitPoint = SplitPoints.Count - 1;
								sl2.Reserved = 5;
								sl2.Option = OpeningSelectOption.LintelAndSill;
								SplitLines[slIndex + 2] = sl2;

								int nLintelLength = Util.RoundToRealWorld(UnitUtils.ConvertFromInternalUnits(
										opening.Points[0].DistanceTo(sl1.Points[0])
										, CCommonSettings.Unit));
								if (m_nLintelBearing > 0 && m_nLintelBearing != nLintelLength)
								{
									//throw new Exception("Using different lintel bearing length is not supported");
								}
								else
								{
									m_nLintelBearing = nLintelLength;
								}

								//	Left
								AddSplitPoint(new SSplitPoint
								{
									Point = opening.Points[3],
									ReferenceToOpening = SelectedOpenings.Count - 1,
									Reserved = 1
								});

								sl3.ReferenceToSplitPoint = SplitPoints.Count - 1;
								sl3.Reserved = 0;
								sl3.Option = OpeningSelectOption.LintelAndSill;
								SplitLines[slIndex + 3] = sl3;

								sl4.ReferenceToSplitPoint = SplitPoints.Count - 1;
								sl4.Reserved = 1;
								sl4.Option = OpeningSelectOption.LintelAndSill;
								SplitLines[slIndex + 4] = sl4;

								sl5.ReferenceToSplitPoint = SplitPoints.Count - 1;
								sl5.Reserved = 2;
								sl5.Option = OpeningSelectOption.LintelAndSill;
								SplitLines[slIndex + 5] = sl5;

								int nCillLength = Util.RoundToRealWorld(UnitUtils.ConvertFromInternalUnits(
										opening.Points[3].DistanceTo(sl4.Points[0])
										, CCommonSettings.Unit));
								if (m_nCillBearing > 0 && m_nCillBearing != nCillLength)
								{
									throw new Exception("Using different cill bearing length is not supported");
								}
								else
								{
									m_nCillBearing = nCillLength;
								}
							}
						}
					}
				}

				if (splitline.Option != OpeningSelectOption.None)
				{
					//	If this split line is always recognized, we move to the next one
					SplitLines[slIndex] = splitline;
					continue;
				}
				//	Use case for only one of Lintel and Cill exists
				if (slIndex < SplitLines.Count - 2)
				{
					SSplitLine nextSplitLine = SplitLines[slIndex + 1],
							nextNextSplitLine = SplitLines[slIndex + 2];

					if (Util.IsEqual(splitline.Points[0], nextSplitLine.Points[0])
						&& Util.IsEqual(nextSplitLine.Points[1], nextNextSplitLine.Points[0]))
					{
						//	Find the appropriate opening
						foreach (COpening opening in openings)
						{
							Line topLine = Line.CreateBound(opening.Points[0], opening.Points[1]);
							SetComparisonResult irTop = nextSplitLine.SplitCurveWithMargin.Intersect(topLine);
							if (irTop == SetComparisonResult.Equal
								|| irTop == SetComparisonResult.Subset
								|| irTop == SetComparisonResult.Superset)
							{
								//	Lintel
								opening.Option = OpeningSelectOption.LintelAndSill;
								AddSelectedOpening(opening);

								int nLintelLength = Util.RoundToRealWorld(UnitUtils.ConvertFromInternalUnits(
										opening.Points[0].DistanceTo(nextSplitLine.Points[0])
										, CCommonSettings.Unit));
								if (m_nLintelBearing > 0 && m_nLintelBearing != nLintelLength)
								{
									//throw new Exception("Using different lintel bearing length is not supported");
								}
								else
								{
									m_nLintelBearing = nLintelLength;
								}

								//	Left
								AddSplitPoint(new SSplitPoint
								{
									Point = opening.Points[0],
									ReferenceToOpening = SelectedOpenings.Count - 1,
									Reserved = 0
								});

								splitline.ReferenceToSplitPoint = SplitPoints.Count - 1;
								splitline.Reserved = 3;
								splitline.Option = OpeningSelectOption.LintelAndSill;

								nextSplitLine.ReferenceToSplitPoint = SplitPoints.Count - 1;
								nextSplitLine.Reserved = 4;
								nextSplitLine.Option = OpeningSelectOption.LintelAndSill;
								SplitLines[slIndex + 1] = nextSplitLine;

								nextNextSplitLine.ReferenceToSplitPoint = SplitPoints.Count - 1;
								nextNextSplitLine.Reserved = 5;
								nextNextSplitLine.Option = OpeningSelectOption.LintelAndSill;
								SplitLines[slIndex + 2] = nextNextSplitLine;
								break;
							}

							Line bottomLine = Line.CreateBound(opening.Points[2], opening.Points[3]);
							SetComparisonResult irBottom = nextSplitLine.SplitCurveWithMargin.Intersect(bottomLine);
							if (irBottom == SetComparisonResult.Equal
								|| irBottom == SetComparisonResult.Subset
								|| irBottom == SetComparisonResult.Superset)
							{
								//	Cill
								opening.Option = OpeningSelectOption.LintelAndSill;
								AddSelectedOpening(opening);

								int nCillLength = Util.RoundToRealWorld(UnitUtils.ConvertFromInternalUnits(
										opening.Points[3].DistanceTo(nextSplitLine.Points[0])
										, CCommonSettings.Unit));
								if (m_nCillBearing > 0 && m_nCillBearing != nCillLength)
								{
									//throw new Exception("Using different cill bearing length is not supported");
								}
								else
								{
									m_nCillBearing = nCillLength;
								}

								AddSplitPoint(new SSplitPoint
								{
									Point = opening.Points[3],
									ReferenceToOpening = SelectedOpenings.Count - 1,
									Reserved = 1
								});

								splitline.ReferenceToSplitPoint = SplitPoints.Count - 1;
								splitline.Reserved = 0;
								splitline.Option = OpeningSelectOption.LintelAndSill;

								nextSplitLine.ReferenceToSplitPoint = SplitPoints.Count - 1;
								nextSplitLine.Reserved = 1;
								nextSplitLine.Option = OpeningSelectOption.LintelAndSill;
								SplitLines[slIndex + 1] = nextSplitLine;

								nextNextSplitLine.ReferenceToSplitPoint = SplitPoints.Count - 1;
								nextNextSplitLine.Reserved = 2;
								nextNextSplitLine.Option = OpeningSelectOption.LintelAndSill;
								SplitLines[slIndex + 2] = nextNextSplitLine;
							}
						}
					}
				}
				if (splitline.Option != OpeningSelectOption.None)
				{
					//	If this split line is always recognized, we move to the next one
					SplitLines[slIndex] = splitline;
					continue;
				}

				for (i = 0; i < openings.Count; i++)
				{
					XYZ projectedLocation = GetPoint2D(openings[i].ProjectedLocation);
					//	Centre of Opening
					if (Util.IsEqual(ptStart.X, ptEnd.X)
						&& Util.IsEqual(ptStart.X, projectedLocation.X))
					{
						openings[i].Option = OpeningSelectOption.CentreOfOpening;
						AddSelectedOpening(openings[i]);

						AddSplitPoint(new SSplitPoint
						{
							Point = openings[i].ProjectedLocation,
							ReferenceToOpening = SelectedOpenings.Count - 1,
							Reserved = 0
						});

						splitline.ReferenceToSplitPoint = SplitPoints.Count - 1;
						splitline.Reserved = 0;
						splitline.Option = OpeningSelectOption.CentreOfOpening;

						break;
					}
				}
				if (splitline.Option != OpeningSelectOption.None)
				{
					//	If this split line is always recognized, we move to the next one
					SplitLines[slIndex] = splitline;
					continue;
				}

				//	Equal distance between center of openings
				for (i = 0; i < openings.Count; i++)
				{
					XYZ projectedLocation1 = GetPoint2D(openings[i].ProjectedLocation);
					for (j = i + 1; j < openings.Count; j++)
					{
						XYZ projectedLocation2 = GetPoint2D(openings[j].ProjectedLocation);
						if (Util.IsEqual(ptStart.X, ptEnd.X)
							&& Util.IsEqual(projectedLocation1.X - ptStart.X, ptStart.X - projectedLocation2.X))
						{
							openings[i].Option = OpeningSelectOption.EqualDistanceBetweenOpenings;
							AddSelectedOpening(openings[i]);
							openings[j].Option = OpeningSelectOption.EqualDistanceBetweenOpenings;
							AddSelectedOpening(openings[j]);

							AddSplitPoint(new SSplitPoint
							{
								Point = ptStart,
								ReferenceToOpening = SelectedOpenings.Count - 2,
								Reserved = SelectedOpenings.Count - 1
							});

							splitline.ReferenceToSplitPoint = SplitPoints.Count - 1;
							splitline.Reserved = 0;
							splitline.Option = OpeningSelectOption.EqualDistanceBetweenOpenings;

							break;
						}
					}
				}
				if (splitline.Option != OpeningSelectOption.None)
				{
					//	If this split line is always recognized, we move to the next one
					SplitLines[slIndex] = splitline;
					continue;
				}

				if (slIndex < SplitLines.Count - 1)
				{
					SSplitLine nextSplitline = SplitLines[slIndex + 1];
					XYZ ptNextStart = GetPoint2D(nextSplitline.Points[0]),
							ptNextEnd = GetPoint2D(nextSplitline.Points[1]);

					//	Around centre of Opening
					//	TODO: We assume there will be no other split lines between these two split lines
					for (i = 0; i < openings.Count; i++)
					{
						XYZ projectedLocation = GetPoint2D(openings[i].ProjectedLocation);
						if (Util.IsEqual(ptStart.X, ptEnd.X)
							&& Util.IsEqual(ptNextStart.X, ptNextEnd.X)
							&& Util.IsEqual(projectedLocation.X - ptStart.X, ptNextStart.X - projectedLocation.X))
						{
							openings[i].Option = OpeningSelectOption.AroundCentreOfOpening;
							AddSelectedOpening(openings[i]);

							//	Left
							AddSplitPoint(new SSplitPoint
							{
								Point = ptStart,
								ReferenceToOpening = SelectedOpenings.Count - 1,
								Reserved = 0
							});

							splitline.ReferenceToSplitPoint = SplitPoints.Count - 1;
							splitline.Reserved = 0;
							splitline.Option = OpeningSelectOption.AroundCentreOfOpening;

							//	Right
							AddSplitPoint(new SSplitPoint
							{
								Point = ptNextStart,
								ReferenceToOpening = SelectedOpenings.Count - 1,
								Reserved = 1
							});

							nextSplitline.ReferenceToSplitPoint = SplitPoints.Count - 1;
							nextSplitline.Reserved = 1;
							nextSplitline.Option = OpeningSelectOption.AroundCentreOfOpening;
							SplitLines[slIndex + 1] = nextSplitline;

							int nAroundCentreLength = Util.RoundToRealWorld(UnitUtils.ConvertFromInternalUnits(
									ptNextStart.DistanceTo(ptStart)
									, CCommonSettings.Unit));
							if(m_nAroundCentreSplitWidth > 0 && m_nAroundCentreSplitWidth != nAroundCentreLength)
							{
								//throw new Exception("Using different around centre length is not supported");
							}
							else
							{
								m_nAroundCentreSplitWidth = nAroundCentreLength;
							}

							break;
						}
					}
				}
				if (splitline.Option != OpeningSelectOption.None)
				{
					//	If this split line is always recognized, we move to the next one
					SplitLines[slIndex] = splitline;
					continue;
				}
			}

			//	Extract Split Width
			//	Generate new split lines arrays for further process
			List<SSplitLine> lines = new();
			lines.Add(new SSplitLine()
			{
				ReferenceToSplitPoint = 0,
				Option = OpeningSelectOption.None,
				Points = new XYZ[] { LocationCurve.GetEndPoint(0), LocationCurve.GetEndPoint(0) },
				Reserved = 0
			});
			for (slIndex = 0; slIndex < SplitLines.Count; slIndex++)
			{
				SSplitLine sl = SplitLines[slIndex];
				if(sl.Option != OpeningSelectOption.LintelAndSill
					|| sl.Reserved % 3 != 1)
				{
					lines.Add(new SSplitLine()
					{
						ReferenceToSplitPoint = sl.ReferenceToSplitPoint,
						Option = sl.Option,
						Points = new XYZ[] { unboundLocationCurve.Project(sl.Points[0]).XYZPoint, new XYZ() },
						Reserved = sl.Reserved
					});
				}
			}
			lines.Add(new SSplitLine()
			{
				ReferenceToSplitPoint = 0,
				Option = OpeningSelectOption.None,
				Points = new XYZ[] { LocationCurve.GetEndPoint(1), LocationCurve.GetEndPoint(1) },
				Reserved = 0
			});
			
			//	Extract common split width and direction
			//	We think if there is not split lines with None option, the width is just a minimum width value.
			//	TODO
			//	We don't extract the direction for now. No specific algorithm to extract.

			double fWidth = 0;
			for (slIndex = 1; slIndex < lines.Count; slIndex++)
			{
				SSplitLine sl = lines[slIndex];
				SSplitLine sl1 = lines[slIndex - 1];

				double fTmp = 0;
				if((sl.Option == OpeningSelectOption.LintelAndSill && sl1.Option == OpeningSelectOption.LintelAndSill
					&& SplitPoints[sl.ReferenceToSplitPoint].ReferenceToOpening == SplitPoints[sl1.ReferenceToSplitPoint].ReferenceToOpening)
					|| (sl.Option == OpeningSelectOption.AroundCentreOfOpening && sl1.Option == OpeningSelectOption.AroundCentreOfOpening
						&& SplitPoints[sl.ReferenceToSplitPoint].ReferenceToOpening == SplitPoints[sl1.ReferenceToSplitPoint].ReferenceToOpening))
				{
					continue;
				}
				else if((sl.Option == OpeningSelectOption.None && sl1.Option == OpeningSelectOption.LintelAndSill)
					|| (sl.Option == OpeningSelectOption.CentreOfOpening && sl1.Option == OpeningSelectOption.LintelAndSill)
					|| (sl.Option == OpeningSelectOption.AroundCentreOfOpening && sl1.Option == OpeningSelectOption.LintelAndSill)
					|| (sl.Option == OpeningSelectOption.EqualDistanceBetweenOpenings && sl1.Option == OpeningSelectOption.LintelAndSill))
				{
					fTmp = sl.Points[0].DistanceTo(sl1.Points[0]);
					if (sl1.Reserved >= 3)
						fTmp += UnitUtils.ConvertToInternalUnits(m_nLintelBearing, CCommonSettings.Unit);
					else
						fTmp += UnitUtils.ConvertToInternalUnits(m_nCillBearing, CCommonSettings.Unit);
				}
				else if ((sl1.Option == OpeningSelectOption.None && sl.Option == OpeningSelectOption.LintelAndSill)
					|| (sl1.Option == OpeningSelectOption.CentreOfOpening && sl.Option == OpeningSelectOption.LintelAndSill)
					|| (sl1.Option == OpeningSelectOption.AroundCentreOfOpening && sl.Option == OpeningSelectOption.LintelAndSill)
					|| (sl1.Option == OpeningSelectOption.EqualDistanceBetweenOpenings && sl.Option == OpeningSelectOption.LintelAndSill))
				{
					fTmp = sl.Points[0].DistanceTo(sl1.Points[0]);
					if (sl.Reserved >= 3)
						fTmp += UnitUtils.ConvertToInternalUnits(m_nLintelBearing, CCommonSettings.Unit);
					else
						fTmp += UnitUtils.ConvertToInternalUnits(m_nCillBearing, CCommonSettings.Unit);
				}
				else if(sl.Option == OpeningSelectOption.LintelAndSill && sl1.Option == OpeningSelectOption.LintelAndSill)
				{
					fTmp = sl.Points[0].DistanceTo(sl1.Points[0]);
					if (sl.Reserved >= 3)
						fTmp += UnitUtils.ConvertToInternalUnits(m_nLintelBearing, CCommonSettings.Unit);
					else
						fTmp += UnitUtils.ConvertToInternalUnits(m_nCillBearing, CCommonSettings.Unit);
					if (sl1.Reserved >= 3)
						fTmp += UnitUtils.ConvertToInternalUnits(m_nLintelBearing, CCommonSettings.Unit);
					else
						fTmp += UnitUtils.ConvertToInternalUnits(m_nCillBearing, CCommonSettings.Unit);
				}
				else
				{
					/*(sl.Option == OpeningSelectOption.None && sl1.Option == OpeningSelectOption.None)
					|| (sl.Option == OpeningSelectOption.None && sl1.Option == OpeningSelectOption.CentreOfOpening)
					|| (sl.Option == OpeningSelectOption.None && sl1.Option == OpeningSelectOption.AroundCentreOfOpening)
					|| (sl.Option == OpeningSelectOption.None && sl1.Option == OpeningSelectOption.EqualDistanceBetweenOpenings)
					|| (sl.Option == OpeningSelectOption.CentreOfOpening && sl1.Option == OpeningSelectOption.None)
					|| (sl.Option == OpeningSelectOption.CentreOfOpening && sl1.Option == OpeningSelectOption.CentreOfOpening)
					|| (sl.Option == OpeningSelectOption.CentreOfOpening && sl1.Option == OpeningSelectOption.AroundCentreOfOpening)
					|| (sl.Option == OpeningSelectOption.CentreOfOpening && sl1.Option == OpeningSelectOption.EqualDistanceBetweenOpenings)
					|| (sl.Option == OpeningSelectOption.AroundCentreOfOpening && sl1.Option == OpeningSelectOption.None)
					|| (sl.Option == OpeningSelectOption.AroundCentreOfOpening && sl1.Option == OpeningSelectOption.CentreOfOpening)
					|| (sl.Option == OpeningSelectOption.AroundCentreOfOpening && sl1.Option == OpeningSelectOption.AroundCentreOfOpening)
					|| (sl.Option == OpeningSelectOption.AroundCentreOfOpening && sl1.Option == OpeningSelectOption.EqualDistanceBetweenOpenings)
					|| (sl.Option == OpeningSelectOption.EqualDistanceBetweenOpenings && sl1.Option == OpeningSelectOption.None)
					|| (sl.Option == OpeningSelectOption.EqualDistanceBetweenOpenings && sl1.Option == OpeningSelectOption.CentreOfOpening)
					|| (sl.Option == OpeningSelectOption.EqualDistanceBetweenOpenings && sl1.Option == OpeningSelectOption.AroundCentreOfOpening)
					|| (sl.Option == OpeningSelectOption.EqualDistanceBetweenOpenings && sl1.Option == OpeningSelectOption.EqualDistanceBetweenOpenings)*/
					fTmp = sl.Points[0].DistanceTo(sl1.Points[0]);
				}

				if (fTmp > fWidth)
				{
					fWidth = fTmp;
				}
			}
			m_nStandardSplitWidth = Util.RoundToRealWorld(UnitUtils.ConvertFromInternalUnits(fWidth, CCommonSettings.Unit));
		}

		public void GetDetailedInformationOfOpening(COpening opening)
		{
			//				opening.Location = SketchPlane.ProjectOnto((f.Location as LocationPoint).Point);
			Document doc = m_Splitter.GetDocument();
			Util.GetDimensionOfOpening(doc, opening);

			if (opening.Points.Length != 4)
			{
				LogManager.Write(LogManager.WarningLevel.Warning, "CSplitElement::GetDetailedInformationOfOpening", "Rectangular Openings are only supported yet.");
				return;
			}

			//	Determine two top points and two bottom points
			XYZ ptTopLeft = null, ptTopRight = null, ptBottomLeft = null, ptBottomRight = null;
			double minDist = -1;
			Curve lc = LocationCurve.Clone();
			lc.MakeUnbound();
			for (int j = 0; j < opening.Points.Length; j++)
			{
				if (minDist == -1 || minDist > lc.Distance(opening.Points[j]))
				{
					minDist = lc.Distance(opening.Points[j]);
				}
			}
			for (int j = 0; j < opening.Points.Length; j++)
			{
				if (Util.IsEqual(lc.Distance(opening.Points[j]), minDist))
				{
					if (ptBottomLeft == null)
						ptBottomLeft = opening.Points[j];
					else
						ptBottomRight = opening.Points[j];
				}
				else
				{
					if (ptTopLeft == null)
						ptTopLeft = opening.Points[j];
					else
						ptTopRight = opening.Points[j];
				}
			}
			if (ptTopLeft == null || ptTopRight == null || ptBottomLeft == null || ptBottomRight == null)
			{
				LogManager.Write(LogManager.WarningLevel.Warning, "CSplitElement::GetDetailedInformationOfOpening", "Rotated openings are not supported yet.");
				return;
			}
			if (!Util.IsEqual((ptTopRight - ptTopLeft).Normalize(), (LocationCurve.GetEndPoint(1) - LocationCurve.GetEndPoint(0)).Normalize()))
			{
				XYZ ptTmp = ptTopRight;
				ptTopRight = ptTopLeft;
				ptTopLeft = ptTmp;
			}
			if (!Util.IsEqual((ptBottomRight - ptBottomLeft).Normalize(), (LocationCurve.GetEndPoint(1) - LocationCurve.GetEndPoint(0)).Normalize()))
			{
				XYZ ptTmp = ptBottomRight;
				ptBottomRight = ptBottomLeft;
				ptBottomLeft = ptTmp;
			}

			//	Project points to SketchPlane
			ptTopLeft = SketchPlane.ProjectOnto(ptTopLeft);
			ptTopRight = SketchPlane.ProjectOnto(ptTopRight);
			ptBottomLeft = SketchPlane.ProjectOnto(ptBottomLeft);
			ptBottomRight = SketchPlane.ProjectOnto(ptBottomRight);

			opening.Width = (ptTopRight - ptTopLeft).GetLength();  //	For LC option, this should be the width of the opening
			opening.Height = (ptBottomRight - ptTopRight).GetLength();
			opening.Points[0] = ptTopLeft;
			opening.Points[1] = ptTopRight;
			opening.Points[2] = ptBottomRight;
			opening.Points[3] = ptBottomLeft;
			opening.Location = (ptTopLeft + ptBottomRight) / 2;
			opening.ProjectedLocation = lc.Project(opening.Location).XYZPoint;

			//	Check if this Location is on the bottom line of the opening
			/*bool bLocationOnBottom = false;
			BoundingBoxXYZ bbxyz = f.get_BoundingBox(doc.ActiveView);
			XYZ centerPt = SketchPlane.ProjectOnto((bbxyz.Max + bbxyz.Min) / 2);
			if (!Util.IsEqual(centerPt, opening.Location))
			{
				bLocationOnBottom = true;
			}
			if (bLocationOnBottom)
				opening.Location += PV * fHeight / 2;*/

			//	Get Points
			/*opening.Points = new XYZ[4];
			XYZ normalVector = (LocationCurve.GetEndPoint(1) - LocationCurve.GetEndPoint(0)).Normalize();
			opening.Points[0] = opening.Location - PV * fHeight / 2 - normalVector * fWidth / 2;
			opening.Points[1] = opening.Location - PV * fHeight / 2 + normalVector * fWidth / 2;
			opening.Points[2] = opening.Location + PV * fHeight / 2 + normalVector * fWidth / 2;
			opening.Points[3] = opening.Location + PV * fHeight / 2 - normalVector * fWidth / 2;*/
			opening.ComputeArea();
		}
	}
}
