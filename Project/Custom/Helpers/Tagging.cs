using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Architexor.Utils;
using View = Autodesk.Revit.DB.View;

namespace Architexor.Custom.Helpers
{
	public class ViewSpecificCache
	{
		private View mView;  // Source View
		private XYZ mOrigin; // View Origin
		private XYZ mViewUp; // View Up diretion
		private XYZ mViewRight; // View Right direction
		private XY[][] mCropBoundaries = null;
		private double m_dCurveTol = 0;
		public readonly double CELL_SIZE = 0.5;//1.5; // division divider spacing in Internal Unit -- Feet.
		public BoundingBoxXYZ mBoundingBox = null;
		public bool[][] mGrid = null;

		public ViewSpecificCache(View view)
		{
			Document doc = view.Document;

			mView = view;
			mOrigin = mView.Origin;
			mViewUp = mView.UpDirection.Normalize();
			mViewRight = mView.RightDirection.Normalize();
			m_dCurveTol = doc.Application.ShortCurveTolerance;
		}

		public void Initialize()
		{
			//  Get Crop region areas
			if (mView.CropBoxActive)
			{
				SetCropRegionBoundary2D();
			}
		}

		public void InitCacheToAvoidOverlap(List<string> considerCategories)
		{
			Document doc = mView.Document;
			//	Get all Elements
			List<Element> elements = new List<Element>();
			if (considerCategories.Contains("Walls"))
			{
				List<Element> cat_elems = new FilteredElementCollector(doc, mView.Id)
					.OfClass(typeof(Wall))
					.WhereElementIsNotElementType()
					.ToList();
				elements.AddRange(cat_elems);
			}
			if (considerCategories.Contains("Tags"))
			{
				List<Element> cat_elems = new FilteredElementCollector(doc, mView.Id)
					.OfClass(typeof(IndependentTag))
					.WhereElementIsNotElementType()
					.ToList();
				elements.AddRange(cat_elems);

				cat_elems = new FilteredElementCollector(doc)
					.OfClass(typeof(SpatialElementTag))
					.WhereElementIsNotElementType()
					.ToList();
				elements.AddRange(cat_elems);
			}
			if (considerCategories.Contains("FamilyInstances"))
			{
				List<Element> cat_elems = new FilteredElementCollector(doc, mView.Id)
					.OfClass(typeof(FamilyInstance))
					.WhereElementIsNotElementType()
					.ToList();
				elements.AddRange(cat_elems);
			}
			if (considerCategories.Contains("Stairs"))
			{
				List<Element> cat_elems = new FilteredElementCollector(doc, mView.Id)
					.OfClass(typeof(Stairs))
					.WhereElementIsNotElementType()
					.ToList();
				elements.AddRange(cat_elems);
			}
			if (considerCategories.Contains("Floors"))
			{
				List<Element> cat_elems = new FilteredElementCollector(doc, mView.Id)
					.OfClass(typeof(Floor))
					.WhereElementIsNotElementType()
					.ToList();
				elements.AddRange(cat_elems);
			}

			//	Get the global bounding box for the elements
			BoundingBoxXYZ gBB = null;
			if (elements.Count > 0)
				gBB = elements[0].get_BoundingBox(mView);
			foreach(Element elem in elements)
			{
				BoundingBoxXYZ bb = elem.get_BoundingBox(mView);
				if (bb != null)
				{
					gBB.ExpandToContain(bb);
				}
			}
			mBoundingBox = gBB;

			double Z = mView.GenLevel.Elevation;
			XYZ minPt = new XYZ(gBB.Min.X, gBB.Min.Y, Z), maxPt = new XYZ(gBB.Max.X, gBB.Max.Y, Z);
			double step = CELL_SIZE;
			int nGridMaxX = (int)Math.Ceiling((gBB.Max.X - gBB.Min.X) / step);
			int nGridMaxY = (int)Math.Ceiling((gBB.Max.Y - gBB.Min.Y) / step);
			bool[][] grid = new bool[nGridMaxX][];
			for (int i = 0; i < nGridMaxX; i++)
			{
				grid[i] = new bool[nGridMaxY];
			}
			mGrid = grid;

			//	Get Geometrical Curves of all elements
			foreach (Element elem in elements)
			{
				try
				{
					//	Line or BoundingBox
					List<APIObject> geometries = Geometric.GetElementGeometryOnView(elem, mView);
					foreach(APIObject geometry in geometries)
					{
						ApplyGeometryToMap(geometry);
					}
				}
				catch (Exception ex)
				{

				}
			}
		}

		public void ApplyGeometryToMap(APIObject geometry)
		{
			XYZ minPt = new XYZ(mBoundingBox.Min.X, mBoundingBox.Min.Y, 0);

			if (geometry is Line l)
			{
				XYZ pt1 = l.GetEndPoint(0), pt2 = l.GetEndPoint(1);
				double x0 = pt1.X - minPt.X, x1 = pt2.X - minPt.X,
					y0 = pt1.Y - minPt.Y, y1 = pt2.Y - minPt.Y;
				int startX = x0 < x1 ? (int)Math.Floor(x0 / CELL_SIZE) : (int)Math.Floor(x1 / CELL_SIZE)
					, endX = x0 >= x1 ? (int)Math.Ceiling(x0 / CELL_SIZE) : (int)Math.Ceiling(x1 / CELL_SIZE)
					, startY = y0 < y1 ? (int)Math.Floor(y0 / CELL_SIZE) : (int)Math.Floor(y1 / CELL_SIZE)
					, endY = y0 >= y1 ? (int)Math.Ceiling(y0 / CELL_SIZE) : (int)Math.Ceiling(y1 / CELL_SIZE);
				for (int i = startX; i < endX; i++)
				{
					for (int j = startY; j < endY; j++)
					{
						if (mGrid[i][j])
							//	If already filled, don't check
							continue;

						x0 = minPt.X + i * CELL_SIZE; x1 = x0 + CELL_SIZE;
						y0 = minPt.Y + j * CELL_SIZE; y1 = y0 + CELL_SIZE;

						//	Check if the line (pt1.X, pt1.Y)-(pt2.X, pt2.Y) intersects one of the boundaries of the rectangle (x0, y0, x1, y1)
						//
						if (
							Intersects(pt1.X, pt1.Y, pt2.X, pt2.Y, x0, y0, x1, y0) || // bottom
							Intersects(pt1.X, pt1.Y, pt2.X, pt2.Y, x1, y0, x1, y1) || // right
							Intersects(pt1.X, pt1.Y, pt2.X, pt2.Y, x1, y1, x0, y1) || // top
							Intersects(pt1.X, pt1.Y, pt2.X, pt2.Y, x0, y1, x0, y0)    // left
						)
						{
							mGrid[i][j] = true;
						}
					}
				}
			}
			else if(geometry is BoundingBoxXYZ bb)
			{
				int startX = (int)Math.Floor((bb.Min.X - minPt.X) / CELL_SIZE)
					, endX = (int)Math.Ceiling((bb.Max.X - minPt.X) / CELL_SIZE)
					, startY = (int)Math.Floor((bb.Min.Y - minPt.Y) / CELL_SIZE)
					, endY = (int)Math.Ceiling((bb.Max.Y - minPt.Y) / CELL_SIZE);
				for (int i = startX; i < endX; i++) {
					for(int j = startY; j < endY; j++)
					{
						mGrid[i][j] = true;
					}
				}
			}
		}

		public XYZ FindIdealPoint(XYZ basePt, double width, double height, string mode, double maxOffset, List<Curve> curves, Document doc = null, View view = null)
		{
			int nXCnt = (int)Math.Ceiling(width / CELL_SIZE)
				, nYCnt = (int)Math.Ceiling(height / CELL_SIZE);

			int nX = (int)((basePt.X - mBoundingBox.Min.X) / CELL_SIZE)
				, nY = (int)((basePt.Y - mBoundingBox.Min.Y) / CELL_SIZE);

			int gridX = mGrid.Length;
			int gridY = mGrid[0].Length;

			if (mode == "Linear")
			{
				// Build the rectangle region based on curves[0] as centerline
				if (curves == null || curves.Count == 0)
					return basePt;

				List<double[]> rects = new List<double[]>();
				foreach (Curve c in curves)
				{
					XYZ p0 = c.GetEndPoint(0);
					XYZ p1 = c.GetEndPoint(1);
					XYZ dir = (p1 - p0).Normalize();
					XYZ perp = dir.CrossProduct(XYZ.BasisZ).Normalize(); // Perpendicular in XY plane
					double halfLen = c.Length / 2.0;
					double halfWidth = height / 2.0;

					// Rectangle corners in model coordinates
					XYZ center = (p0 + p1) / 2.0;
					XYZ rect0 = center - dir * halfLen - perp * maxOffset; //perp * halfWidth;
					XYZ rect1 = center + dir * halfLen + perp * maxOffset; //perp * halfWidth;

					// Rectangle bounds
					double xmin = Math.Min(rect0.X, rect1.X);
					double xmax = Math.Max(rect0.X, rect1.X);
					double ymin = Math.Min(rect0.Y, rect1.Y);
					double ymax = Math.Max(rect0.Y, rect1.Y);

					double[] rect = { xmin, ymin, xmax, ymax };
					rects.Add(rect);
				}

				//	Draw rect
				/*foreach (double[] rect in rects)
				{
					double z = curves[0].GetEndPoint(0).Z;
					Line line = Line.CreateBound(new XYZ(rect[0], rect[1], z), new XYZ(rect[2], rect[1], z));
					doc.Create.NewDetailCurve(view, line);
					line = Line.CreateBound(new XYZ(rect[2], rect[1], z), new XYZ(rect[2], rect[3], z));
					doc.Create.NewDetailCurve(view, line);
					line = Line.CreateBound(new XYZ(rect[2], rect[3], z), new XYZ(rect[0], rect[3], z));
					doc.Create.NewDetailCurve(view, line);
					line = Line.CreateBound(new XYZ(rect[0], rect[3], z), new XYZ(rect[0], rect[1], z));
					doc.Create.NewDetailCurve(view, line);
				}*/

				// Collect all candidate empty blocks inside the rectangle
				List<(int, int, double)> candidates = new List<(int, int, double)>();
				for (int i = 0; i + nXCnt <= gridX; i++)
				{
					for (int j = 0; j + nYCnt <= gridY; j++)
					{
						// Compute block center
						double x0 = mBoundingBox.Min.X + (i + nXCnt / 2.0) * CELL_SIZE;
						double y0 = mBoundingBox.Min.Y + (j + nYCnt / 2.0) * CELL_SIZE;
						double z0 = basePt.Z;

						bool inside = false;
						foreach (double[] rect in rects)
						{
							// Check if center is inside the rectangle
							if (!(x0 < rect[0] || x0 > rect[2] || y0 < rect[1] || y0 > rect[3]))
							{
								inside = true;
							}
							if (inside)
								break;
						}
						if (!inside)
							continue;

						// Check if block is empty
						bool blockEmpty = true;
						for (int x = 0; x < nXCnt && blockEmpty; x++)
						{
							for (int y = 0; y < nYCnt && blockEmpty; y++)
							{
								if (mGrid[i + x][j + y])
									blockEmpty = false;
							}
						}
						if (blockEmpty)
						{
							// Compute distance to basePt
							double dist2 = (x0 - basePt.X) * (x0 - basePt.X) + (y0 - basePt.Y) * (y0 - basePt.Y);
							candidates.Add((i, j, dist2));
						}
					}
				}
				if (candidates.Count == 0)
					return basePt;

				// Find the candidate with the smallest distance to basePt
				var best = candidates.OrderBy(c => c.Item3).First();
				double bestX = mBoundingBox.Min.X + (best.Item1 + nXCnt / 2.0) * CELL_SIZE;
				double bestY = mBoundingBox.Min.Y + (best.Item2 + nYCnt / 2.0) * CELL_SIZE;
				return new XYZ(bestX, bestY, basePt.Z);
			}
			else if(mode == "Polygonal")
			{
				//	Get the bounding box of the polygon
				BoundingBoxXYZ bb = new BoundingBoxXYZ();
				bb.Min = curves[0].GetEndPoint(0);
				bb.Max = bb.Min;
				List<XYZ> pts = new List<XYZ>();
				foreach (Curve c in curves)
				{
					bb.ExpandToContain(c.GetEndPoint(1));
					pts.Add(c.GetEndPoint(0));
				}
				
				// Collect all candidate empty blocks inside the rectangle
				List<(int, int, double)> candidates = new List<(int, int, double)>();
				for (int i = 0; i + nXCnt <= gridX; i++)
				{
					for (int j = 0; j + nYCnt <= gridY; j++)
					{
						// Compute block center
						double x0 = mBoundingBox.Min.X + (i + nXCnt / 2.0) * CELL_SIZE;
						double y0 = mBoundingBox.Min.Y + (j + nYCnt / 2.0) * CELL_SIZE;
						double z0 = basePt.Z;

						// Check if center is inside the rectangle
						if (x0 < bb.Min.X || x0 > bb.Max.X || y0 < bb.Min.Y || y0 > bb.Max.Y)
						{
							continue;
						}

						//	Check with polygon again
						if(!Geometric.IsPointInPolygon(new XYZ(x0, y0, z0), pts))
						{
							continue;
						}

						// Check if block is empty
						bool blockEmpty = true;
						for (int x = 0; x < nXCnt && blockEmpty; x++)
						{
							for (int y = 0; y < nYCnt && blockEmpty; y++)
							{
								if (mGrid[i + x][j + y])
									blockEmpty = false;
							}
						}
						if (blockEmpty)
						{
							// Compute distance to basePt
							double dist2 = (x0 - basePt.X) * (x0 - basePt.X) + (y0 - basePt.Y) * (y0 - basePt.Y);
							candidates.Add((i, j, dist2));
						}
					}
				}
				if (candidates.Count == 0)
					return basePt;

				// Find the candidate with the smallest distance to basePt
				var best = candidates.OrderBy(c => c.Item3).First();
				double bestX = mBoundingBox.Min.X + (best.Item1 + nXCnt / 2.0) * CELL_SIZE;
				double bestY = mBoundingBox.Min.Y + (best.Item2 + nYCnt / 2.0) * CELL_SIZE;
				return new XYZ(bestX, bestY, basePt.Z);
			}
			else
			{ //	Radial
				//	mGrid[i][j] == false means the cell is empty
				//	Find the nearest empty cell from (nX, nY) in mGrid.
				//	And the empty cells should be grouped by nXCnt, nYCnt

				// Step 1: Search outward from (nX, nY) in increasing Manhattan distance
				// Step 2: For each candidate top-left cell (i, j), check if the nXCnt x nYCnt block is empty
				// Step 3: If found, return the center of the block as XYZ
				// Step 4: If not found, continue searching outward until all grid is checked
				int maxRadius = Math.Max(gridX, gridY);
				for (int radius = 0; radius < maxRadius; radius++)
				{
					List<(int, int, double)> candidates = new List<(int, int, double)>();
					for (int dx = -radius; dx <= radius; dx++)
					{
						for (int dy = -radius; dy <= radius; dy++)
						{
							// Only check the perimeter of the current radius
							if (Math.Abs(dx) != radius && Math.Abs(dy) != radius)
								continue;

							int i = nX + dx;
							int j = nY + dy;

							// Check bounds for the block
							if (i < 0 || j < 0 || i + nXCnt > gridX || j + nYCnt > gridY)
								continue;

							bool blockEmpty = true;
							for (int x = 0; x < nXCnt && blockEmpty; x++)
							{
								for (int y = 0; y < nYCnt && blockEmpty; y++)
								{
									if (mGrid[i + x][j + y])
										blockEmpty = false;
								}
							}
							if (blockEmpty)
							{
								// Compute the center of the block in model coordinates
								double x0 = mBoundingBox.Min.X + (i + nXCnt / 2.0) * CELL_SIZE;
								double y0 = mBoundingBox.Min.Y + (j + nYCnt / 2.0) * CELL_SIZE;
								//double z0 = basePt.Z;
								//return new XYZ(x0, y0, z0);

								// Compute distance to basePt
								double dist2 = (x0 - basePt.X) * (x0 - basePt.X) + (y0 - basePt.Y) * (y0 - basePt.Y);
								candidates.Add((i, j, dist2));
							}
						}
					}

					if (candidates.Count > 0)
					{
						// Find the candidate with the smallest distance to basePt
						var best = candidates.OrderBy(c => c.Item3).First();
						if (best.Item3 > maxOffset)
						{
							return basePt;
						}

						double bestX = mBoundingBox.Min.X + (best.Item1 + nXCnt / 2.0) * CELL_SIZE;
						double bestY = mBoundingBox.Min.Y + (best.Item2 + nYCnt / 2.0) * CELL_SIZE;
						return new XYZ(bestX, bestY, basePt.Z);
					}
				}
				// If no empty block found, return the original basePt
				return basePt;
			}
		}

		// Rectangle boundaries: (x0, y0)-(x1, y1)
		// Four edges: (x0, y0)-(x1, y0), (x1, y0)-(x1, y1), (x1, y1)-(x0, y1), (x0, y1)-(x0, y0)
		private bool Intersects(double xA, double yA, double xB, double yB, double xC, double yC, double xD, double yD)
		{
			// Helper to check if two line segments (A-B and C-D) intersect
			double det = (xB - xA) * (yD - yC) - (yB - yA) * (xD - xC);
			if (Math.Abs(det) < 1e-9) return false; // Parallel
			double t = ((xC - xA) * (yD - yC) - (yC - yA) * (xD - xC)) / det;
			double u = ((xC - xA) * (yB - yA) - (yC - yA) * (xB - xA)) / det;
			return t >= 0 && t <= 1 && u >= 0 && u <= 1;
		}

		private void SetCropRegionBoundary2D()
		{
			ViewCropRegionShapeManager cropMgr = mView.GetCropRegionShapeManager();
#if DEBUG
			Debug.Assert(cropMgr != null);
#endif
			if (cropMgr == null)
				return;

			// Crop region can have multiple loops
			IList<CurveLoop> curveLoops = cropMgr.GetCropShape();
			if (curveLoops == null)
				return;

			XY[][] cropBoundaries = new XY[curveLoops.Count][];
			for(int i = 0; i < curveLoops.Count; i++)
			{
				CurveLoop loop = curveLoops[i];

				cropBoundaries[i] = new XY[loop.Count()];
				for(int j = 0; j < loop.Count(); j++)
				{
					Curve curve = loop.ElementAt(j);
					XYZ pt = curve.GetEndPoint(0);
					cropBoundaries[i][j] = new XY(pt.X, pt.Y);
				}
			}
			mCropBoundaries = cropBoundaries;
		}
	}

	public class XY
	{
		public XY(double x = 0, double y = 0)
		{
			X = x; Y = y;
		}
		public bool isZero { get { return X < Helpers.Geometric.TOL && Y < Helpers.Geometric.TOL; } }
		public double X { get; set; }
		public double Y { get; set; }
		public XY ScaledOne(double scale)
		{
			return new XY(X * scale, Y * scale);
		}
		public string HashCode()
		{
			int nRound = 6;
			return $"{Math.Round(X, nRound)}," + $"{Math.Round(Y, nRound)},";
		}
		public double distanceTo(XY other)
		{
			double dx = X - other.X;
			double dy = Y - other.Y;
			return Math.Sqrt(dx * dx + dy * dy);
		}
		public double Min
		{
			get { return Math.Min(X, Y); }
		}
	}

	public class Rect2D
	{
		private double m_x = 0;
		private double m_y = 0;

		public XY OriginPoint { get { return new XY(m_x, m_y); } }
		public XY MinPoint { get { return new XY(m_x, m_y); } }
		public XY MaxPoint { get { return new XY(m_x + Width, m_y + Height); } }
		public (double, double) Origin { get { return (m_x, m_y); } }
		public (double, double) Min { get { return (m_x, m_y); } }
		public (double, double) Max { get { return (m_x + Width, m_y + Height); } }
		public double Width { get; set; } = 0;
		public double Height { get; set; } = 0;
		public bool IsZero { get { return Width < Helpers.Geometric.TOL && Height < Helpers.Geometric.TOL; } }
		public double Xmin { get { return m_x; } }
		public double Xmax { get { return m_x + Width; } }
		public double Ymin { get { return m_y; } }
		public double Ymax { get { return m_x + Height; } }
		public Rect2D(double x, double y, double width, double height)
		{
			m_x = x;
			m_y = y;
			Width = width;
			Height = height;
		}
		public static Rect2D CreateFromBbox(BoundingBoxXYZ bbox, XYZ origin, XYZ right, XYZ up)
		{
			if (bbox == null) return null;

			var minpt = bbox.Min;
			var maxpt = bbox.Max;
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

			double xmin = double.MaxValue, xmax = double.MinValue, ymin = double.MaxValue, ymax = double.MinValue;
			foreach (var c in corners)
			{
				// Project c to the view plane defined by origin, right, up
				XYZ v = c - origin;
				double x = v.DotProduct(right);
				double y = v.DotProduct(up);
				if (x < xmin) xmin = x;
				if (x > xmax) xmax = x;
				if (y < ymin) ymin = y;
				if (y > ymax) ymax = y;
			}
			return new Rect2D(xmin, ymin, xmax - xmin, ymax - ymin);
		}
		public (int, int, int, int) GetGridKey(double gridSize)
		{
			int gx_min = (int)Math.Floor(Xmin / gridSize);
			int gx_max = (int)Math.Floor(Xmax / gridSize);
			int gy_min = (int)Math.Floor(Ymin / gridSize);
			int gy_max = (int)Math.Floor(Ymax / gridSize);
			return (gx_min, gx_max, gy_min, gy_max);
		}
		public bool IntersectsWith(Rect2D other)
		{
			// Two rectangles intersect if their projections on both axes overlap
			return !(Xmax < other.Xmin || Xmin > other.Xmax || Ymax < other.Ymin || Ymin > other.Ymax);
		}
		public bool IsPointInside(XY p)
		{
			return (p.X >= Xmin && p.X <= Xmax && p.Y >= Ymin && p.Y <= Ymax);
		}
		public List<(XY, XY)> GetEdges()
		{
			return new List<(XY, XY)>
			{
				(new XY(Xmin, Ymin), new XY(Xmax, Ymin)),
				(new XY(Xmax, Ymin), new XY(Xmax, Ymax)),
				(new XY(Xmax, Ymax), new XY(Xmin, Ymax)),
				(new XY(Xmin, Ymax), new XY(Xmin, Ymin))
			};
		}
	}

	public class Segment2D
	{
		double m_xStart = 0;
		double m_yStart = 0;
		double m_xEnd = 0;
		double m_yEnd = 0;
		Rect2D m_rect = null;
		public Rect2D Rect { get { return m_rect; } }
		public double SizeX { get { return Math.Abs(m_xEnd - m_xStart); } }
		public double SizeY { get { return Math.Abs(m_yEnd - m_yStart); } }
		public double StartX { get { return m_xStart; } }
		public double EndX { get { return m_xEnd; } }
		public double StartY { get { return m_yStart; } }
		public double EndY { get { return m_yEnd; } }
		public XY StartPoint { get { return new XY(m_xStart, m_yStart); } }
		public XY EndPoint { get { return new XY(m_xEnd, m_yEnd); } }

		public Segment2D(XY start, XY end)
		{
			m_xStart = start.X;
			m_yStart = start.Y;
			m_xEnd = end.X;
			m_yEnd = end.Y;
			m_rect = new Rect2D(Math.Min(m_xStart, m_xEnd), Math.Min(m_yStart, m_yEnd), Math.Abs(m_xEnd - m_xStart), Math.Abs(m_yEnd - m_yStart));
		}
		public Segment2D(double x0, double y0, double x1, double y1)
		{
			m_xStart = x0;
			m_yStart = y0;
			m_xEnd = x1;
			m_yEnd = y1;
			m_rect = new Rect2D(Math.Min(m_xStart, m_xEnd), Math.Min(m_yStart, m_yEnd), Math.Abs(m_xEnd - m_xStart), Math.Abs(m_yEnd - m_yStart));
		}
		public bool HitsRect(Rect2D rect)
		{
			// Use Helpers.Geometric.SegmentHitsRect to check if this segment hits the rectangle
			var x1 = m_xStart;
			var y1 = m_yStart;
			var x2 = m_xEnd;
			var y2 = m_yEnd;
			var segRect = m_rect;
			if (!segRect.IntersectsWith(rect))
				return false;

			double xmin = rect.Xmin, ymin = rect.Ymin, xmax = rect.Xmax, ymax = rect.Ymax;
			// quick reject
			if ((x1 < xmin && x2 < xmin) || (x1 > xmax && x2 > xmax) ||
				(y1 < ymin && y2 < ymin) || (y1 > ymax && y2 > ymax))
				return false;
			// endpoint inside?
			if ((xmin <= x1 && x1 <= xmax && ymin <= y1 && y1 <= ymax) ||
				(xmin <= x2 && x2 <= xmax && ymin <= y2 && y2 <= ymax))
				return true;

			// test intersection with each rect edge
			bool IntersectsVert(double xe)
			{
				if (x2 == x1) return false;
				double t = (xe - x1) / (x2 - x1);
				if (t >= 0.0 && t <= 1.0)
				{
					double y = y1 + t * (y2 - y1);
					return ymin <= y && y <= ymax;
				}
				return false;
			}
			bool IntersectsHoriz(double ye)
			{
				if (y2 == y1) return false;
				double t = (ye - y1) / (y2 - y1);
				if (t >= 0.0 && t <= 1.0)
				{
					double x = x1 + t * (x2 - x1);
					return xmin <= x && x <= xmax;
				}
				return false;
			}
			return IntersectsVert(xmin) || IntersectsVert(xmax) ||
					 IntersectsHoriz(ymin) || IntersectsHoriz(ymax);
		}

		public string HashCode()
		{
			int nRound = 6;
			string start_str =
				$"{Math.Round(m_xStart, nRound)}," +
				$"{Math.Round(m_yStart, nRound)}," +
				$"{Math.Round(m_xEnd, nRound)}," +
				$"{Math.Round(m_yEnd, nRound)},";
			return start_str;
		}

		public bool IntersectsWithElement(View view, Element elem)
		{
			if (view == null || elem == null)
				return false;

			var elem_bbox = Helpers.Geometric.SafeWorldBoundingBox(elem, view);
			var view_origin = view.Origin;
			var view_right = view.RightDirection;
			var view_up = view.UpDirection;
			Rect2D elem_rect = Rect2D.CreateFromBbox(elem_bbox, view_origin, view_right, view_up);

			// Check if either point is inside the rectangle
			if(elem_rect.IsPointInside(StartPoint) || elem_rect.IsPointInside(EndPoint))
			{
				return true;
			}

			// Define rectangle edges as line segments
			var rectEdges = elem_rect.GetEdges();
			foreach (var edge in rectEdges)
			{
				if (this.IntersetsWith(new Segment2D(edge.Item1, edge.Item2)))
					return true;
			}
			return false;
		}
		public bool IntersetsWith(Segment2D other)
		{
			return (Orientation(other.StartPoint) != Orientation(other.EndPoint)) &&
				(other.Orientation(this.StartPoint) != other.Orientation(this.EndPoint));
		}
		/// <summary>
		/// Get next to p  vector's orientation
		/// </summary>
		/// <param name="p"></param>
		/// <returns>0: Collinear, 1: CW, 2: CCW</returns>
		private int Orientation(XY p)
		{
			double val = (m_yEnd - m_yStart) * (p.X - m_xEnd) - (m_xEnd - m_xStart) * (p.Y - m_yEnd);
			if (Math.Abs(val) < Helpers.Geometric.TOL) return 0; // collinear
			return (val > 0) ? 1 : 2; // 1 = CW, 2 = CCW
		}
	}

	public enum TagEndType
	{
		Auto,
		AttachedEnd,
		FreeEnd
	}

	public enum TagHeadingDirection
	{
		Auto,
		AtCenter,
		InsideRoom,
		FacingExterior,
		FacingInterior,
		Corner,
	}

	public struct TagHostingOption
	{
		public Category Category; // Category Name
		public double MaxOffset = 3; // paper size value in inches.
		public bool LeaderVisible = true;
		public TagEndType EndType = TagEndType.Auto;
		public TagHeadingDirection HeadingDirection = TagHeadingDirection.Auto;
		public TagOrientation Orientation = TagOrientation.AnyModelDirection;
		public string TagTypeName = "";

		//	Reserved for walls
		public double MinWallLength = 0;
		public List<WallFunction> WallFunctions = new List<WallFunction>();

		public TagHostingOption()
		{
		}

		public void Refine()
		{
			if (EndType == TagEndType.Auto)
				RefineEndType();
			if (HeadingDirection == TagHeadingDirection.Auto)
				RefineHeadingDirection();
			if (Orientation == TagOrientation.AnyModelDirection)
				RefineOrientation();
		}

		private void RefineEndType()
		{
			EndType = TagEndType.AttachedEnd;
			// Update the Auto Values depending on the target category
			switch (Category.BuiltInCategory)
			{
				case BuiltInCategory.OST_Areas:
					break;
				case BuiltInCategory.OST_Columns:
					EndType = TagEndType.AttachedEnd;
					break;
				case BuiltInCategory.OST_Doors:
					EndType = TagEndType.AttachedEnd;
					break;
				case BuiltInCategory.OST_GenericModel:
					EndType = TagEndType.FreeEnd;
					break;
				case BuiltInCategory.OST_Rooms:
					break;
				case BuiltInCategory.OST_Walls:
					break;
				case BuiltInCategory.OST_Windows:
					break;
				default:
					// Category cannot be tagged
					break;
			}
		}

		private void RefineHeadingDirection()
		{
			HeadingDirection = TagHeadingDirection.Corner;
			// Update the Auto Values depending on the target category
			switch (Category.BuiltInCategory)
			{
				case BuiltInCategory.OST_Areas:
					break;
				case BuiltInCategory.OST_Columns:
					HeadingDirection = TagHeadingDirection.Corner;
					break;
				case BuiltInCategory.OST_Doors:
					HeadingDirection = TagHeadingDirection.AtCenter;
					break;
				case BuiltInCategory.OST_GenericModel:
					HeadingDirection = TagHeadingDirection.AtCenter;
					break;
				case BuiltInCategory.OST_Rooms:
					break;
				case BuiltInCategory.OST_Walls:
					break;
				case BuiltInCategory.OST_Windows:
					break;
				default:
					// Category cannot be tagged
					break;
			}
		}

		private void RefineOrientation()
		{
			Orientation = TagOrientation.Horizontal;
			// Update the Auto Values depending on the target category
			switch (Category.BuiltInCategory)
			{
				case BuiltInCategory.OST_Areas:
					break;
				case BuiltInCategory.OST_Columns:
					Orientation = TagOrientation.AnyModelDirection;
					break;
				case BuiltInCategory.OST_Doors:
					Orientation = TagOrientation.AnyModelDirection;
					break;
				case BuiltInCategory.OST_GenericModel:
					break;
				case BuiltInCategory.OST_Rooms:
					break;
				case BuiltInCategory.OST_Walls:
					Orientation = TagOrientation.AnyModelDirection;
					break;
				case BuiltInCategory.OST_Windows:
					Orientation = TagOrientation.AnyModelDirection;
					break;
				default:
					// Category cannot be tagged
#if DEBUG
					Debug.Assert(false);
#endif
					break;
			}
		}

		public void SetEndType(string value)
		{
			switch (value)
			{
				case "Attached End":
					EndType = TagEndType.AttachedEnd;
					break;
				case "Free End":
					EndType = TagEndType.FreeEnd;
					break;
				default:
					EndType = TagEndType.Auto;
					break;
			}
		}

		public void SetHeadingDirection(string value)
		{
			switch (value)
			{
				case "At Center":
					HeadingDirection = TagHeadingDirection.AtCenter;
					break;
				case "Inside Room":
					HeadingDirection = TagHeadingDirection.InsideRoom;
					break;
				case "Facing Exterior":
					HeadingDirection = TagHeadingDirection.FacingExterior;
					break;
				case "Facing Interior":
					HeadingDirection = TagHeadingDirection.FacingInterior;
					break;
				default:
					HeadingDirection = TagHeadingDirection.Auto;
					break;
			}
		}

		public void SetOrientation(string value)
		{
			switch (value)
			{
				case "Horizontal":
					Orientation = TagOrientation.Horizontal;
					break;
				case "Vertical":
					Orientation = TagOrientation.Vertical;
					break;
				default:
					Orientation = TagOrientation.AnyModelDirection;
					break;
			}
		}
	}

	internal static class Tagging
	{
		// Map from element BuiltInCategory to its tag BuiltInCategory
		public static readonly Dictionary<BuiltInCategory, BuiltInCategory> element2tagMap = new Dictionary<BuiltInCategory, BuiltInCategory>
		{
			{ BuiltInCategory.OST_Walls, BuiltInCategory.OST_WallTags },
			{ BuiltInCategory.OST_Doors, BuiltInCategory.OST_DoorTags },
			{ BuiltInCategory.OST_Windows, BuiltInCategory.OST_WindowTags },
			{ BuiltInCategory.OST_Rooms, BuiltInCategory.OST_RoomTags },
			{ BuiltInCategory.OST_Furniture, BuiltInCategory.OST_FurnitureTags },
			{ BuiltInCategory.OST_FurnitureSystems, BuiltInCategory.OST_FurnitureSystemTags },
			{ BuiltInCategory.OST_Casework, BuiltInCategory.OST_CaseworkTags },
			{ BuiltInCategory.OST_SpecialityEquipment, BuiltInCategory.OST_SpecialityEquipmentTags },
			{ BuiltInCategory.OST_Ceilings, BuiltInCategory.OST_CeilingTags },
			{ BuiltInCategory.OST_Floors, BuiltInCategory.OST_FloorTags },
			{ BuiltInCategory.OST_GenericModel, BuiltInCategory.OST_GenericModelTags },
			{ BuiltInCategory.OST_PlumbingFixtures, BuiltInCategory.OST_PlumbingFixtureTags },
			{ BuiltInCategory.OST_MechanicalEquipment, BuiltInCategory.OST_MechanicalEquipmentTags },
			{ BuiltInCategory.OST_ElectricalEquipment, BuiltInCategory.OST_ElectricalEquipmentTags },
			{ BuiltInCategory.OST_ElectricalFixtures, BuiltInCategory.OST_ElectricalFixtureTags },
			{ BuiltInCategory.OST_LightingFixtures, BuiltInCategory.OST_LightingFixtureTags },
			{ BuiltInCategory.OST_Columns, BuiltInCategory.OST_ColumnTags },
			{ BuiltInCategory.OST_StructuralFraming, BuiltInCategory.OST_StructuralFramingTags },
			{ BuiltInCategory.OST_StructuralFoundation, BuiltInCategory.OST_StructuralFoundationTags },
			{ BuiltInCategory.OST_Areas, BuiltInCategory.OST_AreaTags },
			{ BuiltInCategory.OST_StructuralColumns, BuiltInCategory.OST_StructuralColumnTags },
			{ BuiltInCategory.OST_Roofs, BuiltInCategory.OST_RoofTags },
			{ BuiltInCategory.OST_CurtainWallPanels, BuiltInCategory.OST_CurtainWallPanelTags },
			{ BuiltInCategory.OST_CurtainWallMullions, BuiltInCategory.OST_CurtainWallMullionTags },
			{ BuiltInCategory.OST_PipeFitting, BuiltInCategory.OST_PipeFittingTags },
			{ BuiltInCategory.OST_DuctFitting, BuiltInCategory.OST_DuctFittingTags },
			{ BuiltInCategory.OST_PipeAccessory, BuiltInCategory.OST_PipeAccessoryTags },
			{ BuiltInCategory.OST_DuctAccessory, BuiltInCategory.OST_DuctAccessoryTags },
			{ BuiltInCategory.OST_Sprinklers, BuiltInCategory.OST_SprinklerTags },
			{ BuiltInCategory.OST_LightingDevices, BuiltInCategory.OST_LightingDeviceTags },
			{ BuiltInCategory.OST_FireAlarmDevices, BuiltInCategory.OST_FireAlarmDeviceTags },
			{ BuiltInCategory.OST_DataDevices, BuiltInCategory.OST_DataDeviceTags },
			{ BuiltInCategory.OST_CommunicationDevices, BuiltInCategory.OST_CommunicationDeviceTags },
			{ BuiltInCategory.OST_SecurityDevices, BuiltInCategory.OST_SecurityDeviceTags },
			{ BuiltInCategory.OST_DuctTerminal, BuiltInCategory.OST_DuctTerminalTags },
			{ BuiltInCategory.OST_Stairs, BuiltInCategory.OST_StairsTags },
			{ BuiltInCategory.OST_Ramps, BuiltInCategory.OST_RampTags },
			{ BuiltInCategory.OST_StructConnections, BuiltInCategory.OST_StructConnectionTags }
		};

		public static bool IsSpatialTag(BuiltInCategory tagCategory)
		{
			return (tagCategory == BuiltInCategory.OST_RoomTags
				|| tagCategory == BuiltInCategory.OST_AreaTags
				|| tagCategory == BuiltInCategory.OST_MEPSpaceTags);
		}

		public static List<Element> GetTargetElements(Document doc, View view, TagHostingOption option)
		{
			BuiltInCategory targetCategory = option.Category.BuiltInCategory;

			List<Element> filteredElements = new List<Element>();
			
			FilteredElementCollector collector = new FilteredElementCollector(doc, view.Id)
					.WhereElementIsNotElementType()
					.OfCategory(targetCategory);

			if (collector.GetElementCount() == 0)
				return filteredElements;

			// - Check if it's fully inside the view crop region.
			List<List<(double, double)>> cropRegions = Geometric.ViewCropRegionBoundary2D(view);
#if DEBUG
			//Geometric.DebugViewPolygonVisualization(doc, view);
#endif

			// check if the element has location information			
			foreach (Element el in collector)
			{
				// Get location
				Location location = el.Location;
//				if (location == null)
//					continue;

				XYZ elPoint = null;

				if (location is LocationPoint locPoint)
				{
					elPoint = locPoint.Point;
				}
				else if (location is LocationCurve locCurve)
				{
					elPoint = locCurve.Curve.Evaluate(0.5, true);
				}

				//if (elPoint == null)
					//continue;

				bool isCut = false;
				foreach (var cropRegion in cropRegions)
				{
					int nCount = cropRegion.Count;
					for (int i = 0; i < nCount; i++)
					{
						var p = cropRegion[i];
						var q = cropRegion[(i + 1) % nCount];
						Segment2D seg = new Segment2D(p.Item1, p.Item2, q.Item1, q.Item2);
						if (seg.IntersectsWithElement(view, el))
						{
							isCut = true;
							break;
						}
					}
					if (isCut)
						break;
				}
				if (isCut)
				{
#if DEBUG
					var bbox = Geometric.SafeWorldBoundingBox(el, view);
					//Geometric.DebugBoundingBoxVisualization(doc, view, bbox);
#endif
					switch (targetCategory)
					{
						case BuiltInCategory.OST_Walls:
						case BuiltInCategory.OST_Rooms:
							// Include the intersected elements for these categories.
							break;
						default:
							// Exclude the intersected elements for common categories.
							continue;
							break;
					}
				}

				//	Filter walls by condition
				if(el is Wall wall)
				{
					//	Filter by wall length and function
					WallType type = doc.GetElement(wall.GetTypeId()) as WallType;
					WallFunction func = (WallFunction)type.get_Parameter(BuiltInParameter.FUNCTION_PARAM).AsInteger();
					if (!option.WallFunctions.Contains(func))
					{
						continue;
					}

					//	We don't support Curtain Wall for now.
					if (type.Kind == WallKind.Curtain)
					{
						continue;
					}

					double fLength = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsDouble();
					if (option.MinWallLength > 0 && fLength < option.MinWallLength)
					{
						continue;
					}
				}

				filteredElements.Add(el);
			}

			return filteredElements;
		}

		public static HashSet<ElementId> GetAlreadyTagged(Document doc, ElementId viewId, BuiltInCategory tagCategory)
		{
			HashSet<ElementId> tagged = new HashSet<ElementId>();

			FilteredElementCollector tagcollector = new FilteredElementCollector(doc, viewId)
				.WhereElementIsNotElementType()
				.OfCategory(tagCategory);

			foreach (Element tag in tagcollector)
			{
				if (tag is IndependentTag independentTag)
				{
					IList<Reference> taggedRefs = independentTag.GetTaggedReferences();
					if (taggedRefs.Count > 0)
					{
						foreach (Reference reference in taggedRefs)
						{
							ElementId id = reference.ElementId;
							if (id != ElementId.InvalidElementId)
								tagged.Add(reference.ElementId);
						}
					}
				}
				else if (tag is SpatialElementTag spatialTag)
				{
					ElementId id = null;
					if (tag is RoomTag roomTag)
					{
						id = roomTag.Room.Id;
					}
					else if (tag is AreaTag areaTag)
					{
						id = areaTag.Area.Id;
					}
					else if (tag is SpaceTag spaceTag)
					{
						id = spaceTag.Space.Id;
					}
					if (id != ElementId.InvalidElementId)
					{
						tagged.Add(id);
					}
				}
			}
			return tagged;
		}

		/// <summary>
		/// Groups a list of elements by a given key selector function.
		/// </summary>
		/// <typeparam name="TKey">The type of the key to group by.</typeparam>
		/// <param name="elements">The list of elements to group.</param>
		/// <param name="keySelector">A function to extract the grouping key from each element.</param>
		/// <returns>A dictionary where each key is associated with a list of elements.</returns>
		public static List<List<Element>> GroupElementsForMultiHosting(Document doc, ElementId viewId, BuiltInCategory category, IEnumerable<Element> elements)
		{
			switch (category)
			{
				case BuiltInCategory.OST_Areas:
				case BuiltInCategory.OST_Rooms:
				case BuiltInCategory.OST_MEPSpaces:
				case BuiltInCategory.OST_Doors:
				case BuiltInCategory.OST_Furniture:
					return null;
				case BuiltInCategory.OST_Columns:
					// When the multiple columns that have the same types are tagged and they are in the same area, not far away from each other,
					// they should be tagged with the same tag.
					// need to define the tagging logic for the columns - direction, leader merge, orientation, offset, etc
					break;
				case BuiltInCategory.OST_Walls:
					// When the multiple walls that have the same types are tagged and they are in the same area, not far away from each other,
					// Commonly when they are located in the same area, intersecting each other, the corner-style multi-tag should be applied.
					// need to define the tagging logic for the walls - direction, leader merge, orientation, offset, etc
					break;
				case BuiltInCategory.OST_Windows:
					// When the multiple widnows that have the same types are tagged and they are in the same area, not far away from each other,
					// Commonly when they are hosted in the same wall or the same side of the building, the same tag should be applied.
					// need to define the tagging logic for the windows - direction, leader merge, orientation, offset, etc
					break;
				case BuiltInCategory.OST_Planting:
					// When the multiple planting elements that have the same types are tagged and they are in the same area, not far away from each other,
					// Commonly when they are located in the same area, intersecting each other, the corner-style multi-tag should be applied.
					// need to define the tagging logic for the planting - direction, leader merge, orientation, offset, etc

					// TODO: Searching the same elements in view with a given distance threshold.
					// 1. Get the same type of elements in the same room.
					// 2. Consider the distance threshold to group the elements.
					break;
				default:
					// For other categories, we can return null or handle them as needed.
					return null;
			}

			// Group elements by their type and location
			// This is a placeholder for the actual grouping logic.
			var groupedElements = new List<List<Element>>();

			// iterate through the elemnts and group them based on their type and location
			foreach (Element element in elements)
			{
				// Find or create a group for this element's type
				var group = groupedElements.FirstOrDefault(g => g.Count > 0 && g[0].GetType() == element.GetType());
				if (group == null)
				{
					group = new List<Element>();
					groupedElements.Add(group);
				}
				group.Add(element);
			}

			// Refine the groupedElements based on the placement area and the distance threshold
			// iterate though the grouped elements and refine them based on the placement area and the distance threshold
			foreach (var group in groupedElements)
			{
				// check the located room or area id
				ElementId roomId = ElementId.InvalidElementId;
				if (group.Count > 0)
				{
					// Get the first element's room or area id
					if (group[0] is SpatialElement spatialElement)
					{
						//roomId = spatialElement.Room?.Id ?? spatialElement.Area?.Id ?? ElementId.InvalidElementId;
					}
				}
				// get the room or area in which the elements are located

			}

			return null;
		}

		public static List<FamilySymbol> GetTagTypes(Document doc, BuiltInCategory targetCategory)
		{
			string sCatName = targetCategory.ToString();
			if (sCatName.EndsWith("s"))
			{
				sCatName = sCatName.Substring(0, sCatName.Length - 1);
				sCatName += "Tags";
			}
			else
			{
				sCatName += "Tags";
			}

			//	Find corresponding tag category
			BuiltInCategory tagCategory = 0;
			foreach (Category category in doc.Settings.Categories)
			{
				if (category.BuiltInCategory.ToString() == sCatName)
				{
					tagCategory = category.BuiltInCategory;
				}
			}

			if (tagCategory == 0)
				return new List<FamilySymbol>();

			return new FilteredElementCollector(doc)
				.WhereElementIsElementType()
				.OfCategory(tagCategory)
				.Cast<FamilySymbol>()
				.ToList();
		}

		/// <summary>
		/// Get Family Symbol of the given Tag category
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="tagCategory"></param>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static FamilySymbol GetTagType(Document doc, BuiltInCategory tagCategory, string typeName = "")
		{
			FilteredElementCollector collector = new FilteredElementCollector(doc)
				.WhereElementIsElementType()
				.OfCategory(tagCategory);

			FamilySymbol fs = null;
			foreach (ElementType type in collector)
			{
				if (type is FamilySymbol symbol)
				{
					//if (typeName == "")
					//{
						fs = symbol;
					//}
					if (((Element)type).Name == typeName)
					{
						return symbol;
					}
				}
			}
			return fs;
		}

		/// <summary>
		/// Determines a sensible tag base point and applies an intelligent offset perpendicular to the element's facing/orientation vector.
		/// </summary>
		/// <param name="el">The element to tag.</param>
		/// <param name="view">The view context.</param>
		/// <param name="placeAtCenter">If true, place at center of curve; otherwise, at start.</param>
		/// <param name="offsetFt">Offset distance in feet.</param>
		/// <returns>Tuple of (XYZ basePoint, XYZ directionVector), or null if not found.</returns>
		public static (XYZ, XYZ) DetermineBasePoint(Element el, View view, bool placeAtCenter, double offsetFt, bool isSpatial)
		{
			// Consider the view-specific context - croping, orientation, etc
			// When the specific elements are cut with the viewcropregion, calculate the base point with the remaining parts of them.

			XYZ xyz = null;

			if (isSpatial && el.Location is LocationPoint spatialLoc && spatialLoc.Point != null)
			{
				xyz = spatialLoc.Point;
			}
			else if (el.Location != null)
			{
				if (el.Location is LocationPoint locPoint)
				{
					xyz = locPoint.Point;
				}
				else if (el.Location is LocationCurve locCurve)
				{
					double param = placeAtCenter ? 0.5 : 0.0;
					xyz = locCurve.Curve.Evaluate(param, true);
				}
			}
			else
			{
				BoundingBoxXYZ bb = Geometric.SafeWorldBoundingBox(el, view);
				if (bb != null)
				{
					xyz = new XYZ(
						(bb.Min.X + bb.Max.X) / 2.0,
						(bb.Min.Y + bb.Max.Y) / 2.0,
						(bb.Min.Z + bb.Max.Z) / 2.0
					);
				}
			}
			if (xyz == null)
				return (null, null);

			XYZ dirVec = Geometric.LogicalOffsetDirection(el);

			// Fallback if direction is not valid
			if (dirVec == null || dirVec.IsZeroLength())
				dirVec = XYZ.BasisY;

			// 3) Offset the base-point in the view-plane
			XYZ dirPlane = Geometric.FlattenToPlane(dirVec, view);
			XYZ dirPlaneU = Geometric.Unit(dirPlane);
			if (!isSpatial && dirPlaneU.GetLength() > Geometric.TOL)
			{
				xyz = Geometric.OffsetAlongPlane(xyz, dirPlaneU, offsetFt);
			}

			return (xyz, dirVec);
		}

		// Helper to get view axes (right and up) for a view
		private static void GetViewAxes(View view, out XYZ right, out XYZ up)
		{
			// For most views, UpDirection and RightDirection are available
			// For 3D views, fallback to default axes if not available
			right = XYZ.BasisX;
			up = XYZ.BasisY;
			var upProp = view.GetType().GetProperty("UpDirection");
			var rightProp = view.GetType().GetProperty("RightDirection");
			if (upProp != null && rightProp != null)
			{
				var upVal = upProp.GetValue(view);
				var rightVal = rightProp.GetValue(view);
				if (upVal is XYZ upXYZ) up = upXYZ;
				if (rightVal is XYZ rightXYZ) right = rightXYZ;
			}
		}

		public static List<Curve> GetWallValidCurves(Document doc, View view, Wall wall, double minSize = 0)
		{
			List<Curve> wallCurves = new List<Curve>();

			// Get hosted openings
			// Get all hosted openings (doors, windows, generic openings) in the wall
			List<Element> hostedOpenings = new List<Element>();
			ICollection<ElementId> hostedIds = wall.FindInserts(true, true, true, true);
			foreach (ElementId eid in hostedIds)
			{
				Element opening = wall.Document.GetElement(eid);
				if (opening != null)
					hostedOpenings.Add(opening);
			}

			if (wall.Location is LocationCurve locCurve)
			{
				Curve wallCurve = locCurve.Curve;
				double wallStart = wallCurve.GetEndParameter(0);
				double wallEnd = wallCurve.GetEndParameter(1);

				// Get the opening segments as (start, end) along the wall curve
				List<(double, double)> openingSegments = new List<(double, double)>();
				foreach (Element opening in hostedOpenings)
				{
					LocationPoint openingLoc = opening.Location as LocationPoint;
					if (openingLoc == null)
						continue;

					XYZ openingPoint = openingLoc.Point;
					double param = wallCurve.Project(openingPoint).Parameter;

					// Try to get width from bounding box
					BoundingBoxXYZ bbox = opening.get_BoundingBox(null);
					double width = 0.0;
					if (bbox != null)
					{
						XYZ size = bbox.Max - bbox.Min;
						// Use the largest horizontal dimension as width
						width = Math.Max(Math.Abs(size.X), Math.Abs(size.Y));
					}
					else
					{
						width = 1.0; // fallback width in feet
					}

					// Project width to curve parameter space
					double halfWidth = width / 2.0;
					XYZ tangent = wallCurve.ComputeDerivatives(param, false).BasisX.Normalize();
					XYZ left = openingPoint - tangent.Multiply(halfWidth);
					XYZ right = openingPoint + tangent.Multiply(halfWidth);
					double paramLeft = wallCurve.Project(left).Parameter;
					double paramRight = wallCurve.Project(right).Parameter;

					double segStart = Math.Min(paramLeft, paramRight);
					double segEnd = Math.Max(paramLeft, paramRight);

					// Clamp to wall curve
					segStart = Math.Max(segStart, wallStart);
					segEnd = Math.Min(segEnd, wallEnd);

					openingSegments.Add((segStart, segEnd));
				}

				// Sort and merge overlapping opening segments
				openingSegments = openingSegments.OrderBy(s => s.Item1).ToList();
				List<(double, double)> merged = new List<(double, double)>();
				foreach (var seg in openingSegments)
				{
					if (merged.Count == 0)
					{
						merged.Add(seg);
					}
					else
					{
						var last = merged[merged.Count - 1];
						if (seg.Item1 <= last.Item2 + Geometric.TOL)
						{
							merged[merged.Count - 1] = (last.Item1, Math.Max(last.Item2, seg.Item2));
						}
						else
						{
							merged.Add(seg);
						}
					}
				}

				// Now split the wall curve by the merged opening segments
				double prev = wallStart;
				foreach (var seg in merged)
				{
					if (seg.Item1 > prev + Geometric.TOL)
					{
						var newCurve = wallCurve.Clone();
						newCurve.MakeBound(prev, seg.Item1);
						wallCurves.Add(newCurve);
					}
					prev = seg.Item2;
				}
				if (prev < wallEnd - Geometric.TOL)
				{
					var newCurve = wallCurve.Clone();
					newCurve.MakeBound(prev, wallEnd);
					wallCurves.Add(newCurve);
				}
			}

			// Remove Tiny Loops
			double tinyLoopTol = minSize == 0 ? 0.5 : minSize;
			wallCurves = wallCurves
				.Where(c => c != null && c.Length > tinyLoopTol)
				.ToList();

			if (wallCurves == null || wallCurves.Count < 1)
				return null;

			// Get Intersecting Walls
			List<ElementId> joinedWallIds = new List<ElementId>();
			List<Wall> joinedWalls = Geometric.GetIntersectingWalls(wall, view);
			foreach (Wall wall1 in joinedWalls)
			{
				joinedWallIds.Add(wall1.Id);
			}

			if (joinedWallIds != null && joinedWallIds.Count > 0)
			{
				var selfCurve = ((LocationCurve)(wall.Location)).Curve;
				List<XYZ> wallJoinIntersections = new List<XYZ>();
				foreach (ElementId idWall in joinedWallIds)
				{
					Wall joined = doc.GetElement(idWall) as Wall;
					if (joined != null && joined.Location is LocationCurve curve)
					{
						List<XYZ> pts = Geometric.GetCurveIntersections(selfCurve, curve.Curve);
						foreach (var pt in pts)
						{
							if (!wallJoinIntersections.Contains(pt))
							{
								wallJoinIntersections.Add(pt);
							}
						}
					}
				}

				// Split each wallCurve at intersection points (if inside curve bounds)
				List<Curve> splitCurves = new List<Curve>();
				foreach (var curve in wallCurves)
				{
					List<double> splitParams = new List<double>();
					double startParam = curve.GetEndParameter(0);
					double endParam = curve.GetEndParameter(1);

					// Find intersection points that are inside the curve (not at endpoints)
					foreach (var pt in wallJoinIntersections)
					{
						IntersectionResult result = curve.Project(pt);
						if (result != null)
						{
							double param = result.Parameter;
							// Only split if strictly inside the curve (not at endpoints)
							if (param > startParam + Helpers.Geometric.TOL && param < endParam - Helpers.Geometric.TOL)
							{
								splitParams.Add(param);
							}
						}
					}

					// Sort and split the curve at these parameters
					if (splitParams.Count > 0)
					{
						splitParams.Add(startParam);
						splitParams.Add(endParam);
						splitParams = splitParams.Distinct().OrderBy(p => p).ToList();
						for (int i = 0; i < splitParams.Count - 1; i++)
						{
							double p0 = splitParams[i];
							double p1 = splitParams[i + 1];
							if (p1 - p0 > Geometric.TOL)
							{
								Curve seg = curve.Clone();
								seg.MakeBound(p0, p1);
								splitCurves.Add(seg);
							}
						}
					}
					else
					{
						splitCurves.Add(curve);
					}
				}

				// Remove tiny loops
				wallCurves = splitCurves
					.Where(c => c != null && c.Length > tinyLoopTol)
					.ToList();
			}
			return wallCurves;
		}
	}
}
