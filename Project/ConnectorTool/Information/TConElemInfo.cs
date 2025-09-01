using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Architexor.Utils;

namespace ConnectorTool.Information
{
	/// <summary>
	/// This class contains Basic information for creating Connectors
	/// </summary>
	public class TConElemInfo
	{
		public Element PrimaryElement { get; set; }

		/// <summary>
		/// Secondary Element to be joined with the connection
		/// </summary>
		public Element SecondaryElement { get; set; }

		public double Primary_Element_Width = 0.0;
		public double Primary_Element_Height = 0.0;
		public double Primary_Element_Depth = 0.0;

		public double Secondary_Element_Width = 0.0;
		public double Secondary_Element_Height = 0.0;
		public double Secondary_Element_Length = 0.0;

		public XYZ Secondary_Width_Vector = new XYZ(0, 0, 1.0);
		public XYZ Secondary_Height_Vector = new XYZ(0, 0, 1.0);

		/// <summary>
		/// The Parameters of Primary Element(width and height of section).
		/// </summary>
		public bool GetPrimaryElementDimension()
		{
			Wall wall = null;
			if (PrimaryElement.Category.Name == "Walls")
			{
				wall = PrimaryElement as Wall;
				Primary_Element_Depth = Util.IUToMm(wall.Width);
			}

			List<Curve> boundary = Util.GetOptimizedBoundaryCurves(HostFace);

			foreach (Curve c in boundary)
			{
				XYZ vector = c.GetEndPoint(1) - c.GetEndPoint(0);
				if (Util.IsEqual(vector.Normalize(), new XYZ(0.0, 0.0, 1.0)))
				{
					Primary_Element_Height = Util.IUToMm(vector.GetLength());
				}
				else if (Util.IsEqual(vector.Normalize(), new XYZ(0.0, 0.0, -1.0)))
				{

				}
				else if (Util.IsZero(vector.Z))
				{
					Primary_Element_Width = Util.IUToMm(vector.GetLength());
				}
				else
				{
					MessageBox.Show("Error", "Seems the face is rotated. We are not supporting such complicated shape yet.");
					return false;
				}
			}

			//	Get Primary Element Depth
			if (Primary_Element_Depth == 0.0)
			{
				Util.GetFaceList(PrimaryElement, out List<PlanarFace> faceList);
				Plane plane = HostFace.GetSurface() as Plane;
				double distance, max = 0.0;
				foreach (PlanarFace face in faceList)
				{
					foreach (CurveLoop cl in face.GetEdgesAsCurveLoops())
					{
						foreach (Curve c in cl)
						{
							if ((distance = Math.Abs(plane.SignedDistanceTo(c.GetEndPoint(0)))) > max)
							{
								max = distance;
							}
						}
					}
				}
				Primary_Element_Depth = Util.IUToMm(max);
			}

			return true;
		}

		/// <summary>
		/// The Parameters of Secondary Element(width and height of section).
		/// </summary>
		public bool GetSecondaryElementDimension()
		{
			LocationCurve lc = SecondaryElement.Location as LocationCurve;
			XYZ lcVector = lc.Curve.GetEndPoint(1) - lc.Curve.GetEndPoint(0);
			lcVector = lcVector.Normalize();

			Util.GetFaceList(SecondaryElement, out List<PlanarFace> faceList);
			foreach(Face face in faceList)
			{
				List<Curve> boundary = Util.GetOptimizedBoundaryCurves(face);
				if(face is PlanarFace
					&& (Util.IsEqual((face as PlanarFace).FaceNormal, lcVector)
					|| Util.IsEqual((face as PlanarFace).FaceNormal, -lcVector))) {
					foreach (Curve c in boundary)
					{
						XYZ vector = c.GetEndPoint(1) - c.GetEndPoint(0);
						if (Util.IsEqual(vector.Normalize(), new XYZ(0.0, 0.0, 1.0)))
						{
							Secondary_Element_Height = Util.IUToMm(vector.GetLength());
							Secondary_Height_Vector = vector.Normalize();
							Secondary_Width_Vector = vector.CrossProduct(lcVector).Normalize();
						}
						else if (Util.IsEqual(vector.Normalize(), new XYZ(0.0, 0.0, -1.0)))
						{

						}
						else if (Util.IsZero(vector.Z))
						{
							Secondary_Element_Width = Util.IUToMm(vector.GetLength());
						}
						else
						{
							MessageBox.Show("Error", "Seems the face is rotated. We are not supporting such complicated shape yet.");
							return false;
						}
					}
				}
			}
			if (Secondary_Element_Width == 0 || Secondary_Element_Height == 0)
				return false;
			//  Finding intersect face
			//XYZ normal = (HostFace as PlanarFace).FaceNormal;
			//if (Util.CheckSelectedElement(SecondaryElement, out List<Face> faceList))
			//{
			//	foreach (Face face in faceList)
			//	{
			//		List<Curve> boundary = PanelToolUtil.GetOptimizedBoundaryCurves(face);

			//		List<ElementId> eIds = (List<ElementId>)SecondaryElement.GetGeneratingElementIds(face);
			//		if (eIds != null && eIds.Contains(PrimaryElement.Id) && face is PlanarFace && (PanelToolUtil.IsEqual((face as PlanarFace).FaceNormal, normal)
			//			|| PanelToolUtil.IsEqual((face as PlanarFace).FaceNormal, -normal))
			//			)
			//		{
			//			//List<Curve> boundary = PanelToolUtil.GetOptimizedBoundaryCurves(face);

			//			foreach (Curve c in boundary)
			//			{
			//				XYZ vector = c.GetEndPoint(1) - c.GetEndPoint(0);
			//				if (PanelToolUtil.IsEqual(vector.Normalize(), new XYZ(0.0, 0.0, 1.0)))
			//				{
			//					Secondary_Element_Height = Util.IUToMm(vector.GetLength());
			//					Secondary_Height_Vector = vector.Normalize();
			//					Secondary_Width_Vector = vector.CrossProduct((HostFace as PlanarFace).FaceNormal).Normalize();
			//				}
			//				else if (PanelToolUtil.IsEqual(vector.Normalize(), new XYZ(0.0, 0.0, -1.0)))
			//				{

			//				}
			//				else if (PanelToolUtil.IsZero(vector.Z))
			//				{
			//					Secondary_Element_Width = Util.IUToMm(vector.GetLength());
			//				}
			//				else
			//				{
			//					MessageBox.Show("Error", "Seems the face is rotated. We are not supporting such complicated shape yet.");
			//					return false;
			//				}
			//			}
			//		}
			//	}
			//}

			//PanelToolUtil.GetFaceList(SecondaryElement, out faceList);
			////	The tool is working with horizontal beams only.
			//if (Secondary_Element_Width == 0 || Secondary_Element_Height == 0)
			//{
			//	foreach (Face face in faceList)
			//	{
			//		List<Curve> boundary = PanelToolUtil.GetOptimizedBoundaryCurves(face);

			//		//SetComparisonResult bRes = PanelToolUtil.IntersectWith(HostFace, face);
			//		if ((PanelToolUtil.IsEqual((face as PlanarFace).FaceNormal, normal)
			//			|| PanelToolUtil.IsEqual((face as PlanarFace).FaceNormal, -normal))
			//			//&& bRes == SetComparisonResult.Superset
			//			)
			//		{
			//			foreach (Curve c in boundary)
			//			{
			//				XYZ vector = c.GetEndPoint(1) - c.GetEndPoint(0);
			//				if (PanelToolUtil.IsEqual(vector.Normalize(), new XYZ(0.0, 0.0, 1.0)))
			//				{
			//					Secondary_Element_Height = Util.IUToMm(vector.GetLength());
			//					Secondary_Height_Vector = vector.Normalize();
			//					Secondary_Width_Vector = vector.CrossProduct((HostFace as PlanarFace).FaceNormal).Normalize();
			//				}
			//				else if (PanelToolUtil.IsEqual(vector.Normalize(), new XYZ(0.0, 0.0, -1.0)))
			//				{

			//				}
			//				else if (PanelToolUtil.IsZero(vector.Z))
			//				{
			//					Secondary_Element_Width = Util.IUToMm(vector.GetLength());
			//				}
			//				else
			//				{
			//					MessageBox.Show("Error", "Seems the face is rotated. We are not supporting such complicated shape yet.");
			//					return false;
			//				}
			//			}
			//		}
			//	}
			//}
			//if (Secondary_Element_Width == 0 || Secondary_Element_Height == 0)
			//{
			//	MessageBox.Show("Error", "Failed to get the dimension of the secondary element.");
			//	return false;
			//}
			return true;
		}

		public XYZ GetOrigin()
		{
			//  Finding intersect face
			XYZ normal = (HostFace as PlanarFace).FaceNormal;
			if (Util.GetFaceList(SecondaryElement, out List<PlanarFace> faceList))
			{
				foreach (PlanarFace face in faceList)
				{
					if (Util.IsEqual(face.FaceNormal, normal))
					{
						List<Curve> boundary = Util.GetOptimizedBoundaryCurves(face);

						//if(!boundary.IsRectangular(face.GetSurface() as Plane))
						//{
						//	MessageBox.Show("Error", "The shape must be rectangular.");
						//	return new XYZ(0, 0, 0);
						//}
						Curve c1 = null, c2 = null;

						foreach (Curve c in boundary)
						{
							if (c1 == null)
								c1 = c;
							else if (c2 == null)
								c2 = c;
							else
								break;
						}
						XYZ origin = c1.GetEndPoint(1);
						origin += (c1.GetEndPoint(0) - c1.GetEndPoint(1)) / 2;
						origin += (c2.GetEndPoint(1) - c2.GetEndPoint(0)) / 2;
						return origin;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Face for Locating Connection(Primary Element)
		/// </summary>
		public Face HostFace { get; set; }

		//private void GetStartAndEndPoint()
		//{
		//	// Get the direction and reference points of SecondaryElement.
		//	if (SecondaryElement.Location is LocationCurve locationCurve)
		//	{
		//		Curve curve = locationCurve.Curve;
		//		Line line = curve as Line;
		//		//  The start and end point of SecondaryElement
		//		XYZ startPt = line.GetEndPoint(0);
		//		XYZ endPt = line.GetEndPoint(1);
		//		// The origin point of PrimaryElement
		//		XYZ priOriginP = XYZ.Zero;
		//		if (PrimaryElement.Category.Name == "Structural Columns") // column
		//		{
		//			LocationPoint locationPoint = PrimaryElement.Location as LocationPoint;
		//			priOriginP = locationPoint.Point;
		//		}
		//		else if (PrimaryElement.Category.Name == "Walls") // wall
		//		{
		//			LocationCurve wallCurve = PrimaryElement.Location as LocationCurve;
		//			Line wallLine = wallCurve.Curve as Line;
		//			priOriginP = Util.Midpoint(wallLine);
		//		}
		//		if (priOriginP.DistanceTo(startPt) <= priOriginP.DistanceTo(endPt))
		//		{
		//			m_startPt = startPt;
		//			m_endPt = endPt;
		//		}
		//		else
		//		{
		//			m_startPt = endPt;
		//			m_endPt = startPt;
		//		}
		//	}
		//	else return;
		//}
	}
}
