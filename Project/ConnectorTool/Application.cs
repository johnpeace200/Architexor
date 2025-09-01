using Architexor.Base;
using Autodesk.Revit.UI;

namespace Architexor.ConnectorTool
{
	public class Application : ArchitexorApplication
	{
		public static Application thisApp = null;//	internal

		public override Result OnStartup(UIControlledApplication application)
		{
			thisApp = this;

			return base.OnStartup(application);
		}
	}
}
