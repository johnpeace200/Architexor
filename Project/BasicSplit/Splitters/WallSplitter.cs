using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Architexor.Request;
using Architexor.Utils;
using Architexor.Utils.Filter;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Architexor.BasicSplit.Splitters
{
	public class WallSplitter : Splitter
	{
		public override bool ProcessRequest(ArchitexorRequestId reqId)
		{
			switch (reqId)
			{
				case ArchitexorRequestId.SelectSplitElements:
					Document doc = m_uiApp.ActiveUIDocument.Document;
					Selection sel = m_uiApp.ActiveUIDocument.Selection;

					try
					{
						//  Select wall/part(s)
						GlobalHook.Subscribe();
						WallsAndPartsPickFilter selFilter = new WallsAndPartsPickFilter(doc);
						IList<Reference> refers = sel.PickObjects(ObjectType.Element, selFilter, "Please select walls.");
						GlobalHook.Unsubscribe();

						m_Elements.Clear();
						foreach (Reference refer in refers)
						{
							m_Elements.Add(doc.GetElement(refer));
						}

						Initialize();
					}
					catch(Autodesk.Revit.Exceptions.OperationCanceledException) {}
					catch(Exception e) {
						//Debug.WriteLine(e.Message);
						MessageBox.Show(e.StackTrace, e.Message);
						LogManager.Write(LogManager.WarningLevel.Error, e.StackTrace, e.Message);
					}
					break;
				default:
					break;
			}
			return base.ProcessRequest(reqId);
		}

		public override void Initialize()
		{
			AssociatedOpenings.Clear();
			m_SplitElements.Clear();
			AdjacentElements.Clear();
		}
	}
}
