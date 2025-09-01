using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Text;
using System;
using Architexor.Request;

namespace Architexor.Commands
{
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
	[Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
	[Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
	public class GlulamTConnector : IExternalCommand
	{
		#region A Class For GLULAM T-CONNECTOR
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			StringBuilder sb = new StringBuilder();
			UIDocument uiDoc = commandData.Application.ActiveUIDocument;
			Document document = uiDoc.Document;
			Autodesk.Revit.ApplicationServices.Application app = commandData.Application.Application;
			FailureDefinitionRegistry failureReg = Autodesk.Revit.ApplicationServices.Application.GetFailureDefinitionRegistry();
			//{}
			Type _type = typeof(BuiltInFailures);
			Type[] _nested = _type.GetNestedTypes(System.Reflection.BindingFlags.Public);
			Dictionary<Guid, Type> _dict = new Dictionary<Guid, Type>();
			string _ClassName = string.Empty;
			foreach (Type nt in _nested)
			{
				try
				{
					_ClassName = nt.FullName.Replace('+', '.');
					//sb.AppendLine(string.Format("#### {0} ####", _ClassName));
					foreach (System.Reflection.PropertyInfo pInfo in nt.GetProperties())
					{
						System.Reflection.MethodInfo mInfo = pInfo.GetGetMethod();
						FailureDefinitionId res = mInfo.Invoke(nt, null) as FailureDefinitionId;
						if (res == null) continue;
						if (_dict.ContainsKey(res.Guid)) continue;
						_dict.Add(res.Guid, nt);
						if(res.Guid.ToString() == "7b5f515b-0b48-432a-979a-696a673982c7")
						{
							FailureDefinitionAccessor _acc = failureReg.FindFailureDefinition(res);
							if (_acc == null) continue;
							sb.AppendLine(string.Format("  * {0} <{1}> {2}", _acc.GetId().Guid, _acc.GetSeverity(), _ClassName));
							sb.AppendLine(string.Format("          {0}", _acc.GetDescriptionText()));
						}
					}
				}
				catch
				{
				}
			}

			_ClassName = "user-defined";
			sb.AppendLine(string.Format("#### {0} ####", _ClassName));
			foreach (FailureDefinitionAccessor _acc in failureReg.ListAllFailureDefinitions())
			{
				Type DefType = null;
				_dict.TryGetValue(_acc.GetId().Guid, out DefType);
				if (DefType != null) continue;  // failure already listed
				sb.AppendLine(string.Format("  * {0} <{1}> {2}", _acc.GetId().Guid, _acc.GetSeverity(), _ClassName));
				sb.AppendLine(string.Format("          {0}", _acc.GetDescriptionText()));
			}
			//TaskDialog.Show("res", sb.ToString());

			ConnectorTool.Application.thisApp.DoRequest(commandData.Application, ArchitexorRequestId.GlulamTConnector);
			return Result.Succeeded;
		}
		#endregion
	}
}
