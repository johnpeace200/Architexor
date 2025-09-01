#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Architexor.Core;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.IFC;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Architexor.Request;
using Architexor.Utils;
#endregion // Namespaces

namespace Architexor.Commands
{
	[Transaction(TransactionMode.Manual)]
	public class CmdPropertyMarker : IExternalCommand
	{
		public static IEnumerable<ExternalFileReference> GetLinkedFileReferences(Document _document)
		{
			var collector = new FilteredElementCollector(
			  _document);

			var linkedElements = collector
			  .OfClass(typeof(RevitLinkType))
			  .Select(x => x.GetExternalFileReference())
			  .ToList();

			return linkedElements;
		}

		public static IEnumerable<Document> GetLinkedDocuments(Document _document)
		{
			var linkedfiles = GetLinkedFileReferences(
			  _document);

			var linkedFileNames = linkedfiles
			  .Select(x => ModelPathUtils
			   .ConvertModelPathToUserVisiblePath(
				 x.GetAbsolutePath())).ToList();

			return _document.Application.Documents
			  .Cast<Document>()
			  .Where(doc => linkedFileNames.Any(
			   fileName => doc.PathName.Equals(fileName)));
		}

		/// <summary>
		/// Improved implementation by Alexander Ignatovich
		/// supporting curved wall with curved window, 
		/// second attempt, published April 10, 2015:
		/// </summary>
		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			//  Get application and document objects
			UIApplication uiApp = commandData.Application;
			Document doc = uiApp.ActiveUIDocument.Document;
			Selection sel = uiApp.ActiveUIDocument.Selection;

			//if (Application.thisLicenses.Count == 0)
			//{
			//	message = "Please request the " + Constants.BRAND + " license for 3D Parameters tool.";
			//	return Result.Failed;
			//}

			//IEnumerable<Document> doc2
			//	= GetLinkedDocuments(doc);

			//// get the ref of a selected plane
			//Reference pickedRef = sel.PickObject(
			//  ObjectType.PointOnElement,
			 // "Please select a Face");

			//Element elem = doc.GetElement(
			//  pickedRef.ElementId);

			//// get the true position picked 
			//// in the active document

			//XYZ pos = pickedRef.GlobalPoint;

			//// get the ID of the element containing the 
			//// face you picked in the active document 
			//// and in its host document

			//string s = pickedRef
			//  .ConvertToStableRepresentation(doc);

			//string[] tab_str = s.Split(':');

			//string id = tab_str[tab_str.Length - 4];

			//int ID;
			//Int32.TryParse(id, out ID);

			//Type et = elem.GetType();

			//if (typeof(RevitLinkType) == et
			//  || typeof(RevitLinkInstance) == et
			//  || typeof(Instance) == et)
			//{
			//	foreach (Document d in doc2)
			//	{
			//		if (elem.Name.Contains(d.Title))
			//		{
			//			Element element = d.GetElement(
			//			  new ElementId(ID));

			//			Options ops = new Options();
			//			ops.ComputeReferences = true;

			//			// write the name of the element and the 
			//			// number of solids in this only for 
			//			// control to show the possibilities

			//			//MessageBox.Show(element.Name,
			//			//  element.get_Geometry(ops)
			//			//	.Objects.Size.ToString());

			//			GeometryObject obj
			//			  = element.get_Geometry(ops).First();

			//			// test all surfaces of solids in the 
			//			// element and return the one containing 
			//			// the picked point as a planarface to 
			//			// build my sketchplan

			//			//foreach (GeometryObject obj2 in
			//			//  element.get_Geometry(ops).)
			//			//{
			//			//	if (obj2.GetType() == typeof(Solid))
			//			//	{
			//			//		Solid solid2 = obj2 as Solid;
			//			//		foreach (Face face2 in solid2.Faces)
			//			//		{
			//			//			try
			//			//			{
			//			//				if (face2.Project(pos)
			//			//				  .XYZPoint.DistanceTo(pos) == 0)
			//			//				{
			//			//					//return face2 as PlanarFace;
			//			//				}
			//			//			}
			//			//			catch (NullReferenceException)
			//			//			{
			//			//			}
			//			//		}
			//			//	}
			//			//}
			//		}
			//	}
			//}

			PropertyMarker.Application.thisApp.DoRequest(uiApp, ArchitexorRequestId.PropertyMarker);

			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	public class CmdUpdate3DParameters : IExternalCommand
	{
		/// <summary>
		/// Improved implementation by Alexander Ignatovich
		/// supporting curved wall with curved window, 
		/// second attempt, published April 10, 2015:
		/// </summary>
		public Result Execute(
		  ExternalCommandData commandData,
		  ref string message,
		  ElementSet elements)
		{
			//  Get application and document objects
			UIApplication uiApp = commandData.Application;
			PropertyMarker.Application.thisApp.DoRequest(uiApp, ArchitexorRequestId.PropertyMarkerUpdate);

			return Result.Succeeded;
		}
	}
}
