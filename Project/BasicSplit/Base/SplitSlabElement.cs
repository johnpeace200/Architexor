using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Architexor.Utils;
using Architexor.Utils.Filter;
using System.Collections.Generic;
using System.Linq;
using Color = Autodesk.Revit.DB.Color;
using Splitter = Architexor.BasicSplit.Splitters.Splitter;

namespace Architexor.BasicSplit
{
	public class CSplitSlabElement : CSplitElement
	{
		protected CSplitSlabElement() { }

		public CSplitSlabElement(Splitter splitter, Element e) : base(splitter, e) { }

		protected override bool GetBaseFace()
		{
			if (Element is Part)
			{
				Document doc = m_Splitter.GetDocument();
				//	Part
				Element parent = Util.GetSourceElementOfPart(doc, Element as Part);

				List<Reference> refers = (List<Reference>)HostObjectUtils.GetTopFaces(parent as HostObject);
				List<PlanarFace> parentFaces = new List<PlanarFace>();
				for (int i = 0; i < refers.Count; i++)
				{
					GeometryObject go = parent.GetGeometryObjectFromReference(refers[i]);
					if(go != null && go is PlanarFace)
						parentFaces.Add(go as PlanarFace);
				}

				List<Face> faces = new List<Face>();
				GeometryElement geomElem = Element.get_Geometry(new Options());
				foreach (GeometryObject geomObject in geomElem)
				{
					if (geomObject is Solid)
					{
						Solid solid = geomObject as Solid;
						FaceArray faceArray = solid.Faces;
						foreach (Face f in faceArray)
						{
							if(!(f is PlanarFace))
							{
								//	Not support yet
								continue;
							}

							foreach(PlanarFace pf in parentFaces)
							{
								if (Util.IsEqual((f as PlanarFace).FaceNormal, pf.FaceNormal)
									&& Util.IsEqual((f as PlanarFace).Origin, pf.Origin))
								{
									faces.Add(f);
								}
							}
						}
					}
				}
				BaseFaces = new Face[faces.Count];
				for(int i = 0; i < faces.Count; i++)
				{
					BaseFaces[i] = faces[i];
				}
			}
			else
			{
				List<Reference> refers = (List<Reference>)HostObjectUtils.GetTopFaces(Element as HostObject);
				List<Face> faces = new List<Face>();
				for (int i = 0; i < refers.Count; i++)
				{
					GeometryObject go = Element.GetGeometryObjectFromReference(refers[i]);
					if(go != null && go is Face)
						faces.Add(go as Face);
				}
				BaseFaces = new Face[faces.Count];
				for (int i = 0; i < faces.Count; i++)
				{
					BaseFaces[i] = faces[i];
				}
			}

			if (BaseFaces.Count() == 0)
				return false;
			return true;
		}

		protected override void GetSketchPlane()
		{
			if(BaseFaces.Length == 1)
				SketchPlane = BaseFaces[0].GetSurface() as Plane;
			else
			{
				//	Find Level X Plane
				Document doc = m_Splitter.GetDocument();
				Element level = doc.GetElement(Element.LevelId);
				SketchPlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0, 0, (level as Level).Elevation));
			}
		}

		protected override void GetLocationCurve()
		{
			if (BoundaryCurves.Count >= 2)
				return;

			Document doc = m_Splitter.GetDocument();
			Selection sel = m_Splitter.UIApp.ActiveUIDocument.Selection;

			OverrideGraphicSettings old = doc.ActiveView.GetElementOverrides(Element.Id);
			Color color = new Color(255, 0, 0);
			OverrideGraphicSettings ogs = new OverrideGraphicSettings();
			ogs.SetProjectionLineColor(color);
			ogs.SetProjectionLineWeight(16);
			ogs.SetSurfaceTransparency(75);

			Transaction trans = new Transaction(doc);
			trans.Start("Isolate");
			//doc.Document.ActiveView.IsolateElementTemporary(e.Id);
			doc.ActiveView.SetElementOverrides(Element.Id, ogs);
			trans.Commit();

			Reference r = sel.PickObject(ObjectType.Edge, new EdgeOnFaceFilter(doc, BaseFaces[0]), "Select path that slab will be split perpendicular to.");
			LocationCurve = (Element.GetGeometryObjectFromReference(r) as Edge).AsCurve();

			trans.Start("Restore");
			doc.ActiveView.SetElementOverrides(Element.Id, old);
			trans.Commit();

			AdjustLocationCurve();
		}
	}
}
