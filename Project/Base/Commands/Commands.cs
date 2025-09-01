using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Architexor.Forms;

namespace Architexor.Commands
{
	[Transaction(TransactionMode.Manual)]
	[Regeneration(RegenerationOption.Manual)]
	public class License : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			FrmRegistration frm = new FrmRegistration();
			frm.ShowDialog();

			return Result.Succeeded;
		}
	}
}