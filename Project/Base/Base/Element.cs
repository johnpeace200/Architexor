using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Architexor.Base
{
	public class CElement
	{
		public Element Element { get; set; }

		protected CElement()
		{

		}

		public CElement(Element e, UIDocument doc)
		{
			Element = e;
		}
	}
}
