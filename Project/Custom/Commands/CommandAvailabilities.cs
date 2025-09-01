using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace PanelTool.CommandAvailabilities
{
	/*
	public class MergePartsAvailability : IExternalCommandAvailability
	{
		public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
		{
			try
			{
				int i;
				Document doc = applicationData.ActiveUIDocument.Document;
				Selection sel = applicationData.ActiveUIDocument.Selection;

				ICollection<ElementId> partIds = sel.GetElementIds();
				if (partIds.Count < 2)
					return false;

				IEnumerator<ElementId> enumerator = partIds.GetEnumerator();
				for (i = 0; i < partIds.Count; i++)
				{
					enumerator.MoveNext();
					if (!(doc.GetElement(enumerator.Current) is Part))
					{
						return false;
					}
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
	}*/
}
