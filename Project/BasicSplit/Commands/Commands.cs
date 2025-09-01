using Architexor.Core;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Architexor.Request;
using System;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Architexor.Commands
{
	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class SplitWall : IExternalCommand
	{
		/// <summary>
		/// show FrmSplitSettings as popup, hand over the control to the form
		/// </summary>
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

			//if (Application.thisLicenses.Count == 0)
			//{
			//	message = "Please request and activate the " + Constants.BRAND + " subscription.";
			//	return Result.Failed;
			//}
			PanelTool.Application.thisApp.DoRequest(uiapp, ArchitexorRequestId.SplitWall);

			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class SplitFloor : IExternalCommand
	{
		/// <summary>
		/// After select slab/part(s) using PickObjects, show FrmSplitSettings as popup, hand over the control to the form
		/// </summary>
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;
			
			//if (Application.thisLicenses.Count == 0)
			//{
			//	message = "Please request and activate the " + Constants.BRAND + " subscription.";
			//	return Result.Failed;
			//}
			PanelTool.Application.thisApp.DoRequest(uiapp, ArchitexorRequestId.SplitFloor);

			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class SplitConfiguration : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			TaskDialog.Show("Notice", "This function is coming soon.");
			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class AutoSplit : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			TaskDialog.Show("Notice", "This function is coming soon.");

			Document doc = commandData.Application.ActiveUIDocument.Document;
			var collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
			var familyInstances = collector.OfClass(typeof(FamilyInstance));
			try
			{
				foreach (var anElem in familyInstances)
				{
					if (anElem is FamilyInstance)
					{
						FamilyInstance aFamilyInst = anElem as FamilyInstance;
						if (aFamilyInst.Name != "Half Lap")
							continue;
						// we need to skip nested family instances 
						// since we already get them as per below
						if (aFamilyInst.SuperComponent == null)
						{
							// this is a family that is a root family
							// ie might have nested families 
							// but is not a nested one
							var subElements = aFamilyInst.GetSubComponentIds();
							if (subElements.Count == 0)
							{
								// no nested families
								System.Diagnostics.Debug.WriteLine(aFamilyInst.Name + " has no nested families");
							}
							else
							{
								// has nested families
								foreach (var aSubElemId in subElements)
								{
									var aSubElem = doc.GetElement(aSubElemId);
									if (aSubElem is FamilyInstance)
									{
										System.Diagnostics.Debug.WriteLine(aSubElem.Name + " is a nested family of " + aFamilyInst.Name);
									}
								}
							}
						}
					}
				}
			}
			catch(Exception)
			{

			}

			return Result.Succeeded;
		}
	}

	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class AnalysisSplit : IExternalCommand
	{
		/// <summary>
		/// show FrmSplitSettings as popup, hand over the control to the form
		/// </summary>
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			UIApplication uiapp = commandData.Application;

			//if (PanelTool.Application.thisLicenses.Count == 0)
			//{
			//	message = "Please request and activate the " + Constants.BRAND + " subscription.";
			//	return Result.Failed;
			//}
			PanelTool.Application.thisApp.DoRequest(uiapp, ArchitexorRequestId.AnalysisSplit);

			return Result.Succeeded;
		}
	}
}
