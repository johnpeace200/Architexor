using Autodesk.Revit.DB;
using ConnectorTool.Object;

namespace ConnectorTool.Base
{
	public abstract class TObject
	{
		#region Fields
		protected TConnector m_Connector;
		#endregion

		#region Properties
		public FamilyInstance Instance { get; set; }
		#endregion

		#region Methods
		/// <summary>
		/// Initialize
		/// </summary>
		protected abstract void Initialize();

		/// <summary>
		/// Check whether the parameters are validated
		/// </summary>
		/// <returns></returns>
		protected abstract bool Validate();
		
		/// <summary>
		/// Create Void Instances for cutting element
		/// </summary>
		public abstract bool CreateInstance();
		#endregion
	}
}

