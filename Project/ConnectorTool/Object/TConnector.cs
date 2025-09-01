using Autodesk.Revit.DB.ExtensibleStorage;
using ConnectorTool.Information;
using ConnectorTool.Location;
using Architexor.Models.ConnectorTool;

namespace ConnectorTool.Object
{
	public class TConnector : Entity
	{

		#region Properties
		/// <summary>
		/// Connected Elements
		/// </summary>
		public TConInfo ConInfo { get; set; } = new TConInfo();

		/// <summary>
		/// The Location of Connector
		/// </summary>
		public TConLocation ConLocation { get; set; } = new TConLocation();

		/// <summary>
		/// The void object of connection for arranging connector
		/// </summary>
		public TConVoidObj ConVoidObj { get; set; }
		#endregion

		#region Constructor
		/// <summary>
		/// it is only used for object factory method
		/// </summary>
		protected TConnector() { }

		public TConnector(TConInfo elemInfo)
		{

			Initialize();

			ConInfo = elemInfo;
			ConLocation.GetLocationParameters(ConInfo.ConElemInfo);
			ConVoidObj = new TConVoidObj(this);
		}
		#endregion

		#region Methods

		/// <summary>
		/// Initialize the GeometryParameters of connection
		/// </summary>
		protected void Initialize()
		{
		}

		public bool CreateVoidInstance()
		{
			// Arrange the Connection Void Objects
			return ConVoidObj.CreateInstance();
		}
		#endregion
	}
}
