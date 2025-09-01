using Autodesk.Revit.DB;
using Architexor.Utils;

namespace ConnectorTool.Information
{
	/// <summary>
	/// The Connection FamilyType Parameters
	/// </summary>
	public class TConTypeParam
	{

		#region Properties

		/// <summary>
		/// Length of Back Plate
		/// </summary>
		public double Geometry_TConnector_Height { get; set; }

		/// <summary>
		/// Width of Back Plate
		/// </summary>
		public double Geometry_TConnector_Width { get; set; }

		/// <summary>
		/// Thickness of Back Plate
		/// </summary>
		public double Geometry_TConnector_BackPlate_Thk { get; set; }

		/// <summary>
		/// Width of Fin Plate
		/// </summary>
		public double Geometry_TConnector_Depth { get; set; }

		/// <summary>
		/// Thickness of Fin Plate
		/// </summary>
		public double Geometry_TConnector_FinPlate_Thk { get; set; }

		#endregion

		/// <summary>
		/// it is only used for object factory method
		/// </summary>
		public TConTypeParam()
		{
			Initialize();
		}

		/// <summary>
		/// Initialize Components
		/// </summary>
		private void Initialize()
		{

		}

		/// <summary>
		/// Get the GeometryParameters of Conn	ection.
		/// <paramref name="symbol"/> The Symbol of Void</param>
		/// </summary>
		public void GetParameters(FamilySymbol symbol)
		{
			// Get the Geometry Parameters of Symbol	
			ParameterSet paramList = symbol.Parameters;
			foreach (Parameter para in paramList)
			{
				if (para.StorageType == StorageType.Double)	
				{
					switch (para.Definition.Name)
					{
						case "Geometry_TConnector_Height":
						//case "Alumaxi_Length":
						//case "Alumidi_Length":
						//case "Alumini_Length":
							Geometry_TConnector_Height = Util.IUToMm(para.AsDouble());	
							break;
						case "Geometry_TConnector_Width":
						//case "Flange_Width":
							Geometry_TConnector_Width = Util.IUToMm(para.AsDouble());
							break;
						case "Geometry_TConnector_BackPlate_Thk":
						//case "Flange_Thickness":
							Geometry_TConnector_BackPlate_Thk = Util.IUToMm(para.AsDouble());
							break;
						case "Geometry_TConnector_Depth":
						//case "Core_Width":
							Geometry_TConnector_Depth = Util.IUToMm(para.AsDouble());
							break;
						case "Geometry_TConnector_FinPlate_Thk":
						//case "Core_Thickness":
						//case "Alumidi_Thickness":
						//case "Alumini_Thickness":
							Geometry_TConnector_FinPlate_Thk = Util.IUToMm(para.AsDouble());
							break;
						default:
							break;
					}
				}
			}
		}
	}
}
