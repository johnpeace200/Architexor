using Autodesk.Revit.DB;

namespace ConnectorTool.Base
{
	public abstract class TGeometry
	{
	
		#region Methods

		/// <summary>
		/// Initialize
		/// </summary>
		protected abstract void Initialize();

		/// <summary>
		/// Get the Type Parameters of Symbol
		/// </summary>
		/// <param name="symbol"></param>
		public abstract void GetParameters(FamilySymbol symbol);

		public abstract bool SetInstanceParameters(FamilyInstance instance);

		/// <summary>
		/// Set Parameters of Instance
		/// </summary>
		/// <param name="instance"></param>
		public abstract bool SetVoidInstanceParameters(FamilyInstance instance);

		#endregion
	}
}

