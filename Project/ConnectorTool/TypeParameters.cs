using System.ComponentModel;
using Autodesk.Revit.DB;

namespace ConnectorTool
{
	/// <summary>
	/// for control PropertyGrid to show and modify parameters of Column and Beam
	/// </summary>
	public class TypeParameters
	{
		Parameter m_hDimension;        // parameter named h
		Parameter m_bDimension;        // parameter named b

		/// <summary>
		/// parameter h in parameter category Dimension
		/// </summary>
		[CategoryAttribute("Dimensions")]
		public double h
		{
			get
			{
				return m_hDimension.AsDouble();
			}
			set
			{
				m_hDimension.Set(value);
			}
		}

		/// <summary>
		/// parameter b in parameter category Dimension
		/// </summary>
		[CategoryAttribute("Dimensions")]
		public double b
		{
			get
			{
				return m_bDimension.AsDouble();
			}
			set
			{
				m_bDimension.Set(value);
			}
		}

		/// <summary>
		/// constructor without parameter is forbidden
		/// </summary>
		private TypeParameters()
		{
		}

		/// <summary>
		/// constructor used only for object factory
		/// </summary>
		/// <param name="symbol">FamilySymbol object has parameters</param>
		private TypeParameters(FamilySymbol symbol)
		{
			// iterate and initialize parameters
			foreach (Parameter para in symbol.Parameters)
			{
				if (para.Definition.Name == "h")
				{
					m_hDimension = para;
					continue;
				}
				if (para.Definition.Name == "b")
				{
					m_bDimension = para;
					continue;
				}
			}
		}

		/// <summary>
		/// object factory to create TypeParameters; 
		/// will return null if necessary Parameters can't be found
		/// </summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		public static TypeParameters CreateInstance(FamilySymbol symbol)
		{
			TypeParameters result = new TypeParameters(symbol);
			if (null == result.m_bDimension || null == result.m_hDimension)
			{
				return null;
			}
			return result;
		}
	}
}
