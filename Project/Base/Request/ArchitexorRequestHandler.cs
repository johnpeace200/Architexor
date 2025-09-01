using Architexor.Base;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Diagnostics;
using System.Reflection;
using TaskDialog = Autodesk.Revit.UI.TaskDialog;

namespace Architexor.Request
{
	//	A class with methods to execute requests made by the SplitSettings Dialog
	public class ArchitexorRequestHandler : IExternalEventHandler
	{
		//	The value of the latest request made by the form 
		public ArchitexorRequest Request { get; } = new ArchitexorRequest();

		public ArchitexorRequestId RequestId { get; set; }

		//	Controller Class Instance
		public Controller Instance { get; set; }

		private ArchitexorApplication m_Application;

		//	A method to identify this External Event Handler
		public string GetName()
		{
			return "ArchitexorRequest";
		}

		public ArchitexorRequestHandler(ArchitexorApplication app, ArchitexorRequestId reqId, UIApplication uiapp)
		{
			m_Application = app;
			Initialize(reqId, uiapp);
		}

		protected void Initialize(ArchitexorRequestId reqId, UIApplication uiapp)
		{

			RequestId = reqId;
			switch (reqId)
			{
				case ArchitexorRequestId.SplitWall:
				case ArchitexorRequestId.SplitFloor:
					if (reqId == ArchitexorRequestId.SplitWall)
						Instance = (Controller)m_Application.GetClassInstance("WallSplitter");
					else if (reqId == ArchitexorRequestId.SplitFloor)
						Instance = (Controller)m_Application.GetClassInstance("FloorSplitter");
					break;
				case ArchitexorRequestId.AnalysisSplit:
					Instance = (Controller)m_Application.GetClassInstance("WallSplitter");
					break;
				case ArchitexorRequestId.HalfLap:
					Instance = (Controller)m_Application.GetClassInstance("HalfLap");
					break;
				case ArchitexorRequestId.JointBoard:
					Instance = (Controller)m_Application.GetClassInstance("JointBoard");
					break;
				case ArchitexorRequestId.PropertyMarker:
				case ArchitexorRequestId.PropertyMarkerUpdate:
					Instance = (Controller)m_Application.GetClassInstance("PropertyMarker");
					break;
				case ArchitexorRequestId.GlulamTConnector:
					Instance = (Controller)m_Application.GetClassInstance("ConnectorBuilder");
					break;
				case ArchitexorRequestId.AutoDimensioning:
					Instance = (Controller)m_Application.GetClassInstance("Dimensioning");
					break;
				case ArchitexorRequestId.AutoTagging:
					Instance = (Controller)m_Application.GetClassInstance("Tagging");
					break;
				default:
					break;
			}

			Instance.UIApp = uiapp;
		}

		//	The top method of the event handler.
		//	<remarks>
		//		This is called by Revit after the corresponding
		//		external event was raised (by the modeless form)
		//		and Revit reached the time at which it could call
		//		the event's handler (i.e. this object)
		//	</remarks>
		public void Execute(UIApplication uiapp)
		{
			bool bFinish = false;
			try
			{
				Document doc = uiapp.ActiveUIDocument.Document;

				ArchitexorRequestId reqId = Request.Take();
				if ((reqId & ArchitexorRequestId.Split) == ArchitexorRequestId.Split)
				{
					bFinish = Instance.ProcessRequest(reqId);
				}
				else if ((reqId & ArchitexorRequestId.ArrangeConnectors) == ArchitexorRequestId.ArrangeConnectors)
				{
					bFinish = Instance.ProcessRequest(reqId);
				}
				else if ((reqId & ArchitexorRequestId.PropertyMarker) == ArchitexorRequestId.PropertyMarker)
				{
					bFinish = Instance.ProcessRequest(reqId);
				}
				else if ((reqId & ArchitexorRequestId.GlulamTConnector) == ArchitexorRequestId.GlulamTConnector)
				{
					bFinish = Instance.ProcessRequest(reqId);
				}
				else if ((reqId & ArchitexorRequestId.AutoDimensioning) == ArchitexorRequestId.AutoDimensioning)
				{
					bFinish = Instance.ProcessRequest(reqId);
				}
				else if ((reqId & ArchitexorRequestId.AutoTagging) == ArchitexorRequestId.AutoTagging)
				{
					bFinish = Instance.ProcessRequest(reqId);
				}
			}
			catch (Exception ex)
			{
				TaskDialog.Show("Error", ex.Message);
				Debug.WriteLine(ex.StackTrace);
			}
			finally
			{
				m_Application.WakeRequestUp(RequestId, bFinish);
			}
		}
	}
}
