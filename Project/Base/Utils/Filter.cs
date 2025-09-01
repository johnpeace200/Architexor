using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;

namespace Architexor.Utils.Filter
{

	public abstract class CSelectionFilter : ISelectionFilter
	{
		public abstract bool AllowElement(Element elem);

		public abstract bool AllowReference(Reference reference, XYZ position);
	}

	public class WallsPickFilter : CSelectionFilter
	{
		public override bool AllowElement(Element elem)
		{
			if (elem.Category != null)
			{
#if REVIT2024 || REVIT2025
				return (elem.Category.Id.Value.Equals((int)BuiltInCategory.OST_Walls));
#else
                return (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Walls));
#endif
			}
			return false;
		}

		public override bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}

	public class WallsAndPartsPickFilter : CSelectionFilter
	{
		private readonly Document m_Doc;

		public WallsAndPartsPickFilter(Document doc)
		{
			m_Doc = doc;
		}

		public override bool AllowElement(Element elem)
		{
			if (elem.Category != null)
			{
#if REVIT2024 || REVIT2025
				if (elem.Category.Id.Value.Equals((int)BuiltInCategory.OST_Parts))
#else
				if (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Parts))
#endif
				{
					Element parent = Util.GetSourceElementOfPart(m_Doc, elem as Part);
#if REVIT2024 || REVIT2025
					return (parent != null)
							&& ((parent.Category.Id.Value.Equals((int)BuiltInCategory.OST_Walls))
							);
#else
					return (parent != null)
						&& ((parent.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Walls))
						);
#endif
				}
				else
				{
#if REVIT2024 || REVIT2025
					return (elem.Category.Id.Value.Equals((int)BuiltInCategory.OST_Walls));
#else
					return (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Walls));
#endif
				}
			}
			return false;
		}

		public override bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}

	public class SlabsPickFilter : CSelectionFilter
	{
		public override bool AllowElement(Element elem)
		{
			if (elem.Category != null)
			{
#if REVIT2024 || REVIT2025
				return (elem.Category.Id.Value.Equals((int)BuiltInCategory.OST_Roofs))
						|| (elem.Category.Id.Value.Equals((int)BuiltInCategory.OST_Floors));
#else
                return (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Roofs))
					|| (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors));
#endif
			}
			return false;
		}

		public override bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}

	public class SlabsAndPartsPickFilter : CSelectionFilter
	{
		private readonly Document m_Doc;

		public SlabsAndPartsPickFilter(Document doc)
		{
			m_Doc = doc;
		}

		public override bool AllowElement(Element elem)
		{
			if (elem.Category != null)
			{
#if REVIT2024 || REVIT2025
				if (elem.Category.Id.Value.Equals((int)BuiltInCategory.OST_Parts))
				{
					Element parent = Util.GetSourceElementOfPart(m_Doc, elem as Part);
					return (parent != null)
							&& ((parent.Category.Id.Value.Equals((int)BuiltInCategory.OST_Roofs))
							|| (parent.Category.Id.Value.Equals((int)BuiltInCategory.OST_Floors))
							);
				}
				else
				{
					return (elem.Category.Id.Value.Equals((int)BuiltInCategory.OST_Roofs))
							|| (elem.Category.Id.Value.Equals((int)BuiltInCategory.OST_Floors));
				}
#else
                if (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Parts))
				{
					Element parent = Util.GetSourceElementOfPart(m_Doc, elem as Part);
					return (parent != null)
						&& ((parent.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Roofs))
						|| (parent.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors))
						);
				}
				else
				{
					return (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Roofs))
						|| (elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors));
				}
#endif
			}
			return false;
		}

		public override bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}

	public class PartPickFilter : CSelectionFilter
	{
		private readonly Document m_Doc;

		public PartPickFilter(Document doc)
		{
			m_Doc = doc;
		}

		public override bool AllowElement(Element elem)
		{
#if REVIT2024 || REVIT2025
			if (elem.Category != null
					&& elem.Category.Id.Value.Equals((int)BuiltInCategory.OST_Parts))
			{
				Element parent = Util.GetSourceElementOfPart(m_Doc, elem as Part);
				return (parent != null)
						&& ((parent.Category.Id.Value.Equals((int)BuiltInCategory.OST_Roofs))
						|| (parent.Category.Id.Value.Equals((int)BuiltInCategory.OST_Floors))
						|| (parent.Category.Id.Value.Equals((int)BuiltInCategory.OST_Walls))
						);
			}
#else
            if (elem.Category != null
				&& elem.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Parts))
            {
				Element parent = Util.GetSourceElementOfPart(m_Doc, elem as Part);
				return (parent != null)
					&& ((parent.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Roofs))
					|| (parent.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Floors))
					|| (parent.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Walls))
					);
			}
#endif
			return false;
		}

		public override bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}

	public class FamilyPickFilter : CSelectionFilter
	{
		private readonly string m_sFamilyName = "";

		public FamilyPickFilter(string sFamilyName = "")
		{
			m_sFamilyName = "";
		}

		public override bool AllowElement(Element elem)
		{
			bool bRet = elem is FamilyInstance;
			if (bRet && m_sFamilyName != "")
				bRet = (elem as FamilyInstance).Symbol.Family.Name == m_sFamilyName;
			return bRet;
		}

		public override bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}

	public class WindowDoorInElemPickFilter : CSelectionFilter
	{
		private readonly ElementId parentId;
		public WindowDoorInElemPickFilter(ElementId parentID)
		{
			parentId = parentID;
		}

		public override bool AllowElement(Element e)
		{
#if REVIT2024 || REVIT2025
			if (e.Category != null && e is FamilyInstance family && family.Host != null)
				return (((e.Category.Id.Value.Equals((int)BuiltInCategory.OST_Doors)) ||
						(e.Category.Id.Value.Equals((int)BuiltInCategory.OST_Windows)) ||
						(e.Category.Id.Value.Equals((int)BuiltInCategory.OST_GenericModel))
						) &&
						family.Host.Id == parentId);
#else
            if (e.Category != null && e is FamilyInstance family && family.Host != null)
				return (((e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Doors)) ||
					(e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Windows)) ||
					(e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_GenericModel))
					) &&
					family.Host.Id == parentId);
#endif
			return false;
		}

		public override bool AllowReference(Reference r, XYZ p)
		{
			return false;
		}
	}

	public class WindowDoorInElemPickFilterExcept : CSelectionFilter
	{
		private readonly ElementId parentId;
		private readonly List<Reference> ExceptList;
		public WindowDoorInElemPickFilterExcept(ElementId parentID, List<Reference> exceptList)
		{
			parentId = parentID;
			ExceptList = exceptList;
		}

		public override bool AllowElement(Element e)
		{
#if REVIT2024 || REVIT2025
			if (e.Category != null && e is FamilyInstance family && family.Host != null &&
				 ((e.Category.Id.Value.Equals((int)BuiltInCategory.OST_Doors)) ||
				 (e.Category.Id.Value.Equals((int)BuiltInCategory.OST_Windows)) ||
				 (e.Category.Id.Value.Equals((int)BuiltInCategory.OST_GenericModel))) &&
				 family.Host.Id == parentId)
#else
            if (e.Category != null && e is FamilyInstance family && family.Host != null &&
			   ((e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Doors)) ||
			   (e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_Windows)) ||
			   (e.Category.Id.IntegerValue.Equals((int)BuiltInCategory.OST_GenericModel))) &&
			   family.Host.Id == parentId)
#endif
			{
				foreach (Reference r in ExceptList)
					if (r.ElementId == e.Id) return false;
				return true;
			}
			return false;
		}

		public override bool AllowReference(Reference r, XYZ p)
		{
			return false;
		}
	}

	public class EdgeInElemPickFilter : CSelectionFilter
	{
		private readonly Element parent;
		public EdgeInElemPickFilter(Element parent)
		{
			this.parent = parent;
		}

		public override bool AllowElement(Element e)
		{
			return true;
			//return false;
		}

		public override bool AllowReference(Reference r, XYZ p)
		{
			try
			{
				Edge edge = parent.GetGeometryObjectFromReference(r) as Edge;
				if (edge != null)
					return true;
				else
					return false;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}

	public class InArrayPickFilter : CSelectionFilter
	{
		private readonly List<ElementId> arr;
		public InArrayPickFilter(List<ElementId> arr)
		{
			this.arr = arr;
		}

		public override bool AllowElement(Element e)
		{
			foreach (ElementId eId in arr)
			{
				if (eId == e.Id)
					return true;
			}
			return false;
		}

		public override bool AllowReference(Reference r, XYZ p)
		{
			return false;
		}
	}

	public class PointOnFaceFilter : CSelectionFilter
	{
		// Revit document.
		private readonly Document m_doc = null;
		private readonly Face m_face = null;

		/// <summary>
		/// Constructor the filter and initialize the document.
		/// </summary>
		/// <param name="doc">The document.</param>
		public PointOnFaceFilter(Document doc, Face face)
		{
			m_doc = doc;
			m_face = face;
		}

		/// <summary>
		/// Allow wall to be selected
		/// </summary>
		/// <param name="element">A candidate element in selection operation.</param>
		/// <returns>Return true for wall. Return false for non wall element.</returns>
		public override bool AllowElement(Element element)
		{
			return true;// element is Wall;
		}

		/// <summary>
		/// Allow face reference to be selected
		/// </summary>
		/// <param name="refer">A candidate reference in selection operation.</param>
		/// <param name="point">The 3D position of the mouse on the candidate reference.</param>
		/// <returns>Return true for face reference. Return false for non face reference.</returns>
		public override bool AllowReference(Reference refer, XYZ point)
		{
			GeometryObject geoObject = m_doc.GetElement(refer).GetGeometryObjectFromReference(refer);
			return geoObject != null && geoObject is Face && geoObject as Face == m_face;
		}
	}

	public class PointOnEdgeFilter : CSelectionFilter
	{
		// Revit document.
		private readonly Document m_doc = null;
		private readonly Edge m_Edge = null;

		/// <summary>
		/// Constructor the filter and initialize the document.
		/// </summary>
		/// <param name="doc">The document.</param>
		public PointOnEdgeFilter(Document doc, Edge edge)
		{
			m_doc = doc;
			m_Edge = edge;
		}

		/// <summary>
		/// Allow wall to be selected
		/// </summary>
		/// <param name="element">A candidate element in selection operation.</param>
		/// <returns>Return true for wall. Return false for non wall element.</returns>
		public override bool AllowElement(Element element)
		{
			return true;// element is Wall;
		}

		/// <summary>
		/// Allow face reference to be selected
		/// </summary>
		/// <param name="refer">A candidate reference in selection operation.</param>
		/// <param name="point">The 3D position of the mouse on the candidate reference.</param>
		/// <returns>Return true for face reference. Return false for non face reference.</returns>
		public override bool AllowReference(Reference refer, XYZ point)
		{
			GeometryObject geoObject = m_doc.GetElement(refer).GetGeometryObjectFromReference(refer);
			return geoObject != null && geoObject is Edge && geoObject as Edge == m_Edge;
		}
	}

	public class PointOnElementFilter : CSelectionFilter
	{
		private readonly Element m_Element = null;

		/// <summary>
		/// Constructor the filter and initialize the document.
		/// </summary>
		public PointOnElementFilter(Element elem)
		{
			m_Element = elem;
		}

		/// <summary>
		/// Allow wall to be selected
		/// </summary>
		/// <param name="element">A candidate element in selection operation.</param>
		/// <returns>Return true for wall. Return false for non wall element.</returns>
		public override bool AllowElement(Element element)
		{
			if (element.Id == m_Element.Id)
				return true;
			return false;
		}

		/// <summary>
		/// Allow face reference to be selected
		/// </summary>
		/// <param name="refer">A candidate reference in selection operation.</param>
		/// <param name="point">The 3D position of the mouse on the candidate reference.</param>
		/// <returns>Return true for face reference. Return false for non face reference.</returns>
		public override bool AllowReference(Reference refer, XYZ point)
		{
			return true;
		}
	}

	public class EdgeOnFaceFilter : CSelectionFilter
	{
		// Revit document.
		private readonly Document m_doc = null;
		private readonly Face m_face = null;

		/// <summary>
		/// Constructor the filter and initialize the document.
		/// </summary>
		/// <param name="doc">The document.</param>
		public EdgeOnFaceFilter(Document doc, Face face)
		{
			m_doc = doc;
			m_face = face;
		}

		/// <summary>
		/// Allow wall to be selected
		/// </summary>
		/// <param name="element">A candidate element in selection operation.</param>
		/// <returns>Return true for wall. Return false for non wall element.</returns>
		public override bool AllowElement(Element element)
		{
			return true;// element is Wall;
		}

		/// <summary>
		/// Allow face reference to be selected
		/// </summary>
		/// <param name="refer">A candidate reference in selection operation.</param>
		/// <param name="point">The 3D position of the mouse on the candidate reference.</param>
		/// <returns>Return true for face reference. Return false for non face reference.</returns>
		public override bool AllowReference(Reference refer, XYZ point)
		{
			GeometryObject geoObject = m_doc.GetElement(refer).GetGeometryObjectFromReference(refer);

			return geoObject != null && geoObject is Edge
				&& ((geoObject as Edge).GetFace(0) == m_face
				|| (geoObject as Edge).GetFace(1) == m_face);
		}
	}
}
