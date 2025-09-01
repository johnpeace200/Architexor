using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Architexor.Request;
using System.Collections.Generic;
using Document = Autodesk.Revit.DB.Document;
using System.Linq;
using Architexor.Core;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Architexor.Commands
{
	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class HalfLapConnection : IExternalCommand
	{
		/// <summary>
		/// After select part(s) using PickObjects, show FrmConnectSettings as popup, hand over the control to the form
		/// </summary>
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			//  Get application and document objects
			UIApplication uiApp = commandData.Application;

			//if (Application.thisLicenses.Count == 0)
			//{
			//	message = "Please request and activate the " + Constants.BRAND + " subscription.";
			//	return Result.Failed;
			//}
			PanelTool.Application.thisApp.DoRequest(uiApp, ArchitexorRequestId.HalfLap);

			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class JointBoardConnection : IExternalCommand
	{
		/// <summary>
		/// After select part(s) using PickObjects, show FrmConnectSettings as popup, hand over the control to the form
		/// </summary>
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			//  Get application and document objects
			UIApplication uiApp = commandData.Application;

			//if (Application.thisLicenses.Count == 0)
			//{
			//	message = "Please request and activate the " + Constants.BRAND + " subscription.";
			//	return Result.Failed;
			//}
			PanelTool.Application.thisApp.DoRequest(uiApp, ArchitexorRequestId.JointBoard);

			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class AutoArrangeConnectors : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			TaskDialog.Show("Notice", "This function is coming soon.");
			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class ConnectorConfiguration : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class ShowHideHLVoids : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			string sState = PanelTool.Application.thisApp.ToggleButton();

			Document doc = commandData.Application.ActiveUIDocument.Document;

			List<FamilyInstance> totalHLAs = new(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(FamilyInstance))
					.Where(ins => ins.Name == "ATX_VoidCut_HalfLapA")
					.ToList()
					.Cast<FamilyInstance>()
					);
			List<FamilyInstance> totalHLBs = new(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(FamilyInstance))
					.Where(ins => ins.Name == "ATX_VoidCut_HalfLapB")
					.ToList()
					.Cast<FamilyInstance>()
					);
			List<ElementId> elemIds = new();
			foreach (FamilyInstance fi in totalHLAs)
				elemIds.Add(fi.Id);

			foreach (FamilyInstance fi in totalHLBs)
				elemIds.Add(fi.Id);

			if (elemIds.Count == 0)
				return Result.Succeeded;

			Transaction trans = new(doc);
			trans.Start("Show/Hide HL Voids");
			if (sState == "Show HL Voids")
				doc.ActiveView.HideElements(elemIds);
			else
			{
				doc.ActiveView.UnhideElements(elemIds);
			}
			trans.Commit();
			return Result.Succeeded;
		}
	}
}
