using Autodesk.Revit.DB;
using Architexor.Utils;
using System.Collections.Generic;
using System.Linq;
using Splitter = Architexor.BasicSplit.Splitters.Splitter;

namespace Architexor.BasicSplit
{
	public class CSplitWallElement : CSplitElement
	{
		protected CSplitWallElement() { }

		public CSplitWallElement(Splitter splitter, Element e) : base(splitter, e)
		{}

		protected override bool GetBaseFace()
		{
			if(Element is Wall)
			{
				List<Reference> refers = (List<Reference>)HostObjectUtils.GetSideFaces(Element as Wall, ShellLayerType.Exterior);
				BaseFaces = new Face[refers.Count];
				for(int i = 0; i < refers.Count; i++)
				{
					BaseFaces[i] = Element.GetGeometryObjectFromReference(refers[i]) as Face;
				}
				if(refers.Count > 1)
				{
					LogManager.Write(LogManager.WarningLevel.Warning, "CSplitWallElement::GetBaseFace", "Unsupported use case.");
				}
			}
			else
			{
				Document doc = m_Splitter.GetDocument();
				//	Part
				Element parent = Util.GetSourceElementOfPart(doc, Element as Part);

				List<Reference> refers = (List<Reference>)HostObjectUtils.GetSideFaces((HostObject)parent, ShellLayerType.Exterior);
				if (refers.Count > 1)
				{
					LogManager.Write(LogManager.WarningLevel.Warning, "CSplitWallElement::GetBaseFace", "Unsupported use case.");
				}

				PlanarFace baseFaceOfParent = parent.GetGeometryObjectFromReference(refers[0]) as PlanarFace;
				XYZ normalVector = (baseFaceOfParent.GetSurface() as Plane).Normal;

				BaseFaces = new Face[1];
				GeometryElement geomElem = Element.get_Geometry(new Options());
				foreach (GeometryObject geomObject in geomElem)
				{
					if (geomObject is Solid)
					{
						Solid solid = geomObject as Solid;
						FaceArray faceArray = solid.Faces;
						foreach (Face f in faceArray)
						{
							if (f is PlanarFace
								&& Util.IsEqual((f as PlanarFace).FaceNormal, normalVector)
								&& Util.IsEqual((f as PlanarFace).Origin, baseFaceOfParent.Origin))
							{
								BaseFaces[0] = f;
							}
						}
					}
				}
			}

			if (BaseFaces.Count() == 0)
				return false;
			return true;
		}

		protected override void GetSketchPlane()
		{
			//	Based on Presumption 1-2
			SketchPlane = BaseFaces[0].GetSurface() as Plane;
		}

		protected override void GetLocationCurve()
		{
			if (Element is Wall)
			{
				LocationCurve = (Element.Location as LocationCurve).Curve;
				if ((Element as Wall).Flipped)
					LocationCurve = LocationCurve.CreateReversed();
			}
			else        //	Part
			{
				Document doc = m_Splitter.GetDocument();
				Element parent = Util.GetSourceElementOfPart(doc, Element as Part);
				LocationCurve = (parent.Location as LocationCurve).Curve;
				if ((parent as Wall).Flipped)
					LocationCurve = LocationCurve.CreateReversed();
			}

			AdjustLocationCurve();
		}
	}
}
