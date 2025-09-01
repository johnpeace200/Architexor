#region namespaces
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ConnectorTool.Information;
using ConnectorTool.Object;
using Architexor.Request;
using Architexor.Utils;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;
using Architexor;
#endregion

namespace ConnectorTool.Engine
{
	/// <summary>
	/// This is main data class for creating connector by face
	/// </summary>
	public class ConnectorBuilder : Controller
	{
		#region Fields
		/// <summary>
		/// ConnectorData
		/// </summary>
		private ConnectorData m_ConnectorData;
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public ConnectorBuilder()
		{
			Initialize();
		}
		#endregion

		#region methods
		public override void Initialize()
		{
			m_ConnectorData = new ConnectorData();
		}

		public ConnectorData GetConnectorData()
		{
			return m_ConnectorData;
		}

		/// <summary>
		/// Create a based-point family instance by face
		/// </summary>
		/// <returns></returns>
		public void CreateConnector()
		{
			UIDocument uidoc = Architexor.ConnectorTool.Application.GetUiApplication().ActiveUIDocument;
			Document doc = uidoc.Document;
			string sErr = "";
			using (TransactionGroup transGroup = new TransactionGroup(doc))
			{
				if (transGroup.Start("CreateConnector") == TransactionStatus.Started)
				{
					if (m_ConnectorData.GetConnectorList().Count > 0)
					{
						using (Transaction trans = new Transaction(doc))
						{
							foreach (TConnector connector in m_ConnectorData.GetConnectorList())
							{
								try
								{
									if (trans.Start("CreateVoidInstance") == TransactionStatus.Started)
									{
										FailureHandlingOptions failureHandlingOptions = trans.GetFailureHandlingOptions();
										failureHandlingOptions.SetFailuresPreprocessor(new WarningExcetion());
										trans.SetFailureHandlingOptions(failureHandlingOptions);

										if(!connector.CreateVoidInstance())
										{
											trans.RollBack();
											sErr += "Can't create ATX_VoidCut instances.\n";
											continue;
										}
									}
									if (trans.Commit() != TransactionStatus.Committed)
									{
										sErr += "Can't create ATX_VoidCut instances.\n";
										continue;
									}
								}
								catch (Exception ex)
								{
									trans.RollBack();
									sErr += ex.Message + "\n";
								}
							}
						}
					}
				}
				transGroup.Assimilate();
				if (sErr != "")
					TaskDialog.Show("Error", sErr);
			}
		}

		public void SelectElements()
		{
			UIDocument uiDoc = Architexor.ConnectorTool.Application.GetUiApplication().ActiveUIDocument;
			Selection selection = uiDoc.Selection;
			try
			{
				GlobalHook.Subscribe();
				List<Reference> refers = (List<Reference>)selection.PickObjects(ObjectType.Element, "Please pick elements to connect.");
				GlobalHook.Unsubscribe();
				m_Elements.Clear();
				foreach (Reference eRef in refers)
				{
					if (eRef != null && eRef.ElementId != ElementId.InvalidElementId)
					{
						m_Elements.Add(uiDoc.Document.GetElement(eRef));
					}
				}
			}
			catch (Exception)
			{
				//Operation is canceled.
			}
		}

		public override bool ProcessRequest(ArchitexorRequestId reqId)
		{
			try
			{
				switch (reqId)
				{
					case ArchitexorRequestId.SelectElementsForTConnector:
						SelectElements();
						return false;
					case ArchitexorRequestId.ArrangeTConnectors:
						CreateConnector();
						return true;
					default:
						break;
				}
				return true;
			}
			catch (Exception ex)
			{
				TaskDialog.Show("Error", ex.ToString());
				return false;
			}
		}

		public void SelectElementsLinkWithConnector(TConElemInfo conElemInfo)
		{
			UIDocument uidoc = Architexor.ConnectorTool.Application.GetUiApplication().ActiveUIDocument;

			List<ElementId> linkElemCollection = new List<ElementId>
			{
				conElemInfo.PrimaryElement.Id,
				conElemInfo.SecondaryElement.Id
			};

			Selection sel = uidoc.Selection;
			sel.SetElementIds(linkElemCollection);
		}

		#region FamilySymbol
		/// <summary>
		/// Check if Void Cut family is already loaded
		/// </summary>
		/// <returns>True is Successful Loading for Void family</returns>
		private FamilySymbol CheckFamilySymbol(string sFamilyName)
		{
			Document doc = Architexor.ConnectorTool.Application.GetUiApplication().ActiveUIDocument.Document;

			FamilySymbol symbol = null;

			List<Family> fis = new List<Family>(
				new FilteredElementCollector(doc)
					.WhereElementIsNotElementType()
					.OfClass(typeof(Family))
					.Where(family => family.Name == sFamilyName)
					.ToList()
					.Cast<Family>()
					);
			if (fis.Count > 0)
			{
				foreach (ElementId eId in fis[0].GetFamilySymbolIds())
				{
					symbol = doc.GetElement(eId) as FamilySymbol;
				}
			}

			if (symbol != null)
			{
				return symbol;
			}
			else
			{
				Transaction trans = new Transaction(doc);
				trans.Start("Load Family");
				try
				{
					string url = Assembly.GetExecutingAssembly().Location;
					url = url.Substring(0, url.LastIndexOf("\\")) + "\\";

					Family cvf = null;
					bool bRet = symbol != null || doc.LoadFamily(url + sFamilyName + ".rfa", out cvf);
					if (bRet)
					{
						if (cvf != null)
						{
							foreach (ElementId eId in cvf.GetFamilySymbolIds())
							{
								symbol = doc.GetElement(eId) as FamilySymbol;
							}
						}
						trans.Commit();
						return symbol;
					}
					else
					{
						trans.RollBack();
						return null;
					}
				}
				catch (Exception)
				{
					trans.RollBack();
					return null;
				}
			}
		}

		#endregion
		#endregion
	}

	public class WarningExcetion : IFailuresPreprocessor
	{
		public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
		{
			IList<FailureMessageAccessor> faillist = new List<FailureMessageAccessor>();
			//Inside event handler, get all warnings
			faillist = failuresAccessor.GetFailureMessages();
			foreach (FailureMessageAccessor failure in faillist)
			{
				//Check FailureDefinitionIds against ones that you want to dismiss
				FailureDefinitionId failID = failure.GetFailureDefinitionId();
				//Prevent Revit from showing Unenclosed warnings.
				// Here, you can add another Failure if you want
				if(failID == BuiltInFailures.GeneralFailures.ErrorInSymbolFamilyResolved
					|| failID == BuiltInFailures.FamilyFailures.InstOutsideFaceBoundary)
				{
					failuresAccessor.DeleteWarning(failure);
				}
			}
			return FailureProcessingResult.Continue;
		}
	}
}
